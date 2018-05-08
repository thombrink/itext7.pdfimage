using iText.Kernel.Pdf;
using itext7.pdfimage.Extensions;
using System;
using System.IO;

namespace TestAppNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bliep");

            var pdf = File.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "parse.pdf"), FileMode.Open);

            var reader = new PdfReader(pdf);
            var pdfDocument = new PdfDocument(reader);
            var streams = pdfDocument.ConvertToJpgStreams();

            Console.WriteLine("Bliep!");
        }
    }
}
