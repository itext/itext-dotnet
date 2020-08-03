using System;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Validate.Impl.Datatype {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Validate.ICssDataTypeValidator"/>
    /// implementation for numeric elements.
    /// </summary>
    public class CssNumericValueValidator : ICssDataTypeValidator {
        private readonly bool allowedPercent;

        private readonly bool allowedNormal;

        /// <summary>
        /// Creates a new
        /// <see cref="CssNumericValueValidator"/>
        /// instance.
        /// </summary>
        /// <param name="allowedPercent">is percent value allowed</param>
        /// <param name="allowedNormal">is 'normal' value allowed</param>
        public CssNumericValueValidator(bool allowedPercent, bool allowedNormal) {
            this.allowedPercent = allowedPercent;
            this.allowedNormal = allowedNormal;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool IsValid(String objectString) {
            if (objectString == null) {
                return false;
            }
            if (CommonCssConstants.INITIAL.Equals(objectString) || CommonCssConstants.INHERIT.Equals(objectString) || 
                CommonCssConstants.UNSET.Equals(objectString)) {
                return true;
            }
            if (CommonCssConstants.NORMAL.Equals(objectString)) {
                return this.allowedNormal;
            }
            if (!CssUtils.IsValidNumericValue(objectString)) {
                return false;
            }
            if (CssUtils.IsPercentageValue(objectString)) {
                return this.allowedPercent;
            }
            return true;
        }
    }
}
