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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Openssl;

namespace iText.Signatures.Testutils {
    public sealed class PemFileHelper {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private PemFileHelper() {
            // Empty constructor.
        }
        
        public static IX509Certificate[] ReadFirstChain(String pemFileName) {
            return ReadCertificates(pemFileName).ToArray(new IX509Certificate[0]);
        }

        public static IPrivateKey ReadFirstKey(String pemFileName, char[] keyPass) {
            return ReadPrivateKey(pemFileName, keyPass);
        }

        public static List<IX509Certificate> InitStore(String pemFileName) {
            IX509Certificate[] chain = ReadFirstChain(pemFileName);
            return chain.Length > 0 ? new List<IX509Certificate> { chain[0] } : chain.ToList();
        }

        private static IList<IX509Certificate> ReadCertificates(String pemFileName) {
            using (TextReader file = new StreamReader(pemFileName)) {
                IPemReader parser = FACTORY.CreatePEMParser(file, null);
                Object readObject = parser.ReadObject();
                IList<IX509Certificate> certificates = new List<IX509Certificate>();
                while (readObject != null) {
                    if (readObject is IX509Certificate) {
                        certificates.Add((IX509Certificate)readObject);
                    }
                    readObject = parser.ReadObject();
                }
                return certificates;
            }
        }

        private static IPrivateKey ReadPrivateKey(String pemFileName, char[] keyPass) {
            using (TextReader file = new StreamReader(pemFileName)) {
                IPemReader parser = FACTORY.CreatePEMParser(file, keyPass);
                Object readObject = parser.ReadObject();
                while (!(readObject is IPrivateKey) && readObject != null) {
                    readObject = parser.ReadObject();
                }
                return (IPrivateKey)readObject;
            }
        }
    }
}
