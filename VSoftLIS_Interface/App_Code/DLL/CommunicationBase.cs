using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VSoftLIS_Interface.BLL;
using VSoftLIS_Interface.Common;

namespace VSoftLIS_Interface.DLL
{
    internal abstract class CommunicationBase
    {
        //variables for incoming message
        private string lastMessage = string.Empty;
        private string currentFrame = string.Empty;
        private int lastFrameNumber = 0;
        private string completeFrame_DataPart = "";
        private List<string> completeMessage = new List<string>();
        List<string> queryBarcodes = new List<string>();
        //variables for outgoing message
        private string lastFrameSent = string.Empty;
        //private Queue<List<string>> responseFrames = new Queue<List<string>>();
        internal ConcurrentQueue<string> responseFramesGuid = new ConcurrentQueue<string>();
        internal List<string> currentFrames = new List<string>();
        private int currentFramesStartingCount = 0;
        private int currentFrameSendRetryCount = 0;
        private int HandshakeAttempts = 0;
        private Stopwatch HandshakeAttemptWatch = new Stopwatch();
        //private Stopwatch FrameCommunicationWatch = new Stopwatch();
        internal bool IsCommunicationIdle = true;
        private bool IsAwaitingACK = false;
        //private bool IsAwaitingFrame = false;
        protected int MaxAwaitTimeoutAttempts = 3;
        protected int AwaitTimeoutCounts = 0;
        protected int CurrentResponseTimeout = 0;
        System.Timers.Timer ResponseTimer = new System.Timers.Timer();
        private Stopwatch swENQContention = new Stopwatch();
        private Stopwatch swWaitToSendEnq = new Stopwatch();
        private double WaitTimeToSendEnq = 0;
        MessageConfiguration msgConfig = null;
        //settings
        private int FrameSendMaxRetryCount = 3;
        private bool EncapsulateAckNak = false;
        private bool SupportsAckNak = true;
        public bool AcknowledgePartialFrame = false;
        private bool SupportsHandshake = true; //supports ENQ-EOT
        protected bool SendEnquiryOnOpeningPort = false;
        internal TransmissionType TransmissionType = TransmissionType.Text;

        protected DataReceivedCallback _DataReceived = null;
        private MessageLogger messageLogger = null;
        private Analyzer analyzer = null;
        CommunicationFrameHandler frameHandler = null;
        protected ConnectionSettings ConnectionSettings = null;

        internal string Sender = "";
        readonly string ACK_Message;
        readonly string NAK_Message;
        //Task<bool> objResponseMonitor = null;

        private ConnectionStatus _ConnectionStatus;
        public ConnectionStatus ConnectionStatus
        {
            get
            {
                return _ConnectionStatus;
            }
            set
            {
                _ConnectionStatus = value;
                OnConnectionStatusChanged(new EventArgs());
            }
        }

        public CommunicationBase(Analyzer _analyzer, DataReceivedCallback DataReceived)
        {
            analyzer = _analyzer;
            _DataReceived = DataReceived;
            messageLogger = new MessageLogger(analyzer.instrumentid);
            ConnectionStatus = ConnectionStatus.Disconnected;

            switch (_analyzer.instrumentgroupid)
            {
                case 3: //Olympus AU 700
                    EncapsulateAckNak = false;
                    SupportsHandshake = false;
                    break;

                case 7: //ATELICA
                    SupportsHandshake = false;
                    break;

                case 155: //Zybio EXZ 6000 H6
                    SupportsHandshake = false;
                    break;

                case 158: //MISPA_CX4
                    SupportsHandshake = true;
                    break;
            }

            frameHandler = new CommunicationFrameHandler(_analyzer.instrumentid);

            if (SupportsAckNak)
            {
                if (EncapsulateAckNak)
                {
                    ACK_Message = Characters.STX + Characters.ACK + Characters.ETX;
                    NAK_Message = Characters.STX + Characters.NAK + Characters.ETX;
                }

                else
                {
                    ACK_Message = Characters.ACK;
                    NAK_Message = Characters.NAK;
                }
            }

            ResponseTimer.Interval = 15000;
            ResponseTimer.Elapsed += ResponseTimer_Elapsed;

        }

        internal abstract bool OpenPort(ConnectionSettings connectionSettings);
        internal abstract bool ClosePort();
        internal abstract bool IsOpen { get; }
        internal abstract ConnectionStatus GetConnectionStatus();
        internal void WriteData(string data, bool displayData = false)
        {
            //System.Threading.Thread.Sleep(1); //sleep 1 millisecond to avoid concurrent send-receive

            if (String.IsNullOrEmpty(data))
                return;

            while (ConnectionStatus != ConnectionStatus.Connected && !Program.IsDebugMode)
            {

            }

            if (TransmissionType == TransmissionType.Hex)
            {
                data = InterfaceHelper.AsciiStringToHexString(data);
            }

            UiMediator.AddUiMessage(analyzer.instrumentid, (int)MessageType.Outgoing, data /*+ "\n"*/);
            MessageLogger.LogCommunication(data, "LIS");

            OnWriteData(data, displayData);

            if (AcknowledgePartialFrame == true)
                return;

            AfterWriteData(data, displayData);

            if (SupportsHandshake)
            {
                if (data == Characters.ENQ || data == ACK_Message)
                {
                    //IsCommunicationIdle = false;

                    if (data == Characters.ENQ)
                    {
                        //Sender = "LIS"; //sender to be changed only if ENQ acknowledged by instrument
                        IsAwaitingACK = true;
                    }
                    else if (data == ACK_Message)
                    {
                        Sender = "Instrument";
                        if (SupportsHandshake)
                        {
                            IsCommunicationIdle = false;
                        }
                        //IsAwaitingFrame = true;
                    }
                }
                else if (data == Characters.EOT)
                {
                    //MainForm.BlockClosingApp_Reason = "";
                    IsCommunicationIdle = true;
                    Sender = "";
                }
                else
                {
                    IsAwaitingACK = true;
                }
            }
        }
        protected abstract void OnWriteData(string data, bool displayData = false);
        private void AfterWriteData(string data, bool displayData = false)
        {

            if (data == Characters.ENQ)

                if (displayData)
                {
                    //UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Outgoing, data /*+ "\n"*/);
                }
        }
        internal abstract void DiscardInOutBuffer();

        public virtual void ProcessResponse(Analyzer analyzer, string ReceivedData)
        {
            try
            {
                AwaitTimeoutCounts = 0;

                ResponseTimer.Stop();

                if (AcknowledgePartialFrame)
                {
                    WriteData(ACK_Message, displayData: false); // Sending immediate ACK for partial frame
                    //Thread.Sleep(100);

                    if (!ReceivedData.EndsWith(Characters.ETX))
                    {
                        currentFrame = currentFrame + ReceivedData;
                        return;
                    }
                }

                ReceivedData = ReceivedData.TrimStart('\t');

                ////in case CRLF comes inbetween, split the string to process separately
                string receivedData = ReceivedData;
                string frame_DataPart = string.Empty;

                if (receivedData != "")
                {
                    FrameMessageType frameMessageType = DetectFrameMessageType(receivedData);
                    TextLogger.WriteLogEntry("Debugging", "frameMessageType:" + frameMessageType);

                    if (frameMessageType == FrameMessageType.ACK)
                    {
                        if (!IsAwaitingACK)
                        {
                            //code added to handle delayed ACK sent by Centralink after no reply to ENQ sent by LIS and afterwards EOT sent by LIS
                            TerminateCommunication(true);
                            return;
                        }

                        if (SupportsHandshake)
                        {
                            IsCommunicationIdle = false;
                        }
                        HandshakeAttemptWatch.Reset();
                        IsAwaitingACK = false;
                        HandshakeAttempts = 0;
                        swENQContention.Reset();

                        //After 3 NAK by instrument, it sends DC1 and then LIS need to skip current and send next WO
                        DiscardInOutBuffer();

                        Sender = "LIS";
                        UiMediator.AddUiMessage(analyzer.instrumentid, (int)MessageType.Incoming, receivedData);
                        //MessageLogger.LogCommunication(receivedData, analyzer.AnalyzerName);
                        MessageLogger.LogCommunication(receivedData, analyzer.instrumentname);

                        currentFrameSendRetryCount = 0;
                        lastFrameSent = string.Empty;

                        //send one frame from available frames prepared for sending, and return to wait for response
                        if (currentFrames != null && currentFrames.Any())
                        {
                            SendPendingFrames();
                            return;
                        }
                        else
                        {
                            if (currentFramesStartingCount > 0)
                            {
                                UiMediator.AddUiMessage(analyzer.instrumentid, (int)MessageType.Outgoing, "Finished sending " + currentFramesStartingCount + " records.");
                            }

                            TerminateCommunication();

                            double timeToWait = 0;

                            //wait to check if analyzer sends ENQ
                            timeToWait = 100;

                            if (IsCommunicationIdle)
                            {
                                InitiateSendPendingFrames(timeToWait);
                            }
                        }
                    }
                    else if (frameMessageType == FrameMessageType.NAK && msgConfig.AnalyzerTypeID != AnalyzerTypes.Bechman)
                    {
                        DiscardInOutBuffer();

                        UiMediator.AddUiMessage(analyzer.instrumentid, (int)MessageType.Warning, receivedData + "NAK Responded by Analyzer.." /*+ "\n"*/);
                        MessageLogger.LogCommunication(receivedData, analyzer.instrumentname);

                        if (currentFrameSendRetryCount < FrameSendMaxRetryCount)
                        {
                            currentFrameSendRetryCount++;
                            //re-send last frame
                            WriteData(lastFrameSent, true);
                        }
                        else
                        {
                            TerminateCommunication(true);
                        }
                    }
                    else if (frameMessageType == FrameMessageType.ENQ)
                    {
                        DiscardInOutBuffer();

                        //in case ENQ or EOT comes inbetween, split the string to process separately
                        string[] arr = System.Text.RegularExpressions.Regex.Split(receivedData, @"(?<=[" + Convert.ToChar(Characters.ENQ) + Convert.ToChar(Characters.EOT) + "])");
                        string rightPart = "";
                        string receivedData1 = arr[0];
                        if (arr.Length > 1)
                        {
                            rightPart = arr[1];
                        }

                        UiMediator.AddUiMessage(analyzer.instrumentid, (int)MessageType.Incoming, receivedData1);
                        MessageLogger.LogCommunication(receivedData1, analyzer.instrumentname);


                        if (swENQContention.IsRunning /*&& swENQContention.ElapsedMilliseconds < 1000*/)
                        {
                            //Do nothing for first ENQ clash, and wait for next ENQ
                            swENQContention.Stop();
                            IsAwaitingACK = false; //reset flag to return to Neutral state
                            return;
                        }

                        if (!String.IsNullOrEmpty(ACK_Message))
                        {
                            WriteData(ACK_Message, displayData: false);
                        }

                        ClearVariables_Incoming(clearInMemoryFrameData: false);
                        //}

                        if (rightPart != "")
                        {
                            ProcessResponse(analyzer, rightPart);
                        }
                    }
                    else if (frameMessageType == FrameMessageType.FrameEnd)
                    {
                        DiscardInOutBuffer();

                        //Advia 1800 - multiple lines of just [CR][LF] characters observed
                        if (currentFrame == "" && receivedData == (Characters.CR + Characters.LF))
                        {
                            return;
                        }


                        currentFrame = currentFrame + receivedData;

                        UiMediator.AddUiMessage(analyzer.instrumentid, (int)MessageType.Incoming, currentFrame);

                        //MessageLogger.LogCommunication(currentFrame, analyzer.instrumentname);

                        string NAK_UserMessage = string.Empty;
                        bool isFinalFrame = false;

                        if (frameHandler.SupportsFraming && frameHandler.ExtractFrameNumber(currentFrame) != CommunicationFrameHandler.GetNextFrameNumber(lastFrameNumber))
                            NAK_UserMessage = "Frame number mismatch";
                        else if (/*frameHandler.SupportsChecksum &&*/ !frameHandler.VerifyChecksumAndExtractData(currentFrame, out isFinalFrame, out frame_DataPart))
                            NAK_UserMessage = "Checksum mismatch";
                        else
                        {
                            NAK_UserMessage = string.Empty;
                            completeFrame_DataPart += frame_DataPart;

                            if (isFinalFrame || (!frameHandler.SupportsFraming && (receivedData.EndsWith(Characters.ETX) || receivedData.EndsWith(Characters.ETB))))
                            {
                                completeMessage.AddRange(completeFrame_DataPart.Split(new char[] { Convert.ToChar(Characters.CR), Convert.ToChar(Characters.LF), Convert.ToChar(Characters.STX) }, StringSplitOptions.RemoveEmptyEntries).Where(r => r != "").Select(r => r.Replace(Characters.ETX, "").Replace(Characters.ETB, "")));
                                completeFrame_DataPart = string.Empty;
                            }
                            if (SupportsAckNak)
                            {

                                WriteData(ACK_Message, displayData: false);


                            }
                            lastFrameNumber = CommunicationFrameHandler.GetNextFrameNumber(lastFrameNumber);

                        }
                        currentFrame = string.Empty;

                        //EOT Sending after result.
                        if ((!SupportsHandshake && receivedData.EndsWith(Characters.ETX))
                           /*|| (analyzer.AnalyzerTypeId == AnalyzerTypes.LABUMAT)*/)
                            PrepareForProcessing(receivedData);

                        if (!String.IsNullOrEmpty(NAK_UserMessage))
                        {
                            UiMediator.AddUiMessage(analyzer.instrumentid, (int)MessageType.Warning, NAK_Message + " (" + NAK_UserMessage + ").");
                            WriteData(NAK_Message, displayData: false);
                        }

                        if (!SupportsHandshake && receivedData.EndsWith(Characters.ETX))
                            PrepareForProcessing(receivedData);
                    }
                    else if (frameMessageType == FrameMessageType.EOT)
                    {
                        //System.Threading.Thread.Sleep(50);
                        //WriteData(Characters.ENQ);
                        UiMediator.AddUiMessage(analyzer.instrumentid, (int)MessageType.Incoming, receivedData);
                        MessageLogger.LogCommunication(currentFrame == "" ? receivedData : currentFrame, analyzer.instrumentname);

                        if (!IsCommunicationIdle && Sender == "Instrument")
                        {
                            IsCommunicationIdle = true;
                            Sender = "";

                            PrepareForProcessing(receivedData);
                        }
                        else if (/*!IsCommunicationIdle &&*/ Sender == "LIS") //Centaur case interrupt by analyzer: positive acknowledgement with interrupt
                        {
                            IsCommunicationIdle = true;
                            Sender = "";

                            TerminateCommunication(true);

                            if (IsCommunicationIdle)
                            {
                                ////wait to check if analyzer sends ENQ
                                //System.Threading.Thread.Sleep(5000);

                                InitiateSendPendingFrames(5000);
                            }
                        }
                    }
                    else if (frameMessageType == FrameMessageType.IncompleteFramePart)
                    {
                        currentFrame = currentFrame + receivedData;
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(analyzer.instrumentid, ex, "Error in reading analyzer message");
                //terminate communication
                //WriteData(Characters.EOT);
                ClearVariables_Incoming();
                //ClearVariables_Outgoing(true);
                ////TerminateCommunication(true); //commented as giving error in Centralink due to connection closure inbetween
            }
        }


        // HL7 Protocol Communication decoding
        public virtual void ProcessResponseHL7(Analyzer analyzer, string ReceivedData)
        {
            try
            {
                AwaitTimeoutCounts = 0;
                ResponseTimer.Stop();

                ReceivedData = ReceivedData.TrimStart('\t');

                string receivedData = ReceivedData;
                string frame_DataPart = string.Empty;
                string pendingFrame = string.Empty;
                string[] splitMessages = Regex.Split(receivedData, @"\x1C\r");
                List<string> hl7FramesToProcess = new List<string>();

                if (!string.IsNullOrEmpty(receivedData))
                {
                    // Special handling for Zybio_EXZ_6000_H6 which may send multiple messages at once
                    if (analyzer.instrumentgroupid == AnalyzerTypes.Zybio_EXZ_6000_H6 && splitMessages.Length > 1)
                    {
                        foreach (string frame in splitMessages)
                        {
                            if (string.IsNullOrWhiteSpace(frame))
                                continue;

                            string framed = frame.Trim() + "\x1C\r"; // Add VT prefix

                            hl7FramesToProcess.Add(framed);

                        }
                    }
                    else
                    {
                        hl7FramesToProcess.Add(receivedData);
                    }

                    foreach (var message in hl7FramesToProcess)
                    {
                        FrameMessageTypeHL7 frameMessageType = DetectFrameMessageTypeHL7(message);
                        TextLogger.WriteLogEntry("Debugging", $"frameMessageType: {frameMessageType}");

                        if (frameMessageType == FrameMessageTypeHL7.EOT)
                        {
                            //TerminateCommunication(true);
                            continue;
                        }

                        if (frameMessageType == FrameMessageTypeHL7.IncompleteFramePart && analyzer.instrumentgroupid == AnalyzerTypes.Zybio_EXZ_6000_H6)
                        {
                            if (message.EndsWith(Characters.FS + Characters.CR))
                            {
                                pendingFrame = message;
                            }
                            if (message.EndsWith(Characters.FS))
                            {
                                pendingFrame = message;
                            }
                            else
                            {
                                currentFrame = currentFrame + message;
                                receivedData = string.Empty;
                            }
                        }

                        if (!string.IsNullOrEmpty(pendingFrame) && analyzer.instrumentgroupid == AnalyzerTypes.Zybio_EXZ_6000_H6)
                        {
                            receivedData = currentFrame + pendingFrame;
                            pendingFrame = string.Empty;
                            currentFrame = string.Empty;
                        }

                        if (!string.IsNullOrEmpty(currentFrame) && analyzer.instrumentgroupid == AnalyzerTypes.Zybio_EXZ_6000_H6)
                        {
                            return;
                        }


                        if (receivedData.Contains("MSH"))
                        {
                            Sender = "LIS";
                            UiMediator.AddUiMessage(analyzer.instrumentid, (int)MessageType.Incoming, receivedData);
                            MessageLogger.LogCommunication(receivedData, analyzer.instrumentname);

                            currentFrameSendRetryCount = 0;
                            lastFrameSent = string.Empty;

                            currentFrame += receivedData;
                            completeFrame_DataPart += frame_DataPart;

                            completeMessage.AddRange(currentFrame
                                .Split(new char[] { Convert.ToChar(Characters.CR), Convert.ToChar(Characters.LF) }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(r => !string.IsNullOrWhiteSpace(r))
                                .Select(r => r.Replace(Characters.ETX, "").Replace(Characters.ETB, ""))
                            );

                            completeFrame_DataPart = string.Empty;

                            PrepareForProcessingHL7(receivedData);

                            if (currentFrame.Any())
                            {
                                WriteData(currentFrame, true);
                            }

                            currentFrame = string.Empty; // Reset after processing
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(analyzer.instrumentid, ex, "Error in reading analyzer message");
                ClearVariables_Incoming();
            }
        }

        private FrameMessageType DetectFrameMessageType(string receivedData)
        {
            FrameMessageType frameMessageType = FrameMessageType.IncompleteFramePart;

            if (frameMessageType != FrameMessageType.IncompleteFramePart)
                return frameMessageType;

            if ((!String.IsNullOrEmpty(ACK_Message) && (receivedData == ACK_Message || receivedData.StartsWith(ACK_Message)))
                    || receivedData == Characters.DC1)
            {
                frameMessageType = FrameMessageType.ACK;
            }

            else if (!String.IsNullOrEmpty(NAK_Message) && receivedData == NAK_Message)
            {
                frameMessageType = FrameMessageType.NAK;
            }
            else if (receivedData == Characters.ENQ || receivedData.StartsWith(Characters.ENQ))
            {
                frameMessageType = FrameMessageType.ENQ;
            }
            else if (receivedData.EndsWith(Characters.LF) || (!frameHandler.SupportsFraming && (receivedData.EndsWith(Characters.ETB) || receivedData.EndsWith(Characters.ETX))))
            {
                frameMessageType = FrameMessageType.FrameEnd;
            }
            else if (receivedData == Characters.EOT || receivedData.StartsWith(Characters.EOT)
                            || receivedData.EndsWith(Characters.STX + "DB" + Characters.ETX) //added this condition for Olympus)
                )
            {
                frameMessageType = FrameMessageType.EOT;
            }
            else
            {
                frameMessageType = FrameMessageType.IncompleteFramePart;
            }
            return frameMessageType;
        }


        // Created Extension logic for HL7 machines
        private FrameMessageTypeHL7 DetectFrameMessageTypeHL7(string receivedData)
        {
            FrameMessageTypeHL7 frameMessageType = FrameMessageTypeHL7.IncompleteFramePart;

            if (frameMessageType != FrameMessageTypeHL7.IncompleteFramePart)
                return frameMessageType;

            if ((!String.IsNullOrEmpty(ACK_Message) && (receivedData == ACK_Message || receivedData.StartsWith(ACK_Message))))
            {
                //ESU^U01^ESU_U01
                frameMessageType = FrameMessageTypeHL7.ACK;
            }

            else if (!String.IsNullOrEmpty(NAK_Message) && receivedData == NAK_Message)
            {
                frameMessageType = FrameMessageTypeHL7.NAK;
            }
            else if (receivedData == Characters.ENQ || receivedData.StartsWith(Characters.ENQ))
            {
                frameMessageType = FrameMessageTypeHL7.ENQ;
            }
            else if (receivedData.EndsWith(Characters.LF) || (!frameHandler.SupportsFraming && (receivedData.EndsWith(Characters.ETB) || receivedData.EndsWith(Characters.ETX))))
            {
                frameMessageType = FrameMessageTypeHL7.FrameEnd;
            }
            else if (receivedData == Characters.EOT || receivedData.StartsWith(Characters.EOT)
                            || receivedData.EndsWith(Characters.STX + "DB" + Characters.ETX) //added this condition for Olympus)
                )
            {
                frameMessageType = FrameMessageTypeHL7.EOT;
            }
            else if (receivedData.Contains("ACK^Q03") || receivedData.Contains("ACK^Q02"))
            {
                frameMessageType = FrameMessageTypeHL7.EOT;
            }
            else
            {
                frameMessageType = FrameMessageTypeHL7.IncompleteFramePart;
            }

            return frameMessageType;
        }


        private void PrepareForProcessing(string receivedData)
        {
            //clean variables of outgoing message first, in case EOT received to inturrent ongoing communication
            lastFrameSent = string.Empty;
            //responseFrames = null;

            string rightPart = "";
            //in case EOT comes inbetween, split the string to process separately
            string[] arr = receivedData.Split(Convert.ToChar(Characters.EOT));
            currentFrame += arr[0] + Characters.EOT;
            if (arr.Length > 1)
            {
                rightPart = arr[1];
            }

            //MessageLogger.LogCommunication(currentFrame, analyzer.Analyzer_Name);

            if (rightPart.Length == 0)
            {
                //string WOBarcode_Single = "";
                List<string> responseRecords = null;

                //System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() => {
                //if(completeMessage.Count!=0)
                responseRecords = new MessageProcessor(analyzer.instrumentid).ProcessCompleteRecord(completeMessage/*, out WOBarcode_Single*/);
                //WritePendingWOFile(responseRecords, WOBarcode_Single);
                //if (completeMessage.Count != 0)
                    TextLogger.WriteLogEntry("Debugging", responseRecords.Count + " records generated.");
                
                ClearVariables_Incoming();

                if (responseRecords != null && responseRecords.Any())
                {
                    SendRecords(responseRecords/*, WOBarcode_Single*/);
                }
                //if no current frames to send, attempt earlier pending frames if any
                
                else if (responseRecords.Count>0 && responseFramesGuid.Any())
                {
                    InitiateSendPendingFrames(100);
                }
                return;
            }
            else
            {
                //for next message session, reset frame number, and remove ENQ so that other variables will not get reset
                ClearVariables_Incoming(clearInMemoryFrameData: false);
                //rightPart = rightPart.TrimStart(Convert.ToChar(Characters.ENQ));
                ProcessResponse(analyzer, rightPart);
            }
        }

        private void PrepareForProcessingHL7(string receivedData)
        {
            //clean variables of outgoing message first, in case EOT received to inturrent ongoing communication
            lastFrameSent = string.Empty;

            string rightPart = "";
            //in case EOT comes inbetween, split the string to process separately
            string[] arr = receivedData.Split(Convert.ToChar(Characters.CR));
            currentFrame += arr[0] + Characters.CR;

            if (analyzer.instrumentgroupid == AnalyzerTypes.Zybio_EXZ_6000_H6 && arr.Length >= 1)
            {
                rightPart = arr[0];
            }

            if (arr.Length > 1)
            {
                rightPart = arr[0];
            }

            if (rightPart.Length != 0)
            {
                //string WOBarcode_Single = "";
                List<string> responseRecords = null;

                //System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() => {
                responseRecords = new MessageProcessor(analyzer.instrumentid).ProcessCompleteRecordHL7(completeMessage/*, out WOBarcode_Single*/);
                //WritePendingWOFile(responseRecords, WOBarcode_Single);
                TextLogger.WriteLogEntry("Debugging", responseRecords.Count + " records generated.");

                ClearVariables_Incoming();

                if (responseRecords != null && responseRecords.Any())
                {
                    SendRecordsHL7(responseRecords/*, WOBarcode_Single*/);
                }
                //if no current frames to send, attempt earlier pending frames if any
                else if (responseFramesGuid.Any())
                {
                    InitiateSendPendingFrames(100);
                }
                return;
            }
            else
            {
                //for next message session, reset frame number, and remove ENQ so that other variables will not get reset
                ClearVariables_Incoming(clearInMemoryFrameData: false);
                //rightPart = rightPart.TrimStart(Convert.ToChar(Characters.ENQ));
                ProcessResponse(analyzer, rightPart);
            }
        }

        internal void SendRecordsHL7(List<string> responseRecords, string WoFileGuid = "")
        {
            //new System.Threading.Thread(() => //Commented threading because giving I/O error in serial communication
            //{
            //MessageLogger.LogCommunication(IsCommunicationIdle.ToString(), "testing");

            if (responseRecords != null && responseRecords.Any())
            {
                currentFrames = frameHandler.PrepareFramesHL7(responseRecords);
                //List<string> frames = frameHandler.PrepareFrames(responseRecords);
                //responseFrames.Enqueue(frames);
                bool hasExistingGuid = true;
                if (WoFileGuid == "")
                {
                    WoFileGuid = Guid.NewGuid().ToString();
                    hasExistingGuid = false;
                }

                if (!responseFramesGuid.Contains(WoFileGuid))
                {
                    responseFramesGuid.Enqueue(WoFileGuid);
                    TextLogger.WriteLogEntry("Debugging", "Enqueued response frames file guid (SendRecords)" + WoFileGuid);
                }

                if (!hasExistingGuid)
                {
                    WritePendingWOFile(responseRecords, WoFileGuid);
                    TextLogger.WriteLogEntry("Debugging", "WoFileGuid: " + WoFileGuid + ", Written WO file");
                }
            }

            TextLogger.WriteLogEntry("Debugging", "WoFileGuid: " + WoFileGuid + ", IsAwaitingACK: " + IsAwaitingACK);
            TextLogger.WriteLogEntry("Debugging", "WoFileGuid: " + WoFileGuid + ", IsCommunicationIdle: " + IsCommunicationIdle);

            //if earlier communication is in-progress, do not proceed to send current WO, wait earlier to finish
            //IsCommunicationIdle would be always true for analyzers that do not support Handshake and send all communication in single frame, so added condition to exclude
            while (IsAwaitingACK || (SupportsHandshake && !IsCommunicationIdle))
                return;

            //CurrentResponseTimeout = 15000;
            if (responseFramesGuid.Any()) //responseFrames.Any())
            {
                TextLogger.WriteLogEntry("Debugging", "WoFileGuid: " + WoFileGuid + ", Calling InitiateSendPendingFrames to send records");
                InitiateSendPendingFrames();
            }

            //while (SupportsHandshake && IsCommunicationIdle)
            //{
            //    if(!HandshakeAttemptWatch.IsRunning)
            //        continue;

            //    //Wait 20 seconds after ENQ and resend if not received response (XN 9000)
            //    if (HandshakeAttemptWatch.IsRunning && HandshakeAttemptWatch.Elapsed.TotalSeconds <= 5 /*20*/)
            //    {
            //        if (HandshakeAttemptWatch.Elapsed.TotalMilliseconds % 1000 == 0)
            //        {
            //            UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Warning, "Awaiting reply ...");
            //        }
            //    }
            //    else
            //    {
            //        //System.Windows.Forms.MessageBox.Show(HandshakeAttemptWatch.Elapsed.TotalSeconds.ToString());
            //        //    HandshakeAttemptWatch.Reset();

            //        ////System.Windows.Forms.MessageBox.Show(IsCommunicationIdle.ToString());
            //        //if(analyzer.AnalyzerTypeId == 203 || analyzer.AnalyzerTypeId == 24)
            //        //{
            //        //    MainForm.RestartApplication();
            //        //}
            //        //InitiateSendPendingFrames();
            //        if (HandshakeAttempts >= 3)
            //        {
            //            HandshakeAttempts = 0;
            //            HandshakeAttemptWatch.Reset();
            //            UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "No reply from analyzer, attempting connection reset");
            //            ClosePort();
            //            OpenPort(ConfigParameters);
            //        }

            //        InitiateSendPendingFrames();
            //    }
            //}
            //}).Start();
        }

        internal void SendRecords(List<string> responseRecords, string WoFileGuid = "")
        {
            //new System.Threading.Thread(() => //Commented threading because giving I/O error in serial communication
            //{
            //MessageLogger.LogCommunication(IsCommunicationIdle.ToString(), "testing");

            if (responseRecords != null && responseRecords.Any())
            {
                currentFrames = frameHandler.PrepareFrames(responseRecords);
                //List<string> frames = frameHandler.PrepareFrames(responseRecords);
                //responseFrames.Enqueue(frames);
                bool hasExistingGuid = true;
                if (WoFileGuid == "")
                {
                    WoFileGuid = Guid.NewGuid().ToString();
                    hasExistingGuid = false;
                }

                if (!responseFramesGuid.Contains(WoFileGuid))
                {
                    responseFramesGuid.Enqueue(WoFileGuid);
                    TextLogger.WriteLogEntry("Debugging", "Enqueued response frames file guid (SendRecords)" + WoFileGuid);
                }

                if (!hasExistingGuid)
                {
                    WritePendingWOFile(responseRecords, WoFileGuid);
                    TextLogger.WriteLogEntry("Debugging", "WoFileGuid: " + WoFileGuid + ", Written WO file");
                }
            }

            TextLogger.WriteLogEntry("Debugging", "WoFileGuid: " + WoFileGuid + ", IsAwaitingACK: " + IsAwaitingACK);
            TextLogger.WriteLogEntry("Debugging", "WoFileGuid: " + WoFileGuid + ", IsCommunicationIdle: " + IsCommunicationIdle);

            //if earlier communication is in-progress, do not proceed to send current WO, wait earlier to finish
            //IsCommunicationIdle would be always true for analyzers that do not support Handshake and send all communication in single frame, so added condition to exclude
            while (IsAwaitingACK || (SupportsHandshake && !IsCommunicationIdle))
                return;

            //CurrentResponseTimeout = 15000;
            if (responseFramesGuid.Any()) //responseFrames.Any())
            {
                TextLogger.WriteLogEntry("Debugging", "WoFileGuid: " + WoFileGuid + ", Calling InitiateSendPendingFrames to send records");
                InitiateSendPendingFrames();
            }
        }

        public void InitiateBulkWOSend()
        {
            //keep single instance of function running continuously, and without clashing with call to InitiateSendPendingFrames after finishing previous communication (ACK received)
            VSoftLISMAIN.ScheduleMethodInThread(AttemptPendingWO, TimeSpan.FromSeconds(10));
        }

        private void AttemptPendingWO()
        {
            //if (connectionSettings_WO.IsFileBased)
            //    return;

            //attempt previous pending communications if any
            string directoryPath = System.IO.Path.Combine(VSoftLISMAIN.ApplicationDataFolder, "PendingWOCommunications");
            if (System.IO.Directory.Exists(directoryPath))
            {
                foreach (var file in System.IO.Directory.GetFiles(directoryPath))
                {
                    string guid = System.IO.Path.GetFileNameWithoutExtension(file);
                    if (!responseFramesGuid.Contains(guid))
                    {
                        responseFramesGuid.Enqueue(guid);
                        TextLogger.WriteLogEntry("Debugging", "Enqueued response frames file guid (AttemptPendingWO)" + guid);
                    }
                }
            }

            //UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Normal, "Loop end AttemptPendingWO()");

            TextLogger.WriteLogEntry("Debugging", "Starting InitiateSendPendingFrames from AttemptPendingWO.");
            InitiateSendPendingFrames();
        }

        private bool ResponseMonitor()
        {
            return true;
        }

        internal static void WritePendingWOFile(List<string> responseRecords, string filename = "")
        {
            if (responseRecords != null && responseRecords.Any())
            {
                filename = !String.IsNullOrEmpty(filename) ? filename : /*DateTime.Now.ToString("yyyyMMdd_HHmmss_fff_") +*/ Guid.NewGuid().ToString();

                string communicationFilePath = PreparePendingWoFilename(filename);
                //responseFramesGuid.Enqueue(WoFileGuid);
                TextLogger.WriteTextFile(communicationFilePath, String.Join(Environment.NewLine, responseRecords), true);
            }
        }

        internal static List<string> ReadPendingWOFile(string filename)
        {
            return System.IO.File.ReadAllLines(PreparePendingWoFilename(filename)).ToList();
        }

        internal void DeletePendingWOFileAndDequeue()
        {
            //implement lock to avoid concurrent delete and load of same Guid
            lock (responseFramesGuid)
            {
                DeletePendingWOFile(ConcurrentPeek(responseFramesGuid));
                ConcurrentDequeue(responseFramesGuid);
            }
        }

        internal void DeletePendingWOFile(string filename)
        {
            try
            {
                //System.IO.File.Move(PreparePendingWoFilename(filename), System.IO.Path.Combine(MainForm.ApplicationDataFolder, "PendingWOCommunications_BACKUP", filename + ".txt"));
                System.IO.File.Delete(PreparePendingWoFilename(filename));
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(analyzer.instrumentid, ex, "Unable to delete file: " + filename);
            }
        }

        private static string PreparePendingWoFilename(string filenameWithoutExtension)
        {
            return System.IO.Path.Combine(VSoftLISMAIN.ApplicationDataFolder, "PendingWOCommunications", filenameWithoutExtension + ".txt");
        }

        internal async Task InitiateSendPendingFrames(double MinimumWaitTimeToSendEnq_MS = 0)
        {
            if (!IsCommunicationIdle)
                return;

            await WaitEnqAsync(MinimumWaitTimeToSendEnq_MS);

            TextLogger.WriteLogEntry("Debugging", "Inside InitiateSendPendingFrames after await WaitEnqAsync");

            if (responseFramesGuid == null || !responseFramesGuid.Any())
                return;

            if (currentFrames == null || !currentFrames.Any() && IsCommunicationIdle)
            {
                LoadResponseFrames();

                if (currentFrames == null || !currentFrames.Any())
                    return;
            }

            if (SupportsHandshake)
            {
            attempt:
                //System.Threading.Thread.Sleep(200); //temporarily added for Cobas to avoid ENQ clash with results

                /*while*/

                if (!IsCommunicationIdle || IsAwaitingACK)
                    return;

                //write ENQ and then write frames after receiving ACK for each data sent
                WriteData(Characters.ENQ);
                HandshakeAttemptWatch.Restart();
                HandshakeAttempts++;
                swENQContention.Restart();

                //if (AwaitResponse(3) == false)
                //    goto attempt;

                //if (ResponseMonitor() == false)
                //    goto attempt;

                //objResponseMonitor = Task.Run(() => ResponseMonitor());
                //objResponseMonitor.Wait();

                //while (!IsCommunicationIdle)
                //{
                //    Thread.Sleep(100);
                //}
            }
            else
            {
                SendPendingFrames();
            }

            if (SupportsAckNak || (analyzer.instrumentgroupid == 41 && !String.IsNullOrEmpty(lastFrameSent) && !lastFrameSent.Contains("MSA")))
            {
                //CommTimer tmrComm = new CommTimer();
                //tmrComm.Start();
                ResponseTimer.Start();
                //UiMediator.AddUiMessage(analyzer.AnalyzerId, (int)MessageType.Error, "awaiting reply.... (Last frame: " + lastFrameSent + ")");
            }
        }

        private async Task WaitAsync(double WaitTime_MS = 0)
        {
            await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(Convert.ToInt32(WaitTime_MS/*WaitTimeToSendEnq*/));
            });
        }

        private async Task<bool> WaitEnqAsync(double MinimumWaitTimeToSendEnq_MS = 0)
        {
            //implemented asynchronous wait delay, so that TCP can listen to analyzer without getting blocked
            await WaitAsync(MinimumWaitTimeToSendEnq_MS);

            return true;
        }

        private void ResponseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ResponseTimer.Stop();
            AwaitTimeoutCounts++;

            if (!IsAwaitingACK)
                return;

            if (AwaitTimeoutCounts >= MaxAwaitTimeoutAttempts)
            {
                AwaitTimeoutCounts = 0;
                UiMediator.AddUiMessage(analyzer.instrumentid, (int)MessageType.Error, "No reply from analyzer."); //", attempting connection reset"
                TerminateCommunication(true);
                System.Threading.Thread.Sleep(15 * 1000);
            }
            else
            {
                if (SupportsHandshake && String.IsNullOrEmpty(lastFrameSent))
                {
                    WriteData(Characters.ENQ);
                    ResponseTimer.Start();
                    swENQContention.Restart();
                }
                else if (!String.IsNullOrEmpty(lastFrameSent))
                {
                    WriteData(lastFrameSent);
                    ResponseTimer.Start();
                }
            }
        }

        internal void TerminateCommunication(bool isAbnormalTermination = false)
        {
            if (SupportsHandshake)
                WriteData(Characters.EOT);
            ClearVariables_Outgoing(!isAbnormalTermination);
        }

        private void ClearVariables_Incoming(bool clearInMemoryFrameData = true)
        {
            lastMessage = string.Empty;
            currentFrame = string.Empty;
            lastFrameNumber = 0;
            completeFrame_DataPart = string.Empty;
            if (clearInMemoryFrameData)
            {
                completeMessage.Clear();
            }
        }

        private void ClearVariables_Outgoing(bool removeCurrentFrames)
        {
            lastFrameSent = string.Empty;
            currentFrames = null; //cleared frames from memory, so that reattmpt will be done for entire set starting from first frame
            currentFramesStartingCount = 0;
            if (removeCurrentFrames && responseFramesGuid.Any() /*responseFrames.Any()*/)
            {
                DeletePendingWOFileAndDequeue();
            }
            //responseFrames.Clear();
            currentFrameSendRetryCount = 0;
            IsAwaitingACK = false;
        }

        private void SendPendingFrames()
        {
        sendFrame:
            //if (responseFrames != null && responseFrames.Any())
            if (responseFramesGuid != null && responseFramesGuid.Any())
            {
                if (currentFrames.Any())
                {
                    WriteData(currentFrames[0], true);
                    lastFrameSent = currentFrames[0];
                    currentFrames.RemoveAt(0);

                    //AwaitResponse(FrameSendMaxRetryCount);

                    if (!SupportsAckNak && responseFramesGuid.Count > 0 /* responseFrames.Count > 0*/)
                    {
                        System.Threading.Thread.Sleep(50);
                        goto sendFrame;
                    }
                }
            }
        }

        private void LoadResponseFrames()
        {
            lock (responseFramesGuid)
            {
                if (responseFramesGuid.Any())
                {
                    string guid = ConcurrentPeek(responseFramesGuid);
                    TextLogger.WriteLogEntry("Debugging", "Loading response frames file guid" + guid);
                    TextLogger.WriteLogEntry("Debugging", Environment.StackTrace);
                    currentFrames = frameHandler.PrepareFrames(ReadPendingWOFile(guid));
                    currentFramesStartingCount = currentFrames.Count();
                }
            }
        }

        //https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentqueue-1?view=netcore-3.1
        private string ConcurrentPeek(ConcurrentQueue<string> concurrentQueue)
        {
            string result;
            while (!concurrentQueue.TryPeek(out result))
            {
                System.Threading.Thread.Sleep(1);
            }
            return result;
        }

        private string ConcurrentDequeue(ConcurrentQueue<string> concurrentQueue)
        {
            string result;

            while (!concurrentQueue.TryDequeue(out result))
            {
                System.Threading.Thread.Sleep(1);
            }
            TextLogger.WriteLogEntry("Debugging", "Removed response frames file guid" + result);
            return result;
        }


        protected virtual void OnConnectionStatusChanged(EventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, e);
        }

        public event EventHandler ConnectionStatusChanged;
    }

    public enum ConnectionStatus
    {
        Disconnected,
        Connected,
        Listening
    }
    public enum FrameMessageType
    {
        IncompleteFramePart,
        ENQ,
        ACK,
        NAK,
        FrameEnd,
        EOT
    }

    public enum FrameMessageTypeHL7
    {
        IncompleteFramePart,
        ENQ,
        ACK,
        NAK,
        FrameEnd,
        EOT,
        QBP, Q11,
        RSP, K11,
        OML, O33,
        ORL, O34,
        OUL, R22,
        EAC, U07,
        R23,
        ESU, U01,
        INU, U05,
        INR, U14,
        ESR, U02
    }

}
