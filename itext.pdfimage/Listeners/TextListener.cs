using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using itext.pdfimage.Extensions;

#if NET45
using System.Drawing;
#else
using System.DrawingCore;
#endif

namespace iText.Kernel.Pdf.Canvas.Parser.Listener
{
    public class TextListener : LocationTextExtractionStrategy
    {
        public List<itext.pdfimage.Models.TextChunk> TextChunks = new List<itext.pdfimage.Models.TextChunk>();

        public override void EventOccurred(IEventData data, EventType type)
        {
            if (!type.Equals(EventType.RENDER_TEXT)) return;

            TextRenderInfo renderInfo = (TextRenderInfo)data;

            var font = renderInfo.GetFont().GetFontProgram();
            var originalFontName = font.ToString();
            var fontRegex = Regex.Match(originalFontName, @"(?<=\+)[a-zA-Z\s]+");

            string fontName = fontRegex.Success ? fontRegex.Value : originalFontName;

            var fontStyle = font.GetFontNames().GetFontStyle();

            float curFontSize = renderInfo.GetFontSize();            

            IList<TextRenderInfo> text = renderInfo.GetCharacterRenderInfos();
            foreach (TextRenderInfo character in text)
            {
                string letter = character.GetText();

                Color color;

                //var fillColor = character.GetFillColor();
                //var colors = fillColor.GetColorValue();
                //if (colors.Length == 3)
                //{
                //    color = Color.FromArgb((int)colors[0], (int)colors[1], (int)colors[2]);
                //}
                //else if (colors.Length == 4)
                //{
                //    color = Color.FromArgb((int)colors[0], (int)colors[1], (int)colors[2], (int)colors[3]);
                //}
                //else
                //{
                    color = Color.Black;
                //}

                //if ((color.R != 0 && color.G != 0 && color.B != 0) || color.A != 255)
                //{

                //}

                //if (letter.Contains("#"))
                //{

                //}

                if (string.IsNullOrWhiteSpace(letter)) continue;

                //Get the bounding box for the chunk of text
                var bottomLeft = character.GetDescentLine().GetStartPoint();
                var topRight = character.GetAscentLine().GetEndPoint();

                //Create a rectangle from it
                var rect = new Geom.Rectangle(
                                                        bottomLeft.Get(Vector.I1),
                                                        topRight.Get(Vector.I2),
                                                        topRight.Get(Vector.I1),
                                                        topRight.Get(Vector.I2)
                                                        );

                var currentChunk = new itext.pdfimage.Models.TextChunk()
                {
                    Text = letter,
                    Rect = rect,
                    FontFamily = fontName,
                    FontSize = (int)curFontSize,
                    FontStyle = fontStyle,
                    Color = color,
                    SpaceWidth = character.GetSingleSpaceWidth() / 2f
                };

                TextChunks.Add(currentChunk);
            }
        }
    }
}
