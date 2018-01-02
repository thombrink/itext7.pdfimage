using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener
{
    public class TextListener : LocationTextExtractionStrategy
    {
        public List<iTextTest.Models.TextChunk> TextChunks = new List<iTextTest.Models.TextChunk>();

        public override void EventOccurred(IEventData data, EventType type)
        {
            if (!type.Equals(EventType.RENDER_TEXT)) return;

            TextRenderInfo renderInfo = (TextRenderInfo)data;

            var font = renderInfo.GetFont().GetFontProgram().ToString();
            var fontRegex = Regex.Match(font, @"(?<=\+)[a-zA-Z\s]+");

            string fontName = fontRegex.Success ? fontRegex.Value : font;

            //var test = fontName.EndsWith("Bold") || fontName.EndsWith("Oblique");
            fontName.Replace("Bold", "");

            float curFontSize = renderInfo.GetFontSize();

            IList<TextRenderInfo> text = renderInfo.GetCharacterRenderInfos();
            foreach (TextRenderInfo character in text)
            {
                string letter = character.GetText();

                if (string.IsNullOrWhiteSpace(letter)) continue;

                //Get the bounding box for the chunk of text
                var bottomLeft = character.GetDescentLine().GetStartPoint();
                var topRight = character.GetAscentLine().GetEndPoint();

                //Create a rectangle from it
                var rect = new Rectangle(
                                                        bottomLeft.Get(Vector.I1),
                                                        bottomLeft.Get(Vector.I2),
                                                        topRight.Get(Vector.I1),
                                                        topRight.Get(Vector.I2)
                                                        );

                var currentChunk = new iTextTest.Models.TextChunk()
                {
                    Text = letter,
                    Rect = rect,
                    FontFamily = fontName,
                    FontSize = (int)curFontSize,
                    SpaceWidth = character.GetSingleSpaceWidth() / 2f
                };

                TextChunks.Add(currentChunk);
            }
        }
    }
}
