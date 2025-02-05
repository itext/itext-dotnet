/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Forms.Fields {
    /// <summary>Builder for terminal form field.</summary>
    /// <typeparam name="T">specific terminal form field builder which extends this class.</typeparam>
    public abstract class TerminalFormFieldBuilder<T> : FormFieldBuilder<T>
        where T : iText.Forms.Fields.TerminalFormFieldBuilder<T> {
        /// <summary>Rectangle which defines widget placement.</summary>
        private Rectangle widgetRectangle = null;

        /// <summary>Page number to place widget at.</summary>
        private int page = 0;

        private PdfFont font;

        /// <summary>Creates builder for terminal form field creation.</summary>
        /// <param name="document">document to be used for form field creation</param>
        /// <param name="formFieldName">name of the form field</param>
        protected internal TerminalFormFieldBuilder(PdfDocument document, String formFieldName)
            : base(document, formFieldName) {
        }

        /// <summary>Gets rectangle which defines widget's placement.</summary>
        /// <returns>
        /// instance of
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// for widget placement
        /// </returns>
        public virtual Rectangle GetWidgetRectangle() {
            return widgetRectangle;
        }

        /// <summary>Gets page to be used for widget creation.</summary>
        /// <returns>number of page to place widget at</returns>
        public virtual int GetPage() {
            return page;
        }

        /// <summary>Sets page to be used for widget creation.</summary>
        /// <param name="page">
        /// instance of
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// . Shall belong to already provided
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// </param>
        /// <returns>this builder</returns>
        public virtual T SetPage(PdfPage page) {
            this.page = GetDocument().GetPageNumber(page);
            return GetThis();
        }

        /// <summary>Sets page to be used for widget creation.</summary>
        /// <param name="page">number of page to place widget at</param>
        /// <returns>this builder</returns>
        public virtual T SetPage(int page) {
            this.page = page;
            return GetThis();
        }

        /// <summary>Sets rectangle which defines widget's placement.</summary>
        /// <param name="widgetRectangle">
        /// instance of
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// for widget placement
        /// </param>
        /// <returns>this builder</returns>
        public virtual T SetWidgetRectangle(Rectangle widgetRectangle) {
            this.widgetRectangle = widgetRectangle;
            return GetThis();
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void SetPageToField(PdfFormField field) {
            if (page != 0) {
                field.GetFirstFormAnnotation().SetPage(page);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetPageToField(PdfFormAnnotation field) {
            if (page != 0) {
                field.SetPage(page);
            }
        }
//\endcond

        /// <summary>Set font to be used for form field creation.</summary>
        /// <param name="font">
        /// instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </param>
        /// <returns>this builder</returns>
        public virtual T SetFont(PdfFont font) {
            this.font = font;
            return GetThis();
        }

        /// <summary>Get font to be used for form field creation.</summary>
        /// <returns>
        /// instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </returns>
        public virtual PdfFont GetFont() {
            return font;
        }
    }
}
