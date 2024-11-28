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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.StyledXmlParser.Node;
using iText.Svg;
using iText.Svg.Converter;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Factories;
using iText.Svg.Renderers.Impl;
using iText.Svg.Utils;
using iText.Test;

namespace iText.Svg.Customization {
    public class CustomizeTextLeafSvgNodeRendererTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/customization/CustomizeTextLeafSvgNodeRendererTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/customization/CustomizeTextLeafSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TestCustomizeTextLeafSvgNodeRenderer() {
            String pdfFilename = "customizeTextLeafSvgNodeRenderer.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + pdfFilename));
            doc.AddNewPage();
            SvgConverterProperties properties = new SvgConverterProperties();
            properties.SetRendererFactory(new CustomizeTextLeafSvgNodeRendererTest.CustomTextLeafOverridingSvgNodeRendererFactory
                ());
            String svg = "<svg viewBox=\"0 0 240 80\" xmlns=\"http://www.w3.org/2000/svg\">\n" + "  <text x=\"20\" y=\"35\" class=\"small\">Hello world</text>\n"
                 + "</svg>";
            PdfFormXObject form = SvgConverter.ConvertToXObject(svg, doc, properties);
            new PdfCanvas(doc.GetPage(1)).AddXObjectFittedIntoRectangle(form, new Rectangle(100, 100, 240, 80));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + pdfFilename, SOURCE_FOLDER
                 + "cmp_" + pdfFilename, DESTINATION_FOLDER, "diff_"));
        }

        private class CustomTextLeafOverridingSvgNodeRendererFactory : DefaultSvgNodeRendererFactory {
            public override ISvgNodeRenderer CreateSvgNodeRendererForTag(IElementNode tag, ISvgNodeRenderer parent) {
                if (SvgConstants.Tags.TEXT_LEAF.Equals(tag.Name())) {
                    return new CustomizeTextLeafSvgNodeRendererTest.CustomTextLeafSvgNodeRenderer();
                }
                else {
                    return base.CreateSvgNodeRendererForTag(tag, parent);
                }
            }
        }

        private class CustomTextLeafSvgNodeRenderer : TextLeafSvgNodeRenderer {
            public override ISvgNodeRenderer CreateDeepCopy() {
                CustomizeTextLeafSvgNodeRendererTest.CustomTextLeafSvgNodeRenderer copy = new CustomizeTextLeafSvgNodeRendererTest.CustomTextLeafSvgNodeRenderer
                    ();
                DeepCopyAttributesAndStyles(copy);
                return copy;
            }

            protected internal override void DoDraw(SvgDrawContext context) {
                if (this.attributesAndStyles != null && this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.TEXT_CONTENT
                    )) {
                    String initialText = this.attributesAndStyles.Get(SvgConstants.Attributes.TEXT_CONTENT);
                    String amendedText = "_" + initialText + "_";
                    SvgTextProperties properties = new SvgTextProperties(context.GetSvgTextProperties());
                    context.GetSvgTextProperties().SetFillColor(ColorConstants.RED);
                    base.DoDraw(context);
                    GetText().SetText(amendedText);
                    context.SetSvgTextProperties(properties);
                }
            }
        }
    }
}
