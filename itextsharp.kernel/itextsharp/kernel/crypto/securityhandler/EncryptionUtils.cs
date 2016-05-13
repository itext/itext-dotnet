using System;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Security;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace iTextSharp.Kernel.Crypto.Securityhandler
{
	internal class EncryptionUtils
	{
        internal static byte[] GenerateSeed(int seedLength) {
            return IVGenerator.GetIV(seedLength);
        }

        internal static byte[] FetchEnvelopedData(ICipherParameters certificateKey, X509Certificate certificate, String certificateKeyProvider,
                                     IExternalDecryptionProcess externalDecryptionProcess, PdfArray recipients) {
            bool foundRecipient = false;
            byte[] envelopedData = null;
            for (int i = 0; i < recipients.Size(); i++) {
                PdfString recipient = recipients.GetAsString(i);
                CmsEnvelopedData data = new CmsEnvelopedData(recipient.GetValueBytes());

                foreach (RecipientInformation recipientInfo in data.GetRecipientInfos().GetRecipients()) {
                    if (recipientInfo.RecipientID.Match(certificate) && !foundRecipient) {
                        envelopedData = recipientInfo.GetContent(certificateKey);
                        foundRecipient = true;
                    }
                }
            }
            if (!foundRecipient || envelopedData == null) {
                throw new PdfException(PdfException.BadCertificateAndKey);
            }
            return envelopedData;
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        internal static byte[] CipherBytes(X509Certificate x509Certificate, byte[] abyte0, AlgorithmIdentifier algorithmidentifier)
		{
            IBufferedCipher cipher = CipherUtilities.GetCipher(algorithmidentifier.ObjectID);
            cipher.Init(true, x509Certificate.GetPublicKey());
            byte[] outp = new byte[10000];
            int len = cipher.DoFinal(abyte0, outp, 0);
            byte[] abyte1 = new byte[len];
            System.Array.Copy(outp, 0, abyte1, 0, len);

            return abyte1;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		internal static DERForRecipientParams CalculateDERForRecipientParams(byte[] @in)
		{
			String s = "1.2.840.113549.3.2";
			DERForRecipientParams parameters = new DERForRecipientParams();

            byte[] outp = new byte[100];
            DerObjectIdentifier derob = new DerObjectIdentifier(s);
            byte[] abyte0 = IVGenerator.GetIV(16); // keyp
            IBufferedCipher cf = CipherUtilities.GetCipher(derob);
            KeyParameter kp = new KeyParameter(abyte0);
            byte[] iv = IVGenerator.GetIV(cf.GetBlockSize());
            ParametersWithIV piv = new ParametersWithIV(kp, iv);
            cf.Init(true, piv);
            int len = cf.DoFinal(@in, outp, 0);

            byte[] abyte1 = new byte[len];
            System.Array.Copy(outp, 0, abyte1, 0, len);

            Asn1EncodableVector ev = new Asn1EncodableVector();
            ev.Add(new DerInteger(58));
            ev.Add(new DerOctetString(iv));
            DerSequence seq = new DerSequence(ev);

            parameters.abyte0 = abyte0;
		    parameters.abyte1 = abyte1;
            parameters.algorithmIdentifier = new AlgorithmIdentifier(derob, seq);
            return parameters;
		}

		internal class DERForRecipientParams
		{
			internal byte[] abyte0;
			internal byte[] abyte1;
			internal AlgorithmIdentifier algorithmIdentifier;
		}
	}
}
