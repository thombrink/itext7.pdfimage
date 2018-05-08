using iText.Kernel.Pdf;
using itext7.pdfimage.Extensions;
using System;
using System.IO;
using System.Globalization;

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

            foreach(var bitmap in bitmaps){
                bitmap.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"wave-{DateTime.Now.Ticks}.png"), System.DrawingCore.Imaging.ImageFormat.Png);
            }

            Console.WriteLine("Bliep!");
        }
    }
}
