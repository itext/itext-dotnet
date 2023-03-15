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
using iText.Commons.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FreeReferencesTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/FreeReferencesTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/FreeReferencesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest01() {
            String src = "freeRefsGapsAndMaxGen.pdf";
            String @out = "freeReferencesTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 15\n" + "0000000010 65535 f \n" + "0000000269 00000 n \n"
                 + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "0000000000 65535 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000000 00000 f \n"
                 + "0000000011 00000 f \n" + "0000000005 00001 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000613 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest02() {
            String src = "freeRefsGapsAndMaxGen.pdf";
            String @out = "freeReferencesTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 5\n" + "0000000010 65535 f \n" + "0000000269 00000 n \n" 
                + "0000000569 00000 n \n" + "0000000314 00000 n \n" + "0000000000 65535 f \n" + "10 5\n" + "0000000011 00000 f \n"
                 + 
                        // Append mode, no possibility to fix subsections in first xref
                        "0000000000 00001 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n" + "0000000480 00000 n \n", "xref\n"
                 + "3 1\n" + "0000000995 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest03() {
            String src = "freeRefsDeletedObj.pdf";
            String @out = "freeReferencesTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            pdfDocument.AddNewPage();
            // fix page content
            PdfStream firstPageContentStream = pdfDocument.GetPage(1).GetContentStream(0);
            String firstPageData = iText.Commons.Utils.JavaUtil.GetStringForBytes(firstPageContentStream.GetBytes());
            firstPageContentStream.SetData((firstPageData.JSubstring(0, firstPageData.LastIndexOf("BT")) + "ET").GetBytes
                ());
            firstPageContentStream.SetModified();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 7\n" + "0000000000 65535 f \n" + "0000000265 00000 n \n" 
                + "0000000564 00000 n \n" + "0000000310 00000 n \n" + "0000000132 00000 n \n" + "0000000015 00001 n \n"
                 + "0000000476 00000 n \n", "xref\n" + "0 1\n" + "0000000005 65535 f \n" + "3 3\n" + "0000000923 00000 n \n"
                 + "0000001170 00000 n \n" + "0000000000 00002 f \n" + "7 1\n" + "0000001303 00000 n \n", "xref\n" + "1 3\n"
                 + "0000001706 00000 n \n" + "0000001998 00000 n \n" + "0000001751 00000 n \n" + "7 3\n" + "0000002055 00000 n \n"
                 + "0000002171 00000 n \n" + "0000002272 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest04() {
            String src = "simpleDoc.pdf";
            String @out = "freeReferencesTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            PdfObject contentsObj = pdfDocument.GetPage(1).GetPdfObject().Remove(PdfName.Contents);
            NUnit.Framework.Assert.IsTrue(contentsObj is PdfIndirectReference);
            PdfIndirectReference contentsRef = (PdfIndirectReference)contentsObj;
            contentsRef.SetFree();
            PdfObject freedContentsRefRefersTo = contentsRef.GetRefersTo();
            NUnit.Framework.Assert.IsNull(freedContentsRefRefersTo);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 7\n" + "0000000005 65535 f \n" + "0000000133 00000 n \n" 
                + "0000000425 00000 n \n" + "0000000178 00000 n \n" + "0000000015 00000 n \n" + "0000000000 00001 f \n"
                 + "0000000476 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest05() {
            String src = "simpleDocWithSubsections.pdf";
            String @out = "freeReferencesTest05.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 14\n" + "0000000004 65535 f \n" + "0000000269 00000 n \n"
                 + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "0000000005 00000 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000010 00000 f \n"
                 + "0000000000 00000 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n" + "0000000613 00000 n \n"
                 };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest06() {
            String src = "simpleDocWithSubsections.pdf";
            String @out = "freeReferencesTest06.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 4\n" + "0000000000 65535 f \n" + "0000000269 00000 n \n" 
                + "0000000569 00000 n \n" + "0000000314 00000 n \n" + "11 3\n" + "0000000133 00000 n \n" + 
                        // Append mode, no possibility to fix subsections in first xref
                        "0000000015 00000 n \n" + "0000000480 00000 n \n", "xref\n" + "3 1\n" + "0000000935 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest07() {
            String @out = "freeReferencesTest07.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + @out));
            pdfDocument.CreateNextIndirectReference();
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 7\n" + "0000000004 65535 f \n" + "0000000203 00000 n \n" 
                + "0000000414 00000 n \n" + "0000000248 00000 n \n" + "0000000000 00001 f \n" + "0000000088 00000 n \n"
                 + "0000000015 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest08() {
            String src = "simpleDoc.pdf";
            String @out = "freeReferencesTest08.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            PdfObject contentsObj = pdfDocument.GetPage(1).GetPdfObject().Remove(PdfName.Contents);
            pdfDocument.GetPage(1).SetModified();
            NUnit.Framework.Assert.IsTrue(contentsObj is PdfIndirectReference);
            PdfIndirectReference contentsRef = (PdfIndirectReference)contentsObj;
            contentsRef.SetFree();
            PdfObject freedContentsRefRefersTo = contentsRef.GetRefersTo();
            NUnit.Framework.Assert.IsNull(freedContentsRefRefersTo);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 7\n" + "0000000000 65535 f \n" + "0000000265 00000 n \n" 
                + "0000000564 00000 n \n" + "0000000310 00000 n \n" + "0000000132 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000476 00000 n \n", "xref\n" + "0 1\n" + "0000000005 65535 f \n" + "3 3\n" + "0000000923 00000 n \n"
                 + "0000001170 00000 n \n" + "0000000000 00001 f \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ALREADY_FLUSHED_INDIRECT_OBJECT_MADE_FREE)]
        public virtual void FreeARefInWrongWayTest01() {
            String @out = "freeARefInWrongWayTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + @out));
            pdfDocument.AddNewPage();
            PdfDictionary catalogDict = pdfDocument.GetCatalog().GetPdfObject();
            String outerString = "Outer array. Contains inner array at both 0 and 1 index. At 0 - as pdf object, at 1 - as in ref.";
            String innerString = "Inner array.";
            String description = "Inner array first flushed, then it's ref is made free.";
            PdfArray a1 = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            PdfArray a2 = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            a1.Add(a2);
            a1.Add(a2.GetIndirectReference());
            a1.Add(new PdfString(outerString));
            a1.Add(new PdfString(description));
            a2.Add(new PdfString(innerString));
            catalogDict.Put(new PdfName("TestArray"), a1);
            a2.Flush();
            a2.GetIndirectReference().SetFree();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 8\n" + "0000000000 65535 f \n" + "0000000235 00000 n \n" 
                + "0000000462 00000 n \n" + "0000000296 00000 n \n" + "0000000120 00000 n \n" + "0000000047 00000 n \n"
                 + "0000000513 00000 n \n" + "0000000015 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FLUSHED_OBJECT_CONTAINS_FREE_REFERENCE, Count = 2)]
        public virtual void FreeARefInWrongWayTest02() {
            String @out = "freeARefInWrongWayTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + @out));
            pdfDocument.AddNewPage();
            PdfDictionary catalogDict = pdfDocument.GetCatalog().GetPdfObject();
            String outerString = "Outer array. Contains inner array at both 0 and 1 index. At 0 - as pdf object, at 1 - as in ref.";
            String innerString = "Inner array.";
            String description = "Inner array ref made free, then outer array is flushed.";
            PdfArray a1 = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            PdfArray a2 = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            a1.Add(a2);
            a1.Add(a2.GetIndirectReference());
            a1.Add(new PdfString(outerString));
            a1.Add(new PdfString(description));
            a2.Add(new PdfString(innerString));
            catalogDict.Put(new PdfName("TestArray"), a1);
            a2.GetIndirectReference().SetFree();
            IList<PdfObject> objects = JavaUtil.ArraysAsList(new PdfObject[] { new PdfString("The answer to life is ")
                , new PdfNumber(42) });
            new PdfArray(objects).MakeIndirect(pdfDocument).Flush();
            NUnit.Framework.Assert.IsTrue(a1.Get(1, false) is PdfIndirectReference);
            NUnit.Framework.Assert.IsTrue(((PdfIndirectReference)a1.Get(1, false)).IsFree());
            a1.Flush();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 9\n" + "0000000007 65535 f \n" + "0000000432 00000 n \n" 
                + "0000000659 00000 n \n" + "0000000493 00000 n \n" + "0000000317 00000 n \n" + "0000000244 00000 n \n"
                 + "0000000060 00000 n \n" + "0000000000 00001 f \n" + "0000000015 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INDIRECT_REFERENCE_USED_IN_FLUSHED_OBJECT_MADE_FREE)]
        public virtual void FreeARefInWrongWayTest03() {
            String @out = "freeARefInWrongWayTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + @out));
            pdfDocument.AddNewPage();
            PdfDictionary catalogDict = pdfDocument.GetCatalog().GetPdfObject();
            String outerString = "Outer array. Contains inner array at both 0 and 1 index. At 0 - as pdf object, at 1 - as in ref.";
            String innerString = "Inner array.";
            String description = "Outer array is flushed, then inner array ref made free.";
            PdfArray a1 = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            PdfArray a2 = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            a1.Add(a2);
            a1.Add(a2.GetIndirectReference());
            a1.Add(new PdfString(outerString));
            a1.Add(new PdfString(description));
            a2.Add(new PdfString(innerString));
            catalogDict.Put(new PdfName("TestArray"), a1);
            a1.Flush();
            a2.GetIndirectReference().SetFree();
            NUnit.Framework.Assert.IsFalse(a2.GetIndirectReference().IsFree());
            IList<PdfObject> objects = JavaUtil.ArraysAsList(new PdfObject[] { new PdfString("The answer to life is ")
                , new PdfNumber(42) });
            new PdfArray(objects).MakeIndirect(pdfDocument).Flush();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 9\n" + "0000000000 65535 f \n" + "0000000431 00000 n \n" 
                + "0000000658 00000 n \n" + "0000000492 00000 n \n" + "0000000316 00000 n \n" + "0000000243 00000 n \n"
                 + "0000000015 00000 n \n" + "0000000709 00000 n \n" + "0000000201 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ALREADY_FLUSHED_INDIRECT_OBJECT_MADE_FREE)]
        public virtual void FreeARefInWrongWayTest04() {
            String @out = "freeARefInWrongWayTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + @out));
            pdfDocument.AddNewPage();
            PdfDictionary catalogDict = pdfDocument.GetCatalog().GetPdfObject();
            String outerString = "Outer array. Contains inner array at both 0 and 1 index. At 0 - as pdf object, at 1 - as in ref.";
            String innerString = "Inner array.";
            String description = "Outer array is flushed, then inner array ref made free.";
            PdfArray a1 = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            PdfArray a2 = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            a1.Add(a2);
            a1.Add(a2.GetIndirectReference());
            a1.Add(new PdfString(outerString));
            a1.Add(new PdfString(description));
            a2.Add(new PdfString(innerString));
            catalogDict.Put(new PdfName("TestArray"), a1);
            a1.Flush();
            a2.Flush();
            a2.GetIndirectReference().SetFree();
            NUnit.Framework.Assert.IsFalse(a2.GetIndirectReference().IsFree());
            IList<PdfObject> objects = JavaUtil.ArraysAsList(new PdfObject[] { new PdfString("The answer to life is ")
                , new PdfNumber(42) });
            new PdfArray(objects).MakeIndirect(pdfDocument).Flush();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 9\n" + "0000000000 65535 f \n" + "0000000431 00000 n \n" 
                + "0000000658 00000 n \n" + "0000000492 00000 n \n" + "0000000316 00000 n \n" + "0000000243 00000 n \n"
                 + "0000000015 00000 n \n" + "0000000709 00000 n \n" + "0000000201 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeRefsAtEndOfXref01() {
            String @out = "freeRefsAtEndOfXref01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + @out));
            pdfDocument.AddNewPage();
            PdfDictionary catalogDict = pdfDocument.GetCatalog().GetPdfObject();
            String outerString = "Outer array. Contains inner array at both 0 and 1 index. At 0 - as pdf object, at 1 - as ind ref.";
            String innerString = "Inner array.";
            String description = "Last entry in the document xref table is free";
            PdfArray a1 = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            PdfArray a2 = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            a1.Add(a2);
            a1.Add(a2.GetIndirectReference());
            a1.Add(new PdfString(outerString));
            a1.Add(new PdfString(description));
            a2.Add(new PdfString(innerString));
            catalogDict.Put(new PdfName("TestArray"), a1);
            new PdfArray().MakeIndirect(pdfDocument).GetIndirectReference().SetFree();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 8\n" + "0000000000 65535 f \n" + "0000000203 00000 n \n" 
                + "0000000430 00000 n \n" + "0000000264 00000 n \n" + "0000000088 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000481 00000 n \n" + "0000000658 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeRefsAtEndOfXref02() {
            String src = "lastXrefEntryFree.pdf";
            String @out = "freeRefsAtEndOfXref02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 8\n" + "0000000000 65535 f \n" + "0000000203 00000 n \n" 
                + "0000000511 00000 n \n" + "0000000264 00000 n \n" + "0000000088 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000562 00000 n \n" + "0000000739 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeRefsAtEndOfXref03() {
            String src = "lastXrefEntryFree.pdf";
            String @out = "freeRefsAtEndOfXref03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            new PdfArray().MakeIndirect(pdfDocument).GetIndirectReference().SetFree();
            IList<PdfObject> objects = JavaUtil.ArraysAsList(new PdfObject[] { new PdfString("The answer to life is ")
                , new PdfNumber(42) });
            new PdfArray(objects).MakeIndirect(pdfDocument).Flush();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 11\n" + "0000000008 65535 f \n" + "0000000246 00000 n \n"
                 + "0000000554 00000 n \n" + "0000000307 00000 n \n" + "0000000131 00000 n \n" + "0000000058 00000 n \n"
                 + "0000000605 00000 n \n" + "0000000782 00000 n \n" + "0000000009 00001 f \n" + "0000000000 00001 f \n"
                 + "0000000015 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeRefsAtEndOfXref04() {
            String src = "lastXrefEntryFree.pdf";
            String @out = "freeRefsAtEndOfXref04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 9\n" + "0000000008 65535 f \n" + "0000000203 00000 n \n" 
                + "0000000430 00000 n \n" + "0000000264 00000 n \n" + "0000000088 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000481 00000 n \n" + "0000000658 00000 n \n" + "0000000000 00001 f \n", "xref\n" + "3 1\n" + "0000001038 00000 n \n"
                 };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeRefsAtEndOfXref05() {
            String src = "lastXrefEntryFree.pdf";
            String @out = "freeRefsAtEndOfXref05.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            new PdfArray().MakeIndirect(pdfDocument).GetIndirectReference().SetFree();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 9\n" + "0000000008 65535 f \n" + "0000000203 00000 n \n" 
                + "0000000430 00000 n \n" + "0000000264 00000 n \n" + "0000000088 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000481 00000 n \n" + "0000000658 00000 n \n" + "0000000000 00001 f \n", "xref\n" + "3 1\n" + "0000001038 00000 n \n"
                 + "8 2\n" + "0000000009 00001 f \n" + "0000000000 00001 f \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeRefsAtEndOfXref06() {
            String src = "lastXrefEntryFree.pdf";
            String @out = "freeRefsAtEndOfXref06.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            new PdfArray().MakeIndirect(pdfDocument).GetIndirectReference().SetFree();
            IList<PdfObject> objects = JavaUtil.ArraysAsList(new PdfObject[] { new PdfString("The answer to life is ")
                , new PdfNumber(42) });
            new PdfArray(objects).MakeIndirect(pdfDocument).Flush();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 9\n" + "0000000008 65535 f \n" + "0000000203 00000 n \n" 
                + "0000000430 00000 n \n" + "0000000264 00000 n \n" + "0000000088 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000481 00000 n \n" + "0000000658 00000 n \n" + "0000000000 00001 f \n", "xref\n" + "3 1\n" + "0000001081 00000 n \n"
                 + "8 3\n" + "0000000009 00001 f \n" + "0000000000 00001 f \n" + "0000001038 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void NotUsedIndRef01() {
            String src = "freeRefsDeletedObj.pdf";
            String @out = "notUsedIndRef01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.SetFlushUnusedObjects(true);
            PdfIndirectReference newIndRef1 = pdfDocument.CreateNextIndirectReference();
            PdfIndirectReference newIndRef2 = pdfDocument.CreateNextIndirectReference();
            IList<PdfObject> objects = JavaUtil.ArraysAsList(new PdfObject[] { new PdfString("The answer to life is ")
                , new PdfNumber(42) });
            new PdfArray(objects).MakeIndirect(pdfDocument).Flush();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 11\n" + "0000000005 65535 f \n" + "0000000308 00000 n \n"
                 + "0000000600 00000 n \n" + "0000000353 00000 n \n" + "0000000175 00000 n \n" + "0000000008 00002 f \n"
                 + "0000000651 00000 n \n" + "0000000058 00000 n \n" + "0000000009 00001 f \n" + "0000000000 00001 f \n"
                 + "0000000015 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void NotUsedIndRef02() {
            String src = "freeRefsDeletedObj.pdf";
            String @out = "notUsedIndRef02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.SetFlushUnusedObjects(false);
            PdfIndirectReference newIndRef1 = pdfDocument.CreateNextIndirectReference();
            PdfIndirectReference newIndRef2 = pdfDocument.CreateNextIndirectReference();
            IList<PdfObject> objects = JavaUtil.ArraysAsList(new PdfObject[] { new PdfString("The answer to life is ")
                , new PdfNumber(42) });
            new PdfArray(objects).MakeIndirect(pdfDocument).Flush();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 11\n" + "0000000005 65535 f \n" + "0000000308 00000 n \n"
                 + "0000000600 00000 n \n" + "0000000353 00000 n \n" + "0000000175 00000 n \n" + "0000000008 00002 f \n"
                 + "0000000651 00000 n \n" + "0000000058 00000 n \n" + "0000000009 00001 f \n" + "0000000000 00001 f \n"
                 + "0000000015 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void NotUsedIndRef03() {
            String src = "freeRefsDeletedObj.pdf";
            String @out = "notUsedIndRef03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            PdfIndirectReference newIndRef1 = pdfDocument.CreateNextIndirectReference();
            PdfIndirectReference newIndRef2 = pdfDocument.CreateNextIndirectReference();
            IList<PdfObject> objects = JavaUtil.ArraysAsList(new PdfObject[] { new PdfString("The answer to life is ")
                , new PdfNumber(42) });
            new PdfArray(objects).MakeIndirect(pdfDocument).Flush();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 7\n" + "0000000000 65535 f \n" + "0000000265 00000 n \n" 
                + "0000000564 00000 n \n" + "0000000310 00000 n \n" + "0000000132 00000 n \n" + "0000000015 00001 n \n"
                 + "0000000476 00000 n \n", "xref\n" + "0 1\n" + "0000000005 65535 f \n" + "3 3\n" + "0000000923 00000 n \n"
                 + "0000001170 00000 n \n" + "0000000000 00002 f \n" + "7 1\n" + "0000001303 00000 n \n", "xref\n" + "3 1\n"
                 + "0000001749 00000 n \n" + "5 1\n" + "0000000008 00002 f \n" + "8 3\n" + "0000000009 00001 f \n" + "0000000000 00001 f \n"
                 + "0000001706 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void CorruptedDocIndRefToFree01() {
            String src = "corruptedDocIndRefToFree.pdf";
            String @out = "corruptedDocIndRefToFree01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(destinationFolder + @out));
            PdfObject contentsObj = pdfDocument.GetPage(1).GetPdfObject().Get(PdfName.Contents);
            NUnit.Framework.Assert.AreEqual(PdfNull.PDF_NULL, contentsObj);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 7\n" + "0000000005 65535 f \n" + "0000000147 00000 n \n" 
                + "0000000439 00000 n \n" + "0000000192 00000 n \n" + "0000000015 00000 n \n" + "0000000000 00001 f \n"
                 + "0000000490 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFreeRefsListHandling01() {
            String src = "invalidFreeRefsList01.pdf";
            String @out = "invalidFreeRefsListHandling01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 15\n" + "0000000010 65535 f \n" + "0000000269 00000 n \n"
                 + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "0000000000 65535 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000011 00000 f \n"
                 + "0000000005 00000 f \n" + "0000000000 00001 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000613 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFreeRefsListHandling02() {
            String src = "invalidFreeRefsList02.pdf";
            String @out = "invalidFreeRefsListHandling02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.SetFlushUnusedObjects(true);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 18\n" + "0000000010 65535 f \n" + "0000000269 00000 n \n"
                 + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "0000000000 65535 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000015 00000 f \n"
                 + "0000000011 00000 f \n" + "0000000005 00001 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000613 00000 n \n" + "0000000016 00001 f \n" + "0000000000 00001 f \n" + "0000000702 00000 n \n"
                 };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFreeRefsListHandling03() {
            String src = "invalidFreeRefsList03.pdf";
            String @out = "invalidFreeRefsListHandling03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.SetFlushUnusedObjects(true);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 18\n" + "0000000010 65535 f \n" + "0000000269 00000 n \n"
                 + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "0000000000 65535 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000015 00000 f \n"
                 + "0000000011 00000 f \n" + "0000000005 00001 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000613 00000 n \n" + "0000000016 00001 f \n" + "0000000000 00001 f \n" + "0000000702 00000 n \n"
                 };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFreeRefsListHandling04() {
            String src = "invalidFreeRefsList04.pdf";
            String @out = "invalidFreeRefsListHandling04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.SetFlushUnusedObjects(true);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 18\n" + "0000000010 65535 f \n" + "0000000269 00000 n \n"
                 + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "0000000006 65535 f \n" + "0000000004 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000015 00000 f \n"
                 + "0000000011 00000 f \n" + "0000000005 00001 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000613 00000 n \n" + "0000000016 00001 f \n" + "0000000000 00001 f \n" + "0000000702 00000 n \n"
                 };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFreeRefsListHandling05() {
            String src = "invalidFreeRefsList05.pdf";
            String @out = "invalidFreeRefsListHandling05.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.SetFlushUnusedObjects(true);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 18\n" + "0000000005 65535 f \n" + "0000000269 00000 n \n"
                 + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "0000000000 65535 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000010 00000 f \n"
                 + "0000000011 00000 f \n" + "0000000015 00001 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000613 00000 n \n" + "0000000016 00001 f \n" + "0000000000 00001 f \n" + "0000000702 00000 n \n"
                 };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFreeRefsListHandling06() {
            String src = "invalidFreeRefsList06.pdf";
            String @out = "invalidFreeRefsListHandling06.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.SetFlushUnusedObjects(true);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 18\n" + "0000000010 65535 f \n" + "0000000269 00000 n \n"
                 + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "0000000000 65535 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000015 00000 f \n"
                 + "0000000011 00000 f \n" + "0000000005 00001 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000613 00000 n \n" + "0000000016 00001 f \n" + "0000000000 00001 f \n" + "0000000702 00000 n \n"
                 };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFreeRefsListHandling07() {
            String src = "invalidFreeRefsList07.pdf";
            String @out = "invalidFreeRefsListHandling07.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.SetFlushUnusedObjects(true);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 18\n" + "0000000010 65535 f \n" + "0000000269 00000 n \n"
                 + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "0000000000 65535 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000015 00000 f \n"
                 + "0000000011 00000 f \n" + "0000000005 00001 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000613 00000 n \n" + "0000000016 00001 f \n" + "0000000004 00001 f \n" + "0000000702 00000 n \n"
                 };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFreeRefsListHandling08() {
            String src = "invalidFreeRefsList08.pdf";
            String @out = "invalidFreeRefsListHandling08.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 18\n" + "0000000010 65535 f \n" + "0000000315 00000 n \n"
                 + "0000000607 00000 n \n" + "0000000360 00000 n \n" + "0000000000 65535 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000015 00000 f \n"
                 + "0000000011 00000 f \n" + "0000000005 00001 f \n" + "0000000179 00000 n \n" + "0000000061 00000 n \n"
                 + "0000000659 00000 n \n" + "0000000016 00001 f \n" + "0000000002 00001 f \n" + "0000000015 00000 n \n"
                , "xref\n" + "3 1\n" + "0000001278 00000 n \n" + "16 1\n" + "0000000000 00001 f \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFreeRefsListHandling09() {
            String src = "invalidFreeRefsList09.pdf";
            String @out = "invalidFreeRefsListHandling09.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 18\n" + "0000000010 65535 f \n" + "0000000315 00000 n \n"
                 + "0000000607 00000 n \n" + "0000000360 00000 n \n" + "0009999999 65535 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000015 00000 f \n"
                 + "0000000011 00000 f \n" + "0000000005 00001 f \n" + "0000000179 00000 n \n" + "0000000061 00000 n \n"
                 + "0000000659 00000 n \n" + "0000999999 00001 f \n" + "0000000000 00001 f \n" + "0000000015 00000 n \n"
                , "xref\n" + "3 2\n" + "0000001278 00000 n \n" + "0000000016 65535 f \n" + "15 1\n" + "0000000004 00001 f \n"
                 };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFreeRefsListHandling10() {
            String src = "invalidFreeRefsList10.pdf";
            String @out = "invalidFreeRefsListHandling10.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 18\n" + "0000000010 65535 f \n" + "0000000315 00000 n \n"
                 + "0000000607 00000 n \n" + "0000000360 00000 n \n" + "0000000000 65535 f \n" + "0000000006 00000 f \n"
                 + "0000000002 00000 f \n" + "0000000016 00000 f \n" + "0000000009 00000 f \n" + "0000000015 00000 f \n"
                 + "0000000011 00000 f \n" + "0000000005 00001 f \n" + "0000000179 00000 n \n" + "0000000061 00000 n \n"
                 + "0000000659 00000 n \n" + "0000000016 00001 f \n" + "0000000008 00001 f \n" + "0000000015 00000 n \n"
                , "xref\n" + "3 1\n" + "0000001278 00000 n \n" + "6 2\n" + "0000000007 00000 f \n" + "0000000008 00000 f \n"
                 + "16 1\n" + "0000000000 00001 f \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeRefsXrefStream01() {
            String src = "freeRefsGapsAndListSpecificOrder.pdf";
            String out1 = "freeRefsXrefStream01_xrefStream.pdf";
            String out2 = "freeRefsXrefStream01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + out1, new WriterProperties().SetFullCompressionMode(true)));
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(destinationFolder + out1), new PdfWriter(destinationFolder + out2
                , new WriterProperties().SetFullCompressionMode(false)));
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(out2);
            String[] expected = new String[] { "xref\n" + "0 15\n" + "0000000011 65535 f \n" + "0000000269 00000 n \n"
                 + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "0000000000 65535 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000000 00000 f \n"
                 + "0000000005 00000 f \n" + "0000000010 00001 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000613 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        /// <summary>
        /// Free refs reusing is disabled at the moment, however it might be valuable to keep an eye on such case,
        /// in case something will change.
        /// </summary>
        [NUnit.Framework.Test]
        public virtual void FreeRefsReusingTest01() {
            String src = "simpleDoc.pdf";
            String @out = "freeRefsReusingTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            PdfString s = new PdfString("New indirect object in the document.");
            PdfArray newIndObj = (PdfArray)new PdfArray(JavaCollectionsUtil.SingletonList<PdfObject>(s)).MakeIndirect(
                pdfDocument);
            pdfDocument.GetCatalog().Put(new PdfName("TestKey"), newIndObj);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 8\n" + "0000000000 65535 f \n" + "0000000265 00000 n \n" 
                + "0000000571 00000 n \n" + "0000000324 00000 n \n" + "0000000132 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000622 00000 n \n" + "0000000710 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        /// <summary>
        /// Free refs reusing is disabled at the moment, however it might be valuable to keep an eye on such case,
        /// in case something will change.
        /// </summary>
        [NUnit.Framework.Test]
        public virtual void FreeRefsReusingTest02() {
            String src = "simpleDocWithSubsections.pdf";
            String @out = "freeRefsReusingTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            PdfString s = new PdfString("New indirect object in the document.");
            PdfArray newIndObj = (PdfArray)new PdfArray(JavaCollectionsUtil.SingletonList<PdfObject>(s)).MakeIndirect(
                pdfDocument);
            pdfDocument.GetCatalog().Put(new PdfName("TestKey"), newIndObj);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 15\n" + "0000000004 65535 f \n" + "0000000269 00000 n \n"
                 + "0000000576 00000 n \n" + "0000000329 00000 n \n" + "0000000005 00000 f \n" + "0000000006 00000 f \n"
                 + "0000000007 00000 f \n" + "0000000008 00000 f \n" + "0000000009 00000 f \n" + "0000000010 00000 f \n"
                 + "0000000000 00000 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n" + "0000000628 00000 n \n"
                 + "0000000717 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        /// <summary>
        /// Free refs reusing is disabled at the moment, however it might be valuable to keep an eye on such case,
        /// in case something will change.
        /// </summary>
        [NUnit.Framework.Test]
        public virtual void FreeRefsReusingTest03() {
            String src = "simpleDocWithFreeList.pdf";
            String @out = "freeRefsReusingTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            PdfString s = new PdfString("New indirect object in the document.");
            PdfArray newIndObj = (PdfArray)new PdfArray(JavaCollectionsUtil.SingletonList<PdfObject>(s)).MakeIndirect(
                pdfDocument);
            pdfDocument.GetCatalog().Put(new PdfName("TestKey"), newIndObj);
            pdfDocument.GetCatalog().Put(new PdfName("TestKey2"), pdfDocument.GetPdfObject(10));
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 12\n" + "0000000009 65535 f \n" + "0000000265 00000 n \n"
                 + "0000000588 00000 n \n" + "0000000341 00000 n \n" + "0000000132 00000 n \n" + "0000000000 00002 f \n"
                 + "0000000639 00000 n \n" + "0000000015 00000 n \n" + "0000000005 00001 f \n" + "0000000008 00001 f \n"
                 + "0000000727 00000 n \n" + "0000000773 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        /// <summary>
        /// Free refs reusing is disabled at the moment, however it might be valuable to keep an eye on such cases,
        /// if something will change in future.
        /// </summary>
        [NUnit.Framework.Test]
        public virtual void FreeRefsReusingTest04() {
            String src = "freeRefsMaxGenOnly.pdf";
            String @out = "freeRefsReusingTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            PdfString s = new PdfString("New indirect object in the document.");
            PdfArray newIndObj = (PdfArray)new PdfArray(JavaCollectionsUtil.SingletonList<PdfObject>(s)).MakeIndirect(
                pdfDocument);
            pdfDocument.GetCatalog().Put(new PdfName("TestKey"), newIndObj);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 8\n" + "0000000005 65535 f \n" + "0000000133 00000 n \n" 
                + "0000000439 00000 n \n" + "0000000192 00000 n \n" + "0000000015 00000 n \n" + "0000000000 65535 f \n"
                 + "0000000490 00000 n \n" + "0000000578 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        /// <summary>
        /// Free refs reusing is disabled at the moment, however it might be valuable to keep an eye on such case,
        /// in case something will change.
        /// </summary>
        [NUnit.Framework.Test]
        public virtual void FreeRefsReusingTest05() {
            String src = "simpleDocWithFreeList.pdf";
            String @out = "freeRefsReusingTest05.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            PdfString s = new PdfString("New indirect object in the document.");
            PdfArray newIndObj = (PdfArray)new PdfArray(JavaCollectionsUtil.SingletonList<PdfObject>(s)).MakeIndirect(
                pdfDocument);
            newIndObj.GetIndirectReference().SetFree();
            pdfDocument.GetCatalog().Put(new PdfName("TestKey"), pdfDocument.GetPdfObject(10));
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 11\n" + "0000000009 65535 f \n" + "0000000265 00000 n \n"
                 + "0000000572 00000 n \n" + "0000000325 00000 n \n" + "0000000132 00000 n \n" + "0000000000 00002 f \n"
                 + "0000000623 00000 n \n" + "0000000015 00000 n \n" + "0000000005 00001 f \n" + "0000000008 00001 f \n"
                 + "0000000711 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void FreeRefsReusingTest06() {
            String src = "simpleDoc.pdf";
            String @out = "freeRefsReusingTest06.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            PdfObject contentsObj = pdfDocument.GetPage(1).GetPdfObject().Remove(PdfName.Contents);
            NUnit.Framework.Assert.IsTrue(contentsObj is PdfIndirectReference);
            PdfIndirectReference contentsRef = (PdfIndirectReference)contentsObj;
            contentsRef.SetFree();
            PdfString s = new PdfString("New indirect object in the document.");
            PdfArray newIndObj = (PdfArray)new PdfArray(JavaCollectionsUtil.SingletonList<PdfObject>(s)).MakeIndirect(
                pdfDocument);
            pdfDocument.GetCatalog().Put(new PdfName("TestKey"), newIndObj);
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 8\n" + "0000000005 65535 f \n" + "0000000133 00000 n \n" 
                + "0000000439 00000 n \n" + "0000000192 00000 n \n" + "0000000015 00000 n \n" + "0000000000 00001 f \n"
                 + "0000000490 00000 n \n" + "0000000578 00000 n \n" };
            CompareXrefTables(xrefString, expected);
        }

        [NUnit.Framework.Test]
        public virtual void ReadingXrefWithLotsOfFreeObjTest() {
            String input = sourceFolder + "readingXrefWithLotsOfFreeObj.pdf";
            String output = destinationFolder + "result_readingXrefWithLotsOfFreeObj.pdf";
            //Test for array out of bounds when a pdf contains multiple free references
            PdfDocument doc = new PdfDocument(new PdfReader(input), new PdfWriter(output));
            int actualNumberOfObj = doc.GetNumberOfPdfObjects();
            NUnit.Framework.Assert.AreEqual(68, actualNumberOfObj);
            NUnit.Framework.Assert.IsNull(doc.GetPdfObject(7));
            PdfXrefTable xref = doc.GetXref();
            int freeRefsCount = 0;
            for (int i = 0; i < xref.Size(); i++) {
                if (xref.Get(i).IsFree()) {
                    freeRefsCount = freeRefsCount + 1;
                }
            }
            NUnit.Framework.Assert.AreEqual(31, freeRefsCount);
            doc.Close();
        }

        private void CompareXrefTables(String[] xrefString, String[] expected) {
            NUnit.Framework.Assert.AreEqual(expected.Length, xrefString.Length);
            for (int i = 0; i < xrefString.Length; ++i) {
                if (!CompareXrefSection(xrefString[i], expected[i])) {
                    // XrefTables are different. Use Assert method in order to show differences gracefully.
                    NUnit.Framework.Assert.AreEqual(expected, xrefString);
                }
            }
        }

        private bool CompareXrefSection(String xrefSection, String expectedSection) {
            String[] xrefEntries = iText.Commons.Utils.StringUtil.Split(xrefSection, "\n");
            String[] expectedEntries = iText.Commons.Utils.StringUtil.Split(expectedSection, "\n");
            if (xrefEntries.Length != expectedEntries.Length) {
                return false;
            }
            for (int i = 0; i < xrefEntries.Length; ++i) {
                String actual = xrefEntries[i].Trim();
                String expected = expectedEntries[i].Trim();
                if (actual.EndsWith("n")) {
                    actual = actual.Substring(10);
                    expected = expected.Substring(10);
                }
                if (!actual.Equals(expected)) {
                    return false;
                }
            }
            return true;
        }

        private String[] ExtractXrefTableAsStrings(String @out) {
            byte[] outPdfBytes = ReadFile(destinationFolder + @out);
            String outPdfContent = iText.Commons.Utils.JavaUtil.GetStringForBytes(outPdfBytes, System.Text.Encoding.ASCII
                );
            String xrefStr = "\nxref";
            String trailerStr = "trailer";
            int xrefInd = outPdfContent.IndexOf(xrefStr, StringComparison.Ordinal);
            int trailerInd = outPdfContent.IndexOf(trailerStr, StringComparison.Ordinal);
            int lastXrefInd = outPdfContent.LastIndexOf(xrefStr);
            IList<String> xrefs = new List<String>();
            while (true) {
                xrefs.Add(outPdfContent.JSubstring(xrefInd + 1, trailerInd));
                if (xrefInd == lastXrefInd) {
                    break;
                }
                xrefInd = outPdfContent.IndexOf(xrefStr, xrefInd + 1);
                trailerInd = outPdfContent.IndexOf(trailerStr, trailerInd + 1);
            }
            return xrefs.ToArray(new String[xrefs.Count]);
        }
    }
}
