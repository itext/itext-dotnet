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
using System.IO;
using System.Xml;
using iText.Kernel.Exceptions;

namespace iText.Signatures.Validation.Xml {
    internal class XmlSaxProcessor {
        public virtual void Process(Stream inputStream, IDefaultXmlHandler handler) {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            try {
                using (XmlReader reader = XmlReader.Create(inputStream, settings)) {
                    while (reader.Read()) {
                        switch (reader.NodeType) {
                            case XmlNodeType.Element:
                                Dictionary<string, string> attributes = new Dictionary<string, string>();
                                if (reader.HasAttributes) {
                                    for (int i = 0; i < reader.AttributeCount; i++) {
                                        reader.MoveToAttribute(i);
                                        attributes.Add(reader.Name, reader.Value);
                                    }
                                }

                                handler.StartElement(reader.BaseURI, RemoveNamespace(reader.Name), reader.Name,
                                    attributes);
                                break;
                            case XmlNodeType.Text:
                                String value = reader.Value;
                                handler.Characters(value.ToCharArray(0, value.Length), 0, value.Length);
                                break;
                            case XmlNodeType.EndElement:
                                handler.EndElement(reader.BaseURI, RemoveNamespace(reader.Name), reader.Name);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (XmlException e) {
                throw new PdfException(e.Message);
            }
        }


        private String RemoveNamespace(String name) {
            int indexOfNamespace = name.IndexOf(":");
            if (indexOfNamespace != -1) {
                return name.Remove(0, indexOfNamespace + 1);
            }
            else {
                return name;
            }
        }
    }
}