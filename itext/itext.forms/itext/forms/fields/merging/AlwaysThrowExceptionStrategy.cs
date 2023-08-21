using iText.Commons.Utils;
using iText.Forms.Exceptions;
using iText.Forms.Fields;
using iText.Kernel.Exceptions;

namespace iText.Forms.Fields.Merging {
    /// <summary>
    /// A
    /// <see cref="OnDuplicateFormFieldNameStrategy"/>
    /// implementation that throws an exception if the second field has the same
    /// name as the first field.
    /// </summary>
    public class AlwaysThrowExceptionStrategy : OnDuplicateFormFieldNameStrategy {
        /// <summary>
        /// Creates an instance of
        /// <see cref="AlwaysThrowExceptionStrategy"/>
        /// </summary>
        public AlwaysThrowExceptionStrategy() {
        }

        //Empty constructor
        /// <summary>executes the strategy.</summary>
        /// <param name="firstField">the first field</param>
        /// <param name="secondField">the second field</param>
        /// <param name="throwExceptionOnError">if true, an exception will be thrown</param>
        /// <returns>true if the second field was renamed successfully, false otherwise</returns>
        public virtual bool Execute(PdfFormField firstField, PdfFormField secondField, bool throwExceptionOnError) {
            throw new PdfException(MessageFormatUtil.Format(FormsExceptionMessageConstant.FIELD_NAME_ALREADY_EXISTS_IN_FORM
                , firstField.GetFieldName().ToUnicodeString()));
        }
    }
}
