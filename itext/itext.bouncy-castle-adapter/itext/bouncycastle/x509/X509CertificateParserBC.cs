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
using System.Collections.Generic;
using iText.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.X509;
using iText.Commons.Utils;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="X509CertificateParser"/>.
    /// </summary>
    public class X509CertificateParserBC : IX509CertificateParser {
        private readonly X509CertificateParser certificateParser;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="X509CertificateParser"/>.
        /// </summary>
        /// <param name="certificateParser">
        /// <see cref="X509CertificateParser"/>
        /// to be wrapped
        /// </param>
        public X509CertificateParserBC(X509CertificateParser certificateParser) {
            this.certificateParser = certificateParser;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="X509CertificateParser"/>.
        /// </returns>
        public X509CertificateParser GetCertificateParser() {
            return certificateParser;
        }

        /// <summary><inheritDoc/></summary>
        public List<IX509Certificate> ReadAllCerts(byte[] contentsKey) {
            List<IX509Certificate> certs = new List<IX509Certificate>();

            foreach (X509Certificate cc in certificateParser.ReadCertificates(contentsKey)) {
                certs.Add(new X509CertificateBC(cc));
            }
            return certs;
        }
        
        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            X509CertificateParserBC that = (X509CertificateParserBC)o;
            return Object.Equals(certificateParser, that.certificateParser);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateParser);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return certificateParser.ToString();
        }
    }
}
