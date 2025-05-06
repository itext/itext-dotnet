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
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils.Checkers;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Class that provides methods for checking PDF/UA-2 compliance of interactive form fields.</summary>
    public sealed class PdfUA2FormChecker {
        private readonly PdfUAValidationContext context;

        /// <summary>
        /// Creates a new
        /// <see cref="PdfUA2FormChecker"/>
        /// instance.
        /// </summary>
        /// <param name="validationContext">the validation context</param>
        public PdfUA2FormChecker(PdfUAValidationContext validationContext) {
            this.context = validationContext;
        }

        /// <summary>Verifies the conformity of the widget annotation present in the document.</summary>
        /// <remarks>
        /// Verifies the conformity of the widget annotation present in the document.
        /// <para />
        /// Checks that each widget annotation is either Form structure element or an Artifact; if label for a widget
        /// annotation is not present or an additional action (AA) entry is present, Contents entry is provided.
        /// </remarks>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to check widgets from
        /// </param>
        public void CheckWidgetAnnotations(PdfDocument document) {
            int amountOfPages = document.GetNumberOfPages();
            for (int i = 1; i <= amountOfPages; ++i) {
                PdfPage page = document.GetPage(i);
                foreach (PdfAnnotation annot in page.GetAnnotations()) {
                    CheckWidget(annot.GetPdfObject());
                }
            }
        }

        /// <summary>Verifies the conformity of the document Acroform dictionary.</summary>
        /// <remarks>
        /// Verifies the conformity of the document Acroform dictionary.
        /// <para />
        /// Checks that each widget annotation is either Form structure element or an Artifact; if label for a widget
        /// annotation is not present or an additional action (AA) entry is present, Contents entry is provided;
        /// text field
        /// <c>RV</c>
        /// and
        /// <c>V</c>
        /// values are textually equal.
        /// </remarks>
        /// <param name="form">
        /// the form
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// to be checked
        /// </param>
        public void CheckFormFields(PdfDictionary form) {
            if (form == null) {
                return;
            }
            PdfArray fields = form.GetAsArray(PdfName.Fields);
            if (fields == null) {
                return;
            }
            fields = PdfCheckersUtil.GetFormFields(fields);
            foreach (PdfObject field in fields) {
                PdfDictionary fieldDic = (PdfDictionary)field;
                CheckWidget(fieldDic);
                CheckTextField(fieldDic);
            }
        }

        /// <summary>Checks "Form" structure element.</summary>
        /// <param name="elem">structure element to check</param>
        public void CheckFormStructElement(IStructureNode elem) {
            if (IsWidget(elem)) {
                String role = context.ResolveToStandardRole(elem.GetParent());
                if (!StandardRoles.ARTIFACT.Equals(role) && !StandardRoles.FORM.Equals(role)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.WIDGET_SHALL_BE_FORM_OR_ARTIFACT);
                }
                if (StandardRoles.FORM.Equals(role)) {
                    PdfDictionary widget = ((PdfObjRef)elem).GetReferencedObject();
                    PdfArray rect = widget.GetAsArray(PdfName.Rect);
                    if (rect != null && rect.Size() == 4) {
                        Rectangle rectangle = rect.ToRectangle();
                        if (rectangle.GetWidth() == 0 && rectangle.GetHeight() == 0) {
                            throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.WIDGET_WITH_ZERO_HEIGHT_SHALL_BE_AN_ARTIFACT
                                );
                        }
                    }
                }
                return;
            }
            PdfStructElem form = context.GetElementIfRoleMatches(PdfName.Form, elem);
            if (form == null) {
                return;
            }
            CheckWidgetKids(form);
        }

        /// <summary>
        /// Checks that
        /// <c>Form</c>
        /// structure element contains at most one widget annotation.
        /// </summary>
        /// <param name="form">
        /// 
        /// <c>Form</c>
        /// structure element to check
        /// </param>
        private static void CheckWidgetKids(IStructureNode form) {
            IList<IStructureNode> kids = form.GetKids();
            bool widgetFound = false;
            foreach (IStructureNode node in kids) {
                if (IsWidget(node)) {
                    if (widgetFound) {
                        throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.FORM_STRUCT_ELEM_SHALL_CONTAIN_AT_MOST_ONE_WIDGET
                            );
                    }
                    widgetFound = true;
                }
            }
        }

        private static bool IsWidget(IStructureNode node) {
            return node is PdfObjRef && PdfFormAnnotationUtil.IsPureWidgetOrMergedField(((PdfObjRef)node).GetReferencedObject
                ());
        }

        private static PdfObject GetValueFromParent(PdfDictionary field) {
            PdfDictionary parent = field.GetAsDictionary(PdfName.Parent);
            PdfObject fieldValue = field.Get(PdfName.V);
            if (parent != null) {
                fieldValue = parent.Get(PdfName.V);
                if (fieldValue == null) {
                    fieldValue = GetValueFromParent(parent);
                }
            }
            return fieldValue;
        }

        private static void CheckTextField(PdfDictionary fieldDic) {
            if (PdfName.Tx.Equals(PdfFormField.GetFormType(fieldDic)) && fieldDic.ContainsKey(PdfName.RV)) {
                String richText = PdfUA2AnnotationChecker.GetRichTextStringValue(fieldDic.Get(PdfName.RV));
                if (String.IsNullOrEmpty(richText)) {
                    return;
                }
                PdfObject fieldValue = fieldDic.Get(PdfName.V);
                if (fieldValue == null) {
                    fieldValue = GetValueFromParent(fieldDic);
                }
                String value = PdfFormField.GetStringValue(fieldValue);
                if (!richText.Equals(value)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.TEXT_FIELD_V_AND_RV_SHALL_BE_TEXTUALLY_EQUIVALENT
                        );
                }
            }
        }

        private void CheckWidget(PdfDictionary fieldDic) {
            if (!PdfFormAnnotationUtil.IsPureWidgetOrMergedField(fieldDic)) {
                return;
            }
            PdfObjRef objRef = null;
            if (fieldDic.GetAsNumber(PdfName.StructParent) != null) {
                int i = fieldDic.GetAsNumber(PdfName.StructParent).IntValue();
                PdfDictionary pageDict = fieldDic.GetAsDictionary(PdfName.P);
                objRef = context.FindObjRefByStructParentIndex(i, pageDict);
            }
            if (objRef != null) {
                String role = context.ResolveToStandardRole(objRef.GetParent());
                if (!StandardRoles.ARTIFACT.Equals(role) && !StandardRoles.FORM.Equals(role)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.WIDGET_SHALL_BE_FORM_OR_ARTIFACT);
                }
            }
            if ((objRef == null || !IsWidgetLabelPresent(objRef)) && !fieldDic.ContainsKey(PdfName.Contents)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS);
            }
            if (fieldDic.ContainsKey(PdfName.AA) && !fieldDic.ContainsKey(PdfName.Contents)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.WIDGET_WITH_AA_SHALL_PROVIDE_CONTENTS);
            }
        }

        private bool IsWidgetLabelPresent(IStructureNode widget) {
            PdfStructElem form = context.GetElementIfRoleMatches(PdfName.Form, widget.GetParent());
            if (form == null) {
                return false;
            }
            IList<IStructureNode> kids = form.GetKids();
            foreach (IStructureNode node in kids) {
                if (StandardRoles.LBL.Equals(context.ResolveToStandardRole(node))) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Handler for checking form field elements in the tag tree.</summary>
        public class PdfUA2FormTagHandler : ContextAwareTagTreeIteratorHandler {
            private readonly PdfUA2FormChecker pdfUA2FormChecker;

            /// <summary>
            /// Creates a new
            /// <see cref="PdfUA2FormTagHandler"/>
            /// instance.
            /// </summary>
            /// <param name="context">the validation context</param>
            public PdfUA2FormTagHandler(PdfUAValidationContext context)
                : base(context) {
                this.pdfUA2FormChecker = new PdfUA2FormChecker(context);
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                pdfUA2FormChecker.CheckFormStructElement(elem);
            }
        }
    }
}
