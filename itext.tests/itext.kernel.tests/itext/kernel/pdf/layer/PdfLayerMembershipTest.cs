/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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

        [NUnit.Framework.Test]
        public virtual void EnabledVisibilityPolicyAllOnTest() {
            String srcPdf = "sourceWithDifferentLayers.pdf";
            String destPdf = "enabledVisibilityPolicyAllOnTest.pdf";
            String cmpPdf = "cmp_" + destPdf;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), new PdfWriter(destinationFolder
                 + destPdf));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), new PdfWriter(destinationFolder
                 + destPdf));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), new PdfWriter(destinationFolder
                 + destPdf));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), new PdfWriter(destinationFolder
                 + destPdf));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), new PdfWriter(destinationFolder
                 + destPdf));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), new PdfWriter(destinationFolder
                 + destPdf));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), new PdfWriter(destinationFolder
                 + destPdf));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), new PdfWriter(destinationFolder
                 + destPdf));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), new PdfWriter(destinationFolder
                 + destPdf));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + srcPdf), new PdfWriter(destinationFolder
                 + destPdf));
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
