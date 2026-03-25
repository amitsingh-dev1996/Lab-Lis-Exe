using System;
using System.Text;
using System.IO.Ports;
using System.ComponentModel;
using System.Collections.Generic;
using VSoftLIS_Interface.Common;

namespace VSoftLIS_Interface.DLL
{
    class SerialCommunication : CommunicationBase
    {
        ErrorLog errorLog = new ErrorLog();
        BackgroundWorker _worker1 = new BackgroundWorker();
        private SerialPort comPort = null;

        public SerialCommunication(Analyzer analyzer, DataReceivedCallback DataReceived) : base(analyzer, DataReceived)
        {
            comPort = new SerialPort();
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }

        internal override bool OpenPort(ConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings;

            if (comPort.IsOpen == true) comPort.Close();

            comPort.PortName = connectionSettings.Serial_PortName;
            comPort.BaudRate = connectionSettings.Serial_BaudRate;
            comPort.Parity = (Parity)Enum.Parse(typeof(Parity), connectionSettings.Serial_Parity.ToString());
            comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), connectionSettings.Serial_StopBits.ToString());
            comPort.DataBits = connectionSettings.Serial_DataBits;
            comPort.Encoding = Encoding.GetEncoding("Windows-1252");

            if (!Program.IsDebugMode)
            {
                comPort.Open();
                ConnectionStatus = ConnectionStatus.Connected;
            }
            return true;
        }

        internal override bool ClosePort()
        {
            //WriteData(Characters.EOT);
            DiscardInOutBuffer();
            comPort.Close();
            comPort.Dispose();
            ConnectionStatus = ConnectionStatus.Disconnected;
            return true;
        }

        internal override bool IsOpen
        {
            get
            {
                return comPort.IsOpen;
            }
        }

        internal override ConnectionStatus GetConnectionStatus()
        {
            if (comPort.IsOpen)
                return ConnectionStatus.Connected;
            else
                return ConnectionStatus.Disconnected;
        }

        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Eof)
                BLL.UiMediator.AddUiMessage(Program.AnalyzerId, (int)MessageType.Normal, "EOF received from analyze.");

            string msg = "";
            if (TransmissionType == TransmissionType.Text)
            {
                msg = comPort.ReadExisting();
                //DiscardInOutBuffer(); //Commented for Dynex
            }
            else if (TransmissionType == TransmissionType.Hex)
            {
                string msgTemp = string.Empty;
                StringBuilder sb = new StringBuilder();

                msg = comPort.ReadExisting();

            }
            _DataReceived(this, msg);
        }

        protected override void OnWriteData(string msg, bool displayData = true)
        {
            if (!Program.IsDebugMode)
            {
                if (TransmissionType == TransmissionType.Hex)
                {
                    msg = InterfaceHelper.HexStringToAsciiString(msg);
                }

                if (!(comPort.IsOpen == true)) comPort.Open();

                comPort.Write(msg);
            }
        }

        internal override void DiscardInOutBuffer()
        {
            if (Program.IsDebugMode)
                return;

            this.comPort.DiscardOutBuffer();
            this.comPort.DiscardInBuffer();
        }

        public override string ToString()
        {
            return comPort.PortName;
        }
    }
}
