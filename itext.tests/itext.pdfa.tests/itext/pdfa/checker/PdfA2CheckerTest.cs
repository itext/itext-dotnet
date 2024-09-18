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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Function;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa.Checker {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfA2CheckerTest : ExtendedITextTest {
        private PdfA2Checker pdfA2Checker = new PdfA2Checker(PdfAConformance.PDF_A_2B);

        [NUnit.Framework.Test]
        public virtual void CheckNameEntryShouldBeUniqueBetweenDefaultAndAdditionalConfigs() {
            PdfDictionary ocProperties = new PdfDictionary();
            PdfDictionary d = new PdfDictionary();
            d.Put(PdfName.Name, new PdfString("CustomName"));
            PdfArray configs = new PdfArray();
            PdfDictionary config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName"));
            configs.Add(config);
            ocProperties.Put(PdfName.D, d);
            ocProperties.Put(PdfName.Configs, configs);
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.OCProperties, ocProperties);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.VALUE_OF_NAME_ENTRY_SHALL_BE_UNIQUE_AMONG_ALL_OPTIONAL_CONTENT_CONFIGURATION_DICTIONARIES
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckAsKeyInContentConfigDictTest() {
            PdfDictionary ocProperties = new PdfDictionary();
            PdfArray configs = new PdfArray();
            PdfDictionary config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName"));
            config.Put(PdfName.AS, new PdfArray());
            configs.Add(config);
            ocProperties.Put(PdfName.Configs, configs);
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.OCProperties, ocProperties);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_AS_KEY_SHALL_NOT_APPEAR_IN_ANY_OPTIONAL_CONTENT_CONFIGURATION_DICTIONARY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckNameEntryShouldBeUniqueBetweenAdditionalConfigs() {
            PdfDictionary ocProperties = new PdfDictionary();
            PdfDictionary d = new PdfDictionary();
            d.Put(PdfName.Name, new PdfString("CustomName"));
            PdfArray configs = new PdfArray();
            PdfDictionary config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName1"));
            configs.Add(config);
            config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName1"));
            configs.Add(config);
            ocProperties.Put(PdfName.D, d);
            ocProperties.Put(PdfName.Configs, configs);
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.OCProperties, ocProperties);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.VALUE_OF_NAME_ENTRY_SHALL_BE_UNIQUE_AMONG_ALL_OPTIONAL_CONTENT_CONFIGURATION_DICTIONARIES
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckOCCDContainName() {
            PdfDictionary ocProperties = new PdfDictionary();
            PdfDictionary d = new PdfDictionary();
            PdfArray configs = new PdfArray();
            PdfDictionary config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName1"));
            configs.Add(config);
            config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName2"));
            configs.Add(config);
            ocProperties.Put(PdfName.D, d);
            ocProperties.Put(PdfName.Configs, configs);
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.OCProperties, ocProperties);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.OPTIONAL_CONTENT_CONFIGURATION_DICTIONARY_SHALL_CONTAIN_NAME_ENTRY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckOrderArrayDoesNotContainRedundantReferences() {
            PdfDictionary ocProperties = new PdfDictionary();
            PdfDictionary d = new PdfDictionary();
            d.Put(PdfName.Name, new PdfString("CustomName"));
            PdfArray configs = new PdfArray();
            PdfDictionary config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName1"));
            PdfArray order = new PdfArray();
            PdfDictionary orderItem = new PdfDictionary();
            orderItem.Put(PdfName.Name, new PdfString("CustomName2"));
            order.Add(orderItem);
            PdfDictionary orderItem1 = new PdfDictionary();
            orderItem1.Put(PdfName.Name, new PdfString("CustomName3"));
            order.Add(orderItem1);
            config.Put(PdfName.Order, order);
            PdfArray ocgs = new PdfArray();
            ocgs.Add(orderItem);
            ocProperties.Put(PdfName.OCGs, ocgs);
            configs.Add(config);
            ocProperties.Put(PdfName.D, d);
            ocProperties.Put(PdfName.Configs, configs);
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.OCProperties, ocProperties);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ORDER_ARRAY_SHALL_CONTAIN_REFERENCES_TO_ALL_OCGS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckOrderArrayContainsReferencesToAllOCGs() {
            PdfDictionary ocProperties = new PdfDictionary();
            PdfDictionary d = new PdfDictionary();
            d.Put(PdfName.Name, new PdfString("CustomName"));
            PdfArray configs = new PdfArray();
            PdfDictionary config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName1"));
            PdfArray order = new PdfArray();
            PdfDictionary orderItem = new PdfDictionary();
            orderItem.Put(PdfName.Name, new PdfString("CustomName2"));
            order.Add(orderItem);
            PdfDictionary orderItem1 = new PdfDictionary();
            orderItem1.Put(PdfName.Name, new PdfString("CustomName3"));
            config.Put(PdfName.Order, order);
            PdfArray ocgs = new PdfArray();
            ocgs.Add(orderItem);
            ocgs.Add(orderItem1);
            ocProperties.Put(PdfName.OCGs, ocgs);
            configs.Add(config);
            ocProperties.Put(PdfName.D, d);
            ocProperties.Put(PdfName.Configs, configs);
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.OCProperties, ocProperties);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ORDER_ARRAY_SHALL_CONTAIN_REFERENCES_TO_ALL_OCGS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckOrderArrayAndOCGsMatch() {
            PdfDictionary ocProperties = new PdfDictionary();
            PdfDictionary d = new PdfDictionary();
            d.Put(PdfName.Name, new PdfString("CustomName"));
            PdfArray configs = new PdfArray();
            PdfDictionary config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName1"));
            PdfArray order = new PdfArray();
            PdfDictionary orderItem = new PdfDictionary();
            orderItem.Put(PdfName.Name, new PdfString("CustomName2"));
            order.Add(orderItem);
            PdfDictionary orderItem1 = new PdfDictionary();
            orderItem1.Put(PdfName.Name, new PdfString("CustomName3"));
            order.Add(orderItem1);
            config.Put(PdfName.Order, order);
            PdfArray ocgs = new PdfArray();
            PdfDictionary orderItem2 = new PdfDictionary();
            orderItem2.Put(PdfName.Name, new PdfString("CustomName4"));
            ocgs.Add(orderItem2);
            PdfDictionary orderItem3 = new PdfDictionary();
            orderItem3.Put(PdfName.Name, new PdfString("CustomName5"));
            ocgs.Add(orderItem3);
            ocProperties.Put(PdfName.OCGs, ocgs);
            configs.Add(config);
            ocProperties.Put(PdfName.D, d);
            ocProperties.Put(PdfName.Configs, configs);
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.OCProperties, ocProperties);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ORDER_ARRAY_SHALL_CONTAIN_REFERENCES_TO_ALL_OCGS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckAbsenceOfOptionalConfigEntryAllowed() {
            PdfDictionary ocProperties = new PdfDictionary();
            PdfDictionary d = new PdfDictionary();
            d.Put(PdfName.Name, new PdfString("CustomName"));
            PdfDictionary orderItem = new PdfDictionary();
            orderItem.Put(PdfName.Name, new PdfString("CustomName2"));
            PdfArray ocgs = new PdfArray();
            ocgs.Add(orderItem);
            ocProperties.Put(PdfName.OCGs, ocgs);
            ocProperties.Put(PdfName.D, d);
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.OCProperties, ocProperties);
            pdfA2Checker.CheckCatalogValidEntries(catalog);
        }

        // checkCatalogValidEntries doesn't change the state of any object
        // and doesn't return any value. The only result is exception which
        // was or wasn't thrown. Successful scenario is tested here therefore
        // no assertion is provided
        [NUnit.Framework.Test]
        public virtual void CheckAbsenceOfOptionalOrderEntryAllowed() {
            PdfDictionary ocProperties = new PdfDictionary();
            PdfDictionary d = new PdfDictionary();
            d.Put(PdfName.Name, new PdfString("CustomName"));
            PdfDictionary orderItem = new PdfDictionary();
            orderItem.Put(PdfName.Name, new PdfString("CustomName2"));
            PdfArray ocgs = new PdfArray();
            ocgs.Add(orderItem);
            PdfArray configs = new PdfArray();
            PdfDictionary config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName1"));
            configs.Add(config);
            ocProperties.Put(PdfName.OCGs, ocgs);
            ocProperties.Put(PdfName.D, d);
            ocProperties.Put(PdfName.Configs, configs);
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.OCProperties, ocProperties);
            pdfA2Checker.CheckCatalogValidEntries(catalog);
        }

        // checkCatalogValidEntries doesn't change the state of any object
        // and doesn't return any value. The only result is exception which
        // was or wasn't thrown. Successful scenario is tested here therefore
        // no assertion is provided
        [NUnit.Framework.Test]
        public virtual void CheckCatalogDictionaryWithoutAlternatePresentations() {
            PdfDictionary names = new PdfDictionary();
            names.Put(PdfName.AlternatePresentations, new PdfDictionary());
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.Names, names);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_ALTERNATEPRESENTATIONS_NAMES_ENTRY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCatalogDictionaryWithoutRequirements() {
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.Requirements, new PdfDictionary());
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_REQUIREMENTS_ENTRY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceNColorspaceNoAttributesDictionary() {
            int numberOfComponents = 2;
            IList<String> tmpArray = new List<String>(numberOfComponents);
            float[] transformArray = new float[numberOfComponents * 2];
            for (int i = 0; i < numberOfComponents; i++) {
                tmpArray.Add("MyColor" + i + 1);
                transformArray[i * 2] = 0;
                transformArray[i * 2 + 1] = 1;
            }
            PdfType4Function function = new PdfType4Function(transformArray, new float[] { 0, 1, 0, 1, 0, 1 }, "{0}".GetBytes
                (iText.Commons.Utils.EncodingUtil.ISO_8859_1));
            PdfDictionary currentColorSpaces = new PdfDictionary();
            //TODO DEVSIX-4203 should not cause an IndexOutOfBoundException.
            // Should throw PdfAConformanceException as Colorants dictionary always must be present
            // for Pdf/A-2
            NUnit.Framework.Assert.Catch(typeof(Exception), () => pdfA2Checker.CheckColorSpace(new PdfSpecialCs.DeviceN
                (tmpArray, new PdfDeviceCs.Rgb(), function), null, currentColorSpaces, true, false));
        }

        [NUnit.Framework.Test]
        public virtual void CheckColorShadingTest() {
            PdfDictionary patternDict = new PdfDictionary();
            patternDict.Put(PdfName.ExtGState, new PdfDictionary());
            PdfPattern.Shading pattern = new PdfPattern.Shading(patternDict);
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.ColorSpace, PdfName.DeviceCMYK);
            pattern.SetShading(dictionary);
            Color color = new PatternColor(pattern);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                pdfA2Checker.CheckColor(null, color, new PdfDictionary(), true, null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void DeprecatedCheckColorShadingTest() {
            PdfDictionary patternDict = new PdfDictionary();
            patternDict.Put(PdfName.ExtGState, new PdfDictionary());
            PdfPattern.Shading pattern = new PdfPattern.Shading(patternDict);
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.ColorSpace, PdfName.DeviceCMYK);
            pattern.SetShading(dictionary);
            Color color = new PatternColor(pattern);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                pdfA2Checker.CheckColor(null, color, new PdfDictionary(), true, null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void CheckColorShadingWithoutExtGStatePropertyInPatternDictTest() {
            PdfDictionary patternDict = new PdfDictionary();
            patternDict.Put(PdfName.PatternType, new PdfNumber(2));
            PdfPattern.Shading pattern = new PdfPattern.Shading(patternDict);
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.ColorSpace, PdfName.DeviceCMYK);
            pattern.SetShading(dictionary);
            Color color = new PatternColor(pattern);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                pdfA2Checker.CheckColor(new PdfA2CheckerTest.UpdateCanvasGraphicsState(new PdfDictionary()), color, new PdfDictionary
                    (), true, null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void CheckSignatureTest() {
            PdfDictionary signatureDict = CreateSignatureDict();
            pdfA2Checker.CheckSignature(signatureDict);
            NUnit.Framework.Assert.IsTrue(pdfA2Checker.ObjectIsChecked(signatureDict));
        }

        [NUnit.Framework.Test]
        public virtual void CheckSignatureDigestMethodTest() {
            PdfDictionary signatureDict = CreateSignatureDict();
            PdfArray types = (PdfArray)signatureDict.Get(PdfName.Reference);
            PdfDictionary reference = (PdfDictionary)types.Get(0);
            PdfArray digestMethod = new PdfArray();
            digestMethod.Add(new PdfName("SHA256"));
            reference.Put(PdfName.DigestMethod, digestMethod);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckSignature
                (signatureDict));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.SIGNATURE_REFERENCES_DICTIONARY_SHALL_NOT_CONTAIN_DIGESTLOCATION_DIGESTMETHOD_DIGESTVALUE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckLZWDecodeInInlineImage() {
            PdfStream stream = new PdfStream();
            stream.Put(PdfName.Filter, PdfName.LZWDecode);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckInlineImage
                (stream, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckImageWithAlternateTest() {
            PdfStream image = new PdfStream();
            image.Put(PdfName.Alternates, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckImage
                (image, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.AN_IMAGE_DICTIONARY_SHALL_NOT_CONTAIN_ALTERNATES_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckImageWithOPITest() {
            PdfStream image = new PdfStream();
            image.Put(PdfName.OPI, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckImage
                (image, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.AN_IMAGE_DICTIONARY_SHALL_NOT_CONTAIN_OPI_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckImageWithInterpolateTest() {
            PdfStream image = new PdfStream();
            image.Put(PdfName.Interpolate, new PdfBoolean(true));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckImage
                (image, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_VALUE_OF_INTERPOLATE_KEY_SHALL_BE_FALSE, 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFormXObjectWithOPITest() {
            PdfStream form = new PdfStream();
            form.Put(PdfName.OPI, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckFormXObject
                (form));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_OPI_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFormXObjectWithPSTest() {
            PdfStream form = new PdfStream();
            form.Put(PdfName.PS, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckFormXObject
                (form));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_PS_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCryptInInlineImage() {
            PdfStream stream = new PdfStream();
            stream.Put(PdfName.Filter, PdfName.Crypt);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckInlineImage
                (stream, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.CRYPT_FILTER_IS_NOT_PERMITTED_INLINE_IMAGE, e
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckLZWDecodeArrayInInlineImage() {
            PdfStream stream = new PdfStream();
            PdfArray array = new PdfArray();
            array.Add(PdfName.LZWDecode);
            stream.Put(PdfName.Filter, array);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckInlineImage
                (stream, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCryptArrayInInlineImage() {
            PdfStream stream = new PdfStream();
            PdfArray array = new PdfArray();
            array.Add(PdfName.Crypt);
            stream.Put(PdfName.Filter, array);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckInlineImage
                (stream, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.CRYPT_FILTER_IS_NOT_PERMITTED_INLINE_IMAGE, e
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckAllowedArrayFilterInInlineImage() {
            PdfStream stream = new PdfStream();
            PdfArray array = new PdfArray();
            array.Add(PdfName.Identity);
            stream.Put(PdfName.Filter, array);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckInlineImage
                (stream, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.INVALID_INLINE_IMAGE_FILTER_USAGE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckAllowedFilterInInlineImage() {
            PdfStream stream = new PdfStream();
            stream.Put(PdfName.Filter, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckInlineImage
                (stream, null));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.INVALID_INLINE_IMAGE_FILTER_USAGE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckEmptyAnnotationTest() {
            PdfDictionary annotation = new PdfDictionary();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckAnnotation
                (annotation));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , "null"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckAnnotationAgainstActionsWithATest() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.A, PdfName.Identity);
            annotation.Put(PdfName.Subtype, PdfName.Widget);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckAnnotationAgainstActions
                (annotation));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.WIDGET_ANNOTATION_DICTIONARY_OR_FIELD_DICTIONARY_SHALL_NOT_INCLUDE_A_OR_AA_ENTRY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckAnnotationAgainstActionsWithAATest() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.AA, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckAnnotationAgainstActions
                (annotation));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.AN_ANNOTATION_DICTIONARY_SHALL_NOT_CONTAIN_AA_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckNeedsRenderingCatalogTest() {
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.NeedsRendering, new PdfBoolean(true));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_THE_NEEDSRENDERING_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCatalogContainsAATest() {
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.AA, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_AA_ENTRY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCatalogContainsSignatureTest() {
            PdfDictionary catalog = new PdfDictionary();
            PdfDictionary perms = new PdfDictionary();
            PdfDictionary docMdp = new PdfDictionary();
            perms.Put(PdfName.DocMDP, docMdp);
            catalog.Put(PdfName.Perms, perms);
            pdfA2Checker.CheckCatalogValidEntries(catalog);
        }

        //nothing to check, expecting that no error is thrown
        [NUnit.Framework.Test]
        public virtual void CheckPageSizeTest() {
            PdfDictionary page = new PdfDictionary();
            PdfArray rect = new PdfArray();
            rect.Add(new PdfNumber(0));
            rect.Add(new PdfNumber(0));
            rect.Add(new PdfNumber(0));
            rect.Add(new PdfNumber(0));
            page.Put(PdfName.CropBox, rect);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckPageSize
                (page));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_PAGE_LESS_3_UNITS_NO_GREATER_14400_IN_EITHER_DIRECTION
                , e.Message);
        }

        //nothing to check, expecting that no error is thrown
        [NUnit.Framework.Test]
        public virtual void CheckOutputIntentsTest() {
            PdfDictionary catalog = new PdfDictionary();
            PdfArray array = new PdfArray();
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.DestOutputProfile, PdfName.Identity);
            PdfDictionary dictionary2 = new PdfDictionary();
            dictionary2.Put(PdfName.DestOutputProfile, PdfName.Crypt);
            array.Add(dictionary);
            array.Add(dictionary2);
            catalog.Put(PdfName.OutputIntents, array);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckOutputIntents
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.IF_OUTPUTINTENTS_ARRAY_HAS_MORE_THAN_ONE_ENTRY_WITH_DESTOUTPUTPROFILE_KEY_THE_SAME_INDIRECT_OBJECT_SHALL_BE_USED_AS_THE_VALUE_OF_THAT_OBJECT
                , e.Message);
        }

        //nothing to check, expecting that no error is thrown
        [NUnit.Framework.Test]
        public virtual void CheckCatalogContainsInvalidPermsTest() {
            PdfDictionary catalog = new PdfDictionary();
            PdfDictionary perms = new PdfDictionary();
            perms.Put(PdfName.Identity, PdfName.Identity);
            catalog.Put(PdfName.Perms, perms);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.NO_KEYS_OTHER_THAN_UR3_AND_DOC_MDP_SHALL_BE_PRESENT_IN_A_PERMISSIONS_DICTIONARY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFileSpecNotContainsFKeyTest() {
            PdfDictionary fileSpec = new PdfDictionary();
            fileSpec.Put(PdfName.EF, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckFileSpec
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
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckFileSpec
                (fileSpec));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EF_KEY_OF_FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_DICTIONARY_WITH_VALID_F_KEY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfStreamContainsFKeyTest() {
            PdfStream pdfStream = new PdfStream();
            pdfStream.Put(PdfName.F, PdfName.Identity);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckPdfStream
                (pdfStream));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.STREAM_OBJECT_DICTIONARY_SHALL_NOT_CONTAIN_THE_F_FFILTER_OR_FDECODEPARAMS_KEYS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfStreamContainsLZWDecodeKeyTest() {
            PdfStream pdfStream = new PdfStream();
            pdfStream.Put(PdfName.Filter, PdfName.LZWDecode);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckPdfStream
                (pdfStream));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfStreamContainsLZWDecodeArrayKeyTest() {
            PdfStream pdfStream = new PdfStream();
            PdfArray array = new PdfArray();
            array.Add(PdfName.LZWDecode);
            pdfStream.Put(PdfName.Filter, array);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckPdfStream
                (pdfStream));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfStreamContainsCryptKeyTest() {
            PdfStream pdfStream = new PdfStream();
            pdfStream.Put(PdfName.Filter, PdfName.Crypt);
            PdfDictionary decodeParams = new PdfDictionary();
            decodeParams.Put(PdfName.Name, PdfName.Crypt);
            pdfStream.Put(PdfName.DecodeParms, decodeParams);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckPdfStream
                (pdfStream));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.NOT_IDENTITY_CRYPT_FILTER_IS_NOT_PERMITTED, e
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfStreamContainsCryptArrayKeyTest() {
            PdfStream pdfStream = new PdfStream();
            PdfArray array = new PdfArray();
            array.Add(PdfName.Crypt);
            pdfStream.Put(PdfName.Filter, array);
            PdfDictionary decodeParams = new PdfDictionary();
            PdfArray decodeArray = new PdfArray();
            decodeArray.Add(decodeParams);
            decodeParams.Put(PdfName.Name, PdfName.Crypt);
            pdfStream.Put(PdfName.DecodeParms, decodeArray);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckPdfStream
                (pdfStream));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.NOT_IDENTITY_CRYPT_FILTER_IS_NOT_PERMITTED, e
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckColorSpaceWithDeviceNWithoutAttributes() {
            IList<String> tmpArray = new List<String>(3);
            float[] transformArray = new float[6];
            tmpArray.Add("Black");
            tmpArray.Add("Magenta");
            tmpArray.Add("White");
            for (int i = 0; i < 3; i++) {
                transformArray[i * 2] = 0;
                transformArray[i * 2 + 1] = 1;
            }
            PdfType4Function function = new PdfType4Function(transformArray, new float[] { 0, 1, 0, 1, 0, 1 }, "{0}".GetBytes
                (iText.Commons.Utils.EncodingUtil.ISO_8859_1));
            PdfArray deviceNAsArray = ((PdfArray)(new PdfSpecialCs.DeviceN(tmpArray, new PdfDeviceCs.Rgb(), function))
                .GetPdfObject());
            PdfDictionary currentColorSpaces = new PdfDictionary();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckColorSpace
                (new PdfSpecialCs.DeviceN(deviceNAsArray), null, currentColorSpaces, true, false));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.COLORANTS_DICTIONARY_SHALL_NOT_BE_EMPTY_IN_DEVICE_N_COLORSPACE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckColorSpaceWithDeviceNWithoutColorants() {
            IList<String> tmpArray = new List<String>(3);
            float[] transformArray = new float[6];
            tmpArray.Add("Black");
            tmpArray.Add("Magenta");
            tmpArray.Add("White");
            for (int i = 0; i < 3; i++) {
                transformArray[i * 2] = 0;
                transformArray[i * 2 + 1] = 1;
            }
            PdfType4Function function = new PdfType4Function(transformArray, new float[] { 0, 1, 0, 1, 0, 1 }, "{0}".GetBytes
                (iText.Commons.Utils.EncodingUtil.ISO_8859_1));
            PdfArray deviceNAsArray = ((PdfArray)(new PdfSpecialCs.DeviceN(tmpArray, new PdfDeviceCs.Rgb(), function))
                .GetPdfObject());
            PdfDictionary currentColorSpaces = new PdfDictionary();
            PdfDictionary attributes = new PdfDictionary();
            deviceNAsArray.Add(attributes);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckColorSpace
                (new PdfSpecialCs.DeviceN(deviceNAsArray), null, currentColorSpaces, true, false));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.COLORANTS_DICTIONARY_SHALL_NOT_BE_EMPTY_IN_DEVICE_N_COLORSPACE
                , e.Message);
        }

        private static PdfDictionary CreateSignatureDict() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfDictionary reference = new PdfDictionary();
            PdfDictionary transformParams = new PdfDictionary();
            transformParams.Put(PdfName.P, new PdfNumber(1));
            transformParams.Put(PdfName.V, new PdfName("1.2"));
            transformParams.Put(PdfName.Type, PdfName.TransformParams);
            reference.Put(PdfName.TransformMethod, PdfName.DocMDP);
            reference.Put(PdfName.Type, PdfName.SigRef);
            reference.Put(PdfName.TransformParams, transformParams);
            PdfArray types = new PdfArray();
            types.Add(reference);
            signatureDict.Put(PdfName.Reference, types);
            return signatureDict;
        }

        private sealed class UpdateCanvasGraphicsState : CanvasGraphicsState {
            public UpdateCanvasGraphicsState(PdfDictionary extGStateDict) {
                UpdateFromExtGState(new PdfExtGState(extGStateDict));
            }
        }
    }
}
