using System;
using System.IO;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Fips;
using Org.BouncyCastle.Security;

namespace iText.Bouncycastlefips.Crypto {
    /// <summary>
    /// Wrapper class for IStreamCalculator<IVerifier> signer.
    /// </summary>
    public class ISignerBCFips : IISigner {
        private IStreamCalculator<IVerifier> iSigner;
        
        private string lastHashAlgorithm;
        private string lastEncryptionAlgorithm;

        private IStreamCalculator<IBlockResult> digest;

        /// <summary>
        /// Creates new wrapper instance for signer.
        /// </summary>
        /// <param name="iSigner">
        /// 
        /// IStreamCalculator<IVerifier> to be wrapped
        /// </param>
        public ISignerBCFips(IStreamCalculator<IVerifier> iSigner) {
            this.iSigner = iSigner;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped IStreamCalculator<IVerifier>.
        /// </returns>
        public virtual IStreamCalculator<IVerifier> GetISigner() {
            return iSigner;
        }

        /// <summary><inheritDoc/></summary>
        public void InitVerify(IPublicKey publicKey) {
            InitVerify(publicKey, lastHashAlgorithm, lastEncryptionAlgorithm);
        }

        /// <summary><inheritDoc/></summary>
        public void InitSign(IPrivateKey key) {
            InitSign(key, lastHashAlgorithm, lastEncryptionAlgorithm);
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf, int off, int len) {
            if (digest != null) {
                using (Stream digStream = digest.Stream) {
                    digStream.Write(buf, off, len);
                }
            } else {
                using (Stream sigStream = iSigner.Stream) {
                    sigStream.Write(buf, off, len);
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] dig) { 
            Update(dig, 0, dig.Length);
        }

        /// <summary><inheritDoc/></summary>
        public bool VerifySignature(byte[] dig) {
            return iSigner.GetResult().IsVerified(dig);
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GenerateSignature() {
            return digest.GetResult().Collect();
        }

        /// <summary><inheritDoc/></summary>
        public void UpdateVerifier(byte[] buf) {
            using (Stream sigStream = iSigner.Stream) {
                sigStream.Write(buf, 0, buf.Length);
            }
        }

        /// <summary><inheritDoc/></summary>
        public void SetDigestAlgorithm(string algorithm) {
            lastHashAlgorithm = algorithm.Split(new string[] { "with" }, StringSplitOptions.None)[0];
            lastEncryptionAlgorithm = algorithm.Split(new string[] { "with" }, StringSplitOptions.None)[1];
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
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

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(iSigner);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return iSigner.ToString();
        }

        private void InitVerify(IPublicKey publicKey, String hashAlgorithm, String encrAlgorithm) {
            InitVerifySignature(((PublicKeyBCFips)publicKey).GetPublicKey(), hashAlgorithm, encrAlgorithm);
        }

        private void InitSign(IPrivateKey key, string hashAlgorithm, string encrAlgorithm) {
            InitSignature(((PrivateKeyBCFips) key).GetPrivateKey(), hashAlgorithm, encrAlgorithm);
        }

        private void InitSignature(IAsymmetricKey key, string hashAlgorithm, string encAlgorithm) {
            ISignatureFactoryService signatureFactoryProvider =
                CryptoServicesRegistrar.CreateService((ICryptoServiceType<ISignatureFactoryService>)key, new SecureRandom());
            FipsShs.Parameters parameters = IDigestBCFips.GetMessageDigestParams(hashAlgorithm);
            switch (encAlgorithm) {
                case "RSA": {
                    ISignatureFactory<FipsRsa.SignatureParameters> rsaSig =
                        signatureFactoryProvider.CreateSignatureFactory(
                            FipsRsa.Pkcs1v15.WithDigest(parameters));
                    digest = rsaSig.CreateCalculator();
                    break;
                }
                case "DSA": {
                    ISignatureFactory<FipsDsa.SignatureParameters> rsaSig =
                        signatureFactoryProvider.CreateSignatureFactory(
                            FipsDsa.Dsa.WithDigest(parameters));
                    digest = rsaSig.CreateCalculator();
                    break;
                }
                case "ECDSA": {
                    ISignatureFactory<FipsEC.SignatureParameters> rsaSig =
                        signatureFactoryProvider.CreateSignatureFactory(
                            FipsEC.Dsa.WithDigest(parameters));
                    digest = rsaSig.CreateCalculator();
                    break;
                }
            }
        }

        private void InitVerifySignature(IAsymmetricKey key, String hashAlgorithm, String encrAlgorithm) {
            IVerifierFactoryService verifierFactoryProvider =
                CryptoServicesRegistrar.CreateService((ICryptoServiceType<IVerifierFactoryService>)key);
            FipsShs.Parameters parameters = IDigestBCFips.GetMessageDigestParams(hashAlgorithm);

            switch (encrAlgorithm) {
                case "RSA": {
                    IVerifierFactory<FipsRsa.SignatureParameters> rsaSig =
                        verifierFactoryProvider.CreateVerifierFactory(FipsRsa.Pkcs1v15.WithDigest(parameters));
                    iSigner = rsaSig.CreateCalculator();
                    break;
                }
                case "DSA": {
                    IVerifierFactory<FipsDsa.SignatureParameters> rsaSig =
                        verifierFactoryProvider.CreateVerifierFactory(FipsDsa.Dsa.WithDigest(parameters));
                    iSigner = rsaSig.CreateCalculator();
                    break;
                }
                case "ECDSA":
                case "EC": {
                    IVerifierFactory<FipsEC.SignatureParameters> rsaSig =
                        verifierFactoryProvider.CreateVerifierFactory(FipsEC.Dsa.WithDigest(parameters));
                    iSigner = rsaSig.CreateCalculator();
                    break;
                }
            }
        }
    }
}