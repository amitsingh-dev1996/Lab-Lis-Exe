using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VSoftLIS_Interface.Common
{
    public partial class frmMessageBox : Form
    {
        public frmMessageBox()
        {
            InitializeComponent();
        }

        public frmMessageBox(string message, string caption = "")
        {
            InitializeComponent();

            lblMessage.Text = message;
            this.Text = String.IsNullOrEmpty(caption) ? "VSoft Service Alert" : caption;
        }

        public static void Show(string message, string caption)
        {
            frmMessageBox messageBox = new frmMessageBox(message, caption);
            messageBox.Show();
        }
    }
}
