using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniWatch.Models;
using System.Data.Entity;

namespace UniWatch.Managers
{
    /// <summary>
    /// Provides central access to class related information
    /// </summary>
    public class ClassManager : IDisposable
    {
        private bool disposed = false;
        private AppDbContext _db;

        /// <summary>
        /// Creates a default ClassManager
        /// </summary>
        public ClassManager()
        {
            _db = new AppDbContext();
        }

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
        public async Task<Class> CreateClass(string name, int number, string section, Semester semester, int year, int teacherId)
        {
            var existing = await _db.Classes
                .Where(c => c.Name == name &&
                        c.Number == number &&
                        c.Section == section &&
                        c.Semester == semester &&
                        c.Teacher.Id == teacherId)
                .CountAsync();

            var teacher = await _db.Teachers.FindAsync(teacherId);

            if(existing > 0 || teacher == null)
                throw new InvalidOperationException("Error creating class");

            var newClass = new Class
            {
                Name = name,
                Number = number,
                Section = section,
                Semester = semester,
                Teacher = teacher
            };

            var added = _db.Classes.Add(newClass);
            await _db.SaveChangesAsync();
            return added;
        }

        /// <summary>
        /// Find a class by Id
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>The class with the given id or null if not found</returns>
        public async Task<Class> GetById(int classId)
        {
            return await _db.Classes.FindAsync(classId);
        }

        /// <summary>
        /// Returns all classes taught by the given teacher
        /// </summary>
        /// <param name="teacherId">The id of the teacher</param>
        /// <returns>List of classes taught by the teacher</returns>
        public async Task<IEnumerable<Class>> GetClassesForTeacher(int teacherId)
        {
            return await _db.Classes
                .Where(@class => @class.Teacher.Id == teacherId)
                .Include(c => c.Lectures)
                .ToListAsync();
        }

        /// <summary>
        /// Returns all classes for which the given student is enrolled
        /// </summary>
        /// <param name="studentId">The id of the student</param>
        /// <returns>List of classes that the student is enrolled in</returns>
        public async Task<IEnumerable<Class>> GetClassesForStudent(int studentId)
        {
            return await _db.Enrollments.Where(e => e.Student.Id == studentId)
                .Select(e => e.Class)
                .Include(c => c.Lectures)
                .ToListAsync();
        }

        /// <summary>
        /// Enrolls a student into a class
        /// </summary>
        /// <param name="classId">The id of the class to enroll the student into</param>
        /// <param name="studentId">The id of the student to enroll</param>
        /// <returns>The enrollment created</returns>
        public async Task<Enrollment> EnrollStudent(int classId, int studentId)
        {
            var existing = await _db.Enrollments.Where(e => e.Class.Id == classId && e.Student.Id == studentId)
                .Include(e => e.Class)
                .Include(e => e.Student)
                .FirstOrDefaultAsync();

            // Already enrolled
            if(existing == null)
                return null;

            var toAdd = new Enrollment
            {
                Class = existing.Class,
                EnrollDate = DateTime.Now,
                Student = existing.Student
            };

            _db.Enrollments.Add(toAdd);
            await _db.SaveChangesAsync();

            return toAdd;
        }

        /// <summary>
        /// Removes a student from a class
        /// </summary>
        /// <param name="classId">The id of the class to remove the student from</param>
        /// <param name="studentId">The id of the student to unenroll</param>
        /// <returns>The deleted enrollment</returns>
        public async Task<Enrollment> UnEnrollStudent(int classId, int studentId)
        {
            var enrollment = await _db.Enrollments
                .Where(e => e.Class.Id == classId && e.Student.Id == studentId)
                .FirstOrDefaultAsync();

            if(enrollment == null)
                return null;

            return _db.Enrollments.Remove(enrollment);

            // TODO: Delete all other student related data
        }

        /// <summary>
        /// Returns all students that are enrolled in a class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>List of all students enrolled in the class</returns>
        public async Task<IEnumerable<Student>> GetEnrolledStudents(int classId)
        {
            return await _db.Enrollments.Where(e => e.Class.Id == classId)
                .Select(e => e.Student)
                .ToListAsync();
        }

        /// <summary>
        /// Deletes a class with the given id and all other related information
        /// </summary>
        /// <param name="classId">Id of the class to delete</param>
        public async Task<Class> DeleteClass(int classId)
        {
            var existing = await _db.Classes.FindAsync(classId);

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
            if(disposed)
                return;

            if(disposing)
            {
                _db.Dispose();
            }

            disposed = true;
        }
    }
}