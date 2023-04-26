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
using System.Collections;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Crypto.Generators;
using iText.Commons.Bouncycastle.Math;
using iText.Kernel.Pdf;

namespace iText.Signatures.Testutils {
    public class SignTestPortUtil {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        public static ICertID GenerateCertificateId(IX509Certificate issuerCert, IBigInteger serialNumber, String hashAlgorithm) {
            return FACTORY.CreateCertificateID(hashAlgorithm, issuerCert, serialNumber);
        }

        public static IOcspRequest GenerateOcspRequestWithNonce(ICertID id) {
            IOcspReqGenerator gen = FACTORY.CreateOCSPReqBuilder();
            gen.AddRequest(id);

            // create details for nonce extension
            IDictionary extensions = new Hashtable();

            extensions[FACTORY.CreateOCSPObjectIdentifiers().GetIdPkixOcspNonce()] = FACTORY.CreateExtension(false, 
                FACTORY.CreateDEROctetString(FACTORY.CreateDEROctetString(PdfEncryption.GenerateNewDocumentId()).GetEncoded()));

            gen.SetRequestExtensions(FACTORY.CreateExtensions(extensions));
            return gen.Build();
        }

        public static IDigest GetMessageDigest(String hashAlgorithm) {
            return FACTORY.CreateIDigest(hashAlgorithm);
        }

        public static IX509Crl ParseCrlFromStream(Stream input) {
            return FACTORY.CreateX509Crl(input);
        }
        
        public static IRsaKeyPairGenerator BuildRSA2048KeyPairGenerator() {
            return FACTORY.CreateRsa2048KeyPairGenerator();
        }
    }
}
