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
    internal partial class frmUpdateAccept : Form
    {
        static frmUpdateAccept updateApp;
        static DialogResult result = DialogResult.No;
        string currentVer, newVer, description, appName;
        internal frmUpdateAccept(string appName, string currentVer, string newVer, string description)
        {
            InitializeComponent();
            this.currentVer = currentVer;
            this.newVer = newVer;
            this.description = description;
            this.appName = appName;
            this.Text = appName + " - Update";
        }

        internal static DialogResult Show(string appName, string currentVer, string newVer, string description)
        {
            updateApp = new frmUpdateAccept(appName, currentVer, newVer, description);            
            updateApp.StartPosition = FormStartPosition.CenterScreen;
            updateApp.ShowDialog();
            return result;
        }
        private void btnYes_Click(object sender, EventArgs e)
        {
            result = DialogResult.Yes;
            updateApp.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            result = DialogResult.No;
            updateApp.Close();
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            frmUpdateInfo.Show(this.appName, this.currentVer, this.newVer, this.description);
            this.Enabled = true;
        }
    }
}
