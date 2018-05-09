using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font;
using System.Text.RegularExpressions;

#if NET45
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
#else
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Imaging;
using System.DrawingCore.Text;
#endif

namespace itext.pdfimage.Extensions
{
    public static class ConvertExtensions
    {
        public static IEnumerable<Bitmap> ConvertToBitmaps(this PdfDocument pdfDocument)
        {
            var numberOfPages = pdfDocument.GetNumberOfPages();

            for (var i = 1; i <= numberOfPages; i++)
            {
                var currentPage = pdfDocument.GetPage(i);

                FilteredEventListener listener = new FilteredEventListener();
                var textStrat = listener.AttachEventListener(new TextListener());
                var imageStrat = listener.AttachEventListener(new ImageListener());
                PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
                processor.ProcessPageContent(currentPage);

                var size = currentPage.GetPageSize();

                var width = size.GetWidth().PointsToPixels();
                var height = size.GetHeight().PointsToPixels();

                using (Bitmap bmp = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                        foreach (var imageChunk in imageStrat.ImagesChunks)
                        {
                            var imgW = imageChunk.W.PointsToPixels();
                            var imgH = imageChunk.H.PointsToPixels();
                            var imgX = imageChunk.X.PointsToPixels();
                            var imgY = (size.GetHeight() - imageChunk.Y - imageChunk.H).PointsToPixels();

                            g.DrawImage(imageChunk.Image, imgX, imgY, imgW, imgH);
                        }

                        foreach (var chunk in textStrat.TextChunks)
                        {
                            var chunkX = chunk.Rect.GetX().PointsToPixels();
                            var chunkY = bmp.Height - chunk.Rect.GetY().PointsToPixels();

                            var fontSize = chunk.FontSize.PointsToPixels();

                            Font font;
                            try
                            {
                                font = new Font(chunk.FontFamily, fontSize, chunk.FontStyle, GraphicsUnit.Pixel);
                            }
                            catch (Exception ex)
                            {
                                //log error

                                font = new Font("Calibri", 11, chunk.FontStyle, GraphicsUnit.Pixel);
                            }

                            g.DrawString(chunk.Text, font, new SolidBrush(chunk.Color), chunkX, chunkY);
                        }

                        g.Flush();
                    }

                    yield return bmp;
                }
            }
        }

        public static IEnumerable<Stream> ConvertToJpgStreams(this PdfDocument pdfDocument)
        {
            foreach (var bmp in pdfDocument.ConvertToBitmaps())
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    yield return ms;
                }
            }
        }

        internal static FontStyle GetFontStyle(this FontNames fontNames)
        {
            var fontname = fontNames.GetFontName();
            var fontStyleRegex = Regex.Match(fontname, @"[-,][\w\s]+$");

            if (fontStyleRegex.Success)
            {
                var result = fontStyleRegex.Value.ToLower();
                if (result.Contains("bold"))
                {
                    return FontStyle.Bold;
                }
            }

            return FontStyle.Regular;
        }
    }
}
