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
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Exceptions;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgDrawContextTest : ExtendedITextTest {
        private PdfDocument tokenDoc;

        private PdfCanvas page1;

        private PdfCanvas page2;

        private SvgDrawContext context;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            tokenDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            page1 = new PdfCanvas(tokenDoc.AddNewPage());
            page2 = new PdfCanvas(tokenDoc.AddNewPage());
            context = new SvgDrawContext(null, null);
        }

        [NUnit.Framework.TearDown]
        public virtual void TearDown() {
            // release all resources
            tokenDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextEmptyDequeGetFirstTest() {
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => context.GetCurrentCanvas());
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextEmptyDequePopTest() {
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => context.PopCanvas());
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextEmptyStackCountTest() {
            NUnit.Framework.Assert.AreEqual(0, context.Size());
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextPushCountTest() {
            context.PushCanvas(page1);
            NUnit.Framework.Assert.AreEqual(1, context.Size());
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextPushPeekTest() {
            context.PushCanvas(page1);
            NUnit.Framework.Assert.AreEqual(page1, context.GetCurrentCanvas());
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextPushPopCountTest() {
            context.PushCanvas(page1);
            context.PopCanvas();
            NUnit.Framework.Assert.AreEqual(0, context.Size());
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextPushPopTest() {
            context.PushCanvas(page1);
            NUnit.Framework.Assert.AreEqual(page1, context.PopCanvas());
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextPushTwiceCountTest() {
            context.PushCanvas(page1);
            context.PushCanvas(page2);
            NUnit.Framework.Assert.AreEqual(2, context.Size());
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextPushTwicePeekTest() {
            context.PushCanvas(page1);
            context.PushCanvas(page2);
            NUnit.Framework.Assert.AreEqual(page2, context.GetCurrentCanvas());
            NUnit.Framework.Assert.AreEqual(2, context.Size());
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextPushTwicePopTest() {
            context.PushCanvas(page1);
            context.PushCanvas(page2);
            NUnit.Framework.Assert.AreEqual(page2, context.PopCanvas());
            NUnit.Framework.Assert.AreEqual(1, context.Size());
            NUnit.Framework.Assert.AreEqual(page1, context.PopCanvas());
        }

        [NUnit.Framework.Test]
        public virtual void AddISvgNodeRender() {
            String name = "expected";
            ISvgNodeRenderer expected = new GroupSvgNodeRenderer();
            this.context.AddNamedObject(name, expected);
            Object actual = this.context.GetNamedObject(name);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void AddNullToNamedObjects() {
            String name = "expected";
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => this.context.AddNamedObject
                (name, null));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.NAMED_OBJECT_NULL, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AddNamedObjectWithNullName() {
            ISvgNodeRenderer expected = new DummySvgNodeRenderer();
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => this.context.AddNamedObject
                (null, expected));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.NAMED_OBJECT_NAME_NULL_OR_EMPTY, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AddNamedObjectWithEmptyName() {
            ISvgNodeRenderer expected = new DummySvgNodeRenderer();
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => this.context.AddNamedObject
                ("", expected));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.NAMED_OBJECT_NAME_NULL_OR_EMPTY, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AddNamedRenderer() {
            ISvgNodeRenderer expected = new DummySvgNodeRenderer();
            String dummyName = "dummy";
            this.context.AddNamedObject(dummyName, expected);
            Object actual = this.context.GetNamedObject(dummyName);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void AddNamedObjects() {
            ISvgNodeRenderer expectedOne = new DummySvgNodeRenderer();
            ISvgNodeRenderer expectedTwo = new DummySvgNodeRenderer();
            ISvgNodeRenderer expectedThree = new DummySvgNodeRenderer();
            String dummyNameOne = "Ed";
            String dummyNameTwo = "Edd";
            String dummyNameThree = "Eddy";
            IDictionary<String, ISvgNodeRenderer> toAdd = new Dictionary<String, ISvgNodeRenderer>();
            toAdd.Put(dummyNameOne, expectedOne);
            toAdd.Put(dummyNameTwo, expectedTwo);
            toAdd.Put(dummyNameThree, expectedThree);
            this.context.AddNamedObjects(toAdd);
            Object actualThree = this.context.GetNamedObject(dummyNameThree);
            Object actualTwo = this.context.GetNamedObject(dummyNameTwo);
            Object actualOne = this.context.GetNamedObject(dummyNameOne);
            NUnit.Framework.Assert.AreEqual(expectedOne, actualOne);
            NUnit.Framework.Assert.AreEqual(expectedTwo, actualTwo);
            NUnit.Framework.Assert.AreEqual(expectedThree, actualThree);
        }

        [NUnit.Framework.Test]
        public virtual void AddNamedObjectAndTryToAddDuplicate() {
            ISvgNodeRenderer expectedOne = new DummySvgNodeRenderer();
            ISvgNodeRenderer expectedTwo = new DummySvgNodeRenderer();
            String dummyName = "Ed";
            context.AddNamedObject(dummyName, expectedOne);
            context.AddNamedObject(dummyName, expectedTwo);
            Object actual = context.GetNamedObject(dummyName);
            NUnit.Framework.Assert.AreEqual(expectedOne, actual);
        }
    }
}
