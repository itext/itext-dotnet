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
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Cms;
using iText.Signatures.Exceptions;

namespace iText.Signatures {
    /// <summary>Helper class to perform signing operation in two steps.</summary>
    /// <remarks>
    /// Helper class to perform signing operation in two steps.
    /// <para />
    /// Firstly
    /// <see cref="CreateCMSContainerWithoutSignature(iText.Commons.Bouncycastle.Cert.IX509Certificate[], System.String, iText.Kernel.Pdf.PdfReader, System.IO.Stream, SignerProperties)
    ///     "/>
    /// prepares document and placeholder
    /// for future signature without actual signing process.
    /// <para />
    /// Secondly follow-up step signs prepared document with corresponding PAdES Baseline profile.
    /// </remarks>
    public class PadesTwoPhaseSigningHelper {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private IOcspClient ocspClient;

        private ICrlClient crlClient;

        private ITSAClient tsaClient;

        private String temporaryDirectoryPath;

        private String timestampSignatureName;

        private StampingProperties stampingProperties = new StampingProperties().UseAppendMode();

        private StampingProperties stampingPropertiesWithMetaInfo = (StampingProperties)new StampingProperties().UseAppendMode
            ().SetEventCountingMetaInfo(new SignMetaInfo());

        private IIssuingCertificateRetriever issuingCertificateRetriever = new IssuingCertificateRetriever();

        private int estimatedSize = -1;

        /// <summary>
        /// Create instance of
        /// <see cref="PadesTwoPhaseSigningHelper"/>.
        /// </summary>
        /// <remarks>
        /// Create instance of
        /// <see cref="PadesTwoPhaseSigningHelper"/>.
        /// <para />
        /// Same instance shall not be used for different signing operations, but can be used for both
        /// <see cref="CreateCMSContainerWithoutSignature(iText.Commons.Bouncycastle.Cert.IX509Certificate[], System.String, iText.Kernel.Pdf.PdfReader, System.IO.Stream, SignerProperties)
        ///     "/>
        /// and follow-up signing.
        /// </remarks>
        public PadesTwoPhaseSigningHelper() {
        }

        // Empty constructor.
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
        /// <see cref="PadesTwoPhaseSigningHelper"/>
        /// </returns>
        public virtual iText.Signatures.PadesTwoPhaseSigningHelper SetOcspClient(IOcspClient ocspClient) {
            this.ocspClient = ocspClient;
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
        /// <see cref="PadesTwoPhaseSigningHelper"/>.
        /// </returns>
        public virtual iText.Signatures.PadesTwoPhaseSigningHelper SetTrustedCertificates(IList<IX509Certificate> 
            certificateList) {
            this.issuingCertificateRetriever.SetTrustedCertificates(certificateList);
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
        /// <see cref="PadesTwoPhaseSigningHelper"/>
        /// </returns>
        public virtual iText.Signatures.PadesTwoPhaseSigningHelper SetCrlClient(ICrlClient crlClient) {
            this.crlClient = crlClient;
            return this;
        }

        /// <summary>
        /// Set
        /// <see cref="ITSAClient"/>
        /// to be used for timestamp signature creation.
        /// </summary>
        /// <remarks>
        /// Set
        /// <see cref="ITSAClient"/>
        /// to be used for timestamp signature creation.
        /// <para />
        /// This client has to be set for Baseline-T Profile level and higher.
        /// </remarks>
        /// <param name="tsaClient">
        /// 
        /// <see cref="ITSAClient"/>
        /// instance to be used for timestamp signature creation.
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="PadesTwoPhaseSigningHelper"/>
        /// </returns>
        public virtual iText.Signatures.PadesTwoPhaseSigningHelper SetTSAClient(ITSAClient tsaClient) {
            this.tsaClient = tsaClient;
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
        /// <see cref="PadesTwoPhaseSigningHelper"/>.
        /// </returns>
        public virtual iText.Signatures.PadesTwoPhaseSigningHelper SetIssuingCertificateRetriever(IIssuingCertificateRetriever
             issuingCertificateRetriever) {
            this.issuingCertificateRetriever = issuingCertificateRetriever;
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
        /// <see cref="PadesTwoPhaseSigningHelper"/>
        /// </returns>
        public virtual iText.Signatures.PadesTwoPhaseSigningHelper SetEstimatedSize(int estimatedSize) {
            this.estimatedSize = estimatedSize;
            return this;
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
        /// <see cref="PadesTwoPhaseSigningHelper"/>
        /// </returns>
        public virtual iText.Signatures.PadesTwoPhaseSigningHelper SetTemporaryDirectoryPath(String temporaryDirectoryPath
            ) {
            this.temporaryDirectoryPath = temporaryDirectoryPath;
            return this;
        }

        /// <summary>Set the name to be used for timestamp signature creation.</summary>
        /// <remarks>
        /// Set the name to be used for timestamp signature creation.
        /// <para />
        /// This setter is only relevant if
        /// <see cref="PdfPadesSigner.SignWithBaselineLTAProfile(SignerProperties, iText.Commons.Bouncycastle.Cert.IX509Certificate[], IExternalSignature, ITSAClient)
        ///     "/>
        /// or
        /// <see cref="PdfPadesSigner.ProlongSignatures()"/>
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
        /// <see cref="PadesTwoPhaseSigningHelper"/>
        /// </returns>
        public virtual iText.Signatures.PadesTwoPhaseSigningHelper SetTimestampSignatureName(String timestampSignatureName
            ) {
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
        /// <see cref="PadesTwoPhaseSigningHelper"/>
        /// </returns>
        public virtual iText.Signatures.PadesTwoPhaseSigningHelper SetStampingProperties(StampingProperties stampingProperties
            ) {
            this.stampingProperties = stampingProperties;
            if (stampingProperties.IsEventCountingMetaInfoSet()) {
                this.stampingPropertiesWithMetaInfo = stampingProperties;
            }
            return this;
        }

        /// <summary>Creates CMS container compliant with PAdES level.</summary>
        /// <remarks>
        /// Creates CMS container compliant with PAdES level. Prepares document and placeholder for the future signature
        /// without actual signing process.
        /// </remarks>
        /// <param name="certificates">certificates to be added to the CMS container</param>
        /// <param name="digestAlgorithm">the algorithm to generate the digest with</param>
        /// <param name="inputDocument">
        /// reader
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// instance to read original PDF file
        /// </param>
        /// <param name="outputStream">
        /// 
        /// <see cref="System.IO.Stream"/>
        /// output stream to write the resulting PDF file into
        /// </param>
        /// <param name="signerProperties">properties to be used in the signing operations</param>
        /// <returns>prepared CMS container without signature.</returns>
        public virtual CMSContainer CreateCMSContainerWithoutSignature(IX509Certificate[] certificates, String digestAlgorithm
            , PdfReader inputDocument, Stream outputStream, SignerProperties signerProperties) {
            IX509Certificate[] fullChain = issuingCertificateRetriever.RetrieveMissingCertificates(certificates);
            IX509Certificate[] x509FullChain = JavaUtil.ArraysAsList(fullChain).ToArray(new IX509Certificate[0]);
            PdfTwoPhaseSigner pdfTwoPhaseSigner = new PdfTwoPhaseSigner(inputDocument, outputStream);
            pdfTwoPhaseSigner.SetStampingProperties(stampingProperties);
            CMSContainer cms = new CMSContainer();
            SignerInfo signerInfo = new SignerInfo();
            String digestAlgorithmOid = DigestAlgorithms.GetAllowedDigest(digestAlgorithm);
            signerInfo.SetSigningCertificateAndAddToSignedAttributes(x509FullChain[0], digestAlgorithmOid);
            signerInfo.SetDigestAlgorithm(new AlgorithmIdentifier(digestAlgorithmOid));
            cms.AddCertificates(x509FullChain);
            cms.SetSignerInfo(signerInfo);
            IMessageDigest messageDigest = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest
                (DigestAlgorithms.GetDigest(digestAlgorithmOid));
            int realSignatureSize = (messageDigest.GetDigestLength() + (int)cms.GetSizeEstimation()) * 2 + 2;
            if (tsaClient != null) {
                realSignatureSize += tsaClient.GetTokenSizeEstimate();
            }
            int expectedSignatureSize = estimatedSize < 0 ? realSignatureSize : estimatedSize;
            byte[] digestedDocumentBytes = pdfTwoPhaseSigner.PrepareDocumentForSignature(signerProperties, digestAlgorithm
                , PdfName.Adobe_PPKLite, PdfName.ETSI_CAdES_DETACHED, expectedSignatureSize, true);
            signerInfo.SetMessageDigest(digestedDocumentBytes);
            return cms;
        }

        /// <summary>Follow-up step that signs prepared document with PAdES Baseline-B profile.</summary>
        /// <param name="externalSignature">external signature to do the actual signing</param>
        /// <param name="inputDocument">
        /// reader
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// instance to read prepared document
        /// </param>
        /// <param name="outputStream">the output PDF</param>
        /// <param name="signatureFieldName">the field to sign</param>
        /// <param name="cmsContainer">the finalized CMS container (e.g. created in the first step)</param>
        public virtual void SignCMSContainerWithBaselineBProfile(IExternalSignature externalSignature, PdfReader inputDocument
            , Stream outputStream, String signatureFieldName, CMSContainer cmsContainer) {
            SetSignatureAlgorithmAndSignature(externalSignature, cmsContainer);
            try {
                PdfTwoPhaseSigner.AddSignatureToPreparedDocument(inputDocument, signatureFieldName, outputStream, cmsContainer
                    );
            }
            finally {
                outputStream.Dispose();
            }
        }

        /// <summary>Follow-up step that signs prepared document with PAdES Baseline-T profile.</summary>
        /// <param name="externalSignature">external signature to do the actual signing</param>
        /// <param name="inputDocument">
        /// reader
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// instance to read prepared document
        /// </param>
        /// <param name="outputStream">the output PDF</param>
        /// <param name="signatureFieldName">the field to sign</param>
        /// <param name="cmsContainer">the finalized CMS container (e.g. created in the first step)</param>
        public virtual void SignCMSContainerWithBaselineTProfile(IExternalSignature externalSignature, PdfReader inputDocument
            , Stream outputStream, String signatureFieldName, CMSContainer cmsContainer) {
            byte[] signature = SetSignatureAlgorithmAndSignature(externalSignature, cmsContainer);
            if (tsaClient == null) {
                throw new PdfException(SignExceptionMessageConstant.TSA_CLIENT_IS_MISSING);
            }
            byte[] signatureDigest = tsaClient.GetMessageDigest().Digest(signature);
            byte[] timestamp = tsaClient.GetTimeStampToken(signatureDigest);
            using (IAsn1InputStream tempStream = FACTORY.CreateASN1InputStream(new MemoryStream(timestamp))) {
                IAsn1Sequence seq = FACTORY.CreateASN1Sequence(tempStream.ReadObject());
                CmsAttribute timestampAttribute = new CmsAttribute(OID.AA_TIME_STAMP_TOKEN, FACTORY.CreateDERSet(seq));
                cmsContainer.GetSignerInfo().AddUnSignedAttribute(timestampAttribute);
            }
            try {
                PdfTwoPhaseSigner.AddSignatureToPreparedDocument(inputDocument, signatureFieldName, outputStream, cmsContainer
                    );
            }
            finally {
                outputStream.Dispose();
            }
        }

        /// <summary>Follow-up step that signs prepared document with PAdES Baseline-LT profile.</summary>
        /// <param name="externalSignature">external signature to do the actual signing</param>
        /// <param name="inputDocument">
        /// reader
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// instance to read prepared document
        /// </param>
        /// <param name="outputStream">the output PDF</param>
        /// <param name="signatureFieldName">the field to sign</param>
        /// <param name="cmsContainer">the finalized CMS container (e.g. created in the first step)</param>
        public virtual void SignCMSContainerWithBaselineLTProfile(IExternalSignature externalSignature, PdfReader 
            inputDocument, Stream outputStream, String signatureFieldName, CMSContainer cmsContainer) {
            PdfPadesSigner padesSigner = CreatePadesSigner(inputDocument, outputStream);
            padesSigner.CreateRevocationClients(cmsContainer.GetSignerInfo().GetSigningCertificate(), true);
            try {
                using (Stream tempOutput = padesSigner.CreateOutputStream()) {
                    SignCMSContainerWithBaselineTProfile(externalSignature, inputDocument, tempOutput, signatureFieldName, cmsContainer
                        );
                    using (Stream inputStream = padesSigner.CreateInputStream()) {
                        using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputStream), new PdfWriter(outputStream), 
                            stampingPropertiesWithMetaInfo)) {
                            padesSigner.PerformLtvVerification(pdfDocument, JavaCollectionsUtil.SingletonList(signatureFieldName), LtvVerification.RevocationDataNecessity
                                .REQUIRED_FOR_SIGNING_CERTIFICATE);
                        }
                    }
                }
            }
            finally {
                padesSigner.DeleteTempFiles();
            }
        }

        /// <summary>Follow-up step that signs prepared document with PAdES Baseline-LTA profile.</summary>
        /// <param name="externalSignature">external signature to do the actual signing</param>
        /// <param name="inputDocument">
        /// reader
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// instance to read prepared document
        /// </param>
        /// <param name="outputStream">the output PDF</param>
        /// <param name="signatureFieldName">the field to sign</param>
        /// <param name="cmsContainer">the finalized CMS container (e.g. created in the first step)</param>
        public virtual void SignCMSContainerWithBaselineLTAProfile(IExternalSignature externalSignature, PdfReader
             inputDocument, Stream outputStream, String signatureFieldName, CMSContainer cmsContainer) {
            PdfPadesSigner padesSigner = CreatePadesSigner(inputDocument, outputStream);
            padesSigner.CreateRevocationClients(cmsContainer.GetSignerInfo().GetSigningCertificate(), true);
            try {
                using (Stream tempOutput = padesSigner.CreateOutputStream()) {
                    SignCMSContainerWithBaselineTProfile(externalSignature, inputDocument, tempOutput, signatureFieldName, cmsContainer
                        );
                    using (Stream inputStream = padesSigner.CreateInputStream()) {
                        using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputStream), new PdfWriter(padesSigner.CreateOutputStream
                            ()), stampingPropertiesWithMetaInfo)) {
                            padesSigner.PerformLtvVerification(pdfDocument, JavaCollectionsUtil.SingletonList(signatureFieldName), LtvVerification.RevocationDataNecessity
                                .REQUIRED_FOR_SIGNING_CERTIFICATE);
                            padesSigner.PerformTimestamping(pdfDocument, outputStream, tsaClient);
                        }
                    }
                }
            }
            finally {
                padesSigner.DeleteTempFiles();
            }
        }

        private byte[] SetSignatureAlgorithmAndSignature(IExternalSignature externalSignature, CMSContainer cmsContainer
            ) {
            String signatureDigest = externalSignature.GetDigestAlgorithmName();
            String containerDigest = cmsContainer.GetDigestAlgorithm().GetAlgorithmOid();
            String providedSignatureAlgorithm = externalSignature.GetSignatureAlgorithmName();
            if (!DigestAlgorithms.GetAllowedDigest(signatureDigest).Equals(containerDigest)) {
                throw new PdfException(MessageFormatUtil.Format(SignExceptionMessageConstant.DIGEST_ALGORITHMS_ARE_NOT_SAME
                    , DigestAlgorithms.GetDigest(containerDigest), signatureDigest));
            }
            ISignatureMechanismParams signatureMechanismParams = externalSignature.GetSignatureMechanismParameters();
            if (signatureMechanismParams == null) {
                cmsContainer.GetSignerInfo().SetSignatureAlgorithm(new AlgorithmIdentifier(SignatureMechanisms.GetSignatureMechanismOid
                    (providedSignatureAlgorithm, signatureDigest)));
            }
            else {
                cmsContainer.GetSignerInfo().SetSignatureAlgorithm(new AlgorithmIdentifier(SignatureMechanisms.GetSignatureMechanismOid
                    (providedSignatureAlgorithm, signatureDigest), signatureMechanismParams.ToEncodable().ToASN1Primitive(
                    )));
            }
            byte[] signedAttributes = cmsContainer.GetSerializedSignedAttributes();
            byte[] signature = externalSignature.Sign(signedAttributes);
            cmsContainer.GetSignerInfo().SetSignature(signature);
            return signature;
        }

        private PdfPadesSigner CreatePadesSigner(PdfReader inputDocument, Stream outputStream) {
            PdfPadesSigner padesSigner = new PdfPadesSigner(inputDocument, outputStream);
            padesSigner.SetOcspClient(ocspClient);
            padesSigner.SetCrlClient(crlClient);
            padesSigner.SetStampingProperties(stampingProperties);
            padesSigner.SetTemporaryDirectoryPath(temporaryDirectoryPath);
            padesSigner.SetTimestampSignatureName(timestampSignatureName);
            padesSigner.SetIssuingCertificateRetriever(issuingCertificateRetriever);
            padesSigner.SetEstimatedSize(estimatedSize);
            return padesSigner;
        }
    }
}
