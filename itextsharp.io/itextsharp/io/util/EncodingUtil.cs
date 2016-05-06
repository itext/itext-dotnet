using System;
using System.Text;

namespace com.itextpdf.io.util
{
	public sealed class EncodingUtil
	{
		private EncodingUtil()
		{
		}

		/// <exception cref="java.nio.charset.CharacterCodingException"/>
		public static byte[] ConvertToBytes(char[] chars, String encoding)
		{
            Encoding encw = IanaEncodings.GetEncodingEncoding(encoding);
            byte[] preamble = encw.GetPreamble();
		    if (preamble.Length == 0)
		    {
		        return encw.GetBytes(chars);
		    }
		    else
		    {
		        byte[] encoded = encw.GetBytes(chars);
		        byte[] total = new byte[encoded.Length + preamble.Length];
		        Array.Copy(preamble, 0, total, 0, preamble.Length);
		        Array.Copy(encoded, 0, total, preamble.Length, encoded.Length);
		        return total;
		    }
		}
	}
}
