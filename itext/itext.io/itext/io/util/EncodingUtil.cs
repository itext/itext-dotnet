using System;
using System.Text;

namespace iText.IO.Util {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that it's API and functionality may be changed in future.
    /// </summary>
    public static class EncodingUtil
	{
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

	    public static String ConvertToString(byte[] bytes, String encoding) {
            String nameU = encoding.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            Encoding enc = null;
            if (nameU.Equals("UNICODEBIGUNMARKED"))
                enc = new UnicodeEncoding(true, false);
            else if (nameU.Equals("UNICODELITTLEUNMARKED"))
                enc = new UnicodeEncoding(false, false);
            if (enc != null)
                return enc.GetString(bytes);
            bool marker = false;
            bool big = false;
            int offset = 0;
            if (bytes.Length >= 2)
            {
                if (bytes[0] == 0xFE && bytes[1] == 0xFF)
                {
                    marker = true;
                    big = true;
                    offset = 2;
                }
                else if (bytes[0] == 0xFF && bytes[1] == 0xFE)
                {
                    marker = true;
                    offset = 2;
                }
            }
            if (nameU.Equals("UNICODEBIG"))
                enc = new UnicodeEncoding(!marker || big, false);
            else if (nameU.Equals("UNICODELITTLE"))
                enc = new UnicodeEncoding(marker && big, false);
            if (enc != null)
                return enc.GetString(bytes, offset, bytes.Length - offset);
            return IanaEncodings.GetEncodingEncoding(encoding).GetString(bytes);
	    }
	}
}
