using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace Touch.CloudSmartCards
{
    class Action
    {
        public Action() { }

        public int Id { get; set; }
        public CardCommand Command { get; private set; }

        public void AddCommand(string type)
        {
            if (SelectCommand.Type.Equals(type))
                this.Command = new SelectCommand();
            else if (AuthenticateCommand.Type.Equals(type))
                this.Command = new AuthenticateCommand();
            else if (ReadCommand.Type.Equals(type))
                this.Command = new ReadCommand();
            else if (WriteCommand.Type.Equals(type))
                this.Command = new WriteCommand();
            else
                Debug.Assert(false);
        }



        public override string ToString() // JSON format
        {
            StringWriter sw = new StringWriter();
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;

                writer.WriteStartObject();
                writer.WritePropertyName("id");
                writer.WriteValue(Id);
                writer.WritePropertyName("command");
                writer.WriteRawValue(Command.ToString());
                writer.WriteEndObject();
            }
            return sw.ToString();
        }
    }



}
