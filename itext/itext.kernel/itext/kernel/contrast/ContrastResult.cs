/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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

namespace iText.Kernel.Contrast {
    /// <summary>Represents the complete contrast analysis result for a single text element.</summary>
    /// <remarks>
    /// Represents the complete contrast analysis result for a single text element.
    /// <para />
    /// This class encapsulates all the contrast information for a text element, including
    /// the text itself and a list of all background elements that it overlaps with, along
    /// with their respective contrast ratios.
    /// <para />
    /// Each text element may have multiple background elements behind it, especially in
    /// complex PDF layouts. This class collects all such relationships to provide a
    /// comprehensive view of the text's contrast characteristics for accessibility analysis.
    /// </remarks>
    public class ContrastResult {
        private readonly TextColorInfo textRenderInfo;

        private readonly IList<ContrastResult.OverlappingArea> overlappingAreas;

        private readonly int pageNumber;

        /// <summary>
        /// Constructs a new
        /// <see cref="ContrastResult"/>
        /// for the specified text element.
        /// </summary>
        /// <remarks>
        /// Constructs a new
        /// <see cref="ContrastResult"/>
        /// for the specified text element.
        /// <para />
        /// The result is initialized with an empty list of background entries, which should
        /// be populated using
        /// <see cref="AddContrastResult(OverlappingArea)"/>.
        /// </remarks>
        /// <param name="textRenderInfo">the text element for which contrast is being analyzed</param>
        /// <param name="pageNumber">the page number where the text element is located</param>
        public ContrastResult(TextColorInfo textRenderInfo, int pageNumber) {
            this.textRenderInfo = textRenderInfo;
            this.pageNumber = pageNumber;
            this.overlappingAreas = new List<ContrastResult.OverlappingArea>();
        }

        /// <summary>Gets the page number where the text element is located.</summary>
        /// <returns>the page number</returns>
        public virtual int GetPageNumber() {
            return pageNumber;
        }

        /// <summary>Gets the text render information for this contrast result.</summary>
        /// <remarks>
        /// Gets the text render information for this contrast result.
        /// <para />
        /// The text information includes the character, parent text, color, geometric path,
        /// and font size of the text element being analyzed.
        /// </remarks>
        /// <returns>the text render information</returns>
        public virtual TextColorInfo GetTextRenderInfo() {
            return textRenderInfo;
        }

        /// <summary>Adds a background contrast entry to this result.</summary>
        /// <remarks>
        /// Adds a background contrast entry to this result.
        /// <para />
        /// Each entry represents a background element that the text overlaps with, along
        /// with the calculated contrast ratio between the text color and background color.
        /// Multiple entries indicate that the text appears over multiple backgrounds.
        /// </remarks>
        /// <param name="overlappingArea">the contrast result entry containing background information and contrast ratio
        ///     </param>
        public virtual void AddContrastResult(ContrastResult.OverlappingArea overlappingArea) {
            this.overlappingAreas.Add(overlappingArea);
        }

        /// <summary>Gets all the background contrast entries for this text element.</summary>
        /// <remarks>
        /// Gets all the background contrast entries for this text element.
        /// <para />
        /// Each entry in the list represents a background element that the text overlaps with,
        /// containing the background's color, path, and the calculated contrast ratio.
        /// The list may be empty if no backgrounds were detected, or may contain multiple
        /// entries if the text overlaps multiple background elements.
        /// </remarks>
        /// <returns>an unmodifiable view of the list of contrast result entries</returns>
        public virtual IList<ContrastResult.OverlappingArea> GetOverlappingAreas() {
            return new List<ContrastResult.OverlappingArea>(overlappingAreas);
        }

        /// <summary>Represents a single contrast analysis result entry between text and a background element.</summary>
        /// <remarks>
        /// Represents a single contrast analysis result entry between text and a background element.
        /// <para />
        /// This class encapsulates the information about a specific background element that intersects
        /// with a text element, along with the calculated contrast ratio between them. It is used as
        /// part of a
        /// <see cref="ContrastResult"/>
        /// to provide detailed information about all backgrounds
        /// that contribute to the overall contrast of a text element.
        /// <para />
        /// The contrast ratio is calculated according to WCAG 2.1 guidelines and ranges from 1:1
        /// (no contrast) to 21:1 (maximum contrast between black and white).
        /// </remarks>
        public class OverlappingArea {
            private readonly BackgroundColorInfo backgroundRenderInfo;

            private readonly double contrastRatio;

            private double overlapRatio;

            /// <summary>Constructs a new ContrastResultEntry with the specified background information and contrast ratio.
            ///     </summary>
            /// <param name="backgroundRenderInfo">the background element that was analyzed for contrast</param>
            /// <param name="contrastRatio">
            /// the calculated contrast ratio between the text and this background,
            /// according to WCAG 2.1 guidelines (ranges from 1.0 to 21.0)
            /// </param>
            public OverlappingArea(BackgroundColorInfo backgroundRenderInfo, double contrastRatio) {
                this.backgroundRenderInfo = backgroundRenderInfo;
                this.contrastRatio = contrastRatio;
            }

            /// <summary>Gets the background render information for this contrast entry.</summary>
            /// <remarks>
            /// Gets the background render information for this contrast entry.
            /// <para />
            /// The background information includes the color and geometric path of the background
            /// element that was compared against the text.
            /// </remarks>
            /// <returns>the background render information</returns>
            public virtual BackgroundColorInfo GetBackgroundRenderInfo() {
                return backgroundRenderInfo;
            }

            /// <summary>Gets the contrast ratio between the text and this background element.</summary>
            /// <remarks>
            /// Gets the contrast ratio between the text and this background element.
            /// <para />
            /// The contrast ratio is calculated according to WCAG 2.1 guidelines:
            /// <para />
            /// *1.0 indicates no contrast (identical colors)
            /// *21.0 is the maximum contrast (black and white)
            /// </remarks>
            /// <returns>the contrast ratio value, ranging from 1.0 to 21.0</returns>
            public virtual double GetContrastRatio() {
                return contrastRatio;
            }

            /// <summary>Gets the percentage of the text area that overlaps with this background element.</summary>
            /// <returns>the overlapping area in percentage.</returns>
            public virtual double GetOverlapRatio() {
                return overlapRatio;
            }

            /// <summary>Sets the percentage of the text area that overlaps with this background element.</summary>
            /// <remarks>
            /// Sets the percentage of the text area that overlaps with this background element.
            /// Should be a value between 0 and 1 representing 0% to 100%.
            /// </remarks>
            /// <param name="overlappingAreaInPercentage">the overlapping area in percentage.</param>
            public virtual void SetOverlapRatio(double overlappingAreaInPercentage) {
                if (overlappingAreaInPercentage < 0 || overlappingAreaInPercentage > 1) {
                    throw new ArgumentException("Overlap ratio must be between 0 and 1.");
                }
                this.overlapRatio = overlappingAreaInPercentage;
            }
        }
    }
}
