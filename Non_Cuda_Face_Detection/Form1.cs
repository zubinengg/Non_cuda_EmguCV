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


        public Form1()
        {
            InitializeComponent();
            haar = new HaarCascade("haarcascade_frontalface_default.xml");
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
                imageBox2.Image = source;
            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = "W:\\research\\1.tif";
            imageBox1.SizeMode = PictureBoxSizeMode.Zoom;
            imageBox2.SizeMode = PictureBoxSizeMode.Zoom;
            Image<Bgr, Byte> ImageFrame = new Image<Bgr, Byte>(@"F:\research\1.jpg");
            imageBox1.Image = ImageFrame;
            
            if (ImageFrame != null)
            {
                Image<Gray, byte> grayframe = ImageFrame.Convert<Gray, byte>();
                var faces = grayframe.DetectHaarCascade(haar, 1.4, 6,
                    HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                    new Size(25, 25))[0];
                //var temp = grayframe.DetectHaarCascade(haar, 1.4 ,6, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE,)
                foreach (var face in faces)
                {
                    ImageFrame.Draw(face.rect, new Bgr(Color.Green), 3);
                    set_image(ImageFrame, face.rect);
                }

            }             

        }
    }
}
