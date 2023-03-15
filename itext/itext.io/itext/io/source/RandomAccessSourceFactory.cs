/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using System.IO;
using System.Net;
using iText.Commons.Utils;
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
        /// <summary>The default value for the forceRead flag
        /// 	</summary>
        private static bool forceReadDefaultValue = false;

        /// <summary>Whether the full content of the source should be read into memory at construction
        /// 	</summary>
        private bool forceRead = false;

		/// <summary>Whether the underlying file should have a RW lock on it or just an R lock
		/// 	</summary>
		private bool exclusivelyLockFile = false;

		/// <summary>Creates a factory that will give preference to accessing the underling data source using memory mapped files
		/// 	</summary>
		public RandomAccessSourceFactory()
		{
		}

        /// <summary>Determines the default value for the forceRead flag
        ///     </summary>
        /// <param name="forceRead">true if by default the full content will be read, false otherwise</param>
        public static void SetForceReadDefaultValue(bool forceRead)
        {
            forceReadDefaultValue = forceRead;
        }

		/// <summary>Determines whether the full content of the source will be read into memory
		/// 	</summary>
		/// <param name="forceRead">true if the full content will be read, false otherwise</param>
		/// <returns>this object (this allows chaining of method calls)</returns>
		public RandomAccessSourceFactory SetForceRead(bool forceRead)
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
		public RandomAccessSourceFactory SetUsePlainRandomAccess(bool usePlainRandomAccess)
		{
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
        public IRandomAccessSource CreateSource(Uri url) {
			// Creation of web request via url.AbsoluteUri breaks UNC pathes (like \\computer-name\\img.jpg),
			// url.LocalPath and url.AbsolutePath - break http links (like https://website.com/img.jpg).
			// It seems enough to simply pass Uri instance as is, WebRequest seems to handle both escaping and UNC issues.
            WebRequest wr = WebRequest.Create(url);
            wr.Credentials = CredentialCache.DefaultCredentials;
            Stream isp = wr.GetResponse().GetResponseStream();
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
        /// Creates or extracts a
        /// <see cref="RandomAccessSource"/>
        /// based on a
        /// <see cref="System.IO.Stream"/>
        /// <para/>
        /// If the InputStream is an instance of
        /// <see cref="RASInputStream"/>
        /// then extracts the source from it.
        /// Otherwise The full content of the InputStream is read into memory and used
        /// as the source for the
        /// <see cref="RandomAccessSource"/>
        /// </summary>
        /// <param name="inputStream">the stream to read from</param>
        /// <returns>
        /// the newly created or extracted
        /// <see cref="RandomAccessSource"/>
        /// </returns>
        public IRandomAccessSource ExtractOrCreateSource(Stream inputStream)
        {
            if (inputStream is RASInputStream)
            {
                return ((RASInputStream) inputStream).GetSource();
            }
            return CreateSource(inputStream);
        }

        /// <summary>
        /// Creates a
        /// <see cref="RandomAccessSource"/>
        /// based on an
        /// <see cref="System.IO.Stream"/>
        /// <para />
        /// The full content of the InputStream is read into memory and used
        /// as the source for the
        /// <see cref="RandomAccessSource"/>
        /// </summary>
        /// <param name="inputStream">the stream to read from</param>
        /// <returns>
        /// the newly created
        /// <see cref="RandomAccessSource"/>
        /// </returns>
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
        private IRandomAccessSource CreateByReadingToMemory(String filename)
		{
			Stream stream = ResourceUtil.GetResourceStream(filename);
			if (stream == null)
			{
				throw new System.IO.IOException(MessageFormatUtil.Format(iText.IO.Exceptions.IOException._1NotFoundAsFileOrResource
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
				catch (System.IO.IOException)
				{
				}
			}
		}
	}
}
