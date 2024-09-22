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
using System.IO;
using iText.Commons.Bouncycastle.Asn1;
using iText.Kernel.Events;

namespace iText.Signatures.Mac {
    /// <summary>Represents an event firing before creating signature container.</summary>
    public class SignatureContainerGenerationEvent : Event {
        public const String START_SIGNATURE_CONTAINER_GENERATION = "StartSignatureContainerGeneration";

        private readonly IAsn1EncodableVector unsignedAttributes;

        private readonly byte[] signature;

        private readonly Stream documentInputStream;

        /// <summary>Creates an event firing before creating the signature container.</summary>
        /// <param name="unsignedAttributes">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1EncodableVector"/>
        /// unsigned signature attributes
        /// </param>
        /// <param name="signature">
        /// 
        /// <c>byte[]</c>
        /// signature value
        /// </param>
        /// <param name="documentInputStream">
        /// 
        /// <see cref="System.IO.Stream"/>
        /// containing document bytes considering byte range
        /// </param>
        public SignatureContainerGenerationEvent(IAsn1EncodableVector unsignedAttributes, byte[] signature, Stream
             documentInputStream)
            : base(START_SIGNATURE_CONTAINER_GENERATION) {
            this.unsignedAttributes = unsignedAttributes;
            this.signature = signature;
            this.documentInputStream = documentInputStream;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1EncodableVector"/>
        /// unsigned signature attributes.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1EncodableVector"/>
        /// unsigned signature attributes
        /// </returns>
        public virtual IAsn1EncodableVector GetUnsignedAttributes() {
            return unsignedAttributes;
        }

        /// <summary>
        /// Gets
        /// <c>byte[]</c>
        /// signature value.
        /// </summary>
        /// <returns>
        /// 
        /// <c>byte[]</c>
        /// signature value
        /// </returns>
        public virtual byte[] GetSignature() {
            return signature;
        }

        /// <summary>
        /// Gets
        /// <see cref="System.IO.Stream"/>
        /// containing document bytes considering byte range.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="System.IO.Stream"/>
        /// containing document bytes considering byte range
        /// </returns>
        public virtual Stream GetDocumentInputStream() {
            return documentInputStream;
        }
    }
}
