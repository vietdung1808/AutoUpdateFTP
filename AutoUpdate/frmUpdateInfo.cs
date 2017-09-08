using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoUpdate
{
    internal partial class frmUpdateInfo : Form
    {
        static frmUpdateInfo InfoUpdate;
        internal frmUpdateInfo()
        {
            InitializeComponent();
        }
        internal static void Show(string appName, string currentVersion, string newVersion, string description)
        {
            InfoUpdate = new frmUpdateInfo();
            InfoUpdate.Text = appName + " - Update info";
            InfoUpdate.lblCurVersion.Text += currentVersion;
            InfoUpdate.lblNewVersion.Text += newVersion;
            InfoUpdate.txtDescription.Text = description;
            InfoUpdate.StartPosition = FormStartPosition.CenterScreen;
            InfoUpdate.ShowDialog();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            InfoUpdate.Close();
        }
    }
}
