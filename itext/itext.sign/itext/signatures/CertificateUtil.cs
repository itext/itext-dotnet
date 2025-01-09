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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Crypto;
using iText.Signatures.Logs;

namespace iText.Signatures {
    /// <summary>
    /// This class contains a series of static methods that
    /// allow you to retrieve information from a Certificate.
    /// </summary>
    public class CertificateUtil {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(CertificateUtil));

        // Certificate Revocation Lists
        /// <summary>Gets a CRLs from the X509 certificate.</summary>
        /// <param name="certificate">the X509Certificate to extract the CRLs from</param>
        /// <returns>CRL list or null if there's no CRL available</returns>
        public static IList<IX509Crl> GetCRLs(IX509Certificate certificate) {
            IList<IX509Crl> crls = new List<IX509Crl>();
            foreach (String crlUrl in GetCRLURLs(certificate)) {
                crls.Add(CertificateUtil.GetCRL(crlUrl));
            }
            return crls;
        }

        /// <summary>Gets the list of the Certificate Revocation List URLs for a Certificate.</summary>
        /// <param name="certificate">the Certificate to get CRL URLs for</param>
        /// <returns>the list of URL strings where you can check if the certificate is revoked.</returns>
        public static IList<String> GetCRLURLs(IX509Certificate certificate) {
            IList<String> crls = new List<String>();
            IDistributionPoint[] dists = GetDistributionPoints(certificate);
            foreach (IDistributionPoint p in dists) {
                IDistributionPointName distributionPointName = p.GetDistributionPoint();
                if (FACTORY.CreateDistributionPointName().GetFullName() != distributionPointName.GetType()) {
                    continue;
                }
                IGeneralNames generalNames = FACTORY.CreateGeneralNames(distributionPointName.GetName());
                IGeneralName[] names = generalNames.GetNames();
                // If the DistributionPointName contains multiple values, each name describes a different mechanism
                // to obtain the same CRL.
                foreach (IGeneralName name in names) {
                    if (name.GetTagNo() != FACTORY.CreateGeneralName().GetUniformResourceIdentifier()) {
                        continue;
                    }
                    IDerIA5String derStr = FACTORY.CreateDERIA5String(FACTORY.CreateASN1TaggedObject(name.ToASN1Primitive()), 
                        false);
                    crls.Add(derStr.GetString());
                }
            }
            return crls;
        }

        /// <summary>
        /// Gets the Distribution Point from the certificate by name specified in the Issuing Distribution Point from the
        /// Certificate Revocation List for a Certificate.
        /// </summary>
        /// <param name="certificate">the certificate to retrieve Distribution Points</param>
        /// <param name="issuingDistributionPointName">distributionPointName retrieved from the IDP of the CRL</param>
        /// <returns>distribution point withthe same name as specified in the IDP.</returns>
        public static IDistributionPoint GetDistributionPointByName(IX509Certificate certificate, IDistributionPointName
             issuingDistributionPointName) {
            IDistributionPoint[] distributionPoints = GetDistributionPoints(certificate);
            IList<IGeneralName> issuingNames = JavaUtil.ArraysAsList(FACTORY.CreateGeneralNames(issuingDistributionPointName
                .GetName()).GetNames());
            foreach (IDistributionPoint distributionPoint in distributionPoints) {
                IDistributionPointName distributionPointName = distributionPoint.GetDistributionPoint();
                IGeneralNames generalNames = distributionPointName.IsNull() ? distributionPoint.GetCRLIssuer() : FACTORY.CreateGeneralNames
                    (distributionPointName.GetName());
                IGeneralName[] names = generalNames.GetNames();
                foreach (IGeneralName name in names) {
                    if (issuingNames.Contains(name)) {
                        return distributionPoint;
                    }
                }
            }
            return null;
        }

        /// <summary>Gets the CRL object using a CRL URL.</summary>
        /// <param name="url">the URL where the CRL is located</param>
        /// <returns>CRL object</returns>
        public static IX509Crl GetCRL(String url) {
            if (url == null) {
                return null;
            }
            return CertificateUtil.ParseCrlFromStream(UrlUtil.OpenStream(new Uri(url)));
        }

        /// <summary>Parses a CRL from an InputStream.</summary>
        /// <param name="input">the InputStream holding the unparsed CRL</param>
        /// <returns>the parsed CRL object.</returns>
        public static IX509Crl ParseCrlFromStream(Stream input) {
            return SignUtils.ParseCrlFromStream(input);
        }

        /// <summary>Parses a CRL from bytes.</summary>
        /// <param name="crlBytes">the bytes holding the unparsed CRL</param>
        /// <returns>the parsed CRL object.</returns>
        public static IX509Crl ParseCrlFromBytes(byte[] crlBytes) {
            return SignUtils.ParseCrlFromStream(new MemoryStream(crlBytes));
        }

        /// <summary>Retrieves the URL for the issuer certificate for the given CRL.</summary>
        /// <param name="crl">the CRL response</param>
        /// <returns>the URL or null.</returns>
        public static String GetIssuerCertURL(IX509Crl crl) {
            IAsn1Object obj;
            try {
                obj = GetExtensionValue(crl, FACTORY.CreateExtensions().GetAuthorityInfoAccess().GetId());
                return GetValueFromAIAExtension(obj, OID.CA_ISSUERS);
            }
            catch (System.IO.IOException) {
                return null;
            }
        }

        // Online Certificate Status Protocol
        /// <summary>Retrieves the OCSP URL from the given certificate.</summary>
        /// <param name="certificate">the certificate</param>
        /// <returns>the URL or null</returns>
        public static String GetOCSPURL(IX509Certificate certificate) {
            IAsn1Object obj;
            try {
                obj = GetExtensionValue(certificate, FACTORY.CreateExtensions().GetAuthorityInfoAccess().GetId());
                return GetValueFromAIAExtension(obj, OID.OCSP);
            }
            catch (System.IO.IOException) {
                return null;
            }
        }

        // Missing certificates in chain
        /// <summary>Retrieves the URL for the issuer lists certificates for the given certificate.</summary>
        /// <param name="certificate">the certificate</param>
        /// <returns>the URL or null.</returns>
        public static String GetIssuerCertURL(IX509Certificate certificate) {
            IAsn1Object obj;
            try {
                obj = GetExtensionValue(certificate, FACTORY.CreateExtensions().GetAuthorityInfoAccess().GetId());
                return GetValueFromAIAExtension(obj, OID.CA_ISSUERS);
            }
            catch (System.IO.IOException) {
                return null;
            }
        }

        // Time Stamp Authority
        /// <summary>Gets the URL of the TSA if it's available on the certificate</summary>
        /// <param name="certificate">a certificate</param>
        /// <returns>a TSA URL</returns>
        public static String GetTSAURL(IX509Certificate certificate) {
            byte[] der = SignUtils.GetExtensionValueByOid(certificate, OID.TSA);
            if (der == null) {
                return null;
            }
            IAsn1Object asn1obj;
            try {
                asn1obj = FACTORY.CreateASN1Primitive(der);
                IDerOctetString octets = FACTORY.CreateDEROctetString(asn1obj);
                asn1obj = FACTORY.CreateASN1Primitive(octets.GetOctets());
                IAsn1Sequence asn1seq = FACTORY.CreateASN1SequenceInstance(asn1obj);
                return GetStringFromGeneralName(asn1seq.GetObjectAt(1).ToASN1Primitive());
            }
            catch (System.IO.IOException) {
                return null;
            }
        }

        /// <summary>Generates a certificate object and initializes it with the data read from the input stream inStream.
        ///     </summary>
        /// <param name="data">the input stream with the certificates.</param>
        /// <returns>a certificate object initialized with the data from the input stream.</returns>
        public static IX509Certificate GenerateCertificate(Stream data) {
            return SignUtils.GenerateCertificate(data);
        }

        /// <summary>Try to retrieve CRL and OCSP responses from the signed data crls field.</summary>
        /// <param name="taggedObj">
        /// signed data crls field as
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1TaggedObject"/>.
        /// </param>
        /// <param name="crls">collection to store retrieved CRL responses.</param>
        /// <param name="ocsps">
        /// collection of
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// wrappers to store retrieved
        /// OCSP responses.
        /// </param>
        /// <param name="otherRevocationInfoFormats">
        /// collection of revocation info other than OCSP and CRL responses,
        /// e.g. SCVP Request and Response, stored as
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Sequence"/>.
        /// </param>
        public static void RetrieveRevocationInfoFromSignedData(IAsn1TaggedObject taggedObj, ICollection<IX509Crl>
             crls, ICollection<IBasicOcspResponse> ocsps, ICollection<IAsn1Sequence> otherRevocationInfoFormats) {
            IEnumerator revInfo = FACTORY.CreateASN1Set(taggedObj, false).GetObjects();
            while (revInfo.MoveNext()) {
                IAsn1Sequence s = FACTORY.CreateASN1Sequence(revInfo.Current);
                IDerObjectIdentifier o = FACTORY.CreateASN1ObjectIdentifier(s.GetObjectAt(0));
                if (o != null && OID.RI_OCSP_RESPONSE.Equals(o.GetId())) {
                    IAsn1Sequence ocspResp = FACTORY.CreateASN1Sequence(s.GetObjectAt(1));
                    IDerEnumerated respStatus = FACTORY.CreateASN1Enumerated(ocspResp.GetObjectAt(0));
                    if (respStatus.IntValueExact() == FACTORY.CreateOCSPResponseStatus().GetSuccessful()) {
                        IAsn1Sequence responseBytes = FACTORY.CreateASN1Sequence(ocspResp.GetObjectAt(1));
                        if (responseBytes != null) {
                            ocsps.Add(CertificateUtil.CreateOcsp(responseBytes));
                        }
                    }
                }
                else {
                    try {
                        crls.AddAll(SignUtils.ReadAllCRLs(s.GetEncoded()));
                    }
                    catch (AbstractCrlException) {
                        LOGGER.LogWarning(SignLogMessageConstant.UNABLE_TO_PARSE_REV_INFO);
                        otherRevocationInfoFormats.Add(s);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the revocation info (crls field) for SignedData structure:
        /// RevocationInfoChoices ::= SET OF RevocationInfoChoice
        /// RevocationInfoChoice ::= CHOICE {
        /// crl CertificateList,
        /// other [1] IMPLICIT OtherRevocationInfoFormat }
        /// OtherRevocationInfoFormat ::= SEQUENCE {
        /// otherRevInfoFormat OBJECT IDENTIFIER,
        /// otherRevInfo ANY DEFINED BY otherRevInfoFormat }
        /// CertificateList  ::=  SEQUENCE  {
        /// tbsCertList          TBSCertList,
        /// signatureAlgorithm   AlgorithmIdentifier,
        /// signatureValue       BIT STRING  }
        /// </summary>
        /// <seealso><a href="https://datatracker.ietf.org/doc/html/rfc5652#section-10.2.1">RFC 5652 §10.2.1</a></seealso>
        /// <param name="crls">collection of CRL revocation status information.</param>
        /// <param name="ocsps">collection of OCSP revocation status information.</param>
        /// <param name="otherRevocationInfoFormats">
        /// collection of revocation info other than OCSP and CRL responses,
        /// e.g. SCVP Request and Response, stored as
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Sequence"/>.
        /// </param>
        /// <returns>
        /// 
        /// <c>crls [1] RevocationInfoChoices</c>
        /// field of SignedData structure. Null if SignedData has
        /// no revocation data.
        /// </returns>
        public static IDerSet CreateRevocationInfoChoices(ICollection<IX509Crl> crls, ICollection<IBasicOcspResponse
            > ocsps, ICollection<IAsn1Sequence> otherRevocationInfoFormats) {
            if (crls.IsEmpty() && ocsps.IsEmpty()) {
                return null;
            }
            IAsn1EncodableVector revocationInfoChoices = FACTORY.CreateASN1EncodableVector();
            // Add CRLs
            foreach (IX509Crl element in crls) {
                // Add crl CertificateList (crl RevocationInfoChoice)
                revocationInfoChoices.Add(FACTORY.CreateASN1Sequence(((IX509Crl)element).GetEncoded()));
            }
            // Add OCSPs
            foreach (IBasicOcspResponse element in ocsps) {
                IAsn1EncodableVector ocspResponseRevInfo = FACTORY.CreateASN1EncodableVector();
                // Add otherRevInfoFormat (ID_RI_OCSP_RESPONSE)
                ocspResponseRevInfo.Add(FACTORY.CreateASN1ObjectIdentifier(OID.RI_OCSP_RESPONSE));
                IAsn1EncodableVector ocspResponse = FACTORY.CreateASN1EncodableVector();
                ocspResponse.Add(FACTORY.CreateOCSPResponseStatus(FACTORY.CreateOCSPResponseStatus().GetSuccessful()).ToASN1Primitive
                    ());
                ocspResponse.Add(FACTORY.CreateResponseBytes(FACTORY.CreateOCSPObjectIdentifiers().GetIdPkixOcspBasic(), FACTORY
                    .CreateDEROctetString(element.ToASN1Primitive().GetEncoded())).ToASN1Primitive());
                // Add otherRevInfo (ocspResponse)
                ocspResponseRevInfo.Add(FACTORY.CreateDERSequence(ocspResponse));
                // Add other [1] IMPLICIT OtherRevocationInfoFormat (ocsp RevocationInfoChoice)
                revocationInfoChoices.Add(FACTORY.CreateDERSequence(ocspResponseRevInfo));
            }
            // Add other RevocationInfo formats
            foreach (IAsn1Sequence revInfo in otherRevocationInfoFormats) {
                revocationInfoChoices.Add(revInfo);
            }
            return FACTORY.CreateDERSet(revocationInfoChoices);
        }

        /// <summary>
        /// Checks if the issuer of the provided certID (specified in the OCSP response) and provided issuer of the
        /// certificate in question matches, i.e. checks that issuerNameHash and issuerKeyHash fields of the certID
        /// is the hash of the issuer's name and public key.
        /// </summary>
        /// <remarks>
        /// Checks if the issuer of the provided certID (specified in the OCSP response) and provided issuer of the
        /// certificate in question matches, i.e. checks that issuerNameHash and issuerKeyHash fields of the certID
        /// is the hash of the issuer's name and public key.
        /// <para />
        /// SingleResp contains the basic information of the status of the certificate identified by the certID. The issuer
        /// name and serial number identify a unique certificate, so if serial numbers of the certificate in question and
        /// certID serial number are equals and issuers match, then SingleResp contains the information about the status of
        /// the certificate in question.
        /// </remarks>
        /// <param name="certID">certID specified in the OCSP response</param>
        /// <param name="issuerCert">the issuer of the certificate in question</param>
        /// <returns>true if the issuers are the same, false otherwise.</returns>
        public static bool CheckIfIssuersMatch(ICertID certID, IX509Certificate issuerCert) {
            return SignUtils.CheckIfIssuersMatch(certID, issuerCert);
        }

        /// <summary>Retrieves certificate extension value by its OID.</summary>
        /// <param name="certificate">to get extension from</param>
        /// <param name="id">extension OID to retrieve</param>
        /// <returns>encoded extension value.</returns>
        public static byte[] GetExtensionValueByOid(IX509Certificate certificate, String id) {
            return SignUtils.GetExtensionValueByOid(certificate, id);
        }

        /// <summary>Checks if an OCSP response is genuine.</summary>
        /// <param name="ocspResp">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// the OCSP response wrapper
        /// </param>
        /// <param name="responderCert">the responder certificate</param>
        /// <returns>true if the OCSP response verifies against the responder certificate.</returns>
        public static bool IsSignatureValid(IBasicOcspResponse ocspResp, IX509Certificate responderCert) {
            try {
                return SignUtils.IsSignatureValid(ocspResp, responderCert);
            }
            catch (Exception) {
                return false;
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks if the certificate is signed by provided issuer certificate.</summary>
        /// <param name="subjectCertificate">a certificate to check</param>
        /// <param name="issuerCertificate">an issuer certificate to check</param>
        /// <returns>true if the first passed certificate is signed by next passed certificate.</returns>
        internal static bool IsIssuerCertificate(IX509Certificate subjectCertificate, IX509Certificate issuerCertificate
            ) {
            return subjectCertificate.GetIssuerDN().Equals(issuerCertificate.GetSubjectDN());
        }
//\endcond

        /// <summary>Checks if the certificate is self-signed.</summary>
        /// <param name="certificate">a certificate to check</param>
        /// <returns>true if the certificate is self-signed.</returns>
        public static bool IsSelfSigned(IX509Certificate certificate) {
            return certificate.GetIssuerDN().Equals(certificate.GetSubjectDN());
        }

        // helper methods
        /// <summary>Gets certificate extension value.</summary>
        /// <param name="certificate">the certificate from which we need the ExtensionValue</param>
        /// <param name="oid">the Object Identifier value for the extension</param>
        /// <returns>
        /// the extension value as an
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Object"/>
        /// object.
        /// </returns>
        public static IAsn1Object GetExtensionValue(IX509Certificate certificate, String oid) {
            return GetExtensionValueFromByteArray(SignUtils.GetExtensionValueByOid(certificate, oid));
        }

        /// <summary>Gets CRL extension value.</summary>
        /// <param name="crl">the CRL from which we need the ExtensionValue</param>
        /// <param name="oid">the Object Identifier value for the extension</param>
        /// <returns>
        /// the extension value as an
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Object"/>
        /// object.
        /// </returns>
        public static IAsn1Object GetExtensionValue(IX509Crl crl, String oid) {
            return GetExtensionValueFromByteArray(SignUtils.GetExtensionValueByOid(crl, oid));
        }

        /// <summary>
        /// Converts extension value represented as byte array to
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Object"/>
        /// object.
        /// </summary>
        /// <param name="extensionValue">the extension value as byte array</param>
        /// <returns>
        /// the extension value as an
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Object"/>
        /// object.
        /// </returns>
        private static IAsn1Object GetExtensionValueFromByteArray(byte[] extensionValue) {
            if (extensionValue == null) {
                return null;
            }
            IAsn1OctetString octs;
            using (IAsn1InputStream aIn = FACTORY.CreateASN1InputStream(new MemoryStream(extensionValue))) {
                octs = FACTORY.CreateASN1OctetString(aIn.ReadObject());
            }
            using (IAsn1InputStream aIn_1 = FACTORY.CreateASN1InputStream(new MemoryStream(octs.GetOctets()))) {
                return aIn_1.ReadObject();
            }
        }

        /// <summary>Gets a String from an ASN1Primitive</summary>
        /// <param name="names">
        /// the
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Object"/>
        /// primitive wrapper
        /// </param>
        /// <returns>a human-readable String</returns>
        private static String GetStringFromGeneralName(IAsn1Object names) {
            IAsn1TaggedObject taggedObject = FACTORY.CreateASN1TaggedObject(names);
            return iText.Commons.Utils.JavaUtil.GetStringForBytes(FACTORY.CreateASN1OctetString(taggedObject, false).GetOctets
                (), iText.Commons.Utils.EncodingUtil.ISO_8859_1);
        }

        /// <summary>Retrieves accessLocation value for specified accessMethod from the Authority Information Access extension.
        ///     </summary>
        /// <param name="extensionValue">Authority Information Access extension value</param>
        /// <param name="accessMethod">accessMethod OID; usually id-ad-caIssuers or id-ad-ocsp</param>
        /// <returns>the location (URI) of the information.</returns>
        private static String GetValueFromAIAExtension(IAsn1Object extensionValue, String accessMethod) {
            if (extensionValue == null) {
                return null;
            }
            IAsn1Sequence accessDescriptions = FACTORY.CreateASN1Sequence(extensionValue);
            for (int i = 0; i < accessDescriptions.Size(); i++) {
                IAsn1Sequence accessDescription = FACTORY.CreateASN1Sequence(accessDescriptions.GetObjectAt(i));
                IDerObjectIdentifier id = FACTORY.CreateASN1ObjectIdentifier(accessDescription.GetObjectAt(0));
                if (accessDescription.Size() == 2 && id != null && accessMethod.Equals(id.GetId())) {
                    IAsn1Object description = FACTORY.CreateASN1Primitive(accessDescription.GetObjectAt(1));
                    return GetStringFromGeneralName(description);
                }
            }
            return null;
        }

        /// <summary>
        /// Helper method that creates the
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// object from the response bytes.
        /// </summary>
        /// <param name="seq">response bytes.</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// object.
        /// </returns>
        private static IBasicOcspResponse CreateOcsp(IAsn1Sequence seq) {
            IDerObjectIdentifier objectIdentifier = FACTORY.CreateASN1ObjectIdentifier(seq.GetObjectAt(0));
            IOcspObjectIdentifiers ocspObjectIdentifiers = FACTORY.CreateOCSPObjectIdentifiers();
            if (objectIdentifier != null && objectIdentifier.GetId().Equals(ocspObjectIdentifiers.GetIdPkixOcspBasic()
                .GetId())) {
                IAsn1OctetString os = FACTORY.CreateASN1OctetString(seq.GetObjectAt(1));
                using (IAsn1InputStream inp = FACTORY.CreateASN1InputStream(os.GetOctets())) {
                    return FACTORY.CreateBasicOCSPResponse(inp.ReadObject());
                }
            }
            return null;
        }

        private static IDistributionPoint[] GetDistributionPoints(IX509Certificate certificate) {
            IAsn1Object obj;
            try {
                obj = GetExtensionValue(certificate, FACTORY.CreateExtensions().GetCRlDistributionPoints().GetId());
            }
            catch (System.IO.IOException) {
                obj = null;
            }
            if (obj == null) {
                return new IDistributionPoint[0];
            }
            ICrlDistPoint dist = FACTORY.CreateCRLDistPoint(obj);
            return dist.GetDistributionPoints();
        }
    }
}
