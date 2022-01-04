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
