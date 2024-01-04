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
using System.Collections.Generic;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Exceptions;

namespace iText.Signatures {
    /// <summary>This class performs signing with PaDES related profiles using provided parameters.</summary>
    public class PdfPadesSigner {
        private const String TEMP_FILE_NAME = "tempPdfFile";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private const String DEFAULT_DIGEST_ALGORITHM = DigestAlgorithms.SHA512;

        private static readonly Object LOCK_OBJECT = new Object();

        private static long increment = 0;

        private IOcspClient ocspClient = null;

        private ICrlClient crlClient;

        private IIssuingCertificateRetriever issuingCertificateRetriever = new IssuingCertificateRetriever();

        private int estimatedSize = 0;

        private String timestampSignatureName;

        private String temporaryDirectoryPath = null;

        private StampingProperties stampingProperties = new StampingProperties().UseAppendMode();

        private MemoryStream tempOutputStream;

        private FileInfo tempFile;

        private readonly ICollection<FileInfo> tempFiles = new HashSet<FileInfo>();

        private readonly PdfReader reader;

        private readonly Stream outputStream;

        /// <summary>Create an instance of PdfPadesSigner class.</summary>
        /// <remarks>Create an instance of PdfPadesSigner class. One instance shall be used for one signing operation.
        ///     </remarks>
        /// <param name="reader">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// instance to read original PDF file
        /// </param>
        /// <param name="outputStream">
        /// 
        /// <see cref="System.IO.Stream"/>
        /// output stream to write the resulting PDF file into
        /// </param>
        public PdfPadesSigner(PdfReader reader, Stream outputStream) {
            this.reader = reader;
            this.outputStream = outputStream;
        }

        /// <summary>
        /// Sign the document provided in
        /// <see cref="PdfSigner"/>
        /// instance with PaDES Baseline-B Profile.
        /// </summary>
        /// <param name="signerProperties">
        /// 
        /// <see cref="SignerProperties"/>
        /// properties to be used for main signing operation
        /// </param>
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        /// <param name="externalSignature">
        /// 
        /// <see cref="IExternalSignature"/>
        /// instance to be used for main signing operation
        /// </param>
        public virtual void SignWithBaselineBProfile(SignerProperties signerProperties, IX509Certificate[] chain, 
            IExternalSignature externalSignature) {
            PerformSignDetached(signerProperties, true, externalSignature, chain, null);
        }

        /// <summary>
        /// Sign the document provided in
        /// <see cref="PdfSigner"/>
        /// instance with PaDES Baseline-B Profile.
        /// </summary>
        /// <param name="signerProperties">
        /// 
        /// <see cref="SignerProperties"/>
        /// properties to be used for main signing operation
        /// </param>
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        /// <param name="privateKey">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPrivateKey"/>
        /// instance to be used for main signing operation
        /// </param>
        public virtual void SignWithBaselineBProfile(SignerProperties signerProperties, IX509Certificate[] chain, 
            IPrivateKey privateKey) {
            IExternalSignature externalSignature = new PrivateKeySignature(privateKey, GetDigestAlgorithm(privateKey));
            SignWithBaselineBProfile(signerProperties, chain, externalSignature);
        }

        /// <summary>
        /// Sign the document provided in
        /// <see cref="PdfSigner"/>
        /// instance with PaDES Baseline-T Profile.
        /// </summary>
        /// <param name="signerProperties">
        /// 
        /// <see cref="SignerProperties"/>
        /// properties to be used for main signing operation
        /// </param>
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        /// <param name="externalSignature">
        /// 
        /// <see cref="IExternalSignature"/>
        /// instance to be used for main signing operation
        /// </param>
        /// <param name="tsaClient">
        /// 
        /// <see cref="ITSAClient"/>
        /// instance to be used for timestamp creation
        /// </param>
        public virtual void SignWithBaselineTProfile(SignerProperties signerProperties, IX509Certificate[] chain, 
            IExternalSignature externalSignature, ITSAClient tsaClient) {
            PerformSignDetached(signerProperties, true, externalSignature, chain, tsaClient);
        }

        /// <summary>
        /// Sign the document provided in
        /// <see cref="PdfSigner"/>
        /// instance with PaDES Baseline-T Profile.
        /// </summary>
        /// <param name="signerProperties">
        /// 
        /// <see cref="SignerProperties"/>
        /// properties to be used for main signing operation
        /// </param>
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        /// <param name="privateKey">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPrivateKey"/>
        /// instance to be used for main signing operation
        /// </param>
        /// <param name="tsaClient">
        /// 
        /// <see cref="ITSAClient"/>
        /// instance to be used for timestamp creation
        /// </param>
        public virtual void SignWithBaselineTProfile(SignerProperties signerProperties, IX509Certificate[] chain, 
            IPrivateKey privateKey, ITSAClient tsaClient) {
            IExternalSignature externalSignature = new PrivateKeySignature(privateKey, GetDigestAlgorithm(privateKey));
            SignWithBaselineTProfile(signerProperties, chain, externalSignature, tsaClient);
        }

        /// <summary>
        /// Sign the document provided in
        /// <see cref="PdfSigner"/>
        /// instance with PaDES Baseline-LT Profile.
        /// </summary>
        /// <param name="signerProperties">
        /// 
        /// <see cref="SignerProperties"/>
        /// properties to be used for main signing operation
        /// </param>
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        /// <param name="externalSignature">
        /// 
        /// <see cref="IExternalSignature"/>
        /// instance to be used for main signing operation
        /// </param>
        /// <param name="tsaClient">
        /// 
        /// <see cref="ITSAClient"/>
        /// instance to be used for timestamp creation
        /// </param>
        public virtual void SignWithBaselineLTProfile(SignerProperties signerProperties, IX509Certificate[] chain, 
            IExternalSignature externalSignature, ITSAClient tsaClient) {
            CreateRevocationClients(chain[0], true);
            try {
                PerformSignDetached(signerProperties, false, externalSignature, chain, tsaClient);
                using (Stream inputStream = CreateInputStream()) {
                    using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputStream), new PdfWriter(outputStream), 
                        new StampingProperties().UseAppendMode())) {
                        PerformLtvVerification(pdfDocument, JavaCollectionsUtil.SingletonList(signerProperties.GetFieldName()), LtvVerification.RevocationDataNecessity
                            .REQUIRED_FOR_SIGNING_CERTIFICATE);
                    }
                }
            }
            finally {
                DeleteTempFiles();
            }
        }

        /// <summary>
        /// Sign the document provided in
        /// <see cref="PdfSigner"/>
        /// instance with PaDES Baseline-LT Profile.
        /// </summary>
        /// <param name="signerProperties">
        /// 
        /// <see cref="SignerProperties"/>
        /// properties to be used for main signing operation
        /// </param>
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        /// <param name="privateKey">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPrivateKey"/>
        /// instance to be used for main signing operation
        /// </param>
        /// <param name="tsaClient">
        /// 
        /// <see cref="ITSAClient"/>
        /// instance to be used for timestamp creation
        /// </param>
        public virtual void SignWithBaselineLTProfile(SignerProperties signerProperties, IX509Certificate[] chain, 
            IPrivateKey privateKey, ITSAClient tsaClient) {
            IExternalSignature externalSignature = new PrivateKeySignature(privateKey, GetDigestAlgorithm(privateKey));
            SignWithBaselineLTProfile(signerProperties, chain, externalSignature, tsaClient);
        }

        /// <summary>
        /// Sign the document provided in
        /// <see cref="PdfSigner"/>
        /// instance with PaDES Baseline-LTA Profile.
        /// </summary>
        /// <param name="signerProperties">
        /// 
        /// <see cref="SignerProperties"/>
        /// properties to be used for main signing operation
        /// </param>
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        /// <param name="externalSignature">
        /// 
        /// <see cref="IExternalSignature"/>
        /// instance to be used for main signing operation
        /// </param>
        /// <param name="tsaClient">
        /// 
        /// <see cref="ITSAClient"/>
        /// instance to be used for timestamp creation
        /// </param>
        public virtual void SignWithBaselineLTAProfile(SignerProperties signerProperties, IX509Certificate[] chain
            , IExternalSignature externalSignature, ITSAClient tsaClient) {
            CreateRevocationClients(chain[0], true);
            try {
                PerformSignDetached(signerProperties, false, externalSignature, chain, tsaClient);
                using (Stream inputStream = CreateInputStream()) {
                    using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputStream), new PdfWriter(CreateOutputStream
                        ()), new StampingProperties().UseAppendMode())) {
                        PerformLtvVerification(pdfDocument, JavaCollectionsUtil.SingletonList(signerProperties.GetFieldName()), LtvVerification.RevocationDataNecessity
                            .REQUIRED_FOR_SIGNING_CERTIFICATE);
                        PerformTimestamping(pdfDocument, outputStream, tsaClient);
                    }
                }
            }
            finally {
                DeleteTempFiles();
            }
        }

        /// <summary>
        /// Sign the document provided in
        /// <see cref="PdfSigner"/>
        /// instance with PaDES Baseline-LTA Profile.
        /// </summary>
        /// <param name="signerProperties">
        /// 
        /// <see cref="SignerProperties"/>
        /// properties to be used for main signing operation
        /// </param>
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        /// <param name="privateKey">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPrivateKey"/>
        /// instance to be used for main signing operation
        /// </param>
        /// <param name="tsaClient">
        /// 
        /// <see cref="ITSAClient"/>
        /// instance to be used for timestamp creation
        /// </param>
        public virtual void SignWithBaselineLTAProfile(SignerProperties signerProperties, IX509Certificate[] chain
            , IPrivateKey privateKey, ITSAClient tsaClient) {
            IExternalSignature externalSignature = new PrivateKeySignature(privateKey, GetDigestAlgorithm(privateKey));
            SignWithBaselineLTAProfile(signerProperties, chain, externalSignature, tsaClient);
        }

        /// <summary>Add revocation information for all the signatures which could be found in the provided document.</summary>
        /// <remarks>
        /// Add revocation information for all the signatures which could be found in the provided document.
        /// Also add timestamp signature on top of that.
        /// </remarks>
        /// <param name="tsaClient">
        /// 
        /// <see cref="ITSAClient"/>
        /// TSA Client to be used for timestamp signature creation
        /// </param>
        public virtual void ProlongSignatures(ITSAClient tsaClient) {
            Stream documentOutputStream = tsaClient == null ? outputStream : CreateOutputStream();
            using (PdfDocument pdfDocument = new PdfDocument(reader, new PdfWriter(documentOutputStream), new StampingProperties
                ().UseAppendMode())) {
                SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
                IList<String> signatureNames = signatureUtil.GetSignatureNames();
                if (signatureNames.IsEmpty()) {
                    throw new PdfException(SignExceptionMessageConstant.NO_SIGNATURES_TO_PROLONG);
                }
                CreateRevocationClients(null, false);
                PerformLtvVerification(pdfDocument, signatureNames, LtvVerification.RevocationDataNecessity.OPTIONAL);
                if (tsaClient != null) {
                    PerformTimestamping(pdfDocument, outputStream, tsaClient);
                }
            }
        }

        /// <summary>Add revocation information for all the signatures which could be found in the provided document.</summary>
        public virtual void ProlongSignatures() {
            ProlongSignatures(null);
        }

        /// <summary>Set temporary directory to be used for temporary files creation.</summary>
        /// <remarks>
        /// Set temporary directory to be used for temporary files creation.
        /// <para />
        /// If none is set, temporary documents will be created in memory.
        /// </remarks>
        /// <param name="temporaryDirectoryPath">
        /// 
        /// <see cref="System.String"/>
        /// representing relative or absolute path to the directory
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="PdfPadesSigner"/>
        /// </returns>
        public virtual iText.Signatures.PdfPadesSigner SetTemporaryDirectoryPath(String temporaryDirectoryPath) {
            this.temporaryDirectoryPath = temporaryDirectoryPath;
            return this;
        }

        /// <summary>Set the name to be used for timestamp signature creation.</summary>
        /// <remarks>
        /// Set the name to be used for timestamp signature creation.
        /// <para />
        /// This setter is only relevant if
        /// <see cref="SignWithBaselineLTAProfile(SignerProperties, iText.Commons.Bouncycastle.Cert.IX509Certificate[], IExternalSignature, ITSAClient)
        ///     "/>
        /// or
        /// <see cref="ProlongSignatures()"/>
        /// methods are used.
        /// <para />
        /// If none is set, randomly generated signature name will be used.
        /// </remarks>
        /// <param name="timestampSignatureName">
        /// 
        /// <see cref="System.String"/>
        /// representing the name of a timestamp signature to be applied
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="PdfPadesSigner"/>
        /// </returns>
        public virtual iText.Signatures.PdfPadesSigner SetTimestampSignatureName(String timestampSignatureName) {
            this.timestampSignatureName = timestampSignatureName;
            return this;
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
        /// <see cref="PdfPadesSigner"/>
        /// </returns>
        public virtual iText.Signatures.PdfPadesSigner SetStampingProperties(StampingProperties stampingProperties
            ) {
            this.stampingProperties = stampingProperties;
            return this;
        }

        /// <summary>Set estimated size of a signature to be applied.</summary>
        /// <remarks>
        /// Set estimated size of a signature to be applied.
        /// <para />
        /// This parameter represents estimated amount of bytes to be preserved for the signature.
        /// <para />
        /// If none is set, 0 will be used and the required space will be calculated during the signing.
        /// </remarks>
        /// <param name="estimatedSize">amount of bytes to be used as estimated value</param>
        /// <returns>
        /// same instance of
        /// <see cref="PdfPadesSigner"/>
        /// </returns>
        public virtual iText.Signatures.PdfPadesSigner SetEstimatedSize(int estimatedSize) {
            this.estimatedSize = estimatedSize;
            return this;
        }

        /// <summary>
        /// Set
        /// <see cref="IOcspClient"/>
        /// to be used for LTV Verification.
        /// </summary>
        /// <remarks>
        /// Set
        /// <see cref="IOcspClient"/>
        /// to be used for LTV Verification.
        /// <para />
        /// This setter is only relevant if Baseline-LT Profile level or higher is used.
        /// <para />
        /// If none is set, there will be an attempt to create default OCSP Client instance using the certificate chain.
        /// </remarks>
        /// <param name="ocspClient">
        /// 
        /// <see cref="IOcspClient"/>
        /// instance to be used for LTV Verification
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="PdfPadesSigner"/>
        /// </returns>
        public virtual iText.Signatures.PdfPadesSigner SetOcspClient(IOcspClient ocspClient) {
            this.ocspClient = ocspClient;
            return this;
        }

        /// <summary>
        /// Set
        /// <see cref="ICrlClient"/>
        /// to be used for LTV Verification.
        /// </summary>
        /// <remarks>
        /// Set
        /// <see cref="ICrlClient"/>
        /// to be used for LTV Verification.
        /// <para />
        /// This setter is only relevant if Baseline-LT Profile level or higher is used.
        /// <para />
        /// If none is set, there will be an attempt to create default CRL Client instance using the certificate chain.
        /// </remarks>
        /// <param name="crlClient">
        /// 
        /// <see cref="ICrlClient"/>
        /// instance to be used for LTV Verification
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="PdfPadesSigner"/>
        /// </returns>
        public virtual iText.Signatures.PdfPadesSigner SetCrlClient(ICrlClient crlClient) {
            this.crlClient = crlClient;
            return this;
        }

        /// <summary>
        /// Set
        /// <see cref="IIssuingCertificateRetriever"/>
        /// to be used before main signing operation.
        /// </summary>
        /// <remarks>
        /// Set
        /// <see cref="IIssuingCertificateRetriever"/>
        /// to be used before main signing operation.
        /// <para />
        /// If none is set,
        /// <see cref="IssuingCertificateRetriever"/>
        /// instance will be used instead.
        /// </remarks>
        /// <param name="issuingCertificateRetriever">
        /// 
        /// <see cref="IIssuingCertificateRetriever"/>
        /// instance to be used for getting missing
        /// certificates in chain or CRL response issuer certificates.
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="PdfPadesSigner"/>.
        /// </returns>
        public virtual iText.Signatures.PdfPadesSigner SetIssuingCertificateRetriever(IIssuingCertificateRetriever
             issuingCertificateRetriever) {
            this.issuingCertificateRetriever = issuingCertificateRetriever;
            return this;
        }

        /// <summary>
        /// Set certificate list to be used by the
        /// <see cref="IIssuingCertificateRetriever"/>
        /// to retrieve missing certificates.
        /// </summary>
        /// <param name="certificateList">
        /// certificate list for getting missing certificates in chain
        /// or CRL response issuer certificates.
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="PdfPadesSigner"/>.
        /// </returns>
        public virtual iText.Signatures.PdfPadesSigner SetTrustedCertificates(IList<IX509Certificate> certificateList
            ) {
            this.issuingCertificateRetriever.SetTrustedCertificates(certificateList);
            return this;
        }

        internal virtual void PerformTimestamping(PdfDocument document, Stream outputStream, ITSAClient tsaClient) {
            PdfSigner timestampSigner = new PdfSigner(document, outputStream, tempOutputStream, tempFile);
            timestampSigner.Timestamp(tsaClient, timestampSignatureName);
        }

        internal virtual PdfSigner CreatePdfSigner(SignerProperties signerProperties, bool isFinal) {
            String tempFilePath = null;
            if (temporaryDirectoryPath != null) {
                tempFilePath = GetNextTempFile().FullName;
            }
            PdfSigner signer = new PdfSigner(reader, isFinal ? outputStream : CreateOutputStream(), tempFilePath, stampingProperties
                );
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

        internal virtual void PerformLtvVerification(PdfDocument pdfDocument, IList<String> signatureNames, LtvVerification.RevocationDataNecessity
             revocationDataNecessity) {
            LtvVerification ltvVerification = new LtvVerification(pdfDocument).SetRevocationDataNecessity(revocationDataNecessity
                ).SetIssuingCertificateRetriever(issuingCertificateRetriever);
            foreach (String signatureName in signatureNames) {
                ltvVerification.AddVerification(signatureName, ocspClient, crlClient, LtvVerification.CertificateOption.ALL_CERTIFICATES
                    , LtvVerification.Level.OCSP_OPTIONAL_CRL, LtvVerification.CertificateInclusion.YES);
            }
            ltvVerification.Merge();
        }

        internal virtual void DeleteTempFiles() {
            foreach (FileInfo tempFile in tempFiles) {
                tempFile.Delete();
            }
        }

        internal virtual Stream CreateOutputStream() {
            if (temporaryDirectoryPath != null) {
                return FileUtil.GetFileOutputStream(GetNextTempFile());
            }
            tempOutputStream = new MemoryStream();
            return tempOutputStream;
        }

        internal virtual Stream CreateInputStream() {
            if (temporaryDirectoryPath != null) {
                return FileUtil.GetInputStreamForFile(tempFile);
            }
            return new MemoryStream(tempOutputStream.ToArray());
        }

        internal virtual void CreateRevocationClients(IX509Certificate signingCert, bool clientsRequired) {
            if (crlClient == null && ocspClient == null && clientsRequired) {
                IX509Certificate signingCertificate = (IX509Certificate)signingCert;
                if (CertificateUtil.GetOCSPURL(signingCertificate) == null && CertificateUtil.GetCRLURL(signingCertificate
                    ) == null) {
                    throw new PdfException(SignExceptionMessageConstant.DEFAULT_CLIENTS_CANNOT_BE_CREATED);
                }
            }
            if (crlClient == null) {
                crlClient = new CrlClientOnline();
            }
            if (ocspClient == null) {
                ocspClient = new OcspClientBouncyCastle(null);
            }
        }

        private void PerformSignDetached(SignerProperties signerProperties, bool isFinal, IExternalSignature externalSignature
            , IX509Certificate[] chain, ITSAClient tsaClient) {
            IX509Certificate[] fullChain = issuingCertificateRetriever.RetrieveMissingCertificates(chain);
            PdfSigner signer = CreatePdfSigner(signerProperties, isFinal);
            try {
                signer.SignDetached(externalSignature, fullChain, null, null, tsaClient, estimatedSize, PdfSigner.CryptoStandard
                    .CADES);
            }
            finally {
                signer.originalOS.Dispose();
            }
        }

        private FileInfo GetNextTempFile() {
            if (!FileUtil.DirectoryExists(temporaryDirectoryPath)) {
                throw new PdfException(MessageFormatUtil.Format(SignExceptionMessageConstant.PATH_IS_NOT_DIRECTORY, temporaryDirectoryPath
                    ));
            }
            lock (LOCK_OBJECT) {
                do {
                    increment++;
                    tempFile = new FileInfo(temporaryDirectoryPath + "/" + TEMP_FILE_NAME + increment + ".pdf");
                }
                while (tempFile.Exists);
                tempFiles.Add(tempFile);
            }
            return tempFile;
        }

        private String GetDigestAlgorithm(IPrivateKey privateKey) {
            String signatureAlgorithm = SignUtils.GetPrivateKeyAlgorithm(privateKey);
            switch (signatureAlgorithm) {
                case "Ed25519": {
                    return DigestAlgorithms.SHA512;
                }

                case "Ed448": {
                    return DigestAlgorithms.SHAKE256;
                }

                default: {
                    return DEFAULT_DIGEST_ALGORITHM;
                }
            }
        }
    }
}
