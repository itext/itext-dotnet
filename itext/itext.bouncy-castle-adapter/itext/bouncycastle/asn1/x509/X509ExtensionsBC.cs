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
    public class X509ExtensionsBC : Asn1EncodableBC, IX509Extensions {
        private static readonly iText.Bouncycastle.Asn1.X509.X509ExtensionsBC INSTANCE = new iText.Bouncycastle.Asn1.X509.X509ExtensionsBC
            (null);

        private static readonly DerObjectIdentifierBC C_RL_DISTRIBUTION_POINTS = new DerObjectIdentifierBC(X509Extensions.CrlDistributionPoints
            );

        private static readonly DerObjectIdentifierBC AUTHORITY_INFO_ACCESS = new DerObjectIdentifierBC(X509Extensions.AuthorityInfoAccess
            );

        private static readonly DerObjectIdentifierBC BASIC_CONSTRAINTS = new DerObjectIdentifierBC(X509Extensions.BasicConstraints
            );

        private static readonly DerObjectIdentifierBC KEY_USAGE = new DerObjectIdentifierBC(X509Extensions.KeyUsage
            );

        private static readonly DerObjectIdentifierBC EXTENDED_KEY_USAGE = new DerObjectIdentifierBC(X509Extensions.ExtendedKeyUsage
            );

        private static readonly DerObjectIdentifierBC AUTHORITY_KEY_IDENTIFIER = new DerObjectIdentifierBC(X509Extensions.AuthorityKeyIdentifier
            );

        private static readonly DerObjectIdentifierBC SUBJECT_KEY_IDENTIFIER = new DerObjectIdentifierBC(X509Extensions.SubjectKeyIdentifier
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
        public X509ExtensionsBC(X509Extensions extension)
            : base(extension) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="X509ExtensionsBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Asn1.X509.X509ExtensionsBC GetInstance() {
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
        public virtual IDerObjectIdentifier GetCRlDistributionPoints() {
            return C_RL_DISTRIBUTION_POINTS;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetAuthorityInfoAccess() {
            return AUTHORITY_INFO_ACCESS;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetBasicConstraints() {
            return BASIC_CONSTRAINTS;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetKeyUsage() {
            return KEY_USAGE;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetExtendedKeyUsage() {
            return EXTENDED_KEY_USAGE;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetAuthorityKeyIdentifier() {
            return AUTHORITY_KEY_IDENTIFIER;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetSubjectKeyIdentifier() {
            return SUBJECT_KEY_IDENTIFIER;
        }
    }
}
