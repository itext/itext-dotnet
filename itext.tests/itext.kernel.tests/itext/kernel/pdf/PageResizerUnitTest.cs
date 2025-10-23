using System;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PageResizerUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ResizeAppearanceStreamsNullAPTest() {
            PdfAnnotation annotation = new PdfInkAnnotation(new Rectangle(50.0f, 50.0f));
            PageResizer.ResizeAppearanceStreams(annotation, null);
            NUnit.Framework.Assert.IsNull(annotation.GetAppearanceDictionary());
        }

        [NUnit.Framework.Test]
        public virtual void ScalePageBoxNullPageSizeTest() {
            Rectangle originalPageSize = null;
            PageSize newPageSize = new PageSize(25.0f, 25.0f);
            Rectangle box = new Rectangle(10.0f, 10.0f);
            Rectangle scaled = PageResizer.ScalePageBox(originalPageSize, newPageSize, box);
            NUnit.Framework.Assert.AreEqual(box, scaled);
        }

        [NUnit.Framework.Test]
        public virtual void ScalePageBoxNullNewPageSizeTest() {
            Rectangle originalPageSize = new Rectangle(50.0f, 50.0f);
            PageSize newPageSize = null;
            Rectangle box = new Rectangle(10.0f, 10.0f);
            Rectangle scaled = PageResizer.ScalePageBox(originalPageSize, newPageSize, box);
            NUnit.Framework.Assert.AreEqual(box, scaled);
        }

        [NUnit.Framework.Test]
        public virtual void ScalePageBoxNullBoxTest() {
            Rectangle originalPageSize = new Rectangle(50.0f, 50.0f);
            PageSize newPageSize = new PageSize(25.0f, 25.0f);
            Rectangle box = null;
            Rectangle scaled = PageResizer.ScalePageBox(originalPageSize, newPageSize, box);
            NUnit.Framework.Assert.AreEqual(box, scaled);
        }

        [NUnit.Framework.Test]
        public virtual void ScalePageBoxZeroHeightTest() {
            Rectangle originalPageSize = new Rectangle(50.0f, 0.0f);
            PageSize newPageSize = new PageSize(25.0f, 25.0f);
            Rectangle box = new Rectangle(10.0f, 10.0f);
            Rectangle scaled = PageResizer.ScalePageBox(originalPageSize, newPageSize, box);
            NUnit.Framework.Assert.AreEqual(box, scaled);
        }

        [NUnit.Framework.Test]
        public virtual void ScalePageBoxZeroWidthTest() {
            Rectangle originalPageSize = new Rectangle(0.0f, 50.0f);
            PageSize newPageSize = new PageSize(25.0f, 25.0f);
            Rectangle box = new Rectangle(10.0f, 10.0f);
            Rectangle scaled = PageResizer.ScalePageBox(originalPageSize, newPageSize, box);
            NUnit.Framework.Assert.AreEqual(box, scaled);
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringSimpleScaleTest() {
            String input = "/Helv 12 Tf";
            double scale = 0.5;
            String expected = "/Helv 6 Tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringMixedOperatorsAndColorTest() {
            String input = "1 0 0 rg /F1 10 Tf 14 TL";
            double scale = 2;
            String expected = "1 0 0 rg /F1 20 Tf 28 TL";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringEdgeNumericFormsTest() {
            NUnit.Framework.Assert.AreEqual("-1 Ts", PageResizer.ScaleDaString("-.5 Ts", 2.0));
            NUnit.Framework.Assert.AreEqual("1 Ts", PageResizer.ScaleDaString(".5 Ts", 2.0));
            NUnit.Framework.Assert.AreEqual("0.5 Tc", PageResizer.ScaleDaString("5.0000 Tc", 0.1));
            NUnit.Framework.Assert.AreEqual("1 TL", PageResizer.ScaleDaString("1e-1 TL", 10.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringMultipleOperatorGroupsTest() {
            String input = "/F1 10 Tf 5 Tc 2.5 Tw 10 TL /F2 20 Tf -2 Ts";
            double scale = 0.5;
            String expected = "/F1 5 Tf 2.5 Tc 1.25 Tw 5 TL /F2 10 Tf -1 Ts";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringNoOpsTest() {
            double scale = 2.0;
            // Operator with no operands should not change.
            NUnit.Framework.Assert.AreEqual("Tf", PageResizer.ScaleDaString("Tf", scale));
            //Operator with non-numeric operand should not change.
            NUnit.Framework.Assert.AreEqual("/F1 Tf", PageResizer.ScaleDaString("/F1 Tf", scale));
            //String with no operators should not change.
            NUnit.Framework.Assert.AreEqual("foo bar baz", PageResizer.ScaleDaString("foo bar baz", scale));
            //Malformed operator sequence should not change unpredictably.
            NUnit.Framework.Assert.AreEqual("/Helv Tf 12", PageResizer.ScaleDaString("/Helv Tf 12", scale));
            //Whitespace-only string should result in empty.
            NUnit.Framework.Assert.AreEqual("", PageResizer.ScaleDaString("", scale));
            //Numbers without operators should not be scaled.
            NUnit.Framework.Assert.AreEqual("1 2 3", PageResizer.ScaleDaString("1 2 3", scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringWhitespaceNormalizationTest() {
            String input = "  /Helv   12 \t Tf  ";
            double scale = 0.5;
            String expected = "/Helv 6 Tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale), "Whitespace should be normalized."
                );
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringWithIdentityScaleTest() {
            String input = "/Helv 12.5 Tf";
            double scale = 1.0;
            String expected = "/Helv 12.5 Tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringIgnoreOtherOperatorsTest() {
            String input = "100 Tz 12 Tf";
            double scale = 2.0;
            String expected = "100 Tz 24 Tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringNullInputTest() {
            NUnit.Framework.Assert.IsNull(PageResizer.ScaleDaString(null, 2.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringOperatorCaseSensitivityTest() {
            String input = "/Helv 12 tf";
            double scale = 2.0;
            String expected = "/Helv 12 tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringSmallResultingValueTest() {
            String input = "0.0001 Tf";
            double scale = 0.1;
            String expected = "0 Tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ResizePageWithZeroSizeTest() {
            PageResizer resizer = new PageResizer(new PageSize(0.0F, 0.0F), PageResizer.ResizeType.DEFAULT);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => resizer.Resize(null));
            String expectedMessage = MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_RESIZE_PAGE_WITH_NEGATIVE_OR_INFINITE_SCALE
                , new PageSize(0.0F, 0.0F));
            NUnit.Framework.Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}
