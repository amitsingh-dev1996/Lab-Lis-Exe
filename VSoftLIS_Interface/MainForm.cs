using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Data.OleDb;
using System.IO;
using VSoftLIS_Interface.Common;
using VSoftLIS_Interface.DLL;
using VSoftLIS_Interface.BLL;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Threading.Tasks;
using System.Text;

namespace VSoftLIS_Interface
{
    internal delegate void DataReceivedCallback(CommunicationBase sender, string data);
    internal delegate void CommunicationEndedCallback();

    public partial class MainForm : Form
    {
        private static MainForm MainFormSelfReference = null;
        //public static string BlockClosingApp_Reason = "";
        CommunicationBase _communication = null;
        CommunicationBase _communication_BulkWO = null;
        ConnectionSettings connectionSettings_Result = null;
        ConnectionSettings connectionSettings_WO = null;
        private bool IsSamePortForBulkWO = false;
        ErrorLog errorLog = new ErrorLog();
        public static Analyzer analyzer { get { return Program.analyzer; } }
        internal static MessageConfiguration msgConfig = null;
        public static MessageLogger messageLogger = null;
        System.Threading.Timer logTimer = null;
        System.Threading.Timer resultSyncTimer = null;
        internal int logFrequency_Milliseconds = 5000;
        internal static int ResultCheckInterval { get { return Program.AnalyzerConfiguration.ConnectionSettings.ResultCheckInterval * 1000; } }
        internal static TimeZoneInfo TimeZone_India = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        BindingSource bSource = new BindingSource();
        private DataSet dsGridRecords = new DataSet();
        private bool IsSyncResultsRunning = true;
        ResultUpdater resultUpdater = null;
        public static string ApplicationDataFolder { get { return CommonSettings.ApplicationDataFolder; } }
        public static string ApplicationDataFolder_Common { get { return CommonSettings.ApplicationDataFolder_Common; } }

        const int analyzerTypeId_ICPThermo = Program.analyzerTypeId_ICPThermo, analyzerTypeId_LCMS = Program.analyzerTypeId_LCMS,
            analyzerTypeId_BiotekReader = Program.analyzerTypeId_BiotekReader, analyzerTypeId_Navios = Program.analyzerTypeId_Navios,
            analyzerTypeId_RotorGene = Program.analyzerTypeId_RotorGene, analyzerTypeId_KingfisherPCR = Program.analyzerTypeId_KingfisherPCR,
            analyzerTypeId_AgilentIcpms = Program.analyzerTypeId_AgilentIcpms, analyzerTypeId_QuantStudioManual = Program.analyzerTypeId_QuantStudioManual,
            analyzerTypeId_BDFacts = Program.analyzerTypeId_BDFacts, analyzerTypeId_Dimension = Program.analyzerTypeId_Dimension;

        protected string[] allowedExtensions = new string[] { };
        string excelSheetName = "";
        protected string dateFormat = "";
        protected string timeFormat = "";
        protected string systemShortTimeFormat = "";
        DataTable dtColumnsInfo = null;
        int headerRowNumber = -1;
        int dataStartRowNumber = -1;
        int unpivotHeaderColumns = -1;
        bool isInputUnpivotFormat = false;
        string[] barcodesToIgnore = new string[] { };
        string extension = "";
        string machineTestCode = "";
        int decimalPlacesToRound = 2;
        string kitCode = String.IsNullOrEmpty(ConfigurationManager.AppSettings["KitCode"]) ? "N gene" : ConfigurationManager.AppSettings["KitCode"];

        public MainForm()
        {
            InitializeComponent();

            UiMediator.mainForm = this;
            MainFormSelfReference = this;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                CachedData.UpdateDescriptiveResultMaster(analyzer.AnalyzerId, analyzer.AnalyzerTypeId);
                CachedData.UpdateValueLimits();

                msgConfig = new MessageConfiguration(analyzer.AnalyzerId);

                CachedData.UpdateTestList(analyzer.AnalyzerId);
                ScheduleMethodInThread(() => { CachedData.UpdateTestList(analyzer.AnalyzerId); }, TimeSpan.FromMinutes(5));

                //SQL CE connection constructor taking 14-15 seconds, so added in thread
                new Thread(() =>
                {
                    //resultUpdater = new ResultUpdater();
                }).Start();

                //MOVED TO CachedData.UpdateTestList()
                //if (msgConfig.AnalyzerTypeID == analyzerTypeId_LCMS)
                //{
                //    CachedData.TestList.Add(new TestListItem { _testCode = "DUMMY", _machineCode = "VDIS" });
                //}

                messageLogger = new MessageLogger(analyzer.AnalyzerId);
                logTimer = new System.Threading.Timer(LogCommunicationInServer, null, logFrequency_Milliseconds, System.Threading.Timeout.Infinite);
                //this.Text += " - v " + lisVersion + " - " + analyzer.AnalyzerName;
                if (!WebAPI.IsVSoftApiCommunication)
                {
                    this.Text += " (Charbi connection)";
                }

                //Thread trdUI = new Thread(() => UpdateUI());
                //trdUI.Start();

                connectionSettings_Result = new ConnectionSettings();
                connectionSettings_WO = new ConnectionSettings();

                connectionSettings_Result.ConnectionType = Program.AnalyzerConfiguration.ConnectionSettings.ConnectionType;
                connectionSettings_Result.Serial_PortName = connectionSettings_WO.Serial_PortName = Program.AnalyzerConfiguration.ConnectionSettings.Serial_PortName;
                connectionSettings_Result.Serial_BaudRate = connectionSettings_WO.Serial_BaudRate = Program.AnalyzerConfiguration.ConnectionSettings.Serial_BaudRate;
                connectionSettings_Result.Serial_Parity = connectionSettings_WO.Serial_Parity = Program.AnalyzerConfiguration.ConnectionSettings.Serial_Parity;
                connectionSettings_Result.Serial_StopBits = connectionSettings_WO.Serial_StopBits = Program.AnalyzerConfiguration.ConnectionSettings.Serial_StopBits;
                connectionSettings_Result.Serial_DataBits = connectionSettings_WO.Serial_DataBits = Program.AnalyzerConfiguration.ConnectionSettings.Serial_DataBits;

                connectionSettings_Result.TCP_IPAddress = Program.AnalyzerConfiguration.ConnectionSettings.TCP_IPAddress;
                connectionSettings_Result.TCP_PortNumber = Program.AnalyzerConfiguration.ConnectionSettings.TCP_PortNumber;
                connectionSettings_Result.TCP_IsServerMode = Program.AnalyzerConfiguration.ConnectionSettings.TCP_IsServerMode;

                // added code to act LIS As server when there is only port number else client
                if (connectionSettings_Result.TCP_IPAddress != "")
                {
                    connectionSettings_Result.TCP_IsServerMode = false;
                }
                else
                {
                    connectionSettings_Result.TCP_IsServerMode = true;
                }

                connectionSettings_Result.FilePath = Program.AnalyzerConfiguration.ConnectionSettings.FilePath;

                connectionSettings_WO.ConnectionType = ConnectionType.TCP;//kept default hardcode to TCP, as dual connection used only in TCP as of now
                connectionSettings_WO.TCP_IPAddress = Program.AnalyzerConfiguration.ConnectionSettings_WO.TCP_IPAddress;
                connectionSettings_WO.TCP_PortNumber = Program.AnalyzerConfiguration.ConnectionSettings_WO.TCP_PortNumber;
                connectionSettings_WO.TCP_IsServerMode = Program.AnalyzerConfiguration.ConnectionSettings_WO.TCP_IsServerMode;
                connectionSettings_WO.FilePath = Program.AnalyzerConfiguration.ConnectionSettings_WO.FilePath;

                //if (connectionSettings_WO.TCP_PortNumber == connectionSettings_Result.TCP_PortNumber)
                //    connectionSettings_WO.TCP_PortNumber = null;
                if (connectionSettings_WO.TCP_PortNumber == connectionSettings_Result.TCP_PortNumber)
                    IsSamePortForBulkWO = true;
                else
                    IsSamePortForBulkWO = false;

                if (connectionSettings_Result.IsFileBased)
                //&& (connectionSettings_WO.TCP_PortNumber == null && String.IsNullOrEmpty(connectionSettings_WO.Serial_PortName)))//added condition of WO to not mix different modes of result and WO
                {
                    if (connectionSettings_Result.IsFolderPickup)
                    {
                        if (analyzer.AnalyzerTypeId == analyzerTypeId_KingfisherPCR)
                        {
                            CachedData.Analyzer.AnalyzerTypeId =
                                analyzer.AnalyzerTypeId = analyzerTypeId_QuantStudioManual;
                            msgConfig = new MessageConfiguration(analyzer.AnalyzerId);
                        }
                    }

                    if (msgConfig.IsManualTestSelection)
                    {
                        cmbTestSelection.Items.Clear();
                        cmbTestSelection.Items.AddRange(CachedData.TestList.Select(r => r._testCode).OrderBy(r => r).ToArray());
                        ResetTestSelection();
                        pnlTestSelection.Visible = true;
                        machineTestCode = "";
                    }
                    else
                    {
                        cmbTestSelection.Visible = false;
                    }

                    if (connectionSettings_Result.ConnectionType == ConnectionType.FileUpload)
                    {
                        pnlSelectFolder_ResultFilePath.Visible = true;
                        msgConfig.UserSelectionToUploadResults = true;
                    }
                    else
                    {
                        tableLayoutPanel1.RowStyles[2].Height = 10;
                        ////tableLayoutPanel2.ColumnStyles[1].Width = 0;
                        //splitContainer1.Panel2.Width = 0;

                        Thread trdDatabaseInterface = new Thread(() => { Thread.Sleep(1000); FetchResultsInBackground(); });
                        trdDatabaseInterface.Start();

                        ////if (msgConfig.IsBulkWorklistSender)
                        ////    PerformBulkWorklistOperations();

                        //return;
                    }

                    if (msgConfig.UserSelectionToUploadResults)
                    {
                        btnUploadResults.Visible = true;
                        dgvRecords.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    }
                }

                CreateGrid();

                AttemptConnection();

                ScheduleMethodInThread(UpdateConnectivityStatus, TimeSpan.FromMilliseconds(1));
                ScheduleMethodInThread(CheckThreadCount, TimeSpan.FromMinutes(5));

                if (WebAPI.IsVSoftApiCommunication)
                {
                    ScheduleMethodInThread(ReattemptResults, TimeSpan.FromMinutes(2));
                }
            }
            catch (Exception ex)
            {
                errorLog.err_insert(ex);
                string message = InterfaceHelper.GetUserFriendlyErrorMessage(ex);
                MessageBox.Show((message != "" ? message : "Error in initializing, restart the application.") + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            if (Program.IsDebugMode)
            {
                txtCommunicationInput.Visible = true;
                pnlControlCharacters.Visible = true;
            }
        }

        private void AttemptConnection()
        {
            if (connectionSettings_Result.IsFileBased && connectionSettings_WO.IsFileBased)
                return;

            if (btnClosePort.Enabled)
                btnClosePort_Click(null, null);

            List<string> ports = FillSerialPorts();

            if (connectionSettings_Result.TCP_PortNumber.HasValue && ports.Count > 0)
            {
                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Both connections (TCP & Serial) are available, using TCP for communication");
            }

            //ConnectionSettings cs = connectionSettings_Result;
            //if (connectionSettings_WO.TCP_PortNumber.HasValue && !IsSamePortForBulkWO)
            //    cs = connectionSettings_WO;
            if (connectionSettings_Result.TCP_PortNumber.HasValue || connectionSettings_WO.TCP_PortNumber.HasValue)
            {
                if (!String.IsNullOrEmpty(connectionSettings_Result.TCP_IPAddress) && InterfaceHelper.ParseValidIpAddress(connectionSettings_Result.TCP_IPAddress) == null)
                {
                    MessageBox.Show("Invalid IP address configured: " + connectionSettings_Result.TCP_IPAddress);
                    connectionSettings_Result.TCP_IPAddress = "";
                }
                if (!String.IsNullOrEmpty(connectionSettings_WO.TCP_IPAddress) && InterfaceHelper.ParseValidIpAddress(connectionSettings_WO.TCP_IPAddress) == null)
                {
                    MessageBox.Show("Invalid IP address configured: " + connectionSettings_WO.TCP_IPAddress);
                    connectionSettings_WO.TCP_IPAddress = "";
                }
                if (connectionSettings_WO.TCP_PortNumber.HasValue)
                {
                    txtTCP_HostIpAddress.Text = !String.IsNullOrEmpty(connectionSettings_WO.TCP_IPAddress) ? connectionSettings_WO.TCP_IPAddress : Program.LocalIpAddress;
                    nudTCP_HostPort.Value = connectionSettings_WO.TCP_PortNumber.Value;
                }
                else if (connectionSettings_Result.TCP_PortNumber.HasValue)
                {
                    txtTCP_HostIpAddress.Text = !String.IsNullOrEmpty(connectionSettings_Result.TCP_IPAddress) ? connectionSettings_Result.TCP_IPAddress : Program.LocalIpAddress;
                    nudTCP_HostPort.Value = connectionSettings_Result.TCP_PortNumber.Value;
                }
                pnlTcpConnection.Visible = true;
                pnlSerialConnection.Location = pnlTcpConnection.Location;

                _communication = new TCPCommunication(analyzer, DataReceived);

                if (!connectionSettings_WO.IsFileBased && connectionSettings_WO.TCP_PortNumber.HasValue)
                {
                    if (IsSamePortForBulkWO)
                    {
                        _communication_BulkWO = _communication;
                    }
                    else
                    {
                        _communication_BulkWO = new TCPCommunication(analyzer, DataReceived);
                    }
                }

                btnOpenPort_Click(null, null);
            }
            else if (ports.Count > 0)
            {
                _communication = new SerialCommunication(analyzer, DataReceived);
                pnlSerialConnection.Visible = true;
                if (!String.IsNullOrEmpty(connectionSettings_Result.Serial_PortName) && ports.Contains(connectionSettings_Result.Serial_PortName))
                {
                    int comPortSelectedIndex = ports.IndexOf(connectionSettings_Result.Serial_PortName);
                    if (comPortSelectedIndex == -1)
                    {
                        UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, connectionSettings_Result.Serial_PortName + " is configured in settings, but not available in connected ports. Kindly connect desired port or correct the port in settings.");
                        return;
                    }
                    cmbSerialPorts.SelectedIndex = comPortSelectedIndex;
                    btnOpenPort_Click(null, null);
                }

                if (Program.IsDebugMode)
                {
                    btnOpenPort_Click(null, null);
                    _communication.ConnectionStatus = ConnectionStatus.Connected;
                    if (connectionSettings_WO.TCP_PortNumber.HasValue && !IsSamePortForBulkWO)
                        _communication_BulkWO.ConnectionStatus = ConnectionStatus.Connected;
                }
            }
            else
            {
                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "No connection available, kindly check Serial Port / TCP Setting, and restart application");
            }

            if (_communication != null)
            {
                _communication.ConnectionStatusChanged += new EventHandler(delegate (object sender, EventArgs e)
                {
                    UpdateConnectionStatus((sender as CommunicationBase).ConnectionStatus);
                });
                //set same value to trigger event
                _communication.ConnectionStatus = _communication.ConnectionStatus;

                if (_communication.IsOpen == false)
                {
                    btnOpenPort.Enabled = true;
                }

                //Thread trdConnectionStatus = new Thread(() =>
                //{
                //    while (true)
                //    {
                //        CommunicationBase cb = _communication2 != null ? _communication2 : _communication;

                //        UpdateConnectionStatus(cb.GetConnectionStatus());
                //        Thread.Sleep(1000);
                //    }
                //});
                //trdConnectionStatus.Start();
            }
        }

        private List<string> FillSerialPorts()
        {
            List<string> ports = new List<string>(SerialPort.GetPortNames());

            if (Program.IsDebugMode)
            {
                ports.Add("Textbox");
            }

            foreach (string p in ports)
                cmbSerialPorts.Items.Add(p);

            if (Program.IsDebugMode)
                cmbSerialPorts.SelectedIndex = 0;

            if (ports.Count == 0)
            {
                cmbSerialPorts.Items.Add("No Ports Available");
            }

            return ports;
        }

        private void DataReceived(CommunicationBase sender, string data)
        {

            //DebugData(MessageType.Normal, data);
            //MessageLogger.LogCommunication(data, "DETAIL");
            //this.lblReceiveInfo.Invoke((MethodInvoker)delegate
            //{
            //    this.lblReceiveInfo.BackColor = Color.Lime;
            //});
            //Added logic for GNPL advia machine which is giving extra unwanted character
            if (data == "ÿ" && msgConfig.AnalyzerTypeID == 8)
            {
                data = string.Empty;
            }

            try
            {
                if (sender == null && Program.IsDebugMode)
                    sender = _communication;

                //if (_communication.TransmissionType == TransmissionType.Hex)
                //{
                //    data = InterfaceHelper.HexStringToAsciiString(data);
                //}

                if (!_communication.AcknowledgePartialFrame)
                {
                    DebugData(MessageType.Normal, data);
                    MessageLogger.LogCommunication(data, "DETAIL");
                    //MessageLogger.LogCommunication(data, analyzer.AnalyzerName);
                }

                sender.ProcessResponse(analyzer, data);

                //if (data == Characters.ENQ)
                //{
                //    sender.IsCommunicationIdle = false;
                //}
                //else if (data == Characters.EOT)
                //{
                //    sender.IsCommunicationIdle = true;
                //    sender.Sender = "";
                //}
            }
            catch (Exception ex)
            {
                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, ex.Message.ToString() /*+ "\n"*/);
                ex.Data.Add("data", data);
                errorLog.err_insert(ex);
            }
        }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            try
            {
                if (_communication is SerialCommunication)
                {
                    if (cmbSerialPorts.SelectedItem == null)
                    {
                        MessageBox.Show("Please select a port.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    connectionSettings_Result.Serial_PortName = cmbSerialPorts.SelectedItem.ToString();
                    _communication.OpenPort(connectionSettings_Result);

                    ////remember port selectcion if connection successful
                    //InterfaceUIHelper.UpdateAppSettingValue("SerialPortName", connectionSettings_Result.Serial_PortName);
                }

                if (_communication is TCPCommunication || _communication is TCPCommunication)
                {
                    if (connectionSettings_WO.TCP_PortNumber.HasValue && !IsSamePortForBulkWO)
                    {
                        _communication_BulkWO.OpenPort(connectionSettings_WO);
                    }
                    else if (connectionSettings_Result.TCP_PortNumber.HasValue)
                        _communication.OpenPort(connectionSettings_Result);

                    if (connectionSettings_WO.TCP_PortNumber.HasValue && !connectionSettings_Result.TCP_PortNumber.HasValue)
                        _communication = _communication_BulkWO;

                }

                if (_communication.IsOpen || Program.IsDebugMode)
                {
                    UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Port " + _communication.ToString() + " opened at " + DateTime.Now /*+ "\n"*/);
                    if (connectionSettings_WO.TCP_PortNumber.HasValue && !IsSamePortForBulkWO && _communication_BulkWO.IsOpen)
                    {
                        UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Port " + _communication_BulkWO.ToString() + " opened at " + DateTime.Now /*+ "\n"*/);
                    }

                    pnlSerialConnection.Enabled = pnlTcpConnection.Enabled = false;
                    btnOpenPort.Enabled = false;
                    btnClosePort.Enabled = true;

                    if (msgConfig.IsBulkWorklistSender)
                    {
                        ScheduleMethodInThread(PerformBulkWorklistOperations, TimeSpan.FromMinutes(2));

                        if (!connectionSettings_WO.IsFileBased)
                            _communication_BulkWO.InitiateBulkWOSend();
                    }
                }
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(analyzer.AnalyzerId, ex);
            }
        }

        private void btnClosePort_Click(object sender, EventArgs e)
        {
            if (_communication.IsOpen)
            {
                _communication.ClosePort();
                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Port " + _communication.ToString() + " closed at " + DateTime.Now /*+ "\n"*/);
            }
            if (connectionSettings_WO.TCP_PortNumber.HasValue && !IsSamePortForBulkWO && _communication_BulkWO.IsOpen)
            {
                _communication_BulkWO.ClosePort();
                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Port " + _communication_BulkWO.ToString() + " closed at " + DateTime.Now /*+ "\n"*/);
            }

            pnlSerialConnection.Enabled = pnlTcpConnection.Enabled = true;
            btnOpenPort.Enabled = true;
            btnClosePort.Enabled = false;

            LogCommunicationInServer(null);
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            //Program.RestartApplication();
            Application.Restart();
        }

        private void btnRefreshConnection_Click(object sender, EventArgs e)
        {
            MessageLogger.LogCommunication("RefreshConn button clicked....", "LIS");
            AttemptConnection();
        }

        private void txtCommunicationInput_TextChanged(object sender, EventArgs e)
        {
            //if (btnOpenPort.Enabled == true)
            //{
            //    txtCommunicationInput.Text = "Connection is closed.";
            //    return;
            //}

            if (txtCommunicationInput.Text.Trim() != "")
            {
                Dictionary<int, string> ReplaceCharacterList = new Dictionary<int, string>{
                { 2, "[STX]" },
                { 3, "[ETX]" },
                { 21, "[NAK]" },
                { 5, "[ENQ]" },
                { 4, "[EOT]" },
                { 6, "[ACK]" },
                { 13, "[CR]" },
                { 23, "[ETB]" },
                { 10, "[LF]" },
                { 1, "[SOH]" },
                { 17, "[DC1]" },
                { 28, "[FS]" }
                };

                string rowSplitMessage = txtCommunicationInput.Text.Trim().Replace("[STX]", "\r\n[STX]");
                foreach (string msg in rowSplitMessage.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string messageToPass = msg;
                    ReplaceCharacterList.ToList<KeyValuePair<int, string>>().ForEach(r => messageToPass = messageToPass.Replace(r.Value, Convert.ToChar(r.Key).ToString()));

                    if (_communication.TransmissionType == TransmissionType.Hex)
                    {
                        messageToPass = messageToPass.Replace(' ', '\0');
                        //messageToPass = InterfaceHelper.AsciiStringToHexString();
                    }

                    if ((_communication as TCPCommunication)?.ConnectionStatus == ConnectionStatus.Connected)
                    {
                        _communication.WriteData(messageToPass);
                    }
                    else
                    {
                        DataReceived(null, messageToPass);
                    }
                }
            }
        }

        private void CreateGrid()
        {
            dsGridRecords = new DataSet("LIS_Interface");
            DataTable dt = new DataTable();
            DataColumn dcIsSelected = dt.Columns.Add("IsSelected", typeof(bool));
            dcIsSelected.DefaultValue = false;
            dt.Columns.Add("Barcode");
            dt.Columns.Add("ProcessDate");
            dt.Columns.Add("Labcode", typeof(long));
            dt.Columns.Add("Timestamp");
            dt.Columns.Add("Request_Type");
            dt.Columns.Add("Test_Code");
            dt.Columns.Add("Value");
            dt.Columns.Add("ResultAbnormalFlag");
            dt.Columns.Add("Remarks");
            dt.Columns.Add("ValuesForReference", typeof(object[]));
            dt.Columns.Add("PatientName");
            dt.Columns.Add("Age");
            dt.Columns.Add("Gender");
            dt.Columns.Add("RefDr");
            dt.Columns.Add("PatientId");
            dsGridRecords.Tables.Add(dt);

            GridDataBound();

            if (msgConfig.UserSelectionToUploadResults)
            {
                dgvRecords.Columns["IsSelected"].Visible = true;
                dgvRecords.Columns["IsSelected"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRecords.Columns["IsSelected"].DataPropertyName = "IsSelected";
            }

            if (msgConfig.ShowPatientDemographics)
            {
                dgvRecords.Columns["ProcessDate"].Visible = true;
                dgvRecords.Columns["Labcode"].Visible = true;
                dgvRecords.Columns["PatientName"].Visible = true;
                dgvRecords.Columns["Age"].Visible = true;
                dgvRecords.Columns["Gender"].Visible = true;
                dgvRecords.Columns["PatientId"].Visible = true;
                dgvRecords.Columns["RefDr"].Visible = true;
            }

            //if (msgConfig.InstrumentType == InstrumentTypes.Sorter)
            //{
            //    dgvRecords.Columns["Test_Code"].HeaderText = "Lab Code";
            //    dgvRecords.Columns["ResultAbnormalFlag"].HeaderText = "Bin No.";
            //    dgvRecords.Columns["value"].Visible = false;
            //}
        }

        private void GridDataBound()
        {
            dgvRecords.AutoGenerateColumns = false;
            dgvRecords.VirtualMode = true;
            //https://stackoverflow.com/questions/10226992/slow-performance-in-populating-datagridview-with-large-data
            dgvRecords.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            dgvRecords.RowHeadersVisible = false;

            //bSource.DataSource = dsGridRecords.Tables[0]; //.Select("", " Timestamp desc");
            dsGridRecords.Tables[0].DefaultView.Sort = "ProcessDate asc, Labcode asc";
            bSource.DataSource = dsGridRecords.Tables[0].DefaultView; //.Select("", " Timestamp desc");
            dgvRecords.DataSource = bSource;
        }

        private void ClearAllOutput()
        {
            ClearGrid();
            rtbMessage.Clear();
            rtbErrorMessages.Clear();
        }

        private void ClearGrid()
        {
            if (dgvRecords.DataSource != null)
                dsGridRecords.Tables[0].Rows.Clear();
        }

        private void DisplayRichText(RichTextBox rtb, MessageType type, string msg)
        {
            rtb.Invoke(new EventHandler(delegate
            {
                if (rtb.TextLength > 9000)
                    rtb.Clear();

                rtb.SelectedText = string.Empty;
                rtb.SelectionFont = new Font(rtb.SelectionFont, FontStyle.Bold);
                rtb.SelectionColor = Color.FromName(((MessageColor)((int)type)).ToString());
                rtb.AppendText(InterfaceHelper.ConvertToWritableMessage(msg) + Environment.NewLine);
                rtb.ScrollToCaret();
            }));
        }

        private void DebugData(MessageType type, string msg)
        {
            new System.Threading.Thread(() =>
            {
                DisplayRichText(rtbDebug, type, msg);
            }).Start();
        }

        private void UpdateUI()
        {
            while (true)
            {
                try
                {
                    ////UpdateUiMessages(UiMediator.GetUiMessages(analyzer.AnalyzerId).AsEnumerable());
                    UpdateUiGridMessages(UiMediator.GetUiGridData(analyzer.AnalyzerId).AsEnumerable());
                }
                catch (Exception ex)
                {
                    errorLog.err_insert(ex);
                }
                Thread.Sleep(100);
            }
        }

        internal void UpdateUiMessages(IEnumerable<DataRow> rows)
        {
            new Task(() =>
            {
                foreach (DataRow drMsg in rows)
                {
                    MessageType type = (MessageType)drMsg["MessageType"];
                    string strDatetime = ((DateTime)drMsg["Timestamp"]).ToString("dd/MM/yyyy HH:mm:ss.fff");
                    string msg = strDatetime + " " + (string)drMsg["Message"];

                    DisplayRichText(rtbMessage, type, msg);

                    if (type == MessageType.Error)
                    {
                        DisplayRichText(rtbErrorMessages, type, msg);
                    }

                    string messageToLog = String.Format("{0,-10}", type.ToString().ToUpper()) + InterfaceHelper.ConvertToWritableMessage((string)drMsg["Message"]);
                    TextLogger.WriteTextFile(Path.Combine(ApplicationDataFolder, "Logs", "UIMessages.txt")
                        , strDatetime + " " + messageToLog + Environment.NewLine
                        , AppendDateInFilename: true);

                    TextLogger.WriteLogEntry("Debugging", messageToLog);
                }
            }).Start();
        }

        internal void UpdateUiGridMessages(IEnumerable<DataRow> rows)
        {
            new Task(() =>
            {
                foreach (DataRow drGrid in rows)
                {
                    //used BeginInvoke instead of Invoke to avoid slowness in UI update observed in large number of results
                    this.dgvRecords.BeginInvoke((MethodInvoker)delegate
                    {
                        try
                        {
                            if (dsGridRecords.Tables[0].Rows.Count > 1000)
                                ClearGrid();

                            string type = (string)drGrid["Type"];
                            DataRow dr = dsGridRecords.Tables[0].NewRow();
                            dr["IsSelected"] = false;
                            dr["Barcode"] = (string)drGrid["Barcode"];
                            dr["Timestamp"] = ((DateTime)drGrid["Timestamp"]).ToString("dd/MM/yyyy hh:mm:ss tt");
                            dr["Request_Type"] = type;
                            dr["Test_Code"] = (string)drGrid["Testcode"];
                            dr["Value"] = drGrid["TestValue"] == DBNull.Value ? "" : drGrid["TestValue"].ToString();
                            dr["ResultAbnormalFlag"] = (string)drGrid["ResultAbnormalFlag"];
                            dr["Remarks"] = (string)drGrid["AdditionalInfo"];
                            dr["ValuesForReference"] = drGrid["ValuesForReference"];
                            if (msgConfig.ShowPatientDemographics && drGrid["PatientInfo"] is BarcodeList)
                            {
                                BarcodeList patientInfo = drGrid["PatientInfo"] as BarcodeList;
                                if (patientInfo != null)
                                {
                                    dr["ProcessDate"] = patientInfo.ProcessDate.ToString("dd/MM/yyyy");
                                    dr["Labcode"] = patientInfo.Labcode;

                                    dr["PatientName"] = patientInfo.patientName;
                                    dr["Age"] = patientInfo.Age;
                                    dr["Gender"] = patientInfo.Gender;
                                    dr["PatientId"] = patientInfo.PatientId;
                                    dr["RefDr"] = patientInfo.RefDr;
                                }
                            }

                            dsGridRecords.Tables[0].Rows.Add(dr);

                            dgvRecords.ClearSelection();
                            if (dgvRecords.Rows.Count > 0)
                            {
                                dgvRecords.Rows[dgvRecords.Rows.Count - 1].Selected = true;
                                dgvRecords.FirstDisplayedScrollingRowIndex = dsGridRecords.Tables[0].Rows.Count - 1;
                            }
                        }
                        catch (Exception ex)
                        {
                            UiMediator.LogAndShowError(analyzer.AnalyzerId, ex);
                        }
                    });
                }
            }).Start();
        }

        void LogCommunicationInServer(object state)
        {
            try
            {
                MessageLogger.LogCommunicationInServer();
            }
            catch (Exception ex)
            {
                errorLog.err_insert(ex);
            }
            finally
            {
                //https://stackoverflow.com/questions/12796148/system-threading-timer-in-c-sharp-it-seems-to-be-not-working-it-runs-very-fast
                logTimer.Change(logFrequency_Milliseconds, System.Threading.Timeout.Infinite);
            }
        }

        private void btnUploadResults_Click(object sender, EventArgs e)
        {
            MessageProcessor messageProcessor_ForUploadResults = new MessageProcessor(analyzer.AnalyzerId);
            int resultsCount = 0;
            var selectedRows = dgvRecords.Rows.Cast<DataGridViewRow>().Where(r => (bool)r.Cells["IsSelected"].Value == true);
            foreach (DataGridViewRow dgvr in selectedRows)
            {
                if ((string)dgvr.Cells["request_type"].Value == "RESULT")
                {
                    object[] values = dgvr.Cells["ValuesForReference"].Value as object[];
                    messageProcessor_ForUploadResults.AddResultRecord(values[0].ToString(), values[1].ToString(), Convert.ToDecimal(values[2]), values[3].ToString(), values[4].ToString(), (DateTime)values[5], descriptiveId: (int)values[6], showInGrid: false);
                }
                resultsCount++;
            }
            messageProcessor_ForUploadResults.UpdateTestValues();
            UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Outgoing, resultsCount + " results uploaded.");

            while (selectedRows.Any())
            {
                dgvRecords.Rows.Remove(selectedRows.ElementAt(0));
            }
            MessageBox.Show(resultsCount + " result(s) uploaded successfully.", "Uploaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dgvRecords_Click(object sender, EventArgs e)
        {
            PauseByUserAction();
        }

        private void dgvRecords_KeyDown(object sender, KeyEventArgs e)
        {
            PauseByUserAction();
        }

        private void PauseByUserAction()
        {
            IsSyncResultsRunning = false;
            resultSyncTimer = new System.Threading.Timer(new TimerCallback(ResumeResultSync), null, 30000, System.Threading.Timeout.Infinite);
        }

        private void ResumeResultSync(object state)
        {
            IsSyncResultsRunning = true;
        }

        private void dgvRecords_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                if (dgvRecords.Rows.Count > 0)
                {
                    bool selectionValue = true;
                    if (!dgvRecords.Rows.Cast<DataGridViewRow>().Any(r => (bool)(r.Cells["IsSelected"]?.Value ?? false) == false))
                    {
                        selectionValue = false;
                    }

                    foreach (DataGridViewRow dgvr in dgvRecords.Rows)
                    {
                        dgvr.Cells["IsSelected"].Value = selectionValue;
                    }
                }
            }
            else
            {
                ToggleRowSelection(e.RowIndex);
            }
            dgvRecords.EndEdit();
        }

        private void ToggleRowSelection(int rowIndex)
        {
            dgvRecords.Rows[rowIndex].Cells["IsSelected"].Value = (dgvRecords.Rows[rowIndex].Cells["IsSelected"].Value == null ? true : !(bool)dgvRecords.Rows[rowIndex].Cells["IsSelected"].Value);
        }

        private void FetchResultsInBackground()
        {
            Invoke(new Action(() =>
            {
                pnlSerialConnection.Visible = false;
                pnlTcpConnection.Visible = false;
                btnOpenPort.Visible = false;
                btnClosePort.Visible = false;
            }));

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection app = config.AppSettings;

            while (true)
            {
                if (!IsSyncResultsRunning)
                {
                    Thread.Sleep(ResultCheckInterval);
                    continue;
                }

                try
                {
                    if (Program.AnalyzerConfiguration.ConnectionSettings.IsFolderPickup)
                    {
                        string pickupFolderName = connectionSettings_Result.FilePath;
                        if (String.IsNullOrEmpty(pickupFolderName) || !Directory.Exists(pickupFolderName))
                            throw new Exception("Result pickup folder '" + pickupFolderName + "' does not exist.");

                        //.OrderByDescending(r => r.LastWriteTime).FirstOrDefault();

                        FileInfo[] filesAvailable = new DirectoryInfo(pickupFolderName).GetFiles();

                        foreach (var fiLatest in filesAvailable)
                        {
                            ClearGrid();

                            UploadFile(fiLatest.FullName);
                        }
                    }
                    else if (connectionSettings_Result.ConnectionType == ConnectionType.FileConnected)
                    {
                        MessageProcessor messageProcessor = null;
                        switch (analyzer.AnalyzerTypeId)
                        {
                            case 11: //TOSOH G8
                            case 62: //TOSOH G11
                                int recordsBatchCount = 10;

                                int lastSyncedRecNo = Convert.ToInt32(app.Settings["LastSyncedRecNo"].Value);
                                DataTable dtPending = new DataTable();
                                List<string> invalidOrFailedRecNo = app.Settings["InvalidOrFailedRecNo"].Value.Split(',').ToList();
                                string strQuery = "SELECT TOP " + (recordsBatchCount + invalidOrFailedRecNo.Count) + " RecNo, Barcode, Element5, DateTime FROM MeasureData WHERE " +
                                    "(" + (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DeviceName"]) ? "DeviceID='" + ConfigurationManager.AppSettings["DeviceName"] + "' AND " : "") + " VAL(RecNo) > " + lastSyncedRecNo + ")";
                                if (invalidOrFailedRecNo.Count > 0)
                                {
                                    strQuery += " OR RecNo IN ('" + String.Join("', '", invalidOrFailedRecNo) + "')";
                                    app.Settings["InvalidOrFailedRecNo"].Value = "";
                                    config.Save(ConfigurationSaveMode.Modified);
                                }
                                strQuery += " ORDER BY VAL(RecNo)";
                                using (OleDbConnection oConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + connectionSettings_Result.FilePath + ";"))
                                {
                                    OleDbCommand oCmd = new OleDbCommand(strQuery, oConn);
                                    OleDbDataAdapter oDa = new OleDbDataAdapter(oCmd);
                                    oConn.Open();
                                    oDa.Fill(dtPending);
                                    oConn.Close();
                                }

                                if (dtPending != null && dtPending.Rows.Count > 0)
                                {
                                    try
                                    {
                                        messageProcessor = new MessageProcessor(analyzer.AnalyzerId);
                                        foreach (DataRow drPending in dtPending.Rows)
                                        {
                                            string barcode = (string)drPending["Barcode"];

                                            //check for valid barcode
                                            if (Regex.IsMatch(barcode, @"^\w+$"))
                                            {
                                                worklist worklist = messageProcessor.Machinerequest(dtPending.AsEnumerable().Select(r => r.Field<string>("Barcode")).ToList(), 3);
                                                messageProcessor.AddResultRecord(barcode, "HbA1C", Convert.ToDecimal(drPending["Element5"]), "", "", (DateTime)drPending["DateTime"], showInGrid: true, patientInfo: worklist.barcodeList.Where(r => r.Barcode.Equals(barcode)).SingleOrDefault());
                                            }
                                            else
                                            {
                                                app.Settings["InvalidOrFailedRecNo"].Value += (String.IsNullOrEmpty(app.Settings["InvalidOrFailedRecNo"].Value) ? "" : ",") + drPending["RecNo"];
                                                invalidOrFailedRecNo.Add(drPending["RecNo"].ToString());
                                                config.Save(ConfigurationSaveMode.Modified);
                                            }
                                        }

                                        if (!msgConfig.UserSelectionToUploadResults)
                                        {
                                            messageProcessor.UpdateTestValues();
                                            UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Outgoing, "Results batch uploaded.");
                                        }

                                        //if (!invalidOrFailedRecNo.Contains(dtPending.Rows[dtPending.Rows.Count - 1]["RecNo"].ToString()))
                                        //{
                                        lastSyncedRecNo = Convert.ToInt32(dtPending.Rows[dtPending.Rows.Count - 1]["RecNo"]);
                                        //}
                                        //lastSyncedRecNo = Convert.ToInt32(dtPending.AsEnumerable().Where(r => !invalidOrFailedRecNo.Contains(r.Field<string>("RecNo"))).LastOrDefault()["RecNo"].ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        UiMediator.LogAndShowError(analyzer.AnalyzerId, ex);
                                        recordsBatchCount = 1;
                                        lastSyncedRecNo = Convert.ToInt32(dtPending.Rows[0]["RecNo"]);
                                    }
                                    app.Settings["LastSyncedRecNo"].Value = lastSyncedRecNo.ToString();
                                    config.Save(ConfigurationSaveMode.Modified);
                                }
                                else
                                {
                                    Thread.Sleep(5000);
                                }
                                break;
                        }
                    }
                    Thread.Sleep(ResultCheckInterval);
                }
                catch (Exception ex)
                {
                    UiMediator.LogAndShowError(analyzer.AnalyzerId, ex);
                }
            }
        }

        private void ResetTestSelection()
        {
            Invoke(new Action(() =>
            {
                if (msgConfig.IsManualTestSelection)
                {
                    //if (cmbTestSelection.Items.Count == 1)
                    //{
                    //    cmbTestSelection.SelectedIndex = 0;
                    //}
                    //else
                    //{
                    cmbTestSelection.SelectedIndex = -1;
                    cmbTestSelection.Text = "--Select Test--";
                    cmbTestSelection.Focus();
                    machineTestCode = "";
                    //}
                }
            }));
        }

        private void AttemptUploadFile()
        {
            //if (connectionSettings_Result.ConnectionType != ConnectionType.FileUpload)
            //    return;

            try
            {
                ClearAllOutput();

                string filePath = txtSelectFolder_ResultFilePath.Text;
                if (!String.IsNullOrEmpty(filePath) && (!msgConfig.IsManualTestSelection || cmbTestSelection.SelectedIndex > -1))
                {
                    UploadFile(filePath);
                    //txtSelectFolder_ResultFilePath.Text = "";
                }
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(analyzer.AnalyzerId, ex);
            }
        }

        private void UploadFile(string fileFullPath)
        {
            string targetFolderName = "ResultFiles";

            string FilePath = fileFullPath;
            string FileName = Path.GetFileName(fileFullPath);
            extension = Path.GetExtension(FileName);

            try
            {
                ///machineTestCode = Path.GetFileNameWithoutExtension(FileName);
                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Started file " + FilePath);

                if (msgConfig.InstrumentType == InstrumentTypes.Analyzer)
                {
                    MessageProcessor messageProcessor = new MessageProcessor(analyzer.AnalyzerId);

                    if (msgConfig.IsFileCommunication)
                    {
                        string[] strFileLines = File.ReadAllLines(FilePath);
                        messageProcessor.ProcessCompleteRecord(strFileLines.ToList());
                    }
                    else
                    {
                        PopulateSettings();

                        if (!allowedExtensions.Any(r => r == extension))
                        {
                            UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Warning, "File is with invalid extension " + FilePath);
                            return;
                        }

                        if (msgConfig.IsManualTestSelection)
                        {
                            this.Invoke(new Action(() =>
                            {
                                if (cmbTestSelection.SelectedIndex == -1)
                                    UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Select desired test code");
                            }));

                            if (Program.AnalyzerConfiguration.ConnectionSettings.IsFolderPickup)
                            {
                                while (String.IsNullOrEmpty(machineTestCode))
                                {
                                    Thread.Sleep(100);
                                }
                            }
                        }

                        DataTable dtImportedData = null;
                        if (extension == ".xls" || extension == ".xlsx" || extension == ".xlsm")
                        {
                            dtImportedData = ImportExcel(FilePath, extension, false);
                        }
                        else if (extension == ".csv" || extension == ".txt")
                        {
                            dtImportedData = ImportCsv(FilePath);
                        }
                        else if (extension == ".xml")
                        {
                            dtImportedData = ImportXml(FilePath, extension, false);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        if (dtImportedData == null)
                        {
                            UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "Could not load file data: " + FilePath);
                            return;
                        }

                        string barcodeColumnName = dtColumnsInfo.AsEnumerable().Where(r => r.Field<string>("ColumnName").Equals("BARCODE", StringComparison.InvariantCultureIgnoreCase)
                            || (r.Field<string>("TargetColumnName")?.Equals("BARCODE", StringComparison.InvariantCultureIgnoreCase) ?? false)).Select(r => r.Field<string>("ColumnName")).SingleOrDefault();
                        if (!String.IsNullOrEmpty(barcodeColumnName))
                            dtImportedData.AsEnumerable().Where(r => String.IsNullOrEmpty(r.Field<string>(barcodeColumnName)) || r.Field<string>(barcodeColumnName).Length < 4 || barcodesToIgnore.Contains(r.Field<string>(barcodeColumnName), StringComparer.InvariantCultureIgnoreCase)).ToList()
                                .ForEach(r => dtImportedData.Rows.Remove(r));

                        Dictionary<KeyValuePair<int, int>, string> cellValidationErrors = new Dictionary<KeyValuePair<int, int>, string>();
                        int tempInt = 0; decimal tempDecimal = 0; DateTime tempDate;
                        for (int i = 0; i < dtImportedData.Rows.Count; i++)
                        {
                            for (int j = 0; j < dtColumnsInfo.Rows.Count; j++)
                            {
                                DataRow drCI = dtColumnsInfo.Rows[j];
                                string strValue = dtImportedData.Rows[i][drCI["ColumnName"].ToString()].ToString();
                                KeyValuePair<int, int> currCellPosition = new KeyValuePair<int, int>(i, j);

                                if (isInputUnpivotFormat && drCI["ColumnName"].ToString() == "TESTCODE")
                                {
                                    if (msgConfig.RemoveUnmappedTestcodes && !CachedData.TestList.Where(r => r._machineCode.Equals(strValue, StringComparison.InvariantCultureIgnoreCase)).Any())
                                    {
                                        cellValidationErrors.Add(currCellPosition, "Mapping for Machine Testcode " + strValue + " does not exist, for selected analyzer.");
                                    }
                                }

                                switch (drCI["DataType"].ToString())
                                {
                                    case "Integer":
                                        if (!int.TryParse(strValue, out tempInt))
                                            cellValidationErrors.Add(currCellPosition, "Invalid integer value (" + strValue + ").");
                                        break;

                                    case "Decimal":
                                        if (String.IsNullOrEmpty(strValue) || strValue.Replace("-", "") == "")//LCMS has ---- value for few results
                                        {
                                            strValue = "0";
                                            dtImportedData.Rows[i][drCI["ColumnName"].ToString()] = 0;
                                        }
                                        if (String.IsNullOrEmpty(strValue) || strValue.Contains(">") || strValue.Contains("<") && msgConfig.AnalyzerTypeID == 35)
                                        {
                                            strValue = "0";
                                            dtImportedData.Rows[i][drCI["ColumnName"].ToString()] = 0;
                                        }

                                        //added NumberStyles.Float for reading exponential value
                                        if (!decimal.TryParse(strValue, NumberStyles.Float, CultureInfo.InvariantCulture, out tempDecimal))
                                            cellValidationErrors.Add(currCellPosition, "Invalid decimal value (" + strValue + ").");

                                        //if (tempDecimal == 0)
                                        //    UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Warning, "Zero result value at");
                                        break;

                                    case "Date":
                                        if (DateTime.TryParseExact(strValue, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate) || DateTime.TryParseExact(strValue, dateFormat + " " + timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                                        {
                                            dtImportedData.Rows[i][drCI["ColumnName"].ToString()] = tempDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                                        }
                                        else
                                        {
                                            cellValidationErrors.Add(currCellPosition, "Invalid date value (" + strValue + ").");
                                        }
                                        break;

                                    case "Time":
                                        if (DateTime.TryParseExact(strValue, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                                        {
                                            dtImportedData.Rows[i][drCI["ColumnName"].ToString()] = tempDate.ToString("hh:mm:ss tt");
                                        }
                                        else
                                        {
                                            cellValidationErrors.Add(currCellPosition, "Invalid time value (" + strValue + ").");
                                        }
                                        break;

                                    case "DateTime":
                                        if (strValue == "")
                                        {
                                            dtImportedData.Rows[i][drCI["ColumnName"].ToString()] = DBNull.Value;
                                        }
                                        else if (DateTime.TryParseExact(strValue, dateFormat + " " + timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate)
                                            || DateTime.TryParseExact(strValue, dateFormat + " " + systemShortTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate)
                                            || DateTime.TryParseExact(strValue, dateFormat + " " + "H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate)) //this format added for edited file of Agilent ICPMS
                                        {
                                            dtImportedData.Rows[i][drCI["ColumnName"].ToString()] = tempDate.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                                        }
                                        else
                                        {
                                            cellValidationErrors.Add(currCellPosition, "Invalid datetime value (" + strValue + ").");
                                        }
                                        break;
                                }
                            }
                        }

                        int validationErrorCount = 0;
                        if (!isInputUnpivotFormat)
                        {
                            var unmappedTestcodes = dtImportedData.Columns.Cast<DataColumn>().Skip(unpivotHeaderColumns).Select(r => r.ColumnName)
                                .Except(CachedData.TestList.Select(r => r._machineCode));

                            if (unmappedTestcodes.Any())
                            {
                                validationErrorCount += unmappedTestcodes.Count();
                                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "Mapping for these Machine Testcode does not exist: " + String.Join(", ", unmappedTestcodes));
                            }
                        }

                        if (cellValidationErrors.Any())
                        {
                            validationErrorCount += cellValidationErrors.Count();
                            foreach (var err in cellValidationErrors)
                            {
                                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "Row " + err.Key.Key + " Column " + err.Key.Value + " : " + err.Value);
                            }
                        }

                        if (validationErrorCount > 0)
                        {
                            UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, validationErrorCount + " validation error(s) found.");
                            dtImportedData = null;
                        }
                        else
                        {
                            //DateTime tempDate;

                            for (int j = 0; j < dtColumnsInfo.Rows.Count; j++)
                            {
                                DataRow drCI = dtColumnsInfo.Rows[j];
                                string columnName = drCI["ColumnName"].ToString();
                                if (!String.IsNullOrEmpty(drCI["TargetColumnName"].ToString()))
                                {
                                    dtImportedData.Columns[drCI["ColumnName"].ToString()].ColumnName = columnName = drCI["TargetColumnName"].ToString();
                                }

                                for (int i = 0; i < dtImportedData.Rows.Count; i++)
                                {
                                    string strValue = dtImportedData.Rows[i][columnName].ToString();
                                    string dataType = !String.IsNullOrEmpty(drCI["TargetDataType"].ToString()) ? drCI["TargetDataType"].ToString() : drCI["DataType"].ToString();

                                    if (String.IsNullOrEmpty(strValue))
                                        dtImportedData.Rows[i][columnName] = DBNull.Value;

                                    switch (dataType)
                                    {
                                        case "DateTime":
                                        case "Date":
                                            if (DateTime.TryParseExact(strValue, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate) || DateTime.TryParseExact(strValue, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                                            {
                                                string format = "";
                                                if (dataType == "DateTime")
                                                    format = "yyyy-MM-ddTHH:mm:ss";
                                                else if (dataType == "Date")
                                                    format = "yyyy-MM-dd";
                                                dtImportedData.Rows[i][columnName] = tempDate.ToString(format);
                                            }
                                            break;
                                    }
                                }
                            }

                            //unpivot datatable
                            DataTable dtImportedData_Unpivot = null;
                            if (isInputUnpivotFormat)
                            {
                                dtImportedData_Unpivot = dtImportedData;
                            }
                            else
                            {
                                dtImportedData_Unpivot = new DataTable();

                                if (analyzer.AnalyzerTypeId == analyzerTypeId_AgilentIcpms)
                                    unpivotHeaderColumns = 2;

                                for (int i = 0; i < unpivotHeaderColumns; i++)
                                {
                                    dtImportedData_Unpivot.Columns.Add(dtImportedData.Columns[i].ColumnName);
                                }
                                dtImportedData_Unpivot.Columns.Add("TESTCODE");
                                dtImportedData_Unpivot.Columns.Add("TESTVALUE");

                                for (int i = 0; i < dtImportedData.Rows.Count; i++)
                                {
                                    DataRow drData = dtImportedData.Rows[i];
                                    IEnumerable<object> itemArray = drData.ItemArray.Take(unpivotHeaderColumns);
                                    for (int k = unpivotHeaderColumns; k < dtImportedData.Columns.Count; k++)
                                    {
                                        string columnName = dtImportedData.Columns[k].ColumnName;
                                        dtImportedData_Unpivot.Rows.Add(itemArray.Concat(new string[] { columnName, dtImportedData.Rows[i][columnName].ToString() }).ToArray());
                                    }
                                }
                            }

                            worklist worklist = messageProcessor.Machinerequest(dtImportedData_Unpivot.AsEnumerable().Select(r => r.Field<string>("Barcode")).Distinct().ToList(), 3);
                            foreach (DataRow drPending in dtImportedData_Unpivot.Rows)
                            {
                                string barcode = (string)drPending["Barcode"];
                                // DateTime resulted_Time = drPending.Table.Columns.Contains("RESULTED_TIME") ? DateTime.ParseExact(drPending["RESULTED_TIME"].ToString(), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) : DateTime.Now;
                                DateTime resulted_Time = drPending.Table.Columns.Contains("RESULTED_TIME") ? DateTime.ParseExact(drPending["RESULTED_TIME"].ToString(), "yyyy-MM-ddTHH:mm:ss", null) : DateTime.Now;

                                machineTestCode = analyzer.AnalyzerTypeId == analyzerTypeId_BiotekReader ? machineTestCode : (string)drPending["TESTCODE"];
                                TestListItem tli = messageProcessor.GetTestMappingOrEmpty(machineTestCode);

                                string strTestValue = drPending["TESTVALUE"].ToString();
                                //if (analyzer.AnalyzerTypeId == 35)
                                //{ 
                                //string strTestValue1  = drPending["PaitentName"].ToString();
                                //}
                                decimal testValue = 0;
                                int descriptiveId = -1;
                                var product_DescriptiveList = messageProcessor.GetDescriptiveMapping(tli._testCode);
                                if (product_DescriptiveList.Any())
                                {
                                    //descriptiveId = Convert.ToInt32(strTestValue);
                                    descriptiveId = messageProcessor.PopulateDescriptiveId(tli._testCode, strTestValue);
                                    testValue = -1;
                                }
                                else
                                {
                                    testValue = decimal.Round(Convert.ToDecimal(strTestValue), decimalPlacesToRound);
                                }
                                messageProcessor.AddResultRecord(barcode, machineTestCode, testValue, "", "", resulted_Time, descriptiveId: descriptiveId, showInGrid: true, patientInfo: worklist.barcodeList.Where(r => r.Barcode.Equals(barcode)).SingleOrDefault());
                            }

                            if (!msgConfig.UserSelectionToUploadResults)
                            {
                                messageProcessor.UpdateTestValues();
                                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Outgoing, "Results batch uploaded.");
                            }
                        }

                        //if (msgConfig.IsManualTestSelection)
                        //{
                        //    ResetTestSelection();
                        //}
                    }
                }
                else if (msgConfig.InstrumentType == InstrumentTypes.Archival)
                {
                    StreamReader streamReader = new StreamReader(FilePath);
                    string end = streamReader.ReadToEnd();
                    streamReader.Close();
                    string _fileContent = end;

                    string[] strArray = _fileContent.Split('|');
                    string RackNo = strArray[66].Replace("^", "");
                    string Position = strArray[67].Replace("^", "");
                    string Barcode = strArray[25];
                    UiMediator.AddUiGridData(msgConfig.AnalyzerID, Barcode, RackNo, Position, null, "", "", null, null);
                    SampleArchivalResponse response = WebAPI.UpdateArchiveSample(new SampleArchivalInput { barcode = Barcode, ArchivalPosition = Position, ArchivalRack = RackNo });
                    UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Barcode : " + Barcode + " Message : " + response.Message);
                }

            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(analyzer.AnalyzerId, ex);
                targetFolderName = "ResultFiles_Error";
            }
            finally
            {
                string newFileDirectory = Path.Combine(MainForm.ApplicationDataFolder, targetFolderName, DateTime.Now.Date.ToString("yyyy-MM-dd"));
                if (!Directory.Exists(newFileDirectory))
                    Directory.CreateDirectory(newFileDirectory);
                string newFileName = Path.GetFileNameWithoutExtension(FileName) + "_" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + extension;

                if (Program.AnalyzerConfiguration.ConnectionSettings.IsFolderPickup)
                {
                    File.Move(FilePath, Path.Combine(newFileDirectory, newFileName));
                }
                else
                {
                    File.Copy(FilePath, Path.Combine(newFileDirectory, newFileName));
                }
            }
        }

        private DataTable ImportExcel(string FileName, string extention, bool hasHeaders)
        {
            string xls_excel = "", xls_provider = "";
            if (extention == ".xls")
            {
                xls_excel = ";Extended Properties=\"Excel 8.0;HDR=";
                xls_provider = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
            }
            else if (extention == ".xlsx" || extention == ".xlsm")
            {
                xls_excel = ";Extended Properties=\"Excel 12.0;HDR=";
                xls_provider = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=";
            }
            string HDR = hasHeaders ? "Yes" : "No";
            string strConn = xls_provider + FileName + xls_excel + HDR + ";IMEX=1\"";
            DataTable dtExcelRawData = new DataTable();
            DataTable dtSheetData = new DataTable(excelSheetName);

            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();

                if (!GetExcelSheetNames(conn).Where(r => r == excelSheetName).Any())
                {
                    UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "Sheet \"" + excelSheetName + "\" does not exist in excel file.");
                    return null;
                }

                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + excelSheetName + "$]", conn);
                cmd.CommandType = CommandType.Text;
                //dsExcelRawData.Tables.Add(dtSheetData);
                new OleDbDataAdapter(cmd).Fill(dtExcelRawData);
            }

            if (analyzer.AnalyzerTypeId == analyzerTypeId_KingfisherPCR)
            {
                var headerRow = dtExcelRawData.AsEnumerable().Where(r => r.ItemArray[0].ToString() == "Well").SingleOrDefault();
                if (headerRow != null)
                {
                    headerRowNumber = dtExcelRawData.Rows.IndexOf(headerRow) + 1;
                    dataStartRowNumber = headerRowNumber + 1;
                }
            }

            DataTable dtImportedData = new DataTable();

            if (dtExcelRawData.Rows.Count < headerRowNumber)
            {
                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "The file uploaded does not have minimum number of rows required. Please check if you have uploaded correct file.");
                return null;
            }

            //TODO: remove following line
            //copy testcode columns
            if (headerRowNumber > 1 && unpivotHeaderColumns > -1)
            {
                dtExcelRawData.Rows[headerRowNumber - 1].ItemArray.Skip(unpivotHeaderColumns).ToList().ForEach(r => dtColumnsInfo.Rows.Add(r, "Decimal"));
            }
            //compare uploaded column names with predefined names
            for (int i = 0; i < dtColumnsInfo.Rows.Count; i++)
            {
                string strColumnInfo = dtColumnsInfo.Rows[i]["ColumnName"].ToString();
                dtImportedData.Columns.Add(strColumnInfo);
                if (headerRowNumber > 1 && analyzer.AnalyzerTypeId != analyzerTypeId_KingfisherPCR)
                {
                    string colName = dtExcelRawData.Rows[headerRowNumber - 1][i].ToString();
                    if (!Regex.IsMatch(strColumnInfo, @"Col\d+") && colName != strColumnInfo)
                    {
                        UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "Column \"" + strColumnInfo + "\" is missing or does not match defined position. Please upload data in predefined format.");
                        return null;
                    }
                }
            }

            if (analyzer.AnalyzerTypeId == analyzerTypeId_Navios)
            {
                string barcode = dtExcelRawData.Rows[3]["F4"].ToString();
                string resultedDateTime = dtExcelRawData.Rows[2]["F2"].ToString() + " " + dtExcelRawData.Rows[2]["F4"].ToString();
                string testcode = "";
                decimal? testValue = 0;

                ////WBC
                //string testcode = dtExcelRawData.Rows[14]["F8"].ToString().Trim(':');
                //string testValueWithUnit = dtExcelRawData.Rows[14]["F11"].ToString();
                //string remainingValuePart = "";
                //decimal? testValue = InterfaceHelper.ExtractNumericValue(testValueWithUnit, out remainingValuePart);
                //string unit = remainingValuePart;
                //dtImportedData.Rows.Add(testcode, barcode, testValue, resultedDateTime, unit);

                ////LYMPH
                //testcode = dtExcelRawData.Rows[14]["F15"].ToString().Trim(':');
                //testValue = Convert.ToDecimal(dtExcelRawData.Rows[14]["F16"].ToString());
                //dtImportedData.Rows.Add(testcode, barcode, testValue, resultedDateTime, "");

                bool IsHb27 = false;
                for (int i = dataStartRowNumber; i <= 25; i++)
                {
                    //CD3, CD4, CD8
                    testcode = dtExcelRawData.Rows[i]["F2"].ToString();
                    testValue = Convert.ToDecimal(dtExcelRawData.Rows[i]["F8"].ToString().TrimEnd('%'));

                    if (testcode == "HLA B27 + HLA B7-")
                    {
                        IsHb27 = true;
                        if (testValue >= 50.0M && testValue <= 60.0M)
                            testValue = 102; //BORDERLINE POSITIVE
                        else if (testValue < 50.0M)
                            testValue = 4; //NEGATIVE
                        else if (testValue > 60.0M)
                            testValue = 7; //POSITIVE
                    }
                    dtImportedData.Rows.Add(testcode, barcode, testValue, resultedDateTime, "");

                    if (IsHb27)
                        break;

                    //ACD3, ACD4, ACD8
                    testcode = dtExcelRawData.Rows[i]["F6"].ToString();
                    testValue = Convert.ToDecimal(dtExcelRawData.Rows[i]["F11"].ToString());
                    dtImportedData.Rows.Add(testcode, barcode, testValue, resultedDateTime, "");
                }

                if (!IsHb27)
                {
                    //RCD48
                    testcode = dtExcelRawData.Rows[26]["F2"].ToString();
                    testValue = Convert.ToDecimal(dtExcelRawData.Rows[26]["F8"].ToString());
                    dtImportedData.Rows.Add(testcode, barcode, testValue, resultedDateTime, "");
                }
            }
            else if (analyzer.AnalyzerTypeId == analyzerTypeId_KingfisherPCR)
            {
                string Testselected = machineTestCode;
                int testValue = 0;
                var headerCellValues = dtExcelRawData.Rows[headerRowNumber - 1].ItemArray.Select(r => r.ToString()).ToList();
                int barcodeColumnIndex = headerCellValues.IndexOf("Sample Name");
                int testcodeColumnIndex = headerCellValues.IndexOf("Target Name");
                int testValueColumnIndex = headerCellValues.IndexOf("CT");
                string resultedTime = dtExcelRawData.AsEnumerable().Where(r => r.ItemArray[0].ToString() == "Experiment Run End Time").SingleOrDefault()?.ItemArray[1].ToString();
                resultedTime = resultedTime.Substring(0, resultedTime.Length - 4);

                var query_RawData = dtExcelRawData.AsEnumerable().Skip(dataStartRowNumber - 1);
                var barcodesGroup = query_RawData.GroupBy(r => r.ItemArray[barcodeColumnIndex].ToString());
                List<Tuple<string, string, string, int>> finalValueLogics = new List<Tuple<string, string, string, int>>();
                List<Tuple<string, string, int>> finalValueKit = new List<Tuple<string, string, int>>();
                List<Tuple<string, string, string, string, int>> finalValueTGLogics = new List<Tuple<string, string, string, string, int>>();

                DataTable dtCovidColumnsInfo = new DataTable();
                if (kitCode == "Seegene" || kitCode == "Siemens" || kitCode == "Unimedica" || kitCode == "Kogene" || kitCode == "Covipath" || kitCode == "Taqpath" || kitCode == "Genes2Me" || kitCode == "TATA_MD" || kitCode == "3DMED")
                {

                    dtCovidColumnsInfo.Columns.Add("Barcode");
                    dtCovidColumnsInfo.Columns.Add("SGENE");
                    dtCovidColumnsInfo.Columns.Add("IC");
                    dtCovidColumnsInfo.Columns.Add("EGENE");
                    dtCovidColumnsInfo.Columns.Add("NGENE");
                    dtCovidColumnsInfo.Columns.Add("ORF1ab");
                    dtCovidColumnsInfo.Columns.Add("RDRP");
                }

                foreach (var bg in barcodesGroup)
                {
                    string barcode = bg.Key;
                    decimal tempValue = 0;

                    decimal maxValue = 0, minvalue = 100;
                    foreach (var dr in bg)
                    {
                        string testCode = dr[testcodeColumnIndex].ToString();
                        decimal componentValue = 0;
                        decimal.TryParse(dr[testValueColumnIndex].ToString(), out componentValue);
                        dtImportedData.Rows.Add(barcode, testCode, componentValue, "", resultedTime);

                        if (new string[] { "N gene", "S gene", "E gene", "ORF1ab", "RdRP" }.Contains(testCode, StringComparer.InvariantCultureIgnoreCase)
                            && componentValue > maxValue)
                            maxValue = componentValue;

                        if (new string[] { "N1", "N2" }.Contains(testCode, StringComparer.InvariantCultureIgnoreCase)
                         && componentValue < minvalue && componentValue != 0)
                            minvalue = componentValue;

                        if (kitCode == "Genes2Me")
                        {
                            if (new string[] { "N gene", "RdRp gene" }.Contains(testCode, StringComparer.InvariantCultureIgnoreCase)
                       && componentValue < minvalue && componentValue != 0)
                                minvalue = componentValue;

                        }
                        else if (kitCode == "Covipath")
                        {
                            if (new string[] { "N gene", "ORF1ab" }.Contains(testCode, StringComparer.InvariantCultureIgnoreCase)
                      && componentValue < minvalue && componentValue != 0)
                                minvalue = componentValue;
                        }

                        else if (kitCode == "TATA_MD")
                        {
                            if (new string[] { "N gene", "RdRp" }.Contains(testCode, StringComparer.InvariantCultureIgnoreCase)
                      && componentValue < minvalue && componentValue != 0)
                                minvalue = componentValue;
                        }

                        else if (kitCode == "Taqpath")
                        {
                            if (new string[] { "N gene", "ORF1ab", "S gene" }.Contains(testCode, StringComparer.InvariantCultureIgnoreCase)
                   && componentValue < minvalue && componentValue != 0)
                                minvalue = componentValue;
                        }
                        else if (kitCode == "3DMED")
                        {
                            if (new string[] { "N gene", "ORF 1ab", "E GENE", "IC" }.Contains(testCode, StringComparer.InvariantCultureIgnoreCase)
                   && componentValue < minvalue && componentValue != 0)
                                minvalue = componentValue;
                        }
                    }

                    if (kitCode == "N gene" || kitCode == "Meril")
                    {
                        string nGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "N gene").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string orf1abValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "ORF1ab").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string sGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "S gene").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string MS2Value = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "MS2").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();

                        if (kitCode == "N gene")
                        {
                            if (decimal.TryParse(nGeneValue, out tempValue)) nGeneValue = "Any value";
                            if (decimal.TryParse(orf1abValue, out tempValue)) orf1abValue = "Any value";
                            if (decimal.TryParse(sGeneValue, out tempValue)) sGeneValue = "Any value";
                            if (decimal.TryParse(MS2Value, out tempValue)) MS2Value = "Any value";
                        }

                        if (analyzer.AnalyzerId == 2045 || analyzer.AnalyzerId == 2046)
                        {
                            finalValueTGLogics = new List<Tuple<string, string, string, string, int>>
                            {
                            new Tuple<string, string, string,string, int>("Undetermined", "Undetermined", "Undetermined", "Undetermined",(int)DescriptiveIDs.Inconclusive),
                            new Tuple<string, string, string,string, int>("Undetermined", "Undetermined", "Undetermined", "Any value",(int)DescriptiveIDs.NotDetected),
                            new Tuple<string, string, string,string, int>("Any value", "Any value", "Any value", "Any value", (int)DescriptiveIDs.Detected),
                            new Tuple<string, string, string,string, int>("Any value", "Any value", "Any value", "Undetermined", (int)DescriptiveIDs.Detected),
                            new Tuple<string, string, string,string, int>("Any value", "Undetermined", "Undetermined", "Any value", (int)DescriptiveIDs.Inconclusive),
                            new Tuple<string, string, string,string, int>("Any value", "Undetermined", "Undetermined", "Undetermined", (int)DescriptiveIDs.Inconclusive),
                            new Tuple<string, string, string,string, int>("Undetermined", "Any value", "Any value", "Any value", (int)DescriptiveIDs.Detected),
                            new Tuple<string, string, string,string, int>("Undetermined", "Any value", "Any value", "Undetermined", (int)DescriptiveIDs.Detected),
                            new Tuple<string, string, string,string, int>("Any value", "Undetermined", "Any value", "Any value", (int)DescriptiveIDs.Detected),
                            new Tuple<string, string, string,string, int>("Any value", "Undetermined", "Any value", "Undetermined", (int)DescriptiveIDs.Detected),
                            new Tuple<string, string, string,string, int>("Undetermined", "Any value", "Undetermined", "Any value", (int)DescriptiveIDs.Inconclusive),
                            new Tuple<string, string, string,string, int>("Undetermined", "Any value", "Undetermined", "Undetermined", (int)DescriptiveIDs.Inconclusive),
                            new Tuple<string, string, string,string, int>("Undetermined", "Undetermined", "Any value", "Any value", (int)DescriptiveIDs.Inconclusive),
                            new Tuple<string, string, string,string, int>("Undetermined", "Undetermined", "Any value", "Undetermined", (int)DescriptiveIDs.Inconclusive),
                            new Tuple<string, string, string,string, int>("Any value", "Any value", "Undetermined", "Any value", (int)DescriptiveIDs.Detected),
                             };

                            var finalValueTG = finalValueTGLogics.Where(r => nGeneValue.Equals(r.Item1, StringComparison.InvariantCultureIgnoreCase)
                               && orf1abValue.Equals(r.Item2, StringComparison.InvariantCultureIgnoreCase)
                               && sGeneValue.Equals(r.Item3, StringComparison.InvariantCultureIgnoreCase)
                               && MS2Value.Equals(r.Item4, StringComparison.InvariantCultureIgnoreCase)
                               );

                            if (finalValueTG.Any())
                            {
                                testValue = finalValueTG.Single().Item5;
                                //dtImportedData.Rows.Add(barcode, "COV19", testValue, "", resultedTime);
                            }
                        }
                        else
                        {
                            if (kitCode == "N gene")
                            {
                                finalValueLogics = new List<Tuple<string, string, string, int>> {
                                    new Tuple<string, string, string, int>( "Undetermined", "undetermined", "undetermined", (int)DescriptiveIDs.NotDetected),
                                    new Tuple<string, string, string, int>( "Any value", "Any value", "Any value", (int)DescriptiveIDs.Detected),
                                    new Tuple<string, string, string, int>( "Any value", "Any value", "undetermined", (int)DescriptiveIDs.Detected),
                                    new Tuple<string, string, string, int>( "Undetermined", "Any value", "Any value", (int)DescriptiveIDs.Detected),
                                    new Tuple<string, string, string, int>( "Any value", "undetermined", "Any value", (int)DescriptiveIDs.Detected),
                                    new Tuple<string, string, string, int>( "Any value", "undetermined", "undetermined", (int)DescriptiveIDs.Inconclusive),
                                    new Tuple<string, string, string, int>( "Undetermined", "Any value", "undetermined", (int)DescriptiveIDs.Inconclusive),
                                    new Tuple<string, string, string, int>( "Undetermined", "undetermined", "Any value", (int)DescriptiveIDs.Inconclusive),
                                   };

                                var finalValue = finalValueLogics.Where(r => nGeneValue.Equals(r.Item1, StringComparison.InvariantCultureIgnoreCase)
                                            && orf1abValue.Equals(r.Item2, StringComparison.InvariantCultureIgnoreCase)
                                            && sGeneValue.Equals(r.Item3, StringComparison.InvariantCultureIgnoreCase));
                                if (finalValue.Any())
                                {
                                    testValue = finalValue.Single().Item4;
                                }
                            }
                            else if (kitCode == "Meril")
                            {
                                decimal.TryParse(orf1abValue, out maxValue);
                                if (maxValue <= 35)
                                //if (/*orf1abValue == "Any value"*/ maxValue > 0)
                                {
                                    testValue = (int)DescriptiveIDs.Detected;
                                }
                                else
                                    testValue = (int)DescriptiveIDs.NotDetected;
                            }
                        }
                    }
                    else if (kitCode == "N1")
                    {
                        string n1GeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "N1").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string n2GeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "N2").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string RNasePValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "RNaseP").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();

                        n1GeneValue = GetGeneValue(n1GeneValue, 35, 0, TupleCalculate.Low, TupleCalculate.Null, TupleCalculate.Null);
                        n2GeneValue = GetGeneValue(n2GeneValue, 35, 0, TupleCalculate.Low, TupleCalculate.Null, TupleCalculate.Null);
                        if (decimal.TryParse(RNasePValue, out tempValue)) RNasePValue = "undetermined";

                        finalValueLogics = new List<Tuple<string, string, string, int>> {


                            new Tuple<string, string, string, int>( "Lower", "Any value", "undetermined", (int)DescriptiveIDs.Detected),
                            new Tuple<string, string, string, int>( "Lower", "undetermined", "undetermined", (int)DescriptiveIDs.Detected),
                            new Tuple<string, string, string, int>( "Lower", "Lower", "undetermined", (int)DescriptiveIDs.Detected),


                            new Tuple<string, string, string, int>( "Any value", "Lower", "undetermined", (int)DescriptiveIDs.Detected),
                            new Tuple<string, string, string, int>( "Any value", "Any value", "undetermined", (int)DescriptiveIDs.NotDetected),
                            new Tuple<string, string, string, int>( "Any value", "undetermined", "undetermined", (int)DescriptiveIDs.NotDetected),

                            new Tuple<string, string, string, int>( "undetermined", "undetermined", "undetermined", (int)DescriptiveIDs.NotDetected),
                             new Tuple<string, string, string, int>( "undetermined", "Lower", "undetermined", (int)DescriptiveIDs.NotDetected),
                            new Tuple<string, string, string, int>( "undetermined", "Any value", "undetermined", (int)DescriptiveIDs.NotDetected),

                           };
                        var finalValue = finalValueLogics.Where(r => n1GeneValue.Equals(r.Item1, StringComparison.InvariantCultureIgnoreCase)
                                         && n2GeneValue.Equals(r.Item2, StringComparison.InvariantCultureIgnoreCase)
                                         && RNasePValue.Equals(r.Item3, StringComparison.InvariantCultureIgnoreCase));
                        if (finalValue.Any())
                        {
                            testValue = finalValue.Single().Item4;
                        }
                    }
                    else if (kitCode == "Seegene")
                    {

                        string sGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "S GENE").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string eGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "E GENE").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string nGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "N gene").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string icGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "IC").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();

                        dtCovidColumnsInfo.Rows.Add(barcode, sGeneValue, icGeneValue, eGeneValue, nGeneValue, null);

                        testValue = CalcualteGene(dtCovidColumnsInfo);

                    }
                    else if (kitCode == "Unimedica")
                    {
                        //string orf1abValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "ORF1ab").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string orf1abValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "ORF1ab").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string nGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "NGENE").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string icGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "IC").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();

                        //dtCovidColumnsInfo.Rows.Add(barcode, null, icGeneValue, null, nGeneValue, icGeneValue);
                        dtCovidColumnsInfo.Rows.Add(barcode, null, icGeneValue, null, nGeneValue, orf1abValue);

                        testValue = CalcualteGene(dtCovidColumnsInfo);

                    }
                    else if (kitCode == "Siemens")
                    {
                        string orfnGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "ORF/N GENE").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string icGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "IC").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();

                        dtCovidColumnsInfo.Rows.Add(barcode, null, icGeneValue, null, null, orfnGeneValue);

                        testValue = CalcualteGene(dtCovidColumnsInfo);

                    }
                    else if (kitCode == "Kogene")
                    {
                        string eGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "E GENE").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string orfnGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "ORF1ab").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string icGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "IC").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();

                        dtCovidColumnsInfo.Rows.Add(barcode, null, icGeneValue, eGeneValue, null, orfnGeneValue);

                        testValue = CalcualteGene(dtCovidColumnsInfo);

                    }

                    else if (kitCode == "Covipath")
                    {
                        string NGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "N Gene").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string orfnGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "ORF1ab").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string icGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "IC").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();

                        dtCovidColumnsInfo.Rows.Add(barcode, null, icGeneValue, null, NGeneValue, orfnGeneValue);

                        testValue = CalcualteGene(dtCovidColumnsInfo);
                    }
                    else if (kitCode == "Genes2Me")
                    {
                        string NGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "N GENE").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string RdRPValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "RdRp gene").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string icGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "IC").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();

                        dtCovidColumnsInfo.Rows.Add(barcode, null, icGeneValue, null, NGeneValue, null, RdRPValue);

                        testValue = CalcualteGene(dtCovidColumnsInfo);
                    }

                    else if (kitCode == "TATA_MD")
                    {
                        //string eGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "E gene").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string NGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "N gene").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string RdRPValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "RdRp").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string icGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "IC").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();

                        dtCovidColumnsInfo.Rows.Add(barcode, null, icGeneValue, null, NGeneValue, null, RdRPValue);

                        testValue = CalcualteGene(dtCovidColumnsInfo);
                    }

                    else if (kitCode == "Taqpath")
                    {
                        string sGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "S Gene").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string NGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "N Gene").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string orfnGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "ORF1ab").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string icGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "IC").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();

                        dtCovidColumnsInfo.Rows.Add(barcode, sGeneValue, icGeneValue, null, NGeneValue, orfnGeneValue);

                        testValue = CalcualteGene(dtCovidColumnsInfo);
                    }

                    else if (kitCode == "3DMED")
                    {
                        string eGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "E GENE").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string NGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "N gene").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string orfnGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "ORF 1ab").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();
                        string icGeneValue = bg.Where(r => r.ItemArray[testcodeColumnIndex].ToString() == "IC").SingleOrDefault()?.ItemArray[testValueColumnIndex].ToString();

                        dtCovidColumnsInfo.Rows.Add(barcode, null, icGeneValue, eGeneValue, NGeneValue, orfnGeneValue, null);

                        testValue = CalcualteGene(dtCovidColumnsInfo);
                    }

                    //string strTestValue = new MessageProcessor(analyzer.AnalyzerId).GetDescriptiveMapping("COVID-19 PCR").Where(r => r.Id == testValue).SingleOrDefault()?.Description;
                    if (kitCode == "TATA_MD")
                    {
                        string strTestValue = new MessageProcessor(analyzer.AnalyzerId).GetDescriptiveMapping("COVTM").Where(r => r.Id == testValue).SingleOrDefault()?.Description;
                        dtImportedData.Rows.Add(barcode, "COVTM", strTestValue, "", resultedTime);
                    }

                    //string strTestValue = new MessageProcessor(analyzer.AnalyzerId).GetDescriptiveMapping("COVID-19 PCR").Where(r => r.Id == testValue).SingleOrDefault()?.Description;
                    if (kitCode == "Unimedica")
                    {
                        string strTestValue = new MessageProcessor(analyzer.AnalyzerId).GetDescriptiveMapping(Testselected).Where(r => r.Id == testValue).SingleOrDefault()?.Description;
                        dtImportedData.Rows.Add(barcode, "COVID-19", strTestValue, "", resultedTime);
                    }

                    if (kitCode == "3DMED")
                    {
                        string strTestValue = new MessageProcessor(analyzer.AnalyzerId).GetDescriptiveMapping(Testselected).Where(r => r.Id == testValue).SingleOrDefault()?.Description;
                        dtImportedData.Rows.Add(barcode, "COVID-19", strTestValue, "", resultedTime);
                    }

                    else
                    {
                        string strTestValue = new MessageProcessor(analyzer.AnalyzerId).GetDescriptiveMapping("COV19").Where(r => r.Id == testValue).SingleOrDefault()?.Description;
                        dtImportedData.Rows.Add(barcode, "COV19", strTestValue, "", resultedTime);
                    }

                    if (testValue != (int)DescriptiveIDs.NotDetected && analyzer.AnalyzerId != 2045 && analyzer.AnalyzerId != 2046)
                    {
                        if (kitCode == "N gene" || kitCode == "Meril")
                        {
                            dtImportedData.Rows.Add(barcode, "TGCT", maxValue, "", resultedTime);
                        }
                        else
                        {
                            dtImportedData.Rows.Add(barcode, "TGCT", minvalue, "", resultedTime);
                        }
                    }
                }
            }
            else if (dataStartRowNumber > 1)
            {
                for (int i = dataStartRowNumber - 1; i < dtExcelRawData.Rows.Count; i++)
                {
                    dtImportedData.Rows.Add(dtExcelRawData.Rows[i].ItemArray);
                }
            }

            return dtImportedData;
        }

        public int CalcualteGene(DataTable dataTable)
        {
            int VALUE = 0;
            DataTable table = dataTable;
            decimal IC, ORF1ab, NGene, EGene, RdRp, SGene;
            foreach (DataRow row in table.Rows)
            {
                decimal.TryParse(row["IC"].ToString(), out IC);
                decimal.TryParse(row["NGENE"].ToString(), out NGene);
                decimal.TryParse(row["EGENE"].ToString(), out EGene);
                decimal.TryParse(row["SGENE"].ToString(), out SGene);

                if (kitCode == "Seegene")
                {
                    if (IC == 0)
                        VALUE = (int)DescriptiveIDs.Inconclusive;
                    else if (IC != 0 && NGene != 0 && EGene != 0 && IC <= 37 && NGene <= 37 && EGene <= 37)
                        VALUE = (int)DescriptiveIDs.Detected;
                    else
                        VALUE = (int)DescriptiveIDs.NotDetected;
                }
                else if (kitCode == "Siemens")
                {
                    decimal.TryParse(row["ORF1ab"].ToString(), out ORF1ab);

                    if (IC < 33 && ORF1ab == 0)
                        VALUE = (int)DescriptiveIDs.NotDetected;

                    else if (IC == 0 && ORF1ab == 0)
                        VALUE = (int)DescriptiveIDs.Inconclusive;

                    else if (IC < 33 && ORF1ab < 33)
                        VALUE = (int)DescriptiveIDs.Detected;

                    else if (ORF1ab >= 33 && ORF1ab <= 37)
                        VALUE = (int)DescriptiveIDs.Review_result;

                    else if (ORF1ab > 37)
                        VALUE = (int)DescriptiveIDs.NotDetected;
                }
                else if (kitCode == "Unimedica")
                {
                    decimal.TryParse(row["ORF1ab"].ToString(), out ORF1ab);
                    decimal.TryParse(row["NGENE"].ToString(), out NGene);
                    if (IC == 0)
                        VALUE = (int)DescriptiveIDs.Inconclusive;

                    if (ORF1ab != 0 && NGene != 0 && ORF1ab <= 37 && NGene <= 37)
                        VALUE = (int)DescriptiveIDs.Detected;
                    else if (ORF1ab < 37 && NGene < 37 || ORF1ab == 0 && NGene == 0 || ORF1ab == 0 && NGene != 0 || ORF1ab != 0 && NGene == 0)
                        VALUE = (int)DescriptiveIDs.NotDetected;

                }
                else if (kitCode == "Kogene")
                {
                    decimal.TryParse(row["ORF1ab"].ToString(), out ORF1ab);
                    decimal.TryParse(row["EGENE"].ToString(), out EGene);

                    if (IC == 0)
                        VALUE = (int)DescriptiveIDs.Inconclusive;
                    else if (ORF1ab != 0 && EGene != 0 && ORF1ab <= 37 && EGene <= 37)
                        VALUE = (int)DescriptiveIDs.Detected;
                    else
                        VALUE = (int)DescriptiveIDs.NotDetected;
                }

                else if (kitCode == "Covipath")
                {
                    decimal.TryParse(row["ORF1ab"].ToString(), out ORF1ab);
                    decimal.TryParse(row["NGENE"].ToString(), out NGene);
                    if (ORF1ab == 0 && NGene == 0)
                        VALUE = (int)DescriptiveIDs.NotDetected;
                    else if (ORF1ab != 0 && NGene != 0 && ORF1ab <= 35 && NGene <= 35)
                        VALUE = (int)DescriptiveIDs.Detected;
                    else if (ORF1ab < 35 && NGene < 35)
                        VALUE = (int)DescriptiveIDs.Inconclusive;
                    else
                        VALUE = (int)DescriptiveIDs.NotDetected;


                }
                else if (kitCode == "Genes2Me")
                {
                    decimal.TryParse(row["RDRP"].ToString(), out RdRp);
                    decimal.TryParse(row["NGENE"].ToString(), out NGene);

                    if (RdRp == 0 && NGene == 0)
                        VALUE = (int)DescriptiveIDs.NotDetected;
                    else if (RdRp != 0 && NGene != 0 && RdRp <= 35 && NGene <= 35)
                        VALUE = (int)DescriptiveIDs.Detected;
                    else if (RdRp < 35 || NGene < 35)
                        VALUE = (int)DescriptiveIDs.Inconclusive;
                    else
                        VALUE = (int)DescriptiveIDs.NotDetected;
                }

                else if (kitCode == "TATA_MD")
                {
                    decimal.TryParse(row["RDRP"].ToString(), out RdRp);
                    decimal.TryParse(row["NGENE"].ToString(), out NGene);
                    //decimal.TryParse(row["EGENE"].ToString(), out EGene);

                    if (RdRp == 0 && NGene == 0 && EGene == 0)
                        VALUE = (int)DescriptiveIDs.NotDetected;

                    else if (RdRp != 0 && NGene != 0 /*&& EGene != 0*/ && RdRp <= 35 && NGene <= 35 /*&& EGene <= 35*/)
                        VALUE = (int)DescriptiveIDs.Detected;

                    else if (RdRp < 35 && NGene == 0)
                        VALUE = (int)DescriptiveIDs.Detected;

                    else if (NGene < 35 && RdRp == 0)
                        VALUE = (int)DescriptiveIDs.Detected;

                    //else if (RdRp > 35 || NGene > 35)
                    //    VALUE = (int)DescriptiveIDs.NotDetected;

                    else if (RdRp > 35 && NGene < 35)
                        VALUE = (int)DescriptiveIDs.Detected;

                    else if (NGene > 35 && RdRp < 35)
                        VALUE = (int)DescriptiveIDs.Detected;

                    else
                        VALUE = (int)DescriptiveIDs.NotDetected;

                }

                else if (kitCode == "Taqpath")
                {
                    decimal.TryParse(row["NGENE"].ToString(), out NGene);
                    decimal.TryParse(row["EGENE"].ToString(), out EGene);
                    decimal.TryParse(row["SGENE"].ToString(), out SGene);

                    if (SGene == 0 && NGene == 0 && EGene == 0)
                        VALUE = (int)DescriptiveIDs.NotDetected;
                    else if (SGene != 0 && NGene != 0 && EGene != 0 && SGene <= 35 && NGene <= 35 && EGene <= 35)
                        VALUE = (int)DescriptiveIDs.Detected;
                    else if (SGene < 35 || NGene < 35 || EGene < 35)
                        VALUE = (int)DescriptiveIDs.Inconclusive;
                    else
                        VALUE = (int)DescriptiveIDs.NotDetected;

                }

                if (kitCode == "3DMED")
                { //3109E
                    decimal.TryParse(row["IC"].ToString(), out IC);
                    decimal.TryParse(row["NGENE"].ToString(), out NGene);
                    decimal.TryParse(row["EGENE"].ToString(), out EGene);
                    decimal.TryParse(row["ORF1ab"].ToString(), out ORF1ab);

                    if ((ORF1ab != 0 && ORF1ab <= 40) || (NGene != 0 && NGene <= 40) /*|| (EGene != 0 && EGene <= 40) */)
                        VALUE = (int)DescriptiveIDs.Detected;
                    else if ((ORF1ab == 0 || ORF1ab > 40) && (NGene == 0 || NGene > 40) && (EGene != 0 && EGene <= 40))
                        VALUE = (int)DescriptiveIDs.NotDetected;
                    else if ((ORF1ab == 0 || ORF1ab > 40) && (NGene == 0 || NGene > 40) && (EGene == 0 || EGene > 40) && (IC != 0 || IC <= 40))
                        VALUE = (int)DescriptiveIDs.NotDetected;
                    else
                        VALUE = (int)DescriptiveIDs.Inconclusive;

                }

            }
            return VALUE;
        }

        public string GetGeneValue(string value, int comparevalue1, int comparevalue2, TupleCalculate low, TupleCalculate between, TupleCalculate high)
        {
            decimal tempValue;
            string geneVal = "";
            decimal.TryParse(value, out tempValue);


            if (low == TupleCalculate.Null && between == TupleCalculate.Null && high == TupleCalculate.Null) geneVal = "Any value";
            else if (tempValue == 0) geneVal = "undetermined";
            else if (between == TupleCalculate.Between && tempValue >= comparevalue2 && tempValue <= comparevalue1) geneVal = "Between";
            else if (low == TupleCalculate.Low && tempValue < comparevalue1) geneVal = "Low";
            else if (low == TupleCalculate.LowThan && tempValue <= comparevalue1) geneVal = "Lowthan";
            else if (high == TupleCalculate.High && tempValue > comparevalue1) geneVal = "High";

            else geneVal = "Any value";
            return geneVal;
        }


        public enum TupleCalculate
        {
            Low = 1,
            LowThan = 2,
            High = 3,
            HighThan = 4,
            Between = 5,
            Null = 0
        }
        //public string GetGeneValue(string value)
        //{
        //    decimal tempValue;
        //    string geneVal = "";
        //    decimal.TryParse(value, out tempValue);

        //    if (tempValue == 0) geneVal = "undetermined";
        //    else if (tempValue < 35) geneVal = "Lower";
        //    else geneVal = "Any value";

        //    return geneVal;
        //}

        private DataTable ImportXml(string FileName, string extention, bool hasHeaders)
        {
            DataSet ds = new DataSet();
            using (XmlReader xmlFile = XmlReader.Create(FileName, new XmlReaderSettings()))
            {
                ds.ReadXml(xmlFile);
            }
            DataTable dtXmlRawData = new DataTable();

            for (int i = 0; i < ds.Tables["Cell"].AsEnumerable().GroupBy(r => r.Field<int>("Row_Id")).Max(r => r.Count()); i++)
            {
                dtXmlRawData.Columns.Add(i.ToString());
            }

            //int columnCount = ds.Tables["cell"].AsEnumerable().Max(r => Convert.ToInt32(r.Field<string>("Index")));
            for (int a = 0; a < ds.Tables["Row"].Rows.Count; a++)
            {
                DataRow drRow = ds.Tables["Row"].Rows[a];
                var queryRow = ds.Tables["Cell"].AsEnumerable().Where(r => /*r.Field<string>("Index") != null &&*/ r.Field<int>("Row_Id") == (int)drRow["Row_Id"]);
                DataRow drNew = dtXmlRawData.Rows.Add();
                int colNumber = 1;
                foreach (var cell in queryRow)
                {
                    var cellValue = ds.Tables["Data"].AsEnumerable().Where(r => r.Field<int>("Cell_Id") == (int)cell["Cell_Id"]).Select(r => r.Field<object>("Data_Text"));
                    //drNew[Convert.ToInt32(cell["Index"]) - 1] = cellValue.Single();
                    if (cell["Index"] != DBNull.Value)
                    {
                        colNumber = Convert.ToInt32(cell["Index"]);
                    }
                    drNew[colNumber - 1] = cellValue.Single();
                    colNumber++;
                }
            }

            dtXmlRawData.Rows[headerRowNumber - 1][0] = "SampleId";
            dtXmlRawData.Rows[headerRowNumber - 1][1] = "SampleLine";

            DataTable dtImportedData = new DataTable();
            if (dtXmlRawData.Rows.Count < headerRowNumber)
            {
                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "The file uploaded does not have minimum number of rows required. Please check if you have uploaded correct file.");
                return null;
            }


            if (msgConfig.AnalyzerTypeID == analyzerTypeId_ICPThermo)
            {
                DataRow drHeader = dtXmlRawData.Rows[headerRowNumber - 1];
                for (int i = unpivotHeaderColumns; i < dtXmlRawData.Columns.Count; i++)
                    drHeader[i] = drHeader[i].ToString().Split(' ')[0];
            }

            //TODO: remove following line
            //copy testcode columns
            dtXmlRawData.Rows[headerRowNumber - 1].ItemArray.Skip(unpivotHeaderColumns).ToList().ForEach(r => dtColumnsInfo.Rows.Add(r, "Decimal"));
            //compare uploaded column names with predefined names
            for (int i = 0; i < dtColumnsInfo.Rows.Count; i++)
            {
                string strColumnInfo = dtColumnsInfo.Rows[i]["ColumnName"].ToString();
                dtImportedData.Columns.Add(strColumnInfo);
                string colName = dtXmlRawData.Rows[headerRowNumber - 1][i].ToString();
                if (!Regex.IsMatch(strColumnInfo, @"Col\d+") && colName != strColumnInfo)
                {
                    UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "Column \"" + strColumnInfo + "\" is missing or does not match defined position. Please upload data in predefined format.");
                    return null;
                }
            }

            for (int i = dataStartRowNumber - 1; i < dtXmlRawData.Rows.Count; i++)
            {
                dtImportedData.Rows.Add(dtXmlRawData.Rows[i].ItemArray);
            }

            return dtImportedData;
        }

        private void btnControlCharacter_Click(object sender, EventArgs e)
        {
            string inputText = "[" + (sender as Button).Text + "]";
            if (inputText != txtCommunicationInput.Text)
            {
                txtCommunicationInput.Text = inputText;
            }
            else
            {
                txtCommunicationInput_TextChanged(null, null);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.IsDebugMode)
                return;

            if (e.CloseReason != CloseReason.TaskManagerClosing)
            {
                try
                {
                    //MessageBox.Show("Not allowed to close LIS", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //e.Cancel = true;
                    //return;

                    if (Program.BlockClosingApp_Reason.Length > 0)
                    {
                        MessageBox.Show("Kindly wait for some time to avoid issues. " + Environment.NewLine + Program.BlockClosingApp_Reason, "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                        return;
                    }

                    //if (_communication != null && _communication.IsOpen)
                    //{
                    //    btnClosePort_Click(null, null);
                    //}
                }
                catch (Exception ex)
                {
                    errorLog.err_insert(ex);
                }
            }
        }

        private IEnumerable<string> GetExcelSheetNames(OleDbConnection conn)
        {
            DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            return schemaTable.AsEnumerable().Select(r1 => r1.Field<string>("TABLE_NAME").ToString()).Select(r2 => r2.StartsWith("'") ? r2.Substring(1, r2.Length - 2) : r2.Substring(0, r2.Length - 1)).ToArray();
        }

        private DataTable ImportCsv(string fileName)
        {
            DataTable dtImportedData = new DataTable();
            DataTable dtImportedData_Testcode = new DataTable();
            using (StreamReader sr = new StreamReader(fileName))
            {
                int cntRow = 0;
                string strRow = String.Empty;
                string testCode = "";
                int columnIndex_Barcode = -1, columnIndex_TestValue = -1, columnIndex_ResultedTime = -1;
                string resultedDateTime = "";

                if (analyzer.AnalyzerTypeId == analyzerTypeId_AgilentIcpms)
                {
                    columnIndex_Barcode = 6;
                    columnIndex_ResultedTime = 3;
                }

                if (isInputUnpivotFormat)
                {
                    for (int i = 0; i < dtColumnsInfo.Rows.Count; i++)
                    {
                        string strColumnInfo = dtColumnsInfo.Rows[i]["ColumnName"].ToString();
                        dtImportedData.Columns.Add(strColumnInfo);
                    }
                }

                while (!sr.EndOfStream)
                {
                    cntRow++;
                    strRow = sr.ReadLine();

                    if (strRow == "")
                        continue;

                    if (Path.GetExtension(fileName) == ".txt")
                    {
                        //tab separated file
                        var fields = strRow.Split('\t');
                        if (fields.Count() == 0 || fields.ElementAt(0) == "ID#")
                            continue;

                        if (analyzer.AnalyzerTypeId == analyzerTypeId_LCMS)
                        {
                            if (fields.ElementAt(0) == "Name")
                            {
                                testCode = fields.ElementAt(1);
                            }
                            else
                            {
                                if (cntRow == headerRowNumber)
                                {
                                    List<string> lstHeader = fields.ToList();
                                    columnIndex_Barcode = lstHeader.IndexOf("Sample ID");
                                    columnIndex_TestValue = lstHeader.IndexOf("Conc.");
                                    columnIndex_ResultedTime = lstHeader.IndexOf("Date Acquired");
                                }
                                else if (fields.ElementAt(0) == "")
                                {
                                    //header row for LCMS first column name is blank
                                }
                                else
                                {
                                    string barcode = fields.ElementAt(columnIndex_Barcode);
                                    DataRow drBarcode = null;
                                    drBarcode = dtImportedData.NewRow();
                                    drBarcode["BARCODE"] = barcode;
                                    drBarcode["TESTCODE"] = testCode;
                                    drBarcode["TESTVALUE"] = fields.ElementAt(columnIndex_TestValue);
                                    drBarcode["RESULTED_TIME"] = columnIndex_ResultedTime == -1 ? DateTime.Now.ToString(dateFormat + " " + timeFormat) : fields.ElementAt(columnIndex_ResultedTime);
                                    dtImportedData.Rows.Add(drBarcode);
                                }
                            }
                        }
                        else if (analyzer.AnalyzerTypeId == analyzerTypeId_BiotekReader)
                        {
                            if (cntRow >= dataStartRowNumber)
                            {
                                columnIndex_Barcode = 1;
                                testCode = machineTestCode;
                                columnIndex_TestValue = 3;
                                string barcode = fields.ElementAt(columnIndex_Barcode);
                                DataRow drBarcode = null;
                                drBarcode = dtImportedData.NewRow();
                                drBarcode["BARCODE"] = barcode;
                                drBarcode["TESTCODE"] = testCode;
                                drBarcode["TESTVALUE"] = fields.ElementAt(columnIndex_TestValue);
                                drBarcode["RESULTED_TIME"] = columnIndex_ResultedTime == -1 ? DateTime.Now.ToString(dateFormat + " " + timeFormat) : fields.ElementAt(columnIndex_ResultedTime);
                                dtImportedData.Rows.Add(drBarcode);
                            }
                        }
                    }
                    else
                    {
                        //comma separated file
                        Regex csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        var fields = csvParser.Split(strRow).Select(r => r.StartsWith("\"") ? r.Substring(1, r.Length - 2).Replace("\"\"", "\"") : r); //r.TrimStart(' ', '"').TrimEnd('"')

                        if (analyzer.AnalyzerTypeId == analyzerTypeId_RotorGene)
                        {
                            if (cntRow == 4)
                                resultedDateTime = fields.ElementAt(1);
                            else if (cntRow == 5)
                                resultedDateTime += " " + fields.ElementAt(1);

                            if (fields.Count() > 0 && fields.ElementAt(0) == "No.")
                            {
                                //headerRowNumber = cntRow;
                                dataStartRowNumber = cntRow + 1;
                            }
                        }
                        if (analyzer.AnalyzerTypeId == analyzerTypeId_BDFacts)
                        {

                            if (cntRow == 1)
                            {
                                unpivotHeaderColumns = 121;

                                DataColumn Col = dtImportedData_Testcode.Columns.Add("Test_Code", System.Type.GetType("System.String"));

                                for (int i = 115; i <= unpivotHeaderColumns; i++)
                                {
                                    dtImportedData_Testcode.Rows.Add(fields.ElementAt(i));
                                }
                            }
                        }
                        if (cntRow == (headerRowNumber) && analyzer.AnalyzerTypeId != analyzerTypeId_BDFacts)
                        {
                            //TODO: remove following line
                            //copy testcode columns
                            fields.Skip(unpivotHeaderColumns).ToList().ForEach(r => dtColumnsInfo.Rows.Add(r.Trim(), "Decimal"));

                            for (int i = 0; i < dtColumnsInfo.Rows.Count; i++)
                            {
                                string strColumnInfo = dtColumnsInfo.Rows[i]["ColumnName"].ToString();
                                dtImportedData.Columns.Add(strColumnInfo);
                                //following logic commented for Agilent ICPMS
                                //if (!Regex.IsMatch(strColumnInfo, @"Col\d+") && fields.ElementAt(i) != strColumnInfo)
                                //{
                                //    UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "Column \"" + strColumnInfo + "\" is missing or does not match defined position. Please upload data in predefined format.");
                                //    return null;
                                //}
                            }
                        }
                        else if (dataStartRowNumber > -1 && cntRow >= dataStartRowNumber)
                        {
                            if (analyzer.AnalyzerTypeId == analyzerTypeId_AgilentIcpms)
                            {
                                dtImportedData.Rows.Add(new object[] { fields.ElementAt(columnIndex_Barcode), fields.ElementAt(columnIndex_ResultedTime) }.Concat(fields.Skip(unpivotHeaderColumns)).ToArray());
                            }
                            else
                            {
                                dtImportedData.Rows.Add(fields.ToArray());
                            }
                        }
                        if (analyzer.AnalyzerTypeId == analyzerTypeId_RotorGene)
                        {
                            double initialVolume = 10;
                            string barcode = fields.ElementAt(2);
                            double testValue = 0;
                            string prefix = "";
                            string testRemarks = "";

                            testCode = machineTestCode;

                            if (new string[] { "COV19", "COVBB" }.Contains(testCode, StringComparer.InvariantCultureIgnoreCase))
                            {
                                if (String.IsNullOrEmpty(fields.ElementAt(4)))
                                {
                                    testValue = (int)DescriptiveIDs.Negative;
                                }
                                else
                                {
                                    testValue = (int)DescriptiveIDs.Positive;
                                }
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(fields.ElementAt(7)))
                                {
                                    UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Warning, "No result value for barcode " + barcode);
                                    continue;
                                }

                                double calcConc = double.Parse(fields.ElementAt(7).Replace(",", ""));
                                double testValue_Common = (calcConc * 90) / initialVolume;
                                double conversionFactor = 1.5;

                                testValue = testValue_Common;
                                if (testCode == "HIVQL" || testCode == "HIVQN")
                                    testValue *= conversionFactor;

                                int testcodeId = CachedData.TestList.Where(r => testCode.Equals(r._testCode, StringComparison.InvariantCultureIgnoreCase)).Single()._testId;
                                var valueLimits = CachedData.ValueLimits.Where(r => r.TestcodeId == testcodeId).Single();
                                if (new string[] { "HBVQL", "HCVQL", "HIVQL" }.Contains(testCode))
                                {
                                    if (testValue < valueLimits.LowerLimit)
                                    {
                                        testValue = (int)DescriptiveIDs.Negative; //will be treated as DescriptiveId - NOT DETECTED
                                    }
                                    else if (testValue > valueLimits.HigherLimit)
                                    {
                                        testValue = (int)DescriptiveIDs.Positive; //will be treated as DescriptiveId - DETECTED
                                    }
                                }
                                else if (new string[] { "HBVQN", "HCVQN", "HIVQN" }.Contains(testCode))
                                {
                                    if (testValue == 0)
                                    {
                                        testValue = 0.01;
                                        prefix = "";
                                    }
                                    if (testValue < valueLimits.LowerLimit)
                                    {
                                        prefix = "<";
                                        testValue = valueLimits.LowerLimit;
                                    }
                                    else if (testValue > valueLimits.HigherLimit)
                                    {
                                        prefix = "";
                                        testValue = 0.02;
                                    }
                                    else
                                    {
                                        testRemarks = Math.Log10(testValue).ToString("0.00");
                                    }
                                }
                            }
                            dtImportedData.Rows.Add(testCode, barcode, testValue, resultedDateTime, prefix);
                        }
                        else if (analyzer.AnalyzerTypeId == analyzerTypeId_BDFacts)
                        {

                            string barcode = fields.ElementAt(7);
                            resultedDateTime = fields.ElementAt(11);
                            //CD3, CD4, CD8
                            bool IsHb27 = fields.ElementAt(152).ToString().TrimEnd('.') == "" ? false : true;
                            if (!IsHb27)
                            {

                                //dtImportedData_Testcode.Rows.Add("RCD48");
                                int m = 0;
                                bool flag = false;
                                for (int i = 115; i < unpivotHeaderColumns; i++)
                                {
                                    string mac = dtImportedData_Testcode.Rows[m]["Test_Code"].ToString();
                                    string _testCode = CachedData.TestList.Where(r => mac.Equals(r._machineCode, StringComparison.InvariantCultureIgnoreCase)).Single()._machineCode;
                                    decimal testvalue = 0;
                                    Decimal.TryParse(fields.ElementAt(i).ToString(), out testvalue);
                                    flag = testvalue == 0 ? true : false;
                                    if (_testCode.Contains("Abs Cnt"))
                                    {
                                        testvalue = Math.Round(testvalue);
                                    }
                                    m++;
                                    if (flag == false)
                                    {
                                        dtImportedData.Rows.Add(barcode, _testCode, testvalue, "", resultedDateTime, "");
                                    }

                                }

                                decimal RCD48 = 0;

                                Decimal.TryParse(fields.ElementAt(124).ToString(), out RCD48);
                                if (flag == false)
                                {
                                    dtImportedData.Rows.Add(barcode, "CD3/CD8/CD45/CD4 4/8 Ratio", RCD48, "", resultedDateTime, "");
                                }

                            }
                            else
                            {
                                decimal testValue = 0;
                                Decimal.TryParse(fields.ElementAt(152).ToString(), out testValue);
                                IsHb27 = true;
                                if (testValue >= 50.0M && testValue <= 60.0M)
                                    testValue = 102; //BORDERLINE POSITIVE
                                else if (testValue < 50.0M)
                                    testValue = 4; //NEGATIVE
                                else if (testValue > 60.0M)
                                    testValue = 7; //POSITIVE

                                dtImportedData.Rows.Add(barcode, "HB27", testValue, resultedDateTime, "");
                            }
                        }

                    }
                }

                if (dtImportedData.Columns.Count == 0)
                {
                    UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "Header row not found at predefined position. Please check if you have uploaded correct file.");
                }
            }
            return dtImportedData;
        }

        private void PopulateSettings()
        {
            dtColumnsInfo = new DataTable();
            dtColumnsInfo.Columns.Add("ColumnName");
            dtColumnsInfo.Columns.Add("DataType");
            dtColumnsInfo.Columns.Add("TargetColumnName");
            dtColumnsInfo.Columns.Add("TargetDataType");

            systemShortTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

            switch (analyzer.AnalyzerTypeId)
            {
                case analyzerTypeId_LCMS:
                    allowedExtensions = new string[] { ".xls", ".xlsx", ".xlsm", ".txt" };
                    if (extension != "")
                    {
                        if (extension == ".txt")
                        {
                            dateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                            timeFormat = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
                            headerRowNumber = 3;
                            dataStartRowNumber = 4;
                            unpivotHeaderColumns = -1;
                            isInputUnpivotFormat = true;

                            #region  Fill Columns Info
                            dtColumnsInfo.Rows.Add("TESTCODE", "Text");
                            dtColumnsInfo.Rows.Add("BARCODE", "Text");
                            dtColumnsInfo.Rows.Add("TESTVALUE", "Decimal");
                            dtColumnsInfo.Rows.Add("RESULTED_TIME", "DateTime");
                            barcodesToIgnore = new string[] { "", "LEVEL1", "LEVEL2", "LEVEL3", "LEVEL4", "LEVEL5", "LEVEL6", "1VITD1", "1VITD2", "1VITD3", "1VITD4", "1VITD5", "1VITD6" };
                            #endregion
                        }
                        else
                        {
                            excelSheetName = "Concentration";
                            dateFormat = "dd/MM/yyyy";
                            timeFormat = "hh:mm tt";
                            headerRowNumber = 5;
                            dataStartRowNumber = 10;
                            unpivotHeaderColumns = 4;

                            #region  Fill Columns Info
                            dtColumnsInfo.Rows.Add("Col1", "Text");
                            dtColumnsInfo.Rows.Add("Col2", "Text");
                            dtColumnsInfo.Rows.Add("Date Acquired", "DateTime", "RESULTED_TIME", "DateTime");
                            dtColumnsInfo.Rows.Add("Tray:Vial", "Text", "BARCODE");
                            #endregion
                        }
                    }
                    break;
                case analyzerTypeId_ICPThermo:
                    allowedExtensions = new string[] { ".xml" };
                    excelSheetName = "";
                    dateFormat = "dd/MM/yyyy";
                    timeFormat = "hh:mm tt";
                    headerRowNumber = 3;
                    dataStartRowNumber = 5;
                    unpivotHeaderColumns = 2;
                    barcodesToIgnore = new string[] { "WASH", "BLANK", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "STD1", "STD2", "STD3", "STD4", "STD5", "STD6", "STD7", "STD8", "STD9", "CCV" };

                    #region  Fill Columns Info
                    dtColumnsInfo.Rows.Add("SampleId", "Text");
                    dtColumnsInfo.Rows.Add("SampleLine", "Text", "BARCODE");
                    #endregion

                    break;
                case analyzerTypeId_BiotekReader: //Biotek Reader
                    allowedExtensions = new string[] { ".txt" };
                    dateFormat = "dd/MM/yyyy";
                    timeFormat = "hh:mm tt";
                    headerRowNumber = -1;
                    dataStartRowNumber = 4;
                    unpivotHeaderColumns = -1;
                    isInputUnpivotFormat = true;
                    decimalPlacesToRound = 3;

                    #region  Fill Columns Info
                    dtColumnsInfo.Rows.Add("TESTCODE", "Text");
                    dtColumnsInfo.Rows.Add("BARCODE", "Text");
                    dtColumnsInfo.Rows.Add("TESTVALUE", "Decimal");
                    dtColumnsInfo.Rows.Add("RESULTED_TIME", "DateTime");
                    #endregion
                    barcodesToIgnore = new string[] { };
                    break;

                case analyzerTypeId_Navios:
                    allowedExtensions = new string[] { ".xls" };
                    excelSheetName = "Sheet1";
                    dateFormat = "M/d/yyyy";
                    timeFormat = "h:mm tt";
                    headerRowNumber = -1;
                    dataStartRowNumber = 23;
                    unpivotHeaderColumns = -1;
                    isInputUnpivotFormat = true;

                    #region  Fill Columns Info
                    dtColumnsInfo.Rows.Add("TESTCODE", "Text");
                    dtColumnsInfo.Rows.Add("BARCODE", "Text");
                    dtColumnsInfo.Rows.Add("TESTVALUE", "Decimal");
                    dtColumnsInfo.Rows.Add("RESULTED_TIME", "DateTime");
                    dtColumnsInfo.Rows.Add("UNIT", "Text");
                    #endregion
                    barcodesToIgnore = new string[] { };
                    break;

                case analyzerTypeId_RotorGene:
                    allowedExtensions = new string[] { ".csv" };
                    dateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                    timeFormat = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
                    headerRowNumber = -1;
                    dataStartRowNumber = -1;
                    unpivotHeaderColumns = -1;
                    isInputUnpivotFormat = true;

                    #region  Fill Columns Info
                    dtColumnsInfo.Rows.Add("TESTCODE", "Text");
                    dtColumnsInfo.Rows.Add("BARCODE", "Text");
                    dtColumnsInfo.Rows.Add("TESTVALUE", "Decimal");
                    dtColumnsInfo.Rows.Add("RESULTED_TIME", "DateTime");
                    dtColumnsInfo.Rows.Add("PREFIX", "Text");
                    #endregion
                    barcodesToIgnore = new string[] { };
                    break;

                case analyzerTypeId_KingfisherPCR:
                    allowedExtensions = new string[] { ".xls" };
                    dateFormat = "yyyy-MM-dd"; // CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                    timeFormat = "HH:mm:ss tt"; // CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
                    headerRowNumber = -1;
                    dataStartRowNumber = -1;
                    unpivotHeaderColumns = -1;
                    isInputUnpivotFormat = true;
                    excelSheetName = "Results";

                    #region  Fill Columns Info
                    dtColumnsInfo.Rows.Add("Sample Name", "Text", "BARCODE");
                    dtColumnsInfo.Rows.Add("TESTCODE", "Text");
                    dtColumnsInfo.Rows.Add("TESTVALUE", "Text");
                    dtColumnsInfo.Rows.Add("PREFIX", "Text");
                    dtColumnsInfo.Rows.Add("RESULTED_TIME", "DateTime");
                    #endregion
                    break;

                case analyzerTypeId_AgilentIcpms:
                    allowedExtensions = new string[] { ".csv" };
                    excelSheetName = "";
                    dateFormat = "M/d/yyyy";
                    timeFormat = "h:mm:ss tt";
                    headerRowNumber = 1;
                    dataStartRowNumber = 3;
                    unpivotHeaderColumns = 7;
                    isInputUnpivotFormat = false;
                    barcodesToIgnore = new string[] { };

                    #region  Fill Columns Info
                    dtColumnsInfo.Rows.Add("BARCODE", "Text");
                    dtColumnsInfo.Rows.Add("RESULTED_TIME", "DateTime");
                    #endregion
                    break;

                case analyzerTypeId_QuantStudioManual:
                    allowedExtensions = new string[] { ".xlsx" };
                    headerRowNumber = 1;
                    dataStartRowNumber = 2;
                    isInputUnpivotFormat = true;
                    excelSheetName = "Results";

                    #region  Fill Columns Info
                    dtColumnsInfo.Rows.Add("A1", "Text");
                    dtColumnsInfo.Rows.Add("A2", "Text");
                    dtColumnsInfo.Rows.Add("BARCODE", "Text");
                    dtColumnsInfo.Rows.Add("A3", "Text");
                    dtColumnsInfo.Rows.Add("Test Code", "Text", "TESTCODE");
                    dtColumnsInfo.Rows.Add("Result", "Text", "TESTVALUE");
                    #endregion
                    break;

                case analyzerTypeId_BDFacts:

                    allowedExtensions = new string[] { ".csv" };
                    excelSheetName = "";
                    dateFormat = "M/d/yyyy";
                    timeFormat = "h:mm:ss tt";
                    headerRowNumber = 1;
                    dataStartRowNumber = 2;
                    //unpivotHeaderColumns = 7;
                    isInputUnpivotFormat = true;

                    #region  Fill Columns Info
                    dtColumnsInfo.Rows.Add("BARCODE", "Text");
                    dtColumnsInfo.Rows.Add("TESTCODE", "Text");
                    dtColumnsInfo.Rows.Add("TESTVALUE", "Text");
                    dtColumnsInfo.Rows.Add("PREFIX", "Text");
                    dtColumnsInfo.Rows.Add("RESULTED_TIME", "DateTime");
                    dtColumnsInfo.Rows.Add("AdditionalInfo", "Text");
                    #endregion
                    break;
                default:
                    allowedExtensions = new string[] { "" };
                    break;
            }
            //new string[] { ".xls", ".xlsx", ".xlsm", ".csv", ".mdb", ".xml" }; //, ".xml"
        }

        private void txtTCP_HostIpAddress_TypeValidationCompleted(object sender, TypeValidationEventArgs e)
        {
            if (InterfaceHelper.ParseValidIpAddress(txtTCP_HostIpAddress.Text) == null)
            {
                MessageBox.Show("Invalid IP address.");
            }
        }

        private void txtTCP_HostIpAddress_Leave(object sender, EventArgs e)
        {
            if (InterfaceHelper.ParseValidIpAddress(txtTCP_HostIpAddress.Text) == null)
            {
                MessageBox.Show("Invalid IP address.");
            }
        }

        private void cmbTestSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTestSelection.SelectedIndex == -1)
                return;

            machineTestCode = cmbTestSelection.SelectedItem.ToString();

            AttemptUploadFile();
        }

        private void PerformBulkWorklistOperations()
        {
            while (true)
            {
                if (!connectionSettings_WO.IsFileBased && _communication_BulkWO?.ConnectionStatus != ConnectionStatus.Connected)
                {
                    MessageLogger.WriteTimeDiffLog("BulkWorklist", null, "Sleeping...");
                    Thread.Sleep(1000);
                    continue;
                }
                else
                {
                    break;
                }
            }

            MessageProcessor messageProcessor = new MessageProcessor(analyzer.AnalyzerId);

            //AttemptPendingWO();

            //0: Single barcode WO, 1: Batch WO, 2: Cancellation
            DateTime dtmStart = DateTime.Now;
            MessageLogger.WriteTimeDiffLog("BulkWorklist", dtmStart, "Loop started");
            foreach (int action in new int[] { /*0,*/ 1, 2 })
            {
                dtmStart = DateTime.Now;
                Program.BlockClosingApp_Reason = "Worklist is being fetched from server.";
                worklist wList = messageProcessor.Machinerequest(new List<string> { "ALL" }, action);
                int barcodeCount = wList == null ? 0 : wList.barcodeList.Count;
                //worklist wList = messageProcessor.GetWorklist(System.IO.File.ReadAllLines(Path.Combine(Application.StartupPath, "barcode.txt")).ToList(), action);
                MessageLogger.WriteTimeDiffLog("BulkWorklist", dtmStart, "Fetched " + barcodeCount + " barcodes WO.");
                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Fetched " + barcodeCount + " barcodes WO.");

                ////for delayed samples Centralink, add extra worklist with Repeat flag
                //var delayedSamples = wList.barcodeList.Where(r => r.BVT.Value > DateTime.Now.Subtract(new TimeSpan(0, 10, 0)));
                //if (analyzer.AnalyzerTypeId == AnalyzerTypes.Aptio && delayedSamples.Any())
                //{
                //    dsfsdd
                //    UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Added repeat flag for " + barcodeCount + " barcodes WO.");
                //}

                bool isCancellation = (action == 2 ? true : false);
                List<string> records = null;
                if (msgConfig.IsSeparateFrameSessionForEachBarcode)
                {
                    foreach (BarcodeList bList in wList.barcodeList)
                    {
                        worklist wl = new worklist();
                        wl.barcodeList = new List<BarcodeList>();
                        wl.barcodeList.Add(bList);

                        records = new MessageProcessor(analyzer.AnalyzerId).PrepareWorklistMessages(new List<string> { bList.Barcode }, wl?.barcodeList, isCancellation);
                        //Thread.Sleep(200);//temporarily added wait for EOT proessing
                        SendWorkOrder(records, bList.Barcode);
                    }
                }
                else
                {
                    int batchSize = 100;
                    for (int cntr = 0; cntr < wList.barcodeList.Count(); cntr += batchSize)
                    {
                        //bList = wList.barcodeList.Skip(batchNumber * batchSize).Take(batchSize);
                        records = new MessageProcessor(analyzer.AnalyzerId).PrepareWorklistMessages(new List<string> { "ALL" }, wList.barcodeList.Skip(cntr).Take(batchSize), isCancellation);
                        SendWorkOrder(records);
                        UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Written WO file for " + records.Count + " records.");
                    }
                }
                Program.BlockClosingApp_Reason = "";

                if ((records?.Count ?? 0) > 0)
                {
                    MessageLogger.WriteTimeDiffLog("BulkWorklist", dtmStart, "Prepared " + (records?.Count ?? 0) + " frame records.");
                }
            }

            //AttemptPendingWO();

            //System.Threading.Thread.Sleep(120000);
            //RestartApplication();
        }

        private void ReattemptResults()
        {
            if (!InterfaceHelper.CheckForInternetConnection() || !WebAPI.IsVSoftConnectionSucceeded)
                return;

            ResultUpdater.ReattemptResults();
        }

        private void UpdateConnectivityStatus()
        {
            string message = "";
            if (!InterfaceHelper.CheckForInternetConnection())
            {
                message = "Internet is not connected";
            }
            //else if (!WebAPI.IsLabsoConnectionSucceeded)
            if (!InterfaceHelper.CheckForVSoftConnection())
            {
                message = "Unable to connect VSoft servers";
            }

            if (message != "")
            {
                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, message);
            }
            lblServerStatus.Invoke(new EventHandler(delegate
            {
                lblServerStatus.BackColor = message == "" ? Color.Lime : Color.Red;
            }));
        }

        private void CheckThreadCount()
        {
            int threadCount = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
            if (threadCount > 320)
            {
                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "ALERT: Consider restarting LIS, thread count reached " + threadCount);
            }
        }

        private void btnSelectFolder_ResultFilePath_Click(object sender, EventArgs e)
        {
            string filePath = Common.InterfaceUIHelper.OpenFileDialog(txtSelectFolder_ResultFilePath);
            if (!String.IsNullOrEmpty(filePath))
            {
                ClearAllOutput();

                if (msgConfig.IsManualTestSelection)
                    ResetTestSelection();
                else
                    AttemptUploadFile();
            }
        }

        public static void ScheduleMethodInThread(Action MethodReference, TimeSpan ts)
        {
            new Thread(() =>
            {
                while (true)
                {
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    try
                    {
                        MethodReference();
                    }
                    catch (Exception ex)
                    {
                        UiMediator.LogAndShowError(analyzer.AnalyzerId, ex, "Error in " + nameof(MethodReference)/*, "Error in " + MethodReference.Method.Name*/);
                    }

                    //sleep to run after a fixed interval, similar to a job frequency
                    //int secondsElapsed = (int)sw.Elapsed.TotalSeconds;
                    //Thread.Sleep((secondsElapsed > FrequencyInSeconds ? FrequencyInSeconds : (FrequencyInSeconds - secondsElapsed)) * 1000);
                    double millisecondsElapsed = sw.Elapsed.TotalMilliseconds;
                    Thread.Sleep((int)(millisecondsElapsed > ts.TotalMilliseconds ? ts.TotalMilliseconds : (ts.TotalMilliseconds - millisecondsElapsed)));
                }
            }).Start();
        }

        //moved function to CommunicationBase.cs
        //private void AttemptPendingWO()
        //{
        //    if (connectionSettings_WO.IsFileBased)
        //        return;

        //    //attempt previous pending communications if any
        //    string directoryPath = System.IO.Path.Combine(MainForm.ApplicationDataFolder, "PendingWOCommunications");
        //    if (Directory.Exists(directoryPath))
        //    {
        //        foreach (var file in Directory.GetFiles(directoryPath))
        //        {
        //            string guid = Path.GetFileNameWithoutExtension(file);
        //            if (!_communication_BulkWO.responseFramesGuid.Contains(guid))
        //            {
        //                _communication_BulkWO.responseFramesGuid.Enqueue(guid);
        //                var records = File.ReadAllLines(file).ToList();
        //                _communication_BulkWO.SendRecords(records, guid);

        //                while (_communication_BulkWO.responseFramesGuid/*.Any()*/.Count > 0)
        //                    Thread.Sleep(100);

        //                UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Finished sending " + records.Count + " records.");

        //                if (records.Count > 100) //to ensure time gap for Centralink
        //                    Thread.Sleep(10000);
        //            }
        //        }
        //    }
        //}

        private void SendWorkOrder(List<string> records, string barcode = "")
        {
            if (records.Count > 0)
            {
                if (connectionSettings_WO.IsFileBased)
                {
                    string woFilePath = Path.Combine(connectionSettings_WO.FilePath, (String.IsNullOrEmpty(barcode) ? Guid.NewGuid().ToString() : barcode) + ".ord");
                    TextLogger.WriteTextFile(woFilePath, String.Join(Characters.LF, records), true);
                    TextLogger.WriteTextFile(Path.Combine(ApplicationDataFolder, "SentWorkOrderLog", DateTime.Now.ToString("yyyyMMdd") + ".txt"), String.Join(" ", DateTime.Now.ToString(), "Barcode : " + barcode, "Records : " + records.Count, Environment.NewLine), false);
                    UiMediator.AddUiGridData(msgConfig.AnalyzerID, barcode, "WOFile", "", null, "", "", null, null);
                    UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Outgoing, "Written WO file to folder: " + woFilePath);
                }
                else
                {
                    //cb.SendRecords(records);
                    CommunicationBase.WritePendingWOFile(records, barcode);
                }
            }
        }

        public void UpdateConnectionStatus(ConnectionStatus connectionStatus)
        {
            Invoke(new Action(() =>
            {
                if (!lblConnectionStatus.Visible)
                {
                    if (connectionSettings_Result.TCP_PortNumber.HasValue || connectionSettings_WO.TCP_PortNumber.HasValue)
                        lblConnectionStatus.Visible = true;
                }

                lblConnectionStatus.Text = connectionStatus.ToString();
                if (connectionStatus == ConnectionStatus.Connected)
                {
                    lblConnectionStatus.BackColor = Color.Lime;
                }
                else if (connectionStatus == ConnectionStatus.Disconnected)
                {
                    lblConnectionStatus.BackColor = Color.Red;
                }
                else if (connectionStatus == ConnectionStatus.Listening)
                {
                    lblConnectionStatus.BackColor = Color.Orange;
                }
            }));

            MessageLogger.WriteTimeDiffLog("TcpConnectionStatus", null, connectionStatus.ToString());
        }
    }
}

