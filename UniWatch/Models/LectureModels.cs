using System;
using System.Collections.Generic;

namespace UniWatch.Models
{
    /// <summary>
    /// Logical unit to represent a single session during which attendance is taken
    /// </summary>
    public class Lecture
    {
        public int Id { get; set; }

        /// <summary>
        /// The time and date of this recording
        /// </summary>
        public DateTime RecordDate { get; set; }

        /// <summary>
        /// The class that this lecture was recorded for
        /// </summary>
        public virtual Class Class { get; set; }

        /// <summary>
        /// The images that were recorded for this lecture
        /// </summary>
        public virtual ICollection<Image> Images { get; set; }

        /// <summary>
        /// The list of attendance information for this lecture
        /// </summary>
        public virtual ICollection<StudentAttendance> Attendance { get; set; }
    }

    /// <summary>
    /// StudentAttendance stores information about a students absence or presence for a lecture
    /// </summary>
    public class StudentAttendance
    {
        public int Id { get; set; }

        /// <summary>
        /// True if the student was recognized during attendance. False otherwise
        /// </summary>
        public bool Present { get; set; }

        /// <summary>
        /// The student related to this record
        /// </summary>
        public virtual Student Student { get; set; }

        /// <summary>
        /// The lecture that this record belongs to
        /// </summary>
        public virtual Lecture Lecture { get; set; }
    }
}