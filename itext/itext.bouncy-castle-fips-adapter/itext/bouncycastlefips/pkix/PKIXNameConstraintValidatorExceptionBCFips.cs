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
using Org.BouncyCastle.Pkix;
using iText.Commons.Bouncycastle.Asn1.Pkix;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Pkix {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Pkix.PkixNameConstraintValidatorException"/>.
    /// </summary>
    public class PKIXNameConstraintValidatorExceptionBCFips : AbstractPKIXNameConstraintValidatorException {
        private readonly PkixNameConstraintValidatorException nameConstraintValidatorException;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TspException"/>.
        /// </summary>
        /// <param name="nameConstraintValidatorException">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TspException"/>
        /// to be wrapped
        /// </param>
        public PKIXNameConstraintValidatorExceptionBCFips(PkixNameConstraintValidatorException nameConstraintValidatorException
            )
            : base() {
            this.nameConstraintValidatorException = nameConstraintValidatorException;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Pkix.PkixNameConstraintValidatorException"/>.
        /// </returns>
        public virtual PkixNameConstraintValidatorException GetNameConstraintValidatorException() {
            return nameConstraintValidatorException;
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
            iText.Bouncycastlefips.Pkix.PKIXNameConstraintValidatorExceptionBCFips that = (iText.Bouncycastlefips.Pkix.PKIXNameConstraintValidatorExceptionBCFips
                )o;
            return Object.Equals(nameConstraintValidatorException, that.nameConstraintValidatorException);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(nameConstraintValidatorException);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return nameConstraintValidatorException.ToString();
        }

        /// <summary>
        /// Delegates
        /// <c>getMessage</c>
        /// method call to the wrapped exception.
        /// </summary>
        public override String Message {
            get {
                return nameConstraintValidatorException.Message;
            }
        }
    }
}
