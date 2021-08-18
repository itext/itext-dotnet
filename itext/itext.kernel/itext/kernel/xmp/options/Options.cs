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
using System.Collections;
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.XMP;

namespace iText.Kernel.XMP.Options {
    /// <summary>The base class for a collection of 32 flag bits.</summary>
    /// <remarks>
    /// The base class for a collection of 32 flag bits. Individual flags are defined as enum value bit
    /// masks. Inheriting classes add convenience accessor methods.
    /// </remarks>
    /// <since>24.01.2006</since>
    public abstract class Options {
        /// <summary>the internal int containing all options</summary>
        private int options = 0;

        /// <summary>a map containing the bit names</summary>
        private IDictionary optionNames = null;

        /// <summary>The default constructor.</summary>
        public Options() {
        }

        // EMTPY
        /// <summary>Constructor with the options bit mask.</summary>
        /// <param name="options">the options bit mask</param>
        public Options(int options) {
            AssertOptionsValid(options);
            SetOptions(options);
        }

        /// <summary>Resets the options.</summary>
        public virtual void Clear() {
            options = 0;
        }

        /// <param name="optionBits">an option bitmask</param>
        /// <returns>Returns true, if this object is equal to the given options.</returns>
        public virtual bool IsExactly(int optionBits) {
            return GetOptions() == optionBits;
        }

        /// <param name="optionBits">an option bitmask</param>
        /// <returns>Returns true, if this object contains all given options.</returns>
        public virtual bool ContainsAllOptions(int optionBits) {
            return (GetOptions() & optionBits) == optionBits;
        }

        /// <param name="optionBits">an option bitmask</param>
        /// <returns>Returns true, if this object contain at least one of the given options.</returns>
        public virtual bool ContainsOneOf(int optionBits) {
            return ((GetOptions()) & optionBits) != 0;
        }

        /// <param name="optionBit">the binary bit or bits that are requested</param>
        /// <returns>Returns if <b>all</b> of the requested bits are set or not.</returns>
        protected internal virtual bool GetOption(int optionBit) {
            return (options & optionBit) != 0;
        }

        /// <param name="optionBits">the binary bit or bits that shall be set to the given value</param>
        /// <param name="value">the boolean value to set</param>
        public virtual void SetOption(int optionBits, bool value) {
            options = value ? options | optionBits : options & ~optionBits;
        }

        /// <summary>Is friendly to access it during the tests.</summary>
        /// <returns>Returns the options.</returns>
        public virtual int GetOptions() {
            return options;
        }

        /// <param name="options">The options to set.</param>
        public virtual void SetOptions(int options) {
            AssertOptionsValid(options);
            this.options = options;
        }

        /// <seealso cref="System.Object.Equals(System.Object)"/>
        public override bool Equals(Object obj) {
            return GetOptions() == ((iText.Kernel.XMP.Options.Options)obj).GetOptions();
        }

        /// <seealso cref="System.Object.GetHashCode()"/>
        public override int GetHashCode() {
            return GetOptions();
        }

        /// <summary>Creates a human readable string from the set options.</summary>
        /// <remarks>
        /// Creates a human readable string from the set options. <em>Note:</em> This method is quite
        /// expensive and should only be used within tests or as
        /// </remarks>
        /// <returns>
        /// Returns a String listing all options that are set to <c>true</c> by their name,
        /// like &amp;quot;option1 | option4&amp;quot;.
        /// </returns>
        public virtual String GetOptionsString() {
            if (options != 0) {
                StringBuilder sb = new StringBuilder();
                int theBits = options;
                while (theBits != 0) {
                    int oneLessBit = theBits & (theBits - 1);
                    int singleBit = theBits ^ oneLessBit;
                    String bitName = GetOptionName(singleBit);
                    sb.Append(bitName);
                    if (oneLessBit != 0) {
                        sb.Append(" | ");
                    }
                    theBits = oneLessBit;
                }
                return sb.ToString();
            }
            else {
                return "<none>";
            }
        }

        /// <returns>Returns the options as hex bitmask.</returns>
        public override String ToString() {
            return "0x" + JavaUtil.IntegerToHexString(options);
        }

        /// <summary>To be implemeted by inheritants.</summary>
        /// <returns>Returns a bit mask where all valid option bits are set.</returns>
        protected internal abstract int GetValidOptions();

        /// <summary>To be implemeted by inheritants.</summary>
        /// <param name="option">a single, valid option bit.</param>
        /// <returns>Returns a human readable name for an option bit.</returns>
        protected internal abstract String DefineOptionName(int option);

        /// <summary>The inheriting option class can do additional checks on the options.</summary>
        /// <remarks>
        /// The inheriting option class can do additional checks on the options.
        /// <em>Note:</em> For performance reasons this method is only called
        /// when setting bitmasks directly.
        /// When get- and set-methods are used, this method must be called manually,
        /// normally only when the Options-object has been created from a client
        /// (it has to be made public therefore).
        /// </remarks>
        /// <param name="options">the bitmask to check.</param>
        protected internal virtual void AssertConsistency(int options) {
        }

        // empty, no checks
        /// <summary>Checks options before they are set.</summary>
        /// <remarks>
        /// Checks options before they are set.
        /// First it is checked if only defined options are used,
        /// second the additional
        /// <see cref="AssertConsistency(int)"/>
        /// -method is called.
        /// </remarks>
        /// <param name="options">the options to check</param>
        private void AssertOptionsValid(int options) {
            int invalidOptions = options & ~GetValidOptions();
            if (invalidOptions == 0) {
                AssertConsistency(options);
            }
            else {
                throw new XMPException("The option bit(s) 0x" + JavaUtil.IntegerToHexString(invalidOptions) + " are invalid!"
                    , XMPError.BADOPTIONS);
            }
        }

        /// <summary>Looks up or asks the inherited class for the name of an option bit.</summary>
        /// <remarks>
        /// Looks up or asks the inherited class for the name of an option bit.
        /// Its save that there is only one valid option handed into the method.
        /// </remarks>
        /// <param name="option">a single option bit</param>
        /// <returns>Returns the option name or undefined.</returns>
        private String GetOptionName(int option) {
            Hashtable optionsNames = ProcureOptionNames();
            int? key = option;
            String result = null;
            if (optionsNames.Contains(key)) {
                result = DefineOptionName(option);
                if (result != null) {
                    optionsNames.Put(key, result);
                }
                else {
                    result = "<option name not defined>";
                }
            }
            return result;
        }

        /// <returns>Returns the optionNames map and creates it if required.</returns>
        private Hashtable ProcureOptionNames() {
            if (optionNames == null) {
                optionNames = new Hashtable();
            }
            return (Hashtable)optionNames;
        }
    }
}
