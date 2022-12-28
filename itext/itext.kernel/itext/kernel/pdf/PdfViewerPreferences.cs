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
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf {
    public class PdfViewerPreferences : PdfObjectWrapper<PdfDictionary> {
        public enum PdfViewerPreferencesConstants {
            /// <summary>
            /// PageMode constant for
            /// <see cref="PdfName.NonFullScreenPageMode"/>.
            /// </summary>
            USE_NONE,
            /// <summary>
            /// PageMode constant for
            /// <see cref="PdfName.NonFullScreenPageMode"/>.
            /// </summary>
            USE_OUTLINES,
            /// <summary>
            /// PageMode constant for
            /// <see cref="PdfName.NonFullScreenPageMode"/>.
            /// </summary>
            USE_THUMBS,
            /// <summary>
            /// PageMode constant for
            /// <see cref="PdfName.NonFullScreenPageMode"/>.
            /// </summary>
            USE_OC,
            /// <summary>
            /// Direction constant for
            /// <see cref="PdfName.Direction"/>.
            /// </summary>
            LEFT_TO_RIGHT,
            /// <summary>
            /// Direction constant for
            /// <see cref="PdfName.Direction"/>.
            /// </summary>
            RIGHT_TO_LEFT,
            /// <summary>
            /// PageBoundary constant for
            /// <see cref="VIEW_AREA"/>
            /// ,
            /// <see cref="VIEW_CLIP"/>
            /// ,
            /// <see cref="PRINT_AREA"/>
            /// ,
            /// <see cref="PRINT_CLIP"/>.
            /// </summary>
            MEDIA_BOX,
            /// <summary>
            /// PageBoundary constant for
            /// <see cref="VIEW_AREA"/>
            /// ,
            /// <see cref="VIEW_CLIP"/>
            /// ,
            /// <see cref="PRINT_AREA"/>
            /// ,
            /// <see cref="PRINT_CLIP"/>.
            /// </summary>
            CROP_BOX,
            /// <summary>
            /// PageBoundary constant for
            /// <see cref="VIEW_AREA"/>
            /// ,
            /// <see cref="VIEW_CLIP"/>
            /// ,
            /// <see cref="PRINT_AREA"/>
            /// ,
            /// <see cref="PRINT_CLIP"/>.
            /// </summary>
            BLEED_BOX,
            /// <summary>
            /// PageBoundary constant for
            /// <see cref="VIEW_AREA"/>
            /// ,
            /// <see cref="VIEW_CLIP"/>
            /// ,
            /// <see cref="PRINT_AREA"/>
            /// ,
            /// <see cref="PRINT_CLIP"/>.
            /// </summary>
            TRIM_BOX,
            /// <summary>
            /// PageBoundary constant for
            /// <see cref="VIEW_AREA"/>
            /// ,
            /// <see cref="VIEW_CLIP"/>
            /// ,
            /// <see cref="PRINT_AREA"/>
            /// ,
            /// <see cref="PRINT_CLIP"/>.
            /// </summary>
            ART_BOX,
            /// <summary>ViewArea constant.</summary>
            VIEW_AREA,
            /// <summary>ViewClip constant.</summary>
            VIEW_CLIP,
            /// <summary>PrintArea constant.</summary>
            PRINT_AREA,
            /// <summary>PrintClip constant.</summary>
            PRINT_CLIP,
            /// <summary>
            /// Page scaling option constant for
            /// <see cref="PdfName.PrintScaling"/>.
            /// </summary>
            NONE,
            /// <summary>
            /// Page scaling option constant for
            /// <see cref="PdfName.PrintScaling"/>.
            /// </summary>
            APP_DEFAULT,
            /// <summary>
            /// The paper handling option constant for
            /// <see cref="PdfName.Duplex"/>.
            /// </summary>
            SIMPLEX,
            /// <summary>
            /// The paper handling option constant for
            /// <see cref="PdfName.Duplex"/>.
            /// </summary>
            DUPLEX_FLIP_SHORT_EDGE,
            /// <summary>
            /// The paper handling option constant for
            /// <see cref="PdfName.Duplex"/>.
            /// </summary>
            DUPLEX_FLIP_LONG_EDGE
        }

        public PdfViewerPreferences()
            : this(new PdfDictionary()) {
        }

        public PdfViewerPreferences(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>This method sets HideToolBar flag to true or false</summary>
        /// <param name="hideToolbar">HideToolBar flag's boolean value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetHideToolbar(bool hideToolbar) {
            return Put(PdfName.HideToolbar, PdfBoolean.ValueOf(hideToolbar));
        }

        /// <summary>This method sets HideMenuBar flag to true or false</summary>
        /// <param name="hideMenubar">HideMenuBar flag's boolean value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetHideMenubar(bool hideMenubar) {
            return Put(PdfName.HideMenubar, PdfBoolean.ValueOf(hideMenubar));
        }

        /// <summary>This method sets HideWindowUI flag to true or false</summary>
        /// <param name="hideWindowUI">HideWindowUI flag's boolean value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetHideWindowUI(bool hideWindowUI) {
            return Put(PdfName.HideWindowUI, PdfBoolean.ValueOf(hideWindowUI));
        }

        /// <summary>This method sets FitWindow flag to true or false</summary>
        /// <param name="fitWindow">FitWindow flag's boolean value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetFitWindow(bool fitWindow) {
            return Put(PdfName.FitWindow, PdfBoolean.ValueOf(fitWindow));
        }

        /// <summary>This method sets CenterWindow flag to true or false</summary>
        /// <param name="centerWindow">CenterWindow flag's boolean value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetCenterWindow(bool centerWindow) {
            return Put(PdfName.CenterWindow, PdfBoolean.ValueOf(centerWindow));
        }

        /// <summary>This method sets DisplayDocTitle flag to true or false</summary>
        /// <param name="displayDocTitle">DisplayDocTitle flag's boolean value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetDisplayDocTitle(bool displayDocTitle) {
            return Put(PdfName.DisplayDocTitle, PdfBoolean.ValueOf(displayDocTitle));
        }

        /// <summary>This method sets NonFullScreenPageMode property.</summary>
        /// <remarks>
        /// This method sets NonFullScreenPageMode property. Allowed values are UseNone, UseOutlines, useThumbs, UseOC.
        /// This entry is meaningful only if the value of the PageMode entry in the Catalog dictionary is FullScreen
        /// </remarks>
        /// <param name="nonFullScreenPageMode">NonFullScreenPageMode property type value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetNonFullScreenPageMode(PdfViewerPreferences.PdfViewerPreferencesConstants
             nonFullScreenPageMode) {
            switch (nonFullScreenPageMode) {
                case PdfViewerPreferences.PdfViewerPreferencesConstants.USE_NONE: {
                    Put(PdfName.NonFullScreenPageMode, PdfName.UseNone);
                    break;
                }

                case PdfViewerPreferences.PdfViewerPreferencesConstants.USE_OUTLINES: {
                    Put(PdfName.NonFullScreenPageMode, PdfName.UseOutlines);
                    break;
                }

                case PdfViewerPreferences.PdfViewerPreferencesConstants.USE_THUMBS: {
                    Put(PdfName.NonFullScreenPageMode, PdfName.UseThumbs);
                    break;
                }

                case PdfViewerPreferences.PdfViewerPreferencesConstants.USE_OC: {
                    Put(PdfName.NonFullScreenPageMode, PdfName.UseOC);
                    break;
                }

                default: {
                    break;
                }
            }
            return this;
        }

        /// <summary>This method sets predominant reading order of text.</summary>
        /// <param name="direction">reading order type value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetDirection(PdfViewerPreferences.PdfViewerPreferencesConstants direction
            ) {
            switch (direction) {
                case PdfViewerPreferences.PdfViewerPreferencesConstants.LEFT_TO_RIGHT: {
                    Put(PdfName.Direction, PdfName.L2R);
                    break;
                }

                case PdfViewerPreferences.PdfViewerPreferencesConstants.RIGHT_TO_LEFT: {
                    Put(PdfName.Direction, PdfName.R2L);
                    break;
                }

                default: {
                    break;
                }
            }
            return this;
        }

        /// <summary>
        /// This method sets the name of the page boundary representing the area of a page that shall be displayed when
        /// viewing the document on the screen.
        /// </summary>
        /// <remarks>
        /// This method sets the name of the page boundary representing the area of a page that shall be displayed when
        /// viewing the document on the screen.
        /// Deprecated in PDF 2.0.
        /// </remarks>
        /// <param name="pageBoundary">page boundary type value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetViewArea(PdfViewerPreferences.PdfViewerPreferencesConstants pageBoundary
            ) {
            return SetPageBoundary(PdfViewerPreferences.PdfViewerPreferencesConstants.VIEW_AREA, pageBoundary);
        }

        /// <summary>
        /// This method sets the name of the page boundary to which the contents of a page shall be clipped when
        /// viewing the document on the screen.
        /// </summary>
        /// <remarks>
        /// This method sets the name of the page boundary to which the contents of a page shall be clipped when
        /// viewing the document on the screen.
        /// Deprecated in PDF 2.0.
        /// </remarks>
        /// <param name="pageBoundary">page boundary type value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetViewClip(PdfViewerPreferences.PdfViewerPreferencesConstants pageBoundary
            ) {
            return SetPageBoundary(PdfViewerPreferences.PdfViewerPreferencesConstants.VIEW_CLIP, pageBoundary);
        }

        /// <summary>
        /// This method sets the name of the page boundary representing the area of a page that shall be
        /// rendered when printing the document.
        /// </summary>
        /// <remarks>
        /// This method sets the name of the page boundary representing the area of a page that shall be
        /// rendered when printing the document.
        /// Deprecated in PDF 2.0.
        /// </remarks>
        /// <param name="pageBoundary">page boundary type value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetPrintArea(PdfViewerPreferences.PdfViewerPreferencesConstants pageBoundary
            ) {
            return SetPageBoundary(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA, pageBoundary);
        }

        /// <summary>
        /// This method sets the name of the page boundary to which the contents of a page shall be clipped when
        /// printing the document.
        /// </summary>
        /// <remarks>
        /// This method sets the name of the page boundary to which the contents of a page shall be clipped when
        /// printing the document.
        /// Deprecated in PDF 2.0.
        /// </remarks>
        /// <param name="pageBoundary">page boundary type value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetPrintClip(PdfViewerPreferences.PdfViewerPreferencesConstants pageBoundary
            ) {
            return SetPageBoundary(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_CLIP, pageBoundary);
        }

        /// <summary>
        /// This method sets the page scaling option that shall be selected when a print dialog is displayed for this
        /// document.
        /// </summary>
        /// <remarks>
        /// This method sets the page scaling option that shall be selected when a print dialog is displayed for this
        /// document. Valid values are None and AppDefault.
        /// </remarks>
        /// <param name="printScaling">page scaling option's type value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetPrintScaling(PdfViewerPreferences.PdfViewerPreferencesConstants printScaling
            ) {
            switch (printScaling) {
                case PdfViewerPreferences.PdfViewerPreferencesConstants.NONE: {
                    Put(PdfName.PrintScaling, PdfName.None);
                    break;
                }

                case PdfViewerPreferences.PdfViewerPreferencesConstants.APP_DEFAULT: {
                    Put(PdfName.PrintScaling, PdfName.AppDefault);
                    break;
                }

                default: {
                    break;
                }
            }
            return this;
        }

        /// <summary>This method sets the paper handling option that shall be used when printing the file from the print dialog.
        ///     </summary>
        /// <remarks>
        /// This method sets the paper handling option that shall be used when printing the file from the print dialog.
        /// The following values are valid: Simplex, DuplexFlipShortEdge, DuplexFlipLongEdge.
        /// </remarks>
        /// <param name="duplex">paper handling option's type value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetDuplex(PdfViewerPreferences.PdfViewerPreferencesConstants duplex) {
            switch (duplex) {
                case PdfViewerPreferences.PdfViewerPreferencesConstants.SIMPLEX: {
                    Put(PdfName.Duplex, PdfName.Simplex);
                    break;
                }

                case PdfViewerPreferences.PdfViewerPreferencesConstants.DUPLEX_FLIP_SHORT_EDGE: {
                    Put(PdfName.Duplex, PdfName.DuplexFlipShortEdge);
                    break;
                }

                case PdfViewerPreferences.PdfViewerPreferencesConstants.DUPLEX_FLIP_LONG_EDGE: {
                    Put(PdfName.Duplex, PdfName.DuplexFlipLongEdge);
                    break;
                }

                default: {
                    break;
                }
            }
            return this;
        }

        /// <summary>This method sets PickTrayByPDFSize flag to true or false.</summary>
        /// <param name="pickTrayByPdfSize">PickTrayByPDFSize flag's boolean value</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetPickTrayByPDFSize(bool pickTrayByPdfSize) {
            return Put(PdfName.PickTrayByPDFSize, PdfBoolean.ValueOf(pickTrayByPdfSize));
        }

        /// <summary>This method sets the page numbers used to initialize the print dialog box when the file is printed.
        ///     </summary>
        /// <param name="printPageRange">the array of page numbers</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetPrintPageRange(int[] printPageRange) {
            return Put(PdfName.PrintPageRange, new PdfArray(printPageRange));
        }

        /// <summary>This method sets the number of copies that shall be printed when the print dialog is opened for this file.
        ///     </summary>
        /// <param name="numCopies">the number of copies to print when the print dialog is opened</param>
        /// <returns>
        /// current instance of
        /// <see cref="PdfViewerPreferences"/>
        /// </returns>
        public virtual PdfViewerPreferences SetNumCopies(int numCopies) {
            return Put(PdfName.NumCopies, new PdfNumber(numCopies));
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>
        /// PDF 2.0. Sets an array of names of Viewer preference settings that
        /// shall be enforced by PDF processors and that shall not be overridden by
        /// subsequent selections in the application user interface
        /// </remarks>
        /// <param name="enforce">array of names specifying settings to enforce in the PDF processors</param>
        /// <returns>
        /// this
        /// <see cref="PdfViewerPreferences"/>
        /// instance
        /// </returns>
        public virtual PdfViewerPreferences SetEnforce(PdfArray enforce) {
            for (int i = 0; i < enforce.Size(); i++) {
                PdfName curEnforce = enforce.GetAsName(i);
                if (curEnforce == null) {
                    throw new ArgumentException("Enforce array shall contain PdfName entries");
                }
                else {
                    if (PdfName.PrintScaling.Equals(curEnforce)) {
                        // This name may appear in the Enforce array only if the corresponding entry in
                        // the viewer preferences dictionary specifies a valid value other than AppDefault
                        PdfName curPrintScaling = GetPdfObject().GetAsName(PdfName.PrintScaling);
                        if (curPrintScaling == null || PdfName.AppDefault.Equals(curPrintScaling)) {
                            throw new PdfException(KernelExceptionMessageConstant.PRINT_SCALING_ENFORCE_ENTRY_INVALID);
                        }
                    }
                }
            }
            return Put(PdfName.Enforce, enforce);
        }

        /// <summary>PDF 2.0.</summary>
        /// <remarks>
        /// PDF 2.0. Gets an array of names of Viewer preference settings that
        /// shall be enforced by PDF processors and that shall not be overridden by
        /// subsequent selections in the application user interface
        /// </remarks>
        /// <returns>array of names specifying settings to enforce in the PDF processors</returns>
        public virtual PdfArray GetEnforce() {
            return GetPdfObject().GetAsArray(PdfName.Enforce);
        }

        public virtual PdfViewerPreferences Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }

        private PdfViewerPreferences SetPageBoundary(PdfViewerPreferences.PdfViewerPreferencesConstants viewerPreferenceType
            , PdfViewerPreferences.PdfViewerPreferencesConstants pageBoundary) {
            PdfName type = null;
            switch (viewerPreferenceType) {
                case PdfViewerPreferences.PdfViewerPreferencesConstants.VIEW_AREA: {
                    type = PdfName.ViewArea;
                    break;
                }

                case PdfViewerPreferences.PdfViewerPreferencesConstants.VIEW_CLIP: {
                    type = PdfName.ViewClip;
                    break;
                }

                case PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA: {
                    type = PdfName.PrintArea;
                    break;
                }

                case PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_CLIP: {
                    type = PdfName.PrintClip;
                    break;
                }

                default: {
                    break;
                }
            }
            if (type != null) {
                switch (pageBoundary) {
                    case PdfViewerPreferences.PdfViewerPreferencesConstants.MEDIA_BOX: {
                        Put(type, PdfName.MediaBox);
                        break;
                    }

                    case PdfViewerPreferences.PdfViewerPreferencesConstants.CROP_BOX: {
                        Put(type, PdfName.CropBox);
                        break;
                    }

                    case PdfViewerPreferences.PdfViewerPreferencesConstants.BLEED_BOX: {
                        Put(type, PdfName.BleedBox);
                        break;
                    }

                    case PdfViewerPreferences.PdfViewerPreferencesConstants.TRIM_BOX: {
                        Put(type, PdfName.TrimBox);
                        break;
                    }

                    case PdfViewerPreferences.PdfViewerPreferencesConstants.ART_BOX: {
                        Put(type, PdfName.ArtBox);
                        break;
                    }

                    default: {
                        break;
                    }
                }
            }
            return this;
        }
    }
}
