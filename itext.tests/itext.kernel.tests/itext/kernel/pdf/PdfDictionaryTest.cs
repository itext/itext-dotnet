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
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfDictionaryTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestEntrySet() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            IList<int> nums = new List<int>(JavaUtil.ArraysAsList(1, 2, 3, 4, 5, 6));
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                NUnit.Framework.Assert.AreEqual(e.Key.ToString(), "/" + e.Value);
                if (!nums.Remove(Convert.ToInt32(((PdfNumber)e.Value).IntValue()))) {
                    NUnit.Framework.Assert.Fail("Element not found");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                NUnit.Framework.Assert.IsTrue(dict.EntrySet().Contains(e));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetRemove() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            IList<KeyValuePair<PdfName, PdfObject>> toRemove = new List<KeyValuePair<PdfName, PdfObject>>();
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                toRemove.Add(e);
            }
            foreach (KeyValuePair<PdfName, PdfObject> e in toRemove) {
                NUnit.Framework.Assert.IsTrue(dict.EntrySet().Remove(e));
            }
            NUnit.Framework.Assert.AreEqual(0, dict.EntrySet().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Values().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetRemove2() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            PdfDictionary dict2 = new PdfDictionary();
            dict2.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict2.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict2.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict2.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict2.Put(new PdfName("5"), new PdfNumber(5));
            dict2.Put(new PdfName("6"), new PdfNumber(6));
            foreach (KeyValuePair<PdfName, PdfObject> e in dict2.EntrySet()) {
                NUnit.Framework.Assert.IsTrue(dict.EntrySet().Remove(e));
            }
            NUnit.Framework.Assert.AreEqual(0, dict.EntrySet().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Values().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetRemoveAll() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            IList<KeyValuePair<PdfName, PdfObject>> toRemove = new List<KeyValuePair<PdfName, PdfObject>>();
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                toRemove.Add(e);
            }
            dict.EntrySet().RemoveAll(toRemove);
            NUnit.Framework.Assert.AreEqual(0, dict.EntrySet().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Values().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetRemoveAll2() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            PdfDictionary dict2 = new PdfDictionary();
            dict2.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict2.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict2.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict2.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict2.Put(new PdfName("5"), new PdfNumber(5));
            dict2.Put(new PdfName("6"), new PdfNumber(6));
            dict.EntrySet().RemoveAll(dict2.EntrySet());
            NUnit.Framework.Assert.AreEqual(0, dict.EntrySet().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Values().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetRetainAll() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            IList<KeyValuePair<PdfName, PdfObject>> toRemove = new List<KeyValuePair<PdfName, PdfObject>>();
            int i = 0;
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                toRemove.Add(e);
                if (i++ > 2) {
                    break;
                }
            }
            dict.EntrySet().RetainAll(toRemove);
            NUnit.Framework.Assert.AreEqual(4, dict.EntrySet().Count);
            NUnit.Framework.Assert.AreEqual(4, dict.Values().Count);
            NUnit.Framework.Assert.AreEqual(4, dict.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetClear() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            dict.EntrySet().Clear();
            NUnit.Framework.Assert.AreEqual(0, dict.EntrySet().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Values().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestValues() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            IList<int> nums = new List<int>(JavaUtil.ArraysAsList(1, 2, 3, 4, 5, 6));
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                NUnit.Framework.Assert.AreEqual(e.Key.ToString(), "/" + e.Value);
                if (!nums.Remove(Convert.ToInt32(((PdfNumber)e.Value).IntValue()))) {
                    NUnit.Framework.Assert.Fail("Element not found");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestValuesContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            foreach (PdfObject v in dict.Values()) {
                NUnit.Framework.Assert.IsTrue(dict.Values().Contains(v));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestValuesIndirectContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("2"), new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            dict.Put(new PdfName("3"), new PdfNumber(3).MakeIndirect(doc));
            dict.Put(new PdfName("4"), new PdfNumber(4).MakeIndirect(doc));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("1"), false)));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("2"), false)));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("3")).GetIndirectReference()));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("4")).GetIndirectReference()));
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfNamesFetching() {
            byte[][] namesBytes = new byte[][] { new byte[] { 
                        // /#C3#9Cberschrift_1
                        35, 67, 51, 35, 57, 67, 98, 101, 114, 115, 99, 104, 114, 105, 102, 116, 95, 49 }, new byte[] { 
                        // /#C3#9Cberschrift_2
                        35, 67, 51, 35, 57, 67, 98, 101, 114, 115, 99, 104, 114, 105, 102, 116, 95, 50 }, new byte[] { 
                        // /Article
                        65, 114, 116, 105, 99, 108, 101 }, new byte[] { 
                        // /Bildunterschrift
                        66, 105, 108, 100, 117, 110, 116, 101, 114, 115, 99, 104, 114, 105, 102, 116 }, new byte[] { 
                        // /NormalParagraphStyle
                        78, 111, 114, 109, 97, 108, 80, 97, 114, 97, 103, 114, 97, 112, 104, 83, 116, 121, 108, 101 }, new byte[] 
                { 
                        // /Story
                        83, 116, 111, 114, 121 }, new byte[] { 
                        // /TOC-1
                        84, 79, 67, 45, 49 }, new byte[] { 
                        // /TOC-2-2
                        84, 79, 67, 45, 50, 45, 50 }, new byte[] { 
                        // /TOC-Head
                        84, 79, 67, 45, 72, 101, 97, 100 }, new byte[] { 
                        // /Tabelle
                        84, 97, 98, 101, 108, 108, 101 }, new byte[] { 
                        // /Tabelle_Head
                        84, 97, 98, 101, 108, 108, 101, 95, 72, 101, 97, 100 }, new byte[] { 
                        // /Tabelle_fett
                        84, 97, 98, 101, 108, 108, 101, 95, 102, 101, 116, 116 }, new byte[] { 
                        // /Text_INFO
                        84, 101, 120, 116, 95, 73, 78, 70, 79 }, new byte[] { 
                        // /Text_Info_Head
                        84, 101, 120, 116, 95, 73, 110, 102, 111, 95, 72, 101, 97, 100 }, new byte[] { 
                        // /Textk#C3#B6rper
                        84, 101, 120, 116, 107, 35, 67, 51, 35, 66, 54, 114, 112, 101, 114 }, new byte[] { 
                        // /Textk#C3#B6rper-Erstzeile
                        84, 101, 120, 116, 107, 35, 67, 51, 35, 66, 54, 114, 112, 101, 114, 45, 69, 114, 115, 116, 122, 101, 105, 
                108, 101 }, new byte[] { 
                        // /Textk#C3#B6rper_Back
                        84, 101, 120, 116, 107, 35, 67, 51, 35, 66, 54, 114, 112, 101, 114, 95, 66, 97, 99, 107 }, new byte[] { 
                        // /_No_paragraph_style_
                        95, 78, 111, 95, 112, 97, 114, 97, 103, 114, 97, 112, 104, 95, 115, 116, 121, 108, 101, 95 } };
            bool[] haveValue = new bool[] { true, true, false, true, true, true, false, false, false, false, false, false
                , false, false, false, false, false, false };
            IList<PdfName> names = new List<PdfName>();
            for (int i = 0; i < namesBytes.Length; i++) {
                byte[] b = namesBytes[i];
                PdfName n = new PdfName(b);
                names.Add(n);
                if (haveValue[i]) {
                    n.GenerateValue();
                }
            }
            PdfDictionary dict = new PdfDictionary();
            foreach (PdfName name in names) {
                dict.Put(name, new PdfName("dummy"));
            }
            PdfName expectedToContain = new PdfName("Article");
            bool found = false;
            foreach (PdfName pdfName in dict.KeySet()) {
                found = pdfName.Equals(expectedToContain);
                if (found) {
                    break;
                }
            }
            NUnit.Framework.Assert.IsTrue(found);
            NUnit.Framework.Assert.IsTrue(dict.ContainsKey(expectedToContain));
        }
    }
}
