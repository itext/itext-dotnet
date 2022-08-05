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
