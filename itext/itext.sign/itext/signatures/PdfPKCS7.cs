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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.Esf;
using iText.Commons.Bouncycastle.Asn1.Ess;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.Pkcs;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Exceptions;

namespace iText.Signatures {
    /// <summary>
    /// This class does all the processing related to signing
    /// and verifying a PKCS#7 / CMS signature.
    /// </summary>
    public class PdfPKCS7 {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private ISignaturePolicyIdentifier signaturePolicyIdentifier;

        // Encryption provider
        // Signature info
        /// <summary>Holds value of property signName.</summary>
        private String signName;

        /// <summary>Holds value of property reason.</summary>
        private String reason;

        /// <summary>Holds value of property location.</summary>
        private String location;

        /// <summary>Holds value of property signDate.</summary>
        private DateTime signDate = (DateTime)TimestampConstants.UNDEFINED_TIMESTAMP_DATE;

        /// <summary>Collection to store revocation info other than OCSP and CRL responses, e.g. SCVP Request and Response.
        ///     </summary>
        private readonly ICollection<IAsn1Sequence> signedDataRevocationInfo = new List<IAsn1Sequence>();

        // Constructors for creating new signatures
        /// <summary>Assembles all the elements needed to create a signature, except for the data.</summary>
        /// <param name="privKey">the private key</param>
        /// <param name="certChain">the certificate chain</param>
        /// <param name="interfaceDigest">the interface digest</param>
        /// <param name="hashAlgorithm">the hash algorithm</param>
        /// <param name="provider">the provider or <c>null</c> for the default provider</param>
        /// <param name="hasEncapContent"><c>true</c> if the sub-filter is adbe.pkcs7.sha1</param>
        public PdfPKCS7(IPrivateKey privKey, IX509Certificate[] certChain, String hashAlgorithm, bool hasEncapContent
            ) {
            // message digest
            digestAlgorithmOid = DigestAlgorithms.GetAllowedDigest(hashAlgorithm);
            if (digestAlgorithmOid == null) {
                throw new PdfException(SignExceptionMessageConstant.UNKNOWN_HASH_ALGORITHM).SetMessageParams(hashAlgorithm
                    );
            }
            // Copy the certificates
            signCert = (IX509Certificate)certChain[0];
            certs = new List<IX509Certificate>();
            certs.AddAll(certChain);
            // initialize and add the digest algorithms.
            digestalgos = new HashSet<String>();
            digestalgos.Add(digestAlgorithmOid);
            // find the signing algorithm
            if (privKey != null) {
                String signatureAlgo = SignUtils.GetPrivateKeyAlgorithm(privKey);
                String mechanismOid = SignatureMechanisms.GetSignatureMechanismOid(signatureAlgo, hashAlgorithm);
                if (mechanismOid == null) {
                    throw new PdfException(SignExceptionMessageConstant.COULD_NOT_DETERMINE_SIGNATURE_MECHANISM_OID).SetMessageParams
                        (signatureAlgo, hashAlgorithm);
                }
                this.signatureMechanismOid = mechanismOid;
            }
            // initialize the encapsulated content
            if (hasEncapContent) {
                encapMessageContent = new byte[0];
                messageDigest = DigestAlgorithms.GetMessageDigest(GetDigestAlgorithmName());
            }
            // initialize the Signature object
            if (privKey != null) {
                sig = InitSignature(privKey);
            }
        }

        // Constructors for validating existing signatures
        /// <summary>Use this constructor if you want to verify a signature using the sub-filter adbe.x509.rsa_sha1.</summary>
        /// <param name="contentsKey">the /Contents key</param>
        /// <param name="certsKey">the /Cert key</param>
        /// <param name="provider">the provider or <c>null</c> for the default provider</param>
        public PdfPKCS7(byte[] contentsKey, byte[] certsKey) {
            try {
                certs = SignUtils.ReadAllCerts(certsKey);
                signCerts = certs;
                signCert = (IX509Certificate)SignUtils.GetFirstElement(certs);
                crls = new List<IX509Crl>();
                using (IAsn1InputStream @in = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(contentsKey))) {
                    signatureValue = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(@in.ReadObject()).GetOctets();
                }
                sig = SignUtils.GetSignatureHelper("SHA1withRSA");
                sig.InitVerify(signCert.GetPublicKey());
                // setting the oid to SHA1withRSA
                digestAlgorithmOid = "1.2.840.10040.4.3";
                signatureMechanismOid = "1.3.36.3.3.1.2";
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        /// <summary>Use this constructor if you want to verify a signature.</summary>
        /// <param name="contentsKey">the /Contents key</param>
        /// <param name="filterSubtype">the filtersubtype</param>
        /// <param name="provider">the provider or <c>null</c> for the default provider</param>
        public PdfPKCS7(byte[] contentsKey, PdfName filterSubtype) {
            this.filterSubtype = filterSubtype;
            isTsp = PdfName.ETSI_RFC3161.Equals(filterSubtype);
            isCades = PdfName.ETSI_CAdES_DETACHED.Equals(filterSubtype);
            try {
                //
                // Basic checks to make sure it's a PKCS#7 SignedData Object
                //
                IAsn1Object pkcs;
                try {
                    using (IAsn1InputStream din = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(contentsKey))) {
                        pkcs = din.ReadObject();
                    }
                }
                catch (System.IO.IOException) {
                    throw new ArgumentException(SignExceptionMessageConstant.CANNOT_DECODE_PKCS7_SIGNED_DATA_OBJECT);
                }
                IAsn1Sequence signedData = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(pkcs);
                if (signedData == null) {
                    throw new ArgumentException(SignExceptionMessageConstant.NOT_A_VALID_PKCS7_OBJECT_NOT_A_SEQUENCE);
                }
                IDerObjectIdentifier objId = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(signedData.GetObjectAt(0));
                if (!objId.GetId().Equals(SecurityIDs.ID_PKCS7_SIGNED_DATA)) {
                    throw new ArgumentException(SignExceptionMessageConstant.NOT_A_VALID_PKCS7_OBJECT_NOT_SIGNED_DATA);
                }
                IAsn1Sequence content = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject
                    (signedData.GetObjectAt(1)).GetObject());
                // the positions that we care are:
                //     0 - version
                //     1 - digestAlgorithms
                //     2 - possible ID_PKCS7_DATA
                //     (the certificates and crls are taken out by other means)
                //     last - signerInfos
                // the version
                version = BOUNCY_CASTLE_FACTORY.CreateASN1Integer(content.GetObjectAt(0)).GetValue().GetIntValue();
                // the digestAlgorithms
                digestalgos = new HashSet<String>();
                IEnumerator e_1 = BOUNCY_CASTLE_FACTORY.CreateASN1Set(content.GetObjectAt(1)).GetObjects();
                while (e_1.MoveNext()) {
                    IAsn1Sequence s = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(e_1.Current);
                    IDerObjectIdentifier o = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(s.GetObjectAt(0));
                    digestalgos.Add(o.GetId());
                }
                // the possible ID_PKCS7_DATA
                IAsn1Sequence encapContentInfo = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(content.GetObjectAt(2));
                if (encapContentInfo.Size() > 1) {
                    IAsn1OctetString encapContent = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject
                        (encapContentInfo.GetObjectAt(1)).GetObject());
                    this.encapMessageContent = encapContent.GetOctets();
                }
                int next = 3;
                IAsn1TaggedObject taggedObj;
                while ((taggedObj = BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(content.GetObjectAt(next))) != null) {
                    ++next;
                    if (taggedObj.GetTagNo() == 1) {
                        // the crls
                        CertificateUtil.RetrieveRevocationInfoFromSignedData(taggedObj, this.signedDataCrls, this.signedDataOcsps, 
                            this.signedDataRevocationInfo);
                    }
                }
                // the certificates
                this.certs = SignUtils.ReadAllCerts(contentsKey);
                // the signerInfos
                IAsn1Set signerInfos = BOUNCY_CASTLE_FACTORY.CreateASN1Set(content.GetObjectAt(next));
                if (signerInfos.Size() != 1) {
                    throw new ArgumentException(SignExceptionMessageConstant.THIS_PKCS7_OBJECT_HAS_MULTIPLE_SIGNERINFOS_ONLY_ONE_IS_SUPPORTED_AT_THIS_TIME
                        );
                }
                IAsn1Sequence signerInfo = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(signerInfos.GetObjectAt(0));
                // the positions that we care are
                //     0 - version
                //     1 - the signing certificate issuer and serial number
                //     2 - the digest algorithm
                //     3 or 4 - digestEncryptionAlgorithm
                //     4 or 5 - encryptedDigest
                signerversion = BOUNCY_CASTLE_FACTORY.CreateASN1Integer(signerInfo.GetObjectAt(0)).GetValue().GetIntValue(
                    );
                // Get the signing certificate
                IAsn1Sequence issuerAndSerialNumber = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(signerInfo.GetObjectAt(1));
                IX500Name issuer = SignUtils.GetIssuerX500Principal(issuerAndSerialNumber);
                IBigInteger serialNumber = BOUNCY_CASTLE_FACTORY.CreateASN1Integer(issuerAndSerialNumber.GetObjectAt(1)).GetValue
                    ();
                foreach (Object element in certs) {
                    IX509Certificate cert = BOUNCY_CASTLE_FACTORY.CreateX509Certificate(element);
                    if (cert.GetIssuerDN().Equals(issuer) && serialNumber.Equals(cert.GetSerialNumber())) {
                        signCert = cert;
                        break;
                    }
                }
                if (signCert == null) {
                    throw new PdfException(SignExceptionMessageConstant.CANNOT_FIND_SIGNING_CERTIFICATE_WITH_THIS_SERIAL).SetMessageParams
                        (issuer.ToString() + " / " + serialNumber.ToString(16));
                }
                SignCertificateChain();
                digestAlgorithmOid = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(BOUNCY_CASTLE_FACTORY.CreateASN1Sequence
                    (signerInfo.GetObjectAt(2)).GetObjectAt(0)).GetId();
                next = 3;
                bool foundCades = false;
                IAsn1TaggedObject tagsig = BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(signerInfo.GetObjectAt(next));
                if (tagsig != null) {
                    IAsn1Set sseq = BOUNCY_CASTLE_FACTORY.CreateASN1Set(tagsig, false);
                    sigAttr = sseq.GetEncoded();
                    // maybe not necessary, but we use the following line as fallback:
                    sigAttrDer = sseq.GetEncoded(BOUNCY_CASTLE_FACTORY.CreateASN1Encoding().GetDer());
                    for (int k = 0; k < sseq.Size(); ++k) {
                        IAsn1Sequence seq2 = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(sseq.GetObjectAt(k));
                        String idSeq2 = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(seq2.GetObjectAt(0)).GetId();
                        if (idSeq2.Equals(SecurityIDs.ID_MESSAGE_DIGEST)) {
                            IAsn1Set set = BOUNCY_CASTLE_FACTORY.CreateASN1Set(seq2.GetObjectAt(1));
                            digestAttr = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(set.GetObjectAt(0)).GetOctets();
                        }
                        else {
                            if (idSeq2.Equals(SecurityIDs.ID_ADBE_REVOCATION)) {
                                IAsn1Set setout = BOUNCY_CASTLE_FACTORY.CreateASN1Set(seq2.GetObjectAt(1));
                                IAsn1Sequence seqout = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(setout.GetObjectAt(0));
                                for (int j = 0; j < seqout.Size(); ++j) {
                                    IAsn1TaggedObject tg = BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(seqout.GetObjectAt(j));
                                    if (tg.GetTagNo() == 0) {
                                        IAsn1Sequence seqin = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(tg.GetObject());
                                        FindCRL(seqin);
                                    }
                                    if (tg.GetTagNo() == 1) {
                                        IAsn1Sequence seqin = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(tg.GetObject());
                                        FindOcsp(seqin);
                                    }
                                }
                            }
                            else {
                                if (isCades && idSeq2.Equals(SecurityIDs.ID_AA_SIGNING_CERTIFICATE_V1)) {
                                    IAsn1Set setout = BOUNCY_CASTLE_FACTORY.CreateASN1Set(seq2.GetObjectAt(1));
                                    IAsn1Sequence seqout = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(setout.GetObjectAt(0));
                                    ISigningCertificate sv2 = BOUNCY_CASTLE_FACTORY.CreateSigningCertificate(seqout);
                                    IEssCertID[] cerv2m = sv2.GetCerts();
                                    IEssCertID cerv2 = cerv2m[0];
                                    byte[] enc2 = signCert.GetEncoded();
                                    IDigest m2 = SignUtils.GetMessageDigest("SHA-1");
                                    byte[] signCertHash = m2.Digest(enc2);
                                    byte[] hs2 = cerv2.GetCertHash();
                                    if (!JavaUtil.ArraysEquals(signCertHash, hs2)) {
                                        throw new ArgumentException("Signing certificate doesn't match the ESS information.");
                                    }
                                    foundCades = true;
                                }
                                else {
                                    if (isCades && idSeq2.Equals(SecurityIDs.ID_AA_SIGNING_CERTIFICATE_V2)) {
                                        IAsn1Set setout = BOUNCY_CASTLE_FACTORY.CreateASN1Set(seq2.GetObjectAt(1));
                                        IAsn1Sequence seqout = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(setout.GetObjectAt(0));
                                        ISigningCertificateV2 sv2 = BOUNCY_CASTLE_FACTORY.CreateSigningCertificateV2(seqout);
                                        IEssCertIDv2[] cerv2m = sv2.GetCerts();
                                        IEssCertIDv2 cerv2 = cerv2m[0];
                                        IAlgorithmIdentifier ai2 = cerv2.GetHashAlgorithm();
                                        byte[] enc2 = signCert.GetEncoded();
                                        IDigest m2 = SignUtils.GetMessageDigest(DigestAlgorithms.GetDigest(ai2.GetAlgorithm().GetId()));
                                        byte[] signCertHash = m2.Digest(enc2);
                                        byte[] hs2 = cerv2.GetCertHash();
                                        if (!JavaUtil.ArraysEquals(signCertHash, hs2)) {
                                            throw new ArgumentException("Signing certificate doesn't match the ESS information.");
                                        }
                                        foundCades = true;
                                    }
                                }
                            }
                        }
                    }
                    if (digestAttr == null) {
                        throw new ArgumentException(SignExceptionMessageConstant.AUTHENTICATED_ATTRIBUTE_IS_MISSING_THE_DIGEST);
                    }
                    ++next;
                }
                if (isCades && !foundCades) {
                    throw new ArgumentException("CAdES ESS information missing.");
                }
                IAsn1Sequence signatureMechanismInfo = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(signerInfo.GetObjectAt(next
                    ));
                ++next;
                signatureMechanismOid = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(signatureMechanismInfo.GetObjectAt
                    (0)).GetId();
                if (signatureMechanismInfo.Size() > 1) {
                    signatureMechanismParameters = signatureMechanismInfo.GetObjectAt(1);
                }
                signatureValue = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(signerInfo.GetObjectAt(next)).GetOctets();
                ++next;
                if (next < signerInfo.Size()) {
                    IAsn1TaggedObject taggedObject = BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(signerInfo.GetObjectAt(next)
                        );
                    if (taggedObject != null) {
                        IAsn1Set unat = BOUNCY_CASTLE_FACTORY.CreateASN1Set(taggedObject, false);
                        IAttributeTable attble = BOUNCY_CASTLE_FACTORY.CreateAttributeTable(unat);
                        IPkcsObjectIdentifiers ipkcsObjectIdentifiers = BOUNCY_CASTLE_FACTORY.CreatePKCSObjectIdentifiers();
                        IAttribute ts = attble.Get(ipkcsObjectIdentifiers.GetIdAaSignatureTimeStampToken());
                        if (!BOUNCY_CASTLE_FACTORY.IsNull(ts) && ts.GetAttrValues().Size() > 0) {
                            IAsn1Set attributeValues = ts.GetAttrValues();
                            IAsn1Sequence tokenSequence = BOUNCY_CASTLE_FACTORY.CreateASN1SequenceInstance(attributeValues.GetObjectAt
                                (0));
                            this.timestampCerts = SignUtils.ReadAllCerts(tokenSequence.GetEncoded());
                            IContentInfo contentInfo = BOUNCY_CASTLE_FACTORY.CreateContentInfo(tokenSequence);
                            this.timeStampTokenInfo = BOUNCY_CASTLE_FACTORY.CreateTSTInfo(contentInfo);
                        }
                    }
                }
                if (isTsp) {
                    IContentInfo contentInfoTsp = BOUNCY_CASTLE_FACTORY.CreateContentInfo(signedData);
                    this.timeStampTokenInfo = BOUNCY_CASTLE_FACTORY.CreateTSTInfo(contentInfoTsp);
                    this.timestampCerts = this.certs;
                    String algOID = timeStampTokenInfo.GetMessageImprint().GetHashAlgorithm().GetAlgorithm().GetId();
                    messageDigest = DigestAlgorithms.GetMessageDigestFromOid(algOID);
                }
                else {
                    if (this.encapMessageContent != null || digestAttr != null) {
                        if (PdfName.Adbe_pkcs7_sha1.Equals(GetFilterSubtype())) {
                            messageDigest = DigestAlgorithms.GetMessageDigest("SHA1");
                        }
                        else {
                            messageDigest = DigestAlgorithms.GetMessageDigest(GetDigestAlgorithmName());
                        }
                        encContDigest = DigestAlgorithms.GetMessageDigest(GetDigestAlgorithmName());
                    }
                    sig = InitSignature(signCert.GetPublicKey());
                }
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        public virtual void SetSignaturePolicy(SignaturePolicyInfo signaturePolicy) {
            this.signaturePolicyIdentifier = signaturePolicy.ToSignaturePolicyIdentifier();
        }

        public virtual void SetSignaturePolicy(ISignaturePolicyIdentifier signaturePolicy) {
            this.signaturePolicyIdentifier = signaturePolicy;
        }

        /// <summary>Getter for property sigName.</summary>
        /// <returns>Value of property sigName.</returns>
        public virtual String GetSignName() {
            return this.signName;
        }

        /// <summary>Setter for property sigName.</summary>
        /// <param name="signName">New value of property sigName.</param>
        public virtual void SetSignName(String signName) {
            this.signName = signName;
        }

        /// <summary>Getter for property reason.</summary>
        /// <returns>Value of property reason.</returns>
        public virtual String GetReason() {
            return this.reason;
        }

        /// <summary>Setter for property reason.</summary>
        /// <param name="reason">New value of property reason.</param>
        public virtual void SetReason(String reason) {
            this.reason = reason;
        }

        /// <summary>Getter for property location.</summary>
        /// <returns>Value of property location.</returns>
        public virtual String GetLocation() {
            return this.location;
        }

        /// <summary>Setter for property location.</summary>
        /// <param name="location">New value of property location.</param>
        public virtual void SetLocation(String location) {
            this.location = location;
        }

        /// <summary>Getter for property signDate.</summary>
        /// <returns>Value of property signDate.</returns>
        public virtual DateTime GetSignDate() {
            DateTime dt = GetTimeStampDate();
            if (dt == TimestampConstants.UNDEFINED_TIMESTAMP_DATE) {
                return this.signDate;
            }
            else {
                return dt;
            }
        }

        /// <summary>Setter for property signDate.</summary>
        /// <param name="signDate">New value of property signDate.</param>
        public virtual void SetSignDate(DateTime signDate) {
            this.signDate = signDate;
        }

        // version info
        /// <summary>Version of the PKCS#7 object</summary>
        private int version = 1;

        /// <summary>Version of the PKCS#7 "SignerInfo" object.</summary>
        private int signerversion = 1;

        /// <summary>Get the version of the PKCS#7 object.</summary>
        /// <returns>the version of the PKCS#7 object.</returns>
        public virtual int GetVersion() {
            return version;
        }

        /// <summary>Get the version of the PKCS#7 "SignerInfo" object.</summary>
        /// <returns>the version of the PKCS#7 "SignerInfo" object.</returns>
        public virtual int GetSigningInfoVersion() {
            return signerversion;
        }

        // Message digest algorithm
        /// <summary>The ID of the digest algorithm, e.g. "2.16.840.1.101.3.4.2.1".</summary>
        private readonly String digestAlgorithmOid;

        /// <summary>The object that will create the digest</summary>
        private IDigest messageDigest;

        /// <summary>The digest algorithms</summary>
        private ICollection<String> digestalgos;

        /// <summary>The digest attributes</summary>
        private byte[] digestAttr;

        private PdfName filterSubtype;

        /// <summary>The signature algorithm.</summary>
        private String signatureMechanismOid;

        private IAsn1Encodable signatureMechanismParameters = null;

        /// <summary>Getter for the ID of the digest algorithm, e.g. "2.16.840.1.101.3.4.2.1".</summary>
        /// <remarks>
        /// Getter for the ID of the digest algorithm, e.g. "2.16.840.1.101.3.4.2.1".
        /// See ISO-32000-1, section 12.8.3.3 PKCS#7 Signatures as used in ISO 32000
        /// </remarks>
        /// <returns>the ID of the digest algorithm</returns>
        public virtual String GetDigestAlgorithmOid() {
            return digestAlgorithmOid;
        }

        /// <summary>Returns the name of the digest algorithm, e.g. "SHA256".</summary>
        /// <returns>the digest algorithm name, e.g. "SHA256"</returns>
        public virtual String GetDigestAlgorithmName() {
            String hashAlgoName = DigestAlgorithms.GetDigest(digestAlgorithmOid);
            // Ed25519 and Ed448 do not allow a choice of hashing algorithm,
            // and ISO 32002 requires using a fixed hashing algorithm to
            // digest the document content
            if (SecurityIDs.ID_ED25519.Equals(this.signatureMechanismOid) && !SecurityIDs.ID_SHA512.Equals(digestAlgorithmOid
                )) {
                // We compare based on OID to ensure that there are no name normalisation issues.
                throw new PdfException(SignExceptionMessageConstant.ALGO_REQUIRES_SPECIFIC_HASH).SetMessageParams("Ed25519"
                    , "SHA-512", hashAlgoName);
            }
            else {
                if (SecurityIDs.ID_ED448.Equals(this.signatureMechanismOid) && !SecurityIDs.ID_SHAKE256.Equals(digestAlgorithmOid
                    )) {
                    throw new PdfException(SignExceptionMessageConstant.ALGO_REQUIRES_SPECIFIC_HASH).SetMessageParams("Ed448", 
                        "512-bit SHAKE256", hashAlgoName);
                }
            }
            return hashAlgoName;
        }

        /// <summary>Getter for the signature algorithm OID.</summary>
        /// <remarks>
        /// Getter for the signature algorithm OID.
        /// See ISO-32000-1, section 12.8.3.3 PKCS#7 Signatures as used in ISO 32000
        /// </remarks>
        /// <returns>the signature algorithm OID</returns>
        public virtual String GetSignatureMechanismOid() {
            return signatureMechanismOid;
        }

        /// <summary>
        /// Get the signature mechanism identifier, including both the digest function
        /// and the signature algorithm, e.g. "SHA1withRSA".
        /// </summary>
        /// <remarks>
        /// Get the signature mechanism identifier, including both the digest function
        /// and the signature algorithm, e.g. "SHA1withRSA".
        /// See ISO-32000-1, section 12.8.3.3 PKCS#7 Signatures as used in ISO 32000
        /// </remarks>
        /// <returns>the algorithm used to calculate the signature</returns>
        public virtual String GetSignatureMechanismName() {
            switch (this.signatureMechanismOid) {
                case SecurityIDs.ID_ED25519: {
                    // Ed25519 and Ed448 do not involve a choice of hashing algorithm
                    return "Ed25519";
                }

                case SecurityIDs.ID_ED448: {
                    return "Ed448";
                }

                case SecurityIDs.ID_RSASSA_PSS: {
                    // For RSASSA-PSS, the algorithm parameters dictate everything, so
                    // there's no need to duplicate that information in the algorithm name.
                    return "RSASSA-PSS";
                }

                default: {
                    return SignatureMechanisms.GetMechanism(signatureMechanismOid, GetDigestAlgorithmName());
                }
            }
        }

        /// <summary>Returns the name of the signature algorithm only (disregarding the digest function, if any).</summary>
        /// <returns>the name of an encryption algorithm</returns>
        public virtual String GetSignatureAlgorithmName() {
            String signAlgo = SignatureMechanisms.GetAlgorithm(signatureMechanismOid);
            if (signAlgo == null) {
                signAlgo = signatureMechanismOid;
            }
            return signAlgo;
        }

        /*
        *	DIGITAL SIGNATURE CREATION
        */
        // The signature is created externally
        /// <summary>The signature value or signed digest, if created outside this class</summary>
        private byte[] externalSignatureValue;

        /// <summary>Externally specified encapsulated message content.</summary>
        private byte[] externalEncapMessageContent;

        /// <summary>Sets the signature to an externally calculated value.</summary>
        /// <param name="signatureValue">the signature value</param>
        /// <param name="signedMessageContent">the extra data that goes into the data tag in PKCS#7</param>
        /// <param name="signatureAlgorithm">
        /// the signature algorithm. It must be <c>null</c> if the
        /// <c>signatureValue</c> is also <c>null</c>.
        /// If the <c>signatureValue</c> is not <c>null</c>,
        /// possible values include "RSA", "DSA", "ECDSA", "Ed25519" and "Ed448".
        /// </param>
        public virtual void SetExternalSignatureValue(byte[] signatureValue, byte[] signedMessageContent, String signatureAlgorithm
            ) {
            SetExternalSignatureValue(signatureValue, signedMessageContent, signatureAlgorithm, null);
        }

        /// <summary>Sets the signature to an externally calculated value.</summary>
        /// <param name="signatureValue">the signature value</param>
        /// <param name="signedMessageContent">the extra data that goes into the data tag in PKCS#7</param>
        /// <param name="signatureAlgorithm">
        /// the signature algorithm. It must be <c>null</c> if the
        /// <c>signatureValue</c> is also <c>null</c>.
        /// If the <c>signatureValue</c> is not <c>null</c>,
        /// possible values include "RSA", "RSASSA-PSS", "DSA",
        /// "ECDSA", "Ed25519" and "Ed448".
        /// </param>
        /// <param name="signatureMechanismParams">parameters for the signature mechanism, if required</param>
        public virtual void SetExternalSignatureValue(byte[] signatureValue, byte[] signedMessageContent, String signatureAlgorithm
            , ISignatureMechanismParams signatureMechanismParams) {
            externalSignatureValue = signatureValue;
            externalEncapMessageContent = signedMessageContent;
            if (signatureAlgorithm != null) {
                String digestAlgo = this.GetDigestAlgorithmName();
                String oid = SignatureMechanisms.GetSignatureMechanismOid(signatureAlgorithm, digestAlgo);
                if (oid == null) {
                    throw new PdfException(SignExceptionMessageConstant.COULD_NOT_DETERMINE_SIGNATURE_MECHANISM_OID).SetMessageParams
                        (signatureAlgorithm, digestAlgo);
                }
                this.signatureMechanismOid = oid;
            }
            if (signatureMechanismParams != null) {
                this.signatureMechanismParameters = signatureMechanismParams.ToEncodable();
            }
        }

        // The signature is created internally
        /// <summary>Class from the Java SDK that provides the functionality of a digital signature algorithm.</summary>
        private ISigner sig;

        /// <summary>The raw signature value as calculated by this class (or extracted from an existing PDF)</summary>
        private byte[] signatureValue;

        /// <summary>The content to which the signature applies, if encapsulated in the PKCS #7 payload.</summary>
        private byte[] encapMessageContent;

        // Signing functionality.
        private ISigner InitSignature(IPrivateKey key) {
            ISigner signature = SignUtils.GetSignatureHelper(GetSignatureMechanismName());
            signature.InitSign(key);
            return signature;
        }

        private ISigner InitSignature(IPublicKey key) {
            String signatureMechanism;
            if (PdfName.Adbe_x509_rsa_sha1.Equals(GetFilterSubtype())) {
                signatureMechanism = "SHA1withRSA";
            }
            else {
                signatureMechanism = GetSignatureMechanismName();
            }
            ISigner signature = SignUtils.GetSignatureHelper(signatureMechanism);
            ConfigureSignatureMechanismParameters(signature);
            signature.InitVerify(key);
            return signature;
        }

        private void ConfigureSignatureMechanismParameters(ISigner signature) {
            if (SecurityIDs.ID_RSASSA_PSS.Equals(this.signatureMechanismOid)) {
                IRsassaPssParameters @params = BOUNCY_CASTLE_FACTORY.CreateRSASSAPSSParams(this.signatureMechanismParameters
                    );
                String mgfOid = @params.GetMaskGenAlgorithm().GetAlgorithm().GetId();
                if (!SecurityIDs.ID_MGF1.Equals(mgfOid)) {
                    throw new ArgumentException(SignExceptionMessageConstant.ONLY_MGF1_SUPPORTED_IN_RSASSA_PSS);
                }
                // Even though having separate digests at all "layers" is mathematically fine,
                // it's bad practice at best (and a security problem at worst).
                // We don't support such hybridisation outside RSASSA-PSS either.
                // => on the authority of RFC 8933 we enforce the restriction here.
                String mechParamDigestAlgoOid = @params.GetHashAlgorithm().GetAlgorithm().GetId();
                if (!this.digestAlgorithmOid.Equals(mechParamDigestAlgoOid)) {
                    throw new ArgumentException(MessageFormatUtil.Format(SignExceptionMessageConstant.RSASSA_PSS_DIGESTMISSMATCH
                        , mechParamDigestAlgoOid, this.digestAlgorithmOid));
                }
                // This is actually morally an IAlgorithmIdentifier too, but since it's pretty much always going to be a
                // one-element sequence, it's probably not worth putting in a conversion method in the factory interface
                IAsn1Sequence mgfParams = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(@params.GetMaskGenAlgorithm().GetParameters
                    ());
                String mgfParamDigestAlgoOid = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(mgfParams.GetObjectAt(0)).
                    GetId();
                if (!this.digestAlgorithmOid.Equals(mgfParamDigestAlgoOid)) {
                    throw new ArgumentException(MessageFormatUtil.Format(SignExceptionMessageConstant.DISGEST_ALGORITM_MGF_MISMATCH
                        , mgfParamDigestAlgoOid, this.digestAlgorithmOid));
                }
                try {
                    int saltLength = @params.GetSaltLength().GetIntValue();
                    int trailerField = @params.GetTrailerField().GetIntValue();
                    SignUtils.SetRSASSAPSSParamsWithMGF1(signature, GetDigestAlgorithmName(), saltLength, trailerField);
                }
                catch (Exception e) {
                    throw new ArgumentException(SignExceptionMessageConstant.INVALID_ARGUMENTS, e);
                }
            }
        }

        /// <summary>Update the digest with the specified bytes.</summary>
        /// <remarks>
        /// Update the digest with the specified bytes.
        /// This method is used both for signing and verifying
        /// </remarks>
        /// <param name="buf">the data buffer</param>
        /// <param name="off">the offset in the data buffer</param>
        /// <param name="len">the data length</param>
        public virtual void Update(byte[] buf, int off, int len) {
            if (encapMessageContent != null || digestAttr != null || isTsp) {
                messageDigest.Update(buf, off, len);
            }
            else {
                sig.Update(buf, off, len);
            }
        }

        // adbe.x509.rsa_sha1 (PKCS#1)
        /// <summary>Gets the bytes for the PKCS#1 object.</summary>
        /// <returns>a byte array</returns>
        public virtual byte[] GetEncodedPKCS1() {
            try {
                if (externalSignatureValue != null) {
                    signatureValue = externalSignatureValue;
                }
                else {
                    signatureValue = sig.GenerateSignature();
                }
                MemoryStream bOut = new MemoryStream();
                IDerOutputStream dout = BOUNCY_CASTLE_FACTORY.CreateASN1OutputStream(bOut);
                dout.WriteObject(BOUNCY_CASTLE_FACTORY.CreateDEROctetString(signatureValue));
                dout.Dispose();
                return bOut.ToArray();
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        // other subfilters (PKCS#7)
        /// <summary>Gets the bytes for the PKCS7SignedData object.</summary>
        /// <returns>the bytes for the PKCS7SignedData object</returns>
        public virtual byte[] GetEncodedPKCS7() {
            return GetEncodedPKCS7(null, PdfSigner.CryptoStandard.CMS, null, null, null);
        }

        /// <summary>Gets the bytes for the PKCS7SignedData object.</summary>
        /// <remarks>
        /// Gets the bytes for the PKCS7SignedData object. Optionally the authenticatedAttributes
        /// in the signerInfo can also be set. If either of the parameters is <c>null</c>, none will be used.
        /// </remarks>
        /// <param name="secondDigest">the digest in the authenticatedAttributes</param>
        /// <returns>the bytes for the PKCS7SignedData object</returns>
        public virtual byte[] GetEncodedPKCS7(byte[] secondDigest) {
            return GetEncodedPKCS7(secondDigest, PdfSigner.CryptoStandard.CMS, null, null, null);
        }

        /// <summary>Gets the bytes for the PKCS7SignedData object.</summary>
        /// <remarks>
        /// Gets the bytes for the PKCS7SignedData object. Optionally the authenticatedAttributes
        /// in the signerInfo can also be set, and/or a time-stamp-authority client
        /// may be provided.
        /// </remarks>
        /// <param name="secondDigest">the digest in the authenticatedAttributes</param>
        /// <param name="sigtype">
        /// specifies the PKCS7 standard flavor to which created PKCS7SignedData object will adhere:
        /// either basic CMS or CAdES
        /// </param>
        /// <param name="tsaClient">TSAClient - null or an optional time stamp authority client</param>
        /// <param name="ocsp">
        /// collection of DER-encoded BasicOCSPResponses for the  certificate in the signature
        /// certificates
        /// chain, or null if OCSP revocation data is not to be added.
        /// </param>
        /// <param name="crlBytes">
        /// collection of DER-encoded CRL for certificates from the signature certificates chain,
        /// or null if CRL revocation data is not to be added.
        /// </param>
        /// <returns>byte[] the bytes for the PKCS7SignedData object</returns>
        /// <seealso><a href="https://datatracker.ietf.org/doc/html/rfc6960#section-4.2.1">RFC 6960 § 4.2.1</a></seealso>
        public virtual byte[] GetEncodedPKCS7(byte[] secondDigest, PdfSigner.CryptoStandard sigtype, ITSAClient tsaClient
            , ICollection<byte[]> ocsp, ICollection<byte[]> crlBytes) {
            try {
                if (externalSignatureValue != null) {
                    signatureValue = externalSignatureValue;
                    if (encapMessageContent != null) {
                        encapMessageContent = externalEncapMessageContent;
                    }
                }
                else {
                    if (externalEncapMessageContent != null && encapMessageContent != null) {
                        encapMessageContent = externalEncapMessageContent;
                        sig.Update(encapMessageContent);
                        signatureValue = sig.GenerateSignature();
                    }
                    else {
                        if (encapMessageContent != null) {
                            encapMessageContent = messageDigest.Digest();
                            sig.Update(encapMessageContent);
                        }
                        signatureValue = sig.GenerateSignature();
                    }
                }
                // Create the set of Hash algorithms
                IAsn1EncodableVector digestAlgorithms = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                foreach (String element in digestalgos) {
                    IAsn1EncodableVector algos = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                    algos.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(element));
                    algos.Add(BOUNCY_CASTLE_FACTORY.CreateDERNull());
                    digestAlgorithms.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(algos));
                }
                // Create the contentInfo.
                IAsn1EncodableVector v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_PKCS7_DATA));
                if (encapMessageContent != null) {
                    v.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(0, BOUNCY_CASTLE_FACTORY.CreateDEROctetString(encapMessageContent
                        )));
                }
                IDerSequence contentinfo = BOUNCY_CASTLE_FACTORY.CreateDERSequence(v);
                // Get all the certificates
                v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                foreach (Object element in certs) {
                    using (IAsn1InputStream tempstream = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(BOUNCY_CASTLE_FACTORY
                        .CreateX509Certificate(element).GetEncoded()))) {
                        v.Add(tempstream.ReadObject());
                    }
                }
                IDerSet dercertificates = BOUNCY_CASTLE_FACTORY.CreateDERSet(v);
                // Get the revocation info (crls field)
                IDerSet revInfoChoices = CertificateUtil.CreateRevocationInfoChoices(this.signedDataCrls, this.signedDataOcsps
                    , this.signedDataRevocationInfo);
                // Create signerInfo structure
                IAsn1EncodableVector signerInfo = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                // Add the signerInfo version
                signerInfo.Add(BOUNCY_CASTLE_FACTORY.CreateASN1Integer(signerversion));
                v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                v.Add(CertificateInfo.GetIssuer(signCert.GetTbsCertificate()));
                v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1Integer(signCert.GetSerialNumber()));
                signerInfo.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
                // Add the digestAlgorithm
                v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(digestAlgorithmOid));
                v.Add(BOUNCY_CASTLE_FACTORY.CreateDERNull());
                signerInfo.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
                // add the authenticated attribute if present
                if (secondDigest != null) {
                    signerInfo.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(false, 0, GetAuthenticatedAttributeSet(secondDigest
                        , ocsp, crlBytes, sigtype)));
                }
                // Add the digestEncryptionAlgorithm
                v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(signatureMechanismOid));
                if (this.signatureMechanismParameters == null) {
                    v.Add(BOUNCY_CASTLE_FACTORY.CreateDERNull());
                }
                else {
                    v.Add(this.signatureMechanismParameters.ToASN1Primitive());
                }
                signerInfo.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
                // Add the digest
                signerInfo.Add(BOUNCY_CASTLE_FACTORY.CreateDEROctetString(signatureValue));
                // When requested, go get and add the timestamp. May throw an exception.
                // Added by Martin Brunecky, 07/12/2007 folowing Aiken Sam, 2006-11-15
                // Sam found Adobe expects time-stamped SHA1-1 of the encrypted digest
                if (tsaClient != null) {
                    byte[] tsImprint = tsaClient.GetMessageDigest().Digest(signatureValue);
                    byte[] tsToken = tsaClient.GetTimeStampToken(tsImprint);
                    if (tsToken != null) {
                        IAsn1EncodableVector unauthAttributes = BuildUnauthenticatedAttributes(tsToken);
                        if (unauthAttributes != null) {
                            signerInfo.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(false, 1, BOUNCY_CASTLE_FACTORY.CreateDERSet(unauthAttributes
                                )));
                        }
                    }
                }
                // Finally build the body out of all the components above
                IAsn1EncodableVector body = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                body.Add(BOUNCY_CASTLE_FACTORY.CreateASN1Integer(version));
                body.Add(BOUNCY_CASTLE_FACTORY.CreateDERSet(digestAlgorithms));
                body.Add(contentinfo);
                body.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(false, 0, dercertificates));
                if (revInfoChoices != null) {
                    body.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(false, 1, revInfoChoices));
                }
                // Only allow one signerInfo
                body.Add(BOUNCY_CASTLE_FACTORY.CreateDERSet(BOUNCY_CASTLE_FACTORY.CreateDERSequence(signerInfo)));
                // Now we have the body, wrap it in it's PKCS7Signed shell
                // and return it
                //
                IAsn1EncodableVector whole = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                whole.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_PKCS7_SIGNED_DATA));
                whole.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(0, BOUNCY_CASTLE_FACTORY.CreateDERSequence(body)));
                MemoryStream bOut = new MemoryStream();
                IDerOutputStream dout = BOUNCY_CASTLE_FACTORY.CreateASN1OutputStream(bOut);
                dout.WriteObject(BOUNCY_CASTLE_FACTORY.CreateDERSequence(whole));
                dout.Dispose();
                return bOut.ToArray();
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        /// <summary>
        /// Added by Aiken Sam, 2006-11-15, modifed by Martin Brunecky 07/12/2007
        /// to start with the timeStampToken (signedData 1.2.840.113549.1.7.2).
        /// </summary>
        /// <remarks>
        /// Added by Aiken Sam, 2006-11-15, modifed by Martin Brunecky 07/12/2007
        /// to start with the timeStampToken (signedData 1.2.840.113549.1.7.2).
        /// Token is the TSA response without response status, which is usually
        /// handled by the (vendor supplied) TSA request/response interface).
        /// </remarks>
        /// <param name="timeStampToken">byte[] - time stamp token, DER encoded signedData</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1EncodableVector"/>
        /// </returns>
        private IAsn1EncodableVector BuildUnauthenticatedAttributes(byte[] timeStampToken) {
            if (timeStampToken == null) {
                return null;
            }
            IAsn1EncodableVector unauthAttributes = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
            IAsn1EncodableVector v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
            v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_AA_TIME_STAMP_TOKEN));
            using (IAsn1InputStream tempstream = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(timeStampToken
                ))) {
                IAsn1Sequence seq = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(tempstream.ReadObject());
                v.Add(BOUNCY_CASTLE_FACTORY.CreateDERSet(seq));
            }
            unauthAttributes.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
            return unauthAttributes;
        }

        // Authenticated attributes
        /// <summary>When using authenticatedAttributes the authentication process is different.</summary>
        /// <remarks>
        /// When using authenticatedAttributes the authentication process is different.
        /// The document digest is generated and put inside the attribute. The signing is done over the DER encoded
        /// authenticatedAttributes. This method provides that encoding and the parameters must be
        /// exactly the same as in
        /// <see cref="GetEncodedPKCS7(byte[])"/>.
        /// <para />
        /// Note: do not pass in the full DER-encoded OCSPResponse object obtained from the responder,
        /// only the DER-encoded IBasicOCSPResponse value contained in the response data.
        /// <para />
        /// A simple example:
        /// <pre>
        /// Calendar cal = Calendar.getInstance();
        /// PdfPKCS7 pk7 = new PdfPKCS7(key, chain, null, "SHA1", null, false);
        /// MessageDigest messageDigest = MessageDigest.getInstance("SHA1");
        /// byte[] buf = new byte[8192];
        /// int n;
        /// InputStream inp = sap.getRangeStream();
        /// while ((n = inp.read(buf)) &gt; 0) {
        /// messageDigest.update(buf, 0, n);
        /// }
        /// byte[] hash = messageDigest.digest();
        /// byte[] sh = pk7.getAuthenticatedAttributeBytes(hash, cal);
        /// pk7.update(sh, 0, sh.length);
        /// byte[] sg = pk7.getEncodedPKCS7(hash, cal);
        /// </pre>
        /// </remarks>
        /// <param name="secondDigest">the content digest</param>
        /// <param name="sigtype">
        /// specifies the PKCS7 standard flavor to which created PKCS7SignedData object will adhere:
        /// either basic CMS or CAdES
        /// </param>
        /// <param name="ocsp">
        /// collection of DER-encoded BasicOCSPResponses for the  certificate in the signature
        /// certificates
        /// chain, or null if OCSP revocation data is not to be added.
        /// </param>
        /// <param name="crlBytes">
        /// collection of DER-encoded CRL for certificates from the signature certificates chain,
        /// or null if CRL revocation data is not to be added.
        /// </param>
        /// <returns>the byte array representation of the authenticatedAttributes ready to be signed</returns>
        /// <seealso><a href="https://datatracker.ietf.org/doc/html/rfc6960#section-4.2.1">RFC 6960 § 4.2.1</a></seealso>
        public virtual byte[] GetAuthenticatedAttributeBytes(byte[] secondDigest, PdfSigner.CryptoStandard sigtype
            , ICollection<byte[]> ocsp, ICollection<byte[]> crlBytes) {
            try {
                return GetAuthenticatedAttributeSet(secondDigest, ocsp, crlBytes, sigtype).GetEncoded(BOUNCY_CASTLE_FACTORY
                    .CreateASN1Encoding().GetDer());
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        /// <summary>
        /// This method provides that encoding and the parameters must be
        /// exactly the same as in
        /// <see cref="GetEncodedPKCS7(byte[])"/>.
        /// </summary>
        /// <param name="secondDigest">the content digest</param>
        /// <returns>the byte array representation of the authenticatedAttributes ready to be signed</returns>
        private IDerSet GetAuthenticatedAttributeSet(byte[] secondDigest, ICollection<byte[]> ocsp, ICollection<byte
            []> crlBytes, PdfSigner.CryptoStandard sigtype) {
            try {
                IAsn1EncodableVector attribute = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                IAsn1EncodableVector v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_CONTENT_TYPE));
                v.Add(BOUNCY_CASTLE_FACTORY.CreateDERSet(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_PKCS7_DATA
                    )));
                attribute.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
                v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_MESSAGE_DIGEST));
                v.Add(BOUNCY_CASTLE_FACTORY.CreateDERSet(BOUNCY_CASTLE_FACTORY.CreateDEROctetString(secondDigest)));
                attribute.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
                bool haveCrl = false;
                if (crlBytes != null) {
                    foreach (byte[] bCrl in crlBytes) {
                        if (bCrl != null) {
                            haveCrl = true;
                            break;
                        }
                    }
                }
                if (ocsp != null && !ocsp.IsEmpty() || haveCrl) {
                    v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                    v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_ADBE_REVOCATION));
                    IAsn1EncodableVector revocationV = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                    if (haveCrl) {
                        IAsn1EncodableVector v2 = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                        foreach (byte[] bCrl in crlBytes) {
                            if (bCrl == null) {
                                continue;
                            }
                            using (IAsn1InputStream t = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(bCrl))) {
                                v2.Add(t.ReadObject());
                            }
                        }
                        revocationV.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(true, 0, BOUNCY_CASTLE_FACTORY.CreateDERSequence
                            (v2)));
                    }
                    if (ocsp != null && !ocsp.IsEmpty()) {
                        IAsn1EncodableVector vo1 = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                        foreach (byte[] ocspBytes in ocsp) {
                            IDerOctetString doctet = BOUNCY_CASTLE_FACTORY.CreateDEROctetString(ocspBytes);
                            IAsn1EncodableVector v2 = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                            IOcspObjectIdentifiers objectIdentifiers = BOUNCY_CASTLE_FACTORY.CreateOCSPObjectIdentifiers();
                            v2.Add(objectIdentifiers.GetIdPkixOcspBasic());
                            v2.Add(doctet);
                            IDerEnumerated den = BOUNCY_CASTLE_FACTORY.CreateASN1Enumerated(0);
                            IAsn1EncodableVector v3 = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                            v3.Add(den);
                            v3.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(true, 0, BOUNCY_CASTLE_FACTORY.CreateDERSequence(v2)));
                            vo1.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v3));
                        }
                        revocationV.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(true, 1, BOUNCY_CASTLE_FACTORY.CreateDERSequence
                            (vo1)));
                    }
                    v.Add(BOUNCY_CASTLE_FACTORY.CreateDERSet(BOUNCY_CASTLE_FACTORY.CreateDERSequence(revocationV)));
                    attribute.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
                }
                if (sigtype == PdfSigner.CryptoStandard.CADES) {
                    v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                    v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_AA_SIGNING_CERTIFICATE_V2));
                    IAsn1EncodableVector aaV2 = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                    if (!SecurityIDs.ID_SHA256.Equals(digestAlgorithmOid)) {
                        IAlgorithmIdentifier algoId = BOUNCY_CASTLE_FACTORY.CreateAlgorithmIdentifier(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier
                            (digestAlgorithmOid));
                        aaV2.Add(algoId);
                    }
                    IDigest md = SignUtils.GetMessageDigest(GetDigestAlgorithmName());
                    byte[] dig = md.Digest(signCert.GetEncoded());
                    aaV2.Add(BOUNCY_CASTLE_FACTORY.CreateDEROctetString(dig));
                    v.Add(BOUNCY_CASTLE_FACTORY.CreateDERSet(BOUNCY_CASTLE_FACTORY.CreateDERSequence(BOUNCY_CASTLE_FACTORY.CreateDERSequence
                        (BOUNCY_CASTLE_FACTORY.CreateDERSequence(aaV2)))));
                    attribute.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
                }
                if (signaturePolicyIdentifier != null) {
                    IPkcsObjectIdentifiers ipkcsObjectIdentifiers = BOUNCY_CASTLE_FACTORY.CreatePKCSObjectIdentifiers();
                    IAttribute attr = BOUNCY_CASTLE_FACTORY.CreateAttribute(ipkcsObjectIdentifiers.GetIdAaEtsSigPolicyId(), BOUNCY_CASTLE_FACTORY
                        .CreateDERSet(signaturePolicyIdentifier));
                    attribute.Add(attr);
                }
                return BOUNCY_CASTLE_FACTORY.CreateDERSet(attribute);
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        /*
        *	DIGITAL SIGNATURE VERIFICATION
        */
        /// <summary>Signature attributes</summary>
        private byte[] sigAttr;

        /// <summary>Signature attributes (maybe not necessary, but we use it as fallback)</summary>
        private byte[] sigAttrDer;

        /// <summary>encrypted digest</summary>
        private IDigest encContDigest;

        // Stefan Santesson
        /// <summary>Indicates if a signature has already been verified</summary>
        private bool verified;

        /// <summary>The result of the verification</summary>
        private bool verifyResult;

        // verification
        /// <summary>
        /// Verifies that signature integrity is intact (or in other words that signed data wasn't modified)
        /// by checking that embedded data digest corresponds to the calculated one.
        /// </summary>
        /// <remarks>
        /// Verifies that signature integrity is intact (or in other words that signed data wasn't modified)
        /// by checking that embedded data digest corresponds to the calculated one. Also ensures that signature
        /// is genuine and is created by the owner of private key that corresponds to the declared public certificate.
        /// <para />
        /// Even though signature can be authentic and signed data integrity can be intact,
        /// one shall also always check that signed data is not only a part of PDF contents but is actually a complete PDF
        /// file.
        /// In order to check that given signature covers the current
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// please
        /// use
        /// <see cref="SignatureUtil.SignatureCoversWholeDocument(System.String)"/>
        /// method.
        /// </remarks>
        /// <returns><c>true</c> if the signature checks out, <c>false</c> otherwise</returns>
        public virtual bool VerifySignatureIntegrityAndAuthenticity() {
            if (verified) {
                return verifyResult;
            }
            if (isTsp) {
                IMessageImprint imprint = timeStampTokenInfo.GetMessageImprint();
                byte[] md = messageDigest.Digest();
                byte[] imphashed = imprint.GetHashedMessage();
                verifyResult = JavaUtil.ArraysEquals(md, imphashed);
            }
            else {
                if (sigAttr != null || sigAttrDer != null) {
                    byte[] msgDigestBytes = messageDigest.Digest();
                    bool verifySignedMessageContent = true;
                    // Stefan Santesson fixed a bug, keeping the code backward compatible
                    bool encContDigestCompare = false;
                    if (encapMessageContent != null) {
                        verifySignedMessageContent = JavaUtil.ArraysEquals(msgDigestBytes, encapMessageContent);
                        encContDigest.Update(encapMessageContent);
                        encContDigestCompare = JavaUtil.ArraysEquals(encContDigest.Digest(), digestAttr);
                    }
                    bool absentEncContDigestCompare = JavaUtil.ArraysEquals(msgDigestBytes, digestAttr);
                    bool concludingDigestCompare = absentEncContDigestCompare || encContDigestCompare;
                    bool sigVerify = VerifySigAttributes(sigAttr) || VerifySigAttributes(sigAttrDer);
                    verifyResult = concludingDigestCompare && sigVerify && verifySignedMessageContent;
                }
                else {
                    if (encapMessageContent != null) {
                        SignUtils.UpdateVerifier(sig, messageDigest.Digest());
                    }
                    verifyResult = sig.VerifySignature(signatureValue);
                }
            }
            verified = true;
            return verifyResult;
        }

        private bool VerifySigAttributes(byte[] attr) {
            ISigner signature = InitSignature(signCert.GetPublicKey());
            SignUtils.UpdateVerifier(signature, attr);
            return signature.VerifySignature(signatureValue);
        }

        /// <summary>Checks if the timestamp refers to this document.</summary>
        /// <returns>true if it checks false otherwise</returns>
        public virtual bool VerifyTimestampImprint() {
            // TODO DEVSIX-6011 ensure this method works correctly
            if (timeStampTokenInfo == null) {
                return false;
            }
            IMessageImprint imprint = timeStampTokenInfo.GetMessageImprint();
            String algOID = imprint.GetHashAlgorithm().GetAlgorithm().GetId();
            byte[] md = SignUtils.GetMessageDigest(DigestAlgorithms.GetDigest(algOID)).Digest(signatureValue);
            byte[] imphashed = imprint.GetHashedMessage();
            return JavaUtil.ArraysEquals(md, imphashed);
        }

        // Certificates
        /// <summary>All the X.509 certificates in no particular order.</summary>
        private ICollection<IX509Certificate> certs;

        private ICollection<IX509Certificate> timestampCerts;

        /// <summary>All the X.509 certificates used for the main signature.</summary>
        internal ICollection<IX509Certificate> signCerts;

        /// <summary>The X.509 certificate that is used to sign the digest.</summary>
        private IX509Certificate signCert;

        /// <summary>Get all the X.509 certificates associated with this PKCS#7 object in no particular order.</summary>
        /// <remarks>
        /// Get all the X.509 certificates associated with this PKCS#7 object in no particular order.
        /// Other certificates, from OCSP for example, will also be included.
        /// </remarks>
        /// <returns>the X.509 certificates associated with this PKCS#7 object</returns>
        public virtual IX509Certificate[] GetCertificates() {
            return certs.ToArray(new IX509Certificate[0]);
        }

        /// <summary>Get all X.509 certificates associated with this PKCS#7 object timestamp in no particular order.</summary>
        /// <returns>
        /// 
        /// <see>Certificate[]</see>
        /// array
        /// </returns>
        public virtual IX509Certificate[] GetTimestampCertificates() {
            return timestampCerts.ToArray(new IX509Certificate[0]);
        }

        /// <summary>Get the X.509 sign certificate chain associated with this PKCS#7 object.</summary>
        /// <remarks>
        /// Get the X.509 sign certificate chain associated with this PKCS#7 object.
        /// Only the certificates used for the main signature will be returned, with
        /// the signing certificate first.
        /// </remarks>
        /// <returns>the X.509 certificates associated with this PKCS#7 object</returns>
        public virtual IX509Certificate[] GetSignCertificateChain() {
            return signCerts.ToArray(new IX509Certificate[0]);
        }

        /// <summary>Get the X.509 certificate actually used to sign the digest.</summary>
        /// <returns>the X.509 certificate actually used to sign the digest</returns>
        public virtual IX509Certificate GetSigningCertificate() {
            return signCert;
        }

        /// <summary>
        /// Helper method that creates the collection of certificates
        /// used for the main signature based on the complete list
        /// of certificates and the sign certificate.
        /// </summary>
        private void SignCertificateChain() {
            IList<IX509Certificate> cc = new List<IX509Certificate>();
            cc.Add(signCert);
            IList<IX509Certificate> oc = new List<IX509Certificate>(certs);
            for (int k = 0; k < oc.Count; ++k) {
                if (signCert.Equals(oc[k])) {
                    oc.JRemoveAt(k);
                    --k;
                }
            }
            bool found = true;
            while (found) {
                IX509Certificate v = (IX509Certificate)cc[cc.Count - 1];
                found = false;
                for (int k = 0; k < oc.Count; ++k) {
                    IX509Certificate issuer = (IX509Certificate)oc[k];
                    if (SignUtils.VerifyCertificateSignature(v, issuer.GetPublicKey())) {
                        found = true;
                        cc.Add(oc[k]);
                        oc.JRemoveAt(k);
                        break;
                    }
                }
            }
            signCerts = cc;
        }

        // Certificate Revocation Lists
        // Stored in the SignerInfo.
        private ICollection<IX509Crl> crls;

        // Stored in crls field of th SignedData.
        private readonly ICollection<IX509Crl> signedDataCrls = new List<IX509Crl>();

        /// <summary>Get the X.509 certificate revocation lists associated with this PKCS#7 object (stored in Signer Info).
        ///     </summary>
        /// <returns>the X.509 certificate revocation lists associated with this PKCS#7 object.</returns>
        public virtual ICollection<IX509Crl> GetCRLs() {
            return crls;
        }

        /// <summary>Get the X.509 certificate revocation lists associated with this PKCS#7 Signed Data object.</summary>
        /// <returns>the X.509 certificate revocation lists associated with this PKCS#7 Signed Data object.</returns>
        public virtual ICollection<IX509Crl> GetSignedDataCRLs() {
            return signedDataCrls;
        }

        /// <summary>Helper method that tries to construct the CRLs.</summary>
        internal virtual void FindCRL(IAsn1Sequence seq) {
            try {
                crls = new List<IX509Crl>();
                for (int k = 0; k < seq.Size(); ++k) {
                    MemoryStream ar = new MemoryStream(seq.GetObjectAt(k).ToASN1Primitive().GetEncoded(BOUNCY_CASTLE_FACTORY.CreateASN1Encoding
                        ().GetDer()));
                    IX509Crl crl = (IX509Crl)SignUtils.ParseCrlFromStream(ar);
                    crls.Add(crl);
                }
            }
            catch (Exception) {
            }
        }

        // ignore
        // Online Certificate Status Protocol
        /// <summary>BouncyCastle IBasicOCSPResponse</summary>
        internal IBasicOcspResponse basicResp;

        private readonly ICollection<IBasicOcspResponse> signedDataOcsps = new List<IBasicOcspResponse>();

        /// <summary>Gets the OCSP basic response collection retrieved from SignedData structure.</summary>
        /// <returns>the OCSP basic response collection.</returns>
        public virtual ICollection<IBasicOcspResponse> GetSignedDataOcsps() {
            return signedDataOcsps;
        }

        /// <summary>Gets the OCSP basic response from the SignerInfo if there is one.</summary>
        /// <returns>the OCSP basic response or null.</returns>
        public virtual IBasicOcspResponse GetOcsp() {
            return basicResp;
        }

        /// <summary>Checks if OCSP revocation refers to the document signing certificate.</summary>
        /// <returns>true if it checks, false otherwise</returns>
        public virtual bool IsRevocationValid() {
            if (basicResp == null) {
                return false;
            }
            if (signCerts.Count < 2) {
                return false;
            }
            try {
                IX509Certificate[] cs = GetSignCertificateChain();
                ISingleResponse sr = BOUNCY_CASTLE_FACTORY.CreateSingleResp(basicResp);
                ICertID cid = sr.GetCertID();
                IX509Certificate sigcer = GetSigningCertificate();
                IX509Certificate isscer = (IX509Certificate)cs[1];
                ICertID tis = SignUtils.GenerateCertificateId(isscer, sigcer.GetSerialNumber(), cid.GetHashAlgOID());
                return tis.Equals(cid);
            }
            catch (Exception) {
            }
            return false;
        }

        /// <summary>Helper method that creates the IBasicOCSPResp object.</summary>
        /// <param name="seq">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Sequence"/>
        /// wrapper
        /// </param>
        private void FindOcsp(IAsn1Sequence seq) {
            basicResp = null;
            bool ret;
            while (true) {
                IDerObjectIdentifier objectIdentifier = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(seq.GetObjectAt(0
                    ));
                IOcspObjectIdentifiers ocspObjectIdentifiers = BOUNCY_CASTLE_FACTORY.CreateOCSPObjectIdentifiers();
                if (objectIdentifier != null && objectIdentifier.GetId().Equals(ocspObjectIdentifiers.GetIdPkixOcspBasic()
                    .GetId())) {
                    break;
                }
                ret = true;
                for (int k = 0; k < seq.Size(); ++k) {
                    IAsn1Sequence nextSeq = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(seq.GetObjectAt(k));
                    if (nextSeq != null) {
                        seq = nextSeq;
                        ret = false;
                        break;
                    }
                    IAsn1TaggedObject tag = BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(seq.GetObjectAt(k));
                    if (tag != null) {
                        nextSeq = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(tag.GetObject());
                        if (nextSeq != null) {
                            seq = nextSeq;
                            ret = false;
                            break;
                        }
                        else {
                            return;
                        }
                    }
                }
                if (ret) {
                    return;
                }
            }
            IAsn1OctetString os = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(seq.GetObjectAt(1));
            using (IAsn1InputStream inp = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(os.GetOctets())) {
                basicResp = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(inp.ReadObject());
            }
        }

        // Time Stamps
        /// <summary>True if there's a PAdES LTV time stamp.</summary>
        private bool isTsp;

        /// <summary>True if it's a CAdES signature type.</summary>
        private bool isCades;

        /// <summary>BouncyCastle TSTInfo.</summary>
        private ITstInfo timeStampTokenInfo;

        /// <summary>Check if it's a PAdES-LTV time stamp.</summary>
        /// <returns>true if it's a PAdES-LTV time stamp, false otherwise</returns>
        public virtual bool IsTsp() {
            return isTsp;
        }

        /// <summary>Gets the timestamp token info if there is one.</summary>
        /// <returns>the timestamp token info or null</returns>
        public virtual ITstInfo GetTimeStampTokenInfo() {
            return timeStampTokenInfo;
        }

        /// <summary>Gets the timestamp date.</summary>
        /// <remarks>
        /// Gets the timestamp date.
        /// <para />
        /// In case the signed document doesn't contain timestamp,
        /// <see cref="TimestampConstants.UNDEFINED_TIMESTAMP_DATE"/>
        /// will be returned.
        /// </remarks>
        /// <returns>the timestamp date</returns>
        public virtual DateTime GetTimeStampDate() {
            if (timeStampTokenInfo == null) {
                return (DateTime)TimestampConstants.UNDEFINED_TIMESTAMP_DATE;
            }
            return SignUtils.GetTimeStampDate(timeStampTokenInfo);
        }

        /// <summary>Getter for the filter subtype.</summary>
        /// <returns>the filter subtype</returns>
        public virtual PdfName GetFilterSubtype() {
            return filterSubtype;
        }
    }
}
