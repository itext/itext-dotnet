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
using System.Collections.Generic;
using System.IO;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfObjectTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void IndirectsChain1() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            catalog.Put(new PdfName("a"), GetTestPdfDictionary().MakeIndirect(document).GetIndirectReference().MakeIndirect
                (document).GetIndirectReference().MakeIndirect(document));
            PdfObject @object = ((PdfIndirectReference)catalog.Get(new PdfName("a"), false)).GetRefersTo(true);
            NUnit.Framework.Assert.IsTrue(@object is PdfDictionary);
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IndirectsChain2() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            PdfDictionary dictionary = GetTestPdfDictionary();
            PdfObject @object = dictionary;
            for (int i = 0; i < 200; i++) {
                @object = @object.MakeIndirect(document).GetIndirectReference();
            }
            catalog.Put(new PdfName("a"), @object);
            ((PdfIndirectReference)catalog.Get(new PdfName("a"))).GetRefersTo(true);
            NUnit.Framework.Assert.IsNotNull(((PdfIndirectReference)catalog.Get(new PdfName("a"))).GetRefersTo(true));
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IndirectsChain3() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            PdfDictionary dictionary = GetTestPdfDictionary();
            PdfObject @object = dictionary;
            for (int i = 0; i < 31; i++) {
                @object = @object.MakeIndirect(document).GetIndirectReference();
            }
            catalog.Put(new PdfName("a"), @object);
            @object = catalog.Get(new PdfName("a"), true);
            NUnit.Framework.Assert.IsTrue(@object is PdfDictionary);
            NUnit.Framework.Assert.AreEqual(new PdfName("c").ToString(), ((PdfDictionary)@object).Get(new PdfName("b")
                ).ToString());
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IndirectsChain4() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            PdfDictionary dictionary = GetTestPdfDictionary();
            PdfObject @object = dictionary;
            for (int i = 0; i < 31; i++) {
                @object = @object.MakeIndirect(document).GetIndirectReference();
            }
            PdfArray array = new PdfArray();
            array.Add(@object);
            catalog.Put(new PdfName("a"), array);
            @object = ((PdfArray)catalog.Get(new PdfName("a"))).Get(0, true);
            NUnit.Framework.Assert.IsTrue(@object is PdfDictionary);
            NUnit.Framework.Assert.AreEqual(new PdfName("c").ToString(), ((PdfDictionary)@object).Get(new PdfName("b")
                ).ToString());
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PdfIndirectReferenceFlags() {
            PdfIndirectReference reference = new PdfIndirectReference(null, 1);
            reference.SetState(PdfObject.FREE);
            reference.SetState(PdfObject.READING);
            reference.SetState(PdfObject.MODIFIED);
            NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.FREE), "Free");
            NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.READING), "Reading");
            NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.MODIFIED), "Modified");
            NUnit.Framework.Assert.AreEqual(true, reference.CheckState((byte)(PdfObject.FREE | PdfObject.MODIFIED | PdfObject
                .READING)), "Free|Reading|Modified");
            reference.ClearState(PdfObject.FREE);
            NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.FREE), "Free");
            NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.READING), "Reading");
            NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.MODIFIED), "Modified");
            NUnit.Framework.Assert.AreEqual(true, reference.CheckState((byte)(PdfObject.READING | PdfObject.MODIFIED))
                , "Reading|Modified");
            NUnit.Framework.Assert.AreEqual(false, reference.CheckState((byte)(PdfObject.FREE | PdfObject.READING | PdfObject
                .MODIFIED)), "Free|Reading|Modified");
            reference.ClearState(PdfObject.READING);
            NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.FREE), "Free");
            NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.READING), "Reading");
            NUnit.Framework.Assert.AreEqual(true, reference.CheckState(PdfObject.MODIFIED), "Modified");
            NUnit.Framework.Assert.AreEqual(false, reference.CheckState((byte)(PdfObject.FREE | PdfObject.READING)), "Free|Reading"
                );
            reference.ClearState(PdfObject.MODIFIED);
            NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.FREE), "Free");
            NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.READING), "Reading");
            NUnit.Framework.Assert.AreEqual(false, reference.CheckState(PdfObject.MODIFIED), "Modified");
            NUnit.Framework.Assert.AreEqual(true, !reference.IsFree(), "Is InUse");
            reference.SetState(PdfObject.FREE);
            NUnit.Framework.Assert.AreEqual(false, !reference.IsFree(), "Not IsInUse");
        }

        [NUnit.Framework.Test]
        public virtual void PdtIndirectReferenceLateInitializing1() {
            MemoryStream baos = new MemoryStream();
            PdfDocument document = new PdfDocument(new PdfWriter(baos));
            document.AddNewPage();
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            PdfIndirectReference indRef = document.CreateNextIndirectReference();
            catalog.Put(new PdfName("Smth"), indRef);
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(new PdfName("A"), new PdfString("a"));
            dictionary.MakeIndirect(document, indRef);
            document.Close();
            MemoryStream bais = new MemoryStream(baos.ToArray());
            document = new PdfDocument(new PdfReader(bais));
            PdfObject @object = document.GetCatalog().GetPdfObject().Get(new PdfName("Smth"));
            NUnit.Framework.Assert.IsTrue(@object is PdfDictionary);
            dictionary = (PdfDictionary)@object;
            PdfString a = (PdfString)dictionary.Get(new PdfName("A"));
            NUnit.Framework.Assert.IsTrue(a.GetValue().Equals("a"));
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FLUSHED_OBJECT_CONTAINS_REFERENCE_WHICH_NOT_REFER_TO_ANY_OBJECT
            )]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INDIRECT_REFERENCE_USED_IN_FLUSHED_OBJECT_MADE_FREE)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void PdtIndirectReferenceLateInitializing2() {
            MemoryStream baos = new MemoryStream();
            PdfDocument document = new PdfDocument(new PdfWriter(baos));
            document.AddNewPage();
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            PdfIndirectReference indRef1 = document.CreateNextIndirectReference();
            PdfIndirectReference indRef2 = document.CreateNextIndirectReference();
            catalog.Put(new PdfName("Smth1"), indRef1);
            catalog.Put(new PdfName("Smth2"), indRef2);
            PdfArray array = new PdfArray();
            array.Add(new PdfString("array string"));
            array.MakeIndirect(document, indRef2);
            document.Close();
            MemoryStream bais = new MemoryStream(baos.ToArray());
            document = new PdfDocument(new PdfReader(bais));
            PdfDictionary catalogDict = document.GetCatalog().GetPdfObject();
            PdfObject object1 = catalogDict.Get(new PdfName("Smth1"));
            PdfObject object2 = catalogDict.Get(new PdfName("Smth2"));
            NUnit.Framework.Assert.IsTrue(object1 is PdfNull);
            NUnit.Framework.Assert.IsTrue(object2 is PdfArray);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FLUSHED_OBJECT_CONTAINS_REFERENCE_WHICH_NOT_REFER_TO_ANY_OBJECT
            )]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INDIRECT_REFERENCE_USED_IN_FLUSHED_OBJECT_MADE_FREE)]
        public virtual void PdtIndirectReferenceLateInitializing3() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            document.AddNewPage();
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            PdfIndirectReference indRef1 = document.CreateNextIndirectReference();
            PdfIndirectReference indRef2 = document.CreateNextIndirectReference();
            PdfArray array = new PdfArray();
            catalog.Put(new PdfName("array1"), array);
            PdfString @string = new PdfString("array string");
            array.Add(@string);
            array.Add(indRef1);
            array.Add(indRef2);
            PdfDictionary dict = new PdfDictionary();
            dict.MakeIndirect(document, indRef1);
            PdfArray arrayClone = (PdfArray)array.Clone();
            PdfObject object0 = arrayClone.Get(0, false);
            PdfObject object1 = arrayClone.Get(1, false);
            PdfObject object2 = arrayClone.Get(2, false);
            NUnit.Framework.Assert.IsTrue(object0 is PdfString);
            NUnit.Framework.Assert.IsTrue(object1 is PdfDictionary);
            NUnit.Framework.Assert.IsTrue(object2 is PdfNull);
            PdfString string1 = (PdfString)object0;
            NUnit.Framework.Assert.IsTrue(@string != string1);
            NUnit.Framework.Assert.IsTrue(@string.GetValue().Equals(string1.GetValue()));
            PdfDictionary dict1 = (PdfDictionary)object1;
            NUnit.Framework.Assert.IsTrue(dict1.GetIndirectReference().GetObjNumber() == dict.GetIndirectReference().GetObjNumber
                ());
            NUnit.Framework.Assert.IsTrue(dict1.GetIndirectReference().GetGenNumber() == dict.GetIndirectReference().GetGenNumber
                ());
            NUnit.Framework.Assert.IsTrue(dict1 == dict);
            document.Close();
        }

        private static PdfDictionary GetTestPdfDictionary() {
            Dictionary<PdfName, PdfObject> tmpMap = new Dictionary<PdfName, PdfObject>();
            tmpMap.Put(new PdfName("b"), new PdfName("c"));
            return new PdfDictionary(tmpMap);
        }
    }
}
