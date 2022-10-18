/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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

        public static ICipherParameters ReadPrivateKeyFromPkcs12KeyStore(Stream keyStore, String pkAlias, char[] pkPassword) {
            return new Pkcs12Store(keyStore, pkPassword).GetKey(pkAlias).Key;
        }

        public static List<X509Certificate> InitStore(String p12FileName, char[] ksPass) {
            List<X509Certificate> certStore = new List<X509Certificate>();
            string alias = null;
            Pkcs12Store pk12 = new Pkcs12Store(new FileStream(p12FileName, FileMode.Open, FileAccess.Read), ksPass);

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
