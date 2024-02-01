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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;
using iText.Commons.Utils;

namespace iText.Signatures {
    /// <summary>Class that encapsulates the signature policy information</summary>
    /// <remarks>
    /// Class that encapsulates the signature policy information
    /// <para />
    /// Sample:
    /// <para />
    /// SignaturePolicyInfo spi = new SignaturePolicyInfo("2.16.724.1.3.1.1.2.1.9",
    /// "G7roucf600+f03r/o0bAOQ6WAs0=", "SHA-1", "https://sede.060.gob.es/politica_de_firma_anexo_1.pdf");
    /// </remarks>
    public class SignaturePolicyInfo {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private String policyIdentifier;

        private byte[] policyHash;

        private String policyDigestAlgorithm;

        private String policyUri;

        /// <summary>
        /// Constructs a new
        /// <see cref="SignaturePolicyInfo"/>
        /// instance
        /// </summary>
        /// <param name="policyIdentifier">the id of the signature policy</param>
        /// <param name="policyHash">the hash of the signature policy</param>
        /// <param name="policyDigestAlgorithm">the digestion algorithm of the signature policy</param>
        /// <param name="policyUri">the uri of the full policy description</param>
        public SignaturePolicyInfo(String policyIdentifier, byte[] policyHash, String policyDigestAlgorithm, String
             policyUri) {
            if (policyIdentifier == null || policyIdentifier.Length == 0) {
                throw new ArgumentException("Policy identifier cannot be null");
            }
            if (policyHash == null) {
                throw new ArgumentException("Policy hash cannot be null");
            }
            if (policyDigestAlgorithm == null || policyDigestAlgorithm.Length == 0) {
                throw new ArgumentException("Policy digest algorithm cannot be null");
            }
            this.policyIdentifier = policyIdentifier;
            this.policyHash = policyHash;
            this.policyDigestAlgorithm = policyDigestAlgorithm;
            this.policyUri = policyUri;
        }

        /// <summary>
        /// Constructs a new
        /// <see cref="SignaturePolicyInfo"/>
        /// instance
        /// </summary>
        /// <param name="policyIdentifier">the id of the signature policy</param>
        /// <param name="policyHashBase64">the Base64 presentation of the hash of the signature policy</param>
        /// <param name="policyDigestAlgorithm">the digestion algorithm of the signature policy</param>
        /// <param name="policyUri">the uri of the full policy description</param>
        public SignaturePolicyInfo(String policyIdentifier, String policyHashBase64, String policyDigestAlgorithm, 
            String policyUri)
            : this(policyIdentifier, policyHashBase64 != null ? Convert.FromBase64String(policyHashBase64) : null, policyDigestAlgorithm
                , policyUri) {
        }

        public virtual String GetPolicyIdentifier() {
            return policyIdentifier;
        }

        public virtual byte[] GetPolicyHash() {
            return policyHash;
        }

        public virtual String GetPolicyDigestAlgorithm() {
            return policyDigestAlgorithm;
        }

        public virtual String GetPolicyUri() {
            return policyUri;
        }

        internal virtual ISignaturePolicyIdentifier ToSignaturePolicyIdentifier() {
            String algId = DigestAlgorithms.GetAllowedDigest(this.policyDigestAlgorithm);
            if (algId == null || algId.Length == 0) {
                throw new ArgumentException("Invalid policy hash algorithm");
            }
            ISignaturePolicyIdentifier signaturePolicyIdentifier = null;
            ISigPolicyQualifierInfo spqi = null;
            if (this.policyUri != null && this.policyUri.Length > 0) {
                spqi = FACTORY.CreateSigPolicyQualifierInfo(FACTORY.CreatePKCSObjectIdentifiers().GetIdSpqEtsUri(), FACTORY
                    .CreateDERIA5String(this.policyUri));
            }
            IDerObjectIdentifier identifier = FACTORY.CreateASN1ObjectIdentifierInstance(FACTORY.CreateASN1ObjectIdentifier
                (this.policyIdentifier.Replace("urn:oid:", "")));
            IOtherHashAlgAndValue otherHashAlgAndValue = FACTORY.CreateOtherHashAlgAndValue(FACTORY.CreateAlgorithmIdentifier
                (FACTORY.CreateASN1ObjectIdentifier(algId)), FACTORY.CreateDEROctetString(this.policyHash));
            ISignaturePolicyId signaturePolicyId = FACTORY.CreateSignaturePolicyId(identifier, otherHashAlgAndValue, spqi
                );
            signaturePolicyIdentifier = FACTORY.CreateSignaturePolicyIdentifier(signaturePolicyId);
            return signaturePolicyIdentifier;
        }
    }
}
