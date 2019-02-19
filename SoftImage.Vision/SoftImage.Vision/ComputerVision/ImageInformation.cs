using System.Collections.Generic;

using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using SoftImage.Vision.Common;
using SoftImage.Vision.ComputerVision.Contracts;

namespace SoftImage.Vision
{
    public abstract class ImageInformation : IImageInformation
    {

        protected string endpoint = ConnectionInfo.endpointComputerVision;

        public abstract ImageAnalysis GetAnalysisAsync(string url);

        public virtual List<VisualFeatureTypes> FeatureTypes()
        {
            return new List<VisualFeatureTypes>() { VisualFeatureTypes.Tags };
        }

        public ComputerVisionClient visionClient = new ComputerVisionClient(
            new ApiKeyServiceClientCredentials(ConnectionInfo.subscriptionKeyComputerVision),
            new System.Net.Http.DelegatingHandler[] { });

    }
}
