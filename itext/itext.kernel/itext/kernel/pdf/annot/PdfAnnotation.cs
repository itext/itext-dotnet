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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Layer;

namespace iText.Kernel.Pdf.Annot {
    /// <summary>This is a super class for the annotation dictionary wrappers.</summary>
    /// <remarks>
    /// This is a super class for the annotation dictionary wrappers. Derived classes represent
    /// different standard types of annotations. See ISO-320001 12.5.6, "Annotation Types."
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
        /// <see cref="PdfWidgetAnnotation.SetHighlightMode(iText.Kernel.Pdf.PdfName)"/>.
        /// </remarks>
        public static readonly PdfName HIGHLIGHT_NONE = PdfName.N;

        /// <summary>Widget annotation highlighting mode.</summary>
        /// <remarks>
        /// Widget annotation highlighting mode. See ISO-320001, Table 188 (H key).
        /// Also see
        /// <see cref="PdfWidgetAnnotation.SetHighlightMode(iText.Kernel.Pdf.PdfName)"/>.
        /// </remarks>
        public static readonly PdfName HIGHLIGHT_INVERT = PdfName.I;

        /// <summary>Widget annotation highlighting mode.</summary>
        /// <remarks>
        /// Widget annotation highlighting mode. See ISO-320001, Table 188 (H key).
        /// Also see
        /// <see cref="PdfWidgetAnnotation.SetHighlightMode(iText.Kernel.Pdf.PdfName)"/>.
        /// </remarks>
        public static readonly PdfName HIGHLIGHT_OUTLINE = PdfName.O;

        /// <summary>Widget annotation highlighting mode.</summary>
        /// <remarks>
        /// Widget annotation highlighting mode. See ISO-320001, Table 188 (H key).
        /// Also see
        /// <see cref="PdfWidgetAnnotation.SetHighlightMode(iText.Kernel.Pdf.PdfName)"/>.
        /// </remarks>
        public static readonly PdfName HIGHLIGHT_PUSH = PdfName.P;

        /// <summary>Widget annotation highlighting mode.</summary>
        /// <remarks>
        /// Widget annotation highlighting mode. See ISO-320001, Table 188 (H key).
        /// Also see
        /// <see cref="PdfWidgetAnnotation.SetHighlightMode(iText.Kernel.Pdf.PdfName)"/>.
        /// </remarks>
        public static readonly PdfName HIGHLIGHT_TOGGLE = PdfName.T;

        /// <summary>Annotation border style.</summary>
        /// <remarks>Annotation border style. See ISO-320001, Table 166 (S key).</remarks>
        public static readonly PdfName STYLE_SOLID = PdfName.S;

        /// <summary>Annotation border style.</summary>
        /// <remarks>Annotation border style. See ISO-320001, Table 166 (S key).</remarks>
        public static readonly PdfName STYLE_DASHED = PdfName.D;

        /// <summary>Annotation border style.</summary>
        /// <remarks>Annotation border style. See ISO-320001, Table 166 (S key).</remarks>
        public static readonly PdfName STYLE_BEVELED = PdfName.B;

        /// <summary>Annotation border style.</summary>
        /// <remarks>Annotation border style. See ISO-320001, Table 166 (S key).</remarks>
        public static readonly PdfName STYLE_INSET = PdfName.I;

        /// <summary>Annotation border style.</summary>
        /// <remarks>Annotation border style. See ISO-320001, Table 166 (S key).</remarks>
        public static readonly PdfName STYLE_UNDERLINE = PdfName.U;

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        public static readonly PdfString Marked = new PdfString("Marked");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        public static readonly PdfString Unmarked = new PdfString("Unmarked");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        public static readonly PdfString Accepted = new PdfString("Accepted");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        public static readonly PdfString Rejected = new PdfString("Rejected");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        public static readonly PdfString Canceled = new PdfString("Cancelled");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        public static readonly PdfString Completed = new PdfString("Completed");

        /// <summary>Annotation state.</summary>
        /// <remarks>
        /// Annotation state. See ISO-320001 12.5.6.3 "Annotation States" and Table 171 in particular.
        /// Also see
        /// <see cref="PdfTextAnnotation.SetState(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        public static readonly PdfString None = new PdfString("None");

        /// <summary>Annotation state model.</summary>
        /// <remarks>
        /// Annotation state model. See ISO-320001, Table 172 (StateModel key).
        /// Also see
        /// <see cref="PdfTextAnnotation.SetStateModel(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        public static readonly PdfString MarkedModel = new PdfString("Marked");

        /// <summary>Annotation state model.</summary>
        /// <remarks>
        /// Annotation state model. See ISO-320001, Table 172 (StateModel key).
        /// Also see
        /// <see cref="PdfTextAnnotation.SetStateModel(iText.Kernel.Pdf.PdfString)"/>.
        /// </remarks>
        public static readonly PdfString ReviewModel = new PdfString("Review");

        protected internal PdfPage page;

        /// <summary>
        /// Factory method that creates the type specific
        /// <see cref="PdfAnnotation"/>
        /// from the given
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// that represents annotation object.
        /// </summary>
        /// <remarks>
        /// Factory method that creates the type specific
        /// <see cref="PdfAnnotation"/>
        /// from the given
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// that represents annotation object. This method is useful for property reading in reading mode or
        /// modifying in stamping mode. See derived classes of this class to see possible specific annotation types
        /// created.
        /// </remarks>
        /// <param name="pdfObject">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// that represents annotation in the document.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfAnnotation"/>.
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
                                    annotation = new Pdf3DAnnotation((PdfDictionary)pdfObject);
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
                                                                                        if (PdfName.Polygon.Equals(subtype)) {
                                                                                            annotation = new PdfPolygonAnnotation((PdfDictionary)pdfObject);
                                                                                        }
                                                                                        else {
                                                                                            if (PdfName.PolyLine.Equals(subtype)) {
                                                                                                annotation = new PdfPolylineAnnotation((PdfDictionary)pdfObject);
                                                                                            }
                                                                                            else {
                                                                                                if (PdfName.Redact.Equals(subtype)) {
                                                                                                    annotation = new PdfRedactAnnotation((PdfDictionary)pdfObject);
                                                                                                }
                                                                                                else {
                                                                                                    if (PdfName.Watermark.Equals(subtype)) {
                                                                                                        annotation = new PdfWatermarkAnnotation((PdfDictionary)pdfObject);
                                                                                                    }
                                                                                                    else {
                                                                                                        annotation = new PdfAnnotation.PdfUnknownAnnotation((PdfDictionary)pdfObject);
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
        /// </summary>
        /// <remarks>
        /// Gets a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// which value is a subtype of this annotation.
        /// See ISO-320001 12.5.6, "Annotation Types" for the reference to the possible types.
        /// </remarks>
        /// <returns>subtype of this annotation.</returns>
        public abstract PdfName GetSubtype();

        /// <summary>Sets the layer this annotation belongs to.</summary>
        /// <param name="layer">the layer this annotation belongs to</param>
        public virtual void SetLayer(IPdfOCG layer) {
            GetPdfObject().Put(PdfName.OC, layer.GetIndirectReference());
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
            PdfIndirectReference annotationIndirectReference;
            if (page == null && (annotationIndirectReference = GetPdfObject().GetIndirectReference()) != null) {
                PdfDocument doc = annotationIndirectReference.GetDocument();
                PdfDictionary pageDictionary = GetPageObject();
                if (pageDictionary != null) {
                    page = doc.GetPage(pageDictionary);
                }
                else {
                    for (int i = 1; i <= doc.GetNumberOfPages(); i++) {
                        PdfPage docPage = doc.GetPage(i);
                        if (!docPage.IsFlushed()) {
                            foreach (iText.Kernel.Pdf.Annot.PdfAnnotation annot in docPage.GetAnnotations()) {
                                if (annotationIndirectReference.Equals(annot.GetPdfObject().GetIndirectReference())) {
                                    page = docPage;
                                    break;
                                }
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
        /// <see cref="iText.Kernel.Pdf.PdfPage.AddAnnotation(PdfAnnotation)"/>.
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
            // Explicitly using object indirect reference here in order to correctly process released objects.
            return Put(PdfName.P, page.GetPdfObject().GetIndirectReference());
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
        /// in ISO-320001 7.9.4, "Dates".
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetDate(PdfString date) {
            return Put(PdfName.M, date);
        }

        /// <summary>A set of flags specifying various characteristics of the annotation (see ISO-320001 12.5.3, "Annotation Flags").
        ///     </summary>
        /// <remarks>
        /// A set of flags specifying various characteristics of the annotation (see ISO-320001 12.5.3, "Annotation Flags").
        /// For specific annotation flag constants see
        /// <see cref="SetFlag(int)"/>.
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

        /// <summary>Sets a set of flags specifying various characteristics of the annotation (see ISO-320001 12.5.3, "Annotation Flags").
        ///     </summary>
        /// <remarks>
        /// Sets a set of flags specifying various characteristics of the annotation (see ISO-320001 12.5.3, "Annotation Flags").
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

        /// <summary>Sets a flag that specifies a characteristic of the annotation to enabled state (see ISO-320001 12.5.3, "Annotation Flags").
        ///     </summary>
        /// <remarks>
        /// Sets a flag that specifies a characteristic of the annotation to enabled state (see ISO-320001 12.5.3, "Annotation Flags").
        /// On the contrary from
        /// <see cref="SetFlags(int)"/>
        /// , this method sets only specified flags to enabled state,
        /// but doesn't disable other flags.
        /// Possible flags:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="INVISIBLE"/>
        /// - If set, do not display the annotation if it does not belong to one of the
        /// standard annotation types and no annotation handler is available. If clear, display such unknown annotation
        /// using an appearance stream specified by its appearance dictionary, if any.
        /// </description></item>
        /// <item><description>
        /// <see cref="HIDDEN"/>
        /// - If set, do not display or print the annotation or allow it to interact with
        /// the user, regardless of its annotation type or whether an annotation handler is available.
        /// </description></item>
        /// <item><description>
        /// <see cref="PRINT"/>
        /// - If set, print the annotation when the page is printed. If clear, never print
        /// the annotation, regardless of whether it is displayed on the screen.
        /// </description></item>
        /// <item><description>
        /// <see cref="NO_ZOOM"/>
        /// - If set, do not scale the annotation’s appearance to match the magnification of
        /// the page. The location of the annotation on the page (defined by the upper-left corner of its annotation
        /// rectangle) shall remain fixed, regardless of the page magnification.}
        /// </description></item>
        /// <item><description>
        /// <see cref="NO_ROTATE"/>
        /// - If set, do not rotate the annotation’s appearance to match the rotation
        /// of the page. The upper-left corner of the annotation rectangle shall remain in a fixed location on the page,
        /// regardless of the page rotation.
        /// </description></item>
        /// <item><description>
        /// <see cref="NO_VIEW"/>
        /// - If set, do not display the annotation on the screen or allow it to interact
        /// with the user. The annotation may be printed (depending on the setting of the Print flag) but should be considered
        /// hidden for purposes of on-screen display and user interaction.
        /// </description></item>
        /// <item><description>
        /// <see cref="READ_ONLY"/>
        /// -  If set, do not allow the annotation to interact with the user. The annotation
        /// may be displayed or printed (depending on the settings of the NoView and Print flags) but should not respond to mouse
        /// clicks or change its appearance in response to mouse motions.
        /// </description></item>
        /// <item><description>
        /// <see cref="LOCKED"/>
        /// -  If set, do not allow the annotation to be deleted or its properties
        /// (including position and size) to be modified by the user. However, this flag does not restrict changes to
        /// the annotation’s contents, such as the value of a form field.
        /// </description></item>
        /// <item><description>
        /// <see cref="TOGGLE_NO_VIEW"/>
        /// - If set, invert the interpretation of the NoView flag for certain events.
        /// </description></item>
        /// <item><description>
        /// <see cref="LOCKED_CONTENTS"/>
        /// - If set, do not allow the contents of the annotation to be modified
        /// by the user. This flag does not restrict deletion of the annotation or changes to other annotation properties,
        /// such as position and size.
        /// </description></item>
        /// </list>
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

        /// <summary>Resets a flag that specifies a characteristic of the annotation to disabled state (see ISO-320001 12.5.3, "Annotation Flags").
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
        /// is in enabled state (see ISO-320001 12.5.3, "Annotation Flags").
        /// </summary>
        /// <remarks>
        /// Checks if the certain flag that specifies a characteristic of the annotation
        /// is in enabled state (see ISO-320001 12.5.3, "Annotation Flags").
        /// This method allows only one flag to be checked at once, use constants listed in
        /// <see cref="SetFlag(int)"/>.
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
        /// interactions with the user (see ISO-320001 12.5.5, "Appearance Streams").
        /// </summary>
        /// <remarks>
        /// An appearance dictionary specifying how the annotation shall be presented visually on the page during its
        /// interactions with the user (see ISO-320001 12.5.5, "Appearance Streams"). An appearance dictionary is a dictionary
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
        /// corresponding to different appearance states of the annotation. See ISO-320001 12.5.5, "Appearance Streams".
        /// </remarks>
        /// <param name="appearanceType">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// specifying appearance type. Possible types are
        /// <see cref="iText.Kernel.Pdf.PdfName.N">Normal</see>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.R">Rollover</see>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfName.D">Down</see>.
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
        /// <see cref="GetAppearanceObject(iText.Kernel.Pdf.PdfName)"/>.
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
        /// <see cref="GetAppearanceObject(iText.Kernel.Pdf.PdfName)"/>.
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
        /// <see cref="GetAppearanceObject(iText.Kernel.Pdf.PdfName)"/>.
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
        /// <see cref="iText.Kernel.Pdf.PdfName.D">Down</see>.
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
        /// </summary>
        /// <remarks>
        /// Sets a specific type of the appearance using
        /// <see cref="PdfAnnotationAppearance"/>
        /// wrapper.
        /// This method is used to set only an appearance subdictionary. See
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
        /// <see cref="iText.Kernel.Pdf.PdfName.D">Down</see>.
        /// </param>
        /// <param name="appearance">
        /// an appearance subdictionary wrapped in
        /// <see cref="PdfAnnotationAppearance"/>.
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
        /// wrapper.
        /// </summary>
        /// <remarks>
        /// Sets normal appearance using
        /// <see cref="PdfAnnotationAppearance"/>
        /// wrapper. This method is used to set only
        /// appearance subdictionary. See
        /// <see cref="GetNormalAppearanceObject()"/>
        /// and
        /// <see cref="GetAppearanceDictionary()"/>
        /// for more info.
        /// </remarks>
        /// <param name="appearance">
        /// an appearance subdictionary wrapped in
        /// <see cref="PdfAnnotationAppearance"/>.
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
        /// wrapper.
        /// </summary>
        /// <remarks>
        /// Sets rollover appearance using
        /// <see cref="PdfAnnotationAppearance"/>
        /// wrapper. This method is used to set only
        /// appearance subdictionary. See
        /// <see cref="GetRolloverAppearanceObject()"/>
        /// and
        /// <see cref="GetAppearanceDictionary()"/>
        /// for more info.
        /// </remarks>
        /// <param name="appearance">
        /// an appearance subdictionary wrapped in
        /// <see cref="PdfAnnotationAppearance"/>.
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
        /// wrapper.
        /// </summary>
        /// <remarks>
        /// Sets down appearance using
        /// <see cref="PdfAnnotationAppearance"/>
        /// wrapper. This method is used to set only
        /// appearance subdictionary. See
        /// <see cref="GetDownAppearanceObject()"/>
        /// and
        /// <see cref="GetAppearanceDictionary()"/>
        /// for more info.
        /// </remarks>
        /// <param name="appearance">
        /// an appearance subdictionary wrapped in
        /// <see cref="PdfAnnotationAppearance"/>.
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

        /// <summary>An array specifying the characteristics of the annotation’s border.</summary>
        /// <remarks>
        /// An array specifying the characteristics of the annotation’s border.
        /// The array consists of three numbers defining the horizontal corner radius,
        /// vertical corner radius, and border width, all in default user space units.
        /// If the corner radii are 0, the border has square (not rounded) corners; if
        /// the border width is 0, no border is drawn.
        /// <para />
        /// The array may have a fourth element, an optional dash array (see ISO-320001 8.4.3.6, "Line Dash Pattern").
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
        /// <see cref="iText.Kernel.Pdf.PdfAnnotationBorder"/>
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
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetBorder(PdfAnnotationBorder border) {
            return Put(PdfName.Border, border.GetPdfObject());
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
        /// <list type="bullet">
        /// <item><description>The background of the annotation’s icon when closed
        /// </description></item>
        /// <item><description>The title bar of the annotation’s pop-up window
        /// </description></item>
        /// <item><description>The border of a link annotation
        /// </description></item>
        /// </list>
        /// The number of array elements determines the colour space in which the colour shall be defined:
        /// <list type="bullet">
        /// <item><description>0 - No colour; transparent
        /// </description></item>
        /// <item><description>1 - DeviceGray
        /// </description></item>
        /// <item><description>3 - DeviceRGB
        /// </description></item>
        /// <item><description>4 - DeviceCMYK
        /// </description></item>
        /// </list>
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
        /// <see cref="GetColorObject()"/>.
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
        /// <see cref="GetColorObject()"/>.
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
        /// <see cref="GetColorObject()"/>.
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
        /// (see ISO-320001 14.7.4.4, "Finding Structure Elements from Content Items").
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
        /// (see ISO-320001 14.7.4.4, "Finding Structure Elements from Content Items").
        /// </summary>
        /// <remarks>
        /// Sets he integer key of the annotation’s entry in the structural parent tree
        /// (see ISO-320001 14.7.4.4, "Finding Structure Elements from Content Items").
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
        /// (ISO-320001 12.6.4.9, "Movie Actions") may use this title to reference the movie annotation.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which value is an annotation title or null if it isn't specified.
        /// </returns>
        public virtual PdfString GetTitle() {
            return GetPdfObject().GetAsString(PdfName.T);
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
        /// with the specified key.
        /// </summary>
        /// <remarks>
        /// Inserts the value into into the underlying
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// of this
        /// <see cref="PdfAnnotation"/>
        /// and associates it
        /// with the specified key. If the key is already present in this
        /// <see cref="PdfAnnotation"/>
        /// , this method will override
        /// the old value with the specified one.
        /// </remarks>
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
        /// <see cref="PdfAnnotation"/>.
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
        /// <para />
        /// Adds file associated with PDF annotation and identifies the relationship between them.
        /// </summary>
        /// <remarks>
        /// <para />
        /// Adds file associated with PDF annotation and identifies the relationship between them.
        /// <para />
        /// Associated files may be used in Pdf/A-3 and Pdf 2.0 documents.
        /// The method adds file to array value of the AF key in the annotation dictionary.
        /// <para />
        /// For associated files their associated file specification dictionaries shall include the AFRelationship key
        /// </remarks>
        /// <param name="fs">file specification dictionary of associated file</param>
        public virtual void AddAssociatedFile(PdfFileSpec fs) {
            if (null == ((PdfDictionary)fs.GetPdfObject()).Get(PdfName.AFRelationship)) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Annot.PdfAnnotation));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.ASSOCIATED_FILE_SPEC_SHALL_INCLUDE_AFRELATIONSHIP);
            }
            PdfArray afArray = GetPdfObject().GetAsArray(PdfName.AF);
            if (afArray == null) {
                afArray = new PdfArray();
                Put(PdfName.AF, afArray);
            }
            afArray.Add(fs.GetPdfObject());
        }

        /// <summary>Returns files associated with PDF annotation.</summary>
        /// <param name="create">defines whether AF arrays will be created if it doesn't exist</param>
        /// <returns>associated files array</returns>
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
        /// </summary>
        /// <remarks>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>.
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </remarks>
        public override void Flush() {
            base.Flush();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

//\cond DO_NOT_DOCUMENT
        // Created as a private static class in order to facilitate autoport.
        internal class PdfUnknownAnnotation : PdfAnnotation {
            protected internal PdfUnknownAnnotation(PdfDictionary pdfObject)
                : base(pdfObject) {
            }

            public override PdfName GetSubtype() {
                return GetPdfObject().GetAsName(PdfName.Subtype);
            }
        }
//\endcond
    }
}
