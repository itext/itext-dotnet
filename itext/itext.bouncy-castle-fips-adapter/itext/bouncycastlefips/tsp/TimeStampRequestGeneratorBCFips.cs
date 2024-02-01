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
using Org.BouncyCastle.Math;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Math;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X509;

namespace iText.Bouncycastlefips.Tsp {
    /// <summary>
    /// Generator for
    /// <see cref="Org.BouncyCastle.Asn1.Tsp.TimeStampReq"/>.
    /// </summary>
    public class TimeStampRequestGeneratorBCFips : ITimeStampRequestGenerator {
        private DerObjectIdentifier reqPolicy;
        private DerBoolean certReq;

        /// <summary>
        /// Creates new generator instance for
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TimeStampReq"/>.
        /// </summary>
        public TimeStampRequestGeneratorBCFips() {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped cert req
        /// <see cref="Org.BouncyCastle.Asn1.DerBoolean"/>.
        /// </returns>
        public virtual DerBoolean GetCertReq() {
            return certReq;
        }
        
        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped req policy
        /// <see cref="Org.BouncyCastle.Asn1.DerObjectIdentifier"/>.
        /// </returns>
        public virtual DerObjectIdentifier GetReqPolicy() {
            return reqPolicy;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void SetCertReq(bool var1) {
            certReq = DerBoolean.GetInstance(var1);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void SetReqPolicy(String reqPolicy) {
            this.reqPolicy = new DerObjectIdentifier(reqPolicy);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampRequest Generate(IDerObjectIdentifier objectIdentifier, byte[] digest, IBigInteger nonceWrapper) {
            var digestAlgorithmOid = ((DerObjectIdentifierBCFips)objectIdentifier).GetDerObjectIdentifier().Id;
            if (digestAlgorithmOid == null) {
                throw new ArgumentException("No digest algorithm specified");
            }
            BigInteger nonce = ((BigIntegerBCFips)nonceWrapper).GetBigInteger();
            DerObjectIdentifier digestAlgOid = new DerObjectIdentifier(digestAlgorithmOid);
            AlgorithmIdentifier algID = new AlgorithmIdentifier(digestAlgOid, DerNull.Instance);
            MessageImprint messageImprint = new MessageImprint(algID, digest);
            DerInteger derNonce = nonce == null ? null : new DerInteger(nonce);
            return new TimeStampRequestBCFips(new TimeStampReq(messageImprint, reqPolicy, derNonce, certReq, null));
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
            iText.Bouncycastlefips.Tsp.TimeStampRequestGeneratorBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampRequestGeneratorBCFips
                )o;
            return Object.Equals(certReq, that.certReq) &&
                   Object.Equals(reqPolicy, that.reqPolicy);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode<object>(certReq, reqPolicy);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return reqPolicy + " " + certReq;
        }
    }
}
