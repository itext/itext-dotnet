/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Collections;
using System.IO;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Utilities.Date;

namespace iText.Bouncycastlefips.Tsp {
    /// <summary>
    /// Wrapper class for TimeStampResponse generator.
    /// </summary>
    public class TimeStampResponseGeneratorBCFips : ITimeStampResponseGenerator {
        private readonly TimeStampTokenGeneratorBCFips tokenGenerator;
        private readonly IList acceptedAlgorithms;

        /// <summary>
        /// Creates new wrapper instance for TimeStampResponse generator.
        /// </summary>
        /// <param name="tokenGenerator">TimeStampToken generator wrapper</param>
        /// <param name="algorithms">list of algorithm strings</param>
        public TimeStampResponseGeneratorBCFips(TimeStampTokenGeneratorBCFips tokenGenerator, IList algorithms) { 
            this.tokenGenerator = tokenGenerator;
            this.acceptedAlgorithms = algorithms;
        }

        /// <summary>Gets ITimeStampTokenGenerator field.</summary>
        /// <returns>TokenGenerator field.</returns>
        public virtual ITimeStampTokenGenerator GetTimeStampTokenGenerator() {
            return tokenGenerator;
        }

        /// <summary>Gets list of accepted algorithms.</summary>
        /// <returns>AcceptedAlgorithms field.</returns>
        public virtual IList GetAcceptedAlgorithms() {
            return acceptedAlgorithms;
        }
        
        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampResponse Generate(ITimeStampRequest req, IBigInteger bigInteger, DateTime date) {
            TimeStampResp resp;
            int failureCode = 0;
            try {
                if (!acceptedAlgorithms.Contains(((TimeStampRequestBCFips)req).GetTimeStampRequest()
                        .MessageImprint.HashAlgorithm.Algorithm.Id)) {
                    failureCode = PkiFailureInfo.BadAlg;
                    throw new Exception("request contains unknown algorithm");
                }
                PkiStatusInfo pkiStatusInfo = new PkiStatusInfo(new DerSequence(
                    new Asn1EncodableVector(new DerInteger((int)PkiStatus.Granted)) 
                    { new PkiFreeText(new DerSequence(new DerUtf8String("Operation Okay"))) }));

                ContentInfo tstTokenContentInfo;
                try {
                    ITimeStampToken token = tokenGenerator.Generate(req, bigInteger, new DateTimeObject(date).Value);
                    byte[] encoded = token.GetEncoded();
                    tstTokenContentInfo = ContentInfo.GetInstance(Asn1Object.FromByteArray(encoded));
                } catch (IOException e) {
                    throw new Exception("Timestamp token received cannot be converted to ContentInfo", e);
                }
                resp = new TimeStampResp(pkiStatusInfo, tstTokenContentInfo);
            } catch (Exception e) {
                resp = new TimeStampResp(new PkiStatusInfo(new DerSequence(
                    new Asn1EncodableVector(new DerInteger((int)PkiStatus.Rejection))
                    {
                        new PkiFreeText(new DerSequence(new DerUtf8String(e.Message))),
                        failureCode == 0 ? null : new PkiFailureInfo(failureCode)
                    })), null);
            }
            return new TimeStampResponseBCFips(resp);
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
            TimeStampResponseGeneratorBCFips that = (TimeStampResponseGeneratorBCFips)o;
            return Object.Equals(tokenGenerator, that.tokenGenerator) &&
                   Object.Equals(acceptedAlgorithms, that.acceptedAlgorithms);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode<object>(tokenGenerator, acceptedAlgorithms);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return tokenGenerator + " " + acceptedAlgorithms;
        }
    }
}
