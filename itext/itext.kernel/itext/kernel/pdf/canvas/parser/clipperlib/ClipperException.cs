using System;

namespace iText.Kernel.Pdf.Canvas.Parser.ClipperLib {
    public class ClipperException : Exception {
        /// <summary>Creates a new instance of ClipperException.</summary>
        /// <param name="message">the detail message.</param>
        public ClipperException(String message)
            : base(message) {
        }
    }
}
