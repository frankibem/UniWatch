using System.Collections.Generic;
using UniWatch.Models;

namespace UniWatch.ViewModels
{
    /// <summary>
    /// View model used for updating attendance information for a lecture
    /// </summary>
    public class UpdateLectureViewModel
    {
        /// <summary>
        /// The class that the lecture belongs to
        /// </summary>
        public Class Class { get; set; }

        /// <summary>
        /// The Lecture that we're updating
        /// </summary>
        public Lecture Lecture { get; set; }

        /// <summary>
        /// A list of attendance items for this lecture
        /// </summary>
        public List<UpdateLectureItem> LectureItems { get; set; }

        public UpdateLectureViewModel()
        {
            LectureItems = new List<UpdateLectureItem>();
        }
    }

    /// <summary>
    /// View model used to generate attendance report for teachers
    /// </summary>
    public class TeacherReportViewModel
    {
        /// <summary>
        /// The id of the class in the report
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// The name of the class in the report
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// A list of all lectures for the class
        /// </summary>
        public List<Lecture> Lectures { get; set; }

        /// <summary>
        /// A list of attendance information for each student
        /// </summary>
        public List<AttendanceStatus> Statuses { get; set; }

        public TeacherReportViewModel()
        {
            Lectures = new List<Lecture>();
            Statuses = new List<AttendanceStatus>();
        }
    }

    /// <summary>
    /// Holds attendance information for a student for all lectures in a class
    /// </summary>
    public class AttendanceStatus
    {
        /// <summary>
        /// The student related to this object
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// A map from lecture id to attendance status (present or not)
        /// </summary>
        public Dictionary<int, bool> Attendance { get; set; }

        public AttendanceStatus()
        {
            Attendance = new Dictionary<int, bool>();
        }
    }
}