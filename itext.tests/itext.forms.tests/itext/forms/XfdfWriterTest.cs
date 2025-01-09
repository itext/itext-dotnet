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
using iText.Commons.Utils;
using iText.Forms.Exceptions;
using iText.Forms.Xfdf;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class XfdfWriterTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/XfdfWriterTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/XfdfWriterTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleFormWithOneFieldTest() {
            String pdfDocumentName = "simpleFormWithOneField.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + pdfDocumentName
                )));
            String xfdfFilename = destinationFolder + "simpleFormWithOneField.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "simpleFormWithOneField.xfdf", sourceFolder + "cmp_simpleFormWithOneField.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void SimpleFormWithMultipleFieldsTest() {
            String pdfDocumentName = "simpleFormWithMultipleFields.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + pdfDocumentName
                )));
            String xfdfFilename = destinationFolder + "simpleFormWithMultipleFields.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDoc, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDoc.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "simpleFormWithMultipleFields.xfdf", sourceFolder +
                 "cmp_simpleFormWithMultipleFields.xfdf")) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfValueRichText() {
            //TODO DEVSIX-3215
            String pdfDocumentName = "xfdfValueRichText.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + pdfDocumentName
                )));
            String xfdfFilename = destinationFolder + "xfdfValueRichText.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDoc, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDoc.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfValueRichText.xfdf", sourceFolder + "cmp_xfdfValueRichText.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfHierarchyFields() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfHierarchyFields.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfHierarchyFields.xfdf";
            String pdfDocumentName = "xfdfHierarchyFields.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfHierarchyFields.xfdf", sourceFolder + "cmp_xfdfHierarchyFields.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfFreeText() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfFreeText.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfFreeText.xfdf";
            String pdfDocumentName = "xfdfFreeText.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfFreeText.xfdf", sourceFolder + "cmp_xfdfFreeText.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfHighlightedText() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfHighlightedText.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfHighlightedText.xfdf";
            String pdfDocumentName = "xfdfHighlightedText.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfHighlightedText.xfdf", sourceFolder + "cmp_xfdfHighlightedText.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfUnderlineText() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfUnderlineText.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfUnderlineText.xfdf";
            String pdfDocumentName = "xfdfUnderlineText.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfUnderlineText.xfdf", sourceFolder + "cmp_xfdfUnderlineText.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfPopupNewFlags() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfPopupNewFlags.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfPopupNewFlags.xfdf";
            String pdfDocumentName = "xfdfPopupNewFlags.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfPopupNewFlags.xfdf", sourceFolder + "cmp_xfdfPopupNewFlags.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfStrikeout() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfStrikeout.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfStrikeout.xfdf";
            String pdfDocumentName = "xfdfStrikeout.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfStrikeout.xfdf", sourceFolder + "cmp_xfdfStrikeout.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfSquigglyText() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfSquigglyText.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfSquigglyText.xfdf";
            String pdfDocumentName = "xfdfSquigglyText.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfSquigglyText.xfdf", sourceFolder + "cmp_xfdfSquigglyText.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLine() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLine.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLine.xfdf";
            String pdfDocumentName = "xfdfLine.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLine.xfdf", sourceFolder + "cmp_xfdfLine.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfCircle() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfCircle.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfCircle.xfdf";
            String pdfDocumentName = "xfdfCircle.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfCircle.xfdf", sourceFolder + "cmp_xfdfCircle.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfSquare() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfSquare.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfSquare.xfdf";
            String pdfDocumentName = "xfdfSquare.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfSquare.xfdf", sourceFolder + "cmp_xfdfSquare.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfSquareAndCircleInteriorColor() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfSquareAndCircleInteriorColor.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfSquareAndCircleInteriorColor.xfdf";
            String pdfDocumentName = "xfdfSquareAndCircleInteriorColor.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfSquareAndCircleInteriorColor.xfdf", sourceFolder
                 + "cmp_xfdfSquareAndCircleInteriorColor.xfdf")) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfCaret() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfCaret.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfCaret.xfdf";
            String pdfDocumentName = "xfdfCaret.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfCaret.xfdf", sourceFolder + "cmp_xfdfCaret.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfPolygon() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfPolygon.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfPolygon.xfdf";
            String pdfDocumentName = "xfdfPolygon.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfPolygon.xfdf", sourceFolder + "cmp_xfdfPolygon.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfPolyline() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfPolyline.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfPolyline.xfdf";
            String pdfDocumentName = "xfdfPolyline.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfPolyline.xfdf", sourceFolder + "cmp_xfdfPolyline.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfStamp() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfStamp.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfStamp.xfdf";
            String pdfDocumentName = "xfdfStamp.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfStamp.xfdf", sourceFolder + "cmp_xfdfStamp.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfStampWithAppearance() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfStampWithAppearance.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfStampWithAppearance.xfdf";
            String pdfDocumentName = "xfdfStampWithAppearance.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfStampWithAppearance.xfdf", sourceFolder + "cmp_xfdfStampWithAppearance.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfInk() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfInk.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfInk.xfdf";
            String pdfDocumentName = "xfdfInk.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfInk.xfdf", sourceFolder + "cmp_xfdfInk.xfdf")) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfFileAttachment() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfFileAttachment.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfFileAttachment.xfdf";
            String pdfDocumentName = "xfdfFileAttachment.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfFileAttachment.xfdf", sourceFolder + "cmp_xfdfFileAttachment.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfSound() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfSound.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfSound.xfdf";
            String pdfDocumentName = "xfdfSound.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfSound.xfdf", sourceFolder + "cmp_xfdfSound.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLink() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLink.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLink.xfdf";
            String pdfDocumentName = "xfdfLink.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLink.xfdf", sourceFolder + "cmp_xfdfLink.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLinkBorderStyle() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLinkBorderStyle.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLinkBorderStyle.xfdf";
            String pdfDocumentName = "xfdfLinkBorderStyle.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLinkBorderStyle.xfdf", sourceFolder + "cmp_xfdfLinkBorderStyle.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLinkDest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLinkDest.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLinkDest.xfdf";
            String pdfDocumentName = "xfdfLinkDest.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLinkDest.xfdf", sourceFolder + "cmp_xfdfLinkDest.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLinkDestFit() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLinkDestFit.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLinkDestFit.xfdf";
            String pdfDocumentName = "xfdfLinkDestFit.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLinkDestFit.xfdf", sourceFolder + "cmp_xfdfLinkDestFit.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLinkDestFitB() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLinkDestFitB.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLinkDestFitB.xfdf";
            String pdfDocumentName = "xfdfLinkDestFitB.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLinkDestFitB.xfdf", sourceFolder + "cmp_xfdfLinkDestFitB.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLinkDestFitR() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLinkDestFitR.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLinkDestFitR.xfdf";
            String pdfDocumentName = "xfdfLinkDestFitR.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLinkDestFitR.xfdf", sourceFolder + "cmp_xfdfLinkDestFitR.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLinkDestFitH() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLinkDestFitH.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLinkDestFitH.xfdf";
            String pdfDocumentName = "xfdfLinkDestFitH.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLinkDestFitH.xfdf", sourceFolder + "cmp_xfdfLinkDestFitH.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLinkDestFitBH() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLinkDestFitBH.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLinkDestFitBH.xfdf";
            String pdfDocumentName = "xfdfLinkDestFitBH.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLinkDestFitBH.xfdf", sourceFolder + "cmp_xfdfLinkDestFitBH.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLinkDestFitBV() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLinkDestFitBV.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLinkDestFitBV.xfdf";
            String pdfDocumentName = "xfdfLinkDestFitBV.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLinkDestFitBV.xfdf", sourceFolder + "cmp_xfdfLinkDestFitBV.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLinkDestFitV() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLinkDestFitV.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLinkDestFitV.xfdf";
            String pdfDocumentName = "xfdfLinkDestFitV.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLinkDestFitV.xfdf", sourceFolder + "cmp_xfdfLinkDestFitV.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfRedact() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfRedact.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfRedact.xfdf";
            String pdfDocumentName = "xfdfRedact.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfRedact.xfdf", sourceFolder + "cmp_xfdfRedact.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfProjection() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfProjection.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfProjection.xfdf";
            String pdfDocumentName = "xfdfProjection.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfProjection.xfdf", sourceFolder + "cmp_xfdfProjection.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfLinkAllParams() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfLinkAllParams.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfLinkAllParams.xfdf";
            String pdfDocumentName = "xfdfLinkAllParams.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfLinkAllParams.xfdf", sourceFolder + "cmp_xfdfLinkAllParams.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfReplaceText() {
            //TODO DEVSIX-3215 Support caret annontation
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfReplaceText.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfReplaceText.xfdf";
            String pdfDocumentName = "xfdfReplaceText.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfReplaceText.xfdf", sourceFolder + "cmp_xfdfReplaceText.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfArrow() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfArrow.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfArrow.xfdf";
            String pdfDocumentName = "xfdfArrow.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfArrow.xfdf", sourceFolder + "cmp_xfdfArrow.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfCallout() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfCallout.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfCallout.xfdf";
            String pdfDocumentName = "xfdfCallout.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfCallout.xfdf", sourceFolder + "cmp_xfdfCallout.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfCloud() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfCloud.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfCloud.xfdf";
            String pdfDocumentName = "xfdfCloud.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfCloud.xfdf", sourceFolder + "cmp_xfdfCloud.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfCloudNested() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfCloudNested.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfCloudNested.xfdf";
            String pdfDocumentName = "xfdfCloudNested.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfCloudNested.xfdf", sourceFolder + "cmp_xfdfCloudNested.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfTextBoxAllParams() {
            //TODO DEVSIX-3215 Support richtext
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfTextBoxAllParams.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfTextBoxAllParams.xfdf";
            String pdfDocumentName = "xfdfTextBoxAllParams.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfTextBoxAllParams.xfdf", sourceFolder + "cmp_xfdfTextBoxAllParams.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfJavaScriptForms() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfJavaScriptForms.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfJavaScriptForms.xfdf";
            String pdfDocumentName = "xfdfJavaScriptForms.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfJavaScriptForms.xfdf", sourceFolder + "cmp_xfdfJavaScriptForms.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfAttrColor() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfAttrColor.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfAttrColor.xfdf";
            String pdfDocumentName = "xfdfAttrColor.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfAttrColor.xfdf", sourceFolder + "cmp_xfdfAttrColor.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfAttrFlagsOpacity() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfAttrFlagsOpacity.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfAttrFlagsOpacity.xfdf";
            String pdfDocumentName = "xfdfAttrFlagsOpacity.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfAttrFlagsOpacity.xfdf", sourceFolder + "cmp_xfdfAttrFlagsOpacity.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfAttrTitle() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfAttrTitle.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfAttrTitle.xfdf";
            String pdfDocumentName = "xfdfAttrTitle.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfAttrTitle.xfdf", sourceFolder + "cmp_xfdfAttrTitle.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfReferenceFor3DMeasurement() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfReferenceFor3DMeasurement.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfReferenceFor3DMeasurement.xfdf";
            String pdfDocumentName = "xfdfReferenceFor3DMeasurement.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfReferenceFor3DMeasurement.xfdf", sourceFolder 
                + "cmp_xfdfReferenceFor3DMeasurement.xfdf")) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfReferenceFor3DAngular() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfReferenceFor3DAngular.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfReferenceFor3DAngular.xfdf";
            String pdfDocumentName = "xfdfReferenceFor3DAngular.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfReferenceFor3DAngular.xfdf", sourceFolder + "cmp_xfdfReferenceFor3DAngular.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfReferenceFor3DRadial() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfReferenceFor3DRadial.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfReferenceFor3DRadial.xfdf";
            String pdfDocumentName = "xfdfReferenceFor3DRadial.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfReferenceFor3DRadial.xfdf", sourceFolder + "cmp_xfdfReferenceFor3DRadial.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfSubelementContents() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfSubelementContents.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfSubelementContents.xfdf";
            String pdfDocumentName = "xfdfSubelementContents.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfSubelementContents.xfdf", sourceFolder + "cmp_xfdfSubelementContents.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfSubelementOverlayAppearance() {
            //check when Redact annotation is implemented
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfSubelementOverlayAppearance.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfSubelementOverlayAppearance.xfdf";
            String pdfDocumentName = "xfdfSubelementOverlayAppearance.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfSubelementOverlayAppearance.xfdf", sourceFolder
                 + "cmp_xfdfSubelementOverlayAppearance.xfdf")) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfButton() {
            //Widget annotation is not supported in xfdf 2014 spec version
            //TODO  DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfButton.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfButton.xfdf";
            String pdfDocumentName = "xfdfButton.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfButton.xfdf", sourceFolder + "cmp_xfdfButton.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfCheckBox() {
            //Widget annotation is not supported in xfdf 2014 spec version
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfCheckBox.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfCheckBox.xfdf";
            String pdfDocumentName = "xfdfCheckBox.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfCheckBox.xfdf", sourceFolder + "cmp_xfdfCheckBox.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfList() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfList.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfList.xfdf";
            String pdfDocumentName = "xfdfList.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfList.xfdf", sourceFolder + "cmp_xfdfList.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfDropDown() {
            //Widget annotation is not supported in 2014 spec version
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder + "xfdfDropDown.pdf"
                )));
            String xfdfFilename = destinationFolder + "xfdfDropDown.xfdf";
            String pdfDocumentName = "xfdfDropDown.pdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
            xfdfObject.WriteToFile(xfdfFilename);
            pdfDocument.Close();
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfDropDown.xfdf", sourceFolder + "cmp_xfdfDropDown.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfEmptyAttributeTest() {
            XfdfObject xfdfObject = new XfdfObject();
            AnnotsObject annots = new AnnotsObject();
            xfdfObject.SetAnnots(annots);
            AnnotObject annot = new AnnotObject();
            annots.AddAnnot(annot);
            String namePresent = "name1";
            String nameAbsent = null;
            String valuePresent = "value";
            String valueAbsent = null;
            Exception e = NUnit.Framework.Assert.Catch(typeof(XfdfException), () => annot.AddAttribute(new AttributeObject
                (nameAbsent, valuePresent)));
            NUnit.Framework.Assert.AreEqual(XfdfException.ATTRIBUTE_NAME_OR_VALUE_MISSING, e.Message);
            Exception e2 = NUnit.Framework.Assert.Catch(typeof(XfdfException), () => annot.AddAttribute(new AttributeObject
                (namePresent, valueAbsent)));
            NUnit.Framework.Assert.AreEqual(XfdfException.ATTRIBUTE_NAME_OR_VALUE_MISSING, e2.Message);
        }

        [NUnit.Framework.Test]
        public virtual void XfdfAnnotationAttributesTest() {
            //TODO DEVSIX-7600 update xfdf and src files after supporting all the annotation types mentioned in xfdf spec
            String pdfDocumentName = "xfdfAnnotationAttributes.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder
                 + pdfDocumentName)))) {
                String xfdfFilename = destinationFolder + "xfdfAnnotationAttributes.xfdf";
                XfdfObjectFactory factory = new XfdfObjectFactory();
                XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
                xfdfObject.WriteToFile(xfdfFilename);
            }
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfAnnotationAttributes.xfdf", sourceFolder + "xfdfAnnotationAttributes.xfdf"
                )) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfdfOnlyRequiredAnnotationAttributesTest() {
            //TODO DEVSIX-7600 update xfdf and src files after supporting all the annotation types mentioned in xfdf spec
            String pdfDocumentName = "xfdfOnlyRequiredAnnotationAttributes.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(FileUtil.GetInputStreamForFile(sourceFolder
                 + pdfDocumentName)))) {
                String xfdfFilename = destinationFolder + "xfdfOnlyRequiredAnnotationAttributes.xfdf";
                XfdfObjectFactory factory = new XfdfObjectFactory();
                XfdfObject xfdfObject = factory.CreateXfdfObject(pdfDocument, pdfDocumentName);
                xfdfObject.WriteToFile(xfdfFilename);
            }
            if (!new CompareTool().CompareXmls(destinationFolder + "xfdfOnlyRequiredAnnotationAttributes.xfdf", sourceFolder
                 + "xfdfOnlyRequiredAnnotationAttributes.xfdf")) {
                NUnit.Framework.Assert.Fail("Xfdf files are not equal");
            }
        }
    }
}
