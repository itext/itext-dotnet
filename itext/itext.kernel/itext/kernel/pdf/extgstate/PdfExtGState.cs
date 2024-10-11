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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Extgstate {
    /// <summary>Graphics state parameter dictionary wrapper.</summary>
    /// <remarks>
    /// Graphics state parameter dictionary wrapper.
    /// See ISO-320001, 8.4.5 Graphics State Parameter Dictionaries.
    /// </remarks>
    public class PdfExtGState : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_NORMAL = PdfName.Normal;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_MULTIPLY = PdfName.Multiply;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_SCREEN = PdfName.Screen;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_OVERLAY = PdfName.Overlay;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_DARKEN = PdfName.Darken;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_LIGHTEN = PdfName.Lighten;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_COLOR_DODGE = PdfName.ColorDodge;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_COLOR_BURN = PdfName.ColorBurn;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_HARD_LIGHT = PdfName.HardLight;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_SOFT_LIGHT = PdfName.SoftLight;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_DIFFERENCE = PdfName.Difference;

        /// <summary>Standard separable blend mode.</summary>
        /// <remarks>Standard separable blend mode. See ISO-320001, table 136</remarks>
        public static readonly PdfName BM_EXCLUSION = PdfName.Exclusion;

        /// <summary>Standard nonseparable blend mode.</summary>
        /// <remarks>Standard nonseparable blend mode. See ISO-320001, table 137</remarks>
        public static readonly PdfName BM_HUE = PdfName.Hue;

        /// <summary>Standard nonseparable blend mode.</summary>
        /// <remarks>Standard nonseparable blend mode. See ISO-320001, table 137</remarks>
        public static readonly PdfName BM_SATURATION = PdfName.Saturation;

        /// <summary>Standard nonseparable blend mode.</summary>
        /// <remarks>Standard nonseparable blend mode. See ISO-320001, table 137</remarks>
        public static readonly PdfName BM_COLOR = PdfName.Color;

        /// <summary>Standard nonseparable blend mode.</summary>
        /// <remarks>Standard nonseparable blend mode. See ISO-320001, table 137</remarks>
        public static readonly PdfName BM_LUMINOSITY = PdfName.Luminosity;

        /// <summary>
        /// Create instance of graphics state parameter dictionary wrapper
        /// by existed
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
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
        /// <see langword="null"/>.
        /// </returns>
        public virtual float? GetLineWidth() {
            return GetPdfObject().GetAsFloat(PdfName.LW);
        }

        /// <summary>
        /// Sets line width value,
        /// <c>LW</c>
        /// key.
        /// </summary>
        /// <param name="lineWidth">
        /// a
        /// <c>float</c>
        /// value.
        /// </param>
        /// <returns>object itself.</returns>
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

        /// <summary>
        /// Sets line gap style value,
        /// <c>LC</c>
        /// key.
        /// </summary>
        /// <param name="lineCapStyle">0 - butt cap, 1 - round cap, 2 - projecting square cap.</param>
        /// <returns>object itself.</returns>
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

        /// <summary>
        /// Sets line join style value,
        /// <c>LJ</c>
        /// key.
        /// </summary>
        /// <param name="lineJoinStyle">0 - miter join (see also miter limit), 1 - round join, 2 - bevel join.</param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetLineJoinStyle(int lineJoinStyle) {
            return Put(PdfName.LJ, new PdfNumber(lineJoinStyle));
        }

        /// <summary>
        /// Gets miter limit value,
        /// <c>ML key</c>.
        /// </summary>
        /// <remarks>
        /// Gets miter limit value,
        /// <c>ML key</c>
        /// . See also line join style.
        /// </remarks>
        /// <returns>
        /// a
        /// <c>float</c>
        /// value if exist, otherwise
        /// <see langword="null"/>.
        /// </returns>
        public virtual float? GetMiterLimit() {
            return GetPdfObject().GetAsFloat(PdfName.ML);
        }

        /// <summary>
        /// Sets miter limit value,
        /// <c>ML key</c>.
        /// </summary>
        /// <remarks>
        /// Sets miter limit value,
        /// <c>ML key</c>
        /// . See also line join style.
        /// </remarks>
        /// <param name="miterLimit">
        /// a
        /// <c>float</c>
        /// value.
        /// </param>
        /// <returns>object itself.</returns>
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

        /// <summary>
        /// Sets line dash pattern value,
        /// <c>D</c>
        /// key.
        /// </summary>
        /// <param name="dashPattern">
        /// a
        /// <c>PdfArray</c>
        /// , that represents line dash pattern.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetDashPattern(PdfArray dashPattern) {
            return Put(PdfName.D, dashPattern);
        }

        /// <summary>
        /// Gets rendering intent value,
        /// <c>RI</c>
        /// key.
        /// </summary>
        /// <remarks>
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
        /// <c>Perceptual</c>.
        /// </remarks>
        /// <returns>
        /// a
        /// <c>PdfName</c>
        /// instance.
        /// </returns>
        public virtual PdfName GetRenderingIntent() {
            return GetPdfObject().GetAsName(PdfName.RI);
        }

        /// <summary>
        /// Sets rendering intent value,
        /// <c>RI</c>
        /// key.
        /// </summary>
        /// <param name="renderingIntent">
        /// a
        /// <c>PdfName</c>
        /// instance, Valid values are:
        /// <c>AbsoluteColorimetric</c>
        /// ,
        /// <c>RelativeColorimetric</c>
        /// ,
        /// <c>Saturation</c>
        /// ,
        /// <c>Perceptual</c>.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetRenderingIntent(PdfName renderingIntent) {
            return Put(PdfName.RI, renderingIntent);
        }

        /// <summary>
        /// Get overprint flag value for <b>stroking</b> operations,
        /// <c>OP</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>boolean</c>
        /// value if exist, otherwise
        /// <see langword="null"/>.
        /// </returns>
        public virtual bool? GetStrokeOverprintFlag() {
            return GetPdfObject().GetAsBool(PdfName.OP);
        }

        /// <summary>
        /// Set overprint flag value for <b>stroking</b> operations,
        /// <c>OP</c>
        /// key.
        /// </summary>
        /// <param name="strokeOverPrintFlag">
        /// 
        /// <see langword="true"/>
        /// , for applying overprint for <b>stroking</b> operations.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetStrokeOverPrintFlag(bool strokeOverPrintFlag) {
            return Put(PdfName.OP, PdfBoolean.ValueOf(strokeOverPrintFlag));
        }

        /// <summary>
        /// Get overprint flag value for <b>non-stroking</b> operations,
        /// <c>op</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>boolean</c>
        /// value if exist, otherwise
        /// <see langword="null"/>.
        /// </returns>
        public virtual bool? GetFillOverprintFlag() {
            return GetPdfObject().GetAsBool(PdfName.op);
        }

        /// <summary>
        /// Set overprint flag value for <b>non-stroking</b> operations,
        /// <c>op</c>
        /// key.
        /// </summary>
        /// <param name="fillOverprintFlag">
        /// 
        /// <see langword="true"/>
        /// , for applying overprint for <b>non-stroking</b> operations.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetFillOverPrintFlag(bool fillOverprintFlag) {
            return Put(PdfName.op, PdfBoolean.ValueOf(fillOverprintFlag));
        }

        /// <summary>
        /// Get overprint control mode,
        /// <c>OPM</c>
        /// key.
        /// </summary>
        /// <returns>
        /// an
        /// <c>int</c>
        /// value if exist, otherwise
        /// <see langword="null"/>.
        /// </returns>
        public virtual int? GetOverprintMode() {
            return GetPdfObject().GetAsInt(PdfName.OPM);
        }

        /// <summary>
        /// Set overprint control mode,
        /// <c>OPM</c>
        /// key.
        /// </summary>
        /// <param name="overprintMode">
        /// an
        /// <c>int</c>
        /// value, see ISO-320001, 8.6.7 Overprint Control.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetOverprintMode(int overprintMode) {
            return Put(PdfName.OPM, new PdfNumber(overprintMode));
        }

        /// <summary>
        /// Gets font and size,
        /// <c>Font</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of the form
        /// <c>[font size]</c>
        /// , where
        /// <c>font</c>
        /// shall be an indirect reference to a font dictionary and
        /// <c>size</c>
        /// shall be a number expressed in text space units.
        /// </returns>
        public virtual PdfArray GetFont() {
            return GetPdfObject().GetAsArray(PdfName.Font);
        }

        /// <summary>
        /// Sets font and size,
        /// <c>Font</c>
        /// key.
        /// </summary>
        /// <remarks>
        /// Sets font and size,
        /// <c>Font</c>
        /// key.
        /// NOTE: If you want add the font object which has just been created, make sure to register the font with
        /// <see cref="iText.Kernel.Pdf.PdfDocument.AddFont(iText.Kernel.Font.PdfFont)"/>
        /// method first.
        /// </remarks>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of the form
        /// <c>[font size]</c>
        /// , where
        /// <paramref name="font"/>
        /// shall be an indirect reference to a font dictionary and
        /// <c>size</c>
        /// shall be a number expressed in text space units.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetFont(PdfArray font) {
            return Put(PdfName.Font, font);
        }

        /// <summary>
        /// Gets the black-generation function value,
        /// <c>BG</c>.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , should be an object representing custom function (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating such function objects).
        /// </returns>
        public virtual PdfObject GetBlackGenerationFunction() {
            return GetPdfObject().Get(PdfName.BG);
        }

        /// <summary>
        /// Sets the black-generation function value,
        /// <c>BG</c>.
        /// </summary>
        /// <param name="blackGenerationFunction">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , shall be an object representing custom function
        /// (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating such
        /// function objects).
        /// </param>
        /// <returns>object itself.</returns>
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
        /// the returned value is
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// value, which is either a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// of
        /// a predefined value or an object representing custom function (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating such function objects).
        /// </returns>
        public virtual PdfObject GetBlackGenerationFunction2() {
            return GetPdfObject().Get(PdfName.BG2);
        }

        /// <summary>
        /// Sets the black-generation function value or
        /// <c>Default</c>
        /// ,
        /// <c>BG2</c>
        /// key.
        /// </summary>
        /// <remarks>
        /// Sets the black-generation function value or
        /// <c>Default</c>
        /// ,
        /// <c>BG2</c>
        /// key.
        /// Note, if both
        /// <c>BG</c>
        /// and
        /// <c>BG2</c>
        /// are present in the same graphics state parameter dictionary,
        /// <c>BG2</c>
        /// takes precedence.
        /// </remarks>
        /// <param name="blackGenerationFunction2">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// value, shall be an object representing custom function
        /// (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating such
        /// function objects) or
        /// <c>Default</c>.
        /// </param>
        /// <returns>object itself.</returns>
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
        /// , should be an object representing custom function (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating such function objects).
        /// </returns>
        public virtual PdfObject GetUndercolorRemovalFunction() {
            return GetPdfObject().Get(PdfName.UCR);
        }

        /// <summary>
        /// Sets the undercolor-removal function,
        /// <c>UCR</c>
        /// key.
        /// </summary>
        /// <param name="undercolorRemovalFunction">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , shall be an object representing custom function
        /// (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating
        /// such function objects).
        /// </param>
        /// <returns>object itself.</returns>
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
        /// the returned value is
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// value, which is either a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// of
        /// a predefined value or an object representing custom function (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating such function objects).
        /// </returns>
        public virtual PdfObject GetUndercolorRemovalFunction2() {
            return GetPdfObject().Get(PdfName.UCR2);
        }

        /// <summary>
        /// Sets the undercolor-removal function value or
        /// <c>Default</c>
        /// ,
        /// <c>UCR2</c>
        /// key.
        /// </summary>
        /// <remarks>
        /// Sets the undercolor-removal function value or
        /// <c>Default</c>
        /// ,
        /// <c>UCR2</c>
        /// key.
        /// Note, if both
        /// <c>UCR</c>
        /// and
        /// <c>UCR2</c>
        /// are present in the same graphics state parameter dictionary,
        /// <c>UCR2</c>
        /// takes precedence.
        /// </remarks>
        /// <param name="undercolorRemovalFunction2">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// value, shall be an object representing custom function
        /// (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating such
        /// function objects) or
        /// <c>Default</c>.
        /// </param>
        /// <returns>object itself.</returns>
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
        /// the returned value is
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// value, which is either a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of
        /// a predefined value or an object representing custom function (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating such function objects).
        /// </returns>
        public virtual PdfObject GetTransferFunction() {
            return GetPdfObject().Get(PdfName.TR);
        }

        /// <summary>
        /// Sets the transfer function value,
        /// <c>TR</c>
        /// key.
        /// </summary>
        /// <param name="transferFunction">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , shall be either a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of
        /// a predefined value or an object representing custom function (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating such function objects).
        /// </param>
        /// <returns>object itself.</returns>
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
        /// the returned value is
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// value, which is either a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of
        /// a predefined value or an object representing custom function (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating such function objects).
        /// </returns>
        public virtual PdfObject GetTransferFunction2() {
            return GetPdfObject().Get(PdfName.TR2);
        }

        /// <summary>
        /// Sets the transfer function value or
        /// <c>Default</c>
        /// ,
        /// <c>TR2</c>
        /// key.
        /// </summary>
        /// <remarks>
        /// Sets the transfer function value or
        /// <c>Default</c>
        /// ,
        /// <c>TR2</c>
        /// key.
        /// Note, if both
        /// <c>TR</c>
        /// and
        /// <c>TR2</c>
        /// are present in the same graphics state parameter dictionary,
        /// <c>TR2</c>
        /// takes precedence.
        /// </remarks>
        /// <param name="transferFunction2">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , shall be either a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of
        /// a predefined value or an object representing custom function (see
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// object wrapper for convenience API in reading/manipulating such function objects).
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetTransferFunction2(PdfObject transferFunction2) {
            return Put(PdfName.TR2, transferFunction2);
        }

        /// <summary>
        /// Gets the halftone dictionary, stream or
        /// <c>Default</c>
        /// ,
        /// <c>HT</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , should be either
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfName"/>.
        /// </returns>
        public virtual PdfObject GetHalftone() {
            return GetPdfObject().Get(PdfName.HT);
        }

        /// <summary>
        /// Sets the halftone or
        /// <c>Default</c>
        /// ,
        /// <c>HT</c>
        /// key.
        /// </summary>
        /// <param name="halftone">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , shall be either
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfName"/>.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetHalftone(PdfObject halftone) {
            return Put(PdfName.HT, halftone);
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
        /// <see langword="null"/>.
        /// </returns>
        public virtual float? GetFlatnessTolerance() {
            return GetPdfObject().GetAsFloat(PdfName.FL);
        }

        /// <summary>
        /// Sets the flatness tolerance value,
        /// <c>FL</c>
        /// key.
        /// </summary>
        /// <param name="flatnessTolerance">
        /// a
        /// <c>float</c>
        /// value.
        /// </param>
        /// <returns>object itself.</returns>
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
        /// <see langword="null"/>.
        /// </returns>
        public virtual float? GetSmothnessTolerance() {
            return GetPdfObject().GetAsFloat(PdfName.SM);
        }

        /// <summary>
        /// Sets the smoothness tolerance value,
        /// <c>SM</c>
        /// key.
        /// </summary>
        /// <param name="smoothnessTolerance">
        /// a
        /// <c>float</c>
        /// value.
        /// </param>
        /// <returns>object itself.</returns>
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
        /// <see langword="null"/>.
        /// </returns>
        public virtual bool? GetAutomaticStrokeAdjustmentFlag() {
            return GetPdfObject().GetAsBool(PdfName.SA);
        }

        /// <summary>
        /// Sets value of an automatic stroke adjustment flag,
        /// <c>SA</c>
        /// key.
        /// </summary>
        /// <param name="strokeAdjustment">
        /// a
        /// <c>boolean</c>
        /// value.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetAutomaticStrokeAdjustmentFlag(bool strokeAdjustment
            ) {
            return Put(PdfName.SA, PdfBoolean.ValueOf(strokeAdjustment));
        }

        /// <summary>
        /// Gets the current blend mode for the transparent imaging model,
        /// <c>BM</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , should be either
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// . array is deprecated in PDF 2.0.
        /// </returns>
        public virtual PdfObject GetBlendMode() {
            return GetPdfObject().Get(PdfName.BM);
        }

        /// <summary>
        /// Sets the current blend mode for the transparent imaging model,
        /// <c>BM</c>
        /// key.
        /// </summary>
        /// <param name="blendMode">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , shall be either
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// ; array is deprecated in PDF 2.0.
        /// </param>
        /// <returns>object itself.</returns>
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
        /// , should be either
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </returns>
        public virtual PdfObject GetSoftMask() {
            return GetPdfObject().Get(PdfName.SMask);
        }

        /// <summary>
        /// Sets the current soft mask,
        /// <c>SMask</c>
        /// key.
        /// </summary>
        /// <param name="sMask">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , shall be either
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </param>
        /// <returns>object itself.</returns>
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
        /// <see langword="null"/>.
        /// </returns>
        public virtual float? GetStrokeOpacity() {
            return GetPdfObject().GetAsFloat(PdfName.CA);
        }

        /// <summary>
        /// Sets the current alpha constant, specifying the constant shape or constant opacity value
        /// for <b>stroking</b> operations in the transparent imaging model,
        /// <c>CA</c>
        /// key.
        /// </summary>
        /// <param name="strokingAlphaConstant">
        /// a
        /// <c>float</c>
        /// value.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetStrokeOpacity(float strokingAlphaConstant) {
            return Put(PdfName.CA, new PdfNumber(strokingAlphaConstant));
        }

        /// <summary>
        /// Gets the current alpha constant, specifying the constant shape or constant opacity value
        /// for <b>non-stroking</b> operations in the transparent imaging model,
        /// <c>ca</c>
        /// key.
        /// </summary>
        /// <returns>
        /// a
        /// <c>float</c>
        /// value if exist, otherwise
        /// <see langword="null"/>.
        /// </returns>
        public virtual float? GetFillOpacity() {
            return GetPdfObject().GetAsFloat(PdfName.ca);
        }

        /// <summary>
        /// Sets the current alpha constant, specifying the constant shape or constant opacity value
        /// for <b>non-stroking</b> operations in the transparent imaging model,
        /// <c>ca</c>
        /// key.
        /// </summary>
        /// <param name="fillingAlphaConstant">
        /// a
        /// <c>float</c>
        /// value.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetFillOpacity(float fillingAlphaConstant) {
            return Put(PdfName.ca, new PdfNumber(fillingAlphaConstant));
        }

        /// <summary>
        /// Gets the alpha source flag ("alpha is shape"), specifying whether the current soft mask and alpha constant
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
        /// <see langword="null"/>.
        /// </returns>
        public virtual bool? GetAlphaSourceFlag() {
            return GetPdfObject().GetAsBool(PdfName.AIS);
        }

        /// <summary>
        /// Sets the alpha source flag ("alpha is shape"), specifying whether the current soft mask and alpha constant
        /// shall be interpreted as shape values (
        /// <see langword="true"/>
        /// ) or opacity values (
        /// <see langword="false"/>
        /// ),
        /// <c>AIS</c>
        /// key.
        /// </summary>
        /// <param name="alphaSourceFlag">
        /// if
        /// <see langword="true"/>
        /// - alpha as shape values, if
        /// <see langword="false"/>
        /// — as opacity values.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetAlphaSourceFlag(bool alphaSourceFlag) {
            return Put(PdfName.AIS, PdfBoolean.ValueOf(alphaSourceFlag));
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
        /// <see langword="null"/>.
        /// </returns>
        public virtual bool? GetTextKnockoutFlag() {
            return GetPdfObject().GetAsBool(PdfName.TK);
        }

        /// <summary>
        /// Sets the text knockout flag, which determine the behaviour of overlapping glyphs
        /// within a text object in the transparent imaging model,
        /// <c>TK</c>
        /// key.
        /// </summary>
        /// <param name="textKnockoutFlag">
        /// 
        /// <see langword="true"/>
        /// if enabled.
        /// </param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetTextKnockoutFlag(bool textKnockoutFlag) {
            return Put(PdfName.TK, PdfBoolean.ValueOf(textKnockoutFlag));
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>
        /// PDF 2.0. This graphics state parameter controls whether black point
        /// compensation is performed while doing CIE-based colour conversions.
        /// </remarks>
        /// <param name="useBlackPointCompensation"><c>true</c> to enable, <c>false</c> to disable</param>
        /// <returns>object itself</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetUseBlackPointCompensation(bool useBlackPointCompensation
            ) {
            return Put(PdfName.UseBlackPtComp, useBlackPointCompensation ? PdfName.ON : PdfName.OFF);
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>PDF 2.0. Checks whether the black point compensation is performed while doing CIE-based colour conversions.
        ///     </remarks>
        /// <returns>
        /// <c>true</c> if black point compensation is used, <c>false</c> if it is not used, or
        /// <c>null</c> is the value is set to Default, or not set at all
        /// </returns>
        public virtual bool? IsBlackPointCompensationUsed() {
            PdfName useBlackPointCompensation = GetPdfObject().GetAsName(PdfName.UseBlackPtComp);
            if (PdfName.ON.Equals(useBlackPointCompensation)) {
                return true;
            }
            else {
                if (PdfName.OFF.Equals(useBlackPointCompensation)) {
                    return false;
                }
                else {
                    return null;
                }
            }
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>PDF 2.0. Sets halftone origin</remarks>
        /// <param name="x">X location of the halftone origin in the current coordinate system</param>
        /// <param name="y">Y location of the halftone origin in the current coordinate system</param>
        /// <returns>
        /// this
        /// <see cref="PdfExtGState"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState SetHalftoneOrigin(float x, float y) {
            PdfArray hto = new PdfArray();
            hto.Add(new PdfNumber(x));
            hto.Add(new PdfNumber(y));
            return Put(PdfName.HTO, hto);
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>PDF 2.0. Gets halftone origin</remarks>
        /// <returns>
        /// an array of two values specifying X and Y values of the halftone origin in the current coordinate system,
        /// respectively, or <c>null</c> if halftone origin is not specified
        /// </returns>
        public virtual float[] GetHalftoneOrigin() {
            PdfArray hto = GetPdfObject().GetAsArray(PdfName.HTO);
            if (hto != null && hto.Size() == 2 && hto.Get(0).IsNumber() && hto.Get(1).IsNumber()) {
                return new float[] { hto.GetAsNumber(0).FloatValue(), hto.GetAsNumber(1).FloatValue() };
            }
            else {
                return null;
            }
        }

        /// <summary>Puts the value into Graphics state parameter dictionary and associates it with the specified key.
        ///     </summary>
        /// <remarks>
        /// Puts the value into Graphics state parameter dictionary and associates it with the specified key.
        /// If the key is already present, it will override the old value with the specified one.
        /// </remarks>
        /// <param name="key">key to insert or to override</param>
        /// <param name="value">the value to associate with the specified key</param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.Extgstate.PdfExtGState Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// </summary>
        /// <remarks>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>.
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </remarks>
        public override void Flush() {
            base.Flush();
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
