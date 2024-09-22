using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Mac {
//\cond DO_NOT_DOCUMENT
    internal class MacStandaloneContainerReader : MacContainerReader {
//\cond DO_NOT_DOCUMENT
        internal MacStandaloneContainerReader(PdfDictionary authDictionary)
            : base(authDictionary) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override byte[] ParseSignature(PdfDictionary authDictionary) {
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override long[] ParseByteRange(PdfDictionary authDictionary) {
            return authDictionary.GetAsArray(PdfName.ByteRange).ToLongArray();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override byte[] ParseMacContainer(PdfDictionary authDictionary) {
            if (authDictionary.GetAsString(PdfName.MAC) == null) {
                throw new PdfException(KernelExceptionMessageConstant.MAC_NOT_SPECIFIED);
            }
            return authDictionary.GetAsString(PdfName.MAC).GetValueBytes();
        }
//\endcond
    }
//\endcond
}
