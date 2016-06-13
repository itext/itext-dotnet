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

namespace iText.Kernel.XMP.Impl {
    /// <since>09.11.2006</since>
    public class QName {
        /// <summary>XML namespace prefix</summary>
        private String prefix;

        /// <summary>XML localname</summary>
        private String localName;

        /// <summary>Splits a qname into prefix and localname.</summary>
        /// <param name="qname">a QName</param>
        public QName(String qname) {
            int colon = qname.IndexOf(':');
            if (colon >= 0) {
                prefix = qname.JSubstring(0, colon);
                localName = qname.Substring(colon + 1);
            }
            else {
                prefix = "";
                localName = qname;
            }
        }

        /// <summary>Constructor that initializes the fields</summary>
        /// <param name="prefix">the prefix</param>
        /// <param name="localName">the name</param>
        public QName(String prefix, String localName) {
            this.prefix = prefix;
            this.localName = localName;
        }

        /// <returns>Returns whether the QName has a prefix.</returns>
        public virtual bool HasPrefix() {
            return prefix != null && prefix.Length > 0;
        }

        /// <returns>the localName</returns>
        public virtual String GetLocalName() {
            return localName;
        }

        /// <returns>the prefix</returns>
        public virtual String GetPrefix() {
            return prefix;
        }
    }
}
