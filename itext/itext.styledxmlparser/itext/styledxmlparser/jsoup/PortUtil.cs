/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Util;

namespace iText.StyledXmlParser.Jsoup
{
    internal static class PortUtil
    {

        public static bool CharsetIsSupported(string charset)
        {
            try
            {
                var enc = EncodingUtil.GetEncoding(charset);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        
        public static int ToInt32(string value, int codeBase)
        {
            if (codeBase == 2 || codeBase == 8 || codeBase == 10 || codeBase == 16)
            {
                return Convert.ToInt32(value, codeBase);
            }

            int result = 0;
            string symbols = "0123456789abcdefghijklmnopqrstuvwxyz".Substring(0, codeBase);
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                int number = symbols.IndexOf(ch);
                if (number == -1)
                {
                    throw new ArgumentException("String cannot be parsed");
                }

                result += number * (int) Math.Pow(codeBase, value.Length - i - 1);
            }

            return result;
        }

        public static string TrimControlCodes(string str)
        {
            char[] controlCodes = new char[' ' + 1];
            for (int i = 0; i < ' ' + 1; ++i)
            {
                controlCodes[i] = (char) i;
            }

            return str.Trim(controlCodes);
        }
    }
}
