using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace iText.Kernel.Utils {
    class XmlUtils {
        public static void WriteXmlDocToStream(XmlDocument xmlReport, Stream stream) {
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Default);
            writer.Formatting = Formatting.Indented;
            xmlReport.WriteTo(writer);
            writer.Flush();
        }

        public static bool CompareXmls(Stream xml1, Stream xml2) {
            XElement el1 = XElement.Load(xml1);
            XElement el2 = XElement.Load(xml2);

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
