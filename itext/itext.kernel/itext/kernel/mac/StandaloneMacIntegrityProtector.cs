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
using iText.Commons.Bouncycastle.Security;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Event;

namespace iText.Kernel.Mac {
//\cond DO_NOT_DOCUMENT
    /// <summary>Class responsible for integrity protection in encrypted documents, which uses MAC container in the standalone mode.
    ///     </summary>
    internal class StandaloneMacIntegrityProtector : AbstractMacIntegrityProtector {
        private MacPdfObject macPdfObject;

//\cond DO_NOT_DOCUMENT
        internal StandaloneMacIntegrityProtector(PdfDocument document, MacProperties macProperties)
            : base(document, macProperties) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal StandaloneMacIntegrityProtector(PdfDocument document, PdfDictionary authDictionary)
            : base(document, authDictionary) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void PrepareDocument() {
            document.AddEventHandler(PdfDocumentEvent.START_DOCUMENT_CLOSING, new StandaloneMacIntegrityProtector.StandaloneMacPdfObjectAdder
                (this));
            document.AddEventHandler(PdfDocumentEvent.START_WRITER_CLOSING, new StandaloneMacIntegrityProtector.StandaloneMacContainerEmbedder
                (this));
        }
//\endcond

        private void EmbedMacContainerInTrailer() {
            byte[] documentBytes = GetDocumentByteArrayOutputStream().ToArray();
            long[] byteRange = macPdfObject.ComputeByteRange(documentBytes.Length);
            long byteRangePosition = macPdfObject.GetByteRangePosition();
            MemoryStream localBaos = new MemoryStream();
            PdfOutputStream os = new PdfOutputStream(localBaos);
            os.Write('[');
            foreach (long l in byteRange) {
                os.WriteLong(l).Write(' ');
            }
            os.Write(']');
            Array.Copy(localBaos.ToArray(), 0, documentBytes, (int)byteRangePosition, localBaos.Length);
            byte[] mac = CreateDocumentDigestAndMacContainer(documentBytes, byteRange);
            PdfString macString = new PdfString(mac).SetHexWriting(true);
            // fill in the MAC
            localBaos.JReset();
            os.Write(macString);
            Array.Copy(localBaos.ToArray(), 0, documentBytes, (int)byteRange[1], localBaos.Length);
            GetDocumentByteArrayOutputStream().JReset();
            document.GetWriter().GetOutputStream().Write(documentBytes, 0, documentBytes.Length);
        }

        private byte[] CreateDocumentDigestAndMacContainer(byte[] documentBytes, long[] byteRange) {
            IRandomAccessSource ras = new RandomAccessSourceFactory().CreateSource(documentBytes);
            try {
                using (Stream rg = new RASInputStream(new RandomAccessSourceFactory().CreateRanged(ras, byteRange))) {
                    byte[] dataDigest = DigestBytes(rg);
                    return CreateMacContainer(dataDigest, GenerateRandomBytes(32), null).GetEncoded();
                }
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(KernelExceptionMessageConstant.CONTAINER_GENERATION_EXCEPTION, e);
            }
        }

        private int GetContainerSizeEstimate() {
            try {
                return CreateMacContainer(DigestBytes(new byte[0]), GenerateRandomBytes(32), null).GetEncoded().Length * 2
                     + 2;
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(KernelExceptionMessageConstant.CONTAINER_GENERATION_EXCEPTION, e);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.CONTAINER_GENERATION_EXCEPTION, e);
            }
        }

        private MemoryStream GetDocumentByteArrayOutputStream() {
            return ((MemoryStream)document.GetWriter().GetOutputStream());
        }

        private sealed class StandaloneMacPdfObjectAdder : AbstractPdfDocumentEventHandler {
            protected internal override void OnAcceptedEvent(AbstractPdfDocumentEvent @event) {
                this._enclosing.macPdfObject = new MacPdfObject(this._enclosing.GetContainerSizeEstimate());
                this._enclosing.document.GetTrailer().Put(PdfName.AuthCode, this._enclosing.macPdfObject.GetPdfObject());
            }

            internal StandaloneMacPdfObjectAdder(StandaloneMacIntegrityProtector _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly StandaloneMacIntegrityProtector _enclosing;
        }

        private sealed class StandaloneMacContainerEmbedder : AbstractPdfDocumentEventHandler {
            protected internal override void OnAcceptedEvent(AbstractPdfDocumentEvent @event) {
                try {
                    this._enclosing.EmbedMacContainerInTrailer();
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(KernelExceptionMessageConstant.CONTAINER_EMBEDDING_EXCEPTION, e);
                }
            }

            internal StandaloneMacContainerEmbedder(StandaloneMacIntegrityProtector _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly StandaloneMacIntegrityProtector _enclosing;
        }
    }
//\endcond
}
