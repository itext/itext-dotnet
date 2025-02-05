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
using iText.Bouncycastlefips.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Utilities.IO;

namespace iText.Bouncycastlefips.X509 {
    /// <summary>
    /// Class to mimic X509CertificateParser logic.
    /// </summary>
    public class X509CertificateParserBCFips : IX509CertificateParser {

        private Asn1Set sData;
        
        private int sDataObjectCount;
        
        /// <summary>
        /// Creates new parser instance.
        /// </summary>
        public X509CertificateParserBCFips() {
            // Empty constructor.
        }
        
        /// <summary><inheritDoc/></summary>
        public List<IX509Certificate> ReadAllCerts(byte[] contentsKey) {
            List<IX509Certificate> certs = new List<IX509Certificate>();
            sData = null;
            sDataObjectCount = 0;
            try {
                // Try parsing der encoded certificates.
                using (MemoryStream stream = new MemoryStream(contentsKey)) {
                    X509Certificate cert;
                    while ((cert = ReadDerCertificate(stream)) != null) {
                        certs.Add(new X509CertificateBCFips(cert));
                    }
                }
            }
            catch (Exception) {
                // Certificates aren't der encoded, try parsing pem certificate.
                try {
                    using (MemoryStream stream = new MemoryStream(contentsKey)) {
                        foreach (IX509Certificate certificate in ReadPemCertificates(stream)) {
                            certs.Add(certificate);
                        }
                    }
                }
                catch (Exception) {
                    // Certificates aren't pem encoded, return what we have.
                }
            }

            return certs;
        }

        private List<IX509Certificate> ReadPemCertificates(MemoryStream stream) {
            using (TextReader file = new StreamReader(stream)) {
                OpenSslPemReader reader = new OpenSslPemReader(file);
                Object readObject = reader.ReadObject();
                List<IX509Certificate> certificates = new List<IX509Certificate>();
                while (readObject != null) {
                    certificates.Add(new X509CertificateBCFips((X509Certificate)readObject));
                    readObject = reader.ReadObject();
                }
                return certificates;
            }
        }
        
        private X509Certificate ReadDerCertificate(MemoryStream stream) {
            if (sData != null) {
                if (sDataObjectCount != sData.Count) {
                    return GetCertificate();
                }

                sData = null;
                sDataObjectCount = 0;
                return null;
            }
            
            PushbackStream pushbackStream = new PushbackStream(stream);
            int tag = pushbackStream.ReadByte();
            if (tag <= 0) {
                return null;
            }
            pushbackStream.Unread(tag);
            Asn1Sequence seq = (Asn1Sequence)(new Asn1InputStream(pushbackStream).ReadObject());
            if (seq.Count > 1 && seq[0] is DerObjectIdentifier) {
                if (seq[0].Equals(PkcsObjectIdentifiers.SignedData)) {
                    sData = SignedData.GetInstance(
                        Asn1Sequence.GetInstance((Asn1TaggedObject) seq[1], true)).Certificates;
                    return GetCertificate();
                }
            }
            return new X509Certificate(X509CertificateStructure.GetInstance(seq));
        }

        private X509Certificate GetCertificate() {
            if (sData != null) {
                while (sDataObjectCount < sData.Count) {
                    object obj = sData[sDataObjectCount++];
                    if (obj is Asn1Sequence) {
                        return new X509Certificate(X509CertificateStructure.GetInstance(obj));
                    }
                }
            }

            return null;
        }
    }
}
