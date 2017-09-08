using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AppTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //Check updated version when run app
            AutoUpdate.Update.UpdateApp(false);
            InitializeComponent();
            this.Text += " - Version " + AutoUpdate.Update.CurrentVersion();
        }
        private void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            AutoUpdate.Update.UpdateApp(true);
        }
    }
}
