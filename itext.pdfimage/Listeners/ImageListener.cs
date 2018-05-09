using iText.Kernel.Pdf.Canvas.Parser.Data;
using itext.pdfimage.Models;
using System.Collections.Generic;
using System.IO;

#if NET45
using System.Drawing;
#else 
using System.DrawingCore;
#endif

namespace iText.Kernel.Pdf.Canvas.Parser.Listener
{
    public class ImageListener : FilteredEventListener
    {
        public List<ImageChunk> ImagesChunks { get; set; } = new List<ImageChunk>();

        public override void EventOccurred(IEventData data, EventType type)
        {
            if (type != EventType.RENDER_IMAGE) return;

            var renderInfo = (ImageRenderInfo)data;

            var matix = renderInfo.GetImageCtm();

            var startPoint = renderInfo.GetStartPoint();

            var imageChunk = new ImageChunk
            {
                X = matix.Get(Geom.Matrix.I31),
                Y = matix.Get(Geom.Matrix.I32),
                W = matix.Get(Geom.Matrix.I11),
                H = matix.Get(Geom.Matrix.I22),
                Image = Image.FromStream(new MemoryStream(renderInfo.GetImage().GetImageBytes()))
            };

            ImagesChunks.Add(imageChunk);

            base.EventOccurred(data, type);
        }
    }
}
