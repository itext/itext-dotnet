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