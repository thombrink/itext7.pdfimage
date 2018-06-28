using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font;
using System.Text.RegularExpressions;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace itext.pdfimage.Extensions
{
    public static class ConvertExtensions
    {
        public static IEnumerable<Bitmap> ConvertToBitmaps(this PdfDocument pdfDocument)
        {
            var converter = new PdfToImageConverter();
            return converter.ConvertToBitmaps(pdfDocument);
        }

        public static Bitmap ConvertPageToBitmap(this PdfPage pdfPage)
        {
            var converter = new PdfToImageConverter();
            return converter.ConvertToBitmap(pdfPage);
        }

        public static IEnumerable<Stream> ConvertToJpgStreams(this PdfDocument pdfDocument)
        {
            var converter = new PdfToImageConverter();
            return converter.ConvertToJpgStreams(pdfDocument);
        }

        public static Stream ConvertPageToJpg(this PdfPage pdfPage)
        {
            var converter = new PdfToImageConverter();
            return converter.ConvertToJpgStream(pdfPage);
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
