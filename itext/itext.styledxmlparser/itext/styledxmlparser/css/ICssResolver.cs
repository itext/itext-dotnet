/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.StyledXmlParser.Css.Resolve;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css {
    /// <summary>Interface for CSS resolvers.</summary>
    public interface ICssResolver {
        /// <summary>Resolves the styles of a node given the passed context.</summary>
        /// <param name="node">the node</param>
        /// <param name="context">the CSS context (RootFontSize, etc.)</param>
        /// <returns>the map containing the resolved styles</returns>
        IDictionary<String, String> ResolveStyles(INode node, AbstractCssContext context);
    }
}
