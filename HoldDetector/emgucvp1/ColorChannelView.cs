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

namespace HoldDetector
{
    public partial class ColorChannelView : Form
    {
        public ColorChannelView()
        {
            InitializeComponent();
        }

        public void setParam1Image(Image <Gray, Byte> img)
        {
            imageBoxRed.Image = img.Resize(imageBoxRed.Width, imageBoxRed.Height);
        }

        public void setParam2Image(Image<Gray, Byte> img)
        {
            imageBoxGreen.Image = img.Resize(imageBoxGreen.Width, imageBoxGreen.Height);
        }

        public void setParam3Image(Image<Gray, Byte> img)
        {
            imageBoxBlue.Image = img.Resize(imageBoxBlue.Width, imageBoxBlue.Height);
        }


    }
}
