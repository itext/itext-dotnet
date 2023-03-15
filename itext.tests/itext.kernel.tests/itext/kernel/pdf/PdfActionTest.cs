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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfActionTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfActionTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfActionTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ActionTest01() {
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + "actionTest01.pdf"), true);
            document.GetCatalog().SetOpenAction(PdfAction.CreateURI("http://itextpdf.com/"));
            document.Close();
            System.Console.Out.WriteLine(MessageFormatUtil.Format("Please open document {0} and make sure that you're automatically redirected to {1} site."
                , destinationFolder + "actionTest01.pdf", "http://itextpdf.com"));
        }

        [NUnit.Framework.Test]
        public virtual void ActionTest02() {
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + "actionTest02.pdf"), false);
            document.GetPage(2).SetAdditionalAction(PdfName.O, PdfAction.CreateURI("http://itextpdf.com/"));
            document.Close();
            System.Console.Out.WriteLine(MessageFormatUtil.Format("Please open document {0} at page 2 and make sure that you're automatically redirected to {1} site."
                , destinationFolder + "actionTest02.pdf", "http://itextpdf.com"));
        }

        [NUnit.Framework.Test]
        public virtual void SoundActionTest() {
            String fileName = "soundActionTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            Stream @is = new FileStream(sourceFolder + "sample.aif", FileMode.Open, FileAccess.Read);
            PdfStream sound1 = new PdfStream(document, @is);
            sound1.Put(PdfName.R, new PdfNumber(32117));
            sound1.Put(PdfName.E, PdfName.Signed);
            sound1.Put(PdfName.B, new PdfNumber(16));
            sound1.Put(PdfName.C, new PdfNumber(1));
            document.GetPage(2).SetAdditionalAction(PdfName.O, PdfAction.CreateSound(sound1));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SoundActionWithRepeatFlagTest() {
            String fileName = "soundActionWithRepeatFlagTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            Stream @is = new FileStream(sourceFolder + "sample.aif", FileMode.Open, FileAccess.Read);
            PdfStream sound1 = new PdfStream(document, @is);
            sound1.Put(PdfName.R, new PdfNumber(32117));
            sound1.Put(PdfName.E, PdfName.Signed);
            sound1.Put(PdfName.B, new PdfNumber(16));
            sound1.Put(PdfName.C, new PdfNumber(1));
            document.GetPage(2).SetAdditionalAction(PdfName.O, PdfAction.CreateSound(sound1, 1f, false, true, false));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SoundActionWithToBigVolumeTest() {
            PdfDocument document = CreateDocument(new PdfWriter(new MemoryStream()), false);
            Stream @is = new FileStream(sourceFolder + "sample.aif", FileMode.Open, FileAccess.Read);
            PdfStream sound1 = new PdfStream(document, @is);
            sound1.Put(PdfName.R, new PdfNumber(32117));
            sound1.Put(PdfName.E, PdfName.Signed);
            sound1.Put(PdfName.B, new PdfNumber(16));
            sound1.Put(PdfName.C, new PdfNumber(1));
            try {
                document.GetPage(2).SetAdditionalAction(PdfName.O, PdfAction.CreateSound(sound1, 1.1f, false, false, false
                    ));
                NUnit.Framework.Assert.Fail("Exception not thrown");
            }
            catch (Exception e) {
                NUnit.Framework.Assert.AreEqual("volume", e.Message);
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void SoundActionWithToLowVolumeTest() {
            PdfDocument document = CreateDocument(new PdfWriter(new MemoryStream()), false);
            Stream @is = new FileStream(sourceFolder + "sample.aif", FileMode.Open, FileAccess.Read);
            PdfStream sound1 = new PdfStream(document, @is);
            sound1.Put(PdfName.R, new PdfNumber(32117));
            sound1.Put(PdfName.E, PdfName.Signed);
            sound1.Put(PdfName.B, new PdfNumber(16));
            sound1.Put(PdfName.C, new PdfNumber(1));
            try {
                document.GetPage(2).SetAdditionalAction(PdfName.O, PdfAction.CreateSound(sound1, -1.1f, false, false, false
                    ));
                NUnit.Framework.Assert.Fail("Exception not thrown");
            }
            catch (Exception e) {
                NUnit.Framework.Assert.AreEqual("volume", e.Message);
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void OcgStateTest() {
            PdfName stateName = PdfName.ON;
            PdfDictionary ocgDict1 = new PdfDictionary();
            ocgDict1.Put(PdfName.Type, PdfName.OCG);
            ocgDict1.Put(PdfName.Name, new PdfName("ocg1"));
            PdfDictionary ocgDict2 = new PdfDictionary();
            ocgDict2.Put(PdfName.Type, PdfName.OCG);
            ocgDict2.Put(PdfName.Name, new PdfName("ocg2"));
            IList<PdfDictionary> dicts = new List<PdfDictionary>();
            dicts.Add(ocgDict1);
            dicts.Add(ocgDict2);
            IList<PdfActionOcgState> ocgStates = new List<PdfActionOcgState>();
            ocgStates.Add(new PdfActionOcgState(stateName, dicts));
            String fileName = "ocgStateTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            document.GetPage(1).SetAdditionalAction(PdfName.O, PdfAction.CreateSetOcgState(ocgStates));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void LaunchActionTest() {
            String fileName = "launchActionTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            document.GetPage(1).SetAdditionalAction(PdfName.O, PdfAction.CreateLaunch(new PdfStringFS("launch.sh")));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void LaunchActionOnNewWindowTest() {
            String fileName = "launchActionOnNewWindowTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            document.GetPage(1).SetAdditionalAction(PdfName.O, PdfAction.CreateLaunch(new PdfStringFS("launch.sh"), true
                ));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateHiddenAnnotationTest() {
            String fileName = "createHiddenAnnotationTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            PdfAnnotation annotation = new PdfLineAnnotation(new Rectangle(10, 10, 200, 200), new float[] { 50, 750, 50
                , 750 });
            document.GetPage(1).SetAdditionalAction(PdfName.O, PdfAction.CreateHide(annotation, true));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateHiddenAnnotationsTest() {
            String fileName = "createHiddenAnnotationsTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            PdfAnnotation[] annotations = new PdfAnnotation[] { new PdfLineAnnotation(new Rectangle(10, 10, 200, 200), 
                new float[] { 50, 750, 50, 750 }), new PdfLineAnnotation(new Rectangle(200, 200, 200, 200), new float[
                ] { 50, 750, 50, 750 }) };
            document.GetPage(1).SetAdditionalAction(PdfName.O, PdfAction.CreateHide(annotations, true));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateHiddenByFieldNameTest() {
            String fileName = "createHiddenByFieldNameTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            document.GetPage(1).SetAdditionalAction(PdfName.O, PdfAction.CreateHide("name", true));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateHiddenByFieldNamesTest() {
            String fileName = "createHiddenByFieldNamesTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            document.GetPage(1).SetAdditionalAction(PdfName.O, PdfAction.CreateHide(new String[] { "name1", "name2" }, 
                true));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateNamedTest() {
            String fileName = "createNamedTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            document.GetPage(1).SetAdditionalAction(PdfName.O, PdfAction.CreateNamed(PdfName.LastPage));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateJavaScriptTest() {
            String fileName = "createJavaScriptTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            String javaScriptRotatePages = "this.setPageRotations(0,2,90)";
            document.GetPage(1).SetAdditionalAction(PdfName.O, PdfAction.CreateJavaScript(javaScriptRotatePages));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SoundAndNextJavaScriptActionTest() {
            String fileName = "soundAndNextJavaScriptActionTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            Stream @is = new FileStream(sourceFolder + "sample.aif", FileMode.Open, FileAccess.Read);
            PdfStream sound1 = new PdfStream(document, @is);
            sound1.Put(PdfName.R, new PdfNumber(32117));
            sound1.Put(PdfName.E, PdfName.Signed);
            sound1.Put(PdfName.B, new PdfNumber(16));
            sound1.Put(PdfName.C, new PdfNumber(1));
            PdfAction action = PdfAction.CreateSound(sound1);
            action.Next(PdfAction.CreateJavaScript("this.setPageRotations(0,2,90)"));
            document.GetPage(2).SetAdditionalAction(PdfName.O, action);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SoundAndTwoNextJavaScriptActionTest() {
            String fileName = "soundAndTwoNextJavaScriptActionTest.pdf";
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + fileName), false);
            Stream @is = new FileStream(sourceFolder + "sample.aif", FileMode.Open, FileAccess.Read);
            PdfStream sound1 = new PdfStream(document, @is);
            sound1.Put(PdfName.R, new PdfNumber(32117));
            sound1.Put(PdfName.E, PdfName.Signed);
            sound1.Put(PdfName.B, new PdfNumber(16));
            sound1.Put(PdfName.C, new PdfNumber(1));
            PdfAction action = PdfAction.CreateSound(sound1);
            action.Next(PdfAction.CreateJavaScript("this.setPageRotations(0,2,90)"));
            action.Next(PdfAction.CreateJavaScript("this.setPageRotations(0,2,180)"));
            document.GetPage(2).SetAdditionalAction(PdfName.O, action);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        private PdfDocument CreateDocument(PdfWriter writer, bool flushPages) {
            PdfDocument document = new PdfDocument(writer);
            PdfPage p1 = document.AddNewPage();
            PdfStream str1 = p1.GetFirstContentStream();
            str1.GetOutputStream().WriteString("1 0 0 rg 100 600 100 100 re f\n");
            if (flushPages) {
                p1.Flush();
            }
            PdfPage p2 = document.AddNewPage();
            PdfStream str2 = p2.GetFirstContentStream();
            str2.GetOutputStream().WriteString("0 1 0 rg 100 600 100 100 re f\n");
            if (flushPages) {
                p2.Flush();
            }
            PdfPage p3 = document.AddNewPage();
            PdfStream str3 = p3.GetFirstContentStream();
            str3.GetOutputStream().WriteString("0 0 1 rg 100 600 100 100 re f\n");
            if (flushPages) {
                p3.Flush();
            }
            return document;
        }
    }
}
