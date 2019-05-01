using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Touch.CloudSmartCards
{
    internal class CardIdentity
    {
        public CardIdentity() {}
        
        public string UID { get; set; }
        public string SAK { get; set; }
        public string Technology { get; set; }
        public string CardInterface { get; set; }

        public override string ToString() // JSON String
        {
            StringWriter sw = new StringWriter();
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;

                writer.WriteStartObject();
                writer.WritePropertyName("UID");
                writer.WriteValue(UID);
                writer.WritePropertyName("SAK");
                writer.WriteValue(SAK);
                if (!string.IsNullOrEmpty(Technology))
                {
                    writer.WritePropertyName("technology");
                    writer.WriteValue(Technology);
                }
                if (!string.IsNullOrEmpty(CardInterface))
                {
                    writer.WritePropertyName("cardInterface");
                    writer.WriteValue(CardInterface);
                }
                writer.WriteEndObject();
            }
            return sw.ToString();
        }
    }



}
