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
using iText.Forms.Exceptions;
using iText.Forms.Logs;
using iText.Kernel.Exceptions;
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
                        addedKids.Put(kidName, kid);
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
            PdfName firstFieldFormType = firstField.GetFormType();
            PdfObject firstFieldValue = firstField.GetValue();
            PdfObject firstFieldDefaultValue = firstField.GetDefaultValue();
            PdfObject secondFieldDefaultValue = secondField.GetDefaultValue();
            if ((firstFieldFormType == null || firstFieldFormType.Equals(secondField.GetFormType())) && (firstFieldValue
                 == null || firstFieldValue.Equals(secondField.GetValue())) && (firstFieldDefaultValue == null || secondFieldDefaultValue
                 == null || firstFieldDefaultValue.Equals(secondFieldDefaultValue))) {
                MergeFormFields(firstField, secondField, throwExceptionOnError);
            }
            else {
                if (throwExceptionOnError) {
                    throw new PdfException(MessageFormatUtil.Format(FormsExceptionMessageConstant.CANNOT_MERGE_FORMFIELDS, firstField
                        .GetPartialFieldName()));
                }
                else {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Fields.PdfFormFieldMergeUtil));
                    logger.LogWarning(MessageFormatUtil.Format(FormsLogMessageConstants.CANNOT_MERGE_FORMFIELDS, firstField.GetPartialFieldName
                        ()));
                    return false;
                }
            }
            return true;
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
                if (!PdfFormAnnotationUtil.IsPureWidgetOrMergedField(formDict) && field.GetChildFields().Count > 0 && field
                    .GetChildFormFields().Count == 0) {
                    bool shouldBeMerged = true;
                    // If parent is radio button we don't care about field related keys, always merge
                    // If not - go over all fields to compare with parent's fields
                    if (!(PdfName.Btn.Equals(parentField.GetFormType()) && parentField.GetFieldFlag(PdfButtonFormField.FF_RADIO
                        ))) {
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

        private static void MergeFormFields(PdfFormField firstField, PdfFormField secondField, bool throwExceptionOnError
            ) {
            PdfFormAnnotationUtil.SeparateWidgetAndField(firstField);
            PdfFormAnnotationUtil.SeparateWidgetAndField(secondField);
            PdfDictionary firstFieldDict = firstField.GetPdfObject();
            PdfDictionary secondFieldDict = secondField.GetPdfObject();
            // Sometimes we merge field with its merged widget annotation, so secondField's /Parent is firstField.
            // It can be a problem in case firstField is a root field, that's why secondField's /Parent is removed.
            secondFieldDict.Remove(PdfName.Parent);
            foreach (PdfName key in new List<PdfName>(secondFieldDict.KeySet())) {
                if (PdfName.Kids.Equals(key)) {
                    // Merge kids
                    foreach (AbstractPdfFormField kid in new List<AbstractPdfFormField>(secondField.GetChildFields())) {
                        firstField.AddKid(kid, throwExceptionOnError);
                    }
                }
                else {
                    if (!firstFieldDict.ContainsKey(key)) {
                        // Add all unique keys from the second field into the first field
                        firstField.Put(key, secondFieldDict.Get(key));
                    }
                }
            }
        }
        // Else values of the first dictionary will remain.
    }
}
