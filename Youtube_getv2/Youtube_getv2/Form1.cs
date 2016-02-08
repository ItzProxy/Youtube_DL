using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExtractor;

namespace Youtube_getv2
{
    public partial class Form1 : Form
    {
        string de_status = "Status: ";
        string folder;

        public Form1()
        {
            InitializeComponent();
            label1.Text = de_status + "Hello, welcome to the youtube extractor!";
            folder = GetDefaultFolder();
            textBox2.Text = folder;
            label1.MaximumSize = new Size(438, 0); //wrap within window space
            label1.AutoSize = true;
            label2.MaximumSize = new Size(438, 0); //wrap within window space
            label2.AutoSize = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string URL = textBox1.Text;
                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(URL, false); //get video formats
                if (radioButton1.Checked)
                {
                    VideoInfo video = videoInfos
                    .First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);
                    if (video.RequiresDecryption)
                    {
                        DownloadUrlResolver.DecryptDownloadUrl(video); //decrypt signiture if needed
                    }
                    var videoDownloader = new VideoDownloader(video, Path.Combine(folder,
                                                                                  RemoveIllegalPathCharacters(video.Title) + 
                                                                                  video.VideoExtension));
                    videoDownloader.DownloadProgressChanged += (se, args) => label1.Text = de_status + " Downloading - " + args.ProgressPercentage;
                    videoDownloader.Execute();
                    label2.Text = video.Title + " is saved to " + folder;
                }
                else if(radioButton2.Checked)
                {
                    VideoInfo audio = videoInfos
                    .Where(info => info.CanExtractAudio)
                    .OrderByDescending(info => info.AudioBitrate)
                    .First();

                    if (audio.RequiresDecryption)
                    {
                        DownloadUrlResolver.DecryptDownloadUrl(audio);
                    }

                    var audioDownload = new AudioDownloader(audio, Path.Combine(folder,
                                                                                RemoveIllegalPathCharacters(audio.Title) + 
                                                                                audio.AudioExtension));
                    audioDownload.DownloadProgressChanged += (se, args) => label1.Text = de_status + " Downloading - " + args.ProgressPercentage;
                    audioDownload.AudioExtractionProgressChanged += (se, args) => label1.Text = de_status + " Extracting audio - " + args.ProgressPercentage;

                    audioDownload.Execute();

                    label2.Text = audio.Title + " is saved to " + folder;
                }
                else
                {
                    textBox1.Text = de_status + " - Choose Video or Audio option -";
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("Exception Message: " + err.Message);
                label1.Text = de_status + " " + err.Message;
            }
        }
        static string GetDefaultFolder()
        {
            var home = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile);

            return Path.Combine(home, "Downloads");
        }
        private static string RemoveIllegalPathCharacters(string path)//remove /,",' and more from path title
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if(folderDialog.ShowDialog() == DialogResult.OK)
            {
                folder = folderDialog.SelectedPath;
                textBox2.Text = folder;
            }
        }
    }

}
