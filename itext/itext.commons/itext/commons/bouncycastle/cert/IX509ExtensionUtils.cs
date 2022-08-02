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
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Cert {
    /// <summary>
    /// This interface represents the wrapper for X509ExtensionUtils that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IX509ExtensionUtils {
        /// <summary>
        /// Calls actual
        /// <c>createAuthorityKeyIdentifier</c>
        /// method for the wrapped X509ExtensionUtils object.
        /// </summary>
        /// <param name="publicKeyInfo">SubjectPublicKeyInfo wrapper</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IAuthorityKeyIdentifier"/>
        /// wrapper for the created AuthorityKeyIdentifier.
        /// </returns>
        IAuthorityKeyIdentifier CreateAuthorityKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo);

        /// <summary>
        /// Calls actual
        /// <c>createSubjectKeyIdentifier</c>
        /// method for the wrapped X509ExtensionUtils object.
        /// </summary>
        /// <param name="publicKeyInfo">SubjectPublicKeyInfo wrapper</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.ISubjectKeyIdentifier"/>
        /// wrapper for the created SubjectKeyIdentifier.
        /// </returns>
        ISubjectKeyIdentifier CreateSubjectKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo);
    }
}
