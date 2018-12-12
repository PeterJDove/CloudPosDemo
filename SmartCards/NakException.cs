using System;
using System.Diagnostics;
using System.Text;

namespace Touch.SmartCards
{
    public class NakException : SmartCardException
    {
        public NakException(string message) :
            base(message) { }

        public NakException(string message, Exception innerException) :
            base(message, innerException) { }

        public static NakException NtagNakException(byte nak)
        {
            Debug.Assert(nak != 0x0A); // because 0x0A is an ACK!
            switch (nak)
            {
                case 0x00:
                    return new NakException("Invalid argument (invalid page address)");
                case 0x01:
                    return new NakException("Parity or CRC error");
                case 0x04:
                    return new NakException("Invalid Authorisation counter overflow");
                case 0x05:
                    return new NakException("EEPROM write error");
                default:
                    return new NakException("NAK " + nak.ToString(("X2")));
            }
        }
    }
}