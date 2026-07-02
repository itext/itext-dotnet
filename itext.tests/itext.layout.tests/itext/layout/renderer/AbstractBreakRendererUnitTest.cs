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
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class AbstractBreakRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void HasOwnPropertyTest() {
            AbstractBreakRenderer abstractBreakRenderer = new SectionBreakRenderer(new SectionBreak());
            abstractBreakRenderer.SetProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS, true);
            NUnit.Framework.Assert.IsTrue(abstractBreakRenderer.HasOwnProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS
                ));
        }

        [NUnit.Framework.Test]
        public virtual void HasPropertyFromItselfTest() {
            AbstractBreakRenderer abstractBreakRenderer = new SectionBreakRenderer(new SectionBreak());
            abstractBreakRenderer.SetProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS, true);
            NUnit.Framework.Assert.IsTrue(abstractBreakRenderer.HasProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS));
        }

        [NUnit.Framework.Test]
        public virtual void HasPropertyFromParentTest() {
            DivRenderer divRenderer = new DivRenderer(new Div());
            divRenderer.SetProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS, true);
            AbstractBreakRenderer abstractBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            divRenderer.AddChildRenderer(abstractBreakRenderer);
            NUnit.Framework.Assert.IsTrue(abstractBreakRenderer.HasProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS));
        }

        [NUnit.Framework.Test]
        public virtual void GetPropertyFromItselfTest() {
            AbstractBreakRenderer abstractBreakRenderer = new SectionBreakRenderer(new SectionBreak());
            abstractBreakRenderer.SetProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS, true);
            NUnit.Framework.Assert.IsTrue(abstractBreakRenderer.GetProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetPropertyFromParentTest() {
            DivRenderer divRenderer = new DivRenderer(new Div());
            divRenderer.SetProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS, true);
            AbstractBreakRenderer abstractBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            divRenderer.AddChildRenderer(abstractBreakRenderer);
            NUnit.Framework.Assert.IsTrue(abstractBreakRenderer.GetProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetPropertyNotFoundTest() {
            AbstractBreakRenderer abstractBreakRenderer = new SectionBreakRenderer(new SectionBreak());
            NUnit.Framework.Assert.IsNull(abstractBreakRenderer.GetProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetPropertyWithDefaultArgumentNotUsedTest() {
            AbstractBreakRenderer abstractBreakRenderer = new SectionBreakRenderer(new SectionBreak());
            abstractBreakRenderer.SetProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS, true);
            NUnit.Framework.Assert.IsTrue(abstractBreakRenderer.GetProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS
                , false));
        }

        [NUnit.Framework.Test]
        public virtual void GetPropertyWithDefaultArgumentUsedTest() {
            AbstractBreakRenderer abstractBreakRenderer = new SectionBreakRenderer(new SectionBreak());
            NUnit.Framework.Assert.IsFalse(abstractBreakRenderer.GetProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS
                , false));
        }

        [NUnit.Framework.Test]
        public virtual void GetOwnPropertyTest() {
            AbstractBreakRenderer abstractBreakRenderer = new SectionBreakRenderer(new SectionBreak());
            abstractBreakRenderer.SetProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS, true);
            NUnit.Framework.Assert.IsTrue(abstractBreakRenderer.GetOwnProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DeleteOwnPropertyTest() {
            AbstractBreakRenderer abstractBreakRenderer = new SectionBreakRenderer(new SectionBreak());
            abstractBreakRenderer.SetProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS, true);
            abstractBreakRenderer.DeleteOwnProperty(Property.IGNORE_AREA_AND_SECTION_BREAKS);
            NUnit.Framework.Assert.IsNull(abstractBreakRenderer.GetOwnProperty<bool?>(Property.IGNORE_AREA_AND_SECTION_BREAKS
                ));
        }
    }
}
