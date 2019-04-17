using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SoftImage.Vision.BlobStorage.Contracts;
using System;

using SoftImage.Vision.Common;

namespace SoftImage.Vision.BlobStorage
{
  public   class BlobService :IBlobService
    {
     private   CloudStorageAccount account => CloudStorageAccount.Parse(ConnectionInfo.connectionStringBlob);

        public  string GetPictureUrlByName(string imageName)
        {
       CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(ConnectionInfo.containerName);
            container.CreateIfNotExistsAsync().Wait();

            CloudBlockBlob blob = container.GetBlockBlobReference(imageName + ".jpg");

            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(5);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read;


            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);
            string imageURL = blob.Uri + sasBlobToken;

            return imageURL;
        }
    }
}
