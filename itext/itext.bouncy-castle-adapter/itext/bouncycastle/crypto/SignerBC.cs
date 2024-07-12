/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using ISigner = iText.Commons.Bouncycastle.Crypto.ISigner;

namespace iText.Bouncycastle.Crypto {
    /// <summary>
    /// Wrapper class for <see cref="Org.BouncyCastle.Crypto.ISigner"/>.
    /// </summary>
    public class SignerBC : ISigner {
        private Org.BouncyCastle.Crypto.ISigner iSigner;
        
        private string lastHashAlgorithm;
        private string lastEncryptionAlgorithm;

        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.Crypto.ISigner"/>.
        /// </summary>
        /// <param name="iSigner">
        /// 
        /// <see cref="Org.BouncyCastle.Crypto.ISigner"/> to be wrapped
        /// </param>
        public SignerBC(Org.BouncyCastle.Crypto.ISigner iSigner) {
            this.iSigner = iSigner;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="Org.BouncyCastle.Crypto.ISigner"/>.
        /// </returns>
        public virtual Org.BouncyCastle.Crypto.ISigner GetISigner() {
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
            if (iSigner != null) {
                return;
            }
            // If trailerField == 1 then trailer = 0xBC (it's the default one)
            if (trailerField != 1) {
                throw new ArgumentException("unknown trailer field");
            }

            Org.BouncyCastle.Crypto.IDigest digest = Org.BouncyCastle.Security.DigestUtilities.GetDigest(digestAlgoName);
            Org.BouncyCastle.Crypto.ISigner signer = new Org.BouncyCastle.Crypto.Signers.PssSigner(
                new Org.BouncyCastle.Crypto.Engines.RsaBlindedEngine(), digest, digest, saltLen, 0xBC);

            this.iSigner = signer;
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf, int off, int len) {
            iSigner.BlockUpdate(buf, off, len);
        }
        
        /// <summary><inheritDoc/></summary>
        public void Update(byte[] digest) { 
            Update(digest, 0, digest.Length);
        }

        /// <summary><inheritDoc/></summary>
        public bool VerifySignature(byte[] digest) {
            return iSigner.VerifySignature(digest);
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GenerateSignature() {
            return iSigner.GenerateSignature();
        }

        /// <summary><inheritDoc/></summary>
        public void UpdateVerifier(byte[] digest) {
            Update(digest);
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
            SignerBC that = (SignerBC)o;
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

        private void InitVerify(IPublicKey publicKey, string hashAlgorithm, string encrAlgorithm) {
            if (iSigner == null) {
                if (string.IsNullOrEmpty(hashAlgorithm)) {
                    iSigner = SignerUtilities.GetSigner(encrAlgorithm);
                } else {
                    iSigner = SignerUtilities.GetSigner(hashAlgorithm + "with" + encrAlgorithm);
                }
            }

            iSigner.Init(false, ((PublicKeyBC) publicKey).GetPublicKey());
        }

        private void InitSign(IPrivateKey key, string hashAlgorithm, string encrAlgorithm) {
            if (iSigner == null) {
                if (string.IsNullOrEmpty(hashAlgorithm)) {
                    iSigner = SignerUtilities.GetSigner(encrAlgorithm);
                } else {
                    iSigner = SignerUtilities.GetSigner(hashAlgorithm + "with" + encrAlgorithm);
                }
            }

            iSigner.Init(true, ((PrivateKeyBC) key).GetPrivateKey());
        }
    }
}
