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
using System;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Collection {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfCollectionFieldTest : ExtendedITextTest {
        private static readonly PdfName[] ALLOWED_PDF_NAMES = new PdfName[] { PdfName.D, PdfName.N, PdfName.F, PdfName
            .Desc, PdfName.ModDate, PdfName.CreationDate, PdfName.Size };

        private static readonly int[] EXPECTED_SUB_TYPES = new int[] { PdfCollectionField.DATE, PdfCollectionField
            .NUMBER, PdfCollectionField.FILENAME, PdfCollectionField.DESC, PdfCollectionField.MODDATE, PdfCollectionField
            .CREATIONDATE, PdfCollectionField.SIZE };

        [NUnit.Framework.Test]
        public virtual void SubTypeInConstructorTest() {
            for (int i = 0; i < ALLOWED_PDF_NAMES.Length; i++) {
                PdfDictionary pdfObject = new PdfDictionary();
                pdfObject.Put(PdfName.Subtype, ALLOWED_PDF_NAMES[i]);
                PdfCollectionField field = new PdfCollectionField(pdfObject);
                NUnit.Framework.Assert.AreEqual(field.subType, EXPECTED_SUB_TYPES[i]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DefaultSubTypeInConstructorTest() {
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.S);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            NUnit.Framework.Assert.AreEqual(field.subType, PdfCollectionField.TEXT);
        }

        [NUnit.Framework.Test]
        public virtual void FieldCreationWithNameAndSubTypeTest() {
            String fieldName = "fieldName";
            for (int i = 0; i < ALLOWED_PDF_NAMES.Length; i++) {
                PdfCollectionField field = new PdfCollectionField(fieldName, EXPECTED_SUB_TYPES[i]);
                NUnit.Framework.Assert.AreEqual(new PdfString(fieldName), field.GetPdfObject().Get(PdfName.N));
                NUnit.Framework.Assert.AreEqual(ALLOWED_PDF_NAMES[i], field.GetPdfObject().Get(PdfName.Subtype));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FieldCreationWithDefaultSubTypeTest() {
            String fieldName = "fieldName";
            int unexpectedSubType = -1;
            PdfName defaultSubType = PdfName.S;
            PdfCollectionField field = new PdfCollectionField(fieldName, unexpectedSubType);
            NUnit.Framework.Assert.AreEqual(defaultSubType, field.GetPdfObject().Get(PdfName.Subtype));
        }

        [NUnit.Framework.Test]
        public virtual void OrderPropertyTest() {
            int testOrder = 5;
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.S);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            NUnit.Framework.Assert.IsNull(field.GetOrder());
            field.SetOrder(testOrder);
            NUnit.Framework.Assert.AreEqual(testOrder, field.GetOrder().IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityPropertyTest() {
            bool testVisibility = true;
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.S);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            NUnit.Framework.Assert.IsNull(field.GetVisibility());
            field.SetVisibility(testVisibility);
            NUnit.Framework.Assert.AreEqual(testVisibility, field.GetVisibility().GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void EditablePropertyTest() {
            bool testEditable = true;
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.S);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            NUnit.Framework.Assert.IsNull(field.GetEditable());
            field.SetEditable(testEditable);
            NUnit.Framework.Assert.AreEqual(testEditable, field.GetEditable().GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void GetTextValueTest() {
            String textValue = "some text";
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.S);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            NUnit.Framework.Assert.AreEqual(new PdfString(textValue), field.GetValue(textValue));
        }

        [NUnit.Framework.Test]
        public virtual void GetNumberValueTest() {
            double numberValue = 125;
            String numberValueAsString = numberValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.N);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            NUnit.Framework.Assert.AreEqual(numberValue, ((PdfNumber)field.GetValue(numberValueAsString)).GetValue(), 
                0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void GetDateValueTest() {
            String timeValueAsString = "D:19860426012347+04'00'";
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.D);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            NUnit.Framework.Assert.IsTrue(((PdfString)field.GetValue(timeValueAsString)).GetValue().StartsWith("D:1986"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetUnsupportedTypeValueTest() {
            String stringValue = "string value";
            String fieldName = "fieldName";
            PdfCollectionField field = new PdfCollectionField(fieldName, PdfCollectionField.FILENAME);
            // this line will throw an exception as getValue() method is not
            // supported for subType which differs from S, N and D.
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => field.GetValue(stringValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.UNACCEPTABLE_FIELD_VALUE
                , stringValue, fieldName), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void IsWrappedObjectMustBeIndirectTest() {
            PdfDictionary pdfObject = new PdfDictionary();
            pdfObject.Put(PdfName.Subtype, PdfName.S);
            PdfCollectionField field = new PdfCollectionField(pdfObject);
            NUnit.Framework.Assert.IsFalse(field.IsWrappedObjectMustBeIndirect());
        }
    }
}
