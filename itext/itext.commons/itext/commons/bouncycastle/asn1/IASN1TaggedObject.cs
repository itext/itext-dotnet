namespace iText.Commons.Bouncycastle.Asn1 {
    public interface IASN1TaggedObject : IASN1Primitive {
        IASN1Primitive GetObject();

        int GetTagNo();
    }
}
