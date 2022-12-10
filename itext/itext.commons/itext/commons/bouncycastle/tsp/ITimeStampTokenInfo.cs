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
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TimeStampTokenInfo that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITimeStampTokenInfo {
        /// <summary>
        /// Calls actual
        /// <c>getHashAlgorithm</c>
        /// method for the wrapped TimeStampTokenInfo object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IAlgorithmIdentifier"/>
        /// the wrapper for the received AlgorithmIdentifier object.
        /// </returns>
        IAlgorithmIdentifier GetHashAlgorithm();

        /// <summary>
        /// Calls actual
        /// <c>toASN1Structure</c>
        /// method for the wrapped TimeStampTokenInfo object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Tsp.ITSTInfo"/>
        /// TSTInfo wrapper.
        /// </returns>
        ITSTInfo ToASN1Structure();

        /// <summary>
        /// Calls actual
        /// <c>getGenTime</c>
        /// method for the wrapped TimeStampTokenInfo object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="System.DateTime"/>
        /// the received genTime.
        /// </returns>
        DateTime GetGenTime();

        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped TimeStampTokenInfo object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();
    }
}
