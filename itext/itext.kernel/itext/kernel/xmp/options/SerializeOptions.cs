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

namespace iText.Kernel.XMP.Options {
    /// <summary>
    /// Options for
    /// <see cref="iText.Kernel.XMP.XMPMetaFactory.SerializeToBuffer(iText.Kernel.XMP.XMPMeta, SerializeOptions)"/
    ///     >
    /// </summary>
    /// <since>24.01.2006</since>
    public sealed class SerializeOptions : iText.Kernel.XMP.Options.Options {
        /// <summary>Omit the XML packet wrapper.</summary>
        public const int OMIT_PACKET_WRAPPER = 0x0010;

        /// <summary>Mark packet as read-only.</summary>
        /// <remarks>Mark packet as read-only. Default is a writeable packet.</remarks>
        public const int READONLY_PACKET = 0x0020;

        /// <summary>Use a compact form of RDF.</summary>
        /// <remarks>
        /// Use a compact form of RDF.
        /// The compact form is the default serialization format (this flag is technically ignored).
        /// To serialize to the canonical form, set the flag USE_CANONICAL_FORMAT.
        /// If both flags &amp;quot;compact&amp;quot; and &amp;quot;canonical&amp;quot; are set, canonical is used.
        /// </remarks>
        public const int USE_COMPACT_FORMAT = 0x0040;

        /// <summary>Use the canonical form of RDF if set.</summary>
        /// <remarks>Use the canonical form of RDF if set. By default the compact form is used</remarks>
        public const int USE_CANONICAL_FORMAT = 0x0080;

        /// <summary>Include a padding allowance for a thumbnail image.</summary>
        /// <remarks>
        /// Include a padding allowance for a thumbnail image. If no <tt>xmp:Thumbnails</tt> property
        /// is present, the typical space for a JPEG thumbnail is used.
        /// </remarks>
        public const int INCLUDE_THUMBNAIL_PAD = 0x0100;

        /// <summary>The padding parameter provides the overall packet length.</summary>
        /// <remarks>
        /// The padding parameter provides the overall packet length. The actual amount of padding is
        /// computed. An exception is thrown if the packet exceeds this length with no padding.
        /// </remarks>
        public const int EXACT_PACKET_LENGTH = 0x0200;

        /// <summary>Omit the &lt;x:xmpmeta&gt;-tag</summary>
        public const int OMIT_XMPMETA_ELEMENT = 0x1000;

        /// <summary>Sort the struct properties and qualifier before serializing</summary>
        public const int SORT = 0x2000;

        // ---------------------------------------------------------------------------------------------
        // encoding bit constants
        /// <summary>Bit indicating little endian encoding, unset is big endian</summary>
        private const int LITTLEENDIAN_BIT = 0x0001;

        /// <summary>Bit indication UTF16 encoding.</summary>
        private const int UTF16_BIT = 0x0002;

        /// <summary>UTF8 encoding; this is the default</summary>
        public const int ENCODE_UTF8 = 0;

        /// <summary>UTF16BE encoding</summary>
        public const int ENCODE_UTF16BE = UTF16_BIT;

        /// <summary>UTF16LE encoding</summary>
        public const int ENCODE_UTF16LE = UTF16_BIT | LITTLEENDIAN_BIT;

        private const int ENCODING_MASK = UTF16_BIT | LITTLEENDIAN_BIT;

        /// <summary>The amount of padding to be added if a writeable XML packet is created.</summary>
        /// <remarks>
        /// The amount of padding to be added if a writeable XML packet is created. If zero is passed
        /// (the default) an appropriate amount of padding is computed.
        /// </remarks>
        private int padding = 2048;

        /// <summary>The string to be used as a line terminator.</summary>
        /// <remarks>
        /// The string to be used as a line terminator. If empty it defaults to; linefeed, U+000A, the
        /// standard XML newline.
        /// </remarks>
        private String newline = "\n";

        /// <summary>
        /// The string to be used for each level of indentation in the serialized
        /// RDF.
        /// </summary>
        /// <remarks>
        /// The string to be used for each level of indentation in the serialized
        /// RDF. If empty it defaults to two ASCII spaces, U+0020.
        /// </remarks>
        private String indent = "  ";

        /// <summary>
        /// The number of levels of indentation to be used for the outermost XML element in the
        /// serialized RDF.
        /// </summary>
        /// <remarks>
        /// The number of levels of indentation to be used for the outermost XML element in the
        /// serialized RDF. This is convenient when embedding the RDF in other text, defaults to 0.
        /// </remarks>
        private int baseIndent = 0;

        /// <summary>Omits the Toolkit version attribute, not published, only used for Unit tests.</summary>
        private bool omitVersionAttribute = false;

        /// <summary>Default constructor.</summary>
        public SerializeOptions() {
        }

        // reveal default constructor
        /// <summary>Constructor using inital options</summary>
        /// <param name="options">the inital options</param>
        public SerializeOptions(int options)
            : base(options) {
        }

        /// <returns>Returns the option.</returns>
        public bool GetOmitPacketWrapper() {
            return GetOption(OMIT_PACKET_WRAPPER);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetOmitPacketWrapper(bool value) {
            SetOption(OMIT_PACKET_WRAPPER, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetOmitXmpMetaElement() {
            return GetOption(OMIT_XMPMETA_ELEMENT);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetOmitXmpMetaElement(bool value) {
            SetOption(OMIT_XMPMETA_ELEMENT, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetReadOnlyPacket() {
            return GetOption(READONLY_PACKET);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetReadOnlyPacket(bool value) {
            SetOption(READONLY_PACKET, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetUseCompactFormat() {
            return GetOption(USE_COMPACT_FORMAT);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetUseCompactFormat(bool value) {
            SetOption(USE_COMPACT_FORMAT, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetUseCanonicalFormat() {
            return GetOption(USE_CANONICAL_FORMAT);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetUseCanonicalFormat(bool value) {
            SetOption(USE_CANONICAL_FORMAT, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetIncludeThumbnailPad() {
            return GetOption(INCLUDE_THUMBNAIL_PAD);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetIncludeThumbnailPad(bool value) {
            SetOption(INCLUDE_THUMBNAIL_PAD, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetExactPacketLength() {
            return GetOption(EXACT_PACKET_LENGTH);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetExactPacketLength(bool value) {
            SetOption(EXACT_PACKET_LENGTH, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetSort() {
            return GetOption(SORT);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetSort(bool value) {
            SetOption(SORT, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetEncodeUTF16BE() {
            return (GetOptions() & ENCODING_MASK) == ENCODE_UTF16BE;
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetEncodeUTF16BE(bool value) {
            // clear unicode bits
            SetOption(UTF16_BIT | LITTLEENDIAN_BIT, false);
            SetOption(ENCODE_UTF16BE, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetEncodeUTF16LE() {
            return (GetOptions() & ENCODING_MASK) == ENCODE_UTF16LE;
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetEncodeUTF16LE(bool value) {
            // clear unicode bits
            SetOption(UTF16_BIT | LITTLEENDIAN_BIT, false);
            SetOption(ENCODE_UTF16LE, value);
            return this;
        }

        /// <returns>Returns the baseIndent.</returns>
        public int GetBaseIndent() {
            return baseIndent;
        }

        /// <param name="baseIndent">The baseIndent to set.</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetBaseIndent(int baseIndent) {
            this.baseIndent = baseIndent;
            return this;
        }

        /// <returns>Returns the indent.</returns>
        public String GetIndent() {
            return indent;
        }

        /// <param name="indent">The indent to set.</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetIndent(String indent) {
            this.indent = indent;
            return this;
        }

        /// <returns>Returns the newline.</returns>
        public String GetNewline() {
            return newline;
        }

        /// <param name="newline">The newline to set.</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetNewline(String newline) {
            this.newline = newline;
            return this;
        }

        /// <returns>Returns the padding.</returns>
        public int GetPadding() {
            return padding;
        }

        /// <param name="padding">The padding to set.</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public iText.Kernel.XMP.Options.SerializeOptions SetPadding(int padding) {
            this.padding = padding;
            return this;
        }

        /// <returns>
        /// Returns whether the Toolkit version attribute shall be omitted.
        /// <em>Note:</em> This options can only be set by unit tests.
        /// </returns>
        public bool GetOmitVersionAttribute() {
            return omitVersionAttribute;
        }

        /// <returns>Returns the encoding as Java encoding String.</returns>
        public String GetEncoding() {
            if (GetEncodeUTF16BE()) {
                return "UTF-16BE";
            }
            else {
                if (GetEncodeUTF16LE()) {
                    return "UTF-16LE";
                }
                else {
                    return "UTF-8";
                }
            }
        }

        /// <returns>Returns clone of this SerializeOptions-object with the same options set.</returns>
        public Object Clone() {
            iText.Kernel.XMP.Options.SerializeOptions clone;
            try {
                clone = new iText.Kernel.XMP.Options.SerializeOptions(GetOptions());
                clone.SetBaseIndent(baseIndent);
                clone.SetIndent(indent);
                clone.SetNewline(newline);
                clone.SetPadding(padding);
                return clone;
            }
            catch (XMPException) {
                // This cannot happen, the options are already checked in "this" object.
                return null;
            }
        }

        /// <seealso cref="Options.DefineOptionName(int)"/>
        protected internal override String DefineOptionName(int option) {
            switch (option) {
                case OMIT_PACKET_WRAPPER: {
                    return "OMIT_PACKET_WRAPPER";
                }

                case READONLY_PACKET: {
                    return "READONLY_PACKET";
                }

                case USE_COMPACT_FORMAT: {
                    return "USE_COMPACT_FORMAT";
                }

                //			case USE_CANONICAL_FORMAT :		return "USE_CANONICAL_FORMAT";
                case INCLUDE_THUMBNAIL_PAD: {
                    return "INCLUDE_THUMBNAIL_PAD";
                }

                case EXACT_PACKET_LENGTH: {
                    return "EXACT_PACKET_LENGTH";
                }

                case OMIT_XMPMETA_ELEMENT: {
                    return "OMIT_XMPMETA_ELEMENT";
                }

                case SORT: {
                    return "NORMALIZED";
                }

                default: {
                    return null;
                }
            }
        }

        /// <seealso cref="Options.GetValidOptions()"/>
        protected internal override int GetValidOptions() {
            return OMIT_PACKET_WRAPPER | READONLY_PACKET | USE_COMPACT_FORMAT | 
                        //		USE_CANONICAL_FORMAT |
                        INCLUDE_THUMBNAIL_PAD | OMIT_XMPMETA_ELEMENT | EXACT_PACKET_LENGTH | SORT;
        }
    }
}
