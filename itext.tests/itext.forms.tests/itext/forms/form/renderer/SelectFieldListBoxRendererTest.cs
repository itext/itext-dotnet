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
using iText.Forms.Form.Element;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Forms.Form.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class SelectFieldListBoxRendererTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetNextRendererTest() {
            SelectFieldListBoxRenderer listBoxRenderer = new SelectFieldListBoxRenderer(new ListBoxField("", 0, false)
                );
            IRenderer nextRenderer = listBoxRenderer.GetNextRenderer();
            NUnit.Framework.Assert.IsTrue(nextRenderer is SelectFieldListBoxRenderer);
        }

        [NUnit.Framework.Test]
        public virtual void AllowLastYLineRecursiveExtractionTest() {
            SelectFieldListBoxRendererTest.CustomSelectFieldListBoxRenderer listBoxRenderer = new SelectFieldListBoxRendererTest.CustomSelectFieldListBoxRenderer
                (new ListBoxField("", 0, false));
            bool lastY = listBoxRenderer.CallAllowLastYLineRecursiveExtraction();
            NUnit.Framework.Assert.IsFalse(lastY);
        }

        private class CustomSelectFieldListBoxRenderer : SelectFieldListBoxRenderer {
            public CustomSelectFieldListBoxRenderer(AbstractSelectField modelElement)
                : base(modelElement) {
            }

            public virtual bool CallAllowLastYLineRecursiveExtraction() {
                return this.AllowLastYLineRecursiveExtraction();
            }
        }
    }
}
