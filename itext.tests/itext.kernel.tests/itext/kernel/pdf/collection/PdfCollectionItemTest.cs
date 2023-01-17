/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
