/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Layer;

namespace iText.Kernel.Pdf.Annot {
    public abstract class PdfAnnotation : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Annotation flags.</summary>
        public const int INVISIBLE = 1;

        public const int HIDDEN = 2;

        public const int PRINT = 4;

        public const int NO_ZOOM = 8;

        public const int NO_ROTATE = 16;

        public const int NO_VIEW = 32;

        public const int READ_ONLY = 64;

        public const int LOCKED = 128;

        public const int TOGGLE_NO_VIEW = 256;

        public const int LOCKED_CONTENTS = 512;

        /// <summary>Annotation highlighting modes.</summary>
        public static readonly PdfName HIGHLIGHT_NONE = PdfName.N;

        public static readonly PdfName HIGHLIGHT_INVERT = PdfName.I;

        public static readonly PdfName HIGHLIGHT_OUTLINE = PdfName.O;

        public static readonly PdfName HIGHLIGHT_PUSH = PdfName.P;

        public static readonly PdfName HIGHLIGHT_TOGGLE = PdfName.T;

        /// <summary>Annotation highlighting modes.</summary>
        public static readonly PdfName STYLE_SOLID = PdfName.S;

        public static readonly PdfName STYLE_DASHED = PdfName.D;

        public static readonly PdfName STYLE_BEVELED = PdfName.B;

        public static readonly PdfName STYLE_INSET = PdfName.I;

        public static readonly PdfName STYLE_UNDERLINE = PdfName.U;

        /// <summary>Annotation states.</summary>
        public static readonly PdfString Marked = new PdfString("Marked");

        public static readonly PdfString Unmarked = new PdfString("Unmarked");

        public static readonly PdfString Accepted = new PdfString("Accepted");

        public static readonly PdfString Rejected = new PdfString("Rejected");

        public static readonly PdfString Canceled = new PdfString("Cancelled");

        public static readonly PdfString Completed = new PdfString("Completed");

        public static readonly PdfString None = new PdfString("None");

        /// <summary>Annotation state models.</summary>
        public static readonly PdfString MarkedModel = new PdfString("Marked");

        public static readonly PdfString ReviewModel = new PdfString("Review");

        protected internal PdfPage page;

        public static iText.Kernel.Pdf.Annot.PdfAnnotation MakeAnnotation(PdfObject pdfObject, iText.Kernel.Pdf.Annot.PdfAnnotation
             parent) {
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
            if (annotation is PdfMarkupAnnotation) {
                PdfMarkupAnnotation markup = (PdfMarkupAnnotation)annotation;
                PdfDictionary inReplyTo = markup.GetInReplyToObject();
                if (inReplyTo != null) {
                    markup.SetInReplyTo(MakeAnnotation(inReplyTo));
                }
                PdfDictionary popup = markup.GetPopupObject();
                if (popup != null) {
                    markup.SetPopup((PdfPopupAnnotation)MakeAnnotation(popup, markup));
                }
            }
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

        public abstract PdfName GetSubtype();

        /// <summary>Sets the layer this annotation belongs to.</summary>
        /// <param name="layer">the layer this annotation belongs to</param>
        public virtual void SetLayer(IPdfOCG layer) {
            GetPdfObject().Put(PdfName.OC, layer.GetIndirectReference());
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetAction(PdfAction action) {
            return Put(PdfName.A, action.GetPdfObject());
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetAdditionalAction(PdfName key, PdfAction action) {
            PdfAction.SetAdditionalAction(this, key, action);
            return this;
        }

        public virtual PdfString GetContents() {
            return GetPdfObject().GetAsString(PdfName.Contents);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetContents(PdfString contents) {
            return Put(PdfName.Contents, contents);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetContents(String contents) {
            return SetContents(new PdfString(contents));
        }

        public virtual PdfDictionary GetPageObject() {
            return GetPdfObject().GetAsDictionary(PdfName.P);
        }

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

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetPage(PdfPage page) {
            this.page = page;
            return Put(PdfName.P, page.GetPdfObject());
        }

        public virtual PdfString GetName() {
            return GetPdfObject().GetAsString(PdfName.NM);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetName(PdfString name) {
            return Put(PdfName.NM, name);
        }

        public virtual PdfString GetDate() {
            return GetPdfObject().GetAsString(PdfName.M);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetDate(PdfString date) {
            return Put(PdfName.M, date);
        }

        public virtual int GetFlags() {
            PdfNumber f = GetPdfObject().GetAsNumber(PdfName.F);
            if (f != null) {
                return f.IntValue();
            }
            else {
                return 0;
            }
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetFlags(int flags) {
            return Put(PdfName.F, new PdfNumber(flags));
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetFlag(int flag) {
            int flags = GetFlags();
            flags = flags | flag;
            return SetFlags(flags);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation ResetFlag(int flag) {
            int flags = GetFlags();
            flags = flags & (~flag & 0xff);
            return SetFlags(flags);
        }

        public virtual bool HasFlag(int flag) {
            int flags = GetFlags();
            return (flags & flag) != 0;
        }

        public virtual PdfDictionary GetAppearanceDictionary() {
            return GetPdfObject().GetAsDictionary(PdfName.AP);
        }

        public virtual PdfDictionary GetAppearanceObject(PdfName appearanceType) {
            PdfDictionary ap = GetAppearanceDictionary();
            if (ap != null) {
                return ap.GetAsDictionary(appearanceType);
            }
            return null;
        }

        public virtual PdfDictionary GetNormalAppearanceObject() {
            return GetAppearanceObject(PdfName.N);
        }

        public virtual PdfDictionary GetRolloverAppearanceObject() {
            return GetAppearanceObject(PdfName.R);
        }

        public virtual PdfDictionary GetDownAppearanceObject() {
            return GetAppearanceObject(PdfName.D);
        }

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

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetNormalAppearance(PdfDictionary appearance) {
            return SetAppearance(PdfName.N, appearance);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetRolloverAppearance(PdfDictionary appearance) {
            return SetAppearance(PdfName.R, appearance);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetDownAppearance(PdfDictionary appearance) {
            return SetAppearance(PdfName.D, appearance);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetAppearance(PdfName appearanceType, PdfAnnotationAppearance
             appearance) {
            return SetAppearance(appearanceType, appearance.GetPdfObject());
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetNormalAppearance(PdfAnnotationAppearance appearance
            ) {
            return SetAppearance(PdfName.N, appearance);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetRolloverAppearance(PdfAnnotationAppearance appearance
            ) {
            return SetAppearance(PdfName.R, appearance);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetDownAppearance(PdfAnnotationAppearance appearance) {
            return SetAppearance(PdfName.D, appearance);
        }

        public virtual PdfName GetAppearanceState() {
            return GetPdfObject().GetAsName(PdfName.AS);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetAppearanceState(PdfName @as) {
            return Put(PdfName.AS, @as);
        }

        public virtual PdfArray GetBorder() {
            return GetPdfObject().GetAsArray(PdfName.Border);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetBorder(PdfArray border) {
            return Put(PdfName.Border, border);
        }

        public virtual PdfArray GetColorObject() {
            return GetPdfObject().GetAsArray(PdfName.C);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetColor(PdfArray color) {
            return Put(PdfName.C, color);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetColor(float[] color) {
            return SetColor(new PdfArray(color));
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetColor(Color color) {
            return SetColor(new PdfArray(color.GetColorValue()));
        }

        public virtual int GetStructParentIndex() {
            PdfNumber n = GetPdfObject().GetAsNumber(PdfName.StructParent);
            if (n == null) {
                return -1;
            }
            else {
                return n.IntValue();
            }
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetStructParentIndex(int structParentIndex) {
            return Put(PdfName.StructParent, new PdfNumber(structParentIndex));
        }

        public virtual PdfArray GetQuadPoints() {
            return GetPdfObject().GetAsArray(PdfName.QuadPoints);
        }

        public virtual bool GetOpen() {
            PdfBoolean open = GetPdfObject().GetAsBoolean(PdfName.Open);
            return open != null && open.GetValue();
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetOpen(bool open) {
            return Put(PdfName.Open, new PdfBoolean(open));
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetQuadPoints(PdfArray quadPoints) {
            return Put(PdfName.QuadPoints, quadPoints);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetBorderStyle(PdfDictionary borderStyle) {
            return Put(PdfName.BS, borderStyle);
        }

        /// <summary>Setter for the annotation's border style.</summary>
        /// <remarks>
        /// Setter for the annotation's border style. Possible values are
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
        /// </remarks>
        /// <param name="style">The new value for the annotation's border style.</param>
        /// <returns>The annotation which this method was called on.</returns>
        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetBorderStyle(PdfName style) {
            PdfDictionary styleDict = GetBorderStyle();
            if (null == styleDict) {
                styleDict = new PdfDictionary();
            }
            styleDict.Put(PdfName.S, style);
            return SetBorderStyle(styleDict);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetDashPattern(PdfArray dashPattern) {
            PdfDictionary styleDict = GetBorderStyle();
            if (null == styleDict) {
                styleDict = new PdfDictionary();
            }
            styleDict.Put(PdfName.D, dashPattern);
            return SetBorderStyle(styleDict);
        }

        public virtual PdfDictionary GetBorderStyle() {
            return GetPdfObject().GetAsDictionary(PdfName.BS);
        }

        public static iText.Kernel.Pdf.Annot.PdfAnnotation MakeAnnotation(PdfObject pdfObject) {
            return MakeAnnotation(pdfObject, null);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetTitle(PdfString title) {
            return Put(PdfName.T, title);
        }

        public virtual PdfString GetTitle() {
            return GetPdfObject().GetAsString(PdfName.T);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetAppearanceCharacteristics(PdfDictionary characteristics
            ) {
            return Put(PdfName.MK, characteristics);
        }

        public virtual PdfDictionary GetAppearanceCharacteristics() {
            return GetPdfObject().GetAsDictionary(PdfName.MK);
        }

        public virtual PdfDictionary GetAction() {
            return GetPdfObject().GetAsDictionary(PdfName.A);
        }

        public virtual PdfDictionary GetAdditionalAction() {
            return GetPdfObject().GetAsDictionary(PdfName.AA);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation SetRectangle(PdfArray array) {
            return Put(PdfName.Rect, array);
        }

        public virtual PdfArray GetRectangle() {
            return GetPdfObject().GetAsArray(PdfName.Rect);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotation Remove(PdfName key) {
            GetPdfObject().Remove(key);
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
