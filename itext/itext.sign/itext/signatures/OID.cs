/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Commons.Utils;

namespace iText.Signatures {
    /// <summary>Class containing all the OID values used by iText.</summary>
    public sealed class OID {
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
