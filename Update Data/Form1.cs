using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.IO;
using System.IO.Compression;

namespace Update_Data
{
    public partial class Form1 : Form
    {
        private WebClient web = new WebClient();
        public Form1()
        {
            InitializeComponent();
        }
        // Trước tiên khai báo biến nè 
        private bool dragging;
        private Point pointClicked;
        // Bắt sự kiện MouseUp cho nó nè 

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                pointClicked = new Point(e.X, e.Y);
            }
            else
            {
                dragging = false;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point pointMoveTo;
                pointMoveTo = this.PointToScreen(new Point(e.X, e.Y));
                pointMoveTo.Offset(-pointClicked.X, -pointClicked.Y);
                this.Location = pointMoveTo;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

       
        private int type;
        private void Form1_Load(object sender, EventArgs e)
        {
           
            
           
            web.DownloadFileCompleted += Client_DownloadFileCompleted;
            new Thread(delegate ()
            {
                Uri uri = new Uri(linkversion);
                Path.GetFileName(uri.AbsolutePath);
                this.web.DownloadFileAsync(uri, linkaddversionserver);
            }).Start();
            versionindex.Text = File.ReadAllText("update\\version.txt");

        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (type == 0)
            {
                base.Invoke(new MethodInvoker(delegate ()
                {
                    double num = double.Parse(e.BytesReceived.ToString());
                    double num2 = double.Parse(e.TotalBytesToReceive.ToString());
                    double num3 = num / num2 * 100.0;
                    status.Text = string.Format("{0:0.##}", num3) + "%";
                    progressBar1.Value = int.Parse(Math.Truncate(num3).ToString());
                }));
                Thread.Sleep(1000);
                MessageBox.Show("Đã cập nhập xong!");
                return;
            }
           
        }
       void updata()
        {
            try
            {
                Thread.Sleep(5000);
                File.Delete("Library");
                web.DownloadFile("http://locdz.tk/Library.zip", @"Library.zip");
                string path = @".\Library.zip";
                string ex = @".\";
                ZipFile.ExtractToDirectory(path, ex);
                File.Delete(@".\Library.zip");
                this.Close();
            }
            catch (Exception)
            {

            }
        }
        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string s = File.ReadAllText(linkaddversion);
            
            try
            {
                if (type == 0)
                {
                    while (!File.Exists(linkaddversion))
                    {
                        Thread.Sleep(100);
                    }
                   
                    versionserver.Text= File.ReadAllText(linkaddversionserver);//lấy phiên bản server gán vào text
                    if (versionserver.Text != versionindex.Text)//nếu mà cái phiên bản trên sv khác vs phien bản hiện tại
                    {
                         DialogResult dia = MessageBox.Show("Đã có phiên bản mới bạn có muốn cập nhập không!", "Thông báo", MessageBoxButtons.YesNo);
                       if(dia==DialogResult.Yes)
                        {
                             
                            new Thread(delegate ()
                            {
                                Uri uri = new Uri(linkversion);
                                Path.GetFileName(uri.AbsolutePath);
                                web.DownloadFileAsync(uri, linkaddversion);
                                updata();
                            }).Start();
                            web.DownloadProgressChanged += Client_DownloadProgressChanged;
                            versionindex.Text = File.ReadAllText(linkaddversionserver);
                        }
                        if (dia == DialogResult.No)
                        {
                            status.Text = "Đã có phiên bản mới!";
                           
                            return;
                        }
                       
                    }
                    Thread.Sleep(3000);
                    status.Text = "Chưa có phiên bản mới!";
                    return;
                }
              
            }
            catch (Exception)
            {

            }
        }
        private string linkversion = "https://drive.google.com/u/0/uc?id=1AxqxTvB_P8fUf1-MviLpBgLLXs9dy4Xw&export=download";
        private string linkaddversion = Application.StartupPath + "\\update\\version.txt";
        private string linkaddversionserver= Application.StartupPath + "\\update\\server.txt";
    }
}
