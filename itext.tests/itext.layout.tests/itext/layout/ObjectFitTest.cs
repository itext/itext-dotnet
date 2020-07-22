using System;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class ObjectFitTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ObjectFitTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ObjectFitTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FillObjectFitTest() {
            String outFileName = destinationFolder + "objectFit_test_fill.pdf";
            String cmpFileName = sourceFolder + "cmp_objectFit_test_fill.pdf";
            GenerateDocumentWithObjectFit(ObjectFit.FILL, outFileName);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CoverObjectFitTest() {
            String outFileName = destinationFolder + "objectFit_test_cover.pdf";
            String cmpFileName = sourceFolder + "cmp_objectFit_test_cover.pdf";
            GenerateDocumentWithObjectFit(ObjectFit.COVER, outFileName);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ContainObjectFitTest() {
            String outFileName = destinationFolder + "objectFit_test_contain.pdf";
            String cmpFileName = sourceFolder + "cmp_objectFit_test_contain.pdf";
            GenerateDocumentWithObjectFit(ObjectFit.CONTAIN, outFileName);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDownObjectFitTest() {
            String outFileName = destinationFolder + "objectFit_test_scale_down.pdf";
            String cmpFileName = sourceFolder + "cmp_objectFit_test_scale_down.pdf";
            GenerateDocumentWithObjectFit(ObjectFit.SCALE_DOWN, outFileName);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void NoneObjectFitTest() {
            String outFileName = destinationFolder + "objectFit_test_none.pdf";
            String cmpFileName = sourceFolder + "cmp_objectFit_test_none.pdf";
            GenerateDocumentWithObjectFit(ObjectFit.NONE, outFileName);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDownSmallImageObjectFitTest() {
            String outFileName = destinationFolder + "objectFit_test_scale_down_small_image.pdf";
            String cmpFileName = sourceFolder + "cmp_objectFit_test_scale_down_small_image.pdf";
            using (PdfWriter writer = new PdfWriter(outFileName)) {
                using (Document doc = new Document(new PdfDocument(writer))) {
                    PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "itis.jpg"));
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject).SetWidth(200).SetHeight(600).SetObjectFit
                        (ObjectFit.SCALE_DOWN);
                    Paragraph p = new Paragraph();
                    p.Add(image);
                    doc.Add(p);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TwoCoverObjectsFitTest() {
            String outFileName = destinationFolder + "objectFit_test_two_objects.pdf";
            String cmpFileName = sourceFolder + "cmp_objectFit_test_two_objects.pdf";
            using (PdfWriter writer = new PdfWriter(outFileName)) {
                using (Document doc = new Document(new PdfDocument(writer))) {
                    PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject).SetWidth(200).SetHeight(600).SetObjectFit
                        (ObjectFit.COVER);
                    iText.Layout.Element.Image image2 = new iText.Layout.Element.Image(xObject).SetWidth(200).SetHeight(600).SetObjectFit
                        (ObjectFit.CONTAIN);
                    Paragraph p = new Paragraph();
                    p.Add(image);
                    p.Add(image2);
                    doc.Add(p);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ContainWithEffectsObjectsFitTest() {
            String outFileName = destinationFolder + "objectFit_test_with_effects.pdf";
            String cmpFileName = sourceFolder + "cmp_objectFit_test_with_effects.pdf";
            using (PdfWriter writer = new PdfWriter(outFileName)) {
                using (Document doc = new Document(new PdfDocument(writer))) {
                    PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject).SetWidth(200).SetHeight(600).SetBorder
                        (new SolidBorder(new DeviceGray(0), 5)).SetBorderRadius(new BorderRadius(100)).SetObjectFit(ObjectFit.
                        CONTAIN);
                    Paragraph p = new Paragraph();
                    p.Add(image);
                    doc.Add(p);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ContainWithRotationObjectsFitTest() {
            // TODO DEVSIX-4286 object-fit property combined with rotation is not processed correctly
            String outFileName = destinationFolder + "objectFit_test_with_rotation.pdf";
            String cmpFileName = sourceFolder + "cmp_objectFit_test_with_rotation.pdf";
            using (PdfWriter writer = new PdfWriter(outFileName)) {
                using (Document doc = new Document(new PdfDocument(writer))) {
                    PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject).SetWidth(200).SetHeight(600).SetRotationAngle
                        (45).SetBorder(new SolidBorder(new DeviceGray(0), 1)).SetObjectFit(ObjectFit.CONTAIN);
                    Paragraph p = new Paragraph();
                    p.Add(image);
                    doc.Add(p);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private void GenerateDocumentWithObjectFit(ObjectFit objectFit, String outFileName) {
            using (PdfWriter writer = new PdfWriter(outFileName)) {
                using (Document doc = new Document(new PdfDocument(writer))) {
                    PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject).SetWidth(200).SetHeight(600).SetObjectFit
                        (objectFit);
                    Paragraph p = new Paragraph();
                    p.Add(image);
                    doc.Add(p);
                }
            }
        }
    }
}
