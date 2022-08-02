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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.X509CRLHolder"/>.
    /// </summary>
    public class X509CRLHolderBCFips : IX509CRLHolder {
        private readonly X509CRLHolder x509CRLHolder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.X509CRLHolder"/>.
        /// </summary>
        /// <param name="x509CRLHolder">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.X509CRLHolder"/>
        /// to be wrapped
        /// </param>
        public X509CRLHolderBCFips(X509CRLHolder x509CRLHolder) {
            this.x509CRLHolder = x509CRLHolder;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.X509CRLHolder"/>.
        /// </returns>
        public virtual X509CRLHolder GetX509CRLHolder() {
            return x509CRLHolder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return x509CRLHolder.GetEncoded();
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
            iText.Bouncycastlefips.Cert.X509CRLHolderBCFips that = (iText.Bouncycastlefips.Cert.X509CRLHolderBCFips)o;
            return Object.Equals(x509CRLHolder, that.x509CRLHolder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(x509CRLHolder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return x509CRLHolder.ToString();
        }
    }
}
