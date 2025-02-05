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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Crypto;

namespace iText.Signatures.Validation.Extensions {
    /// <summary>
    /// Class representing "Basic Constraints" certificate extension,
    /// which uses provided amount of certificates in chain during the comparison.
    /// </summary>
    public class DynamicBasicConstraintsExtension : DynamicCertificateExtension {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        public const String ERROR_MESSAGE = "Expected extension 2.5.29.19 to have a value of at least {0} but found {1}";

        private String errorMessage;

        /// <summary>
        /// Create new instance of
        /// <see cref="DynamicBasicConstraintsExtension"/>.
        /// </summary>
        public DynamicBasicConstraintsExtension()
            : base(OID.X509Extensions.BASIC_CONSTRAINTS, FACTORY.CreateBasicConstraints(true).ToASN1Primitive()) {
        }

        /// <summary>Check if this extension is present in the provided certificate.</summary>
        /// <remarks>
        /// Check if this extension is present in the provided certificate.
        /// In case of
        /// <see cref="DynamicBasicConstraintsExtension"/>
        /// , check if path length for this extension is less or equal
        /// to the path length, specified in the certificate.
        /// </remarks>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// in which this extension shall be present
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this path length is less or equal to a one from the certificate,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public override bool ExistsInCertificate(IX509Certificate certificate) {
            if (certificate.GetBasicConstraints() >= GetCertificateChainSize() - 1) {
                return true;
            }
            errorMessage = MessageFormatUtil.Format(ERROR_MESSAGE, GetCertificateChainSize() - 1, certificate.GetBasicConstraints
                ());
            return false;
        }

        public override String GetMessage() {
            return errorMessage;
        }
    }
}
