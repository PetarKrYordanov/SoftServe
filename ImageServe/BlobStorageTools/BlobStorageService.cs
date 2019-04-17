namespace BlobStorageTools
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using BlobStorageTools.Contracts;
    public class BlobStorageService : IBlobStorageService
    {
        private const string BLOB_CONTAINER_IMAGES = "images";
        private const string BLOB_CONTAINER_THUMBS = "thumbs";

        

        private CloudBlobContainer _cloudBlobContainer;
        
        public BlobStorageService(CloudBlobContainer blobContainer)
        {
            _cloudBlobContainer = blobContainer;
        }

        public BlobStorageService(CloudStorageAccount storageAccount)
        {
            _cloudBlobContainer = GetBlobStorage(storageAccount);
        }
        
        public async Task UploadFromFileAsync(byte[] file, string name, string extension)
        {
            
            
            var blob = GetBlob(name);
            await blob.UploadFromByteArrayAsync(file, 0, file.Length);
            
        }

        

        

        private CloudBlobContainer GetBlobStorage(CloudStorageAccount storageAccount)
        {
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            
            var imagesContainer = cloudBlobClient.GetContainerReference(BLOB_CONTAINER_IMAGES);
            var thumbsContainer = cloudBlobClient.GetContainerReference(BLOB_CONTAINER_THUMBS);

            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            imagesContainer.SetPermissionsAsync(permissions).GetAwaiter().GetResult();
            thumbsContainer.SetPermissionsAsync(permissions).GetAwaiter().GetResult();
            
            return imagesContainer;
        }

        private CloudBlockBlob GetBlob(string name)
        {
            //saves a space(blob) in the blob container then uploads the file inside
            return _cloudBlobContainer.GetBlockBlobReference(name);
        }

        private async Task DeleteBlob(CloudBlockBlob blob)
        {
            await blob.DeleteAsync();
        }

        public async Task DeleteImageAsync(string imageName)
        {
            var blob = GetBlob(imageName);
            await blob.DeleteIfExistsAsync();
        }


    }
}
