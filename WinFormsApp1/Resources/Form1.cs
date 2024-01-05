using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
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

        public enum DilateErode
        { 
            Dilate,
            Erode,
        }


        #endregion 
        public Form1()
        {
            InitializeComponent();
        }
        #region Globals
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
        Dictionary<string, DilateErode> DilateErodeOperations = new Dictionary<string, DilateErode>();  
        BitwiseOperators currentBitwise;
        ThresholdType currentThreshold; 
        ColorShiftOperators currentColorShift;
        ContourType currentContourType;
        ChainApproxMethod currentChainApprox;
        BoundTypes currentBounds;
        BlurTypes currentBlur;
        MaxLum currentColorSpace;
        Mat currentMaskContour;
        DilateErode currentDilateErode; 
        bool notSelected = true;
        VideoCapture capture = new VideoCapture(0);
        #endregion
        void GetFrame(object sender, EventArgs e)
        {
            AffineTransformInputImage.Image?.Dispose(); 
            Mat output = capture.QueryFrame();
            if (output == null) return; 
            CvInvoke.Flip(output, output, FlipType.Horizontal);
            imageBox15.Image = output;
            AffineTransformInputImage.Image = output;
            PerspectiveImageCamera.Image = output;
        }
        void GetFrame(ref ImageBox imageBOx)
        {
            Mat currentFrame = capture.QueryFrame();
            if (currentFrame == null) return;
            using Mat output = currentFrame.Clone();
            CvInvoke.Flip(output, output, FlipType.Horizontal);
            CvInvoke.Flip(output, output, FlipType.Vertical);
            imageBOx.Image = output;
            currentFrame = output;
        }
        private void SaveInfo(TextBox textBox1, ImageBox imageBox1)
        {
            images.Add(textBox1.Text, imageBox1.Image);
            BitwisePic1Select.Items.Add(textBox1.Text);
            BitWisePic2Select.Items.Add(textBox1.Text);
            ColorSplitImageSelect.Items.Add(textBox1.Text);
            BlurOperationImageSelect.Items.Add(textBox1.Text);
            ColorShiftImageSelect.Items.Add(textBox1.Text);
            MaskSelect.Items.Add(textBox1.Text);
            ContourImageSelect.Items.Add(textBox1.Text);
            ThresholdImageSelect.Items.Add(textBox1.Text);
            InRangeSelect.Items.Add(textBox1.Text);
            SpotDiffSelectionBox.Items.Add(textBox1.Text);
            ErodeDilateSelectImage.Items.Add(textBox1.Text); 
            textBox1.Clear();
            imageBox1.Image = default;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            Application.Idle += new EventHandler(GetFrame);
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
            images.Add("Cam", capture.QueryFrame());
            images.Add("snowMan", CvInvoke.Imread("Images/snowman.png"));
            images.Add("room", CvInvoke.Imread("Images/room.png"));
            images.Add("kitchen", CvInvoke.Imread("Images/cooking.png"));
            images.Add("restaurant", CvInvoke.Imread("Images/restaurant.png"));
            images.Add("Affine", CvInvoke.Imread("Images/restaurant.png"));
            images.Add("whiteGuy", CvInvoke.Imread("Images/download.png"));
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
            DilateErodeOperations.Add("Dilate", DilateErode.Dilate);
            DilateErodeOperations.Add("Erode", DilateErode.Erode);
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
            foreach (var item in DilateErodeOperations.Keys)
            {
                ErodeDilateSelectOperation.Items.Add(item) ;
            }
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
                ContourTypeSelect.Items.Add(item); 
            }
            foreach (var item in bitwiseOperations.Keys)
            {
                BitWiseOperationSelect.Items.Add(item);
            }
            foreach (var item in colorshiftOperations.Keys)
            {
                ColorShiftOperationSelect.Items.Add(item); 
            }
            foreach(var item in thresholdOperations.Keys)
            {
                ThresholdTypeSelect.Items.Add(item); 
            }
            foreach (var item in MaximumValueColor.Keys)
            {
                InRangeColorSpaceSelect.Items.Add(item); 
            }
            foreach (var item in images.Keys)
            {
                BitwisePic1Select.Items.Add(item); 
                BitWisePic2Select.Items.Add(item);
                InRangeSelect.Items.Add(item); 
                ColorSplitImageSelect.Items.Add(item);
                BlurOperationImageSelect.Items.Add(item);
                MaskSelect.Items.Add(item);
                ThresholdImageSelect.Items.Add(item);
                ContourImageSelect.Items.Add(item);
                SpotDiffSelectionBox.Items.Add(item);
                ErodeDilateSelectImage.Items.Add(item);
                ColorShiftImageSelect.Items.Add(item);
            }
            image.AsReadOnly();
            #endregion  
        }
        #region Bitwise Operations
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bitWiseInput1.Image = images[BitwisePic1Select.SelectedItem.ToString()] ;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            bitWiseInput2.Image = images[BitWisePic2Select.SelectedItem.ToString()]; 
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveInfo(BitWiseSaveText, bitWiseOutput); 
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
            CvInvoke.BitwiseNot(bitWiseOutput.Image, temp);
            bitWiseOutput.Image = temp; 
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            Mat temp = new Mat();
            CvInvoke.Rotate(bitWiseOutput.Image, temp, Emgu.CV.CvEnum.RotateFlags.Rotate90Clockwise);
            bitWiseOutput.Image = temp;
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            Mat temp = new Mat();
            CvInvoke.Rotate(bitWiseOutput.Image, temp, Emgu.CV.CvEnum.RotateFlags.Rotate90CounterClockwise);
            bitWiseOutput.Image = temp;
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentBitwise = bitwiseOperations[BitWiseOperationSelect.SelectedItem.ToString()]; 
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
            if(bitWiseInput1.Image == null|| bitWiseInput2.Image == null)
            {
                MessageBox.Show("SELECT SOMETHING");
                return;
            }
            Mat image1 = bitWiseInput1.Image as Mat; 
            Mat image2 = bitWiseInput2.Image as Mat; 
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
                    bitWiseInput2.Show();
                    Mat output = new Mat();
                    Stopwatch clock = Stopwatch.StartNew();
                    CvInvoke.BitwiseAnd(bitWiseInput2.Image, bitWiseInput1.Image, output);
                    clock.Stop();
                    bitWiseOutput.Image = output;
                    label1.Text = clock.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Or:
                    bitWiseInput2.Show();
                    Mat output2 = new Mat();
                    Stopwatch clock2 = Stopwatch.StartNew();
                    CvInvoke.BitwiseOr(bitWiseInput2.Image, bitWiseInput1.Image, output2);
                    clock2.Stop();
                    bitWiseOutput.Image = output2;
                    label1.Text = clock2.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Not:
                    Mat output3 = new Mat();
                    Stopwatch clock3 = Stopwatch.StartNew();
                    CvInvoke.BitwiseNot(bitWiseInput1.Image, output3);
                    bitWiseInput2.Hide();
                    clock3.Stop();
                    bitWiseOutput.Image = output3;
                    label1.Text = clock3.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.XOr:
                    bitWiseInput2.Show();
                    Mat output4 = new Mat();
                    Stopwatch clock4 = Stopwatch.StartNew();
                    CvInvoke.BitwiseXor(bitWiseInput2.Image, bitWiseInput1.Image, output4);
                    clock4.Stop();
                    bitWiseOutput.Image = output4;
                    label1.Text = clock4.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Add:
                    bitWiseInput2.Show();
                    Mat output5 = new Mat();
                    Stopwatch clock5 = Stopwatch.StartNew();
                    CvInvoke.Add(bitWiseInput1.Image, bitWiseInput2.Image, output5);
                    clock5.Stop();
                    bitWiseOutput.Image = output5;
                    label1.Text = clock5.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Subtract:
                    bitWiseInput2.Show();
                    Mat output6 = new Mat();
                    Stopwatch clock6 = Stopwatch.StartNew();
                    CvInvoke.Subtract(bitWiseInput1.Image, bitWiseInput2.Image, output6);
                    clock6.Stop();
                    bitWiseOutput.Image = output6;
                    label1.Text = clock6.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Multiply:
                    bitWiseInput2.Show();
                    Mat output7 = new Mat();
                    Stopwatch clock7 = Stopwatch.StartNew();
                    CvInvoke.Multiply(bitWiseInput1.Image, bitWiseInput2.Image, output7);
                    clock7.Stop();
                    bitWiseOutput.Image = output7;
                    label1.Text = clock7.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.Divide:
                    bitWiseInput2.Show();
                    Mat output8 = new Mat();
                    Stopwatch clock8 = Stopwatch.StartNew();
                    CvInvoke.Divide(bitWiseInput1.Image, bitWiseInput2.Image, output8);
                    clock8.Stop();
                    bitWiseOutput.Image = output8;
                    label1.Text = clock8.ElapsedMilliseconds.ToString();
                    break;

                case BitwiseOperators.AddWeighted:
                    bitWiseInput2.Show();
                    double alpha = double.Parse(alphaTextBox.Text);
                    double beta = double.Parse(betaTextBox.Text);
                    double gamma = double.Parse(GammaTextBox.Text); 
                    Mat output9 = new Mat();
                    Stopwatch clock9 = Stopwatch.StartNew();
                    CvInvoke.AddWeighted(bitWiseInput1.Image, alpha, bitWiseInput2.Image, beta, gamma, output9);
                    clock9.Stop();
                    bitWiseOutput.Image = output9;
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
        private void SplitBlueImage_Click(object sender, EventArgs e)
        {

        }
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            ColorSplitImageBox.Image = images[ColorSplitImageSelect.SelectedItem.ToString()];
            Mat temp = ColorSplitImageBox.Image as Mat;
            VectorOfMat vectorOfMat = new VectorOfMat();
            CvInvoke.Split(temp, vectorOfMat);
            SplitRedImage.Image = vectorOfMat[2];
            SplitGreenImage.Image = vectorOfMat[1];
            SplitBlueImage.Image = vectorOfMat[0];
        }
        private void RedSave_Click(object sender, EventArgs e)
        {
            SaveInfo(RedTextBox, SplitRedImage);
        }
        private void GreenSave_Click(object sender, EventArgs e)
        {
            SaveInfo(GreenTextBox, SplitGreenImage);
        }
        private void BlueSave_Click(object sender, EventArgs e)
        {
            SaveInfo(BlueTextBox, SplitBlueImage);

        }
        #endregion
        #region InRange 
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentColorSpace = MaximumValueColor[InRangeColorSpaceSelect.SelectedItem.ToString()];

            switch (currentColorSpace)
            {
                case MaxLum.BGR:
                    numericUpDown4.Maximum = 255;
                    numericUpDown5.Maximum = 255;
                    numericUpDown6.Maximum = 255;
                    break;
                case MaxLum.HSV:
                    numericUpDown4.Maximum = 255;
                    numericUpDown5.Maximum = 255;
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
            InRangeInputImage.Image = images[InRangeSelect.SelectedItem.ToString()];
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveInfo(InRangeSaveText, imageBox5);
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = InRangeInputImage.Image as Mat;
            if (temp == null) return;

            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar(int.Parse(numericUpDown1.Text), int.Parse(numericUpDown2.Text), int.Parse(numericUpDown3.Text)),
                             (ScalarArray)new MCvScalar(int.Parse(numericUpDown6.Text), int.Parse(numericUpDown5.Text), int.Parse(numericUpDown4.Text)), output);
            imageBox5.Image = output; 
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = InRangeInputImage.Image as Mat;
            if (temp == null) return;

            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar((double)numericUpDown1.Value, (double)numericUpDown2.Value, (double)numericUpDown3.Value),
                             (ScalarArray)new MCvScalar((double)numericUpDown6.Value, (double)numericUpDown5.Value, (double)numericUpDown4.Value), output);
            imageBox5.Image = output;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = InRangeInputImage.Image as Mat;
            if (temp == null) return;

            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar(int.Parse(numericUpDown1.Text), int.Parse(numericUpDown2.Text), int.Parse(numericUpDown3.Text)),
                             (ScalarArray)new MCvScalar(int.Parse(numericUpDown6.Text), int.Parse(numericUpDown5.Text), int.Parse(numericUpDown4.Text)), output);
            imageBox5.Image = output;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = InRangeInputImage.Image as Mat;
            if (temp == null) return;
            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar(int.Parse(numericUpDown1.Text), int.Parse(numericUpDown2.Text), int.Parse(numericUpDown3.Text)),
                             (ScalarArray)new MCvScalar(int.Parse(numericUpDown6.Text), int.Parse(numericUpDown5.Text), int.Parse(numericUpDown4.Text)), output);
            imageBox5.Image = output;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = InRangeInputImage.Image as Mat;
            if (temp == null) return;

            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar(int.Parse(numericUpDown1.Text), int.Parse(numericUpDown2.Text), int.Parse(numericUpDown3.Text)),
                             (ScalarArray)new MCvScalar(int.Parse(numericUpDown6.Text), int.Parse(numericUpDown5.Text), int.Parse(numericUpDown4.Text)), output);
            imageBox5.Image = output;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            Mat output = new Mat();
            Mat temp = InRangeInputImage.Image as Mat;
            if (temp == null) return;

            CvInvoke.InRange(temp, (ScalarArray)new MCvScalar(int.Parse(numericUpDown1.Text), int.Parse(numericUpDown2.Text), int.Parse(numericUpDown3.Text)),
                             (ScalarArray)new MCvScalar(int.Parse(numericUpDown6.Text), int.Parse(numericUpDown5.Text), int.Parse(numericUpDown4.Text)), output);
            imageBox5.Image = output;
        }

        #endregion
        #region Threshold Operations
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentThreshold = thresholdOperations[ThresholdTypeSelect.SelectedItem.ToString()]; 
        }
        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            ThresholdImageBox.Image = images[ThresholdImageSelect.SelectedItem.ToString()];
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void button5_Click(object sender, EventArgs e)
        {
            Stopwatch clock = Stopwatch.StartNew();
            Mat output = new Mat();
            CvInvoke.Threshold(ThresholdImageBox.Image, output, int.Parse(ThresholdMinValueText.Text), int.Parse(ThresholdMaxValueText.Text), (Emgu.CV.CvEnum.ThresholdType)currentThreshold);
            ThresholdOutputImageBox.Image = output;
            clock.Stop();
            label10.Text = $"{clock.ElapsedMilliseconds} ms"; 
            ThresholdMinValueText.Clear();
            ThresholdMaxValueText.Clear();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            SaveInfo(ThresholdSaveText, ThresholdOutputImageBox);

        }


        #endregion
        #region blurring
        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            BlurInputImage.Image = images[BlurOperationImageSelect.SelectedItem.ToString()];
        }
        private void button7_Click(object sender, EventArgs e)
        {
            SaveInfo(BlurSaveText, BlurOutputImage); 
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
                    CvInvoke.Blur(BlurInputImage.Image, output, kSize, new Point(-1, -1));
                    break;
                case BlurTypes.Gaussian:
                    CvInvoke.GaussianBlur(BlurInputImage.Image, output, kSize, 0);
                    break;
                case BlurTypes.Median:
                    CvInvoke.MedianBlur(BlurInputImage.Image, output, kSize.Width);
                    break;
                case BlurTypes.Bilateral:
                    CvInvoke.BilateralFilter(BlurInputImage.Image, output, 0, 0, 0, BorderType.Reflect101);
                    break;
            }
            BlurOutputImage.Image = output; 
        }
        #endregion
        #region Contours
        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            ContourInputImage.Image = images[ContourImageSelect.SelectedItem.ToString()];
        }
        private void button8_Click(object sender, EventArgs e)
        {
            SaveInfo(ContourSaveText, ContourOutputImage); 
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentContourType = ContourTypes[ContourTypeSelect.SelectedItem.ToString()];
            if(ContourTypeSelect.SelectedItem.ToString() == "ChainApprox")
            {
                ChainApproxSelect.Show();
                BoundedShapeSelect.Hide(); 
            }
            else if(ContourTypeSelect.SelectedItem.ToString() == "Bounded")
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
            Mat first = ContourInputImage.Image as Mat;
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
                    var temp = ContourInputImage.Image as Mat;
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
                            ContourOutputImage.Image = original;
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
                            ContourOutputImage.Image = original;
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
                            ContourOutputImage.Image = original; 
                            break;
                        default:
                            MessageBox.Show("CHOOSE A METHOD");
                            return; 
                    }
                    break;
                case ContourType.ChainApprox:
                    Mat grayScaledImage2 = new Mat();
                    var temp2 = ContourInputImage.Image as Mat;
                    Mat original2 = temp2;
                    CvInvoke.CvtColor(original2, grayScaledImage2, ColorConversion.Bgr2Gray);
                    VectorOfVectorOfPoint contours2 = new VectorOfVectorOfPoint();
                    Mat nextLayer2 = new Mat();
                    CvInvoke.FindContours(grayScaledImage2, contours2, nextLayer2, RetrType.External, (Emgu.CV.CvEnum.ChainApproxMethod)currentChainApprox);
                    CvInvoke.DrawContours(original2, contours2, -1, new MCvScalar((double)BlueSelect.Value, (double)GreenSelect.Value, (double)RedSelect.Value), 3); 
                    ContourOutputImage.Image = original2;
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
        private void label17_Click(object sender, EventArgs e)
        {

        }
        private void label18_Click(object sender, EventArgs e)
        {

        }
        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #region Color Convert
        private void ColorShiftImageSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ColorCVTInput.Image = images[ColorShiftImageSelect.SelectedItem.ToString()];
        }
        private void ColorShiftOperationSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentColorShift = colorshiftOperations[ColorShiftOperationSelect.SelectedItem.ToString()];

            Mat output = new Mat();
            Mat temp = ColorCVTInput.Image as Mat;
            Stopwatch clock = Stopwatch.StartNew();
            switch (currentColorShift)
            {
                case ColorShiftOperators.BGR_GRAY:
                    CvInvoke.CvtColor(temp, output, ColorConversion.Bgr2Gray);
                    ColorCVTOutput.Image = output;
                    break;
                case ColorShiftOperators.BGR_HSV:
                    if(temp.NumberOfChannels != 3)
                    {
                        MessageBox.Show("MUST HAVE 3 CHANNELSZ!!!!");
                        return; 
                    }
                    CvInvoke.CvtColor(temp, output, ColorConversion.Bgr2Hsv);
                    ColorCVTOutput.Image = output;
                    break;
                case ColorShiftOperators.HSV_BGR:
                    if (temp.NumberOfChannels != 3)
                    {
                        MessageBox.Show("MUST HAVE 3 CHANNELSZ!!!!");
                        return;
                    }
                    CvInvoke.CvtColor(temp, output, ColorConversion.Hsv2Bgr);
                    ColorCVTOutput.Image = output;
                    break;
            }
            clock.Stop();
            label19.Text = $"{clock.ElapsedMilliseconds} ms";

        }
        private void button7_Click_1(object sender, EventArgs e)
        {
            SaveInfo(ColorCVTSaveText, ColorCVTOutput);
        }


        #endregion
        #region Camera
        private void comboBox9_SelectedIndexChanged_2(object sender, EventArgs e)
        {
           
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            Mat temp = (Mat)imageBox15.Image;
            CameraSaveBox.Image = temp.Clone();
            SaveInfo(CameraImageText, CameraSaveBox);  
        }
        private void tabPage9_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #region Spot the difference
        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            SpotTheDifferenceInputImage.Image = images[SpotDiffSelectionBox.SelectedItem.ToString()];
        }
        private void button11_Click(object sender, EventArgs e)
        {
            SaveInfo(ThresholdMaxValueText, SpotTheDifferenceTopOutput); 
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Mat original = new Mat();
            CvInvoke.CvtColor(SpotTheDifferenceInputImage.Image, original, ColorConversion.Bgr2Gray);
            CvInvoke.Threshold(original, original, 1, 255, (Emgu.CV.CvEnum.ThresholdType)ThresholdType.Binary);
            VectorOfVectorOfPoint imageCounter = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.FindContours(original, imageCounter, nextLayer, RetrType.External, (Emgu.CV.CvEnum.ChainApproxMethod)ChainApproxMethod.ChainApproxNone);
            Rectangle Rectangle1 = CvInvoke.BoundingRectangle(imageCounter[0]);
            Rectangle Rectangle2 = CvInvoke.BoundingRectangle(imageCounter[1]);
            Rectangle1.Size = Rectangle2.Size;
            Mat topImage = new Mat((Mat)SpotTheDifferenceInputImage.Image, Rectangle1);
            Mat bottomImage = new Mat((Mat)SpotTheDifferenceInputImage.Image, Rectangle2);
            this.bottomImage.Image = topImage;
            this.topImage.Image = bottomImage;
            Mat ImageDifference = new Mat();
            CvInvoke.AbsDiff(topImage, bottomImage, ImageDifference);
            Mat GrayScaleDifference = new Mat();
            CvInvoke.CvtColor(ImageDifference, GrayScaleDifference, ColorConversion.Bgr2Gray);
            CvInvoke.MedianBlur(GrayScaleDifference, GrayScaleDifference, 5);
            CvInvoke.Threshold(GrayScaleDifference, GrayScaleDifference, (double)SpotTheDifferenceSensitivity.Value, 255, (Emgu.CV.CvEnum.ThresholdType)ThresholdType.Binary);
            VectorOfVectorOfPoint ContourDifferenceArray = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(GrayScaleDifference, ContourDifferenceArray, nextLayer, RetrType.External, (Emgu.CV.CvEnum.ChainApproxMethod)ChainApproxMethod.ChainApproxNone);
            MCvScalar contourColor = new MCvScalar(255, 0, 0);
            for (int i = 0; i < ContourDifferenceArray.Size; i++)
            {
                Rectangle bounds = CvInvoke.BoundingRectangle(ContourDifferenceArray[i]);
                bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                CvInvoke.Rectangle(topImage, bounds, contourColor, 2);
                CvInvoke.Rectangle(bottomImage, bounds, contourColor, 2);
            }

            SpotTheDifferenceTopOutput.Image = topImage; ;
            SpotTheDifferenceOutputImage.Image = bottomImage; 

        }
        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {

        }
        #endregion
        #region Affine
        VectorOfPoint FindLargestContour(VectorOfVectorOfPoint contours)
        {
            VectorOfPoint largest = new VectorOfPoint();
            if(contours.Size == 0)
            {
                return null;
            }
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
        private void button14_Click(object sender, EventArgs e)
        {
            
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            imageBox21.Image = AffineTransformInputImage.Image;
            Mat temp = imageBox21.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown8.Value, (double)numericUpDown9.Value, (double)numericUpDown10.Value), (ScalarArray)new MCvScalar((double)numericUpDown13.Value, (double)numericUpDown12.Value, (double)numericUpDown11.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return;
            }
            VectorOfPoint contour = FindLargestContour(contours);
            RotatedRect rect = CvInvoke.MinAreaRect(contour);
            var verticies = rect.GetVertices().Select(curr => new Point((int)curr.X, (int)curr.Y)).ToArray();

            CvInvoke.Line(imageBox21.Image as Mat, verticies[0], verticies[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[1], verticies[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[2], verticies[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[3], verticies[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[3];
            PointF[] outputPoints = new PointF[3];
            points1[0] = verticies[1];
            points1[1] = verticies[2];
            points1[2] = verticies[0];
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(AffineTransformImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, AffineTransformImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetAffineTransform(points1, outputPoints);
            CvInvoke.WarpAffine(AffineTransformInputImage.Image, output, transformMatrix, AffineTransformImageOutput.Size);
            imageBox22.Image = mask;
            AffineTransformImageOutput.Image = output;
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            imageBox21.Image = AffineTransformInputImage.Image;
            Mat temp = imageBox21.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown8.Value, (double)numericUpDown9.Value, (double)numericUpDown10.Value), (ScalarArray)new MCvScalar((double)numericUpDown13.Value, (double)numericUpDown12.Value, (double)numericUpDown11.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return;
            }
            VectorOfPoint contour = FindLargestContour(contours);
            RotatedRect rect = CvInvoke.MinAreaRect(contour);
            var verticies = rect.GetVertices().Select(curr => new Point((int)curr.X, (int)curr.Y)).ToArray();

            CvInvoke.Line(imageBox21.Image as Mat, verticies[0], verticies[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[1], verticies[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[2], verticies[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[3], verticies[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[3];
            PointF[] outputPoints = new PointF[3];
            points1[0] = verticies[1];
            points1[1] = verticies[2];
            points1[2] = verticies[0];
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(AffineTransformImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, AffineTransformImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetAffineTransform(points1, outputPoints);
            CvInvoke.WarpAffine(AffineTransformInputImage.Image, output, transformMatrix, AffineTransformImageOutput.Size);
            imageBox22.Image = mask;
            AffineTransformImageOutput.Image = output;
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            imageBox21.Image = AffineTransformInputImage.Image;
            Mat temp = imageBox21.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown8.Value, (double)numericUpDown9.Value, (double)numericUpDown10.Value), (ScalarArray)new MCvScalar((double)numericUpDown13.Value, (double)numericUpDown12.Value, (double)numericUpDown11.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return;
            }
            VectorOfPoint contour = FindLargestContour(contours);
            RotatedRect rect = CvInvoke.MinAreaRect(contour);
            var verticies = rect.GetVertices().Select(curr => new Point((int)curr.X, (int)curr.Y)).ToArray();

            CvInvoke.Line(imageBox21.Image as Mat, verticies[0], verticies[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[1], verticies[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[2], verticies[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[3], verticies[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[3];
            PointF[] outputPoints = new PointF[3];
            points1[0] = verticies[1];
            points1[1] = verticies[2];
            points1[2] = verticies[0];
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(AffineTransformImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, AffineTransformImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetAffineTransform(points1, outputPoints);
            CvInvoke.WarpAffine(AffineTransformInputImage.Image, output, transformMatrix, AffineTransformImageOutput.Size);
            imageBox22.Image = mask;
            AffineTransformImageOutput.Image = output;
        }


        private void numericUpDown13_ValueChanged(object sender, EventArgs e)
        {
            imageBox21.Image = AffineTransformInputImage.Image;
            Mat temp = imageBox21.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown8.Value, (double)numericUpDown9.Value, (double)numericUpDown10.Value), (ScalarArray)new MCvScalar((double)numericUpDown13.Value, (double)numericUpDown12.Value, (double)numericUpDown11.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return;
            }
            VectorOfPoint contour = FindLargestContour(contours);
            RotatedRect rect = CvInvoke.MinAreaRect(contour);
            var verticies = rect.GetVertices().Select(curr => new Point((int)curr.X, (int)curr.Y)).ToArray();

            CvInvoke.Line(imageBox21.Image as Mat, verticies[0], verticies[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[1], verticies[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[2], verticies[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[3], verticies[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[3];
            PointF[] outputPoints = new PointF[3];
            points1[0] = verticies[1];
            points1[1] = verticies[2];
            points1[2] = verticies[0];
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(AffineTransformImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, AffineTransformImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetAffineTransform(points1, outputPoints);
            CvInvoke.WarpAffine(AffineTransformInputImage.Image, output, transformMatrix, AffineTransformImageOutput.Size);
            imageBox22.Image = mask;
            AffineTransformImageOutput.Image = output;
        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
            imageBox21.Image = AffineTransformInputImage.Image;
            Mat temp = imageBox21.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown8.Value, (double)numericUpDown9.Value, (double)numericUpDown10.Value), (ScalarArray)new MCvScalar((double)numericUpDown13.Value, (double)numericUpDown12.Value, (double)numericUpDown11.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return;
            }
            VectorOfPoint contour = FindLargestContour(contours);
            RotatedRect rect = CvInvoke.MinAreaRect(contour);
            var verticies = rect.GetVertices().Select(curr => new Point((int)curr.X, (int)curr.Y)).ToArray();

            CvInvoke.Line(imageBox21.Image as Mat, verticies[0], verticies[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[1], verticies[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[2], verticies[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[3], verticies[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[3];
            PointF[] outputPoints = new PointF[3];
            points1[0] = verticies[1];
            points1[1] = verticies[2];
            points1[2] = verticies[0];
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(AffineTransformImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, AffineTransformImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetAffineTransform(points1, outputPoints);
            CvInvoke.WarpAffine(AffineTransformInputImage.Image, output, transformMatrix, AffineTransformImageOutput.Size);
            imageBox22.Image = mask;
            AffineTransformImageOutput.Image = output;
        }
        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            imageBox21.Image = AffineTransformInputImage.Image;
            Mat temp = imageBox21.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown8.Value, (double)numericUpDown9.Value, (double)numericUpDown10.Value), (ScalarArray)new MCvScalar((double)numericUpDown13.Value, (double)numericUpDown12.Value, (double)numericUpDown11.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return;
            }
            VectorOfPoint contour = FindLargestContour(contours);
            RotatedRect rect = CvInvoke.MinAreaRect(contour);
            var verticies = rect.GetVertices().Select(curr => new Point((int)curr.X, (int)curr.Y)).ToArray();

            CvInvoke.Line(imageBox21.Image as Mat, verticies[0], verticies[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[1], verticies[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[2], verticies[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(imageBox21.Image as Mat, verticies[3], verticies[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[3];
            PointF[] outputPoints = new PointF[3];
            
            points1[0] = verticies[1];
            points1[1] = verticies[2];
            points1[2] = verticies[0];
            
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(AffineTransformImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, AffineTransformImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetAffineTransform(points1, outputPoints);
            CvInvoke.WarpAffine(AffineTransformInputImage.Image, output, transformMatrix, AffineTransformImageOutput.Size);
            imageBox22.Image = mask;
            AffineTransformImageOutput.Image = output;
        }


        #endregion
        #region Perspective
        private void numericUpDown19_ValueChanged(object sender, EventArgs e)
        {
            PerspectiveImageInput.Image = PerspectiveImageCamera.Image;
            Mat temp = PerspectiveImageInput.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown19.Value, (double)numericUpDown18.Value, (double)numericUpDown17.Value),
                (ScalarArray)new MCvScalar((double)numericUpDown16.Value, (double)numericUpDown15.Value, (double)numericUpDown14.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return;
            }
            VectorOfPoint contour = FindLargestContour(contours);
            CvInvoke.ApproxPolyDP(contour, contour, 10, true);

            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[0], contour[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[1], contour[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[2], contour[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[3], contour[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[4];
            PointF[] outputPoints = new PointF[4];
            points1[0] = contour[1];
            points1[1] = contour[2];
            points1[2] = contour[0];
            points1[3] = contour[3];
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(PerspectiveImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, PerspectiveImageOutput.Size.Height);
            outputPoints[3] = new PointF(PerspectiveImageOutput.Size.Width, PerspectiveImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetPerspectiveTransform(points1, outputPoints);
            CvInvoke.WarpPerspective(PerspectiveImageInput.Image, output, transformMatrix, PerspectiveImageOutput.Size);
            PerspectiveMask.Image = mask;
            PerspectiveImageOutput.Image = output;
        }
        private void numericUpDown18_ValueChanged(object sender, EventArgs e)
        {
            PerspectiveImageInput.Image = PerspectiveImageCamera.Image;
            Mat temp = PerspectiveImageInput.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown19.Value, (double)numericUpDown18.Value, (double)numericUpDown17.Value),
                (ScalarArray)new MCvScalar((double)numericUpDown16.Value, (double)numericUpDown15.Value, (double)numericUpDown14.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return;
            }
            VectorOfPoint contour = FindLargestContour(contours);
            CvInvoke.ApproxPolyDP(contour, contour, 10, true);

            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[0], contour[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[1], contour[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[2], contour[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[3], contour[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[4];
            PointF[] outputPoints = new PointF[4];
            points1[0] = contour[1];
            points1[1] = contour[2];
            points1[2] = contour[0];
            points1[3] = contour[3];
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(PerspectiveImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, PerspectiveImageOutput.Size.Height);
            outputPoints[3] = new PointF(PerspectiveImageOutput.Size.Width, PerspectiveImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetPerspectiveTransform(points1, outputPoints);
            CvInvoke.WarpPerspective(PerspectiveImageInput.Image, output, transformMatrix, PerspectiveImageOutput.Size);
            PerspectiveMask.Image = mask;
            PerspectiveImageOutput.Image = output;
        }
        private void numericUpDown17_ValueChanged(object sender, EventArgs e)
        {
            PerspectiveImageInput.Image = PerspectiveImageCamera.Image;
            Mat temp = PerspectiveImageInput.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown19.Value, (double)numericUpDown18.Value, (double)numericUpDown17.Value),
                (ScalarArray)new MCvScalar((double)numericUpDown16.Value, (double)numericUpDown15.Value, (double)numericUpDown14.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return;
            }
            VectorOfPoint contour = FindLargestContour(contours);
            CvInvoke.ApproxPolyDP(contour, contour, 10, true);

            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[0], contour[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[1], contour[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[2], contour[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[3], contour[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[4];
            PointF[] outputPoints = new PointF[4];
            points1[0] = contour[1];
            points1[1] = contour[2];
            points1[2] = contour[0];
            points1[3] = contour[3];
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(PerspectiveImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, PerspectiveImageOutput.Size.Height);
            outputPoints[3] = new PointF(PerspectiveImageOutput.Size.Width, PerspectiveImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetPerspectiveTransform(points1, outputPoints);
            CvInvoke.WarpPerspective(PerspectiveImageInput.Image, output, transformMatrix, PerspectiveImageOutput.Size);
            PerspectiveMask.Image = mask;
            PerspectiveImageOutput.Image = output;
        }
        private void numericUpDown16_ValueChanged(object sender, EventArgs e)
        {
            PerspectiveImageInput.Image = PerspectiveImageCamera.Image;
            Mat temp = PerspectiveImageInput.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown19.Value, (double)numericUpDown18.Value, (double)numericUpDown17.Value),
                (ScalarArray)new MCvScalar((double)numericUpDown16.Value, (double)numericUpDown15.Value, (double)numericUpDown14.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return;
            }
            VectorOfPoint contour = FindLargestContour(contours);
            CvInvoke.ApproxPolyDP(contour, contour, 10, true);

            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[0], contour[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[1], contour[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[2], contour[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[3], contour[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[4];
            PointF[] outputPoints = new PointF[4];
            points1[0] = contour[1];
            points1[1] = contour[2];
            points1[2] = contour[0];
            points1[3] = contour[3];
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(PerspectiveImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, PerspectiveImageOutput.Size.Height);
            outputPoints[3] = new PointF(PerspectiveImageOutput.Size.Width, PerspectiveImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetPerspectiveTransform(points1, outputPoints);
            CvInvoke.WarpPerspective(PerspectiveImageInput.Image, output, transformMatrix, PerspectiveImageOutput.Size);
            PerspectiveMask.Image = mask;
            PerspectiveImageOutput.Image = output;
        }
        private void numericUpDown15_ValueChanged(object sender, EventArgs e)
        {
            PerspectiveImageInput.Image = PerspectiveImageCamera.Image;
            Mat temp = PerspectiveImageInput.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown19.Value, (double)numericUpDown18.Value, (double)numericUpDown17.Value),
                (ScalarArray)new MCvScalar((double)numericUpDown16.Value, (double)numericUpDown15.Value, (double)numericUpDown14.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return;
            }
            VectorOfPoint contour = FindLargestContour(contours);
            CvInvoke.ApproxPolyDP(contour, contour, 10, true);

            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[0], contour[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[1], contour[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[2], contour[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[3], contour[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[4];
            PointF[] outputPoints = new PointF[4];
            points1[0] = contour[1];
            points1[1] = contour[2];
            points1[2] = contour[0];
            points1[3] = contour[3];
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(PerspectiveImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, PerspectiveImageOutput.Size.Height);
            outputPoints[3] = new PointF(PerspectiveImageOutput.Size.Width, PerspectiveImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetPerspectiveTransform(points1, outputPoints);
            CvInvoke.WarpPerspective(PerspectiveImageInput.Image, output, transformMatrix, PerspectiveImageOutput.Size);
            PerspectiveMask.Image = mask;
            PerspectiveImageOutput.Image = output;
        }
        private void numericUpDown14_ValueChanged(object sender, EventArgs e)
        {
            PerspectiveImageInput.Image = PerspectiveImageCamera.Image;
            Mat temp = PerspectiveImageInput.Image as Mat;
            Mat mask = temp.Clone();
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray)new MCvScalar((double)numericUpDown19.Value, (double)numericUpDown18.Value, (double)numericUpDown17.Value),
                (ScalarArray)new MCvScalar((double)numericUpDown16.Value, (double)numericUpDown15.Value, (double)numericUpDown14.Value), mask);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.MedianBlur(mask, mask, 5);

            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            if (contours.Size == 0)
            {
                MessageBox.Show("no contours");
                return; 
            }
            VectorOfPoint contour = FindLargestContour(contours);
            CvInvoke.ApproxPolyDP(contour, contour, 10, true);

            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[0], contour[1], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[1], contour[2], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[2], contour[3], new MCvScalar(255, 0, 0), 2);
            CvInvoke.Line(PerspectiveImageInput.Image as Mat, contour[3], contour[0], new MCvScalar(255, 0, 0), 2);

            PointF[] points1 = new PointF[4];
            PointF[] outputPoints = new PointF[4];
            points1[0] = contour[1];
            points1[1] = contour[2];
            points1[2] = contour[0];
            points1[3] = contour[3];
            outputPoints[0] = new PointF(0, 0);
            outputPoints[1] = new PointF(PerspectiveImageOutput.Size.Width, 0);
            outputPoints[2] = new PointF(0, PerspectiveImageOutput.Size.Height);
            outputPoints[3] = new PointF(PerspectiveImageOutput.Size.Width, PerspectiveImageOutput.Size.Height);
            Mat output = new Mat();
            Mat transformMatrix = CvInvoke.GetPerspectiveTransform(points1, outputPoints);
            CvInvoke.WarpPerspective(PerspectiveImageInput.Image, output, transformMatrix, PerspectiveImageOutput.Size);
            PerspectiveMask.Image = mask;
            PerspectiveImageOutput.Image = output;
        }

        #endregion
        
        #region facetracking
        private void timer1_Tick(object sender, EventArgs e)
        {
            Mat currentFrame = capture.QueryFrame();
            CvInvoke.Flip(currentFrame, currentFrame, FlipType.Horizontal);
            Mat mask = currentFrame.Clone();
            Mat finalImage = mask.Clone(); 
            EyeTrackCameraInput.Image = mask;
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(mask, (ScalarArray) new MCvScalar(10, 66, 32), (ScalarArray) new MCvScalar(33, 170, 101), mask);
            CvInvoke.MedianBlur(mask, mask, 5);
            Mat element1 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            CvInvoke.Erode(mask, mask, element1, new Point(-1, -1), 1, BorderType.Constant, new MCvScalar(0, 0, 0));
            CvInvoke.MedianBlur(mask, mask, 5);
            Mat element2 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(10, 10), new Point(-1, -1));
            CvInvoke.Dilate(mask, mask, element2, new Point(-1, -1), 1, BorderType.Constant, new MCvScalar(0, 0, 0));
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat nextLayer = new Mat();
            CvInvoke.FindContours(mask, contours, nextLayer, RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            EyeTrackMaskImage.Image = mask;
            VectorOfPoint contour = FindLargestContour(contours); 
            Rectangle rect = CvInvoke.BoundingRectangle(contour);
            CvInvoke.Rectangle(finalImage, rect, new MCvScalar(255, 0, 0), 5);
            EyeTrackFinalImage.Image = finalImage; 
        }

        private void button19_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled; 
        }
        #endregion

        private void button16_Click(object sender, EventArgs e)
        {
            SaveInfo(PerspectiveTextSave, PerspectiveImageOutput); 
        }

        private void button18_Click(object sender, EventArgs e)
        {
            SaveInfo(DilateErodeSaveText, DilateErodeOutputImage);

        }
    }
}
