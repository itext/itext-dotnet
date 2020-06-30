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

namespace iText.Kernel.XMP.Options {
    /// <summary>
    /// Options for
    /// <see cref="iText.Kernel.XMP.XMPMetaFactory.Parse(System.IO.Stream, ParseOptions)"/>.
    /// </summary>
    /// <since>24.01.2006</since>
    public sealed class ParseOptions : iText.Kernel.XMP.Options.Options {
        /// <summary>Require a surrounding &amp;quot;x:xmpmeta&amp;quot; element in the xml-document.</summary>
        public const int REQUIRE_XMP_META = 0x0001;

        /// <summary>Do not reconcile alias differences, throw an exception instead.</summary>
        public const int STRICT_ALIASING = 0x0004;

        /// <summary>Convert ASCII control characters 0x01 - 0x1F (except tab, cr, and lf) to spaces.</summary>
        public const int FIX_CONTROL_CHARS = 0x0008;

        /// <summary>If the input is not unicode, try to parse it as ISO-8859-1.</summary>
        public const int ACCEPT_LATIN_1 = 0x0010;

        /// <summary>Do not carry run the XMPNormalizer on a packet, leave it as it is.</summary>
        public const int OMIT_NORMALIZATION = 0x0020;

        /// <summary>Sets the options to the default values.</summary>
        public ParseOptions() {
            SetOption(FIX_CONTROL_CHARS | ACCEPT_LATIN_1, true);
        }

        /// <returns>Returns the requireXMPMeta.</returns>
        public bool GetRequireXMPMeta() {
            return GetOption(REQUIRE_XMP_META);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.ParseOptions SetRequireXMPMeta(bool value) {
            SetOption(REQUIRE_XMP_META, value);
            return this;
        }

        /// <returns>Returns the strictAliasing.</returns>
        public bool GetStrictAliasing() {
            return GetOption(STRICT_ALIASING);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.ParseOptions SetStrictAliasing(bool value) {
            SetOption(STRICT_ALIASING, value);
            return this;
        }

        /// <returns>Returns the strictAliasing.</returns>
        public bool GetFixControlChars() {
            return GetOption(FIX_CONTROL_CHARS);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.ParseOptions SetFixControlChars(bool value) {
            SetOption(FIX_CONTROL_CHARS, value);
            return this;
        }

        /// <returns>Returns the strictAliasing.</returns>
        public bool GetAcceptLatin1() {
            return GetOption(ACCEPT_LATIN_1);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.ParseOptions SetOmitNormalization(bool value) {
            SetOption(OMIT_NORMALIZATION, value);
            return this;
        }

        /// <returns>Returns the option "omit normalization".</returns>
        public bool GetOmitNormalization() {
            return GetOption(OMIT_NORMALIZATION);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.ParseOptions SetAcceptLatin1(bool value) {
            SetOption(ACCEPT_LATIN_1, value);
            return this;
        }

        /// <seealso cref="Options.DefineOptionName(int)"/>
        protected internal override String DefineOptionName(int option) {
            switch (option) {
                case REQUIRE_XMP_META: {
                    return "REQUIRE_XMP_META";
                }

                case STRICT_ALIASING: {
                    return "STRICT_ALIASING";
                }

                case FIX_CONTROL_CHARS: {
                    return "FIX_CONTROL_CHARS";
                }

                case ACCEPT_LATIN_1: {
                    return "ACCEPT_LATIN_1";
                }

                case OMIT_NORMALIZATION: {
                    return "OMIT_NORMALIZATION";
                }

                default: {
                    return null;
                }
            }
        }

        /// <seealso cref="Options.GetValidOptions()"/>
        protected internal override int GetValidOptions() {
            return REQUIRE_XMP_META | STRICT_ALIASING | FIX_CONTROL_CHARS | ACCEPT_LATIN_1 | OMIT_NORMALIZATION;
        }
    }
}
