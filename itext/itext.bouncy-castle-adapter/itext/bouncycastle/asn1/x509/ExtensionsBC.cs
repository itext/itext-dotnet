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
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.X509Extensions"/>.
    /// </summary>
    public class ExtensionsBC : ASN1EncodableBC, IExtensions {
        private static readonly iText.Bouncycastle.Asn1.X509.ExtensionsBC INSTANCE = new iText.Bouncycastle.Asn1.X509.ExtensionsBC
            (null);

        private static readonly ASN1ObjectIdentifierBC C_RL_DISTRIBUTION_POINTS = new ASN1ObjectIdentifierBC(X509Extensions.CrlDistributionPoints
            );

        private static readonly ASN1ObjectIdentifierBC AUTHORITY_INFO_ACCESS = new ASN1ObjectIdentifierBC(X509Extensions.AuthorityInfoAccess
            );

        private static readonly ASN1ObjectIdentifierBC BASIC_CONSTRAINTS = new ASN1ObjectIdentifierBC(X509Extensions.BasicConstraints
            );

        private static readonly ASN1ObjectIdentifierBC KEY_USAGE = new ASN1ObjectIdentifierBC(X509Extensions.KeyUsage
            );

        private static readonly ASN1ObjectIdentifierBC EXTENDED_KEY_USAGE = new ASN1ObjectIdentifierBC(X509Extensions.ExtendedKeyUsage
            );

        private static readonly ASN1ObjectIdentifierBC AUTHORITY_KEY_IDENTIFIER = new ASN1ObjectIdentifierBC(X509Extensions.AuthorityKeyIdentifier
            );

        private static readonly ASN1ObjectIdentifierBC SUBJECT_KEY_IDENTIFIER = new ASN1ObjectIdentifierBC(X509Extensions.SubjectKeyIdentifier
            );

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.X509Extensions"/>.
        /// </summary>
        /// <param name="extension">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.X509Extensions"/>
        /// to be wrapped
        /// </param>
        public ExtensionsBC(X509Extensions extension)
            : base(extension) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="ExtensionsBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Asn1.X509.ExtensionsBC GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.X509Extensions"/>.
        /// </returns>
        public virtual X509Extensions GetX509Extensions() {
            return (X509Extensions)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetCRlDistributionPoints() {
            return C_RL_DISTRIBUTION_POINTS;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetAuthorityInfoAccess() {
            return AUTHORITY_INFO_ACCESS;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetBasicConstraints() {
            return BASIC_CONSTRAINTS;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetKeyUsage() {
            return KEY_USAGE;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetExtendedKeyUsage() {
            return EXTENDED_KEY_USAGE;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetAuthorityKeyIdentifier() {
            return AUTHORITY_KEY_IDENTIFIER;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetSubjectKeyIdentifier() {
            return SUBJECT_KEY_IDENTIFIER;
        }
    }
}
