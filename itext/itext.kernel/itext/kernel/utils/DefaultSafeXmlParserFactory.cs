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
    /// <summary>
    /// Implementation of
    /// <see cref="IXmlParserFactory"/>
    /// for creating safe xml parser objects.
    /// </summary>
    /// <remarks>
    /// Implementation of
    /// <see cref="IXmlParserFactory"/>
    /// for creating safe xml parser objects.
    /// Creates parsers with configuration to prevent xml bombs and xxe attacks.
    /// </remarks>
    public class DefaultSafeXmlParserFactory : IXmlParserFactory
    {
        public XmlReader CreateXmlReaderInstance(Stream stream, XmlParserContext inputContext)
        {
            return XmlReader.Create(stream, CreateSafeXmlReaderSettings(), inputContext);
        }

        public XmlReader CreateXmlReaderInstance(TextReader textReader)
        {
            return XmlReader.Create(textReader, CreateSafeXmlReaderSettings());
        }

        /// <summary>
        /// Creates <see cref="XmlReaderSettings"/> to make reader secure against xml attacks.
        /// </summary>
        /// <returns>Configured xml reader settings</returns>
        protected virtual XmlReaderSettings CreateSafeXmlReaderSettings()
        {
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings {DtdProcessing = DtdProcessing.Prohibit};
            return xmlReaderSettings;
        }
    }
}
