using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.IO;

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

namespace itext7.pdfimage.Extensions
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

                        foreach (var character in textStrat.TextChunks)
                        {
                            var charX = character.Rect.GetX().PointsToPixels();
                            var charY = bmp.Height - character.Rect.GetY().PointsToPixels();

                            var fontSize = character.FontSize.PointsToPixels();

                            Font font;
                            try
                            {
                                font = new Font(character.FontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                            }
                            catch (Exception ex)
                            {
                                //log error

                                font = new Font("Calibri", 11, FontStyle.Regular, GraphicsUnit.Pixel);
                            }

                            g.DrawString(character.Text, font, Brushes.Black, charX, charY);
                        }

                        foreach (var imageChunk in imageStrat.ImagesChunks)
                        {
                            var imgW = imageChunk.W.PointsToPixels();
                            var imgH = imageChunk.H.PointsToPixels();
                            var imgX = imageChunk.X.PointsToPixels();
                            var imgY = (size.GetHeight() - imageChunk.Y - imageChunk.H).PointsToPixels();

                            g.DrawImage(imageChunk.Image, imgX, imgY, imgW, imgH);
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
    }
}
