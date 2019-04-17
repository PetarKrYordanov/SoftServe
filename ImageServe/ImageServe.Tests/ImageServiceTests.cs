using AutoMapper;
using BlobStorageTools.Contracts;
using ImageServe.Data;
using ImageServe.Data.Common;
using ImageServe.Models;
using ImageServe.Services;
using ImageServe.Services.Contracts;
using ImageServe.WebModels.BindingModels;
using ImageServe.WebModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageServe.Tests
{
    class ImageServiceTests
    {
        ImageService imageService;
        User friend;
        User unknown;

        public ImageServiceTests()
        {
            //mocks
            var options = new DbContextOptionsBuilder<ImageServeDbContext>()
              .UseInMemoryDatabase(databaseName: "ImageServeTest") // Give a Unique name to the DB
              .Options;

            var dbContext = new ImageServeDbContext(options);

            var imageRepository = new DbRepository<Image>(dbContext);

            var likesRepository = new DbRepository<ImageLike>(dbContext);

            var userRepository = new DbRepository<User>(dbContext);

            var tagsRepository = new DbRepository<ImageTag>(dbContext);

            var mapperMock = new Mock<IMapper>();

            var userServiceMock = new Mock<IUserService>();

            userServiceMock
                .Setup(us => us.GetCurrentId())
                .Returns("2")
                .Verifiable();


            var tagServiceMock = new Mock<ITagService>();

            tagServiceMock
                .Setup(ts => ts.AddToImageAsync(It.IsAny<Image>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var blobServiceMock = new Mock<IBlobStorageService>();

            blobServiceMock
                .Setup(bs => bs.UploadFromFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            



            //arrange
            //AllByUser arrange
            this.friend = new User()
            {
                Id = "2"
            };

            friend.UserFriends.Add(new Friendship()
            {
                UserId = friend.Id,
                FriendId = "2"
            });

            this.unknown = new User()
            {
                Id = "3"
            };

            var friendPublicImage = new Image()
            {
                Id = 101,
                UserId = friend.Id,
                User = friend,
                IsPublic = true

            };

            var friendPrivateImage = new Image()
            {
                Id = 102,
                UserId = friend.Id,
                User = friend,
                IsPublic = true

            };

            var unknownPublicImage = new Image()
            {
                Id = 103,
                UserId = unknown.Id,
                User = unknown,
                IsPublic = true
            };

            var unknownPrivateImage = new Image()
            {
                Id = 104,
                UserId = unknown.Id,
                User = unknown,
                IsPublic = false
            };

            imageRepository.AddAsync(friendPublicImage).GetAwaiter().GetResult();
            imageRepository.AddAsync(friendPrivateImage).GetAwaiter().GetResult();
            imageRepository.AddAsync(unknownPublicImage).GetAwaiter().GetResult();
            imageRepository.AddAsync(unknownPrivateImage).GetAwaiter().GetResult();
            imageRepository.SaveChangesAsync().GetAwaiter().GetResult();


            imageService = new ImageService(imageRepository, mapperMock.Object, tagServiceMock.Object, userServiceMock.Object, blobServiceMock.Object, likesRepository);
        }


        [Test]
        public void AllByUser_FrendUser_ShouldReturnTwoImage()
        {
            var images = this.imageService.AllByUser(friend.Id);

            Assert.AreEqual(3, images.Count);
        }

        [Test]
        public void AllByUser_UnknownUser_ShouldReturnOneImage()
        {
            var images = this.imageService.AllByUser(unknown.Id);

            Assert.AreEqual(1, images.Count);
        }

        [Test]
        public void AddAsync_WithValidImage_Should_Pass()
        {
            var IFormFileMock = new Mock<IFormFile>();
            IFormFileMock
                 .Setup(f => f.OpenReadStream())
                .Returns(this.GenerateStreamFromString("asd"))
                .Verifiable();

            IFormFileMock
                .Setup(f => f.FileName)
                .Returns("test.jpg");

            var uploadBindingModel = new ImageUploadBindingModel()
            {
                File = IFormFileMock.Object
            };

            Assert.That(async () => await this.imageService.AddAsync(uploadBindingModel), Throws.Nothing);
        }

        [Test]
        public void AddAsync_WithEmptyFile_Should_ThrowError()
        {
            var memoryStream = new MemoryStream();

            var IFormFileMock = new Mock<IFormFile>();
            IFormFileMock
                .Setup(f => f.OpenReadStream())
               .Returns(memoryStream)
               .Verifiable();


            IFormFileMock
                .Setup(f => f.FileName)
                .Returns("test.jpg");

            var uploadBindingModel = new ImageUploadBindingModel()
            {
                File = IFormFileMock.Object
            };

            Assert.ThrowsAsync<FileNotFoundException>(async () => await this.imageService.AddAsync(uploadBindingModel));

        }

        [Test]
        public void AddAsync_WithInvalidFileExtension_Should_ThrowError()
        {
            var IFormFileMock = new Mock<IFormFile>();
            IFormFileMock
                .Setup(f => f.OpenReadStream())
               .Returns(this.GenerateStreamFromString("asd"))
               .Verifiable();


            IFormFileMock
                .Setup(f => f.FileName)
                .Returns("test.txt");

            var uploadBindingModel = new ImageUploadBindingModel()
            {
                File = IFormFileMock.Object
            };

            Assert.ThrowsAsync<FormatException>(async () => await this.imageService.AddAsync(uploadBindingModel));

        }

        [Test]
        public void GetImageAsync_FriendUserPrivateImage_ShoudlPass()
        {
            Assert.That(async () => await this.imageService.GetImageAsync<ImageViewModel>(102), Throws.Nothing);
        }

        [Test]
        public void GetImageAsync_UnknownUserPrivateImage_ShoudlThrowError()
        {
            Assert.ThrowsAsync<AccessViolationException>(async () => await this.imageService.GetImageAsync<ImageViewModel>(104));
        }

        [Test]
        public void GetImageAsync_UnknownUserPublicImage_ShoudlPass()
        {
            Assert.That(async () => await this.imageService.GetImageAsync<ImageViewModel>(103), Throws.Nothing);
        }

        [Test]
        public void GetImageAsync_FriendUserPublicImage_ShoudlPass()
        {
            Assert.That(async () => await this.imageService.GetImageAsync<ImageViewModel>(101), Throws.Nothing);
        }

        //Tests privacy on news feed.
        [Test]
        public void Newest_ShouldReturn_ThreeImages()
        {
            var images = this.imageService.Newest(1, 6);

            Assert.AreEqual(images.Count, 4);
        }

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        


        //var textAnalyticsMock = new Mock<ITextAnalyticsService>();
        //
        //textAnalyticsMock
        //    .Setup(t => t.GenerateTagsAsync(It.IsAny<string>()))
        //    .Returns(Task.FromResult((ICollection<ImageTag>)new List<ImageTag>()));
        //
        //var computerVisionMock = new Mock<IComputerVisionService>();
        //
        //computerVisionMock
        //    .Setup(cv => cv.GenerateTagsAsync(It.IsAny<string>()))
        //    .Returns(Task.FromResult((ICollection<ImageTag>)new List<ImageTag>()));
        //
        //
    }
}
