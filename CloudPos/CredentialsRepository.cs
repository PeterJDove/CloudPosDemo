using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using static System.Environment;

namespace CloudPos
{
    /*
     *  This class simply manages the persistent storage of the PosActivator.Credentials
     *  in a file on disk called "CloudPos.auth".  This goes in the Windows 
     *  "CommonApplicationData" folder, e.g. C:\ProgramData\CloudPOS\
     */

    internal class CredentialsRepository
    {
        private Dictionary<String, PosActivator.Credentials> _dictionary;
        private const string _filename = "CloudPos.auth";

        internal CredentialsRepository()
        { }

        internal Dictionary<String, PosActivator.Credentials> Restore()
        {
            try
            {
                string filePath = GetFileLocation();
                var content = File.ReadAllBytes(filePath);
                var yaml = Base64Decode(Encoding.UTF8.GetString(content));
                return deserialise(yaml);
            }
            catch
            {
                return null;
            }
        }

        internal PosActivator.Credentials GetIfPresent(string key)
        {
            EnsureDictionaryExists();
            return _dictionary.ContainsKey(key) ? Get(key) : null;
        }

        internal void SaveCredentials(string key, PosActivator.Credentials credentials)
        {
            EnsureDictionaryExists();

            if (_dictionary.ContainsKey(key))
            {
                _dictionary.Remove(key);
            }
            if (credentials != null)
            {
                _dictionary.Add(key, credentials);
            }
            Save();
        }

        private PosActivator.Credentials Get(string key)
        {
            return _dictionary.TryGetValue(key, out PosActivator.Credentials value) ? value : null;
        }

        private void EnsureDictionaryExists()
        {
            if (_dictionary == null)
            {
                _dictionary = Restore();
            }

            if (_dictionary == null)
            {
                _dictionary = new Dictionary<string, PosActivator.Credentials>();
            }

        }

        private string GetFileLocation()
        {
            var appData = Environment.GetFolderPath(SpecialFolder.CommonApplicationData);
            string filePath = Path.Combine(appData, "CloudPOS", _filename);
            return filePath;
        }

        private Dictionary<string, PosActivator.Credentials> deserialise(string yaml)
        {
            var input = new StringReader(yaml);
            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<Dictionary<String, PosActivator.Credentials>>(input);
        }

        internal void Save()
        {
            string filePath = GetFileLocation();
            var content = Encoding.UTF8.GetBytes(Base64Encode(Serialise(_dictionary)));
            File.WriteAllBytes(filePath, content);
        }

        private string Serialise(Dictionary<string, PosActivator.Credentials> dictionary)
        {
            var serializer = new SerializerBuilder().Build();
            return serializer.Serialize(dictionary);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
