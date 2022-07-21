using System;

namespace iText.Commons.Bouncycastle.Asn1 {
    public interface IASN1InputStream : IDisposable {
        IASN1Primitive ReadObject();

        void Close();
    }
}
