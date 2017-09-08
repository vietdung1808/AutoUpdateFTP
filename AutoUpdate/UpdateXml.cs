using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;

namespace AutoUpdate
{
    internal class UpdateXml
    {
        string version;
        string urlZipFile;
        string fileMain;
        string description;

        internal string Version { get => version;}
        internal string UrlZipFile { get => urlZipFile; }
        internal string FileMain { get => fileMain;}
        internal string Description { get => description; }
        internal UpdateXml(string version, string urlZipFile, string fileMain, string description)
        {
            this.version = version;
            this.urlZipFile = urlZipFile;
            this.fileMain = fileMain;
            this.description = description;
        }
        internal static bool Check_ExistsOnServer(LocalXml localXml)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create( new Uri(localXml.Url));
                request.Credentials = new NetworkCredential(localXml.FtpUser, localXml.FtpPass);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                return true;
            }
            catch { return false; }
        }
        internal static UpdateXml Get_InfoUpdate(LocalXml localXml)
        {
            string version = string.Empty, urlZipFile = string.Empty, fileMain = string.Empty, description = string.Empty;
            try
            {
                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(localXml.FtpUser, localXml.FtpPass);
                byte[] newFileData = request.DownloadData(new Uri(localXml.Url));
                string fileString = System.Text.Encoding.UTF8.GetString(newFileData);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(fileString);

                XmlNode updateNode = doc.DocumentElement;
                if (updateNode == null)
                    return null;

                //Get value
                version = updateNode["version"].InnerText;
                urlZipFile = updateNode["urlZipFile"].InnerText;
                fileMain = updateNode["fileMain"].InnerText;
                description = updateNode["description"].InnerText;

                return new UpdateXml(version, urlZipFile, fileMain, description);
            }
            catch { return null; }
        }
    }
}
