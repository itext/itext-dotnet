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
using System.Collections.Generic;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa.Checker {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfA2CheckerTest : ExtendedITextTest {
        private PdfA2Checker pdfA2Checker = new PdfA2Checker(PdfAConformanceLevel.PDF_A_2B);

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
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.VALUE_OF_NAME_ENTRY_SHALL_BE_UNIQUE_AMONG_ALL_OPTIONAL_CONTENT_CONFIGURATION_DICTIONARIES
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
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.VALUE_OF_NAME_ENTRY_SHALL_BE_UNIQUE_AMONG_ALL_OPTIONAL_CONTENT_CONFIGURATION_DICTIONARIES
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
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.OPTIONAL_CONTENT_CONFIGURATION_DICTIONARY_SHALL_CONTAIN_NAME_ENTRY
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
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.ORDER_ARRAY_SHALL_CONTAIN_REFERENCES_TO_ALL_OCGS, 
                e.Message);
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
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.ORDER_ARRAY_SHALL_CONTAIN_REFERENCES_TO_ALL_OCGS, 
                e.Message);
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
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.ORDER_ARRAY_SHALL_CONTAIN_REFERENCES_TO_ALL_OCGS, 
                e.Message);
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
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_ALTERNATEPRESENTATIONS_NAMES_ENTRY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCatalogDictionaryWithoutRequirements() {
            PdfDictionary catalog = new PdfDictionary();
            catalog.Put(PdfName.Requirements, new PdfDictionary());
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckCatalogValidEntries
                (catalog));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_REQUIREMENTS_ENTRY
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
                (tmpArray, new PdfDeviceCs.Rgb(), function), currentColorSpaces, true, false));
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
                pdfA2Checker.CheckColor(color, new PdfDictionary(), true, null);
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
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.SIGNATURE_REFERENCES_DICTIONARY_SHALL_NOT_CONTAIN_DIGESTLOCATION_DIGESTMETHOD_DIGESTVALUE
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
    }
}
