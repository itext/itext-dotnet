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
using System.Text;
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Internal;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A single key + value attribute.</summary>
    /// <remarks>A single key + value attribute. (Only used for presentation.)</remarks>
    public class Attribute
#if !NETSTANDARD2_0
: ICloneable
#endif
 {
        private static readonly String[] booleanAttributes = new String[] { "allowfullscreen", "async", "autofocus"
            , "checked", "compact", "declare", "default", "defer", "disabled", "formnovalidate", "hidden", "inert"
            , "ismap", "itemscope", "multiple", "muted", "nohref", "noresize", "noshade", "novalidate", "nowrap", 
            "open", "readonly", "required", "reversed", "seamless", "selected", "sortable", "truespeed", "typemustmatch"
             };

        private String key;

        private String val;

        internal Attributes parent;

        // used to update the holding Attributes when the key / value is changed via this interface
        /// <summary>Create a new attribute from unencoded (raw) key and value.</summary>
        /// <param name="key">attribute key; case is preserved.</param>
        /// <param name="value">attribute value (may be null)</param>
        /// <seealso cref="CreateFromEncoded(System.String, System.String)"/>
        public Attribute(String key, String value)
            : this(key, value, null) {
        }

        /// <summary>Create a new attribute from unencoded (raw) key and value.</summary>
        /// <param name="key">attribute key; case is preserved.</param>
        /// <param name="val">attribute value (may be null)</param>
        /// <param name="parent">the containing Attributes (this Attribute is not automatically added to said Attributes)
        ///     </param>
        /// <seealso cref="CreateFromEncoded(System.String, System.String)"/>
        public Attribute(String key, String val, Attributes parent) {
            Validate.NotNull(key);
            key = key.Trim();
            Validate.NotEmpty(key);
            // trimming could potentially make empty, so validate here
            this.key = key;
            this.val = val;
            this.parent = parent;
        }

        /// <summary>Get the attribute key.</summary>
        /// <returns>the attribute key</returns>
        public virtual String Key {
            get {
                return key;
            }
        }

        /// <summary>Set the attribute key; case is preserved.</summary>
        /// <param name="key">the new key; must not be null</param>
        public virtual void SetKey(String key) {
            Validate.NotNull(key);
            key = key.Trim();
            Validate.NotEmpty(key);
            // trimming could potentially make empty, so validate here
            if (parent != null) {
                int i = parent.IndexOfKey(this.key);
                if (i != Attributes.NotFound) {
                    parent.keys[i] = key;
                }
            }
            this.key = key;
        }

        /// <summary>Get the attribute value.</summary>
        /// <remarks>Get the attribute value. Will return an empty string if the value is not set.</remarks>
        /// <returns>the attribute value</returns>
        public virtual String Value {
            get {
                return Attributes.CheckNotNull(val);
            }
        }

        /// <summary>Check if this Attribute has a value.</summary>
        /// <remarks>Check if this Attribute has a value. Set boolean attributes have no value.</remarks>
        /// <returns>if this is a boolean attribute / attribute without a value</returns>
        public virtual bool HasDeclaredValue() {
            return val != null;
        }

        /// <summary>Set the attribute value.</summary>
        /// <param name="val">the new attribute value; must not be null</param>
        public virtual String SetValue(String val) {
            String oldVal = this.val;
            if (parent != null) {
                oldVal = parent.Get(this.key);
                // trust the container more
                int i = parent.IndexOfKey(this.key);
                if (i != Attributes.NotFound) {
                    parent.vals[i] = val;
                }
            }
            this.val = val;
            return Attributes.CheckNotNull(oldVal);
        }

        /// <summary>
        /// Get the HTML representation of this attribute; e.g.
        /// <c>href="index.html"</c>.
        /// </summary>
        /// <returns>HTML</returns>
        public virtual String Html() {
            StringBuilder sb = Internal.StringUtil.BorrowBuilder();
            try {
                Html(sb, (new Document("")).OutputSettings());
            }
            catch (System.IO.IOException exception) {
                throw new SerializationException(exception);
            }
            return Internal.StringUtil.ReleaseBuilder(sb);
        }

        protected internal static void Html(String key, String val, StringBuilder accum, OutputSettings @out) {
            accum.Append(key);
            if (!ShouldCollapseAttribute(key, val, @out)) {
                accum.Append("=\"");
                Entities.Escape(accum, Attributes.CheckNotNull(val), @out, true, false, false);
                accum.Append('"');
            }
        }

        protected internal virtual void Html(StringBuilder accum, OutputSettings @out) {
            Html(key, val, accum, @out);
        }

        /// <summary>
        /// Get the string representation of this attribute, implemented as
        /// <see cref="Html()"/>.
        /// </summary>
        /// <returns>string</returns>
        public override String ToString() {
            return Html();
        }

        /// <summary>Create a new Attribute from an unencoded key and a HTML attribute encoded value.</summary>
        /// <param name="unencodedKey">assumes the key is not encoded, as can be only run of simple \w chars.</param>
        /// <param name="encodedValue">HTML attribute encoded value</param>
        /// <returns>attribute</returns>
        public static iText.StyledXmlParser.Jsoup.Nodes.Attribute CreateFromEncoded(String unencodedKey, String encodedValue
            ) {
            String value = Entities.Unescape(encodedValue, true);
            return new iText.StyledXmlParser.Jsoup.Nodes.Attribute(unencodedKey, value, null);
        }

        // parent will get set when Put
        protected internal virtual bool IsDataAttribute() {
            return IsDataAttribute(key);
        }

        protected internal static bool IsDataAttribute(String key) {
            return key.StartsWith(Attributes.dataPrefix) && key.Length > Attributes.dataPrefix.Length;
        }

        /// <summary>Collapsible if it's a boolean attribute and value is empty or same as name</summary>
        /// <param name="out">output settings</param>
        /// <returns>Returns whether collapsible or not</returns>
        protected internal bool ShouldCollapseAttribute(OutputSettings @out) {
            return ShouldCollapseAttribute(key, val, @out);
        }

        protected internal static bool ShouldCollapseAttribute(String key, String val, OutputSettings @out) {
            return (@out.Syntax() == iText.StyledXmlParser.Jsoup.Nodes.Syntax.html && (val == null || (String.IsNullOrEmpty
                (val) || val.EqualsIgnoreCase(key)) && iText.StyledXmlParser.Jsoup.Nodes.Attribute.IsBooleanAttribute(
                key)));
        }

        /// <summary>Checks if this attribute name is defined as a boolean attribute in HTML5</summary>
        protected internal static bool IsBooleanAttribute(String key) {
            return JavaUtil.ArraysBinarySearch(booleanAttributes, key) >= 0;
        }

        public override bool Equals(Object o) {
            // note parent not considered
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute = (iText.StyledXmlParser.Jsoup.Nodes.Attribute)o;
            if (key != null ? !key.Equals(attribute.key) : attribute.key != null) {
                return false;
            }
            return val != null ? val.Equals(attribute.val) : attribute.val == null;
        }

        public override int GetHashCode() {
            // note parent not considered
            int result = key != null ? key.GetHashCode() : 0;
            result = 31 * result + (val != null ? val.GetHashCode() : 0);
            return result;
        }

        public virtual Object Clone()
        {
            return MemberwiseClone();
        }
    }
}
