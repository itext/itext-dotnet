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
using System.IO;
using System.Linq;
using iText.Commons.Actions.Contexts;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    /// <summary>Validator, which is responsible for document revisions validation according to doc-MDP rules.</summary>
    internal class DocumentRevisionsValidator {
        internal const String DOC_MDP_CHECK = "DocMDP check.";

        internal const String NOT_ALLOWED_CATALOG_CHANGES = "PDF document catalog contains changes other than DSS dictionary addition, which is not allowed.";

        internal const String DSS_REMOVED = "DSS dictionary was removed from catalog.";

        internal const String EXTENSIONS_REMOVED = "Extensions dictionary was removed from catalog.";

        internal const String DEVELOPER_EXTENSION_REMOVED = "Developer extension \"{0}\" dictionary was removed or unexpectedly modified.";

        internal const String EXTENSION_LEVEL_DECREASED = "Extension level number in developer extension \"{0}\" dictionary was decreased.";

        internal const String OBJECT_REMOVED = "Object \"{0}\", which is not allowed to be removed, was removed from the document through XREF table.";

        internal const String UNEXPECTED_ENTRY_IN_XREF = "New PDF document revision contains unexpected entry \"{0}\" in XREF table.";

        private IMetaInfo metaInfo;

        internal DocumentRevisionsValidator() {
        }

        // Empty constructor.
        /// <summary>
        /// Sets the
        /// <see cref="iText.Commons.Actions.Contexts.IMetaInfo"/>
        /// that will be used during
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// creation.
        /// </summary>
        /// <param name="metaInfo">meta info to set</param>
        public virtual void SetEventCountingMetaInfo(IMetaInfo metaInfo) {
            this.metaInfo = metaInfo;
        }

        internal virtual ValidationReport ValidateRevision(PdfDocument originalDocument, PdfDocument documentWithoutRevision
            , DocumentRevision revision) {
            ValidationReport validationReport = new ValidationReport();
            using (Stream inputStream = CreateInputStreamFromRevision(originalDocument, revision)) {
                using (PdfReader newReader = new PdfReader(inputStream)) {
                    using (PdfDocument documentWithRevision = new PdfDocument(newReader, new DocumentProperties().SetEventCountingMetaInfo
                        (metaInfo))) {
                        ICollection<PdfIndirectReference> indirectReferences = revision.GetModifiedObjects();
                        if (!CompareCatalogs(documentWithoutRevision, documentWithRevision, validationReport)) {
                            return validationReport;
                        }
                        IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences = CreateAllowedReferences(documentWithRevision
                            , documentWithoutRevision);
                        foreach (PdfIndirectReference indirectReference in indirectReferences) {
                            if (indirectReference.IsFree()) {
                                // In this boolean flag we check that reference which is about to be removed is the one which
                                // changed in the new revision. For instance DSS reference was 5 0 obj and changed to be 6 0 obj.
                                // In this case and only in this case reference with obj number 5 can be safely removed.
                                bool referenceAllowedToBeRemoved = allowedReferences.Any((reference) => reference.GetPreviousReference() !=
                                     null && reference.GetPreviousReference().GetObjNumber() == indirectReference.GetObjNumber() && (reference
                                    .GetCurrentReference() == null || reference.GetCurrentReference().GetObjNumber() != indirectReference.
                                    GetObjNumber()));
                                // If some reference wasn't in the previous document, it is safe to remove it,
                                // since it is not possible to introduce new reference and remove it at the same revision.
                                bool referenceWasInPrevDocument = documentWithoutRevision.GetPdfObject(indirectReference.GetObjNumber()) !=
                                     null;
                                if (!IsMaxGenerationObject(indirectReference) && referenceWasInPrevDocument && !referenceAllowedToBeRemoved
                                    ) {
                                    validationReport.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(OBJECT_REMOVED, indirectReference
                                        .GetObjNumber()), ReportItem.ReportItemStatus.INVALID));
                                }
                            }
                            else {
                                if (!CheckAllowedReferences(allowedReferences, indirectReference, documentWithoutRevision)) {
                                    validationReport.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(UNEXPECTED_ENTRY_IN_XREF
                                        , indirectReference.GetObjNumber()), ReportItem.ReportItemStatus.INVALID));
                                }
                            }
                        }
                    }
                }
            }
            return validationReport;
        }

        private bool CheckAllowedReferences(IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences, PdfIndirectReference
             indirectReference, PdfDocument documentWithoutRevision) {
            foreach (DocumentRevisionsValidator.ReferencesPair allowedReference in allowedReferences) {
                if (IsSameReference(allowedReference.GetCurrentReference(), indirectReference)) {
                    return documentWithoutRevision.GetPdfObject(indirectReference.GetObjNumber()) == null || allowedReferences
                        .Any((reference) => IsSameReference(reference.GetPreviousReference(), indirectReference));
                }
            }
            return false;
        }

        private IList<DocumentRevisionsValidator.ReferencesPair> CreateAllowedReferences(PdfDocument documentWithRevision
            , PdfDocument documentWithoutRevision) {
            // First indirect reference in the pair is an allowed reference to be present in new xref table,
            // and the second indirect reference in the pair is the same entry in the previous document.
            // If any reference is null, we expect this object to be newly generated or direct reference.
            IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences = new List<DocumentRevisionsValidator.ReferencesPair
                >();
            if (documentWithRevision.GetTrailer().Get(PdfName.Info) != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(documentWithRevision.GetTrailer().Get(
                    PdfName.Info).GetIndirectReference(), GetIndirectReferenceOrNull(() => documentWithoutRevision.GetTrailer
                    ().Get(PdfName.Info).GetIndirectReference())));
            }
            if (documentWithRevision.GetCatalog().GetPdfObject() == null) {
                return allowedReferences;
            }
            allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(documentWithRevision.GetCatalog().GetPdfObject
                ().GetIndirectReference(), GetIndirectReferenceOrNull(() => documentWithoutRevision.GetCatalog().GetPdfObject
                ().GetIndirectReference())));
            if (documentWithRevision.GetCatalog().GetPdfObject().Get(PdfName.Metadata) != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(documentWithRevision.GetCatalog().GetPdfObject
                    ().Get(PdfName.Metadata).GetIndirectReference(), GetIndirectReferenceOrNull(() => documentWithoutRevision
                    .GetCatalog().GetPdfObject().Get(PdfName.Metadata).GetIndirectReference())));
            }
            PdfDictionary currentDssDictionary = documentWithRevision.GetCatalog().GetPdfObject().GetAsDictionary(PdfName
                .DSS);
            PdfDictionary previousDssDictionary = documentWithoutRevision.GetCatalog().GetPdfObject().GetAsDictionary(
                PdfName.DSS);
            if (currentDssDictionary == null) {
                return allowedReferences;
            }
            allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentDssDictionary.GetIndirectReference
                (), GetIndirectReferenceOrNull(() => previousDssDictionary.GetIndirectReference())));
            allowedReferences.AddAll(CreateAllowedDssEntries(documentWithRevision, documentWithoutRevision));
            return allowedReferences;
        }

        private IList<DocumentRevisionsValidator.ReferencesPair> CreateAllowedDssEntries(PdfDocument documentWithRevision
            , PdfDocument documentWithoutRevision) {
            IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences = new List<DocumentRevisionsValidator.ReferencesPair
                >();
            PdfDictionary currentDssDictionary = documentWithRevision.GetCatalog().GetPdfObject().GetAsDictionary(PdfName
                .DSS);
            PdfDictionary previousDssDictionary = documentWithoutRevision.GetCatalog().GetPdfObject().GetAsDictionary(
                PdfName.DSS);
            PdfArray certs = currentDssDictionary.GetAsArray(PdfName.Certs);
            if (certs != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(certs.GetIndirectReference(), GetIndirectReferenceOrNull
                    (() => previousDssDictionary.Get(PdfName.Certs).GetIndirectReference())));
                for (int i = 0; i < certs.Size(); ++i) {
                    int finalI = i;
                    allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(certs.Get(i).GetIndirectReference(), GetIndirectReferenceOrNull
                        (() => previousDssDictionary.GetAsArray(PdfName.Certs).Get(finalI).GetIndirectReference())));
                }
            }
            PdfArray ocsps = currentDssDictionary.GetAsArray(PdfName.OCSPs);
            if (ocsps != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(ocsps.GetIndirectReference(), GetIndirectReferenceOrNull
                    (() => previousDssDictionary.Get(PdfName.OCSPs).GetIndirectReference())));
                for (int i = 0; i < ocsps.Size(); ++i) {
                    int finalI = i;
                    allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(ocsps.Get(i).GetIndirectReference(), GetIndirectReferenceOrNull
                        (() => previousDssDictionary.GetAsArray(PdfName.OCSPs).Get(finalI).GetIndirectReference())));
                }
            }
            PdfArray crls = currentDssDictionary.GetAsArray(PdfName.CRLs);
            if (crls != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(crls.GetIndirectReference(), GetIndirectReferenceOrNull
                    (() => previousDssDictionary.Get(PdfName.CRLs).GetIndirectReference())));
                for (int i = 0; i < crls.Size(); ++i) {
                    int finalI = i;
                    allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(crls.Get(i).GetIndirectReference(), GetIndirectReferenceOrNull
                        (() => previousDssDictionary.GetAsArray(PdfName.CRLs).Get(finalI).GetIndirectReference())));
                }
            }
            PdfDictionary vris = currentDssDictionary.GetAsDictionary(PdfName.VRI);
            if (vris != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vris.GetIndirectReference(), GetIndirectReferenceOrNull
                    (() => previousDssDictionary.Get(PdfName.VRI).GetIndirectReference())));
                foreach (KeyValuePair<PdfName, PdfObject> vri in vris.EntrySet()) {
                    allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vri.Value.GetIndirectReference(), GetIndirectReferenceOrNull
                        (() => previousDssDictionary.GetAsDictionary(PdfName.VRI).Get(vri.Key).GetIndirectReference())));
                    if (vri.Value is PdfDictionary) {
                        PdfDictionary vriDictionary = (PdfDictionary)vri.Value;
                        PdfArray vriCerts = vriDictionary.GetAsArray(PdfName.Cert);
                        if (vriCerts != null) {
                            allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriCerts.GetIndirectReference(), GetIndirectReferenceOrNull
                                (() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(vri.Key).Get(PdfName.Cert).GetIndirectReference
                                ())));
                            for (int i = 0; i < vriCerts.Size(); ++i) {
                                int finalI = i;
                                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriCerts.Get(i).GetIndirectReference()
                                    , GetIndirectReferenceOrNull(() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(
                                    vri.Key).GetAsArray(PdfName.Cert).Get(finalI).GetIndirectReference())));
                            }
                        }
                        PdfArray vriOcsps = vriDictionary.GetAsArray(PdfName.OCSP);
                        if (vriOcsps != null) {
                            allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriOcsps.GetIndirectReference(), GetIndirectReferenceOrNull
                                (() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(vri.Key).Get(PdfName.OCSP).GetIndirectReference
                                ())));
                            for (int i = 0; i < vriOcsps.Size(); ++i) {
                                int finalI = i;
                                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriOcsps.Get(i).GetIndirectReference()
                                    , GetIndirectReferenceOrNull(() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(
                                    vri.Key).GetAsArray(PdfName.OCSP).Get(finalI).GetIndirectReference())));
                            }
                        }
                        PdfArray vriCrls = vriDictionary.GetAsArray(PdfName.CRL);
                        if (vriCrls != null) {
                            allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriCrls.GetIndirectReference(), GetIndirectReferenceOrNull
                                (() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(vri.Key).Get(PdfName.CRL).GetIndirectReference
                                ())));
                            for (int i = 0; i < vriCrls.Size(); ++i) {
                                int finalI = i;
                                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriCrls.Get(i).GetIndirectReference(), 
                                    GetIndirectReferenceOrNull(() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(vri
                                    .Key).GetAsArray(PdfName.CRL).Get(finalI).GetIndirectReference())));
                            }
                        }
                        if (vriDictionary.Get(new PdfName("TS")) != null) {
                            allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriDictionary.Get(new PdfName("TS")).GetIndirectReference
                                (), GetIndirectReferenceOrNull(() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary
                                (vri.Key).Get(new PdfName("TS")).GetIndirectReference())));
                        }
                    }
                }
            }
            return allowedReferences;
        }

        private bool CompareCatalogs(PdfDocument documentWithoutRevision, PdfDocument documentWithRevision, ValidationReport
             report) {
            PdfDictionary previousCatalog = documentWithoutRevision.GetCatalog().GetPdfObject();
            PdfDictionary currentCatalog = documentWithRevision.GetCatalog().GetPdfObject();
            PdfDictionary previousCatalogCopy = new PdfDictionary(previousCatalog);
            previousCatalogCopy.Remove(PdfName.DSS);
            previousCatalogCopy.Remove(PdfName.Extensions);
            previousCatalogCopy.Remove(PdfName.Metadata);
            PdfDictionary currentCatalogCopy = new PdfDictionary(currentCatalog);
            currentCatalogCopy.Remove(PdfName.DSS);
            currentCatalogCopy.Remove(PdfName.Extensions);
            currentCatalogCopy.Remove(PdfName.Metadata);
            if (!ComparePdfObjects(previousCatalogCopy, currentCatalogCopy)) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, NOT_ALLOWED_CATALOG_CHANGES, ReportItem.ReportItemStatus
                    .INVALID));
                return false;
            }
            return CompareExtensions(previousCatalog.Get(PdfName.Extensions), currentCatalog.Get(PdfName.Extensions), 
                report) && CompareDss(previousCatalog.Get(PdfName.DSS), currentCatalog.Get(PdfName.DSS), report);
        }

        private bool CompareExtensions(PdfObject previousExtensions, PdfObject currentExtensions, ValidationReport
             report) {
            if (previousExtensions == null || ComparePdfObjects(previousExtensions, currentExtensions)) {
                return true;
            }
            if (currentExtensions == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, EXTENSIONS_REMOVED, ReportItem.ReportItemStatus.INVALID
                    ));
                return false;
            }
            if (!(previousExtensions is PdfDictionary) || !(currentExtensions is PdfDictionary)) {
                return false;
            }
            PdfDictionary previousExtensionsDictionary = (PdfDictionary)previousExtensions;
            PdfDictionary currentExtensionsDictionary = (PdfDictionary)currentExtensions;
            foreach (KeyValuePair<PdfName, PdfObject> previousExtension in previousExtensionsDictionary.EntrySet()) {
                PdfDictionary currentExtension = currentExtensionsDictionary.GetAsDictionary(previousExtension.Key);
                if (currentExtension == null) {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(DEVELOPER_EXTENSION_REMOVED, previousExtension
                        .Key), ReportItem.ReportItemStatus.INVALID));
                    return false;
                }
                else {
                    PdfDictionary currentExtensionCopy = new PdfDictionary(currentExtension);
                    currentExtensionCopy.Remove(PdfName.ExtensionLevel);
                    PdfDictionary previousExtensionCopy = new PdfDictionary((PdfDictionary)previousExtension.Value);
                    previousExtensionCopy.Remove(PdfName.ExtensionLevel);
                    // Apart from extension level dictionaries are expected to be equal.
                    if (!ComparePdfObjects(previousExtensionCopy, currentExtensionCopy)) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(DEVELOPER_EXTENSION_REMOVED, previousExtension
                            .Key), ReportItem.ReportItemStatus.INVALID));
                        return false;
                    }
                    PdfNumber previousExtensionLevel = ((PdfDictionary)previousExtension.Value).GetAsNumber(PdfName.ExtensionLevel
                        );
                    PdfNumber currentExtensionLevel = currentExtension.GetAsNumber(PdfName.ExtensionLevel);
                    if (previousExtensionLevel != null) {
                        if (currentExtensionLevel == null || previousExtensionLevel.IntValue() > currentExtensionLevel.IntValue()) {
                            report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(EXTENSION_LEVEL_DECREASED, previousExtension
                                .Key), ReportItem.ReportItemStatus.INVALID));
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CompareDss(PdfObject previousDss, PdfObject currentDss, ValidationReport report) {
            if (previousDss == null) {
                return true;
            }
            if (currentDss == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, DSS_REMOVED, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            return true;
        }

        internal static Stream CreateInputStreamFromRevision(PdfDocument originalDocument, DocumentRevision revision
            ) {
            RandomAccessFileOrArray raf = originalDocument.GetReader().GetSafeFile();
            WindowRandomAccessSource source = new WindowRandomAccessSource(raf.CreateSourceView(), 0, revision.GetEofOffset
                ());
            return new RASInputStream(source);
        }

        private static bool ComparePdfObjects(PdfObject pdfObject1, PdfObject pdfObject2) {
            return ComparePdfObjects(pdfObject1, pdfObject2, new HashSet<PdfObject>());
        }

        private static bool ComparePdfObjects(PdfObject pdfObject1, PdfObject pdfObject2, ICollection<PdfObject> visitedObjects
            ) {
            if (visitedObjects.Contains(pdfObject1)) {
                return true;
            }
            visitedObjects.Add(pdfObject1);
            if (Object.Equals(pdfObject1, pdfObject2)) {
                return true;
            }
            if (pdfObject1 == null || pdfObject2 == null) {
                return false;
            }
            if (pdfObject1.GetType() != pdfObject2.GetType()) {
                return false;
            }
            // We don't allow objects to be direct and indirect.
            // Acrobat however allows it, but such change can invalidate the document.
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
                        ).GetRefersTo(), visitedObjects);
                }

                case PdfObject.ARRAY: {
                    return ComparePdfArrays((PdfArray)pdfObject1, (PdfArray)pdfObject2, visitedObjects);
                }

                case PdfObject.DICTIONARY: {
                    return ComparePdfDictionaries((PdfDictionary)pdfObject1, (PdfDictionary)pdfObject2, visitedObjects);
                }

                case PdfObject.STREAM: {
                    return ComparePdfStreams((PdfStream)pdfObject1, (PdfStream)pdfObject2, visitedObjects);
                }

                default: {
                    return false;
                }
            }
        }

        private static bool ComparePdfArrays(PdfArray array1, PdfArray array2, ICollection<PdfObject> visitedObjects
            ) {
            if (array1.Size() != array2.Size()) {
                return false;
            }
            for (int i = 0; i < array1.Size(); i++) {
                if (!ComparePdfObjects(array1.Get(i), array2.Get(i), visitedObjects)) {
                    return false;
                }
            }
            return true;
        }

        private static bool ComparePdfDictionaries(PdfDictionary dictionary1, PdfDictionary dictionary2, ICollection
            <PdfObject> visitedObjects) {
            ICollection<KeyValuePair<PdfName, PdfObject>> entrySet1 = dictionary1.EntrySet();
            ICollection<KeyValuePair<PdfName, PdfObject>> entrySet2 = dictionary2.EntrySet();
            if (entrySet1.Count != entrySet2.Count) {
                return false;
            }
            foreach (KeyValuePair<PdfName, PdfObject> entry1 in entrySet1) {
                if (!entrySet2.Any((entry2) => entry2.Key.Equals(entry1.Key) && ComparePdfObjects(entry2.Value, entry1.Value
                    , visitedObjects))) {
                    return false;
                }
            }
            return true;
        }

        private static bool ComparePdfStreams(PdfStream stream1, PdfStream stream2, ICollection<PdfObject> visitedObjects
            ) {
            return JavaUtil.ArraysEquals(stream1.GetBytes(), stream2.GetBytes()) && ComparePdfDictionaries(stream1, stream2
                , visitedObjects);
        }

        private static bool IsSameReference(PdfIndirectReference indirectReference1, PdfIndirectReference indirectReference2
            ) {
            if (indirectReference1 == indirectReference2) {
                return true;
            }
            if (indirectReference1 == null || indirectReference2 == null) {
                return false;
            }
            return indirectReference1.GetObjNumber() == indirectReference2.GetObjNumber() && indirectReference1.GetGenNumber
                () == indirectReference2.GetGenNumber();
        }

        private static bool IsMaxGenerationObject(PdfIndirectReference indirectReference) {
            return indirectReference.GetObjNumber() == 0 && indirectReference.GetGenNumber() == 65535;
        }

        private static PdfIndirectReference GetIndirectReferenceOrNull(Func<PdfIndirectReference> referenceGetter) {
            try {
                return referenceGetter();
            }
            catch (Exception) {
                return null;
            }
        }

        private class ReferencesPair {
            private readonly PdfIndirectReference currentReference;

            private readonly PdfIndirectReference previousReference;

            internal ReferencesPair(PdfIndirectReference currentReference, PdfIndirectReference previousReference) {
                this.currentReference = currentReference;
                this.previousReference = previousReference;
            }

            public virtual PdfIndirectReference GetCurrentReference() {
                return currentReference;
            }

            public virtual PdfIndirectReference GetPreviousReference() {
                return previousReference;
            }
        }
    }
}
