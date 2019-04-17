namespace ComputerVisionTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
    using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

    using ComputerVisionTools.Contracts;

    public class ComputerVisionService : IComputerVisionService
    {
        private readonly List<VisualFeatureTypes> features = new List<VisualFeatureTypes>() { VisualFeatureTypes.Tags };

        private readonly IComputerVisionClient client;


        public ComputerVisionService(IComputerVisionClient client)
        {
            this.client = client;
        }

        public async Task<ICollection<ImageServe.Models.ImageTag>> GenerateTagsAsync(string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                return null;
            }

            var analysis = await this.client.AnalyzeImageAsync(imageUrl, features);

            var tags = this.ExtractTags(analysis);

            return tags;
        }

        private ICollection<ImageServe.Models.ImageTag> ExtractTags(ImageAnalysis analysis) => analysis.Tags.Select(t => new ImageServe.Models.ImageTag() { Name = t.Name }).ToList();
    }

}
