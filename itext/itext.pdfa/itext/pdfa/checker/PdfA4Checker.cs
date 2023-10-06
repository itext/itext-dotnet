/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Properties;
using iText.Pdfa.Exceptions;
using iText.Pdfa.Logs;

namespace iText.Pdfa.Checker {
    /// <summary>
    /// PdfA4Checker defines the requirements of the PDF/A-4 standard and contains a
    /// number of methods that override the implementations of its superclass
    /// <see cref="PdfA3Checker"/>.
    /// </summary>
    /// <remarks>
    /// PdfA4Checker defines the requirements of the PDF/A-4 standard and contains a
    /// number of methods that override the implementations of its superclass
    /// <see cref="PdfA3Checker"/>.
    /// <para />
    /// The specification implemented by this class is ISO 19005-4
    /// </remarks>
    public class PdfA4Checker : PdfA3Checker {
        private const String CALRGB_COLOR_SPACE = "CalRGB";

        private static readonly ICollection<PdfName> forbiddenAnnotations4 = JavaCollectionsUtil.UnmodifiableSet(new 
            HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName._3D, PdfName.RichMedia, PdfName.FileAttachment, PdfName
            .Sound, PdfName.Screen, PdfName.Movie)));

        private static readonly ICollection<PdfName> forbiddenAnnotations4E = JavaCollectionsUtil.UnmodifiableSet(
            new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.FileAttachment, PdfName.Sound, PdfName.Screen, PdfName
            .Movie)));

        private static readonly ICollection<PdfName> forbiddenAnnotations4F = JavaCollectionsUtil.UnmodifiableSet(
            new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName._3D, PdfName.RichMedia, PdfName.Sound, PdfName.Screen
            , PdfName.Movie)));

        private static readonly ICollection<PdfName> apLessAnnotations = JavaCollectionsUtil.UnmodifiableSet(new HashSet
            <PdfName>(JavaUtil.ArraysAsList(PdfName.Popup, PdfName.Link, PdfName.Projection)));

        private static readonly ICollection<PdfName> allowedBlendModes4 = JavaCollectionsUtil.UnmodifiableSet(new 
            HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Normal, PdfName.Multiply, PdfName.Screen, PdfName.Overlay
            , PdfName.Darken, PdfName.Lighten, PdfName.ColorDodge, PdfName.ColorBurn, PdfName.HardLight, PdfName.SoftLight
            , PdfName.Difference, PdfName.Exclusion, PdfName.Hue, PdfName.Saturation, PdfName.Color, PdfName.Luminosity
            )));

        private const String TRANSPARENCY_ERROR_MESSAGE = PdfaExceptionMessageConstant.THE_DOCUMENT_AND_THE_PAGE_DO_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE;

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(PdfAChecker));

        private static readonly ICollection<PdfName> forbiddenActionsE = JavaCollectionsUtil.UnmodifiableSet(new HashSet
            <PdfName>(JavaUtil.ArraysAsList(PdfName.Launch, PdfName.Sound, PdfName.Movie, PdfName.ResetForm, PdfName
            .ImportData, PdfName.JavaScript, PdfName.Hide, PdfName.Rendition, PdfName.Trans)));

        private static readonly ICollection<PdfName> allowedEntriesInAAWhenNonWidget = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.E, PdfName.X, PdfName.D, PdfName.U, PdfName.Fo, PdfName
            .Bl)));

        // Map pdfObject using CMYK - list of CMYK icc profile streams
        private IDictionary<PdfObject, IList<PdfStream>> iccBasedCmykObjects = new Dictionary<PdfObject, IList<PdfStream
            >>();

        /// <summary>Creates a PdfA4Checker with the required conformance level</summary>
        /// <param name="conformanceLevel">the required conformance level</param>
        public PdfA4Checker(PdfAConformanceLevel conformanceLevel)
            : base(conformanceLevel) {
        }

        /// <summary><inheritDoc/></summary>
        public override void CheckColorSpace(PdfColorSpace colorSpace, PdfObject pdfObject, PdfDictionary currentColorSpaces
            , bool checkAlternate, bool? fill) {
            if (colorSpace is PdfCieBasedCs.IccBased) {
                // 6.2.4.2: An ICCBased colour space shall not be used where the profile is a CMYK destination profile and is
                // identical to that in the current PDF/A OutputIntent or the current transparency blending colorspace.
                PdfStream iccStream = ((PdfArray)colorSpace.GetPdfObject()).GetAsStream(1);
                byte[] iccBytes = iccStream.GetBytes();
                // If not CMYK - we don't care
                if (ICC_COLOR_SPACE_CMYK.Equals(IccProfile.GetIccColorSpaceName(iccBytes))) {
                    if (!iccBasedCmykObjects.ContainsKey(pdfObject)) {
                        iccBasedCmykObjects.Put(pdfObject, new List<PdfStream>());
                    }
                    iccBasedCmykObjects.Get(pdfObject).Add(iccStream);
                }
            }
            base.CheckColorSpace(colorSpace, pdfObject, currentColorSpaces, checkAlternate, fill);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckPageColorsUsages(PdfDictionary pageDict, PdfDictionary pageResources
            ) {
            // Get page pdf/a output intent output profile
            PdfStream pageDestOutputProfile = null;
            PdfArray outputIntents = pageDict.GetAsArray(PdfName.OutputIntents);
            if (outputIntents != null) {
                PdfDictionary pdfAPageOutputIntent = GetPdfAOutputIntent(outputIntents);
                if (pdfAPageOutputIntent != null) {
                    pageDestOutputProfile = pdfAPageOutputIntent.GetAsStream(PdfName.DestOutputProfile);
                }
            }
            if (pageDestOutputProfile == null) {
                pageDestOutputProfile = pdfAOutputIntentDestProfile;
            }
            // Page blending colorspace should be taken into account while checking objects using device dependent colors
            PdfColorSpace pageTransparencyBlendingCS = GetDeviceIndependentTransparencyBlendingCSIfRbgOrCmykBased(pageDict
                );
            // We don't know on which pages these objects are so that we have to go inside anyway
            if (!rgbUsedObjects.IsEmpty() || !cmykUsedObjects.IsEmpty() || !grayUsedObjects.IsEmpty() || !iccBasedCmykObjects
                .IsEmpty()) {
                CheckPageContentsForColorUsages(pageDict, pageDestOutputProfile, pageTransparencyBlendingCS);
                CheckAnnotationsForColorUsages(pageDict.GetAsArray(PdfName.Annots), pageDestOutputProfile, pageTransparencyBlendingCS
                    );
                CheckResourcesForColorUsages(pageResources, pageDestOutputProfile, pageTransparencyBlendingCS);
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckTrailer(PdfDictionary trailer) {
            base.CheckTrailer(trailer);
            if (trailer.Get(PdfName.Info) != null) {
                PdfDictionary info = trailer.GetAsDictionary(PdfName.Info);
                if (info.Size() != 1 || info.Get(PdfName.ModDate) == null) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.DOCUMENT_INFO_DICTIONARY_SHALL_ONLY_CONTAIN_MOD_DATE
                        );
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckCatalog(PdfCatalog catalog) {
            if ('2' != catalog.GetDocument().GetPdfVersion().ToString()[4]) {
                throw new PdfAConformanceException(MessageFormatUtil.Format(PdfaExceptionMessageConstant.THE_FILE_HEADER_SHALL_CONTAIN_RIGHT_PDF_VERSION
                    , "2"));
            }
            PdfDictionary trailer = catalog.GetDocument().GetTrailer();
            if (trailer.Get(PdfName.Info) != null) {
                if (catalog.GetPdfObject().Get(PdfName.PieceInfo) == null) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.DOCUMENT_SHALL_NOT_CONTAIN_INFO_UNLESS_THERE_IS_PIECE_INFO
                        );
                }
            }
            if ("F".Equals(conformanceLevel.GetConformance())) {
                if (!catalog.NameTreeContainsKey(PdfName.EmbeddedFiles)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.NAME_DICTIONARY_SHALL_CONTAIN_EMBEDDED_FILES_KEY
                        );
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckCatalogValidEntries(PdfDictionary catalogDict) {
            base.CheckCatalogValidEntries(catalogDict);
            PdfString version = catalogDict.GetAsString(PdfName.Version);
            if (version != null && (version.ToString()[0] != '2' || version.ToString()[1] != '.' || !char.IsDigit(version
                .ToString()[2]))) {
                throw new PdfAConformanceException(MessageFormatUtil.Format(PdfaExceptionMessageConstant.THE_CATALOG_VERSION_SHALL_CONTAIN_RIGHT_PDF_VERSION
                    , "2"));
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckFileSpec(PdfDictionary fileSpec) {
            if (fileSpec.GetAsName(PdfName.AFRelationship) == null) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_AFRELATIONSHIP_KEY
                    );
            }
            if (!fileSpec.ContainsKey(PdfName.F) || !fileSpec.ContainsKey(PdfName.UF)) {
                throw new PdfAConformanceException(PdfAConformanceException.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                    );
            }
            if (!fileSpec.ContainsKey(PdfName.Desc)) {
                LOGGER.LogWarning(PdfAConformanceLogMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHOULD_CONTAIN_DESC_KEY);
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckPageTransparency(PdfDictionary pageDict, PdfDictionary pageResources
            ) {
            // Get page pdf/a output intent
            PdfDictionary pdfAPageOutputIntent = null;
            PdfArray outputIntents = pageDict.GetAsArray(PdfName.OutputIntents);
            if (outputIntents != null) {
                pdfAPageOutputIntent = GetPdfAOutputIntent(outputIntents);
            }
            if (pdfAOutputIntentColorSpace == null && pdfAPageOutputIntent == null && !transparencyObjects.IsEmpty() &&
                 (pageDict.GetAsDictionary(PdfName.Group) == null || pageDict.GetAsDictionary(PdfName.Group).Get(PdfName
                .CS) == null)) {
                CheckContentsForTransparency(pageDict);
                CheckAnnotationsForTransparency(pageDict.GetAsArray(PdfName.Annots));
                CheckResourcesForTransparency(pageResources, new HashSet<PdfObject>());
            }
        }

        /// <summary>Check the conformity of the AA dictionary on catalog level.</summary>
        /// <param name="dict">the catalog dictionary</param>
        protected internal override void CheckCatalogAAConformance(PdfDictionary dict) {
            PdfDictionary aa = dict.GetAsDictionary(PdfName.AA);
            if (aa != null && HasAAIllegalEntries(aa)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.CATALOG_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                    );
            }
        }

        /// <summary>Check the conformity of the AA dictionary on catalog level.</summary>
        /// <param name="dict">the catalog dictionary</param>
        protected internal override void CheckPageAAConformance(PdfDictionary dict) {
            PdfDictionary aa = dict.GetAsDictionary(PdfName.AA);
            if (aa != null && HasAAIllegalEntries(aa)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.PAGE_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                    );
            }
        }

        //There are no limits for numbers in pdf-a/4
        /// <summary><inheritDoc/></summary>
        protected internal override void CheckPdfNumber(PdfNumber number) {
        }

        //There is no limit for canvas stack in pdf-a/4
        /// <summary><inheritDoc/></summary>
        public override void CheckCanvasStack(char stackOperation) {
        }

        /// <summary><inheritDoc/></summary>
        public override void CheckSignatureType(bool isCAdES) {
            if (!isCAdES) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.SIGNATURE_SHALL_CONFORM_TO_ONE_OF_THE_PADES_PROFILE
                    );
            }
        }

        //There is no limit for String length in pdf-a/4
        /// <summary><inheritDoc/></summary>
        protected internal override int GetMaxStringLength() {
            return int.MaxValue;
        }

        //There is no limit for DeviceN components count in pdf-a/4
        /// <summary><inheritDoc/></summary>
        protected internal override void CheckNumberOfDeviceNComponents(PdfSpecialCs.DeviceN deviceN) {
        }

        public override void CheckExtGState(CanvasGraphicsState extGState, PdfStream contentStream) {
            base.CheckExtGState(extGState, contentStream);
            if (extGState.GetHalftone() is PdfDictionary) {
                PdfDictionary halftoneDict = (PdfDictionary)extGState.GetHalftone();
                if (halftoneDict.ContainsKey(PdfName.TransferFunction)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.ALL_HALFTONES_CONTAINING_TRANSFER_FUNCTION_SHALL_HAVE_HALFTONETYPE_5
                        );
                }
                int halftoneType = halftoneDict.GetAsInt(PdfName.HalftoneType).Value;
                if (halftoneType == 5) {
                    foreach (KeyValuePair<PdfName, PdfObject> entry in halftoneDict.EntrySet()) {
                        //see ISO_32000_2;2020 table 132
                        if (PdfName.Type.Equals(entry.Key) || PdfName.HalftoneType.Equals(entry.Key) || PdfName.HalftoneName.Equals
                            (entry.Key)) {
                            continue;
                        }
                        if (entry.Value is PdfDictionary && IsCMYKColorant(entry.Key) && entry.Value is PdfDictionary && ((PdfDictionary
                            )entry.Value).ContainsKey(PdfName.TransferFunction)) {
                            throw new PdfAConformanceException(PdfaExceptionMessageConstant.ALL_HALFTONES_CONTAINING_TRANSFER_FUNCTION_SHALL_HAVE_HALFTONETYPE_5
                                );
                        }
                    }
                }
            }
        }

        protected internal override void CheckFormXObject(PdfStream form) {
            if (IsAlreadyChecked(form)) {
                return;
            }
            if (form.ContainsKey(PdfName.OPI)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_OPI_KEY
                    );
            }
            if (form.ContainsKey(PdfName.Ref)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_REF_KEY
                    );
            }
            CheckTransparencyGroup(form, null);
            CheckResources(form.GetAsDictionary(PdfName.Resources), form);
            CheckContentStream(form);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckAnnotation(PdfDictionary annotDic) {
            base.CheckAnnotation(annotDic);
            // Extra check for blending mode
            PdfName blendMode = annotDic.GetAsName(PdfName.BM);
            if (blendMode != null && !allowedBlendModes4.Contains(blendMode)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.ONLY_STANDARD_BLEND_MODES_SHALL_BE_USED_FOR_THE_VALUE_OF_THE_BM_KEY_IN_A_GRAPHIC_STATE_AND_ANNOTATION_DICTIONARY
                    );
            }
            // And then treat the annotation as an object with transparency
            if (blendMode != null && !PdfName.Normal.Equals(blendMode)) {
                transparencyObjects.Add(annotDic);
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override ICollection<PdfName> GetForbiddenAnnotations() {
            if ("E".Equals(conformanceLevel.GetConformance())) {
                return forbiddenAnnotations4E;
            }
            else {
                if ("F".Equals(conformanceLevel.GetConformance())) {
                    return forbiddenAnnotations4F;
                }
            }
            return forbiddenAnnotations4;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override ICollection<PdfName> GetAppearanceLessAnnotations() {
            return apLessAnnotations;
        }

        /// <summary>Check the conformity of the AA dictionary on widget level.</summary>
        /// <param name="dict">the widget dictionary</param>
        protected internal virtual void CheckWidgetAAConformance(PdfDictionary dict) {
            if (!PdfName.Widget.Equals(dict.GetAsName(PdfName.Subtype)) && dict.ContainsKey(PdfName.AA)) {
                PdfObject additionalActions = dict.Get(PdfName.AA);
                if (additionalActions.IsDictionary() && HasAAIllegalEntries((PdfDictionary)additionalActions)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.ANNOTATION_AA_DICTIONARY_SHALL_CONTAIN_ONLY_ALLOWED_KEYS
                        );
                }
            }
        }

        /// <param name="catalog">
        /// the catalog
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// to check
        /// </param>
        protected internal override void CheckMetaData(PdfDictionary catalog) {
            base.CheckMetaData(catalog);
            try {
                PdfStream xmpMetadata = catalog.GetAsStream(PdfName.Metadata);
                byte[] bytes = xmpMetadata.GetBytes();
                CheckPacketHeader(bytes);
                XMPMeta meta = XMPMetaFactory.Parse(new MemoryStream(bytes));
                CheckVersionIdentification(meta);
                CheckFileProvenanceSpec(meta);
            }
            catch (XMPException ex) {
                throw new PdfException(ex);
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckAnnotationAgainstActions(PdfDictionary annotDic) {
            if (PdfName.Widget.Equals(annotDic.GetAsName(PdfName.Subtype)) && annotDic.ContainsKey(PdfName.A)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.WIDGET_ANNOTATION_DICTIONARY_OR_FIELD_DICTIONARY_SHALL_NOT_INCLUDE_A_ENTRY
                    );
            }
            CheckWidgetAAConformance(annotDic);
        }

        private static bool HasAAIllegalEntries(PdfDictionary aa) {
            foreach (PdfName key in aa.KeySet()) {
                if (!allowedEntriesInAAWhenNonWidget.Contains(key)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override ICollection<PdfName> GetForbiddenActions() {
            if ("E".Equals(conformanceLevel.GetConformance())) {
                return forbiddenActionsE;
            }
            return base.GetForbiddenActions();
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckContentConfigurationDictAgainstAsKey(PdfDictionary config) {
        }

        // Do nothing because in PDF/A-4 AS key may appear in any optional content configuration dictionary.
        /// <summary><inheritDoc/></summary>
        protected internal override String GetTransparencyErrorMessage() {
            return TRANSPARENCY_ERROR_MESSAGE;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckBlendMode(PdfName blendMode) {
            if (!allowedBlendModes4.Contains(blendMode)) {
                throw new PdfAConformanceException(PdfAConformanceException.ONLY_STANDARD_BLEND_MODES_SHALL_BE_USED_FOR_THE_VALUE_OF_THE_BM_KEY_IN_AN_EXTENDED_GRAPHIC_STATE_DICTIONARY
                    );
            }
        }

        private static bool IsValidXmpConformance(String value) {
            if (value == null) {
                return false;
            }
            if (value.Length != 1) {
                return false;
            }
            return "F".Equals(value) || "E".Equals(value);
        }

        private static bool IsValidXmpRevision(String value) {
            if (value == null) {
                return false;
            }
            if (value.Length != 4) {
                return false;
            }
            foreach (char c in value.ToCharArray()) {
                if (!char.IsDigit(c)) {
                    return false;
                }
            }
            return true;
        }

        private void CheckPacketHeader(byte[] meta) {
            if (meta == null) {
                return;
            }
            String metAsStr = iText.Commons.Utils.JavaUtil.GetStringForBytes(meta);
            String regex = "<\\?xpacket.*encoding|bytes.*\\?>";
            Regex pattern = iText.Commons.Utils.StringUtil.RegexCompile(regex);
            if (iText.Commons.Utils.Matcher.Match(pattern, metAsStr).Find()) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.XMP_METADATA_HEADER_PACKET_MAY_NOT_CONTAIN_BYTES_OR_ENCODING_ATTRIBUTE
                    );
            }
        }

        private void CheckFileProvenanceSpec(XMPMeta meta) {
            try {
                XMPProperty history = meta.GetProperty(XMPConst.NS_XMP_MM, XMPConst.HISTORY);
                if (history == null) {
                    return;
                }
                if (!history.GetOptions().IsArray()) {
                    return;
                }
                int amountOfEntries = meta.CountArrayItems(XMPConst.NS_XMP_MM, XMPConst.HISTORY);
                for (int i = 0; i < amountOfEntries; i++) {
                    int nameSpaceIndex = i + 1;
                    if (!meta.DoesPropertyExist(XMPConst.NS_XMP_MM, XMPConst.HISTORY + "[" + nameSpaceIndex + "]/stEvt:action"
                        )) {
                        throw new PdfAConformanceException(MessageFormatUtil.Format(PdfaExceptionMessageConstant.XMP_METADATA_HISTORY_ENTRY_SHALL_CONTAIN_KEY
                            , "stEvt:action"));
                    }
                    if (!meta.DoesPropertyExist(XMPConst.NS_XMP_MM, XMPConst.HISTORY + "[" + nameSpaceIndex + "]/stEvt:when")) {
                        throw new PdfAConformanceException(MessageFormatUtil.Format(PdfaExceptionMessageConstant.XMP_METADATA_HISTORY_ENTRY_SHALL_CONTAIN_KEY
                            , "stEvt:when"));
                    }
                }
            }
            catch (XMPException e) {
                throw new PdfException(e);
            }
        }

        private void CheckVersionIdentification(XMPMeta meta) {
            try {
                XMPProperty prop = meta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART);
                if (prop == null || !GetConformanceLevel().GetPart().Equals(prop.GetValue())) {
                    throw new PdfAConformanceException(MessageFormatUtil.Format(PdfaExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_PART
                        , GetConformanceLevel().GetPart()));
                }
            }
            catch (XMPException) {
                throw new PdfAConformanceException(MessageFormatUtil.Format(PdfaExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_PART
                    , GetConformanceLevel().GetPart()));
            }
            try {
                XMPProperty prop = meta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.REV);
                if (prop == null || !IsValidXmpRevision(prop.GetValue())) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_REV
                        );
                }
            }
            catch (XMPException) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_REV
                    );
            }
            try {
                XMPProperty prop = meta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE);
                if (prop != null && !IsValidXmpConformance(prop.GetValue())) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_CONFORMANCE
                        );
                }
            }
            catch (XMPException) {
            }
        }

        // ignored because it is not required
        private bool IsCMYKColorant(PdfName colourant) {
            return PdfName.Cyan.Equals(colourant) || PdfName.Magenta.Equals(colourant) || PdfName.Yellow.Equals(colourant
                ) || PdfName.Black.Equals(colourant);
        }

        private void CheckPageContentsForColorUsages(PdfDictionary pageDict, PdfStream pageIntentProfile, PdfColorSpace
             pageTransparencyBlendingCS) {
            PdfStream contentStream = pageDict.GetAsStream(PdfName.Contents);
            if (contentStream != null) {
                CheckContentForColorUsages(contentStream, pageIntentProfile, pageTransparencyBlendingCS);
            }
            else {
                PdfArray contentSteamArray = pageDict.GetAsArray(PdfName.Contents);
                if (contentSteamArray != null) {
                    for (int i = 0; i < contentSteamArray.Size(); i++) {
                        CheckContentForColorUsages(contentSteamArray.Get(i), pageIntentProfile, pageTransparencyBlendingCS);
                    }
                }
            }
        }

        private void CheckAnnotationsForColorUsages(PdfArray annotations, PdfStream pageIntentProfile, PdfColorSpace
             pageTransparencyBlendingCS) {
            if (annotations == null) {
                return;
            }
            for (int i = 0; i < annotations.Size(); ++i) {
                PdfDictionary annot = annotations.GetAsDictionary(i);
                PdfDictionary ap = annot.GetAsDictionary(PdfName.AP);
                if (ap != null) {
                    CheckAppearanceStreamForColorUsages(ap, pageIntentProfile, pageTransparencyBlendingCS);
                }
            }
        }

        private void CheckAppearanceStreamForColorUsages(PdfDictionary ap, PdfStream pageIntentProfile, PdfColorSpace
             pageTransparencyBlendingCS) {
            CheckContentForColorUsages(ap, pageIntentProfile, pageTransparencyBlendingCS);
            foreach (PdfObject val in ap.Values()) {
                CheckContentForColorUsages(val, pageIntentProfile, pageTransparencyBlendingCS);
                if (val.IsDictionary()) {
                    CheckAppearanceStreamForColorUsages((PdfDictionary)val, pageIntentProfile, pageTransparencyBlendingCS);
                }
                else {
                    if (val.IsStream()) {
                        CheckObjectWithResourcesForColorUsages(val, pageIntentProfile, pageTransparencyBlendingCS);
                    }
                }
            }
        }

        private void CheckObjectWithResourcesForColorUsages(PdfObject objectWithResources, PdfStream pageIntentProfile
            , PdfColorSpace pageTransparencyBlendingCS) {
            CheckContentForColorUsages(objectWithResources, pageIntentProfile, pageTransparencyBlendingCS);
            if (objectWithResources is PdfDictionary) {
                CheckResourcesForColorUsages(((PdfDictionary)objectWithResources).GetAsDictionary(PdfName.Resources), pageIntentProfile
                    , pageTransparencyBlendingCS);
            }
        }

        private void CheckResourcesForColorUsages(PdfDictionary resources, PdfStream pageIntentProfile, PdfColorSpace
             pageTransparencyBlendingCS) {
            if (resources != null) {
                CheckSingleResourceTypeForColorUsages(resources.GetAsDictionary(PdfName.XObject), pageIntentProfile, pageTransparencyBlendingCS
                    );
                CheckSingleResourceTypeForColorUsages(resources.GetAsDictionary(PdfName.Pattern), pageIntentProfile, pageTransparencyBlendingCS
                    );
            }
        }

        private void CheckSingleResourceTypeForColorUsages(PdfDictionary singleResourceDict, PdfStream pageIntentProfile
            , PdfColorSpace pageTransparencyBlendingCS) {
            if (singleResourceDict != null) {
                foreach (PdfObject resource in singleResourceDict.Values()) {
                    CheckObjectWithResourcesForColorUsages(resource, pageIntentProfile, pageTransparencyBlendingCS);
                }
            }
        }

        private void CheckContentForColorUsages(PdfObject pdfObject, PdfStream pageIntentProfile, PdfColorSpace pageTransparencyBlendingCS
            ) {
            String pageIntentCSType = pageIntentProfile == null ? null : IccProfile.GetIccColorSpaceName(pageIntentProfile
                .GetBytes());
            PdfColorSpace currentTransparencyBlendingCS = pdfObject is PdfDictionary ? GetDeviceIndependentTransparencyBlendingCSIfRbgOrCmykBased
                ((PdfDictionary)pdfObject) : null;
            // Step 1 - 6.2.4.3: check if device dependent color in the object is allowed
            // Current output intent, page blending colorspace and object blending colorspace should be taken into account
            // Step 1.1 - check if any excuse exists
            if (pageIntentCSType == null && pageTransparencyBlendingCS == null && currentTransparencyBlendingCS == null
                ) {
                if (rgbUsedObjects.Contains(pdfObject)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.DEVICERGB_SHALL_ONLY_BE_USED_IF_CURRENT_RGB_PDFA_OUTPUT_INTENT_OR_DEFAULTRGB_IN_USAGE_CONTEXT
                        );
                }
                else {
                    if (cmykUsedObjects.Contains(pdfObject)) {
                        throw new PdfAConformanceException(PdfaExceptionMessageConstant.DEVICECMYK_SHALL_ONLY_BE_USED_IF_CURRENT_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                            );
                    }
                }
            }
            // pageTransparencyBlendingCS currentTransparencyBlendingCS don't help for DeviceGray
            if (grayUsedObjects.Contains(pdfObject) && pageIntentCSType == null) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.DEVICEGRAY_SHALL_ONLY_BE_USED_IF_CURRENT_PDFA_OUTPUT_INTENT_OR_DEFAULTGRAY_IN_USAGE_CONTEXT
                    );
            }
            String pageTransparencyBlendingCSType = GetColorspaceTypeIfIccBasedOrCalRgb(pageTransparencyBlendingCS);
            String currentTransparencyBlendingCSType = GetColorspaceTypeIfIccBasedOrCalRgb(currentTransparencyBlendingCS
                );
            // Step 1.2 - check for RGB
            if (rgbUsedObjects.Contains(pdfObject) && !ICC_COLOR_SPACE_RGB.Equals(pageIntentCSType) && !ICC_COLOR_SPACE_RGB
                .Equals(pageTransparencyBlendingCSType) && !ICC_COLOR_SPACE_RGB.Equals(currentTransparencyBlendingCSType
                ) && !CALRGB_COLOR_SPACE.Equals(pageTransparencyBlendingCSType) && !CALRGB_COLOR_SPACE.Equals(currentTransparencyBlendingCSType
                )) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.DEVICERGB_SHALL_ONLY_BE_USED_IF_CURRENT_RGB_PDFA_OUTPUT_INTENT_OR_DEFAULTRGB_IN_USAGE_CONTEXT
                    );
            }
            // Step 1.3 - check for CMYK
            if (cmykUsedObjects.Contains(pdfObject) && !ICC_COLOR_SPACE_CMYK.Equals(pageIntentCSType) && !ICC_COLOR_SPACE_CMYK
                .Equals(pageTransparencyBlendingCSType) && !ICC_COLOR_SPACE_CMYK.Equals(currentTransparencyBlendingCSType
                )) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.DEVICECMYK_SHALL_ONLY_BE_USED_IF_CURRENT_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                    );
            }
            // Step 2 - 6.2.4.2: An ICCBased colour space shall not be used where the profile is a CMYK destination profile and is
            // identical to that in the current PDF/A OutputIntent or the current transparency blending colorspace.
            IList<PdfStream> currentICCBasedProfiles = iccBasedCmykObjects.Get(pdfObject);
            if (currentICCBasedProfiles == null) {
                return;
            }
            foreach (PdfStream currentICCBasedProfile in currentICCBasedProfiles) {
                ThrowIfIdenticalProfiles(currentICCBasedProfile, pageIntentProfile);
                if (ICC_COLOR_SPACE_CMYK.Equals(currentTransparencyBlendingCSType)) {
                    PdfStream iccStream = ((PdfArray)currentTransparencyBlendingCS.GetPdfObject()).GetAsStream(1);
                    ThrowIfIdenticalProfiles(currentICCBasedProfile, iccStream);
                }
                if (ICC_COLOR_SPACE_CMYK.Equals(pageTransparencyBlendingCSType)) {
                    PdfStream iccStream = ((PdfArray)pageTransparencyBlendingCS.GetPdfObject()).GetAsStream(1);
                    ThrowIfIdenticalProfiles(currentICCBasedProfile, iccStream);
                }
            }
        }

        private static void ThrowIfIdenticalProfiles(PdfStream iccBasedProfile1, PdfStream iccBasedProfile2) {
            if (iccBasedProfile1 != null && iccBasedProfile2 != null && (iccBasedProfile1.Equals(iccBasedProfile2) || 
                JavaUtil.ArraysEquals(iccBasedProfile1.GetBytes(), iccBasedProfile2.GetBytes()))) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.ICCBASED_COLOUR_SPACE_SHALL_NOT_BE_USED_IF_IT_IS_CMYK_AND_IS_IDENTICAL_TO_CURRENT_PROFILE
                    );
            }
        }

        private static String GetColorspaceTypeIfIccBasedOrCalRgb(PdfColorSpace colorspace) {
            if (colorspace is PdfCieBasedCs.CalRgb) {
                return CALRGB_COLOR_SPACE;
            }
            if (colorspace is PdfCieBasedCs.IccBased) {
                // 6.2.4.2: An ICCBased colour space shall not be used where the profile is a CMYK destination profile and is
                // identical to that in the current PDF/A OutputIntent or the current transparency blending colorspace.
                PdfStream iccStream = ((PdfArray)colorspace.GetPdfObject()).GetAsStream(1);
                return IccProfile.GetIccColorSpaceName(iccStream.GetBytes());
            }
            return null;
        }

        private static PdfColorSpace GetDeviceIndependentTransparencyBlendingCSIfRbgOrCmykBased(PdfDictionary pageDict
            ) {
            if (!IsContainsTransparencyGroup(pageDict)) {
                return null;
            }
            PdfObject cs = pageDict.GetAsDictionary(PdfName.Group).Get(PdfName.CS);
            if (cs == null) {
                return null;
            }
            PdfColorSpace transparencyBlendingCS = PdfColorSpace.MakeColorSpace(cs);
            if (transparencyBlendingCS is PdfCieBasedCs.CalRgb || transparencyBlendingCS is PdfCieBasedCs.IccBased) {
                // Do not take others into account
                return transparencyBlendingCS;
            }
            return null;
        }
    }
}
