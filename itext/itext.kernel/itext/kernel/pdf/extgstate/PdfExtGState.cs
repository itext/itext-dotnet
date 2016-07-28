/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Extgstate {
    /// <summary>Graphics state parameter dictionary wrapper</summary>
    public class PdfExtGState : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_NORMAL = PdfName.Normal;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_MULTIPLY = PdfName.Multiply;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_SCREEN = PdfName.Screen;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_OVERLAY = PdfName.Overlay;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_DARKEN = PdfName.Darken;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_LIGHTEN = PdfName.Lighten;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_COLOR_DODGE = PdfName.ColorDodge;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_COLOR_BURN = PdfName.ColorBurn;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_HARD_LIGHT = PdfName.HardLight;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_SOFT_LIGHT = PdfName.SoftLight;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_DIFFERENCE = PdfName.Difference;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static PdfName BM_EXCLUSION = PdfName.Exclusion;

        /// <summary>Standard nonseparable blend mode.</summary>
        /// <remarks>Standard nonseparable blend mode. See ISO-320001, table 137</remarks>
        public static PdfName BM_HUE = PdfName.Hue;

        /// <summary>Standard nonseparable blend mode.</summary>
        /// <remarks>Standard nonseparable blend mode. See ISO-320001, table 137</remarks>
        public static PdfName BM_SATURATION = PdfName.Saturation;

        /// <summary>Standard nonseparable blend mode.</summary>
        /// <remarks>Standard nonseparable blend mode. See ISO-320001, table 137</remarks>
        public static PdfName BM_COLOR = PdfName.Color;

        /// <summary>Standard nonseparable blend mode.</summary>
        /// <remarks>Standard nonseparable blend mode. See ISO-320001, table 137</remarks>
        public static PdfName BM_LUMINOSITY = PdfName.Luminosity;

        /// <summary>
        /// Create instance of graphics state parameter dictionary wrapper
        /// by existed
        /// <seealso>PdfDictionary</seealso>
        /// object
        /// </summary>
        /// <param name="pdfObject">instance of graphics state parameter dictionary</param>
        public PdfExtGState(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Create default instance of graphics state parameter dictionary</summary>
        public PdfExtGState()
            : this(new PdfDictionary()) {
        }

        /// <summary>
        /// Gets line width value,
        /// <c>LW</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>float</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual float? GetLineWidth() {
            return GetPdfObject().GetAsFloat(PdfName.LW);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetLineWidth(float lineWidth) {
            return Put(PdfName.LW, new PdfNumber(lineWidth));
        }

        /// <summary>
        /// Gets line gap style value,
        /// <c>LC</c>
        /// key.
        /// </summary>
        /// <returns>0 - butt cap, 1 - round cap, 2 - projecting square cap.</returns>
        public virtual int? GetLineCapStyle() {
            return GetPdfObject().GetAsInt(PdfName.LC);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetLineCapStyle(int lineCapStyle) {
            return Put(PdfName.LC, new PdfNumber(lineCapStyle));
        }

        /// <summary>
        /// Gets line join style value,
        /// <c>LJ</c>
        /// key.
        /// </summary>
        /// <returns>0 - miter join (see also miter limit), 1 - round join, 2 - bevel join.</returns>
        public virtual int? GetLineJoinStyle() {
            return GetPdfObject().GetAsInt(PdfName.LJ);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetLineJoinStyle(int lineJoinStyle) {
            return Put(PdfName.LJ, new PdfNumber(lineJoinStyle));
        }

        /// <summary>
        /// Gets miter limit value,
        /// <c>ML key</c>
        /// . See also line join style.
        /// </summary>
        /// <returns>
        /// a
        /// <c>float</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual float? GetMiterLimit() {
            return GetPdfObject().GetAsFloat(PdfName.ML);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetMiterLimit(float miterLimit) {
            return Put(PdfName.ML, new PdfNumber(miterLimit));
        }

        /// <summary>
        /// Gets line dash pattern value,
        /// <c>D</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>PdfArray</c>
        /// , that represents line dash pattern.
        /// </returns>
        public virtual PdfArray GetDashPattern() {
            return GetPdfObject().GetAsArray(PdfName.D);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetDashPattern(PdfArray dashPattern) {
            return Put(PdfName.D, dashPattern);
        }

        /// <summary>
        /// Gets rendering intent value,
        /// <c>RI</c>
        /// key.
        /// Valid values are:
        /// <c>AbsoluteColorimetric</c>
        /// ,
        /// <c>RelativeColorimetric</c>
        /// ,
        /// <c>Saturation</c>
        /// ,
        /// <c>Perceptual</c>
        /// .
        /// </summary>
        /// <returns>
        /// a
        /// <c>PdfName</c>
        /// instance.
        /// </returns>
        public virtual PdfName GetRenderingIntent() {
            return GetPdfObject().GetAsName(PdfName.RI);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetRenderingIntent(PdfName renderingIntent) {
            return Put(PdfName.RI, renderingIntent);
        }

        /// <summary>
        /// Get overprint flag value for stroke operations,
        /// <c>OP</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>boolean</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual bool? GetStrokeOverprintFlag() {
            return GetPdfObject().GetAsBool(PdfName.OP);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetStrokeOverPrintFlag(bool strokeOverPrintFlag) {
            return Put(PdfName.OP, new PdfBoolean(strokeOverPrintFlag));
        }

        /// <summary>
        /// Gets overprint flag value,
        /// <c>op</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>boolean</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual bool? GetFillOverprintFlag() {
            return GetPdfObject().GetAsBool(PdfName.op);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetFillOverPrintFlag(bool fillOverprintFlag) {
            return Put(PdfName.op, new PdfBoolean(fillOverprintFlag));
        }

        /// <summary>
        /// Get overprint control mode,
        /// <c>OPM</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>int</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual int? GetOverprintMode() {
            return GetPdfObject().GetAsInt(PdfName.OPM);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetOverprintMode(int overprintMode) {
            return Put(PdfName.OPM, new PdfNumber(overprintMode));
        }

        /// <summary>
        /// Gets font,
        /// <c>Font</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <seealso>PdfFont</seealso>
        /// instance.
        /// </returns>
        public virtual PdfArray GetFont() {
            return GetPdfObject().GetAsArray(PdfName.Font);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetFont(PdfArray font) {
            return Put(PdfName.Font, font);
        }

        /// <summary>
        /// Gets the black-generation function value,
        /// <c>BG</c>
        /// .
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , represents
        /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
        /// .
        /// </returns>
        public virtual PdfObject GetBlackGenerationFunction() {
            return GetPdfObject().Get(PdfName.BG);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetBlackGenerationFunction(PdfObject blackGenerationFunction
            ) {
            return Put(PdfName.BG, blackGenerationFunction);
        }

        /// <summary>
        /// Gets the black-generation function value or
        /// <c>Default</c>
        /// ,
        /// <c>BG2</c>
        /// key.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// value, represents either
        /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// .
        /// </returns>
        public virtual PdfObject GetBlackGenerationFunction2() {
            return GetPdfObject().Get(PdfName.BG2);
        }

        /// <summary>
        /// Note, if both
        /// <c>BG</c>
        /// and
        /// <c>BG2</c>
        /// are present in the same graphics state parameter dictionary,
        /// <c>BG2</c>
        /// takes precedence.
        /// </summary>
        /// <param name="blackGenerationFunction2"/>
        /// <returns/>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetBlackGenerationFunction2(PdfObject blackGenerationFunction2
            ) {
            return Put(PdfName.BG2, blackGenerationFunction2);
        }

        /// <summary>
        /// Gets the undercolor-removal function,
        /// <c>UCR</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , represents
        /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
        /// .
        /// </returns>
        public virtual PdfObject GetUndercolorRemovalFunction() {
            return GetPdfObject().Get(PdfName.UCR);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetUndercolorRemovalFunction(PdfObject undercolorRemovalFunction
            ) {
            return Put(PdfName.UCR, undercolorRemovalFunction);
        }

        /// <summary>
        /// Gets the undercolor-removal function value or
        /// <c>Default</c>
        /// ,
        /// <c>UCR2</c>
        /// key.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// value, represents either
        /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// .
        /// </returns>
        public virtual PdfObject GetUndercolorRemovalFunction2() {
            return GetPdfObject().Get(PdfName.UCR2);
        }

        /// <summary>
        /// Note, if both
        /// <c>UCR</c>
        /// and
        /// <c>UCR2</c>
        /// are present in the same graphics state parameter dictionary,
        /// <c>UCR2</c>
        /// takes precedence.
        /// </summary>
        /// <param name="undercolorRemovalFunction2"/>
        /// <returns/>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetUndercolorRemovalFunction2(PdfObject undercolorRemovalFunction2
            ) {
            return Put(PdfName.UCR2, undercolorRemovalFunction2);
        }

        /// <summary>
        /// Gets the transfer function value,
        /// <c>TR</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , represents either
        /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// .
        /// </returns>
        public virtual PdfObject GetTransferFunction() {
            return GetPdfObject().Get(PdfName.TR);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetTransferFunction(PdfObject transferFunction) {
            return Put(PdfName.TR, transferFunction);
        }

        /// <summary>
        /// Gets the transfer function value or
        /// <c>Default</c>
        /// ,
        /// <c>TR2</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , represents either
        /// <see cref="iText.Kernel.Pdf.Function.PdfFunction"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// .
        /// </returns>
        public virtual PdfObject GetTransferFunction2() {
            return GetPdfObject().Get(PdfName.TR2);
        }

        /// <summary>
        /// Note, if both
        /// <c>TR</c>
        /// and
        /// <c>TR2</c>
        /// are present in the same graphics state parameter dictionary,
        /// <c>TR2</c>
        /// takes precedence.
        /// </summary>
        /// <param name="transferFunction"/>
        /// <returns/>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetTransferFunction2(PdfObject transferFunction) {
            return Put(PdfName.TR2, transferFunction);
        }

        /// <summary>
        /// Gets the halftone dictionary or stream or
        /// <c>Default</c>
        /// ,
        /// <c>HT</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , represents either
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// .
        /// </returns>
        public virtual PdfObject GetHalftone() {
            return GetPdfObject().Get(PdfName.HT);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetHalftone(PdfObject halftone) {
            return Put(PdfName.HT, halftone);
        }

        /// <summary>
        /// Gets
        /// <c>HTP</c>
        /// key.
        /// </summary>
        [Obsolete]
        public virtual PdfObject GetHTP() {
            return GetPdfObject().Get(PdfName.HTP);
        }

        [Obsolete]
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetHTP(PdfObject htp) {
            return Put(PdfName.HTP, htp);
        }

        /// <summary>
        /// Gets the flatness tolerance value,
        /// <c>FL</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>float</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual float? GetFlatnessTolerance() {
            return GetPdfObject().GetAsFloat(PdfName.FL);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetFlatnessTolerance(float flatnessTolerance) {
            return Put(PdfName.FL, new PdfNumber(flatnessTolerance));
        }

        /// <summary>
        /// Gets the smoothness tolerance value,
        /// <c>SM</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>float</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual float? GetSmothnessTolerance() {
            return GetPdfObject().GetAsFloat(PdfName.SM);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetSmoothnessTolerance(float smoothnessTolerance) {
            return Put(PdfName.SM, new PdfNumber(smoothnessTolerance));
        }

        /// <summary>
        /// Gets value of an automatic stroke adjustment flag,
        /// <c>SA</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>boolean</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual bool? GetAutomaticStrokeAdjustmentFlag() {
            return GetPdfObject().GetAsBool(PdfName.SA);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetAutomaticStrokeAdjustmentFlag(bool strokeAdjustment
            ) {
            return Put(PdfName.SA, new PdfBoolean(strokeAdjustment));
        }

        /// <summary>
        /// Gets the current blend mode for the transparent imaging model,
        /// <c>BM</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , represents either
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// .
        /// </returns>
        public virtual PdfObject GetBlendMode() {
            return GetPdfObject().Get(PdfName.BM);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetBlendMode(PdfObject blendMode) {
            return Put(PdfName.BM, blendMode);
        }

        /// <summary>
        /// Gets the current soft mask,
        /// <c>SMask</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , represents either
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// .
        /// </returns>
        public virtual PdfObject GetSoftMask() {
            return GetPdfObject().Get(PdfName.SMask);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetSoftMask(PdfObject sMask) {
            return Put(PdfName.SMask, sMask);
        }

        /// <summary>
        /// Gets the current alpha constant, specifying the constant shape or constant opacity value
        /// for <b>stroking</b> operations in the transparent imaging model,
        /// <c>CA</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>float</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual float? GetStrokeOpacity() {
            return GetPdfObject().GetAsFloat(PdfName.CA);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetStrokeOpacity(float strokingAlphaConstant) {
            return Put(PdfName.CA, new PdfNumber(strokingAlphaConstant));
        }

        /// <summary>
        /// Gets the current alpha constant, specifying the constant shape or constant opacity value
        /// for <b>nonstroking</b> operations in the transparent imaging model,
        /// <c>ca</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>float</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual float? GetFillOpacity() {
            return GetPdfObject().GetAsFloat(PdfName.ca);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetFillOpacity(float fillingAlphaConstant) {
            return Put(PdfName.ca, new PdfNumber(fillingAlphaConstant));
        }

        /// <summary>
        /// Gets the alpha source flag (â€œalpha is shapeâ€?), specifying whether the current soft mask and alpha constant
        /// shall be interpreted as shape values (
        /// <see langword="true"/>
        /// ) or opacity values (
        /// <see langword="false"/>
        /// ),
        /// <c>AIS</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>boolean</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual bool? GetAlphaSourceFlag() {
            return GetPdfObject().GetAsBool(PdfName.AIS);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetAlphaSourceFlag(bool alphaSourceFlag) {
            return Put(PdfName.AIS, new PdfBoolean(alphaSourceFlag));
        }

        /// <summary>
        /// Gets the text knockout flag, which determine the behaviour of overlapping glyphs
        /// within a text object in the transparent imaging model,
        /// <c>TK</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>boolean</c>
        /// value if exist, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual bool? GetTextKnockoutFlag() {
            return GetPdfObject().GetAsBool(PdfName.TK);
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetTextKnockoutFlag(bool textKnockoutFlag) {
            return Put(PdfName.TK, new PdfBoolean(textKnockoutFlag));
        }

        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </summary>
        public override void Flush() {
            base.Flush();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
