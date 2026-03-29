using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSoftLIS_Interface.Common;

namespace VSoftLIS_Interface.BLL
{
    class CommunicationFrameHandler
    {
        public bool NewFrameForEachRecord = true;
        public int MaxCharacterDataCount = 240;
        public bool HasCarriageReturnAfterRecord = true;
        public bool EtxEtbPositionBeforeChecksum = true;
        public bool SupportsFraming = true;
        public bool SupportsChecksum = true;
        public bool SupportsStxEtx = true;
        public string FrameSeparator = Characters.CR + Characters.LF;
        public int ChecksumLogic = 2;
        //public bool ExcludeChecksumPartForChecksumVerification = false;
        public bool IncludeStxInComputeChecksum = false;
        int AnalyzerTypeId = 0;
        internal static MessageConfiguration msgConfig = null;

        public CommunicationFrameHandler(int analyzer_ID)
        {
            AnalyzerTypeId = CachedData.Analyzer.instrumentgroupid;
            switch (AnalyzerTypeId)
            {
              
                case 9: //Sysmex XN 1000
                    MaxCharacterDataCount = 0;
                    break;

                case 3: //Olympus AU 700
                    MaxCharacterDataCount = 0;
                    SupportsFraming = false;
                    SupportsChecksum = false;
                    SupportsStxEtx = true;
                    HasCarriageReturnAfterRecord = false;
                    FrameSeparator = "";
                    break;

                case 8: //TOSOH G8
                    MaxCharacterDataCount = 240;
                    NewFrameForEachRecord = true;
                    break;

                case 5: //LABUMAT & URISEN
                    MaxCharacterDataCount = 0;
                    NewFrameForEachRecord = false;
                    break;

                case 7: // Atelika
                    SupportsChecksum = false;
                    MaxCharacterDataCount = 50000;
                    NewFrameForEachRecord = false;
                    //FrameSeparator = Characters.CR + Characters.FS;
                    break;

                case 154: //DXH800
                    SupportsChecksum = true;
                    MaxCharacterDataCount = 240;
                    FrameSeparator = Characters.CR + Characters.LF;
                    //NewFrameForEachRecord = true;
                    break;

                case 155://Zybio EXZ 6000 H6
                    SupportsChecksum = false;
                    MaxCharacterDataCount = 10000;
                    NewFrameForEachRecord = false;
                    break;

                case 157: //
                    FrameSeparator = Characters.CR;
                    NewFrameForEachRecord = true;
                    break;

                case 158: // MISPA_CX4
                    SupportsChecksum = true;
                    MaxCharacterDataCount = 240;
                    FrameSeparator = Characters.CR + Characters.LF;
                    break;
            }
        }

        public List<string> PrepareFramesHL7(params string[] frameRecords)
        {
            List<string> framesBeforeSplitting = frameRecords.ToList();

            if (!NewFrameForEachRecord)
            {
                framesBeforeSplitting = new List<string> { String.Join("", framesBeforeSplitting) };
            }

            //split entire message string into fixed character width frames
            List<Frame> frames = new List<Frame>();
            foreach (string strFrameData in framesBeforeSplitting)
            {
                int cntChar = 0;
                while (cntChar < strFrameData.Length)
                {
                    string strFrameData_Final = new string(strFrameData.Skip(cntChar).Take(MaxCharacterDataCount > 0 ? MaxCharacterDataCount : strFrameData.Length).ToArray());
                    frames.Add(new Frame { FrameWithoutChecksum = strFrameData_Final, IsIncomplete = (MaxCharacterDataCount > 0 && cntChar + MaxCharacterDataCount < strFrameData.Length) });
                    cntChar += strFrameData_Final.Length;
                }
            }


            //add frame number and checksum to each frame
            List<string> finalFrames = new List<string>();
            int fn = 0;
            for (int i = 0; i < frames.Count; i++)
            {
                string finalFrame = "";
                string frameData = frames[i].FrameWithoutChecksum;

                fn = GetNextFrameNumber(fn);
                //string etxEtb = !frames[i].IsIncomplete ? Characters.ETX : Characters.ETB;
                //string msg = (SupportsFraming ? fn.ToString() : "") + frames[i].FrameWithoutChecksum + (EtxEtbPositionBeforeChecksum ? etxEtb : "");
                string etxEtb = !frames[i].IsIncomplete ? "" : "";
                //string msg = (SupportsFraming ? fn.ToString() : "") + frames[i].FrameWithoutChecksum + (EtxEtbPositionBeforeChecksum ? etxEtb : "");
                string msg = (SupportsFraming ? "" : "") + frames[i].FrameWithoutChecksum + (EtxEtbPositionBeforeChecksum ? etxEtb : "");

                //finalFrame = Characters.STX + msg + (SupportsChecksum ? ComputeCheckSum((IncludeStxInComputeChecksum ? Characters.STX : "") + msg) : "") + (EtxEtbPositionBeforeChecksum ? "" : etxEtb) + (SupportsFraming ? FrameSeparator : "");
                finalFrame = msg + Characters.FS + Characters.CR;

                finalFrame = finalFrame.Replace("LIS_ID||", "LIS_ID|\"\"|");
                finalFrame = finalFrame.Replace("UIW_LIS||", "UIW_LIS|\"\"|");

                finalFrames.Add(finalFrame);
            }

            return finalFrames;
        }

        public List<string> PrepareFrames(params string[] frameRecords)
        {

            List<string> framesBeforeSplitting = frameRecords.ToList();

            if (!NewFrameForEachRecord)
            {
                framesBeforeSplitting = new List<string> { String.Join("", framesBeforeSplitting) };
            }

            //split entire message string into fixed character width frames
            List<Frame> frames = new List<Frame>();
            foreach (string strFrameData in framesBeforeSplitting)
            {
                int cntChar = 0;
                while (cntChar < strFrameData.Length)
                {
                    string strFrameData_Final = new string(strFrameData.Skip(cntChar).Take(MaxCharacterDataCount > 0 ? MaxCharacterDataCount : strFrameData.Length).ToArray());
                    frames.Add(new Frame { FrameWithoutChecksum = strFrameData_Final, IsIncomplete = (MaxCharacterDataCount > 0 && cntChar + MaxCharacterDataCount < strFrameData.Length) });
                    cntChar += strFrameData_Final.Length;
                }
            }

            //add frame number and checksum to each frame
            List<string> finalFrames = new List<string>();
            int fn = 0;
            for (int i = 0; i < frames.Count; i++)
            {
                string finalFrame = "";
                string frameData = frames[i].FrameWithoutChecksum;

                fn = GetNextFrameNumber(fn);
                string etxEtb = !frames[i].IsIncomplete ? Characters.ETX : Characters.ETB;
                string msg = (SupportsFraming ? fn.ToString() : "") + frames[i].FrameWithoutChecksum + (EtxEtbPositionBeforeChecksum ? etxEtb : "");
                
                finalFrame = Characters.STX + msg + (SupportsChecksum ? ComputeCheckSum((IncludeStxInComputeChecksum ? Characters.STX : "") + msg) : "") + (EtxEtbPositionBeforeChecksum ? "" : etxEtb) + (SupportsFraming ? FrameSeparator : "");

                finalFrames.Add(finalFrame);
            }

            return finalFrames;
        }

        public List<string> PrepareFrames(IEnumerable<string> frameRecords)
        {
            return PrepareFrames(frameRecords.ToArray());
        }

        public List<string> PrepareFramesHL7(IEnumerable<string> frameRecords)
        {
            return PrepareFramesHL7(frameRecords.ToArray());
        }

        public bool VerifyChecksumAndExtractData(string frameText, out bool isFinalFrame, out string frame_DataPart)
        {
            string checksumReceived = "";

            isFinalFrame = false;
            int charactersToSkip_Left = 0; //[STX]
            int charactersToSkip_Right = 0; //[ETB]A0[CR][LF]

            if (SupportsFraming)
            {
                charactersToSkip_Left++; //for Frame Number
            }

            if (SupportsStxEtx)
            {
                charactersToSkip_Left++;
                charactersToSkip_Right++;

                if (frameText.Contains(Convert.ToChar(Characters.ETX)))
                    isFinalFrame = true;
                else if (frameText.Contains(Convert.ToChar(Characters.ETB)))
                    isFinalFrame = false;
            }

            if (SupportsChecksum)
            {
                charactersToSkip_Right += 2;
            }

            charactersToSkip_Right += FrameSeparator.Length;

            frame_DataPart = frameText.Substring(charactersToSkip_Left, frameText.Length - charactersToSkip_Right - charactersToSkip_Left); //[STX]1 ... [ETB]A0[CR][LF]

            if (SupportsChecksum)
            {
                int checksumStartIndex = -1;
                if (EtxEtbPositionBeforeChecksum)
                {
                    checksumStartIndex = frameText.Length - 4;
                }
                else
                {
                    checksumStartIndex = frameText.Length - 3;
                }
                checksumReceived = frameText.Substring(checksumStartIndex, 2);

                string frameToVerifyChecksum = /*!ExcludeChecksumPartForChecksumVerification ? frameText :*/ frameText.Substring(0, checksumStartIndex);

                if (ComputeCheckSum(frameToVerifyChecksum) == checksumReceived || ChecksumLogic == 0)
                    //if (ComputeCheckSum(frameText) == checksumReceived || ChecksumLogic == 0)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        public string ComputeCheckSum(string frame)
        {
            if (ChecksumLogic == 1)
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(frame);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                double sum = 0;
                for (int i = 0; i < inputBytes.Length; i++)
                {
                    sum += inputBytes[i];
                    sb.Append(inputBytes[i].ToString("X2"));
                }

                int quot = Convert.ToInt32(Math.Floor((sum % 256.0) / 16.0) + 48);
                int remainder = Convert.ToInt32(((sum % 256.0) % 16.0) + 48);

                char first = (char)quot;
                char second = (char)remainder;
                System.Text.StringBuilder sb1 = new System.Text.StringBuilder();
                sb1.Append(first);
                sb1.Append(second);
                return (sb1.ToString());
            }
            else if (ChecksumLogic == 2 || ChecksumLogic == 0)
            {
                //Below checksum code copied from http://www.hendricksongroup.com/code_003.aspx
                string checksum = "00";

                int byteVal = 0;
                int sumOfChars = 0;
                bool complete = false;

                //take each byte in the string and add the values
                for (int idx = 0; idx < frame.Length; idx++)
                {
                    byteVal = Convert.ToInt32(frame[idx]);

                    switch (byteVal)
                    {
                        case T.STX:
                            sumOfChars = 0;
                            break;
                        case T.ETX:
                        case T.ETB:
                            sumOfChars += byteVal;
                            complete = true;
                            break;
                        default:
                            sumOfChars += byteVal;
                            break;
                    }

                    if (complete)
                        break;
                }

                if (sumOfChars > 0)
                {
                    //hex value mod 256 is checksum, return as hex value in upper case
                    checksum = Convert.ToString(sumOfChars % 256, 16).ToUpper();
                }

                //if checksum is only 1 char then prepend a 0
                return (string)(checksum.Length == 1 ? "0" + checksum : checksum);
            }
            else if (ChecksumLogic == 3)
            {
                // XOR of N+1 frame characters, written code initially for Chorus LIS
                //reference link http://bobby-dotnet.blogspot.com/2015/09/generating-checksum-in-c.html
                string checksum = "0";

                byte[] inputBytes = InterfaceHelper.AsciiStringToByte(frame);

                byte xor = 0x00;
                for (int i = 0; i < inputBytes.Length; i++)
                {
                    xor ^= inputBytes[i]; //XOR (bitwise OR) operation
                }
                return InterfaceHelper.ByteToAsciiString(new byte[] { xor });
            }

            throw new NotImplementedException("Checksum logic not implemented");
        }

        public static int GetNextFrameNumber(int fn)
        {
            if (fn == 7)
                fn = -1;

            return ++fn;
        }

        public int ExtractFrameNumber(string frame)
        {
            if (SupportsFraming)
                return Convert.ToInt32(frame.Substring(1, 1));
            else
                throw new Exception("Framing not supported, cannot extract frame number.");
        }
    }

    public class T
    {
        public const byte ENQ = 5;
        public const byte ACK = 6;
        public const byte NAK = 21;
        public const byte EOT = 4;
        public const byte ETX = 3;
        public const byte ETB = 23;
        public const byte STX = 2;
        public const byte NEWLINE = 10;
        public static byte[] ACK_BUFF = { ACK };
        public static byte[] ENQ_BUFF = { ENQ };
        public static byte[] NAK_BUFF = { NAK };
        public static byte[] EOT_BUFF = { EOT };
    }

    public class Frame
    {
        public string FrameWithoutChecksum { get; set; }
        public bool IsIncomplete { get; set; }
    }
}
