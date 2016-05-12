using System;
using System.Text.RegularExpressions;

namespace com.itextpdf.io.util
{
	public static class StringUtil
	{
	    public static Regex RegexCompile(String s) {
	        return new Regex(s);
	    }

	    public static Match Match(this Regex r, String s) {
	        return r.Match(s);
	    }

	    public static String Group(this Match match, int index) {
	        return match.Groups[index].Value;
	    }
	}
}
