using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

using System.IO;

namespace HoldDetector
{


    public partial class Form1 : Form
    {

        WallRecognition wallRecgSys = new WallRecognition( /*trackBarMinSize.Value*/ );

        Image<Bgr, byte> capturedImage;
        

        // Buffers
        List<Point[]> vertices = new List<Point[]>();
        List<PolyFromTris> polysTriang; // Triangles - inner points pair list

        Gray cannyThreshold = new Gray(180);
        Gray cannyThresholdLinking = new Gray(120);
        Gray circleAccThreshold = new Gray(120);

        // Forms
        AboutBoxHD about = new AboutBoxHD(); // About dialog

        public Form1()
        {
            InitializeComponent();
        }

        /*******************************************************
        * detectAndDraw()
        * 
        * Calculates background avrege color in LUV, backgr based on binary segmentation
        * 
        * input: /
        * output: /
        * ******************************************************/
        private void detectAndDraw()
        {
            try
            {
                // LUV seg
                if (checkBoxUseit.Checked == true)
                {
                    Image<Lab, Byte> labImg = capturedImage.Convert<Lab, Byte>();
                    Image<Gray, Byte> grImg = labImg[Int32.Parse(textBoxChannel.Text)];
                    grImg = grImg.ThresholdBinary(new Gray(trackBarThresh.Value), new Gray(255.0f));//.Not();
                    capturedImage = capturedImage.Copy(grImg);
                    // capturedImage = capturedImage.Not();
                    //    capturedImage = capturedImage.Erode(10);
                    imageBox1.Image = capturedImage.Resize(imageBox1.Width, imageBox1.Height);
                }


                // clear contures list
                vertices.Clear();

                //   if (konture.Area > 30 && konture.Area < 150 && trenutna.Total < 500)

                // call detect function
                polysTriang = wallRecgSys.detectPolysFromContures(capturedImage, 100, 20.0f, 250.0f, (float)trackBarThresh.Value, checkBoxFilter.Checked, trackBarDilate.Value , checkBoxBack.Checked,ref imageBox1); //detectPolysFromContures(grayImage, 100, 20.0);

                // print number of recognised polys
                labelNumHolds.Text = polysTriang.Count.ToString();

                // show image with marked polys
             //   imageBox1.Image = wallRecgSys.drawPolys(capturedImage, vertices, Color.Red).Resize(imageBox1.Size.Width, imageBox1.Size.Height); //capturedImage.Resize(imageBox1.Size.Width, imageBox1.Size.Height);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Error, is image empty?");
            }
        }


        private void saveData()
        {
            Image<Gray, Byte> maskImg = capturedImage.Convert<Gray, Byte>().PyrDown().PyrUp().ThresholdBinaryInv(new Gray(128.0f), new Gray(255.0f));
            Bgr back = capturedImage.GetAverage(maskImg);

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
            saveDialog.Filter = "(*.WALL)|*.wall|All files (*.*)|*.*";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    wallRecgSys.saveGeometry(polysTriang, saveDialog.FileName, capturedImage.Width, capturedImage.Height, back);
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Napaka, slika ni bila zajeta.");
                }
            }
        }





        private void ReleaseData()
        {

        }

        private void loadImage()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
            openDialog.Filter = "(*.jpg)|*.jpg|All files (*.*)|*.*";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    capturedImage = new Image<Bgr, Byte>(openDialog.FileName);
                    imageBox1.Image = capturedImage.Resize(imageBox1.Width, imageBox1.Height);
                }
                catch (NullReferenceException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void process()
        {

        }



        /// <summary>
        /// Load image function call, button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            loadImage();
        }

        /// <summary>
        /// Engages detection method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
           // process();
            detectAndDraw();

        }

        /// <summary>
        /// Wtf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        /// <summary>
        /// Load image function call, menu option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadImage();
        }

        /// <summary>
        /// Save polys to file menu option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePolysToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveData();
        }

        /// <summary>
        /// Clear detected stuff menu option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearDetectedObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Exit application menu option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Invokes about box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about.Show();
        }

        private void detectStuffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detectAndDraw();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            saveData();
        }

    }
}
