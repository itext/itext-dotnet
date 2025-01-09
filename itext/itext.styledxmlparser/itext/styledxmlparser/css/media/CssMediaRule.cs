/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Media {
    /// <summary>
    /// The
    /// <see cref="iText.StyledXmlParser.Css.CssNestedAtRule"/>
    /// implementation for media rules.
    /// </summary>
    public class CssMediaRule : CssNestedAtRule {
        /// <summary>The media queries.</summary>
        private IList<MediaQuery> mediaQueries;

        /// <summary>
        /// Creates a
        /// <see cref="CssMediaRule"/>.
        /// </summary>
        /// <param name="ruleParameters">the rule parameters</param>
        public CssMediaRule(String ruleParameters)
            : base(CssRuleName.MEDIA, ruleParameters) {
            mediaQueries = MediaQueryParser.ParseMediaQueries(ruleParameters);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.CssNestedAtRule#getCssRuleSets(com.itextpdf.styledxmlparser.html.node.INode, com.itextpdf.styledxmlparser.css.media.MediaDeviceDescription)
        */
        public override IList<CssRuleSet> GetCssRuleSets(INode element, MediaDeviceDescription deviceDescription) {
            IList<CssRuleSet> result = new List<CssRuleSet>();
            foreach (MediaQuery mediaQuery in mediaQueries) {
                if (mediaQuery.Matches(deviceDescription)) {
                    foreach (CssStatement childStatement in body) {
                        result.AddAll(childStatement.GetCssRuleSets(element, deviceDescription));
                    }
                    break;
                }
            }
            return result;
        }

        /// <summary>Tries to match a media device.</summary>
        /// <param name="deviceDescription">the device description</param>
        /// <returns>true, if successful</returns>
        public virtual bool MatchMediaDevice(MediaDeviceDescription deviceDescription) {
            foreach (MediaQuery mediaQuery in mediaQueries) {
                if (mediaQuery.Matches(deviceDescription)) {
                    return true;
                }
            }
            return false;
        }
    }
}
