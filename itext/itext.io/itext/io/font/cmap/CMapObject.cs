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
