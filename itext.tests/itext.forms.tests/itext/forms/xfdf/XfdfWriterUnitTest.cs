/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Collections.Generic;
using System.Xml;
using iText.Test;

namespace iText.Forms.Xfdf {
    [NUnit.Framework.Category("UnitTest")]
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
