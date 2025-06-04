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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;
using iText.Commons.Bouncycastle.Cert;
using Org.BouncyCastle.Security.Certificates;
using System.Xml;

namespace iText.Signatures {
    internal class XmlCertificateRetriever {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        internal XmlCertificateRetriever() {
        }

        internal static IList<IX509Certificate> GetCertificates(String path) {
            XmlCertificateHandler handler = new XmlCertificateHandler();

            XmlCertificateHandler certificateHandler = new XmlCertificateHandler();
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            try {
                handler.ReadXml(stream);
            }
            catch (XmlException e) {
                throw new PdfException(MessageFormatUtil.Format(SignExceptionMessageConstant.FAILED_TO_READ_CERTIFICATE_BYTES_FROM_XML
                    , path), e);
            }

            IList<byte[]> certificateBytes = handler.GetCertificatesBytes();
            IList<IX509Certificate> certificates = new List<IX509Certificate>();
            foreach (byte[] certificateByte in certificateBytes) {
                try {
                    IX509Certificate certificate = BOUNCY_CASTLE_FACTORY.CreateX509Certificate(certificateByte);
                    certificates.Add(certificate);
                }
                catch (CertificateException e) {
                    throw new PdfException(SignExceptionMessageConstant.FAILED_TO_RETRIEVE_CERTIFICATE, e);
                }
            }
            return certificates;
        }
    }
}
