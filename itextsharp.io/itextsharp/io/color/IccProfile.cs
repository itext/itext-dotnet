/*
* $Id: 4b4532cc748a75b1fdce67479766b6219bdf512c $
*
* This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
* Authors: Bruno Lowagie, Paulo Soares, et al.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License version 3
* as published by the Free Software Foundation with the addition of the
* following permission added to Section 15 as permitted in Section 7(a):
* FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
* ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
* OF THIRD PARTY RIGHTS
*
* This program is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
* or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU Affero General Public License for more details.
* You should have received a copy of the GNU Affero General Public License
* along with this program; if not, see http://www.gnu.org/licenses or write to
* the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
* Boston, MA, 02110-1301 USA, or download the license from the following URL:
* http://itextpdf.com/terms-of-use/
*
* The interactive user interfaces in modified source and object code versions
* of this program must display Appropriate Legal Notices, as required under
* Section 5 of the GNU Affero General Public License.
*
* In accordance with Section 7(b) of the GNU Affero General Public License,
* a covered work must retain the producer line in every PDF that is created
* or manipulated using iText.
*
* You can be released from the requirements of the license by purchasing
* a commercial license. Buying such a license is mandatory as soon as you
* develop commercial activities involving the iText software without
* disclosing the source code of your own applications.
* These activities include: offering paid services to customers as an ASP,
* serving PDFs on the fly in a web application, shipping iText with a closed
* source product.
*
* For more information, please contact iText Software Corp. at this
* address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using com.itextpdf.io;
using com.itextpdf.io.source;

namespace com.itextpdf.io.color
{
	public class IccProfile
	{
		protected internal byte[] data;

		protected internal int numComponents;

		private static IDictionary<String, int> cstags = new Dictionary<String, int>();

		protected internal IccProfile()
		{
		}

		public static com.itextpdf.io.color.IccProfile GetInstance(byte[] data, int numComponents
			)
		{
			if (data.Length < 128 || data[36] != 0x61 || data[37] != 0x63 || data[38] != 0x73
				 || data[39] != 0x70)
			{
				throw new IOException(IOException.InvalidIccProfile);
			}
			com.itextpdf.io.color.IccProfile icc = new com.itextpdf.io.color.IccProfile();
			icc.data = data;
			int cs;
			cs = GetIccNumberOfComponents(data);
			int nc = cs == null ? 0 : cs;
			icc.numComponents = nc;
			// invalid ICC
			if (nc != numComponents)
			{
				throw new IOException(IOException.WrongNumberOfComponentsInIccProfile).SetMessageParams
					(nc, numComponents);
			}
			return icc;
		}

		public static com.itextpdf.io.color.IccProfile GetInstance(byte[] data)
		{
			int cs;
			cs = GetIccNumberOfComponents(data);
			int numComponents = cs == null ? 0 : cs;
			return GetInstance(data, numComponents);
		}

		public static com.itextpdf.io.color.IccProfile GetInstance(RandomAccessFileOrArray
			 file)
		{
			try
			{
				byte[] head = new byte[128];
				int remain = head.Length;
				int ptr = 0;
				while (remain > 0)
				{
					int n = file.Read(head, ptr, remain);
					if (n < 0)
					{
						throw new IOException(IOException.InvalidIccProfile);
					}
					remain -= n;
					ptr += n;
				}
				if (head[36] != 0x61 || head[37] != 0x63 || head[38] != 0x73 || head[39] != 0x70)
				{
					throw new IOException(IOException.InvalidIccProfile);
				}
				remain = (head[0] & unchecked((int)(0xff))) << 24 | (head[1] & unchecked((int)(0xff
					))) << 16 | (head[2] & unchecked((int)(0xff))) << 8 | head[3] & unchecked((int)(
					0xff));
				byte[] icc = new byte[remain];
				System.Array.Copy(head, 0, icc, 0, head.Length);
				remain -= head.Length;
				ptr = head.Length;
				while (remain > 0)
				{
					int n = file.Read(icc, ptr, remain);
					if (n < 0)
					{
						throw new IOException(IOException.InvalidIccProfile);
					}
					remain -= n;
					ptr += n;
				}
				return GetInstance(icc);
			}
			catch (Exception ex)
			{
				throw new IOException(IOException.InvalidIccProfile, ex);
			}
		}

		public static com.itextpdf.io.color.IccProfile GetInstance(Stream stream)
		{
			RandomAccessFileOrArray raf;
			try
			{
				raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(stream
					));
			}
			catch (System.IO.IOException e)
			{
				throw new IOException(IOException.InvalidIccProfile, e);
			}
			return GetInstance(raf);
		}

		public static com.itextpdf.io.color.IccProfile GetInstance(String filename)
		{
			RandomAccessFileOrArray raf;
			try
			{
				raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource
					(filename));
			}
			catch (System.IO.IOException e)
			{
				throw new IOException(IOException.InvalidIccProfile, e);
			}
			return GetInstance(raf);
		}

		public static String GetIccColorSpaceName(byte[] data)
		{
			String colorSpace;
			try
			{
				colorSpace = com.itextpdf.io.util.JavaUtil.GetStringForBytes(data, 16, 4, "US-ASCII"
					);
			}
			catch (ArgumentException e)
			{
				throw new IOException(IOException.InvalidIccProfile, e);
			}
			return colorSpace;
		}

		public static String GetIccDeviceClass(byte[] data)
		{
			String deviceClass;
			try
			{
				deviceClass = com.itextpdf.io.util.JavaUtil.GetStringForBytes(data, 12, 4, "US-ASCII"
					);
			}
			catch (ArgumentException e)
			{
				throw new IOException(IOException.InvalidIccProfile, e);
			}
			return deviceClass;
		}

		public static int GetIccNumberOfComponents(byte[] data)
		{
			return cstags[GetIccColorSpaceName(data)];
		}

		public virtual byte[] GetData()
		{
			return data;
		}

		public virtual int GetNumComponents()
		{
			return numComponents;
		}

		static IccProfile()
		{
			cstags["XYZ "] = 3;
			cstags["Lab "] = 3;
			cstags["Luv "] = 3;
			cstags["YCbr"] = 3;
			cstags["Yxy "] = 3;
			cstags["RGB "] = 3;
			cstags["GRAY"] = 1;
			cstags["HSV "] = 3;
			cstags["HLS "] = 3;
			cstags["CMYK"] = 4;
			cstags["CMY "] = 3;
			cstags["2CLR"] = 2;
			cstags["3CLR"] = 3;
			cstags["4CLR"] = 4;
			cstags["5CLR"] = 5;
			cstags["6CLR"] = 6;
			cstags["7CLR"] = 7;
			cstags["8CLR"] = 8;
			cstags["9CLR"] = 9;
			cstags["ACLR"] = 10;
			cstags["BCLR"] = 11;
			cstags["CCLR"] = 12;
			cstags["DCLR"] = 13;
			cstags["ECLR"] = 14;
			cstags["FCLR"] = 15;
		}
	}
}
