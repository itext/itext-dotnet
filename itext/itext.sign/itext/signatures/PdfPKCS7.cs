/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using  Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using iText.Kernel;
using iText.Kernel.Pdf;

namespace iText.Signatures {
    /// <summary>
    /// This class does all the processing related to signing
    /// and verifying a PKCS#7 signature.
    /// </summary>
    public class PdfPKCS7 {
        private SignaturePolicyIdentifier signaturePolicyIdentifier;

        /// <summary>Holds value of property signName.</summary>
        private String signName;

        /// <summary>Holds value of property reason.</summary>
        private String reason;

        /// <summary>Holds value of property location.</summary>
        private String location;

        /// <summary>Holds value of property signDate.</summary>
        private DateTime signDate;

        /// <summary>Assembles all the elements needed to create a signature, except for the data.</summary>
        /// <param name="privKey">the private key</param>
        /// <param name="certChain">the certificate chain</param>
        /// <param name="interfaceDigest">the interface digest</param>
        /// <param name="hashAlgorithm">the hash algorithm</param>
        /// <param name="provider">the provider or <code>null</code> for the default provider</param>
        /// <param name="hasRSAdata"><CODE>true</CODE> if the sub-filter is adbe.pkcs7.sha1</param>
        /// <exception cref="Org.BouncyCastle.Security.InvalidKeyException">on error</exception>
        /// <exception cref="Java.Security.NoSuchProviderException">on error</exception>
        /// <exception cref="Org.BouncyCastle.Security.SecurityUtilityException">on error</exception>
        public PdfPKCS7(ICipherParameters privKey, X509Certificate[] certChain, String hashAlgorithm, bool hasRSAdata
            ) {
            // Encryption provider
            // Signature info
            // Constructors for creating new signatures
            // message digest
            digestAlgorithmOid = DigestAlgorithms.GetAllowedDigest(hashAlgorithm);
            if (digestAlgorithmOid == null) {
                throw new PdfException(PdfException.UnknownHashAlgorithm1).SetMessageParams(hashAlgorithm);
            }
            // Copy the certificates
            signCert = (X509Certificate)certChain[0];
            certs = new List<X509Certificate>();
            foreach (X509Certificate element in certChain) {
                certs.Add(element);
            }
            // initialize and add the digest algorithms.
            digestalgos = new HashSet<String>();
            digestalgos.Add(digestAlgorithmOid);
            // find the signing algorithm (RSA or DSA)
            if (privKey != null) {
                digestEncryptionAlgorithmOid = privKey.GetAlgorithm();
                if (digestEncryptionAlgorithmOid.Equals("RSA")) {
                    digestEncryptionAlgorithmOid = SecurityIDs.ID_RSA;
                }
                else {
                    if (digestEncryptionAlgorithmOid.Equals("DSA")) {
                        digestEncryptionAlgorithmOid = SecurityIDs.ID_DSA;
                    }
                    else {
                        throw new PdfException(PdfException.UnknownKeyAlgorithm1).SetMessageParams(digestEncryptionAlgorithmOid);
                    }
                }
            }
            // initialize the RSA data
            if (hasRSAdata) {
                RSAdata = new byte[0];
                messageDigest = DigestAlgorithms.GetMessageDigest(GetHashAlgorithm());
            }
            // initialize the Signature object
            if (privKey != null) {
                sig = InitSignature(privKey);
            }
        }

        /// <summary>Use this constructor if you want to verify a signature using the sub-filter adbe.x509.rsa_sha1.</summary>
        /// <param name="contentsKey">the /Contents key</param>
        /// <param name="certsKey">the /Cert key</param>
        /// <param name="provider">the provider or <code>null</code> for the default provider</param>
        public PdfPKCS7(byte[] contentsKey, byte[] certsKey) {
            // Constructors for validating existing signatures
            try {
                certs = SignUtils.ReadAllCerts(certsKey);
                signCerts = certs;
                signCert = (X509Certificate)SignUtils.GetFirstElement(certs);
                crls = new List<X509Crl>();
                Asn1InputStream @in = new Asn1InputStream(new MemoryStream(contentsKey));
                digest = ((Asn1OctetString)@in.ReadObject()).GetOctets();
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
        /// <param name="provider">the provider or <code>null</code> for the default provider</param>
        public PdfPKCS7(byte[] contentsKey, PdfName filterSubtype) {
            this.filterSubtype = filterSubtype;
            isTsp = PdfName.ETSI_RFC3161.Equals(filterSubtype);
            isCades = PdfName.ETSI_CAdES_DETACHED.Equals(filterSubtype);
            try {
                Asn1InputStream din = new Asn1InputStream(new MemoryStream(contentsKey));
                //
                // Basic checks to make sure it's a PKCS#7 SignedData Object
                //
                Asn1Object pkcs;
                try {
                    pkcs = din.ReadObject();
                }
                catch (System.IO.IOException) {
                    throw new ArgumentException(PdfException.CannotDecodePkcs7SigneddataObject);
                }
                if (!(pkcs is Asn1Sequence)) {
                    throw new ArgumentException(PdfException.NotAValidPkcs7ObjectNotASequence);
                }
                Asn1Sequence signedData = (Asn1Sequence)pkcs;
                DerObjectIdentifier objId = (DerObjectIdentifier)signedData[0];
                if (!objId.Id.Equals(SecurityIDs.ID_PKCS7_SIGNED_DATA)) {
                    throw new ArgumentException(PdfException.NotAValidPkcs7ObjectNotSignedData);
                }
                Asn1Sequence content = (Asn1Sequence)((Asn1TaggedObject)signedData[1]).GetObject();
                // the positions that we care are:
                //     0 - version
                //     1 - digestAlgorithms
                //     2 - possible ID_PKCS7_DATA
                //     (the certificates and crls are taken out by other means)
                //     last - signerInfos
                // the version
                version = ((DerInteger)content[0]).Value.IntValue;
                // the digestAlgorithms
                digestalgos = new HashSet<String>();
                IEnumerator e_1 = ((Asn1Set)content[1]).GetObjects();
                while (e_1.MoveNext()) {
                    Asn1Sequence s = (Asn1Sequence)e_1.Current;
                    DerObjectIdentifier o = (DerObjectIdentifier)s[0];
                    digestalgos.Add(o.Id);
                }
                // the possible ID_PKCS7_DATA
                Asn1Sequence rsaData = (Asn1Sequence)content[2];
                if (rsaData.Count > 1) {
                    Asn1OctetString rsaDataContent = (Asn1OctetString)((Asn1TaggedObject)rsaData[1]).GetObject();
                    RSAdata = rsaDataContent.GetOctets();
                }
                int next = 3;
                while (content[next] is Asn1TaggedObject) {
                    ++next;
                }
                // the certificates
                /*
                This should work, but that's not always the case because of a bug in BouncyCastle:
                */
                certs = SignUtils.ReadAllCerts(contentsKey);
                /*
                The following workaround was provided by Alfonso Massa, but it doesn't always work either.
                
                ASN1Set certSet = null;
                ASN1Set crlSet = null;
                while (content.getObjectAt(next) instanceof ASN1TaggedObject) {
                ASN1TaggedObject tagged = (ASN1TaggedObject)content.getObjectAt(next);
                
                switch (tagged.getTagNo()) {
                case 0:
                certSet = ASN1Set.getInstance(tagged, false);
                break;
                case 1:
                crlSet = ASN1Set.getInstance(tagged, false);
                break;
                default:
                throw new IllegalArgumentException("unknown tag value " + tagged.getTagNo());
                }
                ++next;
                }
                certs = new ArrayList<Certificate>(certSet.size());
                
                CertificateFactory certFact = CertificateFactory.getInstance("X.509", new BouncyCastleProvider());
                for (Enumeration en = certSet.getObjects(); en.hasMoreElements();) {
                ASN1Primitive obj = ((ASN1Encodable)en.nextElement()).toASN1Primitive();
                if (obj instanceof ASN1Sequence) {
                ByteArrayInputStream stream = new ByteArrayInputStream(obj.getEncoded());
                X509Certificate x509Certificate = (X509Certificate)certFact.generateCertificate(stream);
                stream.close();
                certs.add(x509Certificate);
                }
                }
                */
                // the signerInfos
                Asn1Set signerInfos = (Asn1Set)content[next];
                if (signerInfos.Count != 1) {
                    throw new ArgumentException(PdfException.ThisPkcs7ObjectHasMultipleSignerinfosOnlyOneIsSupportedAtThisTime
                        );
                }
                Asn1Sequence signerInfo = (Asn1Sequence)signerInfos[0];
                // the positions that we care are
                //     0 - version
                //     1 - the signing certificate issuer and serial number
                //     2 - the digest algorithm
                //     3 or 4 - digestEncryptionAlgorithm
                //     4 or 5 - encryptedDigest
                signerversion = ((DerInteger)signerInfo[0]).Value.IntValue;
                // Get the signing certificate
                Asn1Sequence issuerAndSerialNumber = (Asn1Sequence)signerInfo[1];
                X509Name issuer = SignUtils.GetIssuerX509Name(issuerAndSerialNumber);
                BigInteger serialNumber = ((DerInteger)issuerAndSerialNumber[1]).Value;
                foreach (Object element in certs) {
                    X509Certificate cert = (X509Certificate)element;
                    if (cert.IssuerDN.Equals(issuer) && serialNumber.Equals(cert.SerialNumber)) {
                        signCert = cert;
                        break;
                    }
                }
                if (signCert == null) {
                    throw new PdfException(PdfException.CannotFindSigningCertificateWithSerial1).SetMessageParams(issuer.ToString
                        () + " / " + serialNumber.ToString(16));
                }
                SignCertificateChain();
                digestAlgorithmOid = ((DerObjectIdentifier)((Asn1Sequence)signerInfo[2])[0]).Id;
                next = 3;
                bool foundCades = false;
                if (signerInfo[next] is Asn1TaggedObject) {
                    Asn1TaggedObject tagsig = (Asn1TaggedObject)signerInfo[next];
                    Asn1Set sseq = Asn1Set.GetInstance(tagsig, false);
                    sigAttr = sseq.GetEncoded();
                    // maybe not necessary, but we use the following line as fallback:
                    sigAttrDer = sseq.GetEncoded(Org.BouncyCastle.Asn1.Asn1Encodable.Der);
                    for (int k = 0; k < sseq.Count; ++k) {
                        Asn1Sequence seq2 = (Asn1Sequence)sseq[k];
                        String idSeq2 = ((DerObjectIdentifier)seq2[0]).Id;
                        if (idSeq2.Equals(SecurityIDs.ID_MESSAGE_DIGEST)) {
                            Asn1Set set = (Asn1Set)seq2[1];
                            digestAttr = ((Asn1OctetString)set[0]).GetOctets();
                        }
                        else {
                            if (idSeq2.Equals(SecurityIDs.ID_ADBE_REVOCATION)) {
                                Asn1Set setout = (Asn1Set)seq2[1];
                                Asn1Sequence seqout = (Asn1Sequence)setout[0];
                                for (int j = 0; j < seqout.Count; ++j) {
                                    Asn1TaggedObject tg = (Asn1TaggedObject)seqout[j];
                                    if (tg.TagNo == 0) {
                                        Asn1Sequence seqin = (Asn1Sequence)tg.GetObject();
                                        FindCRL(seqin);
                                    }
                                    if (tg.TagNo == 1) {
                                        Asn1Sequence seqin = (Asn1Sequence)tg.GetObject();
                                        FindOcsp(seqin);
                                    }
                                }
                            }
                            else {
                                if (isCades && idSeq2.Equals(SecurityIDs.ID_AA_SIGNING_CERTIFICATE_V1)) {
                                    Asn1Set setout = (Asn1Set)seq2[1];
                                    Asn1Sequence seqout = (Asn1Sequence)setout[0];
                                    SigningCertificate sv2 = SigningCertificate.GetInstance(seqout);
                                    EssCertID[] cerv2m = sv2.GetCerts();
                                    EssCertID cerv2 = cerv2m[0];
                                    byte[] enc2 = signCert.GetEncoded();
                                    IDigest m2 = SignUtils.GetMessageDigest("SHA-1");
                                    byte[] signCertHash = m2.Digest(enc2);
                                    byte[] hs2 = cerv2.GetCertHash();
                                    if (!iText.IO.Util.JavaUtil.ArraysEquals(signCertHash, hs2)) {
                                        throw new ArgumentException("Signing certificate doesn't match the ESS information.");
                                    }
                                    foundCades = true;
                                }
                                else {
                                    if (isCades && idSeq2.Equals(SecurityIDs.ID_AA_SIGNING_CERTIFICATE_V2)) {
                                        Asn1Set setout = (Asn1Set)seq2[1];
                                        Asn1Sequence seqout = (Asn1Sequence)setout[0];
                                        SigningCertificateV2 sv2 = SigningCertificateV2.GetInstance(seqout);
                                        EssCertIDv2[] cerv2m = sv2.GetCerts();
                                        EssCertIDv2 cerv2 = cerv2m[0];
                                        AlgorithmIdentifier ai2 = cerv2.HashAlgorithm;
                                        byte[] enc2 = signCert.GetEncoded();
                                        IDigest m2 = SignUtils.GetMessageDigest(DigestAlgorithms.GetDigest(ai2.ObjectID.Id));
                                        byte[] signCertHash = m2.Digest(enc2);
                                        byte[] hs2 = cerv2.GetCertHash();
                                        if (!iText.IO.Util.JavaUtil.ArraysEquals(signCertHash, hs2)) {
                                            throw new ArgumentException("Signing certificate doesn't match the ESS information.");
                                        }
                                        foundCades = true;
                                    }
                                }
                            }
                        }
                    }
                    if (digestAttr == null) {
                        throw new ArgumentException(PdfException.AuthenticatedAttributeIsMissingTheDigest);
                    }
                    ++next;
                }
                if (isCades && !foundCades) {
                    throw new ArgumentException("CAdES ESS information missing.");
                }
                digestEncryptionAlgorithmOid = ((DerObjectIdentifier)((Asn1Sequence)signerInfo[next++])[0]).Id;
                digest = ((Asn1OctetString)signerInfo[next++]).GetOctets();
                if (next < signerInfo.Count && signerInfo[next] is Asn1TaggedObject) {
                    Asn1TaggedObject taggedObject = (Asn1TaggedObject)signerInfo[next];
                    Asn1Set unat = Asn1Set.GetInstance(taggedObject, false);
                    Org.BouncyCastle.Asn1.Cms.AttributeTable attble = new Org.BouncyCastle.Asn1.Cms.AttributeTable(unat);
                    Org.BouncyCastle.Asn1.Cms.Attribute ts = attble[Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdAASignatureTimeStampToken
                        ];
                    if (ts != null && ts.AttrValues.Count > 0) {
                        Asn1Set attributeValues = ts.AttrValues;
                        Asn1Sequence tokenSequence = Asn1Sequence.GetInstance(attributeValues[0]);
                        ContentInfo contentInfo = ContentInfo.GetInstance(tokenSequence);
                        this.timeStampToken = new TimeStampToken(contentInfo);
                    }
                }
                if (isTsp) {
                    ContentInfo contentInfoTsp = ContentInfo.GetInstance(signedData);
                    this.timeStampToken = new TimeStampToken(contentInfoTsp);
                    TimeStampTokenInfo info = timeStampToken.TimeStampInfo;
                    String algOID = info.HashAlgorithm.ObjectID.Id;
                    messageDigest = DigestAlgorithms.GetMessageDigestFromOid(algOID);
                }
                else {
                    if (RSAdata != null || digestAttr != null) {
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

        public virtual void SetSignaturePolicy(SignaturePolicyIdentifier signaturePolicy) {
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
            if (dt == SignUtils.UNDEFINED_TIMESTAMP_DATE) {
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

        /// <summary>Version of the PKCS#7 object</summary>
        private int version = 1;

        /// <summary>Version of the PKCS#7 "SignerInfo" object.</summary>
        private int signerversion = 1;

        // version info
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

        /// <summary>The ID of the digest algorithm, e.g.</summary>
        /// <remarks>The ID of the digest algorithm, e.g. "2.16.840.1.101.3.4.2.1".</remarks>
        private String digestAlgorithmOid;

        /// <summary>The object that will create the digest</summary>
        private IDigest messageDigest;

        /// <summary>The digest algorithms</summary>
        private ICollection<String> digestalgos;

        /// <summary>The digest attributes</summary>
        private byte[] digestAttr;

        private PdfName filterSubtype;

        // Message digest algorithm
        /// <summary>Getter for the ID of the digest algorithm, e.g.</summary>
        /// <remarks>Getter for the ID of the digest algorithm, e.g. "2.16.840.1.101.3.4.2.1"</remarks>
        public virtual String GetDigestAlgorithmOid() {
            return digestAlgorithmOid;
        }

        /// <summary>Returns the name of the digest algorithm, e.g.</summary>
        /// <remarks>Returns the name of the digest algorithm, e.g. "SHA256".</remarks>
        /// <returns>the digest algorithm name, e.g. "SHA256"</returns>
        public virtual String GetHashAlgorithm() {
            return DigestAlgorithms.GetDigest(digestAlgorithmOid);
        }

        /// <summary>The encryption algorithm.</summary>
        private String digestEncryptionAlgorithmOid;

        // Encryption algorithm
        /// <summary>Getter for the digest encryption algorithm</summary>
        public virtual String GetDigestEncryptionAlgorithmOid() {
            return digestEncryptionAlgorithmOid;
        }

        /// <summary>Get the algorithm used to calculate the message digest, e.g.</summary>
        /// <remarks>Get the algorithm used to calculate the message digest, e.g. "SHA1withRSA".</remarks>
        /// <returns>the algorithm used to calculate the message digest</returns>
        public virtual String GetDigestAlgorithm() {
            return GetHashAlgorithm() + "with" + GetEncryptionAlgorithm();
        }

        /// <summary>The signed digest if created outside this class</summary>
        private byte[] externalDigest;

        /// <summary>External RSA data</summary>
        private byte[] externalRSAdata;

        /*
        *	DIGITAL SIGNATURE CREATION
        */
        // The signature is created externally
        /// <summary>Sets the digest/signature to an external calculated value.</summary>
        /// <param name="digest">the digest. This is the actual signature</param>
        /// <param name="RSAdata">the extra data that goes into the data tag in PKCS#7</param>
        /// <param name="digestEncryptionAlgorithm">
        /// the encryption algorithm. It may must be <CODE>null</CODE> if the <CODE>digest</CODE>
        /// is also <CODE>null</CODE>. If the <CODE>digest</CODE> is not <CODE>null</CODE>
        /// then it may be "RSA" or "DSA"
        /// </param>
        public virtual void SetExternalDigest(byte[] digest, byte[] RSAdata, String digestEncryptionAlgorithm) {
            externalDigest = digest;
            externalRSAdata = RSAdata;
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
                            throw new PdfException(PdfException.UnknownKeyAlgorithm1).SetMessageParams(digestEncryptionAlgorithm);
                        }
                    }
                }
            }
        }

        /// <summary>Class from the Java SDK that provides the functionality of a digital signature algorithm.</summary>
        private ISigner sig;

        /// <summary>The signed digest as calculated by this class (or extracted from an existing PDF)</summary>
        private byte[] digest;

        /// <summary>The RSA data</summary>
        private byte[] RSAdata;

        // The signature is created internally
        // Signing functionality.
        /// <exception cref="Org.BouncyCastle.Security.SecurityUtilityException"/>
        /// <exception cref="Java.Security.NoSuchProviderException"/>
        /// <exception cref="Org.BouncyCastle.Security.InvalidKeyException"/>
        private ISigner InitSignature(ICipherParameters key) {
            ISigner signature = SignUtils.GetSignatureHelper(GetDigestAlgorithm());
            signature.InitSign(key);
            return signature;
        }

        /// <exception cref="Org.BouncyCastle.Security.SecurityUtilityException"/>
        /// <exception cref="Java.Security.NoSuchProviderException"/>
        /// <exception cref="Org.BouncyCastle.Security.InvalidKeyException"/>
        private ISigner InitSignature(AsymmetricKeyParameter key) {
            String digestAlgorithm = GetDigestAlgorithm();
            if (PdfName.Adbe_x509_rsa_sha1.Equals(GetFilterSubtype())) {
                digestAlgorithm = "SHA1withRSA";
            }
            ISigner signature = SignUtils.GetSignatureHelper(digestAlgorithm);
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
        /// <exception cref="Java.Security.SignatureException">on error</exception>
        public virtual void Update(byte[] buf, int off, int len) {
            if (RSAdata != null || digestAttr != null || isTsp) {
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
                Asn1OutputStream dout = new Asn1OutputStream(bOut);
                dout.WriteObject(new DerOctetString(digest));
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
            return GetEncodedPKCS7(null, null, null, null, PdfSigner.CryptoStandard.CMS);
        }

        /// <summary>Gets the bytes for the PKCS7SignedData object.</summary>
        /// <remarks>
        /// Gets the bytes for the PKCS7SignedData object. Optionally the authenticatedAttributes
        /// in the signerInfo can also be set. If either of the parameters is <CODE>null</CODE>, none will be used.
        /// </remarks>
        /// <param name="secondDigest">the digest in the authenticatedAttributes</param>
        /// <returns>the bytes for the PKCS7SignedData object</returns>
        public virtual byte[] GetEncodedPKCS7(byte[] secondDigest) {
            return GetEncodedPKCS7(secondDigest, null, null, null, PdfSigner.CryptoStandard.CMS);
        }

        /// <summary>Gets the bytes for the PKCS7SignedData object.</summary>
        /// <remarks>
        /// Gets the bytes for the PKCS7SignedData object. Optionally the authenticatedAttributes
        /// in the signerInfo can also be set, OR a time-stamp-authority client
        /// may be provided.
        /// </remarks>
        /// <param name="secondDigest">the digest in the authenticatedAttributes</param>
        /// <param name="tsaClient">TSAClient - null or an optional time stamp authority client</param>
        /// <returns>byte[] the bytes for the PKCS7SignedData object</returns>
        public virtual byte[] GetEncodedPKCS7(byte[] secondDigest, ITSAClient tsaClient, byte[] ocsp, ICollection<
            byte[]> crlBytes, PdfSigner.CryptoStandard sigtype) {
            try {
                if (externalDigest != null) {
                    digest = externalDigest;
                    if (RSAdata != null) {
                        RSAdata = externalRSAdata;
                    }
                }
                else {
                    if (externalRSAdata != null && RSAdata != null) {
                        RSAdata = externalRSAdata;
                        sig.Update(RSAdata);
                        digest = sig.GenerateSignature();
                    }
                    else {
                        if (RSAdata != null) {
                            RSAdata = messageDigest.Digest();
                            sig.Update(RSAdata);
                        }
                        digest = sig.GenerateSignature();
                    }
                }
                // Create the set of Hash algorithms
                Asn1EncodableVector digestAlgorithms = new Asn1EncodableVector();
                foreach (Object element in digestalgos) {
                    Asn1EncodableVector algos = new Asn1EncodableVector();
                    algos.Add(new DerObjectIdentifier((String)element));
                    algos.Add(Org.BouncyCastle.Asn1.DerNull.Instance);
                    digestAlgorithms.Add(new DerSequence(algos));
                }
                // Create the contentInfo.
                Asn1EncodableVector v = new Asn1EncodableVector();
                v.Add(new DerObjectIdentifier(SecurityIDs.ID_PKCS7_DATA));
                if (RSAdata != null) {
                    v.Add(new DerTaggedObject(0, new DerOctetString(RSAdata)));
                }
                DerSequence contentinfo = new DerSequence(v);
                // Get all the certificates
                //
                v = new Asn1EncodableVector();
                foreach (Object element in certs) {
                    Asn1InputStream tempstream = new Asn1InputStream(new MemoryStream(((X509Certificate)element).GetEncoded())
                        );
                    v.Add(tempstream.ReadObject());
                }
                DerSet dercertificates = new DerSet(v);
                // Create signerinfo structure.
                //
                Asn1EncodableVector signerinfo = new Asn1EncodableVector();
                // Add the signerInfo version
                //
                signerinfo.Add(new DerInteger(signerversion));
                v = new Asn1EncodableVector();
                v.Add(CertificateInfo.GetIssuer(signCert.GetTbsCertificate()));
                v.Add(new DerInteger(signCert.SerialNumber));
                signerinfo.Add(new DerSequence(v));
                // Add the digestAlgorithm
                v = new Asn1EncodableVector();
                v.Add(new DerObjectIdentifier(digestAlgorithmOid));
                v.Add(Org.BouncyCastle.Asn1.DerNull.Instance);
                signerinfo.Add(new DerSequence(v));
                // add the authenticated attribute if present
                if (secondDigest != null) {
                    signerinfo.Add(new DerTaggedObject(false, 0, GetAuthenticatedAttributeSet(secondDigest, ocsp, crlBytes, sigtype
                        )));
                }
                // Add the digestEncryptionAlgorithm
                v = new Asn1EncodableVector();
                v.Add(new DerObjectIdentifier(digestEncryptionAlgorithmOid));
                v.Add(Org.BouncyCastle.Asn1.DerNull.Instance);
                signerinfo.Add(new DerSequence(v));
                // Add the digest
                signerinfo.Add(new DerOctetString(digest));
                // When requested, go get and add the timestamp. May throw an exception.
                // Added by Martin Brunecky, 07/12/2007 folowing Aiken Sam, 2006-11-15
                // Sam found Adobe expects time-stamped SHA1-1 of the encrypted digest
                if (tsaClient != null) {
                    byte[] tsImprint = tsaClient.GetMessageDigest().Digest(digest);
                    byte[] tsToken = tsaClient.GetTimeStampToken(tsImprint);
                    if (tsToken != null) {
                        Asn1EncodableVector unauthAttributes = BuildUnauthenticatedAttributes(tsToken);
                        if (unauthAttributes != null) {
                            signerinfo.Add(new DerTaggedObject(false, 1, new DerSet(unauthAttributes)));
                        }
                    }
                }
                // Finally build the body out of all the components above
                Asn1EncodableVector body = new Asn1EncodableVector();
                body.Add(new DerInteger(version));
                body.Add(new DerSet(digestAlgorithms));
                body.Add(contentinfo);
                body.Add(new DerTaggedObject(false, 0, dercertificates));
                // Only allow one signerInfo
                body.Add(new DerSet(new DerSequence(signerinfo)));
                // Now we have the body, wrap it in it's PKCS7Signed shell
                // and return it
                //
                Asn1EncodableVector whole = new Asn1EncodableVector();
                whole.Add(new DerObjectIdentifier(SecurityIDs.ID_PKCS7_SIGNED_DATA));
                whole.Add(new DerTaggedObject(0, new DerSequence(body)));
                MemoryStream bOut = new MemoryStream();
                Asn1OutputStream dout = new Asn1OutputStream(bOut);
                dout.WriteObject(new DerSequence(whole));
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
        /// <returns>ASN1EncodableVector</returns>
        /// <exception cref="System.IO.IOException"/>
        private Asn1EncodableVector BuildUnauthenticatedAttributes(byte[] timeStampToken) {
            if (timeStampToken == null) {
                return null;
            }
            // @todo: move this together with the rest of the defintions
            String ID_TIME_STAMP_TOKEN = "1.2.840.113549.1.9.16.2.14";
            // RFC 3161 id-aa-timeStampToken
            Asn1InputStream tempstream = new Asn1InputStream(new MemoryStream(timeStampToken));
            Asn1EncodableVector unauthAttributes = new Asn1EncodableVector();
            Asn1EncodableVector v = new Asn1EncodableVector();
            v.Add(new DerObjectIdentifier(ID_TIME_STAMP_TOKEN));
            // id-aa-timeStampToken
            Asn1Sequence seq = (Asn1Sequence)tempstream.ReadObject();
            v.Add(new DerSet(seq));
            unauthAttributes.Add(new DerSequence(v));
            return unauthAttributes;
        }

        // Authenticated attributes
        /// <summary>When using authenticatedAttributes the authentication process is different.</summary>
        /// <remarks>
        /// When using authenticatedAttributes the authentication process is different.
        /// The document digest is generated and put inside the attribute. The signing is done over the DER encoded
        /// authenticatedAttributes. This method provides that encoding and the parameters must be
        /// exactly the same as in
        /// <see cref="GetEncodedPKCS7(byte[])"/>
        /// .
        /// <p>
        /// A simple example:
        /// <p>
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
        /// <returns>the byte array representation of the authenticatedAttributes ready to be signed</returns>
        public virtual byte[] GetAuthenticatedAttributeBytes(byte[] secondDigest, byte[] ocsp, ICollection<byte[]>
             crlBytes, PdfSigner.CryptoStandard sigtype) {
            try {
                return GetAuthenticatedAttributeSet(secondDigest, ocsp, crlBytes, sigtype).GetEncoded(Org.BouncyCastle.Asn1.Asn1Encodable.Der
                    );
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        /// <summary>
        /// This method provides that encoding and the parameters must be
        /// exactly the same as in
        /// <see cref="GetEncodedPKCS7(byte[])"/>
        /// .
        /// </summary>
        /// <param name="secondDigest">the content digest</param>
        /// <returns>the byte array representation of the authenticatedAttributes ready to be signed</returns>
        private DerSet GetAuthenticatedAttributeSet(byte[] secondDigest, byte[] ocsp, ICollection<byte[]> crlBytes
            , PdfSigner.CryptoStandard sigtype) {
            try {
                Asn1EncodableVector attribute = new Asn1EncodableVector();
                Asn1EncodableVector v = new Asn1EncodableVector();
                v.Add(new DerObjectIdentifier(SecurityIDs.ID_CONTENT_TYPE));
                v.Add(new DerSet(new DerObjectIdentifier(SecurityIDs.ID_PKCS7_DATA)));
                attribute.Add(new DerSequence(v));
                v = new Asn1EncodableVector();
                v.Add(new DerObjectIdentifier(SecurityIDs.ID_MESSAGE_DIGEST));
                v.Add(new DerSet(new DerOctetString(secondDigest)));
                attribute.Add(new DerSequence(v));
                bool haveCrl = false;
                if (crlBytes != null) {
                    foreach (byte[] bCrl in crlBytes) {
                        if (bCrl != null) {
                            haveCrl = true;
                            break;
                        }
                    }
                }
                if (ocsp != null || haveCrl) {
                    v = new Asn1EncodableVector();
                    v.Add(new DerObjectIdentifier(SecurityIDs.ID_ADBE_REVOCATION));
                    Asn1EncodableVector revocationV = new Asn1EncodableVector();
                    if (haveCrl) {
                        Asn1EncodableVector v2 = new Asn1EncodableVector();
                        foreach (byte[] bCrl in crlBytes) {
                            if (bCrl == null) {
                                continue;
                            }
                            Asn1InputStream t = new Asn1InputStream(new MemoryStream(bCrl));
                            v2.Add(t.ReadObject());
                        }
                        revocationV.Add(new DerTaggedObject(true, 0, new DerSequence(v2)));
                    }
                    if (ocsp != null) {
                        DerOctetString doctet = new DerOctetString(ocsp);
                        Asn1EncodableVector vo1 = new Asn1EncodableVector();
                        Asn1EncodableVector v2 = new Asn1EncodableVector();
                        v2.Add(OcspObjectIdentifiers.PkixOcspBasic);
                        v2.Add(doctet);
                        DerEnumerated den = new DerEnumerated(0);
                        Asn1EncodableVector v3 = new Asn1EncodableVector();
                        v3.Add(den);
                        v3.Add(new DerTaggedObject(true, 0, new DerSequence(v2)));
                        vo1.Add(new DerSequence(v3));
                        revocationV.Add(new DerTaggedObject(true, 1, new DerSequence(vo1)));
                    }
                    v.Add(new DerSet(new DerSequence(revocationV)));
                    attribute.Add(new DerSequence(v));
                }
                if (sigtype == PdfSigner.CryptoStandard.CADES) {
                    v = new Asn1EncodableVector();
                    v.Add(new DerObjectIdentifier(SecurityIDs.ID_AA_SIGNING_CERTIFICATE_V2));
                    Asn1EncodableVector aaV2 = new Asn1EncodableVector();
                    AlgorithmIdentifier algoId = new AlgorithmIdentifier(new DerObjectIdentifier(digestAlgorithmOid), null);
                    aaV2.Add(algoId);
                    IDigest md = SignUtils.GetMessageDigest(GetHashAlgorithm());
                    byte[] dig = md.Digest(signCert.GetEncoded());
                    aaV2.Add(new DerOctetString(dig));
                    v.Add(new DerSet(new DerSequence(new DerSequence(new DerSequence(aaV2)))));
                    attribute.Add(new DerSequence(v));
                }
                if (signaturePolicyIdentifier != null) {
                    attribute.Add(new Org.BouncyCastle.Asn1.Cms.Attribute(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdAAEtsSigPolicyID
                        , new DerSet(signaturePolicyIdentifier)));
                }
                return new DerSet(attribute);
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        /// <summary>Signature attributes</summary>
        private byte[] sigAttr;

        /// <summary>Signature attributes (maybe not necessary, but we use it as fallback)</summary>
        private byte[] sigAttrDer;

        /// <summary>encrypted digest</summary>
        private IDigest encContDigest;

        /// <summary>Indicates if a signature has already been verified</summary>
        private bool verified;

        /// <summary>The result of the verification</summary>
        private bool verifyResult;

        /*
        *	DIGITAL SIGNATURE VERIFICATION
        */
        // Stefan Santesson
        // verification
        /// <summary>Verify the digest.</summary>
        /// <returns><CODE>true</CODE> if the signature checks out, <CODE>false</CODE> otherwise</returns>
        /// <exception cref="Java.Security.SignatureException">on error</exception>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        public virtual bool Verify() {
            if (verified) {
                return verifyResult;
            }
            if (isTsp) {
                TimeStampTokenInfo info = timeStampToken.TimeStampInfo;
                MessageImprint imprint = info.TstInfo.MessageImprint;
                byte[] md = messageDigest.Digest();
                byte[] imphashed = imprint.GetHashedMessage();
                verifyResult = iText.IO.Util.JavaUtil.ArraysEquals(md, imphashed);
            }
            else {
                if (sigAttr != null || sigAttrDer != null) {
                    byte[] msgDigestBytes = messageDigest.Digest();
                    bool verifyRSAdata = true;
                    // Stefan Santesson fixed a bug, keeping the code backward compatible
                    bool encContDigestCompare = false;
                    if (RSAdata != null) {
                        verifyRSAdata = iText.IO.Util.JavaUtil.ArraysEquals(msgDigestBytes, RSAdata);
                        encContDigest.Update(RSAdata);
                        encContDigestCompare = iText.IO.Util.JavaUtil.ArraysEquals(encContDigest.Digest(), digestAttr);
                    }
                    bool absentEncContDigestCompare = iText.IO.Util.JavaUtil.ArraysEquals(msgDigestBytes, digestAttr);
                    bool concludingDigestCompare = absentEncContDigestCompare || encContDigestCompare;
                    bool sigVerify = VerifySigAttributes(sigAttr) || VerifySigAttributes(sigAttrDer);
                    verifyResult = concludingDigestCompare && sigVerify && verifyRSAdata;
                }
                else {
                    if (RSAdata != null) {
                        sig.Update(messageDigest.Digest());
                    }
                    verifyResult = sig.VerifySignature(digest);
                }
            }
            verified = true;
            return verifyResult;
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        private bool VerifySigAttributes(byte[] attr) {
            ISigner signature = InitSignature(signCert.GetPublicKey());
            signature.Update(attr);
            return signature.VerifySignature(digest);
        }

        /// <summary>Checks if the timestamp refers to this document.</summary>
        /// <returns>true if it checks false otherwise</returns>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException">on error</exception>
        public virtual bool VerifyTimestampImprint() {
            if (timeStampToken == null) {
                return false;
            }
            TimeStampTokenInfo info = timeStampToken.TimeStampInfo;
            MessageImprint imprint = info.TstInfo.MessageImprint;
            String algOID = info.HashAlgorithm.ObjectID.Id;
            byte[] md = SignUtils.GetMessageDigest(DigestAlgorithms.GetDigest(algOID)).Digest(digest);
            byte[] imphashed = imprint.GetHashedMessage();
            return iText.IO.Util.JavaUtil.ArraysEquals(md, imphashed);
        }

        /// <summary>All the X.509 certificates in no particular order.</summary>
        private ICollection<X509Certificate> certs;

        /// <summary>All the X.509 certificates used for the main signature.</summary>
        private ICollection<X509Certificate> signCerts;

        /// <summary>The X.509 certificate that is used to sign the digest.</summary>
        private X509Certificate signCert;

        // Certificates
        /// <summary>Get all the X.509 certificates associated with this PKCS#7 object in no particular order.</summary>
        /// <remarks>
        /// Get all the X.509 certificates associated with this PKCS#7 object in no particular order.
        /// Other certificates, from OCSP for example, will also be included.
        /// </remarks>
        /// <returns>the X.509 certificates associated with this PKCS#7 object</returns>
        public virtual X509Certificate[] GetCertificates() {
            return certs.ToArray(new X509Certificate[certs.Count]);
        }

        /// <summary>Get the X.509 sign certificate chain associated with this PKCS#7 object.</summary>
        /// <remarks>
        /// Get the X.509 sign certificate chain associated with this PKCS#7 object.
        /// Only the certificates used for the main signature will be returned, with
        /// the signing certificate first.
        /// </remarks>
        /// <returns>the X.509 certificates associated with this PKCS#7 object</returns>
        public virtual X509Certificate[] GetSignCertificateChain() {
            return signCerts.ToArray(new X509Certificate[signCerts.Count]);
        }

        /// <summary>Get the X.509 certificate actually used to sign the digest.</summary>
        /// <returns>the X.509 certificate actually used to sign the digest</returns>
        public virtual X509Certificate GetSigningCertificate() {
            return signCert;
        }

        /// <summary>
        /// Helper method that creates the collection of certificates
        /// used for the main signature based on the complete list
        /// of certificates and the sign certificate.
        /// </summary>
        private void SignCertificateChain() {
            IList<X509Certificate> cc = new List<X509Certificate>();
            cc.Add(signCert);
            IList<X509Certificate> oc = new List<X509Certificate>(certs);
            for (int k = 0; k < oc.Count; ++k) {
                if (signCert.Equals(oc[k])) {
                    oc.JRemoveAt(k);
                    --k;
                }
            }
            bool found = true;
            while (found) {
                X509Certificate v = (X509Certificate)cc[cc.Count - 1];
                found = false;
                for (int k = 0; k < oc.Count; ++k) {
                    X509Certificate issuer = (X509Certificate)oc[k];
                    try {
                        v.Verify(issuer.GetPublicKey());
                        found = true;
                        cc.Add(oc[k]);
                        oc.JRemoveAt(k);
                        break;
                    }
                    catch (Exception) {
                    }
                }
            }
            signCerts = cc;
        }

        private ICollection<X509Crl> crls;

        // Certificate Revocation Lists
        /// <summary>Get the X.509 certificate revocation lists associated with this PKCS#7 object</summary>
        /// <returns>the X.509 certificate revocation lists associated with this PKCS#7 object</returns>
        public virtual ICollection<X509Crl> GetCRLs() {
            return crls;
        }

        /// <summary>Helper method that tries to construct the CRLs.</summary>
        private void FindCRL(Asn1Sequence seq) {
            try {
                crls = new List<X509Crl>();
                for (int k = 0; k < seq.Count; ++k) {
                    MemoryStream ar = new MemoryStream(seq[k].ToAsn1Object().GetEncoded(Org.BouncyCastle.Asn1.Asn1Encodable.Der
                        ));
                    X509Crl crl = (X509Crl)SignUtils.ParseCrlFromStream(ar);
                    crls.Add(crl);
                }
            }
            catch (Exception) {
            }
        }

        /// <summary>BouncyCastle BasicOCSPResp</summary>
        private BasicOcspResp basicResp;

        // ignore
        // Online Certificate Status Protocol
        /// <summary>Gets the OCSP basic response if there is one.</summary>
        /// <returns>the OCSP basic response or null</returns>
        public virtual BasicOcspResp GetOcsp() {
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
                X509Certificate[] cs = (X509Certificate[])GetSignCertificateChain();
                SingleResp sr = basicResp.Responses[0];
                CertificateID cid = sr.GetCertID();
                X509Certificate sigcer = GetSigningCertificate();
                X509Certificate isscer = cs[1];
                CertificateID tis = SignUtils.GenerateCertificateId(isscer, sigcer.SerialNumber, cid.HashAlgOid);
                return tis.Equals(cid);
            }
            catch (Exception) {
            }
            return false;
        }

        /// <summary>Helper method that creates the BasicOCSPResp object.</summary>
        /// <param name="seq"/>
        /// <exception cref="System.IO.IOException"/>
        private void FindOcsp(Asn1Sequence seq) {
            basicResp = (BasicOcspResp)null;
            bool ret = false;
            while (true) {
                if (seq[0] is DerObjectIdentifier && ((DerObjectIdentifier)seq[0]).Id.Equals(OcspObjectIdentifiers.PkixOcspBasic
                    .Id)) {
                    break;
                }
                ret = true;
                for (int k = 0; k < seq.Count; ++k) {
                    if (seq[k] is Asn1Sequence) {
                        seq = (Asn1Sequence)seq[0];
                        ret = false;
                        break;
                    }
                    if (seq[k] is Asn1TaggedObject) {
                        Asn1TaggedObject tag = (Asn1TaggedObject)seq[k];
                        if (tag.GetObject() is Asn1Sequence) {
                            seq = (Asn1Sequence)tag.GetObject();
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
            Asn1OctetString os = (Asn1OctetString)seq[1];
            Asn1InputStream inp = new Asn1InputStream(os.GetOctets());
            BasicOcspResponse resp = BasicOcspResponse.GetInstance(inp.ReadObject());
            basicResp = new BasicOcspResp(resp);
        }

        /// <summary>True if there's a PAdES LTV time stamp.</summary>
        private bool isTsp;

        /// <summary>True if it's a CAdES signature type.</summary>
        private bool isCades;

        /// <summary>BouncyCastle TimeStampToken.</summary>
        private TimeStampToken timeStampToken;

        // Time Stamps
        /// <summary>Check if it's a PAdES-LTV time stamp.</summary>
        /// <returns>true if it's a PAdES-LTV time stamp, false otherwise</returns>
        public virtual bool IsTsp() {
            return isTsp;
        }

        /// <summary>Gets the timestamp token if there is one.</summary>
        /// <returns>the timestamp token or null</returns>
        public virtual TimeStampToken GetTimeStampToken() {
            return timeStampToken;
        }

        /// <summary>Gets the timestamp date</summary>
        /// <returns>a date</returns>
        public virtual DateTime GetTimeStampDate() {
            if (timeStampToken == null) {
                return (DateTime)SignUtils.UNDEFINED_TIMESTAMP_DATE;
            }
            return SignUtils.GetTimeStampDate(timeStampToken);
        }

        /// <summary>Returns the filter subtype.</summary>
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
