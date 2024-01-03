
namespace PongIRL
{
    partial class Form2
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.CameraBox = new Emgu.CV.UI.ImageBox();
            this.BallBox = new Emgu.CV.UI.ImageBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.CameraBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BallBox)).BeginInit();
            this.SuspendLayout();
            // 
            // CameraBox
            // 
            this.CameraBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CameraBox.Location = new System.Drawing.Point(0, 0);
            this.CameraBox.Name = "CameraBox";
            this.CameraBox.Size = new System.Drawing.Size(624, 441);
            this.CameraBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.CameraBox.TabIndex = 2;
            this.CameraBox.TabStop = false;
            this.CameraBox.Click += new System.EventHandler(this.imageBox1_Click);
            // 
            // BallBox
            // 
            this.BallBox.Location = new System.Drawing.Point(384, 219);
            this.BallBox.Name = "BallBox";
            this.BallBox.Size = new System.Drawing.Size(79, 71);
            this.BallBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.BallBox.TabIndex = 2;
            this.BallBox.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 17;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.BallBox);
            this.Controls.Add(this.CameraBox);
            this.Name = "Form2";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.CameraBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BallBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox CameraBox;
        private Emgu.CV.UI.ImageBox BallBox;
        private System.Windows.Forms.Timer timer1;
    }
}

