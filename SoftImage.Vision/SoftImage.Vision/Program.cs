using System;

using SoftImage.Vision.BlobStorage;
using SoftImage.Vision.ComputerVision.Contracts;

namespace SoftImage.Vision
{
    public class Program
    {
        public static void Main(string[] args)
        {
           // Console.WriteLine("Enter picture name");
           //var imageName =  Console.ReadLine();

            BlobService blobService = new BlobService();
            string url = blobService.GetPictureUrlByName("wall2");

            IImageInformation imageInformation = new TagInformation();
            var tagsInfo = imageInformation.GetAnalysisAsync(url);

            Console.WriteLine("Computer vision found tags:");
            foreach (var item in tagsInfo.Tags)
            {
                Console.WriteLine($"{item.Name} with confidence {item.Confidence:f3}");
            }
            Console.ReadKey();
        }
    }
}
