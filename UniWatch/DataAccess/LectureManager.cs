using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RestSharp.Extensions;
using UniWatch.Models;

namespace UniWatch.DataAccess
{
    /// <summary>
    /// Provides central access to Lecture related information
    /// </summary>
    public class LectureManager : ILectureManager
    {
        private bool disposed = false;
        private AppDbContext _db;

        /// <summary>
        /// Creates a default LectureManager
        /// </summary>
        public LectureManager()
        {
            _db = new AppDbContext();
        }

        /// <summary>
        /// Creates a LectureManager with the given context
        /// </summary>
        /// <param name="context">The context to create the manager with</param>
        public LectureManager(AppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Returns the lecture with the given id
        /// </summary>
        /// <param name="lectureId">The id of the lecture</param>
        /// <returns>The lecture with the given id</returns>
        public Lecture Get(int lectureId)
        {
            return _db.Lectures.Find(lectureId);
        }

        /// <summary>
        /// Returns a list of all lectures for the given class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>A list of all lectures for the class</returns>
        public IEnumerable<Lecture> GetTeacherReport(int classId)
        {
            return _db.Lectures.
                Where(lecture => lecture.Class.Id == classId)
                .Include(lecture => lecture.Attendance);
        }

        /// <summary>
        /// Returns a list of attendance for a student in a class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <param name="studentId">The id of the student</param>
        /// <returns>The list of attendance for the student in the class</returns>
        public IEnumerable<StudentAttendance> GetStudentReport(int classId, int studentId)
        {
            return _db.Attendance
                .Where(a => a.Lecture.Class.Id == classId && a.Student.Id == studentId);
        }

        /// <summary>
        /// Updates and saves the given lecture
        /// </summary>
        /// <param name="lecture">The lecture to update</param>
        /// <returns>The updated lecture</returns>
        public Lecture Update(Lecture lecture)
        {
            var existing = _db.Lectures.Find(lecture.Id);

            // Doesn't exist
            if(existing == null)
                throw new InvalidOperationException("Error updating lecture");

            _db.Entry(lecture).State = EntityState.Modified;
            _db.SaveChanges();

            return lecture;
        }

        /// <summary>
        /// Deletes the matched record and all related information
        /// </summary>
        /// <param name="lectureId">The id of the lecture to delete</param>
        /// <returns>The deleted lecture</returns>
        public Lecture Delete(int lectureId)
        {
            var existing = _db.Lectures.Find(lectureId);

            if(existing == null)
                throw new InvalidOperationException("Error deleting class.");

            return _db.Lectures.Remove(existing);

            // TODO: Delete all other lecture related information (Images (and blobs), Attendance)
        }

        /// <summary>
        /// Create and save a new lecture
        /// </summary>
        /// <param name="lecture">The lecture to create</param>
        /// <returns>The created lecture</returns>
        public Lecture Create(Lecture lecture)
        {
            // TODO: Implemente Create Lecture
            return null;
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