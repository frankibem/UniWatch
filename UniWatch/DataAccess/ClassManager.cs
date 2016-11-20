using System;
using System.Collections.Generic;
using System.Linq;
using UniWatch.Models;
using System.Data.Entity;
using UniWatch.Services;

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
                TrainingStatus = TrainingStatus.UnTrained,
                Teacher = teacher
            };

            var added = _db.Classes.Add(newClass);
            _db.SaveChanges();

            // Create the PersonGroup for this class
            var faceClient = RecognitionService.GetFaceClient();
            faceClient.CreatePersonGroupAsync(added.Id.ToString(), added.Name).Wait();

            return added;
        }

        /// <summary>
        /// Find a class by Id
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>The class with the given id or null if not found</returns>
        public Class GetById(int classId)
        {
            return _db.Classes.Find(classId);
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
            var existing = _db.Enrollments.Where(e => e.Class.Id == classId && e.Student.Id == studentId)
                .Include(e => e.Class)
                .Include(e => e.Student)
                .FirstOrDefault();

            // Already enrolled
            if(existing == null)
                throw new InvalidOperationException("Error enrolling student");

            var toAdd = new Enrollment
            {
                Class = existing.Class,
                EnrollDate = DateTime.Now,
                Student = existing.Student
            };

            _db.Enrollments.Add(toAdd);
            _db.SaveChanges();

            return toAdd;
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
                .Where(e => e.Class.Id == classId && e.Student.Id == studentId)
                .FirstOrDefault();

            if(enrollment == null)
                throw new InvalidOperationException("Error unenrolling student");

            return _db.Enrollments.Remove(enrollment);

            // TODO: Delete all other student related data
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
            var existing = _db.Classes.Find(classId);

            if(existing == null)
                throw new InvalidOperationException("Error deleting class.");

            return _db.Classes.Remove(existing);

            // TODO: Delete all other class related data (Lectures, Enrollments)
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