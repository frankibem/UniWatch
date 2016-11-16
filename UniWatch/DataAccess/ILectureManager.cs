using System;
using System.Collections.Generic;
using UniWatch.Models;

namespace UniWatch.DataAccess
{
    public interface ILectureManager : IDisposable
    {
        /// <summary>
        /// Create and save a new lecture
        /// </summary>
        /// <param name="lecture">The lecture to create</param>
        /// <returns>The created lecture</returns>
        Lecture Create(Lecture lecture);

        /// <summary>
        /// Returns the lecture with the given id
        /// </summary>
        /// <param name="lectureId">The id of the lecture</param>
        /// <returns>The lecture with the given id</returns>
        Lecture Get(int lectureId);

        /// <summary>
        /// Returns a list of all lectures for the given class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>A list of all lectures for the class</returns>
        IEnumerable<Lecture> GetTeacherReport(int classId);

        /// <summary>
        /// Returns a list of attendance for a student in a class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <param name="studentId">The id of the student</param>
        /// <returns>The list of attendance for the student in the class</returns>
        IEnumerable<StudentAttendance> GetStudentReport(int classId, int studentId);

        /// <summary>
        /// Updates and saves the given lecture
        /// </summary>
        /// <param name="lecture">The lecture to update</param>
        /// <returns>The updated lecture</returns>
        Lecture Update(Lecture lecture);

        /// <summary>
        /// Deletes the matched record and all related information
        /// </summary>
        /// <param name="lectureId">The id of the lecture to delete</param>
        /// <returns>The deleted lecture</returns>
        Lecture Delete(int lectureId);
    }
}