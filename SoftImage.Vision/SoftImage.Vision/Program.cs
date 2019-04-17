using System;

using SoftImage.Vision.BlobStorage;
using SoftImage.Vision.ComputerVision.Contracts;

namespace SoftImage.Vision
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter picture name");
            var imageName = Console.ReadLine();

         
            IImageInformation imageInformation = new TagInformation();
            var tagsInfo = imageInformation.GetAnalysisAsync(imageName);

            
            Console.ReadKey();
        }
    }
}
