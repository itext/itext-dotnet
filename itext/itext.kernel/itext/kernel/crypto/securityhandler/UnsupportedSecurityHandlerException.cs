using System;
using iText.Kernel;

namespace iText.Kernel.Crypto.Securityhandler {
    public class UnsupportedSecurityHandlerException : PdfException {
        public const String UnsupportedSecurityHandler = "Failed to open the document. Security handler {0} is not supported";

        public UnsupportedSecurityHandlerException(String message)
            : base(message) {
        }
    }
}
