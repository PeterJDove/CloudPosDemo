using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Touch.Tools
{
    /// <summary>
    /// Provides a wrapper for reading and writing settings in an old-style Windows .INI file.
    /// </summary>
    public class IniFile
    {
        private string _path;
        private string _dateFormat = "dd-MMM-yyyy hh:mm:ss";
        private string _hexPrefix = "0x";
        private string _obfuscationKey = "";
        private IniColorFormat _colorFormat = IniColorFormat.NameIfKnown;

        [Flags]
        public enum IniColorFormat
        {
            Decimal = 0,
            Hexadecimal = 1,
            NameIfKnown = 2,
            SystemColors = 4,
        }

        //private const object NullColor = Color.Empty;

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
            string key, string Default, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileSectionA", CharSet = CharSet.Ansi)]
        private static extern int GetPrivateProfileSection(string section,
            byte[] data, int size, string filePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileSectionNamesA", CharSet = CharSet.Ansi)]
        static extern int GetPrivateProfileSectionNames(byte[] data, int size, string filePath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int WritePrivateProfileSection(string section,
            string lpstring, string filePath);

        /// <summary>
        /// Contructor, which takes the path of the .INI file 
        /// </summary>
        /// <param name="path">Full path (file name) of .INI to be accessed.  An instance of IniFile will still be returned, even if the file does not exist.</param>
        public IniFile(string path)
        {
            _path = path;
        }

        /// <summary>
        /// Set or return the path (file name) if the .INI file.
        /// </summary>
        /// <exception cref="FileNotFoundException">Unlike the constructor, setting Path will throw an exception if the file is not found.</exception>
        public string Path
        {
            get { return _path; }
            set
            {
                if (File.Exists(value))
                    _path = value;
                else
                    throw new FileNotFoundException("File not found: " + value, value);
            }
        }

        public bool FileExists()
        {
            return FileExists(_path);
        }

        public bool FileExists(string filename)
        {
            if (filename == null) // Uses win.ini
                return true;
            else
                return File.Exists(filename);
        }

        public IniColorFormat ColorFormat
        {
            get { return _colorFormat; }
            set { _colorFormat = value; }
        }

        public string DateFormat
        {
            get { return _dateFormat; }
            set { _dateFormat = value; }
        }

        public string HexPrefix
        {
            get { return _hexPrefix; }
            set { _hexPrefix = value; }
        }

        public string ObfuscationKey
        {
            get { return _obfuscationKey; }
            set { _obfuscationKey = value; }
        }

        //
        //  Getter Methods
        //
        public string GetString(string section, string key, object Default = null)
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException();

            const string noDefault = "~*<NO_DEFAULT>*~";
            string defaultValue;

            if (Default == null)
                defaultValue = noDefault;
            else
                defaultValue = Default.ToString();

            var value = new StringBuilder(256);
            GetPrivateProfileString(section, key, defaultValue, value, 255, _path);
            if (value.ToString().Equals(noDefault))
                return null;

            return value.ToString();
        }

        public char GetChar(string section, string key, char Default = '\0')
        {
            int value;
            if (Int32.TryParse(GetString(section, key, (int)Default), out value))
                return (char)value;

            return Default;
        }

        public byte GetByte(string section, string key, byte Default = 0)
        {
            byte value;
            if (Byte.TryParse(GetString(section, key, Default), out value))
                return value;

            return Default;
        }

        public int GetInt(string section, string key, int Default = 0)
        {
            int value;
            if (Int32.TryParse(GetString(section, key, Default), out value))
                return value;

            return Default;
        }

        public long GetLong(string section, string key, long Default = 0)
        {
            long value;
            if (Int64.TryParse(GetString(section, key, Default), out value))
                return value;

            return Default;
        }

        public double GetDouble(string section, string key, double Default = 0)
        {
            double value;
            if (Double.TryParse(GetString(section, key, Default), out value))
                return value;

            return Default;
        }

        public decimal GetDecimal(string section, string key, decimal Default = 0)
        {
            decimal value;
            if (Decimal.TryParse(GetString(section, key, Default), out value))
                return value;

            return Default;
        }

        public bool GetBoolean(string section, string key, bool Default = false)
        {
            var value = GetString(section, key, Default);
            long number;
            if (Int64.TryParse(value, out number))
            {
                return (number != 0);
            }
            value = value.Trim().ToLower();
            if ("true".Equals(value) || "t".Equals(value) ||
                "yes".Equals(value) || "y".Equals(value) || "on".Equals(value))
                return true;

            if ("false".Equals(value) || "f".Equals(value) ||
                "no".Equals(value) || "n".Equals(value) || "off".Equals(value))
                return false;

            return Default;
        }

        public DateTime GetDate(string section, string key, DateTime Default = new DateTime())
        {
            try
            {
                var value = GetString(section, key, Default.ToString(_dateFormat));
                var provider = System.Globalization.DateTimeFormatInfo.InvariantInfo;
                return DateTime.ParseExact(value, _dateFormat, provider);
            }
            catch (Exception ex)
            {
                ex.GetType(); // suppress variable not used warning
                return Default;
            }
        }

        public Color GetColor(string section, string key, Color Default = new Color())
        {
            var value = GetString(section, key);
            int alpha = 0;

            if (int.TryParse(value, out alpha))
                return Color.FromArgb(alpha); // Actually an ARGB value
            else
            {
                value = value.Replace(" ", "").ToLower();
                if (value.EndsWith(")") && value.StartsWith("rgb(") || value.StartsWith("argb("))
                {
                    int red = 0;
                    int green = 0;
                    int blue = 0;

                    if (value.StartsWith("rgb("))
                        value = value.Substring(4, value.Length - 5);
                    else // argb(
                        value = value.Substring(5, value.Length - 6);

                    var v = value.Split(',');
                    if (v.Length == 3)
                    {
                        if (int.TryParse(v[0], out red)
                        && int.TryParse(v[1], out green)
                        && int.TryParse(v[2], out blue))
                            return Color.FromArgb(red, green, blue);
                    }
                    else if (v.Length == 4)
                    {
                        if (int.TryParse(v[0], out alpha)
                        && int.TryParse(v[1], out red)
                        && int.TryParse(v[2], out green)
                        && int.TryParse(v[3], out blue))
                            return Color.FromArgb(alpha, red, green, blue);
                    }
                }
                else if (value.StartsWith("#"))
                {
                    value = value.Substring(1).Trim();
                    int argb = 0;
                    if (int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out argb))
                    {
                        if (value.Length <= 6)
                            argb |= Color.Black.ToArgb(); // Set Alpha = 255

                        return Color.FromArgb(argb);
                    }
                }
                else
                    return Color.FromName(value);
            }
            return Default;
        }

        public Dictionary<string, string> GetSection(string section)
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException();
            var dict = new Dictionary<string, string>();

            var length = 4000;
            var data = new byte[length];
            var nul = new string[] { "\0" };

            length = GetPrivateProfileSection(section, data, length, _path);
            if (length > 0)
            {
                var allItems = Encoding.Default.GetString(data);
                var item = allItems.Split(nul, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < item.Length; i++)
                {
                    var eq = item[i].IndexOf('=');
                    dict.Add(item[i].Substring(0, eq), item[i].Substring(eq + 1));
                }
            }
            return dict;
        }

        public string[] GetSectionNames()
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException();
            var length = 4000;
            var data = new byte[length];
            var nul = new string[] { "\0" };

            length = GetPrivateProfileSectionNames(data, length, _path);
            if (length > 0)
            {
                var allItems = Encoding.Default.GetString(data);
                return allItems.Split(nul, StringSplitOptions.RemoveEmptyEntries);
            }
            return new string[0];
        }


        //
        //  Setter Methods
        //

        /// <summary>
        /// Clears a section, but does not remove - or move - the existing [SECTION_NAME].
        /// </summary>
        /// <param name="section"></param>
        public void ClearOutSection(string section)
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException();

            var dict = GetSection(section);
            if (dict != null)
                foreach (var key in dict.Keys)
                    EraseKey(section, key);
        }

        public void EraseSection(string section)
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException();

            WritePrivateProfileSection(section, null, _path);
        }

        public void EraseKey(string section, string key)
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException();

            WritePrivateProfileString(section, key, null, _path);
        }

        /// <summary>
        /// Write a String to the .INI file.
        /// </summary>
        /// <param name="section">Section name under which setting is to be written.  In the .INI file, section names are bound in square brackets.</param>
        /// <param name="key">The key of the setting; unique within Section Name.</param>
        /// <param name="value">The value of the string to be written.  If <b>null</b> (but not an empty string), the setting will be erased.</param>
        /// <param name="Default">The default value of this setting.  If <paramref name="value"/> equals <paramref name="Default"/>, the setting is erased.</param>
        public void Write(string section, string key, string value, string Default = null)
        {
            //if (!File.Exists(_path))
            //    throw new FileNotFoundException();
            if (value == null)
                EraseKey(section, key);
            else
            {
                if (Default != null && value.Equals(Default))
                    EraseKey(section, key);
                else
                {
                    if (value.Trim().Length < value.Length)
                        value = '\"' + value + '\"';

                    WritePrivateProfileString(section, key, value, _path);
                }
            }
        }

        /// <summary>
        /// Write the Unicode Integer representation of a Char to the .INI file.
        /// </summary>
        /// <param name="section">Section name under which setting is to be written.  In the .INI file, section names are bound in square brackets.</param>
        /// <param name="key">The key of the setting; unique within Section Name.</param>
        /// <param name="value">The Char to be written.</param>
        /// <param name="Default">The default value of this setting.  If <paramref name="value"/> equals <paramref name="Default"/>, the setting is erased.</param>
        public void Write(string section, string key, char value, char? Default = null)
        {
            if (Default == null)
                Write(section, key, (int)value, null);
            else
                Write(section, key, (int)value, (int)Default);
        }

        /// <summary>
        /// Write an Integer to the .INI file.
        /// </summary>
        /// <param name="section">Section name under which setting is to be written.  In the .INI file, section names are bound in square brackets.</param>
        /// <param name="key">The key of the setting; unique within Section Name.</param>
        /// <param name="value">The value of the Integer to be written.</param>
        /// <param name="Default">The default value of this setting.  If <paramref name="value"/> equals <paramref name="Default"/>, the setting is erased.</param>
        public void Write(string section, string key, int value, int? Default = null)
        {
            if (Default == null)
                Write(section, key, value.ToString(), null);
            else
                Write(section, key, value.ToString(), Default.ToString());
        }

        /// <summary>
        /// Write a Long to the .INI file.
        /// </summary>
        /// <param name="section">Section name under which setting is to be written.  In the .INI file, section names are bound in square brackets.</param>
        /// <param name="key">The key of the setting; unique within Section Name.</param>
        /// <param name="value">The value of the Long to be written.</param>
        /// <param name="Default">The default value of this setting.  If <paramref name="value"/> equals <paramref name="Default"/>, the setting is erased.</param>
        public void Write(string section, string key, long value, long? Default = null)
        {
            if (Default == null)
                Write(section, key, value.ToString(), null);
            else
                Write(section, key, value.ToString(), Default.ToString());
        }

        /// <summary>
        /// Write a Double to the .INI file.
        /// </summary>
        /// <param name="section">Section name under which setting is to be written.  In the .INI file, section names are bound in square brackets.</param>
        /// <param name="key">The key of the setting; unique within Section Name.</param>
        /// <param name="value">The Double to be written.</param>
        /// <param name="Default">The default value of this setting.  If <paramref name="value"/> equals <paramref name="Default"/>, the setting is erased.</param>
        public void Write(string section, string key, double value, double? Default = null)
        {
            if (Default == null)
                Write(section, key, value.ToString(), null);
            else
                Write(section, key, value.ToString(), Default.ToString());
        }

        /// <summary>
        /// Write a Decimal to the .INI file.
        /// </summary>
        /// <param name="section">Section name under which setting is to be written.  In the .INI file, section names are bound in square brackets.</param>
        /// <param name="key">The key of the setting; unique within Section Name.</param>
        /// <param name="value">The Decimal to be written.</param>
        /// <param name="Default">The default value of this setting.  If <paramref name="value"/> equals <paramref name="Default"/>, the setting is erased.</param>
        public void Write(string section, string key, decimal value, decimal? Default)
        {
            if (Default == null)
                Write(section, key, value.ToString(), null);
            else
                Write(section, key, value.ToString(), Default.ToString());
        }

        /// <summary>
        /// Write a Boolean (true/false) to the .INI file.
        /// </summary>
        /// <param name="section">Section name under which setting is to be written.  In the .INI file, section names are bound in square brackets.</param>
        /// <param name="key">The key of the setting; unique within Section Name.</param>
        /// <param name="value">The Boolean to be written.</param>
        /// <param name="Default">The default value of this setting.  If <paramref name="value"/> equals <paramref name="Default"/>, the setting is erased.</param>
        public void Write(string section, string key, bool value, bool? Default = null)
        {
            if (Default == null)
                Write(section, key, value.ToString(), null);
            else
                Write(section, key, value.ToString(), Default.ToString());
        }

        /// <summary>
        /// Write a DateTime to the .INI file, using the format specified by the IniFile's <see cref="DateFormat"/> string.
        /// </summary>
        /// <remarks>
        /// As there is not "Default" (default) parameter, the value is always written.
        /// </remarks>
        /// <param name="section">Section name under which setting is to be written.  In the .INI file, section names are bound in square brackets.</param>
        /// <param name="key">The key of the setting; unique within Section Name.</param>
        /// <param name="value">The DateTime to be written.</param>
        public void Write(string section, string key, DateTime value)
        {
            Write(section, key, value.ToString(_dateFormat));
        }

        /// <summary>
        /// Write a DateTime to the .INI file, using the format specified by the IniFile's <see cref="DateFormat"/> string.
        /// </summary>
        /// <param name="section">Section name under which setting is to be written.  In the .INI file, section names are bound in square brackets.</param>
        /// <param name="key">The key of the setting; unique within Section Name.</param>
        /// <param name="value">The DateTime to be written.</param>
        /// <param name="Default">The default value of this setting.  If <paramref name="value"/> equals <paramref name="Default"/>, the setting is erased.</param>
        public void Write(string section, string key, DateTime value, DateTime Default)
        {
            Write(section, key, value.ToString(_dateFormat), Default.ToString(_dateFormat));
        }

        /// <summary>
        /// Write a String to the .INI file, where that String is merely the result of calling .ToString() on an arbitrary object.
        /// <para>This method is offered as a convenient alternative to the basic String form of the Write command, for use when the
        /// value is readily obtained from another object, such as a StringBuilder.  There is no corresponding "Get" for arbitrary
        /// objects.</para>
        /// </summary>
        /// <param name="section">Section name under which setting is to be written.  In the .INI file, section names are bound in square brackets.</param>
        /// <param name="key">The key of the setting; unique within Section Name.</param>
        /// <param name="value">The object which is to supply the string value.</param>
        /// <param name="Default">An object which is to supply default value of this setting.
        /// If <code><paramref name="value"/>.ToString().Equals(<paramref name="Default"/>.ToString())</code>, the setting is erased.</param>
        public void Write(string section, string key, Object value, Object Default = null)
        {
            if (Default == null)
                Write(section, key, value.ToString(), null);
            else
                Write(section, key, value.ToString(), Default.ToString());
        }

        public void Write(string section, string key, Color color)
        {
            Write(section, key, color, Color.Empty);
        }

        public void Write(string section, string key, Color color, Color Default)
        {
            if (color == Default)
                EraseKey(section, key);
            else
            {
                string value;
                if (_colorFormat < IniColorFormat.NameIfKnown
                    || TryKnownColors(color, out value) == false)
                {
                    if ((_colorFormat & IniColorFormat.Hexadecimal) != 0)
                    {
                        value = "#";
                        if (color.A < 255)
                            value += color.A.ToString("X2");

                        value += color.R.ToString("X2");
                        value += color.G.ToString("X2");
                        value += color.B.ToString("X2");
                    }
                    else
                    {
                        if (color.A < 255)
                            value = "argb(" + color.A + ",";
                        else
                            value = "rgb(";

                        value += color.R + "," + color.G + "," + color.B + ")";
                    }
                }
                Write(section, key, value);
            }
        }

        private bool TryKnownColors(Color color, out string name)
        {
            var argb = color.ToArgb();
            foreach (var knownName in Enum.GetNames(typeof(KnownColor)))
            {
                var knownColor = Color.FromName(knownName);
                if (knownColor.ToArgb() == argb)
                {
                    if (knownColor.IsSystemColor)
                    {
                        if ((_colorFormat & IniColorFormat.SystemColors) != 0)
                        {
                            name = knownColor.Name;
                            return true;
                        }
                    }
                    else if ((_colorFormat & IniColorFormat.NameIfKnown) != 0)
                    {
                        name = knownColor.Name;
                        return true;
                    }
                }
            }
            name = null;
            return false;
        }

        public static void SelfTest()
        {
            var ini = new IniFile(@"c:\touchtools.ini");

            var section = "STRINGS";
            ini.Write(section, "path", ini.Path);
            ini.Write(section, "ini", ini);
            ini.Write(section, "whitespace", "    ");
            ini.Write(section, "notseen", ini, "Touch.Tools.IniFile");
            ini.Write(section, "visible", "light", "dark");

            section = "BYTES";
            ini.Write(section, "zero", (byte)0);
            ini.Write(section, "one", (byte)1);
            ini.Write(section, "max", Byte.MaxValue);
            ini.Write(section, "min", Byte.MinValue, (byte)42);
            ini.Write(section, "notseen", (byte)0, (byte)0);
            ini.Write(section, "notseen", (byte)255, (byte)255);
            ini.Write(section, "notvalid", "not a number");

            section = "INTEGERS";
            ini.Write(section, "zero", 0);
            ini.Write(section, "one", 1);
            ini.Write(section, "max", Int32.MaxValue);
            ini.Write(section, "min", Int32.MinValue, 42);
            ini.Write(section, "notseen", (int)0, (int)0);
            ini.Write(section, "notseen", 255, 255);
            ini.Write(section, "notvalid", "not a number");

            section = "LONGS";
            ini.Write(section, "zero", 0L);
            ini.Write(section, "one", 1L);
            ini.Write(section, "max", Int64.MaxValue);
            ini.Write(section, "min", Int64.MinValue, 42L);
            ini.Write(section, "notseen", 0L, 0L);
            ini.Write(section, "notseen", 255L, 255L);
            ini.Write(section, "notvalid", "not a number");

            section = "DOUBLES";
            ini.Write(section, "zero", 0D);
            ini.Write(section, "one", 1D);
            ini.Write(section, "max", Double.MaxValue);
            ini.Write(section, "min", Double.MinValue, 42D);
            ini.Write(section, "notseen", 0D, 0D);
            ini.Write(section, "notseen", 255D, 255D);
            ini.Write(section, "notvalid", "not a number");

            section = "BOOLEANS";
            ini.Write(section, "fact", true);
            ini.Write(section, "lie", false);
            ini.Write(section, "fact_XOR", true, false);
            ini.Write(section, "lie_XOR", false, true);
            ini.Write(section, "notseen", true, true);
            ini.Write(section, "notseen", false, false);
            ini.Write(section, "yes", true);
            ini.Write(section, "no", false);
            ini.Write(section, "notvalid", "not a boolean");

            section = "DATETIMES";
            ini.Write(section, "default", new DateTime());
            ini.Write(section, "now", DateTime.Now);
            ini.Write(section, "notseen", DateTime.Now, DateTime.Now);
            ini.Write(section, "notvalid", "not a valid datetime");

            section = "SACRIFICIAL";
            ini.Write(section, "path", ini.Path);
            ini.Write(section, "ini", ini);
            ini.Write(section, "whitespace", "    ");
            ini.Write(section, "notseen", ini, "Touch.Tools.IniFile");
            ini.Write(section, "visible", "light", "dark");

            var message = new StringBuilder();
            Dictionary<string, string> sectData;
            sectData = ini.GetSection(section);
            foreach (var key in sectData.Keys)
            {
                message.Append(key + " = " + ini.GetString(section, key) + "\n");
            }
            MessageBox.Show(message.ToString(), section);

            ini.EraseKey(section, "path");
            ini.EraseKey(section, "visible");
            ini.EraseKey(section, "notseen");
            ini.EraseSection(section);

            message = new StringBuilder();
            foreach (var item in ini.GetSectionNames())
            {
                message.Append(item + "\n");
            }
            MessageBox.Show(message.ToString(), "Section Names");

            message = new StringBuilder();
            section = "STRINGS";
            message.Append(ini.GetString(section, "path") + "\n");
            message.Append(ini.GetString(section, "ini") + "\n");
            message.Append(ini.GetString(section, "whitespace") + "\n");
            message.Append(ini.GetString(section, "notseen", "Touch.Tools.IniFile") + "\n");
            message.Append(ini.GetString(section, "visible", "dark") + "\n");
            MessageBox.Show(message.ToString(), section);

            message = new StringBuilder();
            section = "BYTES";
            message.Append(ini.GetByte(section, "zero") + "\n");
            message.Append(ini.GetByte(section, "one") + "\n");
            message.Append(ini.GetByte(section, "max") + "\n");
            message.Append(ini.GetByte(section, "min", 42) + "\n");
            message.Append(ini.GetByte(section, "notseen", 255) + "\n");
            message.Append(ini.GetByte(section, "notvalid") + "\n");
            message.Append(ini.GetByte(section, "notvalid", 1) + "\n");
            MessageBox.Show(message.ToString(), section);

            message = new StringBuilder();
            section = "INTEGERS";
            message.Append(ini.GetInt(section, "zero") + "\n");
            message.Append(ini.GetInt(section, "one") + "\n");
            message.Append(ini.GetInt(section, "max") + "\n");
            message.Append(ini.GetInt(section, "min", 42) + "\n");
            message.Append(ini.GetInt(section, "notseen", 255) + "\n");
            message.Append(ini.GetInt(section, "notvalid") + "\n");
            message.Append(ini.GetInt(section, "notvalid", -1) + "\n");
            MessageBox.Show(message.ToString(), section);

            message = new StringBuilder();
            section = "LONGS";
            message.Append(ini.GetLong(section, "zero") + "\n");
            message.Append(ini.GetLong(section, "one") + "\n");
            message.Append(ini.GetLong(section, "max") + "\n");
            message.Append(ini.GetLong(section, "min", 42) + "\n");
            message.Append(ini.GetLong(section, "notseen", 255) + "\n");
            message.Append(ini.GetLong(section, "notvalid") + "\n");
            message.Append(ini.GetLong(section, "notvalid", -1) + "\n");
            MessageBox.Show(message.ToString(), section);

            message = new StringBuilder();
            section = "DOUBLES";
            message.Append(ini.GetDouble(section, "zero") + "\n");
            message.Append(ini.GetDouble(section, "one") + "\n");
            message.Append(ini.GetDouble(section, "max") + "\n");
            message.Append(ini.GetDouble(section, "min", 42) + "\n");
            message.Append(ini.GetDouble(section, "notseen", 255) + "\n");
            message.Append(ini.GetDouble(section, "notvalid") + "\n");
            message.Append(ini.GetDouble(section, "notvalid", -1) + "\n");
            MessageBox.Show(message.ToString(), section);

            message = new StringBuilder();
            section = "BOOLEANS";
            message.Append(ini.GetBoolean(section, "fact") + "\n");
            message.Append(ini.GetBoolean(section, "lie", false) + "\n");
            message.Append(ini.GetBoolean(section, "fact_XOR", false) + "\n");
            message.Append(ini.GetBoolean(section, "lie_XOR", true) + "\n");
            message.Append(ini.GetBoolean(section, "notseen", true) + "\n");
            message.Append(ini.GetBoolean(section, "notseen", false) + "\n");
            message.Append(ini.GetBoolean(section, "yes") + "\n");
            message.Append(ini.GetBoolean(section, "no") + "\n");
            message.Append(ini.GetBoolean(section, "notvalid") + "\n");
            MessageBox.Show(message.ToString(), section);

            message = new StringBuilder();
            section = "DATETIMES";
            message.Append(ini.GetDate(section, "default") + "\n");
            message.Append(ini.GetDate(section, "now") + "\n");
            message.Append(ini.GetDate(section, "notseen", DateTime.Now) + "\n");
            message.Append(ini.GetDate(section, "notvalid") + "\n");
            MessageBox.Show(message.ToString(), section);


        }
    }
}
