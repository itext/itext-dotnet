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
using System.Collections.Generic;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Security;
using iText.IO.Util;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    public abstract class PubKeySecurityHandler : SecurityHandler {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private const int SEED_LENGTH = 20;

        private IList<PublicKeyRecipient> recipients = null;

        private byte[] seed;

        protected internal PubKeySecurityHandler() {
            seed = EncryptionUtils.GenerateSeed(SEED_LENGTH);
            recipients = new List<PublicKeyRecipient>();
        }

        protected internal virtual byte[] ComputeGlobalKey(String messageDigestAlgorithm, bool encryptMetadata) {
            IDigest md;
            byte[] encodedRecipient;
            try {
                md = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest(messageDigestAlgorithm
                    );
                md.Update(GetSeed());
                for (int i = 0; i < GetRecipientsSize(); i++) {
                    encodedRecipient = GetEncodedRecipient(i);
                    md.Update(encodedRecipient);
                }
                if (!encryptMetadata) {
                    md.Update(new byte[] { (byte)255, (byte)255, (byte)255, (byte)255 });
                }
            }
            catch (PdfException pdfException) {
                throw;
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
            return md.Digest();
        }

        protected internal static byte[] ComputeGlobalKeyOnReading(PdfDictionary encryptionDictionary, IPrivateKey
             certificateKey, IX509Certificate certificate, bool encryptMetadata, String digestAlgorithm) {
            PdfArray recipients = encryptionDictionary.GetAsArray(PdfName.Recipients);
            if (recipients == null) {
                recipients = encryptionDictionary.GetAsDictionary(PdfName.CF).GetAsDictionary(PdfName.DefaultCryptFilter).
                    GetAsArray(PdfName.Recipients);
            }
            byte[] envelopedData = EncryptionUtils.FetchEnvelopedData(certificateKey, certificate, recipients);
            byte[] encryptionKey;
            IDigest md;
            try {
                md = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest(digestAlgorithm);
                md.Update(envelopedData, 0, 20);
                for (int i = 0; i < recipients.Size(); i++) {
                    byte[] encodedRecipient = recipients.GetAsString(i).GetValueBytes();
                    md.Update(encodedRecipient);
                }
                if (!encryptMetadata) {
                    md.Update(new byte[] { (byte)255, (byte)255, (byte)255, (byte)255 });
                }
                encryptionKey = md.Digest();
            }
            catch (Exception f) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_DECRYPTION, f);
            }
            return encryptionKey;
        }

        protected internal virtual void AddAllRecipients(IX509Certificate[] certs, int[] permissions) {
            if (certs != null) {
                for (int i = 0; i < certs.Length; i++) {
                    AddRecipient(certs[i], permissions[i]);
                }
            }
        }

        protected internal virtual PdfArray CreateRecipientsArray() {
            PdfArray recipients;
            try {
                recipients = GetEncodedRecipients();
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
            return recipients;
        }

        protected internal abstract void SetPubSecSpecificHandlerDicEntries(PdfDictionary encryptionDictionary, bool
             encryptMetadata, bool embeddedFilesOnly);

        protected internal abstract String GetDigestAlgorithm();

        protected internal abstract void InitKey(byte[] globalKey, int keyLength);

        protected internal virtual void InitKeyAndFillDictionary(PdfDictionary encryptionDictionary, IX509Certificate
            [] certs, int[] permissions, bool encryptMetadata, bool embeddedFilesOnly) {
            AddAllRecipients(certs, permissions);
            int? keyLen = encryptionDictionary.GetAsInt(PdfName.Length);
            int keyLength = keyLen != null ? (int)keyLen : 40;
            String digestAlgorithm = GetDigestAlgorithm();
            byte[] digest = ComputeGlobalKey(digestAlgorithm, encryptMetadata);
            InitKey(digest, keyLength);
            SetPubSecSpecificHandlerDicEntries(encryptionDictionary, encryptMetadata, embeddedFilesOnly);
        }

        protected internal virtual void InitKeyAndReadDictionary(PdfDictionary encryptionDictionary, IPrivateKey certificateKey
            , IX509Certificate certificate, bool encryptMetadata) {
            String digestAlgorithm = GetDigestAlgorithm();
            byte[] encryptionKey = ComputeGlobalKeyOnReading(encryptionDictionary, (IPrivateKey)certificateKey, certificate
                , encryptMetadata, digestAlgorithm);
            int? keyLen = encryptionDictionary.GetAsInt(PdfName.Length);
            int keyLength = keyLen != null ? (int)keyLen : 40;
            InitKey(encryptionKey, keyLength);
        }

        private void AddRecipient(IX509Certificate cert, int permission) {
            recipients.Add(new PublicKeyRecipient(cert, permission));
        }

        private byte[] GetSeed() {
            byte[] clonedSeed = new byte[seed.Length];
            Array.Copy(seed, 0, clonedSeed, 0, seed.Length);
            return clonedSeed;
        }

        private int GetRecipientsSize() {
            return recipients.Count;
        }

        private byte[] GetEncodedRecipient(int index) {
            //Certificate certificate = recipient.getX509();
            PublicKeyRecipient recipient = recipients[index];
            byte[] cms = recipient.GetCms();
            if (cms != null) {
                return cms;
            }
            IX509Certificate certificate = recipient.GetCertificate();
            //constants permissions: PdfWriter.AllowCopy | PdfWriter.AllowPrinting | PdfWriter.AllowScreenReaders |
            // PdfWriter.AllowAssembly;
            int permission = recipient.GetPermission();
            // NOTE! Added while porting to itext
            // Previous strange code was:
            // int revision = 3;
            // permission |= revision == 3 ? 0xfffff0c0 : 0xffffffc0;
            // revision value never changed, so code have been replaced to this:
            permission |= unchecked((int)(0xfffff0c0));
            permission &= unchecked((int)(0xfffffffc));
            permission += 1;
            byte[] pkcs7input = new byte[24];
            byte one = (byte)permission;
            byte two = (byte)(permission >> 8);
            byte three = (byte)(permission >> 16);
            byte four = (byte)(permission >> 24);
            // put this seed in the pkcs7 input
            Array.Copy(seed, 0, pkcs7input, 0, 20);
            pkcs7input[20] = four;
            pkcs7input[21] = three;
            pkcs7input[22] = two;
            pkcs7input[23] = one;
            MemoryStream baos = new MemoryStream();
            using (IDerOutputStream k = CryptoUtil.CreateAsn1OutputStream(baos, BOUNCY_CASTLE_FACTORY.CreateASN1Encoding
                ().GetDer())) {
                IAsn1Object obj = CreateDERForRecipient(pkcs7input, (IX509Certificate)certificate);
                k.WriteObject(obj);
            }
            cms = baos.ToArray();
            recipient.SetCms(cms);
            return cms;
        }

        private PdfArray GetEncodedRecipients() {
            PdfArray EncodedRecipients = new PdfArray();
            byte[] cms;
            for (int i = 0; i < recipients.Count; i++) {
                try {
                    cms = GetEncodedRecipient(i);
                    EncodedRecipients.Add(new PdfLiteral(StreamUtil.CreateEscapedString(cms)));
                }
                catch (AbstractGeneralSecurityException) {
                    EncodedRecipients = null;
                    // break was added while porting to itext
                    break;
                }
                catch (System.IO.IOException) {
                    EncodedRecipients = null;
                    break;
                }
            }
            return EncodedRecipients;
        }

        private IAsn1Object CreateDERForRecipient(byte[] @in, IX509Certificate cert) {
            EncryptionUtils.DERForRecipientParams parameters = EncryptionUtils.CalculateDERForRecipientParams(@in);
            IKeyTransRecipientInfo keytransrecipientinfo = ComputeRecipientInfo(cert, parameters.abyte0);
            IDerOctetString deroctetstring = BOUNCY_CASTLE_FACTORY.CreateDEROctetString(parameters.abyte1);
            IDerSet derset = BOUNCY_CASTLE_FACTORY.CreateDERSet(BOUNCY_CASTLE_FACTORY.CreateRecipientInfo(keytransrecipientinfo
                ));
            IEncryptedContentInfo encryptedcontentinfo = BOUNCY_CASTLE_FACTORY.CreateEncryptedContentInfo(BOUNCY_CASTLE_FACTORY
                .CreatePKCSObjectIdentifiers().GetData(), parameters.algorithmIdentifier, deroctetstring);
            IEnvelopedData env = BOUNCY_CASTLE_FACTORY.CreateEnvelopedData(BOUNCY_CASTLE_FACTORY.CreateNullOriginatorInfo
                (), derset, encryptedcontentinfo, BOUNCY_CASTLE_FACTORY.CreateNullASN1Set());
            IContentInfo contentinfo = BOUNCY_CASTLE_FACTORY.CreateContentInfo(BOUNCY_CASTLE_FACTORY.CreatePKCSObjectIdentifiers
                ().GetEnvelopedData(), env);
            return contentinfo.ToASN1Primitive();
        }

        private IKeyTransRecipientInfo ComputeRecipientInfo(IX509Certificate x509Certificate, byte[] abyte0) {
            ITbsCertificateStructure tbsCertificate;
            using (IAsn1InputStream asn1InputStream = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(x509Certificate
                .GetTbsCertificate()))) {
                tbsCertificate = BOUNCY_CASTLE_FACTORY.CreateTBSCertificate(asn1InputStream.ReadObject());
            }
            IAlgorithmIdentifier algorithmIdentifier = tbsCertificate.GetSubjectPublicKeyInfo().GetAlgorithm();
            IIssuerAndSerialNumber issuerAndSerialNumber = BOUNCY_CASTLE_FACTORY.CreateIssuerAndSerialNumber(tbsCertificate
                .GetIssuer(), tbsCertificate.GetSerialNumber().GetValue());
            byte[] cipheredBytes = EncryptionUtils.CipherBytes(x509Certificate, abyte0, algorithmIdentifier);
            IDerOctetString derOctetString = BOUNCY_CASTLE_FACTORY.CreateDEROctetString(cipheredBytes);
            IRecipientIdentifier recipId = BOUNCY_CASTLE_FACTORY.CreateRecipientIdentifier(issuerAndSerialNumber);
            return BOUNCY_CASTLE_FACTORY.CreateKeyTransRecipientInfo(recipId, algorithmIdentifier, derOctetString);
        }
    }
}
