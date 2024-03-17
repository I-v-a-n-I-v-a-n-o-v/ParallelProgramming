namespace homework
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonSelectImage = new System.Windows.Forms.Button();
            this.pictureBoxSelectedImage = new System.Windows.Forms.PictureBox();
            this.pictureBoxScaledImage = new System.Windows.Forms.PictureBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonParallel = new System.Windows.Forms.Button();
            this.buttonConsequential = new System.Windows.Forms.Button();
            this.textBoxScaleFactor = new System.Windows.Forms.TextBox();
            this.scaleLabel = new System.Windows.Forms.Label();
            this.lableScaleTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScaledImage)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSelectImage
            // 
            this.buttonSelectImage.AutoEllipsis = true;
            this.buttonSelectImage.FlatAppearance.BorderSize = 10;
            this.buttonSelectImage.Location = new System.Drawing.Point(12, 714);
            this.buttonSelectImage.Name = "buttonSelectImage";
            this.buttonSelectImage.Size = new System.Drawing.Size(115, 23);
            this.buttonSelectImage.TabIndex = 0;
            this.buttonSelectImage.Text = "Choose image";
            this.buttonSelectImage.UseVisualStyleBackColor = true;
            this.buttonSelectImage.Click += new System.EventHandler(this.openDialogListener);
            // 
            // pictureBoxSelectedImage
            // 
            this.pictureBoxSelectedImage.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxSelectedImage.Name = "pictureBoxSelectedImage";
            this.pictureBoxSelectedImage.Size = new System.Drawing.Size(712, 655);
            this.pictureBoxSelectedImage.TabIndex = 1;
            this.pictureBoxSelectedImage.TabStop = false;
            // 
            // pictureBoxScaledImage
            // 
            this.pictureBoxScaledImage.Location = new System.Drawing.Point(745, 12);
            this.pictureBoxScaledImage.Name = "pictureBoxScaledImage";
            this.pictureBoxScaledImage.Size = new System.Drawing.Size(695, 655);
            this.pictureBoxScaledImage.TabIndex = 2;
            this.pictureBoxScaledImage.TabStop = false;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(1365, 714);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save Image";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonParallel
            // 
            this.buttonParallel.Location = new System.Drawing.Point(584, 718);
            this.buttonParallel.Name = "buttonParallel";
            this.buttonParallel.Size = new System.Drawing.Size(140, 23);
            this.buttonParallel.TabIndex = 4;
            this.buttonParallel.Text = "Downsize parallel";
            this.buttonParallel.UseVisualStyleBackColor = true;
            this.buttonParallel.Click += new System.EventHandler(this.buttonParallel_Click);
            // 
            // buttonConsequential
            // 
            this.buttonConsequential.Location = new System.Drawing.Point(745, 718);
            this.buttonConsequential.Name = "buttonConsequential";
            this.buttonConsequential.Size = new System.Drawing.Size(145, 23);
            this.buttonConsequential.TabIndex = 5;
            this.buttonConsequential.Text = "Downsize consequential";
            this.buttonConsequential.UseVisualStyleBackColor = true;
            this.buttonConsequential.Click += new System.EventHandler(this.buttonConsequential_Click);
            // 
            // textBoxScaleFactor
            // 
            this.textBoxScaleFactor.Location = new System.Drawing.Point(268, 714);
            this.textBoxScaleFactor.Name = "textBoxScaleFactor";
            this.textBoxScaleFactor.Size = new System.Drawing.Size(72, 23);
            this.textBoxScaleFactor.TabIndex = 6;
            // 
            // scaleLabel
            // 
            this.scaleLabel.AutoSize = true;
            this.scaleLabel.Location = new System.Drawing.Point(133, 718);
            this.scaleLabel.Name = "scaleLabel";
            this.scaleLabel.Size = new System.Drawing.Size(129, 15);
            this.scaleLabel.TabIndex = 7;
            this.scaleLabel.Text = "Downscale Factor in %:";
            // 
            // lableScaleTime
            // 
            this.lableScaleTime.AutoSize = true;
            this.lableScaleTime.Location = new System.Drawing.Point(933, 722);
            this.lableScaleTime.Name = "lableScaleTime";
            this.lableScaleTime.Size = new System.Drawing.Size(0, 15);
            this.lableScaleTime.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1452, 749);
            this.Controls.Add(this.lableScaleTime);
            this.Controls.Add(this.scaleLabel);
            this.Controls.Add(this.textBoxScaleFactor);
            this.Controls.Add(this.buttonConsequential);
            this.Controls.Add(this.buttonParallel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.pictureBoxScaledImage);
            this.Controls.Add(this.pictureBoxSelectedImage);
            this.Controls.Add(this.buttonSelectImage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Downsizer";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSelectedImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScaledImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonSelectImage;
        private PictureBox pictureBoxSelectedImage;
        private PictureBox pictureBoxScaledImage;
        private Button buttonSave;
        private Button buttonParallel;
        private Button buttonConsequential;
        private TextBox textBoxScaleFactor;
        private Label scaleLabel;
        private Label lableScaleTime;
    }
}