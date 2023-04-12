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
using Microsoft.Extensions.Logging;
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms;
using iText.IO.Font;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Signatures.Exceptions;

namespace iText.Signatures {
    /// <summary>Add verification according to PAdES-LTV (part 4).</summary>
    /// <author>Paulo Soares</author>
    public class LtvVerification {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.LtvVerification));

        private PdfDocument document;

        private SignatureUtil sgnUtil;

        private PdfAcroForm acroForm;

        private IDictionary<PdfName, LtvVerification.ValidationData> validated = new Dictionary<PdfName, LtvVerification.ValidationData
            >();

        private bool used = false;

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
            WHOLE_CHAIN
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
            this.acroForm = PdfAcroForm.GetAcroForm(document, true);
            this.sgnUtil = new SignatureUtil(document);
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
            IX509Certificate[] xc = pk.GetCertificates();
            IX509Certificate cert;
            IX509Certificate signingCert = pk.GetSigningCertificate();
            LtvVerification.ValidationData vd = new LtvVerification.ValidationData();
            foreach (IX509Certificate certificate in xc) {
                cert = (IX509Certificate)certificate;
                LOGGER.LogInformation(MessageFormatUtil.Format("Certificate: {0}", BOUNCY_CASTLE_FACTORY.CreateX500Name(cert
                    )));
                if (certOption == LtvVerification.CertificateOption.SIGNING_CERTIFICATE && !cert.Equals(signingCert)) {
                    continue;
                }
                byte[] ocspEnc = null;
                if (ocsp != null && level != LtvVerification.Level.CRL) {
                    ocspEnc = ocsp.GetEncoded(cert, GetParent(cert, xc), null);
                    if (ocspEnc != null) {
                        vd.ocsps.Add(BuildOCSPResponse(ocspEnc));
                        LOGGER.LogInformation("OCSP added");
                    }
                }
                if (crl != null && (level == LtvVerification.Level.CRL || level == LtvVerification.Level.OCSP_CRL || (level
                     == LtvVerification.Level.OCSP_OPTIONAL_CRL && ocspEnc == null))) {
                    ICollection<byte[]> cims = crl.GetEncoded(cert, null);
                    if (cims != null) {
                        foreach (byte[] cim in cims) {
                            bool dup = false;
                            foreach (byte[] b in vd.crls) {
                                if (JavaUtil.ArraysEquals(b, cim)) {
                                    dup = true;
                                    break;
                                }
                            }
                            if (!dup) {
                                vd.crls.Add(cim);
                                LOGGER.LogInformation("CRL added");
                            }
                        }
                    }
                }
                if (certInclude == LtvVerification.CertificateInclusion.YES) {
                    vd.certs.Add(cert.GetEncoded());
                }
            }
            if (vd.crls.Count == 0 && vd.ocsps.Count == 0) {
                return false;
            }
            validated.Put(GetSignatureHashKey(signatureName), vd);
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
                    vd.ocsps.Add(BuildOCSPResponse(ocsp));
                }
            }
            if (crls != null) {
                vd.crls.AddAll(crls);
            }
            if (certs != null) {
                vd.certs.AddAll(certs);
            }
            validated.Put(GetSignatureHashKey(signatureName), vd);
            return true;
        }

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
            byte[] bt = null;
            if (PdfName.ETSI_RFC3161.Equals(sig.GetSubFilter())) {
                using (IAsn1InputStream din = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(bc))) {
                    IAsn1Object pkcs = din.ReadObject();
                    bc = pkcs.GetEncoded();
                }
            }
            bt = HashBytesSha1(bc);
            return new PdfName(ConvertToHex(bt));
        }

        private static byte[] HashBytesSha1(byte[] b) {
            IDigest sh = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("SHA1");
            return sh.Digest(b);
        }

        /// <summary>Merges the validation with any validation already in the document or creates a new one.</summary>
        public virtual void Merge() {
            if (used || validated.Count == 0) {
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
                foreach (byte[] b in validated.Get(vkey).crls) {
                    PdfStream ps = new PdfStream(b);
                    ps.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                    ps.MakeIndirect(document);
                    crl.Add(ps);
                    crls.Add(ps);
                    crls.SetModified();
                }
                foreach (byte[] b in validated.Get(vkey).ocsps) {
                    PdfStream ps = new PdfStream(b);
                    ps.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                    ocsp.Add(ps);
                    ocsps.Add(ps);
                    ocsps.SetModified();
                }
                foreach (byte[] b in validated.Get(vkey).certs) {
                    PdfStream ps = new PdfStream(b);
                    ps.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                    ps.MakeIndirect(document);
                    cert.Add(ps);
                    certs.Add(ps);
                    certs.SetModified();
                }
                if (ocsp.Size() > 0) {
                    ocsp.MakeIndirect(document);
                    vri.Put(PdfName.OCSP, ocsp);
                }
                if (crl.Size() > 0) {
                    crl.MakeIndirect(document);
                    vri.Put(PdfName.CRL, crl);
                }
                if (cert.Size() > 0) {
                    cert.MakeIndirect(document);
                    vri.Put(PdfName.Cert, cert);
                }
                vri.MakeIndirect(document);
                vrim.Put(vkey, vri);
            }
            vrim.MakeIndirect(document);
            vrim.SetModified();
            dss.Put(PdfName.VRI, vrim);
            if (ocsps.Size() > 0) {
                ocsps.MakeIndirect(document);
                dss.Put(PdfName.OCSPs, ocsps);
            }
            if (crls.Size() > 0) {
                crls.MakeIndirect(document);
                dss.Put(PdfName.CRLs, crls);
            }
            if (certs.Size() > 0) {
                certs.MakeIndirect(document);
                dss.Put(PdfName.Certs, certs);
            }
            dss.MakeIndirect(document);
            dss.SetModified();
            catalog.Put(PdfName.DSS, dss);
        }

        private class ValidationData {
            public IList<byte[]> crls = new List<byte[]>();

            public IList<byte[]> ocsps = new List<byte[]>();

            public IList<byte[]> certs = new List<byte[]>();
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
    }
}
