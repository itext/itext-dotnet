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
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Utils {
    /// <summary>Utility class which contains methods related to href resolving</summary>
    public sealed class TemplateResolveUtils {
        private TemplateResolveUtils() {
        }

        //private constructor for utility class
        /// <summary>Resolve href to other object within svg and fills renderer with its properties and children if needed.
        ///     </summary>
        /// <param name="renderer">renderer which should be updated after resolving its href attribute</param>
        /// <param name="context">svg draw context instance</param>
        public static void Resolve(IBranchSvgNodeRenderer renderer, SvgDrawContext context) {
            String href = renderer.GetAttribute(SvgConstants.Attributes.HREF);
            if (href == null) {
                href = renderer.GetAttribute(SvgConstants.Attributes.XLINK_HREF);
            }
            if (href == null || href[0] != '#') {
                return;
            }
            String normalizedName = SvgTextUtil.FilterReferenceValue(href);
            ISvgNodeRenderer template = context.GetNamedObject(normalizedName);
            if (!(template is IBranchSvgNodeRenderer)) {
                return;
            }
            IBranchSvgNodeRenderer namedObject = (IBranchSvgNodeRenderer)template.CreateDeepCopy();
            Resolve(namedObject, context);
            if (renderer.GetChildren().IsEmpty()) {
                foreach (ISvgNodeRenderer child in namedObject.GetChildren()) {
                    renderer.AddChild(child);
                }
            }
            //href attributes inheritance rule are really simple, and only attributes not defined at renderer on which
            //href is resolved should be copied from referenced object
            IDictionary<String, String> referencedAttributes = namedObject.GetAttributeMapCopy();
            foreach (KeyValuePair<String, String> entry in referencedAttributes) {
                if (renderer.GetAttribute(entry.Key) == null) {
                    renderer.SetAttribute(entry.Key, entry.Value);
                }
            }
        }
    }
}
