/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Collection {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfCollectionItemTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddItemTest() {
            String fieldName = "fieldName";
            String fieldValue = "fieldValue";
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.S);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            PdfCollectionSchema schema = new PdfCollectionSchema();
            schema.AddField(fieldName, field);
            PdfCollectionItem item = new PdfCollectionItem(schema);
            item.AddItem(fieldName, fieldValue);
            NUnit.Framework.Assert.AreEqual(fieldValue, item.GetPdfObject().GetAsString(new PdfName(fieldName)).GetValue
                ());
        }

        [NUnit.Framework.Test]
        public virtual void AddDateItemTest() {
            String fieldName = "fieldName";
            String timeValueAsString = "D:19860426012347+04'00'";
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.D);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            PdfCollectionSchema schema = new PdfCollectionSchema();
            schema.AddField(fieldName, field);
            PdfCollectionItem item = new PdfCollectionItem(schema);
            item.AddItem(fieldName, new PdfDate(PdfDate.Decode(timeValueAsString)));
            NUnit.Framework.Assert.IsTrue(((PdfString)field.GetValue(timeValueAsString)).GetValue().StartsWith("D:1986"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DontAddDateItemToAnotherSubTypeFieldTest() {
            String fieldName = "fieldName";
            String timeValueAsString = "D:19860426012347+04'00'";
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.F);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            PdfCollectionSchema schema = new PdfCollectionSchema();
            schema.AddField(fieldName, field);
            PdfCollectionItem item = new PdfCollectionItem(schema);
            item.AddItem(fieldName, new PdfDate(PdfDate.Decode(timeValueAsString)));
            NUnit.Framework.Assert.IsNull(item.GetPdfObject().GetAsString(new PdfName(fieldName)));
        }

        [NUnit.Framework.Test]
        public virtual void AddNumberItemTest() {
            String fieldName = "fieldName";
            double numberValue = 0.1234;
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.N);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            PdfCollectionSchema schema = new PdfCollectionSchema();
            schema.AddField(fieldName, field);
            PdfCollectionItem item = new PdfCollectionItem(schema);
            item.AddItem(fieldName, new PdfNumber(numberValue));
            NUnit.Framework.Assert.AreEqual(numberValue, item.GetPdfObject().GetAsNumber(new PdfName(fieldName)).GetValue
                (), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void DontAddNumberItemToAnotherSubTypeFieldTest() {
            String fieldName = "fieldName";
            double numberValue = 0.1234;
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.F);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            PdfCollectionSchema schema = new PdfCollectionSchema();
            schema.AddField(fieldName, field);
            PdfCollectionItem item = new PdfCollectionItem(schema);
            item.AddItem(fieldName, new PdfNumber(numberValue));
            NUnit.Framework.Assert.IsNull(item.GetPdfObject().GetAsString(new PdfName(fieldName)));
        }

        [NUnit.Framework.Test]
        public virtual void AddPrefixTest() {
            String fieldName = "fieldName";
            String fieldValue = "fieldValue";
            String fieldPrefix = "fieldPrefix";
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.S);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            PdfCollectionSchema schema = new PdfCollectionSchema();
            schema.AddField(fieldName, field);
            PdfCollectionItem item = new PdfCollectionItem(schema);
            item.AddItem(fieldName, fieldValue);
            item.SetPrefix(fieldName, fieldPrefix);
            NUnit.Framework.Assert.AreEqual(fieldValue, item.GetPdfObject().GetAsDictionary(new PdfName(fieldName)).GetAsString
                (PdfName.D).GetValue());
            NUnit.Framework.Assert.AreEqual(fieldPrefix, item.GetPdfObject().GetAsDictionary(new PdfName(fieldName)).GetAsString
                (PdfName.P).GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void AddPrefixToEmptyFieldTest() {
            String fieldName = "fieldName";
            String fieldPrefix = "fieldPrefix";
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.S);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            PdfCollectionSchema schema = new PdfCollectionSchema();
            schema.AddField(fieldName, field);
            PdfCollectionItem item = new PdfCollectionItem(schema);
            // this line will throw an exception as setPrefix() method may be called
            // only if value was set previously
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => item.SetPrefix(fieldName, fieldPrefix
                ));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.YOU_MUST_SET_A_VALUE_BEFORE_ADDING_A_PREFIX
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void IsWrappedObjectMustBeIndirectTest() {
            PdfCollectionItem item = new PdfCollectionItem(new PdfCollectionSchema());
            NUnit.Framework.Assert.IsFalse(item.IsWrappedObjectMustBeIndirect());
        }
    }
}
