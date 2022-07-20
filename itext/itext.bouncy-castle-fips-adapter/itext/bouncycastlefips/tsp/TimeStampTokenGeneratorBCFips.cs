using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Cert.Jcajce;
using iText.Bouncycastlefips.Cms;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert.Jcajce;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenGenerator"/>.
    /// </summary>
    public class TimeStampTokenGeneratorBCFips : ITimeStampTokenGenerator {
        private readonly TimeStampTokenGenerator timeStampTokenGenerator;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenGenerator"/>.
        /// </summary>
        /// <param name="timeStampTokenGenerator">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenGenerator"/>
        /// to be wrapped
        /// </param>
        public TimeStampTokenGeneratorBCFips(TimeStampTokenGenerator timeStampTokenGenerator) {
            this.timeStampTokenGenerator = timeStampTokenGenerator;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenGenerator"/>.
        /// </summary>
        /// <param name="siGen">SignerInfoGenerator wrapper</param>
        /// <param name="dgCalc">DigestCalculator wrapper</param>
        /// <param name="policy">ASN1ObjectIdentifier wrapper</param>
        public TimeStampTokenGeneratorBCFips(ISignerInfoGenerator siGen, IDigestCalculator dgCalc, IASN1ObjectIdentifier
             policy) {
            try {
                this.timeStampTokenGenerator = new TimeStampTokenGenerator(((SignerInfoGeneratorBCFips)siGen).GetSignerInfoGenerator
                    (), ((DigestCalculatorBCFips)dgCalc).GetDigestCalculator(), ((ASN1ObjectIdentifierBCFips)policy).GetASN1ObjectIdentifier
                    ());
            }
            catch (TspException e) {
                throw new TSPExceptionBCFips(e);
            }
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenGenerator"/>.
        /// </returns>
        public virtual TimeStampTokenGenerator GetTimeStampTokenGenerator() {
            return timeStampTokenGenerator;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void SetAccuracySeconds(int i) {
            timeStampTokenGenerator.SetAccuracySeconds(i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void AddCertificates(IJcaCertStore jcaCertStore) {
            timeStampTokenGenerator.AddCertificates(((JcaCertStoreBCFips)jcaCertStore).GetJcaCertStore());
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampToken Generate(ITimeStampRequest request, BigInteger bigInteger, DateTime date) {
            try {
                return new TimeStampTokenBCFips(timeStampTokenGenerator.Generate(((TimeStampRequestBCFips)request).GetTimeStampRequest
                    (), bigInteger, date));
            }
            catch (TspException e) {
                throw new TSPExceptionBCFips(e);
            }
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
            iText.Bouncycastlefips.Tsp.TimeStampTokenGeneratorBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampTokenGeneratorBCFips
                )o;
            return Object.Equals(timeStampTokenGenerator, that.timeStampTokenGenerator);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampTokenGenerator);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return timeStampTokenGenerator.ToString();
        }
    }
}
