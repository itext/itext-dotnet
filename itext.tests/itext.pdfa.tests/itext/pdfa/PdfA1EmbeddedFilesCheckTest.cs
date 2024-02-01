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
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA1EmbeddedFilesCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest01() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            PdfDictionary fileNames = new PdfDictionary();
            pdfDocument.GetCatalog().Put(PdfName.Names, fileNames);
            PdfDictionary embeddedFiles = new PdfDictionary();
            fileNames.Put(PdfName.EmbeddedFiles, embeddedFiles);
            PdfArray names = new PdfArray();
            fileNames.Put(PdfName.Names, names);
            names.Add(new PdfString("some/file/path"));
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, sourceFolder + "sample.wav", "sample.wav"
                , "sample", null, null);
            names.Add(spec.GetPdfObject());
            pdfDocument.AddNewPage();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_NAME_DICTIONARY_SHALL_NOT_CONTAIN_THE_EMBEDDED_FILES_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest02() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            PdfStream stream = new PdfStream();
            pdfDocument.GetCatalog().Put(new PdfName("testStream"), stream);
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, sourceFolder + "sample.wav", "sample.wav"
                , "sample", null, null);
            stream.Put(PdfName.F, spec.GetPdfObject());
            pdfDocument.AddNewPage();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.STREAM_OBJECT_DICTIONARY_SHALL_NOT_CONTAIN_THE_F_FFILTER_OR_FDECODEPARAMS_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest03() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent);
            PdfStream stream = new PdfStream();
            pdfDocument.GetCatalog().Put(new PdfName("testStream"), stream);
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, sourceFolder + "sample.wav", "sample.wav"
                , "sample", null, null);
            stream.Put(new PdfName("fileData"), spec.GetPdfObject());
            pdfDocument.AddNewPage();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_NOT_CONTAIN_THE_EF_KEY
                , e.Message);
        }
    }
}
