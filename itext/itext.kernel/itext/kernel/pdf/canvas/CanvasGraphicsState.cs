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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Extgstate;

namespace iText.Kernel.Pdf.Canvas {
    /// <summary>This class is designed for internal usage.</summary>
    /// <remarks>
    /// This class is designed for internal usage. <br />
    /// Use <c>PdfExtGState</c> class and <c>PdfCanvas#setExtGState()</c> method for setting extended graphics properties.
    /// </remarks>
    public class CanvasGraphicsState {
        /// <summary>The current transformation matrix, which maps positions from user coordinates to device coordinates.
        ///     </summary>
        /// <remarks>
        /// The current transformation matrix, which maps positions from user coordinates to device coordinates.
        /// <para />
        /// We use an identity matrix as a default value, but in spec a default value is:
        /// "a matrix that transforms default user coordinates to device coordinates".
        /// </remarks>
        private Matrix ctm = new Matrix();

        // color
        private Color strokeColor = DeviceGray.BLACK;

        private Color fillColor = DeviceGray.BLACK;

        // text state
        private float charSpacing = 0f;

        private float wordSpacing = 0f;

        // horizontal scaling
        private float scale = 100f;

        private float leading = 0f;

        private PdfFont font;

        private float fontSize;

        private int textRenderingMode = PdfCanvasConstants.TextRenderingMode.FILL;

        private float textRise = 0f;

        private bool textKnockout = true;

        private float lineWidth = 1f;

        private int lineCapStyle = PdfCanvasConstants.LineCapStyle.BUTT;

        private int lineJoinStyle = PdfCanvasConstants.LineJoinStyle.MITER;

        private float miterLimit = 10f;

        /// <summary>A description of the dash pattern to be used when paths are stroked.</summary>
        /// <remarks>
        /// A description of the dash pattern to be used when paths are stroked. Default value is solid line.
        /// <para />
        /// The line dash pattern is expressed as an array of the form [ dashArray dashPhase ],
        /// where dashArray is itself an array and dashPhase is an integer.
        /// <para />
        /// An empty dash array (first element in the array) and zero phase (second element in the array)
        /// can be used to restore the dash pattern to a solid line.
        /// </remarks>
        private PdfArray dashPattern = new PdfArray(JavaUtil.ArraysAsList(new PdfObject[] { new PdfArray(), new PdfNumber
            (0) }));

        private PdfName renderingIntent = PdfName.RelativeColorimetric;

        private bool automaticStrokeAdjustment = false;

        private PdfObject blendMode = PdfName.Normal;

        private PdfObject softMask = PdfName.None;

        // alpha constant
        private float strokeAlpha = 1f;

        private float fillAlpha = 1f;

        // alpha source
        private bool alphaIsShape = false;

        private bool strokeOverprint = false;

        private bool fillOverprint = false;

        private int overprintMode = 0;

        private PdfObject blackGenerationFunction;

        private PdfObject blackGenerationFunction2;

        private PdfObject underColorRemovalFunction;

        private PdfObject underColorRemovalFunction2;

        private PdfObject transferFunction;

        private PdfObject transferFunction2;

        private PdfObject halftone;

        private float flatnessTolerance = 1f;

        private float? smoothnessTolerance;

        private PdfObject htp;

        /// <summary>Internal empty and default constructor.</summary>
        protected internal CanvasGraphicsState() {
        }

        /// <summary>Copy constructor.</summary>
        /// <param name="source">the Graphics State to copy from</param>
        public CanvasGraphicsState(iText.Kernel.Pdf.Canvas.CanvasGraphicsState source) {
            CopyFrom(source);
        }

        /// <summary>Updates this object with the values from a dictionary.</summary>
        /// <param name="extGState">the dictionary containing source parameters</param>
        public virtual void UpdateFromExtGState(PdfDictionary extGState) {
            UpdateFromExtGState(new PdfExtGState(extGState), extGState.GetIndirectReference() == null ? null : extGState
                .GetIndirectReference().GetDocument());
        }

        /// <returns>current transformation matrix.</returns>
        public virtual Matrix GetCtm() {
            return ctm;
        }

        /// <summary>Updates current transformation matrix.</summary>
        /// <remarks>
        /// Updates current transformation matrix.
        /// The third column will always be [0 0 1]
        /// </remarks>
        /// <param name="a">element at (1,1) of the transformation matrix</param>
        /// <param name="b">element at (1,2) of the transformation matrix</param>
        /// <param name="c">element at (2,1) of the transformation matrix</param>
        /// <param name="d">element at (2,2) of the transformation matrix</param>
        /// <param name="e">element at (3,1) of the transformation matrix</param>
        /// <param name="f">element at (3,2) of the transformation matrix</param>
        public virtual void UpdateCtm(float a, float b, float c, float d, float e, float f) {
            UpdateCtm(new Matrix(a, b, c, d, e, f));
        }

        /// <summary>Updates current transformation matrix.</summary>
        /// <param name="newCtm">new current transformation matrix.</param>
        public virtual void UpdateCtm(Matrix newCtm) {
            ctm = newCtm.Multiply(ctm);
        }

        public virtual Color GetFillColor() {
            return fillColor;
        }

        public virtual void SetFillColor(Color fillColor) {
            this.fillColor = fillColor;
        }

        public virtual Color GetStrokeColor() {
            return strokeColor;
        }

        public virtual void SetStrokeColor(Color strokeColor) {
            this.strokeColor = strokeColor;
        }

        public virtual float GetLineWidth() {
            return lineWidth;
        }

        public virtual void SetLineWidth(float lineWidth) {
            this.lineWidth = lineWidth;
        }

        public virtual int GetLineCapStyle() {
            return lineCapStyle;
        }

        public virtual void SetLineCapStyle(int lineCapStyle) {
            this.lineCapStyle = lineCapStyle;
        }

        public virtual int GetLineJoinStyle() {
            return lineJoinStyle;
        }

        public virtual void SetLineJoinStyle(int lineJoinStyle) {
            this.lineJoinStyle = lineJoinStyle;
        }

        public virtual float GetMiterLimit() {
            return miterLimit;
        }

        public virtual void SetMiterLimit(float miterLimit) {
            this.miterLimit = miterLimit;
        }

        public virtual PdfArray GetDashPattern() {
            return dashPattern;
        }

        public virtual void SetDashPattern(PdfArray dashPattern) {
            this.dashPattern = dashPattern;
        }

        public virtual PdfName GetRenderingIntent() {
            return renderingIntent;
        }

        public virtual void SetRenderingIntent(PdfName renderingIntent) {
            this.renderingIntent = renderingIntent;
        }

        public virtual float GetFontSize() {
            return fontSize;
        }

        public virtual void SetFontSize(float fontSize) {
            this.fontSize = fontSize;
        }

        public virtual PdfFont GetFont() {
            return font;
        }

        public virtual void SetFont(PdfFont font) {
            this.font = font;
        }

        public virtual int GetTextRenderingMode() {
            return textRenderingMode;
        }

        public virtual void SetTextRenderingMode(int textRenderingMode) {
            this.textRenderingMode = textRenderingMode;
        }

        public virtual float GetTextRise() {
            return textRise;
        }

        public virtual void SetTextRise(float textRise) {
            this.textRise = textRise;
        }

        public virtual float GetFlatnessTolerance() {
            return flatnessTolerance;
        }

        public virtual void SetFlatnessTolerance(float flatnessTolerance) {
            this.flatnessTolerance = flatnessTolerance;
        }

        public virtual void SetWordSpacing(float wordSpacing) {
            this.wordSpacing = wordSpacing;
        }

        public virtual float GetWordSpacing() {
            return wordSpacing;
        }

        public virtual void SetCharSpacing(float characterSpacing) {
            this.charSpacing = characterSpacing;
        }

        public virtual float GetCharSpacing() {
            return charSpacing;
        }

        public virtual float GetLeading() {
            return leading;
        }

        public virtual void SetLeading(float leading) {
            this.leading = leading;
        }

        public virtual float GetHorizontalScaling() {
            return scale;
        }

        public virtual void SetHorizontalScaling(float scale) {
            this.scale = scale;
        }

        public virtual bool GetStrokeOverprint() {
            return strokeOverprint;
        }

        public virtual bool GetFillOverprint() {
            return fillOverprint;
        }

        public virtual int GetOverprintMode() {
            return overprintMode;
        }

        public virtual PdfObject GetBlackGenerationFunction() {
            return blackGenerationFunction;
        }

        public virtual PdfObject GetBlackGenerationFunction2() {
            return blackGenerationFunction2;
        }

        public virtual PdfObject GetUnderColorRemovalFunction() {
            return underColorRemovalFunction;
        }

        public virtual PdfObject GetUnderColorRemovalFunction2() {
            return underColorRemovalFunction2;
        }

        public virtual PdfObject GetTransferFunction() {
            return transferFunction;
        }

        public virtual PdfObject GetTransferFunction2() {
            return transferFunction2;
        }

        public virtual PdfObject GetHalftone() {
            return halftone;
        }

        public virtual float? GetSmoothnessTolerance() {
            return smoothnessTolerance;
        }

        public virtual bool GetAutomaticStrokeAdjustment() {
            return automaticStrokeAdjustment;
        }

        public virtual PdfObject GetBlendMode() {
            return blendMode;
        }

        public virtual PdfObject GetSoftMask() {
            return softMask;
        }

        public virtual float GetStrokeOpacity() {
            return strokeAlpha;
        }

        public virtual float GetFillOpacity() {
            return fillAlpha;
        }

        public virtual bool GetAlphaIsShape() {
            return alphaIsShape;
        }

        public virtual bool GetTextKnockout() {
            return textKnockout;
        }

        public virtual PdfObject GetHTP() {
            return htp;
        }

        /// <summary>Updates current graphic state with values from extended graphic state dictionary.</summary>
        /// <param name="extGState">the wrapper around the extended graphic state dictionary</param>
        public virtual void UpdateFromExtGState(PdfExtGState extGState) {
            UpdateFromExtGState(extGState, null);
        }

        /// <summary>Updates current graphic state with values from extended graphic state dictionary.</summary>
        /// <param name="extGState">the wrapper around the extended graphic state dictionary</param>
        /// <param name="pdfDocument">the document to retrieve fonts from. Needed when the newly created fonts are used
        ///     </param>
        internal virtual void UpdateFromExtGState(PdfExtGState extGState, PdfDocument pdfDocument) {
            float? lw = extGState.GetLineWidth();
            if (lw != null) {
                lineWidth = (float)lw;
            }
            int? lc = extGState.GetLineCapStyle();
            if (lc != null) {
                lineCapStyle = (int)lc;
            }
            int? lj = extGState.GetLineJoinStyle();
            if (lj != null) {
                lineJoinStyle = (int)lj;
            }
            float? ml = extGState.GetMiterLimit();
            if (ml != null) {
                miterLimit = (float)ml;
            }
            PdfArray d = extGState.GetDashPattern();
            if (d != null) {
                dashPattern = d;
            }
            PdfName ri = extGState.GetRenderingIntent();
            if (ri != null) {
                renderingIntent = ri;
            }
            bool? op = extGState.GetStrokeOverprintFlag();
            if (op != null) {
                strokeOverprint = (bool)op;
            }
            op = extGState.GetFillOverprintFlag();
            if (op != null) {
                fillOverprint = (bool)op;
            }
            int? opm = extGState.GetOverprintMode();
            if (opm != null) {
                overprintMode = (int)opm;
            }
            PdfArray fnt = extGState.GetFont();
            if (fnt != null) {
                PdfDictionary fontDictionary = fnt.GetAsDictionary(0);
                if (this.font == null || this.font.GetPdfObject() != fontDictionary) {
                    this.font = pdfDocument.GetFont(fontDictionary);
                }
                PdfNumber fntSz = fnt.GetAsNumber(1);
                if (fntSz != null) {
                    this.fontSize = fntSz.FloatValue();
                }
            }
            PdfObject bg = extGState.GetBlackGenerationFunction();
            if (bg != null) {
                blackGenerationFunction = bg;
            }
            PdfObject bg2 = extGState.GetBlackGenerationFunction2();
            if (bg2 != null) {
                blackGenerationFunction2 = bg2;
            }
            PdfObject ucr = extGState.GetUndercolorRemovalFunction();
            if (ucr != null) {
                underColorRemovalFunction = ucr;
            }
            PdfObject ucr2 = extGState.GetUndercolorRemovalFunction2();
            if (ucr2 != null) {
                underColorRemovalFunction2 = ucr2;
            }
            PdfObject tr = extGState.GetTransferFunction();
            if (tr != null) {
                transferFunction = tr;
            }
            PdfObject tr2 = extGState.GetTransferFunction2();
            if (tr2 != null) {
                transferFunction2 = tr2;
            }
            PdfObject ht = extGState.GetHalftone();
            if (ht != null) {
                halftone = ht;
            }
            PdfObject local_htp = extGState.GetPdfObject().Get(PdfName.HTP);
            if (local_htp != null) {
                this.htp = local_htp;
            }
            float? fl = extGState.GetFlatnessTolerance();
            if (fl != null) {
                flatnessTolerance = (float)fl;
            }
            float? sm = extGState.GetSmothnessTolerance();
            if (sm != null) {
                smoothnessTolerance = sm;
            }
            bool? sa = extGState.GetAutomaticStrokeAdjustmentFlag();
            if (sa != null) {
                automaticStrokeAdjustment = (bool)sa;
            }
            PdfObject bm = extGState.GetBlendMode();
            if (bm != null) {
                blendMode = bm;
            }
            PdfObject sMask = extGState.GetSoftMask();
            if (sMask != null) {
                softMask = sMask;
            }
            float? ca = extGState.GetStrokeOpacity();
            if (ca != null) {
                strokeAlpha = (float)ca;
            }
            ca = extGState.GetFillOpacity();
            if (ca != null) {
                fillAlpha = (float)ca;
            }
            bool? ais = extGState.GetAlphaSourceFlag();
            if (ais != null) {
                alphaIsShape = (bool)ais;
            }
            bool? tk = extGState.GetTextKnockoutFlag();
            if (tk != null) {
                textKnockout = (bool)tk;
            }
        }

        private void CopyFrom(iText.Kernel.Pdf.Canvas.CanvasGraphicsState source) {
            this.ctm = source.ctm;
            this.strokeColor = source.strokeColor;
            this.fillColor = source.fillColor;
            this.charSpacing = source.charSpacing;
            this.wordSpacing = source.wordSpacing;
            this.scale = source.scale;
            this.leading = source.leading;
            this.font = source.font;
            this.fontSize = source.fontSize;
            this.textRenderingMode = source.textRenderingMode;
            this.textRise = source.textRise;
            this.textKnockout = source.textKnockout;
            this.lineWidth = source.lineWidth;
            this.lineCapStyle = source.lineCapStyle;
            this.lineJoinStyle = source.lineJoinStyle;
            this.miterLimit = source.miterLimit;
            this.dashPattern = source.dashPattern;
            this.renderingIntent = source.renderingIntent;
            this.automaticStrokeAdjustment = source.automaticStrokeAdjustment;
            this.blendMode = source.blendMode;
            this.softMask = source.softMask;
            this.strokeAlpha = source.strokeAlpha;
            this.fillAlpha = source.fillAlpha;
            this.alphaIsShape = source.alphaIsShape;
            this.strokeOverprint = source.strokeOverprint;
            this.fillOverprint = source.fillOverprint;
            this.overprintMode = source.overprintMode;
            this.blackGenerationFunction = source.blackGenerationFunction;
            this.blackGenerationFunction2 = source.blackGenerationFunction2;
            this.underColorRemovalFunction = source.underColorRemovalFunction;
            this.underColorRemovalFunction2 = source.underColorRemovalFunction2;
            this.transferFunction = source.transferFunction;
            this.transferFunction2 = source.transferFunction2;
            this.halftone = source.halftone;
            this.flatnessTolerance = source.flatnessTolerance;
            this.smoothnessTolerance = source.smoothnessTolerance;
            this.htp = source.htp;
        }
    }
}
