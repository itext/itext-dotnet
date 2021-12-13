using System;

namespace iText.Layout.Logs {
    /// <summary>Class containing constants to be used in layout.</summary>
    public sealed class LayoutLogMessageConstant {
        public const String ELEMENT_DOES_NOT_FIT_AREA = "Element does not fit current area. {0}";

        private LayoutLogMessageConstant() {
        }
        //Private constructor will prevent the instantiation of this class directly
    }
}
