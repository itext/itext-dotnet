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
using System.Collections.Generic;
using iText.Kernel.Colors;
using iText.Layout.Element;
using iText.Layout.Properties.Margins;

namespace iText.Layout.Testutil {
    /// <summary>Shared utility methods for page margin box test classes.</summary>
    /// <remarks>
    /// Shared utility methods for page margin box test classes.
    /// <para />Provides the standard
    /// <see cref="iText.Layout.Properties.Margins.PageMarginContent"/>
    /// configurations used
    /// across
    /// <see cref="iText.Layout.PageMarginsTest"/>
    /// and its related test classes. Extracting
    /// them here avoids duplication in standalone test classes that cannot extend
    /// <see cref="iText.Layout.PageMarginsTest"/>.
    /// </remarks>
    public sealed class PageMarginsTestUtil {
        private PageMarginsTestUtil() {
        }

        /// <summary>
        /// Returns a four-sided margin box configuration with coloured, labelled
        /// content in each region:
        /// </summary>
        public static IList<PageMarginContent> GetPageMargins1() {
            IList<PageMarginContent> elements = new List<PageMarginContent>();
            elements.Add(new PageMarginContent(MarginBoxName.TOP, new Div().Add(new Paragraph("TEST TOP MARGIN")).SetBackgroundColor
                (ColorConstants.PINK).SetHeight(200)));
            elements.Add(new PageMarginContent(MarginBoxName.RIGHT, new Div().Add(new Paragraph("TEST RIGHT MARGIN")).
                SetBackgroundColor(ColorConstants.YELLOW).SetWidth(200)));
            elements.Add(new PageMarginContent(MarginBoxName.BOTTOM, new Div().Add(new Paragraph("TEST BOTTOM MARGIN\nTEST BOTTOM MARGIN\nTEST BOTTOM MARGIN"
                )).SetBackgroundColor(ColorConstants.GREEN)));
            elements.Add(new PageMarginContent(MarginBoxName.LEFT, new Div().Add(new Paragraph("TEST LEFT MARGIN, TEST LEFT MARGIN"
                )).SetBackgroundColor(ColorConstants.BLUE)));
            return elements;
        }

        /// <summary>
        /// Returns a four-sided margin box configuration with a lighter colour
        /// palette and smaller fixed dimensions:
        /// </summary>
        public static IList<PageMarginContent> GetPageMargins2() {
            IList<PageMarginContent> elements = new List<PageMarginContent>();
            elements.Add(new PageMarginContent(MarginBoxName.TOP, new Div().Add(new Paragraph("TEST TOP MARGIN")).SetBackgroundColor
                (ColorConstants.LIGHT_GRAY).SetHeight(100)));
            elements.Add(new PageMarginContent(MarginBoxName.RIGHT, new Div().Add(new Paragraph("TEST RIGHT MARGIN")).
                SetBackgroundColor(ColorConstants.CYAN)));
            elements.Add(new PageMarginContent(MarginBoxName.BOTTOM, new Div().Add(new Paragraph("TEST BOTTOM MARGIN")
                ).SetBackgroundColor(ColorConstants.ORANGE)));
            elements.Add(new PageMarginContent(MarginBoxName.LEFT, new Div().Add(new Paragraph("TEST LEFT MARGIN")).SetBackgroundColor
                (ColorConstants.RED).SetWidth(100)));
            return elements;
        }
    }
}
