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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font.Constants;
using iText.IO.Source;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Layer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfLayerTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/layer/PdfLayerTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/layer/PdfLayerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void LayerDefaultIntents() {
            PdfLayer pdfLayer = PdfLayerTestUtils.PrepareNewLayer();
            ICollection<PdfName> defaultIntents = pdfLayer.GetIntents();
            NUnit.Framework.Assert.AreEqual(new PdfName[] { PdfName.View }, defaultIntents.ToArray(new PdfName[1]));
        }

        [NUnit.Framework.Test]
        public virtual void LayerSetSingleIntent() {
            PdfLayer pdfLayer = PdfLayerTestUtils.PrepareLayerDesignIntent();
            ICollection<PdfName> defaultIntents = pdfLayer.GetIntents();
            NUnit.Framework.Assert.AreEqual(new PdfName[] { PdfName.Design }, defaultIntents.ToArray(new PdfName[1]));
        }

        [NUnit.Framework.Test]
        public virtual void LayerSetSeveralIntents() {
            PdfName custom = new PdfName("Custom");
            PdfLayer pdfLayer = PdfLayerTestUtils.PrepareLayerDesignAndCustomIntent(custom);
            ICollection<PdfName> defaultIntents = pdfLayer.GetIntents();
            NUnit.Framework.Assert.AreEqual(new PdfName[] { PdfName.Design, custom }, defaultIntents.ToArray(new PdfName
                [2]));
        }

        [NUnit.Framework.Test]
        public virtual void LayerSetIntentsNull() {
            PdfName custom = new PdfName("Custom");
            PdfLayer pdfLayer = PdfLayerTestUtils.PrepareLayerDesignAndCustomIntent(custom);
            pdfLayer.SetIntents(null);
            ICollection<PdfName> postNullIntents = pdfLayer.GetIntents();
            NUnit.Framework.Assert.AreEqual(new PdfName[] { PdfName.View }, postNullIntents.ToArray(new PdfName[1]));
        }

        [NUnit.Framework.Test]
        public virtual void LayerSetIntentsEmpty() {
            PdfName custom = new PdfName("Custom");
            PdfLayer pdfLayer = PdfLayerTestUtils.PrepareLayerDesignAndCustomIntent(custom);
            pdfLayer.SetIntents(JavaCollectionsUtil.EmptyList<PdfName>());
            ICollection<PdfName> postNullIntents = pdfLayer.GetIntents();
            NUnit.Framework.Assert.AreEqual(new PdfName[] { PdfName.View }, postNullIntents.ToArray(new PdfName[1]));
        }

        [NUnit.Framework.Test]
        public virtual void NestedLayers() {
            String outPdf = destinationFolder + "nestedLayers.pdf";
            String cmpPdf = sourceFolder + "cmp_nestedLayers.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer nested = new PdfLayer("Parent layer", pdfDoc);
            PdfLayer nested_1 = new PdfLayer("Nested layer 1", pdfDoc);
            PdfLayer nested_2 = new PdfLayer("Nested layer 2", pdfDoc);
            nested.AddChild(nested_1);
            nested.AddChild(nested_2);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 12);
            PdfLayerTestUtils.AddTextInsideLayer(nested, canvas, "Parent layer text", 50, 755);
            PdfLayerTestUtils.AddTextInsideLayer(nested_1, canvas, "Nested layer 1 text", 100, 700);
            PdfLayerTestUtils.AddTextInsideLayer(nested_2, canvas, "Nested layers 2 text", 100, 650);
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void LockedLayer() {
            String outPdf = destinationFolder + "lockedLayer.pdf";
            String cmpPdf = sourceFolder + "cmp_lockedLayer.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer layer1 = new PdfLayer("Layer 1", pdfDoc);
            PdfLayer layer2 = new PdfLayer("Layer 2", pdfDoc);
            layer2.SetLocked(true);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 12);
            PdfLayerTestUtils.AddTextInsideLayer(layer1, canvas, "Layer 1 text", 100, 700);
            PdfLayerTestUtils.AddTextInsideLayer(layer2, canvas, "Layer 2 text", 100, 650);
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void LayerGroup() {
            String outPdf = destinationFolder + "layerGroup.pdf";
            String cmpPdf = sourceFolder + "cmp_layerGroup.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer group = PdfLayer.CreateTitle("Grouped layers", pdfDoc);
            PdfLayer layer1 = new PdfLayer("Group: layer 1", pdfDoc);
            PdfLayer layer2 = new PdfLayer("Group: layer 2", pdfDoc);
            group.AddChild(layer1);
            group.AddChild(layer2);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 12);
            PdfLayerTestUtils.AddTextInsideLayer(layer1, canvas, "layer 1 in the group", 50, 700);
            PdfLayerTestUtils.AddTextInsideLayer(layer2, canvas, "layer 2 in the group", 50, 675);
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void LayersRadioGroup() {
            String outPdf = destinationFolder + "layersRadioGroup.pdf";
            String cmpPdf = sourceFolder + "cmp_layersRadioGroup.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer radiogroup = PdfLayer.CreateTitle("Radio group", pdfDoc);
            PdfLayer radio1 = new PdfLayer("Radiogroup: layer 1", pdfDoc);
            radio1.SetOn(true);
            PdfLayer radio2 = new PdfLayer("Radiogroup: layer 2", pdfDoc);
            radio2.SetOn(false);
            PdfLayer radio3 = new PdfLayer("Radiogroup: layer 3", pdfDoc);
            radio3.SetOn(false);
            radiogroup.AddChild(radio1);
            radiogroup.AddChild(radio2);
            radiogroup.AddChild(radio3);
            IList<PdfLayer> options = new List<PdfLayer>();
            options.Add(radio1);
            options.Add(radio2);
            options.Add(radio3);
            PdfLayer.AddOCGRadioGroup(pdfDoc, options);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 12);
            PdfLayerTestUtils.AddTextInsideLayer(radio1, canvas, "layer option 1", 50, 600);
            PdfLayerTestUtils.AddTextInsideLayer(radio2, canvas, "layer option 2", 50, 575);
            PdfLayerTestUtils.AddTextInsideLayer(radio3, canvas, "layer option 3", 50, 550);
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void NotPrintNotOnPanel() {
            String outPdf = destinationFolder + "notPrintNotOnPanel.pdf";
            String cmpPdf = sourceFolder + "cmp_notPrintNotOnPanel.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer notPrintedNotOnPanel = new PdfLayer("not printed", pdfDoc);
            notPrintedNotOnPanel.SetOnPanel(false);
            notPrintedNotOnPanel.SetPrint("Print", false);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 14);
            PdfLayerTestUtils.AddTextInsideLayer(null, canvas, "Normal page content, hello lorem ispum!", 100, 750);
            canvas.BeginLayer(notPrintedNotOnPanel);
            canvas.BeginText().SetFontAndSize(font, 24).MoveText(100, 700).ShowText("WHEN PRINTED THIS LINE IS NOT THERE"
                ).EndText();
            canvas.BeginText().SetFontAndSize(font, 16).MoveText(100, 680).ShowText("(this text layer is not in the layers panel as well)"
                ).EndText();
            canvas.EndLayer();
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ZoomNotOnPanel() {
            String outPdf = destinationFolder + "zoomNotOnPanel.pdf";
            String cmpPdf = sourceFolder + "cmp_zoomNotOnPanel.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer zoom = new PdfLayer("Zoom 0.75-1.25", pdfDoc);
            zoom.SetOnPanel(false);
            zoom.SetZoom(0.75f, 1.25f);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 14);
            PdfLayerTestUtils.AddTextInsideLayer(zoom, canvas, "Only visible if the zoomfactor is between 75 and 125%"
                , 30, 530);
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void OcConfigUniqueName() {
            String srcPdf = sourceFolder + "ocpConfigs.pdf";
            String outPdf = destinationFolder + "ocConfigUniqueName.pdf";
            String cmpPdf = sourceFolder + "cmp_ocConfigUniqueName.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), CompareTool.CreateTestPdfWriter(outPdf));
            // init OCProperties to check how they are processed
            pdfDoc.GetCatalog().GetOCProperties(true);
            pdfDoc.Close();
            // start of test assertion logic
            PdfDocument resPdf = new PdfDocument(CompareTool.CreateOutputReader(outPdf));
            PdfDictionary d = resPdf.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.OCProperties).GetAsDictionary
                (PdfName.D);
            NUnit.Framework.Assert.AreEqual(PdfOCProperties.OC_CONFIG_NAME_PATTERN + "2", d.GetAsString(PdfName.Name).
                ToUnicodeString());
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessTitledHierarchies() {
            String srcPdf = sourceFolder + "titledHierarchies.pdf";
            String outPdf = destinationFolder + "processTitledHierarchies.pdf";
            String cmpPdf = sourceFolder + "cmp_processTitledHierarchies.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), CompareTool.CreateTestPdfWriter(outPdf));
            // init OCProperties to check how they are processed
            pdfDoc.GetCatalog().GetOCProperties(true);
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void SetCreatorInfoAndLanguage() {
            String outPdf = destinationFolder + "setCreatorInfoAndLanguage.pdf";
            String cmpPdf = sourceFolder + "cmp_setCreatorInfoAndLanguage.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer layer = new PdfLayer("CreatorAndLanguageInfo", pdfDoc);
            layer.SetCreatorInfo("iText", "Technical");
            // australian english
            layer.SetLanguage("en-AU", true);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 14);
            PdfLayerTestUtils.AddTextInsideLayer(layer, canvas, "Some technical data in English.", 30, 530);
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void SetUserAndPageElement() {
            String outPdf = destinationFolder + "setUserAndPageElement.pdf";
            String cmpPdf = sourceFolder + "cmp_setUserAndPageElement.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer layer = new PdfLayer("UserAndPageElement", pdfDoc);
            layer.SetUser("Org", "iText");
            layer.SetPageElement("HF");
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 14);
            PdfLayerTestUtils.AddTextInsideLayer(layer, canvas, "Page 1 of 1.", 30, 780);
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void SetExportViewIsTrue() {
            String outPdf = destinationFolder + "setExportViewIsTrue.pdf";
            String cmpPdf = sourceFolder + "cmp_setExportViewIsTrue.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            bool view = true;
            CreateCustomExportLayers(pdfDoc, view);
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void SetExportViewIsFalse() {
            String outPdf = destinationFolder + "setExportViewIsFalse.pdf";
            String cmpPdf = sourceFolder + "cmp_setExportViewIsFalse.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            bool view = false;
            CreateCustomExportLayers(pdfDoc, view);
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        private void CreateCustomExportLayers(PdfDocument pdfDoc, bool view) {
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer layerTrue = new PdfLayer("Export - true", pdfDoc);
            layerTrue.SetExport(true);
            layerTrue.SetView(view);
            PdfLayer layerFalse = new PdfLayer("Export - false", pdfDoc);
            layerFalse.SetExport(false);
            layerFalse.SetView(view);
            PdfLayer layerDflt = new PdfLayer("Export - default", pdfDoc);
            layerDflt.SetView(view);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 24);
            PdfLayerTestUtils.AddTextInsideLayer(null, canvas, "Export this PDF as image!", 30, 580);
            canvas.SetFontAndSize(font, 14);
            PdfLayerTestUtils.AddTextInsideLayer(layerTrue, canvas, "Export layer - true.", 30, 780);
            PdfLayerTestUtils.AddTextInsideLayer(null, canvas, "When saved as image text above is expected to be shown."
                , 30, 765);
            PdfLayerTestUtils.AddTextInsideLayer(layerFalse, canvas, "Export layer - false.", 30, 730);
            PdfLayerTestUtils.AddTextInsideLayer(null, canvas, "When saved as image text above is expected to be hidden."
                , 30, 715);
            PdfLayerTestUtils.AddTextInsideLayer(layerDflt, canvas, "Export layer - default.", 30, 680);
            PdfLayerTestUtils.AddTextInsideLayer(null, canvas, "When saved as image text above is expected to have layer visibility."
                , 30, 665);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestInStamperMode1() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "input_layered.pdf"), CompareTool.CreateTestPdfWriter
                (destinationFolder + "output_copy_layered.pdf"));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "output_copy_layered.pdf"
                , sourceFolder + "cmp_output_copy_layered.pdf", destinationFolder, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestInStamperMode2() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "input_layered.pdf"), CompareTool.CreateTestPdfWriter
                (destinationFolder + "output_layered.pdf"));
            PdfCanvas canvas = new PdfCanvas(pdfDoc, 1);
            PdfLayer newLayer = new PdfLayer("appended", pdfDoc);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            PdfLayerTestUtils.AddTextInsideLayer(newLayer, canvas, "APPENDED CONTENT", 200, 600);
            IList<PdfLayer> allLayers = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            foreach (PdfLayer layer in allLayers) {
                if (layer.IsLocked()) {
                    layer.SetLocked(false);
                }
                if ("Grouped layers".Equals(layer.GetTitle())) {
                    layer.AddChild(newLayer);
                }
            }
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "output_layered.pdf", 
                sourceFolder + "cmp_output_layered.pdf", destinationFolder, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestReadAllLayersFromPage1() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "input_layered.pdf"), CompareTool.CreateTestPdfWriter
                (destinationFolder + "output_layered_2.pdf"));
            PdfCanvas canvas = new PdfCanvas(pdfDoc, 1);
            //create layer on page
            PdfLayer newLayer = new PdfLayer("appended", pdfDoc);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 18);
            PdfLayerTestUtils.AddTextInsideLayer(newLayer, canvas, "APPENDED CONTENT", 200, 600);
            IList<PdfLayer> layersFromCatalog = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(13, layersFromCatalog.Count);
            PdfPage page = pdfDoc.GetPage(1);
            ICollection<PdfLayer> layersFromPage = page.GetPdfLayers();
            NUnit.Framework.Assert.AreEqual(11, layersFromPage.Count);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "output_layered_2.pdf"
                , sourceFolder + "cmp_output_layered_2.pdf", destinationFolder, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TestReadAllLayersFromDocumentWithComplexOCG() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "input_complex_layers.pdf"), CompareTool
                .CreateTestPdfWriter(destinationFolder + "output_complex_layers.pdf"));
            IList<PdfLayer> layersFromCatalog = pdfDoc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(12, layersFromCatalog.Count);
            PdfPage page = pdfDoc.GetPage(1);
            ICollection<PdfLayer> layersFromPage = page.GetPdfLayers();
            NUnit.Framework.Assert.AreEqual(10, layersFromPage.Count);
            pdfDoc.Close();
        }

        //Read OCGs from different locations (annotations, content streams, xObjects) test block
        [NUnit.Framework.Test]
        public virtual void TestReadOcgFromStreamProperties() {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = document.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    pdfResource.AddProperties(new PdfLayer("name", document).GetPdfObject());
                    pdfResource.MakeIndirect(document);
                    ICollection<PdfLayer> layersFromPage = page.GetPdfLayers();
                    NUnit.Framework.Assert.AreEqual(1, layersFromPage.Count);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestReadOcgFromAnnotation() {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfAnnotation annotation = new PdfTextAnnotation(new Rectangle(50, 10));
                    annotation.SetLayer(new PdfLayer("name", fromDocument));
                    page.AddAnnotation(annotation);
                    ICollection<PdfLayer> layersFromPage = page.GetPdfLayers();
                    NUnit.Framework.Assert.AreEqual(1, layersFromPage.Count);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestReadOcgFromFlushedAnnotation() {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfAnnotation annotation = new PdfTextAnnotation(new Rectangle(50, 10));
                    annotation.SetLayer(new PdfLayer("name", fromDocument));
                    page.AddAnnotation(annotation);
                    annotation.Flush();
                    ICollection<PdfLayer> layersFromPage = page.GetPdfLayers();
                    NUnit.Framework.Assert.AreEqual(1, layersFromPage.Count);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestReadOcgFromApAnnotation() {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfAnnotation annotation = new PdfTextAnnotation(new Rectangle(50, 10));
                    PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(50, 10));
                    formXObject.SetLayer(new PdfLayer("someName1", fromDocument));
                    formXObject.MakeIndirect(fromDocument);
                    PdfDictionary nDict = new PdfDictionary();
                    nDict.Put(PdfName.ON, formXObject.GetPdfObject());
                    annotation.SetAppearance(PdfName.N, nDict);
                    formXObject = new PdfFormXObject(new Rectangle(50, 10));
                    formXObject.SetLayer(new PdfLayer("someName2", fromDocument));
                    PdfResources formResources = formXObject.GetResources();
                    formResources.AddProperties(new PdfLayer("someName3", fromDocument).GetPdfObject());
                    formXObject.MakeIndirect(fromDocument);
                    PdfDictionary rDict = new PdfDictionary();
                    rDict.Put(PdfName.OFF, formXObject.GetPdfObject());
                    annotation.SetAppearance(PdfName.R, rDict);
                    formXObject = new PdfFormXObject(new Rectangle(50, 10));
                    formXObject.SetLayer(new PdfLayer("someName4", fromDocument));
                    formXObject.MakeIndirect(fromDocument);
                    annotation.SetAppearance(PdfName.D, formXObject.GetPdfObject());
                    page.AddAnnotation(annotation);
                    ICollection<PdfLayer> layersFromPage = page.GetPdfLayers();
                    NUnit.Framework.Assert.AreEqual(4, layersFromPage.Count);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void NestedLayerTwoParentsTest() {
            String outPdf = destinationFolder + "nestedLayerTwoParents.pdf";
            String cmpPdf = sourceFolder + "cmp_nestedLayerTwoParents.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer parentLayer1 = new PdfLayer("Parent layer 1", pdfDoc);
            PdfLayer parentLayer2 = new PdfLayer("Parent layer 2", pdfDoc);
            PdfLayer nestedLayer = new PdfLayer("Nested layer 1", pdfDoc);
            parentLayer1.AddChild(nestedLayer);
            parentLayer2.AddChild(nestedLayer);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 12);
            PdfLayerTestUtils.AddTextInsideLayer(parentLayer1, canvas, "Parent layer 1 text", 50, 750);
            PdfLayerTestUtils.AddTextInsideLayer(parentLayer2, canvas, "Parent layer 2 text", 50, 700);
            PdfLayerTestUtils.AddTextInsideLayer(nestedLayer, canvas, "Nested layer 1 text", 100, 650);
            canvas.Release();
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void NestedLayerTwoParentsWithOneParentTest() {
            String outPdf = destinationFolder + "nestedLayerTwoParentsWithOneParent.pdf";
            String cmpPdf = sourceFolder + "cmp_nestedLayerTwoParentsWithOneParent.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer parentLayer = new PdfLayer("Parent layer", pdfDoc);
            PdfLayer layer1 = new PdfLayer("Layer 1", pdfDoc);
            PdfLayer layer2 = new PdfLayer("Layer 2", pdfDoc);
            PdfLayer nestedLayer = new PdfLayer("Nested layer 1", pdfDoc);
            layer1.AddChild(nestedLayer);
            layer2.AddChild(nestedLayer);
            parentLayer.AddChild(layer1);
            parentLayer.AddChild(layer2);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 12);
            PdfLayerTestUtils.AddTextInsideLayer(parentLayer, canvas, "Parent layer text", 50, 750);
            PdfLayerTestUtils.AddTextInsideLayer(layer1, canvas, "layer 1 text", 100, 700);
            PdfLayerTestUtils.AddTextInsideLayer(layer2, canvas, "layer 2 text", 100, 650);
            PdfLayerTestUtils.AddTextInsideLayer(nestedLayer, canvas, "Nested layer text", 150, 600);
            canvas.Release();
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void DuplicatedNestedLayersTest() {
            String outPdf = destinationFolder + "duplicatedNestedLayers.pdf";
            String cmpPdf = sourceFolder + "cmp_duplicatedNestedLayers.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont();
            PdfLayer parentLayer = new PdfLayer("Parent layer", pdfDoc);
            PdfLayer nestedLayer1 = new PdfLayer("Nested layer", pdfDoc);
            parentLayer.AddChild(nestedLayer1);
            parentLayer.AddChild(nestedLayer1);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFontAndSize(font, 12);
            PdfLayerTestUtils.AddTextInsideLayer(parentLayer, canvas, "Parent layer text", 50, 750);
            PdfLayerTestUtils.AddTextInsideLayer(nestedLayer1, canvas, "Nested layer text", 100, 700);
            canvas.Release();
            pdfDoc.Close();
            PdfLayerTestUtils.CompareLayers(outPdf, cmpPdf);
        }
    }
}
