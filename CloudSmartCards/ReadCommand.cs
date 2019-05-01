using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Touch.SmartCards;

namespace Touch.CloudSmartCards
{
    class ReadCommand : CardCommand
    {
        public static readonly string Type = "READ";

        public int Block { get; set; }
        public int Length { get; set; }
        public string Data { get; set; } //result

        const int BLOCK_SIZE = 16;


        public override ResultStatus Execute(Cloud47x0 cloud47x0)
        {
            try
            {
                ResultStatus = ResultStatus.ERROR; // Assume the worst
                if (cloud47x0.CardStillPresent())
                {
                    byte[] dataBytes = new byte[Length * BLOCK_SIZE];
                    for (int n = 0; n < Length; n++)
                    {
                        byte[] blockBytes = cloud47x0.MifareReadBlock(Block + n);
                        Array.Copy(blockBytes, 0, dataBytes, n * BLOCK_SIZE, BLOCK_SIZE);
                    }
                    Data = Convert.ToBase64String(dataBytes);
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
                writer.WritePropertyName("result");
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("status");
                    writer.WriteValue(ResultStatus.ToString());
                    if (ResultStatus == ResultStatus.OK)
                    {
                        writer.WritePropertyName("data");
                        writer.WriteValue(Data);
                    }
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
