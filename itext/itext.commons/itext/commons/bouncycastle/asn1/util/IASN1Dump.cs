using System;

namespace iText.Commons.Bouncycastle.Asn1.Util {
    public interface IASN1Dump {
        String DumpAsString(Object obj, bool b);

        String DumpAsString(Object obj);
    }
}
