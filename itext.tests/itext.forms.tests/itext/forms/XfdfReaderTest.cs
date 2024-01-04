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
using System.IO;
using iText.Forms.Xfdf;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class XfdfReaderTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/XfdfReaderTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/XfdfReaderTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfNoFields() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfNoFields.pdf", 
                FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfNoFields.pdf", 
                FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfNoFields.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfNoFields.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfNoFields.pdf", sourceFolder
                 + "cmp_xfdfNoFields.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_NO_F_OBJECT_TO_COMPARE)]
        public virtual void XfdfNoFieldsNoFAttributes() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfNoFieldsNoFAttributes.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfNoFieldsNoFAttributes.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfNoFieldsNoFAttributes.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfNoFieldsNoFAttributes.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfNoFieldsNoFAttributes.pdf"
                , sourceFolder + "cmp_xfdfNoFieldsNoFAttributes.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfNoFieldsNoIdsAttributes() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfNoFieldsNoIdsAttributes.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfNoFieldsNoIdsAttributes.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfNoFieldsNoIdsAttributes.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfNoFieldsNoIdsAttributes.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfNoFieldsNoIdsAttributes.pdf"
                , sourceFolder + "cmp_xfdfNoFieldsNoIdsAttributes.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfWithFieldsWithValue() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfWithFieldsWithValue.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfWithFieldsWithValue.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfWithFieldsWithValue.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfWithFieldsWithValue.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfWithFieldsWithValue.pdf"
                , sourceFolder + "cmp_xfdfWithFieldsWithValue.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_NO_SUCH_FIELD_IN_PDF_DOCUMENT)]
        public virtual void XfdfValueRichText() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfValueRichText.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfValueRichText.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfValueRichText.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfValueRichText.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfValueRichText.pdf"
                , sourceFolder + "cmp_xfdfValueRichText.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_NO_SUCH_FIELD_IN_PDF_DOCUMENT, Count = 3)]
        public virtual void XfdfHierarchyFieldsTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "hierarchy_fields.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "hierarchy_fields.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "hierarchy_fields.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "hierarchy_fields.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "hierarchy_fields.pdf"
                , sourceFolder + "cmp_hierarchy_fields.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_NO_SUCH_FIELD_IN_PDF_DOCUMENT, Count = 3)]
        public virtual void XfdfWithFieldsWithValueParentAndChild() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfWithFieldsWithValueParentAndChild.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfWithFieldsWithValueParentAndChild.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfWithFieldsWithValueParentAndChild.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfWithFieldsWithValueParentAndChild.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfWithFieldsWithValueParentAndChild.pdf"
                , sourceFolder + "cmp_xfdfWithFieldsWithValueParentAndChild.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void XfdfAnnotationHighlightedText() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationHighlightedText.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationHighlightedText.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationHighlightedText.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, "xfdfAnnotationHighlightedText.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationHighlightedText.pdf"
                , sourceFolder + "cmp_xfdfAnnotationHighlightedText.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationUnderlineText() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationUnderlineText.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationUnderlineText.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationUnderlineText.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationUnderlineText.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationUnderlineText.pdf"
                , sourceFolder + "cmp_xfdfAnnotationUnderlineText.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationUnderlineTextRectWithTwoCoords() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationUnderlineTextRectWithTwoCoords.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationUnderlineTextRectWithTwoCoords.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationUnderlineTextRectWithTwoCoords.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationUnderlineTextRectWithTwoCoords.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationUnderlineTextRectWithTwoCoords.pdf"
                , sourceFolder + "cmp_xfdfAnnotationUnderlineTextRectWithTwoCoords.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationUnderlinePopupAllFlags() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationUnderlinePopupAllFlags.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationUnderlinePopupAllFlags.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationUnderlinePopupAllFlags.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationUnderlinePopupAllFlags.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationUnderlinePopupAllFlags.pdf"
                , sourceFolder + "cmp_xfdfAnnotationUnderlinePopupAllFlags.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationText() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationText.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationText.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationText.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationText.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationText.pdf"
                , sourceFolder + "cmp_xfdfAnnotationText.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationStrikeout() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationStrikeout.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationStrikeout.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationStrikeout.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationStrikeout.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationStrikeout.pdf"
                , sourceFolder + "cmp_xfdfAnnotationStrikeout.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationSquigglyText() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationSquigglyText.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationSquigglyText.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationSquigglyText.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationSquigglyText.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationSquigglyText.pdf"
                , sourceFolder + "cmp_xfdfAnnotationSquigglyText.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_UNSUPPORTED_ANNOTATION_ATTRIBUTE, Count = 2)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_ANNOTATION_IS_NOT_SUPPORTED)]
        public virtual void XfdfAnnotationLine() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationLine.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationLine.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationLine.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationLine.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationLine.pdf"
                , sourceFolder + "cmp_xfdfAnnotationLine.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationCircle() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationCircle.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationCircle.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationCircle.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationCircle.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationCircle.pdf"
                , sourceFolder + "cmp_xfdfAnnotationCircle.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationSquare() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationSquare.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationSquare.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationSquare.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationSquare.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationSquare.pdf"
                , sourceFolder + "cmp_xfdfAnnotationSquare.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationCaret() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationCaret.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationCaret.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationCaret.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationCaret.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationCaret.pdf"
                , sourceFolder + "cmp_xfdfAnnotationCaret.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationPolygon() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationPolygon.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationPolygon.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationPolygon.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationPolygon.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationPolygon.pdf"
                , sourceFolder + "cmp_xfdfAnnotationPolygon.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationPolyline() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationPolyline.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationPolyline.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationPolyline.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationPolyline.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationPolyline.pdf"
                , sourceFolder + "cmp_xfdfAnnotationPolyline.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationStamp() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationStamp.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationStamp.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationStamp.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationStamp.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationStamp.pdf"
                , sourceFolder + "cmp_xfdfAnnotationStamp.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationInk() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationInk.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationInk.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationInk.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationInk.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationInk.pdf"
                , sourceFolder + "cmp_xfdfAnnotationInk.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationFreeText() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationFreeText.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationFreeText.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationFreeText.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationFreeText.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationFreeText.pdf"
                , sourceFolder + "cmp_xfdfAnnotationFreeText.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationFileAttachment() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationFileAttachment.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationFileAttachment.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationFileAttachment.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationFileAttachment.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationFileAttachment.pdf"
                , sourceFolder + "cmp_xfdfAnnotationFileAttachment.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationSound() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationSound.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationSound.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationSound.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationSound.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationSound.pdf"
                , sourceFolder + "cmp_xfdfAnnotationSound.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationLink() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationLink.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationLink.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationLink.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationLink.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationLink.pdf"
                , sourceFolder + "cmp_xfdfAnnotationLink.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationRedact() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationRedact.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationRedact.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationRedact.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationRedact.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationRedact.pdf"
                , sourceFolder + "cmp_xfdfAnnotationRedact.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationProjection() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationProjection.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationProjection.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationProjection.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationProjection.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationProjection.pdf"
                , sourceFolder + "cmp_xfdfAnnotationProjection.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationLinkAllParams() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationLinkAllParams.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationLinkAllParams.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationLinkAllParams.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationLinkAllParams.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationLinkAllParams.pdf"
                , sourceFolder + "cmp_xfdfAnnotationLinkAllParams.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationReplaceText() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationReplaceText.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationReplaceText.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationReplaceText.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationReplaceText.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationReplaceText.pdf"
                , sourceFolder + "cmp_xfdfAnnotationReplaceText.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_UNSUPPORTED_ANNOTATION_ATTRIBUTE, Count = 2)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_ANNOTATION_IS_NOT_SUPPORTED)]
        public virtual void XfdfAnnotationArrow() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationArrow.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationArrow.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationArrow.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationArrow.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationArrow.pdf"
                , sourceFolder + "cmp_xfdfAnnotationArrow.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationCallout() {
            //TODO DEVSIX-7600 Support callout annotations
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationCallout.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationCallout.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationCallout.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationCallout.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationCallout.pdf"
                , sourceFolder + "cmp_xfdfAnnotationCallout.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationCloud() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationCloud.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationCloud.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationCloud.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationCloud.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationCloud.pdf"
                , sourceFolder + "cmp_xfdfAnnotationCloud.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationCloudNested() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationCloudNested.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationCloudNested.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationCloudNested.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationCloudNested.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationCloudNested.pdf"
                , sourceFolder + "cmp_xfdfAnnotationCloudNested.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationTextBoxAllParams() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationTextBoxAllParams.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationTextBoxAllParams.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationTextBoxAllParams.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationTextBoxAllParams.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationTextBoxAllParams.pdf"
                , sourceFolder + "cmp_xfdfAnnotationTextBoxAllParams.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfJavaScriptForms() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfJavaScriptForms.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfJavaScriptForms.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfJavaScriptForms.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfJavaScriptForms.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfJavaScriptForms.pdf"
                , sourceFolder + "cmp_xfdfJavaScriptForms.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfFormsFieldParams() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfFormsFieldParams.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfFormsFieldParams.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfFormsFieldParams.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfFormsFieldParams.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfFormsFieldParams.pdf"
                , sourceFolder + "cmp_xfdfFormsFieldParams.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationAttrColor() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationAttrColor.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationAttrColor.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationAttrColor.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationAttrColor.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationAttrColor.pdf"
                , sourceFolder + "cmp_xfdfAnnotationAttrColor.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationAttrFlagsOpacity() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationAttrFlagsOpacity.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationAttrFlagsOpacity.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationAttrFlagsOpacity.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationAttrFlagsOpacity.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationAttrFlagsOpacity.pdf"
                , sourceFolder + "cmp_xfdfAnnotationAttrFlagsOpacity.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfAnnotationAttrTitle() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfAnnotationAttrTitle.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationAttrTitle.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfAnnotationAttrTitle.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfAnnotationAttrTitle.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationAttrTitle.pdf"
                , sourceFolder + "cmp_xfdfAnnotationAttrTitle.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfReferenceFor3DMeasurement() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfReferenceFor3DMeasurement.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfReferenceFor3DMeasurement.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfReferenceFor3DMeasurement.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfReferenceFor3DMeasurement.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfReferenceFor3DMeasurement.pdf"
                , sourceFolder + "cmp_xfdfReferenceFor3DMeasurement.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfReferenceFor3DAngular() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfReferenceFor3DAngular.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfReferenceFor3DAngular.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfReferenceFor3DAngular.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfReferenceFor3DAngular.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfReferenceFor3DAngular.pdf"
                , sourceFolder + "cmp_xfdfReferenceFor3DAngular.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfReferenceFor3DRadial() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfReferenceFor3DRadial.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfReferenceFor3DRadial.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfReferenceFor3DRadial.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfReferenceFor3DRadial.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfReferenceFor3DRadial.pdf"
                , sourceFolder + "cmp_xfdfReferenceFor3DRadial.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfSubelementContents() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfSubelementContents.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfSubelementContents.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfSubelementContents.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfSubelementContents.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfSubelementContents.pdf"
                , sourceFolder + "cmp_xfdfSubelementContents.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfSubelementOverlayAppearance() {
            //TODO DEVSIX-3215 Support annots
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfSubelementOverlayAppearance.pdf"
                , FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfSubelementOverlayAppearance.pdf"
                , FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfSubelementOverlayAppearance.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfSubelementOverlayAppearance.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfSubelementOverlayAppearance.pdf"
                , sourceFolder + "cmp_xfdfSubelementOverlayAppearance.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfButton() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfButton.pdf", FileMode.Open
                , FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfButton.pdf", FileMode.Create
                )));
            String xfdfFilename = sourceFolder + "xfdfButton.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfButton.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfButton.pdf", sourceFolder
                 + "cmp_xfdfButton.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfCheckBox() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfCheckBox.pdf", 
                FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfCheckBox.pdf", 
                FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfCheckBox.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfCheckBox.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfCheckBox.pdf", sourceFolder
                 + "cmp_xfdfCheckBox.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfList() {
            //TODO DEVSIX-3215
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfList.pdf", FileMode.Open
                , FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfList.pdf", FileMode.Create)
                ));
            String xfdfFilename = sourceFolder + "xfdfList.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfList.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfList.pdf", sourceFolder
                 + "cmp_xfdfList.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT)]
        public virtual void XfdfDropDown() {
            //TODO DEVSIX-3215 Support richtext
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder + "xfdfDropDown.pdf", 
                FileMode.Open, FileAccess.Read)), new PdfWriter(new FileStream(destinationFolder + "xfdfDropDown.pdf", 
                FileMode.Create)));
            String xfdfFilename = sourceFolder + "xfdfDropDown.xfdf";
            XfdfObjectFactory factory = new XfdfObjectFactory();
            XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                ));
            xfdfObject.MergeToPdf(pdfDocument, sourceFolder + "xfdfDropDown.pdf");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfDropDown.pdf", sourceFolder
                 + "cmp_xfdfDropDown.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_NO_F_OBJECT_TO_COMPARE)]
        public virtual void XfdfBorderStyleAttributesTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "xfdfAnnotationsTemplate.pdf"), 
                new PdfWriter(new FileStream(destinationFolder + "xfdfBorderStyleAttributes.pdf", FileMode.Create)))) {
                String xfdfFilename = sourceFolder + "xfdfBorderStyleAttributes.xfdf";
                XfdfObjectFactory factory = new XfdfObjectFactory();
                XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                    ));
                xfdfObject.MergeToPdf(document, sourceFolder + "xfdfAnnotationsTemplate.pdf");
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfBorderStyleAttributes.pdf"
                , sourceFolder + "cmp_xfdfBorderStyleAttributes.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_NO_F_OBJECT_TO_COMPARE)]
        public virtual void XfdfAnnotationAttributesTest() {
            //TODO DEVSIX-7600 update xfdf and cmp files after supporting all the annotation types mentioned in xfdf spec
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "xfdfAnnotationsTemplate.pdf"), 
                new PdfWriter(new FileStream(destinationFolder + "xfdfAnnotationAttributes.pdf", FileMode.Create)))) {
                String xfdfFilename = sourceFolder + "xfdfAnnotationAttributes.xfdf";
                XfdfObjectFactory factory = new XfdfObjectFactory();
                XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                    ));
                xfdfObject.MergeToPdf(document, sourceFolder + "xfdfAnnotationsTemplate.pdf");
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfAnnotationAttributes.pdf"
                , sourceFolder + "cmp_xfdfAnnotationAttributes.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_NO_F_OBJECT_TO_COMPARE)]
        public virtual void XfdfOnlyRequiredAnnotationAttributesTest() {
            //TODO DEVSIX-7600 update xfdf and src files after supporting all the annotation types mentioned in xfdf spec
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "xfdfAnnotationsTemplate.pdf"), 
                new PdfWriter(new FileStream(destinationFolder + "xfdfOnlyRequiredAnnotationAttributes.pdf", FileMode.Create
                )))) {
                String xfdfFilename = sourceFolder + "xfdfOnlyRequiredAnnotationAttributes.xfdf";
                XfdfObjectFactory factory = new XfdfObjectFactory();
                XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                    ));
                xfdfObject.MergeToPdf(document, sourceFolder + "xfdfAnnotationsTemplate.pdf");
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfOnlyRequiredAnnotationAttributes.pdf"
                , sourceFolder + "cmp_xfdfOnlyRequiredAnnotationAttributes.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XFDF_NO_F_OBJECT_TO_COMPARE)]
        public virtual void XfdfInReplyToTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "xfdfAnnotationHighlightedText.pdf"
                ), new PdfWriter(new FileStream(destinationFolder + "xfdfInReplyTo.pdf", FileMode.Create)))) {
                String xfdfFilename = sourceFolder + "xfdfInReplyTo.xfdf";
                XfdfObjectFactory factory = new XfdfObjectFactory();
                XfdfObject xfdfObject = factory.CreateXfdfObject(new FileStream(xfdfFilename, FileMode.Open, FileAccess.Read
                    ));
                xfdfObject.MergeToPdf(document, sourceFolder + "xfdfAnnotationHighlightedText.pdf");
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "xfdfInReplyTo.pdf", 
                sourceFolder + "cmp_xfdfInReplyTo.pdf", destinationFolder, "diff_"));
        }
    }
}
