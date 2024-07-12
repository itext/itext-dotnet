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
using iText.IO.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
//\cond DO_NOT_DOCUMENT
    internal sealed class AccessibilityPropertiesToStructElem {
//\cond DO_NOT_DOCUMENT
        internal static void Apply(AccessibilityProperties properties, PdfStructElem elem) {
            SetTextualAids(properties, elem);
            SetAttributes(properties.GetAttributesList(), elem);
            if (properties.GetNamespace() != null) {
                elem.SetNamespace(properties.GetNamespace());
            }
            if (properties.GetStructureElementId() != null) {
                elem.SetStructureElementId(new PdfString(properties.GetStructureElementId()));
            }
            foreach (TagTreePointer @ref in properties.GetRefsList()) {
                elem.AddRef(@ref.GetCurrentStructElem());
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static PdfObject CombineAttributesList(PdfObject attributesObject, int insertIndex, IList<PdfStructureAttributes
            > newAttributesList, PdfNumber revision) {
            PdfObject combinedAttributes;
            if (attributesObject is PdfDictionary) {
                PdfArray combinedAttributesArray = new PdfArray();
                combinedAttributesArray.Add(attributesObject);
                AddNewAttributesToAttributesArray(insertIndex, newAttributesList, revision, combinedAttributesArray);
                combinedAttributes = combinedAttributesArray;
            }
            else {
                if (attributesObject is PdfArray) {
                    PdfArray combinedAttributesArray = (PdfArray)attributesObject;
                    AddNewAttributesToAttributesArray(insertIndex, newAttributesList, revision, combinedAttributesArray);
                    combinedAttributes = combinedAttributesArray;
                }
                else {
                    if (newAttributesList.Count == 1) {
                        if (insertIndex > 0) {
                            throw new IndexOutOfRangeException();
                        }
                        combinedAttributes = newAttributesList[0].GetPdfObject();
                    }
                    else {
                        combinedAttributes = new PdfArray();
                        AddNewAttributesToAttributesArray(insertIndex, newAttributesList, revision, (PdfArray)combinedAttributes);
                    }
                }
            }
            return combinedAttributes;
        }
//\endcond

        private static void SetAttributes(IList<PdfStructureAttributes> newAttributesList, PdfStructElem elem) {
            if (newAttributesList.Count > 0) {
                PdfObject attributesObject = elem.GetAttributes(false);
                PdfObject combinedAttributes = CombineAttributesList(attributesObject, -1, newAttributesList, elem.GetPdfObject
                    ().GetAsNumber(PdfName.R));
                elem.SetAttributes(combinedAttributes);
            }
        }

        private static void SetTextualAids(AccessibilityProperties properties, PdfStructElem elem) {
            if (properties.GetLanguage() != null) {
                elem.SetLang(new PdfString(properties.GetLanguage(), PdfEncodings.UNICODE_BIG));
            }
            if (properties.GetActualText() != null) {
                elem.SetActualText(new PdfString(properties.GetActualText(), PdfEncodings.UNICODE_BIG));
            }
            if (properties.GetAlternateDescription() != null) {
                elem.SetAlt(new PdfString(properties.GetAlternateDescription(), PdfEncodings.UNICODE_BIG));
            }
            if (properties.GetExpansion() != null) {
                elem.SetE(new PdfString(properties.GetExpansion(), PdfEncodings.UNICODE_BIG));
            }
            if (properties.GetPhoneme() != null) {
                elem.SetPhoneme(new PdfString(properties.GetPhoneme(), PdfEncodings.UNICODE_BIG));
            }
            if (properties.GetPhoneticAlphabet() != null) {
                elem.SetPhoneticAlphabet(new PdfName(properties.GetPhoneticAlphabet()));
            }
        }

        private static void AddNewAttributesToAttributesArray(int insertIndex, IList<PdfStructureAttributes> newAttributesList
            , PdfNumber revision, PdfArray attributesArray) {
            if (insertIndex < 0) {
                insertIndex = attributesArray.Size();
            }
            if (revision != null) {
                foreach (PdfStructureAttributes attributes in newAttributesList) {
                    attributesArray.Add(insertIndex++, attributes.GetPdfObject());
                    attributesArray.Add(insertIndex++, revision);
                }
            }
            else {
                foreach (PdfStructureAttributes newAttribute in newAttributesList) {
                    attributesArray.Add(insertIndex++, newAttribute.GetPdfObject());
                }
            }
        }
    }
//\endcond
}
