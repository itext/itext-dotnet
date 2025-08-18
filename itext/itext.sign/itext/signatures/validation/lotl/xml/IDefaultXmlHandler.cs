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

namespace iText.Signatures.Validation.Lotl.Xml {
    /// <summary>Interface for handling XML elements during SAX processing.</summary>
    public interface IDefaultXmlHandler {
        /// <summary>Called when a start element is encountered in the XML document.</summary>
        /// <param name="uri">the namespace URI of the element</param>
        /// <param name="localName">the local name of the element</param>
        /// <param name="qName">the qualified name of the element</param>
        /// <param name="attributes">a map of attributes associated with the element</param>
        void StartElement(String uri, String localName, String qName, Dictionary<String, String> attributes);

        /// <summary>Called when an end element is encountered in the XML document.</summary>
        /// <param name="uri">the namespace URI of the element</param>
        /// <param name="localName">the local name of the element</param>
        /// <param name="qName">the qualified name of the element</param>
        void EndElement(String uri, String localName, String qName);

        /// <summary>Called when character data is encountered within an element.</summary>
        /// <param name="ch">the character array containing the data</param>
        /// <param name="start">the start index of the data in the array</param>
        /// <param name="length">the length of the data</param>
        void Characters(char[] ch, int start, int length);
    }
}
