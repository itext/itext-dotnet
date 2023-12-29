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
using iText.IO.Font;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser.Util;
using iText.Kernel.Pdf.Colorspace;
using iText.Pdfa.Exceptions;
using iText.Pdfa.Logs;

namespace iText.Pdfa.Checker {
    /// <summary>
    /// PdfA1Checker defines the requirements of the PDF/A-1 standard and contains
    /// method implementations from the abstract
    /// <see cref="PdfAChecker"/>
    /// class.
    /// </summary>
    /// <remarks>
    /// PdfA1Checker defines the requirements of the PDF/A-1 standard and contains
    /// method implementations from the abstract
    /// <see cref="PdfAChecker"/>
    /// class.
    /// <para />
    /// The specification implemented by this class is ISO 19005-1
    /// </remarks>
    public class PdfA1Checker : PdfAChecker {
        protected internal static readonly ICollection<PdfName> forbiddenAnnotations = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Sound, PdfName.Movie, PdfName.FileAttachment)));

        protected internal static readonly ICollection<PdfName> contentAnnotations = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Text, PdfName.FreeText, PdfName.Line, PdfName.Square
            , PdfName.Circle, PdfName.Stamp, PdfName.Ink, PdfName.Popup)));

        protected internal static readonly ICollection<PdfName> forbiddenActions = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Launch, PdfName.Sound, PdfName.Movie, PdfName.ResetForm
            , PdfName.ImportData, PdfName.JavaScript, PdfName.Hide)));

        protected internal static readonly ICollection<PdfName> allowedNamedActions = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.NextPage, PdfName.PrevPage, PdfName.FirstPage, PdfName
            .LastPage)));

        protected internal static readonly ICollection<PdfName> allowedRenderingIntents = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.RelativeColorimetric, PdfName.AbsoluteColorimetric
            , PdfName.Perceptual, PdfName.Saturation)));

        private const int MAX_NUMBER_OF_DEVICEN_COLOR_COMPONENTS = 8;

        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(PdfAChecker));

        /// <summary>Creates a PdfA1Checker with the required conformance level</summary>
        /// <param name="conformanceLevel">
        /// the required conformance level, <c>a</c> or
        /// <c>b</c>
        /// </param>
        public PdfA1Checker(PdfAConformanceLevel conformanceLevel)
            : base(conformanceLevel) {
        }

        public override void CheckCanvasStack(char stackOperation) {
            if ('q' == stackOperation) {
                if (++gsStackDepth > iText.Pdfa.Checker.PdfA1Checker.maxGsStackDepth) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.GRAPHICS_STATE_STACK_DEPTH_IS_GREATER_THAN_28
                        );
                }
            }
            else {
                if ('Q' == stackOperation) {
                    gsStackDepth--;
                }
            }
        }

        public override void CheckInlineImage(PdfStream inlineImage, PdfDictionary currentColorSpaces) {
            PdfObject filter = inlineImage.Get(PdfName.Filter);
            if (filter is PdfName) {
                if (filter.Equals(PdfName.LZWDecode)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED);
                }
            }
            else {
                if (filter is PdfArray) {
                    for (int i = 0; i < ((PdfArray)filter).Size(); i++) {
                        PdfName f = ((PdfArray)filter).GetAsName(i);
                        if (f.Equals(PdfName.LZWDecode)) {
                            throw new PdfAConformanceException(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED);
                        }
                    }
                }
            }
            CheckImage(inlineImage, currentColorSpaces);
        }

        /// <summary><inheritDoc/></summary>
        [Obsolete]
        public override void CheckColor(Color color, PdfDictionary currentColorSpaces, bool? fill, PdfStream stream
            ) {
            CheckColor(null, color, currentColorSpaces, fill, stream);
        }

        /// <summary><inheritDoc/></summary>
        public override void CheckColor(CanvasGraphicsState graphicsState, Color color, PdfDictionary currentColorSpaces
            , bool? fill, PdfStream stream) {
            CheckColorSpace(color.GetColorSpace(), stream, currentColorSpaces, true, fill);
            if (color is PatternColor) {
                PdfPattern pattern = ((PatternColor)color).GetPattern();
                if (pattern is PdfPattern.Tiling) {
                    CheckContentStream((PdfStream)pattern.GetPdfObject());
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void CheckColorSpace(PdfColorSpace colorSpace, PdfObject pdfObject, PdfDictionary currentColorSpaces
            , bool checkAlternate, bool? fill) {
            if (colorSpace is PdfSpecialCs.Separation) {
                colorSpace = ((PdfSpecialCs.Separation)colorSpace).GetBaseCs();
            }
            else {
                if (colorSpace is PdfSpecialCs.DeviceN) {
                    PdfSpecialCs.DeviceN deviceNColorspace = (PdfSpecialCs.DeviceN)colorSpace;
                    if (deviceNColorspace.GetNumberOfComponents() > MAX_NUMBER_OF_DEVICEN_COLOR_COMPONENTS) {
                        throw new PdfAConformanceException(PdfaExceptionMessageConstant.THE_NUMBER_OF_COLOR_COMPONENTS_IN_DEVICE_N_COLORSPACE_SHOULD_NOT_EXCEED
                            , MAX_NUMBER_OF_DEVICEN_COLOR_COMPONENTS);
                    }
                    colorSpace = deviceNColorspace.GetBaseCs();
                }
            }
            if (colorSpace is PdfDeviceCs.Rgb) {
                if (cmykIsUsed || !cmykUsedObjects.IsEmpty()) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.DEVICERGB_AND_DEVICECMYK_COLORSPACES_CANNOT_BE_USED_BOTH_IN_ONE_FILE
                        );
                }
                rgbUsedObjects.Add(pdfObject);
            }
            else {
                if (colorSpace is PdfDeviceCs.Cmyk) {
                    if (rgbIsUsed || !rgbUsedObjects.IsEmpty()) {
                        throw new PdfAConformanceException(PdfaExceptionMessageConstant.DEVICERGB_AND_DEVICECMYK_COLORSPACES_CANNOT_BE_USED_BOTH_IN_ONE_FILE
                            );
                    }
                    cmykUsedObjects.Add(pdfObject);
                }
                else {
                    if (colorSpace is PdfDeviceCs.Gray) {
                        grayUsedObjects.Add(pdfObject);
                    }
                }
            }
        }

        public override void CheckXrefTable(PdfXrefTable xrefTable) {
            if (xrefTable.GetCountOfIndirectObjects() > GetMaxNumberOfIndirectObjects()) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.MAXIMUM_NUMBER_OF_INDIRECT_OBJECTS_EXCEEDED
                    );
            }
        }

        protected internal override ICollection<PdfName> GetForbiddenActions() {
            return forbiddenActions;
        }

        protected internal override ICollection<PdfName> GetAllowedNamedActions() {
            return allowedNamedActions;
        }

        protected internal override long GetMaxNumberOfIndirectObjects() {
            return 8_388_607;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckColorsUsages() {
        }

        // Do not check anything here. All checks are in checkPageColorsUsages.
        /// <summary><inheritDoc/></summary>
        protected internal override void CheckPageColorsUsages(PdfDictionary pageDict, PdfDictionary pageResources
            ) {
            if ((rgbIsUsed || cmykIsUsed || grayIsUsed || !rgbUsedObjects.IsEmpty() || !cmykUsedObjects.IsEmpty() || grayUsedObjects
                .IsEmpty()) && pdfAOutputIntentColorSpace == null) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.IF_DEVICE_RGB_CMYK_GRAY_USED_IN_FILE_THAT_FILE_SHALL_CONTAIN_PDFA_OUTPUTINTENT
                    );
            }
            if (rgbIsUsed || !rgbUsedObjects.IsEmpty()) {
                if (!ICC_COLOR_SPACE_RGB.Equals(pdfAOutputIntentColorSpace)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.DEVICERGB_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_RGB_PDFA_OUTPUT_INTENT
                        );
                }
            }
            if (cmykIsUsed || !cmykUsedObjects.IsEmpty()) {
                if (!ICC_COLOR_SPACE_CMYK.Equals(pdfAOutputIntentColorSpace)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT
                        );
                }
            }
        }

        public override void CheckExtGState(CanvasGraphicsState extGState, PdfStream contentStream) {
            if (extGState.GetTransferFunction() != null) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.AN_EXTGSTATE_DICTIONARY_SHALL_NOT_CONTAIN_THE_TR_KEY
                    );
            }
            PdfObject transferFunction2 = extGState.GetTransferFunction2();
            if (transferFunction2 != null && !PdfName.Default.Equals(transferFunction2)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.AN_EXTGSTATE_DICTIONARY_SHALL_NOT_CONTAIN_THE_TR_2_KEY_WITH_A_VALUE_OTHER_THAN_DEFAULT
                    );
            }
            CheckRenderingIntent(extGState.GetRenderingIntent());
            PdfObject softMask = extGState.GetSoftMask();
            if (softMask != null && !PdfName.None.Equals(softMask)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.THE_SMASK_KEY_IS_NOT_ALLOWED_IN_EXTGSTATE);
            }
            PdfObject bm = extGState.GetBlendMode();
            if (bm != null && !PdfName.Normal.Equals(bm) && !PdfName.Compatible.Equals(bm)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.BLEND_MODE_SHALL_HAVE_VALUE_NORMAL_OR_COMPATIBLE
                    );
            }
            float? ca = extGState.GetStrokeOpacity();
            if (ca != null && ca != 1) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.TRANSPARENCY_IS_NOT_ALLOWED_CA_SHALL_BE_EQUAL_TO_1
                    );
            }
            ca = extGState.GetFillOpacity();
            if (ca != null && ca != 1) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.TRANSPARENCY_IS_NOT_ALLOWED_AND_CA_SHALL_BE_EQUAL_TO_1
                    );
            }
        }

        public override void CheckFontGlyphs(PdfFont font, PdfStream contentStream) {
        }

        // This check is irrelevant for the PdfA1 checker, so the body of the method is empty
        public override void CheckRenderingIntent(PdfName intent) {
            if (intent == null) {
                return;
            }
            if (!allowedRenderingIntents.Contains(intent)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.IF_SPECIFIED_RENDERING_SHALL_BE_ONE_OF_THE_FOLLOWING_RELATIVECOLORIMETRIC_ABSOLUTECOLORIMETRIC_PERCEPTUAL_OR_SATURATION
                    );
            }
        }

        public override void CheckFont(PdfFont pdfFont) {
            if (!pdfFont.IsEmbedded()) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.ALL_THE_FONTS_MUST_BE_EMBEDDED_THIS_ONE_IS_NOT_0
                    ).SetMessageParams(pdfFont.GetFontProgram().GetFontNames().GetFontName());
            }
            if (pdfFont is PdfTrueTypeFont) {
                PdfTrueTypeFont trueTypeFont = (PdfTrueTypeFont)pdfFont;
                bool symbolic = trueTypeFont.GetFontEncoding().IsFontSpecific();
                if (symbolic) {
                    CheckSymbolicTrueTypeFont(trueTypeFont);
                }
                else {
                    CheckNonSymbolicTrueTypeFont(trueTypeFont);
                }
            }
            if (pdfFont is PdfType3Font) {
                PdfDictionary charProcs = pdfFont.GetPdfObject().GetAsDictionary(PdfName.CharProcs);
                foreach (PdfName charName in charProcs.KeySet()) {
                    CheckContentStream(charProcs.GetAsStream(charName));
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="crypto">
        /// 
        /// <inheritDoc/>
        /// </param>
        public override void CheckCrypto(PdfObject crypto) {
            if (crypto != null) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.KEYWORD_ENCRYPT_SHALL_NOT_BE_USED_IN_THE_TRAILER_DICTIONARY
                    );
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void CheckSignatureType(bool isCAdES) {
        }

        //nothing to do
        /// <summary><inheritDoc/></summary>
        /// <param name="text">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <param name="font">
        /// 
        /// <inheritDoc/>
        /// </param>
        public override void CheckText(String text, PdfFont font) {
            for (int i = 0; i < text.Length; ++i) {
                int ch;
                if (iText.IO.Util.TextUtil.IsSurrogatePair(text, i)) {
                    ch = iText.IO.Util.TextUtil.ConvertToUtf32(text, i);
                    i++;
                }
                else {
                    ch = text[i];
                }
                if (!font.ContainsGlyph(ch)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.EMBEDDED_FONTS_SHALL_DEFINE_ALL_REFERENCED_GLYPHS
                        );
                }
            }
        }

        protected internal override void CheckPageTransparency(PdfDictionary pageDict, PdfDictionary pageResources
            ) {
        }

        // This check is irrelevant for the PdfA1 checker, so the body of the method is empty
        protected internal override void CheckContentStream(PdfStream contentStream) {
            if (IsFullCheckMode() || contentStream.IsModified()) {
                byte[] contentBytes = contentStream.GetBytes();
                PdfTokenizer tokenizer = new PdfTokenizer(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource
                    (contentBytes)));
                PdfCanvasParser parser = new PdfCanvasParser(tokenizer);
                IList<PdfObject> operands = new List<PdfObject>();
                try {
                    while (parser.Parse(operands).Count > 0) {
                        foreach (PdfObject operand in operands) {
                            CheckContentStreamObject(operand);
                        }
                    }
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(PdfaExceptionMessageConstant.CANNOT_PARSE_CONTENT_STREAM, e);
                }
            }
        }

        protected internal override void CheckNonSymbolicTrueTypeFont(PdfTrueTypeFont trueTypeFont) {
            String encoding = trueTypeFont.GetFontEncoding().GetBaseEncoding();
            // non-symbolic true type font will always has an encoding entry in font dictionary in itext
            if (!PdfEncodings.WINANSI.Equals(encoding) && !PdfEncodings.MACROMAN.Equals(encoding) || trueTypeFont.GetFontEncoding
                ().HasDifferences()) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.ALL_NON_SYMBOLIC_TRUE_TYPE_FONT_SHALL_SPECIFY_MAC_ROMAN_OR_WIN_ANSI_ENCODING_AS_THE_ENCODING_ENTRY
                    , trueTypeFont);
            }
        }

        protected internal override void CheckSymbolicTrueTypeFont(PdfTrueTypeFont trueTypeFont) {
            if (trueTypeFont.GetFontEncoding().HasDifferences()) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.ALL_SYMBOLIC_TRUE_TYPE_FONTS_SHALL_NOT_SPECIFY_ENCODING
                    );
            }
        }

        // if symbolic font encoding doesn't have differences, itext won't write encoding for such font
        protected internal override void CheckImage(PdfStream image, PdfDictionary currentColorSpaces) {
            PdfColorSpace colorSpace = null;
            if (IsAlreadyChecked(image)) {
                colorSpace = checkedObjectsColorspace.Get(image);
                CheckColorSpace(colorSpace, image, currentColorSpaces, true, null);
                return;
            }
            PdfObject colorSpaceObj = image.Get(PdfName.ColorSpace);
            if (colorSpaceObj != null) {
                colorSpace = PdfColorSpace.MakeColorSpace(colorSpaceObj);
                CheckColorSpace(colorSpace, image, currentColorSpaces, true, null);
                checkedObjectsColorspace.Put(image, colorSpace);
            }
            if (image.ContainsKey(PdfName.Alternates)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.AN_IMAGE_DICTIONARY_SHALL_NOT_CONTAIN_ALTERNATES_KEY
                    );
            }
            if (image.ContainsKey(PdfName.OPI)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.AN_IMAGE_DICTIONARY_SHALL_NOT_CONTAIN_OPI_KEY
                    );
            }
            if (image.ContainsKey(PdfName.Interpolate) && (bool)image.GetAsBool(PdfName.Interpolate)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.THE_VALUE_OF_INTERPOLATE_KEY_SHALL_BE_FALSE
                    );
            }
            CheckRenderingIntent(image.GetAsName(PdfName.Intent));
            if (image.ContainsKey(PdfName.SMask) && !PdfName.None.Equals(image.GetAsName(PdfName.SMask))) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.THE_SMASK_KEY_IS_NOT_ALLOWED_IN_XOBJECTS);
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
            if (form.ContainsKey(PdfName.PS)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_PS_KEY
                    );
            }
            if (PdfName.PS.Equals(form.GetAsName(PdfName.Subtype2))) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_SUBTYPE2_KEY_WITH_A_VALUE_OF_PS
                    );
            }
            if (form.ContainsKey(PdfName.SMask) && !PdfName.None.Equals(form.GetAsName(PdfName.SMask))) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.THE_SMASK_KEY_IS_NOT_ALLOWED_IN_XOBJECTS);
            }
            if (IsContainsTransparencyGroup(form)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.A_GROUP_OBJECT_WITH_AN_S_KEY_WITH_A_VALUE_OF_TRANSPARENCY_SHALL_NOT_BE_INCLUDED_IN_A_FORM_XOBJECT
                    );
            }
            CheckResources(form.GetAsDictionary(PdfName.Resources), form);
            CheckContentStream(form);
        }

        protected internal override void CheckLogicalStructure(PdfDictionary catalog) {
            if (CheckStructure(conformanceLevel)) {
                PdfDictionary markInfo = catalog.GetAsDictionary(PdfName.MarkInfo);
                if (markInfo == null || markInfo.GetAsBoolean(PdfName.Marked) == null || !markInfo.GetAsBoolean(PdfName.Marked
                    ).GetValue()) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.A_CATALOG_SHALL_INCLUDE_MARK_INFO_DICTIONARY_WITH_MARKED_TRUE_VALUE
                        );
                }
                if (!catalog.ContainsKey(PdfName.Lang)) {
                    logger.LogWarning(PdfAConformanceLogMessageConstant.CATALOG_SHOULD_CONTAIN_LANG_ENTRY);
                }
            }
        }

        protected internal override void CheckMetaData(PdfDictionary catalog) {
            if (!catalog.ContainsKey(PdfName.Metadata)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.A_CATALOG_DICTIONARY_SHALL_CONTAIN_METADATA_ENTRY
                    );
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
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.IF_OUTPUTINTENTS_ARRAY_HAS_MORE_THAN_ONE_ENTRY_WITH_DESTOUTPUTPROFILE_KEY_THE_SAME_INDIRECT_OBJECT_SHALL_BE_USED_AS_THE_VALUE_OF_THAT_OBJECT
                        );
                }
            }
        }

        protected internal override void CheckPdfNumber(PdfNumber number) {
            if (number.HasDecimalPoint()) {
                if (Math.Abs(number.LongValue()) > GetMaxRealValue()) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.REAL_NUMBER_IS_OUT_OF_RANGE);
                }
            }
            else {
                if (number.LongValue() > GetMaxIntegerValue() || number.LongValue() < GetMinIntegerValue()) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.INTEGER_NUMBER_IS_OUT_OF_RANGE);
                }
            }
        }

        /// <summary>Retrieve maximum allowed real value.</summary>
        /// <returns>maximum allowed real number</returns>
        protected internal virtual double GetMaxRealValue() {
            return 32767;
        }

        /// <summary>Retrieve maximal allowed integer value.</summary>
        /// <returns>maximal allowed integer number</returns>
        protected internal virtual long GetMaxIntegerValue() {
            return int.MaxValue;
        }

        /// <summary>Retrieve minimal allowed integer value.</summary>
        /// <returns>minimal allowed integer number</returns>
        protected internal virtual long GetMinIntegerValue() {
            return int.MinValue;
        }

        protected internal override void CheckPdfArray(PdfArray array) {
            if (array.Size() > GetMaxArrayCapacity()) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.MAXIMUM_ARRAY_CAPACITY_IS_EXCEEDED);
            }
        }

        protected internal override void CheckPdfDictionary(PdfDictionary dictionary) {
            if (dictionary.Size() > GetMaxDictionaryCapacity()) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.MAXIMUM_DICTIONARY_CAPACITY_IS_EXCEEDED);
            }
        }

        protected internal override void CheckPdfStream(PdfStream stream) {
            CheckPdfDictionary(stream);
            if (stream.ContainsKey(PdfName.F) || stream.ContainsKey(PdfName.FFilter) || stream.ContainsKey(PdfName.FDecodeParams
                )) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.STREAM_OBJECT_DICTIONARY_SHALL_NOT_CONTAIN_THE_F_FFILTER_OR_FDECODEPARAMS_KEYS
                    );
            }
            PdfObject filter = stream.Get(PdfName.Filter);
            if (filter is PdfName) {
                if (filter.Equals(PdfName.LZWDecode)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED);
                }
            }
            else {
                if (filter is PdfArray) {
                    foreach (PdfObject f in ((PdfArray)filter)) {
                        if (f.Equals(PdfName.LZWDecode)) {
                            throw new PdfAConformanceException(PdfaExceptionMessageConstant.LZWDECODE_FILTER_IS_NOT_PERMITTED);
                        }
                    }
                }
            }
        }

        protected internal override void CheckPdfName(PdfName name) {
            if (name.GetValue().Length > GetMaxNameLength()) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.PDF_NAME_IS_TOO_LONG);
            }
        }

        /// <summary>Retrieve maximum allowed length of the name object.</summary>
        /// <returns>maximum allowed length of the name</returns>
        protected internal virtual int GetMaxNameLength() {
            return 127;
        }

        protected internal override void CheckPdfString(PdfString @string) {
            if (@string.GetValueBytes().Length > GetMaxStringLength()) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.PDF_STRING_IS_TOO_LONG);
            }
        }

        /// <summary>Returns maximum allowed bytes length of the string in a PDF document.</summary>
        /// <returns>maximum string length</returns>
        protected internal virtual int GetMaxStringLength() {
            return 65535;
        }

        protected internal override void CheckPageSize(PdfDictionary page) {
        }

        protected internal override void CheckFileSpec(PdfDictionary fileSpec) {
            if (fileSpec.ContainsKey(PdfName.EF)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.FILE_SPECIFICATION_DICTIONARY_SHALL_NOT_CONTAIN_THE_EF_KEY
                    );
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckAnnotation(PdfDictionary annotDic) {
            PdfName subtype = annotDic.GetAsName(PdfName.Subtype);
            if (subtype == null) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED).SetMessageParams
                    ("null");
            }
            if (GetForbiddenAnnotations().Contains(subtype)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED).SetMessageParams
                    (subtype.GetValue());
            }
            PdfNumber ca = annotDic.GetAsNumber(PdfName.CA);
            if (ca != null && ca.FloatValue() != 1.0) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.AN_ANNOTATION_DICTIONARY_SHALL_NOT_CONTAIN_THE_CA_KEY_WITH_A_VALUE_OTHER_THAN_1
                    );
            }
            if (!annotDic.ContainsKey(PdfName.F)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.AN_ANNOTATION_DICTIONARY_SHALL_CONTAIN_THE_F_KEY
                    );
            }
            int flags = (int)annotDic.GetAsInt(PdfName.F);
            if (!CheckFlag(flags, PdfAnnotation.PRINT) || CheckFlag(flags, PdfAnnotation.HIDDEN) || CheckFlag(flags, PdfAnnotation
                .INVISIBLE) || CheckFlag(flags, PdfAnnotation.NO_VIEW)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.THE_F_KEYS_PRINT_FLAG_BIT_SHALL_BE_SET_TO_1_AND_ITS_HIDDEN_INVISIBLE_AND_NOVIEW_FLAG_BITS_SHALL_BE_SET_TO_0
                    );
            }
            if (subtype.Equals(PdfName.Text) && (!CheckFlag(flags, PdfAnnotation.NO_ZOOM) || !CheckFlag(flags, PdfAnnotation
                .NO_ROTATE))) {
                throw new PdfAConformanceException(PdfAConformanceLogMessageConstant.TEXT_ANNOTATIONS_SHOULD_SET_THE_NOZOOM_AND_NOROTATE_FLAG_BITS_OF_THE_F_KEY_TO_1
                    );
            }
            if (annotDic.ContainsKey(PdfName.C) || annotDic.ContainsKey(PdfName.IC)) {
                if (!ICC_COLOR_SPACE_RGB.Equals(pdfAOutputIntentColorSpace)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.DESTOUTPUTPROFILE_IN_THE_PDFA1_OUTPUTINTENT_DICTIONARY_SHALL_BE_RGB
                        );
                }
            }
            PdfDictionary ap = annotDic.GetAsDictionary(PdfName.AP);
            if (ap != null) {
                if (ap.ContainsKey(PdfName.D) || ap.ContainsKey(PdfName.R)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.APPEARANCE_DICTIONARY_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_STREAM_VALUE
                        );
                }
                if (PdfName.Widget.Equals(annotDic.GetAsName(PdfName.Subtype)) && (PdfName.Btn.Equals(PdfFormField.GetFormType
                    (annotDic)))) {
                    if (ap.GetAsDictionary(PdfName.N) == null) {
                        throw new PdfAConformanceException(PdfaExceptionMessageConstant.N_KEY_SHALL_BE_APPEARANCE_SUBDICTIONARY);
                    }
                }
                else {
                    if (ap.GetAsStream(PdfName.N) == null) {
                        throw new PdfAConformanceException(PdfaExceptionMessageConstant.APPEARANCE_DICTIONARY_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_STREAM_VALUE
                            );
                    }
                }
                CheckResourcesOfAppearanceStreams(ap);
            }
            if (PdfName.Widget.Equals(subtype) && (annotDic.ContainsKey(PdfName.AA) || annotDic.ContainsKey(PdfName.A)
                )) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.WIDGET_ANNOTATION_DICTIONARY_OR_FIELD_DICTIONARY_SHALL_NOT_INCLUDE_A_OR_AA_ENTRY
                    );
            }
            if (annotDic.ContainsKey(PdfName.AA)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.AN_ANNOTATION_DICTIONARY_SHALL_NOT_CONTAIN_AA_KEY
                    );
            }
            if (CheckStructure(conformanceLevel)) {
                if (contentAnnotations.Contains(subtype) && !annotDic.ContainsKey(PdfName.Contents)) {
                    logger.LogWarning(MessageFormatUtil.Format(PdfAConformanceLogMessageConstant.ANNOTATION_OF_TYPE_0_SHOULD_HAVE_CONTENTS_KEY
                        , subtype.GetValue()));
                }
            }
        }

        /// <summary>Gets forbidden annotation types.</summary>
        /// <returns>a set of forbidden annotation types</returns>
        protected internal virtual ICollection<PdfName> GetForbiddenAnnotations() {
            return forbiddenAnnotations;
        }

        protected internal override void CheckForm(PdfDictionary form) {
            if (form == null) {
                return;
            }
            PdfBoolean needAppearances = form.GetAsBoolean(PdfName.NeedAppearances);
            if (needAppearances != null && needAppearances.GetValue()) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.NEEDAPPEARANCES_FLAG_OF_THE_INTERACTIVE_FORM_DICTIONARY_SHALL_EITHER_NOT_BE_PRESENTED_OR_SHALL_BE_FALSE
                    );
            }
            CheckResources(form.GetAsDictionary(PdfName.DR), form);
            PdfArray fields = form.GetAsArray(PdfName.Fields);
            if (fields != null) {
                fields = GetFormFields(fields);
                foreach (PdfObject field in fields) {
                    PdfDictionary fieldDic = (PdfDictionary)field;
                    if (fieldDic.ContainsKey(PdfName.A) || fieldDic.ContainsKey(PdfName.AA)) {
                        throw new PdfAConformanceException(PdfaExceptionMessageConstant.WIDGET_ANNOTATION_DICTIONARY_OR_FIELD_DICTIONARY_SHALL_NOT_INCLUDE_A_OR_AA_ENTRY
                            );
                    }
                    CheckResources(fieldDic.GetAsDictionary(PdfName.DR), fieldDic);
                }
            }
        }

        protected internal override void CheckAction(PdfDictionary action) {
            if (IsAlreadyChecked(action)) {
                return;
            }
            PdfName s = action.GetAsName(PdfName.S);
            if (GetForbiddenActions().Contains(s)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant._0_ACTIONS_ARE_NOT_ALLOWED).SetMessageParams
                    (s.GetValue());
            }
            if (s.Equals(PdfName.Named)) {
                PdfName n = action.GetAsName(PdfName.N);
                if (n != null && !GetAllowedNamedActions().Contains(n)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.NAMED_ACTION_TYPE_0_IS_NOT_ALLOWED).SetMessageParams
                        (n.GetValue());
                }
            }
            if (s.Equals(PdfName.SetState) || s.Equals(PdfName.NoOp)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.DEPRECATED_SETSTATE_AND_NOOP_ACTIONS_ARE_NOT_ALLOWED
                    );
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void CheckCatalog(PdfCatalog catalog) {
            String pdfVersion = catalog.GetDocument().GetPdfVersion().ToString();
            if ('1' != pdfVersion[4] || ('1' > pdfVersion[6] || '7' < pdfVersion[6])) {
                throw new PdfAConformanceException(MessageFormatUtil.Format(PdfaExceptionMessageConstant.THE_FILE_HEADER_SHALL_CONTAIN_RIGHT_PDF_VERSION
                    , "1"));
            }
        }

        protected internal override void CheckCatalogValidEntries(PdfDictionary catalogDict) {
            if (catalogDict.ContainsKey(PdfName.AA)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_AA_ENTRY
                    );
            }
            if (catalogDict.ContainsKey(PdfName.OCProperties)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_OCPROPERTIES_KEY
                    );
            }
            if (catalogDict.ContainsKey(PdfName.Names)) {
                if (catalogDict.GetAsDictionary(PdfName.Names).ContainsKey(PdfName.EmbeddedFiles)) {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.A_NAME_DICTIONARY_SHALL_NOT_CONTAIN_THE_EMBEDDED_FILES_KEY
                        );
                }
            }
        }

        protected internal override void CheckPageObject(PdfDictionary pageDict, PdfDictionary pageResources) {
            PdfDictionary actions = pageDict.GetAsDictionary(PdfName.AA);
            if (actions != null) {
                foreach (PdfName key in actions.KeySet()) {
                    PdfDictionary action = actions.GetAsDictionary(key);
                    CheckAction(action);
                }
            }
            if (IsContainsTransparencyGroup(pageDict)) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.A_GROUP_OBJECT_WITH_AN_S_KEY_WITH_A_VALUE_OF_TRANSPARENCY_SHALL_NOT_BE_INCLUDED_IN_A_PAGE_XOBJECT
                    );
            }
        }

        protected internal override void CheckTrailer(PdfDictionary trailer) {
        }

        /// <summary>
        /// Gets a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of fields with kids from a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// objects.
        /// </summary>
        /// <param name="array">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of form fields
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// objects
        /// </param>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of form fields
        /// </returns>
        protected internal virtual PdfArray GetFormFields(PdfArray array) {
            PdfArray fields = new PdfArray();
            foreach (PdfObject field in array) {
                PdfArray kids = ((PdfDictionary)field).GetAsArray(PdfName.Kids);
                fields.Add(field);
                if (kids != null) {
                    fields.AddAll(GetFormFields(kids));
                }
            }
            return fields;
        }

        private int GetMaxArrayCapacity() {
            return 8191;
        }

        private int GetMaxDictionaryCapacity() {
            return 4095;
        }
    }
}
