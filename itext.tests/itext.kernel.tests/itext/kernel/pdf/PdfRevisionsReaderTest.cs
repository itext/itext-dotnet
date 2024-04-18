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
using System.Collections.Generic;
using System.Linq;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfRevisionsReaderTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfRevisionsReaderTest/";

        [NUnit.Framework.Test]
        public virtual void SingleRevisionDocumentTest() {
            String filename = SOURCE_FOLDER + "singleRevisionDocument.pdf";
            using (PdfReader reader = new PdfReader(filename)) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(reader);
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                NUnit.Framework.Assert.AreEqual(1, documentRevisions.Count);
                DocumentRevision firstRevision = documentRevisions[0];
                AssertResultingRevision(firstRevision, 1, 2, 3, 4, 5, 6);
                NUnit.Framework.Assert.AreEqual(929, firstRevision.GetEofOffset());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SingleRevisionWithXrefStreamTest() {
            String filename = SOURCE_FOLDER + "singleRevisionWithXrefStream.pdf";
            using (PdfReader reader = new PdfReader(filename)) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(reader);
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                NUnit.Framework.Assert.AreEqual(1, documentRevisions.Count);
                DocumentRevision firstRevision = documentRevisions[0];
                AssertResultingRevision(firstRevision, 1, 2, 3, 4, 5, 6, 7, 8);
                NUnit.Framework.Assert.AreEqual(1085, firstRevision.GetEofOffset());
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRevisionsDocument() {
            String filename = SOURCE_FOLDER + "multipleRevisionsDocument.pdf";
            using (PdfReader reader = new PdfReader(filename)) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(reader);
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                NUnit.Framework.Assert.AreEqual(3, documentRevisions.Count);
                DocumentRevision firstRevision = documentRevisions[0];
                AssertResultingRevision(firstRevision, 1, 2, 3, 4, 5, 6);
                NUnit.Framework.Assert.AreEqual(929, firstRevision.GetEofOffset());
                DocumentRevision secondRevision = documentRevisions[1];
                AssertResultingRevision(secondRevision, 1, 3, 4, 7, 8, 9, 10, 11, 12, 13, 14, 15);
                NUnit.Framework.Assert.AreEqual(28119, secondRevision.GetEofOffset());
                DocumentRevision thirdRevision = documentRevisions[2];
                AssertResultingRevision(thirdRevision, 1, 3, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28);
                NUnit.Framework.Assert.AreEqual(36207, thirdRevision.GetEofOffset());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesDocument() {
            String filename = SOURCE_FOLDER + "freeReferencesDocument.pdf";
            using (PdfReader reader = new PdfReader(filename)) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(reader);
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                NUnit.Framework.Assert.AreEqual(5, documentRevisions.Count);
                DocumentRevision firstRevision = documentRevisions[0];
                AssertResultingRevision(firstRevision, 1, 2, 3, 4, 5, 6);
                NUnit.Framework.Assert.AreEqual(929, firstRevision.GetEofOffset());
                DocumentRevision secondRevision = documentRevisions[1];
                AssertResultingRevision(secondRevision, 1, 3, 4, 7, 8, 9, 10, 11, 12, 13, 14, 15);
                NUnit.Framework.Assert.AreEqual(28119, secondRevision.GetEofOffset());
                DocumentRevision thirdRevision = documentRevisions[2];
                AssertResultingRevision(thirdRevision, 1, 3, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28);
                NUnit.Framework.Assert.AreEqual(36207, thirdRevision.GetEofOffset());
                DocumentRevision fourthRevision = documentRevisions[3];
                AssertResultingRevision(fourthRevision, new int[] { 1, 3, 23, 24 }, new int[] { 0, 0, 1, 1 });
                NUnit.Framework.Assert.AreEqual(37006, fourthRevision.GetEofOffset());
                DocumentRevision fifthRevision = documentRevisions[4];
                AssertResultingRevision(fifthRevision, new int[] { 1, 3, 19, 20, 21, 22, 23, 25 }, new int[] { 0, 0, 1, 1, 
                    1, 1, 1, 1 });
                NUnit.Framework.Assert.AreEqual(38094, fifthRevision.GetEofOffset());
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRevisionsWithXrefStreamTest() {
            String filename = SOURCE_FOLDER + "multipleRevisionsWithXrefStream.pdf";
            using (PdfReader reader = new PdfReader(filename)) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(reader);
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                NUnit.Framework.Assert.AreEqual(3, documentRevisions.Count);
                DocumentRevision firstRevision = documentRevisions[0];
                AssertResultingRevision(firstRevision, 1, 2, 3, 4, 5, 6, 7, 8);
                NUnit.Framework.Assert.AreEqual(1085, firstRevision.GetEofOffset());
                DocumentRevision secondRevision = documentRevisions[1];
                AssertResultingRevision(secondRevision, 1, 3, 4, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19);
                NUnit.Framework.Assert.AreEqual(28137, secondRevision.GetEofOffset());
                DocumentRevision thirdRevision = documentRevisions[2];
                AssertResultingRevision(thirdRevision, 1, 3, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34);
                NUnit.Framework.Assert.AreEqual(36059, thirdRevision.GetEofOffset());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesWithXrefStream() {
            String filename = SOURCE_FOLDER + "freeReferencesWithXrefStream.pdf";
            using (PdfReader reader = new PdfReader(filename)) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(reader);
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                NUnit.Framework.Assert.AreEqual(5, documentRevisions.Count);
                DocumentRevision firstRevision = documentRevisions[0];
                AssertResultingRevision(firstRevision, 1, 2, 3, 4, 5, 6, 7, 8);
                NUnit.Framework.Assert.AreEqual(1085, firstRevision.GetEofOffset());
                DocumentRevision secondRevision = documentRevisions[1];
                AssertResultingRevision(secondRevision, 1, 3, 4, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19);
                NUnit.Framework.Assert.AreEqual(28137, secondRevision.GetEofOffset());
                DocumentRevision thirdRevision = documentRevisions[2];
                AssertResultingRevision(thirdRevision, 1, 3, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34);
                NUnit.Framework.Assert.AreEqual(36059, thirdRevision.GetEofOffset());
                DocumentRevision fourthRevision = documentRevisions[3];
                AssertResultingRevision(fourthRevision, new int[] { 1, 3, 27, 28, 35 }, new int[] { 0, 0, 1, 1, 0 });
                NUnit.Framework.Assert.AreEqual(36975, fourthRevision.GetEofOffset());
                DocumentRevision fifthRevision = documentRevisions[4];
                AssertResultingRevision(fifthRevision, new int[] { 1, 3, 23, 24, 25, 26, 27, 29, 36 }, new int[] { 0, 0, 1
                    , 1, 1, 1, 1, 1, 0 });
                NUnit.Framework.Assert.AreEqual(38111, fifthRevision.GetEofOffset());
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithStreamAndTableXref() {
            String filename = SOURCE_FOLDER + "documentWithStreamAndTableXref.pdf";
            using (PdfReader reader = new PdfReader(filename)) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(reader);
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                NUnit.Framework.Assert.AreEqual(3, documentRevisions.Count);
                DocumentRevision thirdRevision = revisionsReader.GetAllRevisions()[0];
                // xref was broken in this revision and fixed in the next one
                AssertResultingRevision(thirdRevision, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new int[] { 0, 0, 0, 0, 0, 
                    0, 65535, 0, 0 });
                NUnit.Framework.Assert.AreEqual(1381, thirdRevision.GetEofOffset());
                DocumentRevision secondRevision = revisionsReader.GetAllRevisions()[1];
                AssertResultingRevision(secondRevision, 1, 2, 3, 4, 5, 6, 7, 8);
                NUnit.Framework.Assert.AreEqual(1550, secondRevision.GetEofOffset());
                DocumentRevision firstRevision = revisionsReader.GetAllRevisions()[2];
                AssertResultingRevision(firstRevision);
                NUnit.Framework.Assert.AreEqual(1550, firstRevision.GetEofOffset());
            }
        }

        private void AssertResultingRevision(DocumentRevision documentRevision, params int[] objNumbers) {
            AssertResultingRevision(documentRevision, objNumbers, new int[objNumbers.Length]);
        }

        private void AssertResultingRevision(DocumentRevision documentRevision, int[] objNumbers, int[] objGens) {
            NUnit.Framework.Assert.AreEqual(objNumbers.Length, objGens.Length);
            NUnit.Framework.Assert.AreEqual(objNumbers.Length + 1, documentRevision.GetModifiedObjects().Count);
            for (int i = 0; i < objNumbers.Length; ++i) {
                int objNumber = objNumbers[i];
                int objGen = objGens[i];
                NUnit.Framework.Assert.IsTrue(documentRevision.GetModifiedObjects().Any((reference) => reference.GetObjNumber
                    () == objNumber && reference.GetGenNumber() == objGen));
            }
            NUnit.Framework.Assert.IsTrue(documentRevision.GetModifiedObjects().Any((reference) => reference.GetObjNumber
                () == 0 && reference.GetGenNumber() == 65535 && reference.IsFree()));
        }
    }
}
