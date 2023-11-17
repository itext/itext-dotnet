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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.IO.Util;

namespace iText.Signatures {
    /// <summary>
    /// This class contains a series of static methods that
    /// allow you to retrieve information from a Certificate.
    /// </summary>
    public class CertificateUtil {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        // Certificate Revocation Lists
        /// <summary>Gets a CRL from an X509 certificate.</summary>
        /// <param name="certificate">the X509Certificate to extract the CRL from</param>
        /// <returns>CRL or null if there's no CRL available</returns>
        public static IX509Crl GetCRL(IX509Certificate certificate) {
            return CertificateUtil.GetCRL(CertificateUtil.GetCRLURL(certificate));
        }

        /// <summary>Gets the URL of the Certificate Revocation List for a Certificate</summary>
        /// <param name="certificate">the Certificate</param>
        /// <returns>the String where you can check if the certificate was revoked</returns>
        public static String GetCRLURL(IX509Certificate certificate) {
            IAsn1Object obj;
            try {
                obj = GetExtensionValue(certificate, FACTORY.CreateExtensions().GetCRlDistributionPoints().GetId());
            }
            catch (System.IO.IOException) {
                obj = null;
            }
            if (obj == null) {
                return null;
            }
            ICrlDistPoint dist = FACTORY.CreateCRLDistPoint(obj);
            IDistributionPoint[] dists = dist.GetDistributionPoints();
            foreach (IDistributionPoint p in dists) {
                IDistributionPointName distributionPointName = p.GetDistributionPoint();
                if (FACTORY.CreateDistributionPointName().GetFullName() != distributionPointName.GetType()) {
                    continue;
                }
                IGeneralNames generalNames = FACTORY.CreateGeneralNames(distributionPointName.GetName());
                IGeneralName[] names = generalNames.GetNames();
                foreach (IGeneralName name in names) {
                    if (name.GetTagNo() != FACTORY.CreateGeneralName().GetUniformResourceIdentifier()) {
                        continue;
                    }
                    IDerIA5String derStr = FACTORY.CreateDERIA5String(FACTORY.CreateASN1TaggedObject(name.ToASN1Primitive()), 
                        false);
                    return derStr.GetString();
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
            return SignUtils.ParseCrlFromStream(UrlUtil.OpenStream(new Uri(url)));
        }

        // Online Certificate Status Protocol
        /// <summary>Retrieves the OCSP URL from the given certificate.</summary>
        /// <param name="certificate">the certificate</param>
        /// <returns>the URL or null</returns>
        public static String GetOCSPURL(IX509Certificate certificate) {
            IAsn1Object obj;
            try {
                obj = GetExtensionValue(certificate, FACTORY.CreateExtensions().GetAuthorityInfoAccess().GetId());
                if (obj == null) {
                    return null;
                }
                IAsn1Sequence accessDescriptions = FACTORY.CreateASN1Sequence(obj);
                for (int i = 0; i < accessDescriptions.Size(); i++) {
                    IAsn1Sequence accessDescription = FACTORY.CreateASN1Sequence(accessDescriptions.GetObjectAt(i));
                    IDerObjectIdentifier id = FACTORY.CreateASN1ObjectIdentifier(accessDescription.GetObjectAt(0));
                    if (accessDescription.Size() == 2 && id != null && SecurityIDs.ID_OCSP.Equals(id.GetId())) {
                        IAsn1Object description = FACTORY.CreateASN1Primitive(accessDescription.GetObjectAt(1));
                        return GetStringFromGeneralName(description);
                    }
                }
            }
            catch (System.IO.IOException) {
                return null;
            }
            return null;
        }

        // Missing certificates in chain
        /// <summary>Retrieves the URL for the issuer lists certificates for the given certificate.</summary>
        /// <param name="certificate">the certificate</param>
        /// <returns>the URL or null.</returns>
        public static String GetIssuerCertURL(IX509Certificate certificate) {
            IAsn1Object obj;
            try {
                obj = GetExtensionValue(certificate, FACTORY.CreateExtensions().GetAuthorityInfoAccess().GetId());
                if (obj == null) {
                    return null;
                }
                IAsn1Sequence accessDescriptions = FACTORY.CreateASN1Sequence(obj);
                for (int i = 0; i < accessDescriptions.Size(); i++) {
                    IAsn1Sequence accessDescription = FACTORY.CreateASN1Sequence(accessDescriptions.GetObjectAt(i));
                    IDerObjectIdentifier id = FACTORY.CreateASN1ObjectIdentifier(accessDescription.GetObjectAt(0));
                    if (accessDescription.Size() == 2 && id != null && SecurityIDs.ID_CA_ISSUERS.Equals(id.GetId())) {
                        IAsn1Object description = FACTORY.CreateASN1Primitive(accessDescription.GetObjectAt(1));
                        return GetStringFromGeneralName(description);
                    }
                }
            }
            catch (System.IO.IOException) {
                return null;
            }
            return null;
        }

        // Time Stamp Authority
        /// <summary>Gets the URL of the TSA if it's available on the certificate</summary>
        /// <param name="certificate">a certificate</param>
        /// <returns>a TSA URL</returns>
        public static String GetTSAURL(IX509Certificate certificate) {
            byte[] der = SignUtils.GetExtensionValueByOid(certificate, SecurityIDs.ID_TSA);
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

        /// <summary>Checks if the certificate is signed by provided issuer certificate.</summary>
        /// <param name="subjectCertificate">a certificate to check</param>
        /// <param name="issuerCertificate">an issuer certificate to check</param>
        /// <returns>true if the first passed certificate is signed by next passed certificate.</returns>
        internal static bool IsIssuerCertificate(IX509Certificate subjectCertificate, IX509Certificate issuerCertificate
            ) {
            return subjectCertificate.GetIssuerDN().Equals(issuerCertificate.GetSubjectDN());
        }

        /// <summary>Checks if the certificate is self-signed.</summary>
        /// <param name="certificate">a certificate to check</param>
        /// <returns>true if the certificate is self-signed.</returns>
        internal static bool IsSelfSigned(IX509Certificate certificate) {
            return certificate.GetIssuerDN().Equals(certificate.GetSubjectDN());
        }

        // helper methods
        /// <param name="certificate">the certificate from which we need the ExtensionValue</param>
        /// <param name="oid">the Object Identifier value for the extension.</param>
        /// <returns>
        /// the extension value as an
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Object"/>
        /// object
        /// </returns>
        private static IAsn1Object GetExtensionValue(IX509Certificate certificate, String oid) {
            byte[] bytes = SignUtils.GetExtensionValueByOid(certificate, oid);
            if (bytes == null) {
                return null;
            }
            IAsn1OctetString octs;
            using (IAsn1InputStream aIn = FACTORY.CreateASN1InputStream(new MemoryStream(bytes))) {
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
    }
}
