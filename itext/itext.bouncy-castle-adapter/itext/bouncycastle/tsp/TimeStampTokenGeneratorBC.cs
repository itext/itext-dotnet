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
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastle.Cert;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace iText.Bouncycastle.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenGenerator"/>.
    /// </summary>
    public class TimeStampTokenGeneratorBC : ITimeStampTokenGenerator {
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
        public TimeStampTokenGeneratorBC(TimeStampTokenGenerator timeStampTokenGenerator) {
            this.timeStampTokenGenerator = timeStampTokenGenerator;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenGenerator"/>.
        /// </summary>
        /// <param name="pk">AsymmetricKeyParameter wrapper</param>
        /// <param name="cert">X509Certificate wrapper</param>
        /// <param name="allowedDigest">allowed digest</param>
        /// <param name="policyOid">policy OID</param>
        public TimeStampTokenGeneratorBC(IPrivateKey pk, IX509Certificate cert,
            string allowedDigest, string policyOid) {
            try {
                timeStampTokenGenerator = new TimeStampTokenGenerator((AsymmetricKeyParameter)((PrivateKeyBC)pk)
                    .GetPrivateKey(), ((X509CertificateBC)cert).GetCertificate(), allowedDigest, policyOid);
            } catch (TspException e) {
                throw new TSPExceptionBC(e);
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
        public virtual void SetCertificates(IList<IX509Certificate> certificateChain) {
            timeStampTokenGenerator.SetCertificates(X509StoreFactory.Create("Certificate/Collection", 
                new X509CollectionStoreParameters(certificateChain.Select(certificate => 
                    ((X509CertificateBC)certificate).GetCertificate()).ToList())));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampToken Generate(ITimeStampRequest request, IBigInteger bigInteger, DateTime date) {
            try {
                return new TimeStampTokenBC(timeStampTokenGenerator.Generate(((TimeStampRequestBC)request).GetTimeStampRequest
                    (), ((BigIntegerBC)bigInteger).GetBigInteger(), date));
            } catch (TspException e) {
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
            iText.Bouncycastle.Tsp.TimeStampTokenGeneratorBC that = (iText.Bouncycastle.Tsp.TimeStampTokenGeneratorBC)
                o;
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
