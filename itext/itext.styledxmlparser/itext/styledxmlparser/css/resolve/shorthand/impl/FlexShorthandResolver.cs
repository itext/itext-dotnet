using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    public class FlexShorthandResolver : IShorthandResolver {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(FlexShorthandResolver));

        private const String DEFAULT_FLEX_GROW = "0";

        private const String DEFAULT_FLEX_SHRINK = "1";

        private const String DEFAULT_FLEX_BASIS = CommonCssConstants.AUTO;

        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.FLEX_GROW, shorthandExpression), new CssDeclaration
                    (CommonCssConstants.FLEX_SHRINK, shorthandExpression), new CssDeclaration(CommonCssConstants.FLEX_BASIS
                    , shorthandExpression));
            }
            if (CommonCssConstants.AUTO.Equals(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.FLEX_GROW, "1"), new CssDeclaration(CommonCssConstants
                    .FLEX_SHRINK, "1"), new CssDeclaration(CommonCssConstants.FLEX_BASIS, CommonCssConstants.AUTO));
            }
            if (CommonCssConstants.NONE.Equals(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.FLEX_GROW, "0"), new CssDeclaration(CommonCssConstants
                    .FLEX_SHRINK, "0"), new CssDeclaration(CommonCssConstants.FLEX_BASIS, CommonCssConstants.AUTO));
            }
            if (CssTypesValidationUtils.ContainsInitialOrInheritOrUnset(shorthandExpression)) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .FLEX, shorthandExpression));
                return JavaCollectionsUtil.EmptyList();
            }
            if (String.IsNullOrEmpty(shorthandExpression)) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.FLEX));
                return JavaCollectionsUtil.EmptyList();
            }
            String[] flexProps = iText.IO.Util.StringUtil.Split(shorthandExpression, " ");
            IList<CssDeclaration> resolvedProperties;
            switch (flexProps.Length) {
                case 1: {
                    resolvedProperties = ResolveShorthandWithOneValue(flexProps[0]);
                    break;
                }

                case 2: {
                    resolvedProperties = ResolveShorthandWithTwoValues(flexProps[0], flexProps[1]);
                    break;
                }

                case 3: {
                    resolvedProperties = ResolveShorthandWithThreeValues(flexProps[0], flexProps[1], flexProps[2]);
                    break;
                }

                default: {
                    LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                        .FLEX, shorthandExpression));
                    return JavaCollectionsUtil.EmptyList();
                }
            }
            if (!resolvedProperties.IsEmpty()) {
                FillUnresolvedPropertiesWithDefaultValues(resolvedProperties);
            }
            return resolvedProperties;
        }

        private IList<CssDeclaration> ResolveShorthandWithOneValue(String firstProperty) {
            IList<CssDeclaration> resolvedProperties = new List<CssDeclaration>();
            CssDeclaration flexGrowDeclaration = new CssDeclaration(CommonCssConstants.FLEX_GROW, firstProperty);
            if (CssDeclarationValidationMaster.CheckDeclaration(flexGrowDeclaration)) {
                resolvedProperties.Add(flexGrowDeclaration);
                return resolvedProperties;
            }
            CssDeclaration flexBasisDeclaration = new CssDeclaration(CommonCssConstants.FLEX_BASIS, firstProperty);
            if (CssDeclarationValidationMaster.CheckDeclaration(flexBasisDeclaration)) {
                resolvedProperties.Add(flexBasisDeclaration);
                return resolvedProperties;
            }
            LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                , firstProperty));
            return JavaCollectionsUtil.EmptyList();
        }

        private IList<CssDeclaration> ResolveShorthandWithTwoValues(String firstProperty, String secondProperty) {
            IList<CssDeclaration> resolvedProperties = new List<CssDeclaration>();
            CssDeclaration flexGrowDeclaration = new CssDeclaration(CommonCssConstants.FLEX_GROW, firstProperty);
            if (CssDeclarationValidationMaster.CheckDeclaration(flexGrowDeclaration)) {
                resolvedProperties.Add(flexGrowDeclaration);
                CssDeclaration flexShrinkDeclaration = new CssDeclaration(CommonCssConstants.FLEX_SHRINK, secondProperty);
                if (CssDeclarationValidationMaster.CheckDeclaration(flexShrinkDeclaration)) {
                    resolvedProperties.Add(flexShrinkDeclaration);
                    return resolvedProperties;
                }
                CssDeclaration flexBasisDeclaration = new CssDeclaration(CommonCssConstants.FLEX_BASIS, secondProperty);
                if (CssDeclarationValidationMaster.CheckDeclaration(flexBasisDeclaration)) {
                    resolvedProperties.Add(flexBasisDeclaration);
                    return resolvedProperties;
                }
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , secondProperty));
                return JavaCollectionsUtil.EmptyList();
            }
            CssDeclaration flexBasisDeclaration_1 = new CssDeclaration(CommonCssConstants.FLEX_BASIS, firstProperty);
            if (CssDeclarationValidationMaster.CheckDeclaration(flexBasisDeclaration_1)) {
                resolvedProperties.Add(flexBasisDeclaration_1);
                flexGrowDeclaration.SetExpression(secondProperty);
                if (CssDeclarationValidationMaster.CheckDeclaration(flexGrowDeclaration)) {
                    resolvedProperties.Add(flexGrowDeclaration);
                    return resolvedProperties;
                }
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .FLEX_GROW, secondProperty));
                return JavaCollectionsUtil.EmptyList();
            }
            LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                , firstProperty));
            return JavaCollectionsUtil.EmptyList();
        }

        private IList<CssDeclaration> ResolveShorthandWithThreeValues(String firstProperty, String secondProperty, 
            String thirdProperty) {
            IList<CssDeclaration> resolvedProperties = new List<CssDeclaration>();
            CssDeclaration flexGrowDeclaration = new CssDeclaration(CommonCssConstants.FLEX_GROW, firstProperty);
            if (CssDeclarationValidationMaster.CheckDeclaration(flexGrowDeclaration)) {
                resolvedProperties.Add(flexGrowDeclaration);
                CssDeclaration flexShrinkDeclaration = new CssDeclaration(CommonCssConstants.FLEX_SHRINK, secondProperty);
                if (!CssDeclarationValidationMaster.CheckDeclaration(flexShrinkDeclaration)) {
                    LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                        .FLEX_SHRINK, secondProperty));
                    return JavaCollectionsUtil.EmptyList();
                }
                resolvedProperties.Add(flexShrinkDeclaration);
                CssDeclaration flexBasisDeclaration = new CssDeclaration(CommonCssConstants.FLEX_BASIS, thirdProperty);
                if (!CssDeclarationValidationMaster.CheckDeclaration(flexBasisDeclaration)) {
                    LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                        .FLEX_BASIS, thirdProperty));
                    return JavaCollectionsUtil.EmptyList();
                }
                resolvedProperties.Add(flexBasisDeclaration);
                return resolvedProperties;
            }
            CssDeclaration flexBasisDeclaration_1 = new CssDeclaration(CommonCssConstants.FLEX_BASIS, firstProperty);
            if (CssDeclarationValidationMaster.CheckDeclaration(flexBasisDeclaration_1)) {
                resolvedProperties.Add(flexBasisDeclaration_1);
                flexGrowDeclaration.SetExpression(secondProperty);
                if (!CssDeclarationValidationMaster.CheckDeclaration(flexGrowDeclaration)) {
                    LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                        .FLEX_GROW, secondProperty));
                    return JavaCollectionsUtil.EmptyList();
                }
                resolvedProperties.Add(flexGrowDeclaration);
                CssDeclaration flexShrinkDeclaration = new CssDeclaration(CommonCssConstants.FLEX_SHRINK, thirdProperty);
                if (!CssDeclarationValidationMaster.CheckDeclaration(flexShrinkDeclaration)) {
                    LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                        .FLEX_SHRINK, thirdProperty));
                    return JavaCollectionsUtil.EmptyList();
                }
                resolvedProperties.Add(flexShrinkDeclaration);
                return resolvedProperties;
            }
            LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                , firstProperty));
            return JavaCollectionsUtil.EmptyList();
        }

        private void FillUnresolvedPropertiesWithDefaultValues(IList<CssDeclaration> resolvedProperties) {
            if (!resolvedProperties.Any((property) => property.GetProperty().Equals(CommonCssConstants.FLEX_GROW))) {
                resolvedProperties.Add(new CssDeclaration(CommonCssConstants.FLEX_GROW, DEFAULT_FLEX_GROW));
            }
            if (!resolvedProperties.Any((property) => property.GetProperty().Equals(CommonCssConstants.FLEX_SHRINK))) {
                resolvedProperties.Add(new CssDeclaration(CommonCssConstants.FLEX_SHRINK, DEFAULT_FLEX_SHRINK));
            }
            if (!resolvedProperties.Any((property) => property.GetProperty().Equals(CommonCssConstants.FLEX_BASIS))) {
                resolvedProperties.Add(new CssDeclaration(CommonCssConstants.FLEX_BASIS, DEFAULT_FLEX_BASIS));
            }
        }
    }
}
