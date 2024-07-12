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
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Class that provides methods for checking PDF/UA compliance of interactive form fields.</summary>
    public class FormCheckUtil {
        /// <summary>
        /// Creates a new
        /// <see cref="FormCheckUtil"/>
        /// instance.
        /// </summary>
        private FormCheckUtil() {
        }

        // Empty constructor
        /// <summary>Handler for checking form field elements in the tag tree.</summary>
        public class FormTagHandler : ContextAwareTagTreeIteratorHandler {
            /// <summary>
            /// Creates a new
            /// <see cref="FormulaTagHandler"/>
            /// instance.
            /// </summary>
            /// <param name="context">The validation context.</param>
            public FormTagHandler(PdfUAValidationContext context)
                : base(context) {
            }

            /// <summary><inheritDoc/></summary>
            public override void NextElement(IStructureNode elem) {
                PdfStructElem form = context.GetElementIfRoleMatches(PdfName.Form, elem);
                if (form == null) {
                    return;
                }
                PdfDictionary formField = GetInteractiveKidForm(form);
                if (formField == null) {
                    return;
                }
                // Check is not applicable for hidden annotations
                if (!AnnotationCheckUtil.IsAnnotationVisible(formField)) {
                    return;
                }
                // Parent check is required for the case when form field and widget annotation are split up.
                // It is still not 100% correct because TU is not inheritable thus shouldn't be taken into account
                // for the case like field -> merged field with widget annotation
                // (here we must not take field.TU into account)
                bool parentContainsTU = formField.Get(PdfName.Parent) != null && formField.GetAsDictionary(PdfName.Parent)
                    .Get(PdfName.TU) != null;
                // Element should have either alternative description or TU entry
                if (formField.Get(PdfName.TU) == null && !parentContainsTU && form.GetAlt() == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION);
                }
            }

            /// <summary>Gets a widget annotation kid if it exists.</summary>
            /// <param name="structElem">Parent structure element.</param>
            /// <returns>Kid as PdfDictionary.</returns>
            private static PdfDictionary GetInteractiveKidForm(PdfStructElem structElem) {
                PdfDictionary @object = structElem.GetPdfObject();
                PdfDictionary kids = @object.GetAsDictionary(PdfName.K);
                // It's a dictionary in this particular case
                if (kids != null && kids.Get(PdfName.Obj) != null && PdfName.Widget.Equals(((PdfDictionary)kids.Get(PdfName
                    .Obj)).GetAsName(PdfName.Subtype))) {
                    return (PdfDictionary)kids.Get(PdfName.Obj);
                }
                return null;
            }
        }
    }
}
