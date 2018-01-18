/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfNameTreeTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfNameTreeTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfNameTreeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void EmbeddedFileAndJavascriptTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "FileWithSingleAttachment.pdf"));
            PdfNameTree embeddedFilesNameTree = pdfDocument.GetCatalog().GetNameTree(PdfName.EmbeddedFiles);
            IDictionary<String, PdfObject> objs = embeddedFilesNameTree.GetNames();
            PdfNameTree javascript = pdfDocument.GetCatalog().GetNameTree(PdfName.JavaScript);
            IDictionary<String, PdfObject> objs2 = javascript.GetNames();
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(1, objs.Count);
            NUnit.Framework.Assert.AreEqual(1, objs2.Count);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationAppearanceTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "AnnotationAppearanceTest.pdf"
                ));
            PdfPage page = pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.MAGENTA).BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts
                .TIMES_ROMAN), 30).SetTextMatrix(25, 500).ShowText("This file has AP key in Names dictionary").EndText
                ();
            PdfArray array = new PdfArray();
            array.Add(new PdfString("normalAppearance"));
            array.Add(new PdfAnnotationAppearance().SetState(PdfName.N, new PdfFormXObject(new Rectangle(50, 50, 50, 50
                ))).GetPdfObject());
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Names, array);
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.AP, dict);
            pdfDocument.GetCatalog().GetPdfObject().Put(PdfName.Names, dictionary);
            PdfNameTree appearance = pdfDocument.GetCatalog().GetNameTree(PdfName.AP);
            IDictionary<String, PdfObject> objs = appearance.GetNames();
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(1, objs.Count);
        }
    }
}
