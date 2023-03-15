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
using System;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for BasicOCSPRespBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IBasicOCSPRespBuilder {
        /// <summary>
        /// Calls actual
        /// <c>setResponseExtensions</c>
        /// method for the wrapped BasicOCSPRespBuilder object.
        /// </summary>
        /// <param name="extensions">response extensions wrapper</param>
        /// <returns>
        /// 
        /// <see cref="IBasicOCSPRespBuilder"/>
        /// this wrapper object.
        /// </returns>
        IBasicOCSPRespBuilder SetResponseExtensions(IExtensions extensions);

        /// <summary>
        /// Calls actual
        /// <c>addResponse</c>
        /// method for the wrapped BasicOCSPRespBuilder object.
        /// </summary>
        /// <param name="certID">wrapped certificate ID details</param>
        /// <param name="certificateStatus">wrapped status of the certificate - wrapped null if okay</param>
        /// <param name="time">date this response was valid on</param>
        /// <param name="time1">date when next update should be requested</param>
        /// <param name="extensions">optional wrapped extensions</param>
        /// <returns>
        /// 
        /// <see cref="IBasicOCSPRespBuilder"/>
        /// this wrapper object.
        /// </returns>
        IBasicOCSPRespBuilder AddResponse(ICertificateID certID, ICertificateStatus certificateStatus, DateTime time
            , DateTime time1, IExtensions extensions);

        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped BasicOCSPRespBuilder object.
        /// </summary>
        /// <param name="signer">ContentSigner wrapper</param>
        /// <param name="chain">list of wrapped X509CertificateHolder objects</param>
        /// <param name="time">produced at</param>
        /// <returns>
        /// 
        /// <see cref="IBasicOCSPResponse"/>
        /// wrapper for built BasicOCSPResp object.
        /// </returns>
        IBasicOCSPResponse Build(IContentSigner signer, IX509Certificate[] chain, DateTime time);
    }
}
