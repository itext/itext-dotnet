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
    public class TimeStampTokenGeneratorBCFips : ITimeStampTokenGenerator {
        private readonly TimeStampTokenGenerator timeStampTokenGenerator;

        public TimeStampTokenGeneratorBCFips(TimeStampTokenGenerator timeStampTokenGenerator) {
            this.timeStampTokenGenerator = timeStampTokenGenerator;
        }

        public TimeStampTokenGeneratorBCFips(ISignerInfoGenerator siGen, IDigestCalculator dgCalc, IASN1ObjectIdentifier
             policy) {
            try {
                this.timeStampTokenGenerator = new TimeStampTokenGenerator(((SignerInfoGeneratorBCFips)siGen).GetSignerInfoGenerator
                    (), ((DigestCalculatorBCFips)dgCalc).GetDigestCalculator(), ((ASN1ObjectIdentifierBCFips)policy).GetASN1ObjectIdentifier
                    ());
            }
            catch (TSPException e) {
                throw new TSPExceptionBCFips(e);
            }
        }

        public virtual TimeStampTokenGenerator GetTimeStampTokenGenerator() {
            return timeStampTokenGenerator;
        }

        public virtual void SetAccuracySeconds(int i) {
            timeStampTokenGenerator.SetAccuracySeconds(i);
        }

        public virtual void AddCertificates(IJcaCertStore jcaCertStore) {
            timeStampTokenGenerator.AddCertificates(((JcaCertStoreBCFips)jcaCertStore).GetJcaCertStore());
        }

        public virtual ITimeStampToken Generate(ITimeStampRequest request, BigInteger bigInteger, DateTime date) {
            try {
                return new TimeStampTokenBCFips(timeStampTokenGenerator.Generate(((TimeStampRequestBCFips)request).GetTimeStampRequest
                    (), bigInteger, date));
            }
            catch (TSPException e) {
                throw new TSPExceptionBCFips(e);
            }
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampTokenGenerator);
        }

        public override String ToString() {
            return timeStampTokenGenerator.ToString();
        }
    }
}
