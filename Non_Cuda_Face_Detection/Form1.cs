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
        private HaarCascade eye;
        //private CascadeClassifier haar1;


        public Form1()
        {
            InitializeComponent();
            haar = new HaarCascade("haarcascade_frontalface_default.xml");
            this.textBox1.Text = "1.2";
            this.textBox2.Text = "4";
            this.textBox3.Text = "25";
            this.textBox4.Text = @"F:\research\1.jpg";
            //haar1 = new CascadeClassifier("haarcascade_frontalface_default.xml");
            eye = new HaarCascade("haarcascade_eye_tree_eyeglasses.xml");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        /*
        public static Image<Gray, byte> AlignEyes(Image<Gray, byte> image)
        {
            Rectangle[] eyes = EyeClassifier.DetectMultiScale(image, 1.4, 0, new Size(1, 1), new Size(50, 50));
            var unifiedEyes = CombineOverlappingRectangles(eyes).OrderBy(r => r.X).ToList();
            if (unifiedEyes.Count == 2)
            {
                var deltaY = (unifiedEyes[1].Y + unifiedEyes[1].Height / 2) - (unifiedEyes[0].Y + unifiedEyes[0].Height / 2);
                var deltaX = (unifiedEyes[1].X + unifiedEyes[1].Width / 2) - (unifiedEyes[0].X + unifiedEyes[0].Width / 2);
                double degrees = Math.Atan2(deltaY, deltaX) * 180 / Math.PI;
                if (Math.Abs(degrees) < 35)
                {
                    image = image.Rotate(-degrees, new Gray(0));
                }
            }
            return image;
        }
        */

        private void set_image(Image<Bgr, Byte> source, Rectangle crop)
        {
            try
            {
                source.ROI = crop;
                imageBox2.Image = source;
                // TESTING FOR COMMIT
            }
            catch
            {

            }
        }

        private Rectangle get_big_rect(Rectangle A, Rectangle B)
        {
            if (A.Width * A.Height > B.Width * B.Height)
            {
                return A;
            }
            return B;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            imageBox1.SizeMode = PictureBoxSizeMode.Zoom;
            imageBox2.SizeMode = PictureBoxSizeMode.Zoom;
            Image<Bgr, Byte> ImageFrame = new Image<Bgr, Byte>(this.textBox4.Text.ToString());
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

                //Eyes 
                var eyes = grayframe.DetectHaarCascade(eye, 1.1, 4, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(25, 25));
                


                //var faces = greyframe.CascadeClassifier(haar1, 1.4, 6, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(25, 25))[0];

                Rectangle bigFace = new Rectangle(0, 0, 0, 0);
                if (faces.Length > 0)
                {
                    foreach (var face in faces)
                    {
                        ImageFrame.Draw(face.rect, new Bgr(Color.Green), 3);
                        bigFace = get_big_rect(bigFace, face.rect);
                    }
                    
                    this.label5.Text = "Number of Face detected = " + faces.Length.ToString();
                }
                if (eyes.Length>0)
                {
                    int loop = 0;
                    foreach(var eyesnap in eyes[0])
                    {
                        //Rectangle eyeRect = eyesnap.rect;
                        ImageFrame.Draw(eyesnap.rect, new Bgr(Color.Blue), 2);
                        loop += 1;
                    }
                    this.label7.Text = "Number of Eyes detected = " + (eyes.Length*2).ToString();
                    this.label7.Text = "Number of Eyes detected = " + (loop).ToString();
                }
                set_image(ImageFrame, bigFace);
            }
        }
    }
}
