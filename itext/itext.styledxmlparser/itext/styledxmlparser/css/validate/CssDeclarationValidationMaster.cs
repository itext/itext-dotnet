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
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Validate.Impl.Datatype;
using iText.StyledXmlParser.Css.Validate.Impl.Declaration;

namespace iText.StyledXmlParser.Css.Validate {
    /// <summary>Class that bundles all the CSS declaration validators.</summary>
    public class CssDeclarationValidationMaster {
        /// <summary>A map containing all the CSS declaration validators.</summary>
        private static readonly IDictionary<String, ICssDeclarationValidator> DEFAULT_VALIDATORS;

        static CssDeclarationValidationMaster() {
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
            DEFAULT_VALIDATORS = new Dictionary<String, ICssDeclarationValidator>();
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BACKGROUND_COLOR, colorCommonValidator);
            DEFAULT_VALIDATORS.Put(CommonCssConstants.COLOR, colorCommonValidator);
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BORDER_COLOR, colorCommonValidator);
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BORDER_BOTTOM_COLOR, colorCommonValidator);
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BORDER_TOP_COLOR, colorCommonValidator);
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BORDER_LEFT_COLOR, colorCommonValidator);
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BORDER_RIGHT_COLOR, colorCommonValidator);
            DEFAULT_VALIDATORS.Put(CommonCssConstants.FLOAT, new SingleTypeDeclarationValidator(new CssEnumValidator(CommonCssConstants
                .LEFT, CommonCssConstants.RIGHT, CommonCssConstants.NONE, CommonCssConstants.INHERIT, CommonCssConstants
                .CENTER)));
            /*center comes from legacy*/
            DEFAULT_VALIDATORS.Put(CommonCssConstants.PAGE_BREAK_BEFORE, new SingleTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.AUTO, CommonCssConstants.ALWAYS, CommonCssConstants.AVOID, CommonCssConstants.LEFT
                , CommonCssConstants.RIGHT)));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.PAGE_BREAK_AFTER, new SingleTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.AUTO, CommonCssConstants.ALWAYS, CommonCssConstants.AVOID, CommonCssConstants.LEFT
                , CommonCssConstants.RIGHT)));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.QUOTES, new MultiTypeDeclarationValidator(new CssEnumValidator(CommonCssConstants
                .INITIAL, CommonCssConstants.INHERIT, CommonCssConstants.NONE), new CssQuotesValidator()));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.TRANSFORM, new SingleTypeDeclarationValidator(new CssTransformValidator
                ()));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.FONT_SIZE, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (false), new CssPercentageValueValidator(false), relativeSizeValidator, absoluteSizeValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.WORD_SPACING, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (true), normalValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.LETTER_SPACING, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (true), normalValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.TEXT_INDENT, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (true), new CssPercentageValueValidator(true), new CssEnumValidator(CommonCssConstants.EACH_LINE, CommonCssConstants
                .HANGING, CommonCssConstants.HANGING + " " + CommonCssConstants.EACH_LINE)));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.LINE_HEIGHT, new MultiTypeDeclarationValidator(new CssNumberValueValidator
                (false), new CssLengthValueValidator(false), new CssPercentageValueValidator(false), normalValidator, 
                inheritInitialUnsetValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.COLUMN_GAP, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (false), new CssPercentageValueValidator(false), normalValidator, inheritInitialUnsetValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.ROW_GAP, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (false), new CssPercentageValueValidator(false), normalValidator, inheritInitialUnsetValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.FLEX_GROW, new MultiTypeDeclarationValidator(new CssNumberValueValidator
                (false), inheritInitialUnsetValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.FLEX_SHRINK, new MultiTypeDeclarationValidator(new CssNumberValueValidator
                (false), inheritInitialUnsetValidator));
            CssEnumValidator flexBasisEnumValidator = new CssEnumValidator(CommonCssConstants.AUTO, CommonCssConstants
                .CONTENT, CommonCssConstants.MIN_CONTENT, CommonCssConstants.MAX_CONTENT, CommonCssConstants.FIT_CONTENT
                );
            DEFAULT_VALIDATORS.Put(CommonCssConstants.FLEX_BASIS, new MultiTypeDeclarationValidator(new CssLengthValueValidator
                (false), new CssPercentageValueValidator(false), flexBasisEnumValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BACKGROUND_REPEAT, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_REPEAT)));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BACKGROUND_IMAGE, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_IMAGE)));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BACKGROUND_POSITION_X, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_POSITION_X)));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BACKGROUND_POSITION_Y, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_POSITION_Y)));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BACKGROUND_SIZE, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_SIZE)));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BACKGROUND_CLIP, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_CLIP)));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BACKGROUND_ORIGIN, new SingleTypeDeclarationValidator(new CssBackgroundValidator
                (CommonCssConstants.BACKGROUND_ORIGIN)));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.BACKGROUND_BLEND_MODE, new SingleTypeDeclarationValidator(new ArrayDataTypeValidator
                (new CssBlendModeValidator())));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.OVERFLOW_WRAP, new MultiTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.ANYWHERE, CommonCssConstants.BREAK_WORD), normalValidator, inheritInitialUnsetValidator
                ));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.WORD_BREAK, new MultiTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.BREAK_ALL, CommonCssConstants.KEEP_ALL, CommonCssConstants.BREAK_WORD), normalValidator
                , inheritInitialUnsetValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.FLEX_DIRECTION, new MultiTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.ROW, CommonCssConstants.ROW_REVERSE, CommonCssConstants.COLUMN, CommonCssConstants
                .COLUMN_REVERSE), inheritInitialUnsetValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.FLEX_WRAP, new MultiTypeDeclarationValidator(new CssEnumValidator
                (CommonCssConstants.NOWRAP, CommonCssConstants.WRAP, CommonCssConstants.WRAP_REVERSE), inheritInitialUnsetValidator
                ));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.ALIGN_ITEMS, new MultiTypeDeclarationValidator(normalValidator, 
                new CssEnumValidator(JavaUtil.ArraysAsList(CommonCssConstants.BASELINE), JavaUtil.ArraysAsList(CommonCssConstants
                .FIRST, CommonCssConstants.LAST)), new CssEnumValidator(JavaUtil.ArraysAsList(CommonCssConstants.STRETCH
                , CommonCssConstants.CENTER, CommonCssConstants.START, CommonCssConstants.END, CommonCssConstants.FLEX_START
                , CommonCssConstants.FLEX_END, CommonCssConstants.SELF_START, CommonCssConstants.SELF_END), JavaUtil.ArraysAsList
                (CommonCssConstants.SAFE, CommonCssConstants.UNSAFE)), inheritInitialUnsetValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.JUSTIFY_CONTENT, new MultiTypeDeclarationValidator(new CssEnumValidator
                (JavaUtil.ArraysAsList(CommonCssConstants.SPACE_AROUND, CommonCssConstants.SPACE_BETWEEN, CommonCssConstants
                .SPACE_EVENLY, CommonCssConstants.STRETCH, CommonCssConstants.NORMAL, CommonCssConstants.LEFT, CommonCssConstants
                .RIGHT)), new CssEnumValidator(JavaUtil.ArraysAsList(CommonCssConstants.CENTER, CommonCssConstants.START
                , CommonCssConstants.FLEX_START, CommonCssConstants.SELF_START, CommonCssConstants.END, CommonCssConstants
                .FLEX_END, CommonCssConstants.SELF_END), JavaUtil.ArraysAsList(CommonCssConstants.SAFE, CommonCssConstants
                .UNSAFE)), inheritInitialUnsetValidator));
            DEFAULT_VALIDATORS.Put(CommonCssConstants.JUSTIFY_ITEMS, new MultiTypeDeclarationValidator(normalValidator
                , new CssEnumValidator(JavaUtil.ArraysAsList(CommonCssConstants.BASELINE), JavaUtil.ArraysAsList(CommonCssConstants
                .FIRST, CommonCssConstants.LAST)), new CssEnumValidator(JavaUtil.ArraysAsList(CommonCssConstants.STRETCH
                , CommonCssConstants.CENTER, CommonCssConstants.START, CommonCssConstants.END, CommonCssConstants.FLEX_START
                , CommonCssConstants.FLEX_END, CommonCssConstants.SELF_START, CommonCssConstants.SELF_END, CommonCssConstants
                .LEFT, CommonCssConstants.RIGHT), JavaUtil.ArraysAsList(CommonCssConstants.SAFE, CommonCssConstants.UNSAFE
                )), new CssEnumValidator(CommonCssConstants.LEGACY, CommonCssConstants.LEGACY + " " + CommonCssConstants
                .LEFT, CommonCssConstants.LEGACY + " " + CommonCssConstants.RIGHT, CommonCssConstants.LEGACY + " " + CommonCssConstants
                .CENTER), inheritInitialUnsetValidator));
        }

        /// <summary>
        /// Creates a new
        /// <c>CssDeclarationValidationMaster</c>
        /// instance.
        /// </summary>
        private CssDeclarationValidationMaster() {
        }

        /// <summary>Checks a CSS declaration.</summary>
        /// <param name="declaration">the CSS declaration</param>
        /// <returns>true, if the validation was successful</returns>
        public static bool CheckDeclaration(CssDeclaration declaration) {
            ICssDeclarationValidator validator = DEFAULT_VALIDATORS.Get(declaration.GetProperty());
            return validator == null || validator.IsValid(declaration);
        }
    }
}
