using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniWatch.Models
{
    /// <summary>
    /// The facial profile holds images used to recognize a student
    /// </summary>
    public class FacialProfile
    {
        /// <summary>
        /// The ID of this facial profile
        /// </summary>
        [Key, ForeignKey("Student")]
        public int Id { get; set; }

        /// <summary>
        /// The id of the student
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// The student that this profile belongs to
        /// </summary>
        public virtual Student Student { get; set; }

        /// <summary>
        /// Collection of facial images for this student
        /// </summary>
        public virtual ICollection<UploadedImage> Images { get; set; }

        public FacialProfile()
        {
            Images = new List<UploadedImage>();
        }
    }
}