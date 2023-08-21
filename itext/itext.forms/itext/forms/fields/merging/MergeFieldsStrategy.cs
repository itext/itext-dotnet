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
