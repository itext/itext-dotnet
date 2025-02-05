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
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils.Objectpathitems {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TrailerPathTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void EqualsAndHashCodeTest() {
            PdfDocument src = CreateDocument();
            PdfDocument dest = CreateDocument();
            Stack<LocalPathItem> stack = new Stack<LocalPathItem>();
            stack.Push(new ArrayPathItem(1));
            TrailerPath path1 = new TrailerPath(src, dest, stack);
            TrailerPath path2 = new TrailerPath(src, dest, stack);
            bool result = path1.Equals(path2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(path1.GetHashCode(), path2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsAndHashCodeTest() {
            PdfDocument src = CreateDocument();
            PdfDocument dest = CreateDocument();
            Stack<LocalPathItem> stack = new Stack<LocalPathItem>();
            stack.Push(new ArrayPathItem(1));
            TrailerPath path1 = new TrailerPath(src, dest, stack);
            stack = new Stack<LocalPathItem>();
            TrailerPath path2 = new TrailerPath(src, dest, stack);
            bool result = path1.Equals(path2);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.AreNotEqual(path1.GetHashCode(), path2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void CloneConstructorTest() {
            PdfDocument src = CreateDocument();
            PdfDocument dest = CreateDocument();
            Stack<LocalPathItem> stack = new Stack<LocalPathItem>();
            stack.Push(new ArrayPathItem(1));
            TrailerPath path1 = new TrailerPath(src, dest, stack);
            TrailerPath path2 = new TrailerPath(path1);
            bool result = path1.Equals(path2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(path1.GetHashCode(), path2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void ToStringTest() {
            PdfDocument src = CreateDocument();
            PdfDocument dest = CreateDocument();
            Stack<LocalPathItem> stack = new Stack<LocalPathItem>();
            stack.Push(new ArrayPathItem(1));
            TrailerPath path1 = new TrailerPath(src, dest, stack);
            NUnit.Framework.Assert.AreEqual("Base cmp object: trailer. Base out object: trailer\nArray index: 1", path1
                .ToString());
        }

        [NUnit.Framework.Test]
        public virtual void GetPdfDocumentsTest() {
            PdfDocument cmp = CreateDocument();
            PdfDocument @out = CreateDocument();
            Stack<LocalPathItem> stack = new Stack<LocalPathItem>();
            TrailerPath path = new TrailerPath(cmp, @out, stack);
            NUnit.Framework.Assert.AreEqual(cmp, path.GetCmpDocument());
            NUnit.Framework.Assert.AreEqual(@out, path.GetOutDocument());
        }

        [NUnit.Framework.Test]
        public virtual void ToXmlNodeTest() {
            PdfDocument src = CreateDocument();
            PdfDocument dest = CreateDocument();
            Stack<LocalPathItem> stack = new Stack<LocalPathItem>();
            stack.Push(new ArrayPathItem(1));
            TrailerPath path1 = new TrailerPath(src, dest, stack);
            XmlDocument doc = XmlUtil.InitNewXmlDocument();
            NUnit.Framework.Assert.IsNotNull(path1.ToXmlNode(doc));
        }

        private static PdfDocument CreateDocument() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            doc.AddNewPage();
            doc.Close();
            return doc;
        }
    }
}
