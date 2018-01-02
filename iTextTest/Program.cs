using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iTextTest.Extensions;
using System;
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Text;
using System.IO;

namespace iTextTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bliep");

            var pdf = File.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "parse.pdf"), FileMode.Open);

            var reader = new PdfReader(pdf);
            var pdfDocument = new PdfDocument(reader);

            FilteredEventListener listener = new FilteredEventListener();
            var textStrat = listener.AttachEventListener(new TextListener());
            var imageStrat = listener.AttachEventListener(new ImageListener());
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(pdfDocument.GetPage(1));

            var size = pdfDocument.GetDefaultPageSize();
            //var size = pdfDocument.GetFirstPage().GetPageSize();

            var width = size.GetWidth().PointsToPixels();
            var height = size.GetHeight().PointsToPixels();

            var text = textStrat.TextChunks;

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

                bmp.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "parse.jpg"), System.DrawingCore.Imaging.ImageFormat.Jpeg);

                Console.WriteLine("Bliep!");
            }
        }
    }
}