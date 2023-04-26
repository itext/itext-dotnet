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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using iText.Signatures.Exceptions;
using iText.Signatures.Logs;

namespace iText.Signatures {
    /// <summary>This class consists of some methods that allow you to verify certificates.</summary>
    public class CertificateVerification {
        public const String HAS_UNSUPPORTED_EXTENSIONS = "Has unsupported critical extension";

        public const String CERTIFICATE_REVOKED = "Certificate revoked";

        /// <summary>The Logger instance.</summary>
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(CertificateVerification));

        /// <summary>Verifies a single certificate for the current date.</summary>
        /// <param name="cert">the certificate to verify</param>
        /// <param name="crls">the certificate revocation list or <c>null</c></param>
        /// <returns>
        /// a <c>String</c> with the error description or <c>null</c>
        /// if no error
        /// </returns>
        public static String VerifyCertificate(IX509Certificate cert, ICollection<IX509Crl> crls) {
            return VerifyCertificate(cert, crls, DateTimeUtil.GetCurrentTime());
        }

        /// <summary>Verifies a single certificate.</summary>
        /// <param name="cert">the certificate to verify</param>
        /// <param name="crls">the certificate revocation list or <c>null</c></param>
        /// <param name="calendar">the date, shall not be null</param>
        /// <returns>
        /// a <c>String</c> with the error description or <c>null</c>
        /// if no error
        /// </returns>
        public static String VerifyCertificate(IX509Certificate cert, ICollection<IX509Crl> crls, DateTime calendar
            ) {
            if (SignUtils.HasUnsupportedCriticalExtension(cert)) {
                return CertificateVerification.HAS_UNSUPPORTED_EXTENSIONS;
            }
            try {
                cert.CheckValidity(calendar.ToUniversalTime());
            }
            catch (Exception e) {
                return e.Message;
            }
            if (crls != null) {
                foreach (IX509Crl crl in crls) {
                    if (crl.IsRevoked(cert)) {
                        return CertificateVerification.CERTIFICATE_REVOKED;
                    }
                }
            }
            return null;
        }

        /// <summary>Verifies a certificate chain against a KeyStore for the current date.</summary>
        /// <param name="certs">the certificate chain</param>
        /// <param name="keystore">the <c>KeyStore</c></param>
        /// <param name="crls">the certificate revocation list or <c>null</c></param>
        /// <returns>
        /// empty list if the certificate chain could be validated or a
        /// <c>Object[]{cert,error}</c> where <c>cert</c> is the
        /// failed certificate and <c>error</c> is the error message
        /// </returns>
        public static IList<VerificationException> VerifyCertificates(IX509Certificate[] certs, List<IX509Certificate>
             keystore, ICollection<IX509Crl> crls) {
            return VerifyCertificates(certs, keystore, crls, DateTimeUtil.GetCurrentTime());
        }

        /// <summary>Verifies a certificate chain against a KeyStore.</summary>
        /// <param name="certs">the certificate chain</param>
        /// <param name="keystore">the <c>KeyStore</c></param>
        /// <param name="crls">the certificate revocation list or <c>null</c></param>
        /// <param name="calendar">the date, shall not be null</param>
        /// <returns>
        /// empty list if the certificate chain could be validated or a
        /// <c>Object[]{cert,error}</c> where <c>cert</c> is the
        /// failed certificate and <c>error</c> is the error message
        /// </returns>
        public static IList<VerificationException> VerifyCertificates(IX509Certificate[] certs, List<IX509Certificate>
             keystore, ICollection<IX509Crl> crls, DateTime calendar) {
            IList<VerificationException> result = new List<VerificationException>();
            for (int k = 0; k < certs.Length; ++k) {
                IX509Certificate cert = (IX509Certificate)certs[k];
                String err = VerifyCertificate(cert, crls, calendar);
                if (err != null) {
                    result.Add(new VerificationException(cert, err));
                }
                try {
                    foreach (IX509Certificate certStoreX509 in SignUtils.GetCertificates(keystore)) {
                        try {
                            if (VerifyCertificate(certStoreX509, crls, calendar) != null) {
                                continue;
                            }
                            try {
                                cert.Verify(certStoreX509.GetPublicKey());
                                return result;
                            }
                            catch (Exception) {
                            }
                        }
                        catch (Exception) {
                        }
                    }
                }
                catch (Exception) {
                }
                // do nothing and continue
                // Do nothing.
                // Do nothing.
                int j;
                for (j = 0; j < certs.Length; ++j) {
                    if (j == k) {
                        continue;
                    }
                    IX509Certificate certNext = (IX509Certificate)certs[j];
                    try {
                        cert.Verify(certNext.GetPublicKey());
                        break;
                    }
                    catch (Exception) {
                    }
                }
                // Do nothing.
                if (j == certs.Length) {
                    result.Add(new VerificationException(cert, SignExceptionMessageConstant.CANNOT_BE_VERIFIED_CERTIFICATE_CHAIN
                        ));
                }
            }
            if (result.Count == 0) {
                result.Add(new VerificationException((IX509Certificate)null, SignExceptionMessageConstant.INVALID_STATE_WHILE_CHECKING_CERT_CHAIN
                    ));
            }
            return result;
        }

        /// <summary>Verifies a certificate chain against a KeyStore for the current date.</summary>
        /// <param name="certs">the certificate chain</param>
        /// <param name="keystore">the <c>KeyStore</c></param>
        /// <returns>
        /// <c>null</c> if the certificate chain could be validated or a
        /// <c>Object[]{cert,error}</c> where <c>cert</c> is the
        /// failed certificate and <c>error</c> is the error message
        /// </returns>
        public static IList<VerificationException> VerifyCertificates(IX509Certificate[] certs, List<IX509Certificate>
             keystore) {
            return VerifyCertificates(certs, keystore, DateTimeUtil.GetCurrentTime());
        }

        /// <summary>Verifies a certificate chain against a KeyStore.</summary>
        /// <param name="certs">the certificate chain</param>
        /// <param name="keystore">the <c>KeyStore</c></param>
        /// <param name="calendar">the date, shall not be null</param>
        /// <returns>
        /// <c>null</c> if the certificate chain could be validated or a
        /// <c>Object[]{cert,error}</c> where <c>cert</c> is the
        /// failed certificate and <c>error</c> is the error message
        /// </returns>
        public static IList<VerificationException> VerifyCertificates(IX509Certificate[] certs, List<IX509Certificate>
             keystore, DateTime calendar) {
            return VerifyCertificates(certs, keystore, null, calendar);
        }

        /// <summary>Verifies an OCSP response against a KeyStore.</summary>
        /// <param name="ocsp">the OCSP response</param>
        /// <param name="keystore">the <c>KeyStore</c></param>
        /// <returns><c>true</c> is a certificate was found</returns>
        public static bool VerifyOcspCertificates(IBasicOcspResponse ocsp, List<IX509Certificate> keystore) {
            IList<Exception> exceptionsThrown = new List<Exception>();
            try {
                foreach (IX509Certificate certStoreX509 in SignUtils.GetCertificates(keystore)) {
                    try {
                        if (SignUtils.IsSignatureValid(ocsp, certStoreX509)) {
                            return true;
                        }
                    }
                    catch (Exception ex) {
                        exceptionsThrown.Add(ex);
                    }
                }
            }
            catch (Exception e) {
                exceptionsThrown.Add(e);
            }
            LogExceptionMessages(exceptionsThrown);
            return false;
        }

        /// <summary>Verifies a time stamp against a KeyStore.</summary>
        /// <param name="ts">the time stamp</param>
        /// <param name="keystore">the <c>KeyStore</c></param>
        /// <returns><c>true</c> is a certificate was found</returns>
        public static bool VerifyTimestampCertificates(ITimeStampToken ts, List<IX509Certificate> keystore) {
            IList<Exception> exceptionsThrown = new List<Exception>();
            try {
                foreach (IX509Certificate certStoreX509 in SignUtils.GetCertificates(keystore)) {
                    try {
                        SignUtils.IsSignatureValid(ts, certStoreX509);
                        return true;
                    }
                    catch (Exception ex) {
                        exceptionsThrown.Add(ex);
                    }
                }
            }
            catch (Exception e) {
                exceptionsThrown.Add(e);
            }
            LogExceptionMessages(exceptionsThrown);
            return false;
        }

        private static void LogExceptionMessages(IList<Exception> exceptionsThrown) {
            foreach (Exception ex in exceptionsThrown) {
                LOGGER.LogError(ex, ex.Message == null ? SignLogMessageConstant.EXCEPTION_WITHOUT_MESSAGE : ex.Message);
            }
        }
    }
}
