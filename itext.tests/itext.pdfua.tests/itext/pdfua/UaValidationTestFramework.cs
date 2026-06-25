/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Text;
using iText.Commons.Internal.Runtime;
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
    public class UaValidationTestFramework {
        private readonly bool defaultCheckDocClosingByReopening;

        private readonly String destinationFolder;

        private readonly IList<Func<PdfDocument, IBlockElement>> elementProducers = new List<Func<PdfDocument, IBlockElement
            >>();

        private readonly IList<Action<PdfDocument>> beforeGeneratorHook = new List<Action<PdfDocument>>();

        private readonly IList<Action<PdfDocument>> afterGeneratorHook = new List<Action<PdfDocument>>();

        private readonly PdfConformance conformance;

        public UaValidationTestFramework(String destinationFolder, PdfConformance conformance)
            : this(destinationFolder, true, conformance) {
        }

        public UaValidationTestFramework(String destinationFolder, bool defaultCheckDocClosingByReopening, PdfConformance
             conformance) {
            this.destinationFolder = destinationFolder;
            this.defaultCheckDocClosingByReopening = defaultCheckDocClosingByReopening;
            this.conformance = conformance;
        }

        public static IList<PdfConformance> GetConformanceList(bool includeBelowPdf2Specification) {
            IList<PdfConformance> conformances = new List<PdfConformance>();
            if (includeBelowPdf2Specification) {
                conformances.Add(new PdfConformance(PdfUAConformance.PDF_UA_1));
            }
            conformances.Add(new PdfConformance(PdfUAConformance.PDF_UA_2));
            conformances.Add(new PdfConformance(WellTaggedPdfConformance.FOR_REUSE));
            conformances.Add(new PdfConformance(WellTaggedPdfConformance.FOR_ACCESSIBILITY));
            conformances.Add(new PdfConformance(JavaUtil.ArraysAsList(WellTaggedPdfConformance.FOR_REUSE, WellTaggedPdfConformance
                .FOR_ACCESSIBILITY)));
            return conformances;
        }

        public static IList<PdfConformance> GetConformanceList() {
            return GetConformanceList(true);
        }

        public void AddSuppliers(params Func<PdfDocument, IBlockElement>[] suppliers) {
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
            CheckError(CheckErrorLayout("itext_" + filename + PathSafeConformance() + ".pdf"), expectedMsg);
            String createdFileName = "vera_" + filename + PathSafeConformance() + ".pdf";
            VeraPdfResult(createdFileName, true);
            if (checkDocClosing) {
                System.Console.Out.WriteLine("Checking closing");
                CheckError(CheckErrorOnClosing(createdFileName), expectedMsg);
            }
        }

        public virtual void AssertBothValid(String fileName) {
            Exception e = CheckErrorLayout("itext_" + fileName + PathSafeConformance() + ".pdf");
            String veraPdf = VeraPdfResult("vera_" + fileName + PathSafeConformance() + ".pdf", false);
            Exception eClosing = CheckErrorOnClosing("vera_" + fileName + PathSafeConformance() + ".pdf");
            if (e == null && veraPdf == null && eClosing == null) {
                return;
            }
            int counter = 0;
            StringBuilder sb = new StringBuilder();
            if (e != null) {
                counter++;
                sb.Append("No exception expected but was: ").Append(e.GetType().FullName).Append(" \nMessage: \n").Append(
                    e.Message).Append('\n').Append("StackTrace:\n").Append(PrintStackTrace(e)).Append('\n');
            }
            if (veraPdf != null) {
                counter++;
                sb.Append("Expected no vera pdf message but was: \n").Append(veraPdf).Append('\n');
            }
            if (eClosing != null) {
                counter++;
                sb.Append("OnClosing no exception expected but was:\nStackTrace:\n").Append(PrintStackTrace(eClosing)).Append
                    (eClosing);
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
            VeraPdfResult("vera_" + filename + PathSafeConformance() + ".pdf", true);
            Exception e = CheckErrorLayout("itext_" + filename + PathSafeConformance() + ".pdf");
            NUnit.Framework.Assert.IsNull(e);
        }

        public virtual void AssertVeraPdfValid(String filename) {
            String veraPdf = VeraPdfResult("vera_" + filename + PathSafeConformance() + ".pdf", false);
            if (veraPdf == null) {
                return;
            }
            NUnit.Framework.Assert.Fail("Expected no vera pdf message but was: \n" + veraPdf + "\n");
        }

        public virtual void AssertOnlyITextFail(String filename, String expectedMsg) {
            CheckError(CheckErrorLayout("itext_" + filename + PathSafeConformance() + ".pdf"), expectedMsg);
            AssertVeraPdfValid(filename);
        }

        public virtual bool IsPdf2Based(PdfConformance conformance) {
            if (conformance.IsWtpdf()) {
                return true;
            }
            if (conformance.IsPdfUA() && conformance.GetUAConformance() == PdfUAConformance.PDF_UA_2) {
                return true;
            }
            return false;
        }

        public virtual PdfDocument CreatePdfDocument(String inputFileName, String outputFileName, String title, String
             language) {
            PdfWriter writer = new PdfWriter(outputFileName);
            writer.GetProperties().SetPdfVersion(IsPdf2Based(conformance) ? PdfVersion.PDF_2_0 : PdfVersion.PDF_1_7);
            PdfReader reader = inputFileName == null ? null : new PdfReader(inputFileName);
            if (reader != null) {
                if (conformance.IsPdfUA()) {
                    return new PdfUADocument(reader, writer, new PdfUAConfig(conformance.GetUAConformance(), title, language));
                }
                else {
                    if (conformance.IsWtpdf()) {
                        return new WellTaggedPdfDocument(reader, writer, new WellTaggedPdfConfig(conformance.GetWtpdfConformances(
                            ), title, language));
                    }
                    else {
                        throw new ArgumentException("PdfConformance not specified");
                    }
                }
            }
            else {
                if (conformance.IsPdfUA()) {
                    return new PdfUADocument(writer, new PdfUAConfig(conformance.GetUAConformance(), title, language));
                }
                else {
                    if (conformance.IsWtpdf()) {
                        return new WellTaggedPdfDocument(writer, new WellTaggedPdfConfig(conformance.GetWtpdfConformances(), title
                            , language));
                    }
                    else {
                        throw new ArgumentException("PdfConformance not specified");
                    }
                }
            }
        }

        public virtual PdfDocument CreatePdfDocument(String inputFile, String outputFile) {
            return CreatePdfDocument(inputFile, outputFile, "English pangram", "en-US");
        }

        public virtual PdfDocument CreatePdfDocument(String outputFile) {
            return CreatePdfDocument(null, outputFile, "English pangram", "en-US");
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
                foreach (Func<PdfDocument, IBlockElement> blockElementSupplier in elementProducers) {
                    document.Add(blockElementSupplier.Invoke(pdfDoc));
                }
                foreach (Action<PdfDocument> pdfDocumentConsumer in this.afterGeneratorHook) {
                    pdfDocumentConsumer(pdfDoc);
                }
            }
            VeraPdfValidator validator = new VeraPdfValidator();
            String validate = null;
            if (failureExpected) {
                validator.ValidateFailure(destinationFolder + filename);
            }
            else {
                validate = validator.Validate(destinationFolder + filename);
            }
            return validate;
        }

        private void CheckError(Exception e, String expectedMsg) {
            if (e == null) {
                NUnit.Framework.Assert.Fail("Expected exception but no exception was thrown");
            }
            if (!(e is PdfUAConformanceException) && !(e is Pdf20ConformanceException)) {
                System.Console.Out.WriteLine(PrintStackTrace(e));
                NUnit.Framework.Assert.Fail("Expected exception of type PdfUAConformanceException or Pdf20ConformanceException but was: "
                     + e.GetType().FullName);
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
                    foreach (Func<PdfDocument, IBlockElement> blockElementSupplier in elementProducers) {
                        document.Add(blockElementSupplier.Invoke(pdfDoc));
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

        public virtual String PathSafeConformance() {
            StringBuilder conformanceShortString = new StringBuilder();
            if (conformance.GetUAConformance() != null) {
                conformanceShortString.Append("_UA_").Append(conformance.GetUAConformance().GetPart());
            }
            if (conformance.IsWtpdf()) {
                conformanceShortString.Append("_WTPDF");
                if (conformance.ConformsTo(WellTaggedPdfConformance.FOR_ACCESSIBILITY)) {
                    conformanceShortString.Append("_A");
                }
                if (conformance.ConformsTo(WellTaggedPdfConformance.FOR_REUSE)) {
                    conformanceShortString.Append("_R");
                }
            }
            return conformanceShortString.ToString();
        }

        private static String PrintStackTrace(Exception e) {
            return e.ToString();
        }
    }
}
