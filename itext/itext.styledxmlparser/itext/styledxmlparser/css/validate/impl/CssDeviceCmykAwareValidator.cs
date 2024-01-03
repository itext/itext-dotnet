/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Validate;
using iText.StyledXmlParser.Css.Validate.Impl.Datatype;
using iText.StyledXmlParser.Css.Validate.Impl.Declaration;

namespace iText.StyledXmlParser.Css.Validate.Impl {
    /// <summary>Class that bundles all the CSS declaration validators.</summary>
    /// <remarks>
    /// Class that bundles all the CSS declaration validators.
    /// It extends the default
    /// <see cref="CssDefaultValidator"/>
    /// to also support device-cmyk color structure.
    /// </remarks>
    public class CssDeviceCmykAwareValidator : CssDefaultValidator {
        public CssDeviceCmykAwareValidator()
            : base() {
            ICssDeclarationValidator colorCmykValidator = new MultiTypeDeclarationValidator(new CssEnumValidator(CommonCssConstants
                .TRANSPARENT, CommonCssConstants.INITIAL, CommonCssConstants.INHERIT, CommonCssConstants.CURRENTCOLOR)
                , new CssCmykAwareColorValidator());
            defaultValidators.Put(CommonCssConstants.BACKGROUND_COLOR, colorCmykValidator);
            defaultValidators.Put(CommonCssConstants.COLOR, colorCmykValidator);
            defaultValidators.Put(CommonCssConstants.BORDER_COLOR, colorCmykValidator);
            defaultValidators.Put(CommonCssConstants.BORDER_BOTTOM_COLOR, colorCmykValidator);
            defaultValidators.Put(CommonCssConstants.BORDER_TOP_COLOR, colorCmykValidator);
            defaultValidators.Put(CommonCssConstants.BORDER_LEFT_COLOR, colorCmykValidator);
            defaultValidators.Put(CommonCssConstants.BORDER_RIGHT_COLOR, colorCmykValidator);
        }
    }
}
