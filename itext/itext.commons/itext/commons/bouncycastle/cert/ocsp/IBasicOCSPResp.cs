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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for BasicOCSPResp that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IBasicOCSPResp {
        /// <summary>
        /// Calls actual
        /// <c>getResponses</c>
        /// method for the wrapped BasicOCSPResp object.
        /// </summary>
        /// <returns>wrapped SingleResp list.</returns>
        ISingleResp[] GetResponses();

        /// <summary>
        /// Calls actual
        /// <c>isSignatureValid</c>
        /// method for the wrapped BasicOCSPResp object.
        /// </summary>
        /// <param name="provider">ContentVerifierProvider wrapper</param>
        /// <returns>boolean value.</returns>
        bool IsSignatureValid(IContentVerifierProvider provider);

        /// <summary>
        /// Calls actual
        /// <c>getCerts</c>
        /// method for the wrapped BasicOCSPResp object.
        /// </summary>
        /// <returns>wrapped certificates list.</returns>
        IX509CertificateHolder[] GetCerts();

        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped BasicOCSPResp object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();

        /// <summary>
        /// Calls actual
        /// <c>getProducedAt</c>
        /// method for the wrapped BasicOCSPResp object.
        /// </summary>
        /// <returns>produced at date.</returns>
        DateTime GetProducedAt();
    }
}
