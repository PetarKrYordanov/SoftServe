using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace SoftImage.Vision.ComputerVision.Contracts
{
    public interface IImageInformation
    {
        ImageAnalysis GetAnalysisAsync(string url);
    }
}
