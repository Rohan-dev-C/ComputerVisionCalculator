using Emgu.CV;
using Emgu.CV.Util;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        enum BitwiseOperators
        { 
            And,
            Or,
            Not,
            XOr,
            Add,
            Subtract,
            Multiply,
            Divide,
            AddWeighted,
        }

        enum ColorShiftOperators
        { 
            BGR_HSV,
            BGR_GRAY,
            HSV_BGR,
        }
        public enum ThresholdType
        {
            Binary = 0,
            BinaryInv = 1,
            Trunc = 2,
            ToZero = 3,
            ToZeroInv = 4,
            Mask = 7,
            Otsu = 8,
            Triangle = 16
        }


        public Form1()
        {
            InitializeComponent();
        }

        List<IInputArray> image = new List<IInputArray>();
        Dictionary<string, IInputArray> images = new Dictionary<string, IInputArray>();
        Dictionary<string, BitwiseOperators> bitwiseOperations = new Dictionary<string, BitwiseOperators>();
        Dictionary<string, ColorShiftOperators> colorshiftOperations = new Dictionary<string, ColorShiftOperators>();
        Dictionary<string, ThresholdType> thresholdOperations = new Dictionary<string, ThresholdType>(); 
        BitwiseOperators currentBitwise;
        ThresholdType currentThreshold; 
        ColorShiftOperators currentColorShift; 


        private void Form1_Load(object sender, EventArgs e)
        {
            #region setup
            alphaTextBox.Hide();
            betaTextBox.Hide();
            GammaTextBox.Hide();
            images.Add("bar", CvInvoke.Imread("Images/bar.png"));
            images.Add("square", CvInvoke.Imread("Images/square.png"));
            images.Add("circle", CvInvoke.Imread("Images/circle.png"));
            images.Add("triangle", CvInvoke.Imread("Images/triangle.png"));
            images.Add("triangle2", CvInvoke.Imread("Images/triangle2.png"));
            images.Add("lab", CvInvoke.Imread("Images/lab.png"));
            images.Add("diagram", CvInvoke.Imread("Images/rgb.png"));
            images.Add("greenSquare", CvInvoke.Imread("Images/greenSquare.png"));
            images.Add("blueAndWhite", CvInvoke.Imread("Images/blueAndWhite.jpg"));
            bitwiseOperations.Add("Add", BitwiseOperators.Add);
            bitwiseOperations.Add("And", BitwiseOperators.And);
            bitwiseOperations.Add("Or", BitwiseOperators.Or);
            bitwiseOperations.Add("Not", BitwiseOperators.Not);
            bitwiseOperations.Add("Xor", BitwiseOperators.XOr);
            bitwiseOperations.Add("Subtract", BitwiseOperators.Subtract);
            bitwiseOperations.Add("Multiply", BitwiseOperators.Multiply);
            bitwiseOperations.Add("Divide", BitwiseOperators.Divide);
            bitwiseOperations.Add("Add Weighted", BitwiseOperators.AddWeighted);
            colorshiftOperations.Add("BGR ------> HSV", ColorShiftOperators.BGR_HSV);
            colorshiftOperations.Add("BGR ------> Grayscale", ColorShiftOperators.BGR_GRAY);
            colorshiftOperations.Add("HSV ------> BGR", ColorShiftOperators.HSV_BGR);
            thresholdOperations.Add("Binary", ThresholdType.Binary);
            thresholdOperations.Add("BinaryInv", ThresholdType.BinaryInv);
            thresholdOperations.Add("Trunc", ThresholdType.Trunc);
            thresholdOperations.Add("ToZero", ThresholdType.ToZero);
            thresholdOperations.Add("ToZeroInv", ThresholdType.ToZeroInv);
            thresholdOperations.Add("Otsu", ThresholdType.Otsu);
            thresholdOperations.Add("Triangle", ThresholdType.Triangle);

            #endregion 
            #region designs 
            Mat fullCircle = CvInvoke.Imread("Images/triangle2.png"); 
                Stopwatch clock = Stopwatch.StartNew();
            Mat topTriangle = new Mat();
            CvInvoke.Rotate(fullCircle, topTriangle, Emgu.CV.CvEnum.RotateFlags.Rotate180);
            Mat hourglass = new Mat();
            CvInvoke.BitwiseOr(topTriangle, fullCircle, hourglass);
            Mat output = new Mat();
            CvInvoke.BitwiseNot(hourglass, output);

            Mat bar1 = CvInvoke.Imread("Images/bar.png");
            Mat bar2 = CvInvoke.Imread("Images/bar.png");
            CvInvoke.Rotate(bar2, bar2, Emgu.CV.CvEnum.RotateFlags.Rotate90Clockwise);
            Mat cross = new Mat();
            Mat circle = CvInvoke.Imread("Images/circle.png");
            Mat output2 = new Mat();
            CvInvoke.BitwiseOr(bar1, bar2, cross);
            CvInvoke.BitwiseXor(circle, cross, output2);

            Mat rotatedEye = new Mat();

            CvInvoke.Rotate(images["triangle2"], rotatedEye, Emgu.CV.CvEnum.RotateFlags.Rotate180);
            Mat eyes = new Mat();
            CvInvoke.BitwiseXor(rotatedEye, images["square"], eyes);
            Mat invertedEye = new Mat();
            CvInvoke.BitwiseNot(eyes, invertedEye);
            Mat output3 = new Mat();
            CvInvoke.BitwiseOr(invertedEye, images["bar"], output3);


            Mat insideSquare = new Mat();
            Mat square2 = CvInvoke.Imread("Images/square.png"); 
            CvInvoke.Resize(square2, insideSquare, new Size(square2.Width / 4, square2.Height / 4));
            int middleX = square2.Width / 2 - insideSquare.Width / 2;
            int middleY = square2.Height / 2 - insideSquare.Height /2;
            Mat insideROI = new Mat(square2, new Rectangle(middleX, middleY, insideSquare.Width, insideSquare.Height));
            insideSquare.CopyTo(insideROI);
            Mat output4 = square2;

            Mat rotatedBar = new Mat();
            var bar = CvInvoke.Imread("Images/bar.png");
            Mat square = CvInvoke.Imread("Images/square.png");
            CvInvoke.Rotate(bar, rotatedBar, Emgu.CV.CvEnum.RotateFlags.Rotate90Clockwise);
            Mat barOrCircle = new Mat();
            CvInvoke.BitwiseOr(rotatedBar, circle, barOrCircle);
            Mat strikeThrough = new Mat();
            CvInvoke.BitwiseAnd(rotatedBar, square, strikeThrough);
            Mat combine = new Mat();
            CvInvoke.BitwiseXor(strikeThrough, barOrCircle, combine);
            var triangle2 = CvInvoke.Imread("Images/triangle2.png"); 
            Mat output5 = new Mat();
            CvInvoke.BitwiseXor(combine, triangle2, output5);

            Mat parent = CvInvoke.Imread("Images/circle.png");
            for (int i = 0; i < 8; i++)
            {
                Mat innerCircle = new Mat();
                int size = parent.Width / 2;
                CvInvoke.Resize(parent, innerCircle, new Size(size, size));
                CvInvoke.BitwiseNot(innerCircle, innerCircle);
                int findX = parent.Width / 2 - innerCircle.Width / 2;
                int findY = parent.Height / 2 - innerCircle.Height / 2;
                Mat middleROI = new Mat(parent, new Rectangle(findX, findY, innerCircle.Width, innerCircle.Height));
                innerCircle.CopyTo(middleROI);
                parent = middleROI;
            }
            Mat output6 = new Mat(); 
            CvInvoke.BitwiseXor(parent, parent, output6);
            // \\GMRDC1\Folder Redirection\Rohan.Sampath\Documents\Visual Studio 2019\Projects\WinFormsApp1\WinFormsApp1\Images\

            images.Add("hourglass", output);
            images.Add("wheel", output2);
            images.Add("angryface", output3);
            images.Add("target", output4);
            images.Add("rocket", output5);
            images.Add("gradient", output6);
            #endregion
            #region comboBoxSetup
            foreach (var item in bitwiseOperations.Keys)
            {
                comboBox3.Items.Add(item);
            }
            foreach (var item in colorshiftOperations.Keys)
            {
                comboBox5.Items.Add(item); 
            }
            foreach(var item in thresholdOperations.Keys)
            {
                comboBox7.Items.Add(item); 
            }
            foreach (var item in images.Keys)
            {
                comboBox1.Items.Add(item); 
                comboBox2.Items.Add(item);
                comboBox4.Items.Add(item); 
                comboBox6.Items.Add(item);
                comboBox8.Items.Add(item); 
            }
            image.AsReadOnly();
            #endregion  
        }
        #region Bitwise Operations
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageBox3.Image = images[comboBox1.SelectedItem.ToString()] ;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageBox2.Image = images[comboBox2.SelectedItem.ToString()]; 
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            images.Add(textBox1.Text, imageBox1.Image);
            comboBox1.Items.Add(textBox1.Text);
            comboBox2.Items.Add(textBox1.Text);
            comboBox6.Items.Add(textBox1.Text);
            textBox1.Clear();
            imageBox1.Image = default; 
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void label6_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            Mat temp = new Mat();
            CvInvoke.BitwiseNot(imageBox1.Image, temp);
            imageBox1.Image = temp; 
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            Mat temp = new Mat();
            CvInvoke.Rotate(imageBox1.Image, temp, Emgu.CV.CvEnum.RotateFlags.Rotate90Clockwise);
            imageBox1.Image = temp;
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            Mat temp = new Mat();
            CvInvoke.Rotate(imageBox1.Image, temp, Emgu.CV.CvEnum.RotateFlags.Rotate90CounterClockwise);
            imageBox1.Image = temp;
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentBitwise = bitwiseOperations[comboBox3.SelectedItem.ToString()]; 
            if(currentBitwise == BitwiseOperators.AddWeighted)
            {
                alphaTextBox.Show();
                betaTextBox.Show();
                GammaTextBox.Show();
            }
            else
            {
                alphaTextBox.Hide();
                betaTextBox.Hide();
                GammaTextBox.Hide();
            }
        }
        private void GoButton_Click(object sender, EventArgs e)
        {
            if(imageBox3.Image == null|| imageBox2.Image == null)
            {
                MessageBox.Show("SELECT SOMETHING");
                return;
            }
            Mat image1 = imageBox3.Image as Mat; 
            Mat image2 = imageBox2.Image as Mat; 
            if(image1.NumberOfChannels != image2.NumberOfChannels)
            {
                MessageBox.Show("Select images with same number of Channels");
                return;
            }
            if(image1.Size != image2.Size)
            {
                MessageBox.Show("Select images with same size");
                return;
            }
            switch (currentBitwise)
            {
                case BitwiseOperators.And:
                    imageBox2.Show();
                    Mat output = new Mat();
                    Stopwatch clock = Stopwatch.StartNew();
                    CvInvoke.BitwiseAnd(imageBox2.Image, imageBox3.Image, output);
                    clock.Stop();
                    imageBox1.Image = output;
                    label1.Text = clock.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Or:
                    imageBox2.Show();
                    Mat output2 = new Mat();
                    Stopwatch clock2 = Stopwatch.StartNew();
                    CvInvoke.BitwiseOr(imageBox2.Image, imageBox3.Image, output2);
                    clock2.Stop();
                    imageBox1.Image = output2;
                    label1.Text = clock2.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Not:
                    Mat output3 = new Mat();
                    Stopwatch clock3 = Stopwatch.StartNew();
                    CvInvoke.BitwiseNot(imageBox3.Image, output3);
                    imageBox2.Hide();
                    clock3.Stop();
                    imageBox1.Image = output3;
                    label1.Text = clock3.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.XOr:
                    imageBox2.Show();
                    Mat output4 = new Mat();
                    Stopwatch clock4 = Stopwatch.StartNew();
                    CvInvoke.BitwiseXor(imageBox2.Image, imageBox3.Image, output4);
                    clock4.Stop();
                    imageBox1.Image = output4;
                    label1.Text = clock4.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Add:
                    imageBox2.Show();
                    Mat output5 = new Mat();
                    Stopwatch clock5 = Stopwatch.StartNew();
                    CvInvoke.Add(imageBox3.Image, imageBox2.Image, output5);
                    clock5.Stop();
                    imageBox1.Image = output5;
                    label1.Text = clock5.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Subtract:
                    imageBox2.Show();
                    Mat output6 = new Mat();
                    Stopwatch clock6 = Stopwatch.StartNew();
                    CvInvoke.Subtract(imageBox3.Image, imageBox2.Image, output6);
                    clock6.Stop();
                    imageBox1.Image = output6;
                    label1.Text = clock6.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Multiply:
                    imageBox2.Show();
                    Mat output7 = new Mat();
                    Stopwatch clock7 = Stopwatch.StartNew();
                    CvInvoke.Multiply(imageBox3.Image, imageBox2.Image, output7);
                    clock7.Stop();
                    imageBox1.Image = output7;
                    label1.Text = clock7.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Divide:
                    imageBox2.Show();
                    Mat output8 = new Mat();
                    Stopwatch clock8 = Stopwatch.StartNew();
                    CvInvoke.Divide(imageBox3.Image, imageBox2.Image, output8);
                    clock8.Stop();
                    imageBox1.Image = output8;
                    label1.Text = clock8.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.AddWeighted:
                    imageBox2.Show();
                    double alpha = double.Parse(alphaTextBox.Text);
                    double beta = double.Parse(betaTextBox.Text);
                    double gamma = double.Parse(GammaTextBox.Text); 
                    Mat output9 = new Mat();
                    Stopwatch clock9 = Stopwatch.StartNew();
                    CvInvoke.AddWeighted(imageBox3.Image, alpha, imageBox2.Image, beta, gamma, output9);
                    clock9.Stop();
                    imageBox1.Image = output9;
                    label1.Text = clock9.ElapsedMilliseconds.ToString();
                    alphaTextBox.ResetText();
                    betaTextBox.ResetText();
                    GammaTextBox.ResetText();
                    alphaTextBox.Hide();
                    betaTextBox.Hide();
                    GammaTextBox.Hide();
                    break;
            }
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #region ColorSplit
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageBox4.Image = images[comboBox6.SelectedItem.ToString()];
            Mat temp = imageBox4.Image as Mat;
            VectorOfMat vectorOfMat = new VectorOfMat();
            CvInvoke.Split(temp, vectorOfMat);
            SplitRedImage.Image = vectorOfMat[2];
            SplitGreenImage.Image = vectorOfMat[1];
            SplitBlueImage.Image = vectorOfMat[0];
        }
        private void RedSave_Click(object sender, EventArgs e)
        {
            images.Add(RedTextBox.Text, SplitRedImage.Image);
            comboBox1.Items.Add(RedTextBox.Text);
            comboBox2.Items.Add(RedTextBox.Text);
            comboBox6.Items.Add(RedTextBox.Text);
            RedTextBox.Clear();
            imageBox1.Image = default;
        }
        private void GreenSave_Click(object sender, EventArgs e)
        {
            images.Add(GreenTextBox.Text, SplitGreenImage.Image);
            comboBox1.Items.Add(GreenTextBox.Text);
            comboBox2.Items.Add(GreenTextBox.Text);
            comboBox6.Items.Add(GreenTextBox.Text);
            GreenTextBox.Clear();
            imageBox1.Image = default;
        }
        private void BlueSave_Click(object sender, EventArgs e)
        {
            images.Add(BlueTextBox.Text, SplitBlueImage.Image);
            comboBox1.Items.Add(BlueTextBox.Text);
            comboBox2.Items.Add(BlueTextBox.Text);
            comboBox6.Items.Add(BlueTextBox.Text);
            BlueTextBox.Clear();
            imageBox1.Image = default;
        }
        #endregion
        #region InRange UNFINISHED
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentColorShift = colorshiftOperations[comboBox5.SelectedItem.ToString()]; 
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageBox6.Image = images[comboBox4.SelectedItem.ToString()];
        }

        private void button6_Click(object sender, EventArgs e)
        {
            images.Add(textBox3.Text, imageBox5.Image);
            comboBox1.Items.Add(textBox3.Text);
            comboBox2.Items.Add(textBox3.Text);
            comboBox6.Items.Add(textBox3.Text);
            textBox3.Clear();
            imageBox5.Image = default;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }
        #endregion
        #region Threshold Operations
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentThreshold = thresholdOperations[comboBox7.SelectedItem.ToString()]; 
        }
        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageBox8.Image = images[comboBox8.SelectedItem.ToString()];
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void button5_Click(object sender, EventArgs e)
        {
                Stopwatch clock = Stopwatch.StartNew();
            switch (currentThreshold)
            {
                case ThresholdType.Binary:
                    Mat output = new Mat();
                    CvInvoke.Threshold(imageBox8.Image, output, int.Parse(textBox4.Text), int.Parse(textBox5.Text), Emgu.CV.CvEnum.ThresholdType.Binary);
                    imageBox7.Image = output; 
                    break;
                case ThresholdType.BinaryInv:
                    Mat output2 = new Mat();
                    CvInvoke.Threshold(imageBox8.Image, output2, int.Parse(textBox4.Text), int.Parse(textBox5.Text), Emgu.CV.CvEnum.ThresholdType.Binary);
                    CvInvoke.BitwiseNot(output2, output2);
                    imageBox7.Image = output2;
                    break;
                case ThresholdType.Trunc:
                    Mat output3 = new Mat();
                    CvInvoke.Threshold(imageBox8.Image, output3, int.Parse(textBox4.Text), int.Parse(textBox5.Text), Emgu.CV.CvEnum.ThresholdType.Trunc);
                    imageBox7.Image = output3;
                    break;
                case ThresholdType.ToZero:
                    Mat output4 = new Mat();
                    CvInvoke.Threshold(imageBox8.Image, output4, int.Parse(textBox4.Text), int.Parse(textBox5.Text), Emgu.CV.CvEnum.ThresholdType.ToZero);
                    imageBox7.Image = output4;
                    break;
                case ThresholdType.ToZeroInv:
                    Mat output5 = new Mat();
                    CvInvoke.Threshold(imageBox8.Image, output5, int.Parse(textBox4.Text), int.Parse(textBox5.Text), Emgu.CV.CvEnum.ThresholdType.ToZeroInv);
                    CvInvoke.BitwiseNot(output5, output5);
                    imageBox7.Image = output5;
                    break;
                case ThresholdType.Triangle:
                case ThresholdType.Otsu:
                    throw new NotImplementedException();
                    break; 
            }
            clock.Stop();
            label10.Text = $"{clock.ElapsedMilliseconds} ms"; 
            textBox4.Clear();
            textBox5.Clear();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            images.Add(textBox2.Text, imageBox7.Image);
            comboBox1.Items.Add(textBox2.Text);
            comboBox2.Items.Add(textBox2.Text);
            comboBox6.Items.Add(textBox2.Text);
            textBox2.Clear();
            imageBox7.Image = default;
        }
        #endregion 
    }
}
