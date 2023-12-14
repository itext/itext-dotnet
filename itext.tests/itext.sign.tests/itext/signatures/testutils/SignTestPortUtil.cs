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
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using iText.Kernel.Pdf;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security.Certificates;

namespace iText.Signatures.Testutils {
    public class SignTestPortUtil {
        public static CertificateID GenerateCertificateId(X509Certificate issuerCert, BigInteger serialNumber, String hashAlgorithm) {
            return new CertificateID(hashAlgorithm, issuerCert, serialNumber);
        }

        public static OcspReq GenerateOcspRequestWithNonce(CertificateID id) {
            OcspReqGenerator gen = new OcspReqGenerator();
            gen.AddRequest(id);

            // create details for nonce extension
            IDictionary extensions = new Hashtable();

            extensions[OcspObjectIdentifiers.PkixOcspNonce] = new X509Extension(false, new DerOctetString(new DerOctetString(PdfEncryption.GenerateNewDocumentId()).GetEncoded()));

            gen.SetRequestExtensions(new X509Extensions(extensions));
            return gen.Generate();
        }

        public static IDigest GetMessageDigest(String hashAlgorithm) {
            return DigestUtilities.GetDigest(hashAlgorithm);
        }

        public static X509Crl ParseCrlFromStream(Stream input) {
            return new X509CrlParser().ReadCrl(input);
        }
        
        public static RsaKeyPairGenerator BuildRSA2048KeyPairGenerator() {
            RsaKeyPairGenerator keyGen = new RsaKeyPairGenerator();
            keyGen.Init(new KeyGenerationParameters(new SecureRandom(new VmpcRandomGenerator()), 2048));
            return keyGen;
        }
    }
}
