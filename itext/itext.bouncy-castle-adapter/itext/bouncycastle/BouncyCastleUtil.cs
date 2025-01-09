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
using System.Collections.Generic;
using System.IO;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle {
    //\cond DO_NOT_DOCUMENT
    internal class BouncyCastleUtil : IBouncyCastleUtil {

        internal BouncyCastleUtil() {
            // Empty constructor.
        }

        /// <summary><inheritDoc/></summary>
        public virtual List<IX509Certificate> ReadPkcs7Certs(Stream data) {
            using (TextReader file = new StreamReader(data)) {
                PemReader reader = new PemReader(file, new BouncyCastleFactory.BouncyCastlePasswordFinder(null));
                object obj = reader.ReadObject();
                if (!(obj is ContentInfo)) {
                    return new List<IX509Certificate>();
                }
                CmsSignedData cmsSignedData = new CmsSignedData((ContentInfo)obj);
                IEnumerable<X509Certificate> certs = cmsSignedData.GetCertificates().EnumerateMatches(null);
                List<IX509Certificate> certsWrappers = new List<IX509Certificate>();
                foreach (X509Certificate certificate in certs) {
                    certsWrappers.Add(new X509CertificateBC(certificate));
                }

                return certsWrappers;
            }
        }
    }
    //\endcond
}
