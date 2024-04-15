using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MicroServiceBlobService.Helpers
{
    public class ImageResizer
    {
        public static Stream ResizeImage(Stream inputImageStream, int maxWidth, int maxHeight, int qualityLevel)
        {
            // Load the image from the input stream
         
                using (var image = Image.FromStream(inputImageStream))
                {
                    // Calculate the new dimensions while maintaining the aspect ratio
                    var ratioX = (double)maxWidth / image.Width;
                    var ratioY = (double)maxHeight / image.Height;
                    var ratio = Math.Min(ratioX, ratioY);

                    var newWidth = (int)(image.Width * ratio);
                    var newHeight = (int)(image.Height * ratio);

                    // Create a new bitmap with the resized dimensions
                    using (var resizedImage = new Bitmap(newWidth, newHeight))
                    {
                        // Draw the original image onto the resized bitmap
                        using (var graphics = Graphics.FromImage(resizedImage))
                        {
                            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                        }

                        // Create a MemoryStream to store the resized image
                        var resizedImageStream = new MemoryStream();

                        // Save the resized image to the MemoryStream with the specified quality level
                        resizedImage.Save(resizedImageStream, ImageFormat.Jpeg); // You can change the format if needed

                        // Reset the position of the MemoryStream to the beginning
                        resizedImageStream.Position = 0;

                        // Return the MemoryStream containing the resized image
                        return resizedImageStream;
                    }
                }
        }
    }
}
