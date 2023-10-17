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
using iText.Commons.Utils;
using iText.Forms.Fields.Merging;
using iText.Kernel.Pdf;

namespace iText.Forms.Fields {
    /// <summary>
    /// Utility class to merge form fields
    /// <see cref="PdfFormField"/>
    /// with the same names.
    /// </summary>
    public sealed class PdfFormFieldMergeUtil {
        private PdfFormFieldMergeUtil() {
        }

        // Private constructor will prevent the instantiation of this class directly.
        /// <summary>This method merges all kids with the same names for the given parent field dictionary (recursively).
        ///     </summary>
        /// <param name="parentField">a field whose kids should be checked and merged in case of same partial names.</param>
        /// <param name="throwExceptionOnError">true if the exception is expected after the merge failed, false if log is expected.
        ///     </param>
        public static void MergeKidsWithSameNames(PdfFormField parentField, bool throwExceptionOnError) {
            PdfDictionary parent = parentField.GetPdfObject();
            if (parentField.IsInReadingMode()) {
                // Do nothing in reading mode.
                return;
            }
            PdfArray kids = parent.GetAsArray(PdfName.Kids);
            if (kids == null || kids.Size() == 0) {
                return;
            }
            IDictionary<String, AbstractPdfFormField> addedKids = new LinkedDictionary<String, AbstractPdfFormField>();
            IList<AbstractPdfFormField> newKids = new List<AbstractPdfFormField>();
            foreach (AbstractPdfFormField kid in parentField.GetChildFields()) {
                if (kid is PdfFormField) {
                    // Try to merge for the kid
                    MergeKidsWithSameNames((PdfFormField)kid, throwExceptionOnError);
                    String kidName = GetPartialName(kid);
                    if (!addedKids.ContainsKey(kidName) || !MergeTwoFieldsWithTheSameNames((PdfFormField)addedKids.Get(kidName
                        ), (PdfFormField)kid, throwExceptionOnError)) {
                        addedKids.Put(GetPartialName(kid), kid);
                        newKids.Add(kid);
                    }
                }
                else {
                    // It's a pure widget
                    newKids.Add(kid);
                }
            }
            parentField.ReplaceKids(newKids);
            ProcessDirtyAnnotations(parentField, throwExceptionOnError);
        }

        /// <summary>This method merges different values from two field dictionaries into the first one and combines kids.
        ///     </summary>
        /// <param name="firstField">a field into which dictionary all values will be merged.</param>
        /// <param name="secondField">a field whose values should be merged into the first dictionary.</param>
        /// <param name="throwExceptionOnError">true if the exception is expected after the merge failed, false if log is expected.
        ///     </param>
        /// <returns>true if fields is successfully merged, false otherwise.</returns>
        public static bool MergeTwoFieldsWithTheSameNames(PdfFormField firstField, PdfFormField secondField, bool 
            throwExceptionOnError) {
            OnDuplicateFormFieldNameStrategy onDuplicateFormFieldNameStrategy = firstField.GetDocument().GetDiContainer
                ().GetInstance<OnDuplicateFormFieldNameStrategy>();
            return onDuplicateFormFieldNameStrategy.Execute(firstField, secondField, throwExceptionOnError);
        }

        /// <summary>Gets partial name for the field dictionary.</summary>
        /// <param name="field">field to get name from.</param>
        /// <returns>
        /// field partial name. Also, null if passed dictionary is a pure widget,
        /// empty string in case it is a field with no /T entry.
        /// </returns>
        public static String GetPartialName(AbstractPdfFormField field) {
            if (PdfFormAnnotationUtil.IsPureWidget(field.GetPdfObject())) {
                return null;
            }
            if (field is PdfFormField) {
                return ((PdfFormField)field).GetPartialFieldName().ToUnicodeString();
            }
            return "";
        }

        /// <summary>Sometimes widgets contain field related keys, and they are the same as these field keys at parent.
        ///     </summary>
        /// <remarks>
        /// Sometimes widgets contain field related keys, and they are the same as these field keys at parent.
        /// During merge process we get something like: ParentField
        /// <c>(/DA &lt;DA1&gt; /Ft &lt;Tx&gt; /T &lt;test&gt; /Kids &lt;Field&gt;) -&gt;</c>
        /// Field
        /// <c>(/DA &lt;DA1&gt; /Kids &lt;Annotation&gt;) -&gt;</c>
        /// Annotation (without any form fields)
        /// <para />
        /// This method combines ParentField with Field.
        /// </remarks>
        /// <param name="parentField">
        /// a field whose form field kids should be checked and merged with parent in case
        /// all their dictionary values (except Parent and Kids) are the same
        /// or parent is a radio button.
        /// </param>
        /// <param name="throwExceptionOnError">true if the exception is expected after the merge failed, false if log is expected.
        ///     </param>
        public static void ProcessDirtyAnnotations(PdfFormField parentField, bool throwExceptionOnError) {
            foreach (PdfFormField field in parentField.GetChildFormFields()) {
                PdfDictionary formDict = field.GetPdfObject();
                // Process form fields without PdfName.Widget having only annotations as children
                if (field.GetChildFields().Count > 0 && field.GetChildFormFields().Count == 0) {
                    bool shouldBeMerged = true;
                    // If parent is radio button or signature we don't care about field related keys, always merge
                    // If not - go over all fields to compare with parent's fields
                    if (!(PdfName.Btn.Equals(parentField.GetFormType()) && parentField.GetFieldFlag(PdfButtonFormField.FF_RADIO
                        )) && !PdfName.Sig.Equals(parentField.GetFormType())) {
                        if (formDict.ContainsKey(PdfName.T)) {
                            // We only want to perform the merge if field doesn't contain any name (even empty one)
                            continue;
                        }
                        foreach (PdfName key in formDict.KeySet()) {
                            // Everything except Parent and Kids must be identical to allow the merge
                            if (!PdfName.Parent.Equals(key) && !PdfName.Kids.Equals(key) && !formDict.Get(key).Equals(parentField.GetPdfObject
                                ().Get(key))) {
                                shouldBeMerged = false;
                                break;
                            }
                        }
                    }
                    if (shouldBeMerged) {
                        parentField.RemoveChild(field);
                        formDict.Remove(PdfName.Parent);
                        // We know for sure that parentField and field must be merged here
                        MergeFormFields(parentField, field, throwExceptionOnError);
                    }
                }
            }
        }

        public static void MergeFormFields(PdfFormField firstField, PdfFormField secondField, bool throwExceptionOnError
            ) {
            PdfFormAnnotationUtil.SeparateWidgetAndField(firstField);
            PdfFormAnnotationUtil.SeparateWidgetAndField(secondField);
            PdfDictionary firstFieldDict = firstField.GetPdfObject();
            PdfDictionary secondFieldDict = secondField.GetPdfObject();
            foreach (PdfName key in new List<PdfName>(secondFieldDict.KeySet())) {
                if (PdfName.Kids.Equals(key)) {
                    // Merge kids
                    foreach (AbstractPdfFormField kid in new List<AbstractPdfFormField>(secondField.GetChildFields())) {
                        firstField.AddKid(kid, throwExceptionOnError);
                    }
                }
                else {
                    if (PdfName.Parent.Equals(key)) {
                    }
                    else {
                        // Never copy parent
                        if (!firstFieldDict.ContainsKey(key)) {
                            // Add all unique keys from the second field into the first field
                            firstField.Put(key, secondFieldDict.Get(key));
                        }
                    }
                }
            }
        }
        // Else values of the first dictionary will remain.
    }
}
