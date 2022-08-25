/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Validate;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css {
    /// <summary>Class that stores all the CSS statements, and thus acts as a CSS style sheet.</summary>
    public class CssStyleSheet {
        /// <summary>The list of CSS statements.</summary>
        private IList<CssStatement> statements;

        /// <summary>
        /// Creates a new
        /// <see cref="CssStyleSheet"/>
        /// instance.
        /// </summary>
        public CssStyleSheet() {
            statements = new List<CssStatement>();
        }

        /// <summary>Adds a CSS statement to the style sheet.</summary>
        /// <param name="statement">the CSS statement</param>
        public virtual void AddStatement(CssStatement statement) {
            statements.Add(statement);
        }

        /// <summary>Append another CSS style sheet to this one.</summary>
        /// <param name="anotherCssStyleSheet">the other CSS style sheet</param>
        public virtual void AppendCssStyleSheet(iText.StyledXmlParser.Css.CssStyleSheet anotherCssStyleSheet) {
            statements.AddAll(anotherCssStyleSheet.statements);
        }

        /* (non-Javadoc)
        * @see java.lang.Object#toString()
        */
        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (CssStatement statement in statements) {
                if (sb.Length > 0) {
                    sb.Append("\n");
                }
                sb.Append(statement.ToString());
            }
            return sb.ToString();
        }

        /// <summary>Gets the CSS statements of this style sheet.</summary>
        /// <returns>the CSS statements</returns>
        public virtual IList<CssStatement> GetStatements() {
            return JavaCollectionsUtil.UnmodifiableList(statements);
        }

        /// <summary>Gets the CSS declarations.</summary>
        /// <param name="node">the node</param>
        /// <param name="deviceDescription">the media device description</param>
        /// <returns>the CSS declarations</returns>
        public virtual IList<CssDeclaration> GetCssDeclarations(INode node, MediaDeviceDescription deviceDescription
            ) {
            IList<CssRuleSet> ruleSets = GetCssRuleSets(node, deviceDescription);
            IDictionary<String, CssDeclaration> declarations = new LinkedDictionary<String, CssDeclaration>();
            foreach (CssRuleSet ruleSet in ruleSets) {
                PopulateDeclarationsMap(ruleSet.GetNormalDeclarations(), declarations);
            }
            foreach (CssRuleSet ruleSet in ruleSets) {
                PopulateDeclarationsMap(ruleSet.GetImportantDeclarations(), declarations);
            }
            return new List<CssDeclaration>(declarations.Values);
        }

        /// <summary>Gets the CSS declarations.</summary>
        /// <param name="ruleSets">list of css rule sets</param>
        /// <returns>the CSS declarations</returns>
        public static IDictionary<String, String> ExtractStylesFromRuleSets(IList<CssRuleSet> ruleSets) {
            IDictionary<String, CssDeclaration> declarations = new LinkedDictionary<String, CssDeclaration>();
            foreach (CssRuleSet ruleSet in ruleSets) {
                PopulateDeclarationsMap(ruleSet.GetNormalDeclarations(), declarations);
            }
            foreach (CssRuleSet ruleSet in ruleSets) {
                PopulateDeclarationsMap(ruleSet.GetImportantDeclarations(), declarations);
            }
            IDictionary<String, String> stringMap = new LinkedDictionary<String, String>();
            foreach (KeyValuePair<String, CssDeclaration> entry in declarations) {
                stringMap.Put(entry.Key, entry.Value.GetExpression());
            }
            return stringMap;
        }

        /// <summary>Populates the CSS declarations map.</summary>
        /// <param name="declarations">the declarations</param>
        /// <param name="map">the map</param>
        private static void PopulateDeclarationsMap(IList<CssDeclaration> declarations, IDictionary<String, CssDeclaration
            > map) {
            foreach (CssDeclaration declaration in declarations) {
                IShorthandResolver shorthandResolver = ShorthandResolverFactory.GetShorthandResolver(declaration.GetProperty
                    ());
                if (shorthandResolver == null) {
                    PutDeclarationInMapIfValid(map, declaration);
                }
                else {
                    IList<CssDeclaration> resolvedShorthandProps = shorthandResolver.ResolveShorthand(declaration.GetExpression
                        ());
                    PopulateDeclarationsMap(resolvedShorthandProps, map);
                }
            }
        }

        /// <summary>Gets the CSS rule sets.</summary>
        /// <param name="node">the node</param>
        /// <param name="deviceDescription">the device description</param>
        /// <returns>the css rule sets</returns>
        public virtual IList<CssRuleSet> GetCssRuleSets(INode node, MediaDeviceDescription deviceDescription) {
            IList<CssRuleSet> ruleSets = new List<CssRuleSet>();
            foreach (CssStatement statement in statements) {
                ruleSets.AddAll(statement.GetCssRuleSets(node, deviceDescription));
            }
            JavaCollectionsUtil.Sort(ruleSets, new CssRuleSetComparator());
            return ruleSets;
        }

        /// <summary>Puts a declaration in a styles map if the declaration is valid.</summary>
        /// <param name="stylesMap">the styles map</param>
        /// <param name="cssDeclaration">the css declaration</param>
        private static void PutDeclarationInMapIfValid(IDictionary<String, CssDeclaration> stylesMap, CssDeclaration
             cssDeclaration) {
            if (CssDeclarationValidationMaster.CheckDeclaration(cssDeclaration)) {
                stylesMap.Put(cssDeclaration.GetProperty(), cssDeclaration);
            }
            else {
                ILogger logger = ITextLogManager.GetLogger(typeof(ICssResolver));
                logger.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , cssDeclaration));
            }
        }
    }
}
