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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>Custom implementation for rendering form field values.</summary>
    /// <remarks>
    /// Custom implementation for rendering form field values. It makes sure that text value
    /// trimming strategy matches Acrobat's behavior
    /// </remarks>
    public class FormFieldValueNonTrimmingTextRenderer : TextRenderer {
        // Temporarily public, make it package private on cleanup of PdfFormAnnotation
        // TODO DEVSIX-7423 (or put another devsix if the usage of this class is not removed from PdfFormAnnotation
        //  as part of DEVSIX-7423)
        // Determines whether we want to trim leading space. In particular we don't want to trim
        // the very first leading spaces of the text value. When text overflows to the next lines,
        // whether we should trim the text depends on why the overflow happened
        private bool callTrimFirst = false;

        public FormFieldValueNonTrimmingTextRenderer(Text textElement)
            : base(textElement) {
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Forms.Form.Renderer.FormFieldValueNonTrimmingTextRenderer((Text)GetModelElement());
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            LayoutResult baseLayoutResult = base.Layout(layoutContext);
            if (baseLayoutResult is TextLayoutResult && baseLayoutResult.GetOverflowRenderer() is iText.Forms.Form.Renderer.FormFieldValueNonTrimmingTextRenderer
                 && !((TextLayoutResult)baseLayoutResult).IsSplitForcedByNewline()) {
                // In case the overflow to the next line happened naturally (without a forced line break),
                // we don't want to preserve the extra spaces at the beginning of the next line
                ((iText.Forms.Form.Renderer.FormFieldValueNonTrimmingTextRenderer)baseLayoutResult.GetOverflowRenderer()).
                    SetCallTrimFirst(true);
            }
            return baseLayoutResult;
        }

        public override void TrimFirst() {
            if (callTrimFirst) {
                base.TrimFirst();
            }
        }

        /// <summary><inheritDoc/></summary>
        protected override TextRenderer CreateCopy(GlyphLine gl, PdfFont font) {
            if (typeof(iText.Forms.Form.Renderer.FormFieldValueNonTrimmingTextRenderer) != this.GetType()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Renderer.FormFieldValueNonTrimmingTextRenderer
                    ));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.CREATE_COPY_SHOULD_BE_OVERRIDDEN
                    ));
            }
            iText.Forms.Form.Renderer.FormFieldValueNonTrimmingTextRenderer copy = new iText.Forms.Form.Renderer.FormFieldValueNonTrimmingTextRenderer
                ((Text)this.modelElement);
            copy.SetProcessedGlyphLineAndFont(gl, font);
            return copy;
        }

        private void SetCallTrimFirst(bool callTrimFirst) {
            this.callTrimFirst = callTrimFirst;
        }
    }
}
