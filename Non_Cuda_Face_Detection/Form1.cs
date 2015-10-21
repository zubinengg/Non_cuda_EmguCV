using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;


namespace Non_Cuda_Face_Detection
{
    // detailed comments pending
    public partial class Form1 : Form
    {
        private HaarCascade haar;
        //private CascadeClassifier eye;
        private HaarCascade eye;
        //private CascadeClassifier haar1;


        public Form1()
        {
            InitializeComponent();
            // I am using haar cassade functions for face detaction non cuda engucv
            haar = new HaarCascade(@"C:\Emgu\emgucv-windows-universal 2.4.10.1940\opencv\data\haarcascades\haarcascade_frontalface_default.xml");
            // the initial values for optimal haar cascasde function are loadad in the text box unesr can vary..... I will remove it later
            this.textBox1.Text = "1.2";
            this.textBox2.Text = "4";
            this.textBox3.Text = "25";
            this.textBox4.Text = @"F:\research\1.tif";
            this.textBox6.Text = "1";
            //My Mod
            //haar1 = new CascadeClassifier("haarcascade_frontalface_default.xml");
            //eye = new CascadeClassifier("haarcascade_eye_tree_eyeglasses.xml");
            // I am using haar cassade functions for eye detaction 
            // the basic objective is for the face alignment.... 
            eye = new HaarCascade(@"C:\Emgu\emgucv-windows-universal 2.4.10.1940\opencv\data\haarcascades\haarcascade_eye.xml");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }



        Bitmap NormalizeLbpMatrix(double[,] Mat, Bitmap lbp, double max)
        {
            int NumRow = lbp.Height;
            int numCol = lbp.Width;
            for (int i = 0; i < NumRow; i++)
            {
                for (int j = 0; j < numCol; j++)
                {
                    // see the Normalization process of dividing pixel by max value and multiplying with 255
                    double d = Mat[j, i] / max;
                    int v = (int)(d * 255);
                    Color c = Color.FromArgb(v, v, v);
                    lbp.SetPixel(j, i, c);
                }
            }
            return lbp;
        }
        double Bin2Dec(List<int> vals)
        {
            double mult = 256;
            double sum = 0;
            //Boolean test = false;
            foreach (int v in vals)
            {
                sum += mult * v;
                mult /= 2;
            }
            return sum;
        }
        Bitmap LBP(Bitmap srcBmp, int R)
        {
            // We want to get LBP image from srcBmp and window R
            Bitmap bmp = srcBmp;
            //1. Extract rows and columns from srcImage . Note Source image is Gray scale Converted Image
            int NumRow = srcBmp.Height;
            int numCol = srcBmp.Width;
            Bitmap lbp = new Bitmap(numCol, NumRow);
            Bitmap GRAY = new Bitmap(numCol, NumRow);// GRAY is the resultant matrix 
            double[,] MAT = new double[numCol, NumRow];
            double max = 0.0;
            //2. Loop through Pixels
            for (int i = 0; i < NumRow; i++)
            {
                for (int j = 0; j < numCol; j++)
                {
                    //  Color c1=Color.FromArgb(0,0,0);
                    MAT[j, i] = 0;
                    //lbp.SetPixel(j, i,c1) ;


                    //define boundary condition, other wise say if you are looking at pixel (0,0), it does not have any suitable neighbors
                    if ((i > R) && (j > R) && (i < (NumRow - R)) && (j < (numCol - R)))
                    {
                        // we want to store binary values in a List
                        List<int> vals = new List<int>();
                        try
                        {
                            for (int i1 = i - R; i1 < (i + R); i1++)
                            {
                                for (int j1 = j - R; j1 < (j + R); j1++)
                                {
                                    int acPixel = srcBmp.GetPixel(j, i).R;
                                    int nbrPixel = srcBmp.GetPixel(j1, i1).R;
                                    // 3. This is the main Logic of LBP
                                    if (nbrPixel > acPixel)
                                    {
                                        vals.Add(1);
                                    }
                                    else
                                    {
                                        vals.Add(0);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        //4. Once we have a list of 1's and 0's , convert the list to decimal
                        // Also for normalization purpose calculate Max value
                        double d1 = Bin2Dec(vals);
                        MAT[j, i] = d1;
                        if (d1 > max)
                        {
                            max = d1;
                        }
                    }

                    //////////////////
                }
            }
            //5. Normalize LBP matrix MAT an obtain LBP image lbp
            lbp = NormalizeLbpMatrix(MAT, lbp, max);
            return lbp;
        }


        private int [] get_vectors(Bitmap srcBmp)
        {
            return new int[] { 1, 2 };
        }

        private void set_image(Image<Bgr, Byte> source, Rectangle crop)
        {
            try
            {
                Image<Bgr, Byte> ImageFrame = new Image<Bgr, Byte>(this.textBox4.Text.ToString());
                //Image<Bgr, Byte> region=source;
                ImageFrame.ROI = crop;
                imageBox2.Image = ImageFrame.Convert<Gray, byte>();
                imageBox3.SizeMode = PictureBoxSizeMode.StretchImage;
                Image<Bgr, Byte> LBP_Image = new Image<Bgr, Byte>(LBP(ImageFrame.ToBitmap(), Convert.ToInt16(this.textBox6.Text.ToString())));
                imageBox3.Image = LBP_Image;
                imageBox4.SizeMode = PictureBoxSizeMode.Zoom;
                //Image<Bgr, Byte> LBP_Image_Resized = resizeImage(LBP_Image.ToBitmap, new Size(50, 50));
                Image<Bgr, Byte> resizedImage = LBP_Image.Resize(150, 150, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                imageBox4.Image = resizedImage;
                this.label10.Text = "Given Face Resolution =" + resizedImage.Width.ToString() + " X " + resizedImage.Height.ToString();

                // TESTING FOR COMMIT

                //@"F:\research\1.tif"
                //resizedImage.ToBitmap
                if (File.Exists(@"F:\research\first_done.jpg"))
                {
                    File.Delete(@"F:\research\first_done.jpg");
                }

                resizedImage.ToBitmap().Save(@"F:\research\first_done.jpg");

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
                //var eyes = eye.DetectMultiScale(grayframe, 1.2, 1, new Size(25, 25), new Size(500, 500));
                var eyes = grayframe.DetectHaarCascade(eye, 1.1, 2, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(25, 25));




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
                    set_image(ImageFrame, bigFace);
                    this.label9.Text = "Given Face Resolution =" + bigFace.Height.ToString() + "x" + bigFace.Width.ToString();
                    //ImageFrame.ROI = bigFace;
                }


                if (eyes.Length > 0)
                {
                    int loop = 0;
                    foreach (var eyesnap in eyes[0])
                    {
                        //Rectangle eyeRect = eyesnap.rect;
                        ImageFrame.Draw(eyesnap.rect, new Bgr(Color.Blue), 2);
                        loop += 1;
                    }
                    this.label7.Text = "Number of Eyes detected = " + (eyes.Length).ToString();
                    //this.label7.Text = "Number of Eyes detected = " + (loop).ToString();
                }

            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}