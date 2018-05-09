using iText.Kernel.Pdf;
using System;
using System.IO;
using itext.pdfimage.Extensions;
using System.Drawing.Imaging;

namespace TestAppNet4
{
    class Program
    {
        static void Main(string[] args)
        {
            var pdf = File.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "landscape.pdf"), FileMode.Open);

            var reader = new PdfReader(pdf);
            var pdfDocument = new PdfDocument(reader);
            var bitmaps = pdfDocument.ConvertToBitmaps();

            foreach (var bitmap in bitmaps)
            {
                bitmap.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"landscape-{DateTime.Now.Ticks}.png"), ImageFormat.Png);
            }
        }
    }
}
