/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
Authors: Apryse Software.

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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;

namespace iText.Bouncycastle.Cert {
    /// <summary>Wrapper for BouncyCastle cipher parameters.</summary>
    public class CipherParamsBC : ICipherParams {
        private readonly ICipherParameters cipherParameters;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="CipherParamsBC"/>
        /// with the specified BouncyCastle cipher parameters.
        /// </summary>
        /// <param name="cipherParameters">the BouncyCastle cipher parameters to wrap</param>
        public CipherParamsBC(ICipherParameters cipherParameters) {
            this.cipherParameters = cipherParameters;
        }

        /// <summary>Returns the wrapped BouncyCastle cipher parameters.</summary>
        /// <returns>the wrapped BouncyCastle cipher parameters</returns>
        public virtual ICipherParameters GetCipherParameters() {
            return cipherParameters;
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.CipherParamsBC that = (iText.Bouncycastle.Cert.CipherParamsBC)o;
            return Object.Equals(cipherParameters, that.cipherParameters);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(cipherParameters);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return this.cipherParameters.ToString();
        }
    }
}
