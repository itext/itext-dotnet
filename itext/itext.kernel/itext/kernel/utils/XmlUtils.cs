/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
