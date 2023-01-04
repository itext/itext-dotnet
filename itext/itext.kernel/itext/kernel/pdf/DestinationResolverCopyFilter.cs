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
            if (dict.Get(PdfName.Dest) != null) {
                fromDocument.StoreDestinationToReaddress(PdfDestination.MakeDestination(dict.Get(PdfName.Dest)), (nd) => {
                    PdfObject newVal = value.CopyTo(targetDocument, this);
                    (new PdfPage((PdfDictionary)newParent)).AddAnnotation(-1, PdfAnnotation.MakeAnnotation(newVal), false);
                }
                , (od) => {
                }
                );
                //do nothing
                return false;
            }
            if (dict.GetAsDictionary(PdfName.A) != null && dict.GetAsDictionary(PdfName.A).Get(PdfName.D) != null) {
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
            fromDocument.StoreDestinationToReaddress(PdfDestination.MakeDestination(dict.Get(PdfName.D)), (nd) => {
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
