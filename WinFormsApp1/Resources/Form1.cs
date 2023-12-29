using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        #region enums
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
            None,
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
        public enum ContourType
        { 
            Bounded,
            ChainApprox,
        }
        public enum BoundTypes
        {
            Rectangle,
            MinimumArea,
            MinumumCircle,
            ConvexHull, 
        }
        public enum ChainApproxMethod
        {
            ChainCode = 0,
            ChainApproxNone = 1,
            ChainApproxSimple = 2,
            ChainApproxTc89L1 = 3,
            ChainApproxTc89Kcos = 4,
            LinkRuns = 5,
        }

        public enum BlurTypes
        { 
            Simple, 
            Gaussian,
            Median,
            Bilateral
        }

        public enum MaxLum
        { 
            HSV,
            BGR,
            GrayScale,
        }

        #endregion 
        public Form1()
        {
            InitializeComponent();
        }

        List<IInputArray> image = new List<IInputArray>();
        Dictionary<string, IInputArray> images = new Dictionary<string, IInputArray>();
        Dictionary<string, BitwiseOperators> bitwiseOperations = new Dictionary<string, BitwiseOperators>();
        Dictionary<string, ColorShiftOperators> colorshiftOperations = new Dictionary<string, ColorShiftOperators>();
        Dictionary<string, ThresholdType> thresholdOperations = new Dictionary<string, ThresholdType>(); 
        Dictionary<string, ChainApproxMethod> ChainApproxMethods = new Dictionary<string, ChainApproxMethod>(); 
        Dictionary<string, BoundTypes> BoundedTypes = new Dictionary<string, BoundTypes>(); 
        Dictionary<string, ContourType> ContourTypes = new Dictionary<string, ContourType>(); 
        Dictionary<string, BlurTypes> BlurType = new Dictionary<string, BlurTypes>();
        Dictionary<string, MaxLum> MaximumValueColor = new Dictionary<string, MaxLum>();  
        BitwiseOperators currentBitwise;
        ThresholdType currentThreshold; 
        ColorShiftOperators currentColorShift;
        ContourType currentContourType;
        ChainApproxMethod currentChainApprox;
        BoundTypes currentBounds;
        BlurTypes currentBlur;
        MaxLum currentColorSpace;
        Mat currentMaskContour;
        bool notSelected = true; 
        private void Form1_Load(object sender, EventArgs e)
        {
            #region setup
            ChainApproxSelect.Hide();
            BoundedShapeSelect.Hide();
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
            images.Add("prompt", CvInvoke.Imread("Images/prompt.png"));
            images.Add("SHApez", CvInvoke.Imread("Images/cont2.png"));
            images.Add("Rubiks", CvInvoke.Imread("Images/rubiks3.png"));
            images.Add("BentRubix", CvInvoke.Imread("Images/rubiks2.png")); 
            bitwiseOperations.Add("Add", BitwiseOperators.Add);
            bitwiseOperations.Add("And", BitwiseOperators.And);
            bitwiseOperations.Add("Or", BitwiseOperators.Or);
            bitwiseOperations.Add("Not", BitwiseOperators.Not);
            bitwiseOperations.Add("Xor", BitwiseOperators.XOr);
            bitwiseOperations.Add("Subtract", BitwiseOperators.Subtract);
            bitwiseOperations.Add("Multiply", BitwiseOperators.Multiply);
            bitwiseOperations.Add("Divide", BitwiseOperators.Divide);
            bitwiseOperations.Add("Add Weighted", BitwiseOperators.AddWeighted);
            colorshiftOperations.Add("None", ColorShiftOperators.None); 
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
            ContourTypes.Add("Bounded", ContourType.Bounded);
            ContourTypes.Add("ChainApprox", ContourType.ChainApprox);
            ChainApproxMethods.Add("Simple", ChainApproxMethod.ChainApproxSimple);
            ChainApproxMethods.Add("NoApprox", ChainApproxMethod.ChainApproxNone);
            ChainApproxMethods.Add("Tc89Kcos", ChainApproxMethod.ChainApproxTc89Kcos);
            ChainApproxMethods.Add("Tc89L1", ChainApproxMethod.ChainApproxTc89L1);
            ChainApproxMethods.Add("ChainCode", ChainApproxMethod.ChainCode);
            BoundedTypes.Add("ConvexHull", BoundTypes.ConvexHull);
            BoundedTypes.Add("MinumumArea", BoundTypes.MinimumArea);
            BoundedTypes.Add("MinumumCircle", BoundTypes.MinumumCircle);
            BoundedTypes.Add("Rectangle", BoundTypes.Rectangle);
            BlurType.Add("Simple", BlurTypes.Simple);
            BlurType.Add("Gaussian", BlurTypes.Gaussian);
            BlurType.Add("Median", BlurTypes.Median);
            BlurType.Add("Bilaterial", BlurTypes.Bilateral);
            MaximumValueColor.Add("BGR", MaxLum.BGR);
            MaximumValueColor.Add("HSV", MaxLum.HSV);
            MaximumValueColor.Add("GrayScale", MaxLum.GrayScale);
            #endregion
            #region designs 

            numericUpDown4.Maximum = 255;
            numericUpDown5.Maximum = 255;
            numericUpDown6.Maximum = 255;
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
            foreach (var item in BlurType.Keys)
            {
                BlurTypeComboBox.Items.Add(item); 
            }
            foreach (var item in ChainApproxMethods.Keys)
            {
                ChainApproxSelect.Items.Add(item);
            }
            foreach (var item in BoundedTypes.Keys)
            {
                BoundedShapeSelect.Items.Add(item); 
            }
            foreach (var item in ContourTypes.Keys)
            {
                comboBox11.Items.Add(item); 
            }
            foreach (var item in bitwiseOperations.Keys)
            {
                comboBox3.Items.Add(item);
            }
            foreach (var item in colorshiftOperations.Keys)
            {
                ColorShiftOperationSelect.Items.Add(item); 
            }
            foreach(var item in thresholdOperations.Keys)
            {
                comboBox7.Items.Add(item); 
            }
            foreach (var item in MaximumValueColor.Keys)
            {
                comboBox5.Items.Add(item); 
            }
            foreach (var item in images.Keys)
            {
                comboBox1.Items.Add(item); 
                comboBox2.Items.Add(item);
                comboBox4.Items.Add(item); 
                comboBox6.Items.Add(item);
                BlurOperationImageSelect.Items.Add(item);
                MaskSelect.Items.Add(item);
                comboBox8.Items.Add(item);
                comboBox12.Items.Add(item);
                ColorShiftImageSelect.Items.Add(item);
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
            BlurOperationImageSelect.Items.Add(textBox1.Text);
            ColorShiftImageSelect.Items.Add(textBox1.Text);
            MaskSelect.Items.Add(textBox1.Text);
            comboBox12.Items.Add(textBox1.Text); 
            comboBox8.Items.Add(textBox1.Text); 
            comboBox4.Items.Add(textBox1.Text); 
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
            BlurOperationImageSelect.Items.Add(RedTextBox.Text);
            comboBox12.Items.Add(RedTextBox.Text);
            comboBox8.Items.Add(RedTextBox.Text);
            comboBox4.Items.Add(RedTextBox.Text);
            RedTextBox.Clear();
            imageBox1.Image = default;
        }
        private void GreenSave_Click(object sender, EventArgs e)
        {
            images.Add(GreenTextBox.Text, SplitGreenImage.Image);
            comboBox1.Items.Add(GreenTextBox.Text);
            comboBox2.Items.Add(GreenTextBox.Text);
            comboBox6.Items.Add(GreenTextBox.Text);
            BlurOperationImageSelect.Items.Add(GreenTextBox.Text);
            comboBox12.Items.Add(GreenTextBox.Text);
            comboBox8.Items.Add(GreenTextBox.Text);
            comboBox4.Items.Add(GreenTextBox.Text);
            GreenTextBox.Clear();
            imageBox1.Image = default;
        }
        private void BlueSave_Click(object sender, EventArgs e)
        {
            images.Add(BlueTextBox.Text, SplitBlueImage.Image);
            comboBox1.Items.Add(BlueTextBox.Text);
            comboBox2.Items.Add(BlueTextBox.Text);
            comboBox6.Items.Add(BlueTextBox.Text);
            BlurOperationImageSelect.Items.Add(BlueTextBox.Text);
            comboBox12.Items.Add(BlueTextBox.Text);
            comboBox8.Items.Add(BlueTextBox.Text);
            comboBox4.Items.Add(BlueTextBox.Text);
            BlueTextBox.Clear();
            imageBox1.Image = default;
        }
        #endregion
        #region InRange 
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentColorSpace = MaximumValueColor[comboBox5.SelectedItem.ToString()];

            switch (currentColorSpace)
            {
                case MaxLum.BGR:
                    numericUpDown4.Maximum = 255;
                    numericUpDown5.Maximum = 255;
                    numericUpDown6.Maximum = 255;
                    break;
                case MaxLum.HSV:
                    numericUpDown4.Maximum = 100;
                    numericUpDown5.Maximum = 100;
                    numericUpDown6.Maximum = 180;
                    break;
                case MaxLum.GrayScale:
                    numericUpDown4.Maximum = 255;
                    numericUpDown5.Maximum = 255;
                    numericUpDown6.Maximum = 255;
                    break;
            }
            numericUpDown1.Maximum = numericUpDown6.Maximum;
            numericUpDown2.Maximum = numericUpDown5.Maximum;
            numericUpDown3.Maximum = numericUpDown4.Maximum;
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown4.Value = numericUpDown4.Maximum; 
            numericUpDown5.Value = numericUpDown5.Maximum; 
            numericUpDown6.Value = numericUpDown6.Maximum; 
            imageBox6.Image = images[comboBox4.SelectedItem.ToString()];
        }

        private void button6_Click(object sender, EventArgs e)
        {
            images.Add(textBox3.Text, imageBox5.Image);
            comboBox1.Items.Add(textBox3.Text);
            comboBox2.Items.Add(textBox3.Text);
            comboBox6.Items.Add(textBox3.Text);
            BlurOperationImageSelect.Items.Add(textBox3.Text);
            ColorShiftImageSelect.Items.Add(textBox3.Text);
            MaskSelect.Items.Add(textBox3.Text);
            comboBox12.Items.Add(textBox3.Text);
            comboBox8.Items.Add(textBox3.Text);
            comboBox4.Items.Add(textBox3.Text);
            textBox3.Clear();
            imageBox5.Image = default;
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = imageBox6.Image as Mat;
            if (temp == null) return;

            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar(int.Parse(numericUpDown1.Text), int.Parse(numericUpDown2.Text), int.Parse(numericUpDown3.Text)),
                             (ScalarArray)new MCvScalar(int.Parse(numericUpDown6.Text), int.Parse(numericUpDown5.Text), int.Parse(numericUpDown4.Text)), output);
            imageBox5.Image = output; 
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = imageBox6.Image as Mat;
            if (temp == null) return;

            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar(int.Parse(numericUpDown1.Text), int.Parse(numericUpDown2.Text), int.Parse(numericUpDown3.Text)),
                             (ScalarArray)new MCvScalar(int.Parse(numericUpDown6.Text), int.Parse(numericUpDown5.Text), int.Parse(numericUpDown4.Text)), output);
            imageBox5.Image = output;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = imageBox6.Image as Mat;
            if (temp == null) return;

            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar(int.Parse(numericUpDown1.Text), int.Parse(numericUpDown2.Text), int.Parse(numericUpDown3.Text)),
                             (ScalarArray)new MCvScalar(int.Parse(numericUpDown6.Text), int.Parse(numericUpDown5.Text), int.Parse(numericUpDown4.Text)), output);
            imageBox5.Image = output;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = imageBox6.Image as Mat;
            if (temp == null) return;
            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar(int.Parse(numericUpDown1.Text), int.Parse(numericUpDown2.Text), int.Parse(numericUpDown3.Text)),
                             (ScalarArray)new MCvScalar(int.Parse(numericUpDown6.Text), int.Parse(numericUpDown5.Text), int.Parse(numericUpDown4.Text)), output);
            imageBox5.Image = output;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = imageBox6.Image as Mat;
            if (temp == null) return;

            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar(int.Parse(numericUpDown1.Text), int.Parse(numericUpDown2.Text), int.Parse(numericUpDown3.Text)),
                             (ScalarArray)new MCvScalar(int.Parse(numericUpDown6.Text), int.Parse(numericUpDown5.Text), int.Parse(numericUpDown4.Text)), output);
            imageBox5.Image = output;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = imageBox6.Image as Mat;
            if (temp == null) return;

            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar(int.Parse(numericUpDown1.Text), int.Parse(numericUpDown2.Text), int.Parse(numericUpDown3.Text)),
                             (ScalarArray)new MCvScalar(int.Parse(numericUpDown6.Text), int.Parse(numericUpDown5.Text), int.Parse(numericUpDown4.Text)), output);
            imageBox5.Image = output;
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
            Mat output = new Mat();
            CvInvoke.Threshold(imageBox8.Image, output, int.Parse(textBox4.Text), int.Parse(textBox5.Text), (Emgu.CV.CvEnum.ThresholdType)currentThreshold);
            imageBox7.Image = output;
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
            BlurOperationImageSelect.Items.Add(textBox2.Text);
            comboBox12.Items.Add(textBox2.Text);
            comboBox8.Items.Add(textBox2.Text);
            MaskSelect.Items.Add(textBox2.Text);
            comboBox4.Items.Add(textBox2.Text);
            textBox2.Clear();
            imageBox7.Image = default;
        }


        #endregion
        #region blurring
        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageBox10.Image = images[BlurOperationImageSelect.SelectedItem.ToString()];
        }
        private void button7_Click(object sender, EventArgs e)
        {
            images.Add(BlurSaveText.Text, imageBox9.Image);
            comboBox1.Items.Add(BlurSaveText.Text);
            comboBox2.Items.Add(BlurSaveText.Text);
            comboBox6.Items.Add(BlurSaveText.Text);
            BlurOperationImageSelect.Items.Add(BlurSaveText.Text);
            comboBox12.Items.Add(BlurSaveText.Text);
            comboBox8.Items.Add(BlurSaveText.Text);
            comboBox4.Items.Add(BlurSaveText.Text);
            MaskSelect.Items.Add(BlurSaveText.Text);
            BlurSaveText.Clear();
            imageBox9.Image = default;
        }
        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentBlur = BlurType[BlurTypeComboBox.SelectedItem.ToString()];
        }
        private void button10_Click(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Size kSize = new Size(int.Parse(KernelSizeText.Text), int.Parse(KernelSizeText.Text));
            switch (currentBlur)
            {
                case BlurTypes.Simple:
                    CvInvoke.Blur(imageBox10.Image, output, kSize, new Point(-1, -1));
                    break;
                case BlurTypes.Gaussian:
                    CvInvoke.GaussianBlur(imageBox10.Image, output, kSize, 0);
                    break;
                case BlurTypes.Median:
                    CvInvoke.MedianBlur(imageBox10.Image, output, kSize.Width);
                    break;
                case BlurTypes.Bilateral:
                    CvInvoke.BilateralFilter(imageBox10.Image, output, 0, 0, 0, BorderType.Reflect101);
                    break;
            }
            imageBox9.Image = output; 
        }
        #endregion
        #region Contours
        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageBox12.Image = images[comboBox12.SelectedItem.ToString()];
        }
        private void button8_Click(object sender, EventArgs e)
        {
            images.Add(textBox7.Text, imageBox11.Image);
            comboBox1.Items.Add(textBox7.Text);
            comboBox2.Items.Add(textBox7.Text);
            comboBox6.Items.Add(textBox7.Text);
            BlurOperationImageSelect.Items.Add(textBox7.Text);
            comboBox12.Items.Add(textBox7.Text);
            MaskSelect.Items.Add(textBox7.Text);
            comboBox8.Items.Add(textBox7.Text);
            comboBox4.Items.Add(textBox7.Text);
            textBox7.Clear();
            imageBox11.Image = default;
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentContourType = ContourTypes[comboBox11.SelectedItem.ToString()];
            if(comboBox11.SelectedItem.ToString() == "ChainApprox")
            {
                ChainApproxSelect.Show();
                BoundedShapeSelect.Hide(); 
            }
            else if(comboBox11.SelectedItem.ToString() == "Bounded")
            {
                ChainApproxSelect.Hide();
                BoundedShapeSelect.Show();
            }
            else
            {
                ChainApproxSelect.Hide();
                BoundedShapeSelect.Hide();
            }
            notSelected = false; 
        }
        private void button9_Click(object sender, EventArgs e)
        {
            Mat first = imageBox12.Image as Mat;
            Mat mask = new Mat();
            if (currentMaskContour != null)
            {
                mask = currentMaskContour;
            }
            else
            {
                MessageBox.Show("CHOOSE A MASK");
                return;
            }

            switch (currentContourType)
            {
                case ContourType.Bounded:
                    Mat grayScaledImage = mask;
                    var temp = imageBox12.Image as Mat;
                    Mat original = temp.Clone();
                    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                    Mat nextLayer = new Mat();
                    CvInvoke.FindContours(grayScaledImage, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
                    switch (currentBounds)
                    {
                        case BoundTypes.ConvexHull:

                            break;

                        case BoundTypes.MinumumCircle:

                            for (int i = 0; i < contours.Size; i++)
                            {
                                CircleF circ = CvInvoke.MinEnclosingCircle(contours[i]);
                                CvInvoke.Circle(original, new Point((int)circ.Center.X, (int)circ.Center.Y), (int)circ.Radius, new MCvScalar((double)BlueSelect.Value, (double)GreenSelect.Value, (double)RedSelect.Value), 2);
                            }
                            imageBox11.Image = original;
                            break;

                        case BoundTypes.Rectangle:
                            if (notSelected)
                            {
                                MessageBox.Show("CHOOSE A METHOD");
                                return;
                            }
                            for (int i = 0; i < contours.Size; i++)
                            {
                                Rectangle rect = CvInvoke.BoundingRectangle(contours[i]); 
                                CvInvoke.Rectangle(original, rect, new MCvScalar((double)BlueSelect.Value, (double)GreenSelect.Value, (double)RedSelect.Value), 2);
                            }
                            imageBox11.Image = original;
                            break;
                        case BoundTypes.MinimumArea:
                            for (int i = 0; i < contours.Size; i++)
                            {
                                RotatedRect rect = CvInvoke.MinAreaRect(contours[i]);
                                var verticies = rect.GetVertices().Select(curr => new Point((int)curr.X, (int)curr.Y)).ToArray();
                                CvInvoke.Line(original, verticies[0], verticies[1], new MCvScalar((double)BlueSelect.Value, (double)GreenSelect.Value, (double)RedSelect.Value), 2);
                                CvInvoke.Line(original, verticies[1], verticies[2], new MCvScalar((double)BlueSelect.Value, (double)GreenSelect.Value, (double)RedSelect.Value), 2);
                                CvInvoke.Line(original, verticies[2], verticies[3], new MCvScalar((double)BlueSelect.Value, (double)GreenSelect.Value, (double)RedSelect.Value), 2);
                                CvInvoke.Line(original, verticies[3], verticies[0], new MCvScalar((double)BlueSelect.Value, (double)GreenSelect.Value, (double)RedSelect.Value), 2);
                            }
                            imageBox11.Image = original; 
                            break;
                        default:
                            MessageBox.Show("CHOOSE A METHOD");
                            return; 
                    }
                    break;
                case ContourType.ChainApprox:
                    Mat grayScaledImage2 = new Mat();
                    var temp2 = imageBox12.Image as Mat;
                    Mat original2 = temp2;
                    CvInvoke.CvtColor(original2, grayScaledImage2, ColorConversion.Bgr2Gray);
                    VectorOfVectorOfPoint contours2 = new VectorOfVectorOfPoint();
                    Mat nextLayer2 = new Mat();
                    CvInvoke.FindContours(grayScaledImage2, contours2, nextLayer2, RetrType.External, (Emgu.CV.CvEnum.ChainApproxMethod)currentChainApprox);
                    CvInvoke.DrawContours(original2, contours2, -1, new MCvScalar((double)BlueSelect.Value, (double)GreenSelect.Value, (double)RedSelect.Value), 3); 
                    imageBox11.Image = original2;
                    break;
                      
            }

        }
        private void comboBox9_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            currentMaskContour = images[MaskSelect.SelectedItem.ToString()] as Mat;
            MaskImage.Image = currentMaskContour; 
        }
        private void ChainApproxSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentChainApprox = ChainApproxMethods[ChainApproxSelect.SelectedItem.ToString()];
        }
        private void BoundedShapeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentBounds = BoundedTypes[BoundedShapeSelect.SelectedItem.ToString()];
        }





        #endregion

        private void label17_Click(object sender, EventArgs e)
        {

        }
        private void label18_Click(object sender, EventArgs e)
        {

        }
        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
       
        #region Color Convert
        private void ColorShiftImageSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageBox14.Image = images[ColorShiftImageSelect.SelectedItem.ToString()];
        }
        private void ColorShiftOperationSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentColorShift = colorshiftOperations[ColorShiftOperationSelect.SelectedItem.ToString()];

            Mat output = new Mat();
            Mat temp = imageBox14.Image as Mat;
            Stopwatch clock = Stopwatch.StartNew();
            switch (currentColorShift)
            {
                case ColorShiftOperators.BGR_GRAY:
                    CvInvoke.CvtColor(temp, output, ColorConversion.Bgr2Gray);
                    imageBox13.Image = output;
                    break;
                case ColorShiftOperators.BGR_HSV:
                    if(temp.NumberOfChannels != 3)
                    {
                        MessageBox.Show("MUST HAVE 3 CHANNELSZ!!!!");
                        return; 
                    }
                    CvInvoke.CvtColor(temp, output, ColorConversion.Bgr2Hsv);
                    imageBox13.Image = output;
                    break;
                case ColorShiftOperators.HSV_BGR:
                    if (temp.NumberOfChannels != 3)
                    {
                        MessageBox.Show("MUST HAVE 3 CHANNELSZ!!!!");
                        return;
                    }
                    CvInvoke.CvtColor(temp, output, ColorConversion.Hsv2Bgr);
                    imageBox13.Image = output;
                    break;
            }
            clock.Stop();
            label19.Text = $"{clock.ElapsedMilliseconds} ms";

        }
        private void button7_Click_1(object sender, EventArgs e)
        {
            images.Add(textBox6.Text, imageBox13.Image);
            comboBox1.Items.Add(textBox6.Text);
            comboBox2.Items.Add(textBox6.Text);
            comboBox6.Items.Add(textBox6.Text);
            BlurOperationImageSelect.Items.Add(textBox6.Text);
            ColorShiftImageSelect.Items.Add(textBox6.Text);
            comboBox12.Items.Add(textBox6.Text);
            comboBox8.Items.Add(textBox6.Text);
            comboBox4.Items.Add(textBox6.Text);
            textBox6.Clear();
            imageBox13.Image = default;
        }
        #endregion

        
    }
}
