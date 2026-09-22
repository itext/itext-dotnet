/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Element {
    [NUnit.Framework.Category("UnitTest")]
    public class VerticalParagraphTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.UNSUPPORTED_PROPERTY, LogLevel = LogLevelConstants.WARN)]
        public virtual void SettingUnsupportedPropertiesMustLogWarning() {
            VerticalParagraph verticalParagraph = new VerticalParagraph(false);
            Leading leading = new Leading(Leading.MULTIPLIED, 3f);
            verticalParagraph.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            verticalParagraph.SetProperty(Property.LEADING, leading);
            verticalParagraph.GetRenderer().SetParent(new VerticalParagraphTest.TestRenderer());
            // Keep sonar happy; the real test is through the log messages, so we just assert true here.
            NUnit.Framework.Assert.AreEqual(leading, verticalParagraph.GetProperty<Leading>(Property.LEADING));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.UNSUPPORTED_PROPERTY, LogLevel = LogLevelConstants.WARN)]
        public virtual void UnsupportedInheritedPropertiesMustLogWarning() {
            VerticalParagraph verticalParagraph = new VerticalParagraph(false);
            VerticalParagraphTest.TestRenderer parentRenderer = new VerticalParagraphTest.TestRenderer();
            parentRenderer.SetProperty(Property.TEXT_ANCHOR, TextAnchor.END);
            IRenderer verticalParagraphRenderer = verticalParagraph.GetRenderer();
            verticalParagraphRenderer.SetParent(parentRenderer);
            // Actual test is done through the log messages, so we just assert here to keep sonar happy.
            NUnit.Framework.Assert.IsNull(verticalParagraph.GetProperty<TextAnchor?>(Property.TEXT_ANCHOR));
        }

        private class TestRenderer : AbstractRenderer {
            public TestRenderer()
                : base() {
            }

            public override LayoutResult Layout(LayoutContext layoutContext) {
                return null;
            }

            public override IRenderer GetNextRenderer() {
                return null;
            }
        }
    }
}
