namespace ImageServe.Services
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using ComputerVisionTools.Contracts;
    using ImageServe.Data.Common;
    using ImageServe.Models;
    using ImageServe.Services.Contracts;
    using TextAnalyticsTools.Contracts;

    public class TagService: ITagService
    {
        private readonly IRepository<ImageTag> tags;
        private readonly ITextAnalyticsService textAnalyticsService;
        private readonly IComputerVisionService computerVisionService;

        public TagService(IRepository<ImageTag> tags, ITextAnalyticsService textAnalyticsService, IComputerVisionService computerVisionService)
        {
            this.tags = tags;
            this.textAnalyticsService = textAnalyticsService;
            this.computerVisionService = computerVisionService;
        }

        public async Task<ICollection<string>> AllByImageAsync(int imageId)
        {
            var tags = await this.tags
                .All()
                .Where(t => t.ImageID == imageId)
                .Select(t => t.Name)
                .ToListAsync();

            return tags;
        }

        public async Task AddToImageAsync(Image image)
        {
            var computerVisionTagsTask = this.computerVisionService.GenerateTagsAsync(image.GetImageUrl());
            var descriptionTagsTask = this.textAnalyticsService.GenerateTagsAsync(image.Description);
            await Task.WhenAll(computerVisionTagsTask, descriptionTagsTask);
            this.AddTags(image, computerVisionTagsTask.Result, descriptionTagsTask.Result);
        }

        private void AddTags(Image image, ICollection<ImageTag> computerVisionTags, ICollection<ImageTag> descriptionTags)
        {
            var tagsUniqueByName = ExtractUniqueTagsByName(new[]
            {
                computerVisionTags,
                descriptionTags,
            });

            image.Tags = tagsUniqueByName.ToList();
        }

        public async Task UpdateDescriptionTagsAsync(Image image)
        {
            var descriptionTags = await this.textAnalyticsService.GenerateTagsAsync(image.Description);
            var otherTags = image.Tags.Where(t => !t.FromDescription).ToList();

            var tagsUniqueByName = ExtractUniqueTagsByName(new[]
            {
                otherTags,
                descriptionTags,
            });

            image.Tags = tagsUniqueByName.ToList();
        }

        private IEnumerable<ImageTag> ExtractUniqueTagsByName(IEnumerable<ImageTag>[] tagsLists) =>
            tagsLists
                .Where(tags => tags != null)
                .SelectMany(tags => tags)
                .GroupBy(tag => tag.Name.ToLower())
                .Select(g => g.First());

        
    }
}