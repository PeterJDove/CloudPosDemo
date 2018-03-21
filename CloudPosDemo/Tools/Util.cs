using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms.Layout;

namespace Touch.Tools
{
    /// <summary>
    /// A static class of various Utility functions.
    /// </summary>
    public static class Util
    {

        /// <summary>
        /// Converts a hex string to an array of bytes.
        /// </summary>
        /// <param name="hex">A string of Hex digits. (Alpha digits, A-F, may be upper or lower case)</param>
        /// <returns>An array of bytes</returns>
        public static byte[] HexStringToByteArray(string hex)
        {
            var data = hex.Trim().Replace(" ", "").Replace("-", "");

            var byteArray = new byte[data.Length / 2];
            var n = 0;
            for (var i = 0; i < data.Length; i += 2)
            {
                byteArray[n++] = byte.Parse(data.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            return byteArray;
        }

        /// <summary>
        /// Converts an array of bytes to a string of Hex digits.
        /// </summary>
        /// <remarks>
        /// This function returns the same thing as <code>BitConverter.ToString(data)</code>
        /// (indeed uses that fuction), but with the dashes removed.
        /// </remarks>
        /// <param name="data">An array of bytes</param>
        /// <returns>a string of Hex digits</returns>
        public static string ByteArrayToHexString(byte[] data)
        {
            var hex = BitConverter.ToString(data);
            return hex.Replace("-", "");
        }

        /// <summary>
        /// Returns a String consisting of a dump of the contents of a byte array, suitable for use in program
        /// logs, or for debugging.  Each byte is shown as a two-digit hexadecimal number and, on the line below,
        /// as its character representation.  Non-printable (control) characters are represented by a "middle dot": ·
        /// <para>The dump is arranged in lines of a fixed number of bytes; 20 by default but able to be varied
        /// using the optional <paramref name="bytesPerLine"/> parameter.</para>
        /// </summary>
        /// <remarks>
        /// The start of each line contains the zero-based address or index of the first byte in that line.  This
        /// index is shown in decimal, unless <paramref name="bytesPerLine"/> is a multiple of eight. In this
        /// case the address/index column is shown as a hexadecimal number (with a "0x" prefix).
        /// </remarks>
        /// <param name="data">Array of bytes to be dumped</param>
        /// <param name="bytesPerLine">Number of bytes to show on a line.  Defaults to 20.</param>
        /// <returns>Human-readable dump of byte array, with embedded newline characters</returns>
        public static string HexDump(byte[] data, int bytesPerLine = 20)
        {
            StringBuilder hex = null;
            StringBuilder asc = null;
            var dump = new StringBuilder("Length = " + data.Length + " (0x" + data.Length.ToString("X2") + ")\n");

            for (var i = 0; i < data.Length; i++)
            {
                if (i % bytesPerLine == 0)
                {
                    if (hex != null)
                    {
                        dump.Append(hex + "\n");
                        dump.Append(asc + "\n");
                    }
                    if (bytesPerLine % 8 == 0) // If bytesPerLine is a multiple of eight...
                    {
                        hex = new StringBuilder("» 0x" + i.ToString("X4") + ": "); // ...display addresses in Hex
                        asc = new StringBuilder("»         "); // 9 spaces
                    }
                    else
                    {
                        hex = new StringBuilder("» " + i.ToString("0000") + ": "); // Decimal by default
                        asc = new StringBuilder("»       "); // 7 spaces
                    }
                }
                if (hex != null) // Which should always be true, but Resharper was worried.
                {
                    hex.Append(data[i].ToString("X2") + " ");
                    if (Char.IsControl((char)data[i]))
                        asc.Append("·  "); // Middle Dot, Alt+0183
                    else
                        asc.Append((char)data[i] + "  ");
                }
            }
            if (hex != null)
            {
                dump.Append(hex + "\n");
                dump.Append(asc + "\n");
            }
            return dump.ToString();
        }

        public static T Execute<T>(Func<T> func, int timeout)
        {
            T result;
            TryExecute(func, timeout, out result);
            return result;
        }

        public static bool TryExecute<T>(Func<T> func, int timeout, out T result)
        {
            var t = default(T);
            var completed = false;
            var thread = new Thread(() => t = func());

            thread.Start();

            completed = thread.Join(timeout);

            if (!completed)
                thread.Abort();

            result = t;

            return completed;
        }
        
        public static ushort IpChecksum(byte[] buffer, int start, int length)
        {
            long sum = 0;
            var endIndex = length + start;
            for (var i = start; i < endIndex; i += 2)
            {
                var word16 = (ushort)((buffer[i] << 8) & 0xFF00);
                if (i + 1 < endIndex)
                    word16 += (ushort)(buffer[i + 1] & 0xFF);

                sum += word16;
            }
            while ((sum >> 16) != 0)
            {
                sum = (sum & 0xFFFF) + (sum >> 16);
            }
            return (ushort)(~sum);
        }

        public static void TestComputeIpChecksum()
        {
            TestIpChecksum(0x392C, new byte[]{
                0xC0, 0xA8, 0x01, 0x1F, 0xC0, 0xA8, 0x01, 0x81, 0x00, 0x06, 0x00, 0x2D,
                0x00, 0x16, 0x04, 0x78, 0xB3, 0xD4, 0xBC, 0x17, 0x00, 0x00, 0xCC, 0x47,
                0x50, 0x18, 0x16, 0xD0, 0x00, 0x00, 0x00, 0x00, 0x53, 0x53, 0x48, 0x2D,
                0x31, 0x2E, 0x39, 0x39, 0x2D, 0x4F, 0x70, 0x65, 0x6E, 0x53, 0x53, 0x48,
                0x5F, 0x32, 0x2E, 0x35, 0x2E, 0x32, 0x70, 0x32, 0x0A });

            TestIpChecksum(0x15D4, new byte[]{0x45, 0x00, 0x00, 0x3C, 0x00, 0xFD, 0x00,
                0x00, 0x20, 0x01, 0x00, 0x00, 0xC0, 0xA8, 0x01, 0x81, 0xC0, 0xA8, 0x01, 0x1F });

            TestIpChecksum(0xFFFE, new byte[]{0x77, 0x77, 0x77, 0x77, 0x88, 0x88, 0x88,
                0x89, 0x77, 0x77, 0x77, 0x77, 0x88, 0x88, 0x88, 0x88 });
        }

        private static void TestIpChecksum(ushort expected, byte[] data)
        {
            var checksum = IpChecksum(data, 0, data.Length);
            System.Windows.Forms.MessageBox.Show("Data Length = " + data.Length + "\n" +
                "Checksum = 0x" + checksum.ToString("X") + "\n" +
                "   Expected = 0x" + expected.ToString("X"), (checksum == expected).ToString());
        }

        public static bool EnsureFolderExists(string path)
        {
            if (path.EndsWith("\\"))
                path = path.Substring(0, path.Length - 1);

            if (Directory.Exists(path))
                return true;

            if (File.Exists(path))
                return false;

            var parent = Directory.GetParent(path);
            if (parent == null)
                return false;

            if (EnsureFolderExists(parent.FullName))
            {
                Directory.CreateDirectory(path);
                return true;
            }
            return false;
        }
    }
}
