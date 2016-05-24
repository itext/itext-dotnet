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
using System.IO;
using System.Text;

namespace iTextSharp.IO.Util {
	public sealed class FileUtil
	{
		private FileUtil()
		{
		}

		public static String GetFontsDir()
		{
            return Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
		}

		public static String GetFileName(String file)
		{
			return new FileInfo(file).Name;
		}

		public static bool FileExists(String path)
		{
			if (!String.IsNullOrEmpty(path))
			{
                return new FileInfo(path).Exists;
			}
			return false;
		}

		public static bool DirectoryExists(String path)
		{
            if (!String.IsNullOrEmpty(path))
			{
                return new DirectoryInfo(path).Exists;
            }
			return false;
		}

        public static String[] ListFilesInDirectory(String path, bool recursive)
		{
            if (!String.IsNullOrEmpty(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
				if (dir.Exists)
				{
					FileInfo[] files = dir.GetFiles("*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
					String[] list = new String[files.Length];
					for (int i = 0; i < files.Length; i++)
					{
						list[i] = files[i].FullName;
					}
					return list;
				}
			}
			return null;
		}

        public static String[] ListFilesInDirectoryByFilter(String path, bool recursive, FileFilter filter)
        {
            if (!String.IsNullOrEmpty(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    FileInfo[] files = dir.GetFiles("*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                    String[] list = new String[files.Length];
                    for (int i = 0; i < files.Length; i++) {
                        if (filter.Accept(files[i].Name)) {
                            list[i] = files[i].FullName;    
                        }
                    }
                    return list;
                }
            }
            return null;
        }

		/// <exception cref="System.ArgumentException"/>
		public static StreamWriter CreatePrintWriter(Stream output, String encoding)
		{
			return new StreamWriter(output, Encoding.GetEncoding(encoding));
		}

		public static Stream GetBufferedOutputStream(String filename)
		{			
			return new FileStream(filename, FileMode.Create);
		}

	    public static long GetFileSize(String filePath) {
	        return new FileInfo(filePath).Length;
	    }


	    public class FileFilter {
	        public virtual bool Accept(String pathname) {
	            return true;
	        }
	    }
	}
}
