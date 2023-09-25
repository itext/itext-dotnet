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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms.Fields;
using iText.IO.Colors;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Extgstate;
using iText.Pdfa.Exceptions;
using iText.Pdfa.Logs;

namespace iText.Pdfa.Checker {
    /// <summary>
    /// PdfA2Checker defines the requirements of the PDF/A-2 standard and contains a
    /// number of methods that override the implementations of its superclass
    /// <see cref="PdfA1Checker"/>.
    /// </summary>
    /// <remarks>
    /// PdfA2Checker defines the requirements of the PDF/A-2 standard and contains a
    /// number of methods that override the implementations of its superclass
    /// <see cref="PdfA1Checker"/>.
    /// <para />
    /// The specification implemented by this class is ISO 19005-2
    /// </remarks>
    public class PdfA2Checker : PdfA1Checker {
        protected internal static readonly ICollection<PdfName> forbiddenAnnotations = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName._3D, PdfName.Sound, PdfName.Screen, PdfName.Movie)
            ));

        protected internal static readonly ICollection<PdfName> apLessAnnotations = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Popup, PdfName.Link)));

        protected internal static readonly ICollection<PdfName> forbiddenActions = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Launch, PdfName.Sound, PdfName.Movie, PdfName.ResetForm
            , PdfName.ImportData, PdfName.JavaScript, PdfName.Hide, PdfName.SetOCGState, PdfName.Rendition, PdfName
            .Trans, PdfName.GoTo3DView)));

        protected internal static readonly ICollection<PdfName> allowedBlendModes = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Normal, PdfName.Compatible, PdfName.Multiply, PdfName
            .Screen, PdfName.Overlay, PdfName.Darken, PdfName.Lighten, PdfName.ColorDodge, PdfName.ColorBurn, PdfName
            .HardLight, PdfName.SoftLight, PdfName.Difference, PdfName.Exclusion, PdfName.Hue, PdfName.Saturation, 
            PdfName.Color, PdfName.Luminosity)));

        protected internal static readonly ICollection<PdfName> allowedFilters = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.ASCIIHexDecode, PdfName.ASCII85Decode, PdfName.RunLengthDecode
            , PdfName.FlateDecode, PdfName.CCITTFaxDecode, PdfName.JBIG2Decode, PdfName.DCTDecode, PdfName.JPXDecode
            , PdfName.Crypt)));

        protected internal static readonly ICollection<PdfName> allowedInlineImageFilters = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.DeviceGray, PdfName.DeviceRGB, PdfName.DeviceCMYK, 
            PdfName.Indexed, PdfName.ASCIIHexDecode, PdfName.ASCII85Decode, PdfName.FlateDecode, PdfName.RunLengthDecode
            , PdfName.CCITTFaxDecode, PdfName.DCTDecode, PdfName.G, PdfName.RGB, PdfName.CMYK, PdfName.I, PdfName.
            AHx, PdfName.A85, PdfName.Fl, PdfName.RL, PdfName.CCF, PdfName.DCT)));

        protected internal ICollection<PdfObject> transparencyObjects = new HashSet<PdfObject>();

        internal const int MAX_PAGE_SIZE = 14400;

        internal const int MIN_PAGE_SIZE = 3;

        private const int MAX_NUMBER_OF_DEVICEN_COLOR_COMPONENTS = 32;

        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(PdfAChecker));

        private const String TRANSPARENCY_ERROR_MESSAGE = PdfAConformanceException.THE_DOCUMENT_DOES_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE;

        private bool currentFillCsIsIccBasedCMYK = false;

        private bool currentStrokeCsIsIccBasedCMYK = false;

        private IDictionary<PdfName, PdfArray> separationColorSpaces = new Dictionary<PdfName, PdfArray>();

        /// <summary>Creates a PdfA2Checker with the required conformance level</summary>
        /// <param name="conformanceLevel">
        /// the required conformance level, <c>a</c> or
        /// <c>u</c> or <c>b</c>
        /// </param>
        public PdfA2Checker(PdfAConformanceLevel conformanceLevel)
            : base(conformanceLevel) {
        }

        public override void CheckInlineImage(PdfStream inlineImage, PdfDictionary currentColorSpaces) {
            PdfObject filter = inlineImage.Get(PdfName.Filter);
            if (filter is PdfName) {
                if (filter.Equals(PdfName.LZWDecode)) {
                    throw new PdfAConformanceException(PdfAConformanceException.LZWDECODE_FILTER_IS_NOT_PERMITTED);
                }
                if (filter.Equals(PdfName.Crypt)) {
                    throw new PdfAConformanceException(PdfAConformanceException.CRYPT_FILTER_IS_NOT_PERMITTED_INLINE_IMAGE);
                }
                if (!allowedInlineImageFilters.Contains((PdfName)filter)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.INVALID_INLINE_IMAGE_FILTER_USAGE);
                }
            }
            else {
                if (filter is PdfArray) {
                    for (int i = 0; i < ((PdfArray)filter).Size(); i++) {
                        PdfName f = ((PdfArray)filter).GetAsName(i);
                        if (f.Equals(PdfName.LZWDecode)) {
                            throw new PdfAConformanceException(PdfAConformanceException.LZWDECODE_FILTER_IS_NOT_PERMITTED);
                        }
                        if (f.Equals(PdfName.Crypt)) {
                            throw new PdfAConformanceException(PdfAConformanceException.CRYPT_FILTER_IS_NOT_PERMITTED_INLINE_IMAGE);
                        }
                        if (!allowedInlineImageFilters.Contains((PdfName)f)) {
                            throw new PdfAConformanceException(PdfaExceptionMessageConstant.INVALID_INLINE_IMAGE_FILTER_USAGE);
                        }
                    }
                }
            }
            CheckImage(inlineImage, currentColorSpaces);
        }

        public override void CheckColor(Color color, PdfDictionary currentColorSpaces, bool? fill, PdfStream contentStream
            ) {
            if (color is PatternColor) {
                PdfPattern pattern = ((PatternColor)color).GetPattern();
                if (pattern is PdfPattern.Shading) {
                    PdfDictionary shadingDictionary = ((PdfPattern.Shading)pattern).GetShading();
                    PdfObject colorSpace = shadingDictionary.Get(PdfName.ColorSpace);
                    CheckColorSpace(PdfColorSpace.MakeColorSpace(colorSpace), currentColorSpaces, true, true);
                    PdfDictionary extGStateDict = ((PdfDictionary)pattern.GetPdfObject()).GetAsDictionary(PdfName.ExtGState);
                    CanvasGraphicsState gState = new PdfA2Checker.UpdateCanvasGraphicsState(extGStateDict);
                    CheckExtGState(gState, contentStream);
                }
                else {
                    if (pattern is PdfPattern.Tiling) {
                        CheckContentStream((PdfStream)pattern.GetPdfObject());
                    }
                }
            }
            base.CheckColor(color, currentColorSpaces, fill, contentStream);
        }

        public override void CheckColorSpace(PdfColorSpace colorSpace, PdfDictionary currentColorSpaces, bool checkAlternate
            , bool? fill) {
            if (fill != null) {
                if ((bool)fill) {
                    currentFillCsIsIccBasedCMYK = false;
                }
                else {
                    currentStrokeCsIsIccBasedCMYK = false;
                }
            }
            if (colorSpace is PdfSpecialCs.Separation) {
                PdfSpecialCs.Separation separation = (PdfSpecialCs.Separation)colorSpace;
                CheckSeparationCS((PdfArray)separation.GetPdfObject());
                if (checkAlternate) {
                    CheckColorSpace(separation.GetBaseCs(), currentColorSpaces, false, fill);
                }
            }
            else {
                if (colorSpace is PdfSpecialCs.DeviceN) {
                    PdfSpecialCs.DeviceN deviceN = (PdfSpecialCs.DeviceN)colorSpace;
                    CheckNumberOfDeviceNComponents(deviceN);
                    //TODO DEVSIX-4203 Fix IndexOutOfBounds exception being thrown for DeviceN (not NChannel) colorspace without
                    // attributes. According to the spec PdfAConformanceException should be thrown.
                    PdfDictionary attributes = ((PdfArray)deviceN.GetPdfObject()).GetAsDictionary(4);
                    PdfDictionary colorants = attributes.GetAsDictionary(PdfName.Colorants);
                    //TODO DEVSIX-4203 Colorants dictionary is mandatory in PDF/A-2 spec. Need to throw an appropriate exception
                    // if it is not present.
                    if (colorants != null) {
                        foreach (KeyValuePair<PdfName, PdfObject> entry in colorants.EntrySet()) {
                            PdfArray separation = (PdfArray)entry.Value;
                            CheckSeparationInsideDeviceN(separation, ((PdfArray)deviceN.GetPdfObject()).Get(2), ((PdfArray)deviceN.GetPdfObject
                                ()).Get(3));
                        }
                    }
                    if (checkAlternate) {
                        CheckColorSpace(deviceN.GetBaseCs(), currentColorSpaces, false, fill);
                    }
                }
                else {
                    if (colorSpace is PdfSpecialCs.Indexed) {
                        if (checkAlternate) {
                            CheckColorSpace(((PdfSpecialCs.Indexed)colorSpace).GetBaseCs(), currentColorSpaces, true, fill);
                        }
                    }
                    else {
                        if (colorSpace is PdfSpecialCs.UncoloredTilingPattern) {
                            if (checkAlternate) {
                                CheckColorSpace(((PdfSpecialCs.UncoloredTilingPattern)colorSpace).GetUnderlyingColorSpace(), currentColorSpaces
                                    , true, fill);
                            }
                        }
                        else {
                            if (colorSpace is PdfDeviceCs.Rgb) {
                                if (!CheckDefaultCS(currentColorSpaces, fill, PdfName.DefaultRGB, 3)) {
                                    rgbIsUsed = true;
                                }
                            }
                            else {
                                if (colorSpace is PdfDeviceCs.Cmyk) {
                                    if (!CheckDefaultCS(currentColorSpaces, fill, PdfName.DefaultCMYK, 4)) {
                                        cmykIsUsed = true;
                                    }
                                }
                                else {
                                    if (colorSpace is PdfDeviceCs.Gray) {
                                        if (!CheckDefaultCS(currentColorSpaces, fill, PdfName.DefaultGray, 1)) {
                                            grayIsUsed = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (fill != null && colorSpace is PdfCieBasedCs.IccBased) {
                byte[] iccBytes = ((PdfArray)colorSpace.GetPdfObject()).GetAsStream(1).GetBytes();
                if (ICC_COLOR_SPACE_CMYK.Equals(IccProfile.GetIccColorSpaceName(iccBytes))) {
                    if ((bool)fill) {
                        currentFillCsIsIccBasedCMYK = true;
                    }
                    else {
                        currentStrokeCsIsIccBasedCMYK = true;
                    }
                }
            }
        }

        public override void CheckExtGState(CanvasGraphicsState extGState, PdfStream contentStream) {
            if (Convert.ToInt32(1).Equals(extGState.GetOverprintMode())) {
                if (extGState.GetFillOverprint() && currentFillCsIsIccBasedCMYK) {
                    throw new PdfAConformanceException(PdfAConformanceException.OVERPRINT_MODE_SHALL_NOT_BE_ONE_WHEN_AN_ICCBASED_CMYK_COLOUR_SPACE_IS_USED_AND_WHEN_OVERPRINTING_IS_SET_TO_TRUE
                        );
                }
                if (extGState.GetStrokeOverprint() && currentStrokeCsIsIccBasedCMYK) {
                    throw new PdfAConformanceException(PdfAConformanceException.OVERPRINT_MODE_SHALL_NOT_BE_ONE_WHEN_AN_ICCBASED_CMYK_COLOUR_SPACE_IS_USED_AND_WHEN_OVERPRINTING_IS_SET_TO_TRUE
                        );
                }
            }
            if (extGState.GetTransferFunction() != null) {
                throw new PdfAConformanceException(PdfAConformanceException.AN_EXTGSTATE_DICTIONARY_SHALL_NOT_CONTAIN_THE_TR_KEY
                    );
            }
            if (extGState.GetHTP() != null) {
                throw new PdfAConformanceException(PdfAConformanceException.AN_EXTGSTATE_DICTIONARY_SHALL_NOT_CONTAIN_THE_HTP_KEY
                    );
            }
            PdfObject transferFunction2 = extGState.GetTransferFunction2();
            if (transferFunction2 != null && !PdfName.Default.Equals(transferFunction2)) {
                throw new PdfAConformanceException(PdfAConformanceException.AN_EXTGSTATE_DICTIONARY_SHALL_NOT_CONTAIN_THE_TR_2_KEY_WITH_A_VALUE_OTHER_THAN_DEFAULT
                    );
            }
            if (extGState.GetHalftone() is PdfDictionary) {
                PdfDictionary halftoneDict = (PdfDictionary)extGState.GetHalftone();
                int? halftoneType = halftoneDict.GetAsInt(PdfName.HalftoneType);
                if (halftoneType != 1 && halftoneType != 5) {
                    throw new PdfAConformanceException(PdfAConformanceException.ALL_HALFTONES_SHALL_HAVE_HALFTONETYPE_1_OR_5);
                }
                if (halftoneDict.ContainsKey(PdfName.HalftoneName)) {
                    throw new PdfAConformanceException(PdfAConformanceException.HALFTONES_SHALL_NOT_CONTAIN_HALFTONENAME);
                }
            }
            CheckRenderingIntent(extGState.GetRenderingIntent());
            if (extGState.GetSoftMask() != null && extGState.GetSoftMask() is PdfDictionary) {
                transparencyObjects.Add(contentStream);
            }
            if (extGState.GetStrokeOpacity() < 1) {
                transparencyObjects.Add(contentStream);
            }
            if (extGState.GetFillOpacity() < 1) {
                transparencyObjects.Add(contentStream);
            }
            PdfObject bm = extGState.GetBlendMode();
            if (bm != null) {
                if (!PdfName.Normal.Equals(bm)) {
                    transparencyObjects.Add(contentStream);
                }
                if (bm is PdfArray) {
                    foreach (PdfObject b in (PdfArray)bm) {
                        CheckBlendMode((PdfName)b);
                    }
                }
                else {
                    if (bm is PdfName) {
                        CheckBlendMode((PdfName)bm);
                    }
                }
            }
        }

        public override void CheckSignature(PdfDictionary signatureDict) {
            if (IsAlreadyChecked(signatureDict)) {
                return;
            }
            PdfArray references = signatureDict.GetAsArray(PdfName.Reference);
            if (references != null) {
                for (int i = 0; i < references.Size(); i++) {
                    PdfDictionary referenceDict = references.GetAsDictionary(i);
                    if (referenceDict.ContainsKey(PdfName.DigestLocation) || referenceDict.ContainsKey(PdfName.DigestMethod) ||
                         referenceDict.ContainsKey(PdfName.DigestValue)) {
                        throw new PdfAConformanceException(PdfAConformanceException.SIGNATURE_REFERENCES_DICTIONARY_SHALL_NOT_CONTAIN_DIGESTLOCATION_DIGESTMETHOD_DIGESTVALUE
                            );
                    }
                }
            }
        }

        protected internal virtual void CheckNumberOfDeviceNComponents(PdfSpecialCs.DeviceN deviceN) {
            if (deviceN.GetNumberOfComponents() > MAX_NUMBER_OF_DEVICEN_COLOR_COMPONENTS) {
                throw new PdfAConformanceException(PdfAConformanceException.THE_NUMBER_OF_COLOR_COMPONENTS_IN_DEVICE_N_COLORSPACE_SHOULD_NOT_EXCEED
                    , MAX_NUMBER_OF_DEVICEN_COLOR_COMPONENTS);
            }
        }

        protected internal override void CheckNonSymbolicTrueTypeFont(PdfTrueTypeFont trueTypeFont) {
            String encoding = trueTypeFont.GetFontEncoding().GetBaseEncoding();
            // non-symbolic true type font will always has an encoding entry in font dictionary in itext
            if (!PdfEncodings.WINANSI.Equals(encoding) && !PdfEncodings.MACROMAN.Equals(encoding)) {
                throw new PdfAConformanceException(PdfAConformanceException.ALL_NON_SYMBOLIC_TRUE_TYPE_FONT_SHALL_SPECIFY_MAC_ROMAN_ENCODING_OR_WIN_ANSI_ENCODING
                    , trueTypeFont);
            }
        }

        // if font has differences array, itext ensures that all names in it are listed in AdobeGlyphList
        protected internal override double GetMaxRealValue() {
            return float.MaxValue;
        }

        protected internal override int GetMaxStringLength() {
            return 32767;
        }

        protected internal override void CheckPdfArray(PdfArray array) {
        }

        // currently no validation for arrays is implemented for PDF/A 2
        protected internal override void CheckPdfDictionary(PdfDictionary dictionary) {
        }

        // currently no validation for dictionaries is implemented for PDF/A 2
        /// <summary><inheritDoc/></summary>
        protected internal override void CheckAnnotation(PdfDictionary annotDic) {
            PdfName subtype = annotDic.GetAsName(PdfName.Subtype);
            if (subtype == null) {
                throw new PdfAConformanceException(PdfAConformanceException.ANNOTATION_TYPE_0_IS_NOT_PERMITTED).SetMessageParams
                    ("null");
            }
            if (GetForbiddenAnnotations().Contains(subtype)) {
                throw new PdfAConformanceException(PdfAConformanceException.ANNOTATION_TYPE_0_IS_NOT_PERMITTED).SetMessageParams
                    (subtype.GetValue());
            }
            if (!subtype.Equals(PdfName.Popup)) {
                PdfNumber f = annotDic.GetAsNumber(PdfName.F);
                if (f == null) {
                    throw new PdfAConformanceException(PdfAConformanceException.AN_ANNOTATION_DICTIONARY_SHALL_CONTAIN_THE_F_KEY
                        );
                }
                int flags = f.IntValue();
                if (!CheckFlag(flags, PdfAnnotation.PRINT) || CheckFlag(flags, PdfAnnotation.HIDDEN) || CheckFlag(flags, PdfAnnotation
                    .INVISIBLE) || CheckFlag(flags, PdfAnnotation.NO_VIEW) || CheckFlag(flags, PdfAnnotation.TOGGLE_NO_VIEW
                    )) {
                    throw new PdfAConformanceException(PdfAConformanceException.THE_F_KEYS_PRINT_FLAG_BIT_SHALL_BE_SET_TO_1_AND_ITS_HIDDEN_INVISIBLE_NOVIEW_AND_TOGGLENOVIEW_FLAG_BITS_SHALL_BE_SET_TO_0
                        );
                }
                if (subtype.Equals(PdfName.Text)) {
                    if (!CheckFlag(flags, PdfAnnotation.NO_ZOOM) || !CheckFlag(flags, PdfAnnotation.NO_ROTATE)) {
                        throw new PdfAConformanceException(PdfAConformanceLogMessageConstant.TEXT_ANNOTATIONS_SHOULD_SET_THE_NOZOOM_AND_NOROTATE_FLAG_BITS_OF_THE_F_KEY_TO_1
                            );
                    }
                }
            }
            CheckAnnotationAgainstActions(annotDic);
            if (CheckStructure(conformanceLevel)) {
                if (contentAnnotations.Contains(subtype) && !annotDic.ContainsKey(PdfName.Contents)) {
                    logger.LogWarning(MessageFormatUtil.Format(PdfAConformanceLogMessageConstant.ANNOTATION_OF_TYPE_0_SHOULD_HAVE_CONTENTS_KEY
                        , subtype.GetValue()));
                }
            }
            PdfDictionary ap = annotDic.GetAsDictionary(PdfName.AP);
            if (ap != null) {
                if (ap.ContainsKey(PdfName.R) || ap.ContainsKey(PdfName.D)) {
                    throw new PdfAConformanceException(PdfAConformanceException.APPEARANCE_DICTIONARY_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_STREAM_VALUE
                        );
                }
                PdfObject n = ap.Get(PdfName.N);
                if (PdfName.Widget.Equals(subtype) && PdfName.Btn.Equals(PdfFormField.GetFormType(annotDic))) {
                    if (n == null || !n.IsDictionary()) {
                        throw new PdfAConformanceException(PdfAConformanceException.APPEARANCE_DICTIONARY_OF_WIDGET_SUBTYPE_AND_BTN_FIELD_TYPE_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_DICTIONARY_VALUE
                            );
                    }
                }
                else {
                    if (n == null || !n.IsStream()) {
                        throw new PdfAConformanceException(PdfAConformanceException.APPEARANCE_DICTIONARY_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_STREAM_VALUE
                            );
                    }
                }
                CheckResourcesOfAppearanceStreams(ap);
            }
            else {
                bool isCorrectRect = false;
                PdfArray rect = annotDic.GetAsArray(PdfName.Rect);
                if (rect != null && rect.Size() == 4) {
                    PdfNumber index0 = rect.GetAsNumber(0);
                    PdfNumber index1 = rect.GetAsNumber(1);
                    PdfNumber index2 = rect.GetAsNumber(2);
                    PdfNumber index3 = rect.GetAsNumber(3);
                    if (index0 != null && index1 != null && index2 != null && index3 != null && index0.FloatValue() == index2.
                        FloatValue() && index1.FloatValue() == index3.FloatValue()) {
                        isCorrectRect = true;
                    }
                }
                if (!GetAppearanceLessAnnotations().Contains(subtype) && !isCorrectRect) {
                    throw new PdfAConformanceException(PdfAConformanceException.EVERY_ANNOTATION_SHALL_HAVE_AT_LEAST_ONE_APPEARANCE_DICTIONARY
                        );
                }
            }
        }

        /// <summary>Gets annotation types which are allowed not to have appearance stream.</summary>
        /// <returns>set of annotation names.</returns>
        protected internal virtual ICollection<PdfName> GetAppearanceLessAnnotations() {
            return apLessAnnotations;
        }

        /// <summary>
        /// Checked annotation against actions, exception will be thrown if either
        /// <c>A</c>
        /// or
        /// <c>AA</c>
        /// actions aren't allowed for specific type of annotation.
        /// </summary>
        /// <param name="annotDic">an annotation PDF dictionary</param>
        protected internal virtual void CheckAnnotationAgainstActions(PdfDictionary annotDic) {
            if (PdfName.Widget.Equals(annotDic.GetAsName(PdfName.Subtype)) && (annotDic.ContainsKey(PdfName.AA) || annotDic
                .ContainsKey(PdfName.A))) {
                throw new PdfAConformanceException(PdfAConformanceException.WIDGET_ANNOTATION_DICTIONARY_OR_FIELD_DICTIONARY_SHALL_NOT_INCLUDE_A_OR_AA_ENTRY
                    );
            }
            if (annotDic.ContainsKey(PdfName.AA)) {
                throw new PdfAConformanceException(PdfAConformanceException.AN_ANNOTATION_DICTIONARY_SHALL_NOT_CONTAIN_AA_KEY
                    );
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override ICollection<PdfName> GetForbiddenAnnotations() {
            return forbiddenAnnotations;
        }

        protected internal override void CheckAppearanceStream(PdfStream appearanceStream) {
            if (IsAlreadyChecked(appearanceStream)) {
                return;
            }
            if (IsContainsTransparencyGroup(appearanceStream)) {
                this.transparencyObjects.Add(appearanceStream);
            }
            CheckResources(appearanceStream.GetAsDictionary(PdfName.Resources));
        }

        protected internal override void CheckForm(PdfDictionary form) {
            if (form != null) {
                PdfBoolean needAppearances = form.GetAsBoolean(PdfName.NeedAppearances);
                if (needAppearances != null && needAppearances.GetValue()) {
                    throw new PdfAConformanceException(PdfAConformanceException.NEEDAPPEARANCES_FLAG_OF_THE_INTERACTIVE_FORM_DICTIONARY_SHALL_EITHER_NOT_BE_PRESENTED_OR_SHALL_BE_FALSE
                        );
                }
                if (form.ContainsKey(PdfName.XFA)) {
                    throw new PdfAConformanceException(PdfAConformanceException.THE_INTERACTIVE_FORM_DICTIONARY_SHALL_NOT_CONTAIN_THE_XFA_KEY
                        );
                }
                CheckResources(form.GetAsDictionary(PdfName.DR));
                PdfArray fields = form.GetAsArray(PdfName.Fields);
                if (fields != null) {
                    fields = GetFormFields(fields);
                    foreach (PdfObject field in fields) {
                        PdfDictionary fieldDic = (PdfDictionary)field;
                        CheckResources(fieldDic.GetAsDictionary(PdfName.DR));
                    }
                }
            }
        }

        /// <summary>Checks if the catalog is compliant with the PDF/A-2 standard.</summary>
        /// <param name="dict">the catalog dictionary</param>
        protected internal virtual void CheckCatalogAAConformance(PdfDictionary dict) {
            if (dict.ContainsKey(PdfName.AA)) {
                throw new PdfAConformanceException(PdfAConformanceException.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_AA_ENTRY
                    );
            }
        }

        protected internal override void CheckCatalogValidEntries(PdfDictionary catalogDict) {
            if (catalogDict.ContainsKey(PdfName.NeedsRendering)) {
                throw new PdfAConformanceException(PdfAConformanceException.THE_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_THE_NEEDSRENDERING_KEY
                    );
            }
            CheckCatalogAAConformance(catalogDict);
            if (catalogDict.ContainsKey(PdfName.Requirements)) {
                throw new PdfAConformanceException(PdfAConformanceException.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_REQUIREMENTS_ENTRY
                    );
            }
            PdfDictionary permissions = catalogDict.GetAsDictionary(PdfName.Perms);
            if (permissions != null) {
                foreach (PdfName dictKey in permissions.KeySet()) {
                    if (PdfName.DocMDP.Equals(dictKey)) {
                        PdfDictionary signatureDict = permissions.GetAsDictionary(PdfName.DocMDP);
                        if (signatureDict != null) {
                            CheckSignature(signatureDict);
                        }
                    }
                    else {
                        if (PdfName.UR3.Equals(dictKey)) {
                        }
                        else {
                            throw new PdfAConformanceException(PdfAConformanceException.NO_KEYS_OTHER_THAN_UR3_AND_DOC_MDP_SHALL_BE_PRESENT_IN_A_PERMISSIONS_DICTIONARY
                                );
                        }
                    }
                }
            }
            PdfDictionary namesDictionary = catalogDict.GetAsDictionary(PdfName.Names);
            if (namesDictionary != null && namesDictionary.ContainsKey(PdfName.AlternatePresentations)) {
                throw new PdfAConformanceException(PdfAConformanceException.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_ALTERNATEPRESENTATIONS_NAMES_ENTRY
                    );
            }
            PdfDictionary oCProperties = catalogDict.GetAsDictionary(PdfName.OCProperties);
            if (oCProperties != null) {
                IList<PdfDictionary> configList = new List<PdfDictionary>();
                PdfDictionary d = oCProperties.GetAsDictionary(PdfName.D);
                if (d != null) {
                    configList.Add(d);
                }
                PdfArray configs = oCProperties.GetAsArray(PdfName.Configs);
                if (configs != null) {
                    foreach (PdfObject config in configs) {
                        configList.Add((PdfDictionary)config);
                    }
                }
                HashSet<PdfObject> ocgs = new HashSet<PdfObject>();
                PdfArray ocgsArray = oCProperties.GetAsArray(PdfName.OCGs);
                if (ocgsArray != null) {
                    foreach (PdfObject ocg in ocgsArray) {
                        ocgs.Add(ocg);
                    }
                }
                HashSet<String> names = new HashSet<String>();
                foreach (PdfDictionary config in configList) {
                    CheckCatalogConfig(config, ocgs, names);
                }
            }
        }

        protected internal override void CheckPageSize(PdfDictionary page) {
            PdfName[] boxNames = new PdfName[] { PdfName.MediaBox, PdfName.CropBox, PdfName.TrimBox, PdfName.ArtBox, PdfName
                .BleedBox };
            foreach (PdfName boxName in boxNames) {
                Rectangle box = page.GetAsRectangle(boxName);
                if (box != null) {
                    float width = box.GetWidth();
                    float height = box.GetHeight();
                    if (width < MIN_PAGE_SIZE || width > MAX_PAGE_SIZE || height < MIN_PAGE_SIZE || height > MAX_PAGE_SIZE) {
                        throw new PdfAConformanceException(PdfAConformanceException.THE_PAGE_LESS_3_UNITS_NO_GREATER_14400_IN_EITHER_DIRECTION
                            );
                    }
                }
            }
        }

        protected internal override void CheckFileSpec(PdfDictionary fileSpec) {
            if (fileSpec.ContainsKey(PdfName.EF)) {
                if (!fileSpec.ContainsKey(PdfName.F) || !fileSpec.ContainsKey(PdfName.UF)) {
                    throw new PdfAConformanceException(PdfAConformanceException.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                        );
                }
                if (!fileSpec.ContainsKey(PdfName.Desc)) {
                    logger.LogWarning(PdfAConformanceLogMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHOULD_CONTAIN_DESC_KEY);
                }
                PdfDictionary ef = fileSpec.GetAsDictionary(PdfName.EF);
                PdfStream embeddedFile = ef.GetAsStream(PdfName.F);
                if (embeddedFile == null) {
                    throw new PdfAConformanceException(PdfAConformanceException.EF_KEY_OF_FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_DICTIONARY_WITH_VALID_F_KEY
                        );
                }
                // iText doesn't check whether provided file is compliant to PDF-A specs.
                logger.LogWarning(PdfAConformanceLogMessageConstant.EMBEDDED_FILE_SHALL_BE_COMPLIANT_WITH_SPEC);
            }
        }

        protected internal override void CheckPdfStream(PdfStream stream) {
            CheckPdfDictionary(stream);
            if (stream.ContainsKey(PdfName.F) || stream.ContainsKey(PdfName.FFilter) || stream.ContainsKey(PdfName.FDecodeParams
                )) {
                throw new PdfAConformanceException(PdfAConformanceException.STREAM_OBJECT_DICTIONARY_SHALL_NOT_CONTAIN_THE_F_FFILTER_OR_FDECODEPARAMS_KEYS
                    );
            }
            PdfObject filter = stream.Get(PdfName.Filter);
            if (filter is PdfName) {
                if (filter.Equals(PdfName.LZWDecode)) {
                    throw new PdfAConformanceException(PdfAConformanceException.LZWDECODE_FILTER_IS_NOT_PERMITTED);
                }
                if (filter.Equals(PdfName.Crypt)) {
                    PdfDictionary decodeParams = stream.GetAsDictionary(PdfName.DecodeParms);
                    if (decodeParams != null) {
                        PdfName cryptFilterName = decodeParams.GetAsName(PdfName.Name);
                        if (cryptFilterName != null && !cryptFilterName.Equals(PdfName.Identity)) {
                            throw new PdfAConformanceException(PdfAConformanceException.NOT_IDENTITY_CRYPT_FILTER_IS_NOT_PERMITTED);
                        }
                    }
                }
                if (!allowedFilters.Contains((PdfName)filter)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.INVALID_STREAM_FILTER_USAGE);
                }
            }
            else {
                if (filter is PdfArray) {
                    for (int i = 0; i < ((PdfArray)filter).Size(); i++) {
                        PdfName f = ((PdfArray)filter).GetAsName(i);
                        if (f.Equals(PdfName.LZWDecode)) {
                            throw new PdfAConformanceException(PdfAConformanceException.LZWDECODE_FILTER_IS_NOT_PERMITTED);
                        }
                        if (f.Equals(PdfName.Crypt)) {
                            PdfArray decodeParams = stream.GetAsArray(PdfName.DecodeParms);
                            if (decodeParams != null && i < decodeParams.Size()) {
                                PdfDictionary decodeParam = decodeParams.GetAsDictionary(i);
                                PdfName cryptFilterName = decodeParam.GetAsName(PdfName.Name);
                                if (cryptFilterName != null && !cryptFilterName.Equals(PdfName.Identity)) {
                                    throw new PdfAConformanceException(PdfAConformanceException.NOT_IDENTITY_CRYPT_FILTER_IS_NOT_PERMITTED);
                                }
                            }
                        }
                        if (!allowedFilters.Contains((PdfName)f)) {
                            throw new PdfAConformanceException(PdfaExceptionMessageConstant.INVALID_STREAM_FILTER_USAGE);
                        }
                    }
                }
            }
        }

        /// <summary>Checks if the page is compliant with the PDF/A-2 standard.</summary>
        /// <param name="dict">the page dictionary</param>
        protected internal virtual void CheckPageAAConformance(PdfDictionary dict) {
            if (dict.ContainsKey(PdfName.AA)) {
                throw new PdfAConformanceException(PdfAConformanceException.THE_PAGE_DICTIONARY_SHALL_NOT_CONTAIN_AA_ENTRY
                    );
            }
        }

        protected internal override void CheckPageObject(PdfDictionary pageDict, PdfDictionary pageResources) {
            CheckPageAAConformance(pageDict);
            if (pageDict.ContainsKey(PdfName.PresSteps)) {
                throw new PdfAConformanceException(PdfAConformanceException.THE_PAGE_DICTIONARY_SHALL_NOT_CONTAIN_PRESSTEPS_ENTRY
                    );
            }
            if (IsContainsTransparencyGroup(pageDict)) {
                PdfObject cs = pageDict.GetAsDictionary(PdfName.Group).Get(PdfName.CS);
                if (cs != null) {
                    PdfDictionary currentColorSpaces = pageResources.GetAsDictionary(PdfName.ColorSpace);
                    CheckColorSpace(PdfColorSpace.MakeColorSpace(cs), currentColorSpaces, true, null);
                }
            }
        }

        protected internal override void CheckPageTransparency(PdfDictionary pageDict, PdfDictionary pageResources
            ) {
            if (pdfAOutputIntentColorSpace == null && transparencyObjects.Count > 0 && (pageDict.GetAsDictionary(PdfName
                .Group) == null || pageDict.GetAsDictionary(PdfName.Group).Get(PdfName.CS) == null)) {
                CheckContentsForTransparency(pageDict);
                CheckAnnotationsForTransparency(pageDict.GetAsArray(PdfName.Annots));
                CheckResourcesForTransparency(pageResources, new HashSet<PdfObject>());
            }
        }

        protected internal override void CheckOutputIntents(PdfDictionary catalog) {
            PdfArray outputIntents = catalog.GetAsArray(PdfName.OutputIntents);
            if (outputIntents == null) {
                return;
            }
            int i;
            PdfObject destOutputProfile = null;
            for (i = 0; i < outputIntents.Size() && destOutputProfile == null; ++i) {
                destOutputProfile = outputIntents.GetAsDictionary(i).Get(PdfName.DestOutputProfile);
            }
            for (; i < outputIntents.Size(); ++i) {
                PdfObject otherDestOutputProfile = outputIntents.GetAsDictionary(i).Get(PdfName.DestOutputProfile);
                if (otherDestOutputProfile != null && destOutputProfile != otherDestOutputProfile) {
                    throw new PdfAConformanceException(PdfAConformanceException.IF_OUTPUTINTENTS_ARRAY_HAS_MORE_THAN_ONE_ENTRY_WITH_DESTOUTPUTPROFILE_KEY_THE_SAME_INDIRECT_OBJECT_SHALL_BE_USED_AS_THE_VALUE_OF_THAT_OBJECT
                        );
                }
            }
            if (destOutputProfile != null) {
                String deviceClass = IccProfile.GetIccDeviceClass(((PdfStream)destOutputProfile).GetBytes());
                if (!ICC_DEVICE_CLASS_OUTPUT_PROFILE.Equals(deviceClass) && !ICC_DEVICE_CLASS_MONITOR_PROFILE.Equals(deviceClass
                    )) {
                    throw new PdfAConformanceException(PdfAConformanceException.PROFILE_STREAM_OF_OUTPUTINTENT_SHALL_BE_OUTPUT_PROFILE_PRTR_OR_MONITOR_PROFILE_MNTR
                        );
                }
                String cs = IccProfile.GetIccColorSpaceName(((PdfStream)destOutputProfile).GetBytes());
                if (!ICC_COLOR_SPACE_RGB.Equals(cs) && !ICC_COLOR_SPACE_CMYK.Equals(cs) && !ICC_COLOR_SPACE_GRAY.Equals(cs
                    )) {
                    throw new PdfAConformanceException(PdfAConformanceException.OUTPUT_INTENT_COLOR_SPACE_SHALL_BE_EITHER_GRAY_RGB_OR_CMYK
                        );
                }
            }
        }

        protected internal override ICollection<PdfName> GetForbiddenActions() {
            return forbiddenActions;
        }

        protected internal override ICollection<PdfName> GetAllowedNamedActions() {
            return allowedNamedActions;
        }

        protected internal override void CheckColorsUsages() {
            if ((rgbIsUsed || cmykIsUsed || grayIsUsed) && pdfAOutputIntentColorSpace == null) {
                throw new PdfAConformanceException(PdfAConformanceException.IF_DEVICE_RGB_CMYK_GRAY_USED_IN_FILE_THAT_FILE_SHALL_CONTAIN_PDFA_OUTPUTINTENT_OR_DEFAULT_RGB_CMYK_GRAY_IN_USAGE_CONTEXT
                    );
            }
            if (rgbIsUsed) {
                if (!ICC_COLOR_SPACE_RGB.Equals(pdfAOutputIntentColorSpace)) {
                    throw new PdfAConformanceException(PdfAConformanceException.DEVICERGB_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_RGB_PDFA_OUTPUT_INTENT_OR_DEFAULTRGB_IN_USAGE_CONTEXT
                        );
                }
            }
            if (cmykIsUsed) {
                if (!ICC_COLOR_SPACE_CMYK.Equals(pdfAOutputIntentColorSpace)) {
                    throw new PdfAConformanceException(PdfAConformanceException.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                        );
                }
            }
        }

        protected internal override void CheckImage(PdfStream image, PdfDictionary currentColorSpaces) {
            PdfColorSpace colorSpace = null;
            if (IsAlreadyChecked(image)) {
                colorSpace = checkedObjectsColorspace.Get(image);
                CheckColorSpace(colorSpace, currentColorSpaces, true, null);
                return;
            }
            PdfObject colorSpaceObj = image.Get(PdfName.ColorSpace);
            if (colorSpaceObj != null) {
                colorSpace = PdfColorSpace.MakeColorSpace(colorSpaceObj);
                CheckColorSpace(colorSpace, currentColorSpaces, true, null);
                checkedObjectsColorspace.Put(image, colorSpace);
            }
            if (image.ContainsKey(PdfName.Alternates)) {
                throw new PdfAConformanceException(PdfAConformanceException.AN_IMAGE_DICTIONARY_SHALL_NOT_CONTAIN_ALTERNATES_KEY
                    );
            }
            if (image.ContainsKey(PdfName.OPI)) {
                throw new PdfAConformanceException(PdfAConformanceException.AN_IMAGE_DICTIONARY_SHALL_NOT_CONTAIN_OPI_KEY);
            }
            if (image.ContainsKey(PdfName.Interpolate) && (bool)image.GetAsBool(PdfName.Interpolate)) {
                throw new PdfAConformanceException(PdfAConformanceException.THE_VALUE_OF_INTERPOLATE_KEY_SHALL_BE_FALSE);
            }
            CheckRenderingIntent(image.GetAsName(PdfName.Intent));
            if (image.GetAsStream(PdfName.SMask) != null) {
                transparencyObjects.Add(image);
            }
            if (image.ContainsKey(PdfName.SMaskInData) && image.GetAsInt(PdfName.SMaskInData) > 0) {
                transparencyObjects.Add(image);
            }
            if (PdfName.JPXDecode.Equals(image.Get(PdfName.Filter))) {
                Jpeg2000ImageData jpgImage = (Jpeg2000ImageData)ImageDataFactory.CreateJpeg2000(image.GetBytes(false));
                Jpeg2000ImageData.Parameters @params = jpgImage.GetParameters();
                /* Concerning !params.isJpxBaseline check
                *
                * In pdf/a-2 ISO (ISO 19005-2:2011  6.2.8.3 JPEG2000) is stated that:
                * "Only the JPX baseline set of features, ... , shall be used."
                *
                * Also in jpeg2000 ISO (ISO/IEC 15444-2:2004   Annex M: M.9.2 Support for JPX feature set) is stated that:
                * "In general, a JPX reader is not required to support the entire set of features defined within this Recommendation |International Standard.
                * However, to promote interoperability, the following baseline set of features is defined. Files that
                * are written in such a way as to allow a reader that supports only this JPX baseline set of features to properly open the
                * file shall contain a CLi field in the File Type box with the value 'jpxb' (0x6a70 7862); all JPX baseline readers are
                * required to properly support all files with this code in the compatibility list in the File Type box."
                *
                * Therefore, I assumed that a file, which doesn't has the jpxb flag (which can be checked with the isJpxBaseline flag)
                * uses not only JPX baseline set of features.
                *
                * But, all the test files used in iText5 failed on this check, so may be my assumption is wrong.
                */
                if (!@params.isJp2) {
                    /*|| !params.isJpxBaseline*/
                    throw new PdfAConformanceException(PdfAConformanceException.ONLY_JPX_BASELINE_SET_OF_FEATURES_SHALL_BE_USED
                        );
                }
                if (@params.numOfComps != 1 && @params.numOfComps != 3 && @params.numOfComps != 4) {
                    throw new PdfAConformanceException(PdfAConformanceException.THE_NUMBER_OF_COLOUR_CHANNELS_IN_THE_JPEG2000_DATA_SHALL_BE_1_3_OR_4
                        );
                }
                if (@params.colorSpecBoxes != null && @params.colorSpecBoxes.Count > 1) {
                    int numOfApprox0x01 = 0;
                    foreach (Jpeg2000ImageData.ColorSpecBox colorSpecBox in @params.colorSpecBoxes) {
                        if (colorSpecBox.GetApprox() == 1) {
                            ++numOfApprox0x01;
                            if (numOfApprox0x01 == 1 && colorSpecBox.GetMeth() != 1 && colorSpecBox.GetMeth() != 2 && colorSpecBox.GetMeth
                                () != 3) {
                                throw new PdfAConformanceException(PdfAConformanceException.THE_VALUE_OF_THE_METH_ENTRY_IN_COLR_BOX_SHALL_BE_1_2_OR_3
                                    );
                            }
                            if (image.Get(PdfName.ColorSpace) == null) {
                                switch (colorSpecBox.GetEnumCs()) {
                                    case 1: {
                                        PdfDeviceCs.Gray deviceGrayCs = new PdfDeviceCs.Gray();
                                        CheckColorSpace(deviceGrayCs, currentColorSpaces, true, null);
                                        checkedObjectsColorspace.Put(image, deviceGrayCs);
                                        break;
                                    }

                                    case 3: {
                                        PdfDeviceCs.Rgb deviceRgbCs = new PdfDeviceCs.Rgb();
                                        CheckColorSpace(deviceRgbCs, currentColorSpaces, true, null);
                                        checkedObjectsColorspace.Put(image, deviceRgbCs);
                                        break;
                                    }

                                    case 12: {
                                        PdfDeviceCs.Cmyk deviceCmykCs = new PdfDeviceCs.Cmyk();
                                        CheckColorSpace(deviceCmykCs, currentColorSpaces, true, null);
                                        checkedObjectsColorspace.Put(image, deviceCmykCs);
                                        break;
                                    }
                                }
                            }
                        }
                        if (colorSpecBox.GetEnumCs() == 19) {
                            throw new PdfAConformanceException(PdfAConformanceException.JPEG2000_ENUMERATED_COLOUR_SPACE_19_CIEJAB_SHALL_NOT_BE_USED
                                );
                        }
                    }
                    if (numOfApprox0x01 != 1) {
                        throw new PdfAConformanceException(PdfAConformanceException.EXACTLY_ONE_COLOUR_SPACE_SPECIFICATION_SHALL_HAVE_THE_VALUE_0X01_IN_THE_APPROX_FIELD
                            );
                    }
                }
                if (jpgImage.GetBpc() < 1 || jpgImage.GetBpc() > 38) {
                    throw new PdfAConformanceException(PdfAConformanceException.THE_BIT_DEPTH_OF_THE_JPEG2000_DATA_SHALL_HAVE_A_VALUE_IN_THE_RANGE_1_TO_38
                        );
                }
                // The Bits Per Component box specifies the bit depth of each component.
                // If the bit depth of all components in the codestream is the same (in both sign and precision),
                // then this box shall not be found. Otherwise, this box specifies the bit depth of each individual component.
                if (@params.bpcBoxData != null) {
                    throw new PdfAConformanceException(PdfAConformanceException.ALL_COLOUR_CHANNELS_IN_THE_JPEG2000_DATA_SHALL_HAVE_THE_SAME_BIT_DEPTH
                        );
                }
            }
        }

        public override void CheckFontGlyphs(PdfFont font, PdfStream contentStream) {
            if (font is PdfType3Font) {
                CheckType3FontGlyphs((PdfType3Font)font, contentStream);
            }
        }

        protected internal override void CheckFormXObject(PdfStream form) {
            CheckFormXObject(form, null);
        }

        /// <summary>
        /// Verify the conformity of the Form XObject with appropriate
        /// specification.
        /// </summary>
        /// <remarks>
        /// Verify the conformity of the Form XObject with appropriate
        /// specification. Throws PdfAConformanceException if any discrepancy was found
        /// </remarks>
        /// <param name="form">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// to be checked
        /// </param>
        /// <param name="contentStream">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// current content stream
        /// </param>
        protected internal virtual void CheckFormXObject(PdfStream form, PdfStream contentStream) {
            if (IsAlreadyChecked(form)) {
                return;
            }
            if (form.ContainsKey(PdfName.OPI)) {
                throw new PdfAConformanceException(PdfAConformanceException.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_OPI_KEY
                    );
            }
            if (form.ContainsKey(PdfName.PS)) {
                throw new PdfAConformanceException(PdfAConformanceException.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_PS_KEY
                    );
            }
            if (PdfName.PS.Equals(form.GetAsName(PdfName.Subtype2))) {
                throw new PdfAConformanceException(PdfAConformanceException.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_SUBTYPE2_KEY_WITH_A_VALUE_OF_PS
                    );
            }
            if (IsContainsTransparencyGroup(form)) {
                if (contentStream != null) {
                    transparencyObjects.Add(contentStream);
                }
                else {
                    transparencyObjects.Add(form);
                }
                PdfObject cs = form.GetAsDictionary(PdfName.Group).Get(PdfName.CS);
                PdfDictionary resources = form.GetAsDictionary(PdfName.Resources);
                if (cs != null && resources != null) {
                    PdfDictionary currentColorSpaces = resources.GetAsDictionary(PdfName.ColorSpace);
                    CheckColorSpace(PdfColorSpace.MakeColorSpace(cs), currentColorSpaces, true, null);
                }
            }
            CheckResources(form.GetAsDictionary(PdfName.Resources));
            CheckContentStream(form);
        }

        /// <summary>Check optional content configuration dictionary against AS key.</summary>
        /// <param name="config">a content configuration dictionary</param>
        protected internal virtual void CheckContentConfigurationDictAgainstAsKey(PdfDictionary config) {
            if (config.ContainsKey(PdfName.AS)) {
                throw new PdfAConformanceException(PdfAConformanceException.THE_AS_KEY_SHALL_NOT_APPEAR_IN_ANY_OPTIONAL_CONTENT_CONFIGURATION_DICTIONARY
                    );
            }
        }

        /// <summary>Retrieve transparency error message valid for the pdf/a standard being used.</summary>
        /// <returns>error message.</returns>
        protected internal virtual String GetTransparencyErrorMessage() {
            return TRANSPARENCY_ERROR_MESSAGE;
        }

        /// <summary>Check if blendMode is compatible with pdf/a standard being used.</summary>
        /// <param name="blendMode">blend mode name to check.</param>
        protected internal virtual void CheckBlendMode(PdfName blendMode) {
            if (!allowedBlendModes.Contains(blendMode)) {
                throw new PdfAConformanceException(PdfAConformanceException.ONLY_STANDARD_BLEND_MODES_SHALL_BE_USED_FOR_THE_VALUE_OF_THE_BM_KEY_IN_AN_EXTENDED_GRAPHIC_STATE_DICTIONARY
                    );
            }
        }

        internal virtual void CheckContentsForTransparency(PdfDictionary pageDict) {
            PdfStream contentStream = pageDict.GetAsStream(PdfName.Contents);
            if (contentStream != null && transparencyObjects.Contains(contentStream)) {
                throw new PdfAConformanceException(GetTransparencyErrorMessage());
            }
            else {
                PdfArray contentSteamArray = pageDict.GetAsArray(PdfName.Contents);
                if (contentSteamArray != null) {
                    for (int i = 0; i < contentSteamArray.Size(); i++) {
                        if (transparencyObjects.Contains(contentSteamArray.Get(i))) {
                            throw new PdfAConformanceException(GetTransparencyErrorMessage());
                        }
                    }
                }
            }
        }

        internal virtual void CheckAnnotationsForTransparency(PdfArray annotations) {
            if (annotations == null) {
                return;
            }
            for (int i = 0; i < annotations.Size(); ++i) {
                PdfDictionary annot = annotations.GetAsDictionary(i);
                if (this.transparencyObjects.Contains(annot)) {
                    throw new PdfAConformanceException(GetTransparencyErrorMessage());
                }
                PdfDictionary ap = annot.GetAsDictionary(PdfName.AP);
                if (ap != null) {
                    CheckAppearanceStreamForTransparency(ap, new HashSet<PdfObject>());
                }
            }
        }

        private void CheckAppearanceStreamForTransparency(PdfDictionary ap, ICollection<PdfObject> checkedObjects) {
            if (checkedObjects.Contains(ap)) {
                return;
            }
            else {
                checkedObjects.Add(ap);
            }
            foreach (PdfObject val in ap.Values()) {
                if (this.transparencyObjects.Contains(val)) {
                    throw new PdfAConformanceException(GetTransparencyErrorMessage());
                }
                else {
                    if (val.IsDictionary()) {
                        CheckAppearanceStreamForTransparency((PdfDictionary)val, checkedObjects);
                    }
                    else {
                        if (val.IsStream()) {
                            CheckObjectWithResourcesForTransparency(val, checkedObjects);
                        }
                    }
                }
            }
        }

        private void CheckObjectWithResourcesForTransparency(PdfObject objectWithResources, ICollection<PdfObject>
             checkedObjects) {
            if (checkedObjects.Contains(objectWithResources)) {
                return;
            }
            else {
                checkedObjects.Add(objectWithResources);
            }
            if (this.transparencyObjects.Contains(objectWithResources)) {
                throw new PdfAConformanceException(GetTransparencyErrorMessage());
            }
            if (objectWithResources is PdfDictionary) {
                CheckResourcesForTransparency(((PdfDictionary)objectWithResources).GetAsDictionary(PdfName.Resources), checkedObjects
                    );
            }
        }

        internal virtual void CheckResourcesForTransparency(PdfDictionary resources, ICollection<PdfObject> checkedObjects
            ) {
            if (resources != null) {
                CheckSingleResourceTypeForTransparency(resources.GetAsDictionary(PdfName.XObject), checkedObjects);
                CheckSingleResourceTypeForTransparency(resources.GetAsDictionary(PdfName.Pattern), checkedObjects);
            }
        }

        private void CheckSingleResourceTypeForTransparency(PdfDictionary singleResourceDict, ICollection<PdfObject
            > checkedObjects) {
            if (singleResourceDict != null) {
                foreach (PdfObject resource in singleResourceDict.Values()) {
                    CheckObjectWithResourcesForTransparency(resource, checkedObjects);
                }
            }
        }

        private void CheckSeparationInsideDeviceN(PdfArray separation, PdfObject deviceNColorSpace, PdfObject deviceNTintTransform
            ) {
            if (!IsAltCSIsTheSame(separation.Get(2), deviceNColorSpace) || !deviceNTintTransform.Equals(separation.Get
                (3))) {
                logger.LogWarning(PdfAConformanceLogMessageConstant.TINT_TRANSFORM_AND_ALTERNATE_SPACE_OF_SEPARATION_ARRAYS_IN_THE_COLORANTS_OF_DEVICE_N_SHOULD_BE_CONSISTENT_WITH_SAME_ATTRIBUTES_OF_DEVICE_N
                    );
            }
            CheckSeparationCS(separation);
        }

        private void CheckSeparationCS(PdfArray separation) {
            if (separationColorSpaces.ContainsKey(separation.GetAsName(0))) {
                bool altCSIsTheSame;
                bool tintTransformIsTheSame;
                PdfArray sameNameSeparation = separationColorSpaces.Get(separation.GetAsName(0));
                PdfObject cs1 = separation.Get(2);
                PdfObject cs2 = sameNameSeparation.Get(2);
                altCSIsTheSame = IsAltCSIsTheSame(cs1, cs2);
                // TODO(DEVSIX-1672) in fact need to check if objects content is equal. ISO 19005-2, 6.2.4.4 "Separation and DeviceN colour spaces":
                // In evaluating equivalence, the PDF objects shall be compared, rather than the computational
                // result of the use of those PDF objects. Compression and whether or not an object is direct or indirect shall be ignored.
                PdfObject f1Obj = separation.Get(3);
                PdfObject f2Obj = sameNameSeparation.Get(3);
                //Can be a stream or dict
                bool bothAllowedType = (f1Obj.GetObjectType() == f2Obj.GetObjectType()) && (f1Obj.IsDictionary() || f1Obj.
                    IsStream());
                //Check if the indirect references are equal
                tintTransformIsTheSame = bothAllowedType && f1Obj.Equals(f2Obj);
                if (!altCSIsTheSame || !tintTransformIsTheSame) {
                    throw new PdfAConformanceException(PdfAConformanceException.TINT_TRANSFORM_AND_ALTERNATE_SPACE_SHALL_BE_THE_SAME_FOR_THE_ALL_SEPARATION_CS_WITH_THE_SAME_NAME
                        );
                }
            }
            else {
                separationColorSpaces.Put(separation.GetAsName(0), separation);
            }
        }

        private bool IsAltCSIsTheSame(PdfObject cs1, PdfObject cs2) {
            bool altCSIsTheSame = false;
            if (cs1 is PdfName) {
                altCSIsTheSame = cs1.Equals(cs2);
            }
            else {
                if (cs1 is PdfArray && cs2 is PdfArray) {
                    // TODO(DEVSIX-1672) in fact need to check if objects content is equal. ISO 19005-2, 6.2.4.4 "Separation and DeviceN colour spaces":
                    // In evaluating equivalence, the PDF objects shall be compared, rather than the computational
                    // result of the use of those PDF objects. Compression and whether or not an object is direct or indirect shall be ignored.
                    altCSIsTheSame = ((PdfArray)cs1).Get(0).Equals(((PdfArray)cs1).Get(0));
                }
            }
            return altCSIsTheSame;
        }

        private void CheckCatalogConfig(PdfDictionary config, HashSet<PdfObject> ocgs, HashSet<String> names) {
            PdfString name = config.GetAsString(PdfName.Name);
            if (name == null) {
                throw new PdfAConformanceException(PdfAConformanceException.OPTIONAL_CONTENT_CONFIGURATION_DICTIONARY_SHALL_CONTAIN_NAME_ENTRY
                    );
            }
            if (!names.Add(name.ToUnicodeString())) {
                throw new PdfAConformanceException(PdfAConformanceException.VALUE_OF_NAME_ENTRY_SHALL_BE_UNIQUE_AMONG_ALL_OPTIONAL_CONTENT_CONFIGURATION_DICTIONARIES
                    );
            }
            CheckContentConfigurationDictAgainstAsKey(config);
            PdfArray orderArray = config.GetAsArray(PdfName.Order);
            if (orderArray != null) {
                HashSet<PdfObject> order = new HashSet<PdfObject>();
                FillOrderRecursively(orderArray, order);
                if (!JavaUtil.SetEquals(order, ocgs)) {
                    throw new PdfAConformanceException(PdfAConformanceException.ORDER_ARRAY_SHALL_CONTAIN_REFERENCES_TO_ALL_OCGS
                        );
                }
            }
        }

        private void FillOrderRecursively(PdfArray orderArray, ICollection<PdfObject> order) {
            foreach (PdfObject orderItem in orderArray) {
                if (!orderItem.IsArray()) {
                    order.Add(orderItem);
                }
                else {
                    FillOrderRecursively((PdfArray)orderItem, order);
                }
            }
        }

        private bool CheckDefaultCS(PdfDictionary currentColorSpaces, bool? fill, PdfName defaultCsName, int numOfComponents
            ) {
            if (currentColorSpaces == null) {
                return false;
            }
            if (!currentColorSpaces.ContainsKey(defaultCsName)) {
                return false;
            }
            PdfObject defaultCsObj = currentColorSpaces.Get(defaultCsName);
            PdfColorSpace defaultCs = PdfColorSpace.MakeColorSpace(defaultCsObj);
            if (defaultCs is PdfDeviceCs) {
                throw new PdfAConformanceException(PdfAConformanceException.COLOR_SPACE_0_SHALL_BE_DEVICE_INDEPENDENT).SetMessageParams
                    (defaultCsName.ToString());
            }
            if (defaultCs.GetNumberOfComponents() != numOfComponents) {
                throw new PdfAConformanceException(PdfAConformanceException.COLOR_SPACE_0_SHALL_HAVE_1_COMPONENTS).SetMessageParams
                    (defaultCsName.GetValue(), numOfComponents);
            }
            CheckColorSpace(defaultCs, currentColorSpaces, false, fill);
            return true;
        }

        private void CheckType3FontGlyphs(PdfType3Font font, PdfStream contentStream) {
            for (int i = 0; i <= PdfFont.SIMPLE_FONT_MAX_CHAR_CODE_VALUE; ++i) {
                FontEncoding fontEncoding = font.GetFontEncoding();
                if (fontEncoding.CanDecode(i)) {
                    Type3Glyph type3Glyph = font.GetType3Glyph(fontEncoding.GetUnicode(i));
                    if (type3Glyph != null) {
                        CheckFormXObject(type3Glyph.GetContentStream(), contentStream);
                    }
                }
            }
        }

        private sealed class UpdateCanvasGraphicsState : CanvasGraphicsState {
            public UpdateCanvasGraphicsState(PdfDictionary extGStateDict) {
                UpdateFromExtGState(new PdfExtGState(extGStateDict));
            }
        }
    }
}
