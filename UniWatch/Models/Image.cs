using System;
using System.ComponentModel.DataAnnotations;

namespace UniWatch.Models
{
    /// <summary>
    /// Image stores data related to an uploaded image
    /// </summary>
    public class UploadedImage
    {
        /// <summary>
        /// The Id of this image
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The URL where the image is saved
        /// </summary>
        [Required]
        public string Url { get; set; }

        /// <summary>
        /// The name of the associated blob in storage
        /// </summary>
        [Required]
        public string BlobName { get; set; }

        /// <summary>
        /// The time when this image was uploaded
        /// </summary>
        [Required]
        public DateTime CreationTime { get; set; }
    }
}