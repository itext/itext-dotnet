/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures;
using iText.Signatures.Exceptions;

namespace iText.Signatures.Cms {
    /// <summary>
    /// The CMS container which represents SignedData structure from
    /// <a href="https://datatracker.ietf.org/doc/html/rfc5652#section-5.1">rfc5652 Cryptographic Message Syntax (CMS)</a>
    /// </summary>
    public class CMSContainer {
        private static readonly IBouncyCastleFactory BC_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        /// <summary>This represents the signed content.</summary>
        /// <remarks>
        /// This represents the signed content.
        /// In the case of a signed PDF document this will of type data with no content.
        /// </remarks>
        private EncapsulatedContentInfo encapContentInfo = new EncapsulatedContentInfo();

        /// <summary>Optional.</summary>
        /// <remarks>
        /// Optional.
        /// <para />
        /// It is intended to add all certificates to be able to validate the entire chain.
        /// </remarks>
        private ICollection<IX509Certificate> certificates = new List<IX509Certificate>();

        /// <summary>This class only supports one signer per signature field.</summary>
        private SignerInfo signerInfo = new SignerInfo();

        /// <summary>Creates an empty SignedData structure.</summary>
        public CMSContainer() {
        }

        // Empty constructor.
        /// <summary>Creates a SignedData structure from a serialized ASN1 structure.</summary>
        /// <param name="encodedCMSdata">the serialized CMS container</param>
        public CMSContainer(byte[] encodedCMSdata) {
            try {
                using (IAsn1InputStream @is = BC_FACTORY.CreateASN1InputStream(new MemoryStream(encodedCMSdata))) {
                    IAsn1Sequence contentInfo = BC_FACTORY.CreateASN1Sequence(@is.ReadObject());
                    IAsn1Sequence signedData = BC_FACTORY.CreateASN1Sequence(BC_FACTORY.CreateASN1TaggedObject(contentInfo.GetObjectAt
                        (1)).GetObject());
                    // The digest algorithm is retrieved from SignerInfo later on, here we just validate
                    // that there is exactly 1 digest algorithm.
                    IAsn1Set digestAlgorithms = BC_FACTORY.CreateASN1Set(signedData.GetObjectAt(1));
                    if (digestAlgorithms.Size() > 1) {
                        throw new PdfException(SignExceptionMessageConstant.CMS_ONLY_ONE_SIGNER_ALLOWED);
                    }
                    IAsn1Sequence lencapContentInfo = BC_FACTORY.CreateASN1Sequence(signedData.GetObjectAt(2));
                    encapContentInfo = new EncapsulatedContentInfo(lencapContentInfo);
                    ProcessCertificates(signedData);
                    IAsn1Set signerInfosS = BC_FACTORY.CreateASN1Set(signedData.GetObjectAt(4));
                    if (signerInfosS == null) {
                        // Most probably revocation data is in place, so read next item.
                        signerInfosS = BC_FACTORY.CreateASN1Set(signedData.GetObjectAt(5));
                    }
                    if (signerInfosS.Size() != 1) {
                        throw new PdfException(SignExceptionMessageConstant.CMS_ONLY_ONE_SIGNER_ALLOWED);
                    }
                    signerInfo = new SignerInfo(signerInfosS.GetObjectAt(0), certificates);
                }
            }
            catch (NullReferenceException npe) {
                throw new PdfException(SignExceptionMessageConstant.CMS_INVALID_CONTAINER_STRUCTURE, npe);
            }
        }

        /// <summary>This class only supports one signer per signature field.</summary>
        /// <param name="signerInfo">the singerInfo</param>
        public virtual void SetSignerInfo(SignerInfo signerInfo) {
            this.signerInfo = signerInfo;
        }

        /// <summary>This class only supports one signer per signature field.</summary>
        /// <returns>the singerInfo</returns>
        public virtual SignerInfo GetSignerInfo() {
            return signerInfo;
        }

        /// <summary>
        /// When all fields except for signer.signedAttributes.digest and signer.signature are completed
        /// it is possible to calculate the eventual size of the signature by serializing except for the signature
        /// (that depends on the digest and cypher but is set at 1024 bytes) and later added unsigned attributes like
        /// timestamps.
        /// </summary>
        /// <returns>
        /// the estimated size of the complete CMS container before signature is added, size for the signature is
        /// added, size for other attributes like timestamps is not.
        /// </returns>
        public virtual long GetSizeEstimation() {
            byte[] result = Serialize(true);
            return result.Length;
        }

        /// <summary>Only version 1 is supported by this class.</summary>
        /// <returns>1 as CMSversion</returns>
        public virtual int GetCmsVersion() {
            return 1;
        }

        /// <summary>The digest algorithm OID and parameters used by the signer.</summary>
        /// <remarks>
        /// The digest algorithm OID and parameters used by the signer.
        /// This class only supports one signer for use in pdf signatures, so only one digest algorithm is supported.
        /// <para />
        /// This field is set when adding the signerInfo.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="AlgorithmIdentifier"/>
        /// digest algorithm.
        /// </returns>
        public virtual AlgorithmIdentifier GetDigestAlgorithm() {
            if (signerInfo == null) {
                return null;
            }
            return signerInfo.GetDigestAlgorithm();
        }

        /// <summary>This represents the signed content.</summary>
        /// <remarks>
        /// This represents the signed content.
        /// In the case of a signed PDF document this will be of type data with no content.
        /// </remarks>
        /// <returns>a representation of the data to be signed.</returns>
        public virtual EncapsulatedContentInfo GetEncapContentInfo() {
            return encapContentInfo;
        }

        /// <summary>This represents the signed content.</summary>
        /// <remarks>
        /// This represents the signed content.
        /// In the case of a signed PDF document this will be of type data with no content.
        /// Defaults to 1.2.840.113549.1.7.1 {iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) pkcs-7(7) id-data(1)}
        /// </remarks>
        /// <param name="encapContentInfo">a representation of the data to be signed.</param>
        public virtual void SetEncapContentInfo(EncapsulatedContentInfo encapContentInfo) {
            this.encapContentInfo = encapContentInfo;
        }

        /// <summary>Adds a certificate.</summary>
        /// <param name="cert">the certificate to be added</param>
        public virtual void AddCertificate(IX509Certificate cert) {
            certificates.Add(cert);
        }

        /// <summary>Adds a set of certificates.</summary>
        /// <param name="certs">the certificates to be added</param>
        public virtual void AddCertificates(IX509Certificate[] certs) {
            certificates = JavaUtil.ArraysAsList(certs);
        }

        /// <summary>Retrieves a copy of the list of certificates.</summary>
        /// <returns>the list of certificates to be used  for signing and certificate validation</returns>
        public virtual ICollection<IX509Certificate> GetCertificates() {
            return JavaCollectionsUtil.UnmodifiableCollection(certificates);
        }

        /// <summary>Sets the  Signed Attributes of the signer info to this serialized version.</summary>
        /// <remarks>
        /// Sets the  Signed Attributes of the signer info to this serialized version.
        /// The signed attributes will become read-only.
        /// </remarks>
        /// <param name="signedAttributesData">the serialized Signed Attributes</param>
        public virtual void SetSerializedSignedAttributes(byte[] signedAttributesData) {
            signerInfo.SetSerializedSignedAttributes(signedAttributesData);
        }

        /// <summary>Retrieves the encoded signed attributes of the signer info.</summary>
        /// <remarks>
        /// Retrieves the encoded signed attributes of the signer info.
        /// This makes the signed attributes read only.
        /// </remarks>
        /// <returns>the encoded signed attributes of the signer info.</returns>
        public virtual byte[] GetSerializedSignedAttributes() {
            if (signerInfo == null) {
                throw new InvalidOperationException(SignExceptionMessageConstant.CMS_SIGNERINFO_NOT_INITIALIZED);
            }
            return signerInfo.SerializeSignedAttributes();
        }

        /// <summary>Serializes the SignedData structure and makes the signer infos signed attributes read only.</summary>
        /// <returns>the encoded DignedData structure.</returns>
        public virtual byte[] Serialize() {
            return Serialize(false);
        }

        private byte[] Serialize(bool forEstimation) {
            /* ContentInfo SEQUENCE
            ContentType OBJECT IDENTIFIER (1.2.840.113549.1.7.2)
            Content [0] SEQUENCE
            SignedData SEQUENCE
            version INTEGER
            digestAlgorithms SET
            DigestAlgorithmIdentifier SEQUENCE
            algorithm OBJECT IDENTIFIER
            parameters ANY
            encapContentInfo EncapsulatedContentInfo SEQUENCE
            eContentType ContentType OBJECT IDENTIFIER (1.2.840.113549.1.7.1 data)
            CertificateSet [0] (set?)
            CertificateChoices SEQUENCE
            tbsCertificate TBSCertificate SEQUENCE
            signerInfos SignerInfos SET
            */
            IAsn1EncodableVector contentInfoV = BC_FACTORY.CreateASN1EncodableVector();
            contentInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(SecurityIDs.ID_PKCS7_SIGNED_DATA));
            IAsn1EncodableVector singedDataV = BC_FACTORY.CreateASN1EncodableVector();
            singedDataV.Add(BC_FACTORY.CreateASN1Integer(GetCmsVersion()));
            // version
            IAsn1EncodableVector digestAlgorithmsV = BC_FACTORY.CreateASN1EncodableVector();
            digestAlgorithmsV.Add(GetDigestAlgorithm().GetAsASN1Sequence());
            singedDataV.Add(BC_FACTORY.CreateDERSet(digestAlgorithmsV));
            IAsn1EncodableVector encapContentInfoV = BC_FACTORY.CreateASN1EncodableVector();
            encapContentInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(encapContentInfo.GetContentType()));
            if (encapContentInfo.GetContent() != null) {
                encapContentInfoV.Add(encapContentInfo.GetContent());
            }
            singedDataV.Add(BC_FACTORY.CreateDERSequence(encapContentInfoV));
            IAsn1EncodableVector certificateSetV = BC_FACTORY.CreateASN1EncodableVector();
            foreach (IX509Certificate cert in certificates) {
                certificateSetV.Add(BC_FACTORY.CreateASN1Primitive(cert.GetEncoded()));
            }
            singedDataV.Add(BC_FACTORY.CreateDERTaggedObject(false, 0, BC_FACTORY.CreateDERSet(certificateSetV)));
            IAsn1EncodableVector signerInfosV = BC_FACTORY.CreateASN1EncodableVector();
            signerInfosV.Add(signerInfo.GetAsDerSequence(forEstimation));
            singedDataV.Add(BC_FACTORY.CreateDERSet(signerInfosV));
            contentInfoV.Add(BC_FACTORY.CreateDERTaggedObject(0, BC_FACTORY.CreateDERSequence(singedDataV)));
            return BC_FACTORY.CreateDERSequence(contentInfoV).GetEncoded();
        }

        private void ProcessCertificates(IAsn1Sequence signedData) {
            // Certificates are optional according to the specs, but we do require at least the signing certificate.
            IAsn1TaggedObject taggedCertificatesSet = BC_FACTORY.CreateASN1TaggedObject(signedData.GetObjectAt(3));
            if (taggedCertificatesSet == null) {
                throw new PdfException(SignExceptionMessageConstant.CMS_MISSING_CERTIFICATES);
            }
            IAsn1Set certificatesSet = BC_FACTORY.CreateASN1Set(taggedCertificatesSet, false);
            if (certificatesSet.IsNull() || certificatesSet.Size() == 0) {
                throw new PdfException(SignExceptionMessageConstant.CMS_MISSING_CERTIFICATES);
            }
            foreach (IAsn1Encodable certObj in certificatesSet.ToArray()) {
                using (Stream cis = new MemoryStream(certObj.ToASN1Primitive().GetEncoded(BC_FACTORY.CreateASN1Encoding().
                    GetDer()))) {
                    certificates.Add((IX509Certificate)CertificateUtil.GenerateCertificate(cis));
                }
            }
        }
    }
}
