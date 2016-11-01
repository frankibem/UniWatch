namespace UniWatch.Models
{
    /// <summary>
    /// Image stores data related to an uploaded image
    /// </summary>
    public class Image
    {
        /// <summary>
        /// The Id of this image
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The URL where the image is saved
        /// </summary>
        public string URL { get; set; }
    }
}