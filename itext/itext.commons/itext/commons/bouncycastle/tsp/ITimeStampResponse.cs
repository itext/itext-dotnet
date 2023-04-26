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
using iText.Commons.Bouncycastle.Asn1.Cmp;

namespace iText.Commons.Bouncycastle.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TimeStampResponse that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITimeStampResponse {
        /// <summary>
        /// Calls actual
        /// <c>validate</c>
        /// method for the wrapped TimeStampResponse object.
        /// </summary>
        /// <param name="request">TimeStampRequest wrapper</param>
        void Validate(ITimeStampRequest request);

        /// <summary>
        /// Calls actual
        /// <c>getFailInfo</c>
        /// method for the wrapped TimeStampResponse object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Cmp.IPkiFailureInfo"/>
        /// the wrapper for the received PKIFailureInfo object.
        /// </returns>
        IPkiFailureInfo GetFailInfo();

        /// <summary>
        /// Calls actual
        /// <c>getTimeStampToken</c>
        /// method for the wrapped TimeStampResponse object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="ITimeStampToken"/>
        /// the wrapper for the received TimeStampToken object.
        /// </returns>
        ITimeStampToken GetTimeStampToken();

        /// <summary>
        /// Calls actual
        /// <c>getStatusString</c>
        /// method for the wrapped TimeStampResponse object.
        /// </summary>
        /// <returns>status string.</returns>
        String GetStatusString();

        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped TimeStampResponse object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();
    }
}
