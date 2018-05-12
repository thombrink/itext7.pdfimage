using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font;
using System.Text.RegularExpressions;
using System.Collections;
using itext.pdfimage.Models;
using itext.pdfimage.Extensions;
using System.Threading;

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

namespace itext.pdfimage
{
    public class PdfToImageConverter
    {
        private static int counter;

        public IEnumerable<Bitmap> ConvertToBitmaps(PdfDocument pdfDocument)
        {
            counter = 0;

            var numberOfPages = pdfDocument.GetNumberOfPages();

            for (var i = 1; i <= numberOfPages; i++)
            {
                var currentPage = pdfDocument.GetPage(i);

                var rotation = currentPage.GetRotation();

                var chunkDictionairy = new SortedDictionary<float, IChunk>();

                FilteredEventListener listener = new FilteredEventListener();
                listener.AttachEventListener(new TextListener(chunkDictionairy, IncreaseCounter));
                listener.AttachEventListener(new ImageListener(chunkDictionairy, IncreaseCounter));
                PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
                processor.ProcessPageContent(currentPage);

                //var size = currentPage.GetPageSizeWithRotation();
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

                        foreach (var chunk in chunkDictionairy)
                        {
                            g.ResetTransform();

                            g.RotateTransform(-rotation);

                            if (chunk.Value is Models.ImageChunk imageChunk)
                            {
                                var imgW = imageChunk.W.PointsToPixels();
                                var imgH = imageChunk.H.PointsToPixels();
                                var imgX = imageChunk.X.PointsToPixels();
                                var imgY = (size.GetHeight() - imageChunk.Y - imageChunk.H).PointsToPixels();

                                g.TranslateTransform(imgX, imgY, MatrixOrder.Append);

                                g.DrawImage(imageChunk.Image, imgX, imgY, imgW, imgH);
                            }
                            else if (chunk.Value is Models.TextChunk textChunk)
                            {
                                var chunkX = textChunk.Rect.GetX().PointsToPixels();
                                var chunkY = bmp.Height - textChunk.Rect.GetY().PointsToPixels();

                                var fontSize = textChunk.FontSize.PointsToPixels();

                                Font font;
                                try
                                {
                                    font = new Font(textChunk.FontFamily, fontSize, textChunk.FontStyle, GraphicsUnit.Pixel);
                                    //font = new Font("Calibri", MeasuringExtensions.PointsToPixels(11), textChunk.FontStyle, GraphicsUnit.Pixel);
                                }
                                catch (Exception ex)
                                {
                                    //log error

                                    font = new Font("Calibri", MeasuringExtensions.PointsToPixels(11), textChunk.FontStyle, GraphicsUnit.Pixel);
                                }

                                g.TranslateTransform(chunkX, chunkY, MatrixOrder.Append);

                                //g.DrawRectangle(new Pen(textChunk.Color, 1), chunkX, chunkY, textChunk.Rect.GetWidth(), textChunk.Rect.GetHeight());

                                //g.DrawString(textChunk.Text, font, new SolidBrush(textChunk.Color), chunkX, chunkY);
                                g.DrawString(textChunk.Text, font, new SolidBrush(textChunk.Color), 0, 0);
                                //g.DrawString(textChunk.Text, font, new SolidBrush(textChunk.Color), new RectangleF(0, 0, textChunk.Rect.GetWidth(), textChunk.Rect.GetHeight()));
                            }
                        }

                        g.Flush();
                    }

                    yield return bmp;
                }
            }
        }

        public IEnumerable<Stream> ConvertToJpgStreams(PdfDocument pdfDocument)
        {
            foreach (var bmp in ConvertToBitmaps(pdfDocument))
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    yield return ms;
                }
            }
        }

        private Func<float> IncreaseCounter = () => counter = Interlocked.Increment(ref counter);
    }
}
