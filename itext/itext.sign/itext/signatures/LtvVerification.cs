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
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using iText.Commons;
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
            X509Certificate[] xc = pk.GetCertificates();
            X509Certificate cert;
            X509Certificate signingCert = pk.GetSigningCertificate();
            LtvVerification.ValidationData vd = new LtvVerification.ValidationData();
            for (int k = 0; k < xc.Length; ++k) {
                cert = (X509Certificate)xc[k];
                LOGGER.LogInformation("Certificate: " + cert.SubjectDN);
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

        /// <summary>Get the issuing certificate for a child certificate.</summary>
        /// <param name="cert">the certificate for which we search the parent</param>
        /// <param name="certs">an array with certificates that contains the parent</param>
        /// <returns>the parent certificate</returns>
        private X509Certificate GetParent(X509Certificate cert, X509Certificate[] certs) {
            X509Certificate parent;
            for (int i = 0; i < certs.Length; i++) {
                parent = (X509Certificate)certs[i];
                if (!cert.IssuerDN.Equals(parent.SubjectDN)) {
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
                foreach (byte[] crl in crls) {
                    vd.crls.Add(crl);
                }
            }
            if (certs != null) {
                foreach (byte[] cert in certs) {
                    vd.certs.Add(cert);
                }
            }
            validated.Put(GetSignatureHashKey(signatureName), vd);
            return true;
        }

        private static byte[] BuildOCSPResponse(byte[] basicOcspResponse) {
            DerOctetString doctet = new DerOctetString(basicOcspResponse);
            OcspResponseStatus respStatus = new OcspResponseStatus(Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful
                );
            ResponseBytes responseBytes = new ResponseBytes(OcspObjectIdentifiers.PkixOcspBasic, doctet);
            OcspResponse ocspResponse = new OcspResponse(respStatus, responseBytes);
            return new OcspResp(ocspResponse).GetEncoded();
        }

        private PdfName GetSignatureHashKey(String signatureName) {
            PdfSignature sig = sgnUtil.GetSignature(signatureName);
            PdfString contents = sig.GetContents();
            byte[] bc = PdfEncodings.ConvertToBytes(contents.GetValue(), null);
            byte[] bt = null;
            if (PdfName.ETSI_RFC3161.Equals(sig.GetSubFilter())) {
                Asn1InputStream din = new Asn1InputStream(new MemoryStream(bc));
                Asn1Object pkcs = din.ReadObject();
                bc = pkcs.GetEncoded();
            }
            bt = HashBytesSha1(bc);
            return new PdfName(ConvertToHex(bt));
        }

        private static byte[] HashBytesSha1(byte[] b) {
            IDigest sh = DigestUtilities.GetDigest("SHA1");
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

        // TODO: Refactor. Copied from itext5 Utilities
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
