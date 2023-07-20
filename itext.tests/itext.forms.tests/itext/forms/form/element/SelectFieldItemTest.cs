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
using iText.Kernel.Exceptions;
using iText.Layout.Element;
using iText.Test;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("UnitTest")]
    public class SelectFieldItemTest : ITextTest {
        [NUnit.Framework.Test]
        public virtual void NewSelectFieldItem2ParamConstructorTest() {
            SelectFieldItem item = new SelectFieldItem("exportValue", "displayValue");
            NUnit.Framework.Assert.AreEqual("exportValue", item.GetExportValue());
            NUnit.Framework.Assert.AreEqual("displayValue", item.GetDisplayValue());
        }

        [NUnit.Framework.Test]
        public virtual void NewSelectFieldItem1ParamConstructorTest() {
            SelectFieldItem item = new SelectFieldItem("exportValue");
            NUnit.Framework.Assert.AreEqual("exportValue", item.GetExportValue());
            NUnit.Framework.Assert.AreEqual("exportValue", item.GetDisplayValue());
        }

        [NUnit.Framework.Test]
        public virtual void NewSelectFieldItem3ParamConstructorTest1() {
            SelectFieldItem item = new SelectFieldItem("exportValue", "displayValue", new Paragraph("displayValue"));
            NUnit.Framework.Assert.AreEqual("exportValue", item.GetExportValue());
            NUnit.Framework.Assert.AreEqual("displayValue", item.GetDisplayValue());
            NUnit.Framework.Assert.IsTrue(item.GetElement() is Paragraph);
        }

        [NUnit.Framework.Test]
        public virtual void NewSelectFieldItem3ParamConstructorTest2() {
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                new SelectFieldItem("exportValue", "displayValue", null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void NewSelectFieldItem3ParamConstructorTest3() {
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                new SelectFieldItem(null, "displayValue", new Paragraph("displayValue"));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void HasExportAndDisplayValuesTest01() {
            SelectFieldItem item = new SelectFieldItem("exportValue", "displayValue");
            NUnit.Framework.Assert.IsTrue(item.HasExportAndDisplayValues());
        }

        [NUnit.Framework.Test]
        public virtual void HasExportAndDisplayValuesTest02() {
            SelectFieldItem item = new SelectFieldItem("exportValue");
            NUnit.Framework.Assert.IsFalse(item.HasExportAndDisplayValues());
        }

        [NUnit.Framework.Test]
        public virtual void HasExportAndDisplayValuesTest03() {
            SelectFieldItem item = new SelectFieldItem("exportValue", new Paragraph("displayValue"));
            NUnit.Framework.Assert.IsFalse(item.HasExportAndDisplayValues());
        }

        [NUnit.Framework.Test]
        public virtual void HasExportAndDisplayValuesTest04() {
            SelectFieldItem item = new SelectFieldItem("exportValue", null, new Paragraph("displayValue"));
            NUnit.Framework.Assert.IsFalse(item.HasExportAndDisplayValues());
        }

        [NUnit.Framework.Test]
        public virtual void HasExportAndDisplayValuesTest05() {
            SelectFieldItem item = new SelectFieldItem("exportValue", "displayValue", new Paragraph("displayValue"));
            NUnit.Framework.Assert.IsTrue(item.HasExportAndDisplayValues());
        }
    }
}
