using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using SoftImage.Vision.BlobStorage;
using SoftImage.Vision.ComputerVision.Contracts;

namespace SoftImage.Vision
{
    public class TagInformation : ImageInformation, IImageInformation
    {
        public override ICollection<string> GetAnalysisAsync(string imageName)
        {
            BlobService blobService = new BlobService();
            string imageUrl = blobService.GetPictureUrlByName(imageName);

            this.visionClient.Endpoint = base.endpoint;

            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine(
                    "\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
            }
            var task = this.visionClient.AnalyzeImageAsync(imageUrl, this.FeatureTypes());
            Task.WaitAll(task);

            ICollection<string> tags = new HashSet<string>();

            foreach (var item in task.Result.Tags)
            {
                tags.Add(item.Name);
            }

            return tags;
        }

        public override List<VisualFeatureTypes> FeatureTypes()
        {
            return new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Tags,
            };
        }
    }
}
