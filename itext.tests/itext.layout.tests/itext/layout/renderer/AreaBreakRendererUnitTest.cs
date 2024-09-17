/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class AreaBreakRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddChildTestUnsupported() {
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsNull(areaBreakRenderer.GetChildRenderers());
            NUnit.Framework.Assert.Catch(typeof(Exception), () => areaBreakRenderer.AddChild(new TextRenderer(new Text
                ("Test"))));
        }

        [NUnit.Framework.Test]
        public virtual void DrawTestUnsupported() {
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => areaBreakRenderer.Draw(new DrawContext(new 
                PdfDocument(new PdfWriter(new ByteArrayOutputStream())), null)));
        }

        [NUnit.Framework.Test]
        public virtual void GetOccupiedAreaTestUnsupported() {
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => areaBreakRenderer.GetOccupiedArea());
        }

        [NUnit.Framework.Test]
        public virtual void HasPropertyTest() {
            //Properties are not supported for AbstractRenderer, and it's expected that the result is false for all the properties.
            //The AREA_BREAK_TYPE property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsFalse(areaBreakRenderer.HasProperty(Property.AREA_BREAK_TYPE));
        }

        [NUnit.Framework.Test]
        public virtual void HasOwnPropertyTest() {
            //Properties are not supported for AbstractRenderer, and it's expected that the result is false for all the properties.
            //The AREA_BREAK_TYPE property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsFalse(areaBreakRenderer.HasOwnProperty(Property.AREA_BREAK_TYPE));
        }

        [NUnit.Framework.Test]
        public virtual void GetPropertyTest() {
            //Properties are not supported for AbstractRenderer, and it's expected that the result is null for all the properties.
            //The AREA_BREAK_TYPE property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsNull(areaBreakRenderer.GetProperty<Property>(Property.AREA_BREAK_TYPE));
        }

        [NUnit.Framework.Test]
        public virtual void GetOwnPropertyTest() {
            //Properties are not supported for AbstractRenderer, and it's expected that the result is null for all the properties.
            //The AREA_BREAK_TYPE property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsNull(areaBreakRenderer.GetOwnProperty<Property>(Property.AREA_BREAK_TYPE));
        }

        [NUnit.Framework.Test]
        public virtual void GetDefaultPropertyTest() {
            //Properties are not supported for AbstractRenderer, and it's expected that the result is null for all the properties.
            //The AREA_BREAK_TYPE property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsNull(areaBreakRenderer.GetDefaultProperty<Property>(Property.AREA_BREAK_TYPE));
        }

        [NUnit.Framework.Test]
        public virtual void GetPropertyWithDefaultValueTestUnsupported() {
            //The BORDER_BOTTOM_LEFT_RADIUS property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => areaBreakRenderer.GetProperty(Property.BORDER_BOTTOM_LEFT_RADIUS
                , 3));
        }

        [NUnit.Framework.Test]
        public virtual void SetPropertyTestUnsupported() {
            //The BORDER_BOTTOM_LEFT_RADIUS property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => areaBreakRenderer.SetProperty(Property.BORDER_BOTTOM_LEFT_RADIUS
                , 5));
        }

        [NUnit.Framework.Test]
        public virtual void DeleteOwnProperty() {
            //The AREA_BREAK_TYPE property is chosen without any specific intention. It could be replaced with any other property.
            //Here we just check that no exception has been thrown.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.DoesNotThrow(() => areaBreakRenderer.DeleteOwnProperty(Property.AREA_BREAK_TYPE));
        }

        [NUnit.Framework.Test]
        public virtual void GetModelElementTest() {
            AreaBreak areaBreak = new AreaBreak();
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(areaBreak);
            NUnit.Framework.Assert.IsNull(areaBreakRenderer.GetModelElement());
        }

        [NUnit.Framework.Test]
        public virtual void GetParentTest() {
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsNull(areaBreakRenderer.GetParent());
        }

        [NUnit.Framework.Test]
        public virtual void SetParentTest() {
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.AreEqual(areaBreakRenderer, areaBreakRenderer.SetParent(new AreaBreakRenderer(new AreaBreak
                ())));
        }

        [NUnit.Framework.Test]
        public virtual void IsFlushedTest() {
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsFalse(areaBreakRenderer.IsFlushed());
        }

        [NUnit.Framework.Test]
        public virtual void MoveTestUnsupported() {
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => areaBreakRenderer.Move(2.0f, 2.0f));
        }

        [NUnit.Framework.Test]
        public virtual void GetNextRendererTest() {
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsNull(areaBreakRenderer.GetNextRenderer());
        }

        [NUnit.Framework.Test]
        public virtual void LayoutTest() {
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            LayoutResult layoutResult = areaBreakRenderer.Layout(new LayoutContext(null));
            NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, layoutResult.GetStatus());
            NUnit.Framework.Assert.IsNull(layoutResult.GetOccupiedArea());
            NUnit.Framework.Assert.IsNull(layoutResult.GetSplitRenderer());
            NUnit.Framework.Assert.IsNull(layoutResult.GetOverflowRenderer());
            NUnit.Framework.Assert.AreEqual(areaBreakRenderer, layoutResult.GetCauseOfNothing());
            NUnit.Framework.Assert.AreEqual(areaBreakRenderer.areaBreak, layoutResult.GetAreaBreak());
        }
    }
}
