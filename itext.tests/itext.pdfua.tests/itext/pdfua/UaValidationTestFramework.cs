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
using System.Text;
using iText.Commons.Utils;
using iText.IO.Util;
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
            CheckError(CheckErrorLayout("layout_" + filename + ".pdf"), expectedMsg);
            String createdFileName = "vera_" + filename + ".pdf";
            String veraPdf = VerAPdfResult(createdFileName);
            System.Console.Out.WriteLine(veraPdf);
            NUnit.Framework.Assert.IsNotNull(veraPdf);
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            if (checkDocClosing) {
                System.Console.Out.WriteLine("Checking closing");
                CheckError(CheckErrorOnClosing(createdFileName), expectedMsg);
            }
        }

        public virtual void AssertBothValid(String fileName) {
            Exception e = CheckErrorLayout("layout_" + fileName + ".pdf");
            String veraPdf = VerAPdfResult("vera_" + fileName + ".pdf");
            Exception eClosing = CheckErrorOnClosing("vera_" + fileName + ".pdf");
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
                sb.Append("OnClosing no expection expected but was:\n").Append(eClosing);
            }
            if (counter != 3) {
                NUnit.Framework.Assert.Fail("One of the checks did not throw\n\n" + sb.ToString());
            }
            NUnit.Framework.Assert.Fail(sb.ToString());
        }

        public virtual String VerAPdfResult(String filename) {
            String outfile = UrlUtil.GetNormalizedFileUriString(destinationFolder + filename);
            System.Console.Out.WriteLine(outfile);
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(destinationFolder + filename));
            Document document = new Document(pdfDoc);
            document.GetPdfDocument().GetDiContainer().Register(typeof(ValidationContainer), new ValidationContainer()
                );
            foreach (Action<PdfDocument> pdfDocumentConsumer in this.beforeGeneratorHook) {
                pdfDocumentConsumer(pdfDoc);
            }
            foreach (UaValidationTestFramework.Generator<IBlockElement> blockElementSupplier in elementProducers) {
                document.Add(blockElementSupplier.Generate());
            }
            document.Close();
            VeraPdfValidator validator = new VeraPdfValidator();
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            String validate = null;
            validate = validator.Validate(destinationFolder + filename);
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            return validate;
        }

        public virtual void AddBeforeGenerationHook(Action<PdfDocument> action) {
            this.beforeGeneratorHook.Add(action);
        }

        private void CheckError(Exception e, String expectedMsg) {
            NUnit.Framework.Assert.IsNotNull(e);
            if (!(e is PdfUAConformanceException)) {
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
                PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPath));
                foreach (Action<PdfDocument> pdfDocumentConsumer in this.beforeGeneratorHook) {
                    pdfDocumentConsumer(pdfDoc);
                }
                Document document = new Document(pdfDoc);
                foreach (UaValidationTestFramework.Generator<IBlockElement> blockElementSupplier in elementProducers) {
                    document.Add(blockElementSupplier.Generate());
                }
                document.Close();
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
                PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfReader(inPath), new PdfWriter(outPath));
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

        public interface Generator<IBlockElement> {
            IBlockElement Generate();
        }
    }
}
