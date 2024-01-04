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
using Org.BouncyCastle.Tsp;
using iText.Bouncycastle.Asn1.Cmp;
using iText.Commons.Bouncycastle.Asn1.Cmp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponse"/>.
    /// </summary>
    public class TimeStampResponseBC : ITimeStampResponse {
        private readonly TimeStampResponse timeStampResponse;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponse"/>.
        /// </summary>
        /// <param name="timeStampResponse">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponse"/>
        /// to be wrapped
        /// </param>
        public TimeStampResponseBC(TimeStampResponse timeStampResponse) {
            this.timeStampResponse = timeStampResponse;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponse"/>.
        /// </returns>
        public virtual TimeStampResponse GetTimeStampResponse() {
            return timeStampResponse;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Validate(ITimeStampRequest request) {
            try {
                timeStampResponse.Validate(((TimeStampRequestBC)request).GetTimeStampRequest());
            }
            catch (TspException e) {
                throw new TSPExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IPkiFailureInfo GetFailInfo() {
            return new PkiFailureInfoBC(timeStampResponse.GetFailInfo());
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampToken GetTimeStampToken() {
            return new TimeStampTokenBC(timeStampResponse.TimeStampToken);
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetStatusString() {
            return timeStampResponse.GetStatusString();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return timeStampResponse.GetEncoded();
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
            iText.Bouncycastle.Tsp.TimeStampResponseBC that = (iText.Bouncycastle.Tsp.TimeStampResponseBC)o;
            return Object.Equals(timeStampResponse, that.timeStampResponse);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampResponse);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return timeStampResponse.ToString();
        }
    }
}
