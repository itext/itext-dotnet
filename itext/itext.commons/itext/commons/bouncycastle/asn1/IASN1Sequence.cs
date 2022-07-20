using System.Collections;

namespace iText.Commons.Bouncycastle.Asn1 {
    public interface IASN1Sequence : IASN1Primitive {
        IASN1Encodable GetObjectAt(int i);

        IEnumerator GetObjects();

        int Size();

        IASN1Encodable[] ToArray();
    }
}
