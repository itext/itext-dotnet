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
using iTextSharp.IO.Log;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Annot;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Pdf.Colorspace;
using iTextSharp.Pdfa;

namespace iTextSharp.Pdfa.Checker
{
    public class PdfA1Checker : PdfAChecker
    {
        protected internal static readonly ICollection<PdfName> forbiddenAnnotations = new 
            HashSet<PdfName>(iTextSharp.IO.Util.JavaUtil.ArraysAsList(PdfName.Sound, PdfName
            .Movie, PdfName.FileAttachment));

        protected internal static readonly ICollection<PdfName> contentAnnotations = new 
            HashSet<PdfName>(iTextSharp.IO.Util.JavaUtil.ArraysAsList(PdfName.Text, PdfName
            .FreeText, PdfName.Line, PdfName.Square, PdfName.Circle, PdfName.Stamp, PdfName
            .Ink, PdfName.Popup));

        protected internal static readonly ICollection<PdfName> forbiddenActions = new HashSet
            <PdfName>(iTextSharp.IO.Util.JavaUtil.ArraysAsList(PdfName.Launch, PdfName.Sound
            , PdfName.Movie, PdfName.ResetForm, PdfName.ImportData, PdfName.JavaScript, PdfName
            .Hide));

        protected internal static readonly ICollection<PdfName> allowedNamedActions = new 
            HashSet<PdfName>(iTextSharp.IO.Util.JavaUtil.ArraysAsList(PdfName.NextPage, PdfName
            .PrevPage, PdfName.FirstPage, PdfName.LastPage));

        protected internal static readonly ICollection<PdfName> allowedRenderingIntents = 
            new HashSet<PdfName>(iTextSharp.IO.Util.JavaUtil.ArraysAsList(PdfName.RelativeColorimetric
            , PdfName.AbsoluteColorimetric, PdfName.Perceptual, PdfName.Saturation));

        public PdfA1Checker(PdfAConformanceLevel conformanceLevel)
            : base(conformanceLevel)
        {
        }

        public override void CheckCanvasStack(char stackOperation)
        {
            if ('q' == stackOperation)
            {
                if (++gsStackDepth > iTextSharp.Pdfa.Checker.PdfA1Checker.maxGsStackDepth)
                {
                    throw new PdfAConformanceException(PdfAConformanceException.GraphicStateStackDepthIsGreaterThan28
                        );
                }
            }
            else
            {
                if ('Q' == stackOperation)
                {
                    gsStackDepth--;
                }
            }
        }

        public override void CheckInlineImage(PdfStream inlineImage, PdfDictionary currentColorSpaces
            )
        {
            PdfObject filter = inlineImage.Get(PdfName.Filter);
            if (filter is PdfName)
            {
                if (filter.Equals(PdfName.LZWDecode))
                {
                    throw new PdfAConformanceException(PdfAConformanceException.LZWDecodeFilterIsNotPermitted
                        );
                }
            }
            else
            {
                if (filter is PdfArray)
                {
                    for (int i = 0; i < ((PdfArray)filter).Size(); i++)
                    {
                        PdfName f = ((PdfArray)filter).GetAsName(i);
                        if (f.Equals(PdfName.LZWDecode))
                        {
                            throw new PdfAConformanceException(PdfAConformanceException.LZWDecodeFilterIsNotPermitted
                                );
                        }
                    }
                }
            }
            CheckImage(inlineImage, currentColorSpaces);
        }

        public override void CheckColor(iTextSharp.Kernel.Color.Color color, PdfDictionary
             currentColorSpaces, bool? fill)
        {
            CheckColorSpace(color.GetColorSpace(), currentColorSpaces, true, fill);
        }

        public override void CheckColorSpace(PdfColorSpace colorSpace, PdfDictionary currentColorSpaces
            , bool checkAlternate, bool? fill)
        {
            if (colorSpace is PdfSpecialCs.Separation)
            {
                colorSpace = ((PdfSpecialCs.Separation)colorSpace).GetBaseCs();
            }
            else
            {
                if (colorSpace is PdfSpecialCs.DeviceN)
                {
                    colorSpace = ((PdfSpecialCs.DeviceN)colorSpace).GetBaseCs();
                }
            }
            if (colorSpace is PdfDeviceCs.Rgb)
            {
                if (cmykIsUsed)
                {
                    throw new PdfAConformanceException(PdfAConformanceException.DevicergbAndDevicecmykColorspacesCannotBeUsedBothInOneFile
                        );
                }
                rgbIsUsed = true;
            }
            else
            {
                if (colorSpace is PdfDeviceCs.Cmyk)
                {
                    if (rgbIsUsed)
                    {
                        throw new PdfAConformanceException(PdfAConformanceException.DevicergbAndDevicecmykColorspacesCannotBeUsedBothInOneFile
                            );
                    }
                    cmykIsUsed = true;
                }
                else
                {
                    if (colorSpace is PdfDeviceCs.Gray)
                    {
                        grayIsUsed = true;
                    }
                }
            }
        }

        protected internal override ICollection<PdfName> GetForbiddenActions()
        {
            return forbiddenActions;
        }

        protected internal override ICollection<PdfName> GetAllowedNamedActions()
        {
            return allowedNamedActions;
        }

        protected internal override void CheckColorsUsages()
        {
            if ((rgbIsUsed || cmykIsUsed || grayIsUsed) && pdfAOutputIntentColorSpace == null)
            {
                throw new PdfAConformanceException(PdfAConformanceException.IfDeviceRgbCmykGrayUsedInFileThatFileShallContainPdfaOutputIntent
                    );
            }
            if (rgbIsUsed)
            {
                if (!ICC_COLOR_SPACE_RGB.Equals(pdfAOutputIntentColorSpace))
                {
                    throw new PdfAConformanceException(PdfAConformanceException.DevicergbMayBeUsedOnlyIfTheFileHasARgbPdfAOutputIntent
                        );
                }
            }
            if (cmykIsUsed)
            {
                if (!ICC_COLOR_SPACE_CMYK.Equals(pdfAOutputIntentColorSpace))
                {
                    throw new PdfAConformanceException(PdfAConformanceException.DevicecmykMayBeUsedOnlyIfTheFileHasACmykPdfAOutputIntent
                        );
                }
            }
        }

        public override void CheckExtGState(CanvasGraphicsState extGState)
        {
            if (extGState.GetTransferFunction() != null)
            {
                throw new PdfAConformanceException(PdfAConformanceException.AnExtgstateDictionaryShallNotContainTheTrKey
                    );
            }
            PdfObject transferFunction2 = extGState.GetTransferFunction2();
            if (transferFunction2 != null && !PdfName.Default.Equals(transferFunction2))
            {
                throw new PdfAConformanceException(PdfAConformanceException.AnExtgstateDictionaryShallNotContainTheTR2KeyWithAValueOtherThanDefault
                    );
            }
            CheckRenderingIntent(extGState.GetRenderingIntent());
            PdfObject softMask = extGState.GetSoftMask();
            if (softMask != null && !PdfName.None.Equals(softMask))
            {
                throw new PdfAConformanceException(PdfAConformanceException.TheSmaskKeyIsNotAllowedInExtgstate
                    );
            }
            PdfObject bm = extGState.GetBlendMode();
            if (bm != null && !PdfName.Normal.Equals(bm) && !PdfName.Compatible.Equals(bm))
            {
                throw new PdfAConformanceException(PdfAConformanceException.BlendModeShallHhaveValueNormalOrCompatible
                    );
            }
            float? ca = extGState.GetStrokeOpacity();
            if (ca != null && ca != 1)
            {
                throw new PdfAConformanceException(PdfAConformanceException.TransparencyIsNotAllowedCAShallBeEqualTo1
                    );
            }
            ca = extGState.GetFillOpacity();
            if (ca != null && ca != 1)
            {
                throw new PdfAConformanceException(PdfAConformanceException.TransparencyIsNotAllowedCaShallBeEqualTo1
                    );
            }
        }

        public override void CheckRenderingIntent(PdfName intent)
        {
            if (intent == null)
            {
                return;
            }
            if (!allowedRenderingIntents.Contains(intent))
            {
                throw new PdfAConformanceException(PdfAConformanceException.IfSpecifiedRenderingShallBeOneOfTheFollowingRelativecolorimetricAbsolutecolorimetricPerceptualOrSaturation
                    );
            }
        }

        protected internal override void CheckImage(PdfStream image, PdfDictionary currentColorSpaces
            )
        {
            PdfColorSpace colorSpace = null;
            if (IsAlreadyChecked(image))
            {
                colorSpace = checkedObjectsColorspace.Get(image);
                CheckColorSpace(colorSpace, currentColorSpaces, true, null);
                return;
            }
            PdfObject colorSpaceObj = image.Get(PdfName.ColorSpace);
            if (colorSpaceObj != null)
            {
                colorSpace = PdfColorSpace.MakeColorSpace(colorSpaceObj);
                CheckColorSpace(colorSpace, currentColorSpaces, true, null);
                checkedObjectsColorspace[image] = colorSpace;
            }
            if (image.ContainsKey(PdfName.Alternates))
            {
                throw new PdfAConformanceException(PdfAConformanceException.AnImageDictionaryShallNotContainAlternatesKey
                    );
            }
            if (image.ContainsKey(PdfName.OPI))
            {
                throw new PdfAConformanceException(PdfAConformanceException.AnImageDictionaryShallNotContainOpiKey
                    );
            }
            if (image.ContainsKey(PdfName.Interpolate) && (bool)image.GetAsBool(PdfName.Interpolate
                ))
            {
                throw new PdfAConformanceException(PdfAConformanceException.TheValueOfInterpolateKeyShallNotBeTrue
                    );
            }
            CheckRenderingIntent(image.GetAsName(PdfName.Intent));
            if (image.ContainsKey(PdfName.SMask) && !PdfName.None.Equals(image.GetAsName(PdfName
                .SMask)))
            {
                throw new PdfAConformanceException(PdfAConformanceException.TheSmaskKeyIsNotAllowedInXobjects
                    );
            }
        }

        protected internal override void CheckFormXObject(PdfStream form)
        {
            if (IsAlreadyChecked(form))
            {
                return;
            }
            if (form.ContainsKey(PdfName.OPI))
            {
                throw new PdfAConformanceException(PdfAConformanceException.AFormXobjectDictionaryShallNotContainOpiKey
                    );
            }
            if (form.ContainsKey(PdfName.PS))
            {
                throw new PdfAConformanceException(PdfAConformanceException.AFormXobjectDictionaryShallNotContainPSKey
                    );
            }
            if (PdfName.PS.Equals(form.GetAsName(PdfName.Subtype2)))
            {
                throw new PdfAConformanceException(PdfAConformanceException.AFormXobjectDictionaryShallNotContainSubtype2KeyWithAValueOfPS
                    );
            }
            if (form.ContainsKey(PdfName.SMask))
            {
                throw new PdfAConformanceException(PdfAConformanceException.TheSmaskKeyIsNotAllowedInXobjects
                    );
            }
            if (form.ContainsKey(PdfName.Group) && PdfName.Transparency.Equals(form.GetAsDictionary
                (PdfName.Group).GetAsName(PdfName.S)))
            {
                throw new PdfAConformanceException(PdfAConformanceException.AGroupObjectWithAnSKeyWithAValueOfTransparencyShallNotBeIncludedInAFormXobject
                    );
            }
        }

        protected internal override void CheckLogicalStructure(PdfDictionary catalog)
        {
            if (CheckStructure(conformanceLevel))
            {
                PdfDictionary markInfo = catalog.GetAsDictionary(PdfName.MarkInfo);
                if (markInfo == null || markInfo.GetAsBoolean(PdfName.Marked) == null || !markInfo
                    .GetAsBoolean(PdfName.Marked).GetValue())
                {
                    throw new PdfAConformanceException(PdfAConformanceException.CatalogShallIncludeMarkInfoDictionaryWithMarkedTrueValue
                        );
                }
                if (!catalog.ContainsKey(PdfName.Lang))
                {
                    ILogger logger = LoggerFactory.GetLogger(typeof(PdfAChecker));
                    logger.Warn(PdfAConformanceException.CatalogShallContainLangEntry);
                }
            }
        }

        protected internal override void CheckMetaData(PdfDictionary catalog)
        {
            if (!catalog.ContainsKey(PdfName.Metadata))
            {
                throw new PdfAConformanceException(PdfAConformanceException.CatalogShallContainMetadataEntry
                    );
            }
        }

        protected internal override void CheckOutputIntents(PdfDictionary catalog)
        {
            PdfArray outputIntents = catalog.GetAsArray(PdfName.OutputIntents);
            if (outputIntents == null)
            {
                return;
            }
            int i;
            PdfObject destOutputProfile = null;
            for (i = 0; i < outputIntents.Size() && destOutputProfile == null; ++i)
            {
                destOutputProfile = outputIntents.GetAsDictionary(i).Get(PdfName.DestOutputProfile
                    );
            }
            for (; i < outputIntents.Size(); ++i)
            {
                PdfObject otherDestOutputProfile = outputIntents.GetAsDictionary(i).Get(PdfName.DestOutputProfile
                    );
                if (otherDestOutputProfile != null && destOutputProfile != otherDestOutputProfile)
                {
                    throw new PdfAConformanceException(PdfAConformanceException.IfOutputintentsArrayHasMoreThanOneEntryWithDestoutputprofileKeyTheSameIndirectObjectShallBeUsedAsTheValueOfThatObject
                        );
                }
            }
        }

        protected internal override void CheckPdfNumber(PdfNumber number)
        {
            if (Math.Abs(number.LongValue()) > GetMaxRealValue() && number.ToString().Contains
                ("."))
            {
                throw new PdfAConformanceException(PdfAConformanceException.RealNumberIsOutOfRange
                    );
            }
        }

        protected internal virtual double GetMaxRealValue()
        {
            return 32767;
        }

        protected internal override void CheckPdfStream(PdfStream stream)
        {
            if (stream.ContainsKey(PdfName.F) || stream.ContainsKey(PdfName.FFilter) || stream
                .ContainsKey(PdfName.FDecodeParams))
            {
                throw new PdfAConformanceException(PdfAConformanceException.StreamObjDictShallNotContainForFFilterOrFDecodeParams
                    );
            }
            PdfObject filter = stream.Get(PdfName.Filter);
            if (filter is PdfName)
            {
                if (filter.Equals(PdfName.LZWDecode))
                {
                    throw new PdfAConformanceException(PdfAConformanceException.LZWDecodeFilterIsNotPermitted
                        );
                }
            }
            else
            {
                if (filter is PdfArray)
                {
                    foreach (PdfObject f in ((PdfArray)filter))
                    {
                        if (f.Equals(PdfName.LZWDecode))
                        {
                            throw new PdfAConformanceException(PdfAConformanceException.LZWDecodeFilterIsNotPermitted
                                );
                        }
                    }
                }
            }
        }

        protected internal override void CheckPdfString(PdfString @string)
        {
            if (@string.GetValueBytes().Length > GetMaxStringLength())
            {
                throw new PdfAConformanceException(PdfAConformanceException.PdfStringIsTooLong);
            }
        }

        protected internal virtual int GetMaxStringLength()
        {
            return 65535;
        }

        protected internal override void CheckPageSize(PdfDictionary page)
        {
        }

        protected internal override void CheckFileSpec(PdfDictionary fileSpec)
        {
            if (fileSpec.ContainsKey(PdfName.EF))
            {
                throw new PdfAConformanceException(PdfAConformanceException.FileSpecificationDictionaryShallNotContainTheEFKey
                    );
            }
        }

        protected internal override void CheckAnnotation(PdfDictionary annotDic)
        {
            PdfName subtype = annotDic.GetAsName(PdfName.Subtype);
            if (subtype == null)
            {
                throw new PdfAConformanceException(PdfAConformanceException.AnnotationType1IsNotPermitted
                    ).SetMessageParams("null");
            }
            if (forbiddenAnnotations.Contains(subtype))
            {
                throw new PdfAConformanceException(PdfAConformanceException.AnnotationType1IsNotPermitted
                    ).SetMessageParams(subtype.GetValue());
            }
            PdfNumber ca = annotDic.GetAsNumber(PdfName.CA);
            if (ca != null && ca.FloatValue() != 1.0)
            {
                throw new PdfAConformanceException(PdfAConformanceException.AnAnnotationDictionaryShallNotContainTheCaKeyWithAValueOtherThan1
                    );
            }
            if (!annotDic.ContainsKey(PdfName.F))
            {
                throw new PdfAConformanceException(PdfAConformanceException.AnnotationShallContainKeyF
                    );
            }
            int flags = (int)annotDic.GetAsInt(PdfName.F);
            if (!CheckFlag(flags, PdfAnnotation.PRINT) || CheckFlag(flags, PdfAnnotation.HIDDEN
                ) || CheckFlag(flags, PdfAnnotation.INVISIBLE) || CheckFlag(flags, PdfAnnotation
                .NO_VIEW))
            {
                throw new PdfAConformanceException(PdfAConformanceException.TheFKeysPrintFlagBitShallBeSetTo1AndItsHiddenInvisibleAndNoviewFlagBitsShallBeSetTo0
                    );
            }
            if (subtype.Equals(PdfName.Text) && (!CheckFlag(flags, PdfAnnotation.NO_ZOOM) || 
                !CheckFlag(flags, PdfAnnotation.NO_ROTATE)))
            {
                throw new PdfAConformanceException(PdfAConformanceException.TextAnnotationsShouldSetTheNozoomAndNorotateFlagBitsOfTheFKeyTo1
                    );
            }
            if (annotDic.ContainsKey(PdfName.C) || annotDic.ContainsKey(PdfName.IC))
            {
                if (!ICC_COLOR_SPACE_RGB.Equals(pdfAOutputIntentColorSpace))
                {
                    throw new PdfAConformanceException(PdfAConformanceException.DestoutputprofileInThePdfa1OutputintentDictionaryShallBeRgb
                        );
                }
            }
            PdfDictionary ap = annotDic.GetAsDictionary(PdfName.AP);
            if (ap != null)
            {
                if (ap.ContainsKey(PdfName.D) || ap.ContainsKey(PdfName.R))
                {
                    throw new PdfAConformanceException(PdfAConformanceException.AppearanceDictionaryShallContainOnlyTheNKeyWithStreamValue
                        );
                }
                PdfStream n = ap.GetAsStream(PdfName.N);
                if (n == null)
                {
                    throw new PdfAConformanceException(PdfAConformanceException.AppearanceDictionaryShallContainOnlyTheNKeyWithStreamValue
                        );
                }
            }
            if (PdfName.Widget.Equals(subtype) && (annotDic.ContainsKey(PdfName.AA) || annotDic
                .ContainsKey(PdfName.A)))
            {
                throw new PdfAConformanceException(PdfAConformanceException.WidgetAnnotationDictionaryOrFieldDictionaryShallNotIncludeAOrAAEntry
                    );
            }
            if (annotDic.ContainsKey(PdfName.AA))
            {
                throw new PdfAConformanceException(PdfAConformanceException.AnAnnotationDictionaryShallNotContainAAKey
                    );
            }
            if (CheckStructure(conformanceLevel))
            {
                if (contentAnnotations.Contains(subtype) && !annotDic.ContainsKey(PdfName.Contents
                    ))
                {
                    throw new PdfAConformanceException(PdfAConformanceException.AnnotationOfType1ShouldHaveContentsKey
                        ).SetMessageParams(subtype);
                }
            }
        }

        protected internal override void CheckForm(PdfDictionary form)
        {
            if (form == null)
            {
                return;
            }
            PdfBoolean needAppearances = form.GetAsBoolean(PdfName.NeedAppearances);
            if (needAppearances != null && needAppearances.GetValue())
            {
                throw new PdfAConformanceException(PdfAConformanceException.NeedAppearancesFlagOfTheInteractiveFormDictionaryShallEitherNotBePresentedOrShallBeFalse
                    );
            }
            PdfArray fields = form.GetAsArray(PdfName.Fields);
            if (fields != null)
            {
                fields = GetFormFields(fields);
                foreach (PdfObject field in fields)
                {
                    PdfDictionary fieldDic = (PdfDictionary)field;
                    if (fieldDic.ContainsKey(PdfName.A) || fieldDic.ContainsKey(PdfName.AA))
                    {
                        throw new PdfAConformanceException(PdfAConformanceException.WidgetAnnotationDictionaryOrFieldDictionaryShallNotIncludeAOrAAEntry
                            );
                    }
                }
            }
        }

        protected internal override void CheckAction(PdfDictionary action)
        {
            if (IsAlreadyChecked(action))
            {
                return;
            }
            PdfName s = action.GetAsName(PdfName.S);
            if (GetForbiddenActions().Contains(s))
            {
                throw new PdfAConformanceException(PdfAConformanceException._1ActionsAreNotAllowed
                    ).SetMessageParams(s.GetValue());
            }
            if (s.Equals(PdfName.Named))
            {
                PdfName n = action.GetAsName(PdfName.N);
                if (n != null && !GetAllowedNamedActions().Contains(n))
                {
                    throw new PdfAConformanceException(PdfAConformanceException.NamedActionType1IsNotAllowed
                        ).SetMessageParams(n.GetValue());
                }
            }
            if (s.Equals(PdfName.SetState) || s.Equals(PdfName.NoOp))
            {
                throw new PdfAConformanceException(PdfAConformanceException.DeprecatedSetStateAndNoOpActionsAreNotAllowed
                    );
            }
        }

        protected internal override void CheckCatalogValidEntries(PdfDictionary catalogDict
            )
        {
            if (catalogDict.ContainsKey(PdfName.AA))
            {
                throw new PdfAConformanceException(PdfAConformanceException.CatalogDictionaryShallNotContainAAEntry
                    );
            }
            if (catalogDict.ContainsKey(PdfName.OCProperties))
            {
                throw new PdfAConformanceException(PdfAConformanceException.CatalogDictionaryShallNotContainOCPropertiesKey
                    );
            }
            if (catalogDict.ContainsKey(PdfName.Names))
            {
                if (catalogDict.GetAsDictionary(PdfName.Names).ContainsKey(PdfName.EmbeddedFiles))
                {
                    throw new PdfAConformanceException(PdfAConformanceException.NameDictionaryShallNotContainTheEmbeddedFilesKey
                        );
                }
            }
        }

        protected internal override void CheckPageObject(PdfDictionary pageDict, PdfDictionary
             pageResources)
        {
            PdfDictionary actions = pageDict.GetAsDictionary(PdfName.AA);
            if (actions != null)
            {
                foreach (PdfName key in actions.KeySet())
                {
                    PdfDictionary action = actions.GetAsDictionary(key);
                    CheckAction(action);
                }
            }
            if (pageDict.ContainsKey(PdfName.Group) && PdfName.Transparency.Equals(pageDict.GetAsDictionary
                (PdfName.Group).GetAsName(PdfName.S)))
            {
                throw new PdfAConformanceException(PdfAConformanceException.AGroupObjectWithAnSKeyWithAValueOfTransparencyShallNotBeIncludedInAPageObject
                    );
            }
        }

        protected internal override void CheckTrailer(PdfDictionary trailer)
        {
            if (trailer.ContainsKey(PdfName.Encrypt))
            {
                throw new PdfAConformanceException(PdfAConformanceException.EncryptShallNotBeUsedInTrailerDictionary
                    );
            }
        }

        private PdfArray GetFormFields(PdfArray array)
        {
            PdfArray fields = new PdfArray();
            // explicit iteration to resolve indirect references on get().
            // TODO DEVSIX-591
            for (int i = 0; i < array.Size(); i++)
            {
                PdfDictionary field = array.GetAsDictionary(i);
                PdfArray kids = field.GetAsArray(PdfName.Kids);
                fields.Add(field);
                if (kids != null)
                {
                    fields.AddAll(GetFormFields(kids));
                }
            }
            return fields;
        }
    }
}
