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
using System;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Security;
using iText.Kernel.Exceptions;
using iText.Kernel.Mac;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Event;

namespace iText.Signatures.Mac {
//\cond DO_NOT_DOCUMENT
    /// <summary>Class responsible for integrity protection in encrypted documents which uses MAC container in the signature mode.
    ///     </summary>
    internal class SignatureMacIntegrityProtector : AbstractMacIntegrityProtector {
        private static readonly IBouncyCastleFactory BC_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private const String ID_ATTR_PDF_MAC_DATA = "1.0.32004.1.2";

//\cond DO_NOT_DOCUMENT
        internal SignatureMacIntegrityProtector(PdfDocument document, MacProperties macProperties)
            : base(document, macProperties) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal SignatureMacIntegrityProtector(PdfDocument document, PdfDictionary authDictionary)
            : base(document, authDictionary) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void PrepareDocument() {
            document.AddEventHandler(SignatureDocumentClosingEvent.START_SIGNATURE_PRE_CLOSE, new SignatureMacIntegrityProtector.SignatureMacPdfObjectAdder
                (this));
            document.AddEventHandler(SignatureContainerGenerationEvent.START_SIGNATURE_CONTAINER_GENERATION, new SignatureMacIntegrityProtector.SignatureMacContainerEmbedder
                (this));
        }
//\endcond

        private void EmbedMacContainerInUnsignedAttributes(IAsn1EncodableVector unsignedAttributes, Stream documentInputStream
            , byte[] signature) {
            IDerSequence mac;
            try {
                byte[] dataDigest = DigestBytes(documentInputStream);
                mac = CreateMacContainer(dataDigest, GenerateRandomBytes(32), signature);
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(KernelExceptionMessageConstant.CONTAINER_GENERATION_EXCEPTION, e);
            }
            IAsn1EncodableVector macAttribute = BC_FACTORY.CreateASN1EncodableVector();
            macAttribute.Add(BC_FACTORY.CreateASN1ObjectIdentifier(ID_ATTR_PDF_MAC_DATA));
            macAttribute.Add(BC_FACTORY.CreateDERSet(mac));
            unsignedAttributes.Add(BC_FACTORY.CreateDERSequence(macAttribute));
        }

        private sealed class SignatureMacPdfObjectAdder : AbstractPdfDocumentEventHandler {
            protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event) {
                if (@event is SignatureDocumentClosingEvent) {
                    PdfDictionary signatureMacDictionary = new PdfDictionary();
                    signatureMacDictionary.Put(PdfName.MACLocation, PdfName.AttachedToSig);
                    signatureMacDictionary.Put(PdfName.SigObjRef, ((SignatureDocumentClosingEvent)@event).GetSignatureReference
                        ());
                    this._enclosing.document.GetTrailer().Put(PdfName.AuthCode, signatureMacDictionary);
                }
            }

            internal SignatureMacPdfObjectAdder(SignatureMacIntegrityProtector _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly SignatureMacIntegrityProtector _enclosing;
        }

        private sealed class SignatureMacContainerEmbedder : AbstractPdfDocumentEventHandler {
            protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event) {
                if (@event is SignatureContainerGenerationEvent) {
                    SignatureContainerGenerationEvent signatureEvent = (SignatureContainerGenerationEvent)@event;
                    try {
                        this._enclosing.EmbedMacContainerInUnsignedAttributes(signatureEvent.GetUnsignedAttributes(), signatureEvent
                            .GetDocumentInputStream(), signatureEvent.GetSignature());
                    }
                    catch (System.IO.IOException e) {
                        throw new PdfException(KernelExceptionMessageConstant.CONTAINER_EMBEDDING_EXCEPTION, e);
                    }
                }
            }

            internal SignatureMacContainerEmbedder(SignatureMacIntegrityProtector _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly SignatureMacIntegrityProtector _enclosing;
        }
    }
//\endcond
}
