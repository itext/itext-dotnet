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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Kernel.Pdf.Action {
    /// <summary>
    /// A target dictionary locates the target in relation to the source,
    /// in much the same way that a relative path describes the physical
    /// relationship between two files in a file system.
    /// </summary>
    /// <remarks>
    /// A target dictionary locates the target in relation to the source,
    /// in much the same way that a relative path describes the physical
    /// relationship between two files in a file system. Target dictionaries may be
    /// nested recursively to specify one or more intermediate targets before reaching the final one.
    /// </remarks>
    public class PdfTarget : PdfObjectWrapper<PdfDictionary> {
        private PdfTarget(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfTarget"/>
        /// object by the underlying dictionary.
        /// </summary>
        /// <param name="pdfObject">the underlying dictionary object</param>
        /// <returns>
        /// a new
        /// <see cref="PdfTarget"/>
        /// object by the underlying dictionary
        /// </returns>
        public static iText.Kernel.Pdf.Action.PdfTarget Create(PdfDictionary pdfObject) {
            return new iText.Kernel.Pdf.Action.PdfTarget(pdfObject);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfTarget"/>
        /// object given its type.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="PdfTarget"/>
        /// object given its type. The type must be either
        /// <see cref="iText.Kernel.Pdf.PdfName.P"/>
        /// , or
        /// <see cref="iText.Kernel.Pdf.PdfName.C"/>
        /// . If it is
        /// <see cref="iText.Kernel.Pdf.PdfName.C"/>
        /// , additional entries must be specified
        /// according to the spec.
        /// </remarks>
        /// <param name="r">the relationship between the current document and the target</param>
        private static iText.Kernel.Pdf.Action.PdfTarget Create(PdfName r) {
            iText.Kernel.Pdf.Action.PdfTarget pdfTarget = new iText.Kernel.Pdf.Action.PdfTarget(new PdfDictionary());
            pdfTarget.Put(PdfName.R, r);
            return pdfTarget;
        }

        /// <summary>Creates a new target object pointing to the parent of the current document.</summary>
        /// <returns>
        /// created
        /// <see cref="PdfTarget"/>
        /// </returns>
        public static iText.Kernel.Pdf.Action.PdfTarget CreateParentTarget() {
            return iText.Kernel.Pdf.Action.PdfTarget.Create(PdfName.P);
        }

        /// <summary>Creates a new target object pointing to the child of the current document.</summary>
        /// <returns>
        /// created
        /// <see cref="PdfTarget"/>
        /// </returns>
        public static iText.Kernel.Pdf.Action.PdfTarget CreateChildTarget() {
            return iText.Kernel.Pdf.Action.PdfTarget.Create(PdfName.C);
        }

        /// <summary>Creates a new target object pointing to a file in the EmbeddedFiles name tree.</summary>
        /// <param name="embeddedFileName">the name of the file in the EmbeddedFiles name tree</param>
        /// <returns>created object</returns>
        public static iText.Kernel.Pdf.Action.PdfTarget CreateChildTarget(String embeddedFileName) {
            return iText.Kernel.Pdf.Action.PdfTarget.Create(PdfName.C).Put(PdfName.N, new PdfString(embeddedFileName));
        }

        /// <summary>Creates a new target object pointing to a file attachment annotation.</summary>
        /// <param name="namedDestination">
        /// a named destination in the current document that
        /// provides the page number of the file attachment annotation
        /// </param>
        /// <param name="annotationIdentifier">
        /// a unique annotation identifier (
        /// <see cref="iText.Kernel.Pdf.PdfName.NM"/>
        /// entry) of the annotation
        /// </param>
        /// <returns>created object</returns>
        public static iText.Kernel.Pdf.Action.PdfTarget CreateChildTarget(String namedDestination, String annotationIdentifier
            ) {
            return iText.Kernel.Pdf.Action.PdfTarget.Create(PdfName.C).Put(PdfName.P, new PdfString(namedDestination))
                .Put(PdfName.A, new PdfString(annotationIdentifier));
        }

        /// <summary>Creates a new target object pointing to a file attachment annotation.</summary>
        /// <param name="pageNumber">the number of the page in the current document, one-based</param>
        /// <param name="annotationIndex">the index of the annotation in the Annots entry of the page, zero-based</param>
        /// <returns>created object</returns>
        public static iText.Kernel.Pdf.Action.PdfTarget CreateChildTarget(int pageNumber, int annotationIndex) {
            return iText.Kernel.Pdf.Action.PdfTarget.Create(PdfName.C).Put(PdfName.P, new PdfNumber(pageNumber - 1)).Put
                (PdfName.A, new PdfNumber(annotationIndex));
        }

        /// <summary>
        /// Sets the name of the file in the EmbeddedFiles name tree for the child target located
        /// in the EmbeddedFiles.
        /// </summary>
        /// <param name="name">the name of the file</param>
        /// <returns>this object wrapper</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTarget SetName(String name) {
            return Put(PdfName.N, new PdfString(name));
        }

        /// <summary>
        /// Gets name of the file in the EmbeddedFiles name tree for the child target located
        /// in the EmbeddedFiles.
        /// </summary>
        /// <returns>the name of the child file for this target</returns>
        public virtual String GetName() {
            return GetPdfObject().GetAsString(PdfName.N).ToString();
        }

        /// <summary>Sets the /P and /A values corresponding to provided annotation, which is already added to a page.
        ///     </summary>
        /// <param name="pdfAnnotation">the annotation to be set</param>
        /// <param name="pdfDocument">the corresponding document</param>
        /// <returns>this object wrapper</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTarget SetAnnotation(PdfFileAttachmentAnnotation pdfAnnotation, 
            PdfDocument pdfDocument) {
            PdfPage page = pdfAnnotation.GetPage();
            if (null == page) {
                throw new PdfException(KernelExceptionMessageConstant.ANNOTATION_SHALL_HAVE_REFERENCE_TO_PAGE);
            }
            else {
                Put(PdfName.P, new PdfNumber(pdfDocument.GetPageNumber(page) - 1));
                int indexOfAnnotation = -1;
                IList<PdfAnnotation> annots = page.GetAnnotations();
                for (int i = 0; i < annots.Count; i++) {
                    if (annots[i] != null && pdfAnnotation.GetPdfObject().Equals(annots[i].GetPdfObject())) {
                        indexOfAnnotation = i;
                        break;
                    }
                }
                Put(PdfName.A, new PdfNumber(indexOfAnnotation));
            }
            return this;
        }

        /// <summary>Gets the annotation specified by /A and /P entry values.</summary>
        /// <param name="pdfDocument">specifies the corresponding document</param>
        /// <returns>the annotation specified by /A and /P entry value.</returns>
        public virtual PdfFileAttachmentAnnotation GetAnnotation(PdfDocument pdfDocument) {
            PdfObject pValue = GetPdfObject().Get(PdfName.P);
            PdfPage page = null;
            if (pValue is PdfNumber) {
                // zero-based index is used
                page = pdfDocument.GetPage(((PdfNumber)pValue).IntValue() + 1);
            }
            else {
                if (pValue is PdfString) {
                    PdfNameTree destsTree = pdfDocument.GetCatalog().GetNameTree(PdfName.Dests);
                    IDictionary<String, PdfObject> dests = destsTree.GetNames();
                    PdfArray pdfArray = (PdfArray)dests.Get(((PdfString)pValue).GetValue());
                    if (null != pdfArray) {
                        if (pdfArray.Get(0) is PdfNumber) {
                            page = pdfDocument.GetPage(((PdfNumber)pdfArray.Get(0)).IntValue());
                        }
                        else {
                            page = pdfDocument.GetPage((PdfDictionary)pdfArray.Get(0));
                        }
                    }
                }
            }
            IList<PdfAnnotation> pageAnnotations = null;
            if (null != page) {
                pageAnnotations = page.GetAnnotations();
            }
            PdfObject aValue = GetPdfObject().Get(PdfName.A);
            PdfFileAttachmentAnnotation resultAnnotation = null;
            if (null != pageAnnotations) {
                if (aValue is PdfNumber) {
                    resultAnnotation = (PdfFileAttachmentAnnotation)pageAnnotations[((PdfNumber)aValue).IntValue()];
                }
                else {
                    if (aValue is PdfString) {
                        foreach (PdfAnnotation annotation in pageAnnotations) {
                            if (aValue.Equals(annotation.GetName())) {
                                resultAnnotation = (PdfFileAttachmentAnnotation)annotation;
                                break;
                            }
                        }
                    }
                }
            }
            if (null == resultAnnotation) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Action.PdfTarget));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.SOME_TARGET_FIELDS_ARE_NOT_SET_OR_INCORRECT);
            }
            return resultAnnotation;
        }

        /// <summary>Sets a target dictionary specifying additional path information to the target document.</summary>
        /// <remarks>
        /// Sets a target dictionary specifying additional path information to the target document.
        /// If this entry is absent, the current document is the target file containing the destination.
        /// </remarks>
        /// <param name="target">the additional path target dictionary</param>
        /// <returns>this object wrapper</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTarget SetTarget(iText.Kernel.Pdf.Action.PdfTarget target) {
            return Put(PdfName.T, target.GetPdfObject());
        }

        /// <summary>Get a target dictionary specifying additional path information to the target document.</summary>
        /// <remarks>
        /// Get a target dictionary specifying additional path information to the target document.
        /// If the current target object is the final node in the target path, <c>null</c> is returned.
        /// </remarks>
        /// <returns>a target dictionary specifying additional path information to the target document</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTarget GetTarget() {
            PdfDictionary targetDictObject = GetPdfObject().GetAsDictionary(PdfName.T);
            return targetDictObject != null ? new iText.Kernel.Pdf.Action.PdfTarget(targetDictObject) : null;
        }

        /// <summary>
        /// This is a convenient method to put key-value pairs to the underlying
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>.
        /// </summary>
        /// <param name="key">
        /// the key, a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// instance
        /// </param>
        /// <param name="value">the value</param>
        /// <returns>this object wrapper</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTarget Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
