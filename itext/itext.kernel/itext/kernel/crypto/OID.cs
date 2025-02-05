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
using iText.Commons.Utils;

namespace iText.Kernel.Crypto {
    /// <summary>Class containing all the OID values used by iText.</summary>
    public sealed class OID {
        public const String PKCS7_DATA = "1.2.840.113549.1.7.1";

        public const String PKCS7_SIGNED_DATA = "1.2.840.113549.1.7.2";

        public const String RSA = "1.2.840.113549.1.1.1";

        public const String RSASSA_PSS = "1.2.840.113549.1.1.10";

        public const String RSA_WITH_SHA256 = "1.2.840.113549.1.1.11";

        public const String AA_SIGNING_CERTIFICATE_V1 = "1.2.840.113549.1.9.16.2.12";

        public const String AA_SIGNING_CERTIFICATE_V2 = "1.2.840.113549.1.9.16.2.47";

        public const String MGF1 = "1.2.840.113549.1.1.8";

        public const String AA_TIME_STAMP_TOKEN = "1.2.840.113549.1.9.16.2.14";

        public const String AUTHENTICATED_DATA = "1.2.840.113549.1.9.16.1.2";

        public const String CONTENT_TYPE = "1.2.840.113549.1.9.3";

        public const String MESSAGE_DIGEST = "1.2.840.113549.1.9.4";

        public const String SIGNING_TIME = "1.2.840.113549.1.9.5";

        public const String CMS_ALGORITHM_PROTECTION = "1.2.840.113549.1.9.52";

        public const String DSA = "1.2.840.10040.4.1";

        public const String ECDSA = "1.2.840.10045.2.1";

        public const String ADBE_REVOCATION = "1.2.840.113583.1.1.8";

        public const String TSA = "1.2.840.113583.1.1.9.1";

        public const String RSA_WITH_SHA3_512 = "2.16.840.1.101.3.4.3.16";

        public const String SHA_224 = "2.16.840.1.101.3.4.2.4";

        public const String SHA_256 = "2.16.840.1.101.3.4.2.1";

        public const String SHA_384 = "2.16.840.1.101.3.4.2.2";

        public const String SHA_512 = "2.16.840.1.101.3.4.2.3";

        public const String SHA3_224 = "2.16.840.1.101.3.4.2.7";

        public const String SHA3_256 = "2.16.840.1.101.3.4.2.8";

        public const String SHA3_384 = "2.16.840.1.101.3.4.2.9";

        public const String SHA3_512 = "2.16.840.1.101.3.4.2.10";

        public const String SHAKE_256 = "2.16.840.1.101.3.4.2.12";

        public const String ED25519 = "1.3.101.112";

        public const String ED448 = "1.3.101.113";

        public const String OCSP = "1.3.6.1.5.5.7.48.1";

        public const String CA_ISSUERS = "1.3.6.1.5.5.7.48.2";

        public const String RI_OCSP_RESPONSE = "1.3.6.1.5.5.7.16.2";

        public const String KDF_PDF_MAC_WRAP_KDF = "1.0.32004.1.1";

        public const String CT_PDF_MAC_INTEGRITY_INFO = "1.0.32004.1.0";

        private OID() {
        }

        // Empty on purpose. Avoiding instantiation of this class.
        /// <summary>Contains all OIDs used by iText in the context of Certificate Extensions.</summary>
        public sealed class X509Extensions {
            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            /// <remarks>
            /// One of the standard extensions from https://tools.ietf.org/html/rfc5280
            /// <para />
            /// "Conforming CAs MUST mark this extension as non-critical."
            /// </remarks>
            public const String AUTHORITY_KEY_IDENTIFIER = "2.5.29.35";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            /// <remarks>
            /// One of the standard extensions from https://tools.ietf.org/html/rfc5280
            /// <para />
            /// "Conforming CAs MUST mark this extension as non-critical."
            /// </remarks>
            public const String SUBJECT_KEY_IDENTIFIER = "2.5.29.14";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            public const String KEY_USAGE = "2.5.29.15";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            public const String CERTIFICATE_POLICIES = "2.5.29.32";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            public const String POLICY_MAPPINGS = "2.5.29.33";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            public const String SUBJECT_ALTERNATIVE_NAME = "2.5.29.17";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            public const String ISSUER_ALTERNATIVE_NAME = "2.5.29.18";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            /// <remarks>
            /// One of the standard extensions from https://tools.ietf.org/html/rfc5280
            /// <para />
            /// "Conforming CAs MUST mark this extension as non-critical."
            /// </remarks>
            public const String SUBJECT_DIRECTORY_ATTRIBUTES = "2.5.29.9";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            public const String BASIC_CONSTRAINTS = "2.5.29.19";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            public const String NAME_CONSTRAINTS = "2.5.29.30";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            public const String POLICY_CONSTRAINTS = "2.5.29.36";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            public const String EXTENDED_KEY_USAGE = "2.5.29.37";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            public const String CRL_DISTRIBUTION_POINTS = "2.5.29.31";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            public const String INHIBIT_ANY_POLICY = "2.5.29.54";

            /// <summary>One of the standard extensions from https://tools.ietf.org/html/rfc5280</summary>
            /// <remarks>
            /// One of the standard extensions from https://tools.ietf.org/html/rfc5280
            /// <para />
            /// "The extension MUST be marked as non-critical by conforming CAs."
            /// </remarks>
            public const String FRESHEST_CRL = "2.5.29.46";

            /// <summary>One of the Internet Certificate Extensions also from https://tools.ietf.org/html/rfc5280</summary>
            /// <remarks>
            /// One of the Internet Certificate Extensions also from https://tools.ietf.org/html/rfc5280
            /// <para />
            /// "The extension MUST be marked as non-critical by conforming CAs."
            /// </remarks>
            public const String AUTHORITY_INFO_ACCESS = "1.3.6.1.5.5.7.1.1";

            /// <summary>One of the Internet Certificate Extensions also from https://tools.ietf.org/html/rfc5280</summary>
            /// <remarks>
            /// One of the Internet Certificate Extensions also from https://tools.ietf.org/html/rfc5280
            /// <para />
            /// "Conforming CAs MUST mark this extension as non-critical."
            /// </remarks>
            public const String SUBJECT_INFO_ACCESS = "1.3.6.1.5.5.7.1.11";

            /// <summary>
            /// One of the
            /// <see cref="EXTENDED_KEY_USAGE"/>
            /// purposes from https://www.ietf.org/rfc/rfc2459.txt
            /// </summary>
            public const String ID_KP_TIMESTAMPING = "1.3.6.1.5.5.7.3.8";

            /// <summary>
            /// Extension for OCSP responder certificate
            /// from https://www.ietf.org/rfc/rfc2560.txt.
            /// </summary>
            public const String ID_PKIX_OCSP_NOCHECK = "1.3.6.1.5.5.7.48.1.5";

            /// <summary>Extension for certificates from ETSI EN 319 412-1 V1.4.4.</summary>
            public const String VALIDITY_ASSURED_SHORT_TERM = "0.4.0.194121.2.1";

            /// <summary>Extension for certificates from RFC 9608 which indicates that no revocation information is available.
            ///     </summary>
            public const String NO_REV_AVAILABLE = "2.5.29.56";

            /// <summary>According to https://tools.ietf.org/html/rfc5280 4.2.</summary>
            /// <remarks>
            /// According to https://tools.ietf.org/html/rfc5280 4.2. "Certificate Extensions":
            /// "A certificate-using system MUST reject the certificate if it encounters a critical extension it
            /// does not recognize or a critical extension that contains information that it cannot process."
            /// <para />
            /// This set consists of standard extensions which are defined in RFC specifications and are not mentioned
            /// as forbidden to be marked as critical.
            /// </remarks>
            public static readonly ICollection<String> SUPPORTED_CRITICAL_EXTENSIONS = JavaCollectionsUtil.UnmodifiableSet
                (new LinkedHashSet<String>(JavaUtil.ArraysAsList(KEY_USAGE, CERTIFICATE_POLICIES, POLICY_MAPPINGS, SUBJECT_ALTERNATIVE_NAME
                , ISSUER_ALTERNATIVE_NAME, BASIC_CONSTRAINTS, NAME_CONSTRAINTS, POLICY_CONSTRAINTS, EXTENDED_KEY_USAGE
                , CRL_DISTRIBUTION_POINTS, INHIBIT_ANY_POLICY, ID_PKIX_OCSP_NOCHECK)));
        }
    }
}
