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
using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>.
    /// </summary>
    public class RecipientIdentifierBC : Asn1EncodableBC, IRecipientIdentifier {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>.
        /// </summary>
        /// <param name="recipientIdentifier">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>
        /// to be wrapped
        /// </param>
        public RecipientIdentifierBC(RecipientIdentifier recipientIdentifier)
            : base(recipientIdentifier) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>.
        /// </summary>
        /// <param name="issuerAndSerialNumber">
        /// IssuerAndSerialNumber wrapper to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>
        /// </param>
        public RecipientIdentifierBC(IIssuerAndSerialNumber issuerAndSerialNumber)
            : base(new RecipientIdentifier(((IssuerAndSerialNumberBC)issuerAndSerialNumber).GetIssuerAndSerialNumber()
                )) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>.
        /// </returns>
        public virtual RecipientIdentifier GetRecipientIdentifier() {
            return (RecipientIdentifier)GetEncodable();
        }
    }
}
