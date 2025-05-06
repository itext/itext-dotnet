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
using System.Text;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua.Exceptions;
using iText.Test.Pdfa;

namespace iText.Pdfua {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    /// <summary>Class that helps to test PDF/UA conformance.</summary>
    /// <remarks>
    /// Class that helps to test PDF/UA conformance.
    /// It creates two pdf documents, one with our checkers disabled to collect the veraPDf result,
    /// one with our checkers enabled to check for exceptions.
    /// It then compares if our checkers and veraPDF produce the same result.
    /// </remarks>
    public class UaValidationTestFramework {
        private readonly bool defaultCheckDocClosingByReopening;

        private readonly String destinationFolder;

        private readonly IList<UaValidationTestFramework.Generator<IBlockElement>> elementProducers = new List<UaValidationTestFramework.Generator
            <IBlockElement>>();

        private readonly IList<Action<PdfDocument>> beforeGeneratorHook = new List<Action<PdfDocument>>();

        private readonly IList<Action<PdfDocument>> afterGeneratorHook = new List<Action<PdfDocument>>();

        public UaValidationTestFramework(String destinationFolder)
            : this(destinationFolder, true) {
        }

        public UaValidationTestFramework(String destinationFolder, bool defaultCheckDocClosingByReopening) {
            this.destinationFolder = destinationFolder;
            this.defaultCheckDocClosingByReopening = defaultCheckDocClosingByReopening;
        }

        public virtual void AddSuppliers(params UaValidationTestFramework.Generator<IBlockElement>[] suppliers) {
            elementProducers.AddAll(suppliers);
        }

        public virtual void AssertBothFail(String filename, PdfUAConformance pdfUAConformance) {
            AssertBothFail(filename, null, pdfUAConformance);
        }

        public virtual void AssertBothFail(String filename, bool checkDocClosing, PdfUAConformance pdfUAConformance
            ) {
            AssertBothFail(filename, null, checkDocClosing, pdfUAConformance);
        }

        public virtual void AssertBothFail(String filename, String expectedMsg, PdfUAConformance pdfUAConformance) {
            AssertBothFail(filename, expectedMsg, defaultCheckDocClosingByReopening, pdfUAConformance);
        }

        public virtual void AssertBothFail(String filename, String expectedMsg, bool checkDocClosing, PdfUAConformance
             pdfUAConformance) {
            CheckError(CheckErrorLayout("itext_" + filename + GetUAConformance(pdfUAConformance) + ".pdf", pdfUAConformance
                ), expectedMsg);
            String createdFileName = "vera_" + filename + GetUAConformance(pdfUAConformance) + ".pdf";
            VeraPdfResult(createdFileName, true, pdfUAConformance);
            if (checkDocClosing) {
                System.Console.Out.WriteLine("Checking closing");
                CheckError(CheckErrorOnClosing(createdFileName, pdfUAConformance), expectedMsg);
            }
        }

        public virtual void AssertITextValid(String fileName, PdfUAConformance pdfUAConformance) {
            Exception e = CheckErrorLayout("itext_" + fileName + GetUAConformance(pdfUAConformance) + ".pdf", pdfUAConformance
                );
            if (e == null) {
                return;
            }
            String sb = "No exception expected but was: " + e.GetType().FullName + " \n" + "Message: \n" + e.Message +
                 '\n' + "StackTrace:\n" + PrintStackTrace(e) + '\n';
            NUnit.Framework.Assert.Fail(sb);
        }

        public virtual void AssertBothValid(String fileName, PdfUAConformance pdfUAConformance) {
            Exception e = CheckErrorLayout("itext_" + fileName + GetUAConformance(pdfUAConformance) + ".pdf", pdfUAConformance
                );
            String veraPdf = VeraPdfResult("vera_" + fileName + GetUAConformance(pdfUAConformance) + ".pdf", false, pdfUAConformance
                );
            Exception eClosing = CheckErrorOnClosing("vera_" + fileName + GetUAConformance(pdfUAConformance) + ".pdf", 
                pdfUAConformance);
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

        public virtual void AssertVeraPdfFail(String filename, PdfUAConformance pdfUAConformance) {
            VeraPdfResult("vera_" + filename + GetUAConformance(pdfUAConformance) + ".pdf", true, pdfUAConformance);
        }

        public virtual void AssertOnlyVeraPdfFail(String filename, PdfUAConformance pdfUAConformance) {
            VeraPdfResult("vera_" + filename + GetUAConformance(pdfUAConformance) + ".pdf", true, pdfUAConformance);
            Exception e = CheckErrorLayout("itext_" + filename + GetUAConformance(pdfUAConformance) + ".pdf", pdfUAConformance
                );
            NUnit.Framework.Assert.IsNull(e);
        }

        public virtual void AssertVeraPdfValid(String filename, PdfUAConformance pdfUAConformance) {
            String veraPdf = VeraPdfResult("vera_" + filename + GetUAConformance(pdfUAConformance) + ".pdf", false, pdfUAConformance
                );
            if (veraPdf == null) {
                return;
            }
            NUnit.Framework.Assert.Fail("Expected no vera pdf message but was: \n" + veraPdf + "\n");
        }

        public virtual void AssertOnlyITextFail(String filename, String expectedMsg, PdfUAConformance pdfUAConformance
            ) {
            CheckError(CheckErrorLayout("itext_" + filename + GetUAConformance(pdfUAConformance) + ".pdf", pdfUAConformance
                ), expectedMsg);
            AssertVeraPdfValid(filename, pdfUAConformance);
        }

        private String VeraPdfResult(String filename, bool failureExpected, PdfUAConformance pdfUAConformance) {
            String outfile = UrlUtil.GetNormalizedFileUriString(destinationFolder + filename);
            System.Console.Out.WriteLine(outfile);
            PdfDocument pdfDoc = CreatePdfDocument(destinationFolder + filename, pdfUAConformance);
            pdfDoc.GetDiContainer().Register(typeof(ValidationContainer), new ValidationContainer());
            foreach (Action<PdfDocument> pdfDocumentConsumer in this.beforeGeneratorHook) {
                pdfDocumentConsumer(pdfDoc);
            }
            using (Document document = new Document(pdfDoc)) {
                foreach (UaValidationTestFramework.Generator<IBlockElement> blockElementSupplier in elementProducers) {
                    document.Add(blockElementSupplier.Generate());
                }
                foreach (Action<PdfDocument> pdfDocumentConsumer in this.afterGeneratorHook) {
                    pdfDocumentConsumer(pdfDoc);
                }
            }
            VeraPdfValidator validator = new VeraPdfValidator();
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

        private Exception CheckErrorLayout(String filename, PdfUAConformance pdfUAConformance) {
            try {
                String outPath = destinationFolder + filename;
                System.Console.Out.WriteLine(UrlUtil.GetNormalizedFileUriString(outPath));
                PdfDocument pdfDoc = CreatePdfDocument(outPath, pdfUAConformance);
                foreach (Action<PdfDocument> pdfDocumentConsumer in this.beforeGeneratorHook) {
                    pdfDocumentConsumer(pdfDoc);
                }
                using (Document document = new Document(pdfDoc)) {
                    foreach (UaValidationTestFramework.Generator<IBlockElement> blockElementSupplier in elementProducers) {
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

        private Exception CheckErrorOnClosing(String filename, PdfUAConformance pdfUAConformance) {
            try {
                String outPath = destinationFolder + "reopen_" + filename;
                String inPath = destinationFolder + filename;
                System.Console.Out.WriteLine(UrlUtil.GetNormalizedFileUriString(outPath));
                PdfDocument pdfDoc = CreatePdfDocument(inPath, outPath, pdfUAConformance);
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

        private static PdfDocument CreatePdfDocument(String filename, PdfUAConformance pdfUAConformance) {
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                return new PdfUATestPdfDocument(new PdfWriter(filename));
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    return new PdfUA2TestPdfDocument(new PdfWriter(filename, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                        )));
                }
                else {
                    throw new ArgumentException("Unsupported PdfUAConformance: " + pdfUAConformance);
                }
            }
        }

        private static PdfDocument CreatePdfDocument(String inputFile, String outputFile, PdfUAConformance pdfUAConformance
            ) {
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                return new PdfUATestPdfDocument(new PdfReader(inputFile), new PdfWriter(outputFile));
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    return new PdfUA2TestPdfDocument(new PdfReader(inputFile), new PdfWriter(outputFile, new WriterProperties(
                        ).SetPdfVersion(PdfVersion.PDF_2_0)));
                }
                else {
                    throw new ArgumentException("Unsupported PdfUAConformance: " + pdfUAConformance);
                }
            }
        }

        public interface Generator<IBlockElement> {
            IBlockElement Generate();
        }

        private static String GetUAConformance(PdfUAConformance conformance) {
            return MessageFormatUtil.Format("_UA_{0}", conformance.GetPart());
        }

        public static IList<PdfUAConformance> GetConformanceList() {
            return JavaUtil.ArraysAsList(PdfUAConformance.PDF_UA_1, PdfUAConformance.PDF_UA_2);
        }
    }
}
