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
    public class FlexFlowShorthandResolver : IShorthandResolver {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(FlexFlowShorthandResolver));

        private const String DEFAULT_FLEX_DIRECTION = CommonCssConstants.ROW;

        private const String DEFAULT_FLEX_WRAP = CommonCssConstants.NOWRAP;

        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.FLEX_DIRECTION, shorthandExpression), new 
                    CssDeclaration(CommonCssConstants.FLEX_WRAP, shorthandExpression));
            }
            if (CssTypesValidationUtils.ContainsInitialOrInheritOrUnset(shorthandExpression)) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .FLEX_FLOW, shorthandExpression));
                return JavaCollectionsUtil.EmptyList();
            }
            if (String.IsNullOrEmpty(shorthandExpression)) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.FLEX_FLOW));
                return JavaCollectionsUtil.EmptyList();
            }
            String[] flexFlowProps = iText.IO.Util.StringUtil.Split(shorthandExpression, " ");
            bool isDirectionResolved = false;
            bool isWrapResolved = false;
            IList<CssDeclaration> resolvedProperties = new List<CssDeclaration>();
            foreach (String flexFlowProp in flexFlowProps) {
                CssDeclaration flexDirectionDeclaration = new CssDeclaration(CommonCssConstants.FLEX_DIRECTION, flexFlowProp
                    );
                CssDeclaration flexWrapDeclaration = new CssDeclaration(CommonCssConstants.FLEX_WRAP, flexFlowProp);
                if (CssDeclarationValidationMaster.CheckDeclaration(flexDirectionDeclaration)) {
                    if (isDirectionResolved) {
                        LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                            .FLEX_FLOW, shorthandExpression));
                        return JavaCollectionsUtil.EmptyList();
                    }
                    isDirectionResolved = true;
                    resolvedProperties.Add(flexDirectionDeclaration);
                }
                else {
                    if (CssDeclarationValidationMaster.CheckDeclaration(flexWrapDeclaration)) {
                        if (isWrapResolved) {
                            LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                                .FLEX_FLOW, shorthandExpression));
                            return JavaCollectionsUtil.EmptyList();
                        }
                        isWrapResolved = true;
                        resolvedProperties.Add(flexWrapDeclaration);
                    }
                    else {
                        LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                            .FLEX_FLOW, shorthandExpression));
                        return JavaCollectionsUtil.EmptyList();
                    }
                }
            }
            FillUnresolvedPropertiesWithDefaultValues(resolvedProperties);
            return resolvedProperties;
        }

        private void FillUnresolvedPropertiesWithDefaultValues(IList<CssDeclaration> resolvedProperties) {
            if (!resolvedProperties.Any((property) => property.GetProperty().Equals(CommonCssConstants.FLEX_DIRECTION)
                )) {
                resolvedProperties.Add(new CssDeclaration(CommonCssConstants.FLEX_DIRECTION, DEFAULT_FLEX_DIRECTION));
            }
            if (!resolvedProperties.Any((property) => property.GetProperty().Equals(CommonCssConstants.FLEX_WRAP))) {
                resolvedProperties.Add(new CssDeclaration(CommonCssConstants.FLEX_WRAP, DEFAULT_FLEX_WRAP));
            }
        }
    }
}
