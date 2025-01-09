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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace iText.Test.Signutils {
    /// <summary>
    /// This class doesn't support bouncy-castle FIPS so it shall not be used in itextcore.
    /// </summary>
    public sealed class Pkcs12FileHelper {
        private Pkcs12FileHelper() {
        }

        public static X509Certificate[] ReadFirstChain(String p12FileName, char[] ksPass) {
            X509Certificate[] chain;
            string alias = null;
     
            Pkcs12Store pk12 = new Pkcs12StoreBuilder().Build();
            pk12.Load(new FileStream(p12FileName, FileMode.Open, FileAccess.Read), ksPass);
            
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
            Pkcs12Store pk12 = new Pkcs12StoreBuilder().Build();
            pk12.Load(new FileStream(p12FileName, FileMode.Open, FileAccess.Read), ksPass);
            
            foreach (var a in pk12.Aliases) {
                alias = ((string)a);
                if (pk12.IsKeyEntry(alias))
                    break;
            }
            ICipherParameters pk = pk12.GetKey(alias).Key;
            return pk;
        }

        public static ICipherParameters ReadPrivateKeyFromPkcs12KeyStore(Stream keyStore, String pkAlias, char[] pkPassword) {
            Pkcs12Store pk12 = new Pkcs12StoreBuilder().Build();
            pk12.Load(keyStore, pkPassword);
            return pk12.GetKey(pkAlias).Key;
        }

        public static List<X509Certificate> InitStore(String p12FileName, char[] ksPass) {
            List<X509Certificate> certStore = new List<X509Certificate>();
            string alias = null;
            Pkcs12Store pk12 = new Pkcs12StoreBuilder().Build();
            pk12.Load(new FileStream(p12FileName, FileMode.Open, FileAccess.Read), ksPass);

            foreach (var a in pk12.Aliases) {
                alias = ((string)a);
                X509CertificateEntry x509CertificateEntry;
                if ((pk12.IsCertificateEntry(alias) || pk12.IsKeyEntry(alias)) 
                    && (x509CertificateEntry = pk12.GetCertificate(alias)) != null) {
                    certStore.Add(x509CertificateEntry.Certificate);
                }
            }
            return certStore;
        }
    }
}
