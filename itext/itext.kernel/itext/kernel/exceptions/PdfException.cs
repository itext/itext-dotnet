/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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

namespace iText.Kernel.Exceptions {
    /// <summary>Exception class for exceptions in kernel module.</summary>
    public class PdfException : ITextException {
        /// <summary>Object for more details</summary>
        protected internal Object @object;

        private IList<Object> messageParams;

        /// <summary>Creates a new instance of PdfException.</summary>
        /// <param name="message">the detail message.</param>
        public PdfException(String message)
            : base(message) {
        }

        /// <summary>Creates a new instance of PdfException.</summary>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public PdfException(Exception cause)
            : this(KernelExceptionMessageConstant.UNKNOWN_PDF_EXCEPTION, cause) {
        }

        /// <summary>Creates a new instance of PdfException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="obj">an object for more details.</param>
        public PdfException(String message, Object obj)
            : this(message) {
            this.@object = obj;
        }

        /// <summary>Creates a new instance of PdfException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public PdfException(String message, Exception cause)
            : base(message, cause) {
        }

        /// <summary>Creates a new instance of PdfException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        /// <param name="obj">an object for more details.</param>
        public PdfException(String message, Exception cause, Object obj)
            : this(message, cause) {
            this.@object = obj;
        }

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

        /// <summary>Sets additional params for Exception message.</summary>
        /// <param name="messageParams">additional params.</param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Exceptions.PdfException SetMessageParams(params Object[] messageParams) {
            this.messageParams = new List<Object>();
            this.messageParams.AddAll(messageParams);
            return this;
        }

        /// <summary>Gets additional params for Exception message.</summary>
        /// <returns>array of additional params</returns>
        protected internal virtual Object[] GetMessageParams() {
            Object[] parameters = new Object[messageParams.Count];
            for (int i = 0; i < messageParams.Count; i++) {
                parameters[i] = messageParams[i];
            }
            return parameters;
        }
    }
}
