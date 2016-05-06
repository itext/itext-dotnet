using System;
using System.IO;
using System.Net;
using com.itextpdf.io.util;

namespace com.itextpdf.io.source
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
		public com.itextpdf.io.source.RandomAccessSourceFactory SetForceRead(bool forceRead
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
		public com.itextpdf.io.source.RandomAccessSourceFactory SetUsePlainRandomAccess(bool
			 usePlainRandomAccess)
		{
			this.usePlainRandomAccess = usePlainRandomAccess;
			return this;
		}

		public com.itextpdf.io.source.RandomAccessSourceFactory SetExclusivelyLockFile(bool
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
        public IRandomAccessSource CreateSource(Uri url)
		{
            WebRequest wr = WebRequest.Create(url);
            wr.Credentials = CredentialCache.DefaultCredentials;
            Stream isp = wr.GetResponse().GetResponseStream();
            try
            {
                return CreateSource(isp);
            }
            finally
            {
                try { isp.Close(); }
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
					stream.Close();
				}
				catch (IOException)
				{
				}
			}
		}
	}
}
