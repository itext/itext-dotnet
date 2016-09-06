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
using System;
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.IO;
using iText.IO.Log;
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
    /// <br/><br/>
    /// NOTE: While it's absolutely not necessary to use the same PdfPageFormCopier instance for copying operations,
    /// it is still worth to know that PdfPageFormCopier uses some caching logic which can potentially improve performance
    /// in case of the reusing of the same instance.
    /// </remarks>
    public class PdfPageFormCopier : IPdfPageExtraCopier {
        internal PdfAcroForm formFrom;

        internal PdfAcroForm formTo;

        internal PdfDocument documentFrom;

        internal PdfDocument documentTo;

        internal ILogger logger = LoggerFactory.GetLogger(typeof(PdfPageFormCopier));

        public virtual void Copy(PdfPage fromPage, PdfPage toPage) {
            if (documentFrom != fromPage.GetDocument()) {
                documentFrom = fromPage.GetDocument();
                formFrom = PdfAcroForm.GetAcroForm(documentFrom, false);
            }
            if (documentTo != toPage.GetDocument()) {
                documentTo = toPage.GetDocument();
                formTo = PdfAcroForm.GetAcroForm(documentTo, true);
            }
            if (formFrom != null) {
                //duplicate AcroForm dictionary
                IList<PdfName> excludedKeys = new List<PdfName>();
                excludedKeys.Add(PdfName.Fields);
                excludedKeys.Add(PdfName.DR);
                PdfDictionary dict = formFrom.GetPdfObject().CopyTo(documentTo, excludedKeys, false);
                formTo.GetPdfObject().MergeDifferent(dict);
            }
            if (formFrom != null) {
                IDictionary<String, PdfFormField> fieldsFrom = formFrom.GetFormFields();
                if (fieldsFrom.Count > 0) {
                    IDictionary<String, PdfFormField> fieldsTo = formTo.GetFormFields();
                    IList<PdfAnnotation> annots = toPage.GetAnnotations();
                    foreach (PdfAnnotation annot in annots) {
                        if (annot.GetSubtype().Equals(PdfName.Widget)) {
                            PdfDictionary parent = annot.GetPdfObject().GetAsDictionary(PdfName.Parent);
                            if (parent != null) {
                                PdfFormField parentField = GetParentField(parent, documentTo);
                                PdfString parentName = parentField.GetFieldName();
                                if (parentName == null) {
                                    continue;
                                }
                                if (!fieldsTo.ContainsKey(parentName.ToUnicodeString())) {
                                    PdfFormField field = CreateParentFieldCopy(annot.GetPdfObject(), documentTo);
                                    PdfArray kids = field.GetKids();
                                    field.GetPdfObject().Remove(PdfName.Kids);
                                    formTo.AddField(field, toPage);
                                    field.GetPdfObject().Put(PdfName.Kids, kids);
                                }
                                else {
                                    PdfFormField field = PdfFormField.MakeFormField(annot.GetPdfObject(), documentTo);
                                    PdfString fieldName = field.GetFieldName();
                                    if (fieldName != null) {
                                        PdfFormField existingField = fieldsTo.Get(fieldName.ToUnicodeString());
                                        if (existingField != null) {
                                            PdfFormField clonedField = PdfFormField.MakeFormField(field.GetPdfObject().Clone().MakeIndirect(documentTo
                                                ), documentTo);
                                            toPage.GetPdfObject().GetAsArray(PdfName.Annots).Add(clonedField.GetPdfObject());
                                            toPage.RemoveAnnotation(annot);
                                            MergeFieldsWithTheSameName(existingField, clonedField);
                                        }
                                        else {
                                            HashSet<String> existingFields = new HashSet<String>();
                                            GetAllFieldNames(formTo.GetFields(), existingFields);
                                            AddChildToExistingParent(annot.GetPdfObject(), existingFields);
                                        }
                                    }
                                    else {
                                        if (parentField.GetKids().Contains(field.GetPdfObject())) {
                                            field = PdfFormField.MakeFormField(field.GetPdfObject().Clone().MakeIndirect(documentTo), documentTo);
                                            toPage.GetPdfObject().GetAsArray(PdfName.Annots).Add(field.GetPdfObject());
                                            toPage.RemoveAnnotation(annot);
                                        }
                                        parentField.AddKid(field);
                                    }
                                }
                            }
                            else {
                                PdfString annotName = annot.GetPdfObject().GetAsString(PdfName.T);
                                String annotNameString = null;
                                if (annotName != null) {
                                    annotNameString = annotName.ToUnicodeString();
                                }
                                if (annotNameString != null && fieldsFrom.ContainsKey(annotNameString)) {
                                    PdfFormField field = fieldsTo.Get(annotNameString);
                                    if (field != null) {
                                        PdfDictionary clonedAnnot = (PdfDictionary)annot.GetPdfObject().Clone().MakeIndirect(documentTo);
                                        toPage.GetPdfObject().GetAsArray(PdfName.Annots).Add(clonedAnnot);
                                        toPage.RemoveAnnotation(annot);
                                        field = MergeFieldsWithTheSameName(field, PdfFormField.MakeFormField(clonedAnnot, toPage.GetDocument()));
                                        logger.Warn(String.Format(LogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, annotNameString));
                                        PdfArray kids = field.GetKids();
                                        field.GetPdfObject().Remove(PdfName.Kids);
                                        formTo.AddField(field, toPage);
                                        field.GetPdfObject().Put(PdfName.Kids, kids);
                                    }
                                    else {
                                        formTo.AddField(PdfFormField.MakeFormField(annot.GetPdfObject(), documentTo), null);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private PdfFormField MergeFieldsWithTheSameName(PdfFormField existingField, PdfFormField newField) {
            String fullFieldName = newField.GetFieldName().ToUnicodeString();
            PdfString fieldName = newField.GetPdfObject().GetAsString(PdfName.T);
            newField.GetPdfObject().Remove(PdfName.T);
            newField.GetPdfObject().Remove(PdfName.P);
            existingField = formTo.GetField(fullFieldName);
            PdfArray kids = existingField.GetKids();
            if (kids != null && !kids.IsEmpty()) {
                existingField.AddKid(newField);
                return existingField;
            }
            existingField.GetPdfObject().Remove(PdfName.T);
            existingField.GetPdfObject().Remove(PdfName.P);
            formTo.GetFields().Remove(existingField.GetPdfObject());
            PdfFormField mergedField = PdfFormField.CreateEmptyField(documentTo);
            mergedField.Put(PdfName.FT, existingField.GetFormType()).Put(PdfName.T, fieldName);
            PdfDictionary parent = existingField.GetParent();
            if (parent != null) {
                mergedField.Put(PdfName.Parent, parent);
                PdfArray parentKids = parent.GetAsArray(PdfName.Kids);
                for (int i = 0; i < parentKids.Size(); i++) {
                    PdfObject obj = parentKids.Get(i);
                    if (obj == existingField.GetPdfObject()) {
                        parentKids.Set(i, mergedField.GetPdfObject());
                        break;
                    }
                }
            }
            kids = existingField.GetKids();
            if (kids != null) {
                mergedField.Put(PdfName.Kids, kids);
            }
            mergedField.AddKid(existingField).AddKid(newField);
            mergedField.Put(PdfName.V, existingField.GetPdfObject().Get(PdfName.V));
            return mergedField;
        }

        private PdfFormField GetParentField(PdfDictionary parent, PdfDocument pdfDoc) {
            PdfFormField parentField = PdfFormField.MakeFormField(parent, pdfDoc);
            PdfDictionary parentOfParent = parentField.GetParent();
            if (parentOfParent != null) {
                parentField = GetParentField(parentOfParent, pdfDoc);
            }
            return parentField;
        }

        private PdfFormField CreateParentFieldCopy(PdfDictionary fieldDic, PdfDocument pdfDoc) {
            fieldDic.Remove(PdfName.Kids);
            PdfDictionary parent = fieldDic.GetAsDictionary(PdfName.Parent);
            PdfFormField field = PdfFormField.MakeFormField(fieldDic, pdfDoc);
            if (parent != null) {
                field = CreateParentFieldCopy(parent, pdfDoc);
                parent.Put(PdfName.Kids, new PdfArray(fieldDic));
            }
            return field;
        }

        private void AddChildToExistingParent(PdfDictionary fieldDic, ICollection<String> existingFields) {
            PdfDictionary parent = fieldDic.GetAsDictionary(PdfName.Parent);
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

        private void GetAllFieldNames(PdfArray fields, ICollection<String> existingFields) {
            foreach (PdfObject field in fields) {
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
