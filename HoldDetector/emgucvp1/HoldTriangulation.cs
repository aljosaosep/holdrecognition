using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

// Emgu CV
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.UI;


namespace HoldDetector
{
    class HoldTriangulation
    {
        // For random point generation
        Random random = new Random();

        /// <summary>
        /// Inserts numP points into the polygon
        /// </summary>
        /// <param name="poly">Eequence of points representing the polygon border</param>
        /// <param name="numP">Number of points to insert</param>
        void insertPoints(ref Seq<Point> poly, ref PolyFromTris triPoly, int numP, int iw, int ih)
        {
            ///////////////////////////////////////////// TODO: FIX !!!!!!
            Seq<PointF> testsek = new Seq<PointF>(new MemStorage());

            foreach (Point p in poly)
            {
                testsek.Insert(testsek.Total, new PointF((float)p.X, (float)p.Y));
            }

            Rectangle rect = testsek.BoundingRectangle;
            ////////////////////////////////////////


            //
            // Calculate BoundingBox to narrow random-inserting points area
          //  Rectangle rect = poly.BoundingRectangle;

            int maxIter = numP*500;

            while (numP > 0) // We want inside numP random points
            {
                if (maxIter-- <= 0) break;

                // Generate point inside BBox
                Point p = new Point(random.Next(rect.X, rect.X + rect.Width), random.Next(rect.Y, rect.Y+rect.Height));

                if (p.X > iw || p.Y > ih || p.Y < 0 || p.X<0) continue;

                if (poly.InContour(p) > 0) // If point is on the poly
                {

                    triPoly.addInnerPoint(new PointF((float)p.X, (float)p.Y));
                    poly.Insert(poly.Total, p); // Insert it to our list
                    numP--;
                }
 
            } 
        }


        /// <summary>
        /// Function inserts points into poly (numInsert points) and triangulates while set of points. 
        /// Result is lsit of triangles representing the poly.
        /// </summary>
        /// <param name="poly">Seq. of points representing the poly</param>
        /// <param name="numInsert">Number of points to insert into poly</param>
        /// <returns>Triangle's list</returns>
        public /*Triangle2DF[]*/ PolyFromTris triangulatePoly(Seq<Point> poly, int numInsert, int iw, int ih)
        {
            //Triangle2DF[] trisList; // Triangles list
            PolyFromTris trisPoly = new PolyFromTris();

            if (poly.Total + numInsert <= 24)
            {

                insertPoints(ref poly, ref trisPoly, numInsert, iw, ih); // Insert random points into the poly
            }

            //Array.ConvertAll(convexHull.ToArray(), new Converter<Point, PointF>(PointToPointF));

            using (PlanarSubdivision subdiv = new PlanarSubdivision(Array.ConvertAll(poly.ToArray(), new Converter<Point, PointF>(PointToPointF))))   //(poly.ToArray()))
            {
                 Console.WriteLine(" ply size: " + poly.Total.ToString());
                 //trisList = subdiv.GetDelaunayTriangles(); // Do triangulation
                 trisPoly.setTris(subdiv.GetDelaunayTriangles());
            }
            
            
            return trisPoly;
        }

        /// <summary>
        /// Draws trangulated poly
        /// </summary>
        /// <param name="img">Image we're drawing to</param>
        /// <param name="trisList">Triangles list</param>
        /// <param name="imgBox">Box we're inserting image to</param>
        public void drawTris(Image<Gray, Byte> img, Triangle2DF[] trisList, ref Emgu.CV.UI.ImageBox imgBox)
        {
            //Draw the Delaunay triangulation
            foreach (Triangle2DF tri in trisList)
            {
                img.Draw(tri, new Gray(128.0f), 2);
            }

            imgBox.Image = img;
        }

        /// <summary>
        /// Draws trangulated poly to colored Bgr image in green
        /// </summary>
        /// <param name="img">Image we're drawing to</param>
        /// <param name="trisList">Triangles list</param>
        /// <param name="imgBox">Box we're inserting image to</param>
        public void drawTris(Image<Bgr, Byte> img, Triangle2DF[] trisList, ref Emgu.CV.UI.ImageBox imgBox)
        {
            //Draw the Delaunay triangulation
            foreach (Triangle2DF tri in trisList)
            {
                img.Draw(tri, new Bgr(Color.DarkOliveGreen), 1);
            }

            imgBox.Image = img;
        }

        public static PointF PointToPointF(Point p)
        {
            return new PointF(((float)p.X), ((float)p.Y));
        }


        /*private void imageBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                currPolyLine.Insert(currPolyLine.Total, new PointF(e.X, e.Y));


                ++currPolyPoints;


                for (int i = 0; i < currPolyPoints; i++)
                {
                    currPolyLineInt[i] = Point.Round(currPolyLine[i]);
                }

                drawedImage.DrawPolyline(currPolyLineInt.Take(currPolyPoints - 1).ToArray(), false, new Gray(0.0f), 3);
                imageBox1.Image = drawedImage;
            }
            //   drawedImage[e.Y, e.X] = new Gray(0.0f);

        }


        private void button1_Click(object sender, EventArgs e)
        {
            delaunayTriangles = triangulatePoly(currPolyLine, 5);

            drawTris(drawedImage, delaunayTriangles, ref imageBox1);

        }*/
    }

}
