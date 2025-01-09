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
using iText.Commons.Utils;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    /// <summary>A copy filter that will handle goto annotations and actions separately.</summary>
    public class DestinationResolverCopyFilter : ICopyFilter {
        private static readonly IList<PdfName> EXCLUDE_KEYS_ACTIONCOPY = JavaCollectionsUtil.SingletonList(PdfName
            .D);

        private readonly PdfDocument targetDocument;

        private readonly PdfDocument fromDocument;

        /// <summary>
        /// Initilazes a copy filter that will set all needed information aside to handle objects with a page destination
        /// after all pages are copied.
        /// </summary>
        /// <remarks>
        /// Initilazes a copy filter that will set all needed information aside to handle objects with a page destination
        /// after all pages are copied.
        /// <para />
        /// </remarks>
        /// <param name="fromDocument">
        /// the
        /// <see cref="PdfDocument"/>
        /// the pages are copied from
        /// </param>
        /// <param name="targetDocument">
        /// the
        /// <see cref="PdfDocument"/>
        /// the pages are copied to
        /// </param>
        public DestinationResolverCopyFilter(PdfDocument fromDocument, PdfDocument targetDocument) {
            this.fromDocument = fromDocument;
            this.targetDocument = targetDocument;
        }

        public virtual bool ShouldProcess(PdfObject newParent, PdfName name, PdfObject value) {
            PdfObject workRef = GetDirectPdfObject(value);
            if (workRef.GetObjectType() == PdfObject.DICTIONARY) {
                PdfDictionary dict = (PdfDictionary)workRef;
                // a goto action
                if (dict.GetAsName(PdfName.S) == PdfName.GoTo) {
                    ProcessAction(newParent, name, dict);
                    return false;
                }
                // a link annotation with destination
                if (PdfName.Link.Equals(dict.GetAsName(PdfName.Subtype)) && newParent.IsDictionary()) {
                    return ProcessLinkAnnotion(newParent, value, dict);
                }
            }
            return true;
        }

        private bool ProcessLinkAnnotion(PdfObject newParent, PdfObject value, PdfDictionary dict) {
            PdfObject destination = dict.Get(PdfName.Dest);
            if (destination != null && !destination.Equals(PdfNull.PDF_NULL)) {
                fromDocument.StoreDestinationToReaddress(PdfDestination.MakeDestination(destination), (nd) => {
                    PdfObject newVal = value.CopyTo(targetDocument, this);
                    (new PdfPage((PdfDictionary)newParent)).AddAnnotation(-1, PdfAnnotation.MakeAnnotation(newVal), false);
                }
                , (od) => {
                }
                );
                //do nothing
                return false;
            }
            if (dict.GetAsDictionary(PdfName.A) != null && dict.GetAsDictionary(PdfName.A).Get(PdfName.D) != null && !
                PdfNull.PDF_NULL.Equals(dict.GetAsDictionary(PdfName.A).Get(PdfName.D)) && !PdfName.GoToR.Equals(dict.
                GetAsDictionary(PdfName.A).Get(PdfName.S))) {
                fromDocument.StoreDestinationToReaddress(PdfDestination.MakeDestination(dict.GetAsDictionary(PdfName.A).Get
                    (PdfName.D)), (nd) => {
                    PdfObject newAnnot = value.CopyTo(targetDocument);
                    ((PdfDictionary)newAnnot).GetAsDictionary(PdfName.A).Put(PdfName.D, nd.GetPdfObject());
                    (new PdfPage((PdfDictionary)newParent)).AddAnnotation(-1, PdfAnnotation.MakeAnnotation(newAnnot), false);
                }
                , (od) => {
                }
                );
                //do nothing
                return false;
            }
            return true;
        }

        private void ProcessAction(PdfObject newParent, PdfName name, PdfDictionary dict) {
            PdfObject destination = dict.Get(PdfName.D);
            if (destination == null || PdfNull.PDF_NULL.Equals(destination)) {
                return;
            }
            fromDocument.StoreDestinationToReaddress(PdfDestination.MakeDestination(destination), (nd) => {
                //Add action with new destination
                PdfObject newVal = dict.CopyTo(targetDocument, EXCLUDE_KEYS_ACTIONCOPY, false);
                ((PdfDictionary)newVal).Put(PdfName.D, nd.GetPdfObject());
                if (newParent.GetObjectType() == PdfObject.DICTIONARY) {
                    ((PdfDictionary)newParent).Put(name, newVal);
                }
                else {
                    ((PdfArray)newParent).Add(newVal);
                }
            }
            , (od) => {
            }
            );
        }

        //do nothing
        private static PdfObject GetDirectPdfObject(PdfObject value) {
            PdfObject workRef = value;
            if (value.IsIndirectReference()) {
                workRef = ((PdfIndirectReference)value).GetRefersTo();
            }
            return workRef;
        }
    }
}
