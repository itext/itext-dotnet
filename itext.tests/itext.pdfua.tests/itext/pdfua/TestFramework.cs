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
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua.Exceptions;
using iText.Test.Pdfa;

namespace iText.Pdfua {
    /// <summary>Class that helps to test PDF/UA conformance.</summary>
    /// <remarks>
    /// Class that helps to test PDF/UA conformance.
    /// It creates two pdf documents, one with our checkers disabled to collect the veraPDf result,
    /// one with our checkers enabled to check for exceptions.
    /// It then compares if our checkers and veraPDF produce the same result.
    /// </remarks>
    public class TestFramework {
        private readonly String destinationFolder;

        private readonly IList<TestFramework.Generator<IBlockElement>> elementProducers = new List<TestFramework.Generator
            <IBlockElement>>();

        public TestFramework(String destinationFolder) {
            this.destinationFolder = destinationFolder;
        }

        public virtual void AddSuppliers(params TestFramework.Generator<IBlockElement>[] suppliers) {
            elementProducers.AddAll(suppliers);
        }

        public virtual void AssertBothFail(String filename) {
            Exception e = CheckErrorLayout("layout_" + filename + ".pdf");
            String veraPdf = VerAPdfResult("vera_" + filename + ".pdf");
            System.Console.Out.WriteLine(veraPdf);
            if (!(e is PdfUAConformanceException) && e != null) {
                System.Console.Out.WriteLine(PrintStackTrace(e));
                NUnit.Framework.Assert.Fail();
            }
            NUnit.Framework.Assert.IsNotNull(e);
            System.Console.Out.WriteLine(PrintStackTrace(e));
            NUnit.Framework.Assert.IsNotNull(veraPdf);
        }

        public virtual void AssertBothValid(String fileName) {
            Exception e = CheckErrorLayout("layout_" + fileName + ".pdf");
            String veraPdf = VerAPdfResult("vera_" + fileName + ".pdf");
            if (e == null && veraPdf == null) {
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
            if (counter != 2) {
                NUnit.Framework.Assert.Fail("One of the checks did not throw\n\n" + sb.ToString());
            }
            NUnit.Framework.Assert.Fail(sb.ToString());
        }

        public virtual String VerAPdfResult(String filename) {
            String outfile = UrlUtil.GetNormalizedFileUriString(destinationFolder + filename);
            System.Console.Out.WriteLine(outfile);
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(destinationFolder + filename, PdfUATestPdfDocument
                .CreateWriterProperties()));
            Document document = new Document(pdfDoc);
            document.GetPdfDocument().GetDiContainer().Register(typeof(ValidationContainer), new ValidationContainer()
                );
            foreach (TestFramework.Generator<IBlockElement> blockElementSupplier in elementProducers) {
                document.Add(blockElementSupplier.Generate());
            }
            document.Close();
            VeraPdfValidator validator = new VeraPdfValidator();
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377
            // introduce pdf/ua validation on Android)
            return validator.Validate(destinationFolder + filename);
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        public virtual Exception CheckErrorLayout(String filename) {
            try {
                String outfile = UrlUtil.GetNormalizedFileUriString(destinationFolder + filename);
                System.Console.Out.WriteLine(outfile);
                PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(destinationFolder + filename, PdfUATestPdfDocument
                    .CreateWriterProperties()));
                Document document = new Document(pdfDoc);
                foreach (TestFramework.Generator<IBlockElement> blockElementSupplier in elementProducers) {
                    document.Add(blockElementSupplier.Generate());
                }
                document.Close();
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
