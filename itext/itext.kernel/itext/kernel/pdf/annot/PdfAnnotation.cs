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
using iText.IO.Font;
using iText.IO.Log;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Layer;

namespace iText.Kernel.Pdf.Annot {
    /// <summary>This is a super class for the annotation dictionary wrappers.</summary>
    /// <remarks>
    /// This is a super class for the annotation dictionary wrappers. Derived classes represent
    /// different standard types of annotations. See ISO-320001 12.5.6, “Annotation Types.”
    /// </remarks>
    public abstract class PdfAnnotation : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Annotation flag.</summary>
        /// <remarks>
        /// Annotation flag.
        /// See also
        /// <see cref="SetFlag(int)"/>
        /// and ISO-320001, table 165.
        /// </remarks>
        public const int INVISIBLE = 1;

        /// <summary>Annotation flag.</summary>
        /// <remarks>
        /// Annotation flag.
        /// See also
        /// <see cref="SetFlag(int)"/>
        /// and ISO-320001, table 165.
        /// </remarks>
        public const int HIDDEN = 2;

        /// <summary>Annotation flag.</summary>
        /// <remarks>
        /// Annotation flag.
        /// See also
        /// <see cref="SetFlag(int)"/>
        /// and ISO-320001, table 165.
        /// </remarks>
        public const int PRINT = 4;

        /// <summary>Annotation flag.</summary>
        /// <remarks>
        /// Annotation flag.
        /// See also
        /// <see cref="SetFlag(int)"/>
        /// and ISO-320001, table 165.
        /// </remarks>
        public const int NO_ZOOM = 8;

        /// <summary>Annotation flag.</summary>
        /// <remarks>
        /// Annotation flag.
        /// See also
        /// <see cref="SetFlag(int)"/>
        /// and ISO-320001, table 165.
        /// </remarks>
        public const int NO_ROTATE = 16;

        /// <summary>Annotation flag.</summary>
        /// <remarks>
        /// Annotation flag.
        /// See also
        /// <see cref="SetFlag(int)"/>
        /// and ISO-320001, table 165.
        /// </remarks>
        public const int NO_VIEW = 32;

        /// <summary>Annotation flag.</summary>
        /// <remarks>
        /// Annotation flag.
        /// See also
        /// <see cref="SetFlag(int)"/>
        /// and ISO-320001, table 165.
        /// </remarks>
        public const int READ_ONLY = 64;

        /// <summary>Annotation flag.</summary>
        /// <remarks>
        /// Annotation flag.
        /// See also
        /// <see cref="SetFlag(int)"/>
        /// and ISO-320001, table 165.
        /// </remarks>
        public const int LOCKED = 128;

        /// <summary>Annotation flag.</summary>
        /// <remarks>
        /// Annotation flag.
        /// See also
        /// <see cref="SetFlag(int)"/>
        /// and ISO-320001, table 165.
        /// </remarks>
        public const int TOGGLE_NO_VIEW = 256;

        /// <summary>Annotation flag.</summary>
        /// <remarks>
        /// Annotation flag.
        /// See also
        /// <see cref="SetFlag(int)"/>
        /// and ISO-320001, table 165.
        /// </remarks>
        public const int LOCKED_CONTENTS = 512;

        /// <summary>Widget annotation highlighting mode.</summary>
        /// <remarks>
        /// Widget annotation highlighting mode. See ISO-320001, Table 188 (H key).
        /// Also see
        /// <see cref="PdfWidgetAnnotation.SetHighlightMode(iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// </remarks>
        public static readonly PdfName HIGHLIGHT_NONE = PdfName.N;

        /// <summary>Widget annotation highlighting mode.</summary>
        /// <remarks>
        /// Widget annotation highlighting mode. See ISO-320001, Table 188 (H key).
        /// Also see
        /// <see cref="PdfWidgetAnnotation.SetHighlightMode(iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// </remarks>
        public static readonly PdfName HIGHLIGHT_INVERT = PdfName.I;

        /// <summary>Widget annotation highlighting mode.</summary>
        /// <remarks>
        /// Widget annotation highlighting mode. See ISO-320001, Table 188 (H key).
        /// Also see
        /// <see cref="PdfWidgetAnnotation.SetHighlightMode(iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// </remarks>
        public static readonly PdfName HIGHLIGHT_OUTLINE = PdfName.O;

        /// <summary>Widget annotation highlighting mode.</summary>
        /// <remarks>
        /// Widget annotation highlighting mode. See ISO-320001, Table 188 (H key).
        /// Also see
        /// <see cref="PdfWidgetAnnotation.SetHighlightMode(iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// </remarks>
        public static readonly PdfName HIGHLIGHT_PUSH = PdfName.P;

        /// <summary>Widget annotation highlighting mode.</summary>
        /// <remarks>
        /// Widget annotation highlighting mode. See ISO-320001, Table 188 (H key).
        /// Also see
        /// <see cref="PdfWidgetAnnotation.SetHighlightMode(iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// </remarks>
        public static readonly PdfName HIGHLIGHT_TOGGLE = PdfName.T;

        /// <summary>Annotation border style.</summary>
        /// <remarks>
        /// Annotation border style. See ISO-320001, Table 166 (S key).
        /// Also see
        /// <see cref="SetBorderStyle(iText.Kernel.Pdf.PdfName)"/>
        /// </remarks>
        public static readonly PdfName STYLE_SOLID = PdfName.S;

        /// <summary>Annotation border style.</summary>
        /// <remarks>
        /// Annotation border style. See ISO-320001, Table 166 (S key).
        /// Also see
        /// <see cref="SetBorderStyle(iText.Kernel.Pdf.PdfName)"/>
        /// </remarks>
        public static readonly PdfName STYLE_DASHED = PdfName.D;

        /// <summary>Annotation border style.</summary>
        /// <remarks>
        /// Annotation border style. See ISO-320001, Table 166 (S key).
        /// Also see
        /// <see cref="SetBorderStyle(iText.Kernel.Pdf.PdfName)"/>
        /// </remarks>
        public static readonly PdfName STYLE_BEVELED = PdfName.B;

        /// <summary>Annotation border style.</summary>
        /// <remarks>
        /// Annotation border style. See ISO-320001, Table 166 (S key).
        /// Also see
        /// <see cref="SetBorderStyle(iText.Kernel.Pdf.PdfName)"/>
        /// </remarks>
        public static readonly PdfName STYLE_INSET = PdfName.I;

        /// <summary>Annotation border style.</summary>
        /// <remarks>
        /// Annotation border style. See ISO-320001, Table 166 (S key).
        /// Also see
        /// <see cref="SetBorderStyle(iText.Kernel.Pdf.PdfName)"/>
        /// </remarks>
        public static readonly PdfName STYLE_UNDERLINE = PdfName.U;

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>
        /// .
        /// </remarks>
        public static readonly PdfString Marked = new PdfString("Marked");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>
        /// .
        /// </remarks>
        public static readonly PdfString Unmarked = new PdfString("Unmarked");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>
        /// .
        /// </remarks>
        public static readonly PdfString Accepted = new PdfString("Accepted");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>
        /// .
        /// </remarks>
        public static readonly PdfString Rejected = new PdfString("Rejected");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>
        /// .
        /// </remarks>
        public static readonly PdfString Canceled = new PdfString("Cancelled");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>
        /// .
        /// </remarks>
        public static readonly PdfString Completed = new PdfString("Completed");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>
        /// .
        /// </remarks>
        public static readonly PdfString None = new PdfString("None");

        /// <summary>Annotation state model.</summary>
        /// <remarks>
        /// Annotation state model. See ISO-320001, Table 172 (StateModel key).
        /// Also see
        /// <see cref="PdfTextAnnotation.SetStateModel(iText.Kernel.Pdf.PdfString)"/>
        /// .
        /// </remarks>
        public static readonly PdfString MarkedModel = new PdfString("Marked");

        /// <summary>Annotation state model.</summary>
        /// <remarks>
        /// Annotation state model. See ISO-320001, Table 172 (StateModel key).
        /// Also see
        /// <see cref="PdfTextAnnotation.SetStateModel(iText.Kernel.Pdf.PdfString)"/>
        /// .
        /// </remarks>
        public static readonly PdfString ReviewModel = new PdfString("Review");

        protected internal PdfPage page;

        /// <summary>
        /// Factory method that creates the type specific
        /// <see cref="PdfAnnotation"/>
        /// from the given
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// that represents annotation object. This method is useful for property reading in reading mode or
        /// modifying in stamping mode. See derived classes of this class to see possible specific annotation types
        /// created.
        /// </summary>
        /// <param name="pdfObject">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// that represents annotation in the document.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfAnnotation"/>
        /// .
        /// </returns>
        public static iText.Kernel.Pdf.Annot.PdfAnnotation MakeAnnotation(PdfObject pdfObject) {
            iText.Kernel.Pdf.Annot.PdfAnnotation annotation = null;
            if (pdfObject.IsIndirectReference()) {
                pdfObject = ((PdfIndirectReference)pdfObject).GetRefersTo();
            }
            if (pdfObject.IsDictionary()) {
                PdfDictionary dictionary = (PdfDictionary)pdfObject;
                PdfName subtype = dictionary.GetAsName(PdfName.Subtype);
                if (PdfName.Link.Equals(subtype)) {
                    annotation = new PdfLinkAnnotation((PdfDictionary)pdfObject);
                }
                else {
                    if (PdfName.Popup.Equals(subtype)) {
                        annotation = new PdfPopupAnnotation((PdfDictionary)pdfObject);
                    }
                    else {
                        if (PdfName.Widget.Equals(subtype)) {
                            annotation = new PdfWidgetAnnotation((PdfDictionary)pdfObject);
                        }
                        else {
                            if (PdfName.Screen.Equals(subtype)) {
                                annotation = new PdfScreenAnnotation((PdfDictionary)pdfObject);
                            }
                            else {
                                if (PdfName._3D.Equals(subtype)) {
                                    throw new NotSupportedException();
                                }
                                else {
                                    if (PdfName.Highlight.Equals(subtype) || PdfName.Underline.Equals(subtype) || PdfName.Squiggly.Equals(subtype
                                        ) || PdfName.StrikeOut.Equals(subtype)) {
                                        annotation = new PdfTextMarkupAnnotation((PdfDictionary)pdfObject);
                                    }
                                    else {
                                        if (PdfName.Caret.Equals(subtype)) {
                                            annotation = new PdfCaretAnnotation((PdfDictionary)pdfObject);
                                        }
                                        else {
                                            if (PdfName.Text.Equals(subtype)) {
                                                annotation = new PdfTextAnnotation((PdfDictionary)pdfObject);
                                            }
                                            else {
                                                if (PdfName.Sound.Equals(subtype)) {
                                                    annotation = new PdfSoundAnnotation((PdfDictionary)pdfObject);
                                                }
                                                else {
                                                    if (PdfName.Stamp.Equals(subtype)) {
                                                        annotation = new PdfStampAnnotation((PdfDictionary)pdfObject);
                                                    }
                                                    else {
                                                        if (PdfName.FileAttachment.Equals(subtype)) {
                                                            annotation = new PdfFileAttachmentAnnotation((PdfDictionary)pdfObject);
                                                        }
                                                        else {
                                                            if (PdfName.Ink.Equals(subtype)) {
                                                                annotation = new PdfInkAnnotation((PdfDictionary)pdfObject);
                                                            }
                                                            else {
                                                                if (PdfName.PrinterMark.Equals(subtype)) {
                                                                    annotation = new PdfPrinterMarkAnnotation((PdfDictionary)pdfObject);
                                                                }
                                                                else {
                                                                    if (PdfName.TrapNet.Equals(subtype)) {
                                                                        annotation = new PdfTrapNetworkAnnotation((PdfDictionary)pdfObject);
                                                                    }
                                                                    else {
                                                                        if (PdfName.FreeText.Equals(subtype)) {
                                                                            annotation = new PdfFreeTextAnnotation((PdfDictionary)pdfObject);
                                                                        }
                                                                        else {
                                                                            if (PdfName.Square.Equals(subtype)) {
                                                                                annotation = new PdfSquareAnnotation((PdfDictionary)pdfObject);
                                                                            }
                                                                            else {
                                                                                if (PdfName.Circle.Equals(subtype)) {
                                                                                    annotation = new PdfCircleAnnotation((PdfDictionary)pdfObject);
                                                                                }
                                                                                else {
                                                                                    if (PdfName.Line.Equals(subtype)) {
                                                                                        annotation = new PdfLineAnnotation((PdfDictionary)pdfObject);
                                                                                    }
                                                                                    else {
                                                                                        if (PdfName.Polygon.Equals(subtype) || PdfName.PolyLine.Equals(subtype)) {
                                                                                            annotation = new PdfPolyGeomAnnotation((PdfDictionary)pdfObject);
                                                                                        }
                                                                                        else {
                                                                                            if (PdfName.Redact.Equals(subtype)) {
                                                                                                annotation = new PdfRedactAnnotation((PdfDictionary)pdfObject);
                                                                                            }
                                                                                            else {
                                                                                                if (PdfName.Watermark.Equals(subtype)) {
                                                                                                    annotation = new PdfWatermarkAnnotation((PdfDictionary)pdfObject);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return annotation;
        }

        /// <summary>
        /// Factory method that creates the type specific
        /// <see cref="PdfAnnotation"/>
        /// from the given
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// that represents annotation object. This method is useful for property reading in reading mode or
        /// modifying in stamping mode.
        /// </summary>
        /// <param name="pdfObject">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// that represents annotation in the document.
        /// </param>
        /// <param name="parent">
        /// parent annotation of the
        /// <see cref="PdfPopupAnnotation"/>
        /// to be created. This parameter is
        /// only needed if passed
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// represents a pop-up annotation in the document.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfAnnotation"/>
        /// .
        /// </returns>
        [System.ObsoleteAttribute(@"This method will be removed in iText 7.1. Please, simply use MakeAnnotation(iText.Kernel.Pdf.PdfObject) ."
            )]
        public static iText.Kernel.Pdf.Annot.PdfAnnotation MakeAnnotation(PdfObject pdfObject, iText.Kernel.Pdf.Annot.PdfAnnotation
             parent) {
            iText.Kernel.Pdf.Annot.PdfAnnotation annotation = MakeAnnotation(pdfObject);
            if (annotation is PdfPopupAnnotation) {
                PdfPopupAnnotation popup = (PdfPopupAnnotation)annotation;
                if (parent != null) {
                    popup.SetParent(parent);
                }
            }
            return annotation;
        }

        protected internal PdfAnnotation(Rectangle rect)
            : this(new PdfDictionary()) {
            Put(PdfName.Rect, new PdfArray(rect));
            Put(PdfName.Subtype, GetSubtype());
        }

        protected internal PdfAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
            MarkObjectAsIndirect(GetPdfObject());
        }

        /// <summary>
        /// Gets a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// which value is a subtype of this annotation.
        /// See ISO-320001 12.5.6, “Annotation Types” for the reference to the possible types.
        /// </summary>
        /// <returns>subtype of this annotation.</returns>
        public abstract PdfName GetSubtype();

        /// <summary>Sets the layer this annotation belongs to.</summary>
        /// <param name="layer">the layer this annotation belongs to</param>
        public virtual void SetLayer(IPdfOCG layer) {
            GetPdfObject().Put(PdfName.OC, layer.GetIndirectReference());
        }

        /// <summary>
        /// Sets a
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to this annotation which will be performed when the annotation is activated.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to set to this annotation.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetAction(PdfAction action) {
            return Put(PdfName.A, action.GetPdfObject());
        }

        /// <summary>
        /// Sets an additional
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to this annotation which will be performed in response to
        /// the specific trigger event defined by
        /// <paramref name="key"/>
        /// . See ISO-320001 12.6.3, "Trigger Events".
        /// </summary>
        /// <param name="key">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// that denotes a type of the additional action to set.
        /// </param>
        /// <param name="action">
        /// 
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to set as additional to this annotation.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetAdditionalAction(PdfName key, PdfAction action) {
            PdfAction.SetAdditionalAction(this, key, action);
            return this;
        }

        /// <summary>
        /// Gets the text that shall be displayed for the annotation or, if this type of annotation does not display text,
        /// an alternate description of the annotation’s contents in human-readable form.
        /// </summary>
        /// <returns>annotation text content.</returns>
        public virtual PdfString GetContents() {
            return GetPdfObject().GetAsString(PdfName.Contents);
        }

        /// <summary>
        /// Sets the text that shall be displayed for the annotation or, if this type of annotation does not display text,
        /// an alternate description of the annotation’s contents in human-readable form.
        /// </summary>
        /// <param name="contents">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// containing text content to be set to the annotation.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetContents(PdfString contents) {
            return Put(PdfName.Contents, contents);
        }

        /// <summary>
        /// Sets the text that shall be displayed for the annotation or, if this type of annotation does not display text,
        /// an alternate description of the annotation’s contents in human-readable form.
        /// </summary>
        /// <param name="contents">
        /// a java
        /// <see cref="System.String"/>
        /// containing text content to be set to the annotation.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetContents(String contents) {
            return SetContents(new PdfString(contents, PdfEncodings.UNICODE_BIG));
        }

        /// <summary>
        /// Gets a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represents a page of the document on which annotation is placed,
        /// i.e. which has this annotation in it's /Annots array.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that is a page pdf object or null if annotation is not added to the page yet.
        /// </returns>
        public virtual PdfDictionary GetPageObject() {
            return GetPdfObject().GetAsDictionary(PdfName.P);
        }

        /// <summary>
        /// Gets a
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// on which annotation is placed.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// on which annotation is placed or null if annotation is not placed yet.
        /// </returns>
        public virtual PdfPage GetPage() {
            if (page == null && GetPdfObject().IsIndirect()) {
                PdfIndirectReference annotationIndirectReference = GetPdfObject().GetIndirectReference();
                PdfDocument doc = annotationIndirectReference.GetDocument();
                PdfDictionary pageDictionary = GetPageObject();
                if (pageDictionary != null) {
                    page = doc.GetPage(pageDictionary);
                }
                else {
                    for (int i = 1; i <= doc.GetNumberOfPages(); i++) {
                        PdfPage docPage = doc.GetPage(i);
                        foreach (iText.Kernel.Pdf.Annot.PdfAnnotation annot in docPage.GetAnnotations()) {
                            if (annot.GetPdfObject().GetIndirectReference().Equals(annotationIndirectReference)) {
                                page = docPage;
                                break;
                            }
                        }
                    }
                }
            }
            return page;
        }

        /// <summary>Method that modifies annotation page property, which defines to which page annotation belongs.</summary>
        /// <remarks>
        /// Method that modifies annotation page property, which defines to which page annotation belongs.
        /// Keep in mind that this doesn't actually add an annotation to the page,
        /// it should be done via
        /// <see cref="iText.Kernel.Pdf.PdfPage.AddAnnotation(PdfAnnotation)"/>
        /// .
        /// Also you don't need to set this property manually, this is done automatically on addition to the page.
        /// </remarks>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// to which annotation will be added.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetPage(PdfPage page) {
            this.page = page;
            return Put(PdfName.P, page.GetPdfObject());
        }

        /// <summary>
        /// Gets the annotation name, a text string uniquely identifying it among all the
        /// annotations on its page.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// with annotation name as it's value or null if name
        /// is not specified.
        /// </returns>
        public virtual PdfString GetName() {
            return GetPdfObject().GetAsString(PdfName.NM);
        }

        /// <summary>
        /// Sets the annotation name, a text string uniquely identifying it among all the
        /// annotations on its page.
        /// </summary>
        /// <param name="name">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// to be set as annotation name.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetName(PdfString name) {
            return Put(PdfName.NM, name);
        }

        /// <summary>The date and time when the annotation was most recently modified.</summary>
        /// <remarks>
        /// The date and time when the annotation was most recently modified.
        /// This is an optional property of the annotation.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// with the modification date as it's value or null if date is not specified.
        /// </returns>
        public virtual PdfString GetDate() {
            return GetPdfObject().GetAsString(PdfName.M);
        }

        /// <summary>The date and time when the annotation was most recently modified.</summary>
        /// <param name="date">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// with date. The format should be a date string as described
        /// in ISO-320001 7.9.4, “Dates”.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetDate(PdfString date) {
            return Put(PdfName.M, date);
        }

        /// <summary>A set of flags specifying various characteristics of the annotation (see ISO-320001 12.5.3, “Annotation Flags”).
        ///     </summary>
        /// <remarks>
        /// A set of flags specifying various characteristics of the annotation (see ISO-320001 12.5.3, “Annotation Flags”).
        /// For specific annotation flag constants see
        /// <see cref="SetFlag(int)"/>
        /// .
        /// Default value: 0.
        /// </remarks>
        /// <returns>an integer interpreted as one-bit flags specifying various characteristics of the annotation.</returns>
        public virtual int GetFlags() {
            PdfNumber f = GetPdfObject().GetAsNumber(PdfName.F);
            if (f != null) {
                return f.IntValue();
            }
            else {
                return 0;
            }
        }

        /// <summary>Sets a set of flags specifying various characteristics of the annotation (see ISO-320001 12.5.3, “Annotation Flags”).
        ///     </summary>
        /// <remarks>
        /// Sets a set of flags specifying various characteristics of the annotation (see ISO-320001 12.5.3, “Annotation Flags”).
        /// On the contrary from
        /// <see cref="SetFlag(int)"/>
        /// , this method sets a complete set of enabled and disabled flags at once.
        /// If not set specifically the default value is 0.
        /// </remarks>
        /// <param name="flags">an integer interpreted as set of one-bit flags specifying various characteristics of the annotation.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetFlags(int flags) {
            return Put(PdfName.F, new PdfNumber(flags));
        }

        /// <summary>Sets a flag that specifies a characteristic of the annotation to enabled state (see ISO-320001 12.5.3, “Annotation Flags”).
        ///     </summary>
        /// <remarks>
        /// Sets a flag that specifies a characteristic of the annotation to enabled state (see ISO-320001 12.5.3, “Annotation Flags”).
        /// On the contrary from
        /// <see cref="SetFlags(int)"/>
        /// , this method sets only specified flags to enabled state,
        /// but doesn't disable other flags.
        /// Possible flags:
        /// <ul>
        /// <li>
        /// <see cref="INVISIBLE"/>
        /// - If set, do not display the annotation if it does not belong to one of the
        /// standard annotation types and no annotation handler is available. If clear, display such unknown annotation
        /// using an appearance stream specified by its appearance dictionary, if any.
        /// </li>
        /// <li>
        /// <see cref="HIDDEN"/>
        /// - If set, do not display or print the annotation or allow it to interact with
        /// the user, regardless of its annotation type or whether an annotation handler is available.
        /// </li>
        /// <li>
        /// <see cref="PRINT"/>
        /// - If set, print the annotation when the page is printed. If clear, never print
        /// the annotation, regardless of whether it is displayed on the screen.
        /// </li>
        /// <li>
        /// <see cref="NO_ZOOM"/>
        /// - If set, do not scale the annotation’s appearance to match the magnification of
        /// the page. The location of the annotation on the page (defined by the upper-left corner of its annotation
        /// rectangle) shall remain fixed, regardless of the page magnification.}
        /// </li>
        /// <li>
        /// <see cref="NO_ROTATE"/>
        /// - If set, do not rotate the annotation’s appearance to match the rotation
        /// of the page. The upper-left corner of the annotation rectangle shall remain in a fixed location on the page,
        /// regardless of the page rotation.
        /// </li>
        /// <li>
        /// <see cref="NO_VIEW"/>
        /// - If set, do not display the annotation on the screen or allow it to interact
        /// with the user. The annotation may be printed (depending on the setting of the Print flag) but should be considered
        /// hidden for purposes of on-screen display and user interaction.
        /// </li>
        /// <li>
        /// <see cref="READ_ONLY"/>
        /// -  If set, do not allow the annotation to interact with the user. The annotation
        /// may be displayed or printed (depending on the settings of the NoView and Print flags) but should not respond to mouse
        /// clicks or change its appearance in response to mouse motions.
        /// </li>
        /// <li>
        /// <see cref="LOCKED"/>
        /// -  If set, do not allow the annotation to be deleted or its properties
        /// (including position and size) to be modified by the user. However, this flag does not restrict changes to
        /// the annotation’s contents, such as the value of a form field.
        /// </li>
        /// <li>
        /// <see cref="TOGGLE_NO_VIEW"/>
        /// - If set, invert the interpretation of the NoView flag for certain events.
        /// </li>
        /// <li>
        /// <see cref="LOCKED_CONTENTS"/>
        /// - If set, do not allow the contents of the annotation to be modified
        /// by the user. This flag does not restrict deletion of the annotation or changes to other annotation properties,
        /// such as position and size.
        /// </li>
        /// </ul>
        /// </remarks>
        /// <param name="flag">- an integer interpreted as set of one-bit flags which will be enabled for this annotation.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetFlag(int flag) {
            int flags = GetFlags();
            flags = flags | flag;
            return SetFlags(flags);
        }

        /// <summary>Resets a flag that specifies a characteristic of the annotation to disabled state (see ISO-320001 12.5.3, “Annotation Flags”).
        ///     </summary>
        /// <param name="flag">an integer interpreted as set of one-bit flags which will be reset to disabled state.</param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation ResetFlag(int flag) {
            int flags = GetFlags();
            flags = flags & ~flag;
            return SetFlags(flags);
        }

        /// <summary>
        /// Checks if the certain flag that specifies a characteristic of the annotation
        /// is in enabled state (see ISO-320001 12.5.3, “Annotation Flags”).
        /// </summary>
        /// <remarks>
        /// Checks if the certain flag that specifies a characteristic of the annotation
        /// is in enabled state (see ISO-320001 12.5.3, “Annotation Flags”).
        /// This method allows only one flag to be checked at once, use constants listed in
        /// <see cref="SetFlag(int)"/>
        /// .
        /// </remarks>
        /// <param name="flag">
        /// an integer interpreted as set of one-bit flags. Only one bit must be set in this integer, otherwise
        /// exception is thrown.
        /// </param>
        /// <returns>true if the given flag is in enabled state.</returns>
        public virtual bool HasFlag(int flag) {
            if (flag == 0) {
                return false;
            }
            if ((flag & flag - 1) != 0) {
                throw new ArgumentException("Only one flag must be checked at once.");
            }
            int flags = GetFlags();
            return (flags & flag) != 0;
        }

        /// <summary>
        /// An appearance dictionary specifying how the annotation shall be presented visually on the page during its
        /// interactions with the user (see ISO-320001 12.5.5, “Appearance Streams”).
        /// </summary>
        /// <remarks>
        /// An appearance dictionary specifying how the annotation shall be presented visually on the page during its
        /// interactions with the user (see ISO-320001 12.5.5, “Appearance Streams”). An appearance dictionary is a dictionary
        /// containing one or several appearance streams or subdictionaries.
        /// </remarks>
        /// <returns>
        /// an appearance
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// or null if it is not specified.
        /// </returns>
        public virtual PdfDictionary GetAppearanceDictionary() {
            return GetPdfObject().GetAsDictionary(PdfName.AP);
        }

        /// <summary>Specific appearance object corresponding to the specific appearance type.</summary>
        /// <remarks>
        /// Specific appearance object corresponding to the specific appearance type. This object might be either an appearance
        /// stream or an appearance subdictionary. In the latter case, the subdictionary defines multiple appearance streams
        /// corresponding to different appearance states of the annotation. See ISO-320001 12.5.5, “Appearance Streams”.
        /// </remarks>
        /// <param name="appearanceType">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// specifying appearance type. Possible types are
        /// <see cref="iText.Kernel.Pdf.PdfName.N">Normal</see>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.R">Rollover</see>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfName.D">Down</see>
        /// .
        /// </param>
        /// <returns>
        /// null if their is no such appearance type or an appearance object which might be either
        /// an appearance stream or an appearance subdictionary.
        /// </returns>
        public virtual PdfDictionary GetAppearanceObject(PdfName appearanceType) {
            PdfDictionary ap = GetAppearanceDictionary();
            if (ap != null) {
                PdfObject apObject = ap.Get(appearanceType);
                if (apObject is PdfDictionary) {
                    return (PdfDictionary)apObject;
                }
            }
            return null;
        }

        /// <summary>The normal appearance is used when the annotation is not interacting with the user.</summary>
        /// <remarks>
        /// The normal appearance is used when the annotation is not interacting with the user.
        /// This appearance is also used for printing the annotation.
        /// See also
        /// <see cref="GetAppearanceObject(iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// </remarks>
        /// <returns>an appearance object which might be either an appearance stream or an appearance subdictionary.</returns>
        public virtual PdfDictionary GetNormalAppearanceObject() {
            return GetAppearanceObject(PdfName.N);
        }

        /// <summary>
        /// The rollover appearance is used when the user moves the cursor into the annotation’s active area
        /// without pressing the mouse button.
        /// </summary>
        /// <remarks>
        /// The rollover appearance is used when the user moves the cursor into the annotation’s active area
        /// without pressing the mouse button. If not specified normal appearance is used.
        /// See also
        /// <see cref="GetAppearanceObject(iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// </remarks>
        /// <returns>
        /// null if rollover appearance is not specified or an appearance object which might be either
        /// an appearance stream or an appearance subdictionary.
        /// </returns>
        public virtual PdfDictionary GetRolloverAppearanceObject() {
            return GetAppearanceObject(PdfName.R);
        }

        /// <summary>The down appearance is used when the mouse button is pressed or held down within the annotation’s active area.
        ///     </summary>
        /// <remarks>
        /// The down appearance is used when the mouse button is pressed or held down within the annotation’s active area.
        /// If not specified normal appearance is used.
        /// See also
        /// <see cref="GetAppearanceObject(iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// </remarks>
        /// <returns>
        /// null if down appearance is not specified or an appearance object which might be either
        /// an appearance stream or an appearance subdictionary.
        /// </returns>
        public virtual PdfDictionary GetDownAppearanceObject() {
            return GetAppearanceObject(PdfName.D);
        }

        /// <summary>Sets a specific type of the appearance.</summary>
        /// <remarks>
        /// Sets a specific type of the appearance. See
        /// <see cref="GetAppearanceObject(iText.Kernel.Pdf.PdfName)"/>
        /// and
        /// <see cref="GetAppearanceDictionary()"/>
        /// for more info.
        /// </remarks>
        /// <param name="appearanceType">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// specifying appearance type. Possible types are
        /// <see cref="iText.Kernel.Pdf.PdfName.N">Normal</see>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.R">Rollover</see>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfName.D">Down</see>
        /// .
        /// </param>
        /// <param name="appearance">an appearance object which might be either an appearance stream or an appearance subdictionary.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetAppearance(PdfName appearanceType, PdfDictionary appearance
            ) {
            PdfDictionary ap = GetAppearanceDictionary();
            if (ap == null) {
                ap = new PdfDictionary();
                GetPdfObject().Put(PdfName.AP, ap);
            }
            ap.Put(appearanceType, appearance);
            return this;
        }

        /// <summary>Sets normal appearance.</summary>
        /// <remarks>
        /// Sets normal appearance. See
        /// <see cref="GetNormalAppearanceObject()"/>
        /// and
        /// <see cref="GetAppearanceDictionary()"/>
        /// for more info.
        /// </remarks>
        /// <param name="appearance">an appearance object which might be either an appearance stream or an appearance subdictionary.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetNormalAppearance(PdfDictionary appearance) {
            return SetAppearance(PdfName.N, appearance);
        }

        /// <summary>Sets rollover appearance.</summary>
        /// <remarks>
        /// Sets rollover appearance. See
        /// <see cref="GetRolloverAppearanceObject()"/>
        /// and
        /// <see cref="GetAppearanceDictionary()"/>
        /// for more info.
        /// </remarks>
        /// <param name="appearance">an appearance object which might be either an appearance stream or an appearance subdictionary.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetRolloverAppearance(PdfDictionary appearance) {
            return SetAppearance(PdfName.R, appearance);
        }

        /// <summary>Sets down appearance.</summary>
        /// <remarks>
        /// Sets down appearance. See
        /// <see cref="GetDownAppearanceObject()"/>
        /// and
        /// <see cref="GetAppearanceDictionary()"/>
        /// for more info.
        /// </remarks>
        /// <param name="appearance">an appearance object which might be either an appearance stream or an appearance subdictionary.
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetDownAppearance(PdfDictionary appearance) {
            return SetAppearance(PdfName.D, appearance);
        }

        /// <summary>
        /// Sets a specific type of the appearance using
        /// <see cref="PdfAnnotationAppearance"/>
        /// wrapper.
        /// This method is used to set only an appearance subdictionary. See
        /// <see cref="GetAppearanceObject(iText.Kernel.Pdf.PdfName)"/>
        /// and
        /// <see cref="GetAppearanceDictionary()"/>
        /// for more info.
        /// </summary>
        /// <param name="appearanceType">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// specifying appearance type. Possible types are
        /// <see cref="iText.Kernel.Pdf.PdfName.N">Normal</see>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.R">Rollover</see>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfName.D">Down</see>
        /// .
        /// </param>
        /// <param name="appearance">
        /// an appearance subdictionary wrapped in
        /// <see cref="PdfAnnotationAppearance"/>
        /// .
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetAppearance(PdfName appearanceType, PdfAnnotationAppearance
             appearance) {
            return SetAppearance(appearanceType, appearance.GetPdfObject());
        }

        /// <summary>
        /// Sets normal appearance using
        /// <see cref="PdfAnnotationAppearance"/>
        /// wrapper. This method is used to set only
        /// appearance subdictionary. See
        /// <see cref="GetNormalAppearanceObject()"/>
        /// and
        /// <see cref="GetAppearanceDictionary()"/>
        /// for more info.
        /// </summary>
        /// <param name="appearance">
        /// an appearance subdictionary wrapped in
        /// <see cref="PdfAnnotationAppearance"/>
        /// .
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetNormalAppearance(PdfAnnotationAppearance appearance
            ) {
            return SetAppearance(PdfName.N, appearance);
        }

        /// <summary>
        /// Sets rollover appearance using
        /// <see cref="PdfAnnotationAppearance"/>
        /// wrapper. This method is used to set only
        /// appearance subdictionary. See
        /// <see cref="GetRolloverAppearanceObject()"/>
        /// and
        /// <see cref="GetAppearanceDictionary()"/>
        /// for more info.
        /// </summary>
        /// <param name="appearance">
        /// an appearance subdictionary wrapped in
        /// <see cref="PdfAnnotationAppearance"/>
        /// .
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetRolloverAppearance(PdfAnnotationAppearance appearance
            ) {
            return SetAppearance(PdfName.R, appearance);
        }

        /// <summary>
        /// Sets down appearance using
        /// <see cref="PdfAnnotationAppearance"/>
        /// wrapper. This method is used to set only
        /// appearance subdictionary. See
        /// <see cref="GetDownAppearanceObject()"/>
        /// and
        /// <see cref="GetAppearanceDictionary()"/>
        /// for more info.
        /// </summary>
        /// <param name="appearance">
        /// an appearance subdictionary wrapped in
        /// <see cref="PdfAnnotationAppearance"/>
        /// .
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetDownAppearance(PdfAnnotationAppearance appearance) {
            return SetAppearance(PdfName.D, appearance);
        }

        /// <summary>
        /// The annotation’s appearance state, which selects the applicable appearance stream
        /// from an appearance subdictionary if there is such.
        /// </summary>
        /// <remarks>
        /// The annotation’s appearance state, which selects the applicable appearance stream
        /// from an appearance subdictionary if there is such. See
        /// <see cref="GetAppearanceObject(iText.Kernel.Pdf.PdfName)"/>
        /// for more info.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// which defines selected appearance state.
        /// </returns>
        public virtual PdfName GetAppearanceState() {
            return GetPdfObject().GetAsName(PdfName.AS);
        }

        /// <summary>
        /// Sets the annotation’s appearance state, which selects the applicable appearance stream
        /// from an appearance subdictionary.
        /// </summary>
        /// <remarks>
        /// Sets the annotation’s appearance state, which selects the applicable appearance stream
        /// from an appearance subdictionary. See
        /// <see cref="GetAppearanceObject(iText.Kernel.Pdf.PdfName)"/>
        /// for more info.
        /// </remarks>
        /// <param name="as">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// which defines appearance state to be selected.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetAppearanceState(PdfName @as) {
            return Put(PdfName.AS, @as);
        }

        /// <summary>
        /// <p>
        /// An array specifying the characteristics of the annotation’s border.
        /// </summary>
        /// <remarks>
        /// <p>
        /// An array specifying the characteristics of the annotation’s border.
        /// The array consists of three numbers defining the horizontal corner radius,
        /// vertical corner radius, and border width, all in default user space units.
        /// If the corner radii are 0, the border has square (not rounded) corners; if
        /// the border width is 0, no border is drawn.
        /// <p>
        /// The array may have a fourth element, an optional dash array (see ISO-320001 8.4.3.6, “Line Dash Pattern”).
        /// </remarks>
        /// <returns>
        /// an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// specifying the characteristics of the annotation’s border.
        /// </returns>
        public virtual PdfArray GetBorder() {
            return GetPdfObject().GetAsArray(PdfName.Border);
        }

        /// <summary>Sets the characteristics of the annotation’s border.</summary>
        /// <param name="border">
        /// an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// specifying the characteristics of the annotation’s border.
        /// See
        /// <see cref="GetBorder()"/>
        /// for more detailes.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetBorder(PdfArray border) {
            return Put(PdfName.Border, border);
        }

        /// <summary>
        /// An array of numbers in the range 0.0 to 1.0, representing a colour used for the following purposes:
        /// <ul>
        /// <li>The background of the annotation’s icon when closed</li>
        /// <li>The title bar of the annotation’s pop-up window</li>
        /// <li>The border of a link annotation</li>
        /// </ul>
        /// The number of array elements determines the colour space in which the colour shall be defined:
        /// <ul>
        /// <li>0 - No colour; transparent</li>
        /// <li>1 - DeviceGray</li>
        /// <li>3 - DeviceRGB</li>
        /// <li>4 - DeviceCMYK</li>
        /// </ul>
        /// </summary>
        /// <returns>An array of numbers in the range 0.0 to 1.0, representing an annotation colour.</returns>
        public virtual PdfArray GetColorObject() {
            return GetPdfObject().GetAsArray(PdfName.C);
        }

        /// <summary>Sets an annotation color.</summary>
        /// <remarks>
        /// Sets an annotation color. For more details on annotation color purposes and the format
        /// of the passing
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// see
        /// <see cref="GetColorObject()"/>
        /// .
        /// </remarks>
        /// <param name="color">an array of numbers in the range 0.0 to 1.0, specifying color.</param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetColor(PdfArray color) {
            return Put(PdfName.C, color);
        }

        /// <summary>Sets an annotation color.</summary>
        /// <remarks>
        /// Sets an annotation color. For more details on annotation color purposes and the format
        /// of the passing array see
        /// <see cref="GetColorObject()"/>
        /// .
        /// </remarks>
        /// <param name="color">an array of numbers in the range 0.0 to 1.0, specifying color.</param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetColor(float[] color) {
            return SetColor(new PdfArray(color));
        }

        /// <summary>Sets an annotation color.</summary>
        /// <remarks>
        /// Sets an annotation color. For more details on annotation color purposes
        /// see
        /// <see cref="GetColorObject()"/>
        /// .
        /// </remarks>
        /// <param name="color">
        /// 
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// object of the either
        /// <see cref="iText.Kernel.Colors.DeviceGray"/>
        /// ,
        /// <see cref="iText.Kernel.Colors.DeviceRgb"/>
        /// or
        /// <see cref="iText.Kernel.Colors.DeviceCmyk"/>
        /// type.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetColor(Color color) {
            return SetColor(new PdfArray(color.GetColorValue()));
        }

        /// <summary>
        /// The integer key of the annotation’s entry in the structural parent tree
        /// (see ISO-320001 14.7.4.4, “Finding Structure Elements from Content Items”).
        /// </summary>
        /// <returns>integer key in structural parent tree or -1 if annotation is not tagged.</returns>
        public virtual int GetStructParentIndex() {
            PdfNumber n = GetPdfObject().GetAsNumber(PdfName.StructParent);
            if (n == null) {
                return -1;
            }
            else {
                return n.IntValue();
            }
        }

        /// <summary>
        /// Sets he integer key of the annotation’s entry in the structural parent tree
        /// (see ISO-320001 14.7.4.4, “Finding Structure Elements from Content Items”).
        /// </summary>
        /// <remarks>
        /// Sets he integer key of the annotation’s entry in the structural parent tree
        /// (see ISO-320001 14.7.4.4, “Finding Structure Elements from Content Items”).
        /// Note: Normally, there is no need to take care of this manually, struct parent index is set automatically
        /// if annotation is added to the tagged document's page.
        /// </remarks>
        /// <param name="structParentIndex">
        /// integer which is to be the key of the annotation's entry
        /// in structural parent tree.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetStructParentIndex(int structParentIndex) {
            return Put(PdfName.StructParent, new PdfNumber(structParentIndex));
        }

        /// <summary>A flag specifying whether the annotation shall initially be displayed open.</summary>
        /// <remarks>
        /// A flag specifying whether the annotation shall initially be displayed open.
        /// This flag has affect to not all kinds of annotations.
        /// </remarks>
        /// <returns>true if annotation is initially open, false - if closed.</returns>
        public virtual bool GetOpen() {
            PdfBoolean open = GetPdfObject().GetAsBoolean(PdfName.Open);
            return open != null && open.GetValue();
        }

        /// <summary>Sets a flag specifying whether the annotation shall initially be displayed open.</summary>
        /// <remarks>
        /// Sets a flag specifying whether the annotation shall initially be displayed open.
        /// This flag has affect to not all kinds of annotations.
        /// </remarks>
        /// <param name="open">true if annotation shall initially be open, false - if closed.</param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetOpen(bool open) {
            return Put(PdfName.Open, PdfBoolean.ValueOf(open));
        }

        /// <summary>An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.</summary>
        /// <remarks>
        /// An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.
        /// Quadrilaterals are used to define:
        /// <ul>
        /// <li>regions inside annotation rectangle in which the link annotation should be activated;</li>
        /// <li>a word or group of contiguous words in the text underlying the text markup annotation;</li>
        /// <li>the content region that is intended to be removed for a redaction annotation;</li>
        /// </ul>
        /// <p>
        /// <p>
        /// IMPORTANT NOTE: According to Table 179 in ISO 32000-1, the QuadPoints array lists the vertices in counterclockwise
        /// order and the text orientation is defined by the first and second vertex. This basically means QuadPoints is
        /// specified as lower-left, lower-right, top-right, top-left. HOWEVER, Adobe's interpretation
        /// (tested at least with Acrobat 10, Acrobat 11, Reader 11) is top-left, top-right, lower-left, lower-right (Z-shaped order).
        /// This means that if the QuadPoints array is specified according to the standard, the rendering is not as expected.
        /// Other viewers seem to follow Adobe's interpretation. Hence we recommend to use and expect QuadPoints array in Z-order,
        /// just as Acrobat and probably most other viewers expect.
        /// </remarks>
        /// <returns>
        /// an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers specifying the coordinates of n quadrilaterals.
        /// </returns>
        public virtual PdfArray GetQuadPoints() {
            return GetPdfObject().GetAsArray(PdfName.QuadPoints);
        }

        /// <summary>
        /// Sets n quadrilaterals in default user space by passing an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers. For more info of what
        /// quadrilaterals define see
        /// <see cref="GetQuadPoints()"/>
        /// .
        /// <p>
        /// <p>
        /// IMPORTANT NOTE: According to Table 179 in ISO 32000-1, the QuadPoints array lists the vertices in counterclockwise
        /// order and the text orientation is defined by the first and second vertex. This basically means QuadPoints is
        /// specified as lower-left, lower-right, top-right, top-left. HOWEVER, Adobe's interpretation
        /// (tested at least with Acrobat 10, Acrobat 11, Reader 11) is top-left, top-right, lower-left, lower-right (Z-shaped order).
        /// This means that if the QuadPoints array is specified according to the standard, the rendering is not as expected.
        /// Other viewers seem to follow Adobe's interpretation. Hence we recommend to use and expect QuadPoints array in Z-order,
        /// just as Acrobat and probably most other viewers expect.
        /// </summary>
        /// <param name="quadPoints">
        /// an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers specifying the coordinates of n quadrilaterals.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetQuadPoints(PdfArray quadPoints) {
            return Put(PdfName.QuadPoints, quadPoints);
        }

        /// <summary>
        /// Sets border style dictionary that has more settings than the array specified for the Border entry (
        /// <see cref="GetBorder()"/>
        /// ).
        /// See ISO-320001, Table 166 and
        /// <see cref="GetBorderStyle()"/>
        /// for more info.
        /// </summary>
        /// <param name="borderStyle">
        /// a border style dictionary specifying the line width and dash pattern that shall be used
        /// in drawing the annotation’s border.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetBorderStyle(PdfDictionary borderStyle) {
            return Put(PdfName.BS, borderStyle);
        }

        /// <summary>Setter for the annotation's preset border style.</summary>
        /// <remarks>
        /// Setter for the annotation's preset border style. Possible values are
        /// <ul>
        /// <li>
        /// <see cref="STYLE_SOLID"/>
        /// - A solid rectangle surrounding the annotation.</li>
        /// <li>
        /// <see cref="STYLE_DASHED"/>
        /// - A dashed rectangle surrounding the annotation.</li>
        /// <li>
        /// <see cref="STYLE_BEVELED"/>
        /// - A simulated embossed rectangle that appears to be raised above the surface of the page.</li>
        /// <li>
        /// <see cref="STYLE_INSET"/>
        /// - A simulated engraved rectangle that appears to be recessed below the surface of the page.</li>
        /// <li>
        /// <see cref="STYLE_UNDERLINE"/>
        /// - A single line along the bottom of the annotation rectangle.</li>
        /// </ul>
        /// See also ISO-320001, Table 166.
        /// </remarks>
        /// <param name="style">The new value for the annotation's border style.</param>
        /// <returns>The annotation which this method was called on.</returns>
        /// <seealso cref="GetBorderStyle()"/>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetBorderStyle(PdfName style) {
            PdfDictionary styleDict = GetBorderStyle();
            if (null == styleDict) {
                styleDict = new PdfDictionary();
            }
            styleDict.Put(PdfName.S, style);
            return SetBorderStyle(styleDict);
        }

        /// <summary>Setter for the annotation's preset dashed border style.</summary>
        /// <remarks>
        /// Setter for the annotation's preset dashed border style. This property has affect only if
        /// <see cref="STYLE_DASHED"/>
        /// style was used for the annotation border style (see
        /// <see cref="SetBorderStyle(iText.Kernel.Pdf.PdfName)"/>
        /// .
        /// See ISO-320001 8.4.3.6, “Line Dash Pattern” for the format in which dash pattern shall be specified.
        /// </remarks>
        /// <param name="dashPattern">
        /// a dash array defining a pattern of dashes and gaps that
        /// shall be used in drawing a dashed border.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetDashPattern(PdfArray dashPattern) {
            PdfDictionary styleDict = GetBorderStyle();
            if (null == styleDict) {
                styleDict = new PdfDictionary();
            }
            styleDict.Put(PdfName.D, dashPattern);
            return SetBorderStyle(styleDict);
        }

        /// <summary>The dictionaries for some annotation types (such as free text and polygon annotations) can include the BS entry.
        ///     </summary>
        /// <remarks>
        /// The dictionaries for some annotation types (such as free text and polygon annotations) can include the BS entry.
        /// That entry specifies a border style dictionary that has more settings than the array specified for the Border
        /// entry (see
        /// <see cref="GetBorder()"/>
        /// ). If an annotation dictionary includes the BS entry, then the Border
        /// entry is ignored. If annotation includes AP (see
        /// <see cref="GetAppearanceDictionary()"/>
        /// ) it takes
        /// precedence over the BS entry. For more info on BS entry see ISO-320001, Table 166.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which is a border style dictionary or null if it is not specified.
        /// </returns>
        public virtual PdfDictionary GetBorderStyle() {
            return GetPdfObject().GetAsDictionary(PdfName.BS);
        }

        /// <summary>Sets annotation title.</summary>
        /// <remarks>Sets annotation title. This property affects not all annotation types.</remarks>
        /// <param name="title">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which value is to be annotation title.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetTitle(PdfString title) {
            return Put(PdfName.T, title);
        }

        /// <summary>Annotation title.</summary>
        /// <remarks>
        /// Annotation title. For example for markup annotations, the title is the text label that shall be displayed in the
        /// title bar of the annotation’s pop-up window when open and active. For movie annotation Movie actions
        /// (ISO-320001 12.6.4.9, “Movie Actions”) may use this title to reference the movie annotation.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which value is an annotation title or null if it isn't specifed.
        /// </returns>
        public virtual PdfString GetTitle() {
            return GetPdfObject().GetAsString(PdfName.T);
        }

        /// <summary>
        /// Sets an appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream.
        /// </summary>
        /// <remarks>
        /// Sets an appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream. See ISO-320001, Table 189.
        /// This property affects
        /// <see cref="PdfWidgetAnnotation"/>
        /// and
        /// <see cref="PdfScreenAnnotation"/>
        /// .
        /// </remarks>
        /// <param name="characteristics">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// with additional information for appearance stream.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetAppearanceCharacteristics(PdfDictionary characteristics
            ) {
            return Put(PdfName.MK, characteristics);
        }

        /// <summary>
        /// An appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream.
        /// </summary>
        /// <remarks>
        /// An appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream. See ISO-320001, Table 189.
        /// This property affects
        /// <see cref="PdfWidgetAnnotation"/>
        /// and
        /// <see cref="PdfScreenAnnotation"/>
        /// .
        /// </remarks>
        /// <returns>an appearance characteristics dictionary or null if it isn't specified.</returns>
        public virtual PdfDictionary GetAppearanceCharacteristics() {
            return GetPdfObject().GetAsDictionary(PdfName.MK);
        }

        /// <summary>
        /// An
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to perform, such as launching an application, playing a sound,
        /// changing an annotation’s appearance state etc, when the annotation is activated.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which defines the characteristics and behaviour of an action.
        /// </returns>
        public virtual PdfDictionary GetAction() {
            return GetPdfObject().GetAsDictionary(PdfName.A);
        }

        /// <summary>An additional actions dictionary that extends the set of events that can trigger the execution of an action.
        ///     </summary>
        /// <remarks>
        /// An additional actions dictionary that extends the set of events that can trigger the execution of an action.
        /// See ISO-320001 12.6.3 Trigger Events.
        /// </remarks>
        /// <returns>
        /// an additional actions
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// .
        /// </returns>
        /// <seealso cref="GetAction()"/>
        public virtual PdfDictionary GetAdditionalAction() {
            return GetPdfObject().GetAsDictionary(PdfName.AA);
        }

        /// <summary>The annotation rectangle, defining the location of the annotation on the page in default user space units.
        ///     </summary>
        /// <param name="array">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// which specifies a rectangle by two diagonally opposite corners.
        /// Typically, the array is of form [llx lly urx ury].
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetRectangle(PdfArray array) {
            return Put(PdfName.Rect, array);
        }

        /// <summary>The annotation rectangle, defining the location of the annotation on the page in default user space units.
        ///     </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// which specifies a rectangle by two diagonally opposite corners.
        /// Typically, the array is of form [llx lly urx ury].
        /// </returns>
        public virtual PdfArray GetRectangle() {
            return GetPdfObject().GetAsArray(PdfName.Rect);
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>
        /// PDF 2.0. A language identifier overriding the document’s language identifier to
        /// specify the natural language for all text in the annotation except where overridden by
        /// other explicit language specifications
        /// </remarks>
        /// <returns>the lang entry</returns>
        public virtual String GetLang() {
            PdfString lang = GetPdfObject().GetAsString(PdfName.Lang);
            return lang != null ? lang.ToUnicodeString() : null;
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>
        /// PDF 2.0. A language identifier overriding the document’s language identifier to
        /// specify the natural language for all text in the annotation except where overridden by
        /// other explicit language specifications
        /// </remarks>
        /// <param name="lang">language identifier</param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetLang(String lang) {
            return Put(PdfName.Lang, new PdfString(lang, PdfEncodings.UNICODE_BIG));
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>PDF 2.0. The blend mode that shall be used when painting the annotation onto the page</remarks>
        /// <returns>the blend mode</returns>
        public virtual PdfName GetBlendMode() {
            return GetPdfObject().GetAsName(PdfName.BM);
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>PDF 2.0. The blend mode that shall be used when painting the annotation onto the page</remarks>
        /// <param name="blendMode">blend mode</param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetBlendMode(PdfName blendMode) {
            return Put(PdfName.BM, blendMode);
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>
        /// PDF 2.0. When regenerating the annotation's appearance stream, this is the
        /// opacity value that shall be used for all nonstroking
        /// operations on all visible elements of the annotation in its closed state (including its
        /// background and border) but not the popup window that appears when the annotation is
        /// opened.
        /// </remarks>
        /// <returns>opacity value for nonstroking operations. Returns 1.0 (default value) if entry is not present</returns>
        public virtual float GetNonStrokingOpacity() {
            PdfNumber nonStrokingOpacity = GetPdfObject().GetAsNumber(PdfName.ca);
            return nonStrokingOpacity != null ? nonStrokingOpacity.FloatValue() : 1;
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>
        /// PDF 2.0. When regenerating the annotation's appearance stream, this is the
        /// opacity value that shall be used for all nonstroking
        /// operations on all visible elements of the annotation in its closed state (including its
        /// background and border) but not the popup window that appears when the annotation is
        /// opened.
        /// </remarks>
        /// <param name="nonStrokingOpacity">opacity for nonstroking operations</param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetNonStrokingOpacity(float nonStrokingOpacity) {
            return Put(PdfName.ca, new PdfNumber(nonStrokingOpacity));
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>
        /// PDF 2.0. When regenerating the annotation's appearance stream, this is the
        /// opacity value that shall be used for stroking all visible
        /// elements of the annotation in its closed state, including its background and border, but
        /// not the popup window that appears when the annotation is opened.
        /// </remarks>
        /// <returns>opacity for stroking operations, including background and border</returns>
        public virtual float GetStrokingOpacity() {
            PdfNumber strokingOpacity = GetPdfObject().GetAsNumber(PdfName.CA);
            return strokingOpacity != null ? strokingOpacity.FloatValue() : 1;
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>
        /// PDF 2.0. When regenerating the annotation's appearance stream, this is the
        /// opacity value that shall be used for stroking all visible
        /// elements of the annotation in its closed state, including its background and border, but
        /// not the popup window that appears when the annotation is opened.
        /// </remarks>
        /// <param name="strokingOpacity">opacity for stroking operations, including background and border</param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// object
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetStrokingOpacity(float strokingOpacity) {
            return Put(PdfName.CA, new PdfNumber(strokingOpacity));
        }

        /// <summary>
        /// Inserts the value into into the underlying
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// of this
        /// <see cref="PdfAnnotation"/>
        /// and associates it
        /// with the specified key. If the key is already present in this
        /// <see cref="PdfAnnotation"/>
        /// , this method will override
        /// the old value with the specified one.
        /// </summary>
        /// <param name="key">key to insert or to override</param>
        /// <param name="value">the value to associate with the specified key</param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        /// <summary>
        /// Removes the specified key from the underlying
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// of this
        /// <see cref="PdfAnnotation"/>
        /// .
        /// </summary>
        /// <param name="key">key to be removed</param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation Remove(PdfName key) {
            GetPdfObject().Remove(key);
            return this;
        }

        /// <summary>
        /// <p>
        /// Adds file associated with PDF annotation and identifies the relationship between them.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Adds file associated with PDF annotation and identifies the relationship between them.
        /// </p>
        /// <p>
        /// Associated files may be used in Pdf/A-3 and Pdf 2.0 documents.
        /// The method adds file to array value of the AF key in the annotation dictionary.
        /// </p>
        /// <p>
        /// For associated files their associated file specification dictionaries shall include the AFRelationship key
        /// </p>
        /// </remarks>
        /// <param name="fs">file specification dictionary of associated file</param>
        public virtual void AddAssociatedFile(PdfFileSpec fs) {
            if (null == ((PdfDictionary)fs.GetPdfObject()).Get(PdfName.AFRelationship)) {
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Pdf.Annot.PdfAnnotation));
                logger.Error(iText.IO.LogMessageConstant.ASSOCIATED_FILE_SPEC_SHALL_INCLUDE_AFRELATIONSHIP);
            }
            PdfArray afArray = GetPdfObject().GetAsArray(PdfName.AF);
            if (afArray == null) {
                afArray = new PdfArray();
                Put(PdfName.AF, afArray);
            }
            afArray.Add(fs.GetPdfObject());
        }

        /// <summary>Returns files associated with PDF annotation.</summary>
        /// <param name="create">iText will create AF array if it doesn't exist and create value is true</param>
        /// <returns>associated files array.</returns>
        public virtual PdfArray GetAssociatedFiles(bool create) {
            PdfArray afArray = GetPdfObject().GetAsArray(PdfName.AF);
            if (afArray == null && create) {
                afArray = new PdfArray();
                Put(PdfName.AF, afArray);
            }
            return afArray;
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </summary>
        public override void Flush() {
            base.Flush();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
