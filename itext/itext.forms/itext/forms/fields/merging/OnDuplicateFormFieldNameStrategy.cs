using iText.Forms.Fields;

namespace iText.Forms.Fields.Merging {
    /// <summary>
    /// A
    /// <see cref="OnDuplicateFormFieldNameStrategy"/>
    /// implementation that throws an exception if the second field has the same
    /// name as the first field.
    /// </summary>
    public interface OnDuplicateFormFieldNameStrategy {
        /// <summary>executes the strategy.</summary>
        /// <param name="firstField">the first field</param>
        /// <param name="secondField">the second field</param>
        /// <param name="throwExceptionOnError">if true, an exception will be thrown</param>
        /// <returns>true if the second field was renamed successfully, false otherwise</returns>
        bool Execute(PdfFormField firstField, PdfFormField secondField, bool throwExceptionOnError);
    }
}
