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
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Math;
using iText.Bouncycastlefips.Asn1.X500;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.X509v2CRLBuilder"/>.
    /// </summary>
    public class X509v2CRLBuilderBCFips : IX509v2CRLBuilder {
        private readonly X509v2CRLBuilder builder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.X509v2CRLBuilder"/>.
        /// </summary>
        /// <param name="builder">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.X509v2CRLBuilder"/>
        /// to be wrapped
        /// </param>
        public X509v2CRLBuilderBCFips(X509v2CRLBuilder builder) {
            this.builder = builder;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.X509v2CRLBuilder"/>.
        /// </summary>
        /// <param name="x500Name">
        /// X500Name wrapper to create
        /// <see cref="Org.BouncyCastle.Cert.X509v2CRLBuilder"/>
        /// </param>
        /// <param name="date">
        /// Date to create
        /// <see cref="Org.BouncyCastle.Cert.X509v2CRLBuilder"/>
        /// </param>
        public X509v2CRLBuilderBCFips(IX500Name x500Name, DateTime date)
            : this(new X509v2CRLBuilder(((X500NameBCFips)x500Name).GetX500Name(), date)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.X509v2CRLBuilder"/>.
        /// </returns>
        public virtual X509v2CRLBuilder GetBuilder() {
            return builder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509v2CRLBuilder AddCRLEntry(BigInteger bigInteger, DateTime date, int i) {
            builder.AddCRLEntry(bigInteger, date, i);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509v2CRLBuilder SetNextUpdate(DateTime nextUpdate) {
            builder.SetNextUpdate(nextUpdate);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509CRLHolder Build(IContentSigner signer) {
            return new X509CRLHolderBCFips(builder.Build(((ContentSignerBCFips)signer).GetContentSigner()));
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
            iText.Bouncycastlefips.Cert.X509v2CRLBuilderBCFips that = (iText.Bouncycastlefips.Cert.X509v2CRLBuilderBCFips
                )o;
            return Object.Equals(builder, that.builder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(builder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return builder.ToString();
        }
    }
}
