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
                IPEMParser parser = FACTORY.CreatePEMParser(file, null);
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
                IPEMParser parser = FACTORY.CreatePEMParser(file, keyPass);
                Object readObject = parser.ReadObject();
                while (!(readObject is IPrivateKey) && readObject != null) {
                    readObject = parser.ReadObject();
                }
                return (IPrivateKey)readObject;
            }
        }
    }
}
