/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TimeStampTokenGenerator that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITimeStampTokenGenerator {
        /// <summary>
        /// Calls actual
        /// <c>setAccuracySeconds</c>
        /// method for the wrapped TimeStampTokenGenerator object.
        /// </summary>
        /// <param name="i">accuracy seconds to set</param>
        void SetAccuracySeconds(int i);

        /// <summary>
        /// Creates certificates collection and calls actual
        /// <c>SetCertificates</c>
        /// method for the wrapped TimeStampTokenGenerator object.
        /// </summary>
        /// <param name="certificateChain">certificates collection to set</param>
        void SetCertificates(IList<IX509Certificate> certificateChain);

        /// <summary>
        /// Calls actual
        /// <c>generate</c>
        /// method for the wrapped TimeStampTokenGenerator object.
        /// </summary>
        /// <param name="request">the originating TimeStampRequest wrapper</param>
        /// <param name="bigInteger">serial number for the TimeStampToken</param>
        /// <param name="date">token generation time</param>
        /// <returns>
        /// 
        /// <see cref="ITimeStampToken"/>
        /// the wrapper for the generated TimeStampToken object.
        /// </returns>
        ITimeStampToken Generate(ITimeStampRequest request, IBigInteger bigInteger, DateTime date);
    }
}
