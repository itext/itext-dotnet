using System;
using iText.Layout.Element;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Utility class for delegating the layout checks to the correct checking logic.</summary>
    public sealed class LayoutCheckUtil {
        /// <summary>
        /// Creates a new
        /// <see cref="LayoutCheckUtil"/>
        /// instance.
        /// </summary>
        private LayoutCheckUtil() {
        }

        // Empty constructor
        /// <summary>Checks if a layout element is valid against the PDF/UA specification.</summary>
        /// <param name="layoutElement">layout element to check</param>
        public static void CheckLayoutElements(Object layoutElement) {
            if (layoutElement is Image) {
                GraphicsCheckUtil.CheckLayoutImage((Image)layoutElement);
                return;
            }
        }
    }
}
