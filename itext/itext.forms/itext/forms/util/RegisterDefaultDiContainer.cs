using iText.Commons.Utils;
using iText.Forms.Fields.Merging;

namespace iText.Forms.Util {
    public class RegisterDefaultDiContainer {
        public RegisterDefaultDiContainer() {
        }

        static RegisterDefaultDiContainer() {
            // Empty constructor but should be public as we need it for automatic class loading
            // sharp
            DIContainer.RegisterDefault(typeof(OnDuplicateFormFieldNameStrategy), () => new MergeFieldsStrategy());
        }
    }
}
