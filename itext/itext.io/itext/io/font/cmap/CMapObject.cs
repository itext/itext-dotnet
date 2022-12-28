/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using System.Text;

namespace iText.IO.Font.Cmap {
    public class CMapObject {
        protected internal const int STRING = 1;

        protected internal const int HEX_STRING = 2;

        protected internal const int NAME = 3;

        protected internal const int NUMBER = 4;

        protected internal const int LITERAL = 5;

        protected internal const int ARRAY = 6;

        protected internal const int DICTIONARY = 7;

        protected internal const int TOKEN = 8;

        private int type;

        private Object value;

        public CMapObject(int objectType, Object value) {
            this.type = objectType;
            this.value = value;
        }

        public virtual Object GetValue() {
            return value;
        }

        public virtual int GetObjectType() {
            return type;
        }

        public virtual void SetValue(Object value) {
            this.value = value;
        }

        public virtual bool IsString() {
            return type == STRING || type == HEX_STRING;
        }

        public virtual bool IsHexString() {
            return type == HEX_STRING;
        }

        public virtual bool IsName() {
            return type == NAME;
        }

        public virtual bool IsNumber() {
            return type == NUMBER;
        }

        public virtual bool IsLiteral() {
            return type == LITERAL;
        }

        public virtual bool IsArray() {
            return type == ARRAY;
        }

        public virtual bool IsDictionary() {
            return type == DICTIONARY;
        }

        public virtual bool IsToken() {
            return type == TOKEN;
        }

        /// <summary>
        /// Return String representation of
        /// <c>value</c>
        /// field.
        /// </summary>
        public override String ToString() {
            if (type == STRING || type == HEX_STRING) {
                byte[] content = (byte[])value;
                StringBuilder str = new StringBuilder(content.Length);
                foreach (byte b in content) {
                    str.Append((char)(b & 0xff));
                }
                return str.ToString();
            }
            return value.ToString();
        }

        public virtual byte[] ToHexByteArray() {
            if (type == HEX_STRING) {
                return (byte[])value;
            }
            else {
                return null;
            }
        }
    }
}
