//Copyright (c) 2006, Adobe Systems Incorporated
//All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//        1. Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//        2. Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//        3. All advertising materials mentioning features or use of this software
//        must display the following acknowledgement:
//        This product includes software developed by the Adobe Systems Incorporated.
//        4. Neither the name of the Adobe Systems Incorporated nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED BY ADOBE SYSTEMS INCORPORATED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL ADOBE SYSTEMS INCORPORATED BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//        http://www.adobe.com/devnet/xmp/library/eula-xmp-library-java.html
using System;

namespace iText.Kernel.XMP {
    /// <summary>This exception wraps all errors that occur in the XMP Toolkit.</summary>
    /// <since>16.02.2006</since>
    public class XMPException : Exception {
        /// <summary>the errorCode of the XMP toolkit</summary>
        private int errorCode;

        /// <summary>Constructs an exception with a message and an error code.</summary>
        /// <param name="message">the message</param>
        /// <param name="errorCode">the error code</param>
        public XMPException(String message, int errorCode)
            : base(message) {
            this.errorCode = errorCode;
        }

        /// <summary>Constructs an exception with a message, an error code and a <c>Throwable</c></summary>
        /// <param name="message">the error message.</param>
        /// <param name="errorCode">the error code</param>
        /// <param name="t">the exception source</param>
        public XMPException(String message, int errorCode, Exception t)
            : base(message, t) {
            this.errorCode = errorCode;
        }

        /// <returns>Returns the errorCode.</returns>
        public virtual int GetErrorCode() {
            return errorCode;
        }
    }
}
