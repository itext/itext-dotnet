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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Pdf;

namespace iText.Signatures {
    public class PdfDSS : PdfObjectWrapper<PdfDictionary> {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        public PdfDSS(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        protected override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        public virtual IX509Certificate[] GetCertificates() {
            PdfArray certs = base.GetPdfObject().GetAsArray(PdfName.Certs);
            IX509Certificate[] result = new IX509Certificate[certs == null ? 0 : certs.Size()];
            for (int i = 0; i < result.Length; i++) {
                try {
                    result[i] = CertificateUtil.GenerateCertificate(new MemoryStream(certs.GetAsStream(i).GetBytes()));
                }
                catch (Exception) {
                }
            }
            // Certificate will be ignored.
            return result;
        }

        public virtual IOcspResponse[] GetOcsps() {
            PdfArray ocsps = GetPdfObject().GetAsArray(PdfName.OCSPs);
            IOcspResponse[] result = new IOcspResponse[ocsps == null ? 0 : ocsps.Size()];
            for (int i = 0; i < result.Length; i++) {
                try {
                    result[i] = BOUNCY_CASTLE_FACTORY.CreateOCSPResponse(ocsps.GetAsStream(i).GetBytes());
                }
                catch (Exception) {
                }
            }
            // ignore failing OCSP responses.
            return result;
        }

        public virtual IX509Crl[] GetCrls() {
            PdfArray crls = GetPdfObject().GetAsArray(PdfName.CRLs);
            IX509Crl[] result = new IX509Crl[crls == null ? 0 : crls.Size()];
            for (int i = 0; i < result.Length; i++) {
                try {
                    result[i] = (IX509Crl)CertificateUtil.ParseCrlFromBytes(crls.GetAsStream(i).GetBytes());
                }
                catch (Exception) {
                }
            }
            // Ignore failing CRLs.
            return result;
        }

        public override int GetHashCode() {
            int result = 17;
            result = 31 * result + JavaUtil.ArraysHashCode(GetCertificates());
            result = 31 * result + JavaUtil.ArraysHashCode(GetOcsps());
            result = 31 * result + JavaUtil.ArraysHashCode(GetCrls());
            return result;
        }

        public override bool Equals(Object obj) {
            return base.Equals(obj);
        }
    }
}
