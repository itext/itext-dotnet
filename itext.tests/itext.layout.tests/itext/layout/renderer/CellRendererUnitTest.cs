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
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class CellRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.GET_NEXT_RENDERER_SHOULD_BE_OVERRIDDEN)]
        public virtual void GetNextRendererShouldBeOverriddenTest() {
            CellRenderer cellRenderer = new _CellRenderer_44(new Cell());
            // Nothing is overridden
            NUnit.Framework.Assert.AreEqual(typeof(CellRenderer), cellRenderer.GetNextRenderer().GetType());
        }

        private sealed class _CellRenderer_44 : CellRenderer {
            public _CellRenderer_44(Cell baseArg1)
                : base(baseArg1) {
            }
        }
    }
}
