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
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;

namespace iText.Signatures {
    /// <summary>
    /// Implementation of the
    /// <see cref="IExternalSignature"/>
    /// interface that
    /// can be used when you have a
    /// <see cref="iText.Commons.Bouncycastle.Crypto.IPrivateKey"/>
    /// object.
    /// </summary>
    public class PrivateKeySignature : IExternalSignature {
        /// <summary>The private key object.</summary>
        private readonly IPrivateKey pk;

        /// <summary>The hash algorithm.</summary>
        private readonly String hashAlgorithm;

        /// <summary>The encryption algorithm (obtained from the private key)</summary>
        private readonly String signatureAlgorithm;

        /// <summary>The algorithm parameters.</summary>
        private readonly IApplicableSignatureParams parameters;

        /// <summary>
        /// Creates a
        /// <see cref="PrivateKeySignature"/>
        /// instance.
        /// </summary>
        /// <param name="pk">
        /// A
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPrivateKey"/>
        /// object.
        /// </param>
        /// <param name="hashAlgorithm">A hash algorithm (e.g. "SHA-1", "SHA-256",...).</param>
        /// <param name="provider">A security provider (e.g. "BC").</param>
        public PrivateKeySignature(IPrivateKey pk, String hashAlgorithm)
            : this(pk, hashAlgorithm, null, null) {
        }

        /// <summary>
        /// Creates a
        /// <see cref="PrivateKeySignature"/>
        /// instance.
        /// </summary>
        /// <param name="pk">
        /// A
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPrivateKey"/>
        /// object.
        /// </param>
        /// <param name="hashAlgorithm">A hash algorithm (e.g. "SHA-1", "SHA-256",...).</param>
        /// <param name="signatureAlgorithm">
        /// A signiture algorithm (e.g. "RSASSA-PSS", "id-signedData",
        /// "sha256WithRSAEncryption", ...)
        /// </param>
        /// <param name="provider">A security provider (e.g. "BC").</param>
        /// <param name="params">Parameters for using RSASSA-PSS or other algorithms requiring them.</param>
        public PrivateKeySignature(IPrivateKey pk, String hashAlgorithm, String signatureAlgorithm, IApplicableSignatureParams
             @params) {
            this.pk = pk;
            String digestAlgorithmOid = DigestAlgorithms.GetAllowedDigest(hashAlgorithm);
            this.hashAlgorithm = DigestAlgorithms.GetDigest(digestAlgorithmOid);
            String adjustedSignatureAlgorithm = signatureAlgorithm == null ? SignUtils.GetPrivateKeyAlgorithm(pk) : signatureAlgorithm;
            if ("RSA/PSS".Equals(adjustedSignatureAlgorithm)) {
                this.signatureAlgorithm = "RSASSA-PSS";
            }
            else {
                this.signatureAlgorithm = adjustedSignatureAlgorithm;
            }
            switch (this.signatureAlgorithm) {
                case "Ed25519": {
                    if (!OID.SHA_512.Equals(digestAlgorithmOid)) {
                        throw new PdfException(SignExceptionMessageConstant.ALGO_REQUIRES_SPECIFIC_HASH).SetMessageParams("Ed25519"
                            , "SHA-512", this.hashAlgorithm);
                    }
                    this.parameters = null;
                    break;
                }

                case "Ed448": {
                    if (!OID.SHAKE_256.Equals(digestAlgorithmOid)) {
                        throw new PdfException(SignExceptionMessageConstant.ALGO_REQUIRES_SPECIFIC_HASH).SetMessageParams("Ed448", 
                            "512-bit SHAKE256", this.hashAlgorithm);
                    }
                    this.parameters = null;
                    break;
                }

                case "EdDSA": {
                    throw new ArgumentException("Key algorithm of EdDSA PrivateKey instance provided by " + pk.GetType() + " is not clear. Expected Ed25519 or Ed448, but got EdDSA. "
                         + "Try a different security provider.");
                }

                case "RSASSA-PSS": {
                    if (@params != null && !(@params is RSASSAPSSMechanismParams)) {
                        throw new ArgumentException("Expected RSASSA-PSS parameters; got " + @params);
                    }
                    if (@params == null) {
                        this.parameters = RSASSAPSSMechanismParams.CreateForDigestAlgorithm(hashAlgorithm);
                    }
                    else {
                        this.parameters = @params;
                    }
                    break;
                }

                default: {
                    this.parameters = null;
                    break;
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetDigestAlgorithmName() {
            return hashAlgorithm;
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetSignatureAlgorithmName() {
            return signatureAlgorithm;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignatureMechanismParams GetSignatureMechanismParameters() {
            return parameters;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] Sign(byte[] message) {
            String algorithm = GetSignatureMechanismName();
            ISigner sig;
            try {
                sig = SignUtils.GetSignatureHelper(algorithm);
                if (parameters != null) {
                    parameters.Apply(sig);
                }
                sig.InitSign(pk);
                sig.Update(message);
                return sig.GenerateSignature();
            }
            catch (Exception) {
                try {
                    sig = SignUtils.GetSignatureHelper(GetSignatureAlgorithmName());
                    if (parameters != null) {
                        parameters.Apply(sig);
                    }
                    sig.InitSign(pk);
                    sig.Update(message);
                    return sig.GenerateSignature();
                }
                catch (Exception e) {
                    throw new PdfException(MessageFormatUtil.Format(SignExceptionMessageConstant.ALGORITHMS_NOT_SUPPORTED, algorithm
                        , GetSignatureAlgorithmName()), e);
                }
            }
        }

        private String GetSignatureMechanismName() {
            String signatureAlgo = this.GetSignatureAlgorithmName();
            // RSASSA-PSS is parameterised
            if ("RSASSA-PSS".Equals(signatureAlgo)) {
                return signatureAlgo;
            }
            return GetDigestAlgorithmName() + "with" + GetSignatureAlgorithmName();
        }
    }
}
