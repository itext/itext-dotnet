/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
namespace iText.Kernel.Pdf {
    /// <summary>Specifying the characteristics of the annotation’s border.</summary>
    /// <remarks>
    /// Specifying the characteristics of the annotation’s border.
    /// See ISO 32000-1 12.5.2, Table 164 - Entries common to all annotation dictionaries, Key - border.
    /// <para />
    /// Note (PDF 1.2): The dictionaries for some annotation types  can include the BS (border style) entry.
    /// That entry specifies a border style dictionary that has more settings than this class.
    /// If an annotation has BS entry, then
    /// <see cref="PdfAnnotationBorder"/>
    /// is ignored.
    /// </remarks>
    public class PdfAnnotationBorder : PdfObjectWrapper<PdfArray> {
        /// <summary>
        /// Creates a
        /// <see cref="PdfAnnotationBorder"/>
        /// with three numbers defining the horizontal
        /// corner radius, vertical corner radius, and border width, all in default user
        /// space units.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="PdfAnnotationBorder"/>
        /// with three numbers defining the horizontal
        /// corner radius, vertical corner radius, and border width, all in default user
        /// space units. If the corner radii are 0, the border has square (not rounded)
        /// corners; if the border width is 0, no border is drawn.
        /// </remarks>
        /// <param name="hRadius">horizontal corner radius</param>
        /// <param name="vRadius">vertical corner radius</param>
        /// <param name="width">width of the border</param>
        public PdfAnnotationBorder(float hRadius, float vRadius, float width)
            : this(hRadius, vRadius, width, null) {
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfAnnotationBorder"/>
        /// with three numbers defining the horizontal
        /// corner radius, vertical corner radius, and border width, all in default user
        /// space units and a dash pattern for the border lines.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="PdfAnnotationBorder"/>
        /// with three numbers defining the horizontal
        /// corner radius, vertical corner radius, and border width, all in default user
        /// space units and a dash pattern for the border lines. If the corner radii are 0,
        /// the border has square (not rounded) corners; if the border width is 0, no border is drawn.
        /// </remarks>
        /// <param name="hRadius">horizontal corner radius</param>
        /// <param name="vRadius">vertical corner radius</param>
        /// <param name="width">width of the border</param>
        /// <param name="dash">the dash pattern</param>
        public PdfAnnotationBorder(float hRadius, float vRadius, float width, PdfDashPattern dash)
            : base(new PdfArray(new float[] { hRadius, vRadius, width })) {
            if (dash != null) {
                PdfArray dashArray = new PdfArray();
                GetPdfObject().Add(dashArray);
                if (dash.GetDash() >= 0) {
                    dashArray.Add(new PdfNumber(dash.GetDash()));
                }
                if (dash.GetGap() >= 0) {
                    dashArray.Add(new PdfNumber(dash.GetGap()));
                }
                if (dash.GetPhase() >= 0) {
                    GetPdfObject().Add(new PdfNumber(dash.GetPhase()));
                }
            }
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
