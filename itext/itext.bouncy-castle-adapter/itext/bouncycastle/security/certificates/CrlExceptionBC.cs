/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using Org.BouncyCastle.Security.Certificates;

namespace iText.Bouncycastle.Security {
    /// <summary>Wrapper class for <see cref="CrlException"/>.</summary>
    public class CrlExceptionBC : AbstractCrlException {
        private readonly CrlException exception;

        /// <summary>
        /// Creates new wrapper for <see cref="CrlException"/>.
        /// </summary>
        /// <param name="exception">
        /// <see cref="CrlException"/> to be wrapped
        /// </param>
        public CrlExceptionBC(CrlException exception) {
            this.exception = exception;
        }

        /// <summary>Get actual org.bouncycastle object being wrapped.</summary>
        /// <returns>wrapped <see cref="CrlException"/>.</returns>
        public CrlException GetException() {
            return exception;
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
            CrlExceptionBC that = (CrlExceptionBC)o;
            return Object.Equals(exception, that.exception);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(exception);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return exception.ToString();
        }

        /// <summary>
        /// Delegates
        /// <c>getMessage</c>
        /// method call to the wrapped exception.
        /// </summary>
        public override String Message => exception.Message;
    }
}
