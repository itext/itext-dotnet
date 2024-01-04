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
using System.IO;
using System.Xml;

namespace iText.Kernel.Utils
{
    /// <summary>The interface in which methods for creating xml parsers are declared.</summary>
    public interface IXmlParserFactory
    {
        /// <summary>
        /// Creates the instance of <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="stream">the stream that contains the XML data.</param>
        /// <param name="inputContext">the context information required to parse the XML fragment.</param>
        /// <returns>an object that is used to read the XML data in the stream.</returns>
        XmlReader CreateXmlReaderInstance(Stream stream, XmlParserContext inputContext);
        
        /// <summary>
        /// Creates the instance of <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="textReader">the text reader from which to read the XML data.</param>
        /// <returns>an object that is used to read the XML data in the stream.</returns>
        XmlReader CreateXmlReaderInstance(TextReader textReader);
    }
}
