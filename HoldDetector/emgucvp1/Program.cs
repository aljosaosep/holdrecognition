using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Drawing;

namespace HoldDetector
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           /* PointF p = new PointF(2.0f, 2.0f);
            List<PointF> list = new List<PointF>();

            list.Add(p);

            list. = 16.0f;

            Console.WriteLine(p.ToString());*/


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
