using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using SoftImage.Vision.ComputerVision.Contracts;

namespace SoftImage.Vision
{
    public class ObjectInformation : ImageInformation, IImageInformation
    {
        public override ImageAnalysis GetAnalysisAsync(string url)
        {
            this.visionClient.Endpoint = base.endpoint;

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.WriteLine(
                    "\nInvalid remoteImageUrl:\n{0} \n", url);

            }
            var task = visionClient.AnalyzeImageAsync(url, this.FeatureTypes());
            Task.WhenAll(task);

            return task.Result;
        }
        public override List<VisualFeatureTypes> FeatureTypes()
        {
            return new List<VisualFeatureTypes>() { VisualFeatureTypes.Objects, VisualFeatureTypes.Adult, VisualFeatureTypes.Color, VisualFeatureTypes.Categories };
        }


    }
}
