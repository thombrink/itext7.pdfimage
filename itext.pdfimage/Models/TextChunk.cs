using System;
using System.Collections.Generic;
using System.Text;

#if NET45
using System.Drawing;
#else
using System.DrawingCore;
#endif

namespace itext.pdfimage.Models
{
    public class TextChunk : IChunk
    {
        public string Text { get; set; }
        public iText.Kernel.Geom.Rectangle Rect { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public FontStyle FontStyle { get; set; }
        public float SpaceWidth { get; set; }
        public Color Color { get; internal set; }
    }
}
