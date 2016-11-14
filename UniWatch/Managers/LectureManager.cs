using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using UniWatch.Models;

namespace UniWatch.Managers
{
    /// <summary>
    /// Provides central access to Lecture related information
    /// </summary>
    public class LectureManager : IDisposable
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
        public async Task<Lecture> GetLecture(int lectureId)
        {
            return await _db.Lectures.FindAsync(lectureId);
        }

        /// <summary>
        /// Returns a list of all lectures for the given class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>A list of all lectures for the class</returns>
        public async Task<IEnumerable<Lecture>> GetTeacherReport(int classId)
        {
            return await _db.Lectures.
                Where(lecture => lecture.Class.Id == classId)
                .ToListAsync();
        }

        /// <summary>
        /// Returns a list of attendance for a student in a class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <param name="studentId">The id of the student</param>
        /// <returns>The list of attendance for the student in the class</returns>
        public async Task<IEnumerable<StudentAttendance>> GetStudentReport(int classId, int studentId)
        {
            return await _db.Attendance
                .Where(a => a.Lecture.Class.Id == classId && a.Student.Id == studentId)
                .ToListAsync();
        }

        /// <summary>
        /// Updates and saves the given lecture
        /// </summary>
        /// <param name="lecture">The lecture to update</param>
        /// <returns>The updated lecture</returns>
        public async Task<Lecture> UpdateLecture(Lecture lecture)
        {
            var lect = await _db.Lectures.FindAsync(lecture.Id);

            // Doesn't exist
            if(lecture == null)
                throw new InvalidOperationException("");

            _db.Entry(lecture).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return lecture;
        }

        /// <summary>
        /// Deletes the matched record and all related information
        /// </summary>
        /// <param name="lectureId">The id of the lecture to delete</param>
        /// <returns>The deleted lecture</returns>
        public async Task<Lecture> DeleteLecture(int lectureId)
        {
            var existing = await _db.Lectures.FindAsync(lectureId);

            if(existing == null)
                throw new InvalidOperationException("Error deleting class.");

            return _db.Lectures.Remove(existing);

            // TODO: Delete all other lecture related information (Images (and blobs), Attendance)
        }

        ///// <summary>
        ///// Creates a new lecture for a class using the given images
        ///// </summary>
        ///// <param name="classId">The id of the class to which the lecture belongs</param>
        ///// <param name="images">A list of images to create the recording from</param>
        ///// <returns></returns>
        //public async Task RecordLecture(int classId, List<Stream> images)
        //{

        //}

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