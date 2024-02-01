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
using Org.BouncyCastle.Asn1.Tsp;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Tsp.MessageImprint"/>.
    /// </summary>
    public class MessageImprintBC : Asn1EncodableBC, IMessageImprint {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.MessageImprint"/>.
        /// </summary>
        /// <param name="messageImprint">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.MessageImprint"/>
        /// to be wrapped
        /// </param>
        public MessageImprintBC(MessageImprint messageImprint)
            : base(messageImprint) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.MessageImprint"/>.
        /// </returns>
        public virtual MessageImprint GetMessageImprint() {
            return (MessageImprint)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetHashedMessage() {
            return GetMessageImprint().GetHashedMessage();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier GetHashAlgorithm() {
            return new AlgorithmIdentifierBC(GetMessageImprint().HashAlgorithm);
        }
    }
}
