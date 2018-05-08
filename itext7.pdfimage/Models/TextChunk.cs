using System;
using System.Collections.Generic;
using System.Text;
using System.DrawingCore;

namespace itext7.pdfimage.Models
{
    public class TextChunk
    {
        public string Text { get; set; }
        public iText.Kernel.Geom.Rectangle Rect { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public FontStyle FontWeight { get; set; }
        public float SpaceWidth { get; set; }
    }
}
