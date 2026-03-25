using VSoftLIS_Interface.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VSoftLIS_Interface
{
    public partial class frmLogin : Form
    {
        private string userCode_Temp = "";

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.Cancel;
            pnlSelectLabLocation.Visible = false;

            if (txtUserCode.Text.Trim() == "")
            {
                MessageBox.Show("User Code is required");
                return;
            }
            if (txtPassword.Text.Trim() == "")
            {
                MessageBox.Show("Password is required");
                return;
            }

            LoginUser loginUser = new LoginUser() { UserCode = txtUserCode.Text.Trim(), PassWord = txtPassword.Text };
            LoginAPIResult loginAPIResult = new LoginAPIResult();
            loginAPIResult = WebAPICommon.PostApi(WebAPI.VSoftApiBaseUrl, "AccountAPI/Login", loginUser, loginAPIResult, TimeoutMilliSeconds: 30 * 1000) as LoginAPIResult;
            if (loginAPIResult?.EmployeeInfo == null)
            {
                MessageBox.Show("Authentication Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //do not allow if not having access to location that was set earlier
            if (Program.LocationId > 0 && !loginAPIResult.LocationAccess.Any(r => r.LocationId == Program.LocationId))
            {
                MessageBox.Show("You do not have access to configure LIS of this lab location.");
                return;
            }
            if (!loginAPIResult.MenuAccess.Any(r => r.ControllerName.Contains("Analyzer")))
            {
                MessageBox.Show("You do not have access to configure LIS. Kindly get it assigned from Labso Web portal.");
                return;
            }

            userCode_Temp = loginAPIResult.EmployeeInfo.UserCode;

            if (loginAPIResult.LocationAccess.Count == 1)
            {
                Program.LoginUserCode = userCode_Temp;
                if (Program.UpdateLocationId(loginAPIResult.LocationAccess[0].LocationId))
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            else if (loginAPIResult.LocationAccess.Count > 1)
            {
                pnlSelectLabLocation.Visible = true;
                cmbLocations.DataSource = loginAPIResult.LocationAccess;
                cmbLocations.ValueMember = "LocationId";
                cmbLocations.DisplayMember = "LocationCode";
                cmbLocations.Focus();

                if (Program.LocationId > 0)
                    cmbLocations.SelectedValue = Program.LocationId;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_LabLocation_Click(object sender, EventArgs e)
        {
            if (cmbLocations.SelectedIndex == -1)
            {
                MessageBox.Show("Location is required.");
                return;
            }

            Program.LoginUserCode = userCode_Temp;
            if (Program.UpdateLocationId((int)cmbLocations.SelectedValue))
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void cmbLocations_KeyDown(object sender, KeyEventArgs e)
        {
            //below code for enter key not working as AcceptButton for form is set
            if (e.KeyData == Keys.Enter && btnOK_LabLocation.Enabled)
            {
                btnOK_LabLocation_Click(null, null);
                e.Handled = true;
            }
        }
    }

    public class LoginUser
    {
        public string UserCode { get; set; }
        public string PassWord { get; set; }
        public string Domain { get { return "Lab_So_Web"; } }
    }

    public class LoginAPIResult
    {
        public EmpDetail EmployeeInfo { get; set; }
        public List<LocationList> LocationAccess { get; set; }

        public List<Menu> MenuAccess { get; set; }
    }
    public class EmpDetail
    {
        public string UserCode { get; set; }
        public string PassKey { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Contact { get; set; }
        public int CompanyId { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
    }
    public class LocationList
    {
        public int LocationId { get; set; }
        public string LocationCode { get; set; }
    }
    public class Menu
    {
        public string MainMenuName { get; set; }
        public int MainMenuId { get; set; }
        public string SubMenuName { get; set; }
        public int SubMenuId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public int? RoleId { get; set; }
        public string FontIcon { get; set; }
        //public string RoleName { get; set; }
    }
}
