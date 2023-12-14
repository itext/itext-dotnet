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
using Org.BouncyCastle;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using iText.IO.Util;

namespace iText.Signatures {
    /// <summary>
    /// This class contains a series of static methods that
    /// allow you to retrieve information from a Certificate.
    /// </summary>
    public class CertificateUtil {
        // Certificate Revocation Lists
        /// <summary>Gets a CRL from an X509 certificate.</summary>
        /// <param name="certificate">the X509Certificate to extract the CRL from</param>
        /// <returns>CRL or null if there's no CRL available</returns>
        public static X509Crl GetCRL(X509Certificate certificate) {
            return CertificateUtil.GetCRL(CertificateUtil.GetCRLURL(certificate));
        }

        /// <summary>Gets the URL of the Certificate Revocation List for a Certificate</summary>
        /// <param name="certificate">the Certificate</param>
        /// <returns>the String where you can check if the certificate was revoked</returns>
        public static String GetCRLURL(X509Certificate certificate) {
            Asn1Object obj;
            try {
                obj = GetExtensionValue(certificate, X509Extensions.CrlDistributionPoints.Id);
            }
            catch (System.IO.IOException) {
                obj = (Asn1Object)null;
            }
            if (obj == null) {
                return null;
            }
            CrlDistPoint dist = CrlDistPoint.GetInstance(obj);
            DistributionPoint[] dists = dist.GetDistributionPoints();
            foreach (DistributionPoint p in dists) {
                DistributionPointName distributionPointName = p.DistributionPointName;
                if (DistributionPointName.FullName != distributionPointName.PointType) {
                    continue;
                }
                GeneralNames generalNames = (GeneralNames)distributionPointName.Name;
                GeneralName[] names = generalNames.GetNames();
                foreach (GeneralName name in names) {
                    if (name.TagNo != GeneralName.UniformResourceIdentifier) {
                        continue;
                    }
                    DerIA5String derStr = (DerIA5String)DerIA5String.GetInstance((Asn1TaggedObject)name.ToAsn1Object(), false);
                    return derStr.GetString();
                }
            }
            return null;
        }

        /// <summary>Gets the CRL object using a CRL URL.</summary>
        /// <param name="url">the URL where the CRL is located</param>
        /// <returns>CRL object</returns>
        public static X509Crl GetCRL(String url) {
            if (url == null) {
                return null;
            }
            return SignUtils.ParseCrlFromStream(UrlUtil.OpenStream(new Uri(url)));
        }

        // Online Certificate Status Protocol
        /// <summary>Retrieves the OCSP URL from the given certificate.</summary>
        /// <param name="certificate">the certificate</param>
        /// <returns>the URL or null</returns>
        public static String GetOCSPURL(X509Certificate certificate) {
            Asn1Object obj;
            try {
                obj = GetExtensionValue(certificate, X509Extensions.AuthorityInfoAccess.Id);
                if (obj == null) {
                    return null;
                }
                Asn1Sequence AccessDescriptions = (Asn1Sequence)obj;
                for (int i = 0; i < AccessDescriptions.Count; i++) {
                    Asn1Sequence AccessDescription = (Asn1Sequence)AccessDescriptions[i];
                    if (AccessDescription.Count != 2) {
                        continue;
                    }
                    else {
                        if (AccessDescription[0] is DerObjectIdentifier) {
                            DerObjectIdentifier id = (DerObjectIdentifier)AccessDescription[0];
                            if (SecurityIDs.ID_OCSP.Equals(id.Id)) {
                                Asn1Object description = (Asn1Object)AccessDescription[1];
                                String AccessLocation = GetStringFromGeneralName(description);
                                if (AccessLocation == null) {
                                    return "";
                                }
                                else {
                                    return AccessLocation;
                                }
                            }
                        }
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
        public static String GetTSAURL(X509Certificate certificate) {
            byte[] der = SignUtils.GetExtensionValueByOid(certificate, SecurityIDs.ID_TSA);
            if (der == null) {
                return null;
            }
            Asn1Object asn1obj;
            try {
                asn1obj = Asn1Object.FromByteArray(der);
                DerOctetString octets = (DerOctetString)asn1obj;
                asn1obj = Asn1Object.FromByteArray(octets.GetOctets());
                Asn1Sequence asn1seq = Asn1Sequence.GetInstance(asn1obj);
                return GetStringFromGeneralName(asn1seq[1].ToAsn1Object());
            }
            catch (System.IO.IOException) {
                return null;
            }
        }

        // helper methods
        /// <param name="certificate">the certificate from which we need the ExtensionValue</param>
        /// <param name="oid">the Object Identifier value for the extension.</param>
        /// <returns>the extension value as an ASN1Primitive object</returns>
        private static Asn1Object GetExtensionValue(X509Certificate certificate, String oid) {
            byte[] bytes = SignUtils.GetExtensionValueByOid(certificate, oid);
            if (bytes == null) {
                return null;
            }
            Asn1InputStream aIn = new Asn1InputStream(new MemoryStream(bytes));
            Asn1OctetString octs = (Asn1OctetString)aIn.ReadObject();
            aIn = new Asn1InputStream(new MemoryStream(octs.GetOctets()));
            return aIn.ReadObject();
        }

        /// <summary>Gets a String from an ASN1Primitive</summary>
        /// <param name="names">the ASN1Primitive</param>
        /// <returns>a human-readable String</returns>
        private static String GetStringFromGeneralName(Asn1Object names) {
            Asn1TaggedObject taggedObject = (Asn1TaggedObject)names;
            return iText.IO.Util.JavaUtil.GetStringForBytes(Asn1OctetString.GetInstance(taggedObject, false).GetOctets
                (), "ISO-8859-1");
        }
    }
}
