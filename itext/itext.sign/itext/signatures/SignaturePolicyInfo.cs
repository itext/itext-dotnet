using System;
using Org.BouncyCastle;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.X509;
using iText.IO.Codec;

namespace iText.Signatures {
    /// <summary>
    /// Class that encapsulates the signature policy information
    /// Sample:
    /// SignaturePolicyInfo spi = new SignaturePolicyInfo("2.16.724.1.3.1.1.2.1.9",
    /// "G7roucf600+f03r/o0bAOQ6WAs0=", "SHA-1", "https://sede.060.gob.es/politica_de_firma_anexo_1.pdf");
    /// </summary>
    public class SignaturePolicyInfo {
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
            : this(policyIdentifier, policyHashBase64 != null ? System.Convert.FromBase64String(policyHashBase64) : null
                , policyDigestAlgorithm, policyUri) {
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

        internal virtual SignaturePolicyIdentifier ToSignaturePolicyIdentifier() {
            String algId = DigestAlgorithms.GetAllowedDigest(this.policyDigestAlgorithm);
            if (algId == null || algId.Length == 0) {
                throw new ArgumentException("Invalid policy hash algorithm");
            }
            SignaturePolicyIdentifier signaturePolicyIdentifier = null;
            SigPolicyQualifierInfo spqi = null;
            if (this.policyUri != null && this.policyUri.Length > 0) {
                spqi = new SigPolicyQualifierInfo(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdSpqEtsUri, new DerIA5String
                    (this.policyUri));
            }
            signaturePolicyIdentifier = new SignaturePolicyIdentifier(new SignaturePolicyId(DerObjectIdentifier.GetInstance
                (new DerObjectIdentifier(this.policyIdentifier.Replace("urn:oid:", ""))), new OtherHashAlgAndValue(new 
                AlgorithmIdentifier(algId), new DerOctetString(this.policyHash)), spqi));
            return signaturePolicyIdentifier;
        }
    }
}
