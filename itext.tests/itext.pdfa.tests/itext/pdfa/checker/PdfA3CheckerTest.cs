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
using iText.Kernel.Pdf;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa.Checker {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfA3CheckerTest : ExtendedITextTest {
        private PdfA1Checker pdfA3Checker = new PdfA3Checker(PdfAConformanceLevel.PDF_A_3B);

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            pdfA3Checker.SetFullCheckMode(true);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFileSpecNotContainsAFRelationshipKeyTest() {
            PdfDictionary fileSpec = new PdfDictionary();
            fileSpec.Put(PdfName.EF, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA3Checker.CheckFileSpec
                (fileSpec));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_ONE_OF_THE_PREDEFINED_AFRELATIONSHIP_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFileSpecNotContainsFKeyTest() {
            PdfDictionary fileSpec = new PdfDictionary();
            fileSpec.Put(PdfName.EF, PdfName.Identity);
            fileSpec.Put(PdfName.AFRelationship, PdfName.Data);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA3Checker.CheckFileSpec
                (fileSpec));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFileSpecContainsNullFKeyTest() {
            PdfDictionary fileSpec = new PdfDictionary();
            fileSpec.Put(PdfName.EF, new PdfDictionary());
            fileSpec.Put(PdfName.F, PdfName.Identity);
            fileSpec.Put(PdfName.UF, PdfName.Identity);
            fileSpec.Put(PdfName.Desc, PdfName.Identity);
            fileSpec.Put(PdfName.AFRelationship, PdfName.Data);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA3Checker.CheckFileSpec
                (fileSpec));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EF_KEY_OF_FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_DICTIONARY_WITH_VALID_F_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFileSpecContainsEmptyFKeyTest() {
            PdfDictionary fileSpec = new PdfDictionary();
            PdfDictionary ef = new PdfDictionary();
            ef.Put(PdfName.F, new PdfStream());
            fileSpec.Put(PdfName.EF, ef);
            fileSpec.Put(PdfName.F, new PdfDictionary());
            fileSpec.Put(PdfName.UF, PdfName.Identity);
            fileSpec.Put(PdfName.Desc, PdfName.Identity);
            fileSpec.Put(PdfName.AFRelationship, PdfName.Data);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA3Checker.CheckFileSpec
                (fileSpec));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.MIME_TYPE_SHALL_BE_SPECIFIED_USING_THE_SUBTYPE_KEY_OF_THE_FILE_SPECIFICATION_STREAM_DICTIONARY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFileSpecContainsFKeyWithParamsTest() {
            PdfDictionary fileSpec = new PdfDictionary();
            PdfDictionary ef = new PdfDictionary();
            PdfStream stream = new PdfStream();
            stream.Put(PdfName.Params, PdfName.Identity);
            stream.Put(PdfName.Subtype, PdfName.Identity);
            ef.Put(PdfName.F, stream);
            fileSpec.Put(PdfName.EF, ef);
            fileSpec.Put(PdfName.F, new PdfDictionary());
            fileSpec.Put(PdfName.UF, PdfName.Identity);
            fileSpec.Put(PdfName.Desc, PdfName.Identity);
            fileSpec.Put(PdfName.AFRelationship, PdfName.Data);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA3Checker.CheckFileSpec
                (fileSpec));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EMBEDDED_FILE_SHALL_CONTAIN_PARAMS_KEY_WITH_DICTIONARY_AS_VALUE
                , e.Message);
        }
    }
}
