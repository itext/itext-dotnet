using System;
using System.IO;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsProcessableInputStream
		: CmsProcessable, CmsReadable
	{
		private Stream input;
		private bool used = false;

		public CmsProcessableInputStream(
			Stream input)
		{
			this.input = input;
		}

		public Stream GetInputStream()
		{
			CheckSingleUsage();

			return input;
		}

		public void Write(Stream output)
		{
			CheckSingleUsage();

			Streams.PipeAll(input, output);
			input.Close();
		}

		[Obsolete]
		public object GetContent()
		{
			return GetInputStream();
		}

		private void CheckSingleUsage()
		{
			lock (this)
			{
				if (used)
					throw new InvalidOperationException("CmsProcessableInputStream can only be used once");

				used = true;
			}
		}
	}
}
