using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Ionic.Zip;
using System.Diagnostics;

namespace AutoUpdate
{
    internal partial class frmDownloading : Form
    {
        static DialogResult result;//Abort:Cancel download. No:Error download. 
        static frmDownloading downloading;
        long bytes_total;
        WebClient webClient;
        string localPath, appName, zipFileName;
        LocalXml localXml;
        UpdateXml updateXml;
        BackgroundWorker bgwInstall;

        internal frmDownloading(string appName, string localPath, LocalXml localXml, UpdateXml updateXml)
        {
            InitializeComponent();
            this.localXml = localXml;
            this.updateXml = updateXml;
            this.localPath = localPath;
            this.appName = appName;
            this.Text = appName + " - Download Update";

            bgwInstall = new BackgroundWorker();
            bgwInstall.WorkerReportsProgress = true;

            bgwInstall.DoWork += bgwInstall_DoWork;
            bgwInstall.RunWorkerCompleted += bgwInstall_RunWorkerCompleted;
        }
        private void frmDownloading_Load(object sender, EventArgs e)
        {
            try
            {
                DownloadUpdate_FTP(this.localXml.FtpUser, this.localXml.FtpPass, this.updateXml.UrlZipFile, this.localPath);
            }
            catch
            {
                result = DialogResult.No;
                downloading.Close();
            }
        }
        internal static DialogResult DownloadUpdate(string appName, string localPath, LocalXml localXml, UpdateXml updateXml)
        {
            downloading = new frmDownloading(appName, localPath, localXml, updateXml);
            downloading.ShowDialog();
            return result;

        }
        private void DownloadUpdate_FTP(string user, string pass, string url, string localPath)
        {
            this.progressBar.Style = ProgressBarStyle.Blocks;
            zipFileName = String.Format("DataUpdate{0:yyMMddHHmmss}.zip", DateTime.Now);
            //Create folder tmp
            if (!Directory.Exists(Path.Combine(localPath, "tmp")))
                    Directory.CreateDirectory(Path.Combine(localPath, "tmp"));

            //Get file size
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.Credentials = new NetworkCredential(user, pass);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responeStream = response.GetResponseStream();
            bytes_total = response.ContentLength;
            response.Close();

            webClient = new WebClient();
            webClient.Credentials = new NetworkCredential(user, pass);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
            webClient.DownloadFileAsync(new Uri(url), Path.Combine(localPath,"tmp\\"+zipFileName));
            webClient.Dispose();
        }
        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = (int)(((float)e.BytesReceived / (float)bytes_total) * 100.0);
            this.label1.Text = String.Format("Downloaded {0} of {1}", FormatBytes(e.BytesReceived, 1, true), FormatBytes(bytes_total, 1, true));
        }
        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                result = DialogResult.No;
                downloading.Close();
            }
            else if (e.Cancelled)
            {
                result = DialogResult.Abort;
                downloading.Close();
            }
            else
            {
                //Move all file
                if (bgwInstall.IsBusy)
                    bgwInstall.CancelAsync();

                this.ControlBox = false;
                this.Text = this.appName + " - Installation";
                label1.Text = "Installing update....";
                this.progressBar.Style = ProgressBarStyle.Marquee;
                bgwInstall.RunWorkerAsync();
            }
        }
        private void frmDownloading_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (webClient == null) return;
            if (webClient.IsBusy)
            {
                webClient.CancelAsync();
                result = DialogResult.Abort;
            }
        }
        private void bgwInstall_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                result = DialogResult.None; //extrack zipfile error
                downloading.Close();
            }
            else
            {
                Move_DataUpdate();
                Application.Exit();
            }
        }
        private void bgwInstall_DoWork(object sender, DoWorkEventArgs e)
        {
            bgwInstall.ReportProgress(0);
            //unzip file
            ZipFile zipFile = new ZipFile(Path.Combine(this.localPath, "tmp\\" + this.zipFileName));
            zipFile.ExtractAll(this.localPath + @"\tmp\DataUpdate\", ExtractExistingFileAction.OverwriteSilently);            
        }
        private void Move_DataUpdate()
        {
            //Move file
            string argument = String.Format("/C taskkill /im {0} /f & choice /C Y /N /D Y /T 3 & ROBOCOPY \"{1}\" \"{2}\" /E /MOVE & powershell -Command \"(gc '{3}') -replace '{4}', '{5}' | Out-File '{3}'\" & Start \"\" /D \"{2}\" \"{0}\" & RMDIR \"{6}\" /S /Q",
                                this.updateXml.FileMain,
                                this.localPath + @"\tmp\DataUpdate",
                                this.localPath,
                                this.localPath + "\\AutoUpdate.xml",
                                "<version>" + this.localXml.Version,
                                "<version>" + this.updateXml.Version,
                                this.localPath + @"\tmp");
            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = argument;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            Process.Start(info);
        }
        private string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "B";

            // Check if best size in KB
            if (newBytes > 1024 && newBytes < 1048576)
            {
                newBytes /= 1024;
                byteType = "KB";
            }
            else if (newBytes > 1048576 && newBytes < 1073741824)
            {
                // Check if best size in MB
                newBytes /= 1048576;
                byteType = "MB";
            }
            else
            {
                // Best size in GB
                newBytes /= 1073741824;
                byteType = "GB";
            }

            // Show decimals
            if (decimalPlaces > 0)
                formatString += ":0.";

            // Add decimals
            for (int i = 0; i < decimalPlaces; i++)
                formatString += "0";

            // Close placeholder
            formatString += "}";

            // Add byte type
            if (showByteType)
                formatString += byteType;

            return String.Format(formatString, newBytes);
        }
    }
}
