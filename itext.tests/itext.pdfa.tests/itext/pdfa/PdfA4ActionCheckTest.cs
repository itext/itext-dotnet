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
using iText.Forms.Form.Element;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Pdfa.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA4ActionCheckTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        private static readonly String CMP_FOLDER = SOURCE_FOLDER + "cmp/PdfA4ActionCheckTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA4ActionCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_LAUNCH_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => GeneratePdfADocument(PdfAConformance
                .PDF_A_4, null, (doc) => {
                doc.GetFirstPage().SetAdditionalAction(PdfName.O, PdfAction.CreateLaunch(new PdfStringFS("launch.sh")));
            }
            ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_SOUND_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4, null, (doc) => {
                    Stream @is = null;
                    try {
                        @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sample.aif");
                    }
                    catch (System.IO.IOException er) {
                        NUnit.Framework.Assert.Fail(er.Message);
                    }
                    PdfStream sound1 = new PdfStream(doc, @is);
                    sound1.Put(PdfName.R, new PdfNumber(32117));
                    sound1.Put(PdfName.E, PdfName.Signed);
                    sound1.Put(PdfName.B, new PdfNumber(16));
                    sound1.Put(PdfName.C, new PdfNumber(1));
                    doc.AddNewPage().SetAdditionalAction(PdfName.O, PdfAction.CreateSound(sound1));
                }
                );
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_MOVIE_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4, null, (doc) => {
                    doc.AddNewPage().SetAdditionalAction(PdfName.O, PdfAction.CreateMovie(null, "Some movie", PdfName.Play));
                }
                );
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_RESETFORM_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4, null, (doc) => {
                    CheckBox checkBox = new CheckBox("test");
                    checkBox.SetChecked(true);
                    Document document = new Document(doc);
                    document.Add(checkBox);
                    doc.AddNewPage().SetAdditionalAction(PdfName.O, PdfAction.CreateResetForm(new Object[] { "test" }, 0));
                }
                );
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_IMPORTDATA_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => GeneratePdfADocument(PdfAConformance
                .PDF_A_4, null, (doc) => {
                doc.AddNewPage();
                PdfDictionary openActions = new PdfDictionary();
                openActions.Put(PdfName.S, PdfName.ImportData);
                doc.AddNewPage().SetAdditionalAction(PdfName.O, new PdfAction(openActions));
            }
            ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_HIDE_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4, null, (doc) => {
                    PdfAnnotation[] annotations = new PdfAnnotation[] { new PdfLineAnnotation(new Rectangle(10, 10, 200, 200), 
                        new float[] { 50, 750, 50, 750 }), new PdfLineAnnotation(new Rectangle(200, 200, 200, 200), new float[
                        ] { 50, 750, 50, 750 }) };
                    doc.AddNewPage().SetAdditionalAction(PdfName.O, PdfAction.CreateHide(annotations, true));
                }
                );
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_RENDITION_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4, null, (doc) => {
                    doc.AddNewPage().SetAdditionalAction(PdfName.O, PdfAction.CreateRendition("empty", PdfFileSpec.CreateEmbeddedFileSpec
                        (doc, null, "bing", "bing", new PdfDictionary(), PdfName.AllOn), "something", new PdfCircleAnnotation(
                        new Rectangle(10, 10, 200, 200))));
                }
                );
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_TRANS_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => GeneratePdfADocument(PdfAConformance
                .PDF_A_4, null, (doc) => {
                PdfDictionary openActions = new PdfDictionary();
                openActions.Put(PdfName.S, PdfName.Trans);
                doc.AddNewPage().SetAdditionalAction(PdfName.O, new PdfAction(openActions));
            }
            ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_SETSTATE_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => GeneratePdfADocument(PdfAConformance
                .PDF_A_4, null, (doc) => {
                PdfDictionary action = new PdfDictionary();
                action.Put(PdfName.S, PdfName.SetState);
                doc.AddNewPage().SetAdditionalAction(PdfName.O, new PdfAction(action));
            }
            ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_NOOP_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => GeneratePdfADocument(PdfAConformance
                .PDF_A_4, null, (doc) => {
                PdfDictionary action = new PdfDictionary();
                action.Put(PdfName.S, PdfName.NoOp);
                doc.AddNewPage().SetAdditionalAction(PdfName.O, new PdfAction(action));
            }
            ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_SETOCGSTATE_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => GeneratePdfADocument(PdfAConformance
                .PDF_A_4, null, (doc) => {
                doc.AddNewPage().SetAdditionalAction(PdfName.O, PdfAction.CreateSetOcgState(new List<PdfActionOcgState>())
                    );
            }
            ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenActions_GOTO3DVIEW_ActionToPage_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => GeneratePdfADocument(PdfAConformance
                .PDF_A_4, null, (doc) => {
                PdfDictionary action = new PdfDictionary();
                action.Put(PdfName.S, PdfName.GoTo3DView);
                doc.AddNewPage().SetAdditionalAction(PdfName.O, new PdfAction(action));
            }
            ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4_SETOCGSTATE_InCatalog_Test() {
            Exception pdfa4Exception = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4, null, (doc) => {
                    doc.GetCatalog().SetAdditionalAction(PdfName.O, PdfAction.CreateSetOcgState(new List<PdfActionOcgState>())
                        );
                }
                );
            }
            );
            String messageFormat = MessageFormatUtil.Format(PdfaExceptionMessageConstant.CATALOG_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , PdfName.SetOCGState.GetValue());
            NUnit.Framework.Assert.AreEqual(messageFormat, pdfa4Exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4_SETOCGSTATE_Annotation_Test() {
            Exception pdfa4Exception = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4, null, (doc) => {
                    doc.AddNewPage().AddAnnotation(ConstructAnnotationWithAction(PdfName.SetOCGState));
                }
                );
            }
            );
            String messageFormat = MessageFormatUtil.Format(PdfaExceptionMessageConstant._0_ACTIONS_ARE_NOT_ALLOWED, PdfName
                .SetOCGState.GetValue());
            NUnit.Framework.Assert.AreEqual(messageFormat, pdfa4Exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4E_SETOCGSTATE_Annotation_Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4ESetOCGStateAnnotation.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4ESetOCGStateAnnotation.pdf";
            GeneratePdfADocument(PdfAConformance.PDF_A_4E, outPdf, (doc) => {
                doc.AddNewPage().AddAnnotation(ConstructAnnotationWithAction(PdfName.SetOCGState));
            }
            );
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4_SETGOTO3DVIEW_Annotation_Test() {
            Exception pdfa4Exception = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4, null, (doc) => {
                    doc.AddNewPage().AddAnnotation(ConstructAnnotationWithAction(PdfName.GoTo3DView));
                }
                );
            }
            );
            String messageFormat = MessageFormatUtil.Format(PdfaExceptionMessageConstant._0_ACTIONS_ARE_NOT_ALLOWED, PdfName
                .GoTo3DView.GetValue());
            NUnit.Framework.Assert.AreEqual(messageFormat, pdfa4Exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4E_GOTO3DVIEW_Annotation_Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4ESetGoto3DViewAnnotation.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4EGoto3DViewAnnotation.pdf";
            GeneratePdfADocument(PdfAConformance.PDF_A_4E, outPdf, (doc) => {
                doc.AddNewPage().AddAnnotation(ConstructAnnotationWithAction(PdfName.GoTo3DView));
            }
            );
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4_AllowedNamedActions_Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4AllowedNamedActions.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4AllowedNamedActions.pdf";
            IList<PdfName> annots = JavaUtil.ArraysAsList(PdfName.NextPage, PdfName.PrevPage, PdfName.FirstPage, PdfName
                .LastPage);
            GeneratePdfADocument(PdfAConformance.PDF_A_4, outPdf, (doc) => {
                PdfPage page = doc.GetFirstPage();
                foreach (PdfName annot in annots) {
                    PdfAnnotation annotation = ConstructAnnotationWithAction(new PdfName(""));
                    annotation.GetPdfObject().Put(PdfName.A, PdfAction.CreateNamed(annot).GetPdfObject());
                    page.AddAnnotation(annotation);
                }
            }
            );
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4_SpecialAllowedAction_Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4SpecialAllowedAction.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4SpecialAllowedAction.pdf";
            IList<PdfName> annots = JavaUtil.ArraysAsList(PdfName.GoToR, PdfName.GoToE, PdfName.URI, PdfName.SubmitForm
                );
            GeneratePdfADocument(PdfAConformance.PDF_A_4, outPdf, (doc) => {
                PdfPage page = doc.GetFirstPage();
                foreach (PdfName annot in annots) {
                    PdfAnnotation annotation = ConstructAnnotationWithAction(annot);
                    page.AddAnnotation(annotation);
                }
            }
            );
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4F_SETOCGSTATE_InCatalog_Test() {
            Exception pdfa4Exception = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4F, null, (doc) => {
                    doc.GetCatalog().SetAdditionalAction(PdfName.O, PdfAction.CreateSetOcgState(new List<PdfActionOcgState>())
                        );
                }
                );
            }
            );
            String messageFormat = MessageFormatUtil.Format(PdfaExceptionMessageConstant.CATALOG_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , PdfName.SetOCGState.GetValue());
            NUnit.Framework.Assert.AreEqual(messageFormat, pdfa4Exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4E_SETOCGSTATE_InCatalog_Test() {
            Exception pdfa4Exception = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4E, null, (doc) => {
                    doc.GetCatalog().SetAdditionalAction(PdfName.O, PdfAction.CreateSetOcgState(new List<PdfActionOcgState>())
                        );
                }
                );
            }
            );
            String messageFormat = MessageFormatUtil.Format(PdfaExceptionMessageConstant.CATALOG_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , PdfName.SetOCGState.GetValue());
            NUnit.Framework.Assert.AreEqual(messageFormat, pdfa4Exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4_GOTO3DVIEW_InCatalog_Test() {
            Exception pdfa4Exception = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4, null, (doc) => {
                    PdfDictionary action = new PdfDictionary();
                    action.Put(PdfName.S, PdfName.GoTo3DView);
                    doc.GetCatalog().SetAdditionalAction(PdfName.O, new PdfAction(action));
                }
                );
            }
            );
            String messageFormat = MessageFormatUtil.Format(PdfaExceptionMessageConstant.CATALOG_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , PdfName.GoTo3DView.GetValue());
            NUnit.Framework.Assert.AreEqual(messageFormat, pdfa4Exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4F_GOTO3DView_InCatalog_Test() {
            Exception pdfa4Exception = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                GeneratePdfADocument(PdfAConformance.PDF_A_4F, null, (doc) => {
                    PdfDictionary action = new PdfDictionary();
                    action.Put(PdfName.S, PdfName.GoTo3DView);
                    doc.GetCatalog().SetAdditionalAction(PdfName.O, new PdfAction(action));
                }
                );
            }
            );
            NUnit.Framework.Assert.AreEqual(pdfa4Exception.Message, PdfaExceptionMessageConstant.CATALOG_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                );
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4E_GOTO3DView_InCatalog_Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => GeneratePdfADocument(PdfAConformance
                .PDF_A_4E, null, (doc) => {
                PdfDictionary action = new PdfDictionary();
                action.Put(PdfName.S, PdfName.GoTo3DView);
                doc.GetCatalog().SetAdditionalAction(PdfName.O, new PdfAction(action));
            }
            ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.CATALOG_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4AAEntriesAllowedInAADocumentCatalog_Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4AAEntriesAllowedInAADocumentCatalog.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4AAEntriesAllowedInAADocumentCatalog.pdf";
            GeneratePdfADocument(PdfAConformance.PDF_A_4, outPdf, (doc) => {
                PdfDictionary allowedAA = new PdfDictionary();
                allowedAA.Put(PdfName.E, new PdfName("HELLO"));
                allowedAA.Put(PdfName.X, new PdfName("HELLO"));
                allowedAA.Put(PdfName.U, new PdfName("HELLO"));
                allowedAA.Put(PdfName.D, new PdfName("HELLO"));
                allowedAA.Put(PdfName.Fo, new PdfName("HELLO"));
                allowedAA.Put(PdfName.Bl, new PdfName("HELLO"));
                doc.GetCatalog().GetPdfObject().Put(PdfName.AA, allowedAA);
            }
            );
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4AAEntriesAllowedInAAPage_Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4AAEntriesAllowedInAAPage.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4AAEntriesAllowedInAAPage.pdf";
            GeneratePdfADocument(PdfAConformance.PDF_A_4, outPdf, (doc) => {
                PdfDictionary allowedAA = new PdfDictionary();
                allowedAA.Put(PdfName.E, new PdfName("HELLO"));
                allowedAA.Put(PdfName.X, new PdfName("HELLO"));
                allowedAA.Put(PdfName.U, new PdfName("HELLO"));
                allowedAA.Put(PdfName.D, new PdfName("HELLO"));
                allowedAA.Put(PdfName.Fo, new PdfName("HELLO"));
                allowedAA.Put(PdfName.Bl, new PdfName("HELLO"));
                doc.GetFirstPage().GetPdfObject().Put(PdfName.AA, allowedAA);
            }
            );
            CompareResult(outPdf, cmpPdf);
        }

        private PdfAnnotation ConstructAnnotationWithAction(PdfName actionType) {
            PdfAnnotation annotation = new PdfCircleAnnotation(new Rectangle(10, 10, 200, 200));
            PdfDictionary action = new PdfDictionary();
            annotation.SetFlag(PdfAnnotation.PRINT);
            action.Put(PdfName.Type, PdfName.Action);
            action.Put(PdfName.S, actionType);
            annotation.SetNormalAppearance(new PdfStream());
            annotation.GetPdfObject().Put(PdfName.A, action);
            return annotation;
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            NUnit.Framework.Assert.IsNull(
                        // Android-Conversion-Skip-Line TODO DEVSIX-7377
                        new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line TODO DEVSIX-7377
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        private void GeneratePdfADocument(PdfAConformance conformance, String outPdf, System.Action<PdfDocument> consumer
            ) {
            String filename = DESTINATION_FOLDER + Guid.NewGuid().ToString() + ".pdf";
            if (outPdf != null) {
                filename = outPdf;
            }
            PdfWriter writer = new PdfWriter(filename, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, conformance, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm"))
                );
            doc.AddNewPage();
            consumer(doc);
            doc.Close();
        }
    }
}
