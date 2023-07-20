/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    internal sealed class EncryptionUtils {
        // 256-bit AES-CBC, PKCS#5 padding
        // Not ideal, but the best that the PDF standard allows.
        public const String ENVELOPE_ENCRYPTION_ALGORITHM_OID = "2.16.840.1.101.3.4.1.42";
        public const int ENVELOPE_ENCRYPTION_KEY_LENGTH = 256;

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();
        
        private static readonly ICollection<string> UNSUPPORTED_ALGORITHMS = new HashSet<string>();

        static EncryptionUtils() {
            UNSUPPORTED_ALGORITHMS.Add("1.2.840.10045.2.1");
            UNSUPPORTED_ALGORITHMS.Add("ECDSA");
        }

        internal static byte[] GenerateSeed(int seedLength) {
            return IVGenerator.GetIV(seedLength);
        }

        internal static byte[] FetchEnvelopedData(IPrivateKey certificateKey, IX509Certificate certificate, PdfArray recipients) {
            bool foundRecipient = false;
            byte[] envelopedData = null;
            if (certificateKey != null && UNSUPPORTED_ALGORITHMS.Contains(certificateKey.GetAlgorithm())) {
                throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.ALGORITHM_IS_NOT_SUPPORTED, certificateKey.GetAlgorithm()));
            }
            for (int i = 0; i < recipients.Size(); i++) {
                try {
                    PdfString recipient = recipients.GetAsString(i);
                    ICmsEnvelopedData data = FACTORY.CreateCMSEnvelopedData(recipient.GetValueBytes());
                    foreach (IRecipientInformation recipientInfo in data.GetRecipientInfos().GetRecipients()) {
                        if (recipientInfo.GetRID().Match(certificate) && !foundRecipient) { 
                            envelopedData = recipientInfo.GetContent(certificateKey); 
                            foundRecipient = true;
                        }
                    }
                } catch (Exception f) {
                    // First check if the feature is supported, it will throw if not
                    // Exact algorithm doesn't matter currently
                    BouncyCastleFactoryCreator.GetFactory().IsEncryptionFeatureSupported(0, true);
                    // Throw the original exception if the feature is supported
                    throw new PdfException(KernelExceptionMessageConstant.PDF_DECRYPTION, f);
                }
            }
            if (!foundRecipient || envelopedData == null) {
                throw new PdfException(KernelExceptionMessageConstant.BAD_CERTIFICATE_AND_KEY);
            }
            return envelopedData;
        }

        internal static byte[] CipherBytes(IX509Certificate x509Certificate, byte[] abyte0, IAlgorithmIdentifier algorithmidentifier) {
            string algorithm = algorithmidentifier.GetAlgorithm().GetId();
            if (UNSUPPORTED_ALGORITHMS.Contains(algorithm)) {
                throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.ALGORITHM_IS_NOT_SUPPORTED, algorithm));
            }
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

            IDerObjectIdentifier derob = FACTORY.CreateASN1ObjectIdentifier(ENVELOPE_ENCRYPTION_ALGORITHM_OID);

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] abyte0 = new byte[ENVELOPE_ENCRYPTION_KEY_LENGTH / 8];
            rng.GetBytes(abyte0);
            
            byte[] iv = IVGenerator.GetIV(16);
            ICryptoTransform encryptor = Aes.Create().CreateEncryptor(abyte0, iv);

            byte[] abyte1 = encryptor.TransformFinalBlock(@in, 0, @in.Length);

            // AES-256-CBC takes an octet string with the IV in the parameters field
            IAsn1Encodable envelopeAlgoParams = FACTORY.CreateDEROctetString(iv);
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
