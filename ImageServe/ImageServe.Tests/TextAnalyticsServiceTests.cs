//using BlobStorageTools;
//using BlobStorageTools.Contracts;
//using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
//using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
//using Microsoft.WindowsAzure.Storage.Blob;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using TextAnalyticsTools;
//using TextAnalyticsTools.Contracts;
//
//namespace ImageServe.Tests
//{
//    [TestFixture]
//    public class TextAnalyticsServiceTests
//    {
//        private readonly Uri MOCK_BLOB_URI = new Uri("http://mock/mock");
//        private const string MOCK_FILE_PATH = @"C:\Users\tunay\Pictures\guesthouse.jpg";
//        private const string MOCK_INVALID_FILE_PATH = @"C:\Users\tunay\Desktop\filmi.txt";
//        private const string MOCK_INVALID_FILE_EXTENSION = ".txt";
//        private const string MOCK_FILE_EXTENSION = ".jpg";
//        private const string MOCK_VALID_STRING = "This is a mock in Mockland";
//
//        private readonly ITextAnalyticsService textAnalyticsService;
//
//        public TextAnalyticsServiceTests()
//        {
//            // Arrange
//            var mockTextAnalyticsClient = this.MockTextAnalyticsClient();
//            this.textAnalyticsService = new TextAnalyticsService(mockTextAnalyticsClient.Object);
//        }
//
//        
//
//        [Test]
//        public async Task GenerateTagsAsync_WithValidString_ShouldReturnNonEmptyCollection()
//        {
//            // Act
//            var result = await this.textAnalyticsService.GenerateTagsAsync(MOCK_VALID_STRING);
//
//            // Assert
//            Assert.That(result, Is.Not.Null.Or.Empty);
//        }
//
//        [Test]
//        public async Task GenerateTagsAsync_WithEmptyString_ShouldReturnNull()
//        {
//            // Act
//            var result = await this.textAnalyticsService.GenerateTagsAsync(MOCK_VALID_STRING);
//
//            // Assert
//            Assert.That(result, Is.Null);
//        }
//
//        //TODO test if method returns null when text language is not supported ?????????
//
//        private Mock<ITextAnalyticsClient> MockTextAnalyticsClient()
//        {
//            var mockTextAnalyticsClient = new Mock<ITextAnalyticsClient>(MockBehavior.Strict);
//            var mockLanguageBatchResult = new LanguageBatchResult();
//
//            //Unable to setup DetectLanguageAsync because it's an extension method
//            mockTextAnalyticsClient
//                .Setup(client => client.DetectLanguageAsync(It.IsAny<BatchInput>(), It.IsAny<CancellationToken>()))
//                .Returns(Task.FromResult(mockLanguageBatchResult))
//                .Verifiable();
//
//            var mockKeyPhraseBatchResult = new KeyPhraseBatchResult();
//
//            mockTextAnalyticsClient
//                .Setup(client => client.KeyPhrasesAsync(It.IsAny<MultiLanguageBatchInput>(), It.IsAny<CancellationToken>()))
//                .Returns(Task.FromResult(mockKeyPhraseBatchResult))
//                .Verifiable();
//
//            return mockTextAnalyticsClient;
//        }
//    }
//}
