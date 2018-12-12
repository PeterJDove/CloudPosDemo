using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.SmartCards;

namespace CloudSmartCards
{
    public static class SmartCardApi
    {
        private static Cloud47x0 _cloud47x0;

        enum ParseState
        {
            NotStarted,
            BetweenActions,
            InsideAction,
            InsideCommand,
            Finished
        }


        static SmartCardApi()
        {
            var readerRegex = "CLOUD 47.0.*Contactless";
            _cloud47x0 = Cloud47x0.GetReader(readerRegex);
        }


        public static bool IsCardPresent()
        {
            if (_cloud47x0 == null)
                return false;

            const double timeout = 0.2; // seconds
            return _cloud47x0.WaitForCard(timeout);
        }

        /*
         * Returns a JSON encoded object like:
         *  {
         *      "UID": "59F08931",
         *      "SAK": "08",
         *      "technology", "TYPE_A",
         *      "cardInterface", "MIFARE_CLASSIC_1K"
         *  }
         */
        public static string ReadCardIdentity()
        {
            if (_cloud47x0 != null && _cloud47x0.ConnectCard())
            {
                var identity = new CardIdentity
                { 
                    UID = _cloud47x0.CardUID,
                    SAK = _cloud47x0.CardSAK,
                    Technology = _cloud47x0.CardTech,
                    CardInterface = _cloud47x0.CardInterface,
                };
                return identity.ToString();
            }
            return null;
        }


        public static string ProcessCardActions(string cardActions)
        {
            List<Action> actions = ParseActionArray(cardActions);

            StringWriter sw = new StringWriter();
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartArray();
                ResultStatus prevResult = ResultStatus.OK;
                foreach (Action action in actions)
                {
                    if (prevResult == ResultStatus.OK)
                        prevResult = action.Command.Execute(_cloud47x0);
                    else
                        prevResult = action.Command.Skip();

                    writer.WriteRawValue(action.ToString());
                }
                writer.WriteEndArray();
            }
            return sw.ToString();
        }
            

        private static List<Action> ParseActionArray(string cardActions)
        {
            List<Action> actions = new List<Action>();
            ParseState parseState = ParseState.NotStarted;
            Action action = null;
            string propertyName = null;

            using (JsonTextReader reader = new JsonTextReader(new StringReader(cardActions)))
            {
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.StartArray:
                            Debug.Assert(parseState == ParseState.NotStarted);
                            parseState = ParseState.BetweenActions;
                            break;
                        case JsonToken.StartObject:
                            if (parseState == ParseState.BetweenActions)
                            {
                                action = new Action();
                                parseState = ParseState.InsideAction;
                            }
                            else if (parseState == ParseState.InsideAction)
                            {
                                parseState = ParseState.InsideCommand;
                            }
                            else
                                Debug.Assert(false);

                            break;
                        case JsonToken.PropertyName:
                            Debug.Assert(parseState == ParseState.InsideAction || parseState == ParseState.InsideCommand);
                            propertyName = reader.Value.ToString();
                            break;
                        case JsonToken.Integer:
                            int number = int.Parse(reader.Value.ToString());
                            if (propertyName == "id")
                            {
                                Debug.Assert(parseState == ParseState.InsideAction);
                                action.Id = number;
                            }
                            else
                            {
                                Debug.Assert(parseState == ParseState.InsideCommand);
                                if (propertyName == "sector")
                                {
                                    Debug.Assert(action.Command.GetType() == typeof(AuthenticateCommand));
                                    ((AuthenticateCommand)action.Command).Sector = number;
                                }
                                else if (propertyName == "block")
                                {
                                    if (action.Command.GetType() == typeof(ReadCommand))
                                        ((ReadCommand)action.Command).Block = number;
                                    else if (action.Command.GetType() == typeof(WriteCommand))
                                        ((WriteCommand)action.Command).Block = number;
                                    else
                                        Debug.Assert(false);
                                }
                                else if (propertyName == "length")
                                {
                                    if (action.Command.GetType() == typeof(ReadCommand))
                                        ((ReadCommand)action.Command).Length = number;
                                    else if (action.Command.GetType() == typeof(WriteCommand))
                                        ((WriteCommand)action.Command).Length = number;
                                    else
                                        Debug.Assert(false);
                                }
                                else
                                    Debug.Assert(false);
                            }
                            break;
                        case JsonToken.String:
                            Debug.Assert(parseState == ParseState.InsideCommand);
                            string value = reader.Value.ToString();
                            if (propertyName == "type")
                            {
                                action.AddCommand(value);
                                break;
                            }
                            else if (propertyName == "key")
                            {
                                Debug.Assert(action.Command.GetType() == typeof(AuthenticateCommand));
                                ((AuthenticateCommand)action.Command).Key = value;
                            }
                            else if (propertyName == "keyData")
                            {
                                Debug.Assert(action.Command.GetType() == typeof(AuthenticateCommand));
                                ((AuthenticateCommand)action.Command).KeyData = value;
                            }
                            else if (propertyName == "mode")
                            {
                                Debug.Assert(action.Command.GetType() == typeof(SelectCommand));
                                ((SelectCommand)action.Command).Mode = value;
                            }
                            else if (propertyName == "uid")
                            {
                                Debug.Assert(action.Command.GetType() == typeof(SelectCommand));
                                ((SelectCommand)action.Command).Uid = value;
                            }
                            else if (propertyName == "data")
                            {
                                Debug.Assert(action.Command.GetType() == typeof(WriteCommand));
                                ((WriteCommand)action.Command).Data = value;
                            }
                            else
                                Debug.Assert(false);

                            break;
                        case JsonToken.EndObject:
                            if (parseState == ParseState.InsideCommand)
                                parseState = ParseState.InsideAction;
                            else if (parseState == ParseState.InsideAction)
                            {
                                actions.Add(action);
                                action = null;
                                parseState = ParseState.BetweenActions;
                            }
                            else
                                Debug.Assert(false);

                            break;
                        case JsonToken.EndArray:
                            Debug.Assert(parseState == ParseState.BetweenActions);
                            parseState = ParseState.Finished;
                            break;
                    }
                }
            }
            return actions;
        }

        public static int GetSmartcardPluginVersionNo()
        {
            return 1;
        }

    }
}
