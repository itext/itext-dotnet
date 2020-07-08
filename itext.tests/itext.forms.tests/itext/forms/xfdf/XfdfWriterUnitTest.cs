using System.Collections.Generic;
using System.Xml;
using iText.Test;

namespace iText.Forms.Xfdf {
    public class XfdfWriterUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void FieldEmptyValueUnitTest() {
            XmlDocument document = XfdfFileUtils.CreateNewXfdfDocument();
            XmlElement fields = document.CreateElement("fields");
            document.AppendChild(fields);
            FieldObject fieldObject = new FieldObject();
            fieldObject.SetName("testname");
            fieldObject.SetValue("");
            IList<FieldObject> fieldList = new List<FieldObject>();
            XfdfWriter.AddField(fieldObject, fields, document, fieldList);
            XmlNode childOfFields = fields.FirstChild;
            NUnit.Framework.Assert.IsNotNull(childOfFields);
            NUnit.Framework.Assert.IsNull(childOfFields.FirstChild);
        }

        [NUnit.Framework.Test]
        public virtual void FieldNullValueUnitTest() {
            XmlDocument document = XfdfFileUtils.CreateNewXfdfDocument();
            XmlElement fields = document.CreateElement("fields");
            document.AppendChild(fields);
            FieldObject fieldObject = new FieldObject();
            fieldObject.SetName("testname");
            IList<FieldObject> fieldList = new List<FieldObject>();
            XfdfWriter.AddField(fieldObject, fields, document, fieldList);
            XmlNode childOfFields = fields.FirstChild;
            NUnit.Framework.Assert.IsNotNull(childOfFields);
            NUnit.Framework.Assert.IsNull(childOfFields.FirstChild);
        }

        [NUnit.Framework.Test]
        public virtual void FieldWithValueUnitTest() {
            XmlDocument document = XfdfFileUtils.CreateNewXfdfDocument();
            XmlElement fields = document.CreateElement("fields");
            document.AppendChild(fields);
            FieldObject fieldObject = new FieldObject();
            fieldObject.SetName("testname");
            fieldObject.SetValue("testValue");
            IList<FieldObject> fieldList = new List<FieldObject>();
            XfdfWriter.AddField(fieldObject, fields, document, fieldList);
            XmlNode childOfFields = fields.FirstChild;
            NUnit.Framework.Assert.IsNotNull(childOfFields);
            NUnit.Framework.Assert.AreEqual("value", childOfFields.FirstChild.Name);
            NUnit.Framework.Assert.AreEqual("testValue", childOfFields.FirstChild.InnerText);
        }
    }
}
