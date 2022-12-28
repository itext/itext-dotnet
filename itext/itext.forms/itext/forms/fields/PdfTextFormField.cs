/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms.Fields {
    /// <summary>An AcroForm field containing textual data.</summary>
    public class PdfTextFormField : PdfFormField {
        public static readonly int FF_FILE_SELECT = MakeFieldFlag(21);

        public static readonly int FF_DO_NOT_SPELL_CHECK = MakeFieldFlag(23);

        public static readonly int FF_DO_NOT_SCROLL = MakeFieldFlag(24);

        public static readonly int FF_COMB = MakeFieldFlag(25);

        public static readonly int FF_RICH_TEXT = MakeFieldFlag(26);

        protected internal PdfTextFormField(PdfDocument pdfDocument)
            : base(pdfDocument) {
        }

        protected internal PdfTextFormField(PdfWidgetAnnotation widget, PdfDocument pdfDocument)
            : base(widget, pdfDocument) {
        }

        protected internal PdfTextFormField(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Returns <c>Tx</c>, the form type for textual form fields.</summary>
        /// <returns>
        /// the form type, as a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// </returns>
        public override PdfName GetFormType() {
            return PdfName.Tx;
        }

        /// <summary>If true, the field can contain multiple lines of text; if false, the field?s text is restricted to a single line.
        ///     </summary>
        /// <param name="multiline">whether or not the file can contain multiple lines of text</param>
        /// <returns>
        /// current
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfTextFormField SetMultiline(bool multiline) {
            return (iText.Forms.Fields.PdfTextFormField)SetFieldFlag(FF_MULTILINE, multiline);
        }

        /// <summary>If true, the field is intended for entering a secure password that should not be echoed visibly to the screen.
        ///     </summary>
        /// <remarks>
        /// If true, the field is intended for entering a secure password that should not be echoed visibly to the screen.
        /// Characters typed from the keyboard should instead be echoed in some unreadable form, such as asterisks or bullet characters.
        /// </remarks>
        /// <param name="password">whether or not to obscure the typed characters</param>
        /// <returns>
        /// current
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfTextFormField SetPassword(bool password) {
            return (iText.Forms.Fields.PdfTextFormField)SetFieldFlag(FF_PASSWORD, password);
        }

        /// <summary>
        /// If true, the text entered in the field represents the pathname of a file
        /// whose contents are to be submitted as the value of the field.
        /// </summary>
        /// <returns>whether or not this field currently represents a path</returns>
        public virtual bool IsFileSelect() {
            return GetFieldFlag(FF_FILE_SELECT);
        }

        /// <summary>
        /// If true, the text entered in the field represents the pathname of a file
        /// whose contents are to be submitted as the value of the field.
        /// </summary>
        /// <param name="fileSelect">whether or not this field should represent a path</param>
        /// <returns>
        /// current
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfTextFormField SetFileSelect(bool fileSelect) {
            return (iText.Forms.Fields.PdfTextFormField)SetFieldFlag(FF_FILE_SELECT, fileSelect);
        }

        /// <summary>If true, text entered in the field is spell-checked.</summary>
        /// <returns>whether or not spell-checking is currently enabled</returns>
        public virtual bool IsSpellCheck() {
            return !GetFieldFlag(FF_DO_NOT_SPELL_CHECK);
        }

        /// <summary>If true, text entered in the field is spell-checked.</summary>
        /// <param name="spellCheck">whether or not to spell-check</param>
        /// <returns>
        /// current
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfTextFormField SetSpellCheck(bool spellCheck) {
            return (iText.Forms.Fields.PdfTextFormField)SetFieldFlag(FF_DO_NOT_SPELL_CHECK, !spellCheck);
        }

        /// <summary>
        /// If true, the field scrolls (horizontally for single-line fields, vertically for multiple-line fields)
        /// to accommodate more text than fits within its annotation rectangle.
        /// </summary>
        /// <remarks>
        /// If true, the field scrolls (horizontally for single-line fields, vertically for multiple-line fields)
        /// to accommodate more text than fits within its annotation rectangle.
        /// Once the field is full, no further text is accepted.
        /// </remarks>
        /// <returns>whether or not longer texts are currently allowed</returns>
        public virtual bool IsScroll() {
            return !GetFieldFlag(FF_DO_NOT_SCROLL);
        }

        /// <summary>
        /// If true, the field scrolls (horizontally for single-line fields, vertically for multiple-line fields)
        /// to accommodate more text than fits within its annotation rectangle.
        /// </summary>
        /// <remarks>
        /// If true, the field scrolls (horizontally for single-line fields, vertically for multiple-line fields)
        /// to accommodate more text than fits within its annotation rectangle.
        /// Once the field is full, no further text is accepted.
        /// </remarks>
        /// <param name="scroll">whether or not to allow longer texts</param>
        /// <returns>
        /// current
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfTextFormField SetScroll(bool scroll) {
            return (iText.Forms.Fields.PdfTextFormField)SetFieldFlag(FF_DO_NOT_SCROLL, !scroll);
        }

        /// <summary>
        /// Meaningful only if the MaxLen entry is present in the text field dictionary
        /// and if the Multiline, Password, and FileSelect flags are clear.
        /// </summary>
        /// <remarks>
        /// Meaningful only if the MaxLen entry is present in the text field dictionary
        /// and if the Multiline, Password, and FileSelect flags are clear.
        /// If true, the field is automatically divided into as many equally spaced positions,
        /// or combs, as the value of MaxLen, and the text is laid out into those combs.
        /// </remarks>
        /// <returns>whether or not combing is enabled</returns>
        public virtual bool IsComb() {
            return GetFieldFlag(FF_COMB);
        }

        /// <summary>
        /// Meaningful only if the MaxLen entry is present in the text field dictionary
        /// and if the Multiline, Password, and FileSelect flags are clear.
        /// </summary>
        /// <remarks>
        /// Meaningful only if the MaxLen entry is present in the text field dictionary
        /// and if the Multiline, Password, and FileSelect flags are clear.
        /// If true, the field is automatically divided into as many equally spaced positions,
        /// or combs, as the value of MaxLen, and the text is laid out into those combs.
        /// </remarks>
        /// <param name="comb">whether or not to enable combing</param>
        /// <returns>
        /// current
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfTextFormField SetComb(bool comb) {
            return (iText.Forms.Fields.PdfTextFormField)SetFieldFlag(FF_COMB, comb);
        }

        /// <summary>If true, the value of this field should be represented as a rich text string.</summary>
        /// <remarks>
        /// If true, the value of this field should be represented as a rich text string.
        /// If the field has a value, the RV entry of the field dictionary specifies the rich text string.
        /// </remarks>
        /// <returns>whether or not text is currently represented as rich text</returns>
        public virtual bool IsRichText() {
            return GetFieldFlag(FF_RICH_TEXT);
        }

        /// <summary>If true, the value of this field should be represented as a rich text string.</summary>
        /// <remarks>
        /// If true, the value of this field should be represented as a rich text string.
        /// If the field has a value, the RV entry of the field dictionary specifies the rich text string.
        /// </remarks>
        /// <param name="richText">whether or not to represent text as rich text</param>
        /// <returns>
        /// current
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfTextFormField SetRichText(bool richText) {
            return (iText.Forms.Fields.PdfTextFormField)SetFieldFlag(FF_RICH_TEXT, richText);
        }

        /// <summary>Gets the maximum length of the field's text, in characters.</summary>
        /// <remarks>
        /// Gets the maximum length of the field's text, in characters.
        /// This is an optional parameter, so if it is not specified, 0 value will be returned.
        /// </remarks>
        /// <returns>the current maximum text length</returns>
        public virtual int GetMaxLen() {
            PdfNumber maxLenEntry = this.GetPdfObject().GetAsNumber(PdfName.MaxLen);
            if (maxLenEntry != null) {
                return maxLenEntry.IntValue();
            }
            else {
                PdfDictionary parent = GetParent();
                // MaxLen is an inherited form field property, therefore we try to recursively extract it from the ancestors
                if (parent != null) {
                    return new iText.Forms.Fields.PdfTextFormField(parent).GetMaxLen();
                }
                else {
                    return 0;
                }
            }
        }

        /// <summary>Sets the maximum length of the field's text, in characters.</summary>
        /// <param name="maxLen">the maximum text length</param>
        /// <returns>current</returns>
        public virtual iText.Forms.Fields.PdfTextFormField SetMaxLen(int maxLen) {
            Put(PdfName.MaxLen, new PdfNumber(maxLen));
            if (GetFieldFlag(FF_COMB)) {
                RegenerateField();
            }
            return this;
        }
    }
}
