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
using iText.Kernel.XMP;

namespace iText.Kernel.XMP.Impl {
//\cond DO_NOT_DOCUMENT
    /// <since>11.08.2006</since>
    internal class ParameterAsserts : XMPConst {
        /// <summary>private constructor</summary>
        private ParameterAsserts() {
        }

        // EMPTY
        /// <summary>Asserts that an array name is set.</summary>
        /// <param name="arrayName">an array name</param>
        public static void AssertArrayName(String arrayName) {
            if (arrayName == null || arrayName.Length == 0) {
                throw new XMPException("Empty array name", XMPError.BADPARAM);
            }
        }

        /// <summary>Asserts that a property name is set.</summary>
        /// <param name="propName">a property name or path</param>
        public static void AssertPropName(String propName) {
            if (propName == null || propName.Length == 0) {
                throw new XMPException("Empty property name", XMPError.BADPARAM);
            }
        }

        /// <summary>Asserts that a schema namespace is set.</summary>
        /// <param name="schemaNS">a schema namespace</param>
        public static void AssertSchemaNS(String schemaNS) {
            if (schemaNS == null || schemaNS.Length == 0) {
                throw new XMPException("Empty schema namespace URI", XMPError.BADPARAM);
            }
        }

        /// <summary>Asserts that a prefix is set.</summary>
        /// <param name="prefix">a prefix</param>
        public static void AssertPrefix(String prefix) {
            if (prefix == null || prefix.Length == 0) {
                throw new XMPException("Empty prefix", XMPError.BADPARAM);
            }
        }

        /// <summary>Asserts that a specific language is set.</summary>
        /// <param name="specificLang">a specific lang</param>
        public static void AssertSpecificLang(String specificLang) {
            if (specificLang == null || specificLang.Length == 0) {
                throw new XMPException("Empty specific language", XMPError.BADPARAM);
            }
        }

        /// <summary>Asserts that a struct name is set.</summary>
        /// <param name="structName">a struct name</param>
        public static void AssertStructName(String structName) {
            if (structName == null || structName.Length == 0) {
                throw new XMPException("Empty array name", XMPError.BADPARAM);
            }
        }

        /// <summary>Asserts that any string parameter is set.</summary>
        /// <param name="param">any string parameter</param>
        public static void AssertNotNull(Object param) {
            if (param == null) {
                throw new XMPException("Parameter must not be null", XMPError.BADPARAM);
            }
            else {
                if ((param is String) && ((String)param).Length == 0) {
                    throw new XMPException("Parameter must not be null or empty", XMPError.BADPARAM);
                }
            }
        }

        /// <summary>
        /// Asserts that the xmp object is of this implemention
        /// (
        /// <see cref="XMPMetaImpl"/>
        /// ).
        /// </summary>
        /// <param name="xmp">the XMP object</param>
        public static void AssertImplementation(XMPMeta xmp) {
            if (xmp == null) {
                throw new XMPException("Parameter must not be null", XMPError.BADPARAM);
            }
            else {
                if (!(xmp is XMPMetaImpl)) {
                    throw new XMPException("The XMPMeta-object is not compatible with this implementation", XMPError.BADPARAM);
                }
            }
        }
    }
//\endcond
}
