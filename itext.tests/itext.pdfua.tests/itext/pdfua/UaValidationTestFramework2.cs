using System;
using System.Collections.Generic;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua.Exceptions;
using iText.Pdfua.Wtpdf;
using iText.Test.Pdfa;

namespace iText.Pdfua {
    public class UaValidationTestFramework2 {
        private readonly bool defaultCheckDocClosingByReopening;

        private readonly String destinationFolder;

        private PdfUAConformance uaConformance = null;

        private WellTaggedPdfConformance? wtpdfConformance = null;

        private readonly IList<UaValidationTestFramework2.Generator<IBlockElement>> elementProducers = new List<UaValidationTestFramework2.Generator
            <IBlockElement>>();

        private readonly IList<Action<PdfDocument>> beforeGeneratorHook = new List<Action<PdfDocument>>();

        private readonly IList<Action<PdfDocument>> afterGeneratorHook = new List<Action<PdfDocument>>();

        public UaValidationTestFramework2(String destinationFolder, Object conformance)
            : this(destinationFolder, true, conformance) {
        }

        public UaValidationTestFramework2(String destinationFolder, bool defaultCheckDocClosingByReopening, Object
             conformance) {
            this.destinationFolder = destinationFolder;
            this.defaultCheckDocClosingByReopening = defaultCheckDocClosingByReopening;
            ParseConformance(conformance);
        }

        private void ParseConformance(Object conformance) {
            if (conformance is PdfUAConformance) {
                if (conformance == PdfUAConformance.PDF_UA_1) {
                    uaConformance = PdfUAConformance.PDF_UA_1;
                }
                else {
                    if (conformance == PdfUAConformance.PDF_UA_2) {
                        uaConformance = PdfUAConformance.PDF_UA_2;
                    }
                }
            }
            else {
                if (conformance is WellTaggedPdfConformance?) {
                    if (conformance == WellTaggedPdfConformance.FOR_ACCESSIBILITY) {
                        wtpdfConformance = WellTaggedPdfConformance.FOR_ACCESSIBILITY;
                    }
                    else {
                        if (conformance == WellTaggedPdfConformance.FOR_REUSE) {
                            wtpdfConformance = WellTaggedPdfConformance.FOR_REUSE;
                        }
                    }
                }
            }
        }

        public virtual void AddSuppliers(params UaValidationTestFramework2.Generator<IBlockElement>[] suppliers) {
            elementProducers.AddAll(suppliers);
        }

        public virtual void AssertBothFail(String filename) {
            AssertBothFail(filename, null);
        }

        public virtual void AssertBothFail(String filename, bool checkDocClosing) {
            AssertBothFail(filename, null, checkDocClosing);
        }

        public virtual void AssertBothFail(String filename, String expectedMsg) {
            AssertBothFail(filename, expectedMsg, defaultCheckDocClosingByReopening);
        }

        public virtual void AssertBothFail(String filename, String expectedMsg, bool checkDocClosing) {
            CheckError(CheckErrorLayout("itext_" + filename + ConformanceToString() + ".pdf"), expectedMsg);
            String createdFileName = "vera_" + filename + ConformanceToString() + ".pdf";
            VeraPdfResult(createdFileName, true);
            if (checkDocClosing) {
                System.Console.Out.WriteLine("Checking closing");
                CheckError(CheckErrorOnClosing(createdFileName), expectedMsg);
            }
        }

        public virtual void AssertBothValid(String fileName) {
            Exception e = CheckErrorLayout("itext_" + fileName + ConformanceToString() + ".pdf");
            String veraPdf = VeraPdfResult("vera_" + fileName + ConformanceToString() + ".pdf", false);
            Exception eClosing = CheckErrorOnClosing("vera_" + fileName + ConformanceToString() + ".pdf");
            if (e == null && veraPdf == null && eClosing == null) {
                return;
            }
            int counter = 0;
            StringBuilder sb = new StringBuilder();
            if (e != null) {
                counter++;
                sb.Append("No exception expected but was: ").Append(e.GetType().FullName).Append(" \n").Append("Message: \n"
                    ).Append(e.Message).Append('\n').Append("StackTrace:\n").Append(PrintStackTrace(e)).Append('\n');
            }
            if (veraPdf != null) {
                counter++;
                sb.Append("Expected no vera pdf message but was: \n").Append(veraPdf).Append("\n");
            }
            if (eClosing != null) {
                counter++;
                sb.Append("OnClosing no exception expected but was:\n").Append(eClosing);
            }
            if (counter != 3) {
                NUnit.Framework.Assert.Fail("One of the checks threw an exception\n\n" + sb.ToString());
            }
            NUnit.Framework.Assert.Fail(sb.ToString());
        }

        public virtual void AddBeforeGenerationHook(Action<PdfDocument> action) {
            this.beforeGeneratorHook.Add(action);
        }

        public virtual void AddAfterGenerationHook(Action<PdfDocument> action) {
            this.afterGeneratorHook.Add(action);
        }

        public virtual void AssertOnlyVeraPdfFail(String filename) {
            VeraPdfResult("vera_" + filename + ConformanceToString() + ".pdf", true);
            Exception e = CheckErrorLayout("itext_" + filename + ConformanceToString() + ".pdf");
            NUnit.Framework.Assert.IsNull(e);
        }

        public virtual void AssertVeraPdfValid(String filename) {
            String veraPdf = VeraPdfResult("vera_" + filename + ConformanceToString() + ".pdf", false);
            if (veraPdf == null) {
                return;
            }
            NUnit.Framework.Assert.Fail("Expected no vera pdf message but was: \n" + veraPdf + "\n");
        }

        public virtual void AssertOnlyITextFail(String filename, String expectedMsg) {
            CheckError(CheckErrorLayout("itext_" + filename + ConformanceToString() + ".pdf"), expectedMsg);
            AssertVeraPdfValid(filename);
        }

        // Android-Conversion-Skip-Block-Start (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        protected internal virtual VeraPdfValidator GetVerapdfValidator() {
            if (uaConformance != null) {
                return new VeraPdfValidator();
            }
            else {
                if (wtpdfConformance == WellTaggedPdfConformance.FOR_ACCESSIBILITY) {
                    return new VeraPdfValidator("WTPDF_ACCESSIBILITY");
                }
                else {
                    if (wtpdfConformance == WellTaggedPdfConformance.FOR_REUSE) {
                        return new VeraPdfValidator("WTPDF_REUSE");
                    }
                }
            }
            return null;
        }

        // Android-Conversion-Skip-Block-End
        protected internal virtual PdfDocument CreatePdfDocument(String filename) {
            if (uaConformance == PdfUAConformance.PDF_UA_1) {
                return new PdfUATestPdfDocument(new PdfWriter(filename));
            }
            else {
                if (uaConformance == PdfUAConformance.PDF_UA_2) {
                    return new PdfUA2TestPdfDocument(new PdfWriter(filename, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                        )));
                }
                else {
                    if (wtpdfConformance == WellTaggedPdfConformance.FOR_ACCESSIBILITY) {
                        return new WellTaggedPdfDocument(new PdfWriter(filename, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                            )), new WellTaggedPdfConfig(WellTaggedPdfConformance.FOR_ACCESSIBILITY, "English pangram", "en-US"));
                    }
                    else {
                        if (wtpdfConformance == WellTaggedPdfConformance.FOR_REUSE) {
                            return new WellTaggedPdfDocument(new PdfWriter(filename, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                                )), new WellTaggedPdfConfig(WellTaggedPdfConformance.FOR_REUSE, "English pangram", "en-US"));
                        }
                        else {
                            throw new ArgumentException("PdfConformance not specified");
                        }
                    }
                }
            }
        }

        protected internal virtual PdfDocument CreatePdfDocument(String inputFile, String outputFile) {
            if (uaConformance == PdfUAConformance.PDF_UA_1) {
                return new PdfUATestPdfDocument(new PdfReader(inputFile), new PdfWriter(outputFile));
            }
            else {
                if (uaConformance == PdfUAConformance.PDF_UA_2) {
                    return new PdfUA2TestPdfDocument(new PdfReader(inputFile), new PdfWriter(outputFile, new WriterProperties(
                        ).SetPdfVersion(PdfVersion.PDF_2_0)));
                }
                else {
                    if (wtpdfConformance == WellTaggedPdfConformance.FOR_ACCESSIBILITY) {
                        return new WellTaggedPdfDocument(new PdfReader(inputFile), new PdfWriter(outputFile, new WriterProperties(
                            ).SetPdfVersion(PdfVersion.PDF_2_0)), new WellTaggedPdfConfig(WellTaggedPdfConformance.FOR_ACCESSIBILITY
                            , "English pangram", "en-US"));
                    }
                    else {
                        if (wtpdfConformance == WellTaggedPdfConformance.FOR_REUSE) {
                            return new WellTaggedPdfDocument(new PdfReader(inputFile), new PdfWriter(outputFile, new WriterProperties(
                                ).SetPdfVersion(PdfVersion.PDF_2_0)), new WellTaggedPdfConfig(WellTaggedPdfConformance.FOR_REUSE, "English pangram"
                                , "en-US"));
                        }
                        else {
                            throw new ArgumentException("PdfConformance not specified");
                        }
                    }
                }
            }
        }

        private String VeraPdfResult(String filename, bool failureExpected) {
            String outfile = UrlUtil.GetNormalizedFileUriString(destinationFolder + filename);
            System.Console.Out.WriteLine(outfile);
            PdfDocument pdfDoc = CreatePdfDocument(destinationFolder + filename);
            pdfDoc.GetDiContainer().Register(typeof(ValidationContainer), new ValidationContainer());
            foreach (Action<PdfDocument> pdfDocumentConsumer in this.beforeGeneratorHook) {
                pdfDocumentConsumer(pdfDoc);
            }
            using (Document document = new Document(pdfDoc)) {
                foreach (UaValidationTestFramework2.Generator<IBlockElement> blockElementSupplier in elementProducers) {
                    document.Add(blockElementSupplier.Generate());
                }
                foreach (Action<PdfDocument> pdfDocumentConsumer in this.afterGeneratorHook) {
                    pdfDocumentConsumer(pdfDoc);
                }
            }
            VeraPdfValidator validator = GetVerapdfValidator();
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            String validate = null;
            if (failureExpected) {
                validator.ValidateFailure(destinationFolder + filename);
            }
            else {
                // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
                validate = validator.Validate(destinationFolder + filename);
            }
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            return validate;
        }

        private void CheckError(Exception e, String expectedMsg) {
            NUnit.Framework.Assert.IsNotNull(e);
            if (!(e is PdfUAConformanceException) && !(e is Pdf20ConformanceException)) {
                System.Console.Out.WriteLine(PrintStackTrace(e));
                NUnit.Framework.Assert.Fail();
            }
            if (expectedMsg != null) {
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
            System.Console.Out.WriteLine(PrintStackTrace(e));
        }

        private Exception CheckErrorLayout(String filename) {
            try {
                String outPath = destinationFolder + filename;
                System.Console.Out.WriteLine(UrlUtil.GetNormalizedFileUriString(outPath));
                PdfDocument pdfDoc = CreatePdfDocument(outPath);
                foreach (Action<PdfDocument> pdfDocumentConsumer in this.beforeGeneratorHook) {
                    pdfDocumentConsumer(pdfDoc);
                }
                using (Document document = new Document(pdfDoc)) {
                    foreach (UaValidationTestFramework2.Generator<IBlockElement> blockElementSupplier in elementProducers) {
                        document.Add(blockElementSupplier.Generate());
                    }
                    foreach (Action<PdfDocument> pdfDocumentConsumer in this.afterGeneratorHook) {
                        pdfDocumentConsumer(pdfDoc);
                    }
                }
            }
            catch (Exception e) {
                return e;
            }
            return null;
        }

        private Exception CheckErrorOnClosing(String filename) {
            try {
                String outPath = destinationFolder + "reopen_" + filename;
                String inPath = destinationFolder + filename;
                System.Console.Out.WriteLine(UrlUtil.GetNormalizedFileUriString(outPath));
                PdfDocument pdfDoc = CreatePdfDocument(inPath, outPath);
                pdfDoc.Close();
            }
            catch (Exception e) {
                return e;
            }
            return null;
        }

        private static String PrintStackTrace(Exception e) {
            return e.ToString();
        }

        private String ConformanceToString() {
            if (uaConformance != null) {
                return MessageFormatUtil.Format("_UA_{0}", uaConformance.GetPart());
            }
            else {
                if (wtpdfConformance == WellTaggedPdfConformance.FOR_ACCESSIBILITY) {
                    return "WTPDF_FOR_ACCESSIBILITY";
                }
                else {
                    if (wtpdfConformance == WellTaggedPdfConformance.FOR_REUSE) {
                        return "WTPDF_FOR_REUSE";
                    }
                }
            }
            return null;
        }

        public static IList<Object> GetConformanceList() {
            return JavaUtil.ArraysAsList(PdfUAConformance.PDF_UA_1, PdfUAConformance.PDF_UA_2, WellTaggedPdfConformance
                .FOR_REUSE);
        }

        public interface Generator<IBlockElement> {
            IBlockElement Generate();
        }
    }
}
