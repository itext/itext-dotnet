/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;

namespace iText.Signatures.Validation.V1.Extensions {
    /// <summary>
    /// Class representing "Basic Constraints" certificate extension,
    /// which uses provided amount of certificates in chain during the comparison.
    /// </summary>
    public class DynamicBasicConstraintsExtension : DynamicCertificateExtension {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

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
            try {
                if (CertificateUtil.GetExtensionValue(certificate, OID.X509Extensions.BASIC_CONSTRAINTS) == null) {
                    return false;
                }
            }
            catch (System.IO.IOException) {
                return false;
            }
            return certificate.GetBasicConstraints() >= GetCertificateChainSize();
        }
    }
}
