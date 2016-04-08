/*
$Id: dcdd09aadb2139c3701a607ae1de54c5d5dbe63b $

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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

namespace com.itextpdf.io.font.cmap
{
	public class CMapObject
	{
		protected internal const int String = 1;

		protected internal const int HexString = 2;

		protected internal const int Name = 3;

		protected internal const int Number = 4;

		protected internal const int Literal = 5;

		protected internal const int Array = 6;

		protected internal const int Dictionary = 7;

		protected internal const int Token = 8;

		private int type;

		private Object value;

		public CMapObject(int objectType, Object value)
		{
			this.type = objectType;
			this.value = value;
		}

		public virtual Object GetValue()
		{
			return value;
		}

		public virtual int GetType()
		{
			return type;
		}

		public virtual void SetValue(Object value)
		{
			this.value = value;
		}

		public virtual bool IsString()
		{
			return type == String || type == HexString;
		}

		public virtual bool IsHexString()
		{
			return type == HexString;
		}

		public virtual bool IsName()
		{
			return type == Name;
		}

		public virtual bool IsNumber()
		{
			return type == Number;
		}

		public virtual bool IsLiteral()
		{
			return type == Literal;
		}

		public virtual bool IsArray()
		{
			return type == Array;
		}

		public virtual bool IsDictionary()
		{
			return type == Dictionary;
		}

		public virtual bool IsToken()
		{
			return type == Token;
		}

		/// <summary>
		/// Return String representation of
		/// <c>value</c>
		/// field.
		/// </summary>
		public override String ToString()
		{
			if (type == String || type == HexString)
			{
				byte[] content = (byte[])value;
				StringBuilder str = new StringBuilder(content.Length);
				foreach (byte b in content)
				{
					str.Append((char)(b & 0xff));
				}
				return str.ToString();
			}
			return value.ToString();
		}
	}
}
