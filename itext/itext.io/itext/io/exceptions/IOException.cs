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
using System.Collections.Generic;
using iText.Commons.Exceptions;
using iText.Commons.Utils;

namespace iText.IO.Exceptions {
    /// <summary>Exception class for exceptions in io module.</summary>
    public class IOException : ITextException {
        /// <summary>Object for more details</summary>
        protected internal Object obj;

        private IList<Object> messageParams;

        /// <summary>Creates a new IOException.</summary>
        /// <param name="message">the detail message.</param>
        public IOException(String message)
            : base(message) {
        }

        /// <summary>Creates a new IOException.</summary>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public IOException(Exception cause)
            : this(IoExceptionMessageConstant.UNKNOWN_IO_EXCEPTION, cause) {
        }

        /// <summary>Creates a new IOException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="obj">an object for more details.</param>
        public IOException(String message, Object obj)
            : this(message) {
            this.obj = obj;
        }

        /// <summary>Creates a new IOException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public IOException(String message, Exception cause)
            : base(message, cause) {
        }

        /// <summary>Creates a new instance of IOException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        /// <param name="obj">an object for more details.</param>
        public IOException(String message, Exception cause, Object obj)
            : this(message, cause) {
            this.obj = obj;
        }

        /// <summary><inheritDoc/></summary>
        public override String Message {
            get {
                if (messageParams == null || messageParams.Count == 0) {
                    return base.Message;
                }
                else {
                    return MessageFormatUtil.Format(base.Message, GetMessageParams());
                }
            }
        }

        /// <summary>Gets additional params for Exception message.</summary>
        /// <returns>params for exception message.</returns>
        protected internal virtual Object[] GetMessageParams() {
            Object[] parameters = new Object[messageParams.Count];
            for (int i = 0; i < messageParams.Count; i++) {
                parameters[i] = messageParams[i];
            }
            return parameters;
        }

        /// <summary>Sets additional params for Exception message.</summary>
        /// <param name="messageParams">additional params.</param>
        /// <returns>object itself.</returns>
        public virtual iText.IO.Exceptions.IOException SetMessageParams(params Object[] messageParams) {
            this.messageParams = new List<Object>();
            this.messageParams.AddAll(messageParams);
            return this;
        }
    }
}
