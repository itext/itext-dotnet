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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    /// <summary>The purpose of a line annotation is to display a single straight line on the page.</summary>
    /// <remarks>
    /// The purpose of a line annotation is to display a single straight line on the page.
    /// When opened, it displays a pop-up window containing the text of the associated note.
    /// See also ISO-320001 12.5.6.7 "Line Annotations".
    /// </remarks>
    public class PdfLineAnnotation : PdfMarkupAnnotation {
        /// <summary>
        /// Creates a
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </summary>
        /// <param name="rect">
        /// the annotation rectangle, defining the location of the annotation on the page
        /// in default user space units. See
        /// <see cref="PdfAnnotation.SetRectangle(iText.Kernel.Pdf.PdfArray)"/>.
        /// </param>
        /// <param name="line">
        /// an array of four numbers, [x1 y1 x2 y2], specifying the starting and ending coordinates
        /// of the line in default user space. See also
        /// <see cref="GetLine()"/>.
        /// </param>
        public PdfLineAnnotation(Rectangle rect, float[] line)
            : base(rect) {
            Put(PdfName.L, new PdfArray(line));
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfLineAnnotation"/>
        /// instance based on
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// instance, that represents existing annotation object in the document.
        /// </summary>
        /// <param name="pdfObject">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// representing annotation object
        /// </param>
        /// <seealso cref="PdfAnnotation.MakeAnnotation(iText.Kernel.Pdf.PdfObject)"/>
        protected internal PdfLineAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary><inheritDoc/></summary>
        public override PdfName GetSubtype() {
            return PdfName.Line;
        }

        /// <summary>
        /// An array of four numbers, [x1 y1 x2 y2], specifying the starting and ending coordinates of the line
        /// in default user space.
        /// </summary>
        /// <remarks>
        /// An array of four numbers, [x1 y1 x2 y2], specifying the starting and ending coordinates of the line
        /// in default user space. If the
        /// <see cref="iText.Kernel.Pdf.PdfName.LL"/>
        /// entry is present, this value represents
        /// the endpoints of the leader lines rather than the endpoints of the line itself.
        /// </remarks>
        /// <returns>An array of four numbers specifying the starting and ending coordinates of the line in default user space.
        ///     </returns>
        public virtual PdfArray GetLine() {
            return GetPdfObject().GetAsArray(PdfName.L);
        }

        /// <summary>The dictionaries for some annotation types (such as free text and polygon annotations) can include the BS entry.
        ///     </summary>
        /// <remarks>
        /// The dictionaries for some annotation types (such as free text and polygon annotations) can include the BS entry.
        /// That entry specifies a border style dictionary that has more settings than the array specified for the Border
        /// entry (see
        /// <see cref="PdfAnnotation.GetBorder()"/>
        /// ). If an annotation dictionary includes the BS entry, then the Border
        /// entry is ignored. If annotation includes AP (see
        /// <see cref="PdfAnnotation.GetAppearanceDictionary()"/>
        /// ) it takes
        /// precedence over the BS entry. For more info on BS entry see ISO-320001, Table 166.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which is a border style dictionary or null if it is not specified.
        /// </returns>
        public virtual PdfDictionary GetBorderStyle() {
            return GetPdfObject().GetAsDictionary(PdfName.BS);
        }

        /// <summary>
        /// Sets border style dictionary that has more settings than the array specified for the Border entry (
        /// <see cref="PdfAnnotation.GetBorder()"/>
        /// ).
        /// </summary>
        /// <remarks>
        /// Sets border style dictionary that has more settings than the array specified for the Border entry (
        /// <see cref="PdfAnnotation.GetBorder()"/>
        /// ).
        /// See ISO-320001, Table 166 and
        /// <see cref="GetBorderStyle()"/>
        /// for more info.
        /// </remarks>
        /// <param name="borderStyle">
        /// a border style dictionary specifying the line width and dash pattern that shall be used
        /// in drawing the annotation’s border.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetBorderStyle(PdfDictionary borderStyle) {
            return (iText.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.BS, borderStyle);
        }

        /// <summary>Setter for the annotation's preset border style.</summary>
        /// <remarks>
        /// Setter for the annotation's preset border style. Possible values are
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_SOLID"/>
        /// - A solid rectangle surrounding the annotation.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_DASHED"/>
        /// - A dashed rectangle surrounding the annotation.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_BEVELED"/>
        /// - A simulated embossed rectangle that appears to be raised above the surface of the page.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_INSET"/>
        /// - A simulated engraved rectangle that appears to be recessed below the surface of the page.
        /// </description></item>
        /// <item><description>
        /// <see cref="PdfAnnotation.STYLE_UNDERLINE"/>
        /// - A single line along the bottom of the annotation rectangle.
        /// </description></item>
        /// </list>
        /// See also ISO-320001, Table 166.
        /// </remarks>
        /// <param name="style">The new value for the annotation's border style.</param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        /// <seealso cref="GetBorderStyle()"/>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetBorderStyle(PdfName style) {
            return SetBorderStyle(BorderStyleUtil.SetStyle(GetBorderStyle(), style));
        }

        /// <summary>Setter for the annotation's preset dashed border style.</summary>
        /// <remarks>
        /// Setter for the annotation's preset dashed border style. This property has affect only if
        /// <see cref="PdfAnnotation.STYLE_DASHED"/>
        /// style was used for the annotation border style (see
        /// <see cref="SetBorderStyle(iText.Kernel.Pdf.PdfName)"/>.
        /// See ISO-320001 8.4.3.6, "Line Dash Pattern" for the format in which dash pattern shall be specified.
        /// </remarks>
        /// <param name="dashPattern">
        /// a dash array defining a pattern of dashes and gaps that
        /// shall be used in drawing a dashed border.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetDashPattern(PdfArray dashPattern) {
            return SetBorderStyle(BorderStyleUtil.SetDashPattern(GetBorderStyle(), dashPattern));
        }

        /// <summary>An array of two names specifying the line ending styles that is used in drawing the line.</summary>
        /// <remarks>
        /// An array of two names specifying the line ending styles that is used in drawing the line.
        /// The first and second elements of the array shall specify the line ending styles for the endpoints defined,
        /// respectively, by the first and second pairs of coordinates, (x1, y1) and (x2, y2), in the
        /// <see cref="iText.Kernel.Pdf.PdfName.L"/>
        /// array
        /// (see
        /// <see cref="GetLine()"/>
        /// . For possible values see
        /// <see cref="SetLineEndingStyles(iText.Kernel.Pdf.PdfArray)"/>.
        /// </remarks>
        /// <returns>
        /// An array of two names specifying the line ending styles that is used in drawing the line; or null if line
        /// endings style is not explicitly defined, default value is [/None /None].
        /// </returns>
        public virtual PdfArray GetLineEndingStyles() {
            return GetPdfObject().GetAsArray(PdfName.LE);
        }

        /// <summary>Sets the line ending styles that are used in drawing the line.</summary>
        /// <remarks>
        /// Sets the line ending styles that are used in drawing the line.
        /// The first and second elements of the array shall specify the line ending styles for the endpoints defined,
        /// respectively, by the first and second pairs of coordinates, (x1, y1) and (x2, y2), in the
        /// <see cref="iText.Kernel.Pdf.PdfName.L"/>
        /// array
        /// (see
        /// <see cref="GetLine()"/>
        /// . Possible values for styles are:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.Square"/>
        /// - A square filled with the annotation's interior color, if any;
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.Circle"/>
        /// - A circle filled with the annotation's interior color, if any;
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.Diamond"/>
        /// - A diamond shape filled with the annotation's interior color, if any;
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.OpenArrow"/>
        /// - Two short lines meeting in an acute angle to form an open arrowhead;
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.ClosedArrow"/>
        /// - Two short lines meeting in an acute angle as in the
        /// <see cref="iText.Kernel.Pdf.PdfName.OpenArrow"/>
        /// style and
        /// connected by a third line to form a triangular closed arrowhead filled with the annotation's interior color, if any;
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.None"/>
        /// - No line ending;
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.Butt"/>
        /// - A short line at the endpoint perpendicular to the line itself;
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.ROpenArrow"/>
        /// - Two short lines in the reverse direction from
        /// <see cref="iText.Kernel.Pdf.PdfName.OpenArrow"/>
        /// ;
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.RClosedArrow"/>
        /// - A triangular closed arrowhead in the reverse direction from
        /// <see cref="iText.Kernel.Pdf.PdfName.ClosedArrow"/>
        /// ;
        /// </description></item>
        /// <item><description>
        /// <see cref="iText.Kernel.Pdf.PdfName.Slash"/>
        /// - A short line at the endpoint approximately 30 degrees clockwise from perpendicular to the line itself;
        /// </description></item>
        /// </list>
        /// see also ISO-320001, Table 176 "Line ending styles".
        /// </remarks>
        /// <param name="lineEndingStyles">An array of two names specifying the line ending styles that is used in drawing the line.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetLineEndingStyles(PdfArray lineEndingStyles) {
            return (iText.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.LE, lineEndingStyles);
        }

        /// <summary>The interior color which is used to fill the annotation's line endings.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// of either
        /// <see cref="iText.Kernel.Colors.DeviceGray"/>
        /// ,
        /// <see cref="iText.Kernel.Colors.DeviceRgb"/>
        /// or
        /// <see cref="iText.Kernel.Colors.DeviceCmyk"/>
        /// type which defines
        /// interior color of the annotation, or null if interior color is not specified.
        /// </returns>
        public virtual Color GetInteriorColor() {
            return InteriorColorUtil.ParseInteriorColor(GetPdfObject().GetAsArray(PdfName.IC));
        }

        /// <summary>
        /// An array of numbers in the range 0.0 to 1.0 specifying the interior color
        /// which is used to fill the annotation's line endings.
        /// </summary>
        /// <param name="interiorColor">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of numbers in the range 0.0 to 1.0. The number of array elements determines
        /// the colour space in which the colour is defined: 0 - No colour, transparent; 1 - DeviceGray,
        /// 3 - DeviceRGB, 4 - DeviceCMYK. For the
        /// <see cref="PdfRedactAnnotation"/>
        /// number of elements shall be
        /// equal to 3 (which defines DeviceRGB colour space).
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetInteriorColor(PdfArray interiorColor) {
            return (iText.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.IC, interiorColor);
        }

        /// <summary>
        /// An array of numbers in the range 0.0 to 1.0 specifying the interior color
        /// which is used to fill the annotation's line endings.
        /// </summary>
        /// <param name="interiorColor">an array of floats in the range 0.0 to 1.0.</param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetInteriorColor(float[] interiorColor) {
            return SetInteriorColor(new PdfArray(interiorColor));
        }

        /// <summary>
        /// The length of leader lines in default user space that extend from each endpoint of the line perpendicular
        /// to the line itself.
        /// </summary>
        /// <remarks>
        /// The length of leader lines in default user space that extend from each endpoint of the line perpendicular
        /// to the line itself. A positive value means that the leader lines appear in the direction that is clockwise
        /// when traversing the line from its starting point to its ending point (as specified by
        /// <see cref="iText.Kernel.Pdf.PdfName.L"/>
        /// (see
        /// <see cref="GetLine()"/>
        /// );
        /// a negative value indicates the opposite direction.
        /// </remarks>
        /// <returns>a float specifying the length of leader lines in default user space.</returns>
        public virtual float GetLeaderLineLength() {
            PdfNumber n = GetPdfObject().GetAsNumber(PdfName.LL);
            return n == null ? 0 : n.FloatValue();
        }

        /// <summary>
        /// Sets the length of leader lines in default user space that extend from each endpoint of the line perpendicular
        /// to the line itself.
        /// </summary>
        /// <remarks>
        /// Sets the length of leader lines in default user space that extend from each endpoint of the line perpendicular
        /// to the line itself. A positive value means that the leader lines appear in the direction that is clockwise
        /// when traversing the line from its starting point to its ending point (as specified by
        /// <see cref="iText.Kernel.Pdf.PdfName.L"/>
        /// (see
        /// <see cref="GetLine()"/>
        /// );
        /// a negative value indicates the opposite direction.
        /// </remarks>
        /// <param name="leaderLineLength">a float specifying the length of leader lines in default user space.</param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetLeaderLineLength(float leaderLineLength) {
            return (iText.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.LL, new PdfNumber(leaderLineLength));
        }

        /// <summary>
        /// A non-negative number that represents the length of leader line extensions that extend from the line proper
        /// 180 degrees from the leader lines.
        /// </summary>
        /// <returns>
        /// a non-negative float that represents the length of leader line extensions; or if the leader line extension
        /// is not explicitly set, returns the default value, which is 0.
        /// </returns>
        public virtual float GetLeaderLineExtension() {
            PdfNumber n = GetPdfObject().GetAsNumber(PdfName.LLE);
            return n == null ? 0 : n.FloatValue();
        }

        /// <summary>Sets the length of leader line extensions that extend from the line proper 180 degrees from the leader lines.
        ///     </summary>
        /// <remarks>
        /// Sets the length of leader line extensions that extend from the line proper 180 degrees from the leader lines.
        /// <b>This value shall not be set unless
        /// <see cref="iText.Kernel.Pdf.PdfName.LL"/>
        /// is set.</b>
        /// </remarks>
        /// <param name="leaderLineExtension">a non-negative float that represents the length of leader line extensions.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetLeaderLineExtension(float leaderLineExtension) {
            return (iText.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.LLE, new PdfNumber(leaderLineExtension));
        }

        /// <summary>
        /// A non-negative number that represents the length of the leader line offset, which is the amount of empty space
        /// between the endpoints of the annotation and the beginning of the leader lines.
        /// </summary>
        /// <returns>
        /// a non-negative number that represents the length of the leader line offset,
        /// or null if leader line offset is not set.
        /// </returns>
        public virtual float GetLeaderLineOffset() {
            PdfNumber n = GetPdfObject().GetAsNumber(PdfName.LLO);
            return n == null ? 0 : n.FloatValue();
        }

        /// <summary>
        /// Sets the length of the leader line offset, which is the amount of empty space between the endpoints of the
        /// annotation and the beginning of the leader lines.
        /// </summary>
        /// <param name="leaderLineOffset">a non-negative number that represents the length of the leader line offset.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetLeaderLineOffset(float leaderLineOffset) {
            return (iText.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.LLO, new PdfNumber(leaderLineOffset));
        }

        /// <summary>
        /// If true, the text specified by the
        /// <see cref="iText.Kernel.Pdf.PdfName.Contents"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfName.RC"/>
        /// entries
        /// (see
        /// <see cref="PdfAnnotation.GetContents()"/>
        /// and
        /// <see cref="PdfMarkupAnnotation.GetRichText()"/>
        /// )
        /// is replicated as a caption in the appearance of the line.
        /// </summary>
        /// <returns>
        /// true, if the annotation text is replicated as a caption, false otherwise. If this property is
        /// not set, default value is used which is <i>false</i>.
        /// </returns>
        public virtual bool GetContentsAsCaption() {
            PdfBoolean b = GetPdfObject().GetAsBoolean(PdfName.Cap);
            return b != null && b.GetValue();
        }

        /// <summary>
        /// If set to true, the text specified by the
        /// <see cref="iText.Kernel.Pdf.PdfName.Contents"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfName.RC"/>
        /// entries
        /// (see
        /// <see cref="PdfAnnotation.GetContents()"/>
        /// and
        /// <see cref="PdfMarkupAnnotation.GetRichText()"/>
        /// )
        /// will be replicated as a caption in the appearance of the line.
        /// </summary>
        /// <param name="contentsAsCaption">true, if the annotation text should be replicated as a caption, false otherwise.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetContentsAsCaption(bool contentsAsCaption) {
            return (iText.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.Cap, PdfBoolean.ValueOf(contentsAsCaption));
        }

        /// <summary>A name describing the annotation's caption positioning.</summary>
        /// <remarks>
        /// A name describing the annotation's caption positioning. Valid values are
        /// <see cref="iText.Kernel.Pdf.PdfName.Inline"/>
        /// , meaning the caption
        /// is centered inside the line, and
        /// <see cref="iText.Kernel.Pdf.PdfName.Top"/>
        /// , meaning the caption is on top of the line.
        /// </remarks>
        /// <returns>
        /// a name describing the annotation's caption positioning, or null if the caption positioning is not
        /// explicitly defined (in this case the default value is used, which is
        /// <see cref="iText.Kernel.Pdf.PdfName.Inline"/>
        /// ).
        /// </returns>
        public virtual PdfName GetCaptionPosition() {
            return GetPdfObject().GetAsName(PdfName.CP);
        }

        /// <summary>Sets annotation's caption positioning.</summary>
        /// <remarks>
        /// Sets annotation's caption positioning. Valid values are
        /// <see cref="iText.Kernel.Pdf.PdfName.Inline"/>
        /// , meaning the caption
        /// is centered inside the line, and
        /// <see cref="iText.Kernel.Pdf.PdfName.Top"/>
        /// , meaning the caption is on top of the line.
        /// </remarks>
        /// <param name="captionPosition">a name describing the annotation's caption positioning.</param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetCaptionPosition(PdfName captionPosition) {
            return (iText.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.CP, captionPosition);
        }

        /// <summary>A measure dictionary (see ISO-320001, Table 261) that specifies the scale and units that apply to the line annotation.
        ///     </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represents a measure dictionary.
        /// </returns>
        public virtual PdfDictionary GetMeasure() {
            return GetPdfObject().GetAsDictionary(PdfName.Measure);
        }

        /// <summary>Sets a measure dictionary that specifies the scale and units that apply to the line annotation.</summary>
        /// <param name="measure">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represents a measure dictionary, see ISO-320001, Table 261 for valid
        /// contents specification.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetMeasure(PdfDictionary measure) {
            return (iText.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.Measure, measure);
        }

        /// <summary>An array of two numbers that specifies the offset of the caption text from its normal position.</summary>
        /// <remarks>
        /// An array of two numbers that specifies the offset of the caption text from its normal position.
        /// The first value is the horizontal offset along the annotation line from its midpoint, with a positive value
        /// indicating offset to the right and a negative value indicating offset to the left. The second value is the vertical
        /// offset perpendicular to the annotation line, with a positive value indicating a shift up and a negative value indicating
        /// a shift down.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of two numbers that specifies the offset of the caption text from its normal position,
        /// or null if caption offset is not explicitly specified (in this case a default value is used, which is [0, 0]).
        /// </returns>
        public virtual PdfArray GetCaptionOffset() {
            return GetPdfObject().GetAsArray(PdfName.CO);
        }

        /// <summary>Sets the offset of the caption text from its normal position.</summary>
        /// <param name="captionOffset">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of two numbers that specifies the offset of the caption text from its
        /// normal position. The first value defines the horizontal offset along the annotation line from
        /// its midpoint, with a positive value indicating offset to the right and a negative value indicating
        /// offset to the left. The second value defines the vertical offset perpendicular to the annotation line,
        /// with a positive value indicating a shift up and a negative value indicating a shift down.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetCaptionOffset(PdfArray captionOffset) {
            return (iText.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.CO, captionOffset);
        }

        /// <summary>Sets the offset of the caption text from its normal position.</summary>
        /// <param name="captionOffset">
        /// an array of two floats that specifies the offset of the caption text from its
        /// normal position. The first value defines the horizontal offset along the annotation line from
        /// its midpoint, with a positive value indicating offset to the right and a negative value indicating
        /// offset to the left. The second value defines the vertical offset perpendicular to the annotation line,
        /// with a positive value indicating a shift up and a negative value indicating a shift down.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfLineAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfLineAnnotation SetCaptionOffset(float[] captionOffset) {
            return SetCaptionOffset(new PdfArray(captionOffset));
        }
    }
}
