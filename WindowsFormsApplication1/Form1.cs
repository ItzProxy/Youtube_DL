using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoLibrary;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string folder;
        public string d_status = "Status: ";
        public bool downloaded = false;
        public bool answer;
        public Form1()
        {
            InitializeComponent();
            label2.MaximumSize = new Size(344, 0);
            f_name.MaximumSize = new Size(344, 0);
            label2.AutoSize = true;
            f_name.AutoSize = true;
            folder = GetDefaultFolder();
            f_name.Text = folder;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = d_status + "Please enter the URL into the text box below";
        }

        private void label1_Click(object sender, EventArgs e)
        {
            //does nothing
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string URL = textBox1.Text;
            if (URL.Trim().StartsWith("https://www.youtube.com/watch?v="))
            {
                using (var service = Client.For(new YouTube()))
                {
                    label2.Text = d_status + "Youtube URL: " + URL;
                    var video = service.GetVideo(URL);
                    label2.Text = d_status + "Your video is downloaded...saving to " + folder;
                    string path = Path.Combine(folder, video.FullName);
                    File.WriteAllBytes(path, video.GetBytes());
                    label2.Text = d_status + "File is saved.";
                    service.Dispose();
                }
            }
            else
            {
                label2.Text = d_status + "Youtube link is invalid, please enter a new one.";
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            //do nothing
        }

        static string GetDefaultFolder()
        {
            var home = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile);

            return Path.Combine(home, "Downloads");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string URL = tb.Text;
            if (URL.Trim().StartsWith("https://www.youtube.com/watch?v="))
            {
                label2.Text = d_status + "Valid youtube link!";
            }
            else
            {
                label2.Text = d_status + "Invalide youtube link!";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.f_name.Text = "Set to: " + folderBrowserDialog1.SelectedPath;
                this.folder = folderBrowserDialog1.SelectedPath;
            }
        }
    }

}
