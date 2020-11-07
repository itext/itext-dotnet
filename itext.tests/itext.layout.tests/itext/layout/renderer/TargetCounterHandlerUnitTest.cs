using System;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    public class TargetCounterHandlerUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddAndGetPageByDestinationNotDocumentRendererTest() {
            RootRenderer documentRenderer = new _RootRenderer_19();
            String id = "id";
            int expectedPage = 5;
            IRenderer renderer = new _TextRenderer_38(expectedPage, new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            int? page = TargetCounterHandler.GetPageByID(renderer, id);
            NUnit.Framework.Assert.IsNull(page);
        }

        private sealed class _RootRenderer_19 : RootRenderer {
            public _RootRenderer_19() {
            }

            public override IRenderer GetNextRenderer() {
                return null;
            }

            protected internal override void FlushSingleRenderer(IRenderer resultRenderer) {
            }

            protected internal override LayoutArea UpdateCurrentArea(LayoutResult overflowResult) {
                return null;
            }
        }

        private sealed class _TextRenderer_38 : TextRenderer {
            public _TextRenderer_38(int expectedPage, Text baseArg1)
                : base(baseArg1) {
                this.expectedPage = expectedPage;
            }

            public override LayoutArea GetOccupiedArea() {
                return new LayoutArea(expectedPage, new Rectangle(50, 50));
            }

            private readonly int expectedPage;
        }

        [NUnit.Framework.Test]
        public virtual void AddAndGetPageByDestinationNullOccupiedAreaTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            String id = "id";
            IRenderer renderer = new _TextRenderer_57(new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            int? page = TargetCounterHandler.GetPageByID(renderer, id);
            NUnit.Framework.Assert.IsNull(page);
        }

        private sealed class _TextRenderer_57 : TextRenderer {
            public _TextRenderer_57(Text baseArg1)
                : base(baseArg1) {
            }

            public override LayoutArea GetOccupiedArea() {
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddAndGetPageByDestinationDoubleAddIncreasedTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            String id = "id";
            IRenderer renderer = new _TextRenderer_76(new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            TargetCounterHandler.AddPageByID(renderer);
            int? page = TargetCounterHandler.GetPageByID(renderer, id);
            NUnit.Framework.Assert.AreEqual((int?)8, page);
        }

        private sealed class _TextRenderer_76 : TextRenderer {
            public _TextRenderer_76(Text baseArg1)
                : base(baseArg1) {
                this.expectedPage = 5;
            }

            internal int expectedPage;

            public override LayoutArea GetOccupiedArea() {
                return new LayoutArea(this.expectedPage++, new Rectangle(50, 50));
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddAndGetPageByDestinationDoubleAddDecreasedTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            String id = "id";
            IRenderer renderer = new _TextRenderer_97(new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            TargetCounterHandler.AddPageByID(renderer);
            int? page = TargetCounterHandler.GetPageByID(renderer, id);
            NUnit.Framework.Assert.AreEqual((int?)4, page);
        }

        private sealed class _TextRenderer_97 : TextRenderer {
            public _TextRenderer_97(Text baseArg1)
                : base(baseArg1) {
                this.expectedPage = 5;
            }

            internal int expectedPage;

            public override LayoutArea GetOccupiedArea() {
                return new LayoutArea(this.expectedPage--, new Rectangle(50, 50));
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddAndGetPageByDestinationTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            String id = "id";
            int expectedPage = 5;
            IRenderer renderer = new _TextRenderer_119(expectedPage, new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            int? page = TargetCounterHandler.GetPageByID(renderer, id);
            NUnit.Framework.Assert.AreEqual((int?)expectedPage, page);
            IRenderer anotherRenderer = new TextRenderer(new Text("another_renderer"));
            anotherRenderer.SetParent(documentRenderer);
            page = TargetCounterHandler.GetPageByID(anotherRenderer, id);
            NUnit.Framework.Assert.AreEqual((int?)expectedPage, page);
        }

        private sealed class _TextRenderer_119 : TextRenderer {
            public _TextRenderer_119(int expectedPage, Text baseArg1)
                : base(baseArg1) {
                this.expectedPage = expectedPage;
            }

            public override LayoutArea GetOccupiedArea() {
                return new LayoutArea(expectedPage, new Rectangle(50, 50));
            }

            private readonly int expectedPage;
        }

        [NUnit.Framework.Test]
        public virtual void IsRelayoutRequiredTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            String id = "id";
            IRenderer renderer = new _TextRenderer_143(new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            NUnit.Framework.Assert.IsTrue(documentRenderer.IsRelayoutRequired());
        }

        private sealed class _TextRenderer_143 : TextRenderer {
            public _TextRenderer_143(Text baseArg1)
                : base(baseArg1) {
            }

            public override LayoutArea GetOccupiedArea() {
                int page = 4;
                return new LayoutArea(page, new Rectangle(50, 50));
            }
        }
    }
}
