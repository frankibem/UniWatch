using System.Collections.Generic;
using UniWatch.Models;

namespace UniWatch.ViewModels
{
    /// <summary>
    /// View model for displaying students enrolled in a class
    /// </summary>
    public class EnrollIndexViewModel
    {
        public Class Class { get; set; }
        public IEnumerable<Student> EnrolledStudents { get; set; }
    }

    /// <summary>
    /// View model for enrolling a student into a class
    /// </summary>
    public class EnrollStudentViewModel
    {
        public Class Class { get; set; }
        public List<StudentFound> StudentsFound { get; set; }

        public EnrollStudentViewModel()
        {
            StudentsFound = new List<StudentFound>();
        }
    }

    /// <summary>
    /// Holds a student and status of enrollment in a class
    /// </summary>
    public class StudentFound
    {
        public Student Student;
        public bool Enrolled;
    }

    /// <summary>
    /// View model for unenrolling a student from a class
    /// </summary>
    public class UnEnrollViewModel
    {
        public Student Student { get; set; }
        public Class Class { get; set; }
    }
}