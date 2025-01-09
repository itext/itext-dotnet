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
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Source;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Exceptions;
using iText.Signatures.Logs;

namespace iText.Signatures {
    /// <summary>Add verification according to PAdES-LTV (part 4).</summary>
    public class LtvVerification {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.LtvVerification
            ));

        private readonly PdfDocument document;

        private readonly SignatureUtil sgnUtil;

        private readonly IDictionary<PdfName, LtvVerification.ValidationData> validated = new Dictionary<PdfName, 
            LtvVerification.ValidationData>();

        private bool used = false;

        private LtvVerification.RevocationDataNecessity revocationDataNecessity = LtvVerification.RevocationDataNecessity
            .OPTIONAL;

        private IIssuingCertificateRetriever issuingCertificateRetriever = new DefaultIssuingCertificateRetriever(
            );

        /// <summary>What type of verification to include.</summary>
        public enum Level {
            /// <summary>Include only OCSP.</summary>
            OCSP,
            /// <summary>Include only CRL.</summary>
            CRL,
            /// <summary>Include both OCSP and CRL.</summary>
            OCSP_CRL,
            /// <summary>Include CRL only if OCSP can't be read.</summary>
            OCSP_OPTIONAL_CRL
        }

        /// <summary>Options for how many certificates to include.</summary>
        public enum CertificateOption {
            /// <summary>Include verification just for the signing certificate.</summary>
            SIGNING_CERTIFICATE,
            /// <summary>Include verification for the whole chain of certificates.</summary>
            WHOLE_CHAIN,
            /// <summary>
            /// Include verification for the whole certificates chain, certificates used to create OCSP responses,
            /// CRL response certificates and timestamp certificates included in the signatures.
            /// </summary>
            ALL_CERTIFICATES
        }

        /// <summary>
        /// Certificate inclusion in the DSS and VRI dictionaries in the CERT and CERTS
        /// keys.
        /// </summary>
        public enum CertificateInclusion {
            /// <summary>Include certificates in the DSS and VRI dictionaries.</summary>
            YES,
            /// <summary>Do not include certificates in the DSS and VRI dictionaries.</summary>
            NO
        }

        /// <summary>Option to determine whether revocation information is required for the signing certificate.</summary>
        public enum RevocationDataNecessity {
            /// <summary>Require revocation information for the signing certificate.</summary>
            REQUIRED_FOR_SIGNING_CERTIFICATE,
            /// <summary>Revocation data for the signing certificate may be optional.</summary>
            OPTIONAL
        }

        /// <summary>The verification constructor.</summary>
        /// <remarks>
        /// The verification constructor. This class should only be created with
        /// PdfStamper.getLtvVerification() otherwise the information will not be
        /// added to the Pdf.
        /// </remarks>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to apply the validation to.
        /// </param>
        public LtvVerification(PdfDocument document) {
            this.document = document;
            this.sgnUtil = new SignatureUtil(document);
        }

        /// <summary>
        /// Sets
        /// <see cref="RevocationDataNecessity"/>
        /// option to specify the necessity of revocation data.
        /// </summary>
        /// <remarks>
        /// Sets
        /// <see cref="RevocationDataNecessity"/>
        /// option to specify the necessity of revocation data.
        /// <para />
        /// Default value is
        /// <see cref="RevocationDataNecessity.OPTIONAL"/>.
        /// </remarks>
        /// <param name="revocationDataNecessity">
        /// 
        /// <see cref="RevocationDataNecessity"/>
        /// value to set
        /// </param>
        /// <returns>
        /// this
        /// <see cref="LtvVerification"/>
        /// instance.
        /// </returns>
        public virtual LtvVerification SetRevocationDataNecessity(LtvVerification.RevocationDataNecessity revocationDataNecessity
            ) {
            this.revocationDataNecessity = revocationDataNecessity;
            return this;
        }

        /// <summary>
        /// Sets
        /// <see cref="IIssuingCertificateRetriever"/>
        /// instance needed to get CRL issuer certificates (using AIA extension).
        /// </summary>
        /// <remarks>
        /// Sets
        /// <see cref="IIssuingCertificateRetriever"/>
        /// instance needed to get CRL issuer certificates (using AIA extension).
        /// <para />
        /// Default value is
        /// <see cref="DefaultIssuingCertificateRetriever"/>.
        /// </remarks>
        /// <param name="issuingCertificateRetriever">
        /// 
        /// <see cref="IIssuingCertificateRetriever"/>
        /// instance to set
        /// </param>
        /// <returns>
        /// this
        /// <see cref="LtvVerification"/>
        /// instance.
        /// </returns>
        public virtual LtvVerification SetIssuingCertificateRetriever(IIssuingCertificateRetriever issuingCertificateRetriever
            ) {
            this.issuingCertificateRetriever = issuingCertificateRetriever;
            return this;
        }

        /// <summary>Add verification for a particular signature.</summary>
        /// <param name="signatureName">the signature to validate (it may be a timestamp)</param>
        /// <param name="ocsp">the interface to get the OCSP</param>
        /// <param name="crl">the interface to get the CRL</param>
        /// <param name="certOption">options as to how many certificates to include</param>
        /// <param name="level">the validation options to include</param>
        /// <param name="certInclude">certificate inclusion options</param>
        /// <returns>true if a validation was generated, false otherwise</returns>
        public virtual bool AddVerification(String signatureName, IOcspClient ocsp, ICrlClient crl, LtvVerification.CertificateOption
             certOption, LtvVerification.Level level, LtvVerification.CertificateInclusion certInclude) {
            if (used) {
                throw new InvalidOperationException(SignExceptionMessageConstant.VERIFICATION_ALREADY_OUTPUT);
            }
            PdfPKCS7 pk = sgnUtil.ReadSignatureData(signatureName);
            LOGGER.LogInformation("Adding verification for " + signatureName);
            IX509Certificate[] certificateChain = pk.GetCertificates();
            IX509Certificate signingCert = pk.GetSigningCertificate();
            LtvVerification.ValidationData validationData = new LtvVerification.ValidationData();
            ICollection<IX509Certificate> processedCerts = new HashSet<IX509Certificate>();
            AddRevocationDataForChain(signingCert, certificateChain, ocsp, crl, level, certInclude, certOption, validationData
                , processedCerts);
            if (certOption == LtvVerification.CertificateOption.ALL_CERTIFICATES) {
                IX509Certificate[] timestampCertsChain = pk.GetTimestampCertificates();
                AddRevocationDataForChain(signingCert, timestampCertsChain, ocsp, crl, level, certInclude, certOption, validationData
                    , processedCerts);
            }
            if (certInclude == LtvVerification.CertificateInclusion.YES) {
                foreach (IX509Certificate processedCert in processedCerts) {
                    IList<byte[]> certs = validationData.GetCerts();
                    certs.Add(processedCert.GetEncoded());
                    validationData.SetCerts(certs);
                }
            }
            if (validationData.GetCrls().IsEmpty() && validationData.GetOcsps().IsEmpty()) {
                return false;
            }
            validated.Put(GetSignatureHashKey(signatureName), validationData);
            return true;
        }

        /// <summary>Adds verification to the signature.</summary>
        /// <param name="signatureName">name of the signature</param>
        /// <param name="ocsps">collection of DER-encoded BasicOCSPResponses</param>
        /// <param name="crls">collection of DER-encoded CRLs</param>
        /// <param name="certs">collection of DER-encoded certificates</param>
        /// <returns>boolean</returns>
        public virtual bool AddVerification(String signatureName, ICollection<byte[]> ocsps, ICollection<byte[]> crls
            , ICollection<byte[]> certs) {
            if (used) {
                throw new InvalidOperationException(SignExceptionMessageConstant.VERIFICATION_ALREADY_OUTPUT);
            }
            LtvVerification.ValidationData vd = new LtvVerification.ValidationData();
            if (ocsps != null) {
                foreach (byte[] ocsp in ocsps) {
                    IList<byte[]> ocspsArr = vd.GetOcsps();
                    ocspsArr.Add(LtvVerification.BuildOCSPResponse(ocsp));
                    vd.SetOcsps(ocspsArr);
                }
            }
            if (crls != null) {
                IList<byte[]> crlsArr = vd.GetCrls();
                crlsArr.AddAll(crls);
                vd.SetCrls(crlsArr);
            }
            if (certs != null) {
                IList<byte[]> certsArr = vd.GetCerts();
                certsArr.AddAll(certs);
                vd.SetCerts(certsArr);
            }
            validated.Put(GetSignatureHashKey(signatureName), vd);
            return true;
        }

        /// <summary>Merges the validation with any validation already in the document or creates a new one.</summary>
        public virtual void Merge() {
            if (used || validated.IsEmpty()) {
                return;
            }
            used = true;
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            PdfObject dss = catalog.Get(PdfName.DSS);
            if (dss == null) {
                CreateDss();
            }
            else {
                UpdateDss();
            }
        }

        /// <summary>Converts an array of bytes to a String of hexadecimal values</summary>
        /// <param name="bytes">a byte array</param>
        /// <returns>the same bytes expressed as hexadecimal values</returns>
        public static String ConvertToHex(byte[] bytes) {
            ByteBuffer buf = new ByteBuffer();
            foreach (byte b in bytes) {
                buf.AppendHex(b);
            }
            return PdfEncodings.ConvertToString(buf.ToByteArray(), null).ToUpperInvariant();
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Get the issuing certificate for a child certificate.</summary>
        /// <param name="cert">the certificate for which we search the parent</param>
        /// <param name="certs">an array with certificates that contains the parent</param>
        /// <returns>the parent certificate</returns>
        internal virtual IX509Certificate GetParent(IX509Certificate cert, IX509Certificate[] certs) {
            IX509Certificate parent;
            foreach (IX509Certificate certificate in certs) {
                parent = (IX509Certificate)certificate;
                if (!cert.GetIssuerDN().Equals(parent.GetSubjectDN())) {
                    continue;
                }
                try {
                    cert.Verify(parent.GetPublicKey());
                    return parent;
                }
                catch (Exception) {
                }
            }
            // do nothing
            return null;
        }
//\endcond

        private void AddRevocationDataForChain(IX509Certificate signingCert, IX509Certificate[] certChain, IOcspClient
             ocsp, ICrlClient crl, LtvVerification.Level level, LtvVerification.CertificateInclusion certInclude, 
            LtvVerification.CertificateOption certOption, LtvVerification.ValidationData validationData, ICollection
            <IX509Certificate> processedCerts) {
            IX509Certificate[] fullChain = certOption == LtvVerification.CertificateOption.ALL_CERTIFICATES ? RetrieveMissingCertificates
                (certChain) : certChain;
            foreach (IX509Certificate certificate in fullChain) {
                IX509Certificate cert = (IX509Certificate)certificate;
                LOGGER.LogInformation(MessageFormatUtil.Format("Certificate: {0}", BOUNCY_CASTLE_FACTORY.CreateX500Name(cert
                    )));
                if ((certOption == LtvVerification.CertificateOption.SIGNING_CERTIFICATE && !cert.Equals(signingCert)) || 
                    processedCerts.Contains(cert)) {
                    continue;
                }
                AddRevocationDataForCertificate(signingCert, fullChain, cert, ocsp, crl, level, certInclude, certOption, validationData
                    , processedCerts);
            }
        }

        private void AddRevocationDataForCertificate(IX509Certificate signingCert, IX509Certificate[] certificateChain
            , IX509Certificate cert, IOcspClient ocsp, ICrlClient crl, LtvVerification.Level level, LtvVerification.CertificateInclusion
             certInclude, LtvVerification.CertificateOption certOption, LtvVerification.ValidationData validationData
            , ICollection<IX509Certificate> processedCerts) {
            processedCerts.Add(cert);
            byte[] validityAssured = SignUtils.GetExtensionValueByOid(cert, OID.X509Extensions.VALIDITY_ASSURED_SHORT_TERM
                );
            if (validityAssured != null) {
                LOGGER.LogInformation(MessageFormatUtil.Format(SignLogMessageConstant.REVOCATION_DATA_NOT_ADDED_VALIDITY_ASSURED
                    , cert.GetSubjectDN()));
                return;
            }
            byte[] ocspEnc = null;
            bool revocationDataAdded = false;
            if (ocsp != null && level != LtvVerification.Level.CRL) {
                ocspEnc = ocsp.GetEncoded(cert, GetParent(cert, certificateChain), null);
                if (ocspEnc != null && BOUNCY_CASTLE_FACTORY.CreateCertificateStatus().GetGood().Equals(OcspClientBouncyCastle
                    .GetCertificateStatus(ocspEnc))) {
                    IList<byte[]> ocsps = validationData.GetOcsps();
                    ocsps.Add(LtvVerification.BuildOCSPResponse(ocspEnc));
                    validationData.SetOcsps(ocsps);
                    revocationDataAdded = true;
                    LOGGER.LogInformation("OCSP added");
                    if (certOption == LtvVerification.CertificateOption.ALL_CERTIFICATES) {
                        AddRevocationDataForOcspCert(ocspEnc, signingCert, ocsp, crl, level, certInclude, certOption, validationData
                            , processedCerts);
                    }
                }
                else {
                    ocspEnc = null;
                }
            }
            if (crl != null && (level == LtvVerification.Level.CRL || level == LtvVerification.Level.OCSP_CRL || (level
                 == LtvVerification.Level.OCSP_OPTIONAL_CRL && ocspEnc == null))) {
                ICollection<byte[]> cims = crl.GetEncoded(cert, null);
                if (cims != null) {
                    foreach (byte[] cim in cims) {
                        revocationDataAdded = true;
                        bool dup = false;
                        foreach (byte[] b in validationData.GetCrls()) {
                            if (JavaUtil.ArraysEquals(b, cim)) {
                                dup = true;
                                break;
                            }
                        }
                        if (!dup) {
                            IList<byte[]> crls = validationData.GetCrls();
                            crls.Add(cim);
                            validationData.SetCrls(crls);
                            LOGGER.LogInformation("CRL added");
                            if (certOption == LtvVerification.CertificateOption.ALL_CERTIFICATES) {
                                IX509Certificate[] certsList = issuingCertificateRetriever.GetCrlIssuerCertificates(SignUtils.ParseCrlFromStream
                                    (new MemoryStream(cim)));
                                AddRevocationDataForChain(signingCert, certsList, ocsp, crl, level, certInclude, certOption, validationData
                                    , processedCerts);
                            }
                        }
                    }
                }
            }
            if (revocationDataNecessity == LtvVerification.RevocationDataNecessity.REQUIRED_FOR_SIGNING_CERTIFICATE &&
                 signingCert.Equals(cert) && !revocationDataAdded) {
                throw new PdfException(SignExceptionMessageConstant.NO_REVOCATION_DATA_FOR_SIGNING_CERTIFICATE);
            }
        }

        private void AddRevocationDataForOcspCert(byte[] ocspEnc, IX509Certificate signingCert, IOcspClient ocsp, 
            ICrlClient crl, LtvVerification.Level level, LtvVerification.CertificateInclusion certInclude, LtvVerification.CertificateOption
             certOption, LtvVerification.ValidationData validationData, ICollection<IX509Certificate> processedCerts
            ) {
            IBasicOcspResponse ocspResp = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(ocspEnc);
            IEnumerable<IX509Certificate> certs = SignUtils.GetCertsFromOcspResponse(ocspResp);
            IList<IX509Certificate> ocspCertsList = IterableToList(certs);
            IX509Certificate ocspSigningCert = null;
            foreach (IX509Certificate ocspCert in ocspCertsList) {
                try {
                    if (SignUtils.IsSignatureValid(ocspResp, ocspCert)) {
                        ocspSigningCert = ocspCert;
                        break;
                    }
                }
                catch (AbstractOperatorCreationException) {
                }
                catch (AbstractOcspException) {
                }
            }
            // Wasn't possible to check if this cert is signing one, skip.
            if (ocspSigningCert != null && SignUtils.GetExtensionValueByOid(ocspSigningCert, OID.X509Extensions.ID_PKIX_OCSP_NOCHECK
                ) != null) {
                // If ocsp_no_check extension is set on OCSP signing cert we shan't collect revocation data for this cert.
                ocspCertsList.Remove(ocspSigningCert);
                processedCerts.Add(ocspSigningCert);
            }
            AddRevocationDataForChain(signingCert, ocspCertsList.ToArray(new IX509Certificate[0]), ocsp, crl, level, certInclude
                , certOption, validationData, processedCerts);
        }

        private static IList<IX509Certificate> IterableToList(IEnumerable<IX509Certificate> iterable) {
            IList<IX509Certificate> list = new List<IX509Certificate>();
            foreach (IX509Certificate certificate in iterable) {
                list.Add(certificate);
            }
            return list;
        }

        private static byte[] BuildOCSPResponse(byte[] basicOcspResponse) {
            IDerOctetString doctet = BOUNCY_CASTLE_FACTORY.CreateDEROctetString(basicOcspResponse);
            IOcspResponseStatus respStatus = BOUNCY_CASTLE_FACTORY.CreateOCSPResponseStatus(BOUNCY_CASTLE_FACTORY.CreateOCSPResponseStatus
                ().GetSuccessful());
            IResponseBytes responseBytes = BOUNCY_CASTLE_FACTORY.CreateResponseBytes(BOUNCY_CASTLE_FACTORY.CreateOCSPObjectIdentifiers
                ().GetIdPkixOcspBasic(), doctet);
            IOcspResponse ocspResponse = BOUNCY_CASTLE_FACTORY.CreateOCSPResponse(respStatus, responseBytes);
            return ocspResponse.GetEncoded();
        }

        private PdfName GetSignatureHashKey(String signatureName) {
            PdfSignature sig = sgnUtil.GetSignature(signatureName);
            PdfString contents = sig.GetContents();
            byte[] bc = PdfEncodings.ConvertToBytes(contents.GetValue(), null);
            byte[] bt = HashBytesSha1(bc);
            return new PdfName(ConvertToHex(bt));
        }

        private static byte[] HashBytesSha1(byte[] b) {
            IMessageDigest sh = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("SHA1"
                );
            return sh.Digest(b);
        }

        private void UpdateDss() {
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            catalog.SetModified();
            PdfDictionary dss = catalog.GetAsDictionary(PdfName.DSS);
            PdfArray ocsps = dss.GetAsArray(PdfName.OCSPs);
            PdfArray crls = dss.GetAsArray(PdfName.CRLs);
            PdfArray certs = dss.GetAsArray(PdfName.Certs);
            dss.Remove(PdfName.OCSPs);
            dss.Remove(PdfName.CRLs);
            dss.Remove(PdfName.Certs);
            PdfDictionary vrim = dss.GetAsDictionary(PdfName.VRI);
            // delete old validations
            if (vrim != null) {
                foreach (PdfName n in vrim.KeySet()) {
                    if (validated.ContainsKey(n)) {
                        PdfDictionary vri = vrim.GetAsDictionary(n);
                        if (vri != null) {
                            DeleteOldReferences(ocsps, vri.GetAsArray(PdfName.OCSP));
                            DeleteOldReferences(crls, vri.GetAsArray(PdfName.CRL));
                            DeleteOldReferences(certs, vri.GetAsArray(PdfName.Cert));
                        }
                    }
                }
            }
            if (ocsps == null) {
                ocsps = new PdfArray();
            }
            if (crls == null) {
                crls = new PdfArray();
            }
            if (certs == null) {
                certs = new PdfArray();
            }
            if (vrim == null) {
                vrim = new PdfDictionary();
            }
            OutputDss(dss, vrim, ocsps, crls, certs);
        }

        private static void DeleteOldReferences(PdfArray all, PdfArray toDelete) {
            if (all == null || toDelete == null) {
                return;
            }
            foreach (PdfObject pi in toDelete) {
                PdfIndirectReference pir = pi.GetIndirectReference();
                for (int i = 0; i < all.Size(); i++) {
                    PdfIndirectReference pod = all.Get(i).GetIndirectReference();
                    if (Object.Equals(pir, pod)) {
                        all.Remove(i);
                        i--;
                    }
                }
            }
        }

        private void CreateDss() {
            OutputDss(new PdfDictionary(), new PdfDictionary(), new PdfArray(), new PdfArray(), new PdfArray());
        }

        private void OutputDss(PdfDictionary dss, PdfDictionary vrim, PdfArray ocsps, PdfArray crls, PdfArray certs
            ) {
            PdfCatalog catalog = document.GetCatalog();
            if (document.GetPdfVersion().CompareTo(PdfVersion.PDF_2_0) < 0) {
                catalog.AddDeveloperExtension(PdfDeveloperExtension.ESIC_1_7_EXTENSIONLEVEL5);
            }
            foreach (PdfName vkey in validated.Keys) {
                PdfArray ocsp = new PdfArray();
                PdfArray crl = new PdfArray();
                PdfArray cert = new PdfArray();
                PdfDictionary vri = new PdfDictionary();
                foreach (byte[] b in validated.Get(vkey).GetCrls()) {
                    PdfStream ps = new PdfStream(b);
                    ps.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                    ps.MakeIndirect(document);
                    crl.Add(ps);
                    crls.Add(ps);
                    crls.SetModified();
                }
                foreach (byte[] b in validated.Get(vkey).GetOcsps()) {
                    PdfStream ps = new PdfStream(b);
                    ps.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                    ocsp.Add(ps);
                    ocsps.Add(ps);
                    ocsps.SetModified();
                }
                foreach (byte[] b in validated.Get(vkey).GetCerts()) {
                    PdfStream ps = new PdfStream(b);
                    ps.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                    ps.MakeIndirect(document);
                    cert.Add(ps);
                    certs.Add(ps);
                    certs.SetModified();
                }
                if (!ocsp.IsEmpty()) {
                    ocsp.MakeIndirect(document);
                    vri.Put(PdfName.OCSP, ocsp);
                }
                if (!crl.IsEmpty()) {
                    crl.MakeIndirect(document);
                    vri.Put(PdfName.CRL, crl);
                }
                if (!cert.IsEmpty()) {
                    cert.MakeIndirect(document);
                    vri.Put(PdfName.Cert, cert);
                }
                vri.MakeIndirect(document);
                vrim.Put(vkey, vri);
            }
            vrim.MakeIndirect(document);
            vrim.SetModified();
            dss.Put(PdfName.VRI, vrim);
            if (!ocsps.IsEmpty()) {
                ocsps.MakeIndirect(document);
                dss.Put(PdfName.OCSPs, ocsps);
            }
            if (!crls.IsEmpty()) {
                crls.MakeIndirect(document);
                dss.Put(PdfName.CRLs, crls);
            }
            if (!certs.IsEmpty()) {
                certs.MakeIndirect(document);
                dss.Put(PdfName.Certs, certs);
            }
            dss.MakeIndirect(document);
            dss.SetModified();
            catalog.Put(PdfName.DSS, dss);
        }

        private class ValidationData {
            private IList<byte[]> crls = new List<byte[]>();

            private IList<byte[]> ocsps = new List<byte[]>();

            private IList<byte[]> certs = new List<byte[]>();

            /// <summary>Sets the crls byte array.</summary>
            /// <param name="crls">crls</param>
            public virtual void SetCrls(IList<byte[]> crls) {
                this.crls = crls;
            }

            /// <summary>Retrieves Crls byte array.</summary>
            /// <returns>crls</returns>
            public virtual IList<byte[]> GetCrls() {
                return crls;
            }

            /// <summary>Sets the ocsps array.</summary>
            /// <param name="ocsps">ocsps</param>
            public virtual void SetOcsps(IList<byte[]> ocsps) {
                this.ocsps = ocsps;
            }

            /// <summary>Retrieves ocsps byte array.</summary>
            /// <returns>ocsps</returns>
            public virtual IList<byte[]> GetOcsps() {
                return ocsps;
            }

            /// <summary>Sets the certs byte array.</summary>
            /// <param name="certs">certs</param>
            public virtual void SetCerts(IList<byte[]> certs) {
                this.certs = certs;
            }

            /// <summary>Retrieves cert byte array.</summary>
            /// <returns>cert</returns>
            public virtual IList<byte[]> GetCerts() {
                return certs;
            }
        }

        private IX509Certificate[] RetrieveMissingCertificates(IX509Certificate[] certChain) {
            IDictionary<String, IX509Certificate> restoredChain = new LinkedDictionary<String, IX509Certificate>();
            IX509Certificate[] subChain;
            foreach (IX509Certificate certificate in certChain) {
                subChain = issuingCertificateRetriever.RetrieveMissingCertificates(new IX509Certificate[] { certificate });
                foreach (IX509Certificate cert in subChain) {
                    restoredChain.Put(((IX509Certificate)cert).GetSubjectDN().ToString(), cert);
                }
            }
            return restoredChain.Values.ToArray(new IX509Certificate[0]);
        }
    }
}
