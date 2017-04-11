using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace iText.Signatures.Testutils {
    public sealed class Pkcs12FileHelper {
        private Pkcs12FileHelper() {
        }

        public static X509Certificate[] ReadFirstChain(String p12FileName, char[] ksPass) {
            X509Certificate[] chain;
            string alias = null;
            Pkcs12Store pk12 = new Pkcs12Store(new FileStream(p12FileName, FileMode.Open, FileAccess.Read), ksPass);

            foreach (var a in pk12.Aliases) {
                alias = ((string)a);
                if (pk12.IsKeyEntry(alias))
                    break;
            }
            X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
            chain = new X509Certificate[ce.Length];
            for (int k = 0; k < ce.Length; ++k)
                chain[k] = ce[k].Certificate;

            return chain;
        }
    

        public static ICipherParameters ReadFirstKey(String p12FileName, char[] ksPass, char[] keyPass) {
            string alias = null;
            Pkcs12Store pk12 = new Pkcs12Store(new FileStream(p12FileName, FileMode.Open, FileAccess.Read), ksPass);

            foreach (var a in pk12.Aliases) {
                alias = ((string)a);
                if (pk12.IsKeyEntry(alias))
                    break;
            }
            ICipherParameters pk = pk12.GetKey(alias).Key;
            return pk;
        }

        public static List<X509Certificate> InitStore(String p12FileName, char[] ksPass) {
            List<X509Certificate> certStore = new List<X509Certificate>();
            string alias = null;
            Pkcs12Store pk12 = new Pkcs12Store(new FileStream(p12FileName, FileMode.Open, FileAccess.Read), ksPass);

            foreach (var a in pk12.Aliases) {
                alias = ((string)a);
                if (pk12.IsCertificateEntry(alias)) {
                    certStore.Add(pk12.GetCertificate(alias).Certificate);
                }
            }
            return certStore;
        }
    }
}
