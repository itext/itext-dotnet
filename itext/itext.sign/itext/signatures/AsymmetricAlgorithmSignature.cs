using System;
using System.Security.Cryptography;

namespace iText.Signatures {
    public class AsymmetricAlgorithmSignature : IExternalSignature {
        private AsymmetricAlgorithm algorithm;
        /** The hash algorithm. */
        private String hashAlgorithm;
        /** The encryption algorithm (obtained from the private key) */
        private String encryptionAlgorithm;

        public AsymmetricAlgorithmSignature(RSACryptoServiceProvider algorithm, String hashAlgorithm)
            : this((AsymmetricAlgorithm) algorithm, hashAlgorithm) {
        }

        public AsymmetricAlgorithmSignature(DSACryptoServiceProvider algorithm)
            : this((AsymmetricAlgorithm) algorithm, null) {
        }

        private AsymmetricAlgorithmSignature(AsymmetricAlgorithm algorithm, String hashAlgorithm) {
            this.algorithm = algorithm;
            this.hashAlgorithm = DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigest(hashAlgorithm));

            if (algorithm is RSACryptoServiceProvider)
                encryptionAlgorithm = "RSA";
            else if (algorithm is DSACryptoServiceProvider)
                encryptionAlgorithm = "DSA";
            else
                throw new ArgumentException("Not supported encryption algorithm " + algorithm);
        }
        
        public byte[] Sign(byte[] message) {
            if (algorithm is RSACryptoServiceProvider) {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider) algorithm;
                return rsa.SignData(message, hashAlgorithm);
            } else {
                DSACryptoServiceProvider dsa = (DSACryptoServiceProvider) algorithm;
                return dsa.SignData(message);
            }
        }

        public string GetHashAlgorithm() {
            return hashAlgorithm;
        }

        public string GetEncryptionAlgorithm() {
            return encryptionAlgorithm;
        }
    }
}
