/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    internal sealed class AccessibilityPropertiesToStructElem {
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
}
