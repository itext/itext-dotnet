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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Utils;
using iText.Signatures.Exceptions;
using System.Xml;
using System.IO;
using iText.Commons.Bouncycastle;
using iText.Bouncycastleconnector;
using Org.BouncyCastle.Security.Certificates;

namespace iText.Signatures.Validation {
    internal abstract class AbstractXmlCertificateHandler
    {

        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY =
            BouncyCastleFactoryCreator.GetFactory();

        internal AbstractXmlCertificateHandler() {
        }

        internal abstract IServiceContext GetServiceContext(IX509Certificate certificate);

        internal abstract IList<IX509Certificate> GetCertificateList();

        public abstract void StartElement(String uri, String localName, String qName, Attributes attributes);

        public abstract void EndElement(String uri, String localName, String qName);

        public abstract void Characters(char[] ch, int start, int length);


        internal void ReadXml(System.IO.Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
        settings.Async = true;

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            Attributes attributes = new Attributes();
                            if (reader.HasAttributes)
                            {
                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    attributes.Add(reader.Name, reader.Value);
                                }
                            }
                            StartElement(reader.BaseURI, RemoveNamespace(reader.Name), reader.Name, attributes);
                            break;
                        case XmlNodeType.Text:
                            String value = reader.Value;
                            Characters(value.ToCharArray(0, value.Length), 0, value.Length);
                            break;
                        case XmlNodeType.EndElement:
                            EndElement(reader.BaseURI, RemoveNamespace(reader.Name), reader.Name);
                            break;
                        default:
                            break;
                    }
}
            }
        }

        internal IX509Certificate GetCertificateFromEncodedData(String certificateString)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(certificateString);
                IX509Certificate certificate = BOUNCY_CASTLE_FACTORY.CreateX509Certificate(bytes);
                return certificate;
            }
            catch (FormatException e) {
                throw new PdfException(SignExceptionMessageConstant.FAILED_TO_RETRIEVE_CERTIFICATE, e);
            } 
            catch (CertificateException e)
            {
                throw new PdfException(SignExceptionMessageConstant.FAILED_TO_RETRIEVE_CERTIFICATE, e);
            }
        }

        internal String RemoveNamespace(String name)
        {
            int indexOfNamespace = name.IndexOf(":");
            if (indexOfNamespace != -1)
            {
                return name.Remove(0, indexOfNamespace + 1);
                
            } else
            {
                return name;
            }
        }

        internal abstract void Clear();

        internal class Attributes : Dictionary<String, String>
        {
            public String GetValue(String qName)
            {
                String result;
                return TryGetValue(qName, out result) ? result : null;
            }
        }
    }
}
