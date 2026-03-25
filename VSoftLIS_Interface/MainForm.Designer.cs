namespace VSoftLIS_Interface
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSerialPorts = new System.Windows.Forms.ComboBox();
            this.btnOpenPort = new System.Windows.Forms.Button();
            this.btnClosePort = new System.Windows.Forms.Button();
            this.rtbMessage = new System.Windows.Forms.RichTextBox();
            this.dgvRecords = new System.Windows.Forms.DataGridView();
            this.rtbDebug = new System.Windows.Forms.RichTextBox();
            this.txtCommunicationInput = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlCommunicationConfig = new System.Windows.Forms.Panel();
            this.pnlSelectFolder_ResultFilePath = new System.Windows.Forms.Panel();
            this.btnSelectFolder_ResultFilePath = new System.Windows.Forms.Button();
            this.txtSelectFolder_ResultFilePath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblServerStatus = new System.Windows.Forms.Label();
            this.txtTCP_HostIpAddress2 = new System.Windows.Forms.MaskedTextBox();
            this.pnlTestSelection = new System.Windows.Forms.Panel();
            this.cmbTestSelection = new System.Windows.Forms.ComboBox();
            this.btnRefreshConnection = new System.Windows.Forms.Button();
            this.btnRestart = new System.Windows.Forms.Button();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.pnlControlCharacters = new System.Windows.Forms.Panel();
            this.btnEOT = new System.Windows.Forms.Button();
            this.btnENQ = new System.Windows.Forms.Button();
            this.btnACK = new System.Windows.Forms.Button();
            this.btnUploadResults = new System.Windows.Forms.Button();
            this.pnlSerialConnection = new System.Windows.Forms.Panel();
            this.pnlTcpConnection = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTCP_HostIpAddress = new System.Windows.Forms.TextBox();
            this.nudTCP_HostPort = new System.Windows.Forms.NumericUpDown();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rtbErrorMessages = new System.Windows.Forms.RichTextBox();
            this.IsSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Timestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Barcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProcessDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Labcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Age = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RefDr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.request_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Test_Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResultAbnormalFlag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValuesForReference = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlCommunicationConfig.SuspendLayout();
            this.pnlSelectFolder_ResultFilePath.SuspendLayout();
            this.pnlTestSelection.SuspendLayout();
            this.pnlControlCharacters.SuspendLayout();
            this.pnlSerialConnection.SuspendLayout();
            this.pnlTcpConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTCP_HostPort)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Available Ports";
            // 
            // cmbSerialPorts
            // 
            this.cmbSerialPorts.FormattingEnabled = true;
            this.cmbSerialPorts.Location = new System.Drawing.Point(110, 3);
            this.cmbSerialPorts.Margin = new System.Windows.Forms.Padding(4);
            this.cmbSerialPorts.Name = "cmbSerialPorts";
            this.cmbSerialPorts.Size = new System.Drawing.Size(92, 24);
            this.cmbSerialPorts.TabIndex = 7;
            // 
            // btnOpenPort
            // 
            this.btnOpenPort.Enabled = false;
            this.btnOpenPort.Location = new System.Drawing.Point(408, 8);
            this.btnOpenPort.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenPort.Name = "btnOpenPort";
            this.btnOpenPort.Size = new System.Drawing.Size(56, 28);
            this.btnOpenPort.TabIndex = 8;
            this.btnOpenPort.Text = "Open";
            this.btnOpenPort.UseVisualStyleBackColor = true;
            this.btnOpenPort.Visible = false;
            this.btnOpenPort.Click += new System.EventHandler(this.btnOpenPort_Click);
            // 
            // btnClosePort
            // 
            this.btnClosePort.Enabled = false;
            this.btnClosePort.Location = new System.Drawing.Point(466, 8);
            this.btnClosePort.Margin = new System.Windows.Forms.Padding(4);
            this.btnClosePort.Name = "btnClosePort";
            this.btnClosePort.Size = new System.Drawing.Size(56, 28);
            this.btnClosePort.TabIndex = 9;
            this.btnClosePort.Text = "Close";
            this.btnClosePort.UseVisualStyleBackColor = true;
            this.btnClosePort.Visible = false;
            this.btnClosePort.Click += new System.EventHandler(this.btnClosePort_Click);
            // 
            // rtbMessage
            // 
            this.rtbMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbMessage.Location = new System.Drawing.Point(0, 0);
            this.rtbMessage.Margin = new System.Windows.Forms.Padding(4);
            this.rtbMessage.Name = "rtbMessage";
            this.rtbMessage.ReadOnly = true;
            this.rtbMessage.Size = new System.Drawing.Size(776, 129);
            this.rtbMessage.TabIndex = 10;
            this.rtbMessage.Text = "";
            this.rtbMessage.WordWrap = false;
            // 
            // dgvRecords
            // 
            this.dgvRecords.AllowUserToAddRows = false;
            this.dgvRecords.AllowUserToDeleteRows = false;
            this.dgvRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecords.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsSelected,
            this.Timestamp,
            this.Barcode,
            this.ProcessDate,
            this.Labcode,
            this.PatientName,
            this.Age,
            this.Gender,
            this.PatientId,
            this.RefDr,
            this.request_type,
            this.Test_Code,
            this.value,
            this.ResultAbnormalFlag,
            this.Remarks,
            this.ValuesForReference});
            this.dgvRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecords.Location = new System.Drawing.Point(0, 0);
            this.dgvRecords.Margin = new System.Windows.Forms.Padding(4);
            this.dgvRecords.Name = "dgvRecords";
            this.dgvRecords.Size = new System.Drawing.Size(1352, 395);
            this.dgvRecords.TabIndex = 11;
            this.dgvRecords.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRecords_CellClick);
            this.dgvRecords.Click += new System.EventHandler(this.dgvRecords_Click);
            this.dgvRecords.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvRecords_KeyDown);
            // 
            // rtbDebug
            // 
            this.rtbDebug.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbDebug.Location = new System.Drawing.Point(28, 4);
            this.rtbDebug.Margin = new System.Windows.Forms.Padding(4);
            this.rtbDebug.Name = "rtbDebug";
            this.rtbDebug.ReadOnly = true;
            this.rtbDebug.Size = new System.Drawing.Size(517, 129);
            this.rtbDebug.TabIndex = 14;
            this.rtbDebug.Text = "";
            // 
            // txtCommunicationInput
            // 
            this.txtCommunicationInput.Location = new System.Drawing.Point(1056, 4);
            this.txtCommunicationInput.Margin = new System.Windows.Forms.Padding(4);
            this.txtCommunicationInput.Multiline = true;
            this.txtCommunicationInput.Name = "txtCommunicationInput";
            this.txtCommunicationInput.Size = new System.Drawing.Size(327, 502);
            this.txtCommunicationInput.TabIndex = 15;
            this.txtCommunicationInput.Visible = false;
            this.txtCommunicationInput.TextChanged += new System.EventHandler(this.txtCommunicationInput_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.pnlCommunicationConfig, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1360, 587);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // pnlCommunicationConfig
            // 
            this.pnlCommunicationConfig.Controls.Add(this.pnlSelectFolder_ResultFilePath);
            this.pnlCommunicationConfig.Controls.Add(this.lblServerStatus);
            this.pnlCommunicationConfig.Controls.Add(this.txtTCP_HostIpAddress2);
            this.pnlCommunicationConfig.Controls.Add(this.pnlTestSelection);
            this.pnlCommunicationConfig.Controls.Add(this.btnRefreshConnection);
            this.pnlCommunicationConfig.Controls.Add(this.btnRestart);
            this.pnlCommunicationConfig.Controls.Add(this.lblConnectionStatus);
            this.pnlCommunicationConfig.Controls.Add(this.pnlControlCharacters);
            this.pnlCommunicationConfig.Controls.Add(this.btnUploadResults);
            this.pnlCommunicationConfig.Controls.Add(this.pnlSerialConnection);
            this.pnlCommunicationConfig.Controls.Add(this.txtCommunicationInput);
            this.pnlCommunicationConfig.Controls.Add(this.btnOpenPort);
            this.pnlCommunicationConfig.Controls.Add(this.btnClosePort);
            this.pnlCommunicationConfig.Controls.Add(this.pnlTcpConnection);
            this.pnlCommunicationConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCommunicationConfig.Location = new System.Drawing.Point(4, 4);
            this.pnlCommunicationConfig.Margin = new System.Windows.Forms.Padding(4);
            this.pnlCommunicationConfig.Name = "pnlCommunicationConfig";
            this.pnlCommunicationConfig.Size = new System.Drawing.Size(1352, 41);
            this.pnlCommunicationConfig.TabIndex = 0;
            // 
            // pnlSelectFolder_ResultFilePath
            // 
            this.pnlSelectFolder_ResultFilePath.Controls.Add(this.btnSelectFolder_ResultFilePath);
            this.pnlSelectFolder_ResultFilePath.Controls.Add(this.txtSelectFolder_ResultFilePath);
            this.pnlSelectFolder_ResultFilePath.Controls.Add(this.label3);
            this.pnlSelectFolder_ResultFilePath.Location = new System.Drawing.Point(35, 1);
            this.pnlSelectFolder_ResultFilePath.Name = "pnlSelectFolder_ResultFilePath";
            this.pnlSelectFolder_ResultFilePath.Size = new System.Drawing.Size(456, 34);
            this.pnlSelectFolder_ResultFilePath.TabIndex = 27;
            this.pnlSelectFolder_ResultFilePath.Visible = false;
            // 
            // btnSelectFolder_ResultFilePath
            // 
            this.btnSelectFolder_ResultFilePath.Location = new System.Drawing.Point(422, 6);
            this.btnSelectFolder_ResultFilePath.Name = "btnSelectFolder_ResultFilePath";
            this.btnSelectFolder_ResultFilePath.Size = new System.Drawing.Size(24, 20);
            this.btnSelectFolder_ResultFilePath.TabIndex = 27;
            this.btnSelectFolder_ResultFilePath.Text = "...";
            this.btnSelectFolder_ResultFilePath.UseVisualStyleBackColor = true;
            this.btnSelectFolder_ResultFilePath.Click += new System.EventHandler(this.btnSelectFolder_ResultFilePath_Click);
            // 
            // txtSelectFolder_ResultFilePath
            // 
            this.txtSelectFolder_ResultFilePath.BackColor = System.Drawing.Color.White;
            this.txtSelectFolder_ResultFilePath.Location = new System.Drawing.Point(84, 6);
            this.txtSelectFolder_ResultFilePath.Name = "txtSelectFolder_ResultFilePath";
            this.txtSelectFolder_ResultFilePath.ReadOnly = true;
            this.txtSelectFolder_ResultFilePath.Size = new System.Drawing.Size(337, 22);
            this.txtSelectFolder_ResultFilePath.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 16);
            this.label3.TabIndex = 25;
            this.label3.Text = "Select File:";
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.AutoSize = true;
            this.lblServerStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblServerStatus.Location = new System.Drawing.Point(353, 13);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(50, 18);
            this.lblServerStatus.TabIndex = 26;
            this.lblServerStatus.Text = "Server";
            // 
            // txtTCP_HostIpAddress2
            // 
            this.txtTCP_HostIpAddress2.Location = new System.Drawing.Point(880, -3);
            this.txtTCP_HostIpAddress2.Mask = "999\\.999\\.999\\.999";
            this.txtTCP_HostIpAddress2.Name = "txtTCP_HostIpAddress2";
            this.txtTCP_HostIpAddress2.PromptChar = ' ';
            this.txtTCP_HostIpAddress2.Size = new System.Drawing.Size(100, 22);
            this.txtTCP_HostIpAddress2.TabIndex = 24;
            this.txtTCP_HostIpAddress2.Visible = false;
            this.txtTCP_HostIpAddress2.TypeValidationCompleted += new System.Windows.Forms.TypeValidationEventHandler(this.txtTCP_HostIpAddress_TypeValidationCompleted);
            // 
            // pnlTestSelection
            // 
            this.pnlTestSelection.Controls.Add(this.cmbTestSelection);
            this.pnlTestSelection.Location = new System.Drawing.Point(507, 6);
            this.pnlTestSelection.Name = "pnlTestSelection";
            this.pnlTestSelection.Size = new System.Drawing.Size(141, 32);
            this.pnlTestSelection.TabIndex = 25;
            this.pnlTestSelection.Visible = false;
            // 
            // cmbTestSelection
            // 
            this.cmbTestSelection.FormattingEnabled = true;
            this.cmbTestSelection.Location = new System.Drawing.Point(4, 1);
            this.cmbTestSelection.Name = "cmbTestSelection";
            this.cmbTestSelection.Size = new System.Drawing.Size(121, 24);
            this.cmbTestSelection.TabIndex = 0;
            this.cmbTestSelection.SelectedIndexChanged += new System.EventHandler(this.cmbTestSelection_SelectedIndexChanged);
            // 
            // btnRefreshConnection
            // 
            this.btnRefreshConnection.Location = new System.Drawing.Point(738, 6);
            this.btnRefreshConnection.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefreshConnection.Name = "btnRefreshConnection";
            this.btnRefreshConnection.Size = new System.Drawing.Size(108, 28);
            this.btnRefreshConnection.TabIndex = 23;
            this.btnRefreshConnection.Text = "Refresh Conn.";
            this.btnRefreshConnection.UseVisualStyleBackColor = true;
            this.btnRefreshConnection.Visible = false;
            this.btnRefreshConnection.Click += new System.EventHandler(this.btnRefreshConnection_Click);
            // 
            // btnRestart
            // 
            this.btnRestart.Location = new System.Drawing.Point(673, 7);
            this.btnRestart.Margin = new System.Windows.Forms.Padding(4);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(64, 28);
            this.btnRestart.TabIndex = 23;
            this.btnRestart.Text = "Restart";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblConnectionStatus.Location = new System.Drawing.Point(256, 13);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(93, 18);
            this.lblConnectionStatus.TabIndex = 22;
            this.lblConnectionStatus.Text = "Disconnected";
            this.lblConnectionStatus.Visible = false;
            // 
            // pnlControlCharacters
            // 
            this.pnlControlCharacters.Controls.Add(this.btnEOT);
            this.pnlControlCharacters.Controls.Add(this.btnENQ);
            this.pnlControlCharacters.Controls.Add(this.btnACK);
            this.pnlControlCharacters.Location = new System.Drawing.Point(986, 1);
            this.pnlControlCharacters.Name = "pnlControlCharacters";
            this.pnlControlCharacters.Size = new System.Drawing.Size(70, 41);
            this.pnlControlCharacters.TabIndex = 21;
            this.pnlControlCharacters.Visible = false;
            // 
            // btnEOT
            // 
            this.btnEOT.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEOT.Location = new System.Drawing.Point(0, 20);
            this.btnEOT.Name = "btnEOT";
            this.btnEOT.Size = new System.Drawing.Size(34, 20);
            this.btnEOT.TabIndex = 2;
            this.btnEOT.Text = "EOT";
            this.btnEOT.UseVisualStyleBackColor = true;
            this.btnEOT.Click += new System.EventHandler(this.btnControlCharacter_Click);
            // 
            // btnENQ
            // 
            this.btnENQ.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnENQ.Location = new System.Drawing.Point(0, 1);
            this.btnENQ.Name = "btnENQ";
            this.btnENQ.Size = new System.Drawing.Size(34, 20);
            this.btnENQ.TabIndex = 1;
            this.btnENQ.Text = "ENQ";
            this.btnENQ.UseVisualStyleBackColor = true;
            this.btnENQ.Click += new System.EventHandler(this.btnControlCharacter_Click);
            // 
            // btnACK
            // 
            this.btnACK.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnACK.Location = new System.Drawing.Point(33, 1);
            this.btnACK.Name = "btnACK";
            this.btnACK.Size = new System.Drawing.Size(34, 20);
            this.btnACK.TabIndex = 0;
            this.btnACK.Text = "ACK";
            this.btnACK.UseVisualStyleBackColor = true;
            this.btnACK.Click += new System.EventHandler(this.btnControlCharacter_Click);
            // 
            // btnUploadResults
            // 
            this.btnUploadResults.Location = new System.Drawing.Point(850, 8);
            this.btnUploadResults.Margin = new System.Windows.Forms.Padding(4);
            this.btnUploadResults.Name = "btnUploadResults";
            this.btnUploadResults.Size = new System.Drawing.Size(109, 28);
            this.btnUploadResults.TabIndex = 20;
            this.btnUploadResults.Text = "Upload Results";
            this.btnUploadResults.UseVisualStyleBackColor = true;
            this.btnUploadResults.Visible = false;
            this.btnUploadResults.Click += new System.EventHandler(this.btnUploadResults_Click);
            // 
            // pnlSerialConnection
            // 
            this.pnlSerialConnection.Controls.Add(this.label1);
            this.pnlSerialConnection.Controls.Add(this.cmbSerialPorts);
            this.pnlSerialConnection.Location = new System.Drawing.Point(26, 6);
            this.pnlSerialConnection.Name = "pnlSerialConnection";
            this.pnlSerialConnection.Size = new System.Drawing.Size(224, 32);
            this.pnlSerialConnection.TabIndex = 18;
            this.pnlSerialConnection.Visible = false;
            // 
            // pnlTcpConnection
            // 
            this.pnlTcpConnection.Controls.Add(this.label2);
            this.pnlTcpConnection.Controls.Add(this.txtTCP_HostIpAddress);
            this.pnlTcpConnection.Controls.Add(this.nudTCP_HostPort);
            this.pnlTcpConnection.Location = new System.Drawing.Point(9, 6);
            this.pnlTcpConnection.Name = "pnlTcpConnection";
            this.pnlTcpConnection.Size = new System.Drawing.Size(244, 32);
            this.pnlTcpConnection.TabIndex = 19;
            this.pnlTcpConnection.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 8);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 16);
            this.label2.TabIndex = 18;
            this.label2.Text = "TCP Host";
            // 
            // txtTCP_HostIpAddress
            // 
            this.txtTCP_HostIpAddress.Location = new System.Drawing.Point(75, 5);
            this.txtTCP_HostIpAddress.Name = "txtTCP_HostIpAddress";
            this.txtTCP_HostIpAddress.Size = new System.Drawing.Size(100, 22);
            this.txtTCP_HostIpAddress.TabIndex = 17;
            this.txtTCP_HostIpAddress.Leave += new System.EventHandler(this.txtTCP_HostIpAddress_Leave);
            // 
            // nudTCP_HostPort
            // 
            this.nudTCP_HostPort.Location = new System.Drawing.Point(178, 5);
            this.nudTCP_HostPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudTCP_HostPort.Name = "nudTCP_HostPort";
            this.nudTCP_HostPort.Size = new System.Drawing.Size(56, 22);
            this.nudTCP_HostPort.TabIndex = 0;
            this.nudTCP_HostPort.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvRecords);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(4, 53);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1352, 395);
            this.panel2.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 455);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rtbMessage);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rtbErrorMessages);
            this.splitContainer1.Panel2.Controls.Add(this.rtbDebug);
            this.splitContainer1.Size = new System.Drawing.Size(1354, 129);
            this.splitContainer1.SplitterDistance = 776;
            this.splitContainer1.TabIndex = 2;
            // 
            // rtbErrorMessages
            // 
            this.rtbErrorMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbErrorMessages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbErrorMessages.Location = new System.Drawing.Point(0, 0);
            this.rtbErrorMessages.Margin = new System.Windows.Forms.Padding(4);
            this.rtbErrorMessages.Name = "rtbErrorMessages";
            this.rtbErrorMessages.ReadOnly = true;
            this.rtbErrorMessages.Size = new System.Drawing.Size(574, 129);
            this.rtbErrorMessages.TabIndex = 15;
            this.rtbErrorMessages.Text = "";
            // 
            // IsSelected
            // 
            this.IsSelected.HeaderText = "";
            this.IsSelected.IndeterminateValue = "";
            this.IsSelected.Name = "IsSelected";
            this.IsSelected.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.IsSelected.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsSelected.Visible = false;
            this.IsSelected.Width = 30;
            // 
            // Timestamp
            // 
            this.Timestamp.DataPropertyName = "Timestamp";
            this.Timestamp.HeaderText = "Timestamp";
            this.Timestamp.Name = "Timestamp";
            this.Timestamp.ReadOnly = true;
            this.Timestamp.Width = 150;
            // 
            // Barcode
            // 
            this.Barcode.DataPropertyName = "Barcode";
            this.Barcode.HeaderText = "Barcode";
            this.Barcode.Name = "Barcode";
            this.Barcode.ReadOnly = true;
            this.Barcode.Width = 80;
            // 
            // ProcessDate
            // 
            this.ProcessDate.DataPropertyName = "ProcessDate";
            this.ProcessDate.HeaderText = "ProcessDate";
            this.ProcessDate.Name = "ProcessDate";
            this.ProcessDate.ReadOnly = true;
            this.ProcessDate.Visible = false;
            this.ProcessDate.Width = 80;
            // 
            // Labcode
            // 
            this.Labcode.DataPropertyName = "Labcode";
            this.Labcode.HeaderText = "Labcode";
            this.Labcode.Name = "Labcode";
            this.Labcode.ReadOnly = true;
            this.Labcode.Visible = false;
            this.Labcode.Width = 80;
            // 
            // PatientName
            // 
            this.PatientName.DataPropertyName = "PatientName";
            this.PatientName.HeaderText = "Patient";
            this.PatientName.Name = "PatientName";
            this.PatientName.ReadOnly = true;
            this.PatientName.Visible = false;
            this.PatientName.Width = 200;
            // 
            // Age
            // 
            this.Age.DataPropertyName = "Age";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Age.DefaultCellStyle = dataGridViewCellStyle1;
            this.Age.HeaderText = "Age";
            this.Age.Name = "Age";
            this.Age.ReadOnly = true;
            this.Age.Visible = false;
            this.Age.Width = 40;
            // 
            // Gender
            // 
            this.Gender.DataPropertyName = "Gender";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Gender.DefaultCellStyle = dataGridViewCellStyle2;
            this.Gender.HeaderText = "Sex";
            this.Gender.Name = "Gender";
            this.Gender.ReadOnly = true;
            this.Gender.Visible = false;
            this.Gender.Width = 40;
            // 
            // PatientId
            // 
            this.PatientId.DataPropertyName = "PatientId";
            this.PatientId.HeaderText = "Patient Id";
            this.PatientId.Name = "PatientId";
            this.PatientId.ReadOnly = true;
            this.PatientId.Visible = false;
            // 
            // RefDr
            // 
            this.RefDr.DataPropertyName = "RefDr";
            this.RefDr.HeaderText = "Ref. Dr.";
            this.RefDr.Name = "RefDr";
            this.RefDr.ReadOnly = true;
            this.RefDr.Visible = false;
            // 
            // request_type
            // 
            this.request_type.DataPropertyName = "request_type";
            this.request_type.HeaderText = "Type";
            this.request_type.Name = "request_type";
            this.request_type.ReadOnly = true;
            this.request_type.Width = 80;
            // 
            // Test_Code
            // 
            this.Test_Code.DataPropertyName = "Test_Code";
            this.Test_Code.HeaderText = "Test";
            this.Test_Code.Name = "Test_Code";
            this.Test_Code.ReadOnly = true;
            this.Test_Code.Width = 80;
            // 
            // value
            // 
            this.value.DataPropertyName = "value";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.value.DefaultCellStyle = dataGridViewCellStyle3;
            this.value.HeaderText = "Test Value";
            this.value.Name = "value";
            this.value.ReadOnly = true;
            this.value.Width = 60;
            // 
            // ResultAbnormalFlag
            // 
            this.ResultAbnormalFlag.DataPropertyName = "ResultAbnormalFlag";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ResultAbnormalFlag.DefaultCellStyle = dataGridViewCellStyle4;
            this.ResultAbnormalFlag.HeaderText = "Prefix";
            this.ResultAbnormalFlag.Name = "ResultAbnormalFlag";
            this.ResultAbnormalFlag.Width = 50;
            // 
            // Remarks
            // 
            this.Remarks.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Remarks.DataPropertyName = "Remarks";
            this.Remarks.HeaderText = "Additional Info";
            this.Remarks.Name = "Remarks";
            this.Remarks.ReadOnly = true;
            // 
            // ValuesForReference
            // 
            this.ValuesForReference.DataPropertyName = "ValuesForReference";
            this.ValuesForReference.HeaderText = "ValuesForReference";
            this.ValuesForReference.Name = "ValuesForReference";
            this.ValuesForReference.ReadOnly = true;
            this.ValuesForReference.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1360, 587);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "VSoft LIS";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pnlCommunicationConfig.ResumeLayout(false);
            this.pnlCommunicationConfig.PerformLayout();
            this.pnlSelectFolder_ResultFilePath.ResumeLayout(false);
            this.pnlSelectFolder_ResultFilePath.PerformLayout();
            this.pnlTestSelection.ResumeLayout(false);
            this.pnlControlCharacters.ResumeLayout(false);
            this.pnlSerialConnection.ResumeLayout(false);
            this.pnlSerialConnection.PerformLayout();
            this.pnlTcpConnection.ResumeLayout(false);
            this.pnlTcpConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTCP_HostPort)).EndInit();
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCommunicationConfig;
        private System.Windows.Forms.Panel pnlSelectFolder_ResultFilePath;
        private System.Windows.Forms.Button btnSelectFolder_ResultFilePath;
        private System.Windows.Forms.TextBox txtSelectFolder_ResultFilePath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblServerStatus;
        private System.Windows.Forms.MaskedTextBox txtTCP_HostIpAddress2;
        private System.Windows.Forms.Panel pnlTestSelection;
        private System.Windows.Forms.ComboBox cmbTestSelection;
        private System.Windows.Forms.Button btnRefreshConnection;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Panel pnlControlCharacters;
        private System.Windows.Forms.Button btnEOT;
        private System.Windows.Forms.Button btnENQ;
        private System.Windows.Forms.Button btnACK;
        private System.Windows.Forms.Button btnUploadResults;
        private System.Windows.Forms.Panel pnlSerialConnection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSerialPorts;
        private System.Windows.Forms.TextBox txtCommunicationInput;
        private System.Windows.Forms.Button btnOpenPort;
        private System.Windows.Forms.Button btnClosePort;
        private System.Windows.Forms.Panel pnlTcpConnection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTCP_HostIpAddress;
        private System.Windows.Forms.NumericUpDown nudTCP_HostPort;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgvRecords;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timestamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn Barcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProcessDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Labcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Age;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gender;
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientId;
        private System.Windows.Forms.DataGridViewTextBoxColumn RefDr;
        private System.Windows.Forms.DataGridViewTextBoxColumn request_type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Test_Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn value;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResultAbnormalFlag;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remarks;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValuesForReference;
        private System.Windows.Forms.RichTextBox rtbErrorMessages;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox rtbMessage;
        private System.Windows.Forms.RichTextBox rtbDebug;
    }
}

