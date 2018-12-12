using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Touch.SmartCards
{
    public class WinSmartCardReader
    {
        //**************************************************************************
        //      Function, user-defined data type and Constant Declarations
        //**************************************************************************

        /* 
        Public Type SCARD_IO_REQUEST
            dwProtocol As Long
            cbPciLength As Long
        End Type
         */

        /* 
        Public Type BytArray
            byBytes(601) As Byte
        End Type
         */

        /* 
        Public Type APDURec
            bCLA As Byte
            bINS As Byte
            bP1 As Byte
            bP2 As Byte
            bP3 As Byte
            Data(1 To 255) As Byte
            IsSend As Boolean
        End Type
         */ 

        protected struct SCARD_READERSTATE
        {
            internal string reader;
            internal string userData;
            internal ReaderState currenState;
            internal ReaderState eventState;
            internal int atrLen;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SCARD_ATR_LENGTH, ArraySubType = UnmanagedType.U1)] // Avoids 'System.AccessViolationException' (somehow!)
            internal byte[] atr;
        }

        //protected struct VERSION_CONTROL
        //{
        //    internal uint SmclibVersion;
        //    internal byte DriverMajor;
        //    internal byte DriverMinor;
        //    internal byte FirmwareMajor;
        //    internal byte FirmwareMinor;
        //    internal byte UpdateKey;
        //}

        // Our own constant declarations
        
        public const uint MINTIMEOUT = 0;
        public const uint MAX_READERS = 8;
        public const int SCARD_ATR_LENGTH = 33;

        // Constants used to access card and reader attributes

        public const uint SCARD_CLASS_VENDOR_INFO = 1; // Vendor information definitions
        public const uint SCARD_ATTR_VENDOR_NAME = (((SCARD_CLASS_VENDOR_INFO) * (2 ^ 16)) | (0x100));
        public const uint SCARD_ATTR_VENDOR_IFD_TYPE = (((SCARD_CLASS_VENDOR_INFO) * (2 ^ 16)) | (0x101));
        public const uint SCARD_ATTR_VENDOR_IFD_VERSION = (((SCARD_CLASS_VENDOR_INFO) * (2 ^ 16)) | (0x102));
        public const uint SCARD_ATTR_VENDOR_IFD_SERIAL_NO = (((SCARD_CLASS_VENDOR_INFO) * (2 ^ 16)) | (0x103));
                                                    
        public const uint SCARD_CLASS_COMMUNICATIONS = 2; // Communication definitions
        public const uint SCARD_ATTR_CHANNEL_ID = (((SCARD_CLASS_COMMUNICATIONS) * (2 ^ 16)) | (0x110));
                                                    
        public const uint SCARD_CLASS_PROTOCOL = 3; // Protocol definitions
        public const uint SCARD_ATTR_ASYNC_PROTOCOL_TYPES = (((SCARD_CLASS_PROTOCOL) * (2 ^ 16)) | (0x120));
        public const uint SCARD_ATTR_DEFAULT_CLK = (((SCARD_CLASS_PROTOCOL) * (2 ^ 16)) | (0x121));
        public const uint SCARD_ATTR_MAX_CLK = (((SCARD_CLASS_PROTOCOL) * (2 ^ 16)) | (0x122));
        public const uint SCARD_ATTR_DEFAULT_DATA_RATE = (((SCARD_CLASS_PROTOCOL) * (2 ^ 16)) | (0x123));
        public const uint SCARD_ATTR_MAX_DATA_RATE = (((SCARD_CLASS_PROTOCOL) * (2 ^ 16)) | (0x124));
        public const uint SCARD_ATTR_MAX_IFSD = (((SCARD_CLASS_PROTOCOL) * (2 ^ 16)) | (0x125));
        public const uint SCARD_ATTR_SYNC_PROTOCOL_TYPES = (((SCARD_CLASS_PROTOCOL) * (2 ^ 16)) | (0x126));
         
        public const uint SCARD_CLASS_POWER_MGMT = 4; // Power Management definitions
        public const uint SCARD_ATTR_POWER_MGMT_SUPPORT = (((SCARD_CLASS_POWER_MGMT) * (2 ^ 16)) | (0x131));
                                                       
        public const uint SCARD_CLASS_SECURITY = 5; // Security Assurance definitions
        public const uint SCARD_ATTR_USER_TO_CARD_AUTH_DEVICE = (((SCARD_CLASS_SECURITY) * (2 ^ 16)) | (0x140));
        public const uint SCARD_ATTR_USER_AUTH_INPUT_DEVICE = (((SCARD_CLASS_SECURITY) * (2 ^ 16)) | (0x142));
                                                           
        public const uint SCARD_CLASS_MECHANICAL = 6; // Mechanical characteristic definitions
        public const uint SCARD_ATTR_CHARACTERISTICS = (((SCARD_CLASS_MECHANICAL) * (2 ^ 16)) | (0x150));

        public const uint SCARD_CLASS_VENDOR_DEFINED = 7; // Vendor specific definitions
        public const uint SCARD_ATTR_ESC_RESET = (((SCARD_CLASS_VENDOR_DEFINED) * (2 ^ 16)) | (0xA000));
        public const uint SCARD_ATTR_ESC_CANCEL = (((SCARD_CLASS_VENDOR_DEFINED) * (2 ^ 16)) | (0xA003));
        public const uint SCARD_ATTR_ESC_AUTHREQUEST = (((SCARD_CLASS_VENDOR_DEFINED) * (2 ^ 16)) | (0xA005));
        public const uint SCARD_ATTR_MAXINPUT = (((SCARD_CLASS_VENDOR_DEFINED) * (2 ^ 16)) | (0xA007));
         
        public const uint SCARD_CLASS_IFD_PROTOCOL = 8; // Interface Device Protocol options
        public const uint SCARD_ATTR_CURRENT_PROTOCOL_TYPE = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x201));
        public const uint SCARD_ATTR_CURRENT_CLK = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x202));
        public const uint SCARD_ATTR_CURRENT_F = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x203));
        public const uint SCARD_ATTR_CURRENT_D = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x204));
        public const uint SCARD_ATTR_CURRENT_N = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x205));
        public const uint SCARD_ATTR_CURRENT_W = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x206));
        public const uint SCARD_ATTR_CURRENT_IFSC = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x207));
        public const uint SCARD_ATTR_CURRENT_IFSD = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x208));
        public const uint SCARD_ATTR_CURRENT_BWT = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x209));
        public const uint SCARD_ATTR_CURRENT_CWT = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x20A));
        public const uint SCARD_ATTR_CURRENT_EBC_ENCODING = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x20B));
        public const uint SCARD_ATTR_EXTENDED_BWT = (((SCARD_CLASS_IFD_PROTOCOL) * (2 ^ 16)) | (0x20C));

        public const uint SCARD_CLASS_ICC_STATE = 9; // ICC State specific definitions
        public const uint SCARD_ATTR_ICC_PRESENCE = (((SCARD_CLASS_ICC_STATE) * (2 ^ 16)) | (0x300));
        public const uint SCARD_ATTR_ICC_INTERFACE_STATUS = (((SCARD_CLASS_ICC_STATE) * (2 ^ 16)) | (0x301));
        public const uint SCARD_ATTR_CURRENT_IO_STATE = (((SCARD_CLASS_ICC_STATE) * (2 ^ 16)) | (0x302));
        public const uint SCARD_ATTR_ATR_STRING = (((SCARD_CLASS_ICC_STATE) * (2 ^ 16)) | (0x303));
        public const uint SCARD_ATTR_ICC_TYPE_PER_ATR = (((SCARD_CLASS_ICC_STATE) * (2 ^ 16)) | (0x304));

        public const uint SCARD_CLASS_SYSTEM = 0x7FFF; // System-specific definitions
        public const uint SCARD_ATTR_DEVICE_UNIT = (((SCARD_CLASS_SYSTEM) * (2 ^ 16)) | (0x1));
        public const uint SCARD_ATTR_DEVICE_IN_USE = (((SCARD_CLASS_SYSTEM) * (2 ^ 16)) | (0x2));
        public const uint SCARD_ATTR_DEVICE_FRIENDLY_NAME_A = (((SCARD_CLASS_SYSTEM) * (2 ^ 16)) | (0x3));
        public const uint SCARD_ATTR_DEVICE_SYSTEM_NAME_A = (((SCARD_CLASS_SYSTEM) * (2 ^ 16)) | (0x4));
        public const uint SCARD_ATTR_DEVICE_FRIENDLY_NAME_W = (((SCARD_CLASS_SYSTEM) * (2 ^ 16)) | (0x5));
        public const uint SCARD_ATTR_DEVICE_SYSTEM_NAME_W = (((SCARD_CLASS_SYSTEM) * (2 ^ 16)) | (0x6));
        public const uint SCARD_ATTR_SUPRESS_T1_IFS_REQUEST = (((SCARD_CLASS_SYSTEM) * (2 ^ 16)) | (0x7));

        public const uint SCARD_ATTR_DEVICE_FRIENDLY_NAME = SCARD_ATTR_DEVICE_FRIENDLY_NAME_A;
        public const uint SCARD_ATTR_DEVICE_SYSTEM_NAME = SCARD_ATTR_DEVICE_SYSTEM_NAME_A;


        //**************************************************************************
        // IO_CTL Codes
        //**************************************************************************

        public const uint METHOD_BUFFERED = 0;
        public const uint METHOD_IN_DIRECT = 1;
        public const uint METHOD_OUT_DIRECT = 2;
        public const uint METHOD_NEITHER = 3;

        public const uint FILE_ANY_ACCESS = 0x0;
        public const uint FILE_READ_ACCESS = 0x1;
        public const uint FILE_WRITE_ACCESS = 0x2;

        public const uint FILE_DEVICE_SMARTCARD = 0x31;

        public const uint IOCTL_PSCR_COMMAND = (((FILE_DEVICE_SMARTCARD) * (2 ^ 16)) | 
                                            ((FILE_ANY_ACCESS) * (2 ^ 14)) | 
                                            ((0x800) * (2 ^ 2)) | 
                                            (METHOD_BUFFERED));

        public const uint IOCTL_GET_VERSIONS = (((FILE_DEVICE_SMARTCARD) * (2 ^ 16)) | 
                                            ((FILE_ANY_ACCESS) * (2 ^ 14)) | 
                                            ((0x801) * (2 ^ 2)) | 
                                            (METHOD_BUFFERED));

        public const uint IOCTL_SET_TIMEOUT = (((FILE_DEVICE_SMARTCARD) * (2 ^ 16)) | 
                                            ((FILE_ANY_ACCESS) * (2 ^ 14)) | 
                                            ((0x802) * (2 ^ 2)) | 
                                            (METHOD_BUFFERED));

        public const uint IOCTL_RESET_INTERFACE = (((FILE_DEVICE_SMARTCARD) * (2 ^ 16)) | 
                                            ((FILE_ANY_ACCESS) * (2 ^ 14)) | 
                                            ((0x803) * (2 ^ 2)) | 
                                            (METHOD_BUFFERED));

        public const uint IOCTL_GET_CONFIGURATION = (((FILE_DEVICE_SMARTCARD) * (2 ^ 16)) | 
                                            ((FILE_ANY_ACCESS) * (2 ^ 14)) | 
                                            ((0x804) * (2 ^ 2)) | 
                                            (METHOD_BUFFERED));

        public const uint IOCTL_SCM_PINPAD = (((FILE_DEVICE_SMARTCARD) * (2 ^ 16)) | 
                                            ((FILE_ANY_ACCESS) * (2 ^ 14)) | 
                                            ((0x805) * (2 ^ 2)) | 
                                            (METHOD_BUFFERED));

        public const uint IOCTL_SCM_SET_P = (((FILE_DEVICE_SMARTCARD) * (2 ^ 16)) | 
                                            ((FILE_ANY_ACCESS) * (2 ^ 14)) | 
                                            ((0x811) * (2 ^ 2)) | 
                                            (METHOD_BUFFERED));

        public const uint IOCTL_SCM_GET_P = (((FILE_DEVICE_SMARTCARD) * (2 ^ 16)) | 
                                            ((FILE_ANY_ACCESS) * (2 ^ 14)) | 
                                            ((0x812) * (2 ^ 2)) | 
                                            (METHOD_BUFFERED));

        public const uint IOCTL_GET_DEVICE_NAME = (((FILE_DEVICE_SMARTCARD) * (2 ^ 16)) | 
                                            ((FILE_ANY_ACCESS) * (2 ^ 14)) | 
                                            ((0x820) * (2 ^ 2)) | 
                                            (METHOD_BUFFERED));


        //**************************************************************************
        // Context Scope
        //**************************************************************************
        
        public enum Scope
        {
            SCARD_SCOPE_USER = 0,       /* The context is a user context, and any database operations
                                         * are performed within the domain of the user. */

            SCARD_SCOPE_TERMINAL = 1,   /* The context is that of the current terminal,and any database
                                         * operations are performed within the domain of that terminal.
                                         * (The calling application must have appropriate access
                                         * permissions for any database actions.) */

            SCARD_SCOPE_SYSTEM = 2,     /* The context is the system context, and any database operations
                                         * are performed within the domain of the system.  (The calling
                                         * application must have appropriate access permissions for
                                         * any database actions.) */
        }

        //**************************************************************************
        //  Reader State
        //**************************************************************************

        [Flags] public enum ReaderState
        {
            SCARD_STATE_UNAWARE = 0x0,      /* The application is unaware of the current state, and would
                                             * like to know.  The use of this value results in an immediate
                                             * return from state transition monitoring services.  This is
                                             * represented by all bits set to zero. */

            SCARD_STATE_IGNORE = 0x1,       /* The application requested that this reader be ignored.
                                             * No other bits will be set. */

            SCARD_STATE_CHANGED = 0x2,      /* This implies that there is a difference between the state
                                             * believed by the application, and the state known by the
                                             * Service Manager.  When this bit is set, the application may
                                             * assume a significant state change has occurred on this reader. */

            SCARD_STATE_UNKNOWN = 0x4,      /* This implies that the given reader name is not recognized
                                             * by the Service Manager.  If this bit is set, then
                                             * SCARD_STATE_CHANGED and SCARD_STATE_IGNORE will also be set. */

            SCARD_STATE_UNAVAILABLE = 0x8,  /* This implies that the actual state of this reader is not
                                             * available.  If this bit is set, then all the following bits
                                             * are clear. */

            SCARD_STATE_EMPTY = 0x10,       /* This implies that there is no card in the reader.  If this
                                             * bit is set, all the following bits will be clear. */

            SCARD_STATE_PRESENT = 0x20,     /* This implies that there is a card in the reader. */

            SCARD_STATE_ATRMATCH = 0x40,    /* This implies that there is a card in the reader with an ATR
                                             * matching one of the target cards. If this bit is set,
                                             * SCARD_STATE_PRESENT will also be set.  This bit is only
                                             * returned on the SCardLocateCard() service. */

            SCARD_STATE_EXCLUSIVE = 0x80,   /* This implies that the card in the reader is allocated for
                                             * exclusive use by another application.  If this bit is set,
                                             * SCARD_STATE_PRESENT will also be set. */

            SCARD_STATE_INUSE = 0x100,      /* This implies that the card in the reader is in use by one or
                                             * more other applications, but may be connected to in shared mode.
                                             * If this bit is set, SCARD_STATE_PRESENT will also be set. */

            SCARD_STATE_MUTE = 0x200,       /* This implies that the card in the reader is unresponsive or
                                             * not supported by the reader or software. */

            SCARD_STATE_UNPOWERED = 0x400,  /* This implies that the card in the reader has not been powered up. */
        }

        //**************************************************************************
        //  Sharing
        //**************************************************************************
        
        public enum Sharing
        {
            SCARD_SHARE_EXCLUSIVE = 1, // Not willing to share this card with other applications.
            SCARD_SHARE_SHARED = 2, // Willing to share this card with other applications.
            SCARD_SHARE_DIRECT = 3 // Demands direct control of the reader, so it is not available to other applications.
        }

        //**************************************************************************
        //   Disposition
        //**************************************************************************

        public enum Disposition
        {
            SCARD_LEAVE_CARD = 0,
            SCARD_RESET_CARD = 1,
            SCARD_UNPOWER_CARD = 2,
            SCARD_EJECT_CARD = 3
        }

        //**************************************************************************
        //   Error Codes
        //**************************************************************************

        public enum Result : uint
        { 
            SCARD_S_SUCCESS = 0x00000000,
            SCARD_F_INTERNAL_ERROR = 0x80100001,
            SCARD_E_CANCELLED = 0x80100002,
            SCARD_E_INVALID_HANDLE = 0x80100003,
            SCARD_E_INVALID_PARAMETER = 0x80100004,
            SCARD_E_INVALID_TARGET = 0x80100005,
            SCARD_E_NO_MEMORY = 0x80100006,
            SCARD_F_WAITED_TOO_LONG = 0x80100007,
            SCARD_E_INSUFFICIENT_BUFFER = 0x80100008,
            SCARD_E_UNKNOWN_READER = 0x80100009,
            SCARD_E_TIMEOUT = 0x8010000A,
            SCARD_E_SHARING_VIOLATION = 0x8010000B,
            SCARD_E_NO_SMARTCARD = 0x8010000C,
            SCARD_E_UNKNOWN_CARD = 0x8010000D,
            SCARD_E_CANT_DISPOSE = 0x8010000E,
            SCARD_E_PROTO_MISMATCH = 0x8010000F,
            SCARD_E_NOT_READY = 0x80100010,
            SCARD_E_INVALID_VALUE = 0x80100011,
            SCARD_E_SYSTEM_CANCELLED = 0x80100012,
            SCARD_F_COMM_ERROR = 0x80100013,
            SCARD_F_UNKNOWN_ERROR = 0x80100014,
            SCARD_E_INVALID_ATR = 0x80100015,
            SCARD_E_NOT_TRANSACTED = 0x80100016,
            SCARD_E_READER_UNAVAILABLE = 0x80100017,
            SCARD_P_SHUTDOWN = 0x80100018,
            SCARD_E_PCI_TOO_SMALL = 0x80100019,
            SCARD_E_READER_UNSUPPORTED = 0x8010001A,
            SCARD_E_DUPLICATE_READER = 0x8010001B,
            SCARD_E_CARD_UNSUPPORTED = 0x8010001C,
            SCARD_E_NO_SERVICE = 0x8010001D,
            SCARD_E_SERVICE_STOPPED = 0x8010001E,
            SCARD_E_UNEXPECTED = 0x8010001F,
            SCARD_E_UNSUPPORTED_FEATURE = 0x8010001F,
            SCARD_E_ICC_INSTALLATION = 0x80100020,
            SCARD_E_ICC_CREATEORDER = 0x80100021,
            SCARD_E_DIR_NOT_FOUND = 0x80100023,
            SCARD_E_FILE_NOT_FOUND = 0x80100024,
            SCARD_E_NO_DIR = 0x80100025,
            SCARD_E_NO_FILE = 0x80100026,
            SCARD_E_NO_ACCESS = 0x80100027,
            SCARD_E_WRITE_TOO_MANY = 0x80100028,
            SCARD_E_BAD_SEEK = 0x80100029,
            SCARD_E_INVALID_CHV = 0x8010002A,
            SCARD_E_UNKNOWN_RES_MNG = 0x8010002B,
            SCARD_E_NO_SUCH_CERTIFICATE = 0x8010002C,
            SCARD_E_CERTIFICATE_UNAVAILABLE = 0x8010002D,
            SCARD_E_NO_READERS_AVAILABLE = 0x8010002E,
            SCARD_E_COMM_DATA_LOST = 0x8010002F,
            SCARD_E_NO_KEY_CONTAINER = 0x80100030,
            SCARD_E_SERVER_TOO_BUSY = 0x80100031,
            SCARD_W_UNSUPPORTED_CARD = 0x80100065,
            SCARD_W_UNRESPONSIVE_CARD = 0x80100066,
            SCARD_W_UNPOWERED_CARD = 0x80100067,
            SCARD_W_RESET_CARD = 0x80100068,
            SCARD_W_REMOVED_CARD = 0x80100069,
            SCARD_W_SECURITY_VIOLATION = 0x8010006A,
            SCARD_W_WRONG_CHV = 0x8010006B,
            SCARD_W_CHV_BLOCKED = 0x8010006C,
            SCARD_W_EOF = 0x8010006D,
            SCARD_W_CANCELLED_BY_USER = 0x8010006E,
            SCARD_W_CARD_NOT_AUTHENTICATED = 0x8010006F
        }

        //**************************************************************************
        //   Protocol
        //**************************************************************************

        public enum CardProtocol : uint
        {
            SCARD_PROTOCOL_UNDEFINED = 0x0, // There is no active protocol.
            SCARD_PROTOCOL_T0 = 0x1, // T=0 is the active protocol.
            SCARD_PROTOCOL_T1 = 0x2, // T=1 is the active protocol.
            SCARD_PROTOCOL_RAW = 0x10000, // Raw is the active protocol.
            SCARD_PROTOCOL_DEFAULT = 0x80000000 // Use implicit PTS.
        }

        //**************************************************************************
        //   Reader State
        //**************************************************************************

        public enum CardState
        {
            SCARD_UNKNOWN = 0, // The driver is unaware of the current state of the reader.
            SCARD_ABSENT = 1, // Thhere is no card in the reader.
            SCARD_PRESENT = 2, // There is a card is present in the reader, but that it has not been moved into position for use.
            SCARD_SWALLOWED = 3, // There is a card in the reader in position for use.  The card is not powered.
            SCARD_POWERED = 4, // Power is being provided to the card, but the Reader Driver is unaware of the mode of the card.
            SCARD_NEGOTIABLE = 5, // The card has been reset and is awaiting PTS negotiation.
            SCARD_SPECIFIC = 6 // The card has been reset and specific communication protocols have been established.
        }

        //**************************************************************************
        // Prototypes
        //**************************************************************************
        
        private const string WINSCARD = "WinScard.dll";

        [DllImport(WINSCARD)]
        static extern Result SCardEstablishContext(
    		Scope  dwScope,
    		UInt32 pvReserved1,
    		UInt32 pvReserved2,
        out IntPtr context);

        [DllImport(WINSCARD)]
        static extern Result SCardReleaseContext(
            IntPtr context);

        [DllImport(WINSCARD, EntryPoint="SCardConnectA", CharSet=CharSet.Ansi)]
        static extern Result SCardConnect(
            IntPtr context,
            String szReaderName,
    		Sharing dwShareMode,
            CardProtocol dwPrefProtocol,
        out	IntPtr hCard,
        out	CardProtocol ActiveProtocol);
                                                         
        [DllImport(WINSCARD)]
        static extern Result SCardDisconnect(
            IntPtr hCard,
            Disposition disposition);

        [DllImport(WINSCARD)]
        static extern Result SCardBeginTransaction(
            IntPtr hCard);

        [DllImport(WINSCARD)]
        static extern Result SCardEndTransaction(
            IntPtr hCard,
    		UInt32 Disposition);

        [DllImport(WINSCARD)]
        static extern Result SCardState(
            IntPtr hCard,
        ref	CardState  state,
        ref	CardProtocol protocol,
        ref byte[] ATR,
        ref	UInt32 ATRLen);

        [DllImport(WINSCARD, EntryPoint="SCardStatusA", CharSet=CharSet.Ansi)] 
        static extern Result SCardStatus(
            IntPtr hCard,
            byte[] szReaderName,
    	ref	int    pcchReaderLen,
        ref	CardState state,
        ref	CardProtocol protocol,
            byte[] ATR,
        ref	int    ATRLen);

        [DllImport(WINSCARD)]
        static extern Result SCardTransmit(
            IntPtr hCard,
            IntPtr pioSendRequest,
            byte[] SendBuff,
            int    SendBuffLen,
            IntPtr pioRecvRequest,
            byte[] RecvBuff,
        ref int    RecvBuffLen);

        [DllImport(WINSCARD, EntryPoint="SCardListReadersA", CharSet=CharSet.Ansi)]
        static extern Result SCardListReaders(
            IntPtr context,
            string mzGroup,
            byte[] ReaderList,
    	ref	int    pcchReaders);

        [DllImport(WINSCARD)]
        static extern Result SCardCancel(
            IntPtr context);

        [DllImport(WINSCARD)]
        static extern Result SCardSetAttrib(
            IntPtr hCard,
    		UInt32 dwAttrId,
            byte[] pbAttr,
    		UInt32 lAttrLen);

        [DllImport(WINSCARD, EntryPoint="SCardGetStatusChangeA", CharSet=CharSet.Ansi)]
        static extern Result SCardGetStatusChange(
            IntPtr context,
    		int    dwTimeOut,
        ref SCARD_READERSTATE rgReaderState,
    	    int    cReaders);
    		
        //[DllImport(WINSCARD)]
        //static extern Result SCardControl(
        //    IntPtr hCard,
        //    UInt32 dwControlCode,
        //    UInt32 lpInBuffer,
        //    UInt32 lSizeofBuffer,
        //    VERSION_CONTROL lpReceiveBuffer,
        //    UInt32 lpReceiveBufferSize,
        //    UInt32 lpBytesReturned);

        [DllImport(WINSCARD)]
        static extern Result SCardGetAttrib(
            IntPtr hCard,
    		UInt32 dwAttrId,
            byte[] pbAttr,
    		UInt32 pcbAttrLen);


        //**********************************************
        //  private variables
        //**********************************************

        private Result _result;
        private IntPtr _context;
        private String _readerName;
        private IntPtr _cardHandle;

        protected CardState _state = CardState.SCARD_UNKNOWN;
        protected CardProtocol _protocol = CardProtocol.SCARD_PROTOCOL_UNDEFINED;
        protected byte _sak = 0;
        protected byte _tech = 0;
        protected byte[] _atr = new byte[0];
        protected byte[] _cardUID = new byte[0];
        protected byte[] _response = new byte[0];


        //*********************************************
        //  constructor
        //*********************************************

        public WinSmartCardReader() {}

        public WinSmartCardReader(String reader)
        {
            ConnectReader(reader);
        }

        //*********************************************
        //  property getters (and setters)
        //*********************************************

        public Result LastResult
        {
            get { return _result; }
        }

        public CardProtocol Protocol
        {
            get { return _protocol; }
        }

        public CardState State
        {
            get { return _state; }
        }

        //*********************************************
        //  public methods
        //*********************************************

        public bool ConnectReader(String readerRegex)
        {
            var readers = GetReaders();
            if (_context != IntPtr.Zero)
            {
                if (readers != null && readers.Length > 0)
                {
                    for (var i = 0; i < readers.Length; i++)
                    {
                        if (Regex.IsMatch(readers[i], readerRegex))
                        {
                            _readerName = readers[i];
                            return true;
                        }
                    }
                }
                SCardReleaseContext(_context);
            }
            _context = IntPtr.Zero;
            return false;
        }

        public string[] GetReaders()
        {
            _result = SCardEstablishContext(Scope.SCARD_SCOPE_USER, 0, 0, out _context);
            if (_result == Result.SCARD_S_SUCCESS)
            {
                var length = 250;
                var szReaderLists = new byte[length];
                var nul = new string[] { "\0" };

                _result = SCardListReaders(_context, null, szReaderLists, ref length);
                if (_result == Result.SCARD_S_SUCCESS)
                {
                    var allReaders = Encoding.Default.GetString(szReaderLists);
                    return allReaders.Split(nul, StringSplitOptions.RemoveEmptyEntries);
                }
                SCardReleaseContext(_context);
            }
            _context = IntPtr.Zero;
            return new string[0];
        }

        public bool WaitForCard(double seconds)
        {
            ReaderState maskedState;
            var rs = new SCARD_READERSTATE();
            rs.reader = _readerName;
            rs.userData = "";
            rs.eventState = ReaderState.SCARD_STATE_UNKNOWN;

            if (_context != IntPtr.Zero)
            {
                rs.currenState = ReaderState.SCARD_STATE_UNAWARE;
                _result = SCardGetStatusChange(_context, 0, ref rs, 1);

                if (_result == Result.SCARD_S_SUCCESS)
                {   /*
                     * If card not already present, wait required time
                     */
                    maskedState = rs.eventState & ReaderState.SCARD_STATE_PRESENT;
                    if (maskedState != ReaderState.SCARD_STATE_PRESENT && seconds > 0)
                    {
                        rs.currenState = rs.eventState;
                        _result = SCardGetStatusChange(_context, (int)(seconds * 1000), ref rs, 1);
                    }
                }
            }
            maskedState = rs.eventState & ReaderState.SCARD_STATE_PRESENT;
            return maskedState == ReaderState.SCARD_STATE_PRESENT;
        }

        public bool ConnectCard()
        {
            if (_context != IntPtr.Zero) 
            {
                if (_cardHandle != IntPtr.Zero)
                    DisconnectCard();

                _result = SCardConnect(_context, _readerName, Sharing.SCARD_SHARE_EXCLUSIVE,
                    CardProtocol.SCARD_PROTOCOL_T0 | CardProtocol.SCARD_PROTOCOL_T1, out _cardHandle, out _protocol);

                if (_result == Result.SCARD_S_SUCCESS)
                {
                    var lenReaderList = 255;
                    var bytReaderList = new byte[lenReaderList];
                    var lenAtr = 255;
                    var bytAtr = new byte[lenAtr];

                    _result = SCardStatus(_cardHandle, bytReaderList, ref lenReaderList, ref _state, ref _protocol, bytAtr, ref lenAtr);
                    if (_result == Result.SCARD_S_SUCCESS)
                    {
                        _atr = new byte[lenAtr];
                        Array.Copy(bytAtr, _atr, lenAtr);
                        return true;
                    }
                }
                //else
                //{
                //    DisconnectCard();
                //    throw new SmartCardException(_result.ToString());
                //}
            }
            DisconnectCard();
            return false;
        }

        public bool DisconnectCard()
        {
            try
            {
                if (_cardHandle != IntPtr.Zero)
                {
                    _result = SCardDisconnect(_cardHandle, Disposition.SCARD_LEAVE_CARD);
                }
            }
            finally
            { 
                _cardHandle = IntPtr.Zero;
                _state = CardState.SCARD_ABSENT;
                _protocol = CardProtocol.SCARD_PROTOCOL_UNDEFINED;
                _atr = new byte[0];
                _cardUID = new byte[0];
                _response = new byte[0];
            }
            return true;
        }

        public bool WaitForRemoval(double seconds)
        {
            var rs = new SCARD_READERSTATE();
            rs.reader = _readerName;
            rs.userData = "";
            rs.eventState = ReaderState.SCARD_STATE_UNKNOWN;

            if (_context != IntPtr.Zero)
            {
                rs.currenState = ReaderState.SCARD_STATE_UNAWARE;
                _result = SCardGetStatusChange(_context, 0, ref rs, 1);

                if (_result == Result.SCARD_S_SUCCESS)
                {   /*
                     * If card  present, wait required time
                     */
                    if ((rs.eventState & ReaderState.SCARD_STATE_EMPTY) == 0 && seconds > 0)
                    {
                        rs.currenState = rs.eventState;
                        _result = SCardGetStatusChange(_context, (int)(seconds * 1000), ref rs, 1);
                    }
                }
            }
            var maskedState = rs.eventState & ReaderState.SCARD_STATE_EMPTY;
            return maskedState != 0;
        }

        public byte[] Transmit(byte[] request)
        {
            _response = new byte[0];

            if (_context != IntPtr.Zero && _cardHandle != IntPtr.Zero)
            {
                var lenRecv = 100;
                var recv = new byte[lenRecv];

                _result = SCardTransmit(_cardHandle, IntPtr.Zero, request, request.Length, IntPtr.Zero, recv, ref lenRecv);
                if (_result == Result.SCARD_S_SUCCESS)
                {
                    _response = new byte[lenRecv];
                    Array.Copy(recv, _response, lenRecv);
                }
            }
            return _response;
        }

        public string ResultText()
        {
            return ResultText(_result);
        }

        public string ResultText(Result result)
        {
            switch (result)
            {
                case Result.SCARD_S_SUCCESS:
                    return "Success";
		        case Result.SCARD_F_INTERNAL_ERROR:
 			        return "An internal consistency check failed.";
		        case Result.SCARD_E_CANCELLED:
 			        return "The action was cancelled by an SCardCancel request.";
		        case Result.SCARD_E_INVALID_HANDLE:
 			        return "The supplied handle was invalid.";
		        case Result.SCARD_E_INVALID_PARAMETER:
 			        return "One or more of the supplied parameters could not be properly interpreted.";
		        case Result.SCARD_E_INVALID_TARGET:
 			        return "Registry startup information is missing or invalid.";
		        case Result.SCARD_E_NO_MEMORY:
 			        return "Not enough memory available to complete this command.";
		        case Result.SCARD_F_WAITED_TOO_LONG:
 			        return "An internal consistency timer has expired.";
		        case Result.SCARD_E_INSUFFICIENT_BUFFER:
 			        return "The data buffer to receive returned data is too small for the returned data.";
		        case Result.SCARD_E_UNKNOWN_READER:
 			        return "The specified reader name is not recognized.";
		        case Result.SCARD_E_TIMEOUT:
 			        return "The user-specified timeout value has expired.";
		        case Result.SCARD_E_SHARING_VIOLATION:
 			        return "The smart card cannot be accessed because of other connections outstanding.";
		        case Result.SCARD_E_NO_SMARTCARD:
 			        return "The operation requires a Smart Card, but no Smart Card is currently in the device.";
		        case Result.SCARD_E_UNKNOWN_CARD:
 			        return "The specified smart card name is not recognized.";
		        case Result.SCARD_E_CANT_DISPOSE:
 			        return "The system could not dispose of the media in the requested manner.";
		        case Result.SCARD_E_PROTO_MISMATCH:
 			        return "The requested protocols are incompatible with the protocol currently in use with the smart card.";
		        case Result.SCARD_E_NOT_READY:
 			        return "The reader or smart card is not ready to accept commands.";
		        case Result.SCARD_E_INVALID_VALUE:
 			        return "One or more of the supplied parameters values could not be properly interpreted.";
		        case Result.SCARD_E_SYSTEM_CANCELLED:
 			        return "The action was cancelled by the system, presumably to log off or shut down.";
		        case Result.SCARD_F_COMM_ERROR:
 			        return "An internal communications error has been detected.";
		        case Result.SCARD_F_UNKNOWN_ERROR:
 			        return "An internal error has been detected, but the source is unknown.";
		        case Result.SCARD_E_INVALID_ATR:
 			        return "An ATR obtained from the registry is not a valid ATR string.";
		        case Result.SCARD_E_NOT_TRANSACTED:
 			        return "An attempt was made to end a non-existent transaction.";
		        case Result.SCARD_E_READER_UNAVAILABLE:
 			        return "The specified reader is not currently available for use.";
		        case Result.SCARD_P_SHUTDOWN:
 			        return "The operation has been aborted to allow the server application to exit.";
		        case Result.SCARD_E_PCI_TOO_SMALL:
 			        return "The PCI Receive buffer was too small.";
		        case Result.SCARD_E_READER_UNSUPPORTED:
 			        return "The reader driver does not meet minimal requirements for support.";
		        case Result.SCARD_E_DUPLICATE_READER:
 			        return "The reader driver did not produce a unique reader name.";
		        case Result.SCARD_E_CARD_UNSUPPORTED:
 			        return "The smart card does not meet minimal requirements for support.";
		        case Result.SCARD_E_NO_SERVICE:
 			        return "The Smart card resource manager is not running.";
		        case Result.SCARD_E_SERVICE_STOPPED:
 			        return "The Smart card resource manager has shut down.";
		        case Result.SCARD_E_UNEXPECTED:
                    return "An unexpected card error has occurred, or this smart card does not support the requested feature.";
		        case Result.SCARD_E_ICC_INSTALLATION:
 			        return "No primary provider can be found for the smart card.";
		        case Result.SCARD_E_ICC_CREATEORDER:
 			        return "The requested order of object creation is not supported.";
		        case Result.SCARD_E_DIR_NOT_FOUND:
 			        return "The identified directory does not exist in the smart card.";
		        case Result.SCARD_E_FILE_NOT_FOUND:
 			        return "The identified file does not exist in the smart card.";
		        case Result.SCARD_E_NO_DIR:
 			        return "The supplied path does not represent a smart card directory.";
		        case Result.SCARD_E_NO_FILE:
 			        return "The supplied path does not represent a smart card file.";
		        case Result.SCARD_E_NO_ACCESS:
 			        return "Access is denied to this file.";
		        case Result.SCARD_E_WRITE_TOO_MANY:
 			        return "The smart card does not have enough memory to store the information.";
		        case Result.SCARD_E_BAD_SEEK:
 			        return "There was an error trying to set the smart card file object pointer.";
		        case Result.SCARD_E_INVALID_CHV:
 			        return "The supplied PIN is incorrect.";
		        case Result.SCARD_E_UNKNOWN_RES_MNG:
 			        return "An unrecognized error code was returned from a layered component.";
		        case Result.SCARD_E_NO_SUCH_CERTIFICATE:
 			        return "The requested certificate does not exist.";
		        case Result.SCARD_E_CERTIFICATE_UNAVAILABLE:
 			        return "The requested certificate could not be obtained.";
		        case Result.SCARD_E_NO_READERS_AVAILABLE:
 			        return "Cannot find a smart card reader.";
		        case Result.SCARD_E_COMM_DATA_LOST:
 			        return "A communications error with the smart card has been detected.";
		        case Result.SCARD_E_NO_KEY_CONTAINER:
 			        return "The requested key container does not exist on the smart card.";
		        case Result.SCARD_E_SERVER_TOO_BUSY:
 			        return "The Smart Card Resource Manager is too busy to complete this operation.";
		        case Result.SCARD_W_UNSUPPORTED_CARD:
 			        return "The reader cannot communicate with the card, due to ATR string configuration conflicts.";
		        case Result.SCARD_W_UNRESPONSIVE_CARD:
 			        return "The smart card is not responding to a reset.";
		        case Result.SCARD_W_UNPOWERED_CARD:
 			        return "Power has been removed from the smart card, so that further communication is not possible.";
		        case Result.SCARD_W_RESET_CARD:
 			        return "The smart card has been reset, so any shared state information is invalid.";
		        case Result.SCARD_W_REMOVED_CARD:
 			        return "The smart card has been removed, so further communication is not possible.";
		        case Result.SCARD_W_SECURITY_VIOLATION:
 			        return "Access was denied because of a security violation.";
		        case Result.SCARD_W_WRONG_CHV:
 			        return "The card cannot be accessed because the wrong PIN was presented.";
		        case Result.SCARD_W_CHV_BLOCKED:
 			        return "The card cannot be accessed because the maximum number of PIN entry attempts has been reached.";
		        case Result.SCARD_W_EOF:
 			        return "The end of the smart card file has been reached.";
		        case Result.SCARD_W_CANCELLED_BY_USER:
 			        return "The user pressed \"Cancel\" on a Smart Card Selection Dialog.";
		        case Result.SCARD_W_CARD_NOT_AUTHENTICATED:
 			        return "No PIN was presented to the smart card.";
                default:
                    return "Unknown Error: " + LastResult.ToString("X");
            }
        }





    }
}
