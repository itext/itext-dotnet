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
using System;
using System.Collections.Generic;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties.Margins;
using iText.Layout.Tagging;
using iText.Layout.Testutil;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PageMarginBoxTagRoleOverrideTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/PageMarginBoxTagRoleOverrideTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/PageMarginBoxTagRoleOverrideTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void RelativePositionWithPageMarginTagRoleOverrideGoldenTest() {
            String fileName = "relativePositionWithPageMarginTagRoleOverride";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza());
                    SectionBreak sectionBreak = new SectionBreak(new PageMarginBoxTagRoleOverrideTest.ParagraphRolePageMarginBoxes
                        (PageMarginsTestUtil.GetPageMargins1()));
                    Div div1 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    div1.SetRelativePosition(50, 50, 0, 0);
                    Div div2 = new Div().Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    document.Add(div1).Add(sectionBreak).Add(div2);
                }
            }
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareTagStructures(outFileName, cmpFileName));
            NUnit.Framework.Assert.IsNull(ct.CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER, "diff_" + 
                fileName));
        }

        private class ParagraphRolePageMarginBoxes : PageMarginBoxes {
//\cond DO_NOT_DOCUMENT
            internal ParagraphRolePageMarginBoxes(IList<PageMarginContent> elements)
                : base(elements) {
            }
//\endcond

            protected internal override void SetPageMarginTagRole(IElement element) {
                if (element is IAccessibleElement) {
                    ((IAccessibleElement)element).GetAccessibilityProperties().SetRole(StandardRoles.CAPTION);
                }
            }
        }
    }
}
