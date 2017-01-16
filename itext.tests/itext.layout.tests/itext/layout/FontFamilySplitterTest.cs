using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace iText.Layout {
    public class FontFamilySplitterTest {
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FontFamilySplitter() {
            String fontFamilies = "'Puritan'\n" + "Puritan\n" + "'Pur itan'\n" + "Pur itan\n" + "'Pur it an'\n" + "Pur it an\n"
                 + "   \"Puritan\"\n" + "Puritan\n" + "  \"Pur itan\"\n" + "Pur itan\n" + "\"Pur it an\"\n" + "Pur it an\n"
                 + "FreeSans\n" + "FreeSans\n" + "'Puritan', FreeSans\n" + "Puritan; FreeSans\n" + "'Pur itan' , FreeSans\n"
                 + "Pur itan; FreeSans\n" + "   'Pur it an'  ,  FreeSans   \n" + "Pur it an; FreeSans\n" + "\"Puritan\", FreeSans\n"
                 + "Puritan; FreeSans\n" + "\"Pur itan\", FreeSans\n" + "Pur itan; FreeSans\n" + "\"Pur it an\", FreeSans\n"
                 + "Pur it an; FreeSans\n" + "\"Puritan\"\n" + "Puritan\n" + "'Free Sans',\n" + "Free Sans\n" + "\"Puritan\", Free Sans\n"
                 + "Puritan\n" + "'Puritan' FreeSans\n" + "-\n" + "Pur itan\n" + "-\n" + "Pur it an\"\n" + "-\n" + "\"Free Sans\n"
                 + "-\n" + "Pur it an'\n" + "-\n" + "'Free Sans\n" + "-\n";
            String[] splitFontFamilies = iText.IO.Util.StringUtil.Split(fontFamilies, "\n");
            MethodInfo splitFontFamily = iText.GetDeclaredMethod(System.Type.GetType("com.itextpdf.layout.font.FontFamilySplitter"
                ), "splitFontFamily", typeof(String));
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < splitFontFamilies.Length; i += 2) {
                IList<String> fontFamily = (IList<String>)splitFontFamily.Invoke(null, splitFontFamilies[i]);
                result.Length = 0;
                foreach (String ff in fontFamily) {
                    result.Append(ff).Append("; ");
                }
                NUnit.Framework.Assert.AreEqual(splitFontFamilies[i + 1], result.Length > 2 ? result.JSubstring(0, result.
                    Length - 2) : "-");
            }
        }
    }
}
