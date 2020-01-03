/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.IO.Colors;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Pdfa.Checker {
    /// <summary>
    /// An abstract class that will run through all necessary checks defined in the
    /// different PDF/A standards and levels.
    /// </summary>
    /// <remarks>
    /// An abstract class that will run through all necessary checks defined in the
    /// different PDF/A standards and levels. A number of common checks are executed
    /// in this class, while standard-dependent specifications are implemented in the
    /// available subclasses. The standard that is followed is the series of ISO
    /// 19005 specifications, currently generations 1 through 3. The ZUGFeRD standard
    /// is derived from ISO 19005-3.
    /// While it is possible to subclass this method and implement its abstract
    /// methods in client code, this is not encouraged and will have little effect.
    /// It is not possible to plug custom implementations into iText, because iText
    /// should always refuse to create non-compliant PDF/A, which would be possible
    /// with client code implementations. Any future generations of the PDF/A
    /// standard and its derivates will get their own implementation in the
    /// iText 7 - pdfa project.
    /// </remarks>
    public abstract class PdfAChecker {
        /// <summary>
        /// The Red-Green-Blue color profile as defined by the International Color
        /// Consortium.
        /// </summary>
        public const String ICC_COLOR_SPACE_RGB = "RGB ";

        /// <summary>
        /// The Cyan-Magenta-Yellow-Key (black) color profile as defined by the
        /// International Color Consortium.
        /// </summary>
        public const String ICC_COLOR_SPACE_CMYK = "CMYK";

        /// <summary>
        /// The Grayscale color profile as defined by the International Color
        /// Consortium.
        /// </summary>
        public const String ICC_COLOR_SPACE_GRAY = "GRAY";

        /// <summary>The Output device class</summary>
        public const String ICC_DEVICE_CLASS_OUTPUT_PROFILE = "prtr";

        /// <summary>The Monitor device class</summary>
        public const String ICC_DEVICE_CLASS_MONITOR_PROFILE = "mntr";

        /// <summary>
        /// The maximum Graphics State stack depth in PDF/A documents, i.e. the
        /// maximum number of graphics state operators with code <c>q</c> that
        /// may be opened (i.e. not yet closed by a corresponding <c>Q</c>) at
        /// any point in a content stream sequence.
        /// </summary>
        /// <remarks>
        /// The maximum Graphics State stack depth in PDF/A documents, i.e. the
        /// maximum number of graphics state operators with code <c>q</c> that
        /// may be opened (i.e. not yet closed by a corresponding <c>Q</c>) at
        /// any point in a content stream sequence.
        /// Defined as 28 by PDF/A-1 section 6.1.12, by referring to the PDF spec
        /// Appendix C table 1 "architectural limits".
        /// </remarks>
        public const int maxGsStackDepth = 28;

        protected internal PdfAConformanceLevel conformanceLevel;

        protected internal String pdfAOutputIntentColorSpace;

        protected internal int gsStackDepth = 0;

        protected internal bool rgbIsUsed = false;

        protected internal bool cmykIsUsed = false;

        protected internal bool grayIsUsed = false;

        /// <summary>Contains some objects that are already checked.</summary>
        /// <remarks>
        /// Contains some objects that are already checked.
        /// NOTE: Not all objects that were checked are stored in that set. This set is used for avoiding double checks for
        /// actions, xObjects and page objects; and for letting those objects to be manually flushed.
        /// Use this mechanism carefully: objects that are able to be changed (or at least if object's properties
        /// that shall be checked are able to be changed) shouldn't be marked as checked if they are not to be
        /// flushed immediately.
        /// </remarks>
        protected internal ICollection<PdfObject> checkedObjects = new HashSet<PdfObject>();

        protected internal IDictionary<PdfObject, PdfColorSpace> checkedObjectsColorspace = new Dictionary<PdfObject
            , PdfColorSpace>();

        protected internal PdfAChecker(PdfAConformanceLevel conformanceLevel) {
            this.conformanceLevel = conformanceLevel;
        }

        /// <summary>
        /// This method checks a number of document-wide requirements of the PDF/A
        /// standard.
        /// </summary>
        /// <remarks>
        /// This method checks a number of document-wide requirements of the PDF/A
        /// standard. The algorithms of some of these checks vary with the PDF/A
        /// level and thus are implemented in subclasses; others are implemented
        /// as private methods in this class.
        /// </remarks>
        /// <param name="catalog">The catalog being checked</param>
        public virtual void CheckDocument(PdfCatalog catalog) {
            PdfDictionary catalogDict = catalog.GetPdfObject();
            SetPdfAOutputIntentColorSpace(catalogDict);
            CheckOutputIntents(catalogDict);
            CheckMetaData(catalogDict);
            CheckCatalogValidEntries(catalogDict);
            CheckTrailer(catalog.GetDocument().GetTrailer());
            CheckLogicalStructure(catalogDict);
            CheckForm(catalogDict.GetAsDictionary(PdfName.AcroForm));
            CheckOutlines(catalogDict);
            CheckPages(catalog.GetDocument());
            CheckOpenAction(catalogDict.Get(PdfName.OpenAction));
            CheckColorsUsages();
        }

        /// <summary>
        /// This method checks all requirements that must be fulfilled by a page in a
        /// PDF/A document.
        /// </summary>
        /// <param name="page">the page that must be checked</param>
        public virtual void CheckSinglePage(PdfPage page) {
            CheckPage(page);
        }

        /// <summary>
        /// This method checks the requirements that must be fulfilled by a COS
        /// object in a PDF/A document.
        /// </summary>
        /// <param name="obj">the COS object that must be checked</param>
        public virtual void CheckPdfObject(PdfObject obj) {
            switch (obj.GetObjectType()) {
                case PdfObject.NUMBER: {
                    CheckPdfNumber((PdfNumber)obj);
                    break;
                }

                case PdfObject.STREAM: {
                    CheckPdfStream((PdfStream)obj);
                    break;
                }

                case PdfObject.STRING: {
                    CheckPdfString((PdfString)obj);
                    break;
                }

                case PdfObject.DICTIONARY: {
                    PdfDictionary dict = (PdfDictionary)obj;
                    PdfName type = dict.GetAsName(PdfName.Type);
                    if (PdfName.Filespec.Equals(type)) {
                        CheckFileSpec(dict);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// for this file.
        /// </summary>
        /// <returns>the defined conformance level for this document.</returns>
        public virtual PdfAConformanceLevel GetConformanceLevel() {
            return conformanceLevel;
        }

        /// <summary>
        /// Remembers which objects have already been checked, in order to avoid
        /// redundant checks.
        /// </summary>
        /// <param name="object">the object to check</param>
        /// <returns>whether or not the object has already been checked</returns>
        public virtual bool ObjectIsChecked(PdfObject @object) {
            return checkedObjects.Contains(@object);
        }

        /// <summary>
        /// This method checks compliance of the tag structure elements, such as struct elements
        /// or parent tree entries.
        /// </summary>
        /// <param name="obj">an object that represents tag structure element.</param>
        public virtual void CheckTagStructureElement(PdfObject obj) {
            // We don't check tag structure as there are no strict constraints,
            // so we just mark tag structure elements to be able to flush them
            checkedObjects.Add(obj);
        }

        /// <summary>
        /// This method checks compliance with the graphics state architectural
        /// limitation, explained by
        /// <see cref="maxGsStackDepth"/>.
        /// </summary>
        /// <param name="stackOperation">the operation to check the graphics state counter for</param>
        public abstract void CheckCanvasStack(char stackOperation);

        /// <summary>
        /// This method checks compliance with the inline image restrictions in the
        /// PDF/A specs, specifically filter parameters.
        /// </summary>
        /// <param name="inlineImage">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// containing the inline image
        /// </param>
        /// <param name="currentColorSpaces">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// containing the color spaces used in the document
        /// </param>
        public abstract void CheckInlineImage(PdfStream inlineImage, PdfDictionary currentColorSpaces);

        /// <summary>
        /// This method checks compliance with the color restrictions imposed by the
        /// available color spaces in the document.
        /// </summary>
        /// <param name="color">the color to check</param>
        /// <param name="currentColorSpaces">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// containing the color spaces used in the document
        /// </param>
        /// <param name="fill">whether the color is used for fill or stroke operations</param>
        [System.ObsoleteAttribute(@"This method will be replaced by CheckColor(iText.Kernel.Colors.Color, iText.Kernel.Pdf.PdfDictionary, bool?, iText.Kernel.Pdf.PdfStream) checkColor in 7.2 release"
            )]
        public abstract void CheckColor(Color color, PdfDictionary currentColorSpaces, bool? fill);

        /// <summary>
        /// This method checks compliance with the color restrictions imposed by the
        /// available color spaces in the document.
        /// </summary>
        /// <remarks>
        /// This method checks compliance with the color restrictions imposed by the
        /// available color spaces in the document.
        /// This method will be abstract in update 7.2
        /// </remarks>
        /// <param name="color">the color to check</param>
        /// <param name="currentColorSpaces">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// containing the color spaces used in the document
        /// </param>
        /// <param name="fill">whether the color is used for fill or stroke operations</param>
        /// <param name="contentStream">current content stream</param>
        public virtual void CheckColor(Color color, PdfDictionary currentColorSpaces, bool? fill, PdfStream contentStream
            ) {
        }

        /// <summary>
        /// This method performs a range of checks on the given color space, depending
        /// on the type and properties of that color space.
        /// </summary>
        /// <param name="colorSpace">the color space to check</param>
        /// <param name="currentColorSpaces">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// containing the color spaces used in the document
        /// </param>
        /// <param name="checkAlternate">whether or not to also check the parent color space</param>
        /// <param name="fill">whether the color space is used for fill or stroke operations</param>
        public abstract void CheckColorSpace(PdfColorSpace colorSpace, PdfDictionary currentColorSpaces, bool checkAlternate
            , bool? fill);

        /// <summary>
        /// Checks whether the rendering intent of the document is within the allowed
        /// range of intents.
        /// </summary>
        /// <remarks>
        /// Checks whether the rendering intent of the document is within the allowed
        /// range of intents. This is defined in ISO 19005-1 section 6.2.9, and
        /// unchanged in newer generations of the PDF/A specification.
        /// </remarks>
        /// <param name="intent">the intent to be analyzed</param>
        public abstract void CheckRenderingIntent(PdfName intent);

        /// <summary>
        /// Performs a number of checks on the graphics state, among others ISO
        /// 19005-1 section 6.2.8 and 6.4 and ISO 19005-2 section 6.2.5 and 6.2.10.
        /// </summary>
        /// <param name="extGState">the graphics state to be checked</param>
        [System.ObsoleteAttribute(@"This method will be replaced by CheckExtGState(iText.Kernel.Pdf.Canvas.CanvasGraphicsState, iText.Kernel.Pdf.PdfStream) checkExtGState in 7.2 release"
            )]
        public abstract void CheckExtGState(CanvasGraphicsState extGState);

        /// <summary>
        /// Performs a number of checks on the graphics state, among others ISO
        /// 19005-1 section 6.2.8 and 6.4 and ISO 19005-2 section 6.2.5 and 6.2.10.
        /// </summary>
        /// <remarks>
        /// Performs a number of checks on the graphics state, among others ISO
        /// 19005-1 section 6.2.8 and 6.4 and ISO 19005-2 section 6.2.5 and 6.2.10.
        /// This method will be abstract in the update 7.2
        /// </remarks>
        /// <param name="extGState">the graphics state to be checked</param>
        /// <param name="contentStream">current content stream</param>
        public virtual void CheckExtGState(CanvasGraphicsState extGState, PdfStream contentStream) {
        }

        /// <summary>Performs a number of checks on the font.</summary>
        /// <remarks>
        /// Performs a number of checks on the font. See ISO 19005-1 section 6.3,
        /// ISO 19005-2 and ISO 19005-3 section 6.2.11.
        /// Be aware that not all constraints defined in the ISO are checked in this method,
        /// for most of them we consider that iText always creates valid fonts.
        /// </remarks>
        /// <param name="pdfFont">font to be checked</param>
        public abstract void CheckFont(PdfFont pdfFont);

        /// <summary>Performs a check of the each font glyph as a Form XObject.</summary>
        /// <remarks>
        /// Performs a check of the each font glyph as a Form XObject. See ISO 19005-2 Annex A.5.
        /// This only applies to type 3 fonts.
        /// This method will be abstract in update 7.2
        /// </remarks>
        /// <param name="font">
        /// 
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// to be checked
        /// </param>
        /// <param name="contentStream">stream containing checked font</param>
        public virtual void CheckFontGlyphs(PdfFont font, PdfStream contentStream) {
        }

        protected internal virtual void CheckPageTransparency(PdfDictionary pageDict, PdfDictionary pageResources) {
        }

        protected internal abstract ICollection<PdfName> GetForbiddenActions();

        protected internal abstract ICollection<PdfName> GetAllowedNamedActions();

        protected internal abstract void CheckAction(PdfDictionary action);

        protected internal abstract void CheckAnnotation(PdfDictionary annotDic);

        protected internal abstract void CheckCatalogValidEntries(PdfDictionary catalogDict);

        protected internal abstract void CheckColorsUsages();

        protected internal abstract void CheckImage(PdfStream image, PdfDictionary currentColorSpaces);

        protected internal abstract void CheckFileSpec(PdfDictionary fileSpec);

        protected internal abstract void CheckForm(PdfDictionary form);

        protected internal abstract void CheckFormXObject(PdfStream form);

        protected internal abstract void CheckLogicalStructure(PdfDictionary catalog);

        protected internal abstract void CheckMetaData(PdfDictionary catalog);

        protected internal abstract void CheckNonSymbolicTrueTypeFont(PdfTrueTypeFont trueTypeFont);

        protected internal abstract void CheckOutputIntents(PdfDictionary catalog);

        protected internal abstract void CheckPageObject(PdfDictionary page, PdfDictionary pageResources);

        protected internal abstract void CheckPageSize(PdfDictionary page);

        protected internal abstract void CheckPdfNumber(PdfNumber number);

        protected internal abstract void CheckPdfStream(PdfStream stream);

        protected internal abstract void CheckPdfString(PdfString @string);

        protected internal abstract void CheckSymbolicTrueTypeFont(PdfTrueTypeFont trueTypeFont);

        protected internal abstract void CheckTrailer(PdfDictionary trailer);

        protected internal virtual void CheckResources(PdfDictionary resources) {
            if (resources == null) {
                return;
            }
            PdfDictionary xObjects = resources.GetAsDictionary(PdfName.XObject);
            PdfDictionary shadings = resources.GetAsDictionary(PdfName.Shading);
            PdfDictionary patterns = resources.GetAsDictionary(PdfName.Pattern);
            if (xObjects != null) {
                foreach (PdfObject xObject in xObjects.Values()) {
                    PdfStream xObjStream = (PdfStream)xObject;
                    PdfObject subtype = null;
                    bool isFlushed = xObjStream.IsFlushed();
                    if (!isFlushed) {
                        subtype = xObjStream.Get(PdfName.Subtype);
                    }
                    if (PdfName.Image.Equals(subtype) || isFlushed) {
                        // if flushed still may be need to check colorspace in given context
                        CheckImage(xObjStream, resources.GetAsDictionary(PdfName.ColorSpace));
                    }
                    else {
                        if (PdfName.Form.Equals(subtype)) {
                            CheckFormXObject(xObjStream);
                        }
                    }
                }
            }
            if (shadings != null) {
                foreach (PdfObject shading in shadings.Values()) {
                    PdfDictionary shadingDict = (PdfDictionary)shading;
                    if (!IsAlreadyChecked(shadingDict)) {
                        CheckColorSpace(PdfColorSpace.MakeColorSpace(shadingDict.Get(PdfName.ColorSpace)), resources.GetAsDictionary
                            (PdfName.ColorSpace), true, null);
                    }
                }
            }
            if (patterns != null) {
                foreach (PdfObject p in patterns.Values()) {
                    if (p.IsStream()) {
                        PdfStream pStream = (PdfStream)p;
                        if (!IsAlreadyChecked(pStream)) {
                            CheckResources(pStream.GetAsDictionary(PdfName.Resources));
                        }
                    }
                }
            }
        }

        protected internal static bool CheckFlag(int flags, int flag) {
            return (flags & flag) != 0;
        }

        protected internal static bool CheckStructure(PdfAConformanceLevel conformanceLevel) {
            return conformanceLevel == PdfAConformanceLevel.PDF_A_1A || conformanceLevel == PdfAConformanceLevel.PDF_A_2A
                 || conformanceLevel == PdfAConformanceLevel.PDF_A_3A;
        }

        protected internal virtual bool IsAlreadyChecked(PdfDictionary dictionary) {
            if (checkedObjects.Contains(dictionary)) {
                return true;
            }
            checkedObjects.Add(dictionary);
            return false;
        }

        protected internal virtual void CheckResourcesOfAppearanceStreams(PdfDictionary appearanceStreamsDict) {
            foreach (PdfObject val in appearanceStreamsDict.Values()) {
                if (val is PdfDictionary) {
                    PdfDictionary ap = (PdfDictionary)val;
                    if (ap.IsDictionary()) {
                        CheckResourcesOfAppearanceStreams(ap);
                    }
                    else {
                        if (ap.IsStream()) {
                            if (!IsAlreadyChecked(ap)) {
                                CheckResources(ap.GetAsDictionary(PdfName.Resources));
                            }
                        }
                    }
                }
            }
        }

        private void CheckPages(PdfDocument document) {
            for (int i = 1; i <= document.GetNumberOfPages(); i++) {
                CheckPage(document.GetPage(i));
            }
        }

        private void CheckPage(PdfPage page) {
            PdfDictionary pageDict = page.GetPdfObject();
            if (IsAlreadyChecked(pageDict)) {
                return;
            }
            CheckPageObject(pageDict, page.GetResources().GetPdfObject());
            PdfDictionary pageResources = page.GetResources().GetPdfObject();
            CheckResources(pageResources);
            CheckAnnotations(pageDict);
            CheckPageSize(pageDict);
            CheckPageTransparency(pageDict, page.GetResources().GetPdfObject());
            int contentStreamCount = page.GetContentStreamCount();
            for (int j = 0; j < contentStreamCount; ++j) {
                checkedObjects.Add(page.GetContentStream(j));
            }
        }

        private void CheckOpenAction(PdfObject openAction) {
            if (openAction != null && openAction.IsDictionary()) {
                CheckAction((PdfDictionary)openAction);
            }
        }

        private void CheckAnnotations(PdfDictionary page) {
            PdfArray annots = page.GetAsArray(PdfName.Annots);
            if (annots != null) {
                for (int i = 0; i < annots.Size(); i++) {
                    PdfDictionary annot = annots.GetAsDictionary(i);
                    CheckAnnotation(annot);
                    PdfDictionary action = annot.GetAsDictionary(PdfName.A);
                    if (action != null) {
                        CheckAction(action);
                    }
                }
            }
        }

        private void CheckOutlines(PdfDictionary catalogDict) {
            PdfDictionary outlines = catalogDict.GetAsDictionary(PdfName.Outlines);
            if (outlines != null) {
                foreach (PdfDictionary outline in GetOutlines(outlines)) {
                    PdfDictionary action = outline.GetAsDictionary(PdfName.A);
                    if (action != null) {
                        CheckAction(action);
                    }
                }
            }
        }

        private IList<PdfDictionary> GetOutlines(PdfDictionary item) {
            IList<PdfDictionary> outlines = new List<PdfDictionary>();
            outlines.Add(item);
            PdfDictionary processItem = item.GetAsDictionary(PdfName.First);
            if (processItem != null) {
                outlines.AddAll(GetOutlines(processItem));
            }
            processItem = item.GetAsDictionary(PdfName.Next);
            if (processItem != null) {
                outlines.AddAll(GetOutlines(processItem));
            }
            return outlines;
        }

        private void SetPdfAOutputIntentColorSpace(PdfDictionary catalog) {
            PdfArray outputIntents = catalog.GetAsArray(PdfName.OutputIntents);
            if (outputIntents == null) {
                return;
            }
            PdfDictionary pdfAOutputIntent = GetPdfAOutputIntent(outputIntents);
            SetCheckerOutputIntent(pdfAOutputIntent);
        }

        private PdfDictionary GetPdfAOutputIntent(PdfArray outputIntents) {
            for (int i = 0; i < outputIntents.Size(); ++i) {
                PdfName outputIntentSubtype = outputIntents.GetAsDictionary(i).GetAsName(PdfName.S);
                if (PdfName.GTS_PDFA1.Equals(outputIntentSubtype)) {
                    return outputIntents.GetAsDictionary(i);
                }
            }
            return null;
        }

        private void SetCheckerOutputIntent(PdfDictionary outputIntent) {
            if (outputIntent != null) {
                PdfStream destOutputProfile = outputIntent.GetAsStream(PdfName.DestOutputProfile);
                if (destOutputProfile != null) {
                    String intentCS = IccProfile.GetIccColorSpaceName(destOutputProfile.GetBytes());
                    this.pdfAOutputIntentColorSpace = intentCS;
                }
            }
        }
    }
}
