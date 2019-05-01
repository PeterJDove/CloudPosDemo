using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Touch.SmartCards;
using Touch.Tools;

namespace Touch.CloudSmartCards
{
    class SelectCommand : CardCommand
    {
        public static readonly string Type = "SELECT";

        public string Mode { get; set; }
        public string Uid { get; set; } // Base64 encoded
        public string Sak { get; set; } // result
        public string Atqa { get; set; } // result


        public override ResultStatus Execute(Cloud47x0 cloud47x0)
        {
            const double timeout = 1; // seconds
            try
            {
                ResultStatus = ResultStatus.ERROR; // Assume the worst
                byte[] uidBytes = Convert.FromBase64String(Uid);
                string uidHex = Util.ByteArrayToHexString(uidBytes);

                if (cloud47x0.WaitForCard(timeout))
                {
                    if (cloud47x0.ConnectCard() && uidHex == cloud47x0.CardUID)
                    {
                        Sak = cloud47x0.CardSAK;
                        Atqa = "04";
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
                writer.WritePropertyName("mode");
                writer.WriteValue(Mode);
                writer.WritePropertyName("uid");
                writer.WriteValue(Uid);
                writer.WritePropertyName("result");
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("status");
                    writer.WriteValue(ResultStatus.ToString());
                    if (ResultStatus == ResultStatus.OK)
                    {
                        writer.WritePropertyName("uid");
                        writer.WriteValue(Uid);
                        writer.WritePropertyName("sak");
                        writer.WriteValue(Convert.ToBase64String(Util.HexStringToByteArray(Sak)));
                        writer.WritePropertyName("atqa");
                        writer.WriteValue(Convert.ToBase64String(Util.HexStringToByteArray(Atqa)));
                    }
                    //writer.WritePropertyName("expectedResult");
                    //writer.WriteValue(true);
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }
            return sw.ToString();
        }
    }
}
