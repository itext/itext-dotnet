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
﻿using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Cms;
using iText.Signatures.Exceptions;

namespace iText.Signatures {
    public class PdfTwoPhaseSigner {
        private readonly PdfReader reader;

        private readonly Stream outputStream;

        private StampingProperties stampingProperties = new StampingProperties().UseAppendMode();

        private bool closed;

        public PdfTwoPhaseSigner(PdfReader reader, Stream outputStream) {
            this.reader = reader;
            this.outputStream = outputStream;
        }

        /// <summary>Prepares document for signing, calculates the document digest to sign and closes the document.</summary>
        /// <param name="signerProperties">
        /// 
        /// <see cref="SignerProperties"/>
        /// properties to be used for main signing operation
        /// </param>
        /// <param name="digestAlgorithm">the algorithm to generate the digest with</param>
        /// <param name="filter">PdfName of the signature handler to use when validating this signature</param>
        /// <param name="subFilter">PdfName that describes the encoding of the signature</param>
        /// <param name="estimatedSize">
        /// the estimated size of the signature, this is the size of the space reserved for
        /// the Cryptographic Message Container
        /// </param>
        /// <param name="includeDate">specifies if the signing date should be set to the signature dictionary</param>
        /// <returns>the message digest of the prepared document.</returns>
        public virtual byte[] PrepareDocumentForSignature(SignerProperties signerProperties, String digestAlgorithm
            , PdfName filter, PdfName subFilter, int estimatedSize, bool includeDate) {
            IDigest digest;
            digest = SignUtils.GetMessageDigest(digestAlgorithm);
            return PrepareDocumentForSignature(signerProperties, digest, filter, subFilter, estimatedSize, includeDate
                );
        }

        /// <summary>Adds an existing signature to a PDF where space was already reserved.</summary>
        /// <param name="document">the original PDF</param>
        /// <param name="fieldName">the field to sign. It must be the last field</param>
        /// <param name="outs">the output PDF</param>
        /// <param name="cmsContainer">the finalized CMS container</param>
        public static void AddSignatureToPreparedDocument(PdfDocument document, String fieldName, Stream outs, CMSContainer
             cmsContainer) {
            PdfSigner.SignatureApplier applier = new PdfSigner.SignatureApplier(document, fieldName, outs);
            applier.Apply((a) => cmsContainer.Serialize());
        }

        /// <summary>Adds an existing signature to a PDF where space was already reserved.</summary>
        /// <param name="document">the original PDF</param>
        /// <param name="fieldName">the field to sign. It must be the last field</param>
        /// <param name="outs">the output PDF</param>
        /// <param name="signedContent">the bytes for the signed data</param>
        public static void AddSignatureToPreparedDocument(PdfDocument document, String fieldName, Stream outs, byte
            [] signedContent) {
            PdfSigner.SignatureApplier applier = new PdfSigner.SignatureApplier(document, fieldName, outs);
            applier.Apply((a) => signedContent);
        }

        /// <summary>Set stamping properties to be used during main signing operation.</summary>
        /// <remarks>
        /// Set stamping properties to be used during main signing operation.
        /// <para />
        /// If none is set, stamping properties with append mode enabled will be used
        /// </remarks>
        /// <param name="stampingProperties">
        /// 
        /// <see cref="iText.Kernel.Pdf.StampingProperties"/>
        /// instance to be used during main signing operation
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="PdfTwoPhaseSigner"/>
        /// </returns>
        public virtual iText.Signatures.PdfTwoPhaseSigner SetStampingProperties(StampingProperties stampingProperties
            ) {
            this.stampingProperties = stampingProperties;
            return this;
        }

        internal virtual PdfSigner CreatePdfSigner(SignerProperties signerProperties) {
            PdfSigner signer = new PdfSigner(reader, outputStream, null, stampingProperties);
            signer.SetFieldLockDict(signerProperties.GetFieldLockDict());
            signer.SetFieldName(signerProperties.GetFieldName());
            // We need to update field name because signer could change it
            signerProperties.SetFieldName(signer.GetFieldName());
            signer.SetCertificationLevel(signerProperties.GetCertificationLevel());
            signer.SetPageRect(signerProperties.GetPageRect());
            signer.SetPageNumber(signerProperties.GetPageNumber());
            signer.SetSignDate(signerProperties.GetSignDate());
            signer.SetSignatureCreator(signerProperties.GetSignatureCreator());
            signer.SetContact(signerProperties.GetContact());
            signer.SetReason(signerProperties.GetReason());
            signer.SetLocation(signerProperties.GetLocation());
            signer.SetSignatureAppearance(signerProperties.GetSignatureAppearance());
            return signer;
        }

        private byte[] PrepareDocumentForSignature(SignerProperties signerProperties, IDigest messageDigest, PdfName
             filter, PdfName subFilter, int estimatedSize, bool includeDate) {
            if (closed) {
                throw new PdfException(SignExceptionMessageConstant.THIS_INSTANCE_OF_PDF_SIGNER_ALREADY_CLOSED);
            }
            PdfSigner pdfSigner = CreatePdfSigner(signerProperties);
            
            PdfDocument document = pdfSigner.GetDocument();
            if (document.GetPdfVersion().CompareTo(PdfVersion.PDF_2_0) < 0) {
                document.GetCatalog().AddDeveloperExtension(PdfDeveloperExtension.ESIC_1_7_EXTENSIONLEVEL2);
            }
            document.GetCatalog().AddDeveloperExtension(PdfDeveloperExtension.ISO_32002);
            document.GetCatalog().AddDeveloperExtension(PdfDeveloperExtension.ISO_32001);
        
            PdfSignature cryptoDictionary = pdfSigner.CreateSignatureDictionary(includeDate);
            cryptoDictionary.Put(PdfName.Filter, filter);
            cryptoDictionary.Put(PdfName.SubFilter, subFilter);
            pdfSigner.cryptoDictionary = cryptoDictionary;
            IDictionary<PdfName, int?> exc = new Dictionary<PdfName, int?>();
            exc.Put(PdfName.Contents, estimatedSize * 2 + 2);
            pdfSigner.PreClose(exc);
            Stream data = pdfSigner.GetRangeStream();
            byte[] digest = DigestAlgorithms.Digest(data, messageDigest);
            byte[] paddedSig = new byte[estimatedSize];
            PdfDictionary dic2 = new PdfDictionary();
            dic2.Put(PdfName.Contents, new PdfString(paddedSig).SetHexWriting(true));
            pdfSigner.Close(dic2);
            pdfSigner.closed = true;
            closed = true;
            return digest;
        }
    }
}