﻿using System;
using System.Collections.Generic;

namespace UniWatch.Models
{
    /// <summary>
    /// Represents a Class that is taught by a teacher
    /// </summary>
    public class Class
    {
        public int Id { get; set; }

        /// <summary>
        /// The name of this course
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The course number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// The section for this class
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// The semester that this class is being taught
        /// </summary>
        public string Semester { get; set; }

        /// <summary>
        /// The teacher in charge of this class
        /// </summary>
        public virtual ApplicationUser Teacher { get; set; }

        /// <summary>
        /// Enrollment information for this class
        /// </summary>
        public virtual ICollection<Enrollment> Enrollment { get; set; }

        /// <summary>
        /// The recorded lectures for this class
        /// </summary>
        public virtual ICollection<Lecture> Lectures { get; set; }
    }

    /// <summary>
    /// Enrollment tracks the students that are enrolled in a class
    /// </summary>
    public class Enrollment
    {
        public int Id { get; set; }

        /// <summary>
        /// The date on which this student was enrolled in the class
        /// </summary>
        public DateTime EnrollDate { get; set; }

        /// <summary>
        /// The class related to this enrollment
        /// </summary>
        public virtual Class Class { get; set; }

        /// <summary>
        /// The student related to this enrollment
        /// </summary>
        public virtual Student Student { get; set; }
    }
}