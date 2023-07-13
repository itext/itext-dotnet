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
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Validate;
using iText.StyledXmlParser.Css.Validate.Impl.Datatype;
using iText.StyledXmlParser.Css.Validate.Impl.Declaration;

namespace iText.StyledXmlParser.Css.Validate.Impl {
    /// <summary>Class that bundles all the CSS declaration validators.</summary>
    /// <remarks>
    /// Class that bundles all the CSS declaration validators.
    /// It validates CSS declarations against the accepted html/css standard.
    /// </remarks>
    public class CssDefaultValidator : ICssDeclarationValidator {
        /// <summary>A map containing all the CSS declaration validators.</summary>
        protected internal readonly IDictionary<String, ICssDeclarationValidator> defaultValidators;

        public CssDefaultValidator() {
            ICssDeclarationValidator colorCommonValidator = new MultiTypeDeclarationValidator(new CssEnumValidator(CommonCssConstants
                .TRANSPARENT, CommonCssConstants.INITIAL, CommonCssConstants.INHERIT, CommonCssConstants.CURRENTCOLOR)
                , new CssColorValidator());
            CssEnumValidator normalValidator = new CssEnumValidator(CommonCssConstants.NORMAL);
            CssEnumValidator relativeSizeValidator = new CssEnumValidator(CommonCssConstants.LARGER, CommonCssConstants
                .SMALLER);
            CssEnumValidator absoluteSizeValidator = new CssEnumValidator();
            absoluteSizeValidator.AddAllowedValues(CommonCssConstants.FONT_ABSOLUTE_SIZE_KEYWORDS_VALUES.Keys);
            CssEnumValidator inheritInitialUnsetValidator = new CssEnumValidator(CommonCssConstants.INHERIT, CommonCssConstants
                .INITIAL, CommonCssConstants.UNSET);
            defaultValidators = new Dictionary<String, ICssDeclarationValidator>();
            defaultValidators.Put(CommonCssConstants.BACKGROUND_COLOR, colorCommonValidator);
            defaultValidators.Put(CommonCssConstants.COLOR, colorCommonValidator);
            defaultValidators.Put(CommonCssConstants.BORDER_COLOR, colorCommonValidator);
            defaultValidators.Put(CommonCssConstants.BORDER_BOTTOM_COLOR, colorCommonValidator);
            defaultValidators.Put(CommonCssConstants.BORDER_TOP_COLOR, colorCommonValidator);
            defaultValidators.Put(CommonCssConstants.BORDER_LEFT_COLOR, colorCommonValidator);
            defaultValidators.Put(CommonCssConstants.BORDER_RIGHT_COLOR, colorCommonValidator);
            defaultValidators.Put(CommonCssConstants.FLOAT, new SingleTypeDeclarationValidator(new CssEnumValidator(CommonCssConstants
                .LEFT, CommonCssConstants.RIGHT, CommonCssConstants.NONE, CommonCssConstants.INHERIT, CommonCssConstants
                .CENTER)));
            /*center comes from legacy*/
            defaultValidators.Put(CommonCssConstants.PAGE_BREAK_BEFORE, new SingleTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.AUTO, CommonCssConstants.ALWAYS, CommonCssConstants.AVOID, CommonCssConstants.LEFT
                , CommonCssConstants.RIGHT)));
            defaultValidators.Put(CommonCssConstants.PAGE_BREAK_AFTER, new SingleTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.AUTO, CommonCssConstants.ALWAYS, CommonCssConstants.AVOID, CommonCssConstants.LEFT
                , CommonCssConstants.RIGHT)));
            defaultValidators.Put(CommonCssConstants.QUOTES, new MultiTypeDeclarationValidator(new CssEnumValidator(CommonCssConstants
                .INITIAL, CommonCssConstants.INHERIT, CommonCssConstants.NONE), new CssQuotesValidator()));
            defaultValidators.Put(CommonCssConstants.TRANSFORM, new SingleTypeDeclarationValidator(new CssTransformValidator
                ()));
            defaultValidators.Put(CommonCssConstants.FONT_SIZE, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (false), new CssPercentageValueValidator(false), relativeSizeValidator, absoluteSizeValidator));
            defaultValidators.Put(CommonCssConstants.WORD_SPACING, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (true), normalValidator));
            defaultValidators.Put(CommonCssConstants.LETTER_SPACING, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (true), normalValidator));
            defaultValidators.Put(CommonCssConstants.TEXT_INDENT, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (true), new CssPercentageValueValidator(true), new CssEnumValidator(CommonCssConstants.EACH_LINE, CommonCssConstants
                .HANGING, CommonCssConstants.HANGING + " " + CommonCssConstants.EACH_LINE)));
            defaultValidators.Put(CommonCssConstants.LINE_HEIGHT, new MultiTypeDeclarationValidator(new CssNumberValueValidator
                (false), new CssLengthValueValidator(false), new CssPercentageValueValidator(false), normalValidator, 
                inheritInitialUnsetValidator));
            defaultValidators.Put(CommonCssConstants.COLUMN_GAP, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (false), new CssPercentageValueValidator(false), normalValidator, inheritInitialUnsetValidator));
            defaultValidators.Put(CommonCssConstants.COLUMN_WIDTH, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (false), new CssPercentageValueValidator(false), new CssEnumValidator(CommonCssConstants.AUTO), inheritInitialUnsetValidator
                ));
            defaultValidators.Put(CommonCssConstants.COLUMN_COUNT, new MultiTypeDeclarationValidator(new CssNumberValueValidator
                (false), new CssEnumValidator(CommonCssConstants.AUTO), inheritInitialUnsetValidator));
            defaultValidators.Put(CommonCssConstants.ROW_GAP, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (false), new CssPercentageValueValidator(false), normalValidator, inheritInitialUnsetValidator));
            defaultValidators.Put(CommonCssConstants.FLEX_GROW, new MultiTypeDeclarationValidator(new CssNumberValueValidator
                (false), inheritInitialUnsetValidator));
            defaultValidators.Put(CommonCssConstants.FLEX_SHRINK, new MultiTypeDeclarationValidator(new CssNumberValueValidator
                (false), inheritInitialUnsetValidator));
            CssEnumValidator flexBasisEnumValidator = new CssEnumValidator(CommonCssConstants.AUTO, CommonCssConstants
                .CONTENT, CommonCssConstants.MIN_CONTENT, CommonCssConstants.MAX_CONTENT, CommonCssConstants.FIT_CONTENT
                );
            defaultValidators.Put(CommonCssConstants.FLEX_BASIS, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (false), new CssPercentageValueValidator(false), flexBasisEnumValidator));
            defaultValidators.Put(CommonCssConstants.BACKGROUND_REPEAT, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_REPEAT)));
            defaultValidators.Put(CommonCssConstants.BACKGROUND_IMAGE, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_IMAGE)));
            defaultValidators.Put(CommonCssConstants.BACKGROUND_POSITION_X, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_POSITION_X)));
            defaultValidators.Put(CommonCssConstants.BACKGROUND_POSITION_Y, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_POSITION_Y)));
            defaultValidators.Put(CommonCssConstants.BACKGROUND_SIZE, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_SIZE)));
            defaultValidators.Put(CommonCssConstants.BACKGROUND_CLIP, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_CLIP)));
            defaultValidators.Put(CommonCssConstants.BACKGROUND_ORIGIN, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_ORIGIN)));
            defaultValidators.Put(CommonCssConstants.BACKGROUND_BLEND_MODE, new SingleTypeDeclarationValidator(new ArrayDataTypeValidator
                (new CssBlendModeValidator())));
            defaultValidators.Put(CommonCssConstants.OVERFLOW_WRAP, new MultiTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.ANYWHERE, CommonCssConstants.BREAK_WORD), normalValidator, inheritInitialUnsetValidator
                ));
            defaultValidators.Put(CommonCssConstants.WORD_BREAK, new MultiTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.BREAK_ALL, CommonCssConstants.KEEP_ALL, CommonCssConstants.BREAK_WORD), normalValidator
                , inheritInitialUnsetValidator));
            defaultValidators.Put(CommonCssConstants.FLEX_DIRECTION, new MultiTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.ROW, CommonCssConstants.ROW_REVERSE, CommonCssConstants.COLUMN, CommonCssConstants
                .COLUMN_REVERSE), inheritInitialUnsetValidator));
            defaultValidators.Put(CommonCssConstants.FLEX_WRAP, new MultiTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.NOWRAP, CommonCssConstants.WRAP, CommonCssConstants.WRAP_REVERSE), inheritInitialUnsetValidator
                ));
            defaultValidators.Put(CommonCssConstants.ALIGN_ITEMS, new MultiTypeDeclarationValidator(normalValidator, new 
                CssEnumValidator(JavaUtil.ArraysAsList(CommonCssConstants.BASELINE), JavaUtil.ArraysAsList(CommonCssConstants
                .FIRST, CommonCssConstants.LAST)), new CssEnumValidator(JavaUtil.ArraysAsList(CommonCssConstants.STRETCH
                , CommonCssConstants.CENTER, CommonCssConstants.START, CommonCssConstants.END, CommonCssConstants.FLEX_START
                , CommonCssConstants.FLEX_END, CommonCssConstants.SELF_START, CommonCssConstants.SELF_END), JavaUtil.ArraysAsList
                (CommonCssConstants.SAFE, CommonCssConstants.UNSAFE)), inheritInitialUnsetValidator));
            defaultValidators.Put(CommonCssConstants.JUSTIFY_CONTENT, new MultiTypeDeclarationValidator(new CssEnumValidator
                (JavaUtil.ArraysAsList(CommonCssConstants.SPACE_AROUND, CommonCssConstants.SPACE_BETWEEN, CommonCssConstants
                .SPACE_EVENLY, CommonCssConstants.STRETCH, CommonCssConstants.NORMAL, CommonCssConstants.LEFT, CommonCssConstants
                .RIGHT)), new CssEnumValidator(JavaUtil.ArraysAsList(CommonCssConstants.CENTER, CommonCssConstants.START
                , CommonCssConstants.FLEX_START, CommonCssConstants.SELF_START, CommonCssConstants.END, CommonCssConstants
                .FLEX_END, CommonCssConstants.SELF_END), JavaUtil.ArraysAsList(CommonCssConstants.SAFE, CommonCssConstants
                .UNSAFE)), inheritInitialUnsetValidator));
            defaultValidators.Put(CommonCssConstants.JUSTIFY_ITEMS, new MultiTypeDeclarationValidator(normalValidator, 
                new CssEnumValidator(JavaUtil.ArraysAsList(CommonCssConstants.BASELINE), JavaUtil.ArraysAsList(CommonCssConstants
                .FIRST, CommonCssConstants.LAST)), new CssEnumValidator(JavaUtil.ArraysAsList(CommonCssConstants.STRETCH
                , CommonCssConstants.CENTER, CommonCssConstants.START, CommonCssConstants.END, CommonCssConstants.FLEX_START
                , CommonCssConstants.FLEX_END, CommonCssConstants.SELF_START, CommonCssConstants.SELF_END, CommonCssConstants
                .LEFT, CommonCssConstants.RIGHT), JavaUtil.ArraysAsList(CommonCssConstants.SAFE, CommonCssConstants.UNSAFE
                )), new CssEnumValidator(CommonCssConstants.LEGACY, CommonCssConstants.LEGACY + " " + CommonCssConstants
                .LEFT, CommonCssConstants.LEGACY + " " + CommonCssConstants.RIGHT, CommonCssConstants.LEGACY + " " + CommonCssConstants
                .CENTER), inheritInitialUnsetValidator));
        }

        /// <summary>Validates a CSS declaration.</summary>
        /// <param name="declaration">the CSS declaration</param>
        /// <returns>true, if the validation was successful</returns>
        public virtual bool IsValid(CssDeclaration declaration) {
            ICssDeclarationValidator validator = defaultValidators.Get(declaration.GetProperty());
            return validator == null || validator.IsValid(declaration);
        }
    }
}
