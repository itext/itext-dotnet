/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Utils {
    /// <summary>Utility methods for working with XML Documents in a safe way.</summary>
    public sealed class XmlUtils {
        /// <summary>Private constructor to prevent instantiation of this utility class.</summary>
        private XmlUtils() {
        }

        // Empty private constructor
        /// <summary>
        /// Writes the provided DOM
        /// <see cref="System.Xml.XmlDocument"/>
        /// to the given
        /// <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="xmlReport">
        /// the DOM document to serialize; must not be
        /// <see langword="null"/>
        /// </param>
        /// <param name="stream">
        /// the output stream to write the XML to; the caller is
        /// responsible for opening and closing the stream
        /// </param>
        public static void WriteXmlDocToStream(XmlDocument xmlReport, Stream stream) {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(stream, settings);
            xmlReport.WriteTo(writer);
            writer.Flush();
        }

        /// <summary>Compares two XML documents provided as input streams for structural equality.</summary>
        /// <remarks>
        /// Compares two XML documents provided as input streams for structural equality.
        /// <para />
        /// The method parses both input streams using a secure
        /// <see cref="Javax.Xml.Parsers.DocumentBuilder"/>
        /// (created via
        /// <see cref="XmlProcessorCreator.CreateSafeDocumentBuilder(bool, bool)"/>
        /// ),
        /// normalizes the documents and removes empty text nodes before
        /// delegating to
        /// <see cref="System.Xml.XmlNode.IsEqualNode(System.Xml.XmlNode)"/>
        /// to perform
        /// the equality check.
        /// </remarks>
        /// <param name="xml1">input stream of the first XML document; must be readable</param>
        /// <param name="xml2">input stream of the second XML document; must be readable</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the documents are structurally equal,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool CompareXmls(System.IO.Stream xml1, System.IO.Stream xml2) {
            XElement el1 = XElement.Load(XmlProcessorCreator.CreateSafeXmlReader(xml1));
            XElement el2 = XElement.Load(XmlProcessorCreator.CreateSafeXmlReader(xml2));

            NormalizeTextNodes(el1);
            NormalizeTextNodes(el2);
            
            return XNode.DeepEquals(Normalize(el1), Normalize(el2));
        }

        /// <summary>
        /// Creates and returns a new empty DOM
        /// <see cref="System.Xml.XmlDocument"/>
        /// <returns>
        /// a new empty
        /// <see cref="System.Xml.XmlDocument"/>
        /// </returns>
        public static XmlDocument InitNewXmlDocument() {
            return new XmlDocument();
        }

        /// <summary>
        /// Parses an XML document from the provided input stream and returns the resulting DOM
        /// <see cref="System.Xml.XmlDocument"/>.
        /// </summary>
        /// <remarks>
        /// Parses an XML document from the provided input stream and returns the resulting DOM
        /// <see cref="System.Xml.XmlDocument"/>.
        /// <para />
        /// If parsing fails for any reason the method wraps the underlying
        /// exception in a
        /// <see cref="iText.Kernel.Exceptions.PdfException"/>.
        /// </remarks>
        /// <param name="inputStream">the input stream containing XML content; must be readable</param>
        /// <returns>
        /// the parsed
        /// <see cref="System.Xml.XmlDocument"/>
        /// </returns>
        public static XmlDocument InitXmlDocument(System.IO.Stream inputStream) {
            try {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.Load(XmlProcessorCreator.CreateSafeXmlReader(inputStream));
                return doc;
            }
            catch (Exception e) {
                throw new PdfException(e.Message, e);
            }
        }

        private static XElement Normalize(XElement element) {
            IEnumerable<XAttribute> attrs = element.Attributes()
                .OrderBy(a => a.Name.ToString());

            bool hasElements = element.Elements().Any();
            bool hasTextNodes = element.Nodes().OfType<XText>().Any();

            // Mixed content: keep text nodes and preserve node order
            if (hasElements && hasTextNodes) {
                return new XElement(
                    element.Name,
                    attrs,
                    element.Nodes().Select(n => {
                        if (n is XElement e)
                        {
                            return (object)Normalize(e);
                        }
                        if (n is XText t)
                        {
                            return (object)new XText(t.Value);
                        }
                        return (object)n; 
                    })
                );
            }

            if (hasElements) {
                return new XElement(
                    element.Name,
                    attrs,
                    element.Elements()
                        .OrderBy(e => e.Name.ToString())
                        .Select(e => Normalize(e))
                );
            }

            if (element.IsEmpty) {
                return new XElement(element.Name, attrs);
            }

            return new XElement(element.Name, attrs, element.Value);
        }
        
        private static void NormalizeTextNodes(XElement element) {
            if (element == null) {
                return;
            }

            List<XText> toRemove = element
                .Nodes()
                .OfType<XText>()
                .Where(t => string.IsNullOrWhiteSpace(t.Value))
                .ToList();

            foreach (XText t in toRemove) {
                t.Remove();
            }

            foreach (XElement child in element.Elements()) {
                NormalizeTextNodes(child);
            }
        }
    }
}
