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

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for OCSPResp that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IOCSPResp {
        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped OCSPResp object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();

        /// <summary>
        /// Calls actual
        /// <c>getStatus</c>
        /// method for the wrapped OCSPResp object.
        /// </summary>
        /// <returns>status value.</returns>
        int GetStatus();

        /// <summary>
        /// Calls actual
        /// <c>getResponseObject</c>
        /// method for the wrapped OCSPResp object.
        /// </summary>
        /// <returns>response object.</returns>
        Object GetResponseObject();

        /// <summary>
        /// Gets
        /// <c>SUCCESSFUL</c>
        /// constant for the wrapped OCSPResp.
        /// </summary>
        /// <returns>OCSPResp.SUCCESSFUL value.</returns>
        int GetSuccessful();
    }
}
