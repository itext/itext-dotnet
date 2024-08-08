/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>This class is designed to search for the occurrences of a regular expression and return the resultant rectangles.
    ///     </summary>
    /// <remarks>
    /// This class is designed to search for the occurrences of a regular expression and return the resultant rectangles.
    /// Do note that this class holds all text locations and can't be used for processing multiple pages.
    /// If you want to extract text from several pages of pdf document you have to create a new instance
    /// of
    /// <see cref="RegexBasedLocationExtractionStrategy"/>
    /// for each page.
    /// <para />
    /// Here is an example of usage with new instance per each page:
    /// <c>
    /// PdfDocument document = new PdfDocument(new PdfReader("..."));
    /// for (int i = 1; i &lt;= document.getNumberOfPages(); ++i) {
    /// RegexBasedLocationExtractionStrategy extractionStrategy = new RegexBasedLocationExtractionStrategy("");
    /// PdfCanvasProcessor processor = new PdfCanvasProcessor(extractionStrategy);
    /// processor.processPageContent(document.getPage(i));
    /// for (IPdfTextLocation location : extractionStrategy.getResultantLocations()) {
    /// //process locations ...
    /// }
    /// }
    /// </c>
    /// </remarks>
    public class RegexBasedLocationExtractionStrategy : ILocationExtractionStrategy {
        private const float EPS = 1.0E-4F;

        private readonly Regex pattern;

        private readonly IList<CharacterRenderInfo> parseResult = new List<CharacterRenderInfo>();

        public RegexBasedLocationExtractionStrategy(String regex) {
            this.pattern = iText.Commons.Utils.StringUtil.RegexCompile(regex);
        }

        public RegexBasedLocationExtractionStrategy(Regex pattern) {
            this.pattern = pattern;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICollection<IPdfTextLocation> GetResultantLocations() {
            // align characters in "logical" order
            JavaCollectionsUtil.Sort(parseResult, new TextChunkLocationBasedComparator(new DefaultTextChunkLocationComparator
                ()));
            // process parse results
            IList<IPdfTextLocation> retval = new List<IPdfTextLocation>();
            CharacterRenderInfo.StringConversionInfo txt = CharacterRenderInfo.MapString(parseResult);
            Matcher mat = iText.Commons.Utils.Matcher.Match(pattern, txt.text);
            while (mat.Find()) {
                int? startIndex = GetStartIndex(txt.indexMap, mat.Start(), txt.text);
                int? endIndex = GetEndIndex(txt.indexMap, mat.End() - 1);
                if (startIndex != null && endIndex != null && startIndex <= endIndex) {
                    foreach (Rectangle r in ToRectangles(parseResult.SubList(startIndex.Value, endIndex.Value + 1))) {
                        retval.Add(new DefaultPdfTextLocation(r, mat.Group(0)));
                    }
                }
            }
            /* sort
            * even though the return type is Collection<Rectangle>, we apply a sorting algorithm here
            * This is to ensure that tests that use this functionality (for instance to generate pdf with
            * areas of interest highlighted) will not break when compared.
            */
            JavaCollectionsUtil.Sort(retval, new RegexBasedLocationExtractionStrategy.PdfTextLocationComparator());
            // ligatures can produces same rectangle
            RemoveDuplicates(retval);
            return retval;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void EventOccurred(IEventData data, EventType type) {
            parseResult.AddAll(ToCRI((TextRenderInfo)data));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICollection<EventType> GetSupportedEvents() {
            return JavaCollectionsUtil.Singleton(EventType.RENDER_TEXT);
        }

        /// <summary>
        /// Convert
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo"/>
        /// to
        /// <see cref="CharacterRenderInfo"/>
        /// This method is public and not final so that custom implementations can choose to override it.
        /// </summary>
        /// <remarks>
        /// Convert
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo"/>
        /// to
        /// <see cref="CharacterRenderInfo"/>
        /// This method is public and not final so that custom implementations can choose to override it.
        /// Other implementations of
        /// <c>CharacterRenderInfo</c>
        /// may choose to store different properties than
        /// merely the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// describing the bounding box. E.g. a custom implementation might choose to
        /// store
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// information as well, to better match the content surrounding the redaction
        /// <see cref="iText.Kernel.Geom.Rectangle"/>.
        /// </remarks>
        /// <param name="tri">
        /// 
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo"/>
        /// object
        /// </param>
        /// <returns>
        /// a list of
        /// <see cref="CharacterRenderInfo"/>
        /// s which represents the passed
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo"/>
        /// ?
        /// </returns>
        protected internal virtual IList<CharacterRenderInfo> ToCRI(TextRenderInfo tri) {
            IList<CharacterRenderInfo> cris = new List<CharacterRenderInfo>();
            foreach (TextRenderInfo subTri in tri.GetCharacterRenderInfos()) {
                cris.Add(new CharacterRenderInfo(subTri));
            }
            return cris;
        }

        /// <summary>
        /// Converts
        /// <see cref="CharacterRenderInfo"/>
        /// objects to
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// s
        /// This method is protected and not final so that custom implementations can choose to override it.
        /// </summary>
        /// <remarks>
        /// Converts
        /// <see cref="CharacterRenderInfo"/>
        /// objects to
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// s
        /// This method is protected and not final so that custom implementations can choose to override it.
        /// E.g. other implementations may choose to add padding/margin to the Rectangles.
        /// This method also offers a convenient access point to the mapping of
        /// <see cref="CharacterRenderInfo"/>
        /// to
        /// <see cref="iText.Kernel.Geom.Rectangle"/>.
        /// This mapping enables (custom implementations) to match color of text in redacted Rectangles,
        /// or match color of background, by the mere virtue of offering access to the
        /// <see cref="CharacterRenderInfo"/>
        /// objects
        /// that generated the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>.
        /// </remarks>
        /// <param name="cris">
        /// list of
        /// <see cref="CharacterRenderInfo"/>
        /// objects
        /// </param>
        /// <returns>an array containing the elements of this list</returns>
        protected internal virtual IList<Rectangle> ToRectangles(IList<CharacterRenderInfo> cris) {
            IList<Rectangle> retval = new List<Rectangle>();
            if (cris.IsEmpty()) {
                return retval;
            }
            int prev = 0;
            int curr = 0;
            while (curr < cris.Count) {
                while (curr < cris.Count && cris[curr].SameLine(cris[prev])) {
                    curr++;
                }
                Rectangle resultRectangle = null;
                foreach (CharacterRenderInfo cri in cris.SubList(prev, curr)) {
                    // in case letters are rotated (imagine text being written with an angle of 90 degrees)
                    resultRectangle = Rectangle.GetCommonRectangle(resultRectangle, cri.GetBoundingBox());
                }
                retval.Add(resultRectangle);
                prev = curr;
            }
            // return
            return retval;
        }

        private void RemoveDuplicates(IList<IPdfTextLocation> sortedList) {
            IPdfTextLocation lastItem = null;
            int orgSize = sortedList.Count;
            for (int i = orgSize - 1; i >= 0; i--) {
                IPdfTextLocation currItem = sortedList[i];
                Rectangle currRect = currItem.GetRectangle();
                if (lastItem != null && currRect.EqualsWithEpsilon(lastItem.GetRectangle())) {
                    sortedList.Remove(currItem);
                }
                lastItem = currItem;
            }
        }

        private static int? GetStartIndex(IDictionary<int, int?> indexMap, int index, String txt) {
            while (!indexMap.ContainsKey(index) && index < txt.Length) {
                index++;
            }
            return indexMap.Get(index);
        }

        private static int? GetEndIndex(IDictionary<int, int?> indexMap, int index) {
            while (!indexMap.ContainsKey(index) && index >= 0) {
                index--;
            }
            return indexMap.Get(index);
        }

        private sealed class PdfTextLocationComparator : IComparer<IPdfTextLocation> {
            public int Compare(IPdfTextLocation l1, IPdfTextLocation l2) {
                Rectangle o1 = l1.GetRectangle();
                Rectangle o2 = l2.GetRectangle();
                if (Math.Abs(o1.GetY() - o2.GetY()) < EPS) {
                    return Math.Abs(o1.GetX() - o2.GetX()) < EPS ? 0 : ((o2.GetX() - o1.GetX()) > EPS ? -1 : 1);
                }
                else {
                    return (o2.GetY() - o1.GetY()) > EPS ? -1 : 1;
                }
            }
        }
    }
}
