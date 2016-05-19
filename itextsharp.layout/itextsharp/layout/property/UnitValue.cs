/*
$Id$

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

namespace iTextSharp.Layout.Property
{
	/// <summary>A specialized class that holds a value and the unit it is measured in.</summary>
	public class UnitValue
	{
		public const int POINT = 1;

		public const int PERCENT = 2;

		protected internal int unitType;

		protected internal float value;

		/// <summary>Creates a UnitValue object with a specified type and value.</summary>
		/// <param name="unitType">
		/// either
		/// <see cref="POINT"/>
		/// or a
		/// <see cref="PERCENT"/>
		/// </param>
		/// <param name="value">the value to be stored.</param>
		public UnitValue(int unitType, float value)
		{
			this.unitType = unitType;
			this.value = value;
		}

		/// <summary>Creates a UnitValue POINT object with a specified value.</summary>
		/// <param name="value">the value to be stored.</param>
		/// <returns>
		/// a new
		/// <see cref="POINT"/>
		/// 
		/// <see cref="UnitValue"/>
		/// </returns>
		public static iTextSharp.Layout.Property.UnitValue CreatePointValue(float value)
		{
			return new iTextSharp.Layout.Property.UnitValue(POINT, value);
		}

		/// <summary>Creates a UnitValue PERCENT object with a specified value.</summary>
		/// <param name="value">the value to be stored.</param>
		/// <returns>
		/// a new
		/// <see cref="PERCENT"/>
		/// 
		/// <see cref="UnitValue"/>
		/// </returns>
		public static iTextSharp.Layout.Property.UnitValue CreatePercentValue(float value
			)
		{
			return new iTextSharp.Layout.Property.UnitValue(PERCENT, value);
		}

		public virtual int GetUnitType()
		{
			return unitType;
		}

		public virtual void SetUnitType(int unitType)
		{
			this.unitType = unitType;
		}

		public virtual float GetValue()
		{
			return value;
		}

		public virtual void SetValue(float value)
		{
			this.value = value;
		}

		public virtual bool IsPointValue()
		{
			return unitType == POINT;
		}

		public virtual bool IsPercentValue()
		{
			return unitType == PERCENT;
		}

		public override bool Equals(Object obj)
		{
			if (!(obj is iTextSharp.Layout.Property.UnitValue))
			{
				return false;
			}
			iTextSharp.Layout.Property.UnitValue other = (iTextSharp.Layout.Property.UnitValue
				)obj;
			return iTextSharp.IO.Util.JavaUtil.IntegerCompare(unitType, other.unitType) == 0 
				&& float?.Compare(value, other.value) == 0;
		}

		public override int GetHashCode()
		{
			int hash = 7;
			hash = 71 * hash + this.unitType;
			hash = 71 * hash + iTextSharp.IO.Util.JavaUtil.FloatToIntBits(this.value);
			return hash;
		}
	}
}
