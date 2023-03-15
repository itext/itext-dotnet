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
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Resolve;
using iText.Svg;

namespace iText.Svg.Css.Impl {
    /// <summary>Helper class that allows you to check if a property is inheritable.</summary>
    public class SvgAttributeInheritance : IStyleInheritance {
        /// <summary>
        /// Set of inheritable SVG style attributes
        /// in accordance with "http://www.w3schools.com/cssref/"
        /// and "https://developer.mozilla.org/en-US/docs/Web/CSS/Reference"
        /// </summary>
        private static readonly ICollection<String> inheritableProperties = JavaCollectionsUtil.UnmodifiableSet(new 
            HashSet<String>(JavaUtil.ArraysAsList(SvgConstants.Attributes.CLIP_RULE, SvgConstants.Attributes.FILL, 
            SvgConstants.Attributes.FILL_RULE, SvgConstants.Attributes.STROKE, SvgConstants.Attributes.STROKE_WIDTH
            , SvgConstants.Attributes.TEXT_ANCHOR)));

        // clip-rule
        // fill
        // fill-rule
        // stroke
        // stroke-width
        // text-anchor
        public virtual bool IsInheritable(String cssProperty) {
            return inheritableProperties.Contains(cssProperty);
        }
    }
}
