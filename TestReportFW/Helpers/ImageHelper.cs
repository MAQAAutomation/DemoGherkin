using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Demo.TestReport.Framework.Helpers
{
    public class ImageHelper
    {
        private const string JPEG_CODE_MIME_TYPE = "image/jpeg";

        /// <summary>
        /// Saves the iamge.
        /// </summary>
        /// <param name="filePath">The path.</param>
        /// <param name="img">The img.</param>
        /// <param name="quality">The quality.</param>
        /// <param name="imageCode">The image code.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">quality must be between 0 and 100.</exception>
        public static void SaveImage(string filePath, Image img, int quality, string imageCode = JPEG_CODE_MIME_TYPE)
        {
            if (quality < 0 || quality > 100)
            {
                throw new ArgumentOutOfRangeException(string.Format("SaveImage: quality must be between 0 and 100 and the current value is {0}", quality));
            }

            // Encoder parameter for image quality 
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
            // JPEG image codec 
            ImageCodecInfo jpegCodec = new ImageHelper().GetEncoderInfo(imageCode);
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            img.Save(filePath, jpegCodec, encoderParams);
        }

        /// <summary>
        /// Gets the encoder information.
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns></returns>
        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }
    }
}
