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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    [NUnit.Framework.Category("IntegrationTest")]
    public class InlineImageExtractionTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/canvas/parser/InlineImageExtractionTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/parser/InlineImageExtractionTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ExtractSingleInlineImageWithIndexedColorSpaceTest() {
            PdfDocument pdf = new PdfDocument(new PdfReader(sourceFolder + "inlineImageExtraction.pdf"));
            InlineImageExtractionTest.InlineImageEventListener eventListener = new InlineImageExtractionTest.InlineImageEventListener
                ();
            PdfCanvasProcessor canvasProcessor = new PdfCanvasProcessor(eventListener);
            canvasProcessor.ProcessPageContent(pdf.GetFirstPage());
            pdf.Close();
            IList<PdfStream> inlineImages = eventListener.GetInlineImages();
            NUnit.Framework.Assert.AreEqual(1, inlineImages.Count);
            byte[] imgBytes = inlineImages[0].GetBytes();
            byte[] cmpImgBytes = File.ReadAllBytes(System.IO.Path.Combine(sourceFolder, "imgtest.dat"));
            NUnit.Framework.Assert.AreEqual(cmpImgBytes, imgBytes);
            PdfDictionary expectedDict = new PdfDictionary();
            expectedDict.Put(PdfName.BitsPerComponent, new PdfNumber(8));
            expectedDict.Put(PdfName.Height, new PdfNumber(50));
            expectedDict.Put(PdfName.Width, new PdfNumber(50));
            String indexedCsLookupData = "\u007F\u007F\u007Fïïï\u000F\u000F\u000F???¿¿¿ÏÏÏ///\u001F\u001F\u001F___ßßß"
                 + "\u009F\u009F\u009FOOO¯¯¯ooo\u008F\u008F\u008F°°µ::<ââàuuy,,-ÜÜâ\u000E\u000E\u000Fúúû\u001D\u001D\u001E"
                 + "ððõXXZ::?\u0004\u0004\u0004226!!$IIK\u0019\u0019\u001Býýþ\u0092\u0092\u0097õõø\f\f\r" + "))-÷÷úììòÍÍÓ66;\b\b\t\u0084\u0084\u0088¡¡¦îîô\u0014\u0014\u0016òòö\u0010\u0010\u0012¾¾Äffiüüýóó÷..2ûûü"
                 + "ööù%%)ííó\u001D\u001D\u001F>>Døøúññö\u000E\u000E\u000Eééç\u008D\u008D\u008CÓÓÒCCI©©¨\u009B\u009B\u009A"
                 + "òòñôôózz|888÷÷÷ììëÝÝãµµ¸bbb\u0095\u0095\u0098··¶ûûûºº¼\u0089\u0089\u008Bååãêêë==>ÑÑÖ***qqpààåZZ\\õõõ"
                 + "\u007F\u007F~\u008E\u008E\u008E\u001E\u001E\u001FÀÀÅååèÆÆÅççåÇÇÊ\u001C\u001C\u001C]]^±±¶TTTççêÉÉÇFFFáá"
                 + "æÅÅÄyy{ÍÍÎÐÐÕ^^^vvyîîí\u0087\u0087\u008A}}}xxzÊÊËjjl--.ëëò\u0000\u0000\u0000ÿÿÿ{{{|||}}}~~~\u007F\u007F"
                 + "\u007F\u0080\u0080\u0080\u0081\u0081\u0081\u0082\u0082\u0082\u0083\u0083\u0083\u0084\u0084\u0084\u0085"
                 + "\u0085\u0085\u0086\u0086\u0086\u0087\u0087\u0087\u0088\u0088\u0088\u0089\u0089\u0089\u008A\u008A\u008A"
                 + "\u008B\u008B\u008B\u008C\u008C\u008C\u008D\u008D\u008D\u008E\u008E\u008E\u008F\u008F\u008F\u0090\u0090"
                 + "\u0090\u0091\u0091\u0091\u0092\u0092\u0092\u0093\u0093\u0093\u0094\u0094\u0094\u0095\u0095\u0095\u0096"
                 + "\u0096\u0096\u0097\u0097\u0097\u0098\u0098\u0098\u0099\u0099\u0099\u009A\u009A\u009A\u009B\u009B\u009B"
                 + "\u009C\u009C\u009C\u009D\u009D\u009D\u009E\u009E\u009E\u009F\u009F\u009F   ¡¡¡¢¢¢£££¤¤¤¥¥¥¦¦¦§§§¨¨¨©©©"
                 + "ªªª«««¬¬¬\u00AD\u00AD\u00AD®®®¯¯¯°°°±±±²²²³³³´´´µµµ¶¶¶···¸¸¸¹¹¹ººº»»»¼¼¼½½½¾¾¾¿¿¿ÀÀÀÁÁÁÂÂÂÃÃÃÄÄÄÅÅÅÆÆÆ"
                 + "ÇÇÇÈÈÈÉÉÉÊÊÊËËËÌÌÌÍÍÍÎÎÎÏÏÏÐÐÐÑÑÑÒÒÒÓÓÓÔÔÔÕÕÕÖÖÖ×××ØØØÙÙÙÚÚÚÛÛÛÜÜÜÝÝÝÞÞÞßßßàààáááâââãããäääåååæææçççèèè"
                 + "éééêêêëëëìììíííîîîïïïðððñññòòòóóóôôôõõõööö÷÷÷øøøùùùúúúûûûüüüýýýþþþÿÿÿ";
            PdfSpecialCs.Indexed expectedIndexedCs = new PdfSpecialCs.Indexed(PdfName.DeviceRGB, 255, new PdfString(indexedCsLookupData
                ));
            expectedDict.Put(PdfName.ColorSpace, expectedIndexedCs.GetPdfObject());
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareDictionaries(inlineImages[0], expectedDict));
        }

        [NUnit.Framework.Test]
        public virtual void ParseInlineImageTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "docWithInlineImage.pdf"));
            InlineImageExtractionTest.InlineImageEventListener listener = new InlineImageExtractionTest.InlineImageEventListener
                ();
            new PdfCanvasProcessor(listener).ProcessPageContent(pdfDocument.GetFirstPage());
            IList<PdfStream> inlineImages = listener.GetInlineImages();
            byte[] data = new PdfImageXObject(inlineImages[0]).GetImageBytes();
            byte[] cmpImgBytes = File.ReadAllBytes(System.IO.Path.Combine(sourceFolder, "docWithInlineImageBytes.dat")
                );
            NUnit.Framework.Assert.AreEqual(cmpImgBytes, data);
        }

        private class InlineImageEventListener : IEventListener {
            private IList<PdfStream> inlineImages = new List<PdfStream>();

            public virtual IList<PdfStream> GetInlineImages() {
                return inlineImages;
            }

            public virtual void EventOccurred(IEventData data, EventType type) {
                switch (type) {
                    case EventType.RENDER_IMAGE: {
                        ImageRenderInfo imageEventData = (ImageRenderInfo)data;
                        if (((ImageRenderInfo)data).IsInline()) {
                            inlineImages.Add(imageEventData.GetImage().GetPdfObject());
                        }
                        break;
                    }
                }
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return new LinkedHashSet<EventType>(JavaCollectionsUtil.SingletonList(EventType.RENDER_IMAGE));
            }
        }
    }
}
