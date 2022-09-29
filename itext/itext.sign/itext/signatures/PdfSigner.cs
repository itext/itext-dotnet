/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Pdfa;
using iText.Signatures.Exceptions;

namespace iText.Signatures {
    /// <summary>Takes care of the cryptographic options and appearances that form a signature.</summary>
    public class PdfSigner {
        /// <summary>Enum containing the Cryptographic Standards.</summary>
        /// <remarks>Enum containing the Cryptographic Standards. Possible values are "CMS" and "CADES".</remarks>
        public enum CryptoStandard {
            /// <summary>Cryptographic Message Syntax.</summary>
            CMS,
            /// <summary>CMS Advanced Electronic Signatures.</summary>
            CADES
        }

        /// <summary>Approval signature.</summary>
        public const int NOT_CERTIFIED = 0;

        /// <summary>Author signature, no changes allowed.</summary>
        public const int CERTIFIED_NO_CHANGES_ALLOWED = 1;

        /// <summary>Author signature, form filling allowed.</summary>
        public const int CERTIFIED_FORM_FILLING = 2;

        /// <summary>Author signature, form filling and annotations allowed.</summary>
        public const int CERTIFIED_FORM_FILLING_AND_ANNOTATIONS = 3;

        /// <summary>The certification level.</summary>
        protected internal int certificationLevel = NOT_CERTIFIED;

        /// <summary>The name of the field.</summary>
        protected internal String fieldName;

        /// <summary>The file right before the signature is added (can be null).</summary>
        protected internal FileStream raf;

        /// <summary>The bytes of the file right before the signature is added (if raf is null).</summary>
        protected internal byte[] bout;

        /// <summary>Array containing the byte positions of the bytes that need to be hashed.</summary>
        protected internal long[] range;

        /// <summary>The PdfDocument.</summary>
        protected internal PdfDocument document;

        /// <summary>The crypto dictionary.</summary>
        protected internal PdfSignature cryptoDictionary;

        /// <summary>Holds value of property signatureEvent.</summary>
        protected internal PdfSigner.ISignatureEvent signatureEvent;

        /// <summary>OutputStream for the bytes of the document.</summary>
        protected internal Stream originalOS;

        /// <summary>Outputstream that temporarily holds the output in memory.</summary>
        protected internal MemoryStream temporaryOS;

        /// <summary>Tempfile to hold the output temporarily.</summary>
        protected internal FileInfo tempFile;

        /// <summary>Name and content of keys that can only be added in the close() method.</summary>
        protected internal IDictionary<PdfName, PdfLiteral> exclusionLocations;

        /// <summary>Indicates if the pdf document has already been pre-closed.</summary>
        protected internal bool preClosed = false;

        /// <summary>Signature field lock dictionary.</summary>
        protected internal PdfSigFieldLock fieldLock;

        /// <summary>The signature appearance.</summary>
        protected internal PdfSignatureAppearance appearance;

        /// <summary>Holds value of property signDate.</summary>
        protected internal DateTime signDate;

        /// <summary>Boolean to check if this PdfSigner instance has been closed already or not.</summary>
        protected internal bool closed;

        /// <summary>Creates a PdfSigner instance.</summary>
        /// <remarks>
        /// Creates a PdfSigner instance. Uses a
        /// <see cref="System.IO.MemoryStream"/>
        /// instead of a temporary file.
        /// </remarks>
        /// <param name="reader">PdfReader that reads the PDF file</param>
        /// <param name="outputStream">OutputStream to write the signed PDF file</param>
        /// <param name="properties">
        /// 
        /// <see cref="iText.Kernel.Pdf.StampingProperties"/>
        /// for the signing document. Note that encryption will be
        /// preserved regardless of what is set in properties.
        /// </param>
        public PdfSigner(PdfReader reader, Stream outputStream, StampingProperties properties)
            : this(reader, outputStream, null, properties) {
        }

        /// <summary>Creates a PdfSigner instance.</summary>
        /// <remarks>
        /// Creates a PdfSigner instance. Uses a
        /// <see cref="System.IO.MemoryStream"/>
        /// instead of a temporary file.
        /// </remarks>
        /// <param name="reader">PdfReader that reads the PDF file</param>
        /// <param name="outputStream">OutputStream to write the signed PDF file</param>
        /// <param name="path">File to which the output is temporarily written</param>
        /// <param name="properties">
        /// 
        /// <see cref="iText.Kernel.Pdf.StampingProperties"/>
        /// for the signing document. Note that encryption will be
        /// preserved regardless of what is set in properties.
        /// </param>
        public PdfSigner(PdfReader reader, Stream outputStream, String path, StampingProperties properties) {
            StampingProperties localProps = new StampingProperties(properties).PreserveEncryption();
            if (path == null) {
                temporaryOS = new MemoryStream();
                document = InitDocument(reader, new PdfWriter(temporaryOS), localProps);
            }
            else {
                this.tempFile = FileUtil.CreateTempFile(path);
                document = InitDocument(reader, new PdfWriter(FileUtil.GetFileOutputStream(tempFile)), localProps);
            }
            originalOS = outputStream;
            signDate = DateTimeUtil.GetCurrentTime();
            fieldName = GetNewSigFieldName();
            appearance = new PdfSignatureAppearance(document, new Rectangle(0, 0), 1);
            appearance.SetSignDate(signDate);
            closed = false;
        }

        protected internal virtual PdfDocument InitDocument(PdfReader reader, PdfWriter writer, StampingProperties
             properties) {
            return new PdfAAgnosticPdfDocument(reader, writer, properties);
        }

        /// <summary>Gets the signature date.</summary>
        /// <returns>Calendar set to the signature date</returns>
        public virtual DateTime GetSignDate() {
            return signDate;
        }

        /// <summary>Sets the signature date.</summary>
        /// <param name="signDate">the signature date</param>
        public virtual void SetSignDate(DateTime signDate) {
            this.signDate = signDate;
            this.appearance.SetSignDate(signDate);
        }

        /// <summary>Provides access to a signature appearance object.</summary>
        /// <remarks>
        /// Provides access to a signature appearance object. Use it to
        /// customize the appearance of the signature.
        /// <para />
        /// Be aware:
        /// <list type="bullet">
        /// <item><description>If you create new signature field (either use
        /// <see cref="SetFieldName(System.String)"/>
        /// with
        /// the name that doesn't exist in the document or don't specify it at all) then
        /// the signature is invisible by default.
        /// </description></item>
        /// <item><description>If you sign already existing field, then the signature appearance object
        /// is modified to have all the properties (page num., rect etc.) consistent with
        /// the state of the field (<strong>if you customized the appearance object
        /// before the
        /// <see cref="SetFieldName(System.String)"/>
        /// call you'll have to do it again</strong>)
        /// </description></item>
        /// </list>
        /// <para />
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="PdfSignatureAppearance"/>
        /// object.
        /// </returns>
        public virtual PdfSignatureAppearance GetSignatureAppearance() {
            return appearance;
        }

        /// <summary>Returns the document's certification level.</summary>
        /// <remarks>
        /// Returns the document's certification level.
        /// For possible values see
        /// <see cref="SetCertificationLevel(int)"/>.
        /// </remarks>
        /// <returns>The certified status.</returns>
        public virtual int GetCertificationLevel() {
            return this.certificationLevel;
        }

        /// <summary>Sets the document's certification level.</summary>
        /// <param name="certificationLevel">
        /// a new certification level for a document.
        /// Possible values are: <list type="bullet">
        /// <item><description>
        /// <see cref="NOT_CERTIFIED"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="CERTIFIED_NO_CHANGES_ALLOWED"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="CERTIFIED_FORM_FILLING"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="CERTIFIED_FORM_FILLING_AND_ANNOTATIONS"/>
        /// </description></item>
        /// </list>
        /// </param>
        public virtual void SetCertificationLevel(int certificationLevel) {
            this.certificationLevel = certificationLevel;
        }

        /// <summary>Gets the field name.</summary>
        /// <returns>the field name</returns>
        public virtual String GetFieldName() {
            return fieldName;
        }

        /// <summary>Returns the user made signature dictionary.</summary>
        /// <remarks>
        /// Returns the user made signature dictionary. This is the dictionary at the /V key
        /// of the signature field.
        /// </remarks>
        /// <returns>The user made signature dictionary.</returns>
        public virtual PdfSignature GetSignatureDictionary() {
            return cryptoDictionary;
        }

        /// <summary>Getter for property signatureEvent.</summary>
        /// <returns>Value of property signatureEvent.</returns>
        public virtual PdfSigner.ISignatureEvent GetSignatureEvent() {
            return this.signatureEvent;
        }

        /// <summary>Sets the signature event to allow modification of the signature dictionary.</summary>
        /// <param name="signatureEvent">the signature event</param>
        public virtual void SetSignatureEvent(PdfSigner.ISignatureEvent signatureEvent) {
            this.signatureEvent = signatureEvent;
        }

        /// <summary>Gets a new signature field name that doesn't clash with any existing name.</summary>
        /// <returns>A new signature field name.</returns>
        public virtual String GetNewSigFieldName() {
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
            String name = "Signature";
            int step = 1;
            while (acroForm.GetField(name + step) != null) {
                ++step;
            }
            return name + step;
        }

        /// <summary>Sets the name indicating the field to be signed.</summary>
        /// <remarks>
        /// Sets the name indicating the field to be signed. The field can already be presented in the
        /// document but shall not be signed. If the field is not presented in the document, it will be created.
        /// </remarks>
        /// <param name="fieldName">The name indicating the field to be signed.</param>
        public virtual void SetFieldName(String fieldName) {
            if (fieldName != null) {
                if (fieldName.IndexOf('.') >= 0) {
                    throw new ArgumentException(SignExceptionMessageConstant.FIELD_NAMES_CANNOT_CONTAIN_A_DOT);
                }
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
                if (acroForm.GetField(fieldName) != null) {
                    PdfFormField field = acroForm.GetField(fieldName);
                    if (!PdfName.Sig.Equals(field.GetFormType())) {
                        throw new ArgumentException(SignExceptionMessageConstant.FIELD_TYPE_IS_NOT_A_SIGNATURE_FIELD_TYPE);
                    }
                    if (field.GetValue() != null) {
                        throw new ArgumentException(SignExceptionMessageConstant.FIELD_ALREADY_SIGNED);
                    }
                    appearance.SetFieldName(fieldName);
                    IList<PdfWidgetAnnotation> widgets = field.GetWidgets();
                    if (widgets.Count > 0) {
                        PdfWidgetAnnotation widget = widgets[0];
                        appearance.SetPageRect(GetWidgetRectangle(widget));
                        appearance.SetPageNumber(GetWidgetPageNumber(widget));
                    }
                }
                this.fieldName = fieldName;
            }
        }

        /// <summary>Gets the PdfDocument associated with this instance.</summary>
        /// <returns>the PdfDocument associated with this instance</returns>
        public virtual PdfDocument GetDocument() {
            return document;
        }

        /// <summary>Sets the PdfDocument.</summary>
        /// <param name="document">The PdfDocument</param>
        protected internal virtual void SetDocument(PdfDocument document) {
            if (null == document.GetReader()) {
                throw new ArgumentException(SignExceptionMessageConstant.DOCUMENT_MUST_HAVE_READER);
            }
            this.document = document;
        }

        /// <summary>Setter for the OutputStream.</summary>
        /// <param name="originalOS">OutputStream for the bytes of the document</param>
        public virtual void SetOriginalOutputStream(Stream originalOS) {
            this.originalOS = originalOS;
        }

        /// <summary>Getter for the field lock dictionary.</summary>
        /// <returns>Field lock dictionary.</returns>
        public virtual PdfSigFieldLock GetFieldLockDict() {
            return fieldLock;
        }

        /// <summary>Setter for the field lock dictionary.</summary>
        /// <remarks>
        /// Setter for the field lock dictionary.
        /// <para />
        /// <strong>Be aware:</strong> if a signature is created on an existing signature field,
        /// then its /Lock dictionary takes the precedence (if it exists).
        /// </remarks>
        /// <param name="fieldLock">Field lock dictionary</param>
        public virtual void SetFieldLockDict(PdfSigFieldLock fieldLock) {
            this.fieldLock = fieldLock;
        }

        /// <summary>Signs the document using the detached mode, CMS or CAdES equivalent.</summary>
        /// <remarks>
        /// Signs the document using the detached mode, CMS or CAdES equivalent.
        /// <br /><br />
        /// NOTE: This method closes the underlying pdf document. This means, that current instance
        /// of PdfSigner cannot be used after this method call.
        /// </remarks>
        /// <param name="externalSignature">the interface providing the actual signing</param>
        /// <param name="chain">the certificate chain</param>
        /// <param name="crlList">the CRL list</param>
        /// <param name="ocspClient">the OCSP client</param>
        /// <param name="tsaClient">the Timestamp client</param>
        /// <param name="estimatedSize">the reserved size for the signature. It will be estimated if 0</param>
        /// <param name="sigtype">Either Signature.CMS or Signature.CADES</param>
        public virtual void SignDetached(IExternalSignature externalSignature, X509Certificate[] chain, ICollection
            <ICrlClient> crlList, IOcspClient ocspClient, ITSAClient tsaClient, int estimatedSize, PdfSigner.CryptoStandard
             sigtype) {
            SignDetached(externalSignature, chain, crlList, ocspClient, tsaClient, estimatedSize, sigtype, (SignaturePolicyIdentifier
                )null);
        }

        /// <summary>Signs the document using the detached mode, CMS or CAdES equivalent.</summary>
        /// <remarks>
        /// Signs the document using the detached mode, CMS or CAdES equivalent.
        /// <br /><br />
        /// NOTE: This method closes the underlying pdf document. This means, that current instance
        /// of PdfSigner cannot be used after this method call.
        /// </remarks>
        /// <param name="externalSignature">the interface providing the actual signing</param>
        /// <param name="chain">the certificate chain</param>
        /// <param name="crlList">the CRL list</param>
        /// <param name="ocspClient">the OCSP client</param>
        /// <param name="tsaClient">the Timestamp client</param>
        /// <param name="estimatedSize">the reserved size for the signature. It will be estimated if 0</param>
        /// <param name="sigtype">Either Signature.CMS or Signature.CADES</param>
        /// <param name="signaturePolicy">the signature policy (for EPES signatures)</param>
        public virtual void SignDetached(IExternalSignature externalSignature, X509Certificate[] chain, ICollection
            <ICrlClient> crlList, IOcspClient ocspClient, ITSAClient tsaClient, int estimatedSize, PdfSigner.CryptoStandard
             sigtype, SignaturePolicyInfo signaturePolicy) {
            SignDetached(externalSignature, chain, crlList, ocspClient, tsaClient, estimatedSize, sigtype, signaturePolicy
                .ToSignaturePolicyIdentifier());
        }

        /// <summary>Signs the document using the detached mode, CMS or CAdES equivalent.</summary>
        /// <remarks>
        /// Signs the document using the detached mode, CMS or CAdES equivalent.
        /// <br /><br />
        /// NOTE: This method closes the underlying pdf document. This means, that current instance
        /// of PdfSigner cannot be used after this method call.
        /// </remarks>
        /// <param name="externalSignature">the interface providing the actual signing</param>
        /// <param name="chain">the certificate chain</param>
        /// <param name="crlList">the CRL list</param>
        /// <param name="ocspClient">the OCSP client</param>
        /// <param name="tsaClient">the Timestamp client</param>
        /// <param name="estimatedSize">the reserved size for the signature. It will be estimated if 0</param>
        /// <param name="sigtype">Either Signature.CMS or Signature.CADES</param>
        /// <param name="signaturePolicy">the signature policy (for EPES signatures)</param>
        public virtual void SignDetached(IExternalSignature externalSignature, X509Certificate[] chain, ICollection
            <ICrlClient> crlList, IOcspClient ocspClient, ITSAClient tsaClient, int estimatedSize, PdfSigner.CryptoStandard
             sigtype, SignaturePolicyIdentifier signaturePolicy) {
            if (closed) {
                throw new PdfException(SignExceptionMessageConstant.THIS_INSTANCE_OF_PDF_SIGNER_ALREADY_CLOSED);
            }
            if (certificationLevel > 0 && IsDocumentPdf2()) {
                if (DocumentContainsCertificationOrApprovalSignatures()) {
                    throw new PdfException(SignExceptionMessageConstant.CERTIFICATION_SIGNATURE_CREATION_FAILED_DOC_SHALL_NOT_CONTAIN_SIGS
                        );
                }
            }
            ICollection<byte[]> crlBytes = null;
            int i = 0;
            while (crlBytes == null && i < chain.Length) {
                crlBytes = ProcessCrl(chain[i++], crlList);
            }
            if (estimatedSize == 0) {
                estimatedSize = 8192;
                if (crlBytes != null) {
                    foreach (byte[] element in crlBytes) {
                        estimatedSize += element.Length + 10;
                    }
                }
                if (ocspClient != null) {
                    estimatedSize += 4192;
                }
                if (tsaClient != null) {
                    estimatedSize += 4192;
                }
            }
            PdfSignatureAppearance appearance = GetSignatureAppearance();
            appearance.SetCertificate(chain[0]);
            if (sigtype == PdfSigner.CryptoStandard.CADES && !IsDocumentPdf2()) {
                AddDeveloperExtension(PdfDeveloperExtension.ESIC_1_7_EXTENSIONLEVEL2);
            }
            String hashAlgorithm = externalSignature.GetHashAlgorithm();
            PdfSignature dic = new PdfSignature(PdfName.Adobe_PPKLite, sigtype == PdfSigner.CryptoStandard.CADES ? PdfName
                .ETSI_CAdES_DETACHED : PdfName.Adbe_pkcs7_detached);
            dic.SetReason(appearance.GetReason());
            dic.SetLocation(appearance.GetLocation());
            dic.SetSignatureCreator(appearance.GetSignatureCreator());
            dic.SetContact(appearance.GetContact());
            dic.SetDate(new PdfDate(GetSignDate()));
            // time-stamp will over-rule this
            cryptoDictionary = dic;
            IDictionary<PdfName, int?> exc = new Dictionary<PdfName, int?>();
            exc.Put(PdfName.Contents, estimatedSize * 2 + 2);
            PreClose(exc);
            PdfPKCS7 sgn = new PdfPKCS7((ICipherParameters)null, chain, hashAlgorithm, false);
            if (signaturePolicy != null) {
                sgn.SetSignaturePolicy(signaturePolicy);
            }
            Stream data = GetRangeStream();
            byte[] hash = DigestAlgorithms.Digest(data, SignUtils.GetMessageDigest(hashAlgorithm));
            IList<byte[]> ocspList = new List<byte[]>();
            if (chain.Length > 1 && ocspClient != null) {
                for (int j = 0; j < chain.Length - 1; ++j) {
                    byte[] ocsp = ocspClient.GetEncoded((X509Certificate)chain[j], (X509Certificate)chain[j + 1], null);
                    if (ocsp != null) {
                        ocspList.Add(ocsp);
                    }
                }
            }
            byte[] sh = sgn.GetAuthenticatedAttributeBytes(hash, sigtype, ocspList, crlBytes);
            byte[] extSignature = externalSignature.Sign(sh);
            sgn.SetExternalDigest(extSignature, null, externalSignature.GetEncryptionAlgorithm());
            byte[] encodedSig = sgn.GetEncodedPKCS7(hash, sigtype, tsaClient, ocspList, crlBytes);
            if (estimatedSize < encodedSig.Length) {
                throw new System.IO.IOException("Not enough space");
            }
            byte[] paddedSig = new byte[estimatedSize];
            Array.Copy(encodedSig, 0, paddedSig, 0, encodedSig.Length);
            PdfDictionary dic2 = new PdfDictionary();
            dic2.Put(PdfName.Contents, new PdfString(paddedSig).SetHexWriting(true));
            Close(dic2);
            closed = true;
        }

        /// <summary>Sign the document using an external container, usually a PKCS7.</summary>
        /// <remarks>
        /// Sign the document using an external container, usually a PKCS7. The signature is fully composed
        /// externally, iText will just put the container inside the document.
        /// <br /><br />
        /// NOTE: This method closes the underlying pdf document. This means, that current instance
        /// of PdfSigner cannot be used after this method call.
        /// </remarks>
        /// <param name="externalSignatureContainer">the interface providing the actual signing</param>
        /// <param name="estimatedSize">the reserved size for the signature</param>
        public virtual void SignExternalContainer(IExternalSignatureContainer externalSignatureContainer, int estimatedSize
            ) {
            if (closed) {
                throw new PdfException(SignExceptionMessageConstant.THIS_INSTANCE_OF_PDF_SIGNER_ALREADY_CLOSED);
            }
            PdfSignature dic = new PdfSignature();
            PdfSignatureAppearance appearance = GetSignatureAppearance();
            dic.SetReason(appearance.GetReason());
            dic.SetLocation(appearance.GetLocation());
            dic.SetSignatureCreator(appearance.GetSignatureCreator());
            dic.SetContact(appearance.GetContact());
            dic.SetDate(new PdfDate(GetSignDate()));
            // time-stamp will over-rule this
            externalSignatureContainer.ModifySigningDictionary(dic.GetPdfObject());
            cryptoDictionary = dic;
            IDictionary<PdfName, int?> exc = new Dictionary<PdfName, int?>();
            exc.Put(PdfName.Contents, estimatedSize * 2 + 2);
            PreClose(exc);
            Stream data = GetRangeStream();
            byte[] encodedSig = externalSignatureContainer.Sign(data);
            if (estimatedSize < encodedSig.Length) {
                throw new System.IO.IOException(SignExceptionMessageConstant.NOT_ENOUGH_SPACE);
            }
            byte[] paddedSig = new byte[estimatedSize];
            Array.Copy(encodedSig, 0, paddedSig, 0, encodedSig.Length);
            PdfDictionary dic2 = new PdfDictionary();
            dic2.Put(PdfName.Contents, new PdfString(paddedSig).SetHexWriting(true));
            Close(dic2);
            closed = true;
        }

        /// <summary>Signs a document with a PAdES-LTV Timestamp.</summary>
        /// <remarks>
        /// Signs a document with a PAdES-LTV Timestamp. The document is closed at the end.
        /// <br /><br />
        /// NOTE: This method closes the underlying pdf document. This means, that current instance
        /// of PdfSigner cannot be used after this method call.
        /// </remarks>
        /// <param name="tsa">the timestamp generator</param>
        /// <param name="signatureName">
        /// the signature name or null to have a name generated
        /// automatically
        /// </param>
        public virtual void Timestamp(ITSAClient tsa, String signatureName) {
            if (closed) {
                throw new PdfException(SignExceptionMessageConstant.THIS_INSTANCE_OF_PDF_SIGNER_ALREADY_CLOSED);
            }
            int contentEstimated = tsa.GetTokenSizeEstimate();
            if (!IsDocumentPdf2()) {
                AddDeveloperExtension(PdfDeveloperExtension.ESIC_1_7_EXTENSIONLEVEL5);
            }
            SetFieldName(signatureName);
            PdfSignature dic = new PdfSignature(PdfName.Adobe_PPKLite, PdfName.ETSI_RFC3161);
            dic.Put(PdfName.Type, PdfName.DocTimeStamp);
            cryptoDictionary = dic;
            IDictionary<PdfName, int?> exc = new Dictionary<PdfName, int?>();
            exc.Put(PdfName.Contents, contentEstimated * 2 + 2);
            PreClose(exc);
            Stream data = GetRangeStream();
            IDigest messageDigest = tsa.GetMessageDigest();
            byte[] buf = new byte[4096];
            int n;
            while ((n = data.Read(buf)) > 0) {
                messageDigest.Update(buf, 0, n);
            }
            byte[] tsImprint = messageDigest.Digest();
            byte[] tsToken;
            try {
                tsToken = tsa.GetTimeStampToken(tsImprint);
            }
            catch (Exception e) {
                throw new GeneralSecurityException(e.Message, e);
            }
            if (contentEstimated + 2 < tsToken.Length) {
                throw new System.IO.IOException("Not enough space");
            }
            byte[] paddedSig = new byte[contentEstimated];
            Array.Copy(tsToken, 0, paddedSig, 0, tsToken.Length);
            PdfDictionary dic2 = new PdfDictionary();
            dic2.Put(PdfName.Contents, new PdfString(paddedSig).SetHexWriting(true));
            Close(dic2);
            closed = true;
        }

        /// <summary>Signs a PDF where space was already reserved.</summary>
        /// <param name="document">the original PDF</param>
        /// <param name="fieldName">the field to sign. It must be the last field</param>
        /// <param name="outs">the output PDF</param>
        /// <param name="externalSignatureContainer">
        /// the signature container doing the actual signing. Only the
        /// method ExternalSignatureContainer.sign is used
        /// </param>
        public static void SignDeferred(PdfDocument document, String fieldName, Stream outs, IExternalSignatureContainer
             externalSignatureContainer) {
            SignatureUtil signatureUtil = new SignatureUtil(document);
            PdfSignature signature = signatureUtil.GetSignature(fieldName);
            if (signature == null) {
                throw new PdfException(SignExceptionMessageConstant.THERE_IS_NO_FIELD_IN_THE_DOCUMENT_WITH_SUCH_NAME).SetMessageParams
                    (fieldName);
            }
            if (!signatureUtil.SignatureCoversWholeDocument(fieldName)) {
                throw new PdfException(SignExceptionMessageConstant.SIGNATURE_WITH_THIS_NAME_IS_NOT_THE_LAST_IT_DOES_NOT_COVER_WHOLE_DOCUMENT
                    ).SetMessageParams(fieldName);
            }
            PdfArray b = signature.GetByteRange();
            long[] gaps = b.ToLongArray();
            if (b.Size() != 4 || gaps[0] != 0) {
                throw new ArgumentException("Single exclusion space supported");
            }
            IRandomAccessSource readerSource = document.GetReader().GetSafeFile().CreateSourceView();
            Stream rg = new RASInputStream(new RandomAccessSourceFactory().CreateRanged(readerSource, gaps));
            byte[] signedContent = externalSignatureContainer.Sign(rg);
            int spaceAvailable = (int)(gaps[2] - gaps[1]) - 2;
            if ((spaceAvailable & 1) != 0) {
                throw new ArgumentException("Gap is not a multiple of 2");
            }
            spaceAvailable /= 2;
            if (spaceAvailable < signedContent.Length) {
                throw new PdfException(SignExceptionMessageConstant.AVAILABLE_SPACE_IS_NOT_ENOUGH_FOR_SIGNATURE);
            }
            StreamUtil.CopyBytes(readerSource, 0, gaps[1] + 1, outs);
            ByteBuffer bb = new ByteBuffer(spaceAvailable * 2);
            foreach (byte bi in signedContent) {
                bb.AppendHex(bi);
            }
            int remain = (spaceAvailable - signedContent.Length) * 2;
            for (int k = 0; k < remain; ++k) {
                bb.Append((byte)48);
            }
            byte[] bbArr = bb.ToByteArray();
            outs.Write(bbArr);
            StreamUtil.CopyBytes(readerSource, gaps[2] - 1, gaps[3] + 1, outs);
        }

        /// <summary>Processes a CRL list.</summary>
        /// <param name="cert">a Certificate if one of the CrlList implementations needs to retrieve the CRL URL from it.
        ///     </param>
        /// <param name="crlList">a list of CrlClient implementations</param>
        /// <returns>a collection of CRL bytes that can be embedded in a PDF</returns>
        protected internal virtual ICollection<byte[]> ProcessCrl(X509Certificate cert, ICollection<ICrlClient> crlList
            ) {
            if (crlList == null) {
                return null;
            }
            IList<byte[]> crlBytes = new List<byte[]>();
            foreach (ICrlClient cc in crlList) {
                if (cc == null) {
                    continue;
                }
                ICollection<byte[]> b = cc.GetEncoded((X509Certificate)cert, null);
                if (b == null) {
                    continue;
                }
                crlBytes.AddAll(b);
            }
            return crlBytes.Count == 0 ? null : crlBytes;
        }

        protected internal virtual void AddDeveloperExtension(PdfDeveloperExtension extension) {
            document.GetCatalog().AddDeveloperExtension(extension);
        }

        /// <summary>Checks if the document is in the process of closing.</summary>
        /// <returns>true if the document is in the process of closing, false otherwise</returns>
        protected internal virtual bool IsPreClosed() {
            return preClosed;
        }

        /// <summary>This is the first method to be called when using external signatures.</summary>
        /// <remarks>
        /// This is the first method to be called when using external signatures. The general sequence is:
        /// preClose(), getDocumentBytes() and close().
        /// <para />
        /// <c>exclusionSizes</c> must contain at least
        /// the <c>PdfName.CONTENTS</c> key with the size that it will take in the
        /// document. Note that due to the hex string coding this size should be byte_size*2+2.
        /// </remarks>
        /// <param name="exclusionSizes">
        /// Map with names and sizes to be excluded in the signature
        /// calculation. The key is a PdfName and the value an Integer. At least the /Contents must be present
        /// </param>
        protected internal virtual void PreClose(IDictionary<PdfName, int?> exclusionSizes) {
            if (preClosed) {
                throw new PdfException(SignExceptionMessageConstant.DOCUMENT_ALREADY_PRE_CLOSED);
            }
            preClosed = true;
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
            SignatureUtil sgnUtil = new SignatureUtil(document);
            String name = GetFieldName();
            bool fieldExist = sgnUtil.DoesSignatureFieldExist(name);
            acroForm.SetSignatureFlags(PdfAcroForm.SIGNATURE_EXIST | PdfAcroForm.APPEND_ONLY);
            PdfSigFieldLock fieldLock = null;
            if (cryptoDictionary == null) {
                throw new PdfException(SignExceptionMessageConstant.NO_CRYPTO_DICTIONARY_DEFINED);
            }
            cryptoDictionary.GetPdfObject().MakeIndirect(document);
            if (fieldExist) {
                fieldLock = PopulateExistingSignatureFormField(acroForm);
            }
            else {
                fieldLock = CreateNewSignatureFormField(acroForm, name);
            }
            exclusionLocations = new Dictionary<PdfName, PdfLiteral>();
            PdfLiteral lit = new PdfLiteral(80);
            exclusionLocations.Put(PdfName.ByteRange, lit);
            cryptoDictionary.Put(PdfName.ByteRange, lit);
            foreach (KeyValuePair<PdfName, int?> entry in exclusionSizes) {
                PdfName key = entry.Key;
                lit = new PdfLiteral((int)entry.Value);
                exclusionLocations.Put(key, lit);
                cryptoDictionary.Put(key, lit);
            }
            if (certificationLevel > 0) {
                AddDocMDP(cryptoDictionary);
            }
            if (fieldLock != null) {
                AddFieldMDP(cryptoDictionary, fieldLock);
            }
            if (signatureEvent != null) {
                signatureEvent.GetSignatureDictionary(cryptoDictionary);
            }
            if (certificationLevel > 0) {
                // add DocMDP entry to root
                PdfDictionary docmdp = new PdfDictionary();
                docmdp.Put(PdfName.DocMDP, cryptoDictionary.GetPdfObject());
                document.GetCatalog().Put(PdfName.Perms, docmdp);
                document.GetCatalog().SetModified();
            }
            document.CheckIsoConformance(cryptoDictionary.GetPdfObject(), IsoKey.SIGNATURE);
            cryptoDictionary.GetPdfObject().Flush(false);
            document.Close();
            range = new long[exclusionLocations.Count * 2];
            long byteRangePosition = exclusionLocations.Get(PdfName.ByteRange).GetPosition();
            exclusionLocations.JRemove(PdfName.ByteRange);
            int idx = 1;
            foreach (PdfLiteral lit1 in exclusionLocations.Values) {
                long n = lit1.GetPosition();
                range[idx++] = n;
                range[idx++] = lit1.GetBytesCount() + n;
            }
            JavaUtil.Sort(range, 1, range.Length - 1);
            for (int k = 3; k < range.Length - 2; k += 2) {
                range[k] -= range[k - 1];
            }
            if (tempFile == null) {
                bout = temporaryOS.ToArray();
                range[range.Length - 1] = bout.Length - range[range.Length - 2];
                MemoryStream bos = new MemoryStream();
                PdfOutputStream os = new PdfOutputStream(bos);
                os.Write('[');
                for (int k = 0; k < range.Length; ++k) {
                    os.WriteLong(range[k]).Write(' ');
                }
                os.Write(']');
                Array.Copy(bos.ToArray(), 0, bout, (int)byteRangePosition, (int)bos.Length);
            }
            else {
                try {
                    raf = FileUtil.GetRandomAccessFile(tempFile);
                    long len = raf.Length;
                    range[range.Length - 1] = len - range[range.Length - 2];
                    MemoryStream bos = new MemoryStream();
                    PdfOutputStream os = new PdfOutputStream(bos);
                    os.Write('[');
                    for (int k = 0; k < range.Length; ++k) {
                        os.WriteLong(range[k]).Write(' ');
                    }
                    os.Write(']');
                    raf.Seek(byteRangePosition);
                    raf.Write(bos.ToArray(), 0, (int)bos.Length);
                }
                catch (System.IO.IOException e) {
                    try {
                        raf.Dispose();
                    }
                    catch (Exception) {
                    }
                    try {
                        tempFile.Delete();
                    }
                    catch (Exception) {
                    }
                    throw;
                }
            }
        }

        /// <summary>Populates already existing signature form field in the acroForm object.</summary>
        /// <remarks>
        /// Populates already existing signature form field in the acroForm object.
        /// This method is called during the
        /// <see cref="PreClose(System.Collections.Generic.IDictionary{K, V})"/>
        /// method if the signature field already exists.
        /// </remarks>
        /// <param name="acroForm">
        /// 
        /// <see cref="iText.Forms.PdfAcroForm"/>
        /// object in which the signature field will be populated
        /// </param>
        /// <returns>signature field lock dictionary</returns>
        protected internal virtual PdfSigFieldLock PopulateExistingSignatureFormField(PdfAcroForm acroForm) {
            PdfSignatureFormField sigField = (PdfSignatureFormField)acroForm.GetField(fieldName);
            sigField.Put(PdfName.V, cryptoDictionary.GetPdfObject());
            PdfSigFieldLock sigFieldLock = sigField.GetSigFieldLockDictionary();
            if (sigFieldLock == null && this.fieldLock != null) {
                this.fieldLock.GetPdfObject().MakeIndirect(document);
                sigField.Put(PdfName.Lock, this.fieldLock.GetPdfObject());
                sigFieldLock = this.fieldLock;
            }
            sigField.Put(PdfName.P, document.GetPage(appearance.GetPageNumber()).GetPdfObject());
            sigField.Put(PdfName.V, cryptoDictionary.GetPdfObject());
            PdfObject obj = sigField.GetPdfObject().Get(PdfName.F);
            int flags = 0;
            if (obj != null && obj.IsNumber()) {
                flags = ((PdfNumber)obj).IntValue();
            }
            flags |= PdfAnnotation.LOCKED;
            sigField.Put(PdfName.F, new PdfNumber(flags));
            if (appearance.IsInvisible()) {
                // According to the spec, appearance stream is not required if the width and height of the rectangle are 0
                sigField.Remove(PdfName.AP);
            }
            else {
                PdfDictionary ap = new PdfDictionary();
                ap.Put(PdfName.N, appearance.GetAppearance().GetPdfObject());
                sigField.Put(PdfName.AP, ap);
            }
            sigField.SetModified();
            return sigFieldLock;
        }

        /// <summary>Creates new signature form field and adds it to the acroForm object.</summary>
        /// <remarks>
        /// Creates new signature form field and adds it to the acroForm object.
        /// This method is called during the
        /// <see cref="PreClose(System.Collections.Generic.IDictionary{K, V})"/>
        /// method if the signature field doesn't exist.
        /// </remarks>
        /// <param name="acroForm">
        /// 
        /// <see cref="iText.Forms.PdfAcroForm"/>
        /// object in which new signature field will be added
        /// </param>
        /// <param name="name">the name of the field</param>
        /// <returns>signature field lock dictionary</returns>
        protected internal virtual PdfSigFieldLock CreateNewSignatureFormField(PdfAcroForm acroForm, String name) {
            PdfWidgetAnnotation widget = new PdfWidgetAnnotation(appearance.GetPageRect());
            widget.SetFlags(PdfAnnotation.PRINT | PdfAnnotation.LOCKED);
            PdfSignatureFormField sigField = PdfFormField.CreateSignature(document);
            sigField.SetFieldName(name);
            sigField.Put(PdfName.V, cryptoDictionary.GetPdfObject());
            sigField.AddKid(widget);
            PdfSigFieldLock sigFieldLock = sigField.GetSigFieldLockDictionary();
            if (this.fieldLock != null) {
                this.fieldLock.GetPdfObject().MakeIndirect(document);
                sigField.Put(PdfName.Lock, this.fieldLock.GetPdfObject());
                sigFieldLock = this.fieldLock;
            }
            int pagen = appearance.GetPageNumber();
            widget.SetPage(document.GetPage(pagen));
            if (appearance.IsInvisible()) {
                // According to the spec, appearance stream is not required if the width and height of the rectangle are 0
                widget.Remove(PdfName.AP);
            }
            else {
                PdfDictionary ap = widget.GetAppearanceDictionary();
                if (ap == null) {
                    ap = new PdfDictionary();
                    widget.Put(PdfName.AP, ap);
                }
                ap.Put(PdfName.N, appearance.GetAppearance().GetPdfObject());
            }
            acroForm.AddField(sigField, document.GetPage(pagen));
            if (acroForm.GetPdfObject().IsIndirect()) {
                acroForm.SetModified();
            }
            else {
                //Acroform dictionary is a Direct dictionary,
                //for proper flushing, catalog needs to be marked as modified
                document.GetCatalog().SetModified();
            }
            return sigFieldLock;
        }

        /// <summary>Gets the document bytes that are hashable when using external signatures.</summary>
        /// <remarks>
        /// Gets the document bytes that are hashable when using external signatures.
        /// The general sequence is:
        /// <see cref="PreClose(System.Collections.Generic.IDictionary{K, V})"/>
        /// ,
        /// <see cref="GetRangeStream()"/>
        /// and
        /// <see cref="Close(iText.Kernel.Pdf.PdfDictionary)"/>.
        /// </remarks>
        /// <returns>
        /// The
        /// <see cref="System.IO.Stream"/>
        /// of bytes to be signed.
        /// </returns>
        protected internal virtual Stream GetRangeStream() {
            RandomAccessSourceFactory fac = new RandomAccessSourceFactory();
            IRandomAccessSource randomAccessSource = fac.CreateRanged(GetUnderlyingSource(), range);
            return new RASInputStream(randomAccessSource);
        }

        /// <summary>This is the last method to be called when using external signatures.</summary>
        /// <remarks>
        /// This is the last method to be called when using external signatures. The general sequence is:
        /// preClose(), getDocumentBytes() and close().
        /// <para />
        /// update is a PdfDictionary that must have exactly the
        /// same keys as the ones provided in
        /// <see cref="PreClose(System.Collections.Generic.IDictionary{K, V})"/>.
        /// </remarks>
        /// <param name="update">
        /// a PdfDictionary with the key/value that will fill the holes defined
        /// in
        /// <see cref="PreClose(System.Collections.Generic.IDictionary{K, V})"/>
        /// </param>
        protected internal virtual void Close(PdfDictionary update) {
            try {
                if (!preClosed) {
                    throw new PdfException(SignExceptionMessageConstant.DOCUMENT_MUST_BE_PRE_CLOSED);
                }
                MemoryStream bous = new MemoryStream();
                PdfOutputStream os = new PdfOutputStream(bous);
                foreach (PdfName key in update.KeySet()) {
                    PdfObject obj = update.Get(key);
                    PdfLiteral lit = exclusionLocations.Get(key);
                    if (lit == null) {
                        throw new ArgumentException("The key didn't reserve space in preclose");
                    }
                    bous.JReset();
                    os.Write(obj);
                    if (bous.Length > lit.GetBytesCount()) {
                        throw new ArgumentException(SignExceptionMessageConstant.TOO_BIG_KEY);
                    }
                    if (tempFile == null) {
                        Array.Copy(bous.ToArray(), 0, bout, (int)lit.GetPosition(), (int)bous.Length);
                    }
                    else {
                        raf.Seek(lit.GetPosition());
                        raf.Write(bous.ToArray(), 0, (int)bous.Length);
                    }
                }
                if (update.Size() != exclusionLocations.Count) {
                    throw new ArgumentException("The update dictionary has less keys than required");
                }
                if (tempFile == null) {
                    originalOS.Write(bout, 0, bout.Length);
                }
                else {
                    if (originalOS != null) {
                        raf.Seek(0);
                        long length = raf.Length;
                        byte[] buf = new byte[8192];
                        while (length > 0) {
                            int r = raf.JRead(buf, 0, (int)Math.Min((long)buf.Length, length));
                            if (r < 0) {
                                throw new EndOfStreamException("unexpected eof");
                            }
                            originalOS.Write(buf, 0, r);
                            length -= r;
                        }
                    }
                }
            }
            finally {
                if (tempFile != null) {
                    raf.Dispose();
                    if (originalOS != null) {
                        tempFile.Delete();
                    }
                }
                if (originalOS != null) {
                    try {
                        originalOS.Dispose();
                    }
                    catch (Exception) {
                    }
                }
            }
        }

        /// <summary>Returns the underlying source.</summary>
        /// <returns>the underlying source</returns>
        protected internal virtual IRandomAccessSource GetUnderlyingSource() {
            RandomAccessSourceFactory fac = new RandomAccessSourceFactory();
            return raf == null ? fac.CreateSource(bout) : fac.CreateSource(raf);
        }

        /// <summary>Adds keys to the signature dictionary that define the certification level and the permissions.</summary>
        /// <remarks>
        /// Adds keys to the signature dictionary that define the certification level and the permissions.
        /// This method is only used for Certifying signatures.
        /// </remarks>
        /// <param name="crypto">the signature dictionary</param>
        protected internal virtual void AddDocMDP(PdfSignature crypto) {
            PdfDictionary reference = new PdfDictionary();
            PdfDictionary transformParams = new PdfDictionary();
            transformParams.Put(PdfName.P, new PdfNumber(certificationLevel));
            transformParams.Put(PdfName.V, new PdfName("1.2"));
            transformParams.Put(PdfName.Type, PdfName.TransformParams);
            reference.Put(PdfName.TransformMethod, PdfName.DocMDP);
            reference.Put(PdfName.Type, PdfName.SigRef);
            reference.Put(PdfName.TransformParams, transformParams);
            reference.Put(PdfName.Data, document.GetTrailer().Get(PdfName.Root));
            PdfArray types = new PdfArray();
            types.Add(reference);
            crypto.Put(PdfName.Reference, types);
        }

        /// <summary>Adds keys to the signature dictionary that define the field permissions.</summary>
        /// <remarks>
        /// Adds keys to the signature dictionary that define the field permissions.
        /// This method is only used for signatures that lock fields.
        /// </remarks>
        /// <param name="crypto">the signature dictionary</param>
        /// <param name="fieldLock">
        /// the
        /// <see cref="iText.Forms.PdfSigFieldLock"/>
        /// instance specified the field lock to be set
        /// </param>
        protected internal virtual void AddFieldMDP(PdfSignature crypto, PdfSigFieldLock fieldLock) {
            PdfDictionary reference = new PdfDictionary();
            PdfDictionary transformParams = new PdfDictionary();
            transformParams.PutAll(fieldLock.GetPdfObject());
            transformParams.Put(PdfName.Type, PdfName.TransformParams);
            transformParams.Put(PdfName.V, new PdfName("1.2"));
            reference.Put(PdfName.TransformMethod, PdfName.FieldMDP);
            reference.Put(PdfName.Type, PdfName.SigRef);
            reference.Put(PdfName.TransformParams, transformParams);
            reference.Put(PdfName.Data, document.GetTrailer().Get(PdfName.Root));
            PdfArray types = crypto.GetPdfObject().GetAsArray(PdfName.Reference);
            if (types == null) {
                types = new PdfArray();
                crypto.Put(PdfName.Reference, types);
            }
            types.Add(reference);
        }

        protected internal virtual bool DocumentContainsCertificationOrApprovalSignatures() {
            bool containsCertificationOrApprovalSignature = false;
            PdfDictionary urSignature = null;
            PdfDictionary catalogPerms = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Perms);
            if (catalogPerms != null) {
                urSignature = catalogPerms.GetAsDictionary(PdfName.UR3);
            }
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, false);
            if (acroForm != null) {
                foreach (KeyValuePair<String, PdfFormField> entry in acroForm.GetFormFields()) {
                    PdfDictionary fieldDict = entry.Value.GetPdfObject();
                    if (!PdfName.Sig.Equals(fieldDict.Get(PdfName.FT))) {
                        continue;
                    }
                    PdfDictionary sigDict = fieldDict.GetAsDictionary(PdfName.V);
                    if (sigDict == null) {
                        continue;
                    }
                    PdfSignature pdfSignature = new PdfSignature(sigDict);
                    if (pdfSignature.GetContents() == null || pdfSignature.GetByteRange() == null) {
                        continue;
                    }
                    if (!pdfSignature.GetType().Equals(PdfName.DocTimeStamp) && sigDict != urSignature) {
                        containsCertificationOrApprovalSignature = true;
                        break;
                    }
                }
            }
            return containsCertificationOrApprovalSignature;
        }

        /// <summary>Get the rectangle associated to the provided widget.</summary>
        /// <param name="widget">PdfWidgetAnnotation to extract the rectangle from</param>
        /// <returns>Rectangle</returns>
        protected internal virtual Rectangle GetWidgetRectangle(PdfWidgetAnnotation widget) {
            return widget.GetRectangle().ToRectangle();
        }

        /// <summary>Get the page number associated to the provided widget.</summary>
        /// <param name="widget">PdfWidgetAnnotation from which to extract the page number</param>
        /// <returns>page number</returns>
        protected internal virtual int GetWidgetPageNumber(PdfWidgetAnnotation widget) {
            int pageNumber = 0;
            PdfDictionary pageDict = widget.GetPdfObject().GetAsDictionary(PdfName.P);
            if (pageDict != null) {
                pageNumber = document.GetPageNumber(pageDict);
            }
            else {
                for (int i = 1; i <= document.GetNumberOfPages(); i++) {
                    PdfPage page = document.GetPage(i);
                    if (!page.IsFlushed()) {
                        if (page.ContainsAnnotation(widget)) {
                            pageNumber = i;
                            break;
                        }
                    }
                }
            }
            return pageNumber;
        }

        private bool IsDocumentPdf2() {
            return document.GetPdfVersion().CompareTo(PdfVersion.PDF_2_0) >= 0;
        }

        /// <summary>An interface to retrieve the signature dictionary for modification.</summary>
        public interface ISignatureEvent {
            /// <summary>Allows modification of the signature dictionary.</summary>
            /// <param name="sig">The signature dictionary</param>
            void GetSignatureDictionary(PdfSignature sig);
        }
    }
}
