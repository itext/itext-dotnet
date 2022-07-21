namespace iText.Commons.Bouncycastle.Crypto
{
    public interface IIDigest {
        byte[] Digest(byte[] enc2);
        
        byte[] Digest();
        
        void Update(byte[] buf, int off, int len);
        
        void Update(byte[] buf);
    }
}