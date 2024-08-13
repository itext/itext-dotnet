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
using System.IO;
using iText.IO.Util;
using iText.Kernel.Geom;
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

        [NUnit.Framework.Test]
        public virtual void RootTransformText() {
            AffineTransform at = new AffineTransform();
            NUnit.Framework.Assert.AreEqual(at, context.GetRootTransform());
            at.SetToRotation(MathUtil.ToRadians(45));
            context.SetRootTransform(at);
            NUnit.Framework.Assert.AreEqual(at, context.GetRootTransform());
        }
    }
}
