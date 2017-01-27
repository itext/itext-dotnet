/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2017 iText Group NV
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
using System.Collections.Generic;
using iText.IO.Source;

namespace iText.Kernel.Pdf {
    public class PdfDictionaryTest {
        [NUnit.Framework.Test]
        public virtual void TestEntrySet() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            IList<int> nums = new List<int>(iText.IO.Util.JavaUtil.ArraysAsList(1, 2, 3, 4, 5, 6));
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                NUnit.Framework.Assert.AreEqual(e.Key.ToString(), "/" + e.Value);
                if (!nums.Remove(((PdfNumber)e.Value).IntValue())) {
                    NUnit.Framework.Assert.Fail("Element not found");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
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
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            IList<KeyValuePair<PdfName, PdfObject>> toRemove = new List<KeyValuePair<PdfName, PdfObject>>();
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                toRemove.Add(e);
            }
            foreach (KeyValuePair<PdfName, PdfObject> e_1 in toRemove) {
                NUnit.Framework.Assert.IsTrue(dict.EntrySet().Remove(e_1));
            }
            NUnit.Framework.Assert.AreEqual(0, dict.EntrySet().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Values().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetRemove2() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            PdfDictionary dict2 = new PdfDictionary();
            dict2.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict2.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict2.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict2.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
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
        public virtual void TestEntrySetClear() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
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
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            IList<int> nums = new List<int>(iText.IO.Util.JavaUtil.ArraysAsList(1, 2, 3, 4, 5, 6));
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                NUnit.Framework.Assert.AreEqual(e.Key.ToString(), "/" + e.Value);
                if (!nums.Remove(((PdfNumber)e.Value).IntValue())) {
                    NUnit.Framework.Assert.Fail("Element not found");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestValuesContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
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
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("1"), false)));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("2"), false)));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("3")).GetIndirectReference()));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("4")).GetIndirectReference()));
        }
    }
}
