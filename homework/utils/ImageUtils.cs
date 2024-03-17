using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace homework.utils
{
    internal class ImageUtils
    {
        /// <summary>
        ///This method downscales an input Bitmap image using a parallelized approach.
        ///It divides the downscaled image into strips based on the number of CPU cores available, 
        ///and each core processes a portion of the image concurrently.The downscaled image is computed by averaging the color values
        ///of the pixels within each region of the original image corresponding to a pixel in the downscaled image.The downscaled image is then returned.
        ///This method avoids using unsafe code and directly accessing BitmapData, achieving the downsampling operation in a safe and efficient manner.
        /// </summary>
        /// <param name="originalImage">The original image to be downscaled</param>
        /// <param name="downscaleFactor">The factor by which the image should be downscaled</param>
        /// <returns>Bitmap (the downscaled image)</returns>
        public static Bitmap DownsizeImageInParallel(Bitmap originalImage, float downscaleFactor)
        {
            int bytesPerPixel = Image.GetPixelFormatSize(originalImage.PixelFormat) / 8;
            int downscaledWidth = (int)(originalImage.Width * downscaleFactor);
            int downscaledHeight = (int)(originalImage.Height * downscaleFactor);

            Bitmap downsizedImage = new(downscaledWidth, downscaledHeight, originalImage.PixelFormat);

            Rectangle originRect = new(0, 0, originalImage.Width, originalImage.Height);
            Rectangle downsizedRect = new(0, 0, downscaledWidth, downscaledHeight);

            // Locks the Bitmap's bit in memory to access its pixel data directly.
            BitmapData originalBitmapData = originalImage.LockBits(originRect, ImageLockMode.ReadOnly, originalImage.PixelFormat);
            BitmapData downsizedBitmapData = downsizedImage.LockBits(downsizedRect, ImageLockMode.WriteOnly, downsizedImage.PixelFormat);

            byte[] originalImageBytes = new byte[originalBitmapData.Stride * originalBitmapData.Height];
            byte[] downsizedImageBytes = new byte[downsizedBitmapData.Stride * downscaledHeight];

            Marshal.Copy(originalBitmapData.Scan0, originalImageBytes, 0, originalImageBytes.Length);
            originalImage.UnlockBits(originalBitmapData);

            try
            {
                ProcessImagePixelsInParallel(downscaledWidth, downscaledHeight, downscaleFactor, bytesPerPixel,
                             originalBitmapData, downsizedBitmapData, originalImageBytes, downsizedImageBytes);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
            finally
            {
                Marshal.Copy(downsizedImageBytes, 0, downsizedBitmapData.Scan0, downsizedImageBytes.Length);
                downsizedImage.UnlockBits(downsizedBitmapData);
            }

            return downsizedImage;
        }

        private static void ProcessImagePixelsInParallel(int downscaledWidth, int downscaledHeight, float downscaleFactor, int bytesPerPixel,
            BitmapData originalBitmapData, BitmapData downsizedBitmapData, byte[] originalImageBytes, byte[] downsizedImageBytes)
        {

            int numCores = Environment.ProcessorCount;

            Parallel.For(0, numCores, coreIndex =>
            {
                int startY = coreIndex * downscaledHeight / numCores;
                int endY = (coreIndex + 1) * downscaledHeight / numCores;

                for (int y = startY; y < endY; y++)
                {
                    for (int x = 0; x < downscaledWidth; x++)
                    {
                        int startX = (int)(x / downscaleFactor);
                        int endX = (int)((x + 1) / downscaleFactor);

                        long sumR = 0, sumG = 0, sumB = 0, sumA = 0;
                        int count = 0;

                        for (int srcY = (int)(y / downscaleFactor); srcY < (int)((y + 1) / downscaleFactor); srcY++)
                        {
                            for (int srcX = startX; srcX < endX; srcX++)
                            {
                                int srcIndex = srcY * originalBitmapData.Stride + srcX * bytesPerPixel;

                                //if the pixel has 3 bytes, it means that the image has 24 Bith depth and represents RGB color model
                                sumB += originalImageBytes[srcIndex];
                                sumG += originalImageBytes[srcIndex + 1];
                                sumR += originalImageBytes[srcIndex + 2];

                                //if the pixel has 4 bytes, it means that the image has 32 Bith depth and
                                //represents ARGB color model, including alpha transparency
                                if (bytesPerPixel == 4)
                                {
                                    sumA += originalImageBytes[srcIndex + 3];
                                }
                                count++;
                            }
                        }

                        int destIndex = y * downsizedBitmapData.Stride + x * bytesPerPixel;
                        downsizedImageBytes[destIndex] = (byte)(sumB / count);
                        downsizedImageBytes[destIndex + 1] = (byte)(sumG / count);
                        downsizedImageBytes[destIndex + 2] = (byte)(sumR / count);
                        if (bytesPerPixel == 4)
                        {
                            downsizedImageBytes[destIndex + 3] = (byte)(sumA / count);
                        }
                    }
                }
            });
        }



        /// <summary>
        /// Downsize an image using unsafe code which directly access BitmapData.
        /// Parallel.For loop is used to process the rows of 
        /// the image in parallel. Each iteration of the loop gets a pointer to the start of a row,
        /// and then processes the pixels in that row.
        /// This allows the pixel data to be processed in parallel.
        /// </summary>
        /// <param name="image">The original image to be downscaled</param>
        /// <param name="downscaleFactor">The factor by which the image should be downscaled</param>
        /// <returns>Bitmap (the downscaled image)</returns>

        public static Bitmap DownsizeImageUnsafeInParallel(Bitmap image, float downscaleFactor)
        {
            int downscaledWidth = (int)(image.Width / downscaleFactor);
            int downscaledHeight = (int)(image.Height / downscaleFactor);

            Bitmap outputImage = new(downscaledWidth, downscaledHeight, image.PixelFormat);

            BitmapData sourceData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);
            BitmapData outputData = outputImage.LockBits(new Rectangle(0, 0, downscaledWidth, downscaledHeight),
                ImageLockMode.WriteOnly, outputImage.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;

            try
            {
                unsafe
                {
                    byte* sourcePtr = (byte*)sourceData.Scan0;
                    byte* outputPtr = (byte*)outputData.Scan0;
                    int numCores = Environment.ProcessorCount;

                    Parallel.For(0, numCores, coreIndex =>
                    {
                        int start = coreIndex * downscaledHeight / numCores;
                        int end = (coreIndex + 1) * downscaledHeight / numCores;

                        for (int y = start; y < end; y++)
                        {
                            for (int x = 0; x < downscaledWidth; x++)
                            {
                                int startX = (int)(x * downscaleFactor);
                                int startY = (int)(y * downscaleFactor);
                                int endX = (int)((x + 1) * downscaleFactor);
                                int endY = (int)((y + 1) * downscaleFactor);

                                long sumR = 0, sumG = 0, sumB = 0, sumA = 0;
                                int count = 0;

                                for (int srcY = startY; srcY < endY; srcY++)
                                {
                                    for (int srcX = startX; srcX < endX; srcX++)
                                    {
                                        byte* srcPixel = sourcePtr + srcY * sourceData.Stride + srcX * bytesPerPixel;

                                        if (bytesPerPixel == 3)
                                        {
                                            sumB += srcPixel[0];
                                            sumG += srcPixel[1];
                                            sumR += srcPixel[2];
                                        }
                                        else if (bytesPerPixel == 4)
                                        {
                                            sumB += srcPixel[0];
                                            sumG += srcPixel[1];
                                            sumR += srcPixel[2];
                                            sumA += srcPixel[3];
                                        }
                                        count++;
                                    }
                                }

                                byte* destPixel = outputPtr + y * outputData.Stride + x * bytesPerPixel;
                                destPixel[0] = (byte)(sumB / count);
                                destPixel[1] = (byte)(sumG / count);
                                destPixel[2] = (byte)(sumR / count);
                                if (bytesPerPixel == 4)
                                {
                                    destPixel[3] = (byte)(sumA / count);
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
            finally
            {
                image.UnlockBits(sourceData);
                outputImage.UnlockBits(outputData);
            }

            return outputImage;
        }

        /// <summary>
        ///This method downscales an input Bitmap image using a sequential approach.
        ///It calculates the size of the downscaled image based on the provided downscale factor.
        ///Then, it iterates over each pixel in the downscaled image and calculates the corresponding region 
        ///in the original image from which to sample.It computes the color of each pixel in the downscaled image by 
        ///averaging the color values of the pixels within the corresponding region of the original image.The downscaled image is then returned.
        /// </summary>
        /// <param name="originalImage">The original image to be downscaled</param>
        /// <param name="downscaleFactor">The factor by which the image should be downscaled</param>
        /// <returns>Bitmap (the downscaled image)</returns>
        public static Bitmap ConsequentialDownscaleImage(Bitmap originalImage, float downscaleFactor)
        {
            int downscaledWidth = (int)(originalImage.Width * downscaleFactor);
            int downscaledHeight = (int)(originalImage.Height * downscaleFactor);

            Bitmap downscaledImage = new Bitmap(downscaledWidth, downscaledHeight, originalImage.PixelFormat);

            Rectangle sourceRect = new(0, 0, originalImage.Width, originalImage.Height);
            Rectangle outputRect = new(0, 0, downscaledWidth, downscaledHeight);

            BitmapData originalData = originalImage.LockBits(sourceRect, ImageLockMode.ReadOnly, originalImage.PixelFormat);
            BitmapData downscaledData = downscaledImage.LockBits(outputRect, ImageLockMode.WriteOnly, downscaledImage.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(originalImage.PixelFormat) / 8;

            byte[] originalBytes = new byte[originalData.Stride * originalData.Height];
            byte[] downscaledBytes = new byte[downscaledData.Stride * downscaledData.Height];

            Marshal.Copy(originalData.Scan0, originalBytes, 0, originalBytes.Length);
            originalImage.UnlockBits(originalData);
            try
            {
                for (int y = 0; y < downscaledHeight; y++)
                {
                    for (int x = 0; x < downscaledWidth; x++)
                    {
                        int startX = (int)(x / downscaleFactor);
                        int startY = (int)(y / downscaleFactor);
                        int endX = (int)((x + 1) / downscaleFactor);
                        int endY = (int)((y + 1) / downscaleFactor);

                        long sumR = 0, sumG = 0, sumB = 0, sumA = 0;
                        int count = 0;

                        for (int srcY = startY; srcY < endY; srcY++)
                        {
                            for (int srcX = startX; srcX < endX; srcX++)
                            {
                                int srcIndex = srcY * originalData.Stride + srcX * bytesPerPixel;

                                if (srcIndex < originalBytes.Length)
                                {
                                    sumB += originalBytes[srcIndex];
                                    sumG += originalBytes[srcIndex + 1];
                                    sumR += originalBytes[srcIndex + 2];
                                    if (bytesPerPixel == 4)
                                        sumA += originalBytes[srcIndex + 3];
                                    count++;
                                }
                            }
                        }

                        int destIndex = y * downscaledData.Stride + x * bytesPerPixel;
                        downscaledBytes[destIndex] = (byte)(sumB / count);
                        downscaledBytes[destIndex + 1] = (byte)(sumG / count);
                        downscaledBytes[destIndex + 2] = (byte)(sumR / count);
                        if (bytesPerPixel == 4)
                            downscaledBytes[destIndex + 3] = (byte)(sumA / count);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
            finally
            {
                Marshal.Copy(downscaledBytes, 0, downscaledData.Scan0, downscaledBytes.Length);
                downscaledImage.UnlockBits(downscaledData);
            }


            return downscaledImage;
        }



        // Bilinear interpolation using parallel loop.
        /// <summary>
        ///This method downscales an input image represented as a 2D array of Color objects using bilinear interpolation in a parallelized manner.
        ///It calculates the dimensions of the downscaled image based on the provided scale factor.
        ///Then, it iterates over each row of the output image in parallel.For each row, 
        ///it computes the corresponding positions in the input image, performs bilinear interpolation to determine the color value at each pixel,
        ///and assigns the interpolated color to the corresponding pixel in the output image.The downscaled image is returned as a 2D array of Color objects.
        /// </summary>
        /// <param name="colorMatrix">The input image represented as a 2D array of Color objects</param>
        /// <param name="downscaleFactor">The scale factor by which the image should be downscaled</param>
        /// <param name="bytesPerPixel">The number of bytes per pixel in the image</param>
        /// <returns>Bitmap (the downscaled image)</returns>
        public static Color[][] DownsizeImageBilinearlyInParallel(Color[][] colorMatrix, double downscaleFactor, int bytesPerPixel)
        {
            int originHeight = colorMatrix.Length;
            int originWidth = colorMatrix[0].Length;

            int scaledHeigth = (int)(originHeight * downscaleFactor);
            int scaledWidth = (int)(originWidth * downscaleFactor);

            Color[][] scaledColorMatrix = new Color[scaledHeigth][];

            try
            {

                Parallel.For(0, scaledHeigth, row =>
                {
                    Color[] newRow = new Color[scaledWidth];

                    double y = row / downscaleFactor;
                    int yBottom = (int)Math.Floor(y);
                    int yTop = Math.Min(yBottom + 1, originHeight - 1);
                    double yRatio = y - yBottom;

                    for (int col = 0; col < scaledWidth; col++)
                    {
                        double x = col / downscaleFactor;
                        int xBottom = (int)Math.Floor(x);
                        int xTop = Math.Min(xBottom + 1, originWidth - 1);
                        double xRatio = x - xBottom;

                        Color interpolatedColor = BilinearInterpolate(
                            colorMatrix[yBottom][xBottom],
                            colorMatrix[yBottom][xTop],
                            colorMatrix[yTop][xBottom],
                            colorMatrix[yTop][xTop],
                            xRatio,
                            yRatio,
                            bytesPerPixel
                        );

                        newRow[col] = interpolatedColor;
                    }

                    scaledColorMatrix[row] = newRow;
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }

            return scaledColorMatrix;
        }


        private static Color BilinearInterpolate(Color topLeftColor, Color topRightColor, Color bottomLeftColor, Color bottomRightColor, double xRatio, double yRatio, int bytesPerPixel)
        {
            int colorBlue = InterpolateComponent(topLeftColor.B, topRightColor.B, bottomLeftColor.B, bottomRightColor.B, xRatio, yRatio);
            int colorGreen = InterpolateComponent(topLeftColor.G, topRightColor.G, bottomLeftColor.G, bottomRightColor.G, xRatio, yRatio);
            int colorRed = InterpolateComponent(topLeftColor.R, topRightColor.R, bottomLeftColor.R, bottomRightColor.R, xRatio, yRatio);
            if (bytesPerPixel == 4)
            {
                int alphaTransparency = InterpolateComponent(topLeftColor.A, topRightColor.A, bottomLeftColor.A, bottomRightColor.A, xRatio, yRatio);
                return Color.FromArgb(alphaTransparency, colorRed, colorGreen, colorBlue);
            }
            else
            {
                return Color.FromArgb(colorRed, colorGreen, colorBlue);
            }

        }

        private static int InterpolateComponent(int topLeft, int topRight, int bottomLeft, int bottomRight, double xRatio, double yRatio)
        {

            double interpolatedValue = topLeft * (1 - xRatio) * (1 - yRatio) +
                                        topRight * xRatio * (1 - yRatio) +
                                        bottomLeft * (1 - xRatio) * yRatio +
                                        bottomRight * xRatio * yRatio;

            //set the interpolated value to the valid color component range [0, 255].
            return (int)Math.Max(0, Math.Min(255, interpolatedValue));
        }



        public static Color[][] BitmapToColorMatrix(Bitmap image)
        {
            int imageHeight = image.Height;
            int imageWidth = image.Width;
            int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;

            //First dimention: rows; Second one: number of pixels in the row
            Color[][] colorMatrix = new Color[imageHeight][];

            Rectangle imageRect = new(0, 0, imageWidth, imageHeight);
            BitmapData imageToBitmapData = image.LockBits(imageRect, ImageLockMode.ReadOnly, image.PixelFormat);

            byte[] imagePixelsInRow = new byte[imageToBitmapData.Stride];

            for (int y = 0; y < imageHeight; y++)
            {
                colorMatrix[y] = new Color[imageWidth];

                Marshal.Copy(imageToBitmapData.Scan0 + (y * imageToBitmapData.Stride), imagePixelsInRow, 0, imagePixelsInRow.Length);

                for (int x = 0; x < imageWidth; x++)
                {
                    // Calculate the starting index of the pixel data in the byte array.
                    int pixelStartIndex = x * bytesPerPixel;

                    int b = imagePixelsInRow[pixelStartIndex];
                    int g = imagePixelsInRow[pixelStartIndex + 1];
                    int r = imagePixelsInRow[pixelStartIndex + 2];
                    if (bytesPerPixel == 4)
                    {
                        int a = imagePixelsInRow[pixelStartIndex + 3];
                        colorMatrix[y][x] = Color.FromArgb(a, r, g, b);
                    }
                    else
                    {
                        colorMatrix[y][x] = Color.FromArgb(r, g, b);
                    }
                }
            }

            image.UnlockBits(imageToBitmapData);

            return colorMatrix;
        }

        public static Bitmap ColorMatrixToBitmap(Color[][] colorMatrix, int bytesPerPixel)
        {
            int colorMatrixHeight = colorMatrix.Length;
            int colorMatrixWidth = colorMatrix[0].Length;

            Bitmap colorMatrixToBitmap = new(colorMatrixWidth, colorMatrixHeight);

            Rectangle colorMatrixRect = new(0, 0, colorMatrixWidth, colorMatrixHeight);
            BitmapData bitmapData = colorMatrixToBitmap.LockBits(colorMatrixRect, ImageLockMode.WriteOnly,
                (bytesPerPixel == 3) ? PixelFormat.Format24bppRgb : PixelFormat.Format32bppArgb);

            for (int y = 0; y < colorMatrixHeight; y++)
            {
                IntPtr rowPointer = bitmapData.Scan0 + (y * bitmapData.Stride);

                byte[] pixelsInRow = new byte[colorMatrixWidth * bytesPerPixel];

                for (int x = 0; x < colorMatrixWidth; x++)
                {
                    Color color = colorMatrix[y][x];

                    int pixelIndex = x * bytesPerPixel;

                    pixelsInRow[pixelIndex] = color.B;
                    pixelsInRow[pixelIndex + 1] = color.G;
                    pixelsInRow[pixelIndex + 2] = color.R;
                    if (bytesPerPixel == 4) pixelsInRow[pixelIndex + 3] = color.A;
                }

                Marshal.Copy(pixelsInRow, 0, rowPointer, pixelsInRow.Length);
            }

            colorMatrixToBitmap.UnlockBits(bitmapData);

            return colorMatrixToBitmap;
        }
    }
}
