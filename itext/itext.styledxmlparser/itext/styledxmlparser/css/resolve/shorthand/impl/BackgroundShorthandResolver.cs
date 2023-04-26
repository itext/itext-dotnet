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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for backgrounds.
    /// </summary>
    public class BackgroundShorthandResolver : IShorthandResolver {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(BackgroundShorthandResolver));

        // With CSS3, you can apply multiple backgrounds to elements. These are layered atop one another
        // with the first background you provide on top and the last background listed in the back. Only
        // the last background can include a background color.
        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.resolve.shorthand.IShorthandResolver#resolveShorthand(java.lang.String)
        */
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.BACKGROUND_COLOR, shorthandExpression), 
                    new CssDeclaration(CommonCssConstants.BACKGROUND_IMAGE, shorthandExpression), new CssDeclaration(CommonCssConstants
                    .BACKGROUND_POSITION, shorthandExpression), new CssDeclaration(CommonCssConstants.BACKGROUND_SIZE, shorthandExpression
                    ), new CssDeclaration(CommonCssConstants.BACKGROUND_REPEAT, shorthandExpression), new CssDeclaration(CommonCssConstants
                    .BACKGROUND_ORIGIN, shorthandExpression), new CssDeclaration(CommonCssConstants.BACKGROUND_CLIP, shorthandExpression
                    ), new CssDeclaration(CommonCssConstants.BACKGROUND_ATTACHMENT, shorthandExpression));
            }
            if (String.IsNullOrEmpty(shorthandExpression.Trim())) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.BACKGROUND));
                return new List<CssDeclaration>();
            }
            IList<IList<String>> propsList = CssUtils.ExtractShorthandProperties(shorthandExpression);
            IDictionary<CssBackgroundUtils.BackgroundPropertyType, String> resolvedProps = new Dictionary<CssBackgroundUtils.BackgroundPropertyType
                , String>();
            FillMapWithPropertiesTypes(resolvedProps);
            foreach (IList<String> props in propsList) {
                if (!ProcessProperties(props, resolvedProps)) {
                    return new List<CssDeclaration>();
                }
            }
            if (resolvedProps.Get(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_COLOR) == null) {
                resolvedProps.Put(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_COLOR, CommonCssConstants.TRANSPARENT
                    );
            }
            if (!CheckProperties(resolvedProps)) {
                return new List<CssDeclaration>();
            }
            return JavaUtil.ArraysAsList(new CssDeclaration(CssBackgroundUtils.GetBackgroundPropertyNameFromType(CssBackgroundUtils.BackgroundPropertyType
                .BACKGROUND_COLOR), resolvedProps.Get(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_COLOR)), new 
                CssDeclaration(CssBackgroundUtils.GetBackgroundPropertyNameFromType(CssBackgroundUtils.BackgroundPropertyType
                .BACKGROUND_IMAGE), resolvedProps.Get(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_IMAGE)), new 
                CssDeclaration(CssBackgroundUtils.GetBackgroundPropertyNameFromType(CssBackgroundUtils.BackgroundPropertyType
                .BACKGROUND_POSITION), resolvedProps.Get(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION
                )), new CssDeclaration(CssBackgroundUtils.GetBackgroundPropertyNameFromType(CssBackgroundUtils.BackgroundPropertyType
                .BACKGROUND_SIZE), resolvedProps.Get(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_SIZE)), new 
                CssDeclaration(CssBackgroundUtils.GetBackgroundPropertyNameFromType(CssBackgroundUtils.BackgroundPropertyType
                .BACKGROUND_REPEAT), resolvedProps.Get(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_REPEAT)), 
                new CssDeclaration(CssBackgroundUtils.GetBackgroundPropertyNameFromType(CssBackgroundUtils.BackgroundPropertyType
                .BACKGROUND_ORIGIN), resolvedProps.Get(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ORIGIN)), 
                new CssDeclaration(CssBackgroundUtils.GetBackgroundPropertyNameFromType(CssBackgroundUtils.BackgroundPropertyType
                .BACKGROUND_CLIP), resolvedProps.Get(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_CLIP)), new 
                CssDeclaration(CssBackgroundUtils.GetBackgroundPropertyNameFromType(CssBackgroundUtils.BackgroundPropertyType
                .BACKGROUND_ATTACHMENT), resolvedProps.Get(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ATTACHMENT
                )));
        }

        private static bool CheckProperties(IDictionary<CssBackgroundUtils.BackgroundPropertyType, String> resolvedProps
            ) {
            foreach (KeyValuePair<CssBackgroundUtils.BackgroundPropertyType, String> property in resolvedProps) {
                if (!CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CssBackgroundUtils.GetBackgroundPropertyNameFromType
                    (property.Key), property.Value))) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , property.Value));
                    return false;
                }
                IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CssBackgroundUtils.GetBackgroundPropertyNameFromType
                    (property.Key));
                if (resolver != null && resolver.ResolveShorthand(property.Value).IsEmpty()) {
                    return false;
                }
            }
            return true;
        }

        private static void RemoveSpacesAroundSlash(IList<String> props) {
            for (int i = 0; i < props.Count; ++i) {
                if ("/".Equals(props[i])) {
                    if (i != 0 && i != props.Count - 1) {
                        String property = props[i - 1] + props[i] + props[i + 1];
                        props[i + 1] = property;
                        props.JRemoveAt(i);
                        props.JRemoveAt(i - 1);
                    }
                    return;
                }
                if (props[i].StartsWith("/")) {
                    if (i != 0) {
                        String property = props[i - 1] + props[i];
                        props[i] = property;
                        props.JRemoveAt(i - 1);
                    }
                    return;
                }
                if (props[i].EndsWith("/")) {
                    if (i != props.Count - 1) {
                        String property = props[i] + props[i + 1];
                        props[i + 1] = property;
                        props.JRemoveAt(i);
                    }
                    return;
                }
            }
        }

        private static void FillMapWithPropertiesTypes(IDictionary<CssBackgroundUtils.BackgroundPropertyType, String
            > resolvedProps) {
            resolvedProps.Put(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_COLOR, null);
            resolvedProps.Put(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_IMAGE, null);
            resolvedProps.Put(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION, null);
            resolvedProps.Put(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_SIZE, null);
            resolvedProps.Put(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_REPEAT, null);
            resolvedProps.Put(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ORIGIN, null);
            resolvedProps.Put(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_CLIP, null);
            resolvedProps.Put(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ATTACHMENT, null);
        }

        private static bool ProcessProperties(IList<String> props, IDictionary<CssBackgroundUtils.BackgroundPropertyType
            , String> resolvedProps) {
            if (props.IsEmpty()) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.BACKGROUND));
                return false;
            }
            if (resolvedProps.Get(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_COLOR) != null) {
                LOGGER.LogWarning(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.ONLY_THE_LAST_BACKGROUND_CAN_INCLUDE_BACKGROUND_COLOR
                    );
                return false;
            }
            RemoveSpacesAroundSlash(props);
            ICollection<CssBackgroundUtils.BackgroundPropertyType> usedTypes = new HashSet<CssBackgroundUtils.BackgroundPropertyType
                >();
            if (ProcessAllSpecifiedProperties(props, resolvedProps, usedTypes)) {
                FillNotProcessedProperties(resolvedProps, usedTypes);
                return true;
            }
            else {
                return false;
            }
        }

        private static bool ProcessAllSpecifiedProperties(IList<String> props, IDictionary<CssBackgroundUtils.BackgroundPropertyType
            , String> resolvedProps, ICollection<CssBackgroundUtils.BackgroundPropertyType> usedTypes) {
            IList<String> boxValues = new List<String>();
            bool slashEncountered = false;
            bool propertyProcessedCorrectly = true;
            foreach (String value in props) {
                int slashCharInd = value.IndexOf('/');
                if (slashCharInd > 0 && slashCharInd < value.Length - 1 && !slashEncountered && !value.Contains("url(") &&
                     !value.Contains("device-cmyk(")) {
                    slashEncountered = true;
                    propertyProcessedCorrectly = ProcessValueWithSlash(value, slashCharInd, resolvedProps, usedTypes);
                }
                else {
                    CssBackgroundUtils.BackgroundPropertyType type = CssBackgroundUtils.ResolveBackgroundPropertyType(value);
                    if (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ORIGIN_OR_CLIP == type) {
                        boxValues.Add(value);
                    }
                    else {
                        propertyProcessedCorrectly = PutPropertyBasedOnType(ChangePropertyType(type, slashEncountered), value, resolvedProps
                            , usedTypes);
                    }
                }
                if (!propertyProcessedCorrectly) {
                    return false;
                }
            }
            return AddBackgroundClipAndBackgroundOriginBoxValues(boxValues, resolvedProps, usedTypes);
        }

        private static bool AddBackgroundClipAndBackgroundOriginBoxValues(IList<String> boxValues, IDictionary<CssBackgroundUtils.BackgroundPropertyType
            , String> resolvedProps, ICollection<CssBackgroundUtils.BackgroundPropertyType> usedTypes) {
            if (boxValues.Count == 1) {
                return PutPropertyBasedOnType(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_CLIP, boxValues[0], resolvedProps
                    , usedTypes);
            }
            else {
                if (boxValues.Count >= 2) {
                    for (int i = 0; i < 2; i++) {
                        CssBackgroundUtils.BackgroundPropertyType type = i == 0 ? CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ORIGIN
                             : CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_CLIP;
                        if (!PutPropertyBasedOnType(type, boxValues[i], resolvedProps, usedTypes)) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static bool ProcessValueWithSlash(String value, int slashCharInd, IDictionary<CssBackgroundUtils.BackgroundPropertyType
            , String> resolvedProps, ICollection<CssBackgroundUtils.BackgroundPropertyType> usedTypes) {
            String value1 = value.JSubstring(0, slashCharInd);
            CssBackgroundUtils.BackgroundPropertyType typeBeforeSlash = ChangePropertyType(CssBackgroundUtils.ResolveBackgroundPropertyType
                (value1), false);
            if (typeBeforeSlash != CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION && typeBeforeSlash !=
                 CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION_OR_SIZE) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY
                    , CommonCssConstants.BACKGROUND_POSITION, value1));
                return false;
            }
            String value2 = value.Substring(slashCharInd + 1);
            CssBackgroundUtils.BackgroundPropertyType typeAfterSlash = ChangePropertyType(CssBackgroundUtils.ResolveBackgroundPropertyType
                (value2), true);
            if (typeAfterSlash != CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_SIZE && typeAfterSlash != CssBackgroundUtils.BackgroundPropertyType
                .BACKGROUND_POSITION_OR_SIZE) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY
                    , CommonCssConstants.BACKGROUND_SIZE, value2));
                return false;
            }
            return PutPropertyBasedOnType(typeBeforeSlash, value1, resolvedProps, usedTypes) && PutPropertyBasedOnType
                (typeAfterSlash, value2, resolvedProps, usedTypes);
        }

        private static void FillNotProcessedProperties(IDictionary<CssBackgroundUtils.BackgroundPropertyType, String
            > resolvedProps, ICollection<CssBackgroundUtils.BackgroundPropertyType> usedTypes) {
            foreach (CssBackgroundUtils.BackgroundPropertyType type in new List<CssBackgroundUtils.BackgroundPropertyType
                >(resolvedProps.Keys)) {
                if (!usedTypes.Contains(type) && type != CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_COLOR) {
                    if (resolvedProps.Get(type) == null) {
                        resolvedProps.Put(type, CssDefaults.GetDefaultValue(CssBackgroundUtils.GetBackgroundPropertyNameFromType(type
                            )));
                    }
                    else {
                        resolvedProps.Put(type, resolvedProps.Get(type) + "," + CssDefaults.GetDefaultValue(CssBackgroundUtils.GetBackgroundPropertyNameFromType
                            (type)));
                    }
                }
            }
        }

        private static CssBackgroundUtils.BackgroundPropertyType ChangePropertyType(CssBackgroundUtils.BackgroundPropertyType
             propertyType, bool slashEncountered) {
            if (propertyType == CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION_X || propertyType == CssBackgroundUtils.BackgroundPropertyType
                .BACKGROUND_POSITION_Y) {
                propertyType = CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION;
            }
            if (propertyType == CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION_OR_SIZE) {
                return slashEncountered ? CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_SIZE : CssBackgroundUtils.BackgroundPropertyType
                    .BACKGROUND_POSITION;
            }
            if (propertyType == CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_SIZE && !slashEncountered) {
                return CssBackgroundUtils.BackgroundPropertyType.UNDEFINED;
            }
            if (propertyType == CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION && slashEncountered) {
                return CssBackgroundUtils.BackgroundPropertyType.UNDEFINED;
            }
            return propertyType;
        }

        /// <summary>Registers a property based on its type.</summary>
        /// <param name="type">the property type</param>
        /// <param name="value">the property value</param>
        /// <param name="resolvedProps">the resolved properties</param>
        /// <param name="usedTypes">already used types</param>
        /// <returns>false if the property is invalid. True in all other cases</returns>
        private static bool PutPropertyBasedOnType(CssBackgroundUtils.BackgroundPropertyType type, String value, IDictionary
            <CssBackgroundUtils.BackgroundPropertyType, String> resolvedProps, ICollection<CssBackgroundUtils.BackgroundPropertyType
            > usedTypes) {
            if (type == CssBackgroundUtils.BackgroundPropertyType.UNDEFINED) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
                    , value));
                return false;
            }
            if (resolvedProps.Get(type) == null) {
                resolvedProps.Put(type, value);
            }
            else {
                if (usedTypes.Contains(type)) {
                    resolvedProps.Put(type, resolvedProps.Get(type) + " " + value);
                }
                else {
                    resolvedProps.Put(type, resolvedProps.Get(type) + "," + value);
                }
            }
            usedTypes.Add(type);
            return true;
        }
    }
}
