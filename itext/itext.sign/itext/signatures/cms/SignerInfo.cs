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
using System.Collections.Generic;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures;
using iText.Signatures.Exceptions;

namespace iText.Signatures.Cms {
    /// <summary>
    /// This class represents the SignerInfo structure from
    /// <a href="https://datatracker.ietf.org/doc/html/rfc5652#section-5.3">rfc5652   Cryptographic Message Syntax (CMS)</a>
    /// </summary>
    public class SignerInfo {
        private static readonly IBouncyCastleFactory BC_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private const int DEFAULT_SIGNATURE_SIZE = 1024;

        private AlgorithmIdentifier digestAlgorithm;

        private AlgorithmIdentifier signingAlgorithm;

        private readonly ICollection<CmsAttribute> signedAttributes = new List<CmsAttribute>();

        private readonly ICollection<CmsAttribute> unSignedAttributes;

        private byte[] serializedSignedAttributes;

        private ICollection<byte[]> ocspResponses;

        private ICollection<byte[]> crlResponses;

        private byte[] signatureData;

        private bool signedAttributesReadOnly;

        private IX509Certificate signerCertificate;

        /// <summary>Creates an empty SignerInfo structure.</summary>
        public SignerInfo() {
            CmsAttribute contentType = new CmsAttribute(SecurityIDs.ID_CONTENT_TYPE, BC_FACTORY.CreateDERSet(BC_FACTORY
                .CreateASN1ObjectIdentifier(SecurityIDs.ID_PKCS7_DATA)));
            signedAttributes.Add(contentType);
            unSignedAttributes = new List<CmsAttribute>();
        }

        /// <summary>Creates a SignerInfo structure from an ASN1 structure.</summary>
        /// <param name="signerInfoStructure">the ASN1 structure containing signerInfo</param>
        /// <param name="certificates">the certificates of the CMS, it should contain the signing certificate</param>
        public SignerInfo(IAsn1Encodable signerInfoStructure, ICollection<IX509Certificate> certificates) {
            int index = 0;
            try {
                IAsn1Sequence signerInfoSeq = BC_FACTORY.CreateASN1Sequence(signerInfoStructure);
                IDerInteger version = BC_FACTORY.CreateASN1Integer(signerInfoSeq.GetObjectAt(index++));
                if (version.GetValue().GetIntValue() == 1) {
                    ProcessIssuerAndSerialNumberSignerCertificate(signerInfoSeq.GetObjectAt(index++), certificates);
                }
                else {
                    ProcessSubjectKeyIdentifierSignerCertificate(signerInfoSeq.GetObjectAt(index++), certificates);
                }
                digestAlgorithm = new AlgorithmIdentifier(signerInfoSeq.GetObjectAt(index++));
                IAsn1TaggedObject taggedSingedAttributes = BC_FACTORY.CreateASN1TaggedObject(signerInfoSeq.GetObjectAt(index
                    ));
                if (taggedSingedAttributes != null) {
                    index++;
                    SetSerializedSignedAttributes(BC_FACTORY.CreateASN1Set(taggedSingedAttributes, false).GetEncoded(BC_FACTORY
                        .CreateASN1Encoding().GetDer()));
                }
                signingAlgorithm = new AlgorithmIdentifier(signerInfoSeq.GetObjectAt(index++));
                IDerOctetString signatureDataOS = BC_FACTORY.CreateDEROctetString(signerInfoSeq.GetObjectAt(index++));
                if (signatureDataOS != null) {
                    signatureData = signatureDataOS.GetOctets();
                }
                if (signerInfoSeq.Size() > index) {
                    IAsn1TaggedObject taggedUnsingedAttributes = BC_FACTORY.CreateASN1TaggedObject(signerInfoSeq.GetObjectAt(index
                        ));
                    unSignedAttributes = ProcessAttributeSet(BC_FACTORY.CreateASN1Set(taggedUnsingedAttributes, false));
                }
                else {
                    unSignedAttributes = new List<CmsAttribute>();
                }
            }
            catch (NullReferenceException npe) {
                throw new PdfException(SignExceptionMessageConstant.CMS_INVALID_CONTAINER_STRUCTURE, npe);
            }
        }

        /// <summary>Returns the algorithmId to create the digest of the data to sign.</summary>
        /// <returns>the OID of the digest algorithm.</returns>
        public virtual AlgorithmIdentifier GetDigestAlgorithm() {
            return digestAlgorithm;
        }

        /// <summary>Sets the algorithmId to create the digest of the data to sign.</summary>
        /// <param name="algorithmId">the OID of the algorithm</param>
        public virtual void SetDigestAlgorithm(AlgorithmIdentifier algorithmId) {
            digestAlgorithm = algorithmId;
        }

        /// <summary>Adds or replaces the message digest signed attribute.</summary>
        /// <param name="digest">ASN.1 type MessageDigest</param>
        public virtual void SetMessageDigest(byte[] digest) {
            if (signedAttributesReadOnly) {
                throw new InvalidOperationException(SignExceptionMessageConstant.CMS_SIGNERINFO_READONLY);
            }
            CmsAttribute digestAttribute = new CmsAttribute(SecurityIDs.ID_MESSAGE_DIGEST, BC_FACTORY.CreateDERSet(BC_FACTORY
                .CreateDEROctetString(digest)));
            signedAttributes.Add(digestAttribute);
        }

        /// <summary>Sets the certificate that is used to sign.</summary>
        /// <param name="certificate">the certificate that is used to sign</param>
        public virtual void SetSigningCertificate(IX509Certificate certificate) {
            this.signerCertificate = certificate;
            ITbsCertificateStructure tbsCert = BC_FACTORY.CreateTBSCertificate(certificate.GetTbsCertificate());
            if (signingAlgorithm != null) {
                return;
            }
            if (tbsCert.GetSubjectPublicKeyInfo().GetAlgorithm().GetParameters() != null) {
                if (tbsCert.GetSubjectPublicKeyInfo().GetAlgorithm().GetParameters().IsNull()) {
                    this.signingAlgorithm = new AlgorithmIdentifier(tbsCert.GetSubjectPublicKeyInfo().GetAlgorithm().GetAlgorithm
                        ().GetId(), BC_FACTORY.CreateDERNull());
                    return;
                }
                this.signingAlgorithm = new AlgorithmIdentifier(tbsCert.GetSubjectPublicKeyInfo().GetAlgorithm().GetAlgorithm
                    ().GetId(), tbsCert.GetSubjectPublicKeyInfo().GetAlgorithm().GetParameters().ToASN1Primitive());
                return;
            }
            this.signingAlgorithm = new AlgorithmIdentifier(tbsCert.GetSubjectPublicKeyInfo().GetAlgorithm().GetAlgorithm
                ().GetId());
        }

        /// <summary>Gets the certificate that is used to sign.</summary>
        /// <returns>the certificate that is used to sign.</returns>
        public virtual IX509Certificate GetSigningCertificate() {
            return signerCertificate;
        }

        /// <summary>Sets the certificate that is used to sign a document and adds it to the signed attributes.</summary>
        /// <param name="certificate">the certificate that is used to sign</param>
        /// <param name="digestAlgorithmOid">the oid of the digest algorithm to be added to the signed attributes</param>
        public virtual void SetSigningCertificateAndAddToSignedAttributes(IX509Certificate certificate, String digestAlgorithmOid
            ) {
            SetSigningCertificate(certificate);
            AddSignerCertificateToSignedAttributes(certificate, digestAlgorithmOid);
        }

        /// <summary>Adds a set of OCSP responses as signed attributes.</summary>
        /// <param name="ocspResponses">a set of binary representations of OCSP responses.</param>
        public virtual void SetOcspResponses(ICollection<byte[]> ocspResponses) {
            if (signedAttributesReadOnly) {
                throw new InvalidOperationException(SignExceptionMessageConstant.CMS_SIGNERINFO_READONLY);
            }
            this.ocspResponses = JavaCollectionsUtil.UnmodifiableCollection(ocspResponses);
            SetRevocationInfo();
        }

        /// <summary>Adds a set of CRL responses as signed attributes.</summary>
        /// <param name="crlResponses">a set of binary representations of CRL responses.</param>
        public virtual void SetCrlResponses(ICollection<byte[]> crlResponses) {
            if (signedAttributesReadOnly) {
                throw new InvalidOperationException(SignExceptionMessageConstant.CMS_SIGNERINFO_READONLY);
            }
            this.crlResponses = JavaCollectionsUtil.UnmodifiableCollection(crlResponses);
            SetRevocationInfo();
        }

        /// <summary>Adds the signer certificate to the signed attributes as a SigningCertificateV2 structure.</summary>
        /// <param name="cert">the certificate to add</param>
        /// <param name="digestAlgorithmOid">the digest algorithm oid that will be used</param>
        public virtual void AddSignerCertificateToSignedAttributes(IX509Certificate cert, String digestAlgorithmOid
            ) {
            if (signedAttributesReadOnly) {
                throw new InvalidOperationException(SignExceptionMessageConstant.CMS_SIGNERINFO_READONLY);
            }
            IMessageDigest md = DigestAlgorithms.GetMessageDigestFromOid(digestAlgorithmOid);
            IAsn1EncodableVector certContents = BC_FACTORY.CreateASN1EncodableVector();
            // don't add if it is the default value
            if (!SecurityIDs.ID_SHA256.Equals(digestAlgorithmOid)) {
                IAlgorithmIdentifier algoId = BC_FACTORY.CreateAlgorithmIdentifier(BC_FACTORY.CreateASN1ObjectIdentifier(digestAlgorithmOid
                    ));
                certContents.Add(algoId);
            }
            byte[] dig = md.Digest(cert.GetEncoded());
            certContents.Add(BC_FACTORY.CreateDEROctetString(dig));
            IAsn1Sequence issuerName = BC_FACTORY.CreateASN1Sequence(CertificateInfo.GetIssuer(cert.GetTbsCertificate(
                )));
            IDerTaggedObject issuerTagged = BC_FACTORY.CreateDERTaggedObject(true, 4, issuerName);
            IDerSequence issuer = BC_FACTORY.CreateDERSequence(issuerTagged);
            IDerInteger serial = BC_FACTORY.CreateASN1Integer(cert.GetSerialNumber());
            IAsn1EncodableVector v = BC_FACTORY.CreateASN1EncodableVector();
            v.Add(issuer);
            v.Add(serial);
            IDerSequence issuerS = BC_FACTORY.CreateDERSequence(v);
            certContents.Add(issuerS);
            IDerSequence certContentsSeq = BC_FACTORY.CreateDERSequence(certContents);
            IDerSequence certContentsSeqSeq = BC_FACTORY.CreateDERSequence(certContentsSeq);
            IDerSequence certContentsSeqSeqSeq = BC_FACTORY.CreateDERSequence(certContentsSeqSeq);
            IDerSet certContentsSeqSeqSeqSet = BC_FACTORY.CreateDERSet(certContentsSeqSeqSeq);
            CmsAttribute attribute = new CmsAttribute(SecurityIDs.ID_AA_SIGNING_CERTIFICATE_V2, certContentsSeqSeqSeqSet
                );
            signedAttributes.Add(attribute);
        }

        /// <summary>Sets the actual signature.</summary>
        /// <param name="signatureData">a byte array containing the signature</param>
        public virtual void SetSignature(byte[] signatureData) {
            this.signatureData = JavaUtil.ArraysCopyOf(signatureData, signatureData.Length);
        }

        /// <summary>Optional.</summary>
        /// <remarks>
        /// Optional.
        /// Sets the OID and parameters of the algorithm that will be used to create the signature.
        /// This will be overwritten when setting the signing certificate.
        /// </remarks>
        /// <param name="algorithm">The OID and parameters of the algorithm that will be used to create the signature.
        ///     </param>
        public virtual void SetSignatureAlgorithm(AlgorithmIdentifier algorithm) {
            this.signingAlgorithm = algorithm;
        }

        /// <summary>Value 0 when no signerIdentifier is available.</summary>
        /// <remarks>
        /// Value 0 when no signerIdentifier is available.
        /// Value 1 when signerIdentifier is of type issuerAndSerialNumber.
        /// Value 3 when signerIdentifier is of type subjectKeyIdentifier.
        /// </remarks>
        /// <returns>CMS version.</returns>
        public virtual int GetCmsVersion() {
            return 1;
        }

        /// <summary>Optional.</summary>
        /// <remarks>
        /// Optional.
        /// <para />
        /// Attributes that should be part of the signed content
        /// optional, but it MUST be present if the content type of
        /// the EncapsulatedContentInfo value being signed is not id-data.
        /// In that case it must at least contain the following two attributes:
        /// <para />
        /// A content-type attribute having as its value the content type
        /// of the EncapsulatedContentInfo value being signed.  Section
        /// 11.1 defines the content-type attribute.  However, the
        /// content-type attribute MUST NOT be used as part of a
        /// countersignature unsigned attribute as defined in Section 11.4.
        /// <para />
        /// A message-digest attribute, having as its value the message
        /// digest of the content.  Section 11.2 defines the message-digest
        /// attribute.
        /// </remarks>
        /// <returns>collection of the signed attributes.</returns>
        public virtual ICollection<CmsAttribute> GetSignedAttributes() {
            return JavaCollectionsUtil.UnmodifiableCollection(signedAttributes);
        }

        /// <summary>Adds a new attribute to the signed attributes.</summary>
        /// <remarks>
        /// Adds a new attribute to the signed attributes.
        /// This become readonly after retrieving the serialized version
        /// <see cref="SerializeSignedAttributes()"/>.
        /// </remarks>
        /// <param name="attribute">the attribute to add</param>
        public virtual void AddSignedAttribute(CmsAttribute attribute) {
            if (signedAttributesReadOnly) {
                throw new InvalidOperationException(SignExceptionMessageConstant.CMS_SIGNERINFO_READONLY);
            }
            signedAttributes.Add(attribute);
        }

        /// <summary>Retrieves the optional unsigned attributes.</summary>
        /// <returns>the optional unsigned attributes.</returns>
        public virtual ICollection<CmsAttribute> GetUnSignedAttributes() {
            return JavaCollectionsUtil.UnmodifiableCollection(unSignedAttributes);
        }

        /// <summary>Optional.</summary>
        /// <remarks>
        /// Optional.
        /// <para />
        /// Adds attribute that should not or can not be part of the signed content.
        /// </remarks>
        /// <param name="attribute">the attribute to add</param>
        public virtual void AddUnSignedAttribute(CmsAttribute attribute) {
            unSignedAttributes.Add(attribute);
        }

        /// <summary>Retrieves the encoded signed attributes of the signer info.</summary>
        /// <remarks>
        /// Retrieves the encoded signed attributes of the signer info.
        /// This makes the signed attributes read only.
        /// </remarks>
        /// <returns>the encoded signed attributes of the signer info.</returns>
        public virtual byte[] SerializeSignedAttributes() {
            if (!signedAttributesReadOnly) {
                IDerSet derView = GetAttributesAsDERSet(signedAttributes);
                serializedSignedAttributes = derView.GetEncoded(BC_FACTORY.CreateASN1Encoding().GetDer());
                signedAttributesReadOnly = true;
            }
            return JavaUtil.ArraysCopyOf(serializedSignedAttributes, serializedSignedAttributes.Length);
        }

        /// <summary>Sets the signed attributes from a serialized version.</summary>
        /// <remarks>
        /// Sets the signed attributes from a serialized version.
        /// This makes the signed attributes read only.
        /// </remarks>
        /// <param name="serializedSignedAttributes">the encoded signed attributes.</param>
        public void SetSerializedSignedAttributes(byte[] serializedSignedAttributes) {
            if (signedAttributesReadOnly) {
                throw new InvalidOperationException(SignExceptionMessageConstant.CMS_SIGNERINFO_READONLY);
            }
            this.signedAttributesReadOnly = true;
            this.serializedSignedAttributes = JavaUtil.ArraysCopyOf(serializedSignedAttributes, serializedSignedAttributes
                .Length);
            try {
                signedAttributes.Clear();
                this.signedAttributes.AddAll(ProcessAttributeSet(BC_FACTORY.CreateASN1Primitive(serializedSignedAttributes
                    )));
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
        }

        /// <summary>Calculates an estimate size for the SignerInfo structure.</summary>
        /// <remarks>
        /// Calculates an estimate size for the SignerInfo structure.
        /// This takes into account the values added including the signature, but does not account for unset items like
        /// a timestamp response added after actual signing.
        /// </remarks>
        /// <returns>the estimated size of the structure.</returns>
        public virtual long GetEstimatedSize() {
            IDerSequence derView = GetAsDerSequence(true);
            byte[] temp = derView.GetEncoded(BC_FACTORY.CreateASN1Encoding().GetDer());
            return temp.Length;
        }

        /// <summary>Serializes the SignerInfo structure and makes the signed attributes readonly.</summary>
        /// <returns>the encoded SignerInfo structure.</returns>
        public virtual IDerSequence GetAsDerSequence() {
            return GetAsDerSequence(false);
        }

        /// <summary>Serializes the SignerInfo structure and makes the signed attributes readonly.</summary>
        /// <remarks>
        /// Serializes the SignerInfo structure and makes the signed attributes readonly.
        /// With the possibility to skip making the signed attributes read only for estimation purposes.
        /// </remarks>
        /// <param name="estimationRun">set to true to not make signed attributes read only</param>
        /// <returns>the encoded SignerInfo structure.</returns>
        internal virtual IDerSequence GetAsDerSequence(bool estimationRun) {
            IAsn1EncodableVector signerInfoV = BC_FACTORY.CreateASN1EncodableVector();
            // version
            signerInfoV.Add(BC_FACTORY.CreateASN1Integer(GetCmsVersion()));
            // sid
            IAsn1EncodableVector issuerAndSerialNumberV = BC_FACTORY.CreateASN1EncodableVector();
            issuerAndSerialNumberV.Add(CertificateInfo.GetIssuer(signerCertificate.GetTbsCertificate()));
            issuerAndSerialNumberV.Add(BC_FACTORY.CreateASN1Integer(signerCertificate.GetSerialNumber()));
            signerInfoV.Add(BC_FACTORY.CreateDERSequence(issuerAndSerialNumberV));
            // digest algorithm
            IAsn1EncodableVector digestalgorithmV = BC_FACTORY.CreateASN1EncodableVector();
            digestalgorithmV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(this.digestAlgorithm.GetAlgorithmOid()));
            digestalgorithmV.AddOptional(digestAlgorithm.GetParameters());
            signerInfoV.Add(BC_FACTORY.CreateDERSequence(digestalgorithmV));
            // signed attributes
            if (!signedAttributes.IsEmpty() || signedAttributesReadOnly) {
                if (estimationRun || !signedAttributesReadOnly) {
                    signerInfoV.Add(BC_FACTORY.CreateDERTaggedObject(false, 0, GetAttributesAsDERSet(signedAttributes)));
                }
                else {
                    try {
                        using (IAsn1InputStream saIS = BC_FACTORY.CreateASN1InputStream(serializedSignedAttributes)) {
                            signerInfoV.Add(BC_FACTORY.CreateDERTaggedObject(false, 0, saIS.ReadObject()));
                        }
                    }
                    catch (System.IO.IOException e) {
                        throw new PdfException(e);
                    }
                }
            }
            // signatureAlgorithm
            if (signingAlgorithm != null) {
                IAsn1EncodableVector signatureAlgorithmV = BC_FACTORY.CreateASN1EncodableVector();
                signatureAlgorithmV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(signingAlgorithm.GetAlgorithmOid()));
                signatureAlgorithmV.AddOptional(signingAlgorithm.GetParameters());
                signerInfoV.Add(BC_FACTORY.CreateDERSequence(signatureAlgorithmV));
            }
            // signatureValue
            byte[] workingSignatureData;
            if (signatureData == null) {
                workingSignatureData = new byte[DEFAULT_SIGNATURE_SIZE];
            }
            else {
                workingSignatureData = signatureData;
            }
            IAsn1OctetString signatureDataOS = BC_FACTORY.CreateDEROctetString(workingSignatureData);
            signerInfoV.Add(signatureDataOS);
            // UnsignedAttributes
            if (!unSignedAttributes.IsEmpty()) {
                signerInfoV.Add(BC_FACTORY.CreateDERTaggedObject(false, 1, GetAttributesAsDERSet(unSignedAttributes)));
            }
            return BC_FACTORY.CreateDERSequence(signerInfoV);
        }

        private void ProcessSubjectKeyIdentifierSignerCertificate(IAsn1Encodable asnStruct, ICollection<IX509Certificate
            > certificates) {
            IAsn1OctetString subjectKeyIdentifierOs = BC_FACTORY.CreateASN1OctetString(BC_FACTORY.CreateASN1TaggedObject
                (asnStruct).GetObject());
            using (IAsn1InputStream aIn = BC_FACTORY.CreateASN1InputStream(new MemoryStream(subjectKeyIdentifierOs.GetOctets
                ()))) {
                IAsn1Object subjectKeyIdentifier = aIn.ReadObject();
                foreach (IX509Certificate certificate in certificates) {
                    IAsn1Object ski = CertificateUtil.GetExtensionValue(certificate, OID.X509Extensions.SUBJECT_KEY_IDENTIFIER
                        );
                    if (ski.Equals(subjectKeyIdentifier)) {
                        this.signerCertificate = certificate;
                        return;
                    }
                }
            }
            throw new PdfException(SignExceptionMessageConstant.CMS_CERTIFICATE_NOT_FOUND);
        }

        private void ProcessIssuerAndSerialNumberSignerCertificate(IAsn1Encodable asnStruct, ICollection<IX509Certificate
            > certificates) {
            IAsn1Sequence signIdSeq = BC_FACTORY.CreateASN1Sequence(asnStruct);
            IDerInteger serial = BC_FACTORY.CreateASN1Integer(signIdSeq.GetObjectAt(1));
            foreach (IX509Certificate certificate in certificates) {
                if (certificate.GetSerialNumber().Equals(serial.GetValue())) {
                    this.signerCertificate = certificate;
                    break;
                }
            }
            if (signerCertificate == null) {
                throw new PdfException(SignExceptionMessageConstant.CMS_CERTIFICATE_NOT_FOUND);
            }
        }

        private static ICollection<CmsAttribute> ProcessAttributeSet(IAsn1Encodable asnStruct) {
            IAsn1Set usaSet = BC_FACTORY.CreateASN1Set(asnStruct);
            ICollection<CmsAttribute> attributes = new List<CmsAttribute>(usaSet.Size());
            for (int i = 0; i < usaSet.Size(); i++) {
                IAsn1Sequence attrSeq = BC_FACTORY.CreateASN1Sequence(usaSet.GetObjectAt(i));
                IDerObjectIdentifier attrType = BC_FACTORY.CreateASN1ObjectIdentifier(attrSeq.GetObjectAt(0));
                IAsn1Object attrVal = BC_FACTORY.CreateASN1Primitive(attrSeq.GetObjectAt(1));
                attributes.Add(new CmsAttribute(attrType.GetId(), attrVal));
            }
            return attributes;
        }

        private void SetRevocationInfo() {
            signedAttributes.RemoveIf((a) => SecurityIDs.ID_ADBE_REVOCATION.Equals(a.GetType()));
            if (ContainsRevocationData()) {
                IAsn1EncodableVector revocationV = BC_FACTORY.CreateASN1EncodableVector();
                CreateCRLStructure(revocationV);
                CreateOCPSStructure(revocationV);
                CmsAttribute digestAttribute = new CmsAttribute(SecurityIDs.ID_ADBE_REVOCATION, BC_FACTORY.CreateDERSequence
                    (revocationV));
                signedAttributes.Add(digestAttribute);
            }
        }

        private void CreateCRLStructure(IAsn1EncodableVector revocationV) {
            if (crlResponses != null && !crlResponses.IsEmpty()) {
                IAsn1EncodableVector v2 = BC_FACTORY.CreateASN1EncodableVector();
                foreach (byte[] bCrl in crlResponses) {
                    if (bCrl == null) {
                        continue;
                    }
                    try {
                        using (IAsn1InputStream t = BC_FACTORY.CreateASN1InputStream(new MemoryStream(bCrl))) {
                            v2.Add(t.ReadObject());
                        }
                    }
                    catch (System.IO.IOException e) {
                        throw new PdfException(e);
                    }
                }
                revocationV.Add(BC_FACTORY.CreateDERTaggedObject(true, 0, BC_FACTORY.CreateDERSequence(v2)));
            }
        }

        private void CreateOCPSStructure(IAsn1EncodableVector revocationV) {
            if (ocspResponses != null && !ocspResponses.IsEmpty()) {
                IAsn1EncodableVector vo1 = BC_FACTORY.CreateASN1EncodableVector();
                foreach (byte[] ocspBytes in ocspResponses) {
                    IDerOctetString doctet = BC_FACTORY.CreateDEROctetString(ocspBytes);
                    IAsn1EncodableVector v2 = BC_FACTORY.CreateASN1EncodableVector();
                    IOcspObjectIdentifiers objectIdentifiers = BC_FACTORY.CreateOCSPObjectIdentifiers();
                    v2.Add(objectIdentifiers.GetIdPkixOcspBasic());
                    v2.Add(doctet);
                    IDerEnumerated den = BC_FACTORY.CreateASN1Enumerated(0);
                    IAsn1EncodableVector v3 = BC_FACTORY.CreateASN1EncodableVector();
                    v3.Add(den);
                    v3.Add(BC_FACTORY.CreateDERTaggedObject(true, 0, BC_FACTORY.CreateDERSequence(v2)));
                    vo1.Add(BC_FACTORY.CreateDERSequence(v3));
                }
                revocationV.Add(BC_FACTORY.CreateDERTaggedObject(true, 1, BC_FACTORY.CreateDERSequence(vo1)));
            }
        }

        private bool ContainsRevocationData() {
            return (ocspResponses != null && !ocspResponses.IsEmpty()) || (crlResponses != null && !crlResponses.IsEmpty
                ());
        }

        private static IDerSet GetAttributesAsDERSet(ICollection<CmsAttribute> attributeSet) {
            IAsn1EncodableVector attributes = BC_FACTORY.CreateASN1EncodableVector();
            foreach (CmsAttribute attr in attributeSet) {
                IAsn1EncodableVector v = BC_FACTORY.CreateASN1EncodableVector();
                v.Add(BC_FACTORY.CreateASN1ObjectIdentifier(attr.GetType()));
                v.Add(attr.GetValue());
                attributes.Add(BC_FACTORY.CreateDERSequence(v));
            }
            return BC_FACTORY.CreateDERSet(attributes);
        }
    }
}
