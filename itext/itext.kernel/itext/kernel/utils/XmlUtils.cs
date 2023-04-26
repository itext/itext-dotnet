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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace iText.Kernel.Utils {
    sealed class XmlUtils {
        public static void WriteXmlDocToStream(XmlDocument xmlReport, Stream stream) {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(stream, settings);
            xmlReport.WriteTo(writer);
            writer.Flush();
        }

        public static bool CompareXmls(Stream xml1, Stream xml2) {
            XElement el1 = XElement.Load(XmlProcessorCreator.CreateSafeXmlReader(xml1));
            XElement el2 = XElement.Load(XmlProcessorCreator.CreateSafeXmlReader(xml2));

            return XNode.DeepEquals(Normalize(el1), Normalize(el2));
        }

        public static XmlDocument InitNewXmlDocument() {
            return new XmlDocument();
        }

        private static XElement Normalize(XElement element) {
            if (element.HasElements) {
                return new XElement(
                    element.Name,
                    element.Attributes().Where(a => a.Name.Namespace == XNamespace.Xmlns)
                        .OrderBy(a => a.Name.ToString()),
                    element.Elements().OrderBy(a => a.Name.ToString())
                        .Select(e => Normalize(e)));
            }

            if (element.IsEmpty) {
                return new XElement(element.Name, element.Attributes()
                    .OrderBy(a => a.Name.ToString()));
            }

            return new XElement(element.Name, element.Attributes()
                .OrderBy(a => a.Name.ToString()), element.Value);
        }
    }
}
