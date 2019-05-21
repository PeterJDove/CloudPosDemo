using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Tools;

namespace Touch.HtmlPrinter
{
    class HtmlParser
    {
        static List<string> HeaderTags = new List<string>(new string[] { "h1", "h2", "h3", "h4", "h5", "th" });

        private Node currentNode;
        private Canvas canvas;

        private int listIndex = 0;

        public HtmlParser(Canvas canvas)
        {
            this.canvas = canvas;
        }


        public Document Parse(string Html)
        {
            Document document = new Document();
            currentNode = document;

            Html = Html.Replace("&nbsp;", "\u00A0");

            LaxParser laxParser = new LaxParser();
            laxParser.StartDocument += Lax_StartDocument;
            laxParser.StartElement += Lax_StartElement;
            laxParser.Characters += Lax_Characters;
            laxParser.IgnorableWhitespace += Lax_Characters;
            laxParser.EndElement += Lax_EndElement;
            laxParser.EndDocument += Lax_EndDocument;
            laxParser.Comment += Lax_Comment;
            laxParser.Error += Lax_Error;

            byte[] byteArray = Encoding.UTF8.GetBytes(Html);
            MemoryStream stream = new MemoryStream(byteArray);
            laxParser.Parse(new StreamReader(stream));

            return document;
        }

        private void Lax_StartDocument()
        {
            Debug.Print("Lax_StartDocument");
        }

        private void Lax_StartElement(string elementName, Dictionary<string, string> attributes)
        {
            try
            {
                elementName = elementName.ToLower();
                string id = null;
                if (attributes.ContainsKey("id"))
                {
                    id = attributes["id"];
                }
                switch (elementName)
                {
                    case "b": // bold
                    case "i": // italic
                    case "u": // underline
                    case "em": // italic
                    case "strong": // bold
                    case "small": // smaller font
                    case "big": // larger font
                    case "code": // non-proportional
                    case "tt": // non-proportional (teletype)
                        AssertState(elementName, NodeType.Block);
                        currentNode.AddChild(Node.TextFormatNode(elementName));
                        break;
                    default:
                        {
                            //if (currentNode.Type == NodeType.Block && currentNode.Parent.Type == NodeType.Document)
                            //{
                            //    currentNode = currentNode.Parent;
                            //}
                            switch (elementName)
                            {
                                case "div":
                                    // We ignore all <div> unless class="cut", meaning cut the paper
                                    if (attributes.ContainsKey("class") && attributes["class"].ToLower() == "cut")
                                    {
                                        AssertState(elementName, NodeType.Document);
                                        currentNode.AddChild(new HorizontalRuleNode(id, true));
                                    }
                                    break;
                                case "p":
                                case "h1":
                                case "h2":
                                case "h3":
                                case "h4":
                                case "h5":
                                case "li":
                                case "td": // Table Data
                                case "th": // Header
                                    AssertState(elementName, new[] { NodeType.Document, NodeType.TableRow, NodeType.Block });
                                    var block = new BlockNode(elementName, id);
                                    currentNode = currentNode.AddChild(block);
                                    if (HeaderTags.Contains(elementName))
                                    {
                                        // Header elementName implies its own formatting
                                        block.AddChild(Node.TextFormatNode(elementName));
                                        block.Alignment = StringAlignment.Center;
                                    }
                                    //
                                    //  Process attributes
                                    //
                                    if (attributes.ContainsKey("align"))
                                    {
                                        if (attributes["align"] == "left")
                                            block.Alignment = StringAlignment.Near;
                                        else if (attributes["align"] == "center")
                                            block.Alignment = StringAlignment.Center;
                                        else if (attributes["align"] == "right")
                                            block.Alignment = StringAlignment.Far;
                                    }
                                    if (attributes.ContainsKey("valign"))
                                    {
                                        if (attributes["valign"] == "top")
                                            block.VerticalAlignment = StringAlignment.Near;
                                        else if (attributes["valign"] == "middle")
                                            block.VerticalAlignment = StringAlignment.Center;
                                        else if (attributes["align"] == "bottom")
                                            block.VerticalAlignment = StringAlignment.Far;
                                    }
                                    if (attributes.ContainsKey("class"))
                                    {
                                        if (attributes["class"] == "barcode")
                                            ; // do nothing
                                        else if (attributes["class"] == "hanging-indent")
                                            block.HangingIndent = 4;
                                        else if (attributes["class"] == "hanging-indent-large")
                                            block.HangingIndent = 12;
                                    }
                                    if (elementName == "li")
                                    {
                                        Debug.Assert(listIndex != 0);
                                        block.ListItemIndex = listIndex;
                                        if (listIndex > 0)
                                            listIndex++;
                                    }
                                    break;
                                case "dt": // Definition Term
                                case "dd": // Definition List
                                    AssertState(elementName, new NodeType[] { NodeType.Document, NodeType.Table, NodeType.TableRow });
                                    // In case there was no <dl> element, make sure we are inside a TableRowNode...
                                    if (currentNode.Type == NodeType.Document)
                                    {
                                        currentNode = currentNode.AddChild(new TableNode(id));
                                    }
                                    if (currentNode.Type == NodeType.Table)
                                    {
                                        currentNode = currentNode.AddChild(new TableRowNode(id));
                                    }
                                    currentNode = currentNode.AddChild(new BlockNode(elementName, id));
                                    break;
                                case "ul": // Unordered List
                                    listIndex = -1;
                                    break;
                                case "ol": // Ordered List
                                    listIndex = 1;
                                    break;
                                case "dl":
                                case "table":
                                    AssertState(elementName, new[] { NodeType.Document, NodeType.Block });
                                    currentNode = currentNode.AddChild(new TableNode(id));
                                    break;
                                case "tr": // Table Row
                                    AssertState(elementName, new[] { NodeType.Document, NodeType.Table });
                                    if (currentNode.Type == NodeType.Document)
                                    {
                                        currentNode = currentNode.AddChild(new TableNode(id));
                                    }
                                    currentNode = currentNode.AddChild(new TableRowNode(id));
                                    break;
                                case "img":
                                    AssertState(elementName, new[] { NodeType.Document, NodeType.Block });
                                    if (currentNode.Type == NodeType.Document)
                                    {
                                        block = new BlockNode(elementName, id);
                                        block.Alignment = StringAlignment.Center;
                                        currentNode = currentNode.AddChild(block);
                                    }
                                    if (attributes.ContainsKey("class") && (attributes["class"] == "barcode"))
                                    {
                                        BarcodeNode barcodeNode = new BarcodeNode(id);
                                        if (attributes.ContainsKey("src"))
                                        {
                                            barcodeNode.Source = (attributes["src"]);
                                        }
                                        currentNode.AddChild(barcodeNode);
                                    }
                                    else // NOT a barcode
                                    {
                                        ImageNode imageNode = new ImageNode(id);
                                        if (attributes.ContainsKey("src"))
                                        {
                                            imageNode.Source = (attributes["src"]);
                                        }
                                        currentNode.AddChild(imageNode);
                                    }
                                    break;
                                case "hr":
                                    AssertState(elementName, NodeType.Document);
                                    currentNode.AddChild(new HorizontalRuleNode(id, false));
                                    break;
                                default:
                                    // Debug.Assert(false);
                                    break;
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Lax_Characters(string characters, int start, int length)
        {
            try
            {
                if (currentNode.Type == NodeType.Block)
                {
                    string value = characters.Substring(start, length);
                    // Replace NewLines with a single space (eliminating any other spaces either side)
                    value = value.Replace("\n", "~NL~");
                    value = value.Replace("~NL~ ", "~NL~");
                    value = value.Replace(" ~NL~", "~NL~");
                    value = value.Replace("~NL~", " ");
                    value = value.Replace("\t", " ");

                    currentNode.AddChild(new TextNode(value));
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private void Lax_EndElement(string elementName)
        {
            try
            {
                elementName = elementName.ToLower();
                switch (elementName)
                {
                    case "b": // bold
                    case "i": // italic
                    case "u": // underline
                    case "em": // italic
                    case "strong": // bold
                    case "small": // smaller font
                    case "big": // larger font
                    case "code": // non-proportional
                    case "tt": // non-proportional (teletype)
                        AssertState(elementName, NodeType.Block);
                        currentNode.AddChild(Node.EndTextFormatNode(elementName));
                        break;
                    case "p":
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "li":
                    case "th":
                    case "td":
                        AssertState(elementName, NodeType.Block);
                        currentNode = currentNode.Parent;
                        break;
                    case "tr":
                        AssertState(elementName, NodeType.TableRow);
                        currentNode = currentNode.Parent;
                        AssertState(elementName, NodeType.Table);
                        break;
                    case "table":
                    case "dl": // Definition List
                        AssertState(elementName, NodeType.Table);
                        currentNode = currentNode.Parent;
                        break;
                    case "dt": // Definition TERM (column 1 of 2)
                        AssertState(elementName, NodeType.Block);
                        currentNode = currentNode.Parent;
                        AssertState(elementName, NodeType.TableRow);
                        break;
                    case "dd": // Definition DATA (column 2 of 2)
                        AssertState(elementName, NodeType.Block);
                        currentNode = currentNode.Parent;
                        AssertState(elementName, NodeType.TableRow);
                        currentNode = currentNode.Parent;
                        AssertState(elementName, NodeType.Table);
                        break;
                    case "ol":
                    case "ul":
                        listIndex = 0;
                        break;
                    default:
                        // Debug.Assert(false);
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Lax_EndDocument()
        {
            Debug.Print("Lax_EndDocument");
        }

        private void Lax_Comment(string data)
        {
            // Debug.Print(data);
        }

        private void Lax_Error(string message, int position, ref bool cancel)
        {
            Debug.Print(message + " ...at position " + position);
        }


        private bool AssertState(string tag, NodeType expectedCurrentNodeType)
        {
            Debug.Assert(currentNode != null);
            if (currentNode.Type == expectedCurrentNodeType)
                return true;

            Debug.Print("Unexpected <" + tag + "> when current Node.Type is " + currentNode.Type.ToString());
            return false;
        }


        private bool AssertState(string tag, NodeType[] validCurrentNodeTypes)
        {
            Debug.Assert(currentNode != null);
            if (validCurrentNodeTypes.Contains(currentNode.Type))
                return true;

            Debug.Print("Unexpected <" + tag + "> when current Node.Type is " + currentNode.Type.ToString());
            return false;
        }



    }
}
