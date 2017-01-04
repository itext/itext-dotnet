/*

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
using System.Net;
#if NETSTANDARD1_6
using System.Net.Http;
#endif
using iText.IO.Util;

namespace iText.IO.Source
{
	/// <summary>
	/// Factory to create
	/// <see cref="RandomAccessSource"/>
	/// objects based on various types of sources
	/// </summary>
	public sealed class RandomAccessSourceFactory
	{
		/// <summary>Whether the full content of the source should be read into memory at construction
		/// 	</summary>
		private bool forceRead = false;

		/// <summary>
		/// Whether
		/// <see cref="System.IO.FileStream"/>
		/// should be used instead of a
		/// <see cref="java.nio.channels.FileChannel"/>
		/// , where applicable
		/// </summary>
		private bool usePlainRandomAccess = false;

		/// <summary>Whether the underlying file should have a RW lock on it or just an R lock
		/// 	</summary>
		private bool exclusivelyLockFile = false;

		/// <summary>Creates a factory that will give preference to accessing the underling data source using memory mapped files
		/// 	</summary>
		public RandomAccessSourceFactory()
		{
		}

		/// <summary>Determines whether the full content of the source will be read into memory
		/// 	</summary>
		/// <param name="forceRead">true if the full content will be read, false otherwise</param>
		/// <returns>this object (this allows chaining of method calls)</returns>
		public RandomAccessSourceFactory SetForceRead(bool forceRead
			)
		{
			this.forceRead = forceRead;
			return this;
		}

		/// <summary>
		/// Determines whether
		/// <see cref="System.IO.FileStream"/>
		/// should be used as the primary data access mechanism
		/// </summary>
		/// <param name="usePlainRandomAccess">
		/// whether
		/// <see cref="System.IO.FileStream"/>
		/// should be used as the primary data access mechanism
		/// </param>
		/// <returns>this object (this allows chaining of method calls)</returns>
		public RandomAccessSourceFactory SetUsePlainRandomAccess(bool
			 usePlainRandomAccess)
		{
			this.usePlainRandomAccess = usePlainRandomAccess;
			return this;
		}

		public RandomAccessSourceFactory SetExclusivelyLockFile(bool
			 exclusivelyLockFile)
		{
			this.exclusivelyLockFile = exclusivelyLockFile;
			return this;
		}

		/// <summary>
		/// Creates a
		/// <see cref="RandomAccessSource"/>
		/// based on a byte array
		/// </summary>
		/// <param name="data">the byte array</param>
		/// <returns>
		/// the newly created
		/// <see cref="RandomAccessSource"/>
		/// </returns>
        public IRandomAccessSource CreateSource(byte[] data)
		{
			return new ArrayRandomAccessSource(data);
		}

		/// <exception cref="System.IO.IOException"/>
        public IRandomAccessSource CreateSource(FileStream raf)
		{
			return new RAFRandomAccessSource(raf);
		}

		/// <summary>
		/// Creates a
		/// <see cref="RandomAccessSource"/>
		/// based on a URL.  The data available at the URL is read into memory and used
		/// as the source for the
		/// <see cref="RandomAccessSource"/>
		/// </summary>
		/// <param name="url">the url to read from</param>
		/// <returns>
		/// the newly created
		/// <see cref="RandomAccessSource"/>
		/// </returns>
		/// <exception cref="System.IO.IOException"/>
        public IRandomAccessSource CreateSource(Uri url) {
#if !NETSTANDARD1_6
            WebRequest wr = WebRequest.Create(url);
            wr.Credentials = CredentialCache.DefaultCredentials;
            Stream isp = wr.GetResponse().GetResponseStream();
#else
		    Stream isp;
		    if (url.IsFile) {
		        isp = new FileStream(url.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
		    } else {
                HttpClient client = new HttpClient();
                isp = client.GetStreamAsync(url).Result;
            }
#endif
            try
            {
                return CreateSource(isp);
            }
            finally
            {
                try { isp.Dispose(); }
                catch { }
            }
		}

		/// <summary>
		/// Creates a
		/// <see cref="RandomAccessSource"/>
		/// based on an
		/// <see cref="System.IO.Stream"/>
		/// .  The full content of the InputStream is read into memory and used
		/// as the source for the
		/// <see cref="RandomAccessSource"/>
		/// </summary>
		/// <param name="inputStream">the stream to read from</param>
		/// <returns>
		/// the newly created
		/// <see cref="RandomAccessSource"/>
		/// </returns>
		/// <exception cref="System.IO.IOException"/>
        public IRandomAccessSource CreateSource(Stream inputStream)
		{
			return CreateSource(StreamUtil.InputStreamToArray(inputStream));
		}

		/// <summary>
		/// Creates a
		/// <see cref="RandomAccessSource"/>
		/// based on a filename string.
		/// If the filename describes a URL, a URL based source is created
		/// If the filename describes a file on disk, the contents may be read into memory (if
		/// <c>forceRead</c>
		/// is true),
		/// opened using memory mapped file channel (if usePlainRandomAccess is false), or
		/// opened using
		/// <see cref="System.IO.FileStream"/>
		/// access (if usePlainRandomAccess is true)
		/// This call will automatically fail over to using
		/// <see cref="System.IO.FileStream"/>
		/// if the memory map operation fails
		/// </summary>
		/// <param name="filename">
		/// the name of the file or resource to create the
		/// <see cref="RandomAccessSource"/>
		/// for
		/// </param>
		/// <returns>
		/// the newly created
		/// <see cref="RandomAccessSource"/>
		/// </returns>
		/// <exception cref="System.IO.IOException"/>
        public IRandomAccessSource CreateBestSource(String filename)
		{
            if (!File.Exists(filename))
            {
                if (filename.StartsWith("file:/")
                        || filename.StartsWith("http://")
                        || filename.StartsWith("https://"))
                {
                    return CreateSource(new Uri(filename));
                }
                else
                {
                    return CreateByReadingToMemory(filename);
                }
            }
            if (forceRead)
            {
                return CreateByReadingToMemory(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
            return new RAFRandomAccessSource(new FileStream(filename, FileMode.Open, FileAccess.Read, exclusivelyLockFile ? FileShare.None : FileShare.Read));
		}

		/// <exception cref="System.IO.IOException"/>
        public IRandomAccessSource CreateRanged(IRandomAccessSource source, long[] ranges)
		{
            IRandomAccessSource[] sources = new IRandomAccessSource[ranges.Length / 2];
			for (int i = 0; i < ranges.Length; i += 2)
			{
				sources[i / 2] = new WindowRandomAccessSource(source, ranges[i], ranges[i + 1]);
			}
			return new GroupedRandomAccessSource(sources);
		}

		/// <summary>
		/// Creates a new
		/// <see cref="RandomAccessSource"/>
		/// by reading the specified file/resource into memory
		/// </summary>
		/// <param name="filename">the name of the resource to read</param>
		/// <returns>
		/// the newly created
		/// <see cref="RandomAccessSource"/>
		/// </returns>
		/// <exception cref="System.IO.IOException">if reading the underling file or stream fails
		/// 	</exception>
        private IRandomAccessSource CreateByReadingToMemory(String filename)
		{
			Stream stream = ResourceUtil.GetResourceStream(filename);
			if (stream == null)
			{
				throw new IOException(String.Format(IOException._1NotFoundAsFileOrResource
					, filename));
			}
			return CreateByReadingToMemory(stream);
		}

		/// <summary>
		/// Creates a new
		/// <see cref="RandomAccessSource"/>
		/// by reading the specified file/resource into memory
		/// </summary>
		/// <param name="stream">the name of the resource to read</param>
		/// <returns>
		/// the newly created
		/// <see cref="RandomAccessSource"/>
		/// </returns>
		/// <exception cref="System.IO.IOException">if reading the underling file or stream fails
		/// 	</exception>
        private IRandomAccessSource CreateByReadingToMemory(Stream stream)
		{
			try
			{
				return new ArrayRandomAccessSource(StreamUtil.InputStreamToArray(stream));
			}
			finally
			{
				try
				{
					stream.Dispose();
				}
				catch (IOException)
				{
				}
			}
		}
	}
}
