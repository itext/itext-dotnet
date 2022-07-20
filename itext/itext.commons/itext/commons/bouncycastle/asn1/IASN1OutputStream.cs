using System;

namespace iText.Commons.Bouncycastle.Asn1 {
    public interface IASN1OutputStream : IDisposable {
        void WriteObject(IASN1Primitive primitive);

        void Close();

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
