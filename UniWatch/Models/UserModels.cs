using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniWatch.Models
{
    /// <summary>
    /// Base class for users in the system
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        /// <summary>
        /// The user's first name
        /// </summary>
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// The user's last name
        /// </summary>
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string IdentityId { get; set; }
    }

    /// <summary>
    /// Represents a student user
    /// </summary>
    public class Student : User
    {
        /// <summary>
        /// The students facial profile
        /// </summary>
        public virtual FacialProfile Profile { get; set; }
    }

    /// <summary>
    /// Represents a teacher
    /// </summary>
    public class Teacher : User
    {
        /// <summary>
        /// Classes taught by this teacher
        /// </summary>
        public virtual ICollection<Class> Classes { get; set; }
    }
}