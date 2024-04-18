//using System.Drawing;
//using System.Drawing.Imaging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Png;

namespace MicroServiceAzureService.Helpers
{
    public class ImageResizer
    {
        //public static Stream ResizeImage(Stream inputImageStream, int maxWidth, int maxHeight, int qualityLevel)
        //{
        //    // Load the image from the input stream

        //        using (var image = Image.FromStream(inputImageStream))
        //        {
        //            // Calculate the new dimensions while maintaining the aspect ratio
        //            var ratioX = (double)maxWidth / image.Width;
        //            var ratioY = (double)maxHeight / image.Height;
        //            var ratio = Math.Min(ratioX, ratioY);

        //            var newWidth = (int)(image.Width * ratio);
        //            var newHeight = (int)(image.Height * ratio);

        //            // Create a new bitmap with the resized dimensions
        //            using (var resizedImage = new Bitmap(newWidth, newHeight))
        //            {
        //                // Draw the original image onto the resized bitmap
        //                using (var graphics = Graphics.FromImage(resizedImage))
        //                {
        //                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        //                }

        //                // Create a MemoryStream to store the resized image
        //                var resizedImageStream = new MemoryStream();

        //                // Save the resized image to the MemoryStream with the specified quality level
        //                resizedImage.Save(resizedImageStream, ImageFormat.Jpeg); // You can change the format if needed

        //                // Reset the position of the MemoryStream to the beginning
        //                resizedImageStream.Position = 0;

        //                // Return the MemoryStream containing the resized image
        //                return resizedImageStream;
        //            }
        //        }
        //}
        public static Stream ResizeImage(Stream inputImageStream, int maxWidth, int maxHeight, int qualityLevel)
        {
            // Load the image using ImageSharp
            using (var image = Image.Load(inputImageStream))
            {
                // Access the image format metadata
                var format = image.Metadata.DecodedImageFormat;

                // Perform resizing operations as before
                var newWidth = CalculateNewWidth(image.Width, image.Height, maxWidth, maxHeight);
                var newHeight = CalculateNewHeight(image.Width, image.Height, maxWidth, maxHeight);

                image.Mutate(x => x.Resize(newWidth, newHeight));

                // Create a MemoryStream to store the resized image
                var resizedImageStream = new MemoryStream();

                // Save the resized image to the MemoryStream with the specified quality level
                switch (format)
                {
                    case PngFormat pngFormat:
                        image.Save(resizedImageStream, new PngEncoder());
                        break;
                    case JpegFormat jpegFormat:
                        image.Save(resizedImageStream, new JpegEncoder { Quality = qualityLevel });
                        break;
                    case GifFormat gifFormat:
                        image.Save(resizedImageStream, new GifEncoder());
                        break;
                    case BmpFormat bmpFormat:
                        image.Save(resizedImageStream, new BmpEncoder());
                        break;
                    case TgaFormat tgaFormat:
                        image.Save(resizedImageStream, new TgaEncoder());
                        break;
                    case TiffFormat tiffFormat:
                        image.Save(resizedImageStream, new TiffEncoder());
                        break;
                    // Add handling for other image formats if necessary
                    default:
                        throw new NotSupportedException($"Image format {format.Name} is not supported for resizing.");
                }

                // Reset the position of the MemoryStream to the beginning
                resizedImageStream.Position = 0;

                // Return the MemoryStream containing the resized image
                return resizedImageStream;
            }
        }

        // Helper methods for calculating new dimensions while maintaining aspect ratio
        private static int CalculateNewWidth(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / originalWidth;
            double ratioY = (double)maxHeight / originalHeight;
            double ratio = Math.Min(ratioX, ratioY);
            return (int)Math.Round(originalWidth * ratio);
        }

        private static int CalculateNewHeight(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / originalWidth;
            double ratioY = (double)maxHeight / originalHeight;
            double ratio = Math.Min(ratioX, ratioY);
            return (int)Math.Round(originalHeight * ratio);
        }
    }
}
