using System;

namespace iText.Svg.Exceptions {
    /// <summary>
    /// Exception thrown by
    /// <see cref="iText.Svg.Processors.ISvgProcessor"/>
    /// when it cannot process an SVG
    /// </summary>
    public class SvgProcessingException : Exception {
        /// <summary>
        /// Creates a new
        /// <see cref="SvgProcessingException"/>
        /// instance.
        /// </summary>
        /// <param name="message">the message</param>
        public SvgProcessingException(String message)
            : base(message) {
        }
    }
}
