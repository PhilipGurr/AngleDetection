using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AForgeAngleDetection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            Assembly assembly = this.GetType().Assembly;
            Bitmap image = Properties.Resources.triangle;

            ProcessImage(image);
        }

        private void ProcessImage(Bitmap bitmap)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite, bitmap.PixelFormat);

            ColorFiltering colorFilter = new ColorFiltering();

            colorFilter.Red = new IntRange(240, 260);
            colorFilter.Green = new IntRange(240, 260);
            colorFilter.Blue = new IntRange(240, 260);
            colorFilter.FillOutsideRange = false;

            colorFilter.ApplyInPlace(bitmapData);

            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;

            blobCounter.ProcessImage(bitmapData);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            bitmap.UnlockBits(bitmapData);

            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            Graphics g = Graphics.FromImage(bitmap);
            Pen bluePen = new Pen(Color.Blue, 5);
            foreach (Blob blob in blobs)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);

                List<IntPoint> corners;
                if(shapeChecker.IsTriangle(edgePoints, out corners))
                {
                    var subType = shapeChecker.CheckPolygonSubType(corners);
                    Console.WriteLine(subType);
                    foreach(var corner in corners)
                    {
                        Console.WriteLine("X: " + corner.X + " Y: " + corner.Y);
                    }
                    g.DrawPolygon(bluePen, ToPointsArray(corners));

                    var point1 = corners[1];
                    var point2 = corners[2];

                    Line verticalLine = Line.FromPoints(point1, point2);
                    Line horizontalLine = Line.FromPoints(corners[0], corners[2]);
                    double aforgeAngle = verticalLine.GetAngleBetweenLines(horizontalLine);
                    //double angle = Math.Atan2(point2.Y - point1.Y, point2.X - point1.X) / Math.PI * 180.0; //ols
                    labelAngle.Text = "Angle: " + aforgeAngle;
                }
            }

            pictureBox.Image = bitmap;
            bluePen.Dispose();
            g.Dispose();
        }

        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }
    }
}
