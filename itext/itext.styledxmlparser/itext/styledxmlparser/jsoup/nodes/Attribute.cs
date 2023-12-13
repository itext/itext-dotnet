/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A single key + value attribute.</summary>
    /// <remarks>A single key + value attribute. Keys are trimmed and normalised to lower-case.</remarks>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class Attribute {
        private static readonly String[] booleanAttributes = new String[] { "allowfullscreen", "async", "autofocus"
            , "checked", "compact", "declare", "default", "defer", "disabled", "formnovalidate", "hidden", "inert"
            , "ismap", "itemscope", "multiple", "muted", "nohref", "noresize", "noshade", "novalidate", "nowrap", 
            "open", "readonly", "required", "reversed", "seamless", "selected", "sortable", "truespeed", "typemustmatch"
             };

        private String key;

        private String value;

        /// <summary>Create a new attribute from unencoded (raw) key and value.</summary>
        /// <param name="key">attribute key</param>
        /// <param name="value">attribute value</param>
        /// <seealso cref="CreateFromEncoded(System.String, System.String)"/>
        public Attribute(String key, String value) {
            Validate.NotEmpty(key);
            Validate.NotNull(value);
            this.key = key.Trim().ToLowerInvariant();
            this.value = value;
        }

        /// <summary>Get the attribute key.</summary>
        /// <returns>the attribute key</returns>
        public virtual String Key {
            get {
                return key;
            }
        }

        /// <summary>Set the attribute key.</summary>
        /// <remarks>Set the attribute key. Gets normalised as per the constructor method.</remarks>
        /// <param name="key">the new key; must not be null</param>
        public virtual void SetKey(String key) {
            Validate.NotEmpty(key);
            this.key = key.Trim().ToLowerInvariant();
        }

        /// <summary>Get the attribute value.</summary>
        /// <returns>the attribute value</returns>
        public virtual String Value {
            get {
                return value;
            }
        }

        /// <summary>Set the attribute value.</summary>
        /// <param name="value">the new attribute value; must not be null</param>
        public virtual String SetValue(String value) {
            Validate.NotNull(value);
            String old = this.value;
            this.value = value;
            return old;
        }

        /// <summary>Get the HTML representation of this attribute; e.g.</summary>
        /// <remarks>
        /// Get the HTML representation of this attribute; e.g.
        /// <c>href="index.html"</c>
        /// .
        /// </remarks>
        /// <returns>HTML</returns>
        public virtual String Html() {
            StringBuilder accum = new StringBuilder();
            try {
                Html(accum, (new Document("")).OutputSettings());
            }
            catch (System.IO.IOException exception) {
                throw new SerializationException(exception);
            }
            return accum.ToString();
        }

        protected internal virtual void Html(StringBuilder accum, OutputSettings @out) {
            accum.Append(key);
            if (!ShouldCollapseAttribute(@out)) {
                accum.Append("=\"");
                Entities.Escape(accum, value, @out, true, false, false);
                accum.Append('"');
            }
        }

        /// <summary>
        /// Get the string representation of this attribute, implemented as
        /// <see cref="Html()"/>
        /// .
        /// </summary>
        /// <returns>string</returns>
        public override String ToString() {
            return Html();
        }

        /// <summary>Create a new Attribute from an unencoded key and a HTML attribute encoded value.</summary>
        /// <param name="unencodedKey">assumes the key is not encoded, as can be only run of simple \w chars.</param>
        /// <param name="encodedValue">HTML attribute encoded value</param>
        /// <returns>attribute</returns>
        public static iText.StyledXmlParser.Jsoup.Nodes.Attribute CreateFromEncoded(String unencodedKey, String encodedValue) {
            String value = Entities.Unescape(encodedValue, true);
            return new iText.StyledXmlParser.Jsoup.Nodes.Attribute(unencodedKey, value);
        }

        protected internal virtual bool IsDataAttribute() {
            return key.StartsWith(Attributes.dataPrefix) && key.Length > Attributes.dataPrefix.Length;
        }

        /// <summary>Collapsible if it's a boolean attribute and value is empty or same as name</summary>
        /// <param name="out">Outputsettings</param>
        /// <returns>Returns whether collapsible or not</returns>
        protected internal bool ShouldCollapseAttribute(OutputSettings @out) {
            return ("".Equals(value) || value.EqualsIgnoreCase(key)) && @out.Syntax() == Syntax.html && IsBooleanAttribute
                ();
        }

        protected internal virtual bool IsBooleanAttribute() {
            return iText.IO.Util.JavaUtil.ArraysBinarySearch(booleanAttributes, key) >= 0;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (!(o is iText.StyledXmlParser.Jsoup.Nodes.Attribute)) {
                return false;
            }
            iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute = (iText.StyledXmlParser.Jsoup.Nodes.Attribute)o;
            if (key != null ? !key.Equals(attribute.key) : attribute.key != null) {
                return false;
            }
            return !(value != null ? !value.Equals(attribute.value) : attribute.value != null);
        }

        public override int GetHashCode() {
            int result = key != null ? key.GetHashCode() : 0;
            result = 31 * result + (value != null ? value.GetHashCode() : 0);
            return result;
        }

        public virtual Object Clone() {
            return MemberwiseClone();
        }
        // only fields are immutable strings key and value, so no more deep copy required
    }
}
