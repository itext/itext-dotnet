using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Svg.Renderers {
    public class SvgDrawContextTest {
        private PdfDocument tokenDoc;

        private PdfCanvas page1;

        private PdfCanvas page2;

        private SvgDrawContext context;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            tokenDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            page1 = new PdfCanvas(tokenDoc.AddNewPage());
            page2 = new PdfCanvas(tokenDoc.AddNewPage());
            context = new SvgDrawContext();
        }

        [NUnit.Framework.TearDown]
        public virtual void TearDown() {
            // release all resources
            tokenDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextEmptyStackPeekTest() {
            NUnit.Framework.Assert.That(() =>  {
                context.GetCurrentCanvas();
            }
            , NUnit.Framework.Throws.TypeOf<InvalidOperationException>());
;
        }

        [NUnit.Framework.Test]
        public virtual void DrawContextEmptyStackPopTest() {
            NUnit.Framework.Assert.That(() =>  {
                context.PopCanvas();
            }
            , NUnit.Framework.Throws.TypeOf<InvalidOperationException>());
;
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
    }
}
