using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Web.Configuration;
using System.Threading.Tasks;
using System.IO;
using UniWatch.Models;

namespace UniWatch.Managers
{
    /// <summary>
    /// Interacts with Azure Storage Manager to handle image storate related operations
    /// </summary>
    public class StorageManager
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;

        /// <summary>
        /// Creates a new storage manager using the default connection string (in Web.config)
        /// </summary>
        public StorageManager()
            : this(WebConfigurationManager.AppSettings["StorageConnectionString"])
        { }

        /// <summary>
        /// Creates a new storage manager with the given connection string
        /// </summary>
        /// <param name="connectionString">The connection string to initialize the storage manager with</param>
        public StorageManager(string connectionString)
        {
            // Retrieve storage account and create the blob client
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _blobClient = _storageAccount.CreateCloudBlobClient();

            // Consider storing externally (e.g. in AppSettings)
            _container = _blobClient.GetContainerReference("attendance");

            // Consider shared access signatures as opposed to public access
            var permission = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
            _container.SetPermissions(permission);

            // Consider - create container on startup (e.g. Global.asax)
            _container.CreateIfNotExists();
        }

        /// <summary>
        /// Saves the given image in storage
        /// </summary>
        /// <param name="image">The image to save</param>
        /// <returns>The uploaded image for the saved image</returns>
        public async Task<UploadedImage> SaveImage(Stream image)
        {
            if(image == null || image.Length == 0)
                return null;

            string blobName = Guid.NewGuid().ToString();
            CloudBlockBlob imageBlob = _container.GetBlockBlobReference(blobName);
            await imageBlob.UploadFromStreamAsync(image);

            return new UploadedImage
            {
                BlobName = blobName,
                CreationTime = DateTime.Now,
                Url = imageBlob.Uri.AbsolutePath
            };
        }

        /// <summary>
        /// Saves the given images in storage
        /// </summary>
        /// <param name="images">The images to save to storage</param>
        /// <returns>A list of uploaded imagees for each image saved</returns>
        public async Task<List<UploadedImage>> SaveImages(List<Stream> images)
        {
            List<UploadedImage> result = new List<UploadedImage>();
            if(images == null)
                return result;

            foreach(var image in images)
            {
                result.Add(await SaveImage(image));
            }

            return result;
        }

        /// <summary>
        /// Deletes a stored image
        /// </summary>
        /// <param name="image">The image to delete</param>
        public async Task DeleteImage(UploadedImage image)
        {
            if(image == null)
                return;

            CloudBlockBlob imageBlob = _container.GetBlockBlobReference(image.BlobName);
            await imageBlob.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Downloads the given image to a stream
        /// </summary>
        /// <param name="image">The image to download</param>
        /// <returns>A memory stream with the downloaded image</returns>
        public async Task<MemoryStream> DownloadImage(UploadedImage image)
        {
            if(image == null)
                return null;

            var result = new MemoryStream();
            CloudBlockBlob imageBlob = _container.GetBlockBlobReference(image.BlobName);
            await imageBlob.DownloadToStreamAsync(result);

            return result;
        }
    }
}