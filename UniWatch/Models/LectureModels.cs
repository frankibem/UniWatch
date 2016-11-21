using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniWatch.Models
{
    /// <summary>
    /// Logical unit to represent a single session during which attendance is taken
    /// </summary>
    public class Lecture
    {
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The time and date of this recording
        /// </summary>
        [Required]
        public DateTime RecordDate { get; set; }

        /// <summary>
        /// The class that this lecture was recorded for
        /// </summary>
        [Required]
        public virtual Class Class { get; set; }

        /// <summary>
        /// The images that were recorded for this lecture
        /// </summary>
        public virtual ICollection<UploadedImage> Images { get; set; }

        /// <summary>
        /// The list of attendance information for this lecture
        /// </summary>
        public virtual ICollection<StudentAttendance> Attendance { get; set; }

        public Lecture()
        {
            Images = new List<UploadedImage>();
            Attendance = new List<StudentAttendance>();
        }
    }

    /// <summary>
    /// StudentAttendance stores information about a students absence or presence for a lecture
    /// </summary>
    public class StudentAttendance
    {
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// True if the student was recognized during attendance. False otherwise
        /// </summary>
        [Required]
        public bool Present { get; set; }

        /// <summary>
        /// The student related to this record
        /// </summary>
        [Required]
        public virtual Student Student { get; set; }

        /// <summary>
        /// The lecture that this record belongs to
        /// </summary>
        [Required]
        public virtual Lecture Lecture { get; set; }
    }
}