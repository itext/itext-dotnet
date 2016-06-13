using System.IO;

namespace iText.IO.Source
{
	public class ByteArrayOutputStream : MemoryStream
	{
		public ByteArrayOutputStream()
			: base()
		{
		}

		public ByteArrayOutputStream(int size)
			: base(size)
		{
		}

		public virtual void AssignBytes(byte[] bytes, int count)
		{
			SetLength(0);
			Write(bytes, 0, count);
		}
	}
}
