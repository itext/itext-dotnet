using System;
using System.Collections.Generic;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponseGenerator"/>.
    /// </summary>
    public class TimeStampResponseGeneratorBC : ITimeStampResponseGenerator {
        private readonly TimeStampResponseGenerator timeStampResponseGenerator;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponseGenerator"/>.
        /// </summary>
        /// <param name="timeStampResponseGenerator">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponseGenerator"/>
        /// to be wrapped
        /// </param>
        public TimeStampResponseGeneratorBC(TimeStampResponseGenerator timeStampResponseGenerator) {
            this.timeStampResponseGenerator = timeStampResponseGenerator;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponseGenerator"/>.
        /// </summary>
        /// <param name="tokenGenerator">TimeStampTokenGenerator wrapper</param>
        /// <param name="algorithms">set of algorithm strings</param>
        public TimeStampResponseGeneratorBC(ITimeStampTokenGenerator tokenGenerator, ICollection<String> algorithms
            )
            : this(new TimeStampResponseGenerator(((TimeStampTokenGeneratorBC)tokenGenerator).GetTimeStampTokenGenerator
                (), algorithms)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponseGenerator"/>.
        /// </returns>
        public virtual TimeStampResponseGenerator GetTimeStampResponseGenerator() {
            return timeStampResponseGenerator;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampResponse Generate(ITimeStampRequest request, BigInteger bigInteger, DateTime date
            ) {
            try {
                return new TimeStampResponseBC(timeStampResponseGenerator.Generate(((TimeStampRequestBC)request).GetTimeStampRequest
                    (), bigInteger, date));
            }
            catch (TspException e) {
                throw new TSPExceptionBC(e);
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
            iText.Bouncycastle.Tsp.TimeStampResponseGeneratorBC that = (iText.Bouncycastle.Tsp.TimeStampResponseGeneratorBC
                )o;
            return Object.Equals(timeStampResponseGenerator, that.timeStampResponseGenerator);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampResponseGenerator);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return timeStampResponseGenerator.ToString();
        }
    }
}
