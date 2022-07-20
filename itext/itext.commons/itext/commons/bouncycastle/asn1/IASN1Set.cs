using System.Collections;

namespace iText.Commons.Bouncycastle.Asn1 {
    public interface IASN1Set : IASN1Primitive {
        IEnumerator GetObjects();

        int Size();

        IASN1Encodable GetObjectAt(int index);

        IASN1Encodable[] ToArray();
    }
}
