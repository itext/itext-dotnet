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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.Eutrustedlistsresources;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;

namespace iText.Signatures.Validation {
    /// <summary>
    /// Loads the certificates as published as part of the european journal publication "Information related to data on
    /// Member States' trusted lists as notified under Commission Implementing Decision (EU) 2015/1505"
    /// </summary>
    public class EuropeanTrustedCertificatesResourceLoader {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();


        private readonly EuropeanTrustedListConfiguration config;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="EuropeanTrustedCertificatesResourceLoader"/>
        /// </summary>
        public EuropeanTrustedCertificatesResourceLoader(EuropeanTrustedListConfiguration config) {
            // empty constructor
            this.config = config;
        }

        internal static void VerifyCertificate(string hashB64, IX509Certificate cert) {
            IMessageDigest digest = FACTORY.CreateIDigest("SHA-256");
            byte[] certHash = digest.Digest(cert.GetEncoded());
            if (hashB64 == null) {
                throw new PdfException(MessageFormatUtil.Format(SignExceptionMessageConstant.CERTIFICATE_HASH_NULL));
            }

            byte[] expectedHash = Convert.FromBase64String(hashB64);
            if (!certHash.SequenceEqual(expectedHash)) {
                throw new PdfException(MessageFormatUtil.Format(SignExceptionMessageConstant.CERTIFICATE_HASH_MISMATCH,
                    cert
                        .GetIssuerDN().ToString()));
            }
        }

        /// <summary>Loads the certificates from the European Trusted List configuration.</summary>
        /// <remarks>
        /// Loads the certificates from the European Trusted List configuration.
        /// And verifies if they match the expected SHA-256 hash.
        /// </remarks>
        /// <returns>A list of X509Certificates.</returns>
        public virtual IList<IX509Certificate> LoadCertificates() {
            List<IX509Certificate> result = new List<IX509Certificate>();
            foreach (var pemContainer in config
                         .GetCertificates()) {
                var pem = CleanPem(pemContainer.GetPemCertificate());
                IX509Certificate cert =
                    CertificateUtil.ReadCertificatesFromPem(new MemoryStream(pem.GetBytes(System.Text.Encoding.UTF8)))
                        [0];
                VerifyCertificate(pemContainer.GetHash(), cert);
                result.Add(cert);
            }

            return result;
        }

        private static string CleanPem(string pemContent) {
            const string header = "-----BEGIN CERTIFICATE-----";
            const string footer = "-----END CERTIFICATE-----";

            string base64 = pemContent
                .Replace(header, "")
                .Replace(footer, "")
                .Trim();

            base64 = Regex.Replace(base64, @"[^A-Za-z0-9+/=]", "");

            StringBuilder formatted = new StringBuilder();
            formatted.AppendLine(header);
            for (int i = 0; i < base64.Length; i += 64) {
                int chunkLength = Math.Min(64, base64.Length - i);
                formatted.AppendLine(base64.Substring(i, chunkLength));
            }

            formatted.AppendLine(footer);

            return formatted.ToString();
        }
    }
}