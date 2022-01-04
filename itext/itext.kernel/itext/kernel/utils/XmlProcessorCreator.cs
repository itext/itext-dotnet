﻿/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
