/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
