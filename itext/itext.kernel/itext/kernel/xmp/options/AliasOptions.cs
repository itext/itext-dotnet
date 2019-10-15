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
    /// <summary>Options for XMPSchemaRegistryImpl#registerAlias.</summary>
    /// <since>20.02.2006</since>
    public sealed class AliasOptions : iText.Kernel.XMP.Options.Options {
        /// <summary>This is a direct mapping.</summary>
        /// <remarks>This is a direct mapping. The actual data type does not matter.</remarks>
        public const int PROP_DIRECT = 0;

        /// <summary>The actual is an unordered array, the alias is to the first element of the array.</summary>
        public const int PROP_ARRAY = PropertyOptions.ARRAY;

        /// <summary>The actual is an ordered array, the alias is to the first element of the array.</summary>
        public const int PROP_ARRAY_ORDERED = PropertyOptions.ARRAY_ORDERED;

        /// <summary>The actual is an alternate array, the alias is to the first element of the array.</summary>
        public const int PROP_ARRAY_ALTERNATE = PropertyOptions.ARRAY_ALTERNATE;

        /// <summary>The actual is an alternate text array, the alias is to the 'x-default' element of the array.</summary>
        public const int PROP_ARRAY_ALT_TEXT = PropertyOptions.ARRAY_ALT_TEXT;

        /// <seealso cref="Options.Options()"/>
        public AliasOptions() {
        }

        // EMPTY
        /// <param name="options">the options to init with</param>
        public AliasOptions(int options)
            : base(options) {
        }

        /// <returns>Returns if the alias is of the simple form.</returns>
        public bool IsSimple() {
            return GetOptions() == PROP_DIRECT;
        }

        /// <returns>Returns the option.</returns>
        public bool IsArray() {
            return GetOption(PROP_ARRAY);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.AliasOptions SetArray(bool value) {
            SetOption(PROP_ARRAY, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool IsArrayOrdered() {
            return GetOption(PROP_ARRAY_ORDERED);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.AliasOptions SetArrayOrdered(bool value) {
            SetOption(PROP_ARRAY | PROP_ARRAY_ORDERED, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool IsArrayAlternate() {
            return GetOption(PROP_ARRAY_ALTERNATE);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.AliasOptions SetArrayAlternate(bool value) {
            SetOption(PROP_ARRAY | PROP_ARRAY_ORDERED | PROP_ARRAY_ALTERNATE, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool IsArrayAltText() {
            return GetOption(PROP_ARRAY_ALT_TEXT);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.AliasOptions SetArrayAltText(bool value) {
            SetOption(PROP_ARRAY | PROP_ARRAY_ORDERED | PROP_ARRAY_ALTERNATE | PROP_ARRAY_ALT_TEXT, value);
            return this;
        }

        /// <returns>
        /// returns a
        /// <see cref="PropertyOptions"/>
        /// s object
        /// </returns>
        public PropertyOptions ToPropertyOptions() {
            return new PropertyOptions(GetOptions());
        }

        /// <seealso cref="Options.DefineOptionName(int)"/>
        protected internal override String DefineOptionName(int option) {
            switch (option) {
                case PROP_DIRECT: {
                    return "PROP_DIRECT";
                }

                case PROP_ARRAY: {
                    return "ARRAY";
                }

                case PROP_ARRAY_ORDERED: {
                    return "ARRAY_ORDERED";
                }

                case PROP_ARRAY_ALTERNATE: {
                    return "ARRAY_ALTERNATE";
                }

                case PROP_ARRAY_ALT_TEXT: {
                    return "ARRAY_ALT_TEXT";
                }

                default: {
                    return null;
                }
            }
        }

        /// <seealso cref="Options.GetValidOptions()"/>
        protected internal override int GetValidOptions() {
            return PROP_DIRECT | PROP_ARRAY | PROP_ARRAY_ORDERED | PROP_ARRAY_ALTERNATE | PROP_ARRAY_ALT_TEXT;
        }
    }
}
