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
using System.Collections;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>
    /// Generates standard structure attributes for current tag
    /// based on the layout element properties and renderer layout results.
    /// </summary>
    public class AccessibleAttributesApplier {
        public static PdfStructureAttributes GetLayoutAttributes(AbstractRenderer renderer, TagTreePointer taggingPointer
            ) {
            IRoleMappingResolver resolvedMapping = ResolveMappingToStandard(taggingPointer);
            if (resolvedMapping == null) {
                return null;
            }
            String role = resolvedMapping.GetRole();
            int tagType = AccessibleTypes.IdentifyType(role);
            PdfDictionary attributes = new PdfDictionary();
            attributes.Put(PdfName.O, PdfName.Layout);
            // TODO DEVSIX-7016 WritingMode attribute applying when needed
            ApplyCommonLayoutAttributes(renderer, attributes);
            if (tagType == AccessibleTypes.BlockLevel) {
                ApplyBlockLevelLayoutAttributes(role, renderer, attributes);
            }
            if (tagType == AccessibleTypes.InlineLevel) {
                ApplyInlineLevelLayoutAttributes(renderer, attributes);
            }
            if (tagType == AccessibleTypes.Illustration) {
                ApplyIllustrationLayoutAttributes(renderer, attributes);
            }
            return attributes.Size() > 1 ? new PdfStructureAttributes(attributes) : null;
        }

        public static PdfStructureAttributes GetListAttributes(AbstractRenderer renderer, TagTreePointer taggingPointer
            ) {
            IRoleMappingResolver resolvedMapping = null;
            resolvedMapping = ResolveMappingToStandard(taggingPointer);
            if (resolvedMapping == null || !StandardRoles.L.Equals(resolvedMapping.GetRole())) {
                return null;
            }
            PdfDictionary attributes = new PdfDictionary();
            attributes.Put(PdfName.O, PdfName.List);
            Object listSymbol = renderer.GetProperty<Object>(Property.LIST_SYMBOL);
            bool tagStructurePdf2 = IsTagStructurePdf2(resolvedMapping.GetNamespace());
            if (listSymbol is ListNumberingType) {
                ListNumberingType numberingType = (ListNumberingType)listSymbol;
                attributes.Put(PdfName.ListNumbering, TransformNumberingTypeToName(numberingType, tagStructurePdf2));
            }
            else {
                if (tagStructurePdf2) {
                    if (listSymbol is IListSymbolFactory) {
                        attributes.Put(PdfName.ListNumbering, PdfName.Ordered);
                    }
                    else {
                        attributes.Put(PdfName.ListNumbering, PdfName.Unordered);
                    }
                }
            }
            return attributes.Size() > 1 ? new PdfStructureAttributes(attributes) : null;
        }

        public static PdfStructureAttributes GetTableAttributes(AbstractRenderer renderer, TagTreePointer taggingPointer
            ) {
            IRoleMappingResolver resolvedMapping = ResolveMappingToStandard(taggingPointer);
            if (resolvedMapping == null || !StandardRoles.TD.Equals(resolvedMapping.GetRole()) && !StandardRoles.TH.Equals
                (resolvedMapping.GetRole())) {
                return null;
            }
            PdfDictionary attributes = new PdfDictionary();
            attributes.Put(PdfName.O, PdfName.Table);
            if (renderer.GetModelElement() is Cell) {
                Cell cell = (Cell)renderer.GetModelElement();
                if (cell.GetRowspan() != 1) {
                    attributes.Put(PdfName.RowSpan, new PdfNumber(cell.GetRowspan()));
                }
                if (cell.GetColspan() != 1) {
                    attributes.Put(PdfName.ColSpan, new PdfNumber(cell.GetColspan()));
                }
            }
            return attributes.Size() > 1 ? new PdfStructureAttributes(attributes) : null;
        }

        private static void ApplyCommonLayoutAttributes(AbstractRenderer renderer, PdfDictionary attributes) {
            Background background = renderer.GetProperty<Background>(Property.BACKGROUND);
            if (background != null && background.GetColor() is DeviceRgb) {
                attributes.Put(PdfName.BackgroundColor, new PdfArray(background.GetColor().GetColorValue()));
            }
            //TODO DEVSIX-6255: applying border attributes for cells is temporarily turned off on purpose. Remove this 'if' in future.
            // The reason is that currently, we can't distinguish if all cells have same border style or not.
            // Therefore for every cell in every table we have to write the same border attributes, which creates lots of clutter.
            if (!(renderer.GetModelElement() is Cell)) {
                ApplyBorderAttributes(renderer, attributes);
            }
            ApplyPaddingAttribute(renderer, attributes);
            TransparentColor transparentColor = renderer.GetPropertyAsTransparentColor(Property.FONT_COLOR);
            if (transparentColor != null && transparentColor.GetColor() is DeviceRgb) {
                attributes.Put(PdfName.Color, new PdfArray(transparentColor.GetColor().GetColorValue()));
            }
        }

        private static void ApplyBlockLevelLayoutAttributes(String role, AbstractRenderer renderer, PdfDictionary 
            attributes) {
            UnitValue[] margins = new UnitValue[] { renderer.GetPropertyAsUnitValue(Property.MARGIN_TOP), renderer.GetPropertyAsUnitValue
                (Property.MARGIN_BOTTOM), renderer.GetPropertyAsUnitValue(Property.MARGIN_LEFT), renderer.GetPropertyAsUnitValue
                (Property.MARGIN_RIGHT) };
            //TODO DEVSIX-4218 set depending on writing direction
            int[] marginsOrder = new int[] { 0, 1, 2, 3 };
            UnitValue spaceBefore = margins[marginsOrder[0]];
            if (spaceBefore != null) {
                if (!spaceBefore.IsPointValue()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(AccessibleAttributesApplier));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                        , Property.MARGIN_TOP));
                }
                if (0 != spaceBefore.GetValue()) {
                    attributes.Put(PdfName.SpaceBefore, new PdfNumber(spaceBefore.GetValue()));
                }
            }
            UnitValue spaceAfter = margins[marginsOrder[1]];
            if (spaceAfter != null) {
                if (!spaceAfter.IsPointValue()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(AccessibleAttributesApplier));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                        , Property.MARGIN_BOTTOM));
                }
                if (0 != spaceAfter.GetValue()) {
                    attributes.Put(PdfName.SpaceAfter, new PdfNumber(spaceAfter.GetValue()));
                }
            }
            UnitValue startIndent = margins[marginsOrder[2]];
            if (startIndent != null) {
                if (!startIndent.IsPointValue()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(AccessibleAttributesApplier));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                        , Property.MARGIN_LEFT));
                }
                if (0 != startIndent.GetValue()) {
                    attributes.Put(PdfName.StartIndent, new PdfNumber(startIndent.GetValue()));
                }
            }
            UnitValue endIndent = margins[marginsOrder[3]];
            if (endIndent != null) {
                if (!endIndent.IsPointValue()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(AccessibleAttributesApplier));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                        , Property.MARGIN_RIGHT));
                }
                if (0 != endIndent.GetValue()) {
                    attributes.Put(PdfName.EndIndent, new PdfNumber(endIndent.GetValue()));
                }
            }
            float? firstLineIndent = renderer.GetPropertyAsFloat(Property.FIRST_LINE_INDENT);
            if (firstLineIndent != null && firstLineIndent != 0) {
                attributes.Put(PdfName.TextIndent, new PdfNumber((float)firstLineIndent));
            }
            TextAlignment? textAlignment = renderer.GetProperty<TextAlignment?>(Property.TEXT_ALIGNMENT);
            if (textAlignment != null && 
                        //for table cells there is an InlineAlign attribute (see below)
                        (!StandardRoles.TH.Equals(role) && !StandardRoles.TD.Equals(role))) {
                attributes.Put(PdfName.TextAlign, TransformTextAlignmentValueToName(textAlignment));
            }
            // attributes are applied only on the first renderer
            if (renderer.isLastRendererForModelElement) {
                Rectangle bbox = renderer.GetOccupiedArea().GetBBox();
                attributes.Put(PdfName.BBox, new PdfArray(bbox));
            }
            if (StandardRoles.TH.Equals(role) || StandardRoles.TD.Equals(role) || StandardRoles.TABLE.Equals(role)) {
                // For large tables the width can be changed from flush to flush so the Width attribute shouldn't be applied.
                // There are also technical issues with large tables widths being explicitly set as property on element during layouting
                // (even if user didn't explcitly specfied it). This is required due to specificity of large elements implementation,
                // however in this case we cannot distinguish layout-specific and user-specified width properties.
                if (!(renderer is TableRenderer) || ((Table)renderer.GetModelElement()).IsComplete()) {
                    UnitValue width = renderer.GetProperty<UnitValue>(Property.WIDTH);
                    if (width != null && width.IsPointValue()) {
                        attributes.Put(PdfName.Width, new PdfNumber(width.GetValue()));
                    }
                }
                UnitValue height = renderer.GetProperty<UnitValue>(Property.HEIGHT);
                if (height != null && height.IsPointValue()) {
                    attributes.Put(PdfName.Height, new PdfNumber(height.GetValue()));
                }
            }
            if (StandardRoles.TH.Equals(role) || StandardRoles.TD.Equals(role)) {
                HorizontalAlignment? horizontalAlignment = renderer.GetProperty<HorizontalAlignment?>(Property.HORIZONTAL_ALIGNMENT
                    );
                if (horizontalAlignment != null) {
                    attributes.Put(PdfName.BlockAlign, TransformBlockAlignToName(horizontalAlignment));
                }
                if (textAlignment != null && 
                                //there is no justified alignment for InlineAlign attribute
                                (textAlignment != TextAlignment.JUSTIFIED && textAlignment != TextAlignment.JUSTIFIED_ALL)) {
                    attributes.Put(PdfName.InlineAlign, TransformTextAlignmentValueToName(textAlignment));
                }
            }
        }

        private static void ApplyInlineLevelLayoutAttributes(AbstractRenderer renderer, PdfDictionary attributes) {
            float? textRise = renderer.GetPropertyAsFloat(Property.TEXT_RISE);
            if (textRise != null && textRise != 0) {
                attributes.Put(PdfName.BaselineShift, new PdfNumber((float)textRise));
            }
            Object underlines = renderer.GetProperty<Object>(Property.UNDERLINE);
            if (underlines != null) {
                UnitValue fontSize = renderer.GetPropertyAsUnitValue(Property.FONT_SIZE);
                if (!fontSize.IsPointValue()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(AccessibleAttributesApplier));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                        , Property.FONT_SIZE));
                }
                Underline underline = null;
                if (underlines is IList && ((IList)underlines).Count > 0 && ((IList)underlines)[0] is Underline) {
                    // in standard attributes only one text decoration could be described for an element. That's why we take only the first underline from the list.
                    underline = (Underline)((IList)underlines)[0];
                }
                else {
                    if (underlines is Underline) {
                        underline = (Underline)underlines;
                    }
                }
                if (underline != null) {
                    attributes.Put(PdfName.TextDecorationType, underline.GetYPosition(fontSize.GetValue()) > 0 ? PdfName.LineThrough
                         : PdfName.Underline);
                    if (underline.GetColor() is DeviceRgb) {
                        attributes.Put(PdfName.TextDecorationColor, new PdfArray(underline.GetColor().GetColorValue()));
                    }
                    attributes.Put(PdfName.TextDecorationThickness, new PdfNumber(underline.GetThickness(fontSize.GetValue()))
                        );
                }
            }
        }

        private static void ApplyIllustrationLayoutAttributes(AbstractRenderer renderer, PdfDictionary attributes) {
            Rectangle bbox = renderer.GetOccupiedArea().GetBBox();
            attributes.Put(PdfName.BBox, new PdfArray(bbox));
            UnitValue width = renderer.GetProperty<UnitValue>(Property.WIDTH);
            if (width != null && width.IsPointValue()) {
                attributes.Put(PdfName.Width, new PdfNumber(width.GetValue()));
            }
            else {
                attributes.Put(PdfName.Width, new PdfNumber(bbox.GetWidth()));
            }
            UnitValue height = renderer.GetProperty<UnitValue>(Property.HEIGHT);
            if (height != null) {
                attributes.Put(PdfName.Height, new PdfNumber(height.GetValue()));
            }
            else {
                attributes.Put(PdfName.Height, new PdfNumber(bbox.GetHeight()));
            }
        }

        private static void ApplyPaddingAttribute(AbstractRenderer renderer, PdfDictionary attributes) {
            UnitValue[] paddingsUV = new UnitValue[] { renderer.GetPropertyAsUnitValue(Property.PADDING_TOP), renderer
                .GetPropertyAsUnitValue(Property.PADDING_RIGHT), renderer.GetPropertyAsUnitValue(Property.PADDING_BOTTOM
                ), renderer.GetPropertyAsUnitValue(Property.PADDING_LEFT) };
            if (!paddingsUV[0].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(AccessibleAttributesApplier));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_TOP));
            }
            if (!paddingsUV[1].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(AccessibleAttributesApplier));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_RIGHT));
            }
            if (!paddingsUV[2].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(AccessibleAttributesApplier));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_BOTTOM));
            }
            if (!paddingsUV[3].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(AccessibleAttributesApplier));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_LEFT));
            }
            float[] paddings = new float[] { paddingsUV[0].GetValue(), paddingsUV[1].GetValue(), paddingsUV[2].GetValue
                (), paddingsUV[3].GetValue() };
            PdfObject padding = null;
            if (paddings[0] == paddings[1] && paddings[0] == paddings[2] && paddings[0] == paddings[3]) {
                if (paddings[0] != 0) {
                    padding = new PdfNumber(paddings[0]);
                }
            }
            else {
                PdfArray paddingArray = new PdfArray();
                //TODO DEVSIX-4218 set depending on writing direction
                int[] paddingsOrder = new int[] { 0, 1, 2, 3 };
                foreach (int i in paddingsOrder) {
                    paddingArray.Add(new PdfNumber(paddings[i]));
                }
                padding = paddingArray;
            }
            if (padding != null) {
                attributes.Put(PdfName.Padding, padding);
            }
        }

        private static void ApplyBorderAttributes(AbstractRenderer renderer, PdfDictionary attributes) {
            bool specificBorderProperties = renderer.GetProperty<Border>(Property.BORDER_TOP) != null || renderer.GetProperty
                <Border>(Property.BORDER_RIGHT) != null || renderer.GetProperty<Border>(Property.BORDER_BOTTOM) != null
                 || renderer.GetProperty<Border>(Property.BORDER_LEFT) != null;
            bool generalBorderProperties = !specificBorderProperties && renderer.GetProperty<Object>(Property.BORDER) 
                != null;
            if (generalBorderProperties) {
                Border generalBorder = renderer.GetProperty<Border>(Property.BORDER);
                Color generalBorderColor = generalBorder.GetColor();
                int borderType = generalBorder.GetBorderType();
                float borderWidth = generalBorder.GetWidth();
                if (generalBorderColor is DeviceRgb) {
                    attributes.Put(PdfName.BorderColor, new PdfArray(generalBorderColor.GetColorValue()));
                    attributes.Put(PdfName.BorderStyle, TransformBorderTypeToName(borderType));
                    attributes.Put(PdfName.BorderThickness, new PdfNumber(borderWidth));
                }
            }
            if (specificBorderProperties) {
                PdfArray borderColors = new PdfArray();
                PdfArray borderTypes = new PdfArray();
                PdfArray borderWidths = new PdfArray();
                bool atLeastOneRgb = false;
                Border[] borders = renderer.GetBorders();
                bool allColorsEqual = true;
                bool allTypesEqual = true;
                bool allWidthsEqual = true;
                for (int i = 1; i < borders.Length; i++) {
                    Border border = borders[i];
                    if (border != null) {
                        if (null == borders[0] || !border.GetColor().Equals(borders[0].GetColor())) {
                            allColorsEqual = false;
                        }
                        if (null == borders[0] || border.GetWidth() != borders[0].GetWidth()) {
                            allWidthsEqual = false;
                        }
                        if (null == borders[0] || border.GetBorderType() != borders[0].GetBorderType()) {
                            allTypesEqual = false;
                        }
                    }
                }
                //TODO DEVSIX-4218 set depending on writing direction
                int[] borderOrder = new int[] { 0, 1, 2, 3 };
                foreach (int i in borderOrder) {
                    if (borders[i] != null) {
                        if (borders[i].GetColor() is DeviceRgb) {
                            borderColors.Add(new PdfArray(borders[i].GetColor().GetColorValue()));
                            atLeastOneRgb = true;
                        }
                        else {
                            borderColors.Add(PdfNull.PDF_NULL);
                        }
                        borderTypes.Add(TransformBorderTypeToName(borders[i].GetBorderType()));
                        borderWidths.Add(new PdfNumber(borders[i].GetWidth()));
                    }
                    else {
                        borderColors.Add(PdfNull.PDF_NULL);
                        borderTypes.Add(PdfName.None);
                        borderWidths.Add(PdfNull.PDF_NULL);
                    }
                }
                if (atLeastOneRgb) {
                    if (allColorsEqual) {
                        attributes.Put(PdfName.BorderColor, borderColors.Get(0));
                    }
                    else {
                        attributes.Put(PdfName.BorderColor, borderColors);
                    }
                }
                if (allTypesEqual) {
                    attributes.Put(PdfName.BorderStyle, borderTypes.Get(0));
                }
                else {
                    attributes.Put(PdfName.BorderStyle, borderTypes);
                }
                if (allWidthsEqual) {
                    attributes.Put(PdfName.BorderThickness, borderWidths.Get(0));
                }
                else {
                    attributes.Put(PdfName.BorderThickness, borderWidths);
                }
            }
        }

        private static IRoleMappingResolver ResolveMappingToStandard(TagTreePointer taggingPointer) {
            TagStructureContext tagContext = taggingPointer.GetDocument().GetTagStructureContext();
            PdfNamespace @namespace = taggingPointer.GetProperties().GetNamespace();
            return tagContext.ResolveMappingToStandardOrDomainSpecificRole(taggingPointer.GetRole(), @namespace);
        }

        private static bool IsTagStructurePdf2(PdfNamespace @namespace) {
            return @namespace != null && StandardNamespaces.PDF_2_0.Equals(@namespace.GetNamespaceName());
        }

        private static PdfName TransformTextAlignmentValueToName(TextAlignment? textAlignment) {
            //TODO DEVSIX-4218 set rightToLeft value according with actual text content if it is possible.
            bool isLeftToRight = true;
            switch (textAlignment) {
                case TextAlignment.LEFT: {
                    if (isLeftToRight) {
                        return PdfName.Start;
                    }
                    else {
                        return PdfName.End;
                    }
                    goto case TextAlignment.CENTER;
                }

                case TextAlignment.CENTER: {
                    return PdfName.Center;
                }

                case TextAlignment.RIGHT: {
                    if (isLeftToRight) {
                        return PdfName.End;
                    }
                    else {
                        return PdfName.Start;
                    }
                    goto case TextAlignment.JUSTIFIED;
                }

                case TextAlignment.JUSTIFIED:
                case TextAlignment.JUSTIFIED_ALL: {
                    return PdfName.Justify;
                }

                default: {
                    return PdfName.Start;
                }
            }
        }

        private static PdfName TransformBlockAlignToName(HorizontalAlignment? horizontalAlignment) {
            //TODO DEVSIX-4218 set rightToLeft value according with actual text content if it is possible.
            bool isLeftToRight = true;
            switch (horizontalAlignment) {
                case HorizontalAlignment.LEFT: {
                    if (isLeftToRight) {
                        return PdfName.Before;
                    }
                    else {
                        return PdfName.After;
                    }
                    goto case HorizontalAlignment.CENTER;
                }

                case HorizontalAlignment.CENTER: {
                    return PdfName.Middle;
                }

                case HorizontalAlignment.RIGHT: {
                    if (isLeftToRight) {
                        return PdfName.After;
                    }
                    else {
                        return PdfName.Before;
                    }
                    goto default;
                }

                default: {
                    return PdfName.Before;
                }
            }
        }

        private static PdfName TransformBorderTypeToName(int borderType) {
            switch (borderType) {
                case Border.SOLID: {
                    return PdfName.Solid;
                }

                case Border.DASHED: {
                    return PdfName.Dashed;
                }

                case Border.DOTTED: {
                    return PdfName.Dotted;
                }

                case Border.ROUND_DOTS: {
                    return PdfName.Dotted;
                }

                case Border.DOUBLE: {
                    return PdfName.Double;
                }

                case Border._3D_GROOVE: {
                    return PdfName.Groove;
                }

                case Border._3D_INSET: {
                    return PdfName.Inset;
                }

                case Border._3D_OUTSET: {
                    return PdfName.Outset;
                }

                case Border._3D_RIDGE: {
                    return PdfName.Ridge;
                }

                default: {
                    return PdfName.Solid;
                }
            }
        }

        private static PdfName TransformNumberingTypeToName(ListNumberingType numberingType, bool isTagStructurePdf2
            ) {
            switch (numberingType) {
                case ListNumberingType.DECIMAL:
                case ListNumberingType.DECIMAL_LEADING_ZERO: {
                    return PdfName.Decimal;
                }

                case ListNumberingType.ROMAN_UPPER: {
                    return PdfName.UpperRoman;
                }

                case ListNumberingType.ROMAN_LOWER: {
                    return PdfName.LowerRoman;
                }

                case ListNumberingType.ENGLISH_UPPER:
                case ListNumberingType.GREEK_UPPER: {
                    return PdfName.UpperAlpha;
                }

                case ListNumberingType.ENGLISH_LOWER:
                case ListNumberingType.GREEK_LOWER: {
                    return PdfName.LowerAlpha;
                }

                default: {
                    if (isTagStructurePdf2) {
                        return PdfName.Ordered;
                    }
                    else {
                        return PdfName.None;
                    }
                    break;
                }
            }
        }
    }
}
