using System;
using System.Collections.Generic;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Tsp {
    public class TimeStampResponseGeneratorBCFips : ITimeStampResponseGenerator {
        private readonly TimeStampResponseGenerator timeStampResponseGenerator;

        public TimeStampResponseGeneratorBCFips(TimeStampResponseGenerator timeStampResponseGenerator) {
            this.timeStampResponseGenerator = timeStampResponseGenerator;
        }

        public TimeStampResponseGeneratorBCFips(ITimeStampTokenGenerator tokenGenerator, ICollection<String> algorithms
            )
            : this(new TimeStampResponseGenerator(((TimeStampTokenGeneratorBCFips)tokenGenerator).GetTimeStampTokenGenerator
                (), algorithms)) {
        }

        public virtual TimeStampResponseGenerator GetTimeStampResponseGenerator() {
            return timeStampResponseGenerator;
        }

        public virtual ITimeStampResponse Generate(ITimeStampRequest request, BigInteger bigInteger, DateTime date
            ) {
            try {
                return new TimeStampResponseBCFips(timeStampResponseGenerator.Generate(((TimeStampRequestBCFips)request).GetTimeStampRequest
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
            iText.Bouncycastlefips.Tsp.TimeStampResponseGeneratorBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampResponseGeneratorBCFips
                )o;
            return Object.Equals(timeStampResponseGenerator, that.timeStampResponseGenerator);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampResponseGenerator);
        }

        public override String ToString() {
            return timeStampResponseGenerator.ToString();
        }
    }
}
