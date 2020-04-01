/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    public class AreaBreakRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddChildTestUnsupported() {
            NUnit.Framework.Assert.That(() =>  {
                AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
                NUnit.Framework.Assert.IsNull(areaBreakRenderer.GetChildRenderers());
                areaBreakRenderer.AddChild(new TextRenderer(new Text("Test")));
            }
            , NUnit.Framework.Throws.InstanceOf<Exception>())
;
        }

        [NUnit.Framework.Test]
        public virtual void DrawTestUnsupported() {
            NUnit.Framework.Assert.That(() =>  {
                AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
                areaBreakRenderer.Draw(new DrawContext(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())), null));
            }
            , NUnit.Framework.Throws.InstanceOf<NotSupportedException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void GetOccupiedAreaTestUnsupported() {
            NUnit.Framework.Assert.That(() =>  {
                AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
                areaBreakRenderer.GetOccupiedArea();
            }
            , NUnit.Framework.Throws.InstanceOf<NotSupportedException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void HasPropertyTest() {
            //Properties are not supported for AbstractRenderer, and it's expected that the result is false for all the properties.
            //The BORDER property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsFalse(areaBreakRenderer.HasProperty(Property.BORDER));
        }

        [NUnit.Framework.Test]
        public virtual void HasOwnPropertyTest() {
            //Properties are not supported for AbstractRenderer, and it's expected that the result is false for all the properties.
            //The BORDER property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsFalse(areaBreakRenderer.HasProperty(Property.BORDER));
        }

        [NUnit.Framework.Test]
        public virtual void GetPropertyTest() {
            //Properties are not supported for AbstractRenderer, and it's expected that the result is null for all the properties.
            //The BORDER property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsNull(areaBreakRenderer.GetProperty<Property>(Property.BORDER));
        }

        [NUnit.Framework.Test]
        public virtual void GetOwnPropertyTest() {
            //Properties are not supported for AbstractRenderer, and it's expected that the result is null for all the properties.
            //The BORDER property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsNull(areaBreakRenderer.GetOwnProperty<Property>(Property.BORDER));
        }

        [NUnit.Framework.Test]
        public virtual void GetDefaultPropertyTest() {
            //Properties are not supported for AbstractRenderer, and it's expected that the result is null for all the properties.
            //The BORDER property is chosen without any specific intention. It could be replaced with any other property.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            NUnit.Framework.Assert.IsNull(areaBreakRenderer.GetDefaultProperty<Property>(Property.BORDER));
        }

        [NUnit.Framework.Test]
        public virtual void GetPropertyWithDefaultValueTestUnsupported() {
            NUnit.Framework.Assert.That(() =>  {
                //The BORDER_RADIUS property is chosen without any specific intention. It could be replaced with any other property.
                AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
                areaBreakRenderer.GetProperty(Property.BORDER_RADIUS, 3);
            }
            , NUnit.Framework.Throws.InstanceOf<NotSupportedException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void SetPropertyTestUnsupported() {
            NUnit.Framework.Assert.That(() =>  {
                //The BORDER_RADIUS property is chosen without any specific intention. It could be replaced with any other property.
                AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
                areaBreakRenderer.SetProperty(Property.BORDER_RADIUS, 5);
            }
            , NUnit.Framework.Throws.InstanceOf<NotSupportedException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void DeleteOwnProperty() {
            //The BORDER property is chosen without any specific intention. It could be replaced with any other property.
            //Here we just check that no exception has been thrown.
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
            areaBreakRenderer.DeleteOwnProperty(Property.BORDER);
            NUnit.Framework.Assert.IsTrue(true);
        }

        [NUnit.Framework.Test]
        public virtual void GetModelElementTest() {
            AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
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
            NUnit.Framework.Assert.That(() =>  {
                AreaBreakRenderer areaBreakRenderer = new AreaBreakRenderer(new AreaBreak());
                areaBreakRenderer.Move(2.0f, 2.0f);
            }
            , NUnit.Framework.Throws.InstanceOf<NotSupportedException>())
;
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
