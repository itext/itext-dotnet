/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequest"/>.
    /// </summary>
    public class TimeStampRequestBC : ITimeStampRequest {
        private readonly TimeStampRequest timeStampRequest;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequest"/>.
        /// </summary>
        /// <param name="timeStampRequest">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequest"/>
        /// to be wrapped
        /// </param>
        public TimeStampRequestBC(TimeStampRequest timeStampRequest) {
            this.timeStampRequest = timeStampRequest;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequest"/>.
        /// </returns>
        public virtual TimeStampRequest GetTimeStampRequest() {
            return timeStampRequest;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return timeStampRequest.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBigInteger GetNonce() {
            return new BigIntegerBC(timeStampRequest.Nonce);
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
            iText.Bouncycastle.Tsp.TimeStampRequestBC that = (iText.Bouncycastle.Tsp.TimeStampRequestBC)o;
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
