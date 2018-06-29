using iText.Kernel.Pdf;
using itext.pdfimage.Extensions;
using System;
using System.IO;
using System.Drawing.Imaging;

namespace TestAppNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bliep");

            var pdf = File.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "wave.pdf"), FileMode.Open);

            var reader = new PdfReader(pdf);
            var pdfDocument = new PdfDocument(reader);
            var bitmaps = pdfDocument.ConvertToBitmaps();

            foreach (var bitmap in bitmaps)
            {
                bitmap.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"wave-{DateTime.Now.Ticks}.png"), ImageFormat.Png);
                bitmap.Dispose();
            }

            var page1 = pdfDocument.GetPage(1);
            var bitmap1 = page1.ConvertPageToBitmap();
            bitmap1.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"wave-page1-{DateTime.Now.Ticks}.png"), ImageFormat.Png);
            bitmap1.Dispose();

            Console.WriteLine("Bliep!");
        }
    }
}
