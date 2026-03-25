using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.Common
{
    #region Manager Enums
    /// <summary>
    /// enumeration to hold our transmission types
    /// </summary>
    public enum TransmissionType { Text, Hex }

    /// <summary>
    /// enumeration to hold our message types
    /// </summary>
    public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };
    #endregion

    //global manager variables
    public enum MessageColor { Blue, Green, Black, Orange, Red };

    public enum enmCharacters
    {
        STX = 2,
        ETX = 3,
        NAK = 21,
        ENQ = 5,
        EOT = 4,
        ACK = 6,
        CR = 13,
        LF = 10,
        ETB = 23,
        SOH = 1,
        DC1 = 17,
        FS = 28,
        VT = 11
    }

    public static class Characters
    {
        public static string STX = Convert.ToChar(enmCharacters.STX).ToString(),
           ETX = Convert.ToChar(enmCharacters.ETX).ToString(),
           NAK = Convert.ToChar(enmCharacters.NAK).ToString(),
           ENQ = Convert.ToChar(enmCharacters.ENQ).ToString(),
           EOT = Convert.ToChar(enmCharacters.EOT).ToString(),
           ACK = Convert.ToChar(enmCharacters.ACK).ToString(),
           CR = Convert.ToChar(enmCharacters.CR).ToString(),
           LF = Convert.ToChar(enmCharacters.LF).ToString(),
           ETB = Convert.ToChar(enmCharacters.ETB).ToString(),
           SOH = Convert.ToChar(enmCharacters.SOH).ToString(),
           DC1 = Convert.ToChar(enmCharacters.DC1).ToString(),
           FS = Convert.ToChar(enmCharacters.FS).ToString(),
           VT = Convert.ToChar(enmCharacters.VT).ToString();
    }

    public enum InstrumentTypes
    {
        Analyzer = 1,
        //Sorter = 2,
        Archival = 3
    }

    public enum DescriptiveIDs
    {
        Negative = 4,
        Positive = 7,
        Inconclusive = 347,
        NotDetected = 117,
        Detected = 107,
        Equivocal = 332,
        Invalid = 364,
        Review_result = 372
    }
}
