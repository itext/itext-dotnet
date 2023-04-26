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
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class TargetCounterHandlerUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddAndGetPageByDestinationNotDocumentRendererTest() {
            RootRenderer documentRenderer = new _RootRenderer_41();
            String id = "id";
            int expectedPage = 5;
            IRenderer renderer = new _TextRenderer_60(expectedPage, new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            int? page = TargetCounterHandler.GetPageByID(renderer, id);
            NUnit.Framework.Assert.IsNull(page);
        }

        private sealed class _RootRenderer_41 : RootRenderer {
            public _RootRenderer_41() {
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

        private sealed class _TextRenderer_60 : TextRenderer {
            public _TextRenderer_60(int expectedPage, Text baseArg1)
                : base(baseArg1) {
                this.expectedPage = expectedPage;
            }

            public override LayoutArea GetOccupiedArea() {
                return new LayoutArea(expectedPage, new Rectangle(50, 50));
            }

            private readonly int expectedPage;
        }

        [NUnit.Framework.Test]
        public virtual void IsValueDefinedForThisIdNotDocumentRendererTest() {
            RootRenderer documentRenderer = new _RootRenderer_76();
            String id = "id";
            IRenderer renderer = new _TextRenderer_94(new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            NUnit.Framework.Assert.IsFalse(TargetCounterHandler.IsValueDefinedForThisId(renderer, id));
        }

        private sealed class _RootRenderer_76 : RootRenderer {
            public _RootRenderer_76() {
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

        private sealed class _TextRenderer_94 : TextRenderer {
            public _TextRenderer_94(Text baseArg1)
                : base(baseArg1) {
            }

            public override LayoutArea GetOccupiedArea() {
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddAndGetPageByDestinationNullOccupiedAreaTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            String id = "id";
            IRenderer renderer = new _TextRenderer_111(new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            int? page = TargetCounterHandler.GetPageByID(renderer, id);
            NUnit.Framework.Assert.IsNull(page);
        }

        private sealed class _TextRenderer_111 : TextRenderer {
            public _TextRenderer_111(Text baseArg1)
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
            IRenderer renderer = new _TextRenderer_130(new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            TargetCounterHandler.AddPageByID(renderer);
            documentRenderer.GetTargetCounterHandler().PrepareHandlerToRelayout();
            int? page = TargetCounterHandler.GetPageByID(renderer, id);
            NUnit.Framework.Assert.AreEqual((int?)8, page);
        }

        private sealed class _TextRenderer_130 : TextRenderer {
            public _TextRenderer_130(Text baseArg1)
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
            IRenderer renderer = new _TextRenderer_152(new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            TargetCounterHandler.AddPageByID(renderer);
            documentRenderer.GetTargetCounterHandler().PrepareHandlerToRelayout();
            int? page = TargetCounterHandler.GetPageByID(renderer, id);
            NUnit.Framework.Assert.AreEqual((int?)2, page);
        }

        private sealed class _TextRenderer_152 : TextRenderer {
            public _TextRenderer_152(Text baseArg1)
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
            IRenderer renderer = new _TextRenderer_175(expectedPage, new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            documentRenderer.GetTargetCounterHandler().PrepareHandlerToRelayout();
            int? page = TargetCounterHandler.GetPageByID(renderer, id);
            NUnit.Framework.Assert.AreEqual((int?)expectedPage, page);
            IRenderer anotherRenderer = new TextRenderer(new Text("another_renderer"));
            anotherRenderer.SetParent(documentRenderer);
            page = TargetCounterHandler.GetPageByID(anotherRenderer, id);
            NUnit.Framework.Assert.AreEqual((int?)expectedPage, page);
        }

        private sealed class _TextRenderer_175 : TextRenderer {
            public _TextRenderer_175(int expectedPage, Text baseArg1)
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
            IRenderer renderer = new _TextRenderer_200(new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            NUnit.Framework.Assert.IsFalse(documentRenderer.IsRelayoutRequired());
            TargetCounterHandler.AddPageByID(renderer);
            NUnit.Framework.Assert.IsTrue(documentRenderer.IsRelayoutRequired());
            documentRenderer.GetTargetCounterHandler().PrepareHandlerToRelayout();
            NUnit.Framework.Assert.IsFalse(documentRenderer.IsRelayoutRequired());
        }

        private sealed class _TextRenderer_200 : TextRenderer {
            public _TextRenderer_200(Text baseArg1)
                : base(baseArg1) {
            }

            public override LayoutArea GetOccupiedArea() {
                int page = 4;
                return new LayoutArea(page, new Rectangle(50, 50));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyConstructorTest() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            String id = "id";
            IRenderer renderer = new _TextRenderer_222(new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            TargetCounterHandler copy = new TargetCounterHandler(documentRenderer.GetTargetCounterHandler());
            NUnit.Framework.Assert.IsTrue(copy.IsRelayoutRequired());
        }

        private sealed class _TextRenderer_222 : TextRenderer {
            public _TextRenderer_222(Text baseArg1)
                : base(baseArg1) {
            }

            public override LayoutArea GetOccupiedArea() {
                int page = 4;
                return new LayoutArea(page, new Rectangle(50, 50));
            }
        }

        [NUnit.Framework.Test]
        public virtual void IsValueDefinedForThisId() {
            DocumentRenderer documentRenderer = new DocumentRenderer(null);
            String id = "id";
            String notAddedId = "not added id";
            IRenderer renderer = new _TextRenderer_243(new Text("renderer"));
            renderer.SetParent(documentRenderer);
            renderer.SetProperty(Property.ID, id);
            TargetCounterHandler.AddPageByID(renderer);
            NUnit.Framework.Assert.IsTrue(TargetCounterHandler.IsValueDefinedForThisId(renderer, id));
            NUnit.Framework.Assert.IsFalse(TargetCounterHandler.IsValueDefinedForThisId(renderer, notAddedId));
        }

        private sealed class _TextRenderer_243 : TextRenderer {
            public _TextRenderer_243(Text baseArg1)
                : base(baseArg1) {
            }

            public override LayoutArea GetOccupiedArea() {
                int page = 4;
                return new LayoutArea(page, new Rectangle(50, 50));
            }
        }
    }
}
