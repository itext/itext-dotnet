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
using System.Collections.Generic;
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
using iText.Pdfa;

namespace iText.Pdfa.Checker {
    /// <summary>
    /// PdfA2Checker defines the requirements of the PDF/A-2 standard and contains a
    /// number of methods that override the implementations of its superclass
    /// <see cref="PdfA1Checker"/>
    /// .
    /// <p>
    /// The specification implemented by this class is ISO 19005-2
    /// </summary>
    public class PdfA2Checker : PdfA1Checker {
        protected internal static readonly ICollection<PdfName> forbiddenAnnotations = new HashSet<PdfName>(iText.IO.Util.JavaUtil.ArraysAsList
            (PdfName._3D, PdfName.Sound, PdfName.Screen, PdfName.Movie));

        protected internal static readonly ICollection<PdfName> forbiddenActions = new HashSet<PdfName>(iText.IO.Util.JavaUtil.ArraysAsList
            (PdfName.Launch, PdfName.Sound, PdfName.Movie, PdfName.ResetForm, PdfName.ImportData, PdfName.JavaScript
            , PdfName.Hide, PdfName.SetOCGState, PdfName.Rendition, PdfName.Trans, PdfName.GoTo3DView));

        protected internal static readonly ICollection<PdfName> allowedBlendModes = new HashSet<PdfName>(iText.IO.Util.JavaUtil.ArraysAsList
            (PdfName.Normal, PdfName.Compatible, PdfName.Multiply, PdfName.Screen, PdfName.Overlay, PdfName.Darken
            , PdfName.Lighten, PdfName.ColorDodge, PdfName.ColorBurn, PdfName.HardLight, PdfName.SoftLight, PdfName
            .Difference, PdfName.Exclusion, PdfName.Hue, PdfName.Saturation, PdfName.Color, PdfName.Luminosity));

        internal const int MAX_PAGE_SIZE = 14400;

        internal const int MIN_PAGE_SIZE = 3;

        private bool transparencyIsUsed = false;

        private bool currentFillCsIsIccBasedCMYK = false;

        private bool currentStrokeCsIsIccBasedCMYK = false;

        private IDictionary<PdfName, PdfArray> separationColorSpaces = new Dictionary<PdfName, PdfArray>();

        /// <summary>Creates a PdfA2Checker with the required conformance level</summary>
        /// <param name="conformanceLevel">
        /// the required conformance level, <code>a</code> or
        /// <code>u</code> or <code>b</code>
        /// </param>
        public PdfA2Checker(PdfAConformanceLevel conformanceLevel)
            : base(conformanceLevel) {
        }

        public override void CheckInlineImage(PdfStream inlineImage, PdfDictionary currentColorSpaces) {
            PdfObject filter = inlineImage.Get(PdfName.Filter);
            if (filter is PdfName) {
                if (filter.Equals(PdfName.LZWDecode)) {
                    throw new PdfAConformanceException(PdfAConformanceException.LZWDecodeFilterIsNotPermitted);
                }
                if (filter.Equals(PdfName.Crypt)) {
                    throw new PdfAConformanceException(PdfAConformanceException.CryptFilterIsNotPermitted);
                }
            }
            else {
                if (filter is PdfArray) {
                    for (int i = 0; i < ((PdfArray)filter).Size(); i++) {
                        PdfName f = ((PdfArray)filter).GetAsName(i);
                        if (f.Equals(PdfName.LZWDecode)) {
                            throw new PdfAConformanceException(PdfAConformanceException.LZWDecodeFilterIsNotPermitted);
                        }
                        if (f.Equals(PdfName.Crypt)) {
                            throw new PdfAConformanceException(PdfAConformanceException.CryptFilterIsNotPermitted);
                        }
                    }
                }
            }
            CheckImage(inlineImage, currentColorSpaces);
        }

        public override void CheckColor(Color color, PdfDictionary currentColorSpaces, bool? fill) {
            if (color is PatternColor) {
                PdfPattern pattern = ((PatternColor)color).GetPattern();
                if (pattern is PdfPattern.Shading) {
                    PdfDictionary shadingDictionary = ((PdfPattern.Shading)pattern).GetShading();
                    PdfObject colorSpace = shadingDictionary.Get(PdfName.ColorSpace);
                    CheckColorSpace(PdfColorSpace.MakeColorSpace(colorSpace), currentColorSpaces, true, true);
                    PdfDictionary extGStateDict = ((PdfDictionary)pattern.GetPdfObject()).GetAsDictionary(PdfName.ExtGState);
                    CanvasGraphicsState gState = new _CanvasGraphicsState_153(extGStateDict);
                    CheckExtGState(gState);
                }
            }
            CheckColorSpace(color.GetColorSpace(), currentColorSpaces, true, fill);
        }

        private sealed class _CanvasGraphicsState_153 : CanvasGraphicsState {
            public _CanvasGraphicsState_153(PdfDictionary extGStateDict) {
                this.extGStateDict = extGStateDict;
 {
                    this.UpdateFromExtGState(new PdfExtGState(extGStateDict));
                }
            }

            private readonly PdfDictionary extGStateDict;
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
                    PdfDictionary attributes = ((PdfArray)deviceN.GetPdfObject()).GetAsDictionary(4);
                    PdfDictionary colorants = attributes.GetAsDictionary(PdfName.Colorants);
                    if (colorants != null) {
                        foreach (KeyValuePair<PdfName, PdfObject> entry in colorants.EntrySet()) {
                            PdfArray separation = (PdfArray)entry.Value;
                            CheckSeparationInsideDeviceN(separation, ((PdfArray)deviceN.GetPdfObject()).Get(2), ((PdfArray)deviceN.GetPdfObject
                                ()).Get(3).GetIndirectReference());
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

        public override void CheckExtGState(CanvasGraphicsState extGState) {
            if (System.Convert.ToInt32(1).Equals(extGState.GetOverprintMode())) {
                if (extGState.GetFillOverprint() && currentFillCsIsIccBasedCMYK) {
                    throw new PdfAConformanceException(PdfAConformanceException.OverprintModeShallNotBeOneWhenAnICCBasedCMYKColourSpaceIsUsedAndWhenOverprintingIsSetToTrue
                        );
                }
                if (extGState.GetStrokeOverprint() && currentStrokeCsIsIccBasedCMYK) {
                    throw new PdfAConformanceException(PdfAConformanceException.OverprintModeShallNotBeOneWhenAnICCBasedCMYKColourSpaceIsUsedAndWhenOverprintingIsSetToTrue
                        );
                }
            }
            if (extGState.GetTransferFunction() != null) {
                throw new PdfAConformanceException(PdfAConformanceException.AnExtgstateDictionaryShallNotContainTheTrKey);
            }
            if (extGState.GetHTP() != null) {
                throw new PdfAConformanceException(PdfAConformanceException.AnExtgstateDictionaryShallNotContainTheHTPKey);
            }
            PdfObject transferFunction2 = extGState.GetTransferFunction2();
            if (transferFunction2 != null && !PdfName.Default.Equals(transferFunction2)) {
                throw new PdfAConformanceException(PdfAConformanceException.AnExtgstateDictionaryShallNotContainTheTR2KeyWithAValueOtherThanDefault
                    );
            }
            if (extGState.GetHalftone() is PdfDictionary) {
                PdfDictionary halftoneDict = (PdfDictionary)extGState.GetHalftone();
                int? halftoneType = halftoneDict.GetAsInt(PdfName.HalftoneType);
                if (halftoneType != 1 && halftoneType != 5) {
                    throw new PdfAConformanceException(PdfAConformanceException.AllHalftonesShallHaveHalftonetype1Or5);
                }
                if (halftoneDict.ContainsKey(PdfName.HalftoneName)) {
                    throw new PdfAConformanceException(PdfAConformanceException.HalftonesShallNotContainHalftonename);
                }
            }
            CheckRenderingIntent(extGState.GetRenderingIntent());
            if (extGState.GetSoftMask() != null && extGState.GetSoftMask() is PdfDictionary) {
                transparencyIsUsed = true;
            }
            if (extGState.GetStrokeOpacity() < 1) {
                transparencyIsUsed = true;
            }
            if (extGState.GetFillOpacity() < 1) {
                transparencyIsUsed = true;
            }
            PdfObject bm = extGState.GetBlendMode();
            if (bm != null) {
                if (!PdfName.Normal.Equals(bm)) {
                    transparencyIsUsed = true;
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

        protected internal override void CheckNonSymbolicTrueTypeFont(PdfTrueTypeFont trueTypeFont) {
            String encoding = trueTypeFont.GetFontEncoding().GetBaseEncoding();
            // non-symbolic true type font will always has an encoding entry in font dictionary in itext7
            if (!PdfEncodings.WINANSI.Equals(encoding) && !encoding.Equals(PdfEncodings.MACROMAN)) {
                throw new PdfAConformanceException(PdfAConformanceException.AllNonSymbolicTrueTypeFontShallSpecifyMacRomanEncodingOrWinAnsiEncoding
                    , trueTypeFont);
            }
        }

        // if font has differences array, itext7 ensures that all names in it are listed in AdobeGlyphList
        protected internal override double GetMaxRealValue() {
            return float.MaxValue;
        }

        protected internal override int GetMaxStringLength() {
            return 32767;
        }

        protected internal override void CheckAnnotation(PdfDictionary annotDic) {
            PdfName subtype = annotDic.GetAsName(PdfName.Subtype);
            if (subtype == null) {
                throw new PdfAConformanceException(PdfAConformanceException.AnnotationType1IsNotPermitted).SetMessageParams
                    ("null");
            }
            if (forbiddenAnnotations.Contains(subtype)) {
                throw new PdfAConformanceException(PdfAConformanceException.AnnotationType1IsNotPermitted).SetMessageParams
                    (subtype.GetValue());
            }
            if (!subtype.Equals(PdfName.Popup)) {
                PdfNumber f = annotDic.GetAsNumber(PdfName.F);
                if (f == null) {
                    throw new PdfAConformanceException(PdfAConformanceException.AnAnnotationDictionaryShallContainTheFKey);
                }
                int flags = f.IntValue();
                if (!CheckFlag(flags, PdfAnnotation.PRINT) || CheckFlag(flags, PdfAnnotation.HIDDEN) || CheckFlag(flags, PdfAnnotation
                    .INVISIBLE) || CheckFlag(flags, PdfAnnotation.NO_VIEW) || CheckFlag(flags, PdfAnnotation.TOGGLE_NO_VIEW
                    )) {
                    throw new PdfAConformanceException(PdfAConformanceException.TheFKeysPrintFlagBitShallBeSetTo1AndItsHiddenInvisibleNoviewAndTogglenoviewFlagBitsShallBeSetTo0
                        );
                }
                if (subtype.Equals(PdfName.Text)) {
                    if (!CheckFlag(flags, PdfAnnotation.NO_ZOOM) || !CheckFlag(flags, PdfAnnotation.NO_ROTATE)) {
                        throw new PdfAConformanceException(PdfAConformanceException.TextAnnotationsShouldSetTheNozoomAndNorotateFlagBitsOfTheFKeyTo1
                            );
                    }
                }
            }
            if (PdfName.Widget.Equals(subtype) && (annotDic.ContainsKey(PdfName.AA) || annotDic.ContainsKey(PdfName.A)
                )) {
                throw new PdfAConformanceException(PdfAConformanceException.WidgetAnnotationDictionaryOrFieldDictionaryShallNotIncludeAOrAAEntry
                    );
            }
            if (annotDic.ContainsKey(PdfName.AA)) {
                throw new PdfAConformanceException(PdfAConformanceException.AnAnnotationDictionaryShallNotContainAAKey);
            }
            if (CheckStructure(conformanceLevel)) {
                if (contentAnnotations.Contains(subtype) && !annotDic.ContainsKey(PdfName.Contents)) {
                    throw new PdfAConformanceException(PdfAConformanceException.AnnotationOfType1ShouldHaveContentsKey).SetMessageParams
                        (subtype);
                }
            }
            PdfDictionary ap = annotDic.GetAsDictionary(PdfName.AP);
            if (ap != null) {
                if (ap.ContainsKey(PdfName.R) || ap.ContainsKey(PdfName.D)) {
                    throw new PdfAConformanceException(PdfAConformanceException.AppearanceDictionaryShallContainOnlyTheNKeyWithStreamValue
                        );
                }
                PdfObject n = ap.Get(PdfName.N);
                if (PdfName.Widget.Equals(subtype) && PdfName.Btn.Equals(annotDic.GetAsName(PdfName.FT))) {
                    if (n == null || !n.IsDictionary()) {
                        throw new PdfAConformanceException(PdfAConformanceException.AppearanceDictionaryOfWidgetSubtypeAndBtnFieldTypeShallContainOnlyTheNKeyWithDictionaryValue
                            );
                    }
                }
                else {
                    if (n == null || !n.IsStream()) {
                        throw new PdfAConformanceException(PdfAConformanceException.AppearanceDictionaryShallContainOnlyTheNKeyWithStreamValue
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
                if (!PdfName.Popup.Equals(subtype) && !PdfName.Link.Equals(subtype) && !isCorrectRect) {
                    throw new PdfAConformanceException(PdfAConformanceException.EveryAnnotationShallHaveAtLeastOneAppearanceDictionary
                        );
                }
            }
        }

        protected internal override void CheckForm(PdfDictionary form) {
            if (form != null) {
                PdfBoolean needAppearances = form.GetAsBoolean(PdfName.NeedAppearances);
                if (needAppearances != null && needAppearances.GetValue()) {
                    throw new PdfAConformanceException(PdfAConformanceException.NeedAppearancesFlagOfTheInteractiveFormDictionaryShallEitherNotBePresentedOrShallBeFalse
                        );
                }
                if (form.ContainsKey(PdfName.XFA)) {
                    throw new PdfAConformanceException(PdfAConformanceException.TheInteractiveFormDictionaryShallNotContainTheXfaKey
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

        protected internal override void CheckCatalogValidEntries(PdfDictionary catalogDict) {
            if (catalogDict.ContainsKey(PdfName.NeedsRendering)) {
                throw new PdfAConformanceException(PdfAConformanceException.TheCatalogDictionaryShallNotContainTheNeedsrenderingKey
                    );
            }
            if (catalogDict.ContainsKey(PdfName.AA)) {
                throw new PdfAConformanceException(PdfAConformanceException.CatalogDictionaryShallNotContainAAEntry);
            }
            if (catalogDict.ContainsKey(PdfName.Requirements)) {
                throw new PdfAConformanceException(PdfAConformanceException.CatalogDictionaryShallNotContainRequirementsEntry
                    );
            }
            PdfDictionary permissions = catalogDict.GetAsDictionary(PdfName.Perms);
            if (permissions != null) {
                foreach (PdfName dictKey in permissions.KeySet()) {
                    if (PdfName.DocMDP.Equals(dictKey)) {
                        PdfDictionary signatureDict = permissions.GetAsDictionary(PdfName.DocMDP);
                        if (signatureDict != null) {
                            PdfArray references = signatureDict.GetAsArray(PdfName.Reference);
                            if (references != null) {
                                for (int i = 0; i < references.Size(); i++) {
                                    PdfDictionary referenceDict = references.GetAsDictionary(i);
                                    if (referenceDict.ContainsKey(PdfName.DigestLocation) || referenceDict.ContainsKey(PdfName.DigestMethod) ||
                                         referenceDict.ContainsKey(PdfName.DigestValue)) {
                                        throw new PdfAConformanceException(PdfAConformanceException.SigRefDicShallNotContDigestParam);
                                    }
                                }
                            }
                        }
                    }
                    else {
                        if (PdfName.UR3.Equals(dictKey)) {
                        }
                        else {
                            throw new PdfAConformanceException(PdfAConformanceException.NoKeysOtherUr3andDocMdpShallBePresentInPerDict
                                );
                        }
                    }
                }
            }
            PdfDictionary namesDictionary = catalogDict.GetAsDictionary(PdfName.Names);
            if (namesDictionary != null && namesDictionary.ContainsKey(PdfName.AlternatePresentations)) {
                throw new PdfAConformanceException(PdfAConformanceException.CatalogDictionaryShallNotContainAlternatepresentationsNamesEntry
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
                ICollection<PdfObject> ocgs = new HashSet<PdfObject>();
                PdfArray ocgsArray = oCProperties.GetAsArray(PdfName.OCGs);
                if (ocgsArray != null) {
                    foreach (PdfObject ocg in ocgsArray) {
                        ocgs.Add(ocg);
                    }
                }
                HashSet<String> names = new HashSet<String>();
                HashSet<PdfObject> order = new HashSet<PdfObject>();
                foreach (PdfDictionary config in configList) {
                    PdfString name = config.GetAsString(PdfName.Name);
                    if (name == null) {
                        throw new PdfAConformanceException(PdfAConformanceException.OptionalContentConfigurationDictionaryShallContainNameEntry
                            );
                    }
                    if (!names.Add(name.ToUnicodeString())) {
                        throw new PdfAConformanceException(PdfAConformanceException.ValueOfNameEntryShallBeUniqueAmongAllOptionalContentConfigurationDictionaries
                            );
                    }
                    if (config.ContainsKey(PdfName.AS)) {
                        throw new PdfAConformanceException(PdfAConformanceException.TheAsKeyShallNotAppearInAnyOptionalContentConfigurationDictionary
                            );
                    }
                    PdfArray orderArray = config.GetAsArray(PdfName.Order);
                    if (orderArray != null) {
                        FillOrderRecursively(orderArray, order);
                    }
                }
                if (order.Count != ocgs.Count) {
                    throw new PdfAConformanceException(PdfAConformanceException.OrderArrayShallContainReferencesToAllOcgs);
                }
                order.RetainAll(ocgs);
                if (order.Count != ocgs.Count) {
                    throw new PdfAConformanceException(PdfAConformanceException.OrderArrayShallContainReferencesToAllOcgs);
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
                        throw new PdfAConformanceException(PdfAConformanceException.PageLess3UnitsNoGreater14400InEitherDirection);
                    }
                }
            }
        }

        protected internal override void CheckFileSpec(PdfDictionary fileSpec) {
            if (fileSpec.ContainsKey(PdfName.EF)) {
                if (!fileSpec.ContainsKey(PdfName.F) || !fileSpec.ContainsKey(PdfName.UF) || !fileSpec.ContainsKey(PdfName
                    .Desc)) {
                    throw new PdfAConformanceException(PdfAConformanceException.FileSpecificationDictionaryShallContainFKeyUFKeyAndDescKey
                        );
                }
                PdfDictionary ef = fileSpec.GetAsDictionary(PdfName.EF);
                PdfStream embeddedFile = ef.GetAsStream(PdfName.F);
                if (embeddedFile == null) {
                    throw new PdfAConformanceException(PdfAConformanceException.EFKeyOfFileSpecificationDictionaryShallContainDictionaryWithValidFKey
                        );
                }
                PdfName subtype = embeddedFile.GetAsName(PdfName.Subtype);
                if (!PdfName.ApplicationPdf.Equals(subtype)) {
                    throw new PdfAConformanceException(PdfAConformanceException.EmbeddedFileShallBeOfPdfMimeType);
                }
            }
        }

        protected internal override void CheckPdfStream(PdfStream stream) {
            if (stream.ContainsKey(PdfName.F) || stream.ContainsKey(PdfName.FFilter) || stream.ContainsKey(PdfName.FDecodeParams
                )) {
                throw new PdfAConformanceException(PdfAConformanceException.StreamObjDictShallNotContainForFFilterOrFDecodeParams
                    );
            }
            PdfObject filter = stream.Get(PdfName.Filter);
            if (filter is PdfName) {
                if (filter.Equals(PdfName.LZWDecode)) {
                    throw new PdfAConformanceException(PdfAConformanceException.LZWDecodeFilterIsNotPermitted);
                }
                if (filter.Equals(PdfName.Crypt)) {
                    PdfDictionary decodeParams = stream.GetAsDictionary(PdfName.DecodeParms);
                    if (decodeParams != null) {
                        PdfString cryptFilterName = decodeParams.GetAsString(PdfName.Name);
                        if (cryptFilterName != null && !cryptFilterName.Equals(PdfName.Identity)) {
                            throw new PdfAConformanceException(PdfAConformanceException.NotIdentityCryptFilterIsNotPermitted);
                        }
                    }
                }
            }
            else {
                if (filter is PdfArray) {
                    for (int i = 0; i < ((PdfArray)filter).Size(); i++) {
                        PdfName f = ((PdfArray)filter).GetAsName(i);
                        if (f.Equals(PdfName.LZWDecode)) {
                            throw new PdfAConformanceException(PdfAConformanceException.LZWDecodeFilterIsNotPermitted);
                        }
                        if (f.Equals(PdfName.Crypt)) {
                            PdfArray decodeParams = stream.GetAsArray(PdfName.DecodeParms);
                            if (decodeParams != null && i < decodeParams.Size()) {
                                PdfDictionary decodeParam = decodeParams.GetAsDictionary(i);
                                PdfString cryptFilterName = decodeParam.GetAsString(PdfName.Name);
                                if (cryptFilterName != null && !cryptFilterName.Equals(PdfName.Identity)) {
                                    throw new PdfAConformanceException(PdfAConformanceException.NotIdentityCryptFilterIsNotPermitted);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected internal override void CheckPageObject(PdfDictionary pageDict, PdfDictionary pageResources) {
            if (pageDict.ContainsKey(PdfName.AA)) {
                throw new PdfAConformanceException(PdfAConformanceException.PageDictionaryShallNotContainAAEntry);
            }
            if (pageDict.ContainsKey(PdfName.PresSteps)) {
                throw new PdfAConformanceException(PdfAConformanceException.PageDictionaryShallNotContainPressstepsEntry);
            }
            if (pageDict.ContainsKey(PdfName.Group) && PdfName.Transparency.Equals(pageDict.GetAsDictionary(PdfName.Group
                ).GetAsName(PdfName.S))) {
                transparencyIsUsed = true;
                PdfObject cs = pageDict.GetAsDictionary(PdfName.Group).Get(PdfName.CS);
                if (cs != null) {
                    PdfDictionary currentColorSpaces = pageResources.GetAsDictionary(PdfName.ColorSpace);
                    CheckColorSpace(PdfColorSpace.MakeColorSpace(cs), currentColorSpaces, true, null);
                }
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
                    throw new PdfAConformanceException(PdfAConformanceException.IfOutputintentsArrayHasMoreThanOneEntryWithDestoutputprofileKeyTheSameIndirectObjectShallBeUsedAsTheValueOfThatObject
                        );
                }
            }
            if (destOutputProfile != null) {
                String deviceClass = IccProfile.GetIccDeviceClass(((PdfStream)destOutputProfile).GetBytes());
                if (!ICC_DEVICE_CLASS_OUTPUT_PROFILE.Equals(deviceClass) && !ICC_DEVICE_CLASS_MONITOR_PROFILE.Equals(deviceClass
                    )) {
                    throw new PdfAConformanceException(PdfAConformanceException.ProfileStreamOfOutputintentShallBeOutputProfilePrtrOrMonitorProfileMntr
                        );
                }
                String cs = IccProfile.GetIccColorSpaceName(((PdfStream)destOutputProfile).GetBytes());
                if (!ICC_COLOR_SPACE_RGB.Equals(cs) && !ICC_COLOR_SPACE_CMYK.Equals(cs) && !ICC_COLOR_SPACE_GRAY.Equals(cs
                    )) {
                    throw new PdfAConformanceException(PdfAConformanceException.OutputIntentColorSpaceShallBeEitherGrayRgbOrCmyk
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
            if (transparencyIsUsed && pdfAOutputIntentColorSpace == null) {
                throw new PdfAConformanceException(PdfAConformanceException.IfTheDocumentDoesNotContainAPdfAOutputIntentTransparencyIsForbidden
                    );
            }
            if ((rgbIsUsed || cmykIsUsed || grayIsUsed) && pdfAOutputIntentColorSpace == null) {
                throw new PdfAConformanceException(PdfAConformanceException.IfDeviceRgbCmykGrayUsedInFileThatFileShallContainPdfaOutputIntentOrDefaultRgbCmykGrayInUsageContext
                    );
            }
            if (rgbIsUsed) {
                if (!ICC_COLOR_SPACE_RGB.Equals(pdfAOutputIntentColorSpace)) {
                    throw new PdfAConformanceException(PdfAConformanceException.DevicergbMayBeUsedOnlyIfTheFileHasARgbPdfAOutputIntentOrDefaultRgbInUsageContext
                        );
                }
            }
            if (cmykIsUsed) {
                if (!ICC_COLOR_SPACE_CMYK.Equals(pdfAOutputIntentColorSpace)) {
                    throw new PdfAConformanceException(PdfAConformanceException.DevicecmykMayBeUsedOnlyIfTheFileHasACmykPdfAOutputIntentOrDefaultCmykInUsageContext
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
                checkedObjectsColorspace[image] = colorSpace;
            }
            if (image.ContainsKey(PdfName.Alternates)) {
                throw new PdfAConformanceException(PdfAConformanceException.AnImageDictionaryShallNotContainAlternatesKey);
            }
            if (image.ContainsKey(PdfName.OPI)) {
                throw new PdfAConformanceException(PdfAConformanceException.AnImageDictionaryShallNotContainOpiKey);
            }
            if (image.ContainsKey(PdfName.Interpolate) && (bool)image.GetAsBool(PdfName.Interpolate)) {
                throw new PdfAConformanceException(PdfAConformanceException.TheValueOfInterpolateKeyShallNotBeTrue);
            }
            CheckRenderingIntent(image.GetAsName(PdfName.Intent));
            if (image.GetAsStream(PdfName.SMask) != null) {
                transparencyIsUsed = true;
            }
            if (image.ContainsKey(PdfName.SMaskInData) && image.GetAsInt(PdfName.SMaskInData) > 0) {
                transparencyIsUsed = true;
            }
            if (PdfName.JPXDecode.Equals(image.Get(PdfName.Filter))) {
                Jpeg2000ImageData jpgImage = (Jpeg2000ImageData)ImageDataFactory.CreateJpeg2000(image.GetBytes());
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
                    throw new PdfAConformanceException(PdfAConformanceException.OnlyJpxBaselineSetOfFeaturesShallBeUsed);
                }
                if (@params.numOfComps != 1 && @params.numOfComps != 3 && @params.numOfComps != 4) {
                    throw new PdfAConformanceException(PdfAConformanceException.TheNumberOfColourChannelsInTheJpeg2000DataShallBe123
                        );
                }
                if (@params.colorSpecBoxes != null && @params.colorSpecBoxes.Count > 1) {
                    int numOfApprox0x01 = 0;
                    foreach (Jpeg2000ImageData.ColorSpecBox colorSpecBox in @params.colorSpecBoxes) {
                        if (colorSpecBox.GetApprox() == 1) {
                            ++numOfApprox0x01;
                            if (numOfApprox0x01 == 1 && colorSpecBox.GetMeth() != 1 && colorSpecBox.GetMeth() != 2 && colorSpecBox.GetMeth
                                () != 3) {
                                throw new PdfAConformanceException(PdfAConformanceException.TheValueOfTheMethEntryInColrBoxShallBe123);
                            }
                            if (image.Get(PdfName.ColorSpace) == null) {
                                switch (colorSpecBox.GetEnumCs()) {
                                    case 1: {
                                        PdfDeviceCs.Gray deviceGrayCs = new PdfDeviceCs.Gray();
                                        CheckColorSpace(deviceGrayCs, currentColorSpaces, true, null);
                                        checkedObjectsColorspace[image] = deviceGrayCs;
                                        break;
                                    }

                                    case 3: {
                                        PdfDeviceCs.Rgb deviceRgbCs = new PdfDeviceCs.Rgb();
                                        CheckColorSpace(deviceRgbCs, currentColorSpaces, true, null);
                                        checkedObjectsColorspace[image] = deviceRgbCs;
                                        break;
                                    }

                                    case 12: {
                                        PdfDeviceCs.Cmyk deviceCmykCs = new PdfDeviceCs.Cmyk();
                                        CheckColorSpace(deviceCmykCs, currentColorSpaces, true, null);
                                        checkedObjectsColorspace[image] = deviceCmykCs;
                                        break;
                                    }
                                }
                            }
                        }
                        if (colorSpecBox.GetEnumCs() == 19) {
                            throw new PdfAConformanceException(PdfAConformanceException.Jpeg2000EnumeratedColourSpace19CIEJabShallNotBeUsed
                                );
                        }
                    }
                    if (numOfApprox0x01 != 1) {
                        throw new PdfAConformanceException(PdfAConformanceException.ExactlyOneColourSpaceSpecificationShallHaveTheValue0x01InTheApproxField
                            );
                    }
                }
                if (jpgImage.GetBpc() < 1 || jpgImage.GetBpc() > 38) {
                    throw new PdfAConformanceException(PdfAConformanceException.TheBitDepthOfTheJpeg2000DataShallHaveAValueInTheRange1To38
                        );
                }
                // The Bits Per Component box specifies the bit depth of each component.
                // If the bit depth of all components in the codestream is the same (in both sign and precision),
                // then this box shall not be found. Otherwise, this box specifies the bit depth of each individual component.
                if (@params.bpcBoxData != null) {
                    throw new PdfAConformanceException(PdfAConformanceException.AllColourChannelsInTheJpeg2000DataShallHaveTheSameBitDepth
                        );
                }
            }
        }

        protected internal override void CheckFormXObject(PdfStream form) {
            if (IsAlreadyChecked(form)) {
                return;
            }
            if (form.ContainsKey(PdfName.OPI)) {
                throw new PdfAConformanceException(PdfAConformanceException.AFormXobjectDictionaryShallNotContainOpiKey);
            }
            if (form.ContainsKey(PdfName.PS)) {
                throw new PdfAConformanceException(PdfAConformanceException.AFormXobjectDictionaryShallNotContainPSKey);
            }
            if (PdfName.PS.Equals(form.GetAsName(PdfName.Subtype2))) {
                throw new PdfAConformanceException(PdfAConformanceException.AFormXobjectDictionaryShallNotContainSubtype2KeyWithAValueOfPS
                    );
            }
            if (form.ContainsKey(PdfName.Group) && PdfName.Transparency.Equals(form.GetAsDictionary(PdfName.Group).GetAsName
                (PdfName.S))) {
                transparencyIsUsed = true;
                PdfObject cs = form.GetAsDictionary(PdfName.Group).Get(PdfName.CS);
                PdfDictionary resources = form.GetAsDictionary(PdfName.Resources);
                if (cs != null && resources != null) {
                    PdfDictionary currentColorSpaces = resources.GetAsDictionary(PdfName.ColorSpace);
                    CheckColorSpace(PdfColorSpace.MakeColorSpace(cs), currentColorSpaces, true, null);
                }
            }
            CheckResources(form.GetAsDictionary(PdfName.Resources));
        }

        private void CheckBlendMode(PdfName blendMode) {
            if (!allowedBlendModes.Contains(blendMode)) {
                throw new PdfAConformanceException(PdfAConformanceException.OnlyStandardBlendModesShallBeusedForTheValueOfTheBMKeyOnAnExtendedGraphicStateDictionary
                    );
            }
        }

        private void CheckSeparationInsideDeviceN(PdfArray separation, PdfObject deviceNColorSpace, PdfIndirectReference
             deviceNTintTransform) {
            if (!IsAltCSIsTheSame(separation.Get(2), deviceNColorSpace) || !deviceNTintTransform.Equals(separation.GetAsDictionary
                (3).GetIndirectReference())) {
                throw new PdfAConformanceException(PdfAConformanceException.TintTransformAndAlternateSpaceOfSeparationArraysInTheColorantsOfDeviceNShallBeConsistentWithSameAttributesOfDeviceN
                    );
            }
            CheckSeparationCS(separation);
        }

        private void CheckSeparationCS(PdfArray separation) {
            if (separationColorSpaces.ContainsKey(separation.GetAsName(0))) {
                bool altCSIsTheSame = false;
                bool tintTransformIsTheSame = false;
                PdfArray sameNameSeparation = separationColorSpaces.Get(separation.GetAsName(0));
                PdfObject cs1 = separation.Get(2);
                PdfObject cs2 = sameNameSeparation.Get(2);
                altCSIsTheSame = IsAltCSIsTheSame(cs1, cs2);
                PdfDictionary f1 = separation.GetAsDictionary(3);
                PdfDictionary f2 = sameNameSeparation.GetAsDictionary(3);
                //todo compare dictionaries or stream references
                tintTransformIsTheSame = f1.GetIndirectReference().Equals(f2.GetIndirectReference());
                if (!altCSIsTheSame || !tintTransformIsTheSame) {
                    throw new PdfAConformanceException(PdfAConformanceException.TintTransformAndAlternateSpaceShallBeTheSameForTheAllSeparationCSWithTheSameName
                        );
                }
            }
            else {
                separationColorSpaces[separation.GetAsName(0)] = separation;
            }
        }

        private bool IsAltCSIsTheSame(PdfObject cs1, PdfObject cs2) {
            bool altCSIsTheSame = false;
            if (cs1 is PdfName) {
                altCSIsTheSame = cs1.Equals(cs2);
            }
            else {
                if (cs1 is PdfArray && cs2 is PdfArray) {
                    //todo compare cs dictionaries or stream reference
                    altCSIsTheSame = ((PdfArray)cs1).Get(0).Equals(((PdfArray)cs1).Get(0));
                }
            }
            return altCSIsTheSame;
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
                throw new PdfAConformanceException(PdfAConformanceException.ColorSpace1ShallBeDeviceIndependent).SetMessageParams
                    (defaultCsName.ToString());
            }
            if (defaultCs.GetNumberOfComponents() != numOfComponents) {
                throw new PdfAConformanceException(PdfAConformanceException.ColorSpace1ShallHave2Components).SetMessageParams
                    (defaultCsName.ToString(), numOfComponents);
            }
            CheckColorSpace(defaultCs, currentColorSpaces, false, fill);
            return true;
        }
    }
}
