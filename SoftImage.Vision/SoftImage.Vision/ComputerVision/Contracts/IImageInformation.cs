using System.Collections.Generic;

namespace SoftImage.Vision.ComputerVision.Contracts
{
    public interface IImageInformation
    {
        ICollection<string> GetAnalysisAsync(string url);
    }
}
