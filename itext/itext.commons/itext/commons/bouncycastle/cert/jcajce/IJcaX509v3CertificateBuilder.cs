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
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert.Jcajce {
    /// <summary>
    /// This interface represents the wrapper for JcaX509v3CertificateBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IJcaX509v3CertificateBuilder {
        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped JcaX509v3CertificateBuilder object.
        /// </summary>
        /// <param name="contentSigner">ContentSigner wrapper</param>
        /// <returns>{IX509CertificateHolder} wrapper for built X509CertificateHolder object.</returns>
        IX509Certificate Build(IContentSigner contentSigner);

        /// <summary>
        /// Calls actual
        /// <c>addExtension</c>
        /// method for the wrapped JcaX509v3CertificateBuilder object.
        /// </summary>
        /// <param name="extensionOID">wrapper for the OID defining the extension type</param>
        /// <param name="critical">true if the extension is critical, false otherwise</param>
        /// <param name="extensionValue">wrapped ASN.1 structure that forms the extension's value</param>
        /// <returns>
        /// 
        /// <see cref="IJcaX509v3CertificateBuilder"/>
        /// this wrapper object.
        /// </returns>
        IJcaX509v3CertificateBuilder AddExtension(IASN1ObjectIdentifier extensionOID, bool critical, IASN1Encodable
             extensionValue);
    }
}
