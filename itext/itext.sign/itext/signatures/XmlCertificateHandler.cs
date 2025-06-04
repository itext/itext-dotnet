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
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace iText.Signatures {
    internal class XmlCertificateHandler {
        private const String CERTIFICATE_TAG = "X509Certificate";

        private const String SIGNATURE_CERTIFICATE_TAG = "ds:X509Certificate";

        private bool isReadingCertificate = false;

        private StringBuilder certificateByteBuilder;

        internal IList<byte[]> certificateBytes = new List<byte[]>();

        internal XmlCertificateHandler() {
            //empty constructor
        }

        public void ReadXml(System.IO.Stream stream)
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
                            StartElement(reader.Name);
                            break;
                        case XmlNodeType.Text:
                            Characters(reader.GetValueAsync().Result);
                            break;
                        case XmlNodeType.EndElement:
                            EndElement(reader.Name);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void StartElement(String qName) {
            if (CERTIFICATE_TAG.Equals(qName) || SIGNATURE_CERTIFICATE_TAG.Equals(qName)) {
                isReadingCertificate = true;
                certificateByteBuilder = new StringBuilder();
            }
        }

        private void EndElement(String qName) {
            if (CERTIFICATE_TAG.Equals(qName) || SIGNATURE_CERTIFICATE_TAG.Equals(qName)) {
                certificateBytes.Add(Convert.FromBase64String(certificateByteBuilder.ToString()));
            }
        }

        private void Characters(String value) {
            if (isReadingCertificate) {
                certificateByteBuilder.Append(value);
            }
        }

        public virtual IList<byte[]> GetCertificatesBytes() {
            return certificateBytes;
        }
    }
}
