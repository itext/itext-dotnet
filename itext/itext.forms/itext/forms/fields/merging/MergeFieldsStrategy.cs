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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms.Exceptions;
using iText.Forms.Fields;
using iText.Forms.Logs;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Forms.Fields.Merging {
    /// <summary>
    /// A
    /// <see cref="OnDuplicateFormFieldNameStrategy"/>
    /// implementation that merges the second field into the first field if the
    /// second field has the same name as the first field.
    /// </summary>
    /// <remarks>
    /// A
    /// <see cref="OnDuplicateFormFieldNameStrategy"/>
    /// implementation that merges the second field into the first field if the
    /// second field has the same name as the first field.
    /// This strategy is used by default.
    /// </remarks>
    public class MergeFieldsStrategy : OnDuplicateFormFieldNameStrategy {
        /// <summary>
        /// Creates an instance of
        /// <see cref="MergeFieldsStrategy"/>
        /// </summary>
        public MergeFieldsStrategy() {
        }

        // Empty constructor
        /// <summary>executes the strategy.</summary>
        /// <param name="firstField">the first field</param>
        /// <param name="secondField">the second field</param>
        /// <param name="throwExceptionOnError">if true, an exception will be thrown</param>
        /// <returns>true if the second field was merged successfully, false otherwise</returns>
        public virtual bool Execute(PdfFormField firstField, PdfFormField secondField, bool throwExceptionOnError) {
            PdfName firstFieldFormType = firstField.GetFormType();
            PdfObject firstFieldValue = firstField.GetValue();
            PdfObject secondFieldValue = secondField.GetValue();
            PdfObject firstFieldDefaultValue = firstField.GetDefaultValue();
            PdfObject secondFieldDefaultValue = secondField.GetDefaultValue();
            if ((firstFieldFormType == null || firstFieldFormType.Equals(secondField.GetFormType())) && (firstFieldValue
                 == null || secondFieldValue == null || firstFieldValue.Equals(secondFieldValue)) && (firstFieldDefaultValue
                 == null || secondFieldDefaultValue == null || firstFieldDefaultValue.Equals(secondFieldDefaultValue))
                ) {
                PdfFormFieldMergeUtil.MergeFormFields(firstField, secondField, throwExceptionOnError);
            }
            else {
                if (throwExceptionOnError) {
                    throw new PdfException(MessageFormatUtil.Format(FormsExceptionMessageConstant.CANNOT_MERGE_FORMFIELDS, firstField
                        .GetPartialFieldName()));
                }
                else {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Fields.Merging.MergeFieldsStrategy));
                    logger.LogWarning(MessageFormatUtil.Format(FormsLogMessageConstants.CANNOT_MERGE_FORMFIELDS, firstField.GetPartialFieldName
                        ()));
                    return false;
                }
            }
            return true;
        }
    }
}
