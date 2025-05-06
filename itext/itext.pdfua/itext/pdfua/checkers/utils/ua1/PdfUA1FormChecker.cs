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
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua1 {
    /// <summary>Class that provides methods for checking PDF/UA-1 compliance of interactive form fields.</summary>
    public sealed class PdfUA1FormChecker {
        /// <summary>
        /// Creates a new
        /// <see cref="PdfUA1FormChecker"/>
        /// instance.
        /// </summary>
        private PdfUA1FormChecker() {
        }

        // Empty constructor.
        /// <summary>Checks "Form" structure element.</summary>
        /// <param name="form">structure element to check</param>
        public static void CheckFormStructElement(PdfStructElem form) {
            PdfDictionary widget = GetInteractiveKidForm(form);
            if (widget == null || !PdfUA1AnnotationChecker.IsAnnotationVisible(widget)) {
                // Check is also not applicable for hidden annotations.
                return;
            }
            PdfDictionary formField = widget;
            // Parent check is required for the case when form field and widget annotation are split up.
            if (!PdfFormField.IsFormField(widget) && widget.ContainsKey(PdfName.Parent)) {
                formField = widget.GetAsDictionary(PdfName.Parent);
            }
            // Element should have either alternative description or TU entry.
            bool fieldContainsTU = formField != null && formField.Get(PdfName.TU) != null;
            if (!fieldContainsTU && form.GetAlt() == null) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION);
            }
        }

        /// <summary>Gets a widget annotation kid if it exists.</summary>
        /// <param name="structElem">parent structure element</param>
        /// <returns>
        /// kid as
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// </returns>
        private static PdfDictionary GetInteractiveKidForm(PdfStructElem structElem) {
            IList<IStructureNode> kids = structElem.GetKids();
            bool containsSingleWidget = false;
            if (kids.Count == 1) {
                containsSingleWidget = kids[0] is PdfObjRef && PdfFormAnnotationUtil.IsPureWidgetOrMergedField(((PdfObjRef
                    )kids[0]).GetReferencedObject());
            }
            if (!containsSingleWidget && !ContainsRole(structElem)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.FORM_STRUCT_ELEM_WITHOUT_ROLE_SHALL_CONTAIN_ONE_WIDGET
                    );
            }
            return containsSingleWidget ? ((PdfObjRef)kids[0]).GetReferencedObject() : null;
        }

        private static bool ContainsRole(PdfStructElem structElem) {
            foreach (PdfStructureAttributes attributes in structElem.GetAttributesList()) {
                if ("PrintField".Equals(attributes.GetAttributeAsEnum("O")) && attributes.GetAttributeAsEnum("Role") != null
                    ) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Handler for checking form field elements in the tag tree.</summary>
        public class PdfUA1FormTagHandler : ContextAwareTagTreeIteratorHandler {
            /// <summary>
            /// Creates a new
            /// <see cref="PdfUA1FormTagHandler"/>
            /// instance.
            /// </summary>
            /// <param name="context">the validation context</param>
            public PdfUA1FormTagHandler(PdfUAValidationContext context)
                : base(context) {
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                if (elem is PdfObjRef && PdfFormAnnotationUtil.IsPureWidgetOrMergedField(((PdfObjRef)elem).GetReferencedObject
                    ()) && !StandardRoles.FORM.Equals(context.ResolveToStandardRole(elem.GetParent()))) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.WIDGET_SHALL_BE_FORM_OR_ARTIFACT);
                }
                PdfStructElem form = context.GetElementIfRoleMatches(PdfName.Form, elem);
                if (form == null) {
                    return;
                }
                PdfUA1FormChecker.CheckFormStructElement(form);
            }
        }
    }
}
