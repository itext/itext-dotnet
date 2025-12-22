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
using iText.Commons.Utils.Collections;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Class that provides methods for checking PDF/UA-2 compliance of link annotations.</summary>
    public sealed class PdfUA2LinkChecker {
        private readonly PdfDocument pdfDoc;

        private readonly PdfUAValidationContext context;

        private readonly IDictionary<PdfObject, ICollection<IStructureNode>> destinationToStructParentsMap = new Dictionary
            <PdfObject, ICollection<IStructureNode>>();

        private PdfUA2LinkChecker(PdfUAValidationContext context, PdfDocument pdfDoc) {
            this.context = context;
            this.pdfDoc = pdfDoc;
        }

        /// <summary>Verifies that each link annotation present in the document is tagged.</summary>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to check links for
        /// </param>
        public static void CheckLinkAnnotations(PdfDocument document) {
            int amountOfPages = document.GetNumberOfPages();
            for (int i = 1; i <= amountOfPages; ++i) {
                PdfPage page = document.GetPage(i);
                foreach (PdfAnnotation annot in page.GetAnnotations()) {
                    if (!(annot is PdfLinkAnnotation)) {
                        continue;
                    }
                    if (annot.GetStructParentIndex() == -1) {
                        throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK_OR_REFERENCE
                            );
                    }
                }
            }
        }

        /// <summary>Checks that link annotation is enclosed in either a Link or Reference structure element.</summary>
        /// <remarks>
        /// Checks that link annotation is enclosed in either a Link or Reference structure element.
        /// <para />
        /// Also checks that link annotations that target different locations are in separate Link or Reference structure
        /// elements, and multiple link annotations targeting the same location are included in a single Link or Reference
        /// structure element.
        /// </remarks>
        /// <param name="elem">link annotation object reference in the structure tree</param>
        private void CheckLinkAnnotationStructureParent(IStructureNode elem) {
            if (!(elem is PdfObjRef) || ((PdfObjRef)elem).GetReferencedObject() == null) {
                return;
            }
            PdfName subtype = ((PdfObjRef)elem).GetReferencedObject().GetAsName(PdfName.Subtype);
            if (!PdfName.Link.Equals(subtype)) {
                return;
            }
            IStructureNode linkParent = elem.GetParent();
            PdfStructElem parentLink = context.GetElementIfRoleMatches(PdfName.Link, linkParent);
            if (parentLink == null) {
                PdfStructElem parentRef = context.GetElementIfRoleMatches(PdfName.Reference, linkParent);
                if (parentRef == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK_OR_REFERENCE
                        );
                }
            }
            CheckStructDestinationsInLinkAndReference((PdfObjRef)elem);
        }

        /// <summary>
        /// Checks that link annotations that target different locations (destinations) are in separate Link or Reference
        /// structure elements, and multiple link annotations targeting the same location are included in a single Link
        /// or Reference structure element.
        /// </summary>
        /// <param name="objRef">link annotation object reference in the structure tree</param>
        private void CheckStructDestinationsInLinkAndReference(PdfObjRef objRef) {
            IStructureNode parent = objRef.GetParent();
            if (parent == null) {
                return;
            }
            PdfObject structDestination = GetStructureDestinationObject(objRef.GetReferencedObject());
            if (structDestination == null) {
                return;
            }
            // Go through all other already checked destinations. They shall have separate Link or Reference structure
            // elements, so no other parent should be equal to the current one. Otherwise, exception will be thrown.
            foreach (KeyValuePair<PdfObject, ICollection<IStructureNode>> entry in destinationToStructParentsMap) {
                if (structDestination.Equals(entry.Key)) {
                    // Skip current destination.
                    continue;
                }
                foreach (IStructureNode parentNode in entry.Value) {
                    if (parent.Equals(parentNode)) {
                        throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DIFFERENT_LINKS_IN_SINGLE_STRUCT_ELEM);
                    }
                }
            }
            // In the map, key is a destination object from current link annotation, value is a set of Link or Reference
            // structure elements enclosing already checked links annotation with that same destination (actually, value
            // always contains either 0 or 1 parent, it's just more convenient to use set during checks).
            ICollection<IStructureNode> destinationStructParents = destinationToStructParentsMap.ComputeIfAbsent(structDestination
                , (k) => new HashSet<IStructureNode>());
            // Add current parent to the map.
            destinationStructParents.Add(parent);
        }

        private PdfObject GetStructureDestinationObject(PdfDictionary annotObj) {
            PdfLinkAnnotation linkAnnotation = (PdfLinkAnnotation)PdfAnnotation.MakeAnnotation(annotObj);
            PdfObject destination = null;
            PdfDictionary action = linkAnnotation.GetAction();
            if (action != null) {
                if (PdfName.GoTo.Equals(action.GetAsName(PdfName.S))) {
                    destination = action.Get(PdfName.SD);
                    if (destination == null) {
                        destination = action.Get(PdfName.D);
                    }
                }
            }
            else {
                destination = linkAnnotation.GetDestinationObject();
            }
            if (destination == null) {
                return null;
            }
            PdfArray dest = GetDestination(destination);
            if (dest == null || dest.IsEmpty()) {
                return null;
            }
            else {
                return dest.Get(0);
            }
        }

        private PdfArray GetDestination(PdfObject destination) {
            return GetDestination(destination, new HashSet<PdfObject>());
        }

        private PdfArray GetDestination(PdfObject destination, ICollection<PdfObject> checkedDestinations) {
            if (destination == null || checkedDestinations.Contains(destination)) {
                return null;
            }
            checkedDestinations.Add(destination);
            switch (destination.GetObjectType()) {
                case PdfObject.STRING: {
                    PdfNameTree destinations = pdfDoc.GetCatalog().GetNameTree(PdfName.Dests);
                    destination = GetDestination(destinations.GetEntry((PdfString)destination), checkedDestinations);
                    break;
                }

                case PdfObject.NAME: {
                    PdfDictionary dests = pdfDoc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Dests);
                    if (dests != null) {
                        destination = GetDestination(dests.Get((PdfName)destination), checkedDestinations);
                    }
                    break;
                }

                case PdfObject.ARRAY: {
                    break;
                }

                case PdfObject.DICTIONARY: {
                    PdfObject actualDestinationObject = GetDestination(((PdfDictionary)destination).Get(PdfName.SD), checkedDestinations
                        );
                    if (actualDestinationObject == null) {
                        destination = GetDestination(((PdfDictionary)destination).Get(PdfName.D), checkedDestinations);
                    }
                    else {
                        destination = actualDestinationObject;
                    }
                    break;
                }

                default: {
                    return null;
                }
            }
            if (destination is PdfArray) {
                return (PdfArray)destination;
            }
            return null;
        }

        /// <summary>Helper class that checks the conformance of link annotations while iterating the tag tree structure.
        ///     </summary>
        public class PdfUA2LinkAnnotationHandler : ContextAwareTagTreeIteratorHandler {
            private readonly PdfUA2LinkChecker checker;

            /// <summary>
            /// Creates a new instance of the
            /// <see cref="PdfUA2LinkAnnotationHandler"/>.
            /// </summary>
            /// <param name="context">the validation context</param>
            /// <param name="document">
            /// the
            /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
            /// to check link annotations for
            /// </param>
            public PdfUA2LinkAnnotationHandler(PdfUAValidationContext context, PdfDocument document)
                : base(context) {
                this.checker = new PdfUA2LinkChecker(context, document);
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                this.checker.CheckLinkAnnotationStructureParent(elem);
            }
        }
    }
}
