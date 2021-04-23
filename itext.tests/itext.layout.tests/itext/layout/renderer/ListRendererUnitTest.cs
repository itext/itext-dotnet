/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    public class ListRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.GET_NEXT_RENDERER_SHOULD_BE_OVERRIDDEN)]
        public virtual void GetNextRendererShouldBeOverriddenTest() {
            ListRenderer listRenderer = new _ListRenderer_44(new List());
            // Nothing is overridden
            NUnit.Framework.Assert.AreEqual(typeof(ListRenderer), listRenderer.GetNextRenderer().GetType());
        }

        private sealed class _ListRenderer_44 : ListRenderer {
            public _ListRenderer_44(List baseArg1)
                : base(baseArg1) {
            }
        }
    }
}
