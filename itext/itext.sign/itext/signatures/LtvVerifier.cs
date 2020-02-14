/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using Common.Logging;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using iText.Forms;
using iText.IO.Util;
using iText.Kernel.Counter.Event;
using iText.Kernel.Pdf;

namespace iText.Signatures {
    /// <summary>Verifies the signatures in an LTV document.</summary>
    public class LtvVerifier : RootStoreVerifier {
        /// <summary>The Logger instance</summary>
        protected internal static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Signatures.LtvVerifier)
            );

        /// <summary>Option to specify level of verification; signing certificate only or the entire chain.</summary>
        protected internal LtvVerification.CertificateOption option = LtvVerification.CertificateOption.SIGNING_CERTIFICATE;

        /// <summary>Verify root.</summary>
        protected internal bool verifyRootCertificate = true;

        /// <summary>A document object for the revision that is being verified.</summary>
        protected internal PdfDocument document;

        /// <summary>The fields in the revision that is being verified.</summary>
        protected internal PdfAcroForm acroForm;

        /// <summary>The date the revision was signed, or <c>null</c> for the highest revision.</summary>
        protected internal DateTime signDate;

        /// <summary>The signature that covers the revision.</summary>
        protected internal String signatureName;

        /// <summary>The PdfPKCS7 object for the signature.</summary>
        protected internal PdfPKCS7 pkcs7;

        /// <summary>Indicates if we're working with the latest revision.</summary>
        protected internal bool latestRevision = true;

        /// <summary>The document security store for the revision that is being verified</summary>
        protected internal PdfDictionary dss;

        /// <summary>The meta info</summary>
        protected internal IMetaInfo metaInfo;

        private SignatureUtil sgnUtil;

        /// <summary>Creates a VerificationData object for a PdfReader</summary>
        /// <param name="document">The document we want to verify.</param>
        public LtvVerifier(PdfDocument document)
            : base(null) {
            InitLtvVerifier(document);
        }

        /// <summary>Sets an extra verifier.</summary>
        /// <param name="verifier">the verifier to set</param>
        public virtual void SetVerifier(CertificateVerifier verifier) {
            this.verifier = verifier;
        }

        /// <summary>Sets the certificate option.</summary>
        /// <param name="option">Either CertificateOption.SIGNING_CERTIFICATE (default) or CertificateOption.WHOLE_CHAIN
        ///     </param>
        public virtual void SetCertificateOption(LtvVerification.CertificateOption option) {
            this.option = option;
        }

        /// <summary>Set the verifyRootCertificate to false if you can't verify the root certificate.</summary>
        /// <param name="verifyRootCertificate"/>
        public virtual void SetVerifyRootCertificate(bool verifyRootCertificate) {
            this.verifyRootCertificate = verifyRootCertificate;
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Kernel.Counter.Event.IMetaInfo"/>
        /// that will be used during
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// creation.
        /// </summary>
        /// <param name="metaInfo">meta info to set</param>
        public virtual void SetEventCountingMetaInfo(IMetaInfo metaInfo) {
            this.metaInfo = metaInfo;
        }

        /// <summary>Verifies all the document-level timestamps and all the signatures in the document.</summary>
        /// <param name="result"/>
        public virtual IList<VerificationOK> Verify(IList<VerificationOK> result) {
            if (result == null) {
                result = new List<VerificationOK>();
            }
            while (pkcs7 != null) {
                result.AddAll(VerifySignature());
            }
            return result;
        }

        /// <summary>Verifies a document level timestamp.</summary>
        public virtual IList<VerificationOK> VerifySignature() {
            LOGGER.Info("Verifying signature.");
            IList<VerificationOK> result = new List<VerificationOK>();
            // Get the certificate chain
            X509Certificate[] chain = pkcs7.GetSignCertificateChain();
            VerifyChain(chain);
            // how many certificates in the chain do we need to check?
            int total = 1;
            if (LtvVerification.CertificateOption.WHOLE_CHAIN.Equals(option)) {
                total = chain.Length;
            }
            // loop over the certificates
            X509Certificate signCert;
            X509Certificate issuerCert;
            for (int i = 0; i < total; ) {
                // the certificate to check
                signCert = (X509Certificate)chain[i++];
                // its issuer
                issuerCert = (X509Certificate)null;
                if (i < chain.Length) {
                    issuerCert = (X509Certificate)chain[i];
                }
                // now lets verify the certificate
                LOGGER.Info(signCert.SubjectDN.ToString());
                IList<VerificationOK> list = Verify(signCert, issuerCert, signDate);
                if (list.Count == 0) {
                    try {
                        signCert.Verify(signCert.GetPublicKey());
                        if (latestRevision && chain.Length > 1) {
                            list.Add(new VerificationOK(signCert, this.GetType(), "Root certificate in final revision"));
                        }
                        if (list.Count == 0 && verifyRootCertificate) {
                            throw new GeneralSecurityException();
                        }
                        else {
                            if (chain.Length > 1) {
                                list.Add(new VerificationOK(signCert, this.GetType(), "Root certificate passed without checking"));
                            }
                        }
                    }
                    catch (GeneralSecurityException) {
                        throw new VerificationException(signCert, "Couldn't verify with CRL or OCSP or trusted anchor");
                    }
                }
                result.AddAll(list);
            }
            // go to the previous revision
            SwitchToPreviousRevision();
            return result;
        }

        /// <summary>
        /// Checks the certificates in a certificate chain:
        /// are they valid on a specific date, and
        /// do they chain up correctly?
        /// </summary>
        /// <param name="chain">the certificate chain</param>
        public virtual void VerifyChain(X509Certificate[] chain) {
            // Loop over the certificates in the chain
            for (int i = 0; i < chain.Length; i++) {
                X509Certificate cert = (X509Certificate)chain[i];
                // check if the certificate was/is valid
                cert.CheckValidity(signDate);
                // check if the previous certificate was issued by this certificate
                if (i > 0) {
                    chain[i - 1].Verify(chain[i].GetPublicKey());
                }
            }
            LOGGER.Info("All certificates are valid on " + signDate.ToString());
        }

        /// <summary>Verifies certificates against a list of CRLs and OCSP responses.</summary>
        /// <param name="signCert">the signing certificate</param>
        /// <param name="issuerCert">the issuer's certificate</param>
        /// <returns>
        /// a list of <c>VerificationOK</c> objects.
        /// The list will be empty if the certificate couldn't be verified.
        /// </returns>
        /// <seealso cref="RootStoreVerifier.Verify(Org.BouncyCastle.X509.X509Certificate, Org.BouncyCastle.X509.X509Certificate, System.DateTime)
        ///     "/>
        public override IList<VerificationOK> Verify(X509Certificate signCert, X509Certificate issuerCert, DateTime
             signDate) {
            // we'll verify agains the rootstore (if present)
            RootStoreVerifier rootStoreVerifier = new RootStoreVerifier(verifier);
            rootStoreVerifier.SetRootStore(rootStore);
            // We'll verify against a list of CRLs
            CRLVerifier crlVerifier = new CRLVerifier(rootStoreVerifier, GetCRLsFromDSS());
            crlVerifier.SetRootStore(rootStore);
            crlVerifier.SetOnlineCheckingAllowed(latestRevision || onlineCheckingAllowed);
            // We'll verify against a list of OCSPs
            OCSPVerifier ocspVerifier = new OCSPVerifier(crlVerifier, GetOCSPResponsesFromDSS());
            ocspVerifier.SetRootStore(rootStore);
            ocspVerifier.SetOnlineCheckingAllowed(latestRevision || onlineCheckingAllowed);
            // We verify the chain
            return ocspVerifier.Verify(signCert, issuerCert, signDate);
        }

        /// <summary>Switches to the previous revision.</summary>
        public virtual void SwitchToPreviousRevision() {
            LOGGER.Info("Switching to previous revision.");
            latestRevision = false;
            dss = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            DateTime cal = pkcs7.GetTimeStampDate();
            if (cal == TimestampConstants.UNDEFINED_TIMESTAMP_DATE) {
                cal = pkcs7.GetSignDate();
            }
            // TODO: get date from signature
            signDate = cal.ToUniversalTime();
            IList<String> names = sgnUtil.GetSignatureNames();
            if (names.Count > 1) {
                signatureName = names[names.Count - 2];
                document = new PdfDocument(new PdfReader(sgnUtil.ExtractRevision(signatureName)), new DocumentProperties()
                    .SetEventCountingMetaInfo(metaInfo));
                this.acroForm = PdfAcroForm.GetAcroForm(document, true);
                this.sgnUtil = new SignatureUtil(document);
                names = sgnUtil.GetSignatureNames();
                signatureName = names[names.Count - 1];
                pkcs7 = CoversWholeDocument();
                LOGGER.Info(MessageFormatUtil.Format("Checking {0}signature {1}", pkcs7.IsTsp() ? "document-level timestamp "
                     : "", signatureName));
            }
            else {
                LOGGER.Info("No signatures in revision");
                pkcs7 = null;
            }
        }

        /// <summary>Gets a list of X509CRL objects from a Document Security Store.</summary>
        /// <returns>a list of CRLs</returns>
        public virtual IList<X509Crl> GetCRLsFromDSS() {
            IList<X509Crl> crls = new List<X509Crl>();
            if (dss == null) {
                return crls;
            }
            PdfArray crlarray = dss.GetAsArray(PdfName.CRLs);
            if (crlarray == null) {
                return crls;
            }
            for (int i = 0; i < crlarray.Size(); i++) {
                PdfStream stream = crlarray.GetAsStream(i);
                crls.Add((X509Crl)SignUtils.ParseCrlFromStream(new MemoryStream(stream.GetBytes())));
            }
            return crls;
        }

        /// <summary>Gets OCSP responses from the Document Security Store.</summary>
        /// <returns>a list of BasicOCSPResp objects</returns>
        public virtual IList<BasicOcspResp> GetOCSPResponsesFromDSS() {
            IList<BasicOcspResp> ocsps = new List<BasicOcspResp>();
            if (dss == null) {
                return ocsps;
            }
            PdfArray ocsparray = dss.GetAsArray(PdfName.OCSPs);
            if (ocsparray == null) {
                return ocsps;
            }
            for (int i = 0; i < ocsparray.Size(); i++) {
                PdfStream stream = ocsparray.GetAsStream(i);
                OcspResp ocspResponse = new OcspResp(stream.GetBytes());
                if (ocspResponse.Status == 0) {
                    try {
                        ocsps.Add((BasicOcspResp)ocspResponse.GetResponseObject());
                    }
                    catch (OcspException e) {
                        throw new GeneralSecurityException(e.ToString());
                    }
                }
            }
            return ocsps;
        }

        protected internal virtual void InitLtvVerifier(PdfDocument document) {
            this.document = document;
            this.acroForm = PdfAcroForm.GetAcroForm(document, true);
            this.sgnUtil = new SignatureUtil(document);
            IList<String> names = sgnUtil.GetSignatureNames();
            signatureName = names[names.Count - 1];
            this.signDate = DateTimeUtil.GetCurrentUtcTime();
            pkcs7 = CoversWholeDocument();
            LOGGER.Info(MessageFormatUtil.Format("Checking {0}signature {1}", pkcs7.IsTsp() ? "document-level timestamp "
                 : "", signatureName));
        }

        /// <summary>
        /// Checks if the signature covers the whole document
        /// and throws an exception if the document was altered
        /// </summary>
        /// <returns>a PdfPKCS7 object</returns>
        protected internal virtual PdfPKCS7 CoversWholeDocument() {
            PdfPKCS7 pkcs7 = sgnUtil.ReadSignatureData(signatureName);
            if (sgnUtil.SignatureCoversWholeDocument(signatureName)) {
                LOGGER.Info("The timestamp covers whole document.");
            }
            else {
                throw new VerificationException((X509Certificate)null, "Signature doesn't cover whole document.");
            }
            if (pkcs7.VerifySignatureIntegrityAndAuthenticity()) {
                LOGGER.Info("The signed document has not been modified.");
                return pkcs7;
            }
            else {
                throw new VerificationException((X509Certificate)null, "The document was altered after the final signature was applied."
                    );
            }
        }
    }
}
