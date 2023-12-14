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
using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils {
    public class TaggedPdfReaderToolTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/utils/TaggedPdfReaderToolTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/utils/TaggedPdfReaderToolTest/";

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void TaggedPdfReaderToolTest01() {
            String filename = "iphone_user_guide.pdf";
            String outXmlPath = destinationFolder + "outXml01.xml";
            String cmpXmlPath = sourceFolder + "cmpXml01.xml";
            PdfReader reader = new PdfReader(sourceFolder + filename);
            using (FileStream outXml = new FileStream(outXmlPath, FileMode.Create)) {
                using (PdfDocument document = new PdfDocument(reader)) {
                    TaggedPdfReaderTool tool = new TaggedPdfReaderTool(document);
                    tool.SetRootTag("root");
                    tool.ConvertToXml(outXml);
                }
            }
            CompareTool compareTool = new CompareTool();
            if (!compareTool.CompareXmls(outXmlPath, cmpXmlPath)) {
                NUnit.Framework.Assert.Fail("Resultant xml is different.");
            }
        }
    }
}
