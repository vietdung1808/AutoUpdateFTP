using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AutoUpdate
{
    internal class LocalXml
    {
        string url;
        string ftpUser;
        string ftpPass;
        string version;
        internal string Url { get => url;}
        internal string FtpUser { get => ftpUser; }
        internal string FtpPass { get => ftpPass; }
        internal string Version { get => version; }
        internal LocalXml(string url, string ftpUser, string ftpPass, string version)
        {
            this.url = url;
            this.ftpUser = ftpUser;
            this.ftpPass = ftpPass;
            this.version = version;
        }
        internal static LocalXml Get_LocalXml(string pathFileXml)
        {
            string url = string.Empty, ftpUser = string.Empty, ftpPass = string.Empty, version = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pathFileXml);
                XmlNode node = doc.DocumentElement;
                if (node == null)
                    return null;                
                url = node["url"].InnerText;
                ftpUser = node["ftpUser"].InnerText;
                ftpPass = node["ftpPass"].InnerText;
                version = node["version"].InnerText;
                return new LocalXml(url, ftpUser, ftpPass, version);
            }
            catch
            { return null; }
        }
    }
}
