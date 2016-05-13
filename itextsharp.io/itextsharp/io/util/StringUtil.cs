using System;
using System.Text.RegularExpressions;

namespace iTextSharp.IO.Util {
	public static class StringUtil
	{
	    public static String ReplaceAll(String srcString, String regex, String replacement) {
	        return Regex.Replace(srcString, regex, replacement);
	    }

	    public static Regex RegexCompile(String s) {
	        return new Regex(s);
	    }

	    public static Match Match(Regex r, String s) {
	        return r.Match(s);
	    }

	    public static String Group(Match match, int index) {
	        return match.Groups[index].Value;
	    }
	}
}
