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
using System.Collections.Generic;
using System.Linq;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.Kernel.Pdf;

namespace iText.Signatures.Validation {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Utility class for comparing PDF objects, used in
    /// <see cref="DocumentRevisionsValidator"/>.
    /// </summary>
    internal sealed class DocumentRevisionPdfObjectComparator {
        private DocumentRevisionPdfObjectComparator() {
        }

        //Empty constructor used to avoid instantiation of utility class
        public static bool IsSameReference(PdfIndirectReference indirectReference1, PdfIndirectReference indirectReference2
            ) {
            if (indirectReference1 == null || indirectReference2 == null) {
                return false;
            }
            return indirectReference1.GetObjNumber() == indirectReference2.GetObjNumber() && indirectReference1.GetGenNumber
                () == indirectReference2.GetGenNumber();
        }

        public static bool ComparePdfObjects(PdfObject pdfObject1, PdfObject pdfObject2, Tuple2<ICollection<PdfIndirectReference
            >, ICollection<PdfIndirectReference>> usuallyModifiedObjects) {
            return ComparePdfObjects(pdfObject1, pdfObject2, new List<Tuple2<PdfObject, PdfObject>>(), usuallyModifiedObjects
                );
        }

        public static bool IsMaxGenerationObject(PdfIndirectReference indirectReference) {
            return indirectReference.GetObjNumber() == 0 && indirectReference.GetGenNumber() == PdfXrefTable.MAX_GENERATION;
        }

        public static bool ComparePdfDictionaries(PdfDictionary dictionary1, PdfDictionary dictionary2, IList<Tuple2
            <PdfObject, PdfObject>> visitedObjects, Tuple2<ICollection<PdfIndirectReference>, ICollection<PdfIndirectReference
            >> usuallyModifiedObjects) {
            ICollection<KeyValuePair<PdfName, PdfObject>> entrySet1 = dictionary1.EntrySet();
            ICollection<KeyValuePair<PdfName, PdfObject>> entrySet2 = dictionary2.EntrySet();
            if (entrySet1.Count != entrySet2.Count) {
                return false;
            }
            foreach (KeyValuePair<PdfName, PdfObject> entry1 in entrySet1) {
                if (!entrySet2.Any((entry2) => entry2.Key.Equals(entry1.Key) && ComparePdfObjects(entry2.Value, entry1.Value
                    , visitedObjects, usuallyModifiedObjects))) {
                    return false;
                }
            }
            return true;
        }

        private static bool ComparePdfObjects(PdfObject pdfObject1, PdfObject pdfObject2, IList<Tuple2<PdfObject, 
            PdfObject>> visitedObjects, Tuple2<ICollection<PdfIndirectReference>, ICollection<PdfIndirectReference
            >> usuallyModifiedObjects) {
            foreach (Tuple2<PdfObject, PdfObject> pair in visitedObjects) {
                if (pair.GetFirst() == pdfObject1) {
                    return pair.GetSecond() == pdfObject2;
                }
            }
            visitedObjects.Add(new Tuple2<PdfObject, PdfObject>(pdfObject1, pdfObject2));
            if (Object.Equals(pdfObject1, pdfObject2)) {
                return true;
            }
            if (pdfObject1 == null || pdfObject2 == null) {
                return false;
            }
            if (pdfObject1.GetType() != pdfObject2.GetType()) {
                return false;
            }
            if (pdfObject1.GetIndirectReference() != null && usuallyModifiedObjects.GetFirst().Any((reference) => IsSameReference
                (reference, pdfObject1.GetIndirectReference())) && pdfObject2.GetIndirectReference() != null && usuallyModifiedObjects
                .GetSecond().Any((reference) => IsSameReference(reference, pdfObject2.GetIndirectReference()))) {
                // These two objects are expected to not be completely equal, we check them independently.
                // However, we still need to make sure those are same instances.
                return IsSameReference(pdfObject1.GetIndirectReference(), pdfObject2.GetIndirectReference());
            }
            // We don't allow objects to change from being direct to indirect and vice versa.
            // Acrobat allows it, but such change can invalidate the document.
            if (pdfObject1.GetIndirectReference() == null ^ pdfObject2.GetIndirectReference() == null) {
                return false;
            }
            switch (pdfObject1.GetObjectType()) {
                case PdfObject.BOOLEAN:
                case PdfObject.NAME:
                case PdfObject.NULL:
                case PdfObject.LITERAL:
                case PdfObject.NUMBER:
                case PdfObject.STRING: {
                    return pdfObject1.Equals(pdfObject2);
                }

                case PdfObject.INDIRECT_REFERENCE: {
                    return ComparePdfObjects(((PdfIndirectReference)pdfObject1).GetRefersTo(), ((PdfIndirectReference)pdfObject2
                        ).GetRefersTo(), visitedObjects, usuallyModifiedObjects);
                }

                case PdfObject.ARRAY: {
                    return ComparePdfArrays((PdfArray)pdfObject1, (PdfArray)pdfObject2, visitedObjects, usuallyModifiedObjects
                        );
                }

                case PdfObject.DICTIONARY: {
                    return ComparePdfDictionaries((PdfDictionary)pdfObject1, (PdfDictionary)pdfObject2, visitedObjects, usuallyModifiedObjects
                        );
                }

                case PdfObject.STREAM: {
                    return ComparePdfStreams((PdfStream)pdfObject1, (PdfStream)pdfObject2, visitedObjects, usuallyModifiedObjects
                        );
                }

                default: {
                    return false;
                }
            }
        }

        private static bool ComparePdfArrays(PdfArray array1, PdfArray array2, IList<Tuple2<PdfObject, PdfObject>>
             visitedObjects, Tuple2<ICollection<PdfIndirectReference>, ICollection<PdfIndirectReference>> usuallyModifiedObjects
            ) {
            if (array1.Size() != array2.Size()) {
                return false;
            }
            for (int i = 0; i < array1.Size(); i++) {
                if (!ComparePdfObjects(array1.Get(i), array2.Get(i), visitedObjects, usuallyModifiedObjects)) {
                    return false;
                }
            }
            return true;
        }

        private static bool ComparePdfStreams(PdfStream stream1, PdfStream stream2, IList<Tuple2<PdfObject, PdfObject
            >> visitedObjects, Tuple2<ICollection<PdfIndirectReference>, ICollection<PdfIndirectReference>> usuallyModifiedObjects
            ) {
            return JavaUtil.ArraysEquals(stream1.GetBytes(false), stream2.GetBytes(false)) && ComparePdfDictionaries(stream1
                , stream2, visitedObjects, usuallyModifiedObjects);
        }
    }
//\endcond
}
