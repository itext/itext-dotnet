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
using System;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert {
    /// <summary>
    /// This interface represents the wrapper for X509v2CRLBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IX509V2CrlGenerator {
        /// <summary>
        /// Calls actual
        /// <c>addCRLEntry</c>
        /// method for the wrapped X509v2CRLBuilder object.
        /// </summary>
        /// <param name="bigInteger">serial number of revoked certificate</param>
        /// <param name="date">date of certificate revocation</param>
        /// <param name="i">the reason code, as indicated in CRLReason, i.e CRLReason.keyCompromise, or 0 if not to be used
        ///     </param>
        /// <returns>
        /// 
        /// <see cref="IX509V2CrlGenerator"/>
        /// the current wrapper object.
        /// </returns>
        IX509V2CrlGenerator AddCRLEntry(IBigInteger bigInteger, DateTime date, int i);

        /// <summary>
        /// Calls actual
        /// <c>addExtension</c>
        /// method for the wrapped X509v2CRLBuilder object.
        /// </summary>
        /// <param name="objectIdentifier">extension object identifier</param>
        /// <param name="isCritical">specifies if extension is critical or not</param>
        /// <param name="extension">encoded extension value</param>
        /// <returns>
        /// 
        /// <see cref="IX509V2CrlGenerator"/>
        /// the current wrapper object.
        /// </returns>
        IX509V2CrlGenerator AddExtension(IDerObjectIdentifier objectIdentifier, bool isCritical, IAsn1Encodable extension
            );

        /// <summary>
        /// Calls actual
        /// <c>setNextUpdate</c>
        /// method for the wrapped X509v2CRLBuilder object.
        /// </summary>
        /// <param name="nextUpdate">date of next CRL update</param>
        /// <returns>
        /// 
        /// <see cref="IX509V2CrlGenerator"/>
        /// the current wrapper object.
        /// </returns>
        IX509V2CrlGenerator SetNextUpdate(DateTime nextUpdate);

        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped X509v2CRLBuilder object.
        /// </summary>
        /// <param name="signer">ContentSigner wrapper</param>
        /// <returns>
        /// 
        /// <see cref="IX509Crl"/>
        /// the wrapper for built X509CRLHolder object.
        /// </returns>
        IX509Crl Build(IContentSigner signer);
    }
}
