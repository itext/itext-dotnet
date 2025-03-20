/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections.Generic;
using System.Reflection;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    public class HtmlTreeBuilderStateTest : ExtendedITextTest {
        internal static IList<Object[]> FindConstantArrays(Type aClass)
        {
            List<Object[]> array = new List<Object[]>();
            FieldInfo[] fields = aClass.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                if (field.IsStatic && !field.IsPrivate && field.FieldType.IsArray)
                {
                    array.Add((Object[]) field.GetValue(null));
                }
            }

            return array;
        }

        internal static void EnsureSorted(IList<Object[]> constants) {
            foreach (Object[] array in constants) {
                Object[] copy = JavaUtil.ArraysCopyOf(array, array.Length);
                JavaUtil.Sort(array);
                NUnit.Framework.Assert.AreEqual(array, copy);
            }
        }

        [NUnit.Framework.Test]
        public virtual void EnsureArraysAreSorted() {
            IList<Object[]> constants = FindConstantArrays(typeof(HtmlTreeBuilderState.Constants));
            EnsureSorted(constants);
            NUnit.Framework.Assert.AreEqual(38, constants.Count);
        }

        [NUnit.Framework.Test]
        public virtual void NestedAnchorElements01() {
            String html = "<html>\n" + "  <body>\n" + "    <a href='#1'>\n" + "        <div>\n" + "          <a href='#2'>child</a>\n"
                 + "        </div>\n" + "    </a>\n" + "  </body>\n" + "</html>";
            String s = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).ToString();
            NUnit.Framework.Assert.AreEqual("<html> \n" + " <head></head>\n" + " <body> <a href=\"#1\"> </a>\n" + "  <div>\n"
                 + "   <a href=\"#1\"> </a><a href=\"#2\">child</a> \n" + "  </div>   \n" + " </body>\n" + "</html>", 
                s);
        }

        [NUnit.Framework.Test]
        public virtual void NestedAnchorElements02() {
            String html = "<html>\n" + "  <body>\n" + "    <a href='#1'>\n" + "      <div>\n" + "        <div>\n" + "          <a href='#2'>child</a>\n"
                 + "        </div>\n" + "      </div>\n" + "    </a>\n" + "  </body>\n" + "</html>";
            String s = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).ToString();
            NUnit.Framework.Assert.AreEqual("<html> \n" + " <head></head>\n" + " <body> <a href=\"#1\"> </a>\n" + "  <div>\n"
                 + "   <a href=\"#1\"> </a>\n" + "   <div>\n" + "    <a href=\"#1\"> </a><a href=\"#2\">child</a> \n" 
                + "   </div> \n" + "  </div>   \n" + " </body>\n" + "</html>", s);
        }
    }
}
