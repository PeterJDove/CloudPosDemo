using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Touch.SmartCards
{
    public class Cloud47x0 : WinSmartCardReader
    {
        private static Dictionary<string, Cloud47x0> _readers;

        private static readonly byte[] ESC_RF_SWITCH =        { 0xFF, 0xCC, 0, 0, 2, 0x96, 0x00 }; // byte[6] = 0 for OFF; 1 for ON
        private static readonly byte[] ESC_GET_CARD_INFO =    { 0xFF, 0xCC, 0, 0, 1, 0x11 };
        private static readonly byte[] ESC_GET_CARD_DETAILS = { 0xFF, 0xCC, 0, 0, 1, 0xDA };

        private static readonly byte[] PAPDU_MIFARE_LOAD_KEYS =      { 0xFF, 0x82, 0, 0x60, 6 }; // Change byte[3] to 0x61 for Key B
        private static readonly byte[] PAPDU_MIFARE_AUTHENTICATE =   { 0xFF, 0x86, 0, 0, 5, 1, 0x00, 0x00, 0x60, 1 }; 
                                                                                            // Change byte[7] to Block # (= 4 x Sector #)
                                                                                            // Change byte[8] to 0x61 for Key B
        private static readonly byte[] PAPDU_MIFARE_READ_BINARY =    { 0xFF, 0xB0, 0, 0, 0x10 };  // Change byte[3] to Block #
        private static readonly byte[] PAPDU_MIFARE_UPDATE_BINARY =  { 0xFF, 0xD6, 0, 0, 0x10 };  // Change byte[3] to Block #
        private static readonly byte[] PAPDU_MIFARE_READ_SECTOR =    { 0xFF, 0xB1, 0, 0, 0 };  // Change byte[3] to Sector #
        private static readonly byte[] PAPDU_MIFARE_READ_SECTOR_EX = { 0xFF, 0xB3, 0, 0, 0 }; // Change byte[3] to Sector #
        private static readonly byte[] PAPDU_MIFARE_WRITE_SECTOR =   { 0xFF, 0xD7, 0, 0, 0x30 }; // Change byte[3] to Sector #

        private static readonly byte[] PAPDU_ISO14443_PART3_PASS_THRU = { 0xFF, 0xEF, 1, 0, 0 }; 
        // Byte[2] is 1 to discard/ignore CRC; Change byte[4] to length of appended data

        // The following commands are based on PAPDU_ISO14443_PART3_PASS_THRU:
        private const byte NTAG_GET_VERSION = 0x60; // nothing further to append
        private const byte NTAG_PWD_AUTH = 0x1B; // append 4-byte password
        private const byte NTAG_READ = 0x30; // append 1-byte address of first page (of four) to read
        private const byte NTAG_FAST_READ = 0x3A; // append 2 x 1-byte addresses of first and last pages to read
        private const byte NTAG_WRITE = 0xA2; // append 1-byte address of page + 4 bytes of data

        private const byte NTAG_ACK = 0x0A;

        private CardStatusWord _statusWord;
        private byte[] _data; // read from Sector or Block


        public enum CardStatusWord
        {
            SW_UNKNOWN = 0,
            SW_NO_ERROR = 0x9000,
            SW_NO_INFORMATION_GIVEN = 0x6300,
            SW_MEMORY_FAILURE = 0x6581,
            SW_LENGTH_INCORRECT = 0x6700,
            SW_CLASS_BYTE_INCORRECT = 0x6800,
            SW_SECURITY_STATUS_NOT_SATISFIED = 0x6982,
            SW_AUTHENTICATION_CANNOT_BE_DONE = 0x6983,
            SW_REFERENCE_KEY_NO_USEABLE = 0x6984,
            SW_CONDITIONS_NOT_SATISFIED = 0x6985,
            SW_KEY_TYPE_NOT_KNOWN = 0x6986,
            SW_KEY_NUMBER_NOT_VALID = 0x6988,
            SW_KEY_LENGTH_NOT_CORRECT = 0x6989,
            SW_FUNCTION_NOT_SUPPORTED = 0x6A81,
            SW_WRONG_PARAMETER_P1_P2 = 0x6B00,
            SW_INVALID_INSTRUCTION = 0x6D00,
            SW_CLASS_NOT_SUPPORTED = 0x6E00,
            SW_UNKNOWN_COMMAND = 0x6F00,
        }

        public enum RfState
        {
            OFF = 0,
            ON = 1,
            GET_STATE = 0xFF,
        }

        public enum MifareKey
        {
            A = 0x60,
            B = 0x61,
        }

        private Cloud47x0() { }

        public static Cloud47x0 GetReader(string readerRegex)
        {
            if (_readers == null)
                _readers = new Dictionary<string, Cloud47x0>();

            if (_readers.ContainsKey(readerRegex))
                return _readers[readerRegex];
            else
            {
                var reader = new Cloud47x0();
                if (reader.ConnectReader(readerRegex))
                {
                    _readers.Add(readerRegex, reader);
                    return reader;
                }
                else
                    throw new SmartCardException("Unable to connect to Cloud 47x0 Card Reader");
            }
        }

        public string ATR
        {
            get { return BitConverter.ToString(_atr).Replace("-", ""); }
        }

        public string CardUID
        {
            get { return BitConverter.ToString(_cardUID).Replace("-", ""); }
        }

        public string CardSAK
        {
            get { return _sak.ToString("X2"); } // X2 to ensure we get leading zeroes for _sak < 0x10
        }

        public string CardTech
        {
            get { return _tech == 0x01 ? "TYPE_B" : "TYPE_A"; }
        }

        public string CardInterface
        {
            get
            {
                switch (_sak)
                {
                    case 0x08:
                        return "MIFARE_CLASSIC_1K";
                    case 0x18:
                        return "MIFARE_CLASSIC_4K";
                    case 0x20:
                        return "MIFARE_DESFIRE";
                    default:
                        return null;
                }
            }
        }

        public CardStatusWord StatusWord
        {
            get { return _statusWord; }
        }

        public byte[] SectorData
        {
            get { return _data;  }
        }

        public string SectorHex
        {
            get { return BitConverter.ToString(_data).Replace("-", ""); }
        }

        //public bool ConnectCard(double wait)
        //{
        //    if (WaitForCard(wait))
        //        return ConnectCard();
        //    else
        //        return false;
        //}

        public new bool ConnectCard()
        {
            var result = false;
            if (base.ConnectCard())
                result = ReadCardUID(ref _cardUID, ref _sak, ref _tech);

            return result;
        }

        public bool MifareLoadKey(MifareKey keyType, byte[] key)
        {
            Debug.Assert(key.Length == 6);

            if (CardStillPresent()) // Check if Card still present
            {
                var request = new byte[PAPDU_MIFARE_LOAD_KEYS.Length + key.Length];
                Array.Copy(PAPDU_MIFARE_LOAD_KEYS, request, PAPDU_MIFARE_LOAD_KEYS.Length);
                request[3] = (byte)keyType;
                Array.Copy(key, 0, request, PAPDU_MIFARE_LOAD_KEYS.Length, key.Length);

                return ParseResponse(Transmit(request));
            }
            throw new CardRemovedException();
        }

        public bool MifareAuthenticate(MifareKey keyType, int sector)
        {
            if (CardStillPresent()) // Check if Card still present
            {
                var request = (byte[])PAPDU_MIFARE_AUTHENTICATE.Clone();
                var block = sector * 4;
                request[6] = (byte)(block & 0xFF00); // always zero
                request[7] = (byte)(block & 0xFF);
                request[8] = (byte)keyType;

                return ParseResponse(Transmit(request));
            }
            throw new CardRemovedException();
        }

        public byte[] MifareReadBlock(int block)
        {
            if (CardStillPresent()) // Check if Card still present
            {
                var request = (byte[])PAPDU_MIFARE_READ_BINARY.Clone();
                request[2] = (byte)((block & 0xFF00) >> 8); // always zero
                request[3] = (byte)(block & 0xFF);

                if (ParseResponse(Transmit(request)))
                    return _data;
                else
                    throw new SmartCardException("Error: " + _statusWord.ToString());
            }
            throw new CardRemovedException();
        }

        public bool MifareWriteBlock(int block, byte[] data)
        {
            Debug.Assert(data.Length == 16);

            if (CardStillPresent()) // Check if Card still present
            {
                var request = new byte[PAPDU_MIFARE_UPDATE_BINARY.Length + data.Length];
                Array.Copy(PAPDU_MIFARE_UPDATE_BINARY, request, PAPDU_MIFARE_UPDATE_BINARY.Length);
                request[2] = (byte)((block & 0xFF00) >> 8); // always zero
                request[3] = (byte)(block & 0xFF);
                Array.Copy(data, 0, request, PAPDU_MIFARE_UPDATE_BINARY.Length, data.Length);

                return ParseResponse(Transmit(request));
            }
            throw new CardRemovedException();
        }

        public byte[] MifareReadSector(int sector)
        {
            if (CardStillPresent()) // Check if Card still present
            {
                var request = (byte[])PAPDU_MIFARE_READ_SECTOR.Clone();
                request[2] = (byte)((sector & 0xFF00) >> 8); // always zero
                request[3] = (byte)(sector & 0xFF);

                if (ParseResponse(Transmit(request)))
                    return _data;
                else
                    throw new SmartCardException("Error: " + _statusWord.ToString());
            }
            throw new CardRemovedException();
        }

        public bool MifareWriteSector(int sector, byte[] data)
        {
            Debug.Assert(data.Length == 48);

            if (CardStillPresent()) // Check if Card still present
            {
                var request = new byte[PAPDU_MIFARE_WRITE_SECTOR.Length + data.Length];
                Array.Copy(PAPDU_MIFARE_WRITE_SECTOR, request, PAPDU_MIFARE_WRITE_SECTOR.Length);
                request[2] = (byte)((sector & 0xFF00) >> 8); // always zero
                request[3] = (byte)(sector & 0xFF);
                Array.Copy(data, 0, request, PAPDU_MIFARE_WRITE_SECTOR.Length, data.Length);

                return ParseResponse(Transmit(request));
            }
            throw new CardRemovedException();
        }

        public byte[] Iso14443Part3PassThru(params byte[] data)
        {
            var cardPresent = CardStillPresent();
            if (!cardPresent)
            {
                cardPresent = ConnectCard();
            }
            if (cardPresent) // Check if Card still present
                {
                var i = PAPDU_ISO14443_PART3_PASS_THRU.Length;
                var request = new byte[i + data.Length];
                Array.Copy(PAPDU_ISO14443_PART3_PASS_THRU, request, i);
                request[i - 1] = (byte)data.Length;
                Array.Copy(data, 0, request, i, data.Length);

                return Transmit(request);
            }
            else
                throw new CardRemovedException();
        }

        public string NtagGetVersion()
        {
            _data = Iso14443Part3PassThru(NTAG_GET_VERSION);
            switch (_data.Length)
            {
                case 1: // NAK
                    throw NakException.NtagNakException(_data[0]);
                case 8:
                    Debug.Assert(_data[0] == 0x00); // header
                    Debug.Assert(_data[1] == 0x04); // NXP Semiconductors
                    Debug.Assert(_data[2] == 0x04); // NTAG
                    Debug.Assert(_data[3] == 0x02); // product subtype
                    Debug.Assert(_data[4] == 0x01); // major version 
                    Debug.Assert(_data[5] == 0x00); // minor version 
                    Debug.Assert(_data[7] == 0x03); // protocol type 

                    switch (_data[6])
                    {
                        case 0x0F:
                            return "NTAG213";
                        case 0x11:
                            return "NTAG215";
                        case 0x13:
                            return "NTAG216";
                        default:
                            return "NTAGXXX";
                    }
                default:
                    throw new SmartCardException("Response of unexpected length (" + _data.Length + ") returned");
            }
        }

        public bool NtagPasswordAuthWithPack(byte[] password, byte[] pack)
        {
            if (password.Length == 4 && pack.Length == 2)
            {
                if (NtagPasswordAuth(password))
                {
                    if (_data[0] == pack[0] && _data[1] == pack[1])
                        return true;
                }
            }
            return false;
        }


        public bool NtagPasswordAuth(byte[] password)
        {
            if (password.Length == 4)
            {
                _data = Iso14443Part3PassThru(NTAG_PWD_AUTH, password[0], password[1], password[2], password[3]);
                switch (_data.Length)
                {
                    case 0:
                        return false; // Incorrect Password
                    case 1: // NAK
                        throw NakException.NtagNakException(_data[0]);
                    case 2: // PACK
                        return true;
                    default:
                        throw new SmartCardException("Response of unexpected length (" + _data.Length + ") returned");
                }
            }
            return false;
        }


        public byte[] NtagRead(byte startPage)
        {
            _data = Iso14443Part3PassThru(NTAG_READ, startPage);
            switch (_data.Length)
            {
                case 1: // NAK
                    throw NakException.NtagNakException(_data[0]);
                case 16: // PACK
                    return _data;
                default:
                    throw new SmartCardException("Response of unexpected length (" + _data.Length + ") returned");
            }
        }


        public byte[] NtagFastRead(byte startPage, byte endPage)
        {
            _data = Iso14443Part3PassThru(NTAG_FAST_READ, startPage, endPage);
            if (_data.Length == 4*(endPage - startPage + 1))
                return _data;
            else if (_data.Length == 1)
                throw NakException.NtagNakException(_data[0]);
            else
                throw new SmartCardException("Response of unexpected length (" + _data.Length + ") returned");
        }

        public bool NtagWrite(byte page, byte[] data)
        {
            if (data.Length == 4)
            {
                _data = Iso14443Part3PassThru(NTAG_WRITE, page, data[0], data[1], data[2], data[3]);
                switch (_data.Length)
                {
                    case 1: // ACK or NAK
                        if (_data[0] == NTAG_ACK)
                            return true;
                        else
                            throw NakException.NtagNakException(_data[0]);
                    default:
                        throw new SmartCardException("Response of unexpected length (" + _data.Length + ") returned");
                }
            }
            throw new SmartCardException("Input data should be 4 bytes");
        }

        public bool CardStillPresent()
        {
            var result = false;
            if (WaitForCard(0))
            {
                var uid = new byte[0];
                byte sak = 0;
                byte tech = 0;
                if (ReadCardUID(ref uid, ref sak, ref tech))
                    if (BitConverter.ToString(_cardUID) == BitConverter.ToString(uid))
                        result = true;
            }
            return result;
        }

        public RfState SwitchRF(RfState state)
        {
            var request = new byte[ESC_RF_SWITCH.Length];
            Array.Copy(ESC_RF_SWITCH, request, ESC_RF_SWITCH.Length);
            request[6] = (byte)state;

            var result = Transmit(request);
            if (state == RfState.GET_STATE)
                return (RfState)result[0];
            else
                return state;
        }

        private bool ReadCardUID(ref byte[] uid, ref byte sak, ref byte tech)
        {
            var cardDetails = Transmit(ESC_GET_CARD_DETAILS);
            if (cardDetails.Length > 19)
            {
                int lenUID = cardDetails[2];
                uid = new byte[lenUID];
                Array.Copy(cardDetails, 3, uid, 0, lenUID);
                sak = cardDetails[19];
                tech = (byte)(cardDetails[0] & 0x01);
                return true;
            }
            else
            {
                uid = new byte[0];
                sak = 0;
                return false;
            }
        }

        private bool ParseResponse(byte[] response)
        {
            var len = response.Length;
            if (len < 2)
            {
                _statusWord = CardStatusWord.SW_UNKNOWN;
                _data = new byte[0];
            }
            else if (len == 2)
            {
                _statusWord = (CardStatusWord)(0x100 * response[0] + response[1]);
                _data = new byte[0];
            }
            else
            {
                _statusWord = (CardStatusWord)(0x100 * response[len-2] + response[len-1]);
                _data = new byte[len-2];
                Array.Copy(response, 0, _data, 0, len-2);
            }
            return (_statusWord == CardStatusWord.SW_NO_ERROR);
        }


    }
}
