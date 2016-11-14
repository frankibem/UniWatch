using System.Collections.Generic;

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
        public string Id { get; set; }

        /// <summary>
        /// True if the recognizer has been trained to recognize the images in this profile.
        /// False otherwise
        /// </summary>
        public bool RecognizerTrained { get; set; }

        /// <summary>
        /// The student that this profile belongs to
        /// </summary>
        public virtual Student Student { get; set; }

        /// <summary>
        /// Collection of facial images for this student
        /// </summary>
        public virtual ICollection<UploadedImage> Images { get; set; }
    }
}