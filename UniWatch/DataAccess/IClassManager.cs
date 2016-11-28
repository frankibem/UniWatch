using System;
using System.Collections.Generic;
using UniWatch.Models;

namespace UniWatch.DataAccess
{
    public interface IClassManager : IDisposable
    {
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
        Class CreateClass(string name, int number, string section, Semester semester, int year, int teacherId);

        /// <summary>
        /// Find a class by Id
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>The class with the given id or null if not found</returns>
        Class GetById(int classId);

        /// <summary>
        /// Returns all classes taught by the given teacher
        /// </summary>
        /// <param name="teacherId">The id of the teacher</param>
        /// <returns>List of classes taught by the teacher</returns>
        IEnumerable<Class> GetClassesForTeacher(int teacherId);

        /// <summary>
        /// Returns all classes for which the given student is enrolled
        /// </summary>
        /// <param name="studentId">The id of the student</param>
        /// <returns>List of classes that the student is enrolled in</returns>
        IEnumerable<Class> GetClassesForStudent(int studentId);

        /// <summary>
        /// Enrolls a student into a class
        /// </summary>
        /// <param name="classId">The id of the class to enroll the student into</param>
        /// <param name="studentId">The id of the student to enroll</param>
        /// <returns>The enrollment created</returns>
        Enrollment EnrollStudent(int classId, int studentId);

        /// <summary>
        /// Removes a student from a class
        /// </summary>
        /// <param name="classId">The id of the class to remove the student from</param>
        /// <param name="studentId">The id of the student to unenroll</param>
        /// <returns>The deleted enrollment</returns>
        Enrollment UnEnrollStudent(int classId, int studentId);

        /// <summary>
        /// Returns all students that are enrolled in a class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        /// <returns>List of all students enrolled in the class</returns>
        IEnumerable<Student> GetEnrolledStudents(int classId);

        /// <summary>
        /// Deletes a class with the given id and all other related information
        /// </summary>
        /// <param name="classId">Id of the class to delete</param>
        Class DeleteClass(int classId);

        /// <summary>
        /// Train the recognizer for the given class
        /// </summary>
        /// <param name="classId">The id of the class</param>
        void TrainRecognizer(int classId);
    }
}