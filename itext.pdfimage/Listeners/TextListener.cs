using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using itext.pdfimage.Extensions;
using itext.pdfimage.Models;
using System;
using System.Threading;

#if NET45
using System.Drawing;
#else
using System.DrawingCore;
#endif

namespace iText.Kernel.Pdf.Canvas.Parser.Listener
{
    public class TextListener : LocationTextExtractionStrategy
    {
        private readonly SortedDictionary<float, IChunk> chunkDictionairy;
        private Func<float> increaseCounter;

        public TextListener(SortedDictionary<float, IChunk> chunkDictionairy, Func<float> increaseCounter)
        {
            this.chunkDictionairy = chunkDictionairy;
            this.increaseCounter = increaseCounter;
        }

        public override void EventOccurred(IEventData data, EventType type)
        {
            if (!type.Equals(EventType.RENDER_TEXT)) return;

            TextRenderInfo renderInfo = (TextRenderInfo)data;

            float counter = increaseCounter();

            var font = renderInfo.GetFont().GetFontProgram();
            var originalFontName = font.ToString();
            var fontRegex = Regex.Match(originalFontName, @"(?<=\+)[a-zA-Z\s]+");

            string fontName = fontRegex.Success ? fontRegex.Value : originalFontName;

            var fontStyle = font.GetFontNames().GetFontStyle();

            float curFontSize = renderInfo.GetFontSize();

            if (curFontSize == 1)
            {
                var tm = renderInfo.GetTextMatrix();
                curFontSize = tm.Get(1);
            }

            float key = counter;

            IList<TextRenderInfo> text = renderInfo.GetCharacterRenderInfos();
            foreach (TextRenderInfo character in text)
            {
                string letter = character.GetText();

                if (string.IsNullOrWhiteSpace(letter)) continue;

                key += 0.001f;

                //var textRenderMode = character.GetTextRenderMode();
                //var opacity = character.GetGraphicsState().GetFillOpacity();

                //if (textRenderMode != 0 || opacity != 1)
                //{

                //}

                Color color;

                var fillColor = character.GetFillColor();
                var colors = fillColor.GetColorValue();
                if (colors.Length == 3)
                {
                    color = Color.FromArgb((int)(colors[0] * 255), (int)(colors[1] * 255), (int)(colors[2] * 255));
                }
                else if (colors.Length == 4)
                {
                    color = Color.FromArgb((int)(colors[0] * 255), (int)(colors[1] * 255), (int)(colors[2] * 255), (int)(colors[3] * 255));
                }
                else
                {
                    color = Color.Black;
                }

                //if ((color.R != 0 && color.G != 0 && color.B != 0) || color.A != 255)
                //{

                //}

                //if (letter.Contains("#"))
                //{

                //}

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

                var tm = character.GetTextMatrix();
                rect.SetX(tm.Get(Matrix.I31));
                rect.SetY(tm.Get(Matrix.I32));

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

                chunkDictionairy.Add(key, currentChunk);
            }

            base.EventOccurred(data, type);
        }
    }
}
