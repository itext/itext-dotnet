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
using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.TbsCertificateStructure"/>.
    /// </summary>
    public class TBSCertificateBC : ASN1EncodableBC, ITBSCertificate {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.TbsCertificateStructure"/>.
        /// </summary>
        /// <param name="tbsCertificate">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.TbsCertificateStructure"/>
        /// to be wrapped
        /// </param>
        public TBSCertificateBC(TbsCertificateStructure tbsCertificate)
            : base(tbsCertificate) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.TbsCertificateStructure"/>.
        /// </returns>
        public virtual TbsCertificateStructure GetTBSCertificate() {
            return (TbsCertificateStructure)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISubjectPublicKeyInfo GetSubjectPublicKeyInfo() {
            return new SubjectPublicKeyInfoBC(GetTBSCertificate().SubjectPublicKeyInfo);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX500Name GetIssuer() {
            return new X500NameBC(GetTBSCertificate().Issuer);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Integer GetSerialNumber() {
            return new ASN1IntegerBC(GetTBSCertificate().SerialNumber);
        }
    }
}
