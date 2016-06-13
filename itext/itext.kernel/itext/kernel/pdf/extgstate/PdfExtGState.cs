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
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Kernel.Pdf.Extgstate {
    public class PdfExtGState : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Blend mode constants</summary>
        public static PdfName BM_NORMAL = PdfName.Normal;

        public static PdfName BM_MULTIPLY = PdfName.Multiply;

        public static PdfName BM_SCREEN = PdfName.Screen;

        public static PdfName BM_OVERLAY = PdfName.Overlay;

        public static PdfName BM_DARKEN = PdfName.Darken;

        public static PdfName BM_LIGHTEN = PdfName.Lighten;

        public static PdfName BM_COLOR_DODGE = PdfName.ColorDodge;

        public static PdfName BM_COLOR_BURN = PdfName.ColorBurn;

        public static PdfName BM_HARD_LIGHT = PdfName.HardLight;

        public static PdfName BM_SOFT_LIGHT = PdfName.SoftLight;

        public static PdfName BM_DIFFERENCE = PdfName.Difference;

        public static PdfName BM_EXCLUSION = PdfName.Exclusion;

        public static PdfName BM_HUE = PdfName.Hue;

        public static PdfName BM_SATURATION = PdfName.Saturation;

        public static PdfName BM_COLOR = PdfName.Color;

        public static PdfName BM_LUMINOSITY = PdfName.Luminosity;

        public PdfExtGState(PdfDictionary pdfObject)
            : base(pdfObject) {
            MarkObjectAsIndirect(GetPdfObject());
        }

        public PdfExtGState()
            : this(new PdfDictionary()) {
        }

        public virtual float? GetLineWidth() {
            return GetPdfObject().GetAsFloat(PdfName.LW);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetLineWidth(float lineWidth) {
            return Put(PdfName.LW, new PdfNumber(lineWidth));
        }

        public virtual int? GetLineCapStyle() {
            return GetPdfObject().GetAsInt(PdfName.LC);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetLineCapStryle(int lineCapStyle) {
            return Put(PdfName.LC, new PdfNumber(lineCapStyle));
        }

        public virtual int? GetLineJoinStyle() {
            return GetPdfObject().GetAsInt(PdfName.LJ);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetLineJoinStyle(int lineJoinStyle) {
            return Put(PdfName.LJ, new PdfNumber(lineJoinStyle));
        }

        public virtual float? GetMiterLimit() {
            return GetPdfObject().GetAsFloat(PdfName.ML);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetMiterLimit(float miterLimit) {
            return Put(PdfName.ML, new PdfNumber(miterLimit));
        }

        public virtual PdfArray GetDashPattern() {
            return GetPdfObject().GetAsArray(PdfName.D);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetDashPattern(PdfArray dashPattern) {
            return Put(PdfName.D, dashPattern);
        }

        public virtual PdfName GetRenderingIntent() {
            return GetPdfObject().GetAsName(PdfName.RI);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetRenderingIntent(PdfName renderingIntent) {
            return Put(PdfName.RI, renderingIntent);
        }

        public virtual int? GetOverprintMode() {
            return GetPdfObject().GetAsInt(PdfName.OPM);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetOverprintMode(int overprintMode) {
            return Put(PdfName.OPM, new PdfNumber(overprintMode));
        }

        public virtual bool? GetFillOverprintFlag() {
            return GetPdfObject().GetAsBool(PdfName.op);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetFillOverPrintFlag(bool fillOverprintFlag) {
            return Put(PdfName.op, new PdfBoolean(fillOverprintFlag));
        }

        public virtual bool? GetStrokeOverprintFlag() {
            return GetPdfObject().GetAsBool(PdfName.OP);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetStrokeOverPrintFlag(bool strokeOverPrintFlag
            ) {
            return Put(PdfName.OP, new PdfBoolean(strokeOverPrintFlag));
        }

        public virtual PdfArray GetFont() {
            return GetPdfObject().GetAsArray(PdfName.Font);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetFont(PdfArray font) {
            return Put(PdfName.Font, font);
        }

        public virtual PdfObject GetBlackGenerationFunction() {
            return GetPdfObject().Get(PdfName.BG);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetBlackGenerationFunction(PdfObject blackGenerationFunction
            ) {
            return Put(PdfName.BG, blackGenerationFunction);
        }

        public virtual PdfObject GetBlackGenerationFunction2() {
            return GetPdfObject().Get(PdfName.BG2);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetBlackGenerationFunction2(PdfObject blackGenerationFunction2
            ) {
            return Put(PdfName.BG2, blackGenerationFunction2);
        }

        public virtual PdfObject GetUndercolorRemovalFunction() {
            return GetPdfObject().Get(PdfName.UCR);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetUndercolorRemovalFunction(PdfObject undercolorRemovalFunction
            ) {
            return Put(PdfName.UCR, undercolorRemovalFunction);
        }

        public virtual PdfObject GetUndercolorRemovalFunction2() {
            return GetPdfObject().Get(PdfName.UCR2);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetUndercolorRemovalFunction2(PdfObject undercolorRemovalFunction2
            ) {
            return Put(PdfName.UCR2, undercolorRemovalFunction2);
        }

        public virtual PdfObject GetTransferFunction() {
            return GetPdfObject().Get(PdfName.TR);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetTransferFunction(PdfObject transferFunction
            ) {
            return Put(PdfName.TR, transferFunction);
        }

        public virtual PdfObject GetTransferFunction2() {
            return GetPdfObject().Get(PdfName.TR2);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetTransferFunction2(PdfObject transferFunction
            ) {
            return Put(PdfName.TR2, transferFunction);
        }

        public virtual PdfObject GetHalftone() {
            return GetPdfObject().Get(PdfName.HT);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetHalftone(PdfObject halftone) {
            return Put(PdfName.HT, halftone);
        }

        public virtual PdfObject GetHTP() {
            return GetPdfObject().Get(PdfName.HTP);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetHTP(PdfObject htp) {
            return Put(PdfName.HTP, htp);
        }

        public virtual float? GetFlatnessTolerance() {
            return GetPdfObject().GetAsFloat(PdfName.FT);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetFlatnessTolerance(float flatnessTolerance) {
            return Put(PdfName.FT, new PdfNumber(flatnessTolerance));
        }

        public virtual float? GetSmothnessTolerance() {
            return GetPdfObject().GetAsFloat(PdfName.SM);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetSmoothnessTolerance(float smoothnessTolerance
            ) {
            return Put(PdfName.SM, new PdfNumber(smoothnessTolerance));
        }

        public virtual bool? GetAutomaticStrokeAdjustmentFlag() {
            return GetPdfObject().GetAsBool(PdfName.SA);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetAutomaticStrokeAdjustmentFlag(bool strokeAdjustment
            ) {
            return Put(PdfName.SA, new PdfBoolean(strokeAdjustment));
        }

        public virtual PdfObject GetBlendMode() {
            return GetPdfObject().Get(PdfName.BM);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetBlendMode(PdfObject blendMode) {
            return Put(PdfName.BM, blendMode);
        }

        public virtual PdfObject GetSoftMask() {
            return GetPdfObject().Get(PdfName.SMask);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetSoftMask(PdfObject sMask) {
            return Put(PdfName.SMask, sMask);
        }

        public virtual float? GetStrokeOpacity() {
            return GetPdfObject().GetAsFloat(PdfName.CA);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetStrokeOpacity(float strokingAlphaConstant) {
            return Put(PdfName.CA, new PdfNumber(strokingAlphaConstant));
        }

        public virtual float? GetFillOpacity() {
            return GetPdfObject().GetAsFloat(PdfName.ca);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetFillOpacity(float fillingAlphaConstant) {
            return Put(PdfName.ca, new PdfNumber(fillingAlphaConstant));
        }

        public virtual bool? GetAlphaSourceFlag() {
            return GetPdfObject().GetAsBool(PdfName.AIS);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetAlphaSourceFlag(bool alphaSourceFlag) {
            return Put(PdfName.AIS, new PdfBoolean(alphaSourceFlag));
        }

        public virtual bool? GetTextKnockoutFlag() {
            return GetPdfObject().GetAsBool(PdfName.TK);
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState SetTextKnockoutFlag(bool textKnockoutFlag) {
            return Put(PdfName.TK, new PdfBoolean(textKnockoutFlag));
        }

        public virtual iTextSharp.Kernel.Pdf.Extgstate.PdfExtGState Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
