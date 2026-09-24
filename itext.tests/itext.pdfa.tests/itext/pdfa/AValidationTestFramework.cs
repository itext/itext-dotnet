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
using System.IO;
using System.Text;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfa.Exceptions;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    public class AValidationTestFramework {
        private static readonly String ICC_PROFILE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/sRGB Color Space Profile.icm";

        private readonly bool defaultCheckDocClosingByReopening;

        private readonly String destinationFolder;

        private readonly PdfAConformance conformance;

        private readonly IList<Func<PdfDocument, IBlockElement>> elementProducers = new List<Func<PdfDocument, IBlockElement
            >>();

        private readonly IList<Action<PdfDocument>> beforeGeneratorHook = new List<Action<PdfDocument>>();

        private readonly IList<Action<PdfDocument>> afterGeneratorHook = new List<Action<PdfDocument>>();

        public AValidationTestFramework(String destinationFolder, bool defaultCheckDocClosingByReopening, PdfAConformance
             conformance) {
            if (conformance == null) {
                throw new ArgumentException("PDF/A conformance not specified");
            }
            this.destinationFolder = destinationFolder;
            this.defaultCheckDocClosingByReopening = defaultCheckDocClosingByReopening;
            this.conformance = conformance;
        }

        public void AddSuppliers(params Func<PdfDocument, IBlockElement>[] suppliers) {
            elementProducers.AddAll(suppliers);
        }

        public virtual void AddBeforeGenerationHook(Action<PdfDocument> action) {
            beforeGeneratorHook.Add(action);
        }

        public virtual void AddAfterGenerationHook(Action<PdfDocument> action) {
            afterGeneratorHook.Add(action);
        }

        public virtual void AssertBothFail(String filename, String expectedMsg) {
            CheckError(CheckErrorLayout(FileName("itext_", filename)), expectedMsg);
            String veraFileName = FileName("vera_", filename);
            VeraPdfResult(veraFileName, true);
            if (defaultCheckDocClosingByReopening) {
                CheckError(CheckErrorOnClosing(veraFileName), expectedMsg);
            }
        }

        public virtual void AssertBothValid(String filename) {
            Exception e = CheckErrorLayout(FileName("itext_", filename));
            String veraFileName = FileName("vera_", filename);
            String veraPdf = VeraPdfResult(veraFileName, false);
            Exception eClosing = defaultCheckDocClosingByReopening ? CheckErrorOnClosing(veraFileName) : null;
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

        public virtual void AssertVeraPdfFailITextValid(String filename) {
            VeraPdfResult(FileName("vera_", filename), true);
            Exception e = CheckErrorLayout(FileName("itext_", filename));
            NUnit.Framework.Assert.IsNull(e);
        }

        public virtual void AssertITextFailVeraPdfValid(String filename, String expectedMsg) {
            CheckError(CheckErrorLayout(FileName("itext_", filename)), expectedMsg);
            AssertVeraPdfValid(filename);
        }

        private void AssertVeraPdfValid(String filename) {
            NUnit.Framework.Assert.IsNull(VeraPdfResult(FileName("vera_", filename), false), "Expected no veraPDF validation errors"
                );
        }

        private PdfADocument CreatePdfDocument(String outputFile) {
            return CreatePdfDocument(null, outputFile);
        }

        private PdfADocument CreatePdfDocument(String inputFile, String outputFile) {
            if (inputFile != null) {
                return new PdfADocument(new PdfReader(inputFile), new PdfWriter(outputFile));
            }
            PdfOutputIntent outputIntent;
            using (Stream profile = FileUtil.GetInputStreamForFile(ICC_PROFILE)) {
                outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", profile);
            }
            return new PdfADocument(new PdfWriter(outputFile), conformance, outputIntent);
        }

        private String PathSafeConformance() {
            return "_A_" + conformance.GetPart() + (conformance.GetLevel() == null ? "" : conformance.GetLevel());
        }

        private String FileName(String prefix, String filename) {
            return prefix + filename + PathSafeConformance() + ".pdf";
        }

        private void GenerateDocument(String filename, bool disableValidation) {
            String outPath = destinationFolder + filename;
            System.Console.Out.WriteLine(UrlUtil.GetNormalizedFileUriString(outPath));
            using (PdfADocument pdfDoc = CreatePdfDocument(outPath)) {
                if (disableValidation) {
                    pdfDoc.GetDiContainer().Register(typeof(ValidationContainer), new ValidationContainer());
                }
                foreach (Action<PdfDocument> hook in beforeGeneratorHook) {
                    hook(pdfDoc);
                }
                using (Document document = new Document(pdfDoc)) {
                    foreach (Func<PdfDocument, IBlockElement> supplier in elementProducers) {
                        document.Add(supplier.Invoke(pdfDoc));
                    }
                    foreach (Action<PdfDocument> hook in afterGeneratorHook) {
                        hook(pdfDoc);
                    }
                }
            }
        }

        private String VeraPdfResult(String filename, bool failureExpected) {
            GenerateDocument(filename, true);
            VeraPdfValidator validator = new VeraPdfValidator();
            if (failureExpected) {
                validator.ValidateFailure(destinationFolder + filename);
                return null;
            }
            return validator.Validate(destinationFolder + filename);
        }

        private Exception CheckErrorLayout(String filename) {
            try {
                GenerateDocument(filename, false);
            }
            catch (Exception e) {
                return e;
            }
            return null;
        }

        private Exception CheckErrorOnClosing(String filename) {
            String outPath = destinationFolder + "reopen_" + filename;
            System.Console.Out.WriteLine(UrlUtil.GetNormalizedFileUriString(outPath));
            try {
                using (PdfADocument document = CreatePdfDocument(destinationFolder + filename, outPath)) {
                }
            }
            catch (Exception e) {
                // Closing validates the serialized document, without running generation hooks again.
                return e;
            }
            return null;
        }

        private static void CheckError(Exception e, String expectedMsg) {
            if (e == null) {
                NUnit.Framework.Assert.Fail("Expected exception but no exception was thrown");
            }
            if (!(e is PdfAConformanceException) && !(e is Pdf20ConformanceException)) {
                System.Console.Out.WriteLine(PrintStackTrace(e));
                NUnit.Framework.Assert.Fail("Expected exception of type PdfAConformanceException or Pdf20ConformanceException but was: "
                     + e.GetType().FullName);
            }
            if (expectedMsg != null) {
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
            System.Console.Out.WriteLine(PrintStackTrace(e));
        }

        private static String PrintStackTrace(Exception e) {
            return e.ToString();
        }
    }
}
