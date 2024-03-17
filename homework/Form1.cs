using homework.utils;
using System.Collections;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace homework
{
    /*
     * Recources:
     * http://www.vcskicks.com/image-to-byte.php
     * http://www.java2s.com
     * 
     * 
     */
    public partial class Form1 : Form
    {

        Bitmap selectedImage;
        Bitmap scaledImage;
        private Color[][] selectedImageToColorMatrix;
        private Color[][] scaledImageToColorMatrix;
        private int bytesPerPixel;
        private Stopwatch stopwatch;

        public Form1()
        {
            InitializeComponent();
            stopwatch = new Stopwatch();
            //lableScaleTime.Visible = false;
            buttonParallel.Enabled = false;
            buttonConsequential.Enabled = false;
            buttonSave.Enabled = false;
        }

        private void openDialogListener(object sender, EventArgs e)
        {
            lableScaleTime.Text = string.Empty;
            pictureBoxScaledImage.Image = null;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PNG Image |*.jpg;*.png;*.jpeg",
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Please Select an Image:"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedImage = new Bitmap(openFileDialog.FileName);
                pictureBoxSelectedImage.Image = selectedImage;
                buttonParallel.Enabled = true;
                buttonConsequential.Enabled = true;

                //scaledImage = ImageUtils.DownsizeImageInParallel(selectedImage, 0.10f);


                /*//Downsize selected image Bilinearly in Parallel
                selectedImageToColorMatrix = ImageUtils.BitmapToColorMatrix(selectedImage);
                bytesPerPixel = Image.GetPixelFormatSize(selectedImage.PixelFormat) / 8;
                scaledImageToColorMatrix = ImageUtils.DownsizeImageBilinearlyInParallel(selectedImageToColorMatrix, 0.1f, bytesPerPixel);
                scaledImage = ImageUtils.ColorMatrixToBitmap(scaledImageToColorMatrix, bytesPerPixel);*/

            }

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (scaledImage != null)
                {
                    scaledImage.Save(saveFileDialog.FileName, ImageFormat.Png);
                }
            }
        }

        private void buttonParallel_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBoxScaleFactor.Text, out double downscaleFactor) || downscaleFactor <= 0 || downscaleFactor > 100)
            {
                MessageBox.Show("Please enter a valid scale factor!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            stopwatch.Restart();
            scaledImage = ImageUtils.DownsizeImageInParallel(selectedImage, (float)downscaleFactor / 100);
            stopwatch.Stop();
            pictureBoxScaledImage.Image = scaledImage;
            lableScaleTime.Text = stopwatch.ElapsedMilliseconds.ToString() + " ms";
        }

        private void buttonConsequential_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBoxScaleFactor.Text, out double downscaleFactor) || downscaleFactor <= 0 || downscaleFactor > 100)
            {
                MessageBox.Show("Please enter a valid scale factor!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            stopwatch.Restart();
            scaledImage = ImageUtils.ConsequentialDownscaleImage(selectedImage, (float)downscaleFactor / 100);
            stopwatch.Stop();
            pictureBoxScaledImage.Image = scaledImage;
            lableScaleTime.Text = stopwatch.ElapsedMilliseconds.ToString() + " ms";
        }
    }
}