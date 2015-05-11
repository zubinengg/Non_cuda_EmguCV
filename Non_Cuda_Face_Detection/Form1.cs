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
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;


namespace Non_Cuda_Face_Detection
{
    public partial class Form1 : Form
    {
        private HaarCascade haar;
        private CascadeClassifier haar1;


        public Form1()
        {
            InitializeComponent();
            haar = new HaarCascade("haarcascade_frontalface_default.xml");
            this.textBox1.Text = "1.2";
            this.textBox2.Text = "4";
            this.textBox3.Text = "25";
            //haar1 = new CascadeClassifier("haarcascade_frontalface_default.xml");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }


        private void set_image(Image<Bgr, Byte> source, Rectangle crop)
        {
            try
            {
                source.ROI = crop;
                imageBox2.Image = source;
            }
            catch
            {

            }
        }

        private Rectangle get_big_rect(Rectangle A,Rectangle B)
        {
            if (A.Width*A.Height>B.Width*B.Height)
            {
                return A;
            }
            return B;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = "W:\\research\\1.tif";
            imageBox1.SizeMode = PictureBoxSizeMode.Zoom;
            imageBox2.SizeMode = PictureBoxSizeMode.Zoom;
            Image<Bgr, Byte> ImageFrame = new Image<Bgr, Byte>(@"W:\research\2.jpg");
            imageBox1.Image = ImageFrame;
            
            if (ImageFrame != null)
            {
                double scaleIncreaseRate = Convert.ToDouble(this.textBox1.Text.ToString());
                int minNeighbours = Convert.ToInt16(this.textBox2.Text.ToString());
                int minDetectionScale = Convert.ToInt16(this.textBox3.Text.ToString());
                Image<Gray, byte> grayframe = ImageFrame.Convert<Gray, byte>();
                //var faces = grayframe.DetectHaarCascade(haar, 1.4, 6,HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,new Size(25, 25))[0];
                var faces = grayframe.DetectHaarCascade(haar,
                    scaleIncreaseRate,
                    minNeighbours, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                    new Size(minDetectionScale, minDetectionScale))[0];
                //var faces = greyframe.CascadeClassifier(haar1, 1.4, 6, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(25, 25))[0];
                
                Rectangle bigFace= new Rectangle(0,0,0,0);
                if (faces.Length > 0)
                {
                    foreach (var face in faces)
                    {
                        ImageFrame.Draw(face.rect, new Bgr(Color.Green), 3);
                        bigFace = get_big_rect(bigFace, face.rect);
                    }
                    set_image(ImageFrame, bigFace);
                    this.label5.Text = "Number of Face detected = " + faces.Length.ToString();
                }
            }             

        }
    }
}
