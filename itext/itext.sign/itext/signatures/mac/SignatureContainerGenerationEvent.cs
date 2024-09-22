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
