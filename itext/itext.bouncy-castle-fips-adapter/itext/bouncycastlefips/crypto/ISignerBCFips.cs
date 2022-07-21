using System;
using System.IO;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Asymmetric;
using Org.BouncyCastle.Crypto.Fips;

namespace iText.Bouncycastlefips.Crypto {
    public class ISignerBCFips: IISigner {
        private IStreamCalculator<IVerifier> iSigner;

        public ISignerBCFips(IStreamCalculator<IVerifier> iSigner) {
            this.iSigner = iSigner;
        }

        public virtual IStreamCalculator<IVerifier> GetISigner() {
            return iSigner;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            ISignerBCFips that = (ISignerBCFips)o;
            return Object.Equals(iSigner, that.iSigner);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(iSigner);
        }

        public override String ToString() {
            return iSigner.ToString();
        }

        public void InitVerify(IPublicKey publicKey, String hashAlgorithm, String encrAlgorithm) {
            InitVerifySignature((AsymmetricRsaPublicKey) ((PublicKeyBCFips)publicKey).GetPublicKey(), 
                hashAlgorithm, encrAlgorithm);
        }

        public void Update(byte[] buf, int off, int len) {
            using (Stream sigStream = iSigner.Stream) {
                sigStream.Write(buf, off, len);
            }
        }

        public void Update(byte[] digest) { 
            Update(digest, 0, digest.Length);
        }

        public bool VerifySignature(byte[] digest) {
            return iSigner.GetResult().IsVerified(digest);
        }

        private void InitVerifySignature(AsymmetricRsaPublicKey key, String hashAlgorithm, String encrAlgorithm) {
            IVerifierFactoryService verifierFactoryProvider = CryptoServicesRegistrar.CreateService(key);
            FipsShs.Parameters parameters;
            switch (hashAlgorithm) {
                case "SHA-256": {
                    parameters = FipsShs.Sha256;
                    break;
                }
                case "SHA-512": {
                    parameters = FipsShs.Sha512;
                    break;
                }
                case "SHA-1": {
                    parameters = FipsShs.Sha1;
                    break;
                }
                default: {
                    throw new ArgumentException("Hash algorithm " + hashAlgorithm + "is not supported");
                }
            }
            
            switch (encrAlgorithm)
            {
                case "RSA":
                {
                    IVerifierFactory<FipsRsa.SignatureParameters> rsaSig =
                        verifierFactoryProvider.CreateVerifierFactory((
                            FipsRsa.Pkcs1v15.WithDigest(parameters)));
                    iSigner = rsaSig.CreateCalculator();
                    break;
                }
                case "DSA":
                {
                    IVerifierFactory<FipsEC.SignatureParameters> rsaSig =
                        verifierFactoryProvider.CreateVerifierFactory((
                            FipsEC.Dsa.WithDigest(parameters)));
                    iSigner = rsaSig.CreateCalculator();
                    break;
                }
            }
        }
    }
}