/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
    /// and verifying a PKCS#7 signature.
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

        // Constructors for creating new signatures
        /// <summary>Assembles all the elements needed to create a signature, except for the data.</summary>
        /// <param name="privKey">the private key</param>
        /// <param name="certChain">the certificate chain</param>
        /// <param name="interfaceDigest">the interface digest</param>
        /// <param name="hashAlgorithm">the hash algorithm</param>
        /// <param name="provider">the provider or <c>null</c> for the default provider</param>
        /// <param name="hasRSAdata"><c>true</c> if the sub-filter is adbe.pkcs7.sha1</param>
        public PdfPKCS7(IPrivateKey privKey, IX509Certificate[] certChain, String hashAlgorithm, bool hasRSAdata) {
            // message digest
            digestAlgorithmOid = DigestAlgorithms.GetAllowedDigest(hashAlgorithm);
            if (digestAlgorithmOid == null) {
                throw new PdfException(SignExceptionMessageConstant.UNKNOWN_HASH_ALGORITHM).SetMessageParams(hashAlgorithm
                    );
            }
            // Copy the certificates
            signCert = (IX509Certificate)certChain[0];
            certs = new List<IX509Certificate>();
            foreach (IX509Certificate element in certChain) {
                certs.Add(element);
            }
            // initialize and add the digest algorithms.
            digestalgos = new HashSet<String>();
            digestalgos.Add(digestAlgorithmOid);
            // find the signing algorithm (RSA or DSA)
            if (privKey != null) {
                digestEncryptionAlgorithmOid = SignUtils.GetPrivateKeyAlgorithm(privKey);
                if (digestEncryptionAlgorithmOid.Equals("RSA")) {
                    digestEncryptionAlgorithmOid = SecurityIDs.ID_RSA;
                }
                else {
                    if (digestEncryptionAlgorithmOid.Equals("DSA")) {
                        digestEncryptionAlgorithmOid = SecurityIDs.ID_DSA;
                    }
                    else {
                        throw new PdfException(SignExceptionMessageConstant.UNKNOWN_KEY_ALGORITHM).SetMessageParams(digestEncryptionAlgorithmOid
                            );
                    }
                }
            }
            // initialize the RSA data
            if (hasRSAdata) {
                rsaData = new byte[0];
                messageDigest = DigestAlgorithms.GetMessageDigest(GetHashAlgorithm());
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
                using (IASN1InputStream @in = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(contentsKey))) {
                    digest = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(@in.ReadObject()).GetOctets();
                }
                sig = SignUtils.GetSignatureHelper("SHA1withRSA");
                sig.InitVerify(signCert.GetPublicKey());
                // setting the oid to SHA1withRSA
                digestAlgorithmOid = "1.2.840.10040.4.3";
                digestEncryptionAlgorithmOid = "1.3.36.3.3.1.2";
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
                IASN1Primitive pkcs;
                try {
                    using (IASN1InputStream din = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(contentsKey))) {
                        pkcs = din.ReadObject();
                    }
                }
                catch (System.IO.IOException) {
                    throw new ArgumentException(SignExceptionMessageConstant.CANNOT_DECODE_PKCS7_SIGNED_DATA_OBJECT);
                }
                IASN1Sequence signedData = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(pkcs);
                if (signedData == null) {
                    throw new ArgumentException(SignExceptionMessageConstant.NOT_A_VALID_PKCS7_OBJECT_NOT_A_SEQUENCE);
                }
                IASN1ObjectIdentifier objId = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(signedData.GetObjectAt(0));
                if (!objId.GetId().Equals(SecurityIDs.ID_PKCS7_SIGNED_DATA)) {
                    throw new ArgumentException(SignExceptionMessageConstant.NOT_A_VALID_PKCS7_OBJECT_NOT_SIGNED_DATA);
                }
                IASN1Sequence content = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject
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
                    IASN1Sequence s = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(e_1.Current);
                    IASN1ObjectIdentifier o = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(s.GetObjectAt(0));
                    digestalgos.Add(o.GetId());
                }
                // the possible ID_PKCS7_DATA
                IASN1Sequence rsaData = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(content.GetObjectAt(2));
                if (rsaData.Size() > 1) {
                    IASN1OctetString rsaDataContent = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject
                        (rsaData.GetObjectAt(1)).GetObject());
                    this.rsaData = rsaDataContent.GetOctets();
                }
                int next = 3;
                while (BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(content.GetObjectAt(next)) != null) {
                    ++next;
                }
                // the certificates
                /*
                This should work, but that's not always the case because of a bug in BouncyCastle:
                */
                certs = SignUtils.ReadAllCerts(contentsKey);
                // the signerInfos
                IASN1Set signerInfos = BOUNCY_CASTLE_FACTORY.CreateASN1Set(content.GetObjectAt(next));
                if (signerInfos.Size() != 1) {
                    throw new ArgumentException(SignExceptionMessageConstant.THIS_PKCS7_OBJECT_HAS_MULTIPLE_SIGNERINFOS_ONLY_ONE_IS_SUPPORTED_AT_THIS_TIME
                        );
                }
                IASN1Sequence signerInfo = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(signerInfos.GetObjectAt(0));
                // the positions that we care are
                //     0 - version
                //     1 - the signing certificate issuer and serial number
                //     2 - the digest algorithm
                //     3 or 4 - digestEncryptionAlgorithm
                //     4 or 5 - encryptedDigest
                signerversion = BOUNCY_CASTLE_FACTORY.CreateASN1Integer(signerInfo.GetObjectAt(0)).GetValue().GetIntValue(
                    );
                // Get the signing certificate
                IASN1Sequence issuerAndSerialNumber = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(signerInfo.GetObjectAt(1));
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
                IASN1TaggedObject tagsig = BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(signerInfo.GetObjectAt(next));
                if (tagsig != null) {
                    IASN1Set sseq = BOUNCY_CASTLE_FACTORY.CreateASN1Set(tagsig, false);
                    sigAttr = sseq.GetEncoded();
                    // maybe not necessary, but we use the following line as fallback:
                    sigAttrDer = sseq.GetEncoded(BOUNCY_CASTLE_FACTORY.CreateASN1Encoding().GetDer());
                    for (int k = 0; k < sseq.Size(); ++k) {
                        IASN1Sequence seq2 = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(sseq.GetObjectAt(k));
                        String idSeq2 = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(seq2.GetObjectAt(0)).GetId();
                        if (idSeq2.Equals(SecurityIDs.ID_MESSAGE_DIGEST)) {
                            IASN1Set set = BOUNCY_CASTLE_FACTORY.CreateASN1Set(seq2.GetObjectAt(1));
                            digestAttr = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(set.GetObjectAt(0)).GetOctets();
                        }
                        else {
                            if (idSeq2.Equals(SecurityIDs.ID_ADBE_REVOCATION)) {
                                IASN1Set setout = BOUNCY_CASTLE_FACTORY.CreateASN1Set(seq2.GetObjectAt(1));
                                IASN1Sequence seqout = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(setout.GetObjectAt(0));
                                for (int j = 0; j < seqout.Size(); ++j) {
                                    IASN1TaggedObject tg = BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(seqout.GetObjectAt(j));
                                    if (tg.GetTagNo() == 0) {
                                        IASN1Sequence seqin = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(tg.GetObject());
                                        FindCRL(seqin);
                                    }
                                    if (tg.GetTagNo() == 1) {
                                        IASN1Sequence seqin = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(tg.GetObject());
                                        FindOcsp(seqin);
                                    }
                                }
                            }
                            else {
                                if (isCades && idSeq2.Equals(SecurityIDs.ID_AA_SIGNING_CERTIFICATE_V1)) {
                                    IASN1Set setout = BOUNCY_CASTLE_FACTORY.CreateASN1Set(seq2.GetObjectAt(1));
                                    IASN1Sequence seqout = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(setout.GetObjectAt(0));
                                    ISigningCertificate sv2 = BOUNCY_CASTLE_FACTORY.CreateSigningCertificate(seqout);
                                    IESSCertID[] cerv2m = sv2.GetCerts();
                                    IESSCertID cerv2 = cerv2m[0];
                                    byte[] enc2 = signCert.GetEncoded();
                                    IIDigest m2 = SignUtils.GetMessageDigest("SHA-1");
                                    byte[] signCertHash = m2.Digest(enc2);
                                    byte[] hs2 = cerv2.GetCertHash();
                                    if (!JavaUtil.ArraysEquals(signCertHash, hs2)) {
                                        throw new ArgumentException("Signing certificate doesn't match the ESS information.");
                                    }
                                    foundCades = true;
                                }
                                else {
                                    if (isCades && idSeq2.Equals(SecurityIDs.ID_AA_SIGNING_CERTIFICATE_V2)) {
                                        IASN1Set setout = BOUNCY_CASTLE_FACTORY.CreateASN1Set(seq2.GetObjectAt(1));
                                        IASN1Sequence seqout = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(setout.GetObjectAt(0));
                                        ISigningCertificateV2 sv2 = BOUNCY_CASTLE_FACTORY.CreateSigningCertificateV2(seqout);
                                        IESSCertIDv2[] cerv2m = sv2.GetCerts();
                                        IESSCertIDv2 cerv2 = cerv2m[0];
                                        IAlgorithmIdentifier ai2 = cerv2.GetHashAlgorithm();
                                        byte[] enc2 = signCert.GetEncoded();
                                        IIDigest m2 = SignUtils.GetMessageDigest(DigestAlgorithms.GetDigest(ai2.GetAlgorithm().GetId()));
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
                digestEncryptionAlgorithmOid = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(BOUNCY_CASTLE_FACTORY.CreateASN1Sequence
                    (signerInfo.GetObjectAt(next++)).GetObjectAt(0)).GetId();
                digest = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(signerInfo.GetObjectAt(next++)).GetOctets();
                if (next < signerInfo.Size()) {
                    IASN1TaggedObject taggedObject = BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(signerInfo.GetObjectAt(next)
                        );
                    if (taggedObject != null) {
                        IASN1Set unat = BOUNCY_CASTLE_FACTORY.CreateASN1Set(taggedObject, false);
                        IAttributeTable attble = BOUNCY_CASTLE_FACTORY.CreateAttributeTable(unat);
                        IPKCSObjectIdentifiers ipkcsObjectIdentifiers = BOUNCY_CASTLE_FACTORY.CreatePKCSObjectIdentifiers();
                        IAttribute ts = attble.Get(ipkcsObjectIdentifiers.GetIdAaSignatureTimeStampToken());
                        if (ts != null && ts.GetAttrValues().Size() > 0) {
                            IASN1Set attributeValues = ts.GetAttrValues();
                            IASN1Sequence tokenSequence = BOUNCY_CASTLE_FACTORY.CreateASN1SequenceInstance(attributeValues.GetObjectAt
                                (0));
                            IContentInfo contentInfo = BOUNCY_CASTLE_FACTORY.CreateContentInfo(tokenSequence);
                            this.timeStampTokenInfo = BOUNCY_CASTLE_FACTORY.CreateTSTInfo(contentInfo);
                        }
                    }
                }
                if (isTsp) {
                    IContentInfo contentInfoTsp = BOUNCY_CASTLE_FACTORY.CreateContentInfo(signedData);
                    this.timeStampTokenInfo = BOUNCY_CASTLE_FACTORY.CreateTSTInfo(contentInfoTsp);
                    String algOID = timeStampTokenInfo.GetMessageImprint().GetHashAlgorithm().GetAlgorithm().GetId();
                    messageDigest = DigestAlgorithms.GetMessageDigestFromOid(algOID);
                }
                else {
                    if (this.rsaData != null || digestAttr != null) {
                        if (PdfName.Adbe_pkcs7_sha1.Equals(GetFilterSubtype())) {
                            messageDigest = DigestAlgorithms.GetMessageDigest("SHA1");
                        }
                        else {
                            messageDigest = DigestAlgorithms.GetMessageDigest(GetHashAlgorithm());
                        }
                        encContDigest = DigestAlgorithms.GetMessageDigest(GetHashAlgorithm());
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
        private String digestAlgorithmOid;

        /// <summary>The object that will create the digest</summary>
        private IIDigest messageDigest;

        /// <summary>The digest algorithms</summary>
        private ICollection<String> digestalgos;

        /// <summary>The digest attributes</summary>
        private byte[] digestAttr;

        private PdfName filterSubtype;

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
        public virtual String GetHashAlgorithm() {
            return DigestAlgorithms.GetDigest(digestAlgorithmOid);
        }

        // Encryption algorithm
        /// <summary>The encryption algorithm.</summary>
        private String digestEncryptionAlgorithmOid;

        /// <summary>Getter for the digest encryption algorithm.</summary>
        /// <remarks>
        /// Getter for the digest encryption algorithm.
        /// See ISO-32000-1, section 12.8.3.3 PKCS#7 Signatures as used in ISO 32000
        /// </remarks>
        /// <returns>the encryption algorithm</returns>
        public virtual String GetDigestEncryptionAlgorithmOid() {
            return digestEncryptionAlgorithmOid;
        }

        /// <summary>Get the algorithm used to calculate the message digest, e.g. "SHA1withRSA".</summary>
        /// <remarks>
        /// Get the algorithm used to calculate the message digest, e.g. "SHA1withRSA".
        /// See ISO-32000-1, section 12.8.3.3 PKCS#7 Signatures as used in ISO 32000
        /// </remarks>
        /// <returns>the algorithm used to calculate the message digest</returns>
        public virtual String GetDigestAlgorithm() {
            return GetHashAlgorithm() + "with" + GetEncryptionAlgorithm();
        }

        /*
        *	DIGITAL SIGNATURE CREATION
        */
        // The signature is created externally
        /// <summary>The signed digest if created outside this class</summary>
        private byte[] externalDigest;

        /// <summary>External RSA data</summary>
        private byte[] externalRsaData;

        /// <summary>Sets the digest/signature to an external calculated value.</summary>
        /// <param name="digest">the digest. This is the actual signature</param>
        /// <param name="rsaData">the extra data that goes into the data tag in PKCS#7</param>
        /// <param name="digestEncryptionAlgorithm">
        /// the encryption algorithm. It may must be <c>null</c> if the
        /// <c>digest</c> is also <c>null</c>. If the <c>digest</c>
        /// is not <c>null</c> then it may be "RSA" or "DSA"
        /// </param>
        public virtual void SetExternalDigest(byte[] digest, byte[] rsaData, String digestEncryptionAlgorithm) {
            externalDigest = digest;
            externalRsaData = rsaData;
            if (digestEncryptionAlgorithm != null) {
                if (digestEncryptionAlgorithm.Equals("RSA")) {
                    this.digestEncryptionAlgorithmOid = SecurityIDs.ID_RSA;
                }
                else {
                    if (digestEncryptionAlgorithm.Equals("DSA")) {
                        this.digestEncryptionAlgorithmOid = SecurityIDs.ID_DSA;
                    }
                    else {
                        if (digestEncryptionAlgorithm.Equals("ECDSA")) {
                            this.digestEncryptionAlgorithmOid = SecurityIDs.ID_ECDSA;
                        }
                        else {
                            throw new PdfException(SignExceptionMessageConstant.UNKNOWN_KEY_ALGORITHM).SetMessageParams(digestEncryptionAlgorithm
                                );
                        }
                    }
                }
            }
        }

        // The signature is created internally
        /// <summary>Class from the Java SDK that provides the functionality of a digital signature algorithm.</summary>
        private IISigner sig;

        /// <summary>The signed digest as calculated by this class (or extracted from an existing PDF)</summary>
        private byte[] digest;

        /// <summary>The RSA data</summary>
        private byte[] rsaData;

        // Signing functionality.
        private IISigner InitSignature(IPrivateKey key) {
            IISigner signature = SignUtils.GetSignatureHelper(GetDigestAlgorithm());
            signature.InitSign(key);
            return signature;
        }

        private IISigner InitSignature(IPublicKey key) {
            String digestAlgorithm = GetDigestAlgorithm();
            if (PdfName.Adbe_x509_rsa_sha1.Equals(GetFilterSubtype())) {
                digestAlgorithm = "SHA1withRSA";
            }
            IISigner signature = SignUtils.GetSignatureHelper(digestAlgorithm);
            signature.InitVerify(key);
            return signature;
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
            if (rsaData != null || digestAttr != null || isTsp) {
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
                if (externalDigest != null) {
                    digest = externalDigest;
                }
                else {
                    digest = sig.GenerateSignature();
                }
                MemoryStream bOut = new MemoryStream();
                IASN1OutputStream dout = BOUNCY_CASTLE_FACTORY.CreateASN1OutputStream(bOut);
                dout.WriteObject(BOUNCY_CASTLE_FACTORY.CreateDEROctetString(digest));
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
                if (externalDigest != null) {
                    digest = externalDigest;
                    if (rsaData != null) {
                        rsaData = externalRsaData;
                    }
                }
                else {
                    if (externalRsaData != null && rsaData != null) {
                        rsaData = externalRsaData;
                        sig.Update(rsaData);
                        digest = sig.GenerateSignature();
                    }
                    else {
                        if (rsaData != null) {
                            rsaData = messageDigest.Digest();
                            sig.Update(rsaData);
                        }
                        digest = sig.GenerateSignature();
                    }
                }
                // Create the set of Hash algorithms
                IASN1EncodableVector digestAlgorithms = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                foreach (Object element in digestalgos) {
                    IASN1EncodableVector algos = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                    algos.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier((String)element));
                    algos.Add(BOUNCY_CASTLE_FACTORY.CreateDERNull());
                    digestAlgorithms.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(algos));
                }
                // Create the contentInfo.
                IASN1EncodableVector v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_PKCS7_DATA));
                if (rsaData != null) {
                    v.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(0, BOUNCY_CASTLE_FACTORY.CreateDEROctetString(rsaData)));
                }
                IDERSequence contentinfo = BOUNCY_CASTLE_FACTORY.CreateDERSequence(v);
                // Get all the certificates
                //
                v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                foreach (Object element in certs) {
                    using (IASN1InputStream tempstream = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(BOUNCY_CASTLE_FACTORY
                        .CreateX509Certificate(element).GetEncoded()))) {
                        v.Add(tempstream.ReadObject());
                    }
                }
                IDERSet dercertificates = BOUNCY_CASTLE_FACTORY.CreateDERSet(v);
                // Create signerinfo structure.
                IASN1EncodableVector signerinfo = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                // Add the signerInfo version
                signerinfo.Add(BOUNCY_CASTLE_FACTORY.CreateASN1Integer(signerversion));
                v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                v.Add(CertificateInfo.GetIssuer(signCert.GetTbsCertificate()));
                v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1Integer(signCert.GetSerialNumber()));
                signerinfo.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
                // Add the digestAlgorithm
                v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(digestAlgorithmOid));
                v.Add(BOUNCY_CASTLE_FACTORY.CreateDERNull());
                signerinfo.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
                // add the authenticated attribute if present
                if (secondDigest != null) {
                    signerinfo.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(false, 0, GetAuthenticatedAttributeSet(secondDigest
                        , ocsp, crlBytes, sigtype)));
                }
                // Add the digestEncryptionAlgorithm
                v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(digestEncryptionAlgorithmOid));
                v.Add(BOUNCY_CASTLE_FACTORY.CreateDERNull());
                signerinfo.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
                // Add the digest
                signerinfo.Add(BOUNCY_CASTLE_FACTORY.CreateDEROctetString(digest));
                // When requested, go get and add the timestamp. May throw an exception.
                // Added by Martin Brunecky, 07/12/2007 folowing Aiken Sam, 2006-11-15
                // Sam found Adobe expects time-stamped SHA1-1 of the encrypted digest
                if (tsaClient != null) {
                    byte[] tsImprint = tsaClient.GetMessageDigest().Digest(digest);
                    byte[] tsToken = tsaClient.GetTimeStampToken(tsImprint);
                    if (tsToken != null) {
                        IASN1EncodableVector unauthAttributes = BuildUnauthenticatedAttributes(tsToken);
                        if (unauthAttributes != null) {
                            signerinfo.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(false, 1, BOUNCY_CASTLE_FACTORY.CreateDERSet(unauthAttributes
                                )));
                        }
                    }
                }
                // Finally build the body out of all the components above
                IASN1EncodableVector body = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                body.Add(BOUNCY_CASTLE_FACTORY.CreateASN1Integer(version));
                body.Add(BOUNCY_CASTLE_FACTORY.CreateDERSet(digestAlgorithms));
                body.Add(contentinfo);
                body.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(false, 0, dercertificates));
                // Only allow one signerInfo
                body.Add(BOUNCY_CASTLE_FACTORY.CreateDERSet(BOUNCY_CASTLE_FACTORY.CreateDERSequence(signerinfo)));
                // Now we have the body, wrap it in it's PKCS7Signed shell
                // and return it
                //
                IASN1EncodableVector whole = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                whole.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_PKCS7_SIGNED_DATA));
                whole.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(0, BOUNCY_CASTLE_FACTORY.CreateDERSequence(body)));
                MemoryStream bOut = new MemoryStream();
                IASN1OutputStream dout = BOUNCY_CASTLE_FACTORY.CreateASN1OutputStream(bOut);
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
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IASN1EncodableVector"/>
        /// </returns>
        private IASN1EncodableVector BuildUnauthenticatedAttributes(byte[] timeStampToken) {
            if (timeStampToken == null) {
                return null;
            }
            // @todo: move this together with the rest of the defintions
            String ID_TIME_STAMP_TOKEN = "1.2.840.113549.1.9.16.2.14";
            // RFC 3161 id-aa-timeStampToken
            IASN1EncodableVector unauthAttributes = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
            IASN1EncodableVector v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
            v.Add(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(ID_TIME_STAMP_TOKEN));
            // id-aa-timeStampToken
            using (IASN1InputStream tempstream = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(timeStampToken
                ))) {
                IASN1Sequence seq = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(tempstream.ReadObject());
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
        private IDERSet GetAuthenticatedAttributeSet(byte[] secondDigest, ICollection<byte[]> ocsp, ICollection<byte
            []> crlBytes, PdfSigner.CryptoStandard sigtype) {
            try {
                IASN1EncodableVector attribute = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                IASN1EncodableVector v = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
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
                    IASN1EncodableVector revocationV = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                    if (haveCrl) {
                        IASN1EncodableVector v2 = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                        foreach (byte[] bCrl in crlBytes) {
                            if (bCrl == null) {
                                continue;
                            }
                            using (IASN1InputStream t = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(bCrl))) {
                                v2.Add(t.ReadObject());
                            }
                        }
                        revocationV.Add(BOUNCY_CASTLE_FACTORY.CreateDERTaggedObject(true, 0, BOUNCY_CASTLE_FACTORY.CreateDERSequence
                            (v2)));
                    }
                    if (ocsp != null && !ocsp.IsEmpty()) {
                        IASN1EncodableVector vo1 = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                        foreach (byte[] ocspBytes in ocsp) {
                            IDEROctetString doctet = BOUNCY_CASTLE_FACTORY.CreateDEROctetString(ocspBytes);
                            IASN1EncodableVector v2 = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                            IOCSPObjectIdentifiers objectIdentifiers = BOUNCY_CASTLE_FACTORY.CreateOCSPObjectIdentifiers();
                            v2.Add(objectIdentifiers.GetIdPkixOcspBasic());
                            v2.Add(doctet);
                            IASN1Enumerated den = BOUNCY_CASTLE_FACTORY.CreateASN1Enumerated(0);
                            IASN1EncodableVector v3 = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
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
                    IASN1EncodableVector aaV2 = BOUNCY_CASTLE_FACTORY.CreateASN1EncodableVector();
                    IAlgorithmIdentifier algoId = BOUNCY_CASTLE_FACTORY.CreateAlgorithmIdentifier(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier
                        (digestAlgorithmOid));
                    aaV2.Add(algoId);
                    IIDigest md = SignUtils.GetMessageDigest(GetHashAlgorithm());
                    byte[] dig = md.Digest(signCert.GetEncoded());
                    aaV2.Add(BOUNCY_CASTLE_FACTORY.CreateDEROctetString(dig));
                    v.Add(BOUNCY_CASTLE_FACTORY.CreateDERSet(BOUNCY_CASTLE_FACTORY.CreateDERSequence(BOUNCY_CASTLE_FACTORY.CreateDERSequence
                        (BOUNCY_CASTLE_FACTORY.CreateDERSequence(aaV2)))));
                    attribute.Add(BOUNCY_CASTLE_FACTORY.CreateDERSequence(v));
                }
                if (signaturePolicyIdentifier != null) {
                    IPKCSObjectIdentifiers ipkcsObjectIdentifiers = BOUNCY_CASTLE_FACTORY.CreatePKCSObjectIdentifiers();
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
        private IIDigest encContDigest;

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
                    bool verifyRSAdata = true;
                    // Stefan Santesson fixed a bug, keeping the code backward compatible
                    bool encContDigestCompare = false;
                    if (rsaData != null) {
                        verifyRSAdata = JavaUtil.ArraysEquals(msgDigestBytes, rsaData);
                        encContDigest.Update(rsaData);
                        encContDigestCompare = JavaUtil.ArraysEquals(encContDigest.Digest(), digestAttr);
                    }
                    bool absentEncContDigestCompare = JavaUtil.ArraysEquals(msgDigestBytes, digestAttr);
                    bool concludingDigestCompare = absentEncContDigestCompare || encContDigestCompare;
                    bool sigVerify = VerifySigAttributes(sigAttr) || VerifySigAttributes(sigAttrDer);
                    verifyResult = concludingDigestCompare && sigVerify && verifyRSAdata;
                }
                else {
                    if (rsaData != null) {
                        SignUtils.UpdateVerifier(sig, messageDigest.Digest());
                    }
                    verifyResult = sig.VerifySignature(digest);
                }
            }
            verified = true;
            return verifyResult;
        }

        private bool VerifySigAttributes(byte[] attr) {
            IISigner signature = InitSignature(signCert.GetPublicKey());
            SignUtils.UpdateVerifier(signature, attr);
            return signature.VerifySignature(digest);
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
            byte[] md = SignUtils.GetMessageDigest(DigestAlgorithms.GetDigest(algOID)).Digest(digest);
            byte[] imphashed = imprint.GetHashedMessage();
            return JavaUtil.ArraysEquals(md, imphashed);
        }

        // Certificates
        /// <summary>All the X.509 certificates in no particular order.</summary>
        private ICollection<IX509Certificate> certs;

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
            return certs.ToArray(new IX509Certificate[certs.Count]);
        }

        /// <summary>Get the X.509 sign certificate chain associated with this PKCS#7 object.</summary>
        /// <remarks>
        /// Get the X.509 sign certificate chain associated with this PKCS#7 object.
        /// Only the certificates used for the main signature will be returned, with
        /// the signing certificate first.
        /// </remarks>
        /// <returns>the X.509 certificates associated with this PKCS#7 object</returns>
        public virtual IX509Certificate[] GetSignCertificateChain() {
            return signCerts.ToArray(new IX509Certificate[signCerts.Count]);
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
        private ICollection<IX509Crl> crls;

        /// <summary>Get the X.509 certificate revocation lists associated with this PKCS#7 object</summary>
        /// <returns>the X.509 certificate revocation lists associated with this PKCS#7 object</returns>
        public virtual ICollection<IX509Crl> GetCRLs() {
            return crls;
        }

        /// <summary>Helper method that tries to construct the CRLs.</summary>
        internal virtual void FindCRL(IASN1Sequence seq) {
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
        internal IBasicOCSPResponse basicResp;

        /// <summary>Gets the OCSP basic response if there is one.</summary>
        /// <returns>the OCSP basic response or null</returns>
        public virtual IBasicOCSPResponse GetOcsp() {
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
                IX509Certificate[] cs = (IX509Certificate[])GetSignCertificateChain();
                ISingleResp sr = BOUNCY_CASTLE_FACTORY.CreateSingleResp(basicResp);
                ICertificateID cid = sr.GetCertID();
                IX509Certificate sigcer = GetSigningCertificate();
                IX509Certificate isscer = cs[1];
                ICertificateID tis = SignUtils.GenerateCertificateId(isscer, sigcer.GetSerialNumber(), cid.GetHashAlgOID()
                    );
                return tis.Equals(cid);
            }
            catch (Exception) {
            }
            return false;
        }

        /// <summary>Helper method that creates the IBasicOCSPResp object.</summary>
        /// <param name="seq"/>
        private void FindOcsp(IASN1Sequence seq) {
            basicResp = null;
            bool ret = false;
            while (true) {
                IASN1ObjectIdentifier objectIdentifier = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(seq.GetObjectAt(
                    0));
                IOCSPObjectIdentifiers ocspObjectIdentifiers = BOUNCY_CASTLE_FACTORY.CreateOCSPObjectIdentifiers();
                if (objectIdentifier != null && objectIdentifier.GetId().Equals(ocspObjectIdentifiers.GetIdPkixOcspBasic()
                    .GetId())) {
                    break;
                }
                ret = true;
                for (int k = 0; k < seq.Size(); ++k) {
                    IASN1Sequence nextSeq = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(seq.GetObjectAt(k));
                    if (nextSeq != null) {
                        seq = nextSeq;
                        ret = false;
                        break;
                    }
                    IASN1TaggedObject tag = BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(seq.GetObjectAt(k));
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
            IASN1OctetString os = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(seq.GetObjectAt(1));
            using (IASN1InputStream inp = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(os.GetOctets())) {
                basicResp = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(inp.ReadObject());
            }
        }

        // Time Stamps
        /// <summary>True if there's a PAdES LTV time stamp.</summary>
        private bool isTsp;

        /// <summary>True if it's a CAdES signature type.</summary>
        private bool isCades;

        /// <summary>BouncyCastle TSTInfo.</summary>
        private ITSTInfo timeStampTokenInfo;

        /// <summary>Check if it's a PAdES-LTV time stamp.</summary>
        /// <returns>true if it's a PAdES-LTV time stamp, false otherwise</returns>
        public virtual bool IsTsp() {
            return isTsp;
        }

        /// <summary>Gets the timestamp token info if there is one.</summary>
        /// <returns>the timestamp token info or null</returns>
        public virtual ITSTInfo GetTimeStampTokenInfo() {
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

        /// <summary>Returns the encryption algorithm</summary>
        /// <returns>the name of an encryption algorithm</returns>
        public virtual String GetEncryptionAlgorithm() {
            String encryptAlgo = EncryptionAlgorithms.GetAlgorithm(digestEncryptionAlgorithmOid);
            if (encryptAlgo == null) {
                encryptAlgo = digestEncryptionAlgorithmOid;
            }
            return encryptAlgo;
        }
    }
}
