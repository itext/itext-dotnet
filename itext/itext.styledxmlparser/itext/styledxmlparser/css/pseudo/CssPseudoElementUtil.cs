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
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Pseudo {
    /// <summary>Utilities class for pseudo elements.</summary>
    public class CssPseudoElementUtil {
        /// <summary>The prefix for pseudo elements.</summary>
        private const String TAG_NAME_PREFIX = "pseudo-element::";

        /// <summary>Creates the pseudo element tag name.</summary>
        /// <param name="pseudoElementName">the pseudo element name</param>
        /// <returns>the tag name</returns>
        public static String CreatePseudoElementTagName(String pseudoElementName) {
            return TAG_NAME_PREFIX + pseudoElementName;
        }

        /// <summary>Checks for before or after elements.</summary>
        /// <param name="node">the node</param>
        /// <returns>true, if successful</returns>
        public static bool HasBeforeAfterElements(IElementNode node) {
            if (node == null || node is CssPseudoElementNode || node.Name().StartsWith(TAG_NAME_PREFIX)) {
                return false;
            }
            return true;
        }
    }
}
