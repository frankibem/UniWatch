using System;

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
        public int Id { get; set; }

        /// <summary>
        /// The URL where the image is saved
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The name of the associated blob in storage
        /// </summary>
        public string BlobName { get; set; }

        /// <summary>
        /// The time when this image was uploaded
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}