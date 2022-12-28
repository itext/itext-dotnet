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
