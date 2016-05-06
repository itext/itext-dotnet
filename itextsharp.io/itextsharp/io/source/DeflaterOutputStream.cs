using System.IO;
using System.util.zlib;

namespace com.itextpdf.io.source
{
	public class DeflaterOutputStream : ZDeflaterOutputStream
	{
		public DeflaterOutputStream(Stream outp, int level, int size)
			: base(outp, level)
		{
		}

		public DeflaterOutputStream(Stream outp, int level)
			: this(outp, level, 512)
		{
		}

		public DeflaterOutputStream(Stream outp)
			: this(outp, -1)
		{
		}

		/// <exception cref="System.IO.IOException"/>
		public override void Close()
		{
			Finish();
			base.Close();
		}
	}
}
