/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>.
    /// </summary>
    public class ExtensionsBC : ASN1EncodableBC, IExtensions {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>.
        /// </summary>
        /// <param name="extensions">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>
        /// to be wrapped
        /// </param>
        public ExtensionsBC(Extensions extensions)
            : base(extensions) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>.
        /// </summary>
        /// <param name="extensions">Extension wrapper</param>
        public ExtensionsBC(IExtension extensions)
            : base(new Extensions(((ExtensionBC)extensions).GetExtension())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.Extensions"/>.
        /// </returns>
        public virtual Extensions GetExtensions() {
            return (Extensions)GetEncodable();
        }
    }
}