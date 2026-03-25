namespace VSoftLIS_Interface
{
    partial class frmLISConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLISConfiguration));
            this.dgvLISList = new System.Windows.Forms.DataGridView();
            this.dgvcAnalyzerId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcAnalyzerTypeId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcAnalyzerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcIsActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvcConnectionMode = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgvcIPAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcPortDetails = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgvcFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcBaudRate = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgvcParity = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgvcStopBits = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgvcDataBits = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlConnectionSettings = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlTcpPortSettings = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTCP_HostIpAddress = new System.Windows.Forms.TextBox();
            this.nudTCP_HostPort = new System.Windows.Forms.NumericUpDown();
            this.pnlSerialPortSettings = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbBaudRate = new System.Windows.Forms.ComboBox();
            this.cmbSerialPorts = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbStopBits = new System.Windows.Forms.ComboBox();
            this.lblDataBits = new System.Windows.Forms.Label();
            this.cmbParity = new System.Windows.Forms.ComboBox();
            this.lblBaudRate = new System.Windows.Forms.Label();
            this.cmbDataBits = new System.Windows.Forms.ComboBox();
            this.lblStopBits = new System.Windows.Forms.Label();
            this.rdoSerial = new System.Windows.Forms.RadioButton();
            this.rdoTCP = new System.Windows.Forms.RadioButton();
            this.pnlSelectFolder_ResultFilePath = new System.Windows.Forms.Panel();
            this.btnSelectFolder_ResultFilePath = new System.Windows.Forms.Button();
            this.txtSelectFolder_ResultFilePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlSelectFolder_WOFilePath = new System.Windows.Forms.Panel();
            this.btnSelectFolder_WOFilePath = new System.Windows.Forms.Button();
            this.txtSelectFolder_WOFilePath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlSelectDBFile = new System.Windows.Forms.Panel();
            this.btnSelectDBFile = new System.Windows.Forms.Button();
            this.txtSelectDBFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.rdoFile = new System.Windows.Forms.RadioButton();
            this.rdoTcpSerial = new System.Windows.Forms.RadioButton();
            this.chkAdvanced = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.dgvAdditionalSettings = new System.Windows.Forms.DataGridView();
            this.dgvcSettingName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcSettingValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLISList)).BeginInit();
            this.pnlConnectionSettings.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlTcpPortSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTCP_HostPort)).BeginInit();
            this.pnlSerialPortSettings.SuspendLayout();
            this.pnlSelectFolder_ResultFilePath.SuspendLayout();
            this.pnlSelectFolder_WOFilePath.SuspendLayout();
            this.pnlSelectDBFile.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdditionalSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLISList
            // 
            this.dgvLISList.AllowUserToAddRows = false;
            this.dgvLISList.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLISList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLISList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLISList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvcAnalyzerId,
            this.dgvcAnalyzerTypeId,
            this.dgvcAnalyzerName,
            this.dgvcIsActive,
            this.dgvcConnectionMode,
            this.dgvcIPAddress,
            this.dgvcPortDetails,
            this.dgvcFileName,
            this.dgvcBaudRate,
            this.dgvcParity,
            this.dgvcStopBits,
            this.dgvcDataBits});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLISList.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLISList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLISList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvLISList.Location = new System.Drawing.Point(0, 0);
            this.dgvLISList.Name = "dgvLISList";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLISList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvLISList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLISList.Size = new System.Drawing.Size(874, 427);
            this.dgvLISList.TabIndex = 0;
            this.dgvLISList.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvLISList_CellBeginEdit);
            this.dgvLISList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLISList_CellContentClick);
            this.dgvLISList.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLISList_CellValueChanged);
            this.dgvLISList.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvLISList_DataError);
            this.dgvLISList.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLISList_RowValidated);
            this.dgvLISList.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvLISList_RowValidating);
            this.dgvLISList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvLISList_KeyDown);
            // 
            // dgvcAnalyzerId
            // 
            this.dgvcAnalyzerId.HeaderText = "AnalyzerId";
            this.dgvcAnalyzerId.Name = "dgvcAnalyzerId";
            this.dgvcAnalyzerId.Visible = false;
            // 
            // dgvcAnalyzerTypeId
            // 
            this.dgvcAnalyzerTypeId.HeaderText = "AnalyzerTypeId";
            this.dgvcAnalyzerTypeId.Name = "dgvcAnalyzerTypeId";
            this.dgvcAnalyzerTypeId.Visible = false;
            // 
            // dgvcAnalyzerName
            // 
            this.dgvcAnalyzerName.DataPropertyName = "AnalyzerName";
            this.dgvcAnalyzerName.FillWeight = 20F;
            this.dgvcAnalyzerName.HeaderText = "Analyzer Name";
            this.dgvcAnalyzerName.Name = "dgvcAnalyzerName";
            this.dgvcAnalyzerName.ReadOnly = true;
            this.dgvcAnalyzerName.Width = 150;
            // 
            // dgvcIsActive
            // 
            this.dgvcIsActive.DataPropertyName = "IsActive";
            this.dgvcIsActive.HeaderText = "";
            this.dgvcIsActive.Name = "dgvcIsActive";
            this.dgvcIsActive.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvcIsActive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dgvcIsActive.Width = 20;
            // 
            // dgvcConnectionMode
            // 
            this.dgvcConnectionMode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvcConnectionMode.FillWeight = 20F;
            this.dgvcConnectionMode.HeaderText = "Connection Mode";
            this.dgvcConnectionMode.Name = "dgvcConnectionMode";
            this.dgvcConnectionMode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvcConnectionMode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dgvcIPAddress
            // 
            this.dgvcIPAddress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvcIPAddress.FillWeight = 60F;
            this.dgvcIPAddress.HeaderText = "IPAddress";
            this.dgvcIPAddress.Name = "dgvcIPAddress";
            // 
            // dgvcPortDetails
            // 
            this.dgvcPortDetails.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvcPortDetails.FillWeight = 60F;
            this.dgvcPortDetails.HeaderText = "Port Details";
            this.dgvcPortDetails.MaxDropDownItems = 100;
            this.dgvcPortDetails.Name = "dgvcPortDetails";
            this.dgvcPortDetails.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvcPortDetails.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dgvcFileName
            // 
            this.dgvcFileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvcFileName.HeaderText = "Folder Path";
            this.dgvcFileName.MinimumWidth = 100;
            this.dgvcFileName.Name = "dgvcFileName";
            // 
            // dgvcBaudRate
            // 
            this.dgvcBaudRate.HeaderText = "Baud Rate";
            this.dgvcBaudRate.Items.AddRange(new object[] {
            "110",
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "38400",
            "57600",
            "115200",
            "128000",
            "256000"});
            this.dgvcBaudRate.Name = "dgvcBaudRate";
            this.dgvcBaudRate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvcBaudRate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dgvcBaudRate.Width = 60;
            // 
            // dgvcParity
            // 
            this.dgvcParity.HeaderText = "Parity";
            this.dgvcParity.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.dgvcParity.Name = "dgvcParity";
            this.dgvcParity.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvcParity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dgvcParity.Width = 60;
            // 
            // dgvcStopBits
            // 
            this.dgvcStopBits.HeaderText = "Stop Bits";
            this.dgvcStopBits.Items.AddRange(new object[] {
            "None",
            "One",
            "Two",
            "OnePointFive"});
            this.dgvcStopBits.Name = "dgvcStopBits";
            this.dgvcStopBits.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvcStopBits.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dgvcStopBits.Width = 60;
            // 
            // dgvcDataBits
            // 
            this.dgvcDataBits.HeaderText = "Data Bits";
            this.dgvcDataBits.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.dgvcDataBits.Name = "dgvcDataBits";
            this.dgvcDataBits.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvcDataBits.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dgvcDataBits.Width = 60;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(3, 268);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 27);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(84, 268);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 27);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Visible = false;
            // 
            // pnlConnectionSettings
            // 
            this.pnlConnectionSettings.Controls.Add(this.panel1);
            this.pnlConnectionSettings.Location = new System.Drawing.Point(31, 98);
            this.pnlConnectionSettings.Name = "pnlConnectionSettings";
            this.pnlConnectionSettings.Size = new System.Drawing.Size(756, 144);
            this.pnlConnectionSettings.TabIndex = 3;
            this.pnlConnectionSettings.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlTcpPortSettings);
            this.panel1.Controls.Add(this.pnlSerialPortSettings);
            this.panel1.Controls.Add(this.rdoSerial);
            this.panel1.Controls.Add(this.rdoTCP);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(753, 42);
            this.panel1.TabIndex = 0;
            // 
            // pnlTcpPortSettings
            // 
            this.pnlTcpPortSettings.Controls.Add(this.label7);
            this.pnlTcpPortSettings.Controls.Add(this.label6);
            this.pnlTcpPortSettings.Controls.Add(this.txtTCP_HostIpAddress);
            this.pnlTcpPortSettings.Controls.Add(this.nudTCP_HostPort);
            this.pnlTcpPortSettings.Location = new System.Drawing.Point(77, 1);
            this.pnlTcpPortSettings.Name = "pnlTcpPortSettings";
            this.pnlTcpPortSettings.Size = new System.Drawing.Size(173, 42);
            this.pnlTcpPortSettings.TabIndex = 30;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(111, 2);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Port No:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 2);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "IP Address:";
            // 
            // txtTCP_HostIpAddress
            // 
            this.txtTCP_HostIpAddress.Location = new System.Drawing.Point(8, 18);
            this.txtTCP_HostIpAddress.Name = "txtTCP_HostIpAddress";
            this.txtTCP_HostIpAddress.Size = new System.Drawing.Size(100, 20);
            this.txtTCP_HostIpAddress.TabIndex = 19;
            // 
            // nudTCP_HostPort
            // 
            this.nudTCP_HostPort.Location = new System.Drawing.Point(114, 18);
            this.nudTCP_HostPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudTCP_HostPort.Name = "nudTCP_HostPort";
            this.nudTCP_HostPort.Size = new System.Drawing.Size(56, 20);
            this.nudTCP_HostPort.TabIndex = 18;
            this.nudTCP_HostPort.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            // 
            // pnlSerialPortSettings
            // 
            this.pnlSerialPortSettings.Controls.Add(this.label5);
            this.pnlSerialPortSettings.Controls.Add(this.cmbBaudRate);
            this.pnlSerialPortSettings.Controls.Add(this.cmbSerialPorts);
            this.pnlSerialPortSettings.Controls.Add(this.label1);
            this.pnlSerialPortSettings.Controls.Add(this.cmbStopBits);
            this.pnlSerialPortSettings.Controls.Add(this.lblDataBits);
            this.pnlSerialPortSettings.Controls.Add(this.cmbParity);
            this.pnlSerialPortSettings.Controls.Add(this.lblBaudRate);
            this.pnlSerialPortSettings.Controls.Add(this.cmbDataBits);
            this.pnlSerialPortSettings.Controls.Add(this.lblStopBits);
            this.pnlSerialPortSettings.Location = new System.Drawing.Point(292, 1);
            this.pnlSerialPortSettings.Name = "pnlSerialPortSettings";
            this.pnlSerialPortSettings.Size = new System.Drawing.Size(380, 42);
            this.pnlSerialPortSettings.TabIndex = 29;
            this.pnlSerialPortSettings.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 2);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "COM Port:";
            // 
            // cmbBaudRate
            // 
            this.cmbBaudRate.FormattingEnabled = true;
            this.cmbBaudRate.Items.AddRange(new object[] {
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cmbBaudRate.Location = new System.Drawing.Point(101, 18);
            this.cmbBaudRate.Name = "cmbBaudRate";
            this.cmbBaudRate.Size = new System.Drawing.Size(69, 21);
            this.cmbBaudRate.TabIndex = 22;
            // 
            // cmbSerialPorts
            // 
            this.cmbSerialPorts.FormattingEnabled = true;
            this.cmbSerialPorts.Location = new System.Drawing.Point(4, 18);
            this.cmbSerialPorts.Margin = new System.Windows.Forms.Padding(4);
            this.cmbSerialPorts.Name = "cmbSerialPorts";
            this.cmbSerialPorts.Size = new System.Drawing.Size(78, 21);
            this.cmbSerialPorts.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Parity:";
            // 
            // cmbStopBits
            // 
            this.cmbStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStopBits.FormattingEnabled = true;
            this.cmbStopBits.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cmbStopBits.Location = new System.Drawing.Point(308, 18);
            this.cmbStopBits.Name = "cmbStopBits";
            this.cmbStopBits.Size = new System.Drawing.Size(65, 21);
            this.cmbStopBits.TabIndex = 28;
            // 
            // lblDataBits
            // 
            this.lblDataBits.AutoSize = true;
            this.lblDataBits.Location = new System.Drawing.Point(244, 2);
            this.lblDataBits.Name = "lblDataBits";
            this.lblDataBits.Size = new System.Drawing.Size(53, 13);
            this.lblDataBits.TabIndex = 25;
            this.lblDataBits.Text = "Data Bits:";
            // 
            // cmbParity
            // 
            this.cmbParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParity.FormattingEnabled = true;
            this.cmbParity.Items.AddRange(new object[] {
            "None",
            "Even",
            "Odd"});
            this.cmbParity.Location = new System.Drawing.Point(176, 18);
            this.cmbParity.Name = "cmbParity";
            this.cmbParity.Size = new System.Drawing.Size(60, 21);
            this.cmbParity.TabIndex = 24;
            // 
            // lblBaudRate
            // 
            this.lblBaudRate.AutoSize = true;
            this.lblBaudRate.Location = new System.Drawing.Point(100, 2);
            this.lblBaudRate.Name = "lblBaudRate";
            this.lblBaudRate.Size = new System.Drawing.Size(61, 13);
            this.lblBaudRate.TabIndex = 21;
            this.lblBaudRate.Text = "Baud Rate:";
            // 
            // cmbDataBits
            // 
            this.cmbDataBits.FormattingEnabled = true;
            this.cmbDataBits.Items.AddRange(new object[] {
            "7",
            "8",
            "9"});
            this.cmbDataBits.Location = new System.Drawing.Point(242, 18);
            this.cmbDataBits.Name = "cmbDataBits";
            this.cmbDataBits.Size = new System.Drawing.Size(60, 21);
            this.cmbDataBits.TabIndex = 26;
            // 
            // lblStopBits
            // 
            this.lblStopBits.AutoSize = true;
            this.lblStopBits.Location = new System.Drawing.Point(310, 2);
            this.lblStopBits.Name = "lblStopBits";
            this.lblStopBits.Size = new System.Drawing.Size(52, 13);
            this.lblStopBits.TabIndex = 27;
            this.lblStopBits.Text = "Stop Bits:";
            // 
            // rdoSerial
            // 
            this.rdoSerial.AutoSize = true;
            this.rdoSerial.Location = new System.Drawing.Point(4, 23);
            this.rdoSerial.Name = "rdoSerial";
            this.rdoSerial.Size = new System.Drawing.Size(51, 17);
            this.rdoSerial.TabIndex = 1;
            this.rdoSerial.Text = "Serial";
            this.rdoSerial.UseVisualStyleBackColor = true;
            // 
            // rdoTCP
            // 
            this.rdoTCP.AutoSize = true;
            this.rdoTCP.Checked = true;
            this.rdoTCP.Location = new System.Drawing.Point(4, 5);
            this.rdoTCP.Name = "rdoTCP";
            this.rdoTCP.Size = new System.Drawing.Size(46, 17);
            this.rdoTCP.TabIndex = 0;
            this.rdoTCP.TabStop = true;
            this.rdoTCP.Text = "TCP";
            this.rdoTCP.UseVisualStyleBackColor = true;
            // 
            // pnlSelectFolder_ResultFilePath
            // 
            this.pnlSelectFolder_ResultFilePath.Controls.Add(this.btnSelectFolder_ResultFilePath);
            this.pnlSelectFolder_ResultFilePath.Controls.Add(this.txtSelectFolder_ResultFilePath);
            this.pnlSelectFolder_ResultFilePath.Controls.Add(this.label2);
            this.pnlSelectFolder_ResultFilePath.Location = new System.Drawing.Point(38, 319);
            this.pnlSelectFolder_ResultFilePath.Name = "pnlSelectFolder_ResultFilePath";
            this.pnlSelectFolder_ResultFilePath.Size = new System.Drawing.Size(753, 23);
            this.pnlSelectFolder_ResultFilePath.TabIndex = 1;
            this.pnlSelectFolder_ResultFilePath.Visible = false;
            // 
            // btnSelectFolder_ResultFilePath
            // 
            this.btnSelectFolder_ResultFilePath.Location = new System.Drawing.Point(435, 1);
            this.btnSelectFolder_ResultFilePath.Name = "btnSelectFolder_ResultFilePath";
            this.btnSelectFolder_ResultFilePath.Size = new System.Drawing.Size(24, 20);
            this.btnSelectFolder_ResultFilePath.TabIndex = 24;
            this.btnSelectFolder_ResultFilePath.Text = "...";
            this.btnSelectFolder_ResultFilePath.UseVisualStyleBackColor = true;
            this.btnSelectFolder_ResultFilePath.Click += new System.EventHandler(this.btnSelectFolder_ResultFilePath_Click);
            // 
            // txtSelectFolder_ResultFilePath
            // 
            this.txtSelectFolder_ResultFilePath.BackColor = System.Drawing.Color.White;
            this.txtSelectFolder_ResultFilePath.Location = new System.Drawing.Point(98, 1);
            this.txtSelectFolder_ResultFilePath.Name = "txtSelectFolder_ResultFilePath";
            this.txtSelectFolder_ResultFilePath.ReadOnly = true;
            this.txtSelectFolder_ResultFilePath.Size = new System.Drawing.Size(337, 20);
            this.txtSelectFolder_ResultFilePath.TabIndex = 23;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Result Files Path:";
            // 
            // pnlSelectFolder_WOFilePath
            // 
            this.pnlSelectFolder_WOFilePath.Controls.Add(this.btnSelectFolder_WOFilePath);
            this.pnlSelectFolder_WOFilePath.Controls.Add(this.txtSelectFolder_WOFilePath);
            this.pnlSelectFolder_WOFilePath.Controls.Add(this.label3);
            this.pnlSelectFolder_WOFilePath.Location = new System.Drawing.Point(38, 348);
            this.pnlSelectFolder_WOFilePath.Name = "pnlSelectFolder_WOFilePath";
            this.pnlSelectFolder_WOFilePath.Size = new System.Drawing.Size(753, 23);
            this.pnlSelectFolder_WOFilePath.TabIndex = 25;
            this.pnlSelectFolder_WOFilePath.Visible = false;
            // 
            // btnSelectFolder_WOFilePath
            // 
            this.btnSelectFolder_WOFilePath.Location = new System.Drawing.Point(435, 1);
            this.btnSelectFolder_WOFilePath.Name = "btnSelectFolder_WOFilePath";
            this.btnSelectFolder_WOFilePath.Size = new System.Drawing.Size(24, 20);
            this.btnSelectFolder_WOFilePath.TabIndex = 24;
            this.btnSelectFolder_WOFilePath.Text = "...";
            this.btnSelectFolder_WOFilePath.UseVisualStyleBackColor = true;
            // 
            // txtSelectFolder_WOFilePath
            // 
            this.txtSelectFolder_WOFilePath.BackColor = System.Drawing.Color.White;
            this.txtSelectFolder_WOFilePath.Location = new System.Drawing.Point(98, 1);
            this.txtSelectFolder_WOFilePath.Name = "txtSelectFolder_WOFilePath";
            this.txtSelectFolder_WOFilePath.ReadOnly = true;
            this.txtSelectFolder_WOFilePath.Size = new System.Drawing.Size(337, 20);
            this.txtSelectFolder_WOFilePath.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "WO Output Path:";
            // 
            // pnlSelectDBFile
            // 
            this.pnlSelectDBFile.Controls.Add(this.btnSelectDBFile);
            this.pnlSelectDBFile.Controls.Add(this.txtSelectDBFile);
            this.pnlSelectDBFile.Controls.Add(this.label4);
            this.pnlSelectDBFile.Location = new System.Drawing.Point(38, 377);
            this.pnlSelectDBFile.Name = "pnlSelectDBFile";
            this.pnlSelectDBFile.Size = new System.Drawing.Size(753, 23);
            this.pnlSelectDBFile.TabIndex = 26;
            this.pnlSelectDBFile.Visible = false;
            // 
            // btnSelectDBFile
            // 
            this.btnSelectDBFile.Location = new System.Drawing.Point(435, 1);
            this.btnSelectDBFile.Name = "btnSelectDBFile";
            this.btnSelectDBFile.Size = new System.Drawing.Size(24, 20);
            this.btnSelectDBFile.TabIndex = 24;
            this.btnSelectDBFile.Text = "...";
            this.btnSelectDBFile.UseVisualStyleBackColor = true;
            // 
            // txtSelectDBFile
            // 
            this.txtSelectDBFile.BackColor = System.Drawing.Color.White;
            this.txtSelectDBFile.Location = new System.Drawing.Point(98, 1);
            this.txtSelectDBFile.Name = "txtSelectDBFile";
            this.txtSelectDBFile.ReadOnly = true;
            this.txtSelectDBFile.Size = new System.Drawing.Size(337, 20);
            this.txtSelectDBFile.TabIndex = 23;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "DB File Path:";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(31, 19);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(65, 25);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "&Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Visible = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // rdoFile
            // 
            this.rdoFile.AutoSize = true;
            this.rdoFile.Location = new System.Drawing.Point(106, 16);
            this.rdoFile.Name = "rdoFile";
            this.rdoFile.Size = new System.Drawing.Size(41, 17);
            this.rdoFile.TabIndex = 6;
            this.rdoFile.Text = "File";
            this.rdoFile.UseVisualStyleBackColor = true;
            this.rdoFile.CheckedChanged += new System.EventHandler(this.rdoConnectionMode_CheckedChanged);
            // 
            // rdoTcpSerial
            // 
            this.rdoTcpSerial.AutoSize = true;
            this.rdoTcpSerial.Checked = true;
            this.rdoTcpSerial.Location = new System.Drawing.Point(12, 16);
            this.rdoTcpSerial.Name = "rdoTcpSerial";
            this.rdoTcpSerial.Size = new System.Drawing.Size(77, 17);
            this.rdoTcpSerial.TabIndex = 5;
            this.rdoTcpSerial.TabStop = true;
            this.rdoTcpSerial.Text = "TCP/Serial";
            this.rdoTcpSerial.UseVisualStyleBackColor = true;
            this.rdoTcpSerial.CheckedChanged += new System.EventHandler(this.rdoConnectionMode_CheckedChanged);
            // 
            // chkAdvanced
            // 
            this.chkAdvanced.AutoSize = true;
            this.chkAdvanced.Location = new System.Drawing.Point(362, 16);
            this.chkAdvanced.Name = "chkAdvanced";
            this.chkAdvanced.Size = new System.Drawing.Size(75, 17);
            this.chkAdvanced.TabIndex = 7;
            this.chkAdvanced.Text = "Advanced";
            this.chkAdvanced.UseVisualStyleBackColor = true;
            this.chkAdvanced.CheckedChanged += new System.EventHandler(this.chkAdvanced_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnRemove);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.pnlSelectFolder_ResultFilePath);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.pnlSelectFolder_WOFilePath);
            this.panel2.Controls.Add(this.pnlConnectionSettings);
            this.panel2.Controls.Add(this.pnlSelectDBFile);
            this.panel2.Location = new System.Drawing.Point(1101, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 483);
            this.panel2.TabIndex = 27;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(880, 583);
            this.tableLayoutPanel1.TabIndex = 28;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.txtSearch);
            this.panel3.Controls.Add(this.rdoTcpSerial);
            this.panel3.Controls.Add(this.rdoFile);
            this.panel3.Controls.Add(this.chkAdvanced);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(874, 44);
            this.panel3.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(686, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "&Search (F3)";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(751, 10);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(100, 20);
            this.txtSearch.TabIndex = 8;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dgvLISList);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 53);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(874, 427);
            this.panel4.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.dgvAdditionalSettings);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(3, 486);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(874, 94);
            this.panel5.TabIndex = 2;
            // 
            // dgvAdditionalSettings
            // 
            this.dgvAdditionalSettings.AllowUserToAddRows = false;
            this.dgvAdditionalSettings.AllowUserToDeleteRows = false;
            this.dgvAdditionalSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAdditionalSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvcSettingName,
            this.dgvcSettingValue});
            this.dgvAdditionalSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAdditionalSettings.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvAdditionalSettings.Location = new System.Drawing.Point(0, 0);
            this.dgvAdditionalSettings.Name = "dgvAdditionalSettings";
            this.dgvAdditionalSettings.Size = new System.Drawing.Size(874, 94);
            this.dgvAdditionalSettings.TabIndex = 0;
            // 
            // dgvcSettingName
            // 
            this.dgvcSettingName.DataPropertyName = "Name";
            this.dgvcSettingName.HeaderText = "Setting";
            this.dgvcSettingName.Name = "dgvcSettingName";
            this.dgvcSettingName.ReadOnly = true;
            this.dgvcSettingName.Width = 200;
            // 
            // dgvcSettingValue
            // 
            this.dgvcSettingValue.DataPropertyName = "Value";
            this.dgvcSettingValue.HeaderText = "Value";
            this.dgvcSettingValue.Name = "dgvcSettingValue";
            this.dgvcSettingValue.Width = 200;
            // 
            // frmLISConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 583);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmLISConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LIS Configuration";
            this.Load += new System.EventHandler(this.LISConfiguration_Load);
            this.Shown += new System.EventHandler(this.LISConfiguration_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmLISConfiguration_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLISList)).EndInit();
            this.pnlConnectionSettings.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlTcpPortSettings.ResumeLayout(false);
            this.pnlTcpPortSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTCP_HostPort)).EndInit();
            this.pnlSerialPortSettings.ResumeLayout(false);
            this.pnlSerialPortSettings.PerformLayout();
            this.pnlSelectFolder_ResultFilePath.ResumeLayout(false);
            this.pnlSelectFolder_ResultFilePath.PerformLayout();
            this.pnlSelectFolder_WOFilePath.ResumeLayout(false);
            this.pnlSelectFolder_WOFilePath.PerformLayout();
            this.pnlSelectDBFile.ResumeLayout(false);
            this.pnlSelectDBFile.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdditionalSettings)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSelectFolder_ResultFilePath;
        private System.Windows.Forms.Button btnSelectFolder_ResultFilePath;
        private System.Windows.Forms.TextBox txtSelectFolder_ResultFilePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelectFolder_WOFilePath;
        private System.Windows.Forms.TextBox txtSelectFolder_WOFilePath;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Panel pnlSelectFolder_WOFilePath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSerialPorts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbStopBits;
        private System.Windows.Forms.Label lblDataBits;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.ComboBox cmbBaudRate;
        private System.Windows.Forms.RadioButton rdoTCP;
        private System.Windows.Forms.Panel pnlSelectDBFile;
        private System.Windows.Forms.Button btnSelectDBFile;
        private System.Windows.Forms.TextBox txtSelectDBFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rdoTcpSerial;
        private System.Windows.Forms.RadioButton rdoFile;
        private System.Windows.Forms.CheckBox chkAdvanced;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView dgvLISList;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcAnalyzerId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcAnalyzerTypeId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcAnalyzerName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvcIsActive;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvcConnectionMode;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcIPAddress;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvcPortDetails;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcFileName;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvcBaudRate;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvcParity;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvcStopBits;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvcDataBits;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.DataGridView dgvAdditionalSettings;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcSettingName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcSettingValue;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.FlowLayoutPanel pnlConnectionSettings;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlTcpPortSettings;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTCP_HostIpAddress;
        private System.Windows.Forms.NumericUpDown nudTCP_HostPort;
        private System.Windows.Forms.Panel pnlSerialPortSettings;
        private System.Windows.Forms.Label lblBaudRate;
        private System.Windows.Forms.ComboBox cmbDataBits;
        private System.Windows.Forms.Label lblStopBits;
        private System.Windows.Forms.RadioButton rdoSerial;
    }
}