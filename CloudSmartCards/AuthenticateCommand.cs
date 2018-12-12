using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Touch.SmartCards;

namespace CloudSmartCards
{
    class AuthenticateCommand : CardCommand
    {
        public static readonly string Type = "AUTHENTICATE";

        public string Key { get; set; }
        public string KeyData { get; set; }
        public int Sector { get; set; }


        public override ResultStatus Execute(Cloud47x0 cloud47x0)
        {
            try
            {
                //if (KeyData == "QvzVmm9D")
                //    KeyData = "MxpY8AME";

                ResultStatus = ResultStatus.ERROR; // Assume the worst
                if (cloud47x0.CardStillPresent())
                {
                    byte[] keyBytes = Convert.FromBase64String(KeyData);
                    if ("A".Equals(Key))
                    {
                        if (cloud47x0.MifareLoadKey(Cloud47x0.MifareKey.A, keyBytes) &&
                        cloud47x0.MifareAuthenticate(Cloud47x0.MifareKey.A, Sector))
                            ResultStatus = ResultStatus.OK;
                    }
                    else if ("B".Equals(Key))
                    {
                        if (cloud47x0.MifareLoadKey(Cloud47x0.MifareKey.B, keyBytes) &&
                        cloud47x0.MifareAuthenticate(Cloud47x0.MifareKey.B, Sector))
                            ResultStatus = ResultStatus.OK;
                    }
                }
            }
            catch
            {
                ResultStatus = ResultStatus.ERROR;
            }
            return ResultStatus;
        }

        public override string ToString() // JSON format
        {
            StringWriter sw = new StringWriter();
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;

                writer.WriteStartObject();
                writer.WritePropertyName("type");
                writer.WriteValue(Type);
                writer.WritePropertyName("key");
                writer.WriteValue(Key);
                writer.WritePropertyName("keyData");
                writer.WriteValue(KeyData);
                writer.WritePropertyName("sector");
                writer.WriteValue(Sector);
                writer.WritePropertyName("result");
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("status");
                    writer.WriteValue(ResultStatus.ToString());
                    writer.WriteEndObject();
                }
                //writer.WritePropertyName("expectedResult");
                //writer.WriteValue(true);
                writer.WriteEndObject();
            }
            return sw.ToString();
        }
    }
}
