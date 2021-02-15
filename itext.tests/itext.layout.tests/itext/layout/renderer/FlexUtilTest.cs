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
using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    public class FlexUtilTest : ExtendedITextTest {
        private const float EPS = 0.001f;

        private static readonly Style DEFAULT_STYLE;

        private static readonly Style WRAP_STYLE;

        private static readonly IList<UnitValue> NULL_FLEX_BASIS_LIST;

        // Here one can find an html version for all of the tests
        // TODO DEVSIX-5049 Make html2pdf+layout tests from these htmls
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FlexUtilTest/";

        static FlexUtilTest() {
            DEFAULT_STYLE = new Style().SetWidth(400).SetHeight(100);
            WRAP_STYLE = new Style().SetWidth(400).SetHeight(100);
            WRAP_STYLE.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.WRAP);
            NULL_FLEX_BASIS_LIST = new List<UnitValue>();
            for (int i = 0; i < 3; i++) {
                NULL_FLEX_BASIS_LIST.Add(null);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DefaultTest01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(NULL_FLEX_BASIS_LIST, JavaUtil.ArraysAsList(1f, 1f, 1f
                ), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(400f / 3, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveSizePtTest01() {
            Div div = new Div().SetWidth(UnitValue.CreatePercentValue(80));
            float size = FlexUtil.RetrieveSize(div.CreateRendererSubTree(), Property.WIDTH, 100);
            NUnit.Framework.Assert.AreEqual(80f, size, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveSizeNoSetWidthTest01() {
            Div div = new Div();
            float size = FlexUtil.RetrieveSize(div.CreateRendererSubTree(), Property.WIDTH, 100);
            NUnit.Framework.Assert.AreEqual(100f, size, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void Item1BasisGtWidthGrow0Shrink01Test01() {
            Rectangle bBox = new Rectangle(545, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(150f), UnitValue.CreatePointValue
                (50f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(100).SetHeight(100);
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph("x"));
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 0, 
                    0.1f, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(135f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(45f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisGrow1Shrink0MarginBorderPaddingOnContainerTest01() {
            Style style = new Style().SetWidth(100).SetHeight(100).SetMargin(15).SetBorder(new SolidBorder(10)).SetPadding
                (50);
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(style, JavaUtil.ArraysAsList(UnitValue.CreatePointArray
                (new float[] { 10f, 20f, 30f })), JavaUtil.ArraysAsList(1f, 1f, 1f), JavaUtil.ArraysAsList(0f, 0f, 0f)
                );
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            NUnit.Framework.Assert.AreEqual(23.333334f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(33.333336f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(43.333336f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisGrow1Shrink0MarginBorderPaddingOnContainerNoWidthTest01() {
            Style style = new Style().SetMargin(15).SetBorder(new SolidBorder(10)).SetPadding(5);
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(style, JavaUtil.ArraysAsList(UnitValue.CreatePointArray
                (new float[] { 50f, 100f, 150f })), JavaUtil.ArraysAsList(1f, 1f, 1f), JavaUtil.ArraysAsList(0f, 0f, 0f
                ));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            NUnit.Framework.Assert.AreEqual(104.333336f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(154.33334f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(204.33334f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleStretchTest01() {
            Style stretchStyle = new Style(WRAP_STYLE);
            stretchStyle.SetProperty(Property.ALIGN_CONTENT, AlignmentPropertyValue.STRETCH);
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(stretchStyle, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (100f)), JavaUtil.ArraysAsList(0f), JavaUtil.ArraysAsList(0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(100f, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BasisGtWidthGrow0Shrink0Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (500f)), JavaUtil.ArraysAsList(0f), JavaUtil.ArraysAsList(0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(500f, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BasisGtWidthGrow0Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (500f)), JavaUtil.ArraysAsList(0f), JavaUtil.ArraysAsList(1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(400f, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BasisMinGrow0Shrink1Item2Grow05Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(NULL_FLEX_BASIS_LIST, JavaUtil.ArraysAsList(0f, 0.5f, 
                0f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            for (int i = 0; i < rectangleTable.Count; i++) {
                FlexItemInfo flexItemInfo = rectangleTable[0][i];
                NUnit.Framework.Assert.AreEqual(i == 1 ? 197 : 6f, flexItemInfo.GetRectangle().GetWidth(), EPS);
                NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BasisMinGrow0Shrink1Item2Grow2Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(NULL_FLEX_BASIS_LIST, JavaUtil.ArraysAsList(0f, 2f, 0f
                ), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            for (int i = 0; i < rectangleTable.Count; i++) {
                FlexItemInfo flexItemInfo = rectangleTable[0][i];
                NUnit.Framework.Assert.AreEqual(i == 1 ? 388f : 6f, flexItemInfo.GetRectangle().GetWidth(), EPS);
                NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BasisMinGrow2Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(NULL_FLEX_BASIS_LIST, JavaUtil.ArraysAsList(2f, 2f, 2f
                ), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(400f / 3, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BasisMinGrow05SumGt1Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(NULL_FLEX_BASIS_LIST, JavaUtil.ArraysAsList(0.5f, 0.5f
                , 0.5f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(400f / 3, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BasisMinGrow01SumLt1Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(NULL_FLEX_BASIS_LIST, JavaUtil.ArraysAsList(0.1f, 0.1f
                , 0.1f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(44.2f, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BasisMinGrow0Shrink05SumGt1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(NULL_FLEX_BASIS_LIST, JavaUtil.ArraysAsList(0f, 0f, 0f
                ), JavaUtil.ArraysAsList(0.5f, 0.5f, 0.5f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(6f, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BasisMinGrow0Shrink01SumLt1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(NULL_FLEX_BASIS_LIST, JavaUtil.ArraysAsList(0f, 0f, 0f
                ), JavaUtil.ArraysAsList(0.1f, 0.1f, 0.1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(6f, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void Basis50SumLtWidthGrow0Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(50f)
                , UnitValue.CreatePointValue(50f), UnitValue.CreatePointValue(50f)), JavaUtil.ArraysAsList(0f, 0f, 0f)
                , JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(50f, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void Basis250SumGtWidthGrow0Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(250f
                ), UnitValue.CreatePointValue(250f), UnitValue.CreatePointValue(250f)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(400f / 3, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow0Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(50f)
                , UnitValue.CreatePointValue(80f), UnitValue.CreatePointValue(100f)), JavaUtil.ArraysAsList(0f, 0f, 0f
                ), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(50f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(80f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(100f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(50f)
                , UnitValue.CreatePointValue(80f), UnitValue.CreatePointValue(100f)), JavaUtil.ArraysAsList(1f, 1f, 1f
                ), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(106.66667f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(136.66667f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(156.66667f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink0Item2MarginBorderPadding30Test01() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(50f), UnitValue.CreatePointValue
                (80f), UnitValue.CreatePointValue(100f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(400).SetHeight(100);
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph("x"));
                if (1 == i) {
                    flexItem.SetMargin(10).SetBorder(new SolidBorder(15)).SetPadding(5);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    0, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(86.66667f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(176.66667f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(136.66667f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink1Item2MarginBorderPadding30Test01() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(50f), UnitValue.CreatePointValue
                (80f), UnitValue.CreatePointValue(100f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(200).SetHeight(100);
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph("x"));
                if (1 == i) {
                    flexItem.SetMargin(10).SetBorder(new SolidBorder(15)).SetPadding(5);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    1, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(30.434784f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(108.69565f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(60.869568f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink1Item2MuchContentTest01() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(37.5f), UnitValue.CreatePointValue
                (60f), UnitValue.CreatePointValue(75f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(300).SetHeight(100);
            // We use Courier as a monotype font to ensure that min width calculated by iText
            // is more or less the same as the width calculated by browsers
            FontProvider provider = new FontProvider();
            provider.GetFontSet().AddFont(StandardFonts.COURIER, null, "courier");
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            documentRenderer.SetProperty(Property.FONT_PROVIDER, provider);
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph(1 == i ? "2222222222222222222222222" : JavaUtil.IntegerToString
                    (i)));
                if (1 == i) {
                    flexItem.SetFontFamily(StandardFontFamilies.COURIER);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    1, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(41.250023f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(179.99995f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(78.75002f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink1Item2MuchContentSetMinWidthLtBasisTest01() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(37.5f), UnitValue.CreatePointValue
                (60f), UnitValue.CreatePointValue(75f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(300).SetHeight(100);
            // We use Courier as a monotype font to ensure that min width calculated by iText
            // is more or less the same as the width calculated by browsers
            FontProvider provider = new FontProvider();
            provider.GetFontSet().AddFont(StandardFonts.COURIER, null, "courier");
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            documentRenderer.SetProperty(Property.FONT_PROVIDER, provider);
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph(1 == i ? "2222222222222222222222222" : JavaUtil.IntegerToString
                    (i)));
                if (1 == i) {
                    flexItem.SetFontFamily(StandardFontFamilies.COURIER).SetMinWidth(37.5f);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    1, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(80f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(102.5f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(117.5f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink1Item2MaxWidthLtBasisTest01() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(50f), UnitValue.CreatePointValue
                (80f), UnitValue.CreatePointValue(100f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(400).SetHeight(100);
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph(JavaUtil.IntegerToString(i)));
                if (1 == i) {
                    flexItem.SetMaxWidth(50f);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    1, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(150f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(50f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(200f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink1Item2MaxWidthLtBasisTest02() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(50f), UnitValue.CreatePointValue
                (80f), UnitValue.CreatePointValue(100f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(100).SetHeight(100);
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph(JavaUtil.IntegerToString(i)));
                if (1 == i) {
                    flexItem.SetMaxWidth(30f);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    1, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(23.333332f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(30f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(46.666664f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink1Item2MaxWidthLtBasisTest03() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(7f), UnitValue.CreatePointValue
                (80f), UnitValue.CreatePointValue(7f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(100).SetHeight(100);
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph(JavaUtil.IntegerToString(i)));
                if (1 == i) {
                    flexItem.SetMaxWidth(30f);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    1, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(35f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(30f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(35f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow1Shrink1Item1MinWidthGtBasisTest01() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(100f), UnitValue.CreatePointValue
                (150f), UnitValue.CreatePointValue(200f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(400).SetHeight(100);
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph(JavaUtil.IntegerToString(i)));
                if (0 == i) {
                    flexItem.SetMinWidth(150f);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    1, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(150f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(107.14285f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(142.85715f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ImgGtUsedWidthTest01() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(50f), UnitValue.CreatePointValue
                (30f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(100).SetHeight(100);
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            for (int i = 0; i < flexBasisValues.Count; i++) {
                IElement flexItem = (0 == i) ? (IElement)new Image(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg")) : 
                    (IElement)new Div().Add(new Paragraph(JavaUtil.IntegerToString(i)));
                if (0 == i) {
                    flexItem.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(50f));
                    div.Add((iText.Layout.Element.Image)flexItem);
                }
                else {
                    div.Add((IBlockElement)flexItem);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 0, 
                    0, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(50f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(30f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink1Item2MuchContentSetMinWidthGtBasisTest01() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(37.5f), UnitValue.CreatePointValue
                (60f), UnitValue.CreatePointValue(75f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(300).SetHeight(100);
            // We use Courier as a monotype font to ensure that min width calculated by iText
            // is more or less the same as the width calculated by browsers
            FontProvider provider = new FontProvider();
            provider.GetFontSet().AddFont(StandardFonts.COURIER, null, "courier");
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            documentRenderer.SetProperty(Property.FONT_PROVIDER, provider);
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph(1 == i ? "2222222222222222222222222" : JavaUtil.IntegerToString
                    (i)));
                if (1 == i) {
                    flexItem.SetFontFamily(StandardFontFamilies.COURIER).SetMinWidth(75f);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    1, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(80f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(102.5f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(117.5f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void Basis1Grow0Test01() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(1f), UnitValue.CreatePointValue
                (30f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(100).SetHeight(100);
            // We use Courier as a monotype font to ensure that min width calculated by iText
            // is more or less the same as the width calculated by browsers
            FontProvider provider = new FontProvider();
            provider.GetFontSet().AddFont(StandardFonts.COURIER, null, "courier");
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            documentRenderer.SetProperty(Property.FONT_PROVIDER, provider);
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph(JavaUtil.IntegerToString(i))).SetFontFamily(StandardFontFamilies
                    .COURIER);
                if (0 == i) {
                    flexItem.SetFontSize(100f);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 0, 
                    0, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(60f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(30f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink1Item2MuchContentSetMinWidthGtBasisTest02() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(37.5f), UnitValue.CreatePointValue
                (60f), UnitValue.CreatePointValue(75f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(300).SetHeight(100);
            // We use Courier as a monotype font to ensure that min width calculated by iText
            // is more or less the same as the width calculated by browsers
            FontProvider provider = new FontProvider();
            provider.GetFontSet().AddFont(StandardFonts.COURIER, null, "courier");
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            documentRenderer.SetProperty(Property.FONT_PROVIDER, provider);
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph(1 == i ? "2222222222222222222222222" : JavaUtil.IntegerToString
                    (i)));
                if (1 == i) {
                    flexItem.SetFontFamily(StandardFontFamilies.COURIER).SetMinWidth(150f);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    1, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(56.25f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(150f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(93.75f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink1Item2MuchContentSetMinWidthGtBasisTest03() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(112.5f), UnitValue.CreatePointValue
                (60f), UnitValue.CreatePointValue(187.5f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(300).SetHeight(100);
            // We use Courier as a monotype font to ensure that min width calculated by iText
            // is more or less the same as the width calculated by browsers
            FontProvider provider = new FontProvider();
            provider.GetFontSet().AddFont(StandardFonts.COURIER, null, "courier");
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            documentRenderer.SetProperty(Property.FONT_PROVIDER, provider);
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph(1 == i ? "2222222222222222222222222" : JavaUtil.IntegerToString
                    (i)));
                if (1 == i) {
                    flexItem.SetFontFamily(StandardFontFamilies.COURIER).SetMinWidth(150f);
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    1, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(56.25f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(150f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(93.75f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumEqWidthGrow1Shrink1Item2Basis0Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (400f), UnitValue.CreatePointValue(0f), UnitValue.CreatePointValue(100f)), JavaUtil.ArraysAsList(1f, 1f
                , 1f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            NUnit.Framework.Assert.AreEqual(1, rectangleTable[0].Count);
            NUnit.Framework.Assert.AreEqual(2, rectangleTable[1].Count);
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(400f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(150f, rectangleTable[1][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(250f, rectangleTable[1][1].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumEqWidthGrow1Shrink1Item2Basis0NoContentTest02() {
            Rectangle bBox = new Rectangle(575, 842);
            IList<UnitValue> flexBasisValues = JavaUtil.ArraysAsList(UnitValue.CreatePointValue(400f), UnitValue.CreatePointValue
                (0f), UnitValue.CreatePointValue(100f));
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div().SetWidth(400).SetHeight(100);
            div.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.WRAP);
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div();
                if (1 != i) {
                    flexItem.Add(new Paragraph(JavaUtil.IntegerToString(i)));
                }
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, flexBasisValues[i], 1, 
                    1, bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            IList<IList<FlexItemInfo>> rectangleTable = FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer
                )div.GetRenderer(), flexItemCalculationInfos);
            NUnit.Framework.Assert.AreEqual(2, rectangleTable[0].Count);
            NUnit.Framework.Assert.AreEqual(1, rectangleTable[1].Count);
            NUnit.Framework.Assert.AreEqual(400f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(0f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f, rectangleTable[1][0].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow0Shrink0Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(50f)
                , UnitValue.CreatePointValue(80f), UnitValue.CreatePointValue(100f)), JavaUtil.ArraysAsList(0f, 0f, 0f
                ), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(50f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(80f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(100f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow0Shrink0Item2Grow2Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(50f)
                , UnitValue.CreatePointValue(80f), UnitValue.CreatePointValue(100f)), JavaUtil.ArraysAsList(0f, 2f, 0f
                ), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(50f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(250f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(100f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumLtWidthGrow1Shrink0Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(50f)
                , UnitValue.CreatePointValue(80f), UnitValue.CreatePointValue(100f)), JavaUtil.ArraysAsList(1f, 1f, 1f
                ), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(106.66667f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(136.66667f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(156.66667f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow0Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(100f
                ), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(200f / 3, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f / 3, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(600f / 3, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow0Shrink05Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(100f
                ), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(0.5f, 0.5f, 0.5f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(200f / 3, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f / 3, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(600f / 3, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow0Shrink01Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(100f
                ), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(0.1f, 0.1f, 0.1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(90f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(180f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(270f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow0Shrink5Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(100f
                ), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(5f, 5f, 5f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(200f / 3, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f / 3, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(600f / 3, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow1Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(100f
                ), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(1f, 1f, 
                1f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(200f / 3, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f / 3, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(600f / 3, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow1Shrink1Item3Shrink50Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(100f
                ), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(1f, 1f, 
                1f), JavaUtil.ArraysAsList(1f, 1f, 50f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(98.69281f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(197.38562f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(103.92157f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow1Shrink1Item3Shrink5Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(100f
                ), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(1f, 1f, 
                1f), JavaUtil.ArraysAsList(1f, 1f, 5f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(88.888885f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(177.77777f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(133.33334f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow0Shrink0Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(100f
                ), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(100f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(200f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(300f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow1Shrink0Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePointValue(100f
                ), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(1f, 1f, 
                1f), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(100f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(200f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(300f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void Basis250SumGtWidthGrow0Shrink1WrapTest01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (250f), UnitValue.CreatePointValue(250f), UnitValue.CreatePointValue(250f)), JavaUtil.ArraysAsList(0f, 
                0f, 0f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(250f, flexItemInfo.GetRectangle().GetWidth(), EPS);
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow0Shrink1WrapTest01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (100f), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(0f, 
                0f, 0f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(100f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(200f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(300f, rectangleTable[1][0].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow0Shrink05WrapTest01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (100f), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(0f, 
                0f, 0f), JavaUtil.ArraysAsList(0.5f, 0.5f, 0.5f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(100f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(200f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(300f, rectangleTable[1][0].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow0Shrink01WrapTest01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (100f), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(0f, 
                0f, 0f), JavaUtil.ArraysAsList(0.1f, 0.1f, 0.1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(100f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(200f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(300f, rectangleTable[1][0].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow0Shrink5WrapTest01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (100f), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(0f, 
                0f, 0f), JavaUtil.ArraysAsList(5f, 5f, 5f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(100f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(200f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(300f, rectangleTable[1][0].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow1Shrink1WrapTest01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (100f), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(1f, 
                1f, 1f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(150f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(250f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f, rectangleTable[1][0].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow1Shrink1Item3Shrink50WrapTest01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (100f), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(1f, 
                1f, 1f), JavaUtil.ArraysAsList(1f, 1f, 50f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(150f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(250f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f, rectangleTable[1][0].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow1Shrink1Item3Shrink5WrapTest01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (100f), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(1f, 
                1f, 1f), JavaUtil.ArraysAsList(1f, 1f, 5f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(150f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(250f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f, rectangleTable[1][0].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow0Shrink0WrapTest01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (100f), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(0f, 
                0f, 0f), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(100f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(200f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(300f, rectangleTable[1][0].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisSumGtWidthGrow1Shrink0WrapTest01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(WRAP_STYLE, JavaUtil.ArraysAsList(UnitValue.CreatePointValue
                (100f), UnitValue.CreatePointValue(200f), UnitValue.CreatePointValue(300f)), JavaUtil.ArraysAsList(1f, 
                1f, 1f), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(150f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(250f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f, rectangleTable[1][0].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumLtWidthGrow0Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(10
                ), UnitValue.CreatePercentValue(20), UnitValue.CreatePercentValue(30)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(40f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(80f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(120f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumLtWidthGrow1Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(10
                ), UnitValue.CreatePercentValue(20), UnitValue.CreatePercentValue(30)), JavaUtil.ArraysAsList(1f, 1f, 
                1f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(93.333336f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(133.33333f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(173.33333f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumLtWidthGrow0Shrink0Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(10
                ), UnitValue.CreatePercentValue(20), UnitValue.CreatePercentValue(30)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(40f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(80f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(120f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumLtWidthGrow0Shrink0Item2Grow2Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(10
                ), UnitValue.CreatePercentValue(20), UnitValue.CreatePercentValue(30)), JavaUtil.ArraysAsList(0f, 2f, 
                0f), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(40f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(240f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(120f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumLtWidthGrow1Shrink0Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(10
                ), UnitValue.CreatePercentValue(20), UnitValue.CreatePercentValue(30)), JavaUtil.ArraysAsList(1f, 1f, 
                1f), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(93.333336f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(133.33333f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(173.33333f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumGtWidthGrow0Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(30
                ), UnitValue.CreatePercentValue(40), UnitValue.CreatePercentValue(50)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(300f / 3, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f / 3, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(500f / 3, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumGtWidthGrow0Shrink05Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(30
                ), UnitValue.CreatePercentValue(40), UnitValue.CreatePercentValue(50)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(0.5f, 0.5f, 0.5f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(100f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f / 3, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(500f / 3, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumGtWidthGrow0Shrink01Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(30
                ), UnitValue.CreatePercentValue(40), UnitValue.CreatePercentValue(50)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(0.1f, 0.1f, 0.1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(114f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(152f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(190f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumGtWidthGrow0Shrink5Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(30
                ), UnitValue.CreatePercentValue(40), UnitValue.CreatePercentValue(50)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(5f, 5f, 5f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(300f / 3, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f / 3, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(500f / 3, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumGtWidthGrow1Shrink1Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(30
                ), UnitValue.CreatePercentValue(40), UnitValue.CreatePercentValue(50)), JavaUtil.ArraysAsList(1f, 1f, 
                1f), JavaUtil.ArraysAsList(1f, 1f, 1f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(300f / 3, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(400f / 3, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(500f / 3, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumGtWidthGrow1Shrink1Item3Shrink50Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(30
                ), UnitValue.CreatePercentValue(40), UnitValue.CreatePercentValue(50)), JavaUtil.ArraysAsList(1f, 1f, 
                1f), JavaUtil.ArraysAsList(1f, 1f, 50f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(119.06615f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(158.75487f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(122.178986f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumGtWidthGrow1Shrink1Item3Shrink5Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(30
                ), UnitValue.CreatePercentValue(40), UnitValue.CreatePercentValue(50)), JavaUtil.ArraysAsList(1f, 1f, 
                1f), JavaUtil.ArraysAsList(1f, 1f, 5f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(112.5f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(150f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(137.5f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumGtWidthGrow0Shrink0Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(30
                ), UnitValue.CreatePercentValue(40), UnitValue.CreatePercentValue(50)), JavaUtil.ArraysAsList(0f, 0f, 
                0f), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(120f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(160f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(200f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentBasisPercentSumGtWidthGrow1Shrink0Test01() {
            IList<IList<FlexItemInfo>> rectangleTable = TestFlex(JavaUtil.ArraysAsList(UnitValue.CreatePercentValue(30
                ), UnitValue.CreatePercentValue(40), UnitValue.CreatePercentValue(50)), JavaUtil.ArraysAsList(1f, 1f, 
                1f), JavaUtil.ArraysAsList(0f, 0f, 0f));
            // after checks
            NUnit.Framework.Assert.IsFalse(rectangleTable.IsEmpty());
            foreach (IList<FlexItemInfo> line in rectangleTable) {
                foreach (FlexItemInfo flexItemInfo in line) {
                    NUnit.Framework.Assert.AreEqual(25.976562f, flexItemInfo.GetRectangle().GetHeight(), EPS);
                }
            }
            NUnit.Framework.Assert.AreEqual(120f, rectangleTable[0][0].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(160f, rectangleTable[0][1].GetRectangle().GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(200f, rectangleTable[0][2].GetRectangle().GetWidth(), EPS);
        }

        private static IList<IList<FlexItemInfo>> TestFlex(IList<UnitValue> flexBasisValues, IList<float> flexGrowValues
            , IList<float> flexShrinkValues) {
            return TestFlex(DEFAULT_STYLE, flexBasisValues, flexGrowValues, flexShrinkValues);
        }

        private static IList<IList<FlexItemInfo>> TestFlex(Style containerStyle, IList<UnitValue> flexBasisValues, 
            IList<float> flexGrowValues, IList<float> flexShrinkValues) {
            System.Diagnostics.Debug.Assert(flexBasisValues.Count == flexGrowValues.Count);
            System.Diagnostics.Debug.Assert(flexBasisValues.Count == flexShrinkValues.Count);
            Rectangle bBox = new Rectangle(PageSize.A4);
            bBox.ApplyMargins(36f, 36f, 36f, 36f, false);
            IList<FlexUtil.FlexItemCalculationInfo> flexItemCalculationInfos = new List<FlexUtil.FlexItemCalculationInfo
                >();
            Div div = new Div();
            div.AddStyle(containerStyle);
            DocumentRenderer documentRenderer = new DocumentRenderer(new Document(new PdfDocument(new PdfWriter(new MemoryStream
                ()))));
            for (int i = 0; i < flexBasisValues.Count; i++) {
                Div flexItem = new Div().Add(new Paragraph("x"));
                AbstractRenderer flexItemRenderer = (AbstractRenderer)flexItem.CreateRendererSubTree().SetParent(documentRenderer
                    );
                div.Add(flexItem);
                flexItemCalculationInfos.Add(new FlexUtil.FlexItemCalculationInfo(flexItemRenderer, null == flexBasisValues
                    [i] ? UnitValue.CreatePointValue(flexItemRenderer.GetMinMaxWidth().GetMinWidth()) : flexBasisValues[i]
                    , flexGrowValues[i], flexShrinkValues[i], bBox.GetWidth()));
            }
            FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
            div.SetNextRenderer(flexContainerRenderer);
            // before checks
            foreach (FlexUtil.FlexItemCalculationInfo info in flexItemCalculationInfos) {
                NUnit.Framework.Assert.IsNull(info.mainSize);
                NUnit.Framework.Assert.IsNull(info.crossSize);
            }
            return FlexUtil.CalculateChildrenRectangles(bBox, (FlexContainerRenderer)div.GetRenderer(), flexItemCalculationInfos
                );
        }
    }
}
