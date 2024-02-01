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
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequestGenerator"/>.
    /// </summary>
    public class TimeStampRequestGeneratorBC : ITimeStampRequestGenerator {
        private readonly TimeStampRequestGenerator requestGenerator;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequestGenerator"/>.
        /// </summary>
        /// <param name="requestGenerator">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequestGenerator"/>
        /// to be wrapped
        /// </param>
        public TimeStampRequestGeneratorBC(TimeStampRequestGenerator requestGenerator) {
            this.requestGenerator = requestGenerator;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequestGenerator"/>.
        /// </returns>
        public virtual TimeStampRequestGenerator GetRequestGenerator() {
            return requestGenerator;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void SetCertReq(bool var1) {
            requestGenerator.SetCertReq(var1);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void SetReqPolicy(String reqPolicy) {
            requestGenerator.SetReqPolicy(reqPolicy);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampRequest Generate(IDerObjectIdentifier objectIdentifier, byte[] imprint, IBigInteger
             nonce) {
            return new TimeStampRequestBC(requestGenerator.Generate(((DerObjectIdentifierBC)objectIdentifier).GetDerObjectIdentifier
                (), imprint, ((BigIntegerBC) nonce).GetBigInteger()));
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
            iText.Bouncycastle.Tsp.TimeStampRequestGeneratorBC that = (iText.Bouncycastle.Tsp.TimeStampRequestGeneratorBC
                )o;
            return Object.Equals(requestGenerator, that.requestGenerator);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(requestGenerator);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return requestGenerator.ToString();
        }
    }
}
