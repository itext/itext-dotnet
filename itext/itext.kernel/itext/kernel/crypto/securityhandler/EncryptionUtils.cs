/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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

using System;
using System.Globalization;
using System.Security.Cryptography;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    internal sealed class EncryptionUtils {
        // 256-bit AES-CBC, PKCS#5 padding
        // Not ideal, but the best that the PDF standard allows.
        public const String ENVELOPE_ENCRYPTION_ALGORITHM_OID = "2.16.840.1.101.3.4.1.42";
        public const int ENVELOPE_ENCRYPTION_KEY_LENGTH = 256;

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        internal static byte[] GenerateSeed(int seedLength) {
            return IVGenerator.GetIV(seedLength);
        }

        internal static byte[] FetchEnvelopedData(IPrivateKey certificateKey, IX509Certificate certificate, PdfArray recipients) {
            bool foundRecipient = false;
            byte[] envelopedData = null;
            for (int i = 0; i < recipients.Size(); i++) {
                try {
                    PdfString recipient = recipients.GetAsString(i);
                    ICMSEnvelopedData data = FACTORY.CreateCMSEnvelopedData(recipient.GetValueBytes());
                    foreach (IRecipientInformation recipientInfo in data.GetRecipientInfos().GetRecipients()) {
                        if (recipientInfo.GetRID().Match(certificate) && !foundRecipient) { 
                            envelopedData = recipientInfo.GetContent(certificateKey); 
                            foundRecipient = true;
                        }
                    }
                } catch (Exception f) {
                    throw new PdfException(KernelExceptionMessageConstant.PDF_DECRYPTION, f);
                }
            }
            if (!foundRecipient || envelopedData == null) {
                throw new PdfException(KernelExceptionMessageConstant.BAD_CERTIFICATE_AND_KEY);
            }
            return envelopedData;
        }

        internal static byte[] CipherBytes(IX509Certificate x509Certificate, byte[] abyte0, IAlgorithmIdentifier algorithmidentifier) {
            return FACTORY.CreateCipherBytes(x509Certificate, abyte0, algorithmidentifier);
        }

        // TODO Review this method and it's usages. It is used in bouncy castle sources in itextsharp, so we need to be carefull about it in case we update BouncyCastle.
        internal static CultureInfo GetStandartEnUsLocale() {
            CultureInfo locale = (CultureInfo) CultureInfo.InvariantCulture.Clone();
            //                          en-US                        Invariant
            //=====================     ==================           ==================
            //Currency Symbol           $                            
            //Currency                  $123456.78                   123456.78
            //Short Date                1/11/2012                    01/11/2012
            //Time                      10:36:52 PM                  22:36:52
            //Metric                    No                           Yes
            //Long Date                 Wednesday, January 11, 2012  Wednesday, 11 January, 2012
            //Year Month                January, 2012                2012 January
            locale.NumberFormat.CurrencySymbol = "$";
            locale.DateTimeFormat.ShortDatePattern = "M/d/yyyy";
            locale.DateTimeFormat.ShortTimePattern = "h:mm tt";
            locale.DateTimeFormat.LongDatePattern = "dddd, MMMM dd, yyyy";
            locale.DateTimeFormat.YearMonthPattern = "MMMM, yyyy";
            return locale;
        }

        internal static DERForRecipientParams CalculateDERForRecipientParams(byte[] @in) {
            DERForRecipientParams parameters = new DERForRecipientParams();

            IASN1ObjectIdentifier derob = FACTORY.CreateASN1ObjectIdentifier(ENVELOPE_ENCRYPTION_ALGORITHM_OID);

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] abyte0 = new byte[ENVELOPE_ENCRYPTION_KEY_LENGTH / 8];
            rng.GetBytes(abyte0);
            
            byte[] iv = IVGenerator.GetIV(16);
            ICryptoTransform encryptor = Aes.Create().CreateEncryptor(abyte0, iv);

            byte[] abyte1 = encryptor.TransformFinalBlock(@in, 0, @in.Length);

            // AES-256-CBC takes an octet string with the IV in the parameters field
            IASN1Encodable envelopeAlgoParams = FACTORY.CreateDEROctetString(iv);
            parameters.abyte0 = abyte0;
            parameters.abyte1 = abyte1;
            parameters.algorithmIdentifier = FACTORY.CreateAlgorithmIdentifier(derob, envelopeAlgoParams);
            return parameters;
        }

        internal class DERForRecipientParams {
            internal byte[] abyte0;
            internal byte[] abyte1;
            internal IAlgorithmIdentifier algorithmIdentifier;
        }
    }
}
