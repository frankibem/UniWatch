﻿using System.ComponentModel.DataAnnotations;

namespace UniWatch.Models
{
    /// <summary>
    /// Base class for users in the system
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// The Identity information related to this user (e.g. email, phone, ...)
        /// </summary>
        public ApplicationUser Identity { get; set; }
    }

    /// <summary>
    /// Represents a student user
    /// </summary>
    public class Student : User
    {
        public int Id { get; set; }

        /// <summary>
        /// The students facial profile
        /// </summary>
        public virtual FacialProfile Profile { get; set; }
    }
}