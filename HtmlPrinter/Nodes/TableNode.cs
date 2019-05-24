using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class TableNode : Node
    {
        public TableNode(string id) : base(NodeType.Table, id) { }

        public override void Render(Surface surface, float left, float width)
        {
            //
            //  Determine the maximum number of Columns in the rows in this table
            //
            int maxColumns = 0;
            foreach (TableRowNode row in childNodes)
            {
                int columns = row.MinMaxWidths(surface, width).Count;
                if (maxColumns < columns)
                    maxColumns = columns;
            }
            //
            //  Initialise an Array of MinMax's for the maximum number of Columns
            //
            MinMax[] colWidths = new MinMax[maxColumns];
            for (int i = 0; i < maxColumns; i++)
            {
                colWidths[i] = new MinMax(0, 0);
            }
            //
            //  Now, for each column, determine the biggest MinWidth and biggest MaxWidth in all rows
            //
            foreach (TableRowNode row in childNodes)
            {
                List<MinMax> blockWidths = row.MinMaxWidths(surface, width);
                for (int i = 0; i < blockWidths.Count; i++)
                {
                    if (colWidths[i].MinWidth < blockWidths[i].MinWidth)
                        colWidths[i].MinWidth = blockWidths[i].MinWidth;

                    if (colWidths[i].MaxWidth < blockWidths[i].MaxWidth)
                        colWidths[i].MaxWidth = blockWidths[i].MaxWidth;
                }
            }
            //
            //  Determine what the ratios are between all colWidths[n].MaxWidth
            //
            float totalMaxWidth = 0;
            for (int i = 0; i < maxColumns; i++)
            {
                totalMaxWidth += colWidths[i].MaxWidth;
            }
            float[] widthRatio = new float[maxColumns];
            for (int i = 0; i < maxColumns; i++)
            {
                widthRatio[i] = colWidths[i].MaxWidth / totalMaxWidth;
            }
            //
            //  Determine optimum column widths
            //
            float gutterWidth = width * 0.03f; // make each gutter 3% of total available width
            float totalGutters = gutterWidth * (maxColumns - 1);
            for (int i = 0; i < maxColumns; i++)
            {
                colWidths[i].MaxWidth = widthRatio[i] * (width - totalGutters);
            }

            foreach (TableRowNode row in childNodes)
            {
                float savedCurrentY = surface.CurrentY;
                float maxCurrentY = float.MinValue;
                int i = 0;
                float x = left;
                foreach (Node node in row)
                {
                    BlockNode block = (BlockNode)node;
                    surface.CurrentY = savedCurrentY;
                    block.Render(surface, x, colWidths[i].MaxWidth);
                    x += colWidths[i++].MaxWidth + gutterWidth;
                    if (maxCurrentY < surface.CurrentY)
                        maxCurrentY = surface.CurrentY;
                }
                surface.CurrentY = maxCurrentY;
            }
        }


        public override float Width
        {
            get
            {
                foreach (TableRowNode row in childNodes)
                {
                    // TODO: Calculate true Width, somehow
                }
                return 999;
            }
        }

        public override float Height
        {
            get
            {
                foreach (TableRowNode row in childNodes)
                {
                    // TODO: Calculate true Height, somehow
                }
                return 999;
            }
        }
    }



    public class MinMax
    {
        public float MinWidth { get; set; }
        public float MaxWidth { get; set; }

        public MinMax(float min, float max)
        {
            MinWidth = min;
            MaxWidth = max;
        }
    }

}
