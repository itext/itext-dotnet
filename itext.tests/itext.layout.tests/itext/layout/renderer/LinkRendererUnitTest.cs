/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class LinkRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.GET_NEXT_RENDERER_SHOULD_BE_OVERRIDDEN)]
        public virtual void GetNextRendererShouldBeOverriddenTest() {
            LinkRenderer linkRenderer = new _LinkRenderer_47(new Link("test", new PdfLinkAnnotation(new Rectangle(0, 0
                ))));
            // Nothing is overridden
            NUnit.Framework.Assert.AreEqual(typeof(LinkRenderer), linkRenderer.GetNextRenderer().GetType());
        }

        private sealed class _LinkRenderer_47 : LinkRenderer {
            public _LinkRenderer_47(Link baseArg1)
                : base(baseArg1) {
            }
        }
    }
}
