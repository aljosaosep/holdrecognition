using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV.Structure;

namespace HoldDetector
{
    class PolyFromTris
    {

        // Data
        public Triangle2DF[] tris;
        public List<PointF> innerPoints;


        /// <summary>
        /// Tris-List-Setter Constructor
        /// </summary>
        public PolyFromTris()
        {
            //tris = new Triangle2DF();
            innerPoints = new List<PointF>();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
    /*    public PolyFromTris(Triangle2DF[] t)
        {
            //tris = new Triangle2DF();
            innerPoints = new List<PointF>();
            this.tris = t;
        }*/

        /// <summary>
        /// Sets triangles structure
        /// </summary>
        public void setTris(Triangle2DF[] t)
        {
            this.tris = t;
        }

        /// <summary>
        /// Sets inner points list
        /// </summary>
        public void setInnerPoints(List<PointF> ipts)
        {
            this.innerPoints = ipts;
        }

        /// <summary>
        /// Adds an inner point to inner points list
        /// </summary>
        public void addInnerPoint(PointF p)
        {
            innerPoints.Add(p);
        }
    }

    
}
