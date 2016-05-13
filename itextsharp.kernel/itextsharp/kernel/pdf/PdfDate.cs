/*
$Id: 4cc720577348f5cc763bf83a8271961680f94122 $

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
using System.Globalization;
using System.Text;

namespace iTextSharp.Kernel.Pdf
{
	/// <summary>
	/// <c>PdfDate</c>
	/// is the PDF date object.
	/// <P>
	/// PDF defines a standard date format. The PDF date format closely follows the format
	/// defined by the international standard ASN.1 (Abstract Syntax Notation One, defined
	/// in CCITT X.208 or ISO/IEC 8824). A date is a
	/// <c>PdfString</c>
	/// of the form:
	/// <P><BLOCKQUOTE>
	/// (D:YYYYMMDDHHmmSSOHH'mm')
	/// </BLOCKQUOTE><P>
	/// This object is described in the 'Portable Document Format Reference Manual version 1.3'
	/// section 7.2 (page 183-184)
	/// </summary>
	/// <seealso cref="PdfString"/>
	/// <seealso cref="System.Globalization.GregorianCalendar"/>
	public class PdfDate : PdfObjectWrapper<PdfString>
	{
		private const long serialVersionUID = -7424858548790000216L;

		private static readonly int[] DATE_SPACE = new int[] { Calendar.YEAR, 4, 0, Calendar
			.MONTH, 2, -1, Calendar.DAY_OF_MONTH, 2, 0, Calendar.HOUR_OF_DAY, 2, 0, Calendar
			.MINUTE, 2, 0, Calendar.SECOND, 2, 0 };

		/// <summary>
		/// Constructs a
		/// <c>PdfDate</c>
		/// -object.
		/// </summary>
		/// <param name="d">
		/// the date that has to be turned into a
		/// <c>PdfDate</c>
		/// &gt;-object
		/// </param>
		public PdfDate(Calendar d)
			: base(new PdfString(GenerateStringByCalendar(d)))
		{
		}

		/// <summary>
		/// Constructs a
		/// <c>PdfDate</c>
		/// -object, representing the current day and time.
		/// </summary>
		public PdfDate()
			: this(new GregorianCalendar())
		{
		}

		/// <summary>Gives the W3C format of the PdfDate.</summary>
		/// <returns>a formatted date</returns>
		public virtual String GetW3CDate()
		{
			return GetW3CDate(GetPdfObject().GetValue());
		}

		/// <summary>
		/// Gives the W3C format of the
		/// <c>PdfDate</c>
		/// .
		/// </summary>
		/// <param name="d">the date in the format D:YYYYMMDDHHmmSSOHH'mm'</param>
		/// <returns>a formatted date</returns>
		public static String GetW3CDate(String d)
		{
			if (d.StartsWith("D:"))
			{
				d = d.Substring(2);
			}
			StringBuilder sb = new StringBuilder();
			if (d.Length < 4)
			{
				return "0000";
			}
			sb.Append(d.JSubstring(0, 4));
			//year
			d = d.Substring(4);
			if (d.Length < 2)
			{
				return sb.ToString();
			}
			sb.Append('-').Append(d.JSubstring(0, 2));
			//month
			d = d.Substring(2);
			if (d.Length < 2)
			{
				return sb.ToString();
			}
			sb.Append('-').Append(d.JSubstring(0, 2));
			//day
			d = d.Substring(2);
			if (d.Length < 2)
			{
				return sb.ToString();
			}
			sb.Append('T').Append(d.JSubstring(0, 2));
			//hour
			d = d.Substring(2);
			if (d.Length < 2)
			{
				sb.Append(":00Z");
				return sb.ToString();
			}
			sb.Append(':').Append(d.JSubstring(0, 2));
			//minute
			d = d.Substring(2);
			if (d.Length < 2)
			{
				sb.Append('Z');
				return sb.ToString();
			}
			sb.Append(':').Append(d.JSubstring(0, 2));
			//second
			d = d.Substring(2);
			if (d.StartsWith("-") || d.StartsWith("+"))
			{
				String sign = d.JSubstring(0, 1);
				d = d.Substring(1);
				if (d.Length >= 2)
				{
					String h = d.JSubstring(0, 2);
					String m = "00";
					if (d.Length > 2)
					{
						d = d.Substring(3);
						if (d.Length >= 2)
						{
							m = d.JSubstring(0, 2);
						}
					}
					sb.Append(sign).Append(h).Append(':').Append(m);
					return sb.ToString();
				}
			}
			sb.Append('Z');
			return sb.ToString();
		}

		/// <summary>
		/// Converts a PDF string representing a date into a
		/// <c>Calendar</c>
		/// .
		/// </summary>
		/// <param name="s">the PDF string representing a date</param>
		/// <returns>
		/// a
		/// <c>Calendar</c>
		/// representing the date or
		/// <see langword="null"/>
		/// if the string
		/// was not a date
		/// </returns>
		public static Calendar Decode(String s)
		{
			try
			{
				if (s.StartsWith("D:"))
				{
					s = s.Substring(2);
				}
				GregorianCalendar calendar;
				int slen = s.Length;
				int idx = s.IndexOf('Z');
				if (idx >= 0)
				{
					slen = idx;
					calendar = new GregorianCalendar(new SimpleTimeZone(0, "ZPDF"));
				}
				else
				{
					int sign = 1;
					idx = s.IndexOf('+');
					if (idx < 0)
					{
						idx = s.IndexOf('-');
						if (idx >= 0)
						{
							sign = -1;
						}
					}
					if (idx < 0)
					{
						calendar = new GregorianCalendar();
					}
					else
					{
						int offset = System.Convert.ToInt32(s.JSubstring(idx + 1, idx + 3)) * 60;
						if (idx + 5 < s.Length)
						{
							offset += System.Convert.ToInt32(s.JSubstring(idx + 4, idx + 6));
						}
						calendar = new GregorianCalendar(new SimpleTimeZone(offset * sign * 60000, "ZPDF"
							));
						slen = idx;
					}
				}
				calendar.Clear();
				idx = 0;
				for (int k = 0; k < DATE_SPACE.Length; k += 3)
				{
					if (idx >= slen)
					{
						break;
					}
					calendar.Set(DATE_SPACE[k], System.Convert.ToInt32(s.JSubstring(idx, idx + DATE_SPACE
						[k + 1])) + DATE_SPACE[k + 2]);
					idx += DATE_SPACE[k + 1];
				}
				return calendar;
			}
			catch (Exception)
			{
				return null;
			}
		}

		protected internal override bool IsWrappedObjectMustBeIndirect()
		{
			return false;
		}

		private static String GenerateStringByCalendar(Calendar d)
		{
			StringBuilder date = new StringBuilder("D:");
			date.Append(SetLength(d.Get(Calendar.YEAR), 4));
			date.Append(SetLength(d.Get(Calendar.MONTH) + 1, 2));
			date.Append(SetLength(d.Get(Calendar.DATE), 2));
			date.Append(SetLength(d.Get(Calendar.HOUR_OF_DAY), 2));
			date.Append(SetLength(d.Get(Calendar.MINUTE), 2));
			date.Append(SetLength(d.Get(Calendar.SECOND), 2));
			int timezone = (d.Get(Calendar.ZONE_OFFSET) + d.Get(Calendar.DST_OFFSET)) / (60 *
				 60 * 1000);
			if (timezone == 0)
			{
				date.Append('Z');
			}
			else
			{
				if (timezone < 0)
				{
					date.Append('-');
					timezone = -timezone;
				}
				else
				{
					date.Append('+');
				}
			}
			if (timezone != 0)
			{
				date.Append(SetLength(timezone, 2)).Append('\'');
				int zone = Math.Abs((d.Get(Calendar.ZONE_OFFSET) + d.Get(Calendar.DST_OFFSET)) / 
					(60 * 1000)) - (timezone * 60);
				date.Append(SetLength(zone, 2)).Append('\'');
			}
			return date.ToString();
		}

		/// <summary>
		/// Adds a number of leading zeros to a given
		/// <c>String</c>
		/// in order to get a
		/// <c>String</c>
		/// of a certain length.
		/// </summary>
		/// <param name="i">a given number</param>
		/// <param name="length">
		/// the length of the resulting
		/// <c>String</c>
		/// </param>
		/// <returns>
		/// the resulting
		/// <c>String</c>
		/// </returns>
		private static String SetLength(int i, int length)
		{
			// 1.3-1.4 problem fixed by Finn Bock
			StringBuilder tmp = new StringBuilder();
			tmp.Append(i);
			while (tmp.Length < length)
			{
				tmp.Insert(0, "0");
			}
			tmp.Length = length;
			return tmp.ToString();
		}
	}
}
