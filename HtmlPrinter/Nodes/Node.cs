using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    enum NodeType
    {
        Document, // i.e. the root node
        Block, // typically a Block of text, but may contain image or barcode, too
        Table, // of TableRows
        TableRow, // of blocks, side by side
        Text, // the text inside a Block
        TextFormat,
        EndTextFormat,
        Image,
        Barcode,
        HorizontalRule, // or Cut
    }

    class Node
    {
        public NodeType Type { get; protected set; }
        public Node Parent { get; protected set; }
        public string Tag { get; protected set; }
        public bool Auto { get; protected set; }
        public string Id { get; set; }

        protected List<Node> childNodes = new List<Node>();

        protected Node(NodeType type, string id)
        {
            Type = type;
            Id = id;
        }

        public Node AddChild(Node child)
        {
            child.Parent = this;
            childNodes.Add(child);
            return child;
        }

        public virtual float Width { get; }
        public virtual float Height { get; }

        public virtual void Render(Surface surface, float left, float width)
        {
            throw new NotImplementedException("Needs to be overridden");
        }

        public static Node TextFormatNode(string tag)
        {
            return new Node(NodeType.TextFormat, null)
            {
                Tag = tag
            };
        }

        public static Node EndTextFormatNode(string tag)
        {
            return new Node(NodeType.EndTextFormat, null)
            {
                Tag = tag
            };
        }





    }



}
