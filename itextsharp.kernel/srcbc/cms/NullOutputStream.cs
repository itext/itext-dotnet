using System;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Cms
{
	internal class NullOutputStream
		: BaseOutputStream
	{
		public override void WriteByte(byte b)
		{
			// do nothing
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			// do nothing
		}
	}
}
