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
using System.Collections.Generic;

namespace iText.StyledXmlParser.Node {
    /// <summary>Interface for classes that describe a Node with a parent and children.</summary>
    public interface INode {
        /// <summary>Gets the child nodes.</summary>
        /// <returns>
        /// a list of
        /// <see cref="INode"/>
        /// instances.
        /// </returns>
        IList<INode> ChildNodes();

        /// <summary>Adds a child node.</summary>
        /// <param name="node">a child node that will be added to the current node</param>
        void AddChild(INode node);

        /// <summary>Gets the parent node.</summary>
        /// <returns>the parent node</returns>
        INode ParentNode();
    }
}
