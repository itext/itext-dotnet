/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Signatures.Validation.V1.Extensions {
    /// <summary>Certificate extension which is populated with additional dynamically changing validation related information.
    ///     </summary>
    public class DynamicCertificateExtension : CertificateExtension {
        private int certificateChainSize;

        /// <summary>
        /// Create new instance of
        /// <see cref="CertificateExtension"/>
        /// using provided extension OID and value.
        /// </summary>
        /// <param name="extensionOid">
        /// 
        /// <see cref="System.String"/>
        /// , which represents extension OID
        /// </param>
        /// <param name="extensionValue">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Object"/>
        /// , which represents extension value
        /// </param>
        public DynamicCertificateExtension(String extensionOid, IAsn1Object extensionValue)
            : base(extensionOid, extensionValue) {
        }

        /// <summary>Sets amount of certificates currently present in the chain.</summary>
        /// <param name="certificateChainSize">amount of certificates currently present in the chain</param>
        /// <returns>
        /// this
        /// <see cref="DynamicCertificateExtension"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.V1.Extensions.DynamicCertificateExtension WithCertificateChainSize
            (int certificateChainSize) {
            this.certificateChainSize = certificateChainSize;
            return this;
        }

        /// <summary>Gets amount of certificates currently present in the chain.</summary>
        /// <returns>amount of certificates currently present in the chain</returns>
        public virtual int GetCertificateChainSize() {
            return certificateChainSize;
        }
    }
}
