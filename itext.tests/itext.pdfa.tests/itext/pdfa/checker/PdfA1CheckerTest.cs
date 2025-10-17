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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa.Checker {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfA1CheckerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CheckCatalogDictionaryWithoutAAEntry() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.AA, new PdfDictionary());
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_AA_ENTRY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCatalogDictionaryWithoutOCPropertiesEntry() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.OCProperties, new PdfDictionary());
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_OCPROPERTIES_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCatalogDictionaryWithoutEmbeddedFiles() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfDictionary names = new PdfDictionary();
            names.Put(PdfName.EmbeddedFiles, new PdfDictionary());
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.Names, names);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_NAME_DICTIONARY_SHALL_NOT_CONTAIN_THE_EMBEDDED_FILES_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckValidCatalog() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            pdfA1Checker.CheckCatalogValidEntries(new PdfDictionary());
        }

        // checkCatalogValidEntries doesn't change the state of any object
        // and doesn't return any value. The only result is exception which
        // was or wasn't thrown. Successful scenario is tested here therefore
        // no assertion is provided
        [NUnit.Framework.Test]
        public virtual void DeprecatedCheckColorShadingTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfDictionary patternDict = new PdfDictionary();
            patternDict.Put(PdfName.ExtGState, new PdfDictionary());
            PdfPattern.Shading pattern = new PdfPattern.Shading(patternDict);
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.ColorSpace, PdfName.DeviceCMYK);
            pattern.SetShading(dictionary);
            Color color = new PatternColor(pattern);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                pdfA1Checker.CheckColor(null, color, new PdfDictionary(), true, null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void CheckSignatureTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfDictionary dict = new PdfDictionary();
            pdfA1Checker.CheckSignature(dict);
            NUnit.Framework.Assert.IsTrue(pdfA1Checker.IsPdfObjectReadyToFlush(dict));
        }

        [NUnit.Framework.Test]
        public virtual void CheckSignatureTypeTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            pdfA1Checker.CheckSignatureType(true);
        }

        //nothing to check, only for coverage
        [NUnit.Framework.Test]
        public virtual void CheckLZWDecodeInInlineImage() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream stream = new PdfStream();
            stream.Put(PdfName.Filter, PdfName.LZWDecode);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckInlineImage
                (stream, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckLZWDecodeArrayInInlineImage() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream stream = new PdfStream();
            PdfArray array = new PdfArray();
            array.Add(PdfName.LZWDecode);
            stream.Put(PdfName.Filter, array);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckInlineImage
                (stream, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckEmptyImageTwiceTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream image = new PdfStream();
            pdfA1Checker.CheckImage(image, null);
            pdfA1Checker.CheckImage(image, null);
        }

        //nothing to check, expecting that no error is thrown
        [NUnit.Framework.Test]
        public virtual void CheckImageWithAlternateTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream image = new PdfStream();
            image.Put(PdfName.Alternates, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckImage
                (image, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.AN_IMAGE_DICTIONARY_SHALL_NOT_CONTAIN_ALTERNATES_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckImageWithOPITest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream image = new PdfStream();
            image.Put(PdfName.OPI, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckImage
                (image, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.AN_IMAGE_DICTIONARY_SHALL_NOT_CONTAIN_OPI_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckImageWithInterpolateTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream image = new PdfStream();
            image.Put(PdfName.Interpolate, new PdfBoolean(true));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckImage
                (image, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_VALUE_OF_INTERPOLATE_KEY_SHALL_BE_FALSE, 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckImageWithSMaskTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream image = new PdfStream();
            image.Put(PdfName.SMask, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckImage
                (image, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_SMASK_KEY_IS_NOT_ALLOWED_IN_XOBJECTS, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CheckFormXObjectWithOPITest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream form = new PdfStream();
            form.Put(PdfName.OPI, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckFormXObject
                (form));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_OPI_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFormXObjectWithPSTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream form = new PdfStream();
            form.Put(PdfName.PS, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckFormXObject
                (form));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_PS_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFormXObjectWithSubtype2PSTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream form = new PdfStream();
            form.Put(PdfName.Subtype2, PdfName.PS);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckFormXObject
                (form));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_SUBTYPE2_KEY_WITH_A_VALUE_OF_PS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFormXObjectWithSMaskTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream form = new PdfStream();
            form.Put(PdfName.SMask, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckFormXObject
                (form));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_SMASK_KEY_IS_NOT_ALLOWED_IN_XOBJECTS, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CheckCatalogContainsMetadataTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfDictionary catalog = new PdfDictionary();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckMetaData
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_CATALOG_DICTIONARY_SHALL_CONTAIN_METADATA_ENTRY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckOutputIntentsTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfDictionary catalog = new PdfDictionary();
            PdfArray array = new PdfArray();
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.DestOutputProfile, PdfName.Identity);
            PdfDictionary dictionary2 = new PdfDictionary();
            dictionary2.Put(PdfName.DestOutputProfile, PdfName.Crypt);
            array.Add(dictionary);
            array.Add(dictionary2);
            catalog.Put(PdfName.OutputIntents, array);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckOutputIntents
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.IF_OUTPUTINTENTS_ARRAY_HAS_MORE_THAN_ONE_ENTRY_WITH_DESTOUTPUTPROFILE_KEY_THE_SAME_INDIRECT_OBJECT_SHALL_BE_USED_AS_THE_VALUE_OF_THAT_OBJECT
                , e.Message);
        }

        //nothing to check, expecting that no error is thrown
        [NUnit.Framework.Test]
        public virtual void CheckLZWDecodeInPdfStreamTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream stream = new PdfStream();
            stream.Put(PdfName.Filter, PdfName.LZWDecode);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckPdfStream
                (stream));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckLZWDecodeInPdfStreamArrayTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfStream stream = new PdfStream();
            PdfArray array = new PdfArray();
            array.Add(PdfName.LZWDecode);
            stream.Put(PdfName.Filter, array);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckPdfStream
                (stream));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFileSpecTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            pdfA1Checker.CheckFileSpec(new PdfDictionary());
        }

        //nothing to check, only for coverage
        [NUnit.Framework.Test]
        public virtual void CheckEmptyAnnotationTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfDictionary annotation = new PdfDictionary();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckAnnotation
                (annotation));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , "null"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckAnnotationWithoutFKeyTest() {
            PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformance.PDF_A_1B);
            pdfA1Checker.SetFullCheckMode(true);
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckAnnotation
                (annotation));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.AN_ANNOTATION_DICTIONARY_SHALL_CONTAIN_THE_F_KEY
                , "null"), e.Message);
        }
    }
}
