namespace iText.Commons.Bouncycastle.Asn1 {
    public interface IASN1Encodable {
        IASN1Primitive ToASN1Primitive();

        bool IsNull();
    }
}
