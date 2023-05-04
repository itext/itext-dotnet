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
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms.Fields;
using iText.Forms.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms {
    /// <summary>
    /// A sample implementation of the {#link IPdfPageExtraCopier} interface which
    /// copies only AcroForm fields to a new page.
    /// </summary>
    /// <remarks>
    /// A sample implementation of the {#link IPdfPageExtraCopier} interface which
    /// copies only AcroForm fields to a new page.
    /// <para />
    /// NOTE: While it's absolutely not necessary to use the same PdfPageFormCopier instance for copying operations,
    /// it is still worth to know that PdfPageFormCopier uses some caching logic which can potentially improve performance
    /// in case of the reusing of the same instance.
    /// </remarks>
    public class PdfPageFormCopier : IPdfPageFormCopier {
        private PdfAcroForm formFrom;

        private PdfAcroForm formTo;

        private PdfDocument documentFrom;

        private PdfDocument documentTo;

        private readonly ICollection<PdfObject> collectedFieldObjects = new LinkedHashSet<PdfObject>();

        private static ILogger logger = ITextLogManager.GetLogger(typeof(PdfPageFormCopier));

        public virtual void Copy(PdfPage fromPage, PdfPage toPage) {
            if (documentFrom != fromPage.GetDocument()) {
                documentFrom = fromPage.GetDocument();
                formFrom = PdfAcroForm.GetAcroForm(documentFrom, false);
            }
            if (documentTo != toPage.GetDocument()) {
                documentTo = toPage.GetDocument();
            }
            // We should always regenerate the acroform if we expect the same result when the old or new
            // PdfPageFormCopier instance is used because getAcroForm changes the fields structure,
            // e.g. removes wrong field keys from the pure widget annotations dictionaries.
            formTo = PdfAcroForm.GetAcroForm(documentTo, true);
            if (formFrom == null) {
                return;
            }
            //duplicate AcroForm dictionary
            IList<PdfName> excludedKeys = new List<PdfName>();
            excludedKeys.Add(PdfName.Fields);
            excludedKeys.Add(PdfName.DR);
            PdfDictionary dict = formFrom.GetPdfObject().CopyTo(documentTo, excludedKeys, false);
            formTo.GetPdfObject().MergeDifferent(dict);
            IDictionary<String, PdfFormField> fieldsFrom = formFrom.GetAllFormFields();
            if (fieldsFrom.Count <= 0) {
                return;
            }
            IDictionary<String, PdfFormField> fieldsTo = formTo.GetRootFormFields();
            IList<PdfAnnotation> annots = toPage.GetAnnotations();
            try {
                foreach (PdfAnnotation annot in annots) {
                    if (!annot.GetSubtype().Equals(PdfName.Widget)) {
                        continue;
                    }
                    CopyField(fieldsFrom, fieldsTo, annot);
                }
                // Add collected field objects to the acroform PDF object.
                PdfArray fieldsArray = formTo.GetFields();
                fieldsArray.AddAll(collectedFieldObjects);
                fieldsArray.SetModified();
                if (!documentFrom.IsTagged() || !documentTo.IsTagged()) {
                    // It makes sense to create the Acroform only after copying the tag structure,
                    // so fields with the same names will be merged ang tag structure will be correct,
                    // but when the document is not tagged we can re-create Acroform with added fields right away.
                    PdfAcroForm.GetAcroForm(documentTo, true);
                }
            }
            finally {
                collectedFieldObjects.Clear();
            }
        }

        public virtual void RecreateAcroformToProcessCopiedFields(PdfDocument documentTo) {
            PdfAcroForm.GetAcroForm(documentTo, true);
        }

        private AbstractPdfFormField MakeFormField(PdfObject fieldDict) {
            AbstractPdfFormField field = PdfFormField.MakeFormFieldOrAnnotation(fieldDict, documentTo);
            if (field == null) {
                logger.LogWarning(MessageFormatUtil.Format(FormsLogMessageConstants.CANNOT_CREATE_FORMFIELD, fieldDict.GetIndirectReference
                    ()));
            }
            return field;
        }

        private void CopyField(IDictionary<String, PdfFormField> fieldsFrom, IDictionary<String, PdfFormField> fieldsTo
            , PdfAnnotation currentAnnot) {
            PdfDictionary parent = currentAnnot.GetPdfObject().GetAsDictionary(PdfName.Parent);
            if (parent != null) {
                PdfFormField parentField = GetParentField(parent, documentTo);
                if (parentField == null) {
                    return;
                }
                PdfString parentName = parentField.GetFieldName();
                if (parentName == null) {
                    return;
                }
                CopyParentFormField(fieldsTo, currentAnnot, parentField);
            }
            else {
                PdfString annotName = currentAnnot.GetPdfObject().GetAsString(PdfName.T);
                String annotNameString = null;
                if (annotName != null) {
                    annotNameString = annotName.ToUnicodeString();
                }
                if (annotNameString != null && fieldsFrom.ContainsKey(annotNameString)) {
                    // In this piece on code we expect annotation with T field
                    // It could mean only merged form field and annotation
                    // This cast must be ok
                    PdfFormField field = (PdfFormField)MakeFormField(currentAnnot.GetPdfObject());
                    if (field == null) {
                        return;
                    }
                    if (!collectedFieldObjects.Contains(field.GetPdfObject())) {
                        if (fieldsTo.Get(annotNameString) != null) {
                            logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, 
                                annotNameString));
                        }
                        collectedFieldObjects.Add(field.GetPdfObject());
                    }
                    field.UpdateDefaultAppearance();
                }
            }
        }

        private void CopyParentFormField(IDictionary<String, PdfFormField> fieldsTo, PdfAnnotation annot, PdfFormField
             parentField) {
            String parentName = parentField.GetFieldName().ToUnicodeString();
            PdfFormField existingField = fieldsTo.Get(parentName);
            PdfFormField field = CreateParentFieldCopy(annot.GetPdfObject(), documentTo);
            if (!collectedFieldObjects.Contains(field.GetPdfObject())) {
                if (existingField != null) {
                    logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, 
                        parentName));
                }
                collectedFieldObjects.Add(field.GetPdfObject());
            }
        }

        private static PdfFormField GetParentField(PdfDictionary parent, PdfDocument pdfDoc) {
            PdfDictionary parentOfParent = parent.GetAsDictionary(PdfName.Parent);
            if (parentOfParent != null) {
                return GetParentField(parentOfParent, pdfDoc);
            }
            return PdfFormField.MakeFormField(parent, pdfDoc);
        }

        private PdfFormField CreateParentFieldCopy(PdfDictionary fieldDict, PdfDocument pdfDoc) {
            PdfDictionary parent = fieldDict.GetAsDictionary(PdfName.Parent);
            PdfFormField field;
            if (parent != null) {
                // Here we operate with Kids array to do not run split/merge logic before PdfAcroForm.addField
                PdfArray kids = (PdfArray)parent.Get(PdfName.Kids);
                if (kids == null) {
                    parent.Put(PdfName.Kids, new PdfArray(fieldDict));
                }
                else {
                    if (!kids.Contains(fieldDict)) {
                        kids.Add(fieldDict);
                    }
                }
                field = CreateParentFieldCopy(parent, pdfDoc);
            }
            else {
                field = PdfFormField.MakeFormField(fieldDict, pdfDoc);
            }
            return field;
        }
    }
}
