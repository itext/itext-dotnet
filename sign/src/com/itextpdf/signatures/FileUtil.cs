using System;
using System.IO;
using java.io;

namespace com.itextpdf.signatures
{
	public class FileUtil
	{
		/// <exception cref="System.IO.IOException"/>
		public static Stream InputStreamToOutputStream(Stream inputStream, String path)
		{
			Stream outputStream = new FileOutputStream(path);
			int read = 0;
			byte[] bytes = new byte[1024];
			while ((read = inputStream.Read(bytes)) != -1)
			{
				outputStream.Write(bytes, 0, read);
			}
			return outputStream;
		}
	}
}
