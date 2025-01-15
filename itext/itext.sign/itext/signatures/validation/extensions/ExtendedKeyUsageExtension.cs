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
using System.Text;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Security;
using iText.Kernel.Crypto;
using Microsoft.Extensions.Primitives;

namespace iText.Signatures.Validation.Extensions {
    /// <summary>Class representing "Extended Key Usage" extension.</summary>
    public class ExtendedKeyUsageExtension : CertificateExtension {
        public const String ANY_EXTENDED_KEY_USAGE_OID = "2.5.29.37.0";

        public const String TIME_STAMPING = "1.3.6.1.5.5.7.3.8";

        public const String OCSP_SIGNING = "1.3.6.1.5.5.7.3.9";

        public const String CODE_SIGNING = "1.3.6.1.5.5.7.3.3";

        public const String CLIENT_AUTH = "1.3.6.1.5.5.7.3.2";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        public const String EXPECTED_KEY_USAGES = "Expected extended key usages:";
        public const String ACTUAL = "But found :";
        public const String NO_EXTENDED_KEY_USAGES_WERE_FOUND = " But no extended key usages were found.";
        public const String ERROR_OCCURRED_DURING_RETRIEVAL = " But error occurred during retrieval ";


        private readonly IList<String> extendedKeyUsageOids;

        private String errorMessage = "";

        /// <summary>
        /// Create new
        /// <see cref="ExtendedKeyUsageExtension"/>
        /// instance.
        /// </summary>
        /// <param name="extendedKeyUsageOids">
        /// 
        /// <see>List<string></see>
        /// , representing extended key usages OIDs
        /// </param>
        public ExtendedKeyUsageExtension(IList<String> extendedKeyUsageOids)
            : base(OID.X509Extensions.EXTENDED_KEY_USAGE, FACTORY.CreateExtendedKeyUsage(CreateKeyPurposeIds(extendedKeyUsageOids
                )).ToASN1Primitive()) {
            this.extendedKeyUsageOids = extendedKeyUsageOids;
        }

        /// <summary>Check if this extension is present in the provided certificate.</summary>
        /// <remarks>
        /// Check if this extension is present in the provided certificate. In case of
        /// <see cref="ExtendedKeyUsageExtension"/>
        /// ,
        /// check if this extended key usage OIDs are present. Other values may be present as well.
        /// </remarks>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// in which this extension shall be present
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if all OIDs are present in certificate extension,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public override bool ExistsInCertificate(IX509Certificate certificate) {
            IList<String> providedExtendedKeyUsage = new List<string>();
            try
            {
                if (certificate.GetExtendedKeyUsage() == null)
                {
                    errorMessage = NO_EXTENDED_KEY_USAGES_WERE_FOUND;
                    return false;
                }

                foreach (string singleExtendedKeyUsage in certificate.GetExtendedKeyUsage())
                {
                    providedExtendedKeyUsage.Add(singleExtendedKeyUsage);
                }
            }
            catch (Exception e) {
                errorMessage = ERROR_OCCURRED_DURING_RETRIEVAL + e.GetType().Name + " " + e.Message;
                return false;
            }
            if (providedExtendedKeyUsage.Contains(ANY_EXTENDED_KEY_USAGE_OID) || new HashSet<String>(providedExtendedKeyUsage
                ).ContainsAll(extendedKeyUsageOids)) {
                return true;
            }
            StringBuilder sb = new StringBuilder(ACTUAL);
            char sep = '(';
            foreach (String usage in providedExtendedKeyUsage)
            {
                sb.Append(sep).Append(usage);
                sep = ',';
            }
            sb.Append(')');
            errorMessage = sb.ToString();
            return false;
        }

        private static IDerObjectIdentifier[] CreateKeyPurposeIds(IList<String> extendedKeyUsageOids) {
            IDerObjectIdentifier[] keyPurposeIds = new IDerObjectIdentifier[extendedKeyUsageOids.Count];
            for (int i = 0; i < extendedKeyUsageOids.Count; ++i) {
                keyPurposeIds[i] = FACTORY.CreateASN1ObjectIdentifier(extendedKeyUsageOids[i]);
            }
            return keyPurposeIds;
        }

        
        public override String GetMessage()
        {
            StringBuilder sb = new StringBuilder(EXPECTED_KEY_USAGES);
            char sep = '(';
            foreach (String usage in extendedKeyUsageOids)
            {
                sb.Append(sep).Append(usage);
                sep = ',';
            }
            sb.Append(')');
            sb.Append(errorMessage);
            return sb.ToString();
        }
    }
}
