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
using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
    /// </summary>
    public class ExtendedKeyUsageBCFips : Asn1EncodableBCFips, IExtendedKeyUsage {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
        /// </summary>
        /// <param name="extendedKeyUsage">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>
        /// to be wrapped
        /// </param>
        public ExtendedKeyUsageBCFips(ExtendedKeyUsage extendedKeyUsage)
            : base(extendedKeyUsage) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
        /// </summary>
        /// <param name="purposeId">KeyPurposeId wrapper</param>
        public ExtendedKeyUsageBCFips(IKeyPurposeID purposeId)
            : base(new ExtendedKeyUsage(((KeyPurposeIDBCFips)purposeId).GetKeyPurposeID())) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
        /// </summary>
        /// <param name="purposeIds">KeyPurposeId wrappers array</param>
        public ExtendedKeyUsageBCFips(IKeyPurposeID[] purposeIds)
            : base(new ExtendedKeyUsage(UnwrapPurposeIds(purposeIds))) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage"/>.
        /// </returns>
        public virtual ExtendedKeyUsage GetExtendedKeyUsage() {
            return (ExtendedKeyUsage)GetEncodable();
        }

        private static KeyPurposeID[] UnwrapPurposeIds(IKeyPurposeID[] purposeIds) {
            KeyPurposeID[] purposeIdsUnwrapped = new KeyPurposeID[purposeIds.Length];
            for (int i = 0; i < purposeIds.Length; ++i) {
                purposeIdsUnwrapped[i] = ((KeyPurposeIDBCFips)purposeIds[i]).GetKeyPurposeID();
            }
            return purposeIdsUnwrapped;
        }
    }
}
