namespace TriggerFunctions
{

    using System.Drawing;
    using System.IO;
    using ImageProcessor;
    using ImageProcessor.Imaging.Formats;

    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    public static class ImageThumbFunction
    {
        private const int RECOMMENDED_WIDTH = 497;
        private const int RECOMMENDED_HEIGHT = 373;
        private const string CONNECTION_STRING = "connection";

        [FunctionName("ImageThumbFunction")]
        public static void Run(
            [BlobTrigger("images/{name}", Connection = CONNECTION_STRING)]Stream image,
            [Blob("thumbs/{name}-thumb.jpg", FileAccess.Write, Connection = CONNECTION_STRING)]Stream imageResized, 
            ILogger logger)
        {
            logger.LogInformation("A file has been uploaded to the images container.");

            using (var imageFactory = new ImageFactory())
            {
                var size = new Size(RECOMMENDED_WIDTH, RECOMMENDED_HEIGHT);
                var format = new JpegFormat() { Quality = 70 };
                imageFactory
                    .Load(image)
                    .Resize(size)
                    .Format(format)
                    .Save(imageResized);
            }
            
            logger.LogInformation("A thumb has been generated in the thumbs container.");
            
        }

    }
}
