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
using System.IO;
using System.Xml;
using iText.Kernel.Exceptions;
using iText.Kernel.Utils;

namespace iText.Forms.Xfa
{
    public class SecurityTestXmlParserFactory : IXmlParserFactory
    {
        private XmlReaderSettings xmlReaderSettings;

        public SecurityTestXmlParserFactory()
        {
            xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
            xmlReaderSettings.XmlResolver = new TestXmlResolver();
        }

        public XmlReader CreateXmlReaderInstance(Stream stream, XmlParserContext inputContext)
        {
            return XmlReader.Create(stream, xmlReaderSettings);
        }

        public XmlReader CreateXmlReaderInstance(TextReader textReader)
        {
            return XmlReader.Create(textReader, xmlReaderSettings);
        }

        private class TestXmlResolver : XmlResolver
        {
            public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
            {
                throw new PdfException("Test message");
            }
        }
    }
}
