/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests for FormElement</summary>
    /// <author>Jonathan Hedley</author>
    [NUnit.Framework.Category("UnitTest")]
    public class FormElementTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void HasAssociatedControls() {
            //"button", "fieldset", "input", "keygen", "object", "output", "select", "textarea"
            String html = "<form id=1><button id=1><fieldset id=2 /><input id=3><keygen id=4><object id=5><output id=6>"
                 + "<select id=7><option></select><textarea id=8><p id=9>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            FormElement form = (FormElement)doc.Select("form").First();
            NUnit.Framework.Assert.AreEqual(8, form.Elements().Count);
        }

        [NUnit.Framework.Test]
        public virtual void CreatesFormData() {
            String html = "<form><input name='one' value='two'><select name='three'><option value='not'>" + "<option value='four' selected><option value='five' selected><textarea name=six>seven</textarea>"
                 + "<input name='seven' type='radio' value='on' checked><input name='seven' type='radio' value='off'>"
                 + "<input name='eight' type='checkbox' checked><input name='nine' type='checkbox' value='unset'>" + "<input name='ten' value='text' disabled>"
                 + "<input name='eleven' value='text' type='button'>" + "</form>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            FormElement form = (FormElement)doc.Select("form").First();
            IList<KeyVal> data = form.FormData();
            NUnit.Framework.Assert.AreEqual(6, data.Count);
            NUnit.Framework.Assert.AreEqual("one=two", data[0].ToString());
            NUnit.Framework.Assert.AreEqual("three=four", data[1].ToString());
            NUnit.Framework.Assert.AreEqual("three=five", data[2].ToString());
            NUnit.Framework.Assert.AreEqual("six=seven", data[3].ToString());
            NUnit.Framework.Assert.AreEqual("seven=on", data[4].ToString());
            // set
            NUnit.Framework.Assert.AreEqual("eight=on", data[5].ToString());
        }

        // default
        // nine should not appear, not checked checkbox
        // ten should not appear, disabled
        // eleven should not appear, button
        [NUnit.Framework.Test]
        public virtual void FormDataUsesFirstAttribute() {
            String html = "<form><input name=test value=foo name=test2 value=bar>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            FormElement form = (FormElement)doc.SelectFirst("form");
            NUnit.Framework.Assert.AreEqual("test=foo", form.FormData()[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void FormsAddedAfterParseAreFormElements() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<body />");
            doc.Body().Html("<form action='http://example.com/search'><input name='q' value='search'>");
            iText.StyledXmlParser.Jsoup.Nodes.Element formEl = doc.Select("form").First();
            NUnit.Framework.Assert.IsTrue(formEl is FormElement);
            FormElement form = (FormElement)formEl;
            NUnit.Framework.Assert.AreEqual(1, form.Elements().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ControlsAddedAfterParseAreLinkedWithForms() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<body />");
            doc.Body().Html("<form />");
            iText.StyledXmlParser.Jsoup.Nodes.Element formEl = doc.Select("form").First();
            formEl.Append("<input name=foo value=bar>");
            NUnit.Framework.Assert.IsTrue(formEl is FormElement);
            FormElement form = (FormElement)formEl;
            NUnit.Framework.Assert.AreEqual(1, form.Elements().Count);
            IList<KeyVal> data = form.FormData();
            NUnit.Framework.Assert.AreEqual("foo=bar", data[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void UsesOnForCheckboxValueIfNoValueSet() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<form><input type=checkbox checked name=foo></form>"
                );
            FormElement form = (FormElement)doc.Select("form").First();
            IList<KeyVal> data = form.FormData();
            NUnit.Framework.Assert.AreEqual("on", data[0].Value());
            NUnit.Framework.Assert.AreEqual("foo", data[0].Key());
        }

        [NUnit.Framework.Test]
        public virtual void AdoptedFormsRetainInputs() {
            // test for https://github.com/jhy/jsoup/issues/249
            String html = "<html>\n" + "<body>  \n" + "  <table>\n" + "      <form action=\"/hello.php\" method=\"post\">\n"
                 + "      <tr><td>User:</td><td> <input type=\"text\" name=\"user\" /></td></tr>\n" + "      <tr><td>Password:</td><td> <input type=\"password\" name=\"pass\" /></td></tr>\n"
                 + "      <tr><td><input type=\"submit\" name=\"login\" value=\"login\" /></td></tr>\n" + "   </form>\n"
                 + "  </table>\n" + "</body>\n" + "</html>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            FormElement form = (FormElement)doc.Select("form").First();
            IList<KeyVal> data = form.FormData();
            NUnit.Framework.Assert.AreEqual(3, data.Count);
            NUnit.Framework.Assert.AreEqual("user", data[0].Key());
            NUnit.Framework.Assert.AreEqual("pass", data[1].Key());
            NUnit.Framework.Assert.AreEqual("login", data[2].Key());
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFormElement() {
            String html = "<html>\n" + "  <body> \n" + "      <form action=\"/hello.php\" method=\"post\">\n" + "      User:<input type=\"text\" name=\"user\" />\n"
                 + "      Password:<input type=\"password\" name=\"pass\" />\n" + "      <input type=\"submit\" name=\"login\" value=\"login\" />\n"
                 + "   </form>\n" + "  </body>\n" + "</html>  ";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            FormElement form = (FormElement)doc.SelectFirst("form");
            iText.StyledXmlParser.Jsoup.Nodes.Element pass = form.SelectFirst("input[name=pass]");
            pass.Remove();
            IList<KeyVal> data = form.FormData();
            NUnit.Framework.Assert.AreEqual(2, data.Count);
            NUnit.Framework.Assert.AreEqual("user", data[0].Key());
            NUnit.Framework.Assert.AreEqual("login", data[1].Key());
            NUnit.Framework.Assert.IsNull(doc.SelectFirst("input[name=pass]"));
        }
    }
}
