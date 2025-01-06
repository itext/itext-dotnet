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
using System.IO;
using System.Xml;

namespace iText.Kernel.Utils
{
    /// <summary>
    /// Utility class for creating XML processors.
    /// </summary>
    public static class XmlProcessorCreator
    {
        private static IXmlParserFactory _xmlParserFactory = new DefaultSafeXmlParserFactory();
        
        /// <summary>
        /// Specifies an <see cref="IXmlParserFactory"/> implementation that will be used to create the xml parsers in
        /// the <see cref="XmlProcessorCreator"/>. Pass <see cref="DefaultSafeXmlParserFactory"/> to use default safe factory
        /// that should prevent XML attacks like XML bombs and XXE attacks. This will definitely throw an exception if
        /// any DTD object is present in the XML. If you need the special XML parser creating, you can declare your
        /// own <see cref="IXmlParserFactory"/> implementation and pass it to this method.
        /// </summary>
        /// <param name="factory">factory to be used to create xml parsers. If the passed factory is <c>null</c>,
        /// the <see cref="DefaultSafeXmlParserFactory"/> will be used</param>
        public static void SetXmlParserFactory(IXmlParserFactory factory)
        {
            _xmlParserFactory = factory ?? new DefaultSafeXmlParserFactory();
        }

        /// <summary>
        /// Creates the instance of <see cref="XmlReader"/>. The default implementation is configured to prevent
        /// possible XML attacks (see <see cref="DefaultSafeXmlParserFactory"/>). But you can use
        /// <see cref="SetXmlParserFactory"/> to set your specific factory for creating xml parsers.
        /// </summary>
        /// <param name="stream">the stream that contains the XML data.</param>
        /// <returns>an object that is used to read the XML data in the stream.</returns>
        public static XmlReader CreateSafeXmlReader(Stream stream)
        {
            return CreateSafeXmlReader(stream, null);
        }

        /// <summary>
        /// Creates the instance of <see cref="XmlReader"/>. The default implementation is configured to prevent
        /// possible XML attacks (see <see cref="DefaultSafeXmlParserFactory"/>). But you can use
        /// <see cref="SetXmlParserFactory"/> to set your specific factory for creating xml parsers.
        /// </summary>
        /// <param name="stream">the stream that contains the XML data.</param>
        /// <param name="inputContext">the context information required to parse the XML fragment.</param>
        /// <returns>an object that is used to read the XML data in the stream.</returns>
        public static XmlReader CreateSafeXmlReader(Stream stream, XmlParserContext inputContext)
        {
            return _xmlParserFactory.CreateXmlReaderInstance(stream, inputContext);
        }

        /// <summary>
        /// Creates the instance of <see cref="XmlReader"/>. The default implementation is configured to prevent
        /// possible XML attacks (see <see cref="DefaultSafeXmlParserFactory"/>). But you can use
        /// <see cref="SetXmlParserFactory"/> to set your specific factory for creating xml parsers.
        /// </summary>
        /// <param name="textReader">the text reader from which to read the XML data.</param>
        /// <returns>an object that is used to read the XML data in the stream.</returns>
        public static XmlReader CreateSafeXmlReader(TextReader textReader)
        {
            return _xmlParserFactory.CreateXmlReaderInstance(textReader);
        }
    }
}
