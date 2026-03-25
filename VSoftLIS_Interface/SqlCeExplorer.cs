using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VSoftLIS_Interface.Common;
using VSoftLIS_Interface.DLL;

namespace VSoftLIS_Interface
{
    public partial class SqlCeExplorer : Form
    {
        public SqlCeExplorer()
        {
            InitializeComponent();
        }

        private void SqlCeExplorer_Load(object sender, EventArgs e)
        {
            txtQuery.Text = "select * from tbl_FailedResults where IsResultSent=0";
            dgvOutput.AutoGenerateColumns = true;
        }

        private void btnExecuteQuery_Click(object sender, EventArgs e)
        {
            //DataTable dtPending = LocalSqlCE.ExecuteDataSet().Tables[0];
            //dgvOutput.DataSource = dtPending;

            //LocalSqlCE localSqlCE = new LocalSqlCE(txtDbFilePath.Text);
            LocalSqlCE.ChangeConnection(txtDbFilePath.Text);
            dgvOutput.DataSource = LocalSqlCE.ExecuteDataSet(txtQuery.Text).Tables[0];
        }

        private void txtDbFilePath_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
