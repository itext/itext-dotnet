/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.Kernel.Pdf;

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
    public class PdfTargetDictionary : PdfObjectWrapper<PdfDictionary> {
        private PdfTargetDictionary(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfTargetDictionary"/>
        /// object by the underlying dictionary.
        /// </summary>
        /// <param name="pdfObject">the underlying dictionary object</param>
        public static iText.Kernel.Pdf.Action.PdfTargetDictionary Create(PdfDictionary pdfObject) {
            return new iText.Kernel.Pdf.Action.PdfTargetDictionary(pdfObject);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfTargetDictionary"/>
        /// object given its type. The type must be either
        /// <see cref="iText.Kernel.Pdf.PdfName.P"/>
        /// , or
        /// <see cref="iText.Kernel.Pdf.PdfName.C"/>
        /// . If it is
        /// <see cref="iText.Kernel.Pdf.PdfName.C"/>
        /// , additional entries must be specified
        /// according to the spec.
        /// </summary>
        /// <param name="r">the relationship between the current document and the target</param>
        public static iText.Kernel.Pdf.Action.PdfTargetDictionary Create(PdfName r) {
            iText.Kernel.Pdf.Action.PdfTargetDictionary pdfTargetDictionary = new iText.Kernel.Pdf.Action.PdfTargetDictionary
                (new PdfDictionary());
            pdfTargetDictionary.Put(PdfName.R, r);
            return pdfTargetDictionary;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfTargetDictionary"/>
        /// object.
        /// </summary>
        /// <param name="r">the relationship between the current document and the target</param>
        /// <param name="n">the name of the file in the EmbeddedFiles name tree</param>
        /// <param name="p">
        /// if the value is an integer, it specifies the page number (zero-based) in the current
        /// document containing the file attachment annotation. If the value is a string,
        /// it specifies a named destination in the current document that provides the page
        /// number of the file attachment annotation
        /// </param>
        /// <param name="a">
        /// If the value is an integer, it specifies the index (zero-based) of the annotation in the
        /// Annots array of the page specified by P. If the value is a text string,
        /// it specifies the value of NM in the annotation dictionary
        /// </param>
        /// <param name="t">
        /// A target dictionary specifying additional path information to the target document.
        /// If this entry is absent, the current document is the target file containing the destination
        /// </param>
        public static iText.Kernel.Pdf.Action.PdfTargetDictionary Create(PdfName r, PdfString n, PdfObject p, PdfObject
             a, iText.Kernel.Pdf.Action.PdfTargetDictionary t) {
            iText.Kernel.Pdf.Action.PdfTargetDictionary pdfTargetDictionary = new iText.Kernel.Pdf.Action.PdfTargetDictionary
                (new PdfDictionary());
            pdfTargetDictionary.Put(PdfName.R, r).Put(PdfName.N, n).Put(PdfName.P, p).Put(PdfName.A, a).Put(PdfName.T, 
                t.GetPdfObject());
            return pdfTargetDictionary;
        }

        /// <summary>Creates a new target object pointing to the parent of the current document.</summary>
        /// <returns>
        /// created
        /// <see cref="PdfTargetDictionary"/>
        /// </returns>
        public static iText.Kernel.Pdf.Action.PdfTargetDictionary CreateParentTarget() {
            return iText.Kernel.Pdf.Action.PdfTargetDictionary.Create(PdfName.P);
        }

        /// <summary>Creates a new target object pointing to a file in the EmbeddedFiles name tree.</summary>
        /// <param name="embeddedFileName">the name of the file in the EmbeddedFiles name tree</param>
        /// <returns>created object</returns>
        public static iText.Kernel.Pdf.Action.PdfTargetDictionary CreateChildTarget(String embeddedFileName) {
            return iText.Kernel.Pdf.Action.PdfTargetDictionary.Create(PdfName.C).Put(PdfName.N, new PdfString(embeddedFileName
                ));
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
        public static iText.Kernel.Pdf.Action.PdfTargetDictionary CreateChildTarget(String namedDestination, String
             annotationIdentifier) {
            return iText.Kernel.Pdf.Action.PdfTargetDictionary.Create(PdfName.C).Put(PdfName.P, new PdfString(namedDestination
                )).Put(PdfName.A, new PdfString(annotationIdentifier));
        }

        /// <summary>Creates a new target object pointing to a file attachment annotation.</summary>
        /// <param name="pageNumber">the number of the page in the current document, one-based</param>
        /// <param name="annotationIndex">the index of the annotation in the Annots entry of the page, zero-based</param>
        /// <returns>created object</returns>
        public static iText.Kernel.Pdf.Action.PdfTargetDictionary CreateChildTarget(int pageNumber, int annotationIndex
            ) {
            return iText.Kernel.Pdf.Action.PdfTargetDictionary.Create(PdfName.C).Put(PdfName.P, new PdfNumber(pageNumber
                 - 1)).Put(PdfName.A, new PdfNumber(annotationIndex));
        }

        /// <summary>
        /// Sets the name of the file in the EmbeddedFiles name tree for the child target located
        /// in the EmbeddedFiles.
        /// </summary>
        /// <param name="name">the name of the file</param>
        /// <returns>this object wrapper</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTargetDictionary SetName(String name) {
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

        /// <summary>
        /// Sets the page number in the current document containing the file attachment annotation for the
        /// child target associates with a file attachment annotation.
        /// </summary>
        /// <param name="pageNumber">
        /// the page number (one-based) in the current document containing
        /// the file attachment annotation
        /// </param>
        /// <returns>this object wrapper</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTargetDictionary SetPage(int pageNumber) {
            return Put(PdfName.P, new PdfNumber(pageNumber - 1));
        }

        /// <summary>
        /// Sets a named destination in the current document that provides the page number of the file
        /// attachment annotation for the child target associated with a file attachment annotation.
        /// </summary>
        /// <param name="namedDestination">
        /// a named destination in the current document that provides the page
        /// number of the file attachment annotation
        /// </param>
        /// <returns>this object wrapper</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTargetDictionary SetPage(String namedDestination) {
            return Put(PdfName.P, new PdfString(namedDestination));
        }

        /// <summary>Get the contents of the /P entry of this target object.</summary>
        /// <remarks>
        /// Get the contents of the /P entry of this target object.
        /// If the value is an integer, it specifies the page number (zero-based)
        /// in the current document containing the file attachment annotation.
        /// If the value is a string, it specifies a named destination in the current
        /// document that provides the page number of the file attachment annotation.
        /// </remarks>
        /// <returns>the /P entry of target object</returns>
        public virtual PdfObject GetPage() {
            return GetPdfObject().Get(PdfName.P);
        }

        /// <summary>
        /// Sets the index of the annotation in Annots array of the page specified by /P entry
        /// for the child target associated with a file attachment annotation.
        /// </summary>
        /// <param name="annotationIndex">the index (zero-based) of the annotation in the Annots array</param>
        /// <returns>this object wrapper</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTargetDictionary SetAnnotation(int annotationIndex) {
            return Put(PdfName.A, new PdfNumber(annotationIndex));
        }

        /// <summary>
        /// Sets the text value, which uniquely identifies an annotation (/NM entry) in an annotation dictionary
        /// for the child target associated with a file attachment annotation.
        /// </summary>
        /// <param name="annotationName">specifies the value of NM in the annotation dictionary of the target annotation
        ///     </param>
        /// <returns>this object wrapper</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTargetDictionary SetAnnotation(String annotationName) {
            return Put(PdfName.A, new PdfString(annotationName));
        }

        /// <summary>Gets the object in the /A entry of the underlying object.</summary>
        /// <remarks>
        /// Gets the object in the /A entry of the underlying object. If the value is an integer,
        /// it specifies the index (zero-based) of the annotation in the Annots array of the page specified by P.
        /// If the value is a text string, it specifies the value of NM in the annotation dictionary.
        /// </remarks>
        /// <returns>the /A entry in the target object</returns>
        public virtual PdfObject GetAnnotation() {
            return GetPdfObject().Get(PdfName.A);
        }

        /// <summary>Sets a target dictionary specifying additional path information to the target document.</summary>
        /// <remarks>
        /// Sets a target dictionary specifying additional path information to the target document.
        /// If this entry is absent, the current document is the target file containing the destination.
        /// </remarks>
        /// <param name="target">the additional path target dictionary</param>
        /// <returns>this object wrapper</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTargetDictionary SetTarget(iText.Kernel.Pdf.Action.PdfTargetDictionary
             target) {
            return Put(PdfName.T, target.GetPdfObject());
        }

        /// <summary>Get a target dictionary specifying additional path information to the target document.</summary>
        /// <remarks>
        /// Get a target dictionary specifying additional path information to the target document.
        /// If the current target object is the final node in the target path, <code>null</code> is returned.
        /// </remarks>
        /// <returns>a target dictionary specifying additional path information to the target document</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTargetDictionary GetTarget() {
            PdfDictionary targetDictObject = GetPdfObject().GetAsDictionary(PdfName.T);
            return targetDictObject != null ? new iText.Kernel.Pdf.Action.PdfTargetDictionary(targetDictObject) : null;
        }

        /// <summary>
        /// This is a convenient method to put key-value pairs to the underlying
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// .
        /// </summary>
        /// <param name="key">
        /// the key, a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// instance
        /// </param>
        /// <param name="value">the value</param>
        /// <returns>this object wrapper</returns>
        public virtual iText.Kernel.Pdf.Action.PdfTargetDictionary Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
