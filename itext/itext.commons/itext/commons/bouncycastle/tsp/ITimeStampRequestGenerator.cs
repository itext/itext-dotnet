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
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TimeStampRequestGenerator that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITimeStampRequestGenerator {
        /// <summary>
        /// Calls actual
        /// <c>setCertReq</c>
        /// method for the wrapped TimeStampRequestGenerator object.
        /// </summary>
        /// <param name="var1">the value to be set</param>
        void SetCertReq(bool var1);

        /// <summary>
        /// Calls actual
        /// <c>setReqPolicy</c>
        /// method for the wrapped TimeStampRequestGenerator object.
        /// </summary>
        /// <param name="reqPolicy">the value to be set</param>
        void SetReqPolicy(String reqPolicy);

        /// <summary>
        /// Calls actual
        /// <c>generate</c>
        /// method for the wrapped TimeStampRequestGenerator object.
        /// </summary>
        /// <param name="objectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="imprint">byte array</param>
        /// <param name="nonce">BigInteger</param>
        /// <returns>
        /// 
        /// <see cref="ITimeStampRequest"/>
        /// the wrapper for generated TimeStampRequest object.
        /// </returns>
        ITimeStampRequest Generate(IDerObjectIdentifier objectIdentifier, byte[] imprint, IBigInteger nonce);
    }
}
