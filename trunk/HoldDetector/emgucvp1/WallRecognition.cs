using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.UI;

using System.IO;

using System.Windows.Forms; // for msgbox. can be removed :) 
 
namespace HoldDetector
{
    class WallRecognition
    {
        // bufferji za detektirane oblike
        List<Point[]> polys = new List<Point[]>();
        List<Bgr> avgClrs; // = new List<Bgr>();

        // meje
        Gray cannyThreshold = new Gray(180);
        Gray cannyThresholdLinking = new Gray(120);
        Gray circleAccThreshold = new Gray(120);

        // Hold triangulation
        HoldTriangulation triangulator = new HoldTriangulation();

        // Random
        Random random = new Random();


        /// <summary>
        /// Draws given polys on image
        /// </summary>
        /// <param name="origImage">Src image</param>
        /// <param name="polys">List of polys</param>
        /// <param name="clr">Color</param>
        /// <returns></returns>
        public Image<Bgr, Byte> drawPolys(Image<Bgr, Byte> origImage, List<Point[]> polys, Color clr)
        {
            Image<Bgr, Byte> newImage = origImage;

            foreach (Point[] ply in polys)
                newImage.DrawPolyline(ply, true, new Bgr(clr), 2);

            return newImage;
        }

        /// <summary>
        /// Caluclates avrege color of polygon
        /// </summary>
        /// <param name="refImg">Image where polygin is appearing</param>
        /// <param name="ply">Polygon vertices</param>
        /// <returns></returns>
        private Bgr calcPolyBgrAvrege(Image<Bgr, Byte> refImg, /*Point[] ply*/ Rectangle rect)
        {
            // 1. calc poly's mins and maxs
          //  float minx = 0.0f, maxx = 0.0f, miny = 0.0f, maxy = 0.0f;
           // int minxi = 0, maxxi = 0, minyi = 0, maxyi = 0;

         /*   Point currMin = ply[0], currMax = ply[0];

            for (int i = 0; i < ply.Length; i++)
            {
                if (ply[i].X > currMax.X || ply[i].Y > currMax.Y)
                    currMax = ply[i];

                if (ply[i].X < currMin.X || ply[i].Y < currMin.Y)
                    currMin = ply[i];
            }*/

            // 2 calc avg color

            const int padding = 3;

            int div = 0;  // (currMax.X - currMin.X) * (currMax.Y - currMin.Y);
            Bgr avgColor = new Bgr();

            for (int i = rect.X+padding; i < rect.Right-padding; i++)
            {
                for (int j = rect.Y + padding; j < rect.Bottom - padding; j++)
                {
                    Bgr curr = refImg[j, i];

                 //   refImg.Draw(rect, new Bgr(Color.Blue), 2); 
                  //  double avgLenSq = avgColor.Red * avgColor.Red + avgColor.Green * avgColor.Green + avgColor.Blue * avgColor.Blue;
                   // double currLenSq = curr.Red * curr.Red + curr.Green * curr.Green + curr.Blue * curr.Blue;

                   // if (Math.Abs(avgLenSq - currLenSq) < 10000) // 400 ... 20*2*-0
                  //  {

                        //  avgColor += refImg[j, i];
                        avgColor.Red += curr.Red;
                        avgColor.Green += curr.Green;
                        avgColor.Blue += curr.Blue;

                        div++;
                  //  }
                    
                }
            }

            if (div == 0) div = 1;

            avgColor.Red /= div;
            avgColor.Green /= div;
            avgColor.Blue /= div;

            return avgColor;
        }


        /// <summary>
        /// Calculates minimal and max. acceptable area based on img size. 
        /// </summary>
        /// <param name="refImg">Image</param>
        /// <param name="min">Minimal area, reference</param>
        /// <param name="max">Max. area, reference</param>
        /// <param name="maxPerim">Max. perimeter, reference</param>
        private void calcMinMaxArea(Image<Bgr, Byte> refImg, ref float min, ref float max, ref float maxPerim)
        {
            int imgArea = refImg.Width * refImg.Height;
            int perim = 2*refImg.Width + 2*refImg.Height;

            // Emperic values
            // max ... 1/180
            // min ... 1/9000

            min = (float)imgArea * (1.0f / 4000.0f);
            max = (float)imgArea * (1.0f / 220.0f);

            maxPerim = (float)perim * (1.0f / 24.0f);
        }


        /// <summary>
        /// Method detects polys from contoures, claculates poly's convex hull and triangulates it.
        /// </summary>
        /// <param name="refImg">Image</param>
        /// <param name="maxVertices">Max. vertices in poly</param>
        /// <param name="minArea">Minimal area for poly</param>
        /// <param name="maxArea">Max. area for poly</param>
        /// <param name="threshold">Threshold for segmentation</param>
        /// <param name="imageBox">Imagebox (for dumping a result)</param>
        /// <returns></returns>
        public List<PolyFromTris> detectPolysFromContures(Image<Bgr, Byte> refImg, int maxVertices, double minArea, double maxArea, float threshold, bool filter, int dilate, bool back, ref ImageBox imageBox)
        {
            // Lists, for storing data
            List<Point[]> polys = new List<Point[]>(); // Polys list
            List<Point[]> chull = new List<Point[]>(); // CONV HULL
            avgClrs = new List<Bgr>();
            List<PolyFromTris> polysTriang = new List<PolyFromTris>();

            try
            {
                Bgr backc = new Bgr(threshold, threshold, threshold);
                if (back)
                {
                    Image<Gray, Byte> maskImg = refImg.Convert<Gray, Byte>().PyrDown().PyrUp().ThresholdBinaryInv(new Gray(128.0f), new Gray(255.0f));
                    backc = refImg.GetAverage(maskImg);
                }

               // Image<Bgr, Byte> clrSeg = refImg.ThresholdBinary (back, new Bgr(120.0f, 120.0f, 120.0f));
               // imageBox.Image = clrSeg;
                // Canny
                Image<Bgr, Byte> cannyEdges = refImg.PyrUp().PyrDown().Canny(backc, new Bgr(120.0f, 120.0f, 120.0f));

                cannyEdges = cannyEdges.Dilate(dilate); // To stress borders, improves contoures detection

       
                // Conture detection & Triangulation
                using (MemStorage shramba = new MemStorage())
                    for (Contour<Point> konture = cannyEdges.Convert<Gray, Byte>().FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_TC89_L1, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, new MemStorage()); konture != null; konture = konture.HNext)
                    {
                     //   Contour<Point> trenutna = konture.ApproxPoly(konture.Perimeter * 0.005, shramba);
                        Seq<Point> convexHull = konture.ApproxPoly(konture.Perimeter * 0.005, shramba).GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);    //konture.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);

                        // Calculate max. nad min. area of valid polys (holds)
                        float min = 0.0f, max = 0.0f, perim = 0.0f;
                        calcMinMaxArea(refImg, ref min, ref max, ref perim);


                        //      if (konture.Area > minArea && konture.Area < maxArea && trenutna.Total < maxVertices)
                        
                        if ( (convexHull.Area > min && convexHull.Area < max && convexHull.Perimeter < perim && convexHull.Total < maxVertices) || !filter)
                        {
                        //    if ((convexHull.Area / convexHull.Perimeter) >5.5)
                        //    {
                                Point[] conHull = convexHull.ToArray(); //Array.ConvertAll(convexHull.ToArray(), new Converter<Point, PointF>(PointToPointF));

                                chull.Add(conHull);
                                polysTriang.Add(  triangulator.triangulatePoly(convexHull, 4, refImg.Width, refImg.Height));
                                avgClrs.Add(calcPolyBgrAvrege(refImg, convexHull.BoundingRectangle));

                                // Draw detected holds and triangulation
                                imageBox.Image = drawPolys(refImg, chull, Color.DarkRed);
                                triangulator.drawTris(refImg, polysTriang[polysTriang.Count - 1].tris, ref imageBox);
                        //    }
                            
                        }
                    }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Error, please, input an image.");
            }

            // Return triangulated polys
            return polysTriang;
        }

        /*******************************************************
         * luvSegmentation()
         * 
         * LUV color model based picture segmentation
         * 
         * input: original image(Bgr), min values vector, max values vector
         * output: new modified image
         * ******************************************************/
        public Image<Bgr, Byte> luvSegmentation(Image<Bgr, Byte> srcImg, mVector3 min, mVector3 max)
        {
            Image<Luv, float> lImg = srcImg.Convert<Luv, float>();
            Image<Bgr, Byte> newImg = srcImg;

            for (int i = 0; i < lImg.Width; i++)
            {
                for (int j = 0; j < lImg.Height; j++)
                {
                    if (
                        ((lImg[j, i].X >= max.x) || (lImg[j, i].X <= min.x)) ||
                        ((lImg[j, i].Y >= max.y) || (lImg[j, i].Y <= min.y)) ||
                        ((lImg[j, i].Z >= max.z) || (lImg[j, i].Z <= min.z))
                        )
                    {

                        /*            if (
                ((lImg[j, i].X >= (border.X + tolerance)) || (lImg[j, i].X <= (border.X - tolerance))) ||
                ((lImg[j, i].Y >= (border.Y + toleranc/)) || (lImg[j, i].Y <= (border.Y - tolerance))) ||
                ((lImg[j, i].Z >= (border.Z+tolerance)) || (lImg[j, i].Z <= (border.Z - tolerance)))
                )
            {*/


                        newImg[j, i] = new Bgr(255.0f, 255.0f, 255.0f);
                    }
                }
            }

            return newImg;
        }



        /*******************************************************
         * luvSegmentation()
         * 
         * LUV color model based picture segmentation
         * 
         * input: original image(Bgr), min values vector, max values vector
         * output: new modified image
         * ******************************************************/
        public Image<Bgr, Byte> labAndBinSegmentation(Image<Bgr, Byte> srcImg, double threshold, int channel)
        {
            Image<Lab, Byte> lImg = srcImg.Convert<Lab, Byte>();

            Image<Bgr, Byte> newImg = srcImg;
            Image<Gray, Byte> grayImg = lImg[channel];

            grayImg = grayImg.ThresholdBinary(new Gray(threshold), new Gray(255)).Not();

           // lImg[channel] = grayImg;

          //  newImg[channel] = grayImg;

           // this.New

            for (int i = 0; i < lImg.Width; i++)
            {
                for (int j = 0; j < lImg.Height; j++)
                {
                    if (grayImg[j, i].Intensity == 0)
                        newImg[j, i] = new Bgr(255.0f, 255.0f, 255.0f);
                    
                }
            }



            return newImg; // lImg.Convert<Bgr, Byte>(); ////grayImg.Convert<Bgr, Byte>(); //srcImg; //lImg.Convert<Bgr, Byte>();
        }

        /*******************************************************
        * calcBackgroundAvg(()
        * 
        * Calculates background avrege color in LUV, backgr based on binary segmentation
        * 
        * input: original image (Bgr), min values vector, threshold for segmentation
        * output: image avrege color in LUV
        * ******************************************************/
        //   private Image<Gray, Byte> toBin
        // ne pozabi na invert
        private Luv calcBackgroundAvg(Image<Bgr, Byte> refImage, float thresh, ref ImageBox imageBox)
        {
            // to binary
            Image<Gray, Byte> binImage = refImage.Convert<Gray, Byte>().PyrDown().PyrUp().ThresholdBinary(new Gray(thresh), new Gray(255));

            // to luv
            Image<Luv, float> luvImage = refImage.Convert<Luv, float>();

            // show bin img
            if (imageBox != null)
                imageBox.Image = binImage;

            int pixCount = 0; // stevec pikslov
            double ch1 = 0.0f;
            double ch2 = 0.0f;
            double ch3 = 0.0f;

            for (int i = 0; i < binImage.Width; i++)
            {
                for (int j = 0; j < binImage.Height; j++)
                {
                    // piksel predstavlja ozadje? (bel)
                    if (binImage[j, i].Intensity == 255)
                    {
                        pixCount++;

                        ch1 += luvImage[j, i].X;
                        ch2 += luvImage[j, i].X;
                        ch3 += luvImage[j, i].X;
                    }
                }
            }

            ch1 /= (double)pixCount;
            ch2 /= (double)pixCount;
            ch3 /= (double)pixCount;

            MessageBox.Show(ch1.ToString() + " " + ch2.ToString() + " " + ch3.ToString());

            return new Luv(ch1, ch2, ch3);

        }

 

        /// <summary>
        /// Returns points string ready to be written directly into file (normalised and with appropriate z-coord)
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="innPts"></param>
        /// <returns></returns>
        private string getNormalisedPointString(PointF pt, int w, int h, List<PointF> innPts)
        {
            int l = w > h ? w : h;
            if (innPts.Contains(pt)) // The point is inner point!
            {
                return (pt.X / (float)l).ToString() + " " + (pt.Y / (float)l).ToString() + " 1.0"; //" " + ((float)random.Next(2, 10) / 10.0f).ToString(); //1.0";  // 1.0 ... default z value, for the moment
            }
            else
                return (pt.X / (float)l).ToString() + " " + (pt.Y / (float)l).ToString() + " 0.0";
        }



        /// <summary>
        /// Dumps contures (holds) data into a file
        /// </summary>
        /// <param name="trisPoly"></param>
        /// <param name="fileName"></param>
        /// <param name="imgW"></param>
        /// <param name="imgH"></param>
        /// <param name="back"></param>
        public void saveGeometry(List<PolyFromTris> trisPoly, string fileName, int imgW, int imgH, Bgr back)
        {
            float l = (float)imgW > imgH ? imgW : imgH;

            try
            {
                StreamWriter writer = new StreamWriter(fileName);
                writer.WriteLine("0.0 0.0 0.0"); // p1
                writer.WriteLine(((float)imgW/l).ToString() + " 0.0 0.0" ); // p2
                writer.WriteLine(((float)imgW / l).ToString() + " " + ((float)imgH / l).ToString() + " 0.0"); // p3
                writer.WriteLine("0.0 " + ((float)imgH / l).ToString() + " 0.0"); // p4
                writer.WriteLine("###");

                // Background color
                writer.WriteLine("C:" + back.Red.ToString() + " " + back.Green.ToString() + " " + back.Blue.ToString()); //255.0 0.0 0.0");

                int i = 0;
                foreach (PolyFromTris poly in trisPoly)
                {
                    // poisci ponavljajoce tocke
                    List<PointF> innerPoints = poly.innerPoints; //null; // = findInnerPoints(poly);

                    foreach (Triangle2DF tri in poly.tris)
                    {
                        writer.WriteLine(getNormalisedPointString(tri.V0, imgW, imgH, innerPoints));
                        writer.WriteLine(getNormalisedPointString(tri.V1, imgW, imgH, innerPoints));
                        writer.WriteLine(getNormalisedPointString(tri.V2, imgW, imgH, innerPoints));
                        writer.WriteLine("#");
                        

                        /*foreach (Point p in poly)
                        {
                            float x = (float)p.X / (float)imgW; // get normalised xcoord
                            float y = (float)p.Y / (float)imgH;
                            writer.WriteLine(  x.ToString() + " " + y.ToString());
                            //    MessageBox.Show(p.ToString());
                        }*/

                    }

                
                    writer.WriteLine("C:" + avgClrs[i].Red.ToString() + " " + avgClrs[i].Green.ToString() + " " + avgClrs[i].Blue.ToString());
                    writer.WriteLine("#####");
                    i++;
                }
                //writer.WriteLine( ply[0].ToString  );
                //refImage.DrawPolyline(ply, true, new Bgr(Color.Chocolate), 3);



                /*
                foreach (Point[] ply in polys)
                {
                    
                    foreach (Point p in ply)
                    {
                        float x = (float)p.X / (float)imgW; // get normalised xcoord
                        float y = (float)p.Y / (float)imgH;
                        writer.WriteLine(  x.ToString() + " " + y.ToString());
                        //    MessageBox.Show(p.ToString());
                    }

                    writer.WriteLine("C:" + avgClrs[i].Red.ToString() + " " + avgClrs[i].Green.ToString() + " " + avgClrs[i].Blue.ToString());

                    writer.WriteLine("#####");
                    i++;
                }
                */

                writer.Close();
            }
            catch (IOException)
            {
                MessageBox.Show("IO error!");
            }

            MessageBox.Show("Geometry dumped to " + fileName);
        }


        ///
        // __ENDOF_CLASS
        ///
    }
}
