using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.Tools
{
    /// <summary>
    /// This parser is loosely modelled on the SAX Parser that is available in the Java world.
    /// While this parser is intended to parse XML files, it is a "lax" parser because it is
    /// deliberately designed to be somewhat tolerant of badly-formed XML, doing the best
    /// it can to make sense of it anyway.  As such, it is possible to use this class to 
    /// parse (older) HTML, which does not conform to the XML standard.
    /// </summary>
    public class LaxParser
    {
        public delegate void EmptyDelegate();
        public delegate void StartElementDelegate(string elementName, Dictionary<string, string> attributes);
        public delegate void EndElementDelegate(string elementName);
        public delegate void CharactersDelegate(string characters, int start, int length);
        public delegate void ProcessingInstructionDelegate(string target, string data);
        public delegate void CommentDelegate(string data);
        public delegate void ErrorDelegate(string message, int position, ref bool cancel);

        private enum ParseState
        {
            Outside, // Seeking first <
            LessThan, // Found a < 
            DocType, // Found <!DOCTYPE
            XmlDeclaration, // <?xml
            NewElementName, // parsing a new Element name
            SeekingAttrName, // New Element complete; seeking (next) attribute
            AttrName, // parsing an Attribute name
            SeekingEquals, // Attribute name complete; seeking the = sign
            SeekingAttrValue, // Equals sign found; seeking quote char (" or ')
            AttrValue, // parsing the Attribute value
            EmptyElement, // Found a slash at the end of a start element tag
            InsideElement, // Inside an Element; parsing Data
            EndElementName, // Found a slash before the Element name
            CData, // Found <!CDATA[[
            Comment, // Found <!--
            ProcessingInstruction, // Found <?
        }

        /// <summary>
        /// Raised when the XML document is started, that is when the first "<" is encountered.
        /// </summary>
        public event EmptyDelegate StartDocument;
        public event EmptyDelegate EndDocument;
        public event StartElementDelegate StartElement;
        public event EndElementDelegate EndElement;
        public event CharactersDelegate Characters;
        public event CharactersDelegate IgnorableWhitespace;
        public event ProcessingInstructionDelegate ProcessingInstruction;
        public event CommentDelegate Comment;
        public event ErrorDelegate Error;

        public void Parse(StreamReader streamReader)
        {
            ParseState state = ParseState.Outside;
            bool parseCancelled = false;
            int depth = 0;

            string buffer = streamReader.ReadToEnd();
            char[] c = buffer.ToCharArray();
            char attrQuoteChar = '\0';
            int i = -1;

            string elementName = "";
            string attrName = "";
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            bool documentStarted = false;
            bool tokenComplete = false;
            StringBuilder token = new StringBuilder();
            while (++i < c.Length && !parseCancelled)
            {
                char cc = c[i]; // current char

                switch (state)
                {
                    case ParseState.Outside:
                        if (cc == '<')
                        {
                            documentStarted = true;
                            StartDocument?.Invoke();
                            state = ParseState.LessThan;
                        }
                        else
                        {
                            token.Append(cc);
                        }
                        break;
                    case ParseState.LessThan:
                        if (char.IsWhiteSpace(cc))
                        {
                            // do nothing
                        }
                        else if (MayStartXmlName(cc))
                        {
                            elementName = "";
                            attributes = new Dictionary<string, string>();
                            token = new StringBuilder(cc.ToString());
                            state = ParseState.NewElementName;
                        }
                        else if (cc == '/')
                        {
                            tokenComplete = false;
                            token = new StringBuilder();
                            state = ParseState.EndElementName;
                        }
                        else if (cc == '!') // !DOCTYPE or !CDATA[[ or !--
                        {
                            var bang = new string(c.SubArray(i, 10));
                            if (bang.StartsWith("!DOCTYPE "))
                            {
                                i += 8;
                                token = new StringBuilder();
                                state = ParseState.DocType;
                            }
                            else if (bang.StartsWith("!CDATA[["))
                            {
                                i += 7;
                                token = new StringBuilder();
                                state = ParseState.CData;
                            }
                            else if (bang.StartsWith("!--"))
                            {
                                i += 2;
                                token = new StringBuilder();
                                state = ParseState.Comment;
                            }
                            else
                            {
                                parseCancelled = RaiseError("Unexpected chars after <! : <" + c.SubArray(i, 5), i);
                            }
                        }
                        else if (cc == '?') // ?xml declaration ?
                        {
                            if (new string(c.SubArray(i, 5)) == "?xml ")
                            {
                                i += 4;
                                state = ParseState.XmlDeclaration;
                            }
                            else
                            {
                                state = ParseState.ProcessingInstruction;
                            }
                        }
                        else
                        {
                            parseCancelled = RaiseError("Unexpected char '" + cc.ToString() + "' after <", i);
                        }
                        break;
                    case ParseState.DocType:
                    case ParseState.XmlDeclaration:
                        if (cc == '>')
                        {
                            state = ParseState.Outside;
                        }
                        // Ignore anything else, until we get to '>'
                        break;
                    case ParseState.NewElementName:
                        if (ValidInXmlName(cc))
                        {
                            token.Append(cc);
                        }
                        else if (cc == '>')
                        {
                            elementName = token.ToString();
                            StartElement?.Invoke(elementName, attributes);
                            depth++;
                            token = new StringBuilder();
                            state = ParseState.InsideElement;
                        }
                        else if (cc == '/')
                        {
                            elementName = token.ToString();
                            state = ParseState.EmptyElement;
                        }
                        else if (cc == ' ')
                        {
                            elementName = token.ToString();
                            state = ParseState.SeekingAttrName;
                            attributes = new Dictionary<string, string>();
                        }
                        else
                        {
                            parseCancelled = RaiseError("Invalid char '" + cc.ToString() + "' in Element Name", i);
                        }
                        break;
                    case ParseState.SeekingAttrName:
                        if (char.IsWhiteSpace(cc))
                        {
                            // do nothing
                        }
                        else if (MayStartXmlName(cc))
                        {
                            token = new StringBuilder(cc.ToString());
                            state = ParseState.AttrName;
                        }
                        else if (cc == '/')
                        {
                            state = ParseState.EmptyElement;
                        }
                        else if (cc == '>')
                        {
                            StartElement?.Invoke(elementName, attributes);
                            depth++;
                            token = new StringBuilder();
                            state = ParseState.InsideElement;
                        }
                        else
                        {
                            parseCancelled = RaiseError("Invalid char '" + cc.ToString() + "' at start of Attribute Name", i);
                        }
                        break;
                    case ParseState.AttrName:
                        if (ValidInXmlName(cc))
                        {
                            token.Append(cc);
                        }
                        else if (cc == ' ')
                        {
                            attrName = token.ToString();
                            token = new StringBuilder();
                            state = ParseState.SeekingEquals;
                        }
                        else if (cc == '=')
                        {
                            attrName = token.ToString();
                            token = new StringBuilder();
                            state = ParseState.SeekingAttrValue;
                        }
                        else
                        {
                            parseCancelled = RaiseError("Invalid char '" + cc.ToString() + "' in Attribute Name", i);
                        }
                        break;
                    case ParseState.SeekingEquals:
                        if (cc == '=')
                        {
                            state = ParseState.SeekingAttrValue;
                        }
                        else if (cc == '/')
                        {
                            attributes.Add(attrName, "");
                            state = ParseState.EmptyElement;
                        }
                        else if (cc == '>')
                        {
                            attributes.Add(attrName, "");
                            token = new StringBuilder();
                            state = ParseState.InsideElement;
                        }
                        else if (MayStartXmlName(cc))
                        {
                            // Assume empty attribute (with no value)
                            attributes.Add(attrName, "");
                            token = new StringBuilder(cc.ToString());
                            state = ParseState.AttrName;
                        }
                        else
                        {
                            parseCancelled = RaiseError("Found '" + cc.ToString() + "' when expecting '=' after Attribute Name", i);
                        }
                        break;
                    case ParseState.SeekingAttrValue:
                        if (cc == '\'' || cc == '\"')
                        {
                            attrQuoteChar = cc;
                            token = new StringBuilder();
                            state = ParseState.AttrValue;
                        }
                        else
                        {
                            parseCancelled = RaiseError("Found '" + cc.ToString() + "' when expecting quoted Attribute Value", i);
                        }
                        break;
                    case ParseState.AttrValue:
                        if (cc == attrQuoteChar)
                        {
                            attributes.Add(attrName, token.ToString());
                            token = new StringBuilder();
                            state = ParseState.SeekingAttrName;
                        }
                        else
                        {
                            token.Append(cc);
                        }
                        break;
                    case ParseState.EmptyElement:
                        if (cc == '>')
                        {
                            StartElement?.Invoke(elementName, attributes);
                            // no change to depth, because...
                            EndElement?.Invoke(elementName);
                            token = new StringBuilder();
                            state = ParseState.InsideElement;
                        }
                        else if (cc == ' ')
                        {
                            // do nothing
                        }
                        else
                        {
                            parseCancelled = RaiseError("Found '" + cc.ToString() + "' after '/' in empty Element Tag", i);
                        }
                        break;
                    case ParseState.InsideElement:
                        if (cc == '<')
                        {
                            string data = token.ToString();
                            if (data.Length > 0)
                            {
                                if (string.IsNullOrWhiteSpace(data))
                                    IgnorableWhitespace?.Invoke(data, 0, data.Length);
                                else
                                    Characters?.Invoke(data, 0, data.Length);
                            }
                            token = new StringBuilder();
                            state = ParseState.LessThan;
                        }
                        else
                        {
                            token.Append(cc);
                        }
                        break;
                    case ParseState.EndElementName:
                        if (MayStartXmlName(cc) && token.Length == 0)
                        {
                            token.Append(cc);
                            tokenComplete = false;
                        }
                        else if (ValidInXmlName(cc) && !tokenComplete)
                        {
                            token.Append(cc);
                        }
                        else if (cc == '>')
                        {
                            elementName = token.ToString();
                            EndElement?.Invoke(elementName);
                            token = new StringBuilder();
                            if (--depth > 0)
                                state = ParseState.InsideElement;
                            else
                                state = ParseState.Outside;
                        }
                        else if (cc == ' ')
                        {
                            tokenComplete = true;
                        }
                        else
                        {
                            parseCancelled = RaiseError("Found '" + cc.ToString() + "' after Name in End-of-Element tag.", i);
                        }
                        break;
                    case ParseState.Comment:
                        if (cc == '-' && new string(c.SubArray(i, 3)) == "-->")
                        {
                            i += 2;
                            Comment?.Invoke(token.ToString());
                            token = new StringBuilder();
                            if (depth > 0)
                                state = ParseState.InsideElement;
                            else
                                state = ParseState.Outside;
                        }
                        else
                        {
                            token.Append(cc);
                        }
                        break;
                    case ParseState.ProcessingInstruction:
                        if (cc == '?' && new string(c.SubArray(i, 2)) == "?>")
                        {
                            i += 1;
                            elementName = token.ToString();
                            ProcessingInstruction?.Invoke(elementName.Head(), elementName.Tail());
                            token = new StringBuilder();
                            if (depth > 0)
                                state = ParseState.InsideElement;
                            else
                                state = ParseState.Outside;
                        }
                        else
                        {
                            token.Append(cc);
                        }
                        break;
                    default:
                        parseCancelled = RaiseError("Unhandled ParseState: " + state.ToString(), i);
                        break;
                }
            }
            if (documentStarted)
            {
                EndDocument?.Invoke();
            }


        }


        private bool RaiseError(string message, int position)
        {
            bool cancel = false;
            if (Error != null)
                Error(message, position, ref cancel);
            else
                Debug.Assert(false);

            return cancel;
        }


        private bool ValidInXmlName(char c)
        {
            return (char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.');
        }

        private bool MayStartXmlName(char c)
        {
            return (char.IsLetterOrDigit(c) || c == '_');
        }



    }
}
