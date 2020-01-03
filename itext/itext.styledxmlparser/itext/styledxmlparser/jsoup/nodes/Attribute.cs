/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
