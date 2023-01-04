/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ess;

namespace iText.Bouncycastle.Asn1.Ess {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificateV2"/>.
    /// </summary>
    public class SigningCertificateV2BC : ASN1EncodableBC, ISigningCertificateV2 {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificateV2"/>.
        /// </summary>
        /// <param name="signingCertificateV2">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificateV2"/>
        /// to be wrapped
        /// </param>
        public SigningCertificateV2BC(SigningCertificateV2 signingCertificateV2)
            : base(signingCertificateV2) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificateV2"/>.
        /// </returns>
        public virtual SigningCertificateV2 GetSigningCertificateV2() {
            return (SigningCertificateV2)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IESSCertIDv2[] GetCerts() {
            EssCertIDv2[] certs = GetSigningCertificateV2().GetCerts();
            IESSCertIDv2[] certsBC = new IESSCertIDv2[certs.Length];
            for (int i = 0; i < certsBC.Length; i++) {
                certsBC[i] = new ESSCertIDv2BC(certs[i]);
            }
            return certsBC;
        }
    }
}
