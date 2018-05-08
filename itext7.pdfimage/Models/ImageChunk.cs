#if NET45
using System.Drawing;
#else 
using System.DrawingCore;
#endif

namespace itext7.pdfimage.Models
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
