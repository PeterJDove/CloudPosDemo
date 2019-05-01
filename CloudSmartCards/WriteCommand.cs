using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Touch.SmartCards;

namespace Touch.CloudSmartCards
{
    class WriteCommand : CardCommand
    {
        public static readonly string Type = "WRITE";

        public int Block { get; set; }
        public int Length { get; set; }
        public string Data { get; set; }

        const int BLOCK_SIZE = 16;


        public override ResultStatus Execute(Cloud47x0 cloud47x0)
        {
            try
            {
                ResultStatus = ResultStatus.ERROR; // Assume the worst
                if (cloud47x0.CardStillPresent())
                {
                    byte[] dataBytes = Convert.FromBase64String(Data);
                    byte[] blockBytes = new byte[BLOCK_SIZE];
                    for (int n = 0; n < Length; n++)
                    {
                        Array.Copy(dataBytes, n * BLOCK_SIZE, blockBytes, 0, BLOCK_SIZE);
                        cloud47x0.MifareWriteBlock(Block + n, blockBytes);
                    }
                    ResultStatus = ResultStatus.OK;
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
                writer.WritePropertyName("block");
                writer.WriteValue(Block);
                writer.WritePropertyName("length");
                writer.WriteValue(Length);
                writer.WritePropertyName("data");
                writer.WriteValue(Data);
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
