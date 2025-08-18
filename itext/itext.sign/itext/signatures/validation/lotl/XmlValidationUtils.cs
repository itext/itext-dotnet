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
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using iText.Bouncycastleconnector;
using iText.Kernel.Utils;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    internal sealed class XmlValidationUtils {
        private XmlValidationUtils() {
        }

        // Private constructor so that the class instance cannot be instantiated.
        public static bool CreateXmlDocumentAndCheckValidity(Stream xmlDocumentInputStream, CertificateSelector keySelector) {
            RegisterNotSupportedAlgorithms();
            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(XmlProcessorCreator.CreateSafeXmlReader(xmlDocumentInputStream));
            SignedXml signedXml = new SignedXml(xmlDoc);
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("ds:Signature");
            if (nodeList.Count == 0) {
                nodeList = xmlDoc.GetElementsByTagName("dsig:Signature");
            }
            if (nodeList.Count == 0) {
                nodeList = xmlDoc.GetElementsByTagName("Signature");
            }
            signedXml.LoadXml((XmlElement)nodeList[0]);
            keySelector.Select(signedXml.KeyInfo);
            AsymmetricAlgorithm algorithm;
            try {
                algorithm = keySelector.GetNetCert().PublicKey.Key;
            }
            catch (Exception) {
                algorithm = keySelector.GetNetCert().GetECDsaPublicKey();
            }
            try {
                return signedXml.CheckSignature(algorithm);
            }
            catch (Exception e) {
                // Try with different public key
                RSAParameters? rsaParameters = BouncyCastleFactoryCreator.GetFactory()
                    .GetRsaParametersFromCertificate(keySelector.GetCertificate());
                if (rsaParameters != null) {
                    RSACng bcAlgorithm = new RSACng();
                    bcAlgorithm.ImportParameters((RSAParameters)rsaParameters);
                    return signedXml.CheckSignature(bcAlgorithm);
                }
                throw;
            }
        }

        private static void RegisterNotSupportedAlgorithms() {
            if (CryptoConfig.CreateFromName("http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha1") == null) {
                CryptoConfig.AddAlgorithm(typeof(ECDSASignatureDescription.ECDSASignatureDescritionSHA1),
                    "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha1");
            }
            if (CryptoConfig.CreateFromName("http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha256") == null) {
                CryptoConfig.AddAlgorithm(typeof(ECDSASignatureDescription.ECDSASignatureDescritionSHA256),
                    "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha256");
            }
            if (CryptoConfig.CreateFromName("http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha384") == null) {
                CryptoConfig.AddAlgorithm(typeof(ECDSASignatureDescription.ECDSASignatureDescritionSHA384),
                    "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha384");
            }
            if (CryptoConfig.CreateFromName("http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha512") == null) {
                CryptoConfig.AddAlgorithm(typeof(ECDSASignatureDescription.ECDSASignatureDescritionSHA512),
                    "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha512");
            }

            if (CryptoConfig.CreateFromName("http://www.w3.org/2007/05/xmldsig-more#sha1-rsa-MGF1") == null) {
                CryptoConfig.AddAlgorithm(typeof(RsaPssSignatureDescription.RsaPssSignatureDescriptionSHA1),
                    "http://www.w3.org/2007/05/xmldsig-more#sha1-rsa-MGF1");
            }
            if (CryptoConfig.CreateFromName("http://www.w3.org/2007/05/xmldsig-more#sha256-rsa-MGF1") == null) {
                CryptoConfig.AddAlgorithm(typeof(RsaPssSignatureDescription.RsaPssSignatureDescriptionSHA256),
                    "http://www.w3.org/2007/05/xmldsig-more#sha256-rsa-MGF1");
            }
            if (CryptoConfig.CreateFromName("http://www.w3.org/2007/05/xmldsig-more#sha384-rsa-MGF1") == null) {
                CryptoConfig.AddAlgorithm(typeof(RsaPssSignatureDescription.RsaPssSignatureDescriptionSHA384),
                    "http://www.w3.org/2007/05/xmldsig-more#sha384-rsa-MGF1");
            }
            if (CryptoConfig.CreateFromName("http://www.w3.org/2007/05/xmldsig-more#sha512-rsa-MGF1") == null) {
                CryptoConfig.AddAlgorithm(typeof(RsaPssSignatureDescription.RsaPssSignatureDescriptionSHA512),
                    "http://www.w3.org/2007/05/xmldsig-more#sha512-rsa-MGF1");
            }
        }
    }
//\endcond
}
