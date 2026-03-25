using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using VSoftLIS_Interface.Common;
using System.Net;
using System.IO;
using System.Threading;

namespace VSoftLIS_Interface.DLL
{
    internal class TCPCommunication : CommunicationBase
    {
        ErrorLog errorLog = new ErrorLog();
        NetworkStream nwStream = null;
        TcpListenerEx server = null;
        TcpClient tcpClient = null;
        Thread TcpThread = null;
        bool isTcpOpen = false;


        public TCPCommunication(Analyzer analyzer, DataReceivedCallback DataReceived) : base(analyzer, DataReceived)
        {

        }

        internal override bool IsOpen
        {
            get 
            {
                return isTcpOpen;
            }
        }
        internal override ConnectionStatus GetConnectionStatus()
        {
            return ConnectionStatus;
        }

        protected override void OnWriteData(string data, bool displayData = false)
        {
            //if (!Program.IsDebugMode)
            //{
            if (nwStream == null)
                return;

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(data);
            nwStream.Write(ba, 0, ba.Length);
            //}
        }

        internal override bool ClosePort()
        {
            //if (tcpClient != null && client.Connected)
            //    tcpClient.Close();

            isTcpOpen = false;

            //Tcpthread.abort();
            DisposeObjects();

            return true;
        }

        internal override void DiscardInOutBuffer()
        {

        }

        internal override bool OpenPort(ConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings;

            isTcpOpen = true;

            if (this.TcpThread != null && this.TcpThread.IsAlive)
                return true;

            this.TcpThread = new Thread(() =>
            {
                while (true)
                {
                    if (!isTcpOpen)
                    {
                        Thread.Sleep(100);

                        //if (tcpClient != null)
                        //    DisposeObjects();

                        continue;
                    }

                    try
                    {
                        string hostIPAddress = "";
                        if (!String.IsNullOrEmpty(connectionSettings.TCP_IPAddress))
                            hostIPAddress = connectionSettings.TCP_IPAddress;
                        int hostPort = Convert.ToInt32(connectionSettings.TCP_PortNumber);

                        if (tcpClient == null)
                        {
                            //if (hostIPAddress == "" || hostIPAddress == "127.0.0.1" || hostIPAddress == Program.LocalIpAddress)
                            if (connectionSettings.TCP_IsServerMode)
                            {
                                IPAddress localAddr = null;
                                if (hostIPAddress == "")
                                    localAddr = IPAddress.Any;
                                else
                                    localAddr = IPAddress.Parse(hostIPAddress);

                                server = new TcpListenerEx(localAddr, hostPort);
                                server.Start();
                                ConnectionStatus = ConnectionStatus.Listening;

                                BLL.MessageLogger.WriteTimeDiffLog("TcpServer", null, "before accept");
                                tcpClient = server.AcceptTcpClient();
                                //tcpClient.ReceiveTimeout = 5 * 60 * 1000; //set timeout to restart TCP listener after timeout
                                BLL.UiMediator.AddUiMessage(Program.AnalyzerId, (int)MessageType.Normal, "Connected to instrument.");
                                BLL.MessageLogger.WriteTimeDiffLog("TcpServer", null, "after accept");
                                nwStream = tcpClient.GetStream();
                            }
                            else
                            {
                                tcpClient = new TcpClient();
                                tcpClient.Connect(hostIPAddress, hostPort);

                                nwStream = tcpClient.GetStream();

                                if (SendEnquiryOnOpeningPort)
                                {
                                    WriteData(Convert.ToChar(Characters.ENQ).ToString());
                                }
                            }
                        }

                        ConnectionStatus = ConnectionStatus.Connected;

                        BLL.MessageLogger.WriteTimeDiffLog("TcpServer", null, "after GetStream");
                        //}

                        while (isTcpOpen && true) // tcpClient.Connected)
                        {
                            if (tcpClient == null)
                            {
                                BLL.UiMediator.AddUiMessage(Program.AnalyzerId, (int)MessageType.Warning, "tcpClient is null.");
                                Thread.Sleep(5000);
                                continue;
                            }

                            //BLL.MessageLogger.WriteTimeDiffLog("TcpServer", null, "entered loop");
                            int i;
                            Byte[] buffer = new Byte[tcpClient.ReceiveBufferSize];
                            byte[] data = null;

                            if (!tcpClient.Connected)
                                BLL.UiMediator.AddUiMessage(Program.AnalyzerId, (int)MessageType.Warning, "Connection closed.");

                            using (MemoryStream ms = new MemoryStream())
                            {
                                while ((i = nwStream.Read(buffer, 0, buffer.Length)) >= 0)
                                {
                                    if (i < 1)
                                    {
                                        throw new Exception("Zero bytes received from instrument, LIS connection is reset");
                                    }

                                    //BLL.MessageLogger.WriteTimeDiffLog("TcpServer", null, "after Read");
                                    ms.Write(buffer, 0, i);

                                    if (i < buffer.Length)
                                        break;
                                }
                                data = ms.ToArray();
                            }

                            _DataReceived(this, System.Text.Encoding.ASCII/*UTF8*/.GetString(data, 0, data.Length));
                            
                        }

                    }
                    
                    catch (ThreadAbortException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        ConnectionStatus = ConnectionStatus.Disconnected;
                        DisposeObjects();
                        BLL.UiMediator.LogAndShowError(Program.AnalyzerId, ex);
                        BLL.MessageLogger.WriteTimeDiffLog("TcpServer", null, ex.Message + ex.StackTrace);
                        Thread.Sleep(500);
                    }
                    finally
                    {

                    }
                }
                ConnectionStatus = ConnectionStatus.Disconnected;
            });
            this.TcpThread.IsBackground = true;
            this.TcpThread.Start();

            return true;
        }

        private void DisposeObjects()
        {
            if (tcpClient != null && tcpClient.Client.Connected)
            {
                tcpClient.Client.Shutdown(SocketShutdown.Both);
                tcpClient.Client.Close();
                tcpClient.Close();
            }

            tcpClient = null;

            if (nwStream != null)
            {
                nwStream.Close();
                nwStream.Dispose();
            }

            nwStream = null;

            if (server != null && server.Active)
                server.Stop();

            server = null;
        }

        public override string ToString()
        {
            if (server != null)
                return ((IPEndPoint)server.LocalEndpoint).Address + ":" + ((IPEndPoint)server.LocalEndpoint).Port;
            else if (tcpClient != null)
            {
                System.Threading.Thread.Sleep(5000);
                return ((IPEndPoint)tcpClient.Client.LocalEndPoint).Address + ":" + ((IPEndPoint)tcpClient.Client.LocalEndPoint).Port;
            }
            else return "";
        }
    }


    public class TcpListenerEx : TcpListener
    {
        public TcpListenerEx(IPEndPoint localEP) : base(localEP)
        {
        }

        public TcpListenerEx(IPAddress localaddr, int port) : base(localaddr, port)
        {
        }

        //https://stackoverflow.com/questions/7630094/is-there-a-property-method-for-determining-if-a-tcplistener-is-currently-listeni
        public new bool Active
        {
            get { return base.Active; }
        }
    }
}
