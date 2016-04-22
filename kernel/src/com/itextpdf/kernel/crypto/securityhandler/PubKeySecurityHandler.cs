/*
$Id: e451f24178ccc59d93295befdd150801617980a5 $

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System.Collections.Generic;
using System.IO;
using com.itextpdf.io.util;
using com.itextpdf.kernel;
using com.itextpdf.kernel.pdf;
using com.itextpdf.kernel.security;
using java.security;
using java.security.cert;
using javax.crypto;
using org.bouncycastle.asn1;
using org.bouncycastle.asn1.cms;
using org.bouncycastle.asn1.pkcs;
using org.bouncycastle.asn1.x509;
using org.bouncycastle.cert;
using org.bouncycastle.cms;

namespace com.itextpdf.kernel.crypto.securityhandler
{
	/// <author>Aiken Sam (aikensam@ieee.org)</author>
	public abstract class PubKeySecurityHandler : SecurityHandler
	{
		private const int SEED_LENGTH = 20;

		private IList<PublicKeyRecipient> recipients = null;

		private byte[] seed = new byte[SEED_LENGTH];

		public PubKeySecurityHandler()
		{
			KeyGenerator key;
			try
			{
				key = KeyGenerator.GetInstance("AES");
				key.Init(192, new SecureRandom());
				SecretKey sk = key.GenerateKey();
				System.Array.Copy(sk.GetEncoded(), 0, seed, 0, SEED_LENGTH);
			}
			catch (NoSuchAlgorithmException)
			{
				// create the 20 bytes seed
				seed = SecureRandom.GetSeed(SEED_LENGTH);
			}
			recipients = new List<PublicKeyRecipient>();
		}

		protected internal virtual byte[] ComputeGlobalKey(String messageDigestAlgorithm, 
			bool encryptMetadata)
		{
			MessageDigest md;
			byte[] encodedRecipient;
			try
			{
				md = MessageDigest.GetInstance(messageDigestAlgorithm);
				md.Update(GetSeed());
				for (int i = 0; i < GetRecipientsSize(); i++)
				{
					encodedRecipient = GetEncodedRecipient(i);
					md.Update(encodedRecipient);
				}
				if (!encryptMetadata)
				{
					md.Update(new byte[] { unchecked((byte)255), unchecked((byte)255), unchecked((byte
						)255), unchecked((byte)255) });
				}
			}
			catch (Exception e)
			{
				throw new PdfException(PdfException.PdfEncryption, e);
			}
			return md.Digest();
		}

		protected internal static byte[] ComputeGlobalKeyOnReading(PdfDictionary encryptionDictionary
			, PrivateKey certificateKey, Certificate certificate, String certificateKeyProvider
			, ExternalDecryptionProcess externalDecryptionProcess, bool encryptMetadata, String
			 digestAlgorithm)
		{
			PdfArray recipients = encryptionDictionary.GetAsArray(PdfName.Recipients);
			if (recipients == null)
			{
				recipients = encryptionDictionary.GetAsDictionary(PdfName.CF).GetAsDictionary(PdfName
					.DefaultCryptFilter).GetAsArray(PdfName.Recipients);
			}
			bool foundRecipient = false;
			byte[] envelopedData = null;
			X509CertificateHolder certHolder;
			try
			{
				certHolder = new X509CertificateHolder(certificate.GetEncoded());
			}
			catch (Exception f)
			{
				throw new PdfException(PdfException.PdfDecryption, f);
			}
			if (externalDecryptionProcess == null)
			{
				for (int i = 0; i < recipients.Size(); i++)
				{
					PdfString recipient = recipients.GetAsString(i);
					CMSEnvelopedData data;
					try
					{
						data = new CMSEnvelopedData(recipient.GetValueBytes());
						IEnumerator<RecipientInformation> recipientCertificatesIt = data.GetRecipientInfos
							().GetRecipients().GetEnumerator();
						while (recipientCertificatesIt.MoveNext())
						{
							RecipientInformation recipientInfo = recipientCertificatesIt.Current;
							if (recipientInfo.GetRID().Match(certHolder) && !foundRecipient)
							{
								envelopedData = PdfEncryptor.GetContent(recipientInfo, certificateKey, certificateKeyProvider
									);
								foundRecipient = true;
							}
						}
					}
					catch (Exception f)
					{
						throw new PdfException(PdfException.PdfDecryption, f);
					}
				}
			}
			else
			{
				for (int i = 0; i < recipients.Size(); i++)
				{
					PdfString recipient = recipients.GetAsString(i);
					CMSEnvelopedData data;
					try
					{
						data = new CMSEnvelopedData(recipient.GetValueBytes());
						RecipientInformation recipientInfo = data.GetRecipientInfos().Get(externalDecryptionProcess
							.GetCmsRecipientId());
						if (recipientInfo != null)
						{
							envelopedData = recipientInfo.GetContent(externalDecryptionProcess.GetCmsRecipient
								());
							foundRecipient = true;
						}
					}
					catch (Exception f)
					{
						throw new PdfException(PdfException.PdfDecryption, f);
					}
				}
			}
			if (!foundRecipient || envelopedData == null)
			{
				throw new PdfException(PdfException.BadCertificateAndKey);
			}
			byte[] encryptionKey;
			MessageDigest md;
			try
			{
				md = MessageDigest.GetInstance(digestAlgorithm);
				md.Update(envelopedData, 0, 20);
				for (int i = 0; i < recipients.Size(); i++)
				{
					byte[] encodedRecipient = recipients.GetAsString(i).GetValueBytes();
					md.Update(encodedRecipient);
				}
				if (!encryptMetadata)
				{
					md.Update(new byte[] { unchecked((byte)255), unchecked((byte)255), unchecked((byte
						)255), unchecked((byte)255) });
				}
				encryptionKey = md.Digest();
			}
			catch (Exception f)
			{
				throw new PdfException(PdfException.PdfDecryption, f);
			}
			return encryptionKey;
		}

		protected internal virtual void AddAllRecipients(Certificate[] certs, int[] permissions
			)
		{
			if (certs != null)
			{
				for (int i = 0; i < certs.Length; i++)
				{
					AddRecipient(certs[i], permissions[i]);
				}
			}
		}

		protected internal virtual PdfArray CreateRecipientsArray()
		{
			PdfArray recipients;
			try
			{
				recipients = GetEncodedRecipients();
			}
			catch (Exception e)
			{
				throw new PdfException(PdfException.PdfEncryption, e);
			}
			return recipients;
		}

		protected internal abstract void SetPubSecSpecificHandlerDicEntries(PdfDictionary
			 encryptionDictionary, bool encryptMetadata, bool embeddedFilesOnly);

		protected internal abstract String GetDigestAlgorithm();

		protected internal abstract void InitKey(byte[] globalKey, int keyLength);

		protected internal virtual void InitKeyAndFillDictionary(PdfDictionary encryptionDictionary
			, Certificate[] certs, int[] permissions, bool encryptMetadata, bool embeddedFilesOnly
			)
		{
			AddAllRecipients(certs, permissions);
			int keyLen = encryptionDictionary.GetAsInt(PdfName.Length);
			int keyLength = keyLen != null ? keyLen : 40;
			String digestAlgorithm = GetDigestAlgorithm();
			byte[] digest = ComputeGlobalKey(digestAlgorithm, encryptMetadata);
			InitKey(digest, keyLength);
			SetPubSecSpecificHandlerDicEntries(encryptionDictionary, encryptMetadata, embeddedFilesOnly
				);
		}

		protected internal virtual void InitKeyAndReadDictionary(PdfDictionary encryptionDictionary
			, Key certificateKey, Certificate certificate, String certificateKeyProvider, ExternalDecryptionProcess
			 externalDecryptionProcess, bool encryptMetadata)
		{
			String digestAlgorithm = GetDigestAlgorithm();
			byte[] encryptionKey = ComputeGlobalKeyOnReading(encryptionDictionary, (PrivateKey
				)certificateKey, certificate, certificateKeyProvider, externalDecryptionProcess, 
				encryptMetadata, digestAlgorithm);
			int keyLen = encryptionDictionary.GetAsInt(PdfName.Length);
			int keyLength = keyLen != null ? keyLen : 40;
			InitKey(encryptionKey, keyLength);
		}

		private void AddRecipient(Certificate cert, int permission)
		{
			recipients.Add(new PublicKeyRecipient(cert, permission));
		}

		private byte[] GetSeed()
		{
			return seed.Clone();
		}

		private int GetRecipientsSize()
		{
			return recipients.Count;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="java.security.GeneralSecurityException"/>
		private byte[] GetEncodedRecipient(int index)
		{
			//Certificate certificate = recipient.getX509();
			PublicKeyRecipient recipient = recipients[index];
			byte[] cms = recipient.GetCms();
			if (cms != null)
			{
				return cms;
			}
			Certificate certificate = recipient.GetCertificate();
			//constants permissions: PdfWriter.AllowCopy | PdfWriter.AllowPrinting | PdfWriter.AllowScreenReaders | PdfWriter.AllowAssembly;
			int permission = recipient.GetPermission();
			// NOTE! Added while porting to itext7
			// Previous strange code was:
			// int revision = 3;
			// permission |= revision == 3 ? 0xfffff0c0 : 0xffffffc0;
			// revision value never changed, so code have been replaced to this:
			permission |= 0xfffff0c0;
			permission &= 0xfffffffc;
			permission += 1;
			byte[] pkcs7input = new byte[24];
			byte one = unchecked((byte)permission);
			byte two = unchecked((byte)(permission >> 8));
			byte three = unchecked((byte)(permission >> 16));
			byte four = unchecked((byte)(permission >> 24));
			System.Array.Copy(seed, 0, pkcs7input, 0, 20);
			// put this seed in the pkcs7 input
			pkcs7input[20] = four;
			pkcs7input[21] = three;
			pkcs7input[22] = two;
			pkcs7input[23] = one;
			MemoryStream baos = new MemoryStream();
			DEROutputStream k = new DEROutputStream(baos);
			ASN1Primitive obj = CreateDERForRecipient(pkcs7input, (X509Certificate)certificate
				);
			k.WriteObject(obj);
			cms = baos.ToArray();
			recipient.SetCms(cms);
			return cms;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="java.security.GeneralSecurityException"/>
		private PdfArray GetEncodedRecipients()
		{
			PdfArray EncodedRecipients = new PdfArray();
			byte[] cms;
			for (int i = 0; i < recipients.Count; i++)
			{
				try
				{
					cms = GetEncodedRecipient(i);
					EncodedRecipients.Add(new PdfLiteral(StreamUtil.CreateEscapedString(cms)));
				}
				catch (GeneralSecurityException)
				{
					EncodedRecipients = null;
					// break was added while porting to itext7
					break;
				}
				catch (System.IO.IOException)
				{
					EncodedRecipients = null;
					// break was added while porting to itext7
					break;
				}
			}
			return EncodedRecipients;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="java.security.GeneralSecurityException"/>
		private ASN1Primitive CreateDERForRecipient(byte[] @in, X509Certificate cert)
		{
			String s = "1.2.840.113549.3.2";
			AlgorithmParameterGenerator algorithmparametergenerator = AlgorithmParameterGenerator
				.GetInstance(s);
			AlgorithmParameters algorithmparameters = algorithmparametergenerator.GenerateParameters
				();
			MemoryStream bytearrayinputstream = new MemoryStream(algorithmparameters.GetEncoded
				("ASN.1"));
			ASN1InputStream asn1inputstream = new ASN1InputStream(bytearrayinputstream);
			ASN1Primitive derobject = asn1inputstream.ReadObject();
			KeyGenerator keygenerator = KeyGenerator.GetInstance(s);
			keygenerator.Init(128);
			SecretKey secretkey = keygenerator.GenerateKey();
			Cipher cipher = Cipher.GetInstance(s);
			cipher.Init(1, secretkey, algorithmparameters);
			byte[] abyte1 = cipher.DoFinal(@in);
			DEROctetString deroctetstring = new DEROctetString(abyte1);
			KeyTransRecipientInfo keytransrecipientinfo = ComputeRecipientInfo(cert, secretkey
				.GetEncoded());
			DERSet derset = new DERSet(new RecipientInfo(keytransrecipientinfo));
			AlgorithmIdentifier algorithmidentifier = new AlgorithmIdentifier(new ASN1ObjectIdentifier
				(s), derobject);
			EncryptedContentInfo encryptedcontentinfo = new EncryptedContentInfo(PKCSObjectIdentifiers
				.data, algorithmidentifier, deroctetstring);
			EnvelopedData env = new EnvelopedData(null, derset, encryptedcontentinfo, (ASN1Set
				)null);
			ContentInfo contentinfo = new ContentInfo(PKCSObjectIdentifiers.envelopedData, env
				);
			return contentinfo.ToASN1Primitive();
		}

		/// <exception cref="java.security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		private KeyTransRecipientInfo ComputeRecipientInfo(X509Certificate x509certificate
			, byte[] abyte0)
		{
			ASN1InputStream asn1inputstream = new ASN1InputStream(new MemoryStream(x509certificate
				.GetTBSCertificate()));
			TBSCertificateStructure tbscertificatestructure = TBSCertificateStructure.GetInstance
				(asn1inputstream.ReadObject());
			System.Diagnostics.Debug.Assert(tbscertificatestructure != null);
			AlgorithmIdentifier algorithmidentifier = tbscertificatestructure.GetSubjectPublicKeyInfo
				().GetAlgorithm();
			IssuerAndSerialNumber issuerandserialnumber = new IssuerAndSerialNumber(tbscertificatestructure
				.GetIssuer(), tbscertificatestructure.GetSerialNumber().GetValue());
			Cipher cipher = Cipher.GetInstance(algorithmidentifier.GetAlgorithm().GetId());
			try
			{
				cipher.Init(1, x509certificate);
			}
			catch (InvalidKeyException)
			{
				cipher.Init(1, x509certificate.GetPublicKey());
			}
			DEROctetString deroctetstring = new DEROctetString(cipher.DoFinal(abyte0));
			RecipientIdentifier recipId = new RecipientIdentifier(issuerandserialnumber);
			return new KeyTransRecipientInfo(recipId, algorithmidentifier, deroctetstring);
		}
	}
}
