using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class BlockNode : Node
    {
        List<Line> lines;
        Line currentLine;
        Word spaces;
        Word word;

        float hangingIndent = 0;


        const char SPACE =  ' ';
        const char TAB = '\t';
        const char NBSP = '\u00a0';
        const char BULLET = '\u2022';
        const char MEDIUMCIRCLE = '\u26AB';
        const char BLACKCIRCLE = '\u25CF';


        public BlockNode(string tag, string id) : base(NodeType.Block, id)
        {
            Tag = tag;
            Alignment = StringAlignment.Near; // Left
            VerticalAlignment = StringAlignment.Near; // Top
        }

        public StringAlignment Alignment { get; set; }
        public StringAlignment VerticalAlignment { get; set; }

        // -1 = bulleted list; positive number = numbered list
        public int ListItemIndex { get; set; }

        // HangingIndent is measured in Ens
        public int HangingIndent { get; set; }

        public override float Width
        {
            get { return 0; }
        }

        public override float Height
        {
            get
            {
                float height = 0;
                foreach (Node node in childNodes)
                {
                    if (node.Type == NodeType.Image || node.Type == NodeType.Barcode)
                    {
                        if (height < ((ImageNode)node).Image.Height)
                            height = ((ImageNode)node).Image.Height;
                    }
                }
                return height;
            }
        }

        public override void Render(Canvas canvas, float left, float width)
        {
            // Debug.Assert(Id != "listitem");

            BuildLines(canvas, width);

            foreach (Line line in lines)
            {
                if (line.Node == null)
                    PrintWords(canvas, left, width, line, this.Alignment);
                else
                {
                    var type = line.Node.GetType();
                    if (type == typeof(BlockNode) || type == typeof(TableNode))
                        line.Node.Render(canvas, left + line.Indent, width);
                    else if (type == typeof(ImageNode) || type == typeof(BarcodeNode))
                        PrintImage(canvas, left, width, line, this.Alignment);
                    else
                        Debug.Assert(false);
                }
            }
        }


        private float CalculateIndent(Graphics g, Font font)
        {
            string prefix;

            if (ListItemIndex < 0) // Bulleted List
            {
                prefix = new string(new char[] { NBSP, BLACKCIRCLE, NBSP });
                childNodes.Insert(0, new TextNode(prefix));
                return g.MeasureString(prefix, font).Width;
            }
            else if (ListItemIndex > 0) // Numbered List
            {
                prefix = ListItemIndex + "." + NBSP.ToString();
                childNodes.Insert(0, new TextNode(prefix));
                return g.MeasureString(prefix, font).Width;
            }
            else if (HangingIndent > 0)
            {
                return g.MeasureString(new string('0', HangingIndent - 1), font).Width;
            }
            return 0;
        }


        private MinMax BuildLines(Canvas canvas, float maxWidth)
        {
            Graphics g = canvas.Graphics;
            Font font = canvas.BaseFont;
            float minWidth = float.MaxValue;

            Stack<Font> fontStack = new Stack<Font>();

            lines = new List<Line>();
            currentLine = new Line(0); // First line is never indented
            spaces = new Word(font, g);
            word = new Word(font, g);

            hangingIndent = CalculateIndent(g, font);

            foreach (Node node in childNodes)
            {
                if (node.Type == NodeType.Text)
                {
                    char[] chars = ((TextNode)node).Chars;
                    foreach (char c in chars)
                    {
                        if (c.Equals(SPACE))
                        {
                            if (word.Length > 0)
                            {
                                if (minWidth > word.Length)
                                {
                                    minWidth = word.Length;
                                }
                                FlushWordBuffer(canvas, maxWidth);
                                spaces = new Word(font, g);
                                word = new Word(font, g);
                            }
                            spaces.Add(SPACE);
                        }
                        else
                        {
                            word.Add(c);
                            if (word.Width > maxWidth) // Word is too long for block width
                            {
                                word.Backspace(); // remove c (just added)
                                char lastChar = word.Backspace(); 
                                word.Add('-'); // replace lastChar with a hyphen
                                FlushWordBuffer(canvas, maxWidth);
                                spaces = new Word(font, g);
                                word = new Word(font, g);
                                word.Add(lastChar); // move lastChar to new word
                                word.Add(c); // append c to new word
                            }
                        }
                    }
                }
                else if (node.Type == NodeType.TextFormat)
                {
                    FlushWordBuffer(canvas, maxWidth);
                    fontStack.Push(font);
                    font = ModifyFont(font, node.Tag);
                    spaces = new Word(font, g);
                    word = new Word(font, g);
                }
                else if (node.Type == NodeType.EndTextFormat)
                {
                    FlushWordBuffer(canvas, maxWidth);
                    font = fontStack.Pop();
                    spaces = new Word(font, g);
                    word = new Word(font, g);
                }
                else if(node.Type == NodeType.Image || node.Type == NodeType.Barcode
                     || node.Type == NodeType.Table || node.Type == NodeType.Block)
                {
                    if (currentLine.Width > 0)
                        lines.Add(currentLine);

                    currentLine = new Line(hangingIndent, node);
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            FlushWordBuffer(canvas, maxWidth);
            if (currentLine.Width > 0)
                lines.Add(currentLine);

            maxWidth = 0;
            foreach (Line line in lines)
            {
                var lineWidth = line.Width;
                if (maxWidth < lineWidth)
                    maxWidth = lineWidth;
            }
            return new MinMax(minWidth, maxWidth);
        }

        private void FlushWordBuffer(Canvas canvas, float maxWidth)
        {
            if (word.Width > 0)
            {
                if (currentLine.Indent + currentLine.Width + spaces.Width + word.Width < maxWidth)
                {
                    if(spaces.Length > 0)
                        currentLine.Add(spaces);

                    currentLine.Add(word);
                }
                else
                {
                    lines.Add(currentLine);
                    currentLine = new Line(hangingIndent, word);
                }
            }
        }

        private static Font ModifyFont(Font old, string tag)
        {
            Font newFont = new Font(old, old.Style);
            switch (tag)
            {
                case "h1":
                    newFont = new Font(old.FontFamily, 20, old.Style);
                    break;
                case "h2":
                    newFont = new Font(old.FontFamily, 18, old.Style);
                    break;
                case "h3":
                    newFont = new Font(old.FontFamily, 16, old.Style);
                    break;
                case "h4":
                    newFont = new Font(old.FontFamily, 14, old.Style);
                    break;
                case "h5":
                    newFont = new Font(old.FontFamily, 12, old.Style);
                    break;
                case "b": // bold
                case "strong": // bold
                case "th": // Table Header
                    newFont = new Font(old, old.Style | FontStyle.Bold);
                    break;
                case "i": // italic
                case "em": // italic
                    newFont = new Font(old, old.Style | FontStyle.Italic);
                    break;
                case "u": // underline
                    newFont = new Font(old, old.Style | FontStyle.Underline);
                    break;
                case "big": // larger font
                    newFont = new Font(old.FontFamily, old.Size * 1.25f, old.Style);
                    break;
                case "small": // smaller font
                    newFont = new Font(old.FontFamily, old.Size * 0.8f, old.Style);
                    break;
                case "code": // non-proportional
                case "tt": // non-proportional (teletype)
                    newFont = new Font("Times New Roman", old.Size, old.Style);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            return newFont;
        }


        private static void PrintImage(Canvas canvas, float left, float width, Line line, StringAlignment alignment)
        {
            Graphics g = canvas.Graphics;
            Image image = ((ImageNode)line.Node).Image;
            float w = image.Width;
            float h = image.Height;

            float x = left;
            if (w < width)
            {
                switch (alignment)
                {
                    case StringAlignment.Near:
                        break;
                    case StringAlignment.Center:
                        x += (width - image.Width) / 2;
                        break;
                    case StringAlignment.Far:
                        x += (width - image.Width);
                        break;
                }
            }
            else // Scale down image to fit into available width
            {
                h = h * (width / w);
                w = width;
            }
            g.DrawImage(image, x, canvas.CurrentY, w, h);
            canvas.CurrentY += h;
        }


        private void PrintWords(Canvas canvas, float left, float width, Line line, StringAlignment alignment)
        {
            Graphics g = canvas.Graphics;
            Brush brush = Brushes.Black;

            float x = left;
            switch (alignment)
            {
                case StringAlignment.Near:
                    x += line.Indent;
                    break;
                case StringAlignment.Center:
                    x += (width - line.Width) / 2;
                    break;
                case StringAlignment.Far:
                    x += (width - line.Width);
                    break;
            }

            foreach (Word Word in line.Words)
            {
                float y = canvas.CurrentY + (line.Height - Word.Height) / 2;
                g.DrawString(Word.ToString(), Word.Font, brush, x, y);
                x += Word.Width;
            }
            canvas.CurrentY += line.Height;
        }


        public MinMax MinMax(Canvas canvas, float width)
        {
            return BuildLines(canvas, width);
        }


        /// <summary>
        /// Inner classes
        /// </summary>

        class Word 
        {
            public Font Font { get; set; }
            private Graphics graphics { get; set; }
            private StringBuilder sb = new StringBuilder();

            public Word(Font font, Graphics g)
            {
                Font = font;
                graphics = g;
            }

            public void Add (char c)
            {
                sb.Append(c);
            }

            public void Clear()
            {
                sb.Clear();
            }

            public char Backspace()
            {
                int last = sb.Length - 1;
                char lastChar = sb[last];
                sb.Remove(last, 1);
                return lastChar;
            }

            public int Length
            {
                get { return sb.Length; }
            }

            public float Height
            {
                get { return graphics.MeasureString(sb.ToString(), Font).Height; }
            }

            public float Width
            {
                get
                {
                    if (sb.Length > 0)
                    {
                        string word = sb.ToString();
                        if (string.IsNullOrWhiteSpace(word))
                            word = new string('n', sb.Length);

                        return graphics.MeasureString(word, Font, int.MaxValue, StringFormat.GenericTypographic).Width;
                    }
                    return 0;
                }
            }

            public override string ToString()
            {
                return sb.ToString();
            }
        }



        class Line // : IEnumerable<Word>
        {
            public float Indent { get; set; } // GraphicsUnits      
            public List<Word> Words { get; private set; }
            public Node Node { get; private set; }

            public Line(float indent)
            {
                Indent = indent;
                Words = new List<Word>();
            }

            public Line(float indent, Word word) : this(indent)
            {
                Words.Add(word);
            }

            public Line(float indent, Node node) : this(indent)
            {
                Node = node;
            }


            public void Add(Word Word)
            {
                if (Word.Length > 0)
                    Words.Add(Word);
            }


            public float Width
            {
                get
                {
                    if (this.Node != null)
                    {
                        return this.Node.Width;
                    }
                    float width = 0;
                    foreach (Word Word in Words)
                        width += Word.Width;

                    return width;
                }
            }

            public float Height
            {
                get
                {
                    if (this.Node != null)
                    {
                        return this.Node.Height;
                    }
                    float height = 0;
                    foreach (Word Word in Words)
                    {
                        if (Word.Height > height)
                            height = Word.Height;
                    }
                    return height;
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (Word Word in Words)
                {
                    sb.Append(Word.ToString());
                }
                return sb.ToString();
            }


        }




    }
}
