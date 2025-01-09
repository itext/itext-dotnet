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

namespace iText.StyledXmlParser.Node {
    /// <summary>Interface for a series of HTML attributes.</summary>
    public interface IAttributes : IEnumerable<IAttribute> {
        /// <summary>Gets the value of an attribute, given a key.</summary>
        /// <param name="key">the key</param>
        /// <returns>the attribute</returns>
        String GetAttribute(String key);

        /// <summary>Adds a key and a value of an attributes.</summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        void SetAttribute(String key, String value);

        /// <summary>Returns the number of attributes.</summary>
        /// <returns>the number of attributes</returns>
        int Size();
    }
}
