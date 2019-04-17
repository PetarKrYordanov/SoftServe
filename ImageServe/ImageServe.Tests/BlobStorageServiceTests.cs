//using BlobStorageTools;
using BlobStorageTools;
using BlobStorageTools.Contracts;
using ImageServe.Tests.Properties;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageServe.Tests
{
    [TestFixture]
    public class BlobStorageServiceTests
    {
        private readonly Uri MOCK_BLOB_URI = new Uri("http://mock/mock");
        private const string MOCK_INVALID_FILE_EXTENSION = ".txt";
        private const string MOCK_FILE_EXTENSION = ".jpg";
        private const string TEST_IMAGE_FILENAME = "kitty";
        private IBlobStorageService blobStorageService;



        public BlobStorageServiceTests()
        {
            // Arrange
            Mock<CloudBlockBlob> mockBlobItem = MockCloudBlob();
            Mock<CloudBlobContainer> mockBlobContainer = MockBlobContainer(mockBlobItem);
            blobStorageService = new BlobStorageService(mockBlobContainer.Object);
            
        }

        

        [Test]
        public void UploadFromFileAsync_WithValidStream_ShouldPassWithoutException()
        {
            // Act, Assert
            Assert.That(async() => await blobStorageService.UploadFromFileAsync(Resources.TestImage, TEST_IMAGE_FILENAME, MOCK_FILE_EXTENSION), Throws.Nothing);
        }

       



//        //[Test]
//        //public async Task UploadFromFileAsync_WithValidPath_ShouldReturnTrue()
//        //{
//        //    // Act
//        //    _result = await _blobStorageService.UploadFromFileAsync(MOCK_FILE_PATH, Mock_FILE_EXTENSION);
//        //
//        //    // Assert
//        //    Assert.That(_result, Is.True);
//        //}
//        //
//        //[Test]
//        //public async Task UploadFromFileAsync_WithInvalidPath_ShouldReturnFalse()
//        //{
//        //    // Act
//        //    _result = await _blobStorageService.UploadFromFileAsync(String.Empty, Mock_FILE_EXTENSION);
//        //
//        //    // Assert
//        //    Assert.That(_result, Is.False);
//        //}



        private Mock<CloudBlobContainer> MockBlobContainer(Mock<CloudBlockBlob> mockBlobItem)
        {
            var mockBlobContainer = new Mock<CloudBlobContainer>(MockBehavior.Strict, MOCK_BLOB_URI);

            mockBlobContainer
                .Setup(c => c.GetBlockBlobReference(It.IsAny<string>()))
                .Returns(mockBlobItem.Object)
                .Verifiable();

            return mockBlobContainer;
        }

        private Mock<CloudBlockBlob> MockCloudBlob()
        {
            var mockBlobItem = new Mock<CloudBlockBlob>(MockBehavior.Default, MOCK_BLOB_URI);

            mockBlobItem
                .Setup(b => b.UploadFromStreamAsync(It.IsAny<Stream>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            mockBlobItem
                .Setup(b => b.UploadFromFileAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            mockBlobItem
                .Setup(b => b.DeleteAsync())
                .Returns(Task.CompletedTask);
            return mockBlobItem;
        }
    }
}

