/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Pdf.Annot;

namespace iText.Kernel.Pdf {
    internal sealed class OcgPropertiesCopier {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.OcgPropertiesCopier
            ));

        private OcgPropertiesCopier() {
        }

        // Empty constructor
        public static void CopyOCGProperties(PdfDocument fromDocument, PdfDocument toDocument, IDictionary<PdfPage
            , PdfPage> page2page) {
            try {
                // Configs are not copied
                PdfDictionary toOcProperties = toDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.OCProperties
                    );
                ICollection<PdfIndirectReference> fromOcgsToCopy = iText.Kernel.Pdf.OcgPropertiesCopier.GetAllUsedNonFlushedOCGs
                    (page2page, toOcProperties);
                if (fromOcgsToCopy.IsEmpty()) {
                    return;
                }
                // Reset ocProperties field in order to create it a new at the
                // method end using the new (merged) OCProperties dictionary
                toOcProperties = toDocument.GetCatalog().FillAndGetOcPropertiesDictionary();
                PdfDictionary fromOcProperties = fromDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.OCProperties
                    );
                iText.Kernel.Pdf.OcgPropertiesCopier.CopyOCGs(fromOcgsToCopy, toOcProperties, toDocument);
                iText.Kernel.Pdf.OcgPropertiesCopier.CopyDDictionary(fromOcgsToCopy, fromOcProperties.GetAsDictionary(PdfName
                    .D), toOcProperties, toDocument);
            }
            catch (Exception ex) {
                LOGGER.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCG_COPYING_ERROR, ex.ToString
                    ()));
            }
        }

        private static ICollection<PdfIndirectReference> GetAllUsedNonFlushedOCGs(IDictionary<PdfPage, PdfPage> page2page
            , PdfDictionary toOcProperties) {
            // NOTE: the PDF is considered to be valid and therefore the presence of OСG in OCProperties.OCGs is not checked
            ICollection<PdfIndirectReference> fromUsedOcgs = new LinkedHashSet<PdfIndirectReference>();
            // Visit the pages in parallel to find non-flush OSGs
            PdfPage[] fromPages = page2page.Keys.ToArray(new PdfPage[0]);
            PdfPage[] toPages = page2page.Values.ToArray(new PdfPage[0]);
            for (int i = 0; i < toPages.Length; i++) {
                PdfPage fromPage = fromPages[i];
                PdfPage toPage = toPages[i];
                // Copy OCGs from annotations
                IList<PdfAnnotation> toAnnotations = toPage.GetAnnotations();
                IList<PdfAnnotation> fromAnnotations = fromPage.GetAnnotations();
                for (int j = 0; j < toAnnotations.Count; j++) {
                    if (!toAnnotations[j].IsFlushed()) {
                        PdfDictionary toAnnotDict = toAnnotations[j].GetPdfObject();
                        PdfDictionary fromAnnotDict = fromAnnotations[j].GetPdfObject();
                        PdfAnnotation toAnnot = toAnnotations[j];
                        PdfAnnotation fromAnnot = fromAnnotations[j];
                        if (!toAnnotDict.IsFlushed()) {
                            iText.Kernel.Pdf.OcgPropertiesCopier.GetUsedNonFlushedOCGsFromOcDict(toAnnotDict.GetAsDictionary(PdfName.OC
                                ), fromAnnotDict.GetAsDictionary(PdfName.OC), fromUsedOcgs, toOcProperties);
                            iText.Kernel.Pdf.OcgPropertiesCopier.GetUsedNonFlushedOCGsFromXObject(toAnnot.GetNormalAppearanceObject(), 
                                fromAnnot.GetNormalAppearanceObject(), fromUsedOcgs, toOcProperties, new HashSet<PdfObject>());
                            iText.Kernel.Pdf.OcgPropertiesCopier.GetUsedNonFlushedOCGsFromXObject(toAnnot.GetRolloverAppearanceObject(
                                ), fromAnnot.GetRolloverAppearanceObject(), fromUsedOcgs, toOcProperties, new HashSet<PdfObject>());
                            iText.Kernel.Pdf.OcgPropertiesCopier.GetUsedNonFlushedOCGsFromXObject(toAnnot.GetDownAppearanceObject(), fromAnnot
                                .GetDownAppearanceObject(), fromUsedOcgs, toOcProperties, new HashSet<PdfObject>());
                        }
                    }
                }
                PdfDictionary toResources = toPage.GetPdfObject().GetAsDictionary(PdfName.Resources);
                PdfDictionary fromResources = fromPage.GetPdfObject().GetAsDictionary(PdfName.Resources);
                iText.Kernel.Pdf.OcgPropertiesCopier.GetUsedNonFlushedOCGsFromResources(toResources, fromResources, fromUsedOcgs
                    , toOcProperties, new HashSet<PdfObject>());
            }
            return fromUsedOcgs;
        }

        private static void GetUsedNonFlushedOCGsFromResources(PdfDictionary toResources, PdfDictionary fromResources
            , ICollection<PdfIndirectReference> fromUsedOcgs, PdfDictionary toOcProperties, ICollection<PdfObject>
             visitedObjects) {
            if (toResources != null && !toResources.IsFlushed()) {
                // Copy OCGs from properties
                PdfDictionary toProperties = toResources.GetAsDictionary(PdfName.Properties);
                PdfDictionary fromProperties = fromResources.GetAsDictionary(PdfName.Properties);
                if (toProperties != null && !toProperties.IsFlushed()) {
                    foreach (PdfName name in toProperties.KeySet()) {
                        PdfObject toCurrObj = toProperties.Get(name);
                        PdfObject fromCurrObj = fromProperties.Get(name);
                        iText.Kernel.Pdf.OcgPropertiesCopier.GetUsedNonFlushedOCGsFromOcDict(toCurrObj, fromCurrObj, fromUsedOcgs, 
                            toOcProperties);
                    }
                }
                // Copy OCGs from xObject
                PdfDictionary toXObject = toResources.GetAsDictionary(PdfName.XObject);
                PdfDictionary fromXObject = fromResources.GetAsDictionary(PdfName.XObject);
                iText.Kernel.Pdf.OcgPropertiesCopier.GetUsedNonFlushedOCGsFromXObject(toXObject, fromXObject, fromUsedOcgs
                    , toOcProperties, visitedObjects);
            }
        }

        private static void GetUsedNonFlushedOCGsFromXObject(PdfDictionary toXObject, PdfDictionary fromXObject, ICollection
            <PdfIndirectReference> fromUsedOcgs, PdfDictionary toOcProperties, ICollection<PdfObject> visitedObjects
            ) {
            //Resolving cycled properties, by memorizing the visited objects
            if (visitedObjects.Contains(fromXObject)) {
                return;
            }
            visitedObjects.Add(fromXObject);
            if (toXObject != null && !toXObject.IsFlushed()) {
                if (toXObject.IsStream() && !toXObject.IsFlushed()) {
                    PdfStream toStream = (PdfStream)toXObject;
                    PdfStream fromStream = (PdfStream)fromXObject;
                    iText.Kernel.Pdf.OcgPropertiesCopier.GetUsedNonFlushedOCGsFromOcDict(toStream.GetAsDictionary(PdfName.OC), 
                        fromStream.GetAsDictionary(PdfName.OC), fromUsedOcgs, toOcProperties);
                    iText.Kernel.Pdf.OcgPropertiesCopier.GetUsedNonFlushedOCGsFromResources(toStream.GetAsDictionary(PdfName.Resources
                        ), fromStream.GetAsDictionary(PdfName.Resources), fromUsedOcgs, toOcProperties, visitedObjects);
                }
                else {
                    foreach (PdfName name in toXObject.KeySet()) {
                        PdfObject toCurrObj = toXObject.Get(name);
                        PdfObject fromCurrObj = fromXObject.Get(name);
                        if (toCurrObj.IsStream() && !toCurrObj.IsFlushed()) {
                            PdfStream toStream = (PdfStream)toCurrObj;
                            PdfStream fromStream = (PdfStream)fromCurrObj;
                            iText.Kernel.Pdf.OcgPropertiesCopier.GetUsedNonFlushedOCGsFromXObject(toStream, fromStream, fromUsedOcgs, 
                                toOcProperties, visitedObjects);
                        }
                    }
                }
            }
        }

        private static void GetUsedNonFlushedOCGsFromOcDict(PdfObject toObj, PdfObject fromObj, ICollection<PdfIndirectReference
            > fromUsedOcgs, PdfDictionary toOcProperties) {
            if (toObj != null && toObj.IsDictionary() && !toObj.IsFlushed()) {
                PdfDictionary toCurrDict = (PdfDictionary)toObj;
                PdfDictionary fromCurrDict = (PdfDictionary)fromObj;
                PdfName typeName = toCurrDict.GetAsName(PdfName.Type);
                if (PdfName.OCG.Equals(typeName) && !iText.Kernel.Pdf.OcgPropertiesCopier.OcgAlreadyInOCGs(toCurrDict.GetIndirectReference
                    (), toOcProperties)) {
                    fromUsedOcgs.Add(fromCurrDict.GetIndirectReference());
                }
                else {
                    if (PdfName.OCMD.Equals(typeName)) {
                        PdfArray toOcgs = null;
                        PdfArray fromOcgs = null;
                        if (toCurrDict.GetAsDictionary(PdfName.OCGs) != null) {
                            toOcgs = new PdfArray();
                            toOcgs.Add(toCurrDict.GetAsDictionary(PdfName.OCGs));
                            fromOcgs = new PdfArray();
                            fromOcgs.Add(fromCurrDict.GetAsDictionary(PdfName.OCGs));
                        }
                        else {
                            if (toCurrDict.GetAsArray(PdfName.OCGs) != null) {
                                toOcgs = toCurrDict.GetAsArray(PdfName.OCGs);
                                fromOcgs = fromCurrDict.GetAsArray(PdfName.OCGs);
                            }
                        }
                        if (toOcgs != null && !toOcgs.IsFlushed()) {
                            for (int i = 0; i < toOcgs.Size(); i++) {
                                iText.Kernel.Pdf.OcgPropertiesCopier.GetUsedNonFlushedOCGsFromOcDict(toOcgs.Get(i), fromOcgs.Get(i), fromUsedOcgs
                                    , toOcProperties);
                            }
                        }
                    }
                }
            }
        }

        private static void CopyOCGs(ICollection<PdfIndirectReference> fromOcgsToCopy, PdfDictionary toOcProperties
            , PdfDocument toDocument) {
            ICollection<String> layerNames = new HashSet<String>();
            if (toOcProperties.GetAsArray(PdfName.OCGs) != null) {
                PdfArray toOcgs = toOcProperties.GetAsArray(PdfName.OCGs);
                foreach (PdfObject toOcgObj in toOcgs) {
                    if (toOcgObj.IsDictionary()) {
                        layerNames.Add(((PdfDictionary)toOcgObj).GetAsString(PdfName.Name).ToUnicodeString());
                    }
                }
            }
            bool hasConflictingNames = false;
            foreach (PdfIndirectReference fromOcgRef in fromOcgsToCopy) {
                PdfDictionary toOcg = (PdfDictionary)fromOcgRef.GetRefersTo().CopyTo(toDocument, false);
                String currentLayerName = toOcg.GetAsString(PdfName.Name).ToUnicodeString();
                // Here we check on existed layer names only in destination document but not in source document.
                // That is why there is no something like layerNames.add(currentLayerName); after this if statement
                if (layerNames.Contains(currentLayerName)) {
                    hasConflictingNames = true;
                    int i = 0;
                    while (layerNames.Contains(currentLayerName + "_" + i)) {
                        i++;
                    }
                    currentLayerName += "_" + i;
                    toOcg.Put(PdfName.Name, new PdfString(currentLayerName, PdfEncodings.UNICODE_BIG));
                }
                if (toOcProperties.GetAsArray(PdfName.OCGs) == null) {
                    toOcProperties.Put(PdfName.OCGs, new PdfArray());
                }
                toOcProperties.GetAsArray(PdfName.OCGs).Add(toOcg);
            }
            if (hasConflictingNames) {
                LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_HAS_CONFLICTING_OCG_NAMES);
            }
        }

        private static bool OcgAlreadyInOCGs(PdfIndirectReference toOcgRef, PdfDictionary toOcProperties) {
            if (toOcProperties == null) {
                return false;
            }
            PdfArray toOcgs = toOcProperties.GetAsArray(PdfName.OCGs);
            if (toOcgs != null) {
                foreach (PdfObject toOcg in toOcgs) {
                    if (toOcgRef.Equals(toOcg.GetIndirectReference())) {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void CopyDDictionary(ICollection<PdfIndirectReference> fromOcgsToCopy, PdfDictionary fromDDict
            , PdfDictionary toOcProperties, PdfDocument toDocument) {
            if (toOcProperties.GetAsDictionary(PdfName.D) == null) {
                toOcProperties.Put(PdfName.D, new PdfDictionary());
            }
            PdfDictionary toDDict = toOcProperties.GetAsDictionary(PdfName.D);
            // The Name field is not copied because it will be given when flushing the PdfOCProperties
            // Delete the Creator field because the D dictionary are changing
            toDDict.Remove(PdfName.Creator);
            // The BaseState field is not copied because for dictionary D BaseState should have the value ON, which is the default
            iText.Kernel.Pdf.OcgPropertiesCopier.CopyDArrayField(PdfName.ON, fromOcgsToCopy, fromDDict, toDDict, toDocument
                );
            iText.Kernel.Pdf.OcgPropertiesCopier.CopyDArrayField(PdfName.OFF, fromOcgsToCopy, fromDDict, toDDict, toDocument
                );
            // The Intent field is not copied because for dictionary D Intent should have the value View, which is the default
            // The AS field is not copied because it will be given when flushing the PdfOCProperties
            iText.Kernel.Pdf.OcgPropertiesCopier.CopyDArrayField(PdfName.Order, fromOcgsToCopy, fromDDict, toDDict, toDocument
                );
            // The ListModel field is not copied because it only affects the visual presentation of the layers
            iText.Kernel.Pdf.OcgPropertiesCopier.CopyDArrayField(PdfName.RBGroups, fromOcgsToCopy, fromDDict, toDDict, 
                toDocument);
            iText.Kernel.Pdf.OcgPropertiesCopier.CopyDArrayField(PdfName.Locked, fromOcgsToCopy, fromDDict, toDDict, toDocument
                );
        }

        private static void AttemptToAddObjectToArray(ICollection<PdfIndirectReference> fromOcgsToCopy, PdfObject 
            fromObj, PdfArray toArray, PdfDocument toDocument) {
            PdfIndirectReference fromObjRef = fromObj.GetIndirectReference();
            if (fromObjRef != null && fromOcgsToCopy.Contains(fromObjRef)) {
                toArray.Add(fromObj.CopyTo(toDocument, false));
            }
        }

        private static void CopyDArrayField(PdfName fieldToCopy, ICollection<PdfIndirectReference> fromOcgsToCopy, 
            PdfDictionary fromDict, PdfDictionary toDict, PdfDocument toDocument) {
            if (fromDict.GetAsArray(fieldToCopy) == null) {
                return;
            }
            PdfArray fromArray = fromDict.GetAsArray(fieldToCopy);
            if (toDict.GetAsArray(fieldToCopy) == null) {
                toDict.Put(fieldToCopy, new PdfArray());
            }
            PdfArray toArray = toDict.GetAsArray(fieldToCopy);
            ICollection<PdfIndirectReference> toOcgsToCopy = new HashSet<PdfIndirectReference>();
            foreach (PdfIndirectReference fromRef in fromOcgsToCopy) {
                toOcgsToCopy.Add(fromRef.GetRefersTo().CopyTo(toDocument, false).GetIndirectReference());
            }
            if (PdfName.Order.Equals(fieldToCopy)) {
                // Stage 1: delete all Order the entire branches from the output document in which the copied OCGs were
                IList<int> removeIndex = new List<int>();
                for (int i = 0; i < toArray.Size(); i++) {
                    PdfObject toOrderItem = toArray.Get(i);
                    if (iText.Kernel.Pdf.OcgPropertiesCopier.OrderBranchContainsSetElements(toOrderItem, toArray, i, toOcgsToCopy
                        , null, null)) {
                        removeIndex.Add(i);
                    }
                }
                for (int i = removeIndex.Count - 1; i > -1; i--) {
                    toArray.Remove(removeIndex[i]);
                }
                PdfArray toOcgs = toDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.OCProperties).GetAsArray(
                    PdfName.OCGs);
                // Stage 2: copy all the Order the entire branches in which the copied OСGs were
                for (int i = 0; i < fromArray.Size(); i++) {
                    PdfObject fromOrderItem = fromArray.Get(i);
                    if (iText.Kernel.Pdf.OcgPropertiesCopier.OrderBranchContainsSetElements(fromOrderItem, fromArray, i, fromOcgsToCopy
                        , toOcgs, toDocument)) {
                        toArray.Add(fromOrderItem.CopyTo(toDocument, false));
                    }
                }
            }
            else {
                // Stage 3: remove from Order OCGs not presented in the output document. When forming
                // the Order dictionary in the PdfOcProperties constructor, only those OCGs that are
                // in the OCProperties/OCGs array will be taken into account
                if (PdfName.RBGroups.Equals(fieldToCopy)) {
                    // Stage 1: delete all RBGroups from the output document in which the copied OCGs were
                    for (int i = toArray.Size() - 1; i > -1; i--) {
                        PdfArray toRbGroup = (PdfArray)toArray.Get(i);
                        foreach (PdfObject toRbGroupItemObj in toRbGroup) {
                            if (toOcgsToCopy.Contains(toRbGroupItemObj.GetIndirectReference())) {
                                toArray.Remove(i);
                                break;
                            }
                        }
                    }
                    // Stage 2: copy all the RBGroups in which the copied OCGs were
                    foreach (PdfObject fromRbGroupObj in fromArray) {
                        PdfArray fromRbGroup = (PdfArray)fromRbGroupObj;
                        foreach (PdfObject fromRbGroupItemObj in fromRbGroup) {
                            if (fromOcgsToCopy.Contains(fromRbGroupItemObj.GetIndirectReference())) {
                                toArray.Add(fromRbGroup.CopyTo(toDocument, false));
                                break;
                            }
                        }
                    }
                }
                else {
                    // Stage 3: remove from RBGroups OCGs not presented in the output
                    // document (is in the PdfOcProperties#fillDictionary method)
                    foreach (PdfObject fromObj in fromArray) {
                        iText.Kernel.Pdf.OcgPropertiesCopier.AttemptToAddObjectToArray(fromOcgsToCopy, fromObj, toArray, toDocument
                            );
                    }
                }
            }
            if (toArray.IsEmpty()) {
                toDict.Remove(fieldToCopy);
            }
        }

        private static bool OrderBranchContainsSetElements(PdfObject arrayObj, PdfArray array, int currentIndex, ICollection
            <PdfIndirectReference> ocgs, PdfArray toOcgs, PdfDocument toDocument) {
            if (arrayObj.IsDictionary()) {
                if (ocgs.Contains(arrayObj.GetIndirectReference())) {
                    return true;
                }
                else {
                    if (currentIndex < (array.Size() - 1) && array.Get(currentIndex + 1).IsArray()) {
                        PdfArray nextArray = array.GetAsArray(currentIndex + 1);
                        if (!nextArray.Get(0).IsString()) {
                            bool result = iText.Kernel.Pdf.OcgPropertiesCopier.OrderBranchContainsSetElements(nextArray, array, currentIndex
                                 + 1, ocgs, toOcgs, toDocument);
                            if (result && toOcgs != null && !ocgs.Contains(arrayObj.GetIndirectReference())) {
                                // Add the OCG to the OCGs array to register the OCG in document, since it is not used
                                // directly in the document, but is used as a parent for the order group. If it is not added
                                // to the OCGs array, then the OCG will be deleted at the 3rd stage of the /Order entry coping.
                                toOcgs.Add(arrayObj.CopyTo(toDocument, false));
                            }
                            return result;
                        }
                    }
                }
            }
            else {
                if (arrayObj.IsArray()) {
                    PdfArray arrayItem = (PdfArray)arrayObj;
                    for (int i = 0; i < arrayItem.Size(); i++) {
                        PdfObject obj = arrayItem.Get(i);
                        if (iText.Kernel.Pdf.OcgPropertiesCopier.OrderBranchContainsSetElements(obj, arrayItem, i, ocgs, toOcgs, toDocument
                            )) {
                            return true;
                        }
                    }
                    if (!arrayItem.IsEmpty() && !arrayItem.Get(0).IsString()) {
                        if (currentIndex > 0 && array.Get(currentIndex - 1).IsDictionary()) {
                            PdfDictionary previousDict = (PdfDictionary)array.Get(currentIndex - 1);
                            return ocgs.Contains(previousDict.GetIndirectReference());
                        }
                    }
                }
            }
            return false;
        }
    }
}
