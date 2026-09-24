/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Svg.Logs;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class MaskTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/MaskTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/MaskTest/";

        private ISvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void MaskBasic() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskBasic", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnquotedUrlReferenceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnquotedUrlReference", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskSingleQuotedUrlReferenceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskSingleQuotedUrlReference", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskDoubleQuotedUrlReferenceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskDoubleQuotedUrlReference", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskSingleQuotedUrlReferenceWithWhitespaceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskSingleQuotedUrlReferenceWithWhitespace"
                , properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskDoubleQuotedUrlReferenceWithWhitespaceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskDoubleQuotedUrlReferenceWithWhitespace"
                , properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskWithGradientWithStopOpacity() {
            //TODO DEVSIX-4136 update after gradient opacity support implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskWithGradientWithStopOpacity", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskContentUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskContentUnits", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskPatternCombiTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPatternCombi", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskPatternAppliedTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPatternApplied", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskMultiShapesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskMultiShapes", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskPatternGradientAppliedTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPatternGradientApplied", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskGradientAppliedMaskContentUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskGradientAppliedMaskContentUnits", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskPatternMaskContentUnitsUserSpaceOnUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPatternMaskContentUnitsUserSpaceOnUse"
                , properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsObjectBoundingBoxTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsObjectBoundingBox", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsUserSpaceOnUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsUserSpaceOnUse", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTransformTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTransform", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTransform2Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTransform2", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTransform3Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTransform3", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskInheritedBasicTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskInheritedBasic", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskInherited3LevelTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskInherited3Level", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskPatternAppliedInheritedTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPatternAppliedInherited", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsObjectBoundingBoxInheritedTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsObjectBoundingBoxInherited", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsUserSpaceOnUseInherited2Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsUserSpaceOnUseInherited2", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsUserSpaceOnUseInheritedTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsUserSpaceOnUseInherited", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsObjectBoundingBoxInherited2Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsObjectBoundingBoxInherited2", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskTransformInheritedTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTransformInherited", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskWithLinearGradient() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskWithLinearGradient", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskWithRadialGradientTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskWithRadialGradient", properties);
        }

        [NUnit.Framework.Test]
        public virtual void LuminanceMaskWithLinearGradientTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "luminanceMaskWithLinearGradient", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void LuminanceMaskWithRadialGradientTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "luminanceMaskWithRadialGradient", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void LuminanceMaskWithPatternTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "luminanceMaskWithPattern", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskWithPatternMaskTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskWithPatternMask", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskReferenceOnContainerTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskReferenceOnContainer", properties);
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.INVALID_MASK_REFERENCE)]
        public virtual void MaskMissingReferenceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskMissingReference", properties);
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.INVALID_MASK_REFERENCE)]
        public virtual void MaskReferenceToNonMaskElementTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskReferenceToNonMaskElement", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskOffscreenCompositingWithBackgroundTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskOffscreenCompositingWithBackground", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskAppliedAfterStrokeAndMarkerCompositingTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskAppliedAfterStrokeAndMarkerCompositing"
                , properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskAlphaMultiplicationSemanticsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskAlphaMultiplicationSemantics", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskTypeLuminanceWithColoredContentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTypeLuminanceWithColoredContent", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskTypeLuminanceWithStrokeContentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTypeLuminanceWithStrokeContent", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskTypeAlphaWithColoredContentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTypeAlphaWithColoredContent", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskTypeDefaultLuminanceWhenOmittedTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTypeDefaultLuminanceWhenOmitted", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskTypePresentationAttributeAlphaTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTypePresentationAttributeAlpha", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskTypeStyleOverridesPresentationAttributeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTypeStyleOverridesPresentationAttribute"
                , properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTypeInvalidTokenDefaultsToLuminanceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTypeInvalidTokenDefaultsToLuminance", 
                properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTypeAlphaComplexContentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTypeAlphaComplexContent", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTypeAlphaAndLuminanceSideBySideTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTypeAlphaAndLuminanceSideBySide", properties
                );
        }

        // TODO DEVSIX-10201 update cmp when image mask failures are fixed
        [NUnit.Framework.Test]
        public virtual void MaskImageAlphaAndLuminanceSideBySideTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskImageAlphaAndLuminanceSideBySide", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskLuminanceFromRgbaContentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskLuminanceFromRgbaContent", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskLuminanceFromRgbContentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskLuminanceFromRgbContent", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskLuminanceFromGrayscaleGradientTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskLuminanceFromGrayscaleGradient", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskTransparentColorsInContentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTransparentColorsInContent", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskColorInterpolationSrgbTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskColorInterpolationSrgb", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskColorInterpolationLinearRgbTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskColorInterpolationLinearRgb", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskColorInterpolationInvalidTokenFallbackTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskColorInterpolationInvalidTokenFallback"
                , properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskOverlappingTransparentElementsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskOverlappingTransparentElements", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskPaintOrderAffectsResultTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPaintOrderAffectsResult", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskNestedMaskUsageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskNestedMaskUsage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskDefinitionNotRenderedWhenUnusedTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskDefinitionNotRenderedWhenUnused", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskDisplayNoneStillReferenceableTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskDisplayNoneStillReferenceable", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskAncestorDisplayNoneStillReferenceableTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskAncestorDisplayNoneStillReferenceable"
                , properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskElementOpacityIgnoredTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskElementOpacityIgnored", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskElementFilterIgnoredTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskElementFilterIgnored", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsDefaultObjectBoundingBoxTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsDefaultObjectBoundingBox", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskContentUnitsDefaultUserSpaceOnUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskContentUnitsDefaultUserSpaceOnUse", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskDefaultXyWidthHeightValuesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskDefaultXyWidthHeightValues", properties
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.MASK_WIDTH_OR_HEIGHT_IS_NEGATIVE)]
        public virtual void MaskNegativeWidthTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskNegativeWidth", properties);
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.MASK_WIDTH_OR_HEIGHT_IS_NEGATIVE)]
        public virtual void MaskNegativeHeightTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskNegativeHeight", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskZeroWidthDisablesRenderingTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskZeroWidthDisablesRendering", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskZeroHeightDisablesRenderingTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskZeroHeightDisablesRendering", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskClipIntersectionWithClipPathTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskClipIntersectionWithClipPath", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskRadialGradientTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskRadialGradient", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTransparentOverTransparentTargetsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTransparentOverTransparentTargets", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskGroupOverTransparentBackgroundTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskGroupOverTransparentBackground", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskInheritsFromOwnAncestorsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskInheritsFromOwnAncestors", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskDoesNotInheritFromReferencingElementTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskDoesNotInheritFromReferencingElement", 
                properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskPropertyNoneTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPropertyNone", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskUrlOverridesInheritedNoneTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUrlOverridesInheritedNone", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskPropertyInheritTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPropertyInherit", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskPropertyInheritDefaultsToNoneTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPropertyInheritDefaultsToNone", properties
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void MaskAnimatedPropertyInputTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskAnimatedPropertyInput", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskCombinedStressCaseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskCombinedStressCase", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskContentUnitsObjectBoundingBoxGroupCompensationTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskContentUnitsObjectBoundingBoxGroupCompensation"
                , properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskLargeCoordinateScalingStressTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskLargeCoordinateScalingStress", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskSharedMaskAcrossSiblingsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskSharedMaskAcrossSiblings", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskDistinctMasksOnSiblingsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskDistinctMasksOnSiblings", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TransformedMaskedRendererAppliesTransformOnceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "transformedMaskedRendererAppliesTransformOnce"
                , properties);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxMaskContentIsNormalizedTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxMaskContentIsNormalized", 
                properties);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxMaskContentPercentagesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxMaskContentPercentages", 
                properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithDefaultMaskUnitsIsDrawnTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithDefaultMaskUnitsIsDrawn", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void SelfReferencingMaskCycleDoesNotOverflowTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "selfReferencingMaskCycleDoesNotOverflow", 
                properties);
        }

        [NUnit.Framework.Test]
        public virtual void IndirectMaskCycleDoesNotOverflowTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "indirectMaskCycleDoesNotOverflow", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskTextBasicTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTextBasic", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTextTypeLuminanceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTextTypeLuminance", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTextLuminanceWithColoredFillTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTextLuminanceWithColoredFill", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskTextWithTspanTransformTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTextWithTspanTransform", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskUsedOnTextBasicTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUsedOnTextBasic", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskUsedOnTextWithGradientMaskTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUsedOnTextWithGradientMask", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskUsedOnTextWithTspanTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUsedOnTextWithTspan", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskImageAsMaskContentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskImageAsMaskContent", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskAppliedToImageElementTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskAppliedToImageElement", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskGroupWithDisplayNoneChildTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskGroupWithDisplayNoneChild", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskedUseCycleTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskedUseCycle", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskImageSliceWithOffsetTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskImageSliceWithOffset", properties);
        }
    }
}
