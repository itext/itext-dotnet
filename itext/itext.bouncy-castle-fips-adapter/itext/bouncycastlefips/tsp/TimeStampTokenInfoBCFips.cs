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
using iText.Bouncycastlefips.Asn1.Tsp;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Tsp;

namespace iText.Bouncycastlefips.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenInfo"/>.
    /// </summary>
    public class TimeStampTokenInfoBCFips : ITimeStampTokenInfo {
        private readonly TstInfo timeStampTokenInfo;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenInfo"/>.
        /// </summary>
        /// <param name="timeStampTokenInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenInfo"/>
        /// to be wrapped
        /// </param>
        public TimeStampTokenInfoBCFips(TstInfo timeStampTokenInfo) {
            this.timeStampTokenInfo = timeStampTokenInfo;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenInfo"/>.
        /// </returns>
        public virtual TstInfo GetTimeStampTokenInfo() {
            return timeStampTokenInfo;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier GetHashAlgorithm() {
            return new AlgorithmIdentifierBCFips(timeStampTokenInfo.MessageImprint.HashAlgorithm);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITSTInfo ToASN1Structure() {
            return new TSTInfoBCFips(timeStampTokenInfo);
        }

        /// <summary><inheritDoc/></summary>
        public virtual DateTime GetGenTime() {
            return timeStampTokenInfo.GenTime.ToDateTime();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return timeStampTokenInfo.GetEncoded();
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
            iText.Bouncycastlefips.Tsp.TimeStampTokenInfoBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampTokenInfoBCFips
                )o;
            return Object.Equals(timeStampTokenInfo, that.timeStampTokenInfo);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampTokenInfo);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return timeStampTokenInfo.ToString();
        }
    }
}
