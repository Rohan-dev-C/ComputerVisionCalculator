using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;


namespace PongIRL
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        VideoCapture capture = new VideoCapture(0);
        Mat cameraImage = new Mat();
        int XSpeed = 5;
        int YSpeed = 5;
        int currentTick = 0; 
        Mat GetFrame()
        {
            using Mat currentFrame = capture.QueryFrame();
            if (currentFrame == null) return null;
            using Mat output = currentFrame.Clone();
            CvInvoke.Flip(output, output, FlipType.Horizontal);
            return output; 
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            BallBox.Image = CvInvoke.Imread("ball.png");
        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }
        void UpdateBall()
        {
            if (BallBox.Location.X + BallBox.Width >= CameraBox.Location.X + CameraBox.Width)
            {
                XSpeed = -XSpeed;
            }
            if (BallBox.Location.Y + BallBox.Height >= CameraBox.Location.Y + CameraBox.Height)
            {
                YSpeed = -YSpeed;
            }
            if (BallBox.Location.X <= CameraBox.Location.X)
            {
                XSpeed = -XSpeed;
            }
            if (BallBox.Location.Y <= CameraBox.Location.Y)
            {
                YSpeed = -YSpeed;
            }
            BallBox.Location = new Point(BallBox.Location.X + XSpeed, BallBox.Location.Y + YSpeed);
        }
        void UpdateBall(Rectangle rect)
        {
            Rectangle BallBoxRect = new Rectangle(BallBox.Location.X, 
                BallBox.Location.Y,
                BallBox.Width, 
                BallBox.Height); 
            if(BallBoxRect.IntersectsWith(rect)) // && BallBoxRect.X > rect.X + rect.Width)
            {
                XSpeed = -XSpeed;
            }
            else if (BallBoxRect.IntersectsWith(rect) && BallBoxRect.X + BallBoxRect.Width < rect.X)
            {
                XSpeed = -Math.Abs(XSpeed);
            }
        }
        VectorOfPoint FindLargestContour(VectorOfVectorOfPoint contours)
        {
            if(contours.Size == 0)
            {
                return null; 
            }
            VectorOfPoint largest = new VectorOfPoint();
            largest = contours[0];
            for (int i = 0; i < contours.Size; i++)
            {
                if (contours[i].Size > largest.Size)
                {
                    largest = contours[i]; 
                }
            }
            return largest; 
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            cameraImage = capture.QueryFrame();
            UpdateBall(); 
            CvInvoke.Flip(cameraImage, cameraImage, FlipType.Horizontal); 
            Mat mask = cameraImage.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            Mat mask2 = mask.Clone(); 
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar(170, 130, 73), (ScalarArray)new MCvScalar(180, 255, 255), mask);
            CvInvoke.InRange(mask2, (ScalarArray)new MCvScalar(0, 130, 73), (ScalarArray)new MCvScalar(60, 255, 255), mask2);
            CvInvoke.BitwiseOr(mask, mask2, mask); 
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            VectorOfPoint contour = FindLargestContour(contours);
            if (contour == null) return; 
            Rectangle rect = CvInvoke.BoundingRectangle(contour);
            CvInvoke.Rectangle(cameraImage, rect, new MCvScalar(255, 0, 0), 5);
            if (currentTick >= 10)
            {
                currentTick = 0; 
                UpdateBall(rect);
            }
            currentTick++; 
            CameraBox.Image = cameraImage;
        }
    }
}
