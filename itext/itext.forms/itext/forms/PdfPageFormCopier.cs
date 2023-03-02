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
    public class PdfPageFormCopier : IPdfPageExtraCopier {
        private PdfAcroForm formFrom;

        private PdfAcroForm formTo;

        private PdfDocument documentFrom;

        private PdfDocument documentTo;

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
            IDictionary<String, PdfFormField> fieldsTo = formTo.GetDirectFormFields();
            IList<PdfAnnotation> annots = toPage.GetAnnotations();
            foreach (PdfAnnotation annot in annots) {
                if (!annot.GetSubtype().Equals(PdfName.Widget)) {
                    continue;
                }
                CopyField(toPage, fieldsFrom, fieldsTo, annot);
            }
        }

        private AbstractPdfFormField MakeFormField(PdfObject fieldDict) {
            AbstractPdfFormField field = PdfFormField.MakeFormFieldOrAnnotation(fieldDict, documentTo);
            if (field == null) {
                logger.LogWarning(MessageFormatUtil.Format(FormsLogMessageConstants.CANNOT_CREATE_FORMFIELD, fieldDict.GetIndirectReference
                    ()));
            }
            return field;
        }

        private void CopyField(PdfPage toPage, IDictionary<String, PdfFormField> fieldsFrom, IDictionary<String, PdfFormField
            > fieldsTo, PdfAnnotation currentAnnot) {
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
                CopyParentFormField(toPage, fieldsTo, currentAnnot, parentField);
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
                    if (fieldsTo.Get(annotNameString) != null) {
                        field = MergeFieldsWithTheSameName(field);
                    }
                    // Form may be already added to the page. PdfAcroForm will take care about it.
                    formTo.AddField(field, toPage, true);
                    field.UpdateDefaultAppearance();
                }
            }
        }

        private void CopyParentFormField(PdfPage toPage, IDictionary<String, PdfFormField> fieldsTo, PdfAnnotation
             annot, PdfFormField parentField) {
            PdfString parentName = parentField.GetFieldName();
            // parentField should be the root field
            if (!fieldsTo.ContainsKey(parentName.ToUnicodeString())) {
                // no such field, hence we should simply add it
                PdfFormField field = CreateParentFieldCopy(annot.GetPdfObject(), documentTo);
                PdfArray kids = field.GetKids();
                field.GetPdfObject().Remove(PdfName.Kids);
                formTo.AddField(field, toPage, true);
                field.GetPdfObject().Put(PdfName.Kids, kids);
            }
            else {
                // annot is either a field (field name will not be null) or a widget (field name is null)
                AbstractPdfFormField field = MakeFormField(annot.GetPdfObject());
                if (field == null) {
                    return;
                }
                PdfString fieldName = field.GetFieldName();
                if (fieldName != null) {
                    PdfFormField existingField = fieldsTo.Get(fieldName.ToUnicodeString());
                    if (existingField != null) {
                        PdfFormField mergedField = MergeFieldsWithTheSameName(field);
                        formTo.GetDirectFormFields().Put(mergedField.GetFieldName().ToUnicodeString(), mergedField);
                    }
                    else {
                        HashSet<String> existingFields = new HashSet<String>();
                        GetAllFieldNames(formTo.GetFields(), existingFields);
                        AddChildToExistingParent(annot.GetPdfObject(), existingFields, fieldsTo);
                    }
                }
                else {
                    if (!parentField.GetKids().Contains(field.GetPdfObject()) && formTo.GetFields().Contains(parentField.GetPdfObject
                        ())) {
                        // annot's parent is already a field of the resultant document,
                        // hence we only need to update its children
                        HashSet<String> existingFields = new HashSet<String>();
                        GetAllFieldNames(formTo.GetFields(), existingFields);
                        AddChildToExistingParent(annot.GetPdfObject(), existingFields);
                    }
                    else {
                        // its parent is not a field of the resultant document, but the latter contains
                        // a field of the same name, therefore we should merge them (note that merging in this context
                        // differs from merging a widget and an annotation into a single entity)
                        PdfFormField mergedField = MergeFieldsWithTheSameName(field);
                        // we need to add the field not to its representation (#getFormFields()), but to
                        // /Fields entry of the acro form
                        formTo.AddField(mergedField, toPage, true);
                    }
                }
            }
        }

        private PdfFormField MergeFieldsWithTheSameName(AbstractPdfFormField newField) {
            PdfString fieldName = newField.GetPdfObject().GetAsString(PdfName.T);
            PdfDictionary parent = newField.GetParent();
            if (parent != null) {
                newField.SetParent(PdfFormField.MakeFormField(parent, newField.GetDocument()));
                if (fieldName == null) {
                    if (newField.IsTerminalFormField()) {
                        fieldName = new PdfString(parent.GetAsString(PdfName.T).ToUnicodeString() + ".");
                    }
                    else {
                        fieldName = parent.GetAsString(PdfName.T);
                    }
                }
            }
            String fullFieldName = fieldName.ToUnicodeString();
            if (null != newField.GetFieldName()) {
                fullFieldName = newField.GetFieldName().ToUnicodeString();
            }
            logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, 
                fullFieldName));
            PdfFormField existingField = formTo.GetField(fullFieldName);
            if (existingField.IsFlushed() && newField is PdfFormField) {
                int index = 0;
                do {
                    index++;
                    ((PdfFormField)newField).SetFieldName(fieldName.ToUnicodeString() + "_#" + index);
                    fullFieldName = newField.GetFieldName().ToUnicodeString();
                }
                while (formTo.GetField(fullFieldName) != null);
                return (PdfFormField)newField;
            }
            formTo.GetFields().Remove(existingField.GetPdfObject());
            if (newField is PdfFormField) {
                PdfFormFieldMergeUtil.MergeTwoFieldsWithTheSameNames(existingField, (PdfFormField)newField, true);
            }
            else {
                existingField.AddKid(newField);
            }
            return existingField;
        }

        private static PdfFormField GetParentField(PdfDictionary parent, PdfDocument pdfDoc) {
            PdfDictionary parentOfParent = parent.GetAsDictionary(PdfName.Parent);
            if (parentOfParent != null) {
                return GetParentField(parentOfParent, pdfDoc);
            }
            return PdfFormField.MakeFormField(parent, pdfDoc);
        }

        private PdfFormField CreateParentFieldCopy(PdfDictionary fieldDic, PdfDocument pdfDoc) {
            PdfDictionary parent = fieldDic.GetAsDictionary(PdfName.Parent);
            PdfFormField field;
            if (parent != null) {
                field = CreateParentFieldCopy(parent, pdfDoc);
                PdfArray kids = (PdfArray)parent.Get(PdfName.Kids);
                if (kids == null) {
                    parent.Put(PdfName.Kids, new PdfArray(fieldDic));
                }
                else {
                    kids.Add(fieldDic);
                }
            }
            else {
                field = PdfFormField.MakeFormField(fieldDic, pdfDoc);
            }
            return field;
        }

        private void AddChildToExistingParent(PdfDictionary fieldDic, ICollection<String> existingFields) {
            PdfDictionary parent = fieldDic.GetAsDictionary(PdfName.Parent);
            if (parent == null) {
                return;
            }
            PdfString parentName = parent.GetAsString(PdfName.T);
            if (parentName != null) {
                String name = parentName.ToUnicodeString();
                if (existingFields.Contains(name)) {
                    PdfArray kids = parent.GetAsArray(PdfName.Kids);
                    kids.Add(fieldDic);
                }
                else {
                    parent.Put(PdfName.Kids, new PdfArray(fieldDic));
                    AddChildToExistingParent(parent, existingFields);
                }
            }
        }

        private void AddChildToExistingParent(PdfDictionary fieldDic, ICollection<String> existingFields, IDictionary
            <String, PdfFormField> fieldsTo) {
            PdfDictionary parent = fieldDic.GetAsDictionary(PdfName.Parent);
            if (parent == null) {
                return;
            }
            PdfString parentName = parent.GetAsString(PdfName.T);
            if (parentName != null) {
                String name = parentName.ToUnicodeString();
                if (existingFields.Contains(name)) {
                    PdfArray kids = parent.GetAsArray(PdfName.Kids);
                    foreach (PdfObject kid in kids) {
                        if (((PdfDictionary)kid).Get(PdfName.T) != null && ((PdfDictionary)kid).Get(PdfName.T).Equals(fieldDic.Get
                            (PdfName.T))) {
                            AbstractPdfFormField kidField = MakeFormField(kid);
                            AbstractPdfFormField field = MakeFormField(fieldDic);
                            if (kidField == null || field == null) {
                                continue;
                            }
                            fieldsTo.Put(kidField.GetFieldName().ToUnicodeString(), (PdfFormField)kidField);
                            PdfFormField mergedField = MergeFieldsWithTheSameName(field);
                            formTo.GetDirectFormFields().Put(mergedField.GetFieldName().ToUnicodeString(), mergedField);
                            return;
                        }
                    }
                    kids.Add(fieldDic);
                }
                else {
                    parent.Put(PdfName.Kids, new PdfArray(fieldDic));
                    AddChildToExistingParent(parent, existingFields);
                }
            }
        }

        private void GetAllFieldNames(PdfArray fields, ICollection<String> existingFields) {
            foreach (PdfObject field in fields) {
                if (field.IsFlushed()) {
                    continue;
                }
                PdfDictionary dic = (PdfDictionary)field;
                PdfString name = dic.GetAsString(PdfName.T);
                if (name != null) {
                    existingFields.Add(name.ToUnicodeString());
                }
                PdfArray kids = dic.GetAsArray(PdfName.Kids);
                if (kids != null) {
                    GetAllFieldNames(kids, existingFields);
                }
            }
        }
    }
}
