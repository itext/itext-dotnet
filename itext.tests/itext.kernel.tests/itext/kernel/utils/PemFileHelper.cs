using System;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Openssl;

namespace iText.Kernel.Utils {
    public sealed class PemFileHelper {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private PemFileHelper() {
            // Empty constructor.
        }
        
        public static IPrivateKey ReadPrivateKeyFromPemFile(Stream pemFile, char[] keyPass) {
            return ReadPrivateKey(pemFile, keyPass);
        }
        
        private static IPrivateKey ReadPrivateKey(Stream pemFile, char[] keyPass) {
            using (TextReader file = new StreamReader(pemFile)) {
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