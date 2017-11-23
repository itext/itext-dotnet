using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    internal sealed class AccessibilityPropertiesToStructElem {
        internal static void Apply(AccessibilityProperties properties, PdfStructElem elem) {
            if (properties.GetActualText() != null) {
                elem.SetActualText(new PdfString(properties.GetActualText(), PdfEncodings.UNICODE_BIG));
            }
            if (properties.GetAlternateDescription() != null) {
                elem.SetAlt(new PdfString(properties.GetAlternateDescription(), PdfEncodings.UNICODE_BIG));
            }
            if (properties.GetExpansion() != null) {
                elem.SetE(new PdfString(properties.GetExpansion(), PdfEncodings.UNICODE_BIG));
            }
            if (properties.GetLanguage() != null) {
                elem.SetLang(new PdfString(properties.GetLanguage(), PdfEncodings.UNICODE_BIG));
            }
            IList<PdfStructureAttributes> newAttributesList = properties.GetAttributesList();
            if (newAttributesList.Count > 0) {
                PdfObject attributesObject = elem.GetAttributes(false);
                PdfObject combinedAttributes = CombineAttributesList(attributesObject, -1, newAttributesList, elem.GetPdfObject
                    ().GetAsNumber(PdfName.R));
                elem.SetAttributes(combinedAttributes);
            }
            if (properties.GetPhoneme() != null) {
                elem.SetPhoneme(new PdfString(properties.GetPhoneme(), PdfEncodings.UNICODE_BIG));
            }
            if (properties.GetPhoneticAlphabet() != null) {
                elem.SetPhoneticAlphabet(new PdfName(properties.GetPhoneticAlphabet()));
            }
            if (properties.GetNamespace() != null) {
                elem.SetNamespace(properties.GetNamespace());
            }
            foreach (TagTreePointer @ref in properties.GetRefsList()) {
                elem.AddRef(@ref.GetCurrentStructElem());
            }
        }

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
}
