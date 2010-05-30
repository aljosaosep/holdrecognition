namespace HoldDetector
{
    partial class ColorChannelView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageBoxRed = new Emgu.CV.UI.ImageBox();
            this.imageBoxGreen = new Emgu.CV.UI.ImageBox();
            this.imageBoxBlue = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxBlue)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBoxRed
            // 
            this.imageBoxRed.BackColor = System.Drawing.Color.Red;
            this.imageBoxRed.Location = new System.Drawing.Point(13, 13);
            this.imageBoxRed.Name = "imageBoxRed";
            this.imageBoxRed.Size = new System.Drawing.Size(600, 600);
            this.imageBoxRed.TabIndex = 2;
            this.imageBoxRed.TabStop = false;
            // 
            // imageBoxGreen
            // 
            this.imageBoxGreen.BackColor = System.Drawing.Color.Green;
            this.imageBoxGreen.Location = new System.Drawing.Point(619, 13);
            this.imageBoxGreen.Name = "imageBoxGreen";
            this.imageBoxGreen.Size = new System.Drawing.Size(600, 600);
            this.imageBoxGreen.TabIndex = 2;
            this.imageBoxGreen.TabStop = false;
            // 
            // imageBoxBlue
            // 
            this.imageBoxBlue.BackColor = System.Drawing.Color.Blue;
            this.imageBoxBlue.Location = new System.Drawing.Point(1227, 13);
            this.imageBoxBlue.Name = "imageBoxBlue";
            this.imageBoxBlue.Size = new System.Drawing.Size(600, 600);
            this.imageBoxBlue.TabIndex = 2;
            this.imageBoxBlue.TabStop = false;
            // 
            // ColorChannelView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1839, 635);
            this.Controls.Add(this.imageBoxBlue);
            this.Controls.Add(this.imageBoxGreen);
            this.Controls.Add(this.imageBoxRed);
            this.Name = "ColorChannelView";
            this.Text = "ColorChannelView";
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxBlue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox imageBoxRed;
        private Emgu.CV.UI.ImageBox imageBoxGreen;
        private Emgu.CV.UI.ImageBox imageBoxBlue;
    }
}