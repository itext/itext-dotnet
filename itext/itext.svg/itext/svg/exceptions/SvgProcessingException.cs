using System;
using iText.Kernel;

namespace iText.Svg.Exceptions {
    /// <summary>
    /// Exception thrown by
    /// <see cref="iText.Svg.Processors.ISvgProcessor"/>
    /// when it cannot process an SVG
    /// </summary>
    public class SvgProcessingException : PdfException {
        /// <summary>
        /// Creates a new
        /// <see cref="SvgProcessingException"/>
        /// instance.
        /// </summary>
        /// <param name="message">the message</param>
        public SvgProcessingException(String message)
            : base(message) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="SvgProcessingException"/>
        /// instance.
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="cause">the nested exception</param>
        public SvgProcessingException(String message, Exception cause)
            : base(message, cause) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="SvgProcessingException"/>
        /// instance.
        /// </summary>
        /// <param name="cause">the nested exception</param>
        public SvgProcessingException(Exception cause)
            : base(cause) {
        }
    }
}
