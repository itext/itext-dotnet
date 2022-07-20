using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
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
        public virtual ITimeStampRequest Generate(IASN1ObjectIdentifier objectIdentifier, byte[] imprint, BigInteger
             nonce) {
            return new TimeStampRequestBC(requestGenerator.Generate(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier
                (), imprint, nonce));
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
