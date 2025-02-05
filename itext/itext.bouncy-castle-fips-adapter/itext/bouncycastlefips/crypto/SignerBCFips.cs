/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2025 Apryse Group NV
    Authors: Apryse Software.

    This program is offered under a commercial and under the AGPL license.
    For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

    AGPL licensing:
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
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
    public class SignerBCFips : ISigner {
        private IStreamCalculator<IVerifier> iSigner;
        
        private string lastHashAlgorithm;
        private string lastEncryptionAlgorithm;
        private string digestAlgoName;
        private int saltLen;

        private IStreamCalculator<IBlockResult> digest;

        /// <summary>
        /// Creates new wrapper instance for signer.
        /// </summary>
        /// <param name="iSigner">
        /// 
        /// IStreamCalculator<IVerifier> to be wrapped
        /// </param>
        public SignerBCFips(IStreamCalculator<IVerifier> iSigner) {
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
        public void InitRsaPssSigner(string digestAlgoName, int saltLen, int trailerField) {
            this.digestAlgoName = digestAlgoName;
            this.saltLen = saltLen;
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
            string[] splitAlgorithm = algorithm.Split(new string[] { "with" }, StringSplitOptions.None);
            if (splitAlgorithm.Length > 1) {
                lastHashAlgorithm = splitAlgorithm[0];
                lastEncryptionAlgorithm = splitAlgorithm[1];
            } else {
                lastHashAlgorithm = "";
                lastEncryptionAlgorithm = splitAlgorithm[0];
            }
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            SignerBCFips that = (SignerBCFips)o;
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
            FipsShs.Parameters parameters = digestAlgoName == null
                ? DigestBCFips.GetMessageDigestParams(hashAlgorithm)
                : DigestBCFips.GetMessageDigestParams(digestAlgoName);
            switch (encAlgorithm) {
                case "RSASSA-PSS": {
                    ISignatureFactory<FipsRsa.PssSignatureParameters> rsaSig = signatureFactoryProvider.CreateSignatureFactory(
                            FipsRsa.Pss.WithDigest(parameters).WithSaltLength(saltLen));
                    digest = rsaSig.CreateCalculator();
                    break;
                }
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
            FipsShs.Parameters parameters = digestAlgoName == null
                ? DigestBCFips.GetMessageDigestParams(hashAlgorithm)
                : DigestBCFips.GetMessageDigestParams(digestAlgoName);

            switch (encrAlgorithm) {
                case "RSASSA-PSS": {
                    IVerifierFactory<FipsRsa.PssSignatureParameters> rsaSig =
                        verifierFactoryProvider.CreateVerifierFactory(FipsRsa.Pss.WithDigest(parameters).WithSaltLength(saltLen));
                    iSigner = rsaSig.CreateCalculator();
                    break;
                }
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
