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
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Validation.Context;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Utility class which performs UA-2 checks related to intra-document destinations.</summary>
    public class PdfUA2DestinationsChecker {
        private readonly PdfDestinationAdditionContext context;

        private readonly PdfDocument document;

        /// <summary>
        /// Creates
        /// <see cref="PdfUA2DestinationsChecker"/>
        /// instance.
        /// </summary>
        /// <param name="context">
        /// 
        /// <see cref="iText.Kernel.Validation.Context.PdfDestinationAdditionContext"/>
        /// which contains destination which was added
        /// </param>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to which destination was added
        /// </param>
        public PdfUA2DestinationsChecker(PdfDestinationAdditionContext context, PdfDocument document) {
            this.context = context;
            this.document = document;
        }

        /// <summary>
        /// Creates
        /// <see cref="PdfUA2DestinationsChecker"/>
        /// instance.
        /// </summary>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance in which destinations shall be checked
        /// </param>
        public PdfUA2DestinationsChecker(PdfDocument document) {
            this.context = null;
            this.document = document;
        }

        /// <summary>Checks all the destinations in the document.</summary>
        public virtual void CheckDestinations() {
            CheckDestinationsInLinks();
            if (document.HasOutlines()) {
                CheckDestinationsInOutline(document.GetOutlines(true));
            }
            CheckAllGoToActions();
        }

        /// <summary>Checks specific destination which was recently added.</summary>
        public virtual void CheckDestinationsOnCreation() {
            if (context != null && !IsDestinationAllowed(context.GetDestination(), document, 0) && !IsActionAllowed(context
                .GetAction(), document, 0)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION);
            }
        }

        private void CheckDestinationsInLinks() {
            for (int i = 1; i < document.GetNumberOfPages() + 1; ++i) {
                PdfPage page = document.GetPage(i);
                foreach (PdfAnnotation annotation in page.GetAnnotations()) {
                    if (annotation is PdfLinkAnnotation) {
                        PdfLinkAnnotation linkAnnotation = (PdfLinkAnnotation)annotation;
                        if (!IsDestinationAllowed(linkAnnotation.GetDestinationObject(), document, 0)) {
                            throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION);
                        }
                    }
                }
            }
        }

        private void CheckDestinationsInOutline(PdfOutline outline) {
            if (outline != null) {
                if (!IsDestinationAllowed(outline.GetDestination(), document, 0)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION);
                }
                foreach (PdfOutline kid in outline.GetAllChildren()) {
                    CheckDestinationsInOutline(kid);
                }
            }
        }

        private void CheckAllGoToActions() {
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            CheckAllGoToActions(catalog, new List<PdfObject>());
        }

        private void CheckAllGoToActions(PdfObject @object, IList<PdfObject> visitedObjects) {
            if (visitedObjects.Any((visitedObject) => visitedObject == @object)) {
                return;
            }
            visitedObjects.Add(@object);
            switch (@object.GetObjectType()) {
                case PdfObject.ARRAY: {
                    foreach (PdfObject kid in (PdfArray)@object) {
                        CheckAllGoToActions(kid, visitedObjects);
                    }
                    break;
                }

                case PdfObject.DICTIONARY:
                case PdfObject.STREAM: {
                    CheckGoToAction((PdfDictionary)@object);
                    foreach (PdfObject kid in ((PdfDictionary)@object).Values()) {
                        CheckAllGoToActions(kid, visitedObjects);
                    }
                    break;
                }
            }
        }

        private void CheckGoToAction(PdfDictionary dictionary) {
            // If dictionary contains S entry with GoTo value we assume this is GoTo action.
            if (PdfName.GoTo.Equals(dictionary.GetAsName(PdfName.S))) {
                if (!IsDestinationAllowed(dictionary, document, 0)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION);
                }
            }
        }

        private static bool IsDestinationAllowed(PdfObject destinationObject, PdfDocument document, int counter) {
            if (counter > 50) {
                // If we reached this method more than 50 times. Something is definitely wrong and destination isn't valid.
                // This can, for example, happen with named or string destinations pointing towards one another.
                return false;
            }
            counter++;
            return destinationObject == null || IsDestinationAllowed(PdfDestination.MakeDestination(destinationObject, 
                false), document, counter);
        }

        private static bool IsDestinationAllowed(PdfDestination destination, PdfDocument document, int counter) {
            if (destination == null || destination is PdfStructureDestination) {
                return true;
            }
            if (destination is PdfExplicitDestination || destination is PdfExplicitRemoteGoToDestination) {
                return false;
            }
            if (destination is PdfNamedDestination) {
                return IsDestinationAllowed((PdfNamedDestination)destination, document, counter);
            }
            if (destination is PdfStringDestination) {
                return IsDestinationAllowed((PdfStringDestination)destination, document, counter);
            }
            return true;
        }

        private static bool IsDestinationAllowed(PdfNamedDestination namedDestination, PdfDocument document, int counter
            ) {
            PdfCatalog catalog = document.GetCatalog();
            PdfDictionary dests = catalog.GetPdfObject().GetAsDictionary(PdfName.Dests);
            if (dests != null) {
                PdfObject actualDestinationObject = dests.Get((PdfName)namedDestination.GetPdfObject());
                if (actualDestinationObject is PdfDictionary) {
                    return IsDestinationAllowed((PdfDictionary)actualDestinationObject, document, counter);
                }
                return IsDestinationAllowed(actualDestinationObject, document, counter);
            }
            return true;
        }

        private static bool IsDestinationAllowed(PdfStringDestination stringDestination, PdfDocument document, int
             counter) {
            PdfCatalog catalog = document.GetCatalog();
            PdfNameTree dests = catalog.GetNameTree(PdfName.Dests);
            PdfObject actualDestinationObject = dests.GetEntry((PdfString)stringDestination.GetPdfObject());
            if (actualDestinationObject is PdfDictionary) {
                return IsDestinationAllowed((PdfDictionary)actualDestinationObject, document, counter);
            }
            return IsDestinationAllowed(actualDestinationObject, document, counter);
        }

        private static bool IsDestinationAllowed(PdfDictionary destDictionary, PdfDocument document, int counter) {
            if (destDictionary == null) {
                return true;
            }
            bool isSdPresent = destDictionary.Get(PdfName.SD) != null && PdfDestination.MakeDestination(destDictionary
                .Get(PdfName.SD)) != null;
            if (!IsDestinationAllowed(destDictionary.Get(PdfName.SD), document, counter)) {
                return false;
            }
            // We only check D entry if SD is not present.
            return isSdPresent || IsDestinationAllowed(destDictionary.Get(PdfName.D), document, counter);
        }

        private static bool IsActionAllowed(PdfAction action, PdfDocument document, int counter) {
            return action == null || IsDestinationAllowed(action.GetPdfObject(), document, counter);
        }
    }
}
