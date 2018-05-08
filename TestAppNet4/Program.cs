using iText.Kernel.Pdf;
using System;
using System.IO;
using itext7.pdfimage.Extensions;

namespace TestAppNet4
{
    class Program
    {
        static void Main(string[] args)
        {
            var pdf = File.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "parse.pdf"), FileMode.Open);

            var reader = new PdfReader(pdf);
            var pdfDocument = new PdfDocument(reader);
            var bitmaps = pdfDocument.ConvertToBitmaps();
        }
    }
}
