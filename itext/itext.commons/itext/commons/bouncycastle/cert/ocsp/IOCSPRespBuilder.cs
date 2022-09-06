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
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for OCSPRespBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IOCSPRespBuilder {
        /// <summary>
        /// Gets
        /// <c>SUCCESSFUL</c>
        /// constant for the wrapped OCSPRespBuilder.
        /// </summary>
        /// <returns>OCSPRespBuilder.SUCCESSFUL value.</returns>
        int GetSuccessful();

        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped OCSPRespBuilder object.
        /// </summary>
        /// <param name="i">status</param>
        /// <param name="basicOCSPResp">BasicOCSPResp wrapper</param>
        /// <returns>
        /// 
        /// <see cref="IOCSPResponse"/>
        /// the wrapper for built OCSPResp object.
        /// </returns>
        IOCSPResponse Build(int i, IBasicOCSPResponse basicOCSPResp);
    }
}
