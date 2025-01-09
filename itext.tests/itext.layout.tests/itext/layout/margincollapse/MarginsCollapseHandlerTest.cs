/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Margincollapse {
    [NUnit.Framework.Category("UnitTest")]
    public class MarginsCollapseHandlerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 2)]
        public virtual void TestDefiningMarginCollapse() {
            // This test's aim is to test message logging.
            ParagraphRenderer paragraphRenderer = new ParagraphRenderer(new Paragraph());
            Rectangle rectangle = new Rectangle(0f, 0f);
            paragraphRenderer.GetModelElement().SetProperty(Property.MARGIN_TOP, UnitValue.CreatePercentValue(0f));
            paragraphRenderer.GetModelElement().SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePercentValue(0f));
            MarginsCollapseHandler marginsCollapseHandler = new MarginsCollapseHandler(paragraphRenderer, null);
            NUnit.Framework.Assert.DoesNotThrow(() => marginsCollapseHandler.StartMarginsCollapse(rectangle));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 2)]
        public virtual void TestHasPadding() {
            // This test's aim is to test message logging.
            ParagraphRenderer paragraphRenderer = new ParagraphRenderer(new Paragraph());
            Rectangle rectangle = new Rectangle(0f, 0f);
            paragraphRenderer.GetModelElement().SetProperty(Property.PADDING_TOP, UnitValue.CreatePercentValue(0f));
            paragraphRenderer.GetModelElement().SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePercentValue(0f));
            MarginsCollapseHandler marginsCollapseHandler = new MarginsCollapseHandler(paragraphRenderer, null);
            NUnit.Framework.Assert.DoesNotThrow(() => marginsCollapseHandler.StartMarginsCollapse(rectangle));
        }
    }
}
