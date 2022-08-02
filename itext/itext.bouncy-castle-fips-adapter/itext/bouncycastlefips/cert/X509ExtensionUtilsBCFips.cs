/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System;
using Org.BouncyCastle.Cert;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.X509ExtensionUtils"/>.
    /// </summary>
    public class X509ExtensionUtilsBCFips : IX509ExtensionUtils {
        private readonly X509ExtensionUtils extensionUtils;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.X509ExtensionUtils"/>.
        /// </summary>
        /// <param name="extensionUtils">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.X509ExtensionUtils"/>
        /// to be wrapped
        /// </param>
        public X509ExtensionUtilsBCFips(X509ExtensionUtils extensionUtils) {
            this.extensionUtils = extensionUtils;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.X509ExtensionUtils"/>.
        /// </summary>
        /// <param name="digestCalculator">
        /// DigestCalculator wrapper to create
        /// <see cref="Org.BouncyCastle.Cert.X509ExtensionUtils"/>
        /// </param>
        public X509ExtensionUtilsBCFips(IDigestCalculator digestCalculator)
            : this(new X509ExtensionUtils(((DigestCalculatorBCFips)digestCalculator).GetDigestCalculator())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.X509ExtensionUtils"/>.
        /// </returns>
        public virtual X509ExtensionUtils GetExtensionUtils() {
            return extensionUtils;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAuthorityKeyIdentifier CreateAuthorityKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo) {
            return new AuthorityKeyIdentifierBCFips(extensionUtils.CreateAuthorityKeyIdentifier(((SubjectPublicKeyInfoBCFips
                )publicKeyInfo).GetSubjectPublicKeyInfo()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISubjectKeyIdentifier CreateSubjectKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo) {
            return new SubjectKeyIdentifierBCFips(extensionUtils.CreateSubjectKeyIdentifier(((SubjectPublicKeyInfoBCFips
                )publicKeyInfo).GetSubjectPublicKeyInfo()));
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cert.X509ExtensionUtilsBCFips that = (iText.Bouncycastlefips.Cert.X509ExtensionUtilsBCFips
                )o;
            return Object.Equals(extensionUtils, that.extensionUtils);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(extensionUtils);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return extensionUtils.ToString();
        }
    }
}
