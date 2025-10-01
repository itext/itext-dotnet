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
using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class FlexUtilJustifyContentTest : ExtendedITextTest {
        private const float EPS = 0.001f;

        public static IEnumerable<Object[]> JustifyContentAndFlexDirectionAndShiftsProperties() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { JustifyContent.SPACE_AROUND, FlexDirectionPropertyValue
                .ROW, new float[] { 1.666f, 3.333f, 3.333f, 10, 20 } }, new Object[] { JustifyContent.SPACE_BETWEEN, FlexDirectionPropertyValue
                .ROW, new float[] { 0, 5, 5, 0, 40 } }, new Object[] { JustifyContent.SPACE_EVENLY, FlexDirectionPropertyValue
                .ROW, new float[] { 2.5f, 2.5f, 2.5f, 13.333f, 13.333f } }, new Object[] { JustifyContent.SPACE_AROUND
                , FlexDirectionPropertyValue.ROW_REVERSE, new float[] { 3.333f, 3.333f, 1.666f, 20, 10 } }, new Object
                [] { JustifyContent.SPACE_BETWEEN, FlexDirectionPropertyValue.ROW_REVERSE, new float[] { 5, 5, 0, 40, 
                0 } }, new Object[] { JustifyContent.SPACE_EVENLY, FlexDirectionPropertyValue.ROW_REVERSE, new float[]
                 { 2.5f, 2.5f, 2.5f, 13.333f, 13.333f } }, new Object[] { JustifyContent.SPACE_AROUND, FlexDirectionPropertyValue
                .COLUMN, new float[] { 1.666f, 3.333f, 3.333f, 10, 20 } }, new Object[] { JustifyContent.SPACE_BETWEEN
                , FlexDirectionPropertyValue.COLUMN, new float[] { 0, 5, 5, 0, 40 } }, new Object[] { JustifyContent.SPACE_EVENLY
                , FlexDirectionPropertyValue.COLUMN, new float[] { 2.5f, 2.5f, 2.5f, 13.333f, 13.333f } }, new Object[
                ] { JustifyContent.SPACE_AROUND, FlexDirectionPropertyValue.COLUMN_REVERSE, new float[] { 3.333f, 3.333f
                , 1.666f, 20, 10 } }, new Object[] { JustifyContent.SPACE_BETWEEN, FlexDirectionPropertyValue.COLUMN_REVERSE
                , new float[] { 5, 5, 0, 40, 0 } }, new Object[] { JustifyContent.SPACE_EVENLY, FlexDirectionPropertyValue
                .COLUMN_REVERSE, new float[] { 2.5f, 2.5f, 2.5f, 13.333f, 13.333f } } });
        }

        [NUnit.Framework.TestCaseSource("JustifyContentAndFlexDirectionAndShiftsProperties")]
        public virtual void JustifyContentShiftsTest(JustifyContent jstCnt, FlexDirectionPropertyValue flexDir, float
            [] shifts) {
            Rectangle bBox = new Rectangle(575, 842);
            Div div = new Div().SetWidth(100).SetHeight(100);
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            flexContainerRenderer.SetProperty(Property.JUSTIFY_CONTENT, jstCnt);
            flexContainerRenderer.SetProperty(Property.FLEX_DIRECTION, flexDir);
            flexContainerRenderer.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.WRAP);
            flexContainerRenderer.SetParent(documentRenderer);
            div.SetNextRenderer(flexContainerRenderer);
            for (int i = 0; i < 5; i++) {
                Div flexItem = new Div().Add(new Paragraph(JavaUtil.IntegerToString(i)));
                flexItem.SetHeight(30).SetWidth(30);
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                flexItemRenderer.SetParent(flexContainerRenderer);
                flexContainerRenderer.AddChild(flexItemRenderer);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer());
            if (FlexDirectionPropertyValue.ROW.Equals(flexDir) || FlexDirectionPropertyValue.ROW_REVERSE.Equals(flexDir
                )) {
                NUnit.Framework.Assert.AreEqual(shifts[0], rectangleTable[0][0].GetRectangle().GetX(), EPS);
                NUnit.Framework.Assert.AreEqual(shifts[1], rectangleTable[0][1].GetRectangle().GetX(), EPS);
                NUnit.Framework.Assert.AreEqual(shifts[2], rectangleTable[0][2].GetRectangle().GetX(), EPS);
                NUnit.Framework.Assert.AreEqual(shifts[3], rectangleTable[1][0].GetRectangle().GetX(), EPS);
                NUnit.Framework.Assert.AreEqual(shifts[4], rectangleTable[1][1].GetRectangle().GetX(), EPS);
            }
            else {
                NUnit.Framework.Assert.AreEqual(shifts[0], rectangleTable[0][0].GetRectangle().GetY(), EPS);
                NUnit.Framework.Assert.AreEqual(shifts[1], rectangleTable[0][1].GetRectangle().GetY(), EPS);
                NUnit.Framework.Assert.AreEqual(shifts[2], rectangleTable[0][2].GetRectangle().GetY(), EPS);
                NUnit.Framework.Assert.AreEqual(shifts[3], rectangleTable[1][0].GetRectangle().GetY(), EPS);
                NUnit.Framework.Assert.AreEqual(shifts[4], rectangleTable[1][1].GetRectangle().GetY(), EPS);
            }
        }

        [NUnit.Framework.Test]
        public virtual void JustifyContentShiftsItemsBiggerThanContainerTest() {
            JustifyContent[] jstCnts = new JustifyContent[] { JustifyContent.SPACE_AROUND, JustifyContent.SPACE_BETWEEN
                , JustifyContent.SPACE_EVENLY };
            FlexDirectionPropertyValue[] flexDirs = new FlexDirectionPropertyValue[] { FlexDirectionPropertyValue.ROW, 
                FlexDirectionPropertyValue.ROW_REVERSE, FlexDirectionPropertyValue.COLUMN, FlexDirectionPropertyValue.
                COLUMN_REVERSE };
            foreach (JustifyContent jstCnt in jstCnts) {
                foreach (FlexDirectionPropertyValue flexDir in flexDirs) {
                    Rectangle bBox = new Rectangle(575, 842);
                    Div div = new Div().SetWidth(100).SetHeight(100);
                    DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                        ()))));
                    FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
                    flexContainerRenderer.SetProperty(Property.JUSTIFY_CONTENT, jstCnt);
                    flexContainerRenderer.SetProperty(Property.FLEX_DIRECTION, flexDir);
                    flexContainerRenderer.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.WRAP);
                    flexContainerRenderer.SetParent(documentRenderer);
                    div.SetNextRenderer(flexContainerRenderer);
                    for (int i = 0; i < 2; i++) {
                        Div flexItem = new Div().Add(new Paragraph(JavaUtil.IntegerToString(i)));
                        flexItem.SetHeight(110).SetWidth(110).SetMinHeight(110).SetMinWidth(110);
                        AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                            );
                        flexItemRenderer.SetParent(flexContainerRenderer);
                        flexContainerRenderer.AddChild(flexItemRenderer);
                    }
                    IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                        )div.GetRenderer());
                    NUnit.Framework.Assert.AreEqual(0, rectangleTable[0][0].GetRectangle().GetX(), EPS);
                    NUnit.Framework.Assert.AreEqual(0, rectangleTable[1][0].GetRectangle().GetX(), EPS);
                }
            }
        }
    }
}
