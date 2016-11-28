using System;
using System.Collections.Generic;
using System.Linq;
using UniWatch.Models;
using System.Data.Entity;
using UniWatch.Services;
using System.Threading.Tasks;

namespace UniWatch.DataAccess
{
    /// <summary>
    /// Provides central access to class related information
    /// </summary>
    public class ClassManager : IClassManager
    {
        private bool _disposed = false;
        private AppDbContext _db;

        /// <summary>
        /// Creates a default ClassManager
        /// </summary>
        public ClassManager() : this(new AppDbContext())
        { }

        /// <summary>
        /// Creates a ClassManager with the given context
        /// </summary>
        /// <param name="context">The context to create the manager with</param>
        public ClassManager(AppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Creates a new class
        /// </summary>
        /// <param name="name">The name/title of the class</param>
        /// <param name="number">The course number</param>
        /// <param name="section">The section</param>
        /// <param name="semester">The semester e.g. Fall 2016</param>
        /// <param name="teacher">The teacher for this class</param>
        /// <returns>The created class</returns>
        /// <remarks>Throws InvalidOperationException if class already exists with the given details</remarks>
        public Class CreateClass(string name, int number, string section, Semester semester, int year, int teacherId)
        {
            var existing = _db.Classes
                .Count(c => c.Name == name &&
                        c.Number == number &&
                        c.Section == section &&
                        c.Semester == semester &&
                        c.Teacher.Id == teacherId);

            var teacher = _db.Teachers.Find(teacherId);

            if(existing > 0 || teacher == null)
                throw new InvalidOperationException("Error creating class");

            var newClass = new Class
            {
                Name = name,
                Number = number,
                Section = section,
                Semester = semester,
                Year = year,
                TrainingStatus = TrainingStatus.UnTrained,
                Teacher = teacher
            };

            var added = _db.Classes.Add(newClass);
            _db.SaveChanges();

            // Create the PersonGroup for this class
            var faceClient = RecognitionService.GetFaceClient();
            Task.Run(() => faceClient.CreatePersonGroupAsync(added.Id.ToString(), added.Name)).Wait();

            return added;
        }

        /// <summary>
        /// Find a class by Id
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>The class with the given id or null if not found</returns>
        public Class GetById(int classId)
        {
            return _db.Classes.Where(c => c.Id == classId)
                .Include(c => c.Teacher)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns all classes taught by the given teacher
        /// </summary>
        /// <param name="teacherId">The id of the teacher</param>
        /// <returns>List of classes taught by the teacher</returns>
        public IEnumerable<Class> GetClassesForTeacher(int teacherId)
        {
            return _db.Classes
                .Where(@class => @class.Teacher.Id == teacherId)
                .Include(c => c.Lectures);
        }

        /// <summary>
        /// Returns all classes for which the given student is enrolled
        /// </summary>
        /// <param name="studentId">The id of the student</param>
        /// <returns>List of classes that the student is enrolled in</returns>
        public IEnumerable<Class> GetClassesForStudent(int studentId)
        {
            return _db.Enrollments.Where(e => e.Student.Id == studentId)
                .Select(e => e.Class)
                .Include(c => c.Lectures)
                .ToList();
        }

        /// <summary>
        /// Enrolls a student into a class
        /// </summary>
        /// <param name="classId">The id of the class to enroll the student into</param>
        /// <param name="studentId">The id of the student to enroll</param>
        /// <returns>The enrollment created</returns>
        public Enrollment EnrollStudent(int classId, int studentId)
        {
            var existing = _db.Enrollments
                .FirstOrDefault(e => e.Class.Id == classId && e.Student.Id == studentId);

            var @class = _db.Classes.Find(classId);
            var student = _db.Students.Find(studentId);

            // Already enrolled
            if(existing != null)
                throw new InvalidOperationException("Error enrolling student");

            var enrollment = new Enrollment
            {
                Class = @class,
                EnrollDate = DateTime.Now,
                Student = student
            };
            _db.Enrollments.Add(enrollment);

            // Add the faces
            if(!student.Profile.Images.Any())
            {
                throw new InvalidOperationException("Profile must be added before enrollment.");
            }

            // Create the Person for this student
            var faceClient = RecognitionService.GetFaceClient();
            var person = Task.Run(() => faceClient.CreatePersonAsync(classId.ToString(), student.FirstName)).Result;

            foreach(var image in student.Profile.Images)
            {
                Task.Run(() => faceClient.AddPersonFaceAsync(@class.Id.ToString(), person.PersonId, image.Url)).Wait();
            }

            // Update training status for class
            enrollment.PersonId = person.PersonId;
            enrollment.Class.TrainingStatus = TrainingStatus.UnTrained;
            _db.SaveChanges();

            return enrollment;
        }

        /// <summary>
        /// Removes a student from a class
        /// </summary>
        /// <param name="classId">The id of the class to remove the student from</param>
        /// <param name="studentId">The id of the student to unenroll</param>
        /// <returns>The deleted enrollment</returns>
        public Enrollment UnEnrollStudent(int classId, int studentId)
        {
            var enrollment = _db.Enrollments
                .Include(e => e.Class)
                .FirstOrDefault(e => e.Class.Id == classId && e.Student.Id == studentId);

            if(enrollment == null)
                throw new InvalidOperationException("Error unenrolling student");

            // Delete the Person object from the PersonGroup
            var faceClient = RecognitionService.GetFaceClient();
            Task.Run(() => faceClient.DeletePersonAsync(classId.ToString(), enrollment.PersonId));

            // Remove all attendance and enrollment information
            var attendance = _db.Attendance.Where(a => a.Student.Id == studentId);
            _db.Attendance.RemoveRange(attendance);
            enrollment.Class.TrainingStatus = TrainingStatus.UnTrained;

            // Update training status for class
            _db.Enrollments.Remove(enrollment);
            _db.SaveChanges();

            return enrollment;
        }

        /// <summary>
        /// Returns all students that are enrolled in a class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>List of all students enrolled in the class</returns>
        public IEnumerable<Student> GetEnrolledStudents(int classId)
        {
            return _db.Enrollments.Where(e => e.Class.Id == classId)
                .Select(e => e.Student);
        }

        /// <summary>
        /// Deletes a class with the given id and all other related information
        /// </summary>
        /// <param name="classId">Id of the class to delete</param>
        public Class DeleteClass(int classId)
        {
            var @class = _db.Classes.Find(classId);

            if(@class == null)
                throw new InvalidOperationException("Error deleting class.");

            // Delete all lectures
            var lectureManager = new LectureManager(_db);
            var lectures = new List<Lecture>(@class.Lectures);
            foreach(var lecture in lectures)
            {
                lectureManager.Delete(lecture.Id);
            }

            // Delete all enrollments
            _db.Enrollments.RemoveRange(@class.Enrollment);

            // Delete cognitive data
            var faceClient = RecognitionService.GetFaceClient();
            Task.Run(() => faceClient.DeletePersonGroupAsync(@class.Id.ToString())).Wait();          

            _db.Classes.Remove(@class);
            _db.SaveChanges();

            return @class;
        }

        /// <summary>
        /// Train the recognizer for the given class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        public void TrainRecognizer(int classId)
        {
            var @class = GetById(classId);

            if(@class == null)
                throw new InvalidOperationException("Error training recognizer");

            if(@class.TrainingStatus != TrainingStatus.UnTrained)
                return;

            @class.TrainingStatus = TrainingStatus.Training;
            _db.SaveChanges();

            var recognitionService = new RecognitionService();
            Task.Run(() => recognitionService.TrainRecognizer(classId.ToString())).Wait();

            @class.TrainingStatus = TrainingStatus.Trained;
            _db.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_disposed)
                return;

            if(disposing)
            {
                _db.Dispose();
            }

            _disposed = true;
        }
    }
}