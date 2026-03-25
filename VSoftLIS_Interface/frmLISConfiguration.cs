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
    public partial class frmLISConfiguration : Form
    {
        public frmLISConfiguration()
        {
            InitializeComponent();
        }

        private void btnSelectFolder_ResultFilePath_Click(object sender, EventArgs e)
        {

        }

        private void rdoConnectionMode_CheckedChanged(object sender, EventArgs e)
        {
            //below condition to avoid multiple triggering by every radiobutton
            if (sender != null && (sender as RadioButton).Checked == false)
                return;

            //pnlTcpPortSettings.Visible = rdoTCP.Checked;
            //pnlSerialPortSettings.Visible = !rdoTCP.Checked;

            dgvLISList.Rows.Clear();
            txtSearch.Text = "";
            FillSerialPortNames();
            dgvcConnectionMode.Items.Clear();
            if (rdoTcpSerial.Checked)
            {
                dgvcConnectionMode.Items.AddRange("", "TCP", "Serial");
            }
            else if (rdoFile.Checked)
            {
                dgvcConnectionMode.Items.AddRange("", "FilePickup", "FileUpload", "FileConnected");
            }

            LoadAnalyzerList();

            chkAdvanced.Visible = rdoTcpSerial.Checked;
            dgvcFileName.Visible = rdoFile.Checked;
            /*dgvcConnectionMode.Visible =*/
            dgvcPortDetails.Visible = !rdoFile.Checked;

            chkAdvanced_CheckedChanged(null, new EventArgs());
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove selected LIS?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {

            }
        }

        private void LISConfiguration_Load(object sender, EventArgs e)
        {
            //if (!Program.EnsureUserLogin())
            //    return;

            dgvcPortDetails.SortMode = DataGridViewColumnSortMode.Automatic;
            FillSerialPortNames();

            dgvLISList.RowStateChanged += DgvLISList_RowStateChanged;
            dgvLISList.CellValidating += DgvLISList_CellValidating;
            dgvLISList.EditingControlShowing += DgvLISList_EditingControlShowing;

            dgvAdditionalSettings.CellValidating += dgvAdditionalSettings_CellValidating;

            rdoConnectionMode_CheckedChanged(null, new EventArgs());
        }

        private void dgvAdditionalSettings_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewCell dgvc = dgvAdditionalSettings.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (e.ColumnIndex == dgvcSettingValue.Index && e.FormattedValue.ToString() != dgvc.Value.ToString())
            {
                if (MessageBox.Show("You have changed setting value for " + dgvc.Value + "." + Environment.NewLine + "Click OK to save. Click Cancel to keep previous value.", "Setting Change Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                {
                    dgvAdditionalSettings.EditingControl.Text = dgvc.Value.ToString();
                }
            }
        }

        private void DgvLISList_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            //this is used for row-selected event, RowEnter not used as it is triggerred before selection
            //https://stackoverflow.com/questions/1027360/datagridview-capturing-user-row-selection
            if (e.StateChanged == DataGridViewElementStates.Selected)
            {
                if (e.Row.Tag != null)
                {
                    LISConfiguration lisConfiguration = e.Row.Tag as LISConfiguration;
                    var additionalSettings = lisConfiguration.ConnectionSettings.AdditionalSettings;
                    dgvAdditionalSettings.Visible = additionalSettings.Any();
                    dgvAdditionalSettings.DataSource = additionalSettings;
                    //dgvAdditionalSettings.Rows.Clear();
                    //foreach (var item in additionalSettings)
                    //    dgvAdditionalSettings.Rows.Add(item.Key, item.Value);
                }
            }
        }

        private void DgvLISList_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == dgvcPortDetails.Index)
            {
                string value = e.FormattedValue.ToString();

                if (String.IsNullOrEmpty(value))
                {
                    dgvLISList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = null;
                }
                else
                {
                    var portUsedRow = dgvLISList.Rows.Cast<DataGridViewRow>().Where(r => r.Index != e.RowIndex && r.Cells[dgvcPortDetails.Index].Value?.ToString() == value);
                    if (portUsedRow.Any())
                    {
                        MessageBox.Show("Port " + value + " is already configured for " + portUsedRow.First().Cells[dgvcAnalyzerName.Index].Value + ".", "Duplicate Port Selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        e.Cancel = true;
                        return;
                    }

                    //set autocomplete provision for dropdown in grid
                    //https://social.msdn.microsoft.com/Forums/windows/en-US/a44622c0-74e1-463b-97b9-27b87513747e/windows-forms-data-controls-and-databinding-faq?forum=winformsdatacontrols#faq26
                    if (!dgvcPortDetails.Items.Contains(value))
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d+$"))
                        {
                            MessageBox.Show("Only numeric values can be added as new port numbers.", "Invalid Port Number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            e.Cancel = true;
                            return;
                        }

                        dgvcPortDetails.Items.Add(value);
                        dgvLISList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = value;
                    }
                }
            }
        }

        private void DgvLISList_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvLISList.CurrentCellAddress.X == dgvcPortDetails.Index)
                (e.Control as ComboBox).DropDownStyle = ComboBoxStyle.DropDown;

            else if (dgvLISList.CurrentCellAddress.X == dgvcFileName.Index)
            {
                InterfaceUIHelper.OpenFolderDialog(e.Control as TextBox);
            }
        }

        private void dgvLISList_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

        }

        private void dgvLISList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            DataGridViewCell dgvc = dgvLISList.Rows[e.RowIndex].Cells[e.ColumnIndex];
            DataGridViewRow dgvr = dgvLISList.Rows[e.RowIndex];

            if (e.ColumnIndex == dgvcIsActive.Index)
            {
                if (dgvc.Value != null && (bool)dgvc.Value == true)
                {
                    dgvLISList.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else
                {
                    dgvLISList.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                }
            }
            else if (e.ColumnIndex == dgvcConnectionMode.Index)
            {
                ConnectionType connectionType = ParseConnectionType(dgvc.Value);
                bool enableSerial = connectionType == ConnectionType.Serial;

                foreach (DataGridViewCell d in new[] { dgvr.Cells[dgvcBaudRate.Index], dgvr.Cells[dgvcParity.Index], dgvr.Cells[dgvcStopBits.Index], dgvr.Cells[dgvcDataBits.Index] })
                {
                    d.ReadOnly = !enableSerial;
                    //d.Style.BackColor = Color.White;
                    //d.Style.ForeColor = Color.White;
                }

                if (rdoFile.Checked)
                {
                    if (connectionType == ConnectionType.FileUpload)
                    {
                        dgvr.Cells[dgvcFileName.Index].Value = "";
                        dgvr.Cells[dgvcFileName.Index].ReadOnly = true;
                    }
                    else
                    {
                        dgvr.Cells[dgvcFileName.Index].ReadOnly = false;
                    }
                }
            }
        }

        private void LoadAnalyzerList()
        {
            List<AnalyzerMaster> analyzerList = InterfaceHelper.GetAnalyzerList(Program.LocationId);
            DataTable dtLISSetting = LocalSqlCE.ExecuteDataSet("select * from tbl_LISSetting where LocationId=" + Program.LocationId).Tables[0];

            var analyzers = from a in analyzerList
                            join b1 in dtLISSetting.AsEnumerable() on a.AnalyzerId equals b1.Field<int>("AnalyzerId") into x
                            from b in x.DefaultIfEmpty()
                            orderby a.AnalyzerName
                            select new LISConfiguration
                            {
                                AnalyzerMaster = a,
                                ConnectionSettings = AnalyzerConfiguration.PopulateConnectionSettings(a.AnalyzerTypeId, b),
                                IsActive = (b == null || b.Field<bool>("IsActive") == false ? false : true)
                            };

            var query = analyzers;
            if (rdoTcpSerial.Checked)
                query = query.Where(r => new ConnectionType[] { ConnectionType.NotSpecified, ConnectionType.TCP, ConnectionType.Serial }.Contains(r.ConnectionSettings.ConnectionType));
            else if (rdoFile.Checked)
                query = query.Where(r => new ConnectionType[] { ConnectionType.FilePickup, ConnectionType.FileUpload, ConnectionType.FileConnected }.Contains(r.ConnectionSettings.ConnectionType));

            dgvLISList.Rows.Clear();
            foreach (var item in query)
            {
                //dgvLISList.DataSource = query.ToList();
                int newRowIndex = dgvLISList.Rows.Add();
                DataGridViewRow dgvr = dgvLISList.Rows[newRowIndex];
                dgvr.Cells[dgvcAnalyzerId.Index].Value = item.AnalyzerMaster.AnalyzerId;
                dgvr.Cells[dgvcAnalyzerTypeId.Index].Value = item.AnalyzerMaster.AnalyzerTypeId;
                dgvr.Cells[dgvcAnalyzerName.Index].Value = item.AnalyzerMaster.AnalyzerName;
                dgvr.Cells[dgvcIsActive.Index].Value = item.IsActive;
                dgvr.Cells[dgvcConnectionMode.Index].Value = item.ConnectionSettings.ConnectionType == ConnectionType.NotSpecified ? "" : item.ConnectionSettings.ConnectionType.ToString();

                if (rdoTcpSerial.Checked)
                {
                    dgvr.Cells[dgvcIPAddress.Index].Value = item.ConnectionSettings.TCP_IPAddress;
                    //IP address not requierd for server mode
                    if (item.ConnectionSettings.TCP_IsServerMode)
                    {
                        dgvr.Cells[dgvcIPAddress.Index].Value = "";
                        //do not disable IP address field. In case of dual IP address of LIS pc, user might need any of the IP address other than one captured by LIS
                        //dgvr.Cells[dgvcIPAddress.Index].ReadOnly = true;
                        //dgvr.Cells[dgvcIPAddress.Index].Style.BackColor = Color.Gray;
                    }

                    string portNumber = item.ConnectionSettings.TCP_PortNumber.ToString() != "" ? item.ConnectionSettings.TCP_PortNumber.ToString() : item.ConnectionSettings.Serial_PortName;
                    if (!String.IsNullOrEmpty(portNumber) && !dgvcPortDetails.Items.Contains(portNumber))
                    {
                        dgvcPortDetails.Items.Add(portNumber);
                    }
                    dgvr.Cells[dgvcPortDetails.Index].Value = portNumber;
                }
                else if (rdoFile.Checked)
                {
                    dgvr.Cells[dgvcFileName.Index].Value = item.ConnectionSettings.FilePath;
                }
                dgvr.Cells[dgvcBaudRate.Index].Value = item.ConnectionSettings.Serial_BaudRate.ToString();
                dgvr.Cells[dgvcParity.Index].Value = ParseParity(item.ConnectionSettings.Serial_Parity).ToString();
                dgvr.Cells[dgvcStopBits.Index].Value = ParseStopBits(item.ConnectionSettings.Serial_StopBits).ToString();
                dgvr.Cells[dgvcDataBits.Index].Value = item.ConnectionSettings.Serial_DataBits.ToString();
                dgvr.Tag = item;
            }
        }

        private void dgvLISList_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow dgvr = dgvLISList.Rows[e.RowIndex];
            LISConfiguration oldLISConfiguration = dgvr.Tag as LISConfiguration;
            ConnectionType selectedConnectionType = ParseConnectionType(dgvr.Cells[dgvcConnectionMode.Index].Value);
            string selectedPortNumber = (dgvr.Cells[dgvcPortDetails.Index].Value ?? "").ToString();
            bool isActive = ((bool)(dgvr.Cells[dgvcIsActive.Index].Value ?? false));
            if (isActive == true)
            {
                if (selectedConnectionType == ConnectionType.NotSpecified)
                {
                    MessageBox.Show("Connection Mode must be set for Active analyzer.", "Connection Mode Required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }
            }

            if (selectedConnectionType == ConnectionType.TCP || selectedConnectionType == ConnectionType.Serial)
            {
                string ipAddress = (dgvr.Cells[dgvcIPAddress.Index].Value ?? "").ToString();
                if (selectedConnectionType == ConnectionType.TCP)
                {
                    if (String.IsNullOrEmpty(ipAddress))
                    {
                        if (isActive && oldLISConfiguration.ConnectionSettings.TCP_IsServerMode == false)
                        {
                            MessageBox.Show("IP address is mandatory for this analyzer.", "IP Address Mandatory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            e.Cancel = true;
                            return;
                        }
                    }
                    else if (InterfaceHelper.ParseValidIpAddress(ipAddress) == null)
                    {
                        MessageBox.Show("Invalid IP address.", "Invalid IP address", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        e.Cancel = true;
                        return;
                    }
                }

                if (isActive && selectedPortNumber == "")
                {
                    MessageBox.Show("Port number must not be empty.", "Port Number Required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }
                else if (selectedPortNumber != "")
                {
                    if (selectedConnectionType == ConnectionType.TCP)
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(selectedPortNumber, @"^\d+$"))
                        {
                            MessageBox.Show("Only numeric port numbers allowed for TCP connection.", "Invalid Port Number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            e.Cancel = true;
                            return;
                        }
                    }
                    else if (selectedConnectionType == ConnectionType.Serial)
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(selectedPortNumber, @"^COM\d$"))
                        {
                            MessageBox.Show("Invalid port number selected for Serial connection. COM ports to be selected from existing list only", "Invalid Port Number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            e.Cancel = true;
                            return;
                        }
                    }
                }
            }

            if (selectedConnectionType == ConnectionType.FilePickup || selectedConnectionType == ConnectionType.FileConnected || selectedConnectionType == ConnectionType.FileUpload)
            {
                string folderPath = dgvr.Cells[dgvcFileName.Index].Value?.ToString();

                if (selectedConnectionType == ConnectionType.FilePickup || selectedConnectionType == ConnectionType.FileConnected)
                {
                    if (isActive && String.IsNullOrEmpty(folderPath))
                    {
                        MessageBox.Show("Folder Path is mandatory", "Folder Path Mandatory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        private void dgvLISList_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow dgvr = dgvLISList.Rows[e.RowIndex];
                ConnectionType selectedConnectionType = ParseConnectionType(dgvr.Cells[dgvcConnectionMode.Index].Value);
                if ((selectedConnectionType == ConnectionType.TCP || selectedConnectionType == ConnectionType.Serial)
                    && String.IsNullOrEmpty(dgvr.Cells[dgvcPortDetails.Index].Value?.ToString()))
                    return;
                if ((selectedConnectionType == ConnectionType.FilePickup || selectedConnectionType == ConnectionType.FileConnected /*|| selectedConnectionType == ConnectionType.FileUpload*/)
                    && String.IsNullOrEmpty(dgvr.Cells[dgvcFileName.Index].Value?.ToString()))
                    return;

                string strAdditionalSettings = InterfaceHelper.SerializeToJson((dgvr.Tag as LISConfiguration).ConnectionSettings.AdditionalSettings);

                System.Data.SqlServerCe.SqlCeCommand cmd = new System.Data.SqlServerCe.SqlCeCommand();
                if (LocalSqlCE.ExecuteDataSet("select 1 from tbl_LISSetting where AnalyzerId=" + dgvr.Cells[dgvcAnalyzerId.Index].Value).Tables[0].Rows.Count == 0)
                {
                    cmd.CommandText = "insert into tbl_LISSetting(AnalyzerId, AnalyzerTypeId, LocationId, ConnectionType, TCP_IPAddress, TCP_PortNumber, Serial_PortName, Serial_BaudRate, Serial_Parity, Serial_StopBits, Serial_DataBits, FilePath, IsActive, CreatedBy, CreatedOn, AdditionalSettings)" +
                        "values(@AnalyzerId, @AnalyzerTypeId, @LocationId, @ConnectionType, @TCP_IPAddress, @TCP_PortNumber, @Serial_PortName, @Serial_BaudRate, @Serial_Parity, @Serial_StopBits, @Serial_DataBits, @FilePath, @IsActive, @CreatedBy, @CreatedOn, @AdditionalSettings)";
                }
                else
                {
                    cmd.CommandText = "update tbl_LISSetting set AnalyzerTypeId=@AnalyzerTypeId, ConnectionType=@ConnectionType, TCP_IPAddress=@TCP_IPAddress, TCP_PortNumber=@TCP_PortNumber, " +
                        "Serial_PortName=@Serial_PortName, Serial_BaudRate=@Serial_BaudRate, Serial_Parity=@Serial_Parity, Serial_StopBits=@Serial_StopBits, Serial_DataBits=@Serial_DataBits, " +
                        "FilePath=@FilePath, IsActive=@IsActive, LastUpdatedBy=@LastUpdatedBy, LastUpdatedOn=@LastUpdatedOn, AdditionalSettings=@AdditionalSettings " +
                        "where AnalyzerId=@AnalyzerId";
                }

                int analyzerId = (int)dgvr.Cells[dgvcAnalyzerId.Index].Value;
                cmd.Parameters.AddWithValue("@AnalyzerId", analyzerId);
                cmd.Parameters.AddWithValue("@AnalyzerTypeId", (int)dgvr.Cells[dgvcAnalyzerTypeId.Index].Value);
                cmd.Parameters.AddWithValue("@LocationId", Program.LocationId);
                cmd.Parameters.AddWithValue("@ConnectionType", selectedConnectionType);
                cmd.Parameters.AddWithValue("@TCP_IPAddress", (dgvr.Cells[dgvcIPAddress.Index].Value ?? ""));
                cmd.Parameters.AddWithValue("@TCP_PortNumber", (selectedConnectionType == ConnectionType.TCP ? dgvr.Cells[dgvcPortDetails.Index].Value : DBNull.Value));
                cmd.Parameters.AddWithValue("@Serial_PortName", (selectedConnectionType == ConnectionType.Serial ? (dgvr.Cells[dgvcPortDetails.Index].Value ?? "") : ""));
                cmd.Parameters.AddWithValue("@Serial_BaudRate", dgvr.Cells[dgvcBaudRate.Index].Value);
                cmd.Parameters.AddWithValue("@Serial_Parity", ParseParity(dgvr.Cells[dgvcParity.Index].Value));
                cmd.Parameters.AddWithValue("@Serial_StopBits", ParseStopBits(dgvr.Cells[dgvcStopBits.Index].Value));
                cmd.Parameters.AddWithValue("@Serial_DataBits", dgvr.Cells[dgvcDataBits.Index].Value);
                cmd.Parameters.AddWithValue("@FilePath", (selectedConnectionType == ConnectionType.FilePickup || selectedConnectionType == ConnectionType.FileConnected ? (dgvr.Cells[dgvcFileName.Index].Value ?? "") : ""));
                cmd.Parameters.AddWithValue("@IsActive", dgvr.Cells[dgvcIsActive.Index].Value ?? false);
                cmd.Parameters.AddWithValue("@CreatedBy", "");
                cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                cmd.Parameters.AddWithValue("@LastUpdatedBy", "");
                cmd.Parameters.AddWithValue("@LastUpdatedOn", DateTime.Now);
                cmd.Parameters.AddWithValue("@AdditionalSettings", strAdditionalSettings);

                if (LocalSqlCE.ExecuteNonQuery(cmd) != 1)
                {
                    MessageBox.Show("LIS configuration not saved.", "Port Number Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    TextLogger.WriteLogEntry("ServiceLogs", "AnalyzerId: " + analyzerId + ", LIS configuration saved.");
                }
            }
            catch (Exception ex)
            {
                BLL.UiMediator.LogAndShowError(0, ex, "Error in saving analyzer configuration");
            }
        }

        private ConnectionType ParseConnectionType(object connectionType)
        {
            if (connectionType == null || string.IsNullOrEmpty(connectionType.ToString()))
                return ConnectionType.NotSpecified;

            return (ConnectionType)Enum.Parse(typeof(ConnectionType), (connectionType ?? "").ToString());
        }

        private System.IO.Ports.Parity ParseParity(object baudRate)
        {
            return (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), (baudRate ?? "").ToString());
        }

        private System.IO.Ports.StopBits ParseStopBits(object stopBits)
        {
            return (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits), (stopBits ?? "").ToString());
        }

        private void dgvLISList_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"Error in Row \"{e.RowIndex + 1}\" Column \"{dgvLISList.Columns[e.ColumnIndex].HeaderText}\", Value: {dgvLISList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value}" +
                Environment.NewLine + Environment.NewLine + e.Exception.ToString(), "Grid Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void dgvLISList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData & Keys.KeyCode)
            {
                //case Keys.Up:
                case Keys.Right:
                //case Keys.Down:
                case Keys.Left:
                    if (!dgvLISList.IsCurrentCellInEditMode)
                    {
                        // Swallow arrow keys.
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
            }
            base.OnKeyDown(e);
        }

        private void SetGridviewColumnVisibility()
        {


        }

        private void FillSerialPortNames()
        {
            dgvcPortDetails.Items.Clear();
            foreach (string comPort in System.IO.Ports.SerialPort.GetPortNames())
            {
                dgvcPortDetails.Items.Add(comPort);
            }
            //dgvcPortDetails.Items.Add("COM8");
            //dgvcPortDetails.Items.Add("COM9");
        }

        private void chkAdvanced_CheckedChanged(object sender, EventArgs e)
        {
            //dgvcIPAddress.Visible = (rdoTcpSerial.Checked && chkAdvanced.Checked);
            dgvcBaudRate.Visible = dgvcParity.Visible = dgvcStopBits.Visible = dgvcDataBits.Visible = (rdoTcpSerial.Checked && chkAdvanced.Checked);
        }

        private void LISConfiguration_Shown(object sender, EventArgs e)
        {
            if (!Program.EnsureUserLogin())
            {
                InterfaceUIHelper.DisableFormControls(this);
            }
        }

        private void dgvLISList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void frmLISConfiguration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                txtSearch.Focus();
                txtSearch.Select(0, txtSearch.Text.Length);
                e.Handled = true;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            //search/filter grid rows
            //https://stackoverflow.com/questions/5843537/filtering-datagridview-without-changing-datasource
            //(dgvLISList.DataSource as DataTable).DefaultView.RowFilter = string.Format("Field = '{0}'", textBoxFilter.Text);
            //foreach (var dgvr in dgvLISList.Rows.Cast<DataGridViewRow>().Where(r => r.Cells[dgvcAnalyzerName.Index].Value.ToString().Contains(txtSearch.Text)))
            foreach (DataGridViewRow dgvr in dgvLISList.Rows)
            {
                dgvr.Visible = String.IsNullOrEmpty(txtSearch.Text) || dgvr.Cells[dgvcAnalyzerName.Index].Value.ToString().IndexOf(txtSearch.Text, StringComparison.InvariantCultureIgnoreCase) != -1 ? true : false;
            }
        }
    }

}
