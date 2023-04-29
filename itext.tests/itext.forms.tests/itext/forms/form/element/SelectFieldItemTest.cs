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
