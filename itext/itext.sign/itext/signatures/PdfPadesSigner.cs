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

        private ITSAClient tsaClient;

        private IOcspClient ocspClient = new OcspClientBouncyCastle(null);

        private ICrlClient crlClient;

        private int estimatedSize = 0;

        private String timestampSignatureName;

        private String temporaryDirectoryPath = null;

        private readonly PdfSigner pdfSigner;

        private readonly IExternalSignature externalSignature;

        private MemoryStream tempOutputStream;

        private FileInfo tempFile;

        private readonly ICollection<FileInfo> tempFiles = new HashSet<FileInfo>();

        /// <summary>
        /// Creates PdfPadesSigner instance using provided
        /// <see cref="PdfSigner"/>
        /// and
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPrivateKey"/>
        /// parameters.
        /// </summary>
        /// <remarks>
        /// Creates PdfPadesSigner instance using provided
        /// <see cref="PdfSigner"/>
        /// and
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPrivateKey"/>
        /// parameters.
        /// <para />
        /// <see cref="PdfSigner"/>
        /// instance shall be newly created and not closed.
        /// <para />
        /// Same instance of
        /// <see cref="PdfPadesSigner"/>
        /// shall not be used for more than one signing operation.
        /// </remarks>
        /// <param name="pdfSigner">
        /// 
        /// <see cref="PdfSigner"/>
        /// to be used for main signing operation
        /// </param>
        /// <param name="privateKey">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPrivateKey"/>
        /// private key to be used for main signing operation
        /// </param>
        public PdfPadesSigner(PdfSigner pdfSigner, IPrivateKey privateKey) {
            this.pdfSigner = pdfSigner;
            this.externalSignature = new PrivateKeySignature(privateKey, DEFAULT_DIGEST_ALGORITHM);
        }

        /// <summary>
        /// Creates PdfPadesSigner instance using provided
        /// <see cref="PdfSigner"/>
        /// and
        /// <see cref="IExternalSignature"/>
        /// parameters.
        /// </summary>
        /// <remarks>
        /// Creates PdfPadesSigner instance using provided
        /// <see cref="PdfSigner"/>
        /// and
        /// <see cref="IExternalSignature"/>
        /// parameters.
        /// <para />
        /// <see cref="PdfSigner"/>
        /// instance shall be newly created and not closed.
        /// <para />
        /// Same instance of
        /// <see cref="PdfPadesSigner"/>
        /// shall not be used for more than one signing operation.
        /// </remarks>
        /// <param name="pdfSigner">
        /// 
        /// <see cref="PdfSigner"/>
        /// to be used for main signing operation
        /// </param>
        /// <param name="externalSignature">
        /// 
        /// <see cref="IExternalSignature"/>
        /// external signature to be used for main signing operation
        /// </param>
        public PdfPadesSigner(PdfSigner pdfSigner, IExternalSignature externalSignature) {
            this.pdfSigner = pdfSigner;
            this.externalSignature = externalSignature;
        }

        /// <summary>
        /// Sign the document provided in
        /// <see cref="PdfSigner"/>
        /// instance with PaDES Baseline-B Profile.
        /// </summary>
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        public virtual void SignWithBaselineBProfile(IX509Certificate[] chain) {
            PerformSignDetached(chain, null);
        }

        /// <summary>
        /// Sign the document provided in
        /// <see cref="PdfSigner"/>
        /// instance with PaDES Baseline-T Profile.
        /// </summary>
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        public virtual void SignWithBaselineTProfile(IX509Certificate[] chain) {
            CreateTsaClient(chain);
            PerformSignDetached(chain, tsaClient);
        }

        /// <summary>
        /// Sign the document provided in
        /// <see cref="PdfSigner"/>
        /// instance with PaDES Baseline-LT Profile.
        /// </summary>
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        public virtual void SignWithBaselineLTProfile(IX509Certificate[] chain) {
            CreateTsaClient(chain);
            CreateCrlClient(chain);
            try {
                Stream originalOS = SubstituteOutputStream();
                PerformSignDetached(chain, tsaClient);
                PerformLtvVerification(CreateInputStream(), originalOS);
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
        /// <param name="chain">the chain of certificates to be used for signing operation</param>
        public virtual void SignWithBaselineLTAProfile(IX509Certificate[] chain) {
            CreateTsaClient(chain);
            CreateCrlClient(chain);
            try {
                Stream originalOS = SubstituteOutputStream();
                PerformSignDetached(chain, tsaClient);
                PerformLtvVerification(CreateInputStream(), CreateOutputStream());
                PerformTimestamping(originalOS);
            }
            finally {
                DeleteTempFiles();
            }
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
        /// <see cref="SignWithBaselineLTAProfile(iText.Commons.Bouncycastle.Cert.IX509Certificate[])"/>
        /// method is used.
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
        /// <see cref="ITSAClient"/>
        /// to be used for both timestamp signature and usual signature.
        /// </summary>
        /// <remarks>
        /// Set
        /// <see cref="ITSAClient"/>
        /// to be used for both timestamp signature and usual signature.
        /// <para />
        /// This setter is only relevant if Baseline-T Profile level or higher is used.
        /// <para />
        /// If none is set, there will be an attempt to create default TSA Client instance using the certificate chain.
        /// </remarks>
        /// <param name="tsaClient">
        /// 
        /// <see cref="ITSAClient"/>
        /// instance to be used for timestamping
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="PdfPadesSigner"/>
        /// </returns>
        public virtual iText.Signatures.PdfPadesSigner SetTsaClient(ITSAClient tsaClient) {
            this.tsaClient = tsaClient;
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

        private void PerformTimestamping(Stream outputStream) {
            using (Stream tempInputStream = CreateInputStream()) {
                PdfSigner timestampSigner = new PdfSigner(new PdfReader(tempInputStream), outputStream, new StampingProperties
                    ().UseAppendMode());
                timestampSigner.Timestamp(tsaClient, timestampSignatureName);
            }
        }

        private void PerformSignDetached(IX509Certificate[] chain, ITSAClient tsaClient) {
            try {
                pdfSigner.SignDetached(externalSignature, chain, null, null, tsaClient, estimatedSize, PdfSigner.CryptoStandard
                    .CADES);
            }
            finally {
                pdfSigner.originalOS.Dispose();
            }
        }

        private void PerformLtvVerification(Stream inputStream, Stream outputStream) {
            PdfReader tempReader = new PdfReader(inputStream);
            try {
                using (PdfDocument tempDocument = new PdfDocument(tempReader, new PdfWriter(outputStream), new StampingProperties
                    ().UseAppendMode())) {
                    LtvVerification ltvVerification = new LtvVerification(tempDocument);
                    ltvVerification.AddVerification(pdfSigner.fieldName, ocspClient, crlClient, LtvVerification.CertificateOption
                        .SIGNING_CERTIFICATE, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.YES);
                    ltvVerification.Merge();
                }
            }
            finally {
                inputStream.Dispose();
            }
        }

        private Stream SubstituteOutputStream() {
            Stream originalOS = pdfSigner.originalOS;
            pdfSigner.originalOS = CreateOutputStream();
            return originalOS;
        }

        private void DeleteTempFiles() {
            foreach (FileInfo tempFile in tempFiles) {
                tempFile.Delete();
            }
        }

        private Stream CreateOutputStream() {
            if (temporaryDirectoryPath != null) {
                return FileUtil.GetFileOutputStream(GetNextTempFile());
            }
            tempOutputStream = new MemoryStream();
            return tempOutputStream;
        }

        private Stream CreateInputStream() {
            if (temporaryDirectoryPath != null) {
                return FileUtil.GetInputStreamForFile(tempFile);
            }
            return new MemoryStream(tempOutputStream.ToArray());
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

        private void CreateTsaClient(IX509Certificate[] chain) {
            if (tsaClient == null) {
                tsaClient = GetTsaClientFromChain(chain);
            }
            if (tsaClient == null) {
                throw new PdfException(MessageFormatUtil.Format(SignExceptionMessageConstant.DOCUMENT_CANNOT_BE_SIGNED, "TSA Client"
                    ));
            }
        }

        private void CreateCrlClient(IX509Certificate[] chain) {
            if (crlClient == null) {
                crlClient = new CrlClientOnline(chain);
                if (((CrlClientOnline)crlClient).urls.IsEmpty()) {
                    throw new PdfException(MessageFormatUtil.Format(SignExceptionMessageConstant.DOCUMENT_CANNOT_BE_SIGNED, "CRL Client"
                        ));
                }
            }
        }

        private static ITSAClient GetTsaClientFromChain(IX509Certificate[] chain) {
            foreach (IX509Certificate certificate in chain) {
                if (certificate is IX509Certificate) {
                    IX509Certificate x509Certificate = (IX509Certificate)certificate;
                    String tsaUrl = CertificateUtil.GetTSAURL(x509Certificate);
                    if (tsaUrl != null) {
                        return new TSAClientBouncyCastle(tsaUrl);
                    }
                }
            }
            return null;
        }
    }
}
