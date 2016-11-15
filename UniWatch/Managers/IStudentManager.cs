using System;
using UniWatch.Models;

namespace UniWatch.Managers
{
    public interface IStudentManager : IDisposable
    {
        /// <summary>
        /// Finds a student with the given id
        /// </summary>
        /// <param name="studentId">The id of the student to search for</param>
        /// <returns>The student with the given id. Returns null if no such student exists</returns>
        Student GetById(int studentId);
    }
}