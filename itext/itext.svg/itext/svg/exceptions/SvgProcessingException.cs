using System;

namespace iText.Svg.Exceptions {
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
