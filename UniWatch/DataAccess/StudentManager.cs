using System;
using UniWatch.Models;

namespace UniWatch.DataAccess
{
    public class StudentManager : IStudentManager
    {
        private bool disposed = false;
        private AppDbContext _db;

        /// <summary>
        /// Creates a default StudentManager
        /// </summary>
        public StudentManager()
        {
            _db = new AppDbContext();
        }

        /// <summary>
        /// Creates a StudentManager with the given context
        /// </summary>
        /// <param name="context">The context to create the manager with</param>
        public StudentManager(AppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Finds a student with the given id
        /// </summary>
        /// <param name="studentId">The id of the student to search for</param>
        /// <returns>The student with the given id. Returns null if no such student exists</returns>
        public Student GetById(int studentId)
        {
            return _db.Students.Find(studentId);
        }

        public Teacher GetTeacher(int teacherId)
        {
            return _db.Teachers.Find(teacherId);
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