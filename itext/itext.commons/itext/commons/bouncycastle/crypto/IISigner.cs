using System;

namespace iText.Commons.Bouncycastle.Crypto
{
    public interface IISigner {
        void InitVerify(IPublicKey getPublicKey, String hashAlgorithm, String encrAlgorithm);
        
        void Update(byte[] buf, int off, int len);
        
        void Update(byte[] digest);
        
        bool VerifySignature(byte[] digest);
    }
}