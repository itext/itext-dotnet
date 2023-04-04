namespace iText.Commons.Utils {
    /// <summary>Experimental features class which contains constants related to experimental form fields drawing.
    ///     </summary>
    public sealed class ExperimentalFeatures {
        // TODO Shall be removed in the scope of DEVSIX-7385
        /// <summary>Determines, whether the old or the new checkbox form field drawing logic will be used.</summary>
        public static bool ENABLE_EXPERIMENTAL_CHECKBOX_RENDERING = false;

        /// <summary>Determines, whether the old or the new text form field drawing logic will be used.</summary>
        public static bool ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = false;

        private ExperimentalFeatures() {
        }
        // utility class
    }
}
