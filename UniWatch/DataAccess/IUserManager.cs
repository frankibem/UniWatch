using System;
using System.Collections.Generic;
using System.IO;
using UniWatch.Models;

namespace UniWatch.DataAccess
{
    public interface IUserManager : IDisposable
    {
        /// <summary>
        /// Creates a new student
        /// </summary>
        /// <param name="firstName">The student's first name</param>
        /// <param name="lastName">The student's last name</param>
        /// <param name="user">The underlying Identity user object</param>
        /// <returns>The created student</returns>
        Student CreateStudent(string firstName, string lastName, ApplicationUser user);

        /// <summary>
        /// Creates a new teacher
        /// </summary>
        /// <param name="firstName">The teacher's first name</param>
        /// <param name="lastName">The teacher's last name</param>
        /// <param name="user">The underlying Identity user object</param>
        /// <returns>The created teacher</returns>
        Teacher CreateTeacher(string firstName, string lastName, ApplicationUser user);

        /// <summary>
        /// Finds a student with the given id
        /// </summary>
        /// <param name="studentId">The id of the student to search for</param>
        /// <returns>The student with the given id. Returns null if no such student exists</returns>
        Student GetStudentById(int studentId);

        /// <summary>
        /// Finds a teacher with the given id
        /// </summary>
        /// <param name="teacherId">The id of the teacher to search for</param>
        /// <returns>The teacher with the given id. Returns null if no such teacher exists</returns>
        Teacher GetTeacherById(int teacherId);

        /// <summary>
        /// Returns the logged in user
        /// </summary>
        User GetUser();

        /// <summary>
        /// Returns a list of all students whose id matches the given id
        /// </summary>
        /// <param name="id">The search string</param>
        IEnumerable<Student> SearchStudent(string id);

        /// <summary>
        /// Sets the images that make up the facial profile for a student
        /// </summary>
        /// <param name="studentId">The id of the student</param>
        /// <param name="images">The images that make up the profile</param>
        /// <returns>The uploaded images</returns>
        IEnumerable<UploadedImage> SetStudentProfile(int studentId, IEnumerable<Stream> images);
    }
}