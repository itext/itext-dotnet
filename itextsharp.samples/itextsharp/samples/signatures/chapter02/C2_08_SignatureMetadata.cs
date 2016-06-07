/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV

*/
/*
* This class is part of the white paper entitled
* "Digital Signatures for PDF documents"
* written by Bruno Lowagie
*
* For more info, go to: http://itextpdf.com/learn
*/
using System;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Samples.Signatures;
using iTextSharp.Signatures;
using Org.BouncyCastle.Pkcs;

namespace iTextSharp.Samples.Signatures.Chapter02
{
	public class C2_08_SignatureMetadata : SignatureTest
	{
        public static readonly string KEYSTORE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/ks";

	    public static readonly char[] PASSWORD = "password".ToCharArray();

        public static readonly string SRC = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/pdfs/hello_to_sign.pdf";

        public static readonly string DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/field_metadata.pdf";

	    /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Sign(String src, String name, String dest, X509Certificate[] 
			chain, ICipherParameters pk, String digestAlgorithm, PdfSigner.CryptoStandard
			 subfilter, String reason, String location, String contact, DateTime signDate, String
			 fullName)
		{
			// Creating the reader and the signer
			PdfReader reader = new PdfReader(src);
			PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), false
				);
			// Creating the appearance
			PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
			appearance.SetReason(reason);
			appearance.SetLocation(location);
			appearance.SetReuseAppearance(false);
			signer.SetFieldName(name);
			appearance.SetContact(contact);
			// Notice that this option is no more public due to its riskiness
			// appearance.setSignDate(signDate);
			signer.SetSignatureEvent(new _ISignatureEvent_78(fullName));
			// Creating the signature
			PrivateKeySignature pks = new PrivateKeySignature(pk, digestAlgorithm);
			signer.SignDetached(pks, chain, null, null, null, 0, subfilter);
		}

		private sealed class _ISignatureEvent_78 : PdfSigner.ISignatureEvent
		{
			public _ISignatureEvent_78(String fullName)
			{
				this.fullName = fullName;
			}

			public void GetSignatureDictionary(PdfSignature sig)
			{
				sig.Put(PdfName.Name, new PdfString(fullName));
			}

			private readonly String fullName;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		public static void Main(String[] args)
		{


            string alias = null;
            Pkcs12Store pk12;

            pk12 = new Pkcs12Store(new FileStream(KEYSTORE, FileMode.Open, FileAccess.Read), PASSWORD);

            foreach (var a in pk12.Aliases)
            {
                alias = ((string)a);
                if (pk12.IsKeyEntry(alias))
                    break;
            }
            ICipherParameters pk = pk12.GetKey(alias).Key;
            X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
            X509Certificate[] chain = new X509Certificate[ce.Length];
            for (int k = 0; k < ce.Length; ++k)
                chain[k] = ce[k].Certificate;

			
			C2_08_SignatureMetadata app = new C2_08_SignatureMetadata();
			app.Sign(SRC, "Signature1", String.Format(DEST, 1), chain, pk, DigestAlgorithms.SHA256
				, PdfSigner.CryptoStandard.CMS, "Test metadata", "Ghent", "555 123 456"
				, new DateTime(2012, 8, 5) , "Bruno L. Specimen");
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		[NUnit.Framework.Test]
		public virtual void RunTest()
		{
            CreateDestinationFolder(NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/");
			C2_08_SignatureMetadata.Main(null);
			String[] resultFiles = new String[] { "field_metadata.pdf" };
			String destPath = String.Format(outPath, "chapter02");
			String comparePath = String.Format(cmpPath, "chapter02");
			String[] errors = new String[resultFiles.Length];
			bool error = false;
            Dictionary<int, IList<Rectangle>> ignoredAreas = new Dictionary<int, IList<Rectangle>> { { 1, iTextSharp.IO.Util.JavaUtil.ArraysAsList(new Rectangle(46, 472, 287, 255)) } };

			for (int i = 0; i < resultFiles.Length; i++)
			{
				String resultFile = resultFiles[i];
				String fileErrors = CheckForErrors(destPath + resultFile, comparePath + "cmp_" + 
					resultFile, destPath, ignoredAreas);
				if (fileErrors != null)
				{
					errors[i] = fileErrors;
					error = true;
				}
			}
			if (error)
			{
				NUnit.Framework.Assert.Fail(AccumulateErrors(errors));
			}
		}
	}
}
