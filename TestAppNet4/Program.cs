using iText.Kernel.Pdf;
using System;
using System.IO;
using itext.pdfimage.Extensions;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq;

namespace TestAppNet4
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = "landscape-3.pdf";

            var pdf = File.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName), FileMode.Open);

            var reader = new PdfReader(pdf);
            var pdfDocument = new PdfDocument(reader);
            var bitmaps = pdfDocument.ConvertToBitmaps();

            foreach (var bitmap in bitmaps)
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output", $"{fileName}-{DateTime.Now.Ticks}.png");
                bitmap.Save(path, ImageFormat.Png);
                Process.Start(path);

                //bitmap.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output", $"landscape-2-{DateTime.Now.Ticks}.png"), ImageFormat.Png);
            }
        }
    }
}
