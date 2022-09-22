using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steganografi
{
    public partial class Form1 : Form
    {
        string paper = "010100110110010101100011011100100110010101110100001000000110110101100101011100110111001101100001011001110110010100100000011010010111001100111010";//prevents unexpected deadlock when user pick non messaged image and decode it
        string salt = "0000000000000000000000000000000000000000";  // end statement for optimization

        DateTime dt = DateTime.Now;//required for adding new image uniqe name
        string binarySecretMessage;
        int pointToByte = 0;//  this little fella gonna point each member of secret messages bytes from startin 0 to n
        string pictureLocation = "";
        string[] newImageName;
        public Form1()
        {
            InitializeComponent();
        }
        private string SolveMessage()
        {
            Bitmap messagedImage = new Bitmap(pictureLocation);

            string curR;
            string curG;
            string curB;

            string binaryMessage = "";
            for (int x = 0; x < messagedImage.Width; x++)
            {
                for (int y = 0; y < messagedImage.Height; y++)
                {
                    //step1 get rgb  
                    Color currColor = messagedImage.GetPixel(x, y);
                    //s2  set rgb to binary  
                     curR = DecimalToBinary(currColor.R);
                     curG = DecimalToBinary(currColor.G);
                     curB = DecimalToBinary(currColor.B);
                    //s3 get last element of this binary ~~which message coded bytes
                    binaryMessage += curR[curR.Length - 1].ToString()+ curG[curG.Length - 1].ToString()+curB[curB.Length - 1].ToString();
                  
                }
                if (!binaryMessage.StartsWith(paper)) {//prevent deadlock cause by decoding uncoded image
                    return "";
                }
            if (binaryMessage.EndsWith(salt)) {//stops program to work more than necessary
                    break;
                    }
            }

            if (binaryMessage.Length % 8 != 0) {//complete binary to %8=0
                int tempnum = 8-(binaryMessage.Length % 8);
                for (int i = 0; i < tempnum; i++) {
                    binaryMessage += "0";
                }
            }
            var data = GetBytesFromBinaryString(binaryMessage);
            var text = Encoding.ASCII.GetString(data);
            //s4 combine them all together and your message ready
            return text;
        }



        public Byte[] GetBytesFromBinaryString(String binary)
        {
            var list = new List<Byte>();

            for (int i = 0; i < binary.Length; i += 8)
            {
                String t = binary.Substring(i, 8);

                list.Add(Convert.ToByte(t, 2));
            }

            return list.ToArray();
        }

        private void HideMessageInPicture(string m)
        {

            //step 1 get the text =>    m 
            int tempR;
            int tempG;
            int tempB;

            //step 2 convert message to binary
             binarySecretMessage = StringToBinary(m);

            Bitmap image = new Bitmap(pictureLocation);//point to picture which we want to hide message in it
            
           Bitmap bmp = new Bitmap(image.Width,image.Height);//fake bitmap for changed pixels can't overwrite, this is the way
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, image.Width, image.Height);
                graph.FillRectangle(Brushes.White, ImageSize);
            }
            
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color currentColor = image.GetPixel(x,y);

                    //step3 for each 3 elemnet of binary message(rgb) gonna be set on image pixel 

                    tempR = Convert.ToByte(TrimLastElement(DecimalToBinary(currentColor.R)),2);

                    tempG = Convert.ToByte(TrimLastElement(DecimalToBinary(currentColor.G)),2);

                    tempB = Convert.ToByte(TrimLastElement(DecimalToBinary(currentColor.B)),2);
                    Color tempColor =  Color.FromArgb(currentColor.A,tempR,tempG,tempB);

                   bmp.SetPixel(x, y, tempColor);
                }
            }
            pointToByte = 0;//allow user to do multiple operations with application
            try
            {
                bmp.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + newImageName[0] + " " + dt.Day + "-" + dt.Month + "-" + dt.Year + " " + dt.Hour + "_" + dt.Minute + "_" + dt.Second + ".jpg");
            }
            catch (System.IO.PathTooLongException e) {//not supported length of name by windows
                bmp.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\newImage.jpg");
            }
        }

        public  String TrimLastElement(string byteMessage) {//change last bit with part of secret message
              string result = "";
            if (pointToByte < binarySecretMessage.Length)
            {
                for (int i = 0; i < byteMessage.Length - 1; i++)
                {
                    result += byteMessage[i];
                }
                result += binarySecretMessage[pointToByte];
                pointToByte++;
                return result;
            }
            else
            {
                for (int i = 0; i < byteMessage.Length - 1; i++)
                {
                    result += byteMessage[i];
                }
                result += "0";
                return result;
            }
                }

        public  string DecimalToBinary(int num) {
            string result = "";
            result = "";
            int remainder;
            while (num > 1)
            {
                remainder = num % 2;
                result = Convert.ToString(remainder) + result;
                num /= 2;
            }
            result = Convert.ToString(num) + result;
            return result;
        }
        public  string StringToBinary(string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return paper+sb.ToString();
        }
        public  string BinaryToString(string data)
        {
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());
        }

        private async void button1_Click(object sender, EventArgs e)//code
        {
                if (pictureLocation != "")
                {
                    string message = textBox1.Text;
                    HideMessageInPicture(message);
                    textBox1.Text = "";
                    button1.ForeColor = Color.Green;
                    await Task.Delay(3000);
                    button1.ForeColor = Color.Black;
                }
            }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
                commonStuffOnPictureBoxAndLabel();
        }


        private void commonStuffOnPictureBoxAndLabel()
        {
            textBox1.Text = "";
            label1.Visible = false;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select an image file.";
            ofd.Filter = "Jpeg Images(*.jpg)|*.jpg";
            ofd.Filter += "|Png Images(*.png)|*.png";
            ofd.Filter += "|Bitmap Images(*.bmp)|*.bmp";
            ofd.Filter += "|All(*.JPG, *.PNG, *.bmp)| *.JPG; *.PNG; *.bmp";
            ofd.FilterIndex = 1;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pictureLocation = ofd.FileName;
                   newImageName = pictureLocation.Split('\\');
                newImageName = newImageName[newImageName.Length - 1].Split('.');//for new image name = newImageName[0]
                Image img = Image.FromFile(pictureLocation);
                pictureBox1.Image = img;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            button1.Visible = true;
            button2.Visible = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            commonStuffOnPictureBoxAndLabel();
        }

        private async void button2_Click(object sender, EventArgs e)//decode
        {
        textBox1.Text = "";
            if (pictureLocation != "")
            {
                textBox1.Text = SolveMessage();
                button2.ForeColor = Color.Green;
                await Task.Delay(3000);
                button2.ForeColor = Color.Black;
            }
        }

    }
}
