using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Xml;

namespace AutoUpdate
{
    public static class Update
    {
        public static void UpdateApp(bool isManualCheck)
        {
            string appName = Application.ProductName;
            string localPath = Application.StartupPath;
            //check exists config file
            string configFilePath = Path.Combine(localPath, "AutoUpdate.xml");
            if (!File.Exists(configFilePath))
            {
                if (isManualCheck)
                    MessageBox.Show(string.Format("The local configuration file does not exist\n{0}.", configFilePath), appName + " - Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //read Local config
            LocalXml localXml = LocalXml.Get_LocalXml(configFilePath);
            if (localXml == null)
            {
                if (isManualCheck)
                    MessageBox.Show(string.Format("Error reading local configuration file\n{0}.", configFilePath), appName + " - Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Check on server
            if (!UpdateXml.Check_ExistsOnServer(localXml))
            {
                if (isManualCheck)
                    MessageBox.Show("Failed to connect to FTP server.", appName + " - Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Get Info update
            UpdateXml updateValue = UpdateXml.Get_InfoUpdate(localXml);
            if (updateValue == null)
            {
                if(isManualCheck)
                    MessageBox.Show(string.Format("Error reading configuration file on FTP server.\n{0}",localXml.Url), appName + " - Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Check version
            if (!updateValue.Version.Equals(localXml.Version))
            {
                //update
                if (frmUpdateAccept.Show(appName, localXml.Version, updateValue.Version, updateValue.Description) == DialogResult.Yes)
                {
                    DialogResult result = frmDownloading.DownloadUpdate(appName, localPath, localXml, updateValue);
                    if (result==DialogResult.No)
                        MessageBox.Show("There was a problem downloading the update.\nPlease try again later.", appName + " - Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (result == DialogResult.Abort)
                        MessageBox.Show("The update download was cancelled.\nThis program has not been modified.", appName + " - Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (result == DialogResult.None)
                        MessageBox.Show("Error extracting zipped file.\nPlease check the [Ionic] library or zip file.", appName + " - Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (isManualCheck)
            {
                MessageBox.Show("You have the latest version already!", appName + " - Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public static string CurrentVersion()
        {
            string ver = string.Empty;
            try
            {
                LocalXml localXml = LocalXml.Get_LocalXml(Path.Combine(Application.StartupPath, "AutoUpdate.xml"));
                ver = localXml.Version;
            }
            catch(Exception)
            {}
            return ver;
        }
    }
}
