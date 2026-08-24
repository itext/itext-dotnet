/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
    /// <summary>Represents a typed object parsed from CMap content.</summary>
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

        /// <summary>Creates a typed CMap object.</summary>
        /// <param name="objectType">one of this class's object-type constants</param>
        /// <param name="value">the value associated with the type</param>
        public CMapObject(int objectType, Object value) {
            this.type = objectType;
            this.value = value;
        }

        /// <summary>Returns the value represented by this object.</summary>
        /// <returns>the stored value</returns>
        public virtual Object GetValue() {
            return value;
        }

        /// <summary>Returns this object's type code.</summary>
        /// <returns>one of this class's object type constants</returns>
        public virtual int GetObjectType() {
            return type;
        }

        /// <summary>Replaces this object's stored value.</summary>
        /// <param name="value">the new value</param>
        public virtual void SetValue(Object value) {
            this.value = value;
        }

        /// <summary>Tests whether this object is a literal or hexadecimal string.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// for either string type
        /// </returns>
        public virtual bool IsString() {
            return type == STRING || type == HEX_STRING;
        }

        /// <summary>Tests whether this object is a hexadecimal string.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when this object's type is hexadecimal string
        /// </returns>
        public virtual bool IsHexString() {
            return type == HEX_STRING;
        }

        /// <summary>Tests whether this object is a PDF name.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when this object's type is name
        /// </returns>
        public virtual bool IsName() {
            return type == NAME;
        }

        /// <summary>Tests whether this object is a number.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when this object's type is number
        /// </returns>
        public virtual bool IsNumber() {
            return type == NUMBER;
        }

        /// <summary>Tests whether this object is a command literal.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when this object's type is literal
        /// </returns>
        public virtual bool IsLiteral() {
            return type == LITERAL;
        }

        /// <summary>Tests whether this object is an array.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when this object's type is array
        /// </returns>
        public virtual bool IsArray() {
            return type == ARRAY;
        }

        /// <summary>Tests whether this object is a dictionary.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when this object's type is dictionary
        /// </returns>
        public virtual bool IsDictionary() {
            return type == DICTIONARY;
        }

        /// <summary>Tests whether this object is a structural token.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when this object's type is token
        /// </returns>
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

        /// <summary>Returns the byte array of a hexadecimal string object.</summary>
        /// <returns>
        /// the retained byte array, or
        /// <see langword="null"/>
        /// when this is not a hexadecimal string
        /// </returns>
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
