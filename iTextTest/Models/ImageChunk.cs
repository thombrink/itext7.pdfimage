using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Text;

namespace iTextTest.Models
{
    public class ImageChunk
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        public Image Image { get; set; }
    }
}
