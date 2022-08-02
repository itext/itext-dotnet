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
using Org.BouncyCastle.Cms.Jcajce;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.X509;
using iText.Bouncycastlefips.Cms;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Cms.Jcajce;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms.Jcajce {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>.
    /// </summary>
    public class JcaSignerInfoGeneratorBuilderBCFips : IJcaSignerInfoGeneratorBuilder {
        private readonly JcaSignerInfoGeneratorBuilder jcaSignerInfoGeneratorBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>.
        /// </summary>
        /// <param name="jcaSignerInfoGeneratorBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>
        /// to be wrapped
        /// </param>
        public JcaSignerInfoGeneratorBuilderBCFips(JcaSignerInfoGeneratorBuilder jcaSignerInfoGeneratorBuilder) {
            this.jcaSignerInfoGeneratorBuilder = jcaSignerInfoGeneratorBuilder;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>.
        /// </summary>
        /// <param name="calculatorProvider">
        /// DigestCalculatorProvider wrapper to create
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>
        /// </param>
        public JcaSignerInfoGeneratorBuilderBCFips(IDigestCalculatorProvider calculatorProvider)
            : this(new JcaSignerInfoGeneratorBuilder(((DigestCalculatorProviderBCFips)calculatorProvider).GetCalculatorProvider
                ())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSignerInfoGeneratorBuilder"/>.
        /// </returns>
        public virtual JcaSignerInfoGeneratorBuilder GetJcaSignerInfoGeneratorBuilder() {
            return jcaSignerInfoGeneratorBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignerInfoGenerator Build(IContentSigner signer, X509Certificate cert) {
            try {
                return new SignerInfoGeneratorBCFips(jcaSignerInfoGeneratorBuilder.Build(((ContentSignerBCFips)signer).GetContentSigner
                    (), cert));
            }
            catch (OperatorCreationException e) {
                throw new OperatorCreationExceptionBCFips(e);
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
            iText.Bouncycastlefips.Cms.Jcajce.JcaSignerInfoGeneratorBuilderBCFips that = (iText.Bouncycastlefips.Cms.Jcajce.JcaSignerInfoGeneratorBuilderBCFips
                )o;
            return Object.Equals(jcaSignerInfoGeneratorBuilder, that.jcaSignerInfoGeneratorBuilder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(jcaSignerInfoGeneratorBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return jcaSignerInfoGeneratorBuilder.ToString();
        }
    }
}
