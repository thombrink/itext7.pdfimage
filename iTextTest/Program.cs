using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace iTextTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var pdf = File.Open(@"C:\Users\thomb\Desktop\parse6.pdf", FileMode.Open);

            var reader = new PdfReader(pdf);
            var pdfDocument = new PdfDocument(reader);
            //var parser = new PdfDocumentContentParser(pdfDocument);

            FilteredEventListener listener = new FilteredEventListener();
            var strat = listener.AttachEventListener(new TextLocationStrategy());
            var imageStrat = listener.AttachEventListener(new ImageLocationStrategy());
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(pdfDocument.GetPage(1));

            var size = pdfDocument.GetDefaultPageSize();
            //var size = pdfDocument.GetFirstPage().GetPageSize();

            var width = size.GetWidth().PointsToPixels();
            var height = size.GetHeight().PointsToPixels();

            var text = strat.ObjectResult;

            Bitmap bmp = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(bmp);

            g.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            //// Create string formatting options (used for alignment)
            //StringFormat format = new StringFormat()
            //{
            //    FormatFlags = StringFormatFlags.NoClip,
            //};
            //format.SetMeasurableCharacterRanges(characterRanges);


            foreach (var character in strat.ObjectResult)
            {
                //RectangleF rectf = new RectangleF(character.Rect.GetX(), bmp.Height - character.Rect.GetY(), character.Rect.GetWidth() * 10, character.Rect.GetHeight() * 10);

                var charX = character.Rect.GetX().PointsToPixels();
                var charY = bmp.Height - character.Rect.GetY().PointsToPixels();

                var fontSize = character.FontSize.PointsToPixels();

                g.DrawString(character.Text, new Font(character.FontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Black, charX, charY);
            }

            foreach(var image in imageStrat.Images)
            {
                var imgW = image.W.PointsToPixels();
                var imgH = image.H.PointsToPixels();
                var imgX = image.X.PointsToPixels();
                var imgY = (size.GetHeight() - image.Y - image.H).PointsToPixels();
                //var imgY = bmp.Height - image.Y.PointsToPixels() - imgH;

                g.DrawImage(image.Image, imgX, imgY, imgW, imgH);//, new System.DrawingCore.Rectangle(imgX, imgY, image.Image.Width, image.Image.Height), GraphicsUnit.Pixel);
            }

            g.Flush();

            bmp.Save(@"C:\Users\thomb\Desktop\parse6.jpg", System.DrawingCore.Imaging.ImageFormat.Jpeg);
        }
    }

    class TextLocationStrategy : LocationTextExtractionStrategy
    {
        public List<TextChunk> ObjectResult = new List<TextChunk>();

        //private TextChunk currentChunk = null;

        public override void EventOccurred(IEventData data, EventType type)
        {
            if (!type.Equals(EventType.RENDER_TEXT)) return;

            TextRenderInfo renderInfo = (TextRenderInfo)data;

            var font = renderInfo.GetFont().GetFontProgram().ToString();
            var fontRegex = Regex.Match(font, @"(?<=\+)[a-zA-Z]+");

            string fontName = fontRegex.Success ? fontRegex.Value : font;
            //string curFont = "Calibri";

            //var test = fontName.EndsWith("Bold") || fontName.EndsWith("Oblique");
            fontName.Replace("Bold", "");

            float curFontSize = renderInfo.GetFontSize();

            IList<TextRenderInfo> text = renderInfo.GetCharacterRenderInfos();
            //foreach (TextRenderInfo t in text)
            //{
            /*string letter = t.GetText();

            if (string.IsNullOrWhiteSpace(letter))
            {
                //if(currentChunk != null)
                //{
                //    ObjectResult.Add(currentChunk);
                //    currentChunk = null;
                //}

                continue;
            }

            Vector letterStart = t.GetBaseline().GetStartPoint();
            Vector letterEnd = t.GetAscentLine().GetEndPoint();
            iText.Kernel.Geom.Rectangle letterRect = new iText.Kernel.Geom.Rectangle(letterStart.Get(0), letterStart.Get(1), letterEnd.Get(0) - letterStart.Get(0), letterEnd.Get(1) - letterStart.Get(1));

            //if (currentChunk == null)
            //{
            //    currentChunk = new TextChunk
            //    {
            //        Text = letter,
            //        Rect = letterRect,
            //        FontFamily = curFont,
            //        FontSize = (int)curFontSize,
            //        SpaceWidth = t.GetSingleSpaceWidth() / 2f
            //    };
            //}
            //else
            //{
            //    currentChunk.Text += letter;
            //    currentChunk.Rect.SetX(letterRect.GetX());
            //    currentChunk.SpaceWidth += t.GetSingleSpaceWidth() / 2f;
            //}

            var currentChunk = new TextChunk
            {
                Text = letter,
                Rect = letterRect,
                FontFamily = curFont,
                FontSize = (int)curFontSize,
                SpaceWidth = t.GetSingleSpaceWidth() / 2f
            };*/

            foreach (TextRenderInfo character in text)
            {
                string letter = character.GetText();

                if (string.IsNullOrWhiteSpace(letter)) continue;

                //Get the bounding box for the chunk of text
                var bottomLeft = character.GetDescentLine().GetStartPoint();
                var topRight = character.GetAscentLine().GetEndPoint();

                //Create a rectangle from it
                var rect = new iText.Kernel.Geom.Rectangle(
                                                        bottomLeft.Get(Vector.I1),
                                                        bottomLeft.Get(Vector.I2),
                                                        topRight.Get(Vector.I1),
                                                        topRight.Get(Vector.I2)
                                                        );

                var currentChunk = new TextChunk()
                {
                    Text = letter,
                    Rect = rect,
                    FontFamily = fontName,
                    FontSize = (int)curFontSize,
                    SpaceWidth = character.GetSingleSpaceWidth() / 2f
                };

                //Add this to our main collection
                //this.myPoints.Add(new RectAndText(rect, renderInfo.GetText()));

                ObjectResult.Add(currentChunk);
                //}
            }
        }
    }

    class ImageLocationStrategy : FilteredEventListener
    {
        public List<ImageChunk> Images { get; set; } = new List<ImageChunk>();

        public override void EventOccurred(IEventData data, EventType type)
        {
            if (type != EventType.RENDER_IMAGE) return;

            var renderInfo = (ImageRenderInfo)data;

            var matix = renderInfo.GetImageCtm();

            var startPoint = renderInfo.GetStartPoint();

            var imageChunk = new ImageChunk
            {
                X = matix.Get(iText.Kernel.Geom.Matrix.I31),
                Y = matix.Get(iText.Kernel.Geom.Matrix.I32),
                W = matix.Get(iText.Kernel.Geom.Matrix.I11),
                H = matix.Get(iText.Kernel.Geom.Matrix.I22),
                Image = Image.FromStream(new MemoryStream(renderInfo.GetImage().GetImageBytes()))
            };

            Images.Add(imageChunk);

            base.EventOccurred(data, type);
        }
    }

    public static class Extensions
    {
        //72 points == 1
        //72px x 72px is 1inch x 1inch at a 72dpi resolution

        public static int Dpi { get; set; } = 300;

        public static float PixelsToPoints(this float value, int? dpi = null)
        {
            return value / (dpi ?? Dpi) * 72;
        }

        public static int PointsToPixels(this int value, int? dpi = null)
        {
            return PointsToPixels((float)value);
        }

        public static int PointsToPixels(this float value, int? dpi = null)
        {
            return (int)(value * (dpi ?? Dpi) / 72);
        }
    }

    public class TextChunk
    {
        public string Text { get; set; }
        public iText.Kernel.Geom.Rectangle Rect { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public float SpaceWidth { get; set; }
    }

    public class ImageChunk
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        public Image Image { get; set; }
    }
}
