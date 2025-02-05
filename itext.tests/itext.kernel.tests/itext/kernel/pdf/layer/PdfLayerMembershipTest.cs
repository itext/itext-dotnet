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
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Layer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfLayerMembershipTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/layer/PdfLayerMembershipTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/layer/PdfLayerMembershipTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void EnabledVisibilityPolicyAllOnTest() {
            String srcPdf = "sourceWithDifferentLayers.pdf";
            String destPdf = "enabledVisibilityPolicyAllOnTest.pdf";
            String cmpPdf = "cmp_" + destPdf;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), CompareTool.CreateTestPdfWriter
                (destinationFolder + destPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            PdfLayerMembership layerMembershipAllOn = new PdfLayerMembership(pdfDoc);
            layerMembershipAllOn.AddLayer(allLayers[1]);
            layerMembershipAllOn.AddLayer(allLayers[2]);
            layerMembershipAllOn.SetVisibilityPolicy(PdfName.AllOn);
            PdfLayerTestUtils.AddTextInsideLayer(layerMembershipAllOn, canvas, "visibilityPolicyAllOnTest", 200, 500);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destPdf, sourceFolder
                 + cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void DisabledVisibilityPolicyAllOnTest() {
            String srcPdf = "sourceWithDifferentLayers.pdf";
            String destPdf = "disabledVisibilityPolicyAllOnTest.pdf";
            String cmpPdf = "cmp_" + destPdf;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), CompareTool.CreateTestPdfWriter
                (destinationFolder + destPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            PdfLayerMembership layerMembershipAllOn = new PdfLayerMembership(pdfDoc);
            layerMembershipAllOn.AddLayer(allLayers[1]);
            layerMembershipAllOn.AddLayer(allLayers[0]);
            layerMembershipAllOn.SetVisibilityPolicy(PdfName.AllOn);
            PdfLayerTestUtils.AddTextInsideLayer(layerMembershipAllOn, canvas, "visibilityPolicyAllOnTest", 200, 500);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destPdf, sourceFolder
                 + cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void EnabledVisibilityPolicyAllOffTest() {
            String srcPdf = "sourceWithDifferentLayers.pdf";
            String destPdf = "enabledVisibilityPolicyAllOffTest.pdf";
            String cmpPdf = "cmp_" + destPdf;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), CompareTool.CreateTestPdfWriter
                (destinationFolder + destPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            PdfLayerMembership layerMembershipAllOff = new PdfLayerMembership(pdfDoc);
            layerMembershipAllOff.AddLayer(allLayers[0]);
            layerMembershipAllOff.AddLayer(allLayers[3]);
            layerMembershipAllOff.SetVisibilityPolicy(PdfName.AllOff);
            PdfLayerTestUtils.AddTextInsideLayer(layerMembershipAllOff, canvas, "visibilityPolicyAllOffTest", 200, 500
                );
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destPdf, sourceFolder
                 + cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void DisabledVisibilityPolicyAllOffTest() {
            String srcPdf = "sourceWithDifferentLayers.pdf";
            String destPdf = "disabledVisibilityPolicyAllOffTest.pdf";
            String cmpPdf = "cmp_" + destPdf;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), CompareTool.CreateTestPdfWriter
                (destinationFolder + destPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            PdfLayerMembership layerMembershipAllOff = new PdfLayerMembership(pdfDoc);
            layerMembershipAllOff.AddLayer(allLayers[0]);
            layerMembershipAllOff.AddLayer(allLayers[1]);
            layerMembershipAllOff.SetVisibilityPolicy(PdfName.AllOff);
            PdfLayerTestUtils.AddTextInsideLayer(layerMembershipAllOff, canvas, "visibilityPolicyAllOffTest", 200, 500
                );
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destPdf, sourceFolder
                 + cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void EnabledVisibilityPolicyAnyOnTest() {
            String srcPdf = "sourceWithDifferentLayers.pdf";
            String destPdf = "enabledVisibilityPolicyAnyOnTest.pdf";
            String cmpPdf = "cmp_" + destPdf;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), CompareTool.CreateTestPdfWriter
                (destinationFolder + destPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            PdfLayerMembership layerMembershipAnyOn = new PdfLayerMembership(pdfDoc);
            layerMembershipAnyOn.AddLayer(allLayers[0]);
            layerMembershipAnyOn.AddLayer(allLayers[1]);
            layerMembershipAnyOn.SetVisibilityPolicy(PdfName.AnyOn);
            PdfLayerTestUtils.AddTextInsideLayer(layerMembershipAnyOn, canvas, "visibilityPolicyAnyOnTest", 200, 500);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destPdf, sourceFolder
                 + cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void DisabledVisibilityPolicyAnyOnTest() {
            String srcPdf = "sourceWithDifferentLayers.pdf";
            String destPdf = "disabledVisibilityPolicyAnyOnTest.pdf";
            String cmpPdf = "cmp_" + destPdf;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), CompareTool.CreateTestPdfWriter
                (destinationFolder + destPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            PdfLayerMembership layerMembershipAnyOn = new PdfLayerMembership(pdfDoc);
            layerMembershipAnyOn.AddLayer(allLayers[0]);
            layerMembershipAnyOn.AddLayer(allLayers[3]);
            layerMembershipAnyOn.SetVisibilityPolicy(PdfName.AnyOn);
            PdfLayerTestUtils.AddTextInsideLayer(layerMembershipAnyOn, canvas, "visibilityPolicyAnyOnTest", 200, 500);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destPdf, sourceFolder
                 + cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void EnabledVisibilityPolicyAnyOffTest() {
            String srcPdf = "sourceWithDifferentLayers.pdf";
            String destPdf = "enabledVisibilityPolicyAnyOffTest.pdf";
            String cmpPdf = "cmp_" + destPdf;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), CompareTool.CreateTestPdfWriter
                (destinationFolder + destPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            PdfLayerMembership layerMembershipAnyOn = new PdfLayerMembership(pdfDoc);
            layerMembershipAnyOn.AddLayer(allLayers[0]);
            layerMembershipAnyOn.AddLayer(allLayers[1]);
            layerMembershipAnyOn.SetVisibilityPolicy(PdfName.AnyOff);
            PdfLayerTestUtils.AddTextInsideLayer(layerMembershipAnyOn, canvas, "visibilityPolicyAnyOffTest", 200, 500);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destPdf, sourceFolder
                 + cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void DisabledVisibilityPolicyAnyOffTest() {
            String srcPdf = "sourceWithDifferentLayers.pdf";
            String destPdf = "disabledVisibilityPolicyAnyOffTest.pdf";
            String cmpPdf = "cmp_" + destPdf;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), CompareTool.CreateTestPdfWriter
                (destinationFolder + destPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            PdfLayerMembership layerMembershipAnyOn = new PdfLayerMembership(pdfDoc);
            layerMembershipAnyOn.AddLayer(allLayers[1]);
            layerMembershipAnyOn.AddLayer(allLayers[2]);
            layerMembershipAnyOn.SetVisibilityPolicy(PdfName.AnyOff);
            PdfLayerTestUtils.AddTextInsideLayer(layerMembershipAnyOn, canvas, "visibilityPolicyAnyOffTest", 200, 500);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destPdf, sourceFolder
                 + cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void EnabledVisualExpressionTest01() {
            String srcPdf = "sourceWithDifferentLayers.pdf";
            String destPdf = "enabledVisualExpressionTest01.pdf";
            String cmpPdf = "cmp_" + destPdf;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), CompareTool.CreateTestPdfWriter
                (destinationFolder + destPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            PdfLayerMembership layerMembershipAnyOn = new PdfLayerMembership(pdfDoc);
            // create expression with the AND operator as the first operand
            PdfVisibilityExpression expression = new PdfVisibilityExpression(PdfName.And);
            // add an empty dictionary as the second operand
            expression.AddOperand(allLayers[1]);
            // create a nested expression with the OR operator and two empty dictionaries as operands
            PdfVisibilityExpression nestedExpression = new PdfVisibilityExpression(PdfName.Or);
            nestedExpression.AddOperand(allLayers[0]);
            nestedExpression.AddOperand(allLayers[2]);
            // add another expression as the third operand
            expression.AddOperand(nestedExpression);
            layerMembershipAnyOn.AddLayer(allLayers[0]);
            layerMembershipAnyOn.AddLayer(allLayers[1]);
            layerMembershipAnyOn.SetVisibilityExpression(expression);
            PdfLayerTestUtils.AddTextInsideLayer(layerMembershipAnyOn, canvas, "visualExpressionTest01", 200, 500);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destPdf, sourceFolder
                 + cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void DisabledVisualExpressionTest01() {
            String srcPdf = "sourceWithDifferentLayers.pdf";
            String destPdf = "disabledVisualExpressionTest01.pdf";
            String cmpPdf = "cmp_" + destPdf;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), CompareTool.CreateTestPdfWriter
                (destinationFolder + destPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            PdfLayerMembership layerMembershipAnyOn = new PdfLayerMembership(pdfDoc);
            // create expression with the AND operator as the first operand
            PdfVisibilityExpression expression = new PdfVisibilityExpression(PdfName.And);
            // add an empty dictionary as the second operand
            expression.AddOperand(allLayers[1]);
            // create a nested expression with the AND operator and two empty dictionaries as operands
            PdfVisibilityExpression nestedExpression = new PdfVisibilityExpression(PdfName.And);
            nestedExpression.AddOperand(allLayers[0]);
            nestedExpression.AddOperand(allLayers[2]);
            // add another expression as the third operand
            expression.AddOperand(nestedExpression);
            layerMembershipAnyOn.AddLayer(allLayers[0]);
            layerMembershipAnyOn.AddLayer(allLayers[1]);
            layerMembershipAnyOn.SetVisibilityExpression(expression);
            PdfLayerTestUtils.AddTextInsideLayer(layerMembershipAnyOn, canvas, "visualExpressionTest01", 200, 500);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destPdf, sourceFolder
                 + cmpPdf, destinationFolder));
        }
    }
}
