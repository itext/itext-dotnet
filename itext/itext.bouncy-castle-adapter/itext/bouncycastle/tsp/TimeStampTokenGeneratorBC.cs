using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Cert.Jcajce;
using iText.Bouncycastle.Cms;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert.Jcajce;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    public class TimeStampTokenGeneratorBC : ITimeStampTokenGenerator {
        private readonly TimeStampTokenGenerator timeStampTokenGenerator;

        public TimeStampTokenGeneratorBC(TimeStampTokenGenerator timeStampTokenGenerator) {
            this.timeStampTokenGenerator = timeStampTokenGenerator;
        }

        public TimeStampTokenGeneratorBC(ISignerInfoGenerator siGen, IDigestCalculator dgCalc, IASN1ObjectIdentifier
             policy) {
            try {
                this.timeStampTokenGenerator = new TimeStampTokenGenerator(((SignerInfoGeneratorBC)siGen).GetSignerInfoGenerator
                    (), ((DigestCalculatorBC)dgCalc).GetDigestCalculator(), ((ASN1ObjectIdentifierBC)policy).GetASN1ObjectIdentifier
                    ());
            }
            catch (TSPException e) {
                throw new TSPExceptionBC(e);
            }
        }

        public virtual TimeStampTokenGenerator GetTimeStampTokenGenerator() {
            return timeStampTokenGenerator;
        }

        public virtual void SetAccuracySeconds(int i) {
            timeStampTokenGenerator.SetAccuracySeconds(i);
        }

        public virtual void AddCertificates(IJcaCertStore jcaCertStore) {
            timeStampTokenGenerator.AddCertificates(((JcaCertStoreBC)jcaCertStore).GetJcaCertStore());
        }

        public virtual ITimeStampToken Generate(ITimeStampRequest request, BigInteger bigInteger, DateTime date) {
            try {
                return new TimeStampTokenBC(timeStampTokenGenerator.Generate(((TimeStampRequestBC)request).GetTimeStampRequest
                    (), bigInteger, date));
            }
            catch (TSPException e) {
                throw new TSPExceptionBC(e);
            }
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Tsp.TimeStampTokenGeneratorBC that = (iText.Bouncycastle.Tsp.TimeStampTokenGeneratorBC)
                o;
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
