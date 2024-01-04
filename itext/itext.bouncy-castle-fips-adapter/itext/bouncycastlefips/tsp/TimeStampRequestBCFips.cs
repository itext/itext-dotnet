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
using iText.Bouncycastlefips.Math;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Tsp;

namespace iText.Bouncycastlefips.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequest"/>.
    /// </summary>
    public class TimeStampRequestBCFips : ITimeStampRequest {
        private readonly TimeStampReq timeStampRequest;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequest"/>.
        /// </summary>
        /// <param name="timeStampRequest">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TimeStampReq"/>
        /// to be wrapped
        /// </param>
        public TimeStampRequestBCFips(TimeStampReq timeStampRequest) {
            this.timeStampRequest = timeStampRequest;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TimeStampReq"/>.
        /// </returns>
        public virtual TimeStampReq GetTimeStampRequest() {
            return timeStampRequest;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return timeStampRequest.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBigInteger GetNonce() {
            return new BigIntegerBCFips(timeStampRequest.Nonce.Value);
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Tsp.TimeStampRequestBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampRequestBCFips
                )o;
            return Object.Equals(timeStampRequest, that.timeStampRequest);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampRequest);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return timeStampRequest.ToString();
        }
    }
}
