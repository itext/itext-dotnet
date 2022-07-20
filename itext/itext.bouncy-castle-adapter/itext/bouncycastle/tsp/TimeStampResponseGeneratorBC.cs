using System;
using System.Collections.Generic;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    public class TimeStampResponseGeneratorBC : ITimeStampResponseGenerator {
        private readonly TimeStampResponseGenerator timeStampResponseGenerator;

        public TimeStampResponseGeneratorBC(TimeStampResponseGenerator timeStampResponseGenerator) {
            this.timeStampResponseGenerator = timeStampResponseGenerator;
        }

        public TimeStampResponseGeneratorBC(ITimeStampTokenGenerator tokenGenerator, ICollection<String> algorithms
            )
            : this(new TimeStampResponseGenerator(((TimeStampTokenGeneratorBC)tokenGenerator).GetTimeStampTokenGenerator
                (), algorithms)) {
        }

        public virtual TimeStampResponseGenerator GetTimeStampResponseGenerator() {
            return timeStampResponseGenerator;
        }

        public virtual ITimeStampResponse Generate(ITimeStampRequest request, BigInteger bigInteger, DateTime date
            ) {
            try {
                return new TimeStampResponseBC(timeStampResponseGenerator.Generate(((TimeStampRequestBC)request).GetTimeStampRequest
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
            iText.Bouncycastle.Tsp.TimeStampResponseGeneratorBC that = (iText.Bouncycastle.Tsp.TimeStampResponseGeneratorBC
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
