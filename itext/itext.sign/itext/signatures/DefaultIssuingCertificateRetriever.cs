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
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures {
    /// <summary>
    /// Empty
    /// <see cref="IIssuingCertificateRetriever"/>
    /// implementation for compatibility with the older code.
    /// </summary>
    internal class DefaultIssuingCertificateRetriever : IIssuingCertificateRetriever {
        /// <summary>
        /// Creates
        /// <see cref="DefaultIssuingCertificateRetriever"/>
        /// instance.
        /// </summary>
        public DefaultIssuingCertificateRetriever() {
        }

        // Empty constructor.
        /// <summary><inheritDoc/></summary>
        /// <param name="chain">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual IX509Certificate[] RetrieveMissingCertificates(IX509Certificate[] chain) {
            return chain;
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="crl">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual IX509Certificate[] GetCrlIssuerCertificates(IX509Crl crl) {
            return new IX509Certificate[0];
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="certificates">
        /// 
        /// <inheritDoc/>
        /// </param>
        public virtual void SetTrustedCertificates(ICollection<IX509Certificate> certificates) {
        }
        // Do nothing.
    }
}
