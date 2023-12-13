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
using System.Globalization;
using iText.Kernel.Pdf;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace iText.Kernel.Crypto.Securityhandler {
    internal sealed class EncryptionUtils {
        internal static byte[] GenerateSeed(int seedLength) {
            return IVGenerator.GetIV(seedLength);
        }

        internal static byte[] FetchEnvelopedData(ICipherParameters certificateKey, X509Certificate certificate, PdfArray recipients) {
            bool foundRecipient = false;
            byte[] envelopedData = null;
            for (int i = 0; i < recipients.Size(); i++) {
                try {
                    PdfString recipient = recipients.GetAsString(i);
                    CmsEnvelopedData data = new CmsEnvelopedData(recipient.GetValueBytes());

                    foreach (RecipientInformation recipientInfo in data.GetRecipientInfos().GetRecipients()) {
                        if (recipientInfo.RecipientID.Match(certificate) && !foundRecipient) {
                            envelopedData = recipientInfo.GetContent(certificateKey);
                            foundRecipient = true;
                        }
                    }
                } catch (Exception f) {
                    throw new PdfException(PdfException.PdfDecryption, f);
                }
            }
            if (!foundRecipient || envelopedData == null) {
                throw new PdfException(PdfException.BadCertificateAndKey);
            }
            return envelopedData;
        }

        internal static byte[] CipherBytes(X509Certificate x509Certificate, byte[] abyte0, AlgorithmIdentifier algorithmidentifier) {
            IBufferedCipher cipher = CipherUtilities.GetCipher(algorithmidentifier.ObjectID);
            cipher.Init(true, x509Certificate.GetPublicKey());
            byte[] outp = new byte[10000];
            int len = cipher.DoFinal(abyte0, outp, 0);
            byte[] abyte1 = new byte[len];
            Array.Copy(outp, 0, abyte1, 0, len);

            return abyte1;
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
            /*
             According to ISO 32000-2 (7.6.5.3 Public-key encryption algorithms) RC-2 algorithm is outdated
             and should be replaced with a safer one 256-bit AES-CBC:
                 The algorithms that shall be used to encrypt the enveloped data in the CMS object are:
                 - RC4 with key lengths up to 256-bits (deprecated);
                 - DES, Triple DES, RC2 with key lengths up to 128 bits (deprecated);
                 - 128-bit AES in Cipher Block Chaining (CBC) mode (deprecated);
                 - 192-bit AES in CBC mode (deprecated);
                 - 256-bit AES in CBC mode.
             */
            String s = "1.2.840.113549.3.2";
            DERForRecipientParams parameters = new DERForRecipientParams();

            byte[] outp = new byte[100];
            DerObjectIdentifier derob = new DerObjectIdentifier(s);
            // keyp
            byte[] abyte0 = IVGenerator.GetIV(16);
            IBufferedCipher cf = CipherUtilities.GetCipher(derob);
            KeyParameter kp = new KeyParameter(abyte0);
            byte[] iv = IVGenerator.GetIV(cf.GetBlockSize());
            ParametersWithIV piv = new ParametersWithIV(kp, iv);
            cf.Init(true, piv);
            int len = cf.DoFinal(@in, outp, 0);

            byte[] abyte1 = new byte[len];
            Array.Copy(outp, 0, abyte1, 0, len);

            Asn1EncodableVector ev = new Asn1EncodableVector();
            ev.Add(new DerInteger(58));
            ev.Add(new DerOctetString(iv));
            DerSequence seq = new DerSequence(ev);

            parameters.abyte0 = abyte0;
            parameters.abyte1 = abyte1;
            parameters.algorithmIdentifier = new AlgorithmIdentifier(derob, seq);
            return parameters;
        }

        internal class DERForRecipientParams {
            internal byte[] abyte0;
            internal byte[] abyte1;
            internal AlgorithmIdentifier algorithmIdentifier;
        }
    }
}
