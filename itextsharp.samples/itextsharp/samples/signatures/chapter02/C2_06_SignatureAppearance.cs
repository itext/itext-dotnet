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
using iTextSharp.IO.Font;
using iTextSharp.IO.Image;
using iTextSharp.Kernel.Font;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Xobject;
using iTextSharp.Layout;
using iTextSharp.Layout.Element;
using iTextSharp.Layout.Property;
using iTextSharp.Samples.Signatures;
using iTextSharp.Signatures;
using Org.BouncyCastle.Pkcs;

namespace iTextSharp.Samples.Signatures.Chapter02
{
	public class C2_06_SignatureAppearance : SignatureTest
	{
        public static readonly string KEYSTORE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/ks";

	    public static readonly char[] PASSWORD = "password".ToCharArray();

        public static readonly string SRC = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/pdfs/hello_to_sign.pdf";

        public static readonly string DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/signature_appearance{0}.pdf";

        public static readonly string IMG = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/img/1t3xt.gif";

	    /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Sign1(String src, String name, String dest, X509Certificate[]
			 chain, ICipherParameters pk, String digestAlgorithm, PdfSigner.CryptoStandard
			 subfilter, String reason, String location)
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
			// Custom text and custom font
			appearance.SetLayer2Text("This document was signed by Bruno Specimen");
			appearance.SetLayer2Font(PdfFontFactory.CreateFont(FontConstants.TIMES_ROMAN));
			// Creating the signature
			IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
			signer.SignDetached(pks, chain, null, null, null, 0, subfilter);
		}

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
//        public virtual void Sign2(String src, String name, String dest, X509Certificate[]
//             chain, ICipherParameters pk, String digestAlgorithm, String provider, PdfSigner.CryptoStandard
//             subfilter, String reason, String location)
//        {
//            // Creating the reader and the signer
//            PdfReader reader = new PdfReader(src);
//            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), false
//                );
//            // Creating the appearance
//            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
//            appearance.SetReason(reason);
//            appearance.SetLocation(location);
//            appearance.SetReuseAppearance(false);
//            signer.SetFieldName(name);
//            // Creating the appearance for layer 2
//            PdfFormXObject n2 = appearance.GetLayer2();
//            // Custom text, custom font, and right-to-left writing
//            Text text = new Text("\u0644\u0648\u0631\u0627\u0646\u0633 \u0627\u0644\u0639\u0631\u0628"
//                );
//            text.SetFont(PdfFontFactory.CreateFont("./src/test/resources/font/NotoNaskhArabic-Regular.ttf"
//                , PdfEncodings.IDENTITY_H, true));
//            /*"C:/windows/fonts/arialuni.ttf"*/
//            text.SetBaseDirection(BaseDirection.RIGHT_TO_LEFT);
//            new Canvas(n2, signer.GetDocument()).Add(new Paragraph(text).SetTextAlignment(TextAlignment
//                .RIGHT));
//            // Creating the signature
//            PrivateKeySignature pks = new PrivateKeySignature(pk, digestAlgorithm);
//            IExternalDigest digest = new DigestUtilities();
//            signer.SignDetached(pks, chain, null, null, null, 0, subfilter);
//        }

		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Sign3(String src, String name, String dest, X509Certificate[]
			 chain, ICipherParameters pk, String digestAlgorithm, PdfSigner.CryptoStandard
			 subfilter, String reason, String location)
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
			// Custom text and background image
			appearance.SetLayer2Text("This document was signed by Bruno Specimen");
			appearance.SetImage(ImageDataFactory.Create(IMG));
			appearance.SetImageScale(1);
			// Creating the signature
			PrivateKeySignature pks = new PrivateKeySignature(pk, digestAlgorithm);
			signer.SignDetached(pks, chain, null, null, null, 0, subfilter);
		}

		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Sign4(String src, String name, String dest, X509Certificate[]
			 chain, ICipherParameters pk, String digestAlgorithm, PdfSigner.CryptoStandard
			 subfilter, String reason, String location)
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
			// Default text and scaled background image
			appearance.SetImage(ImageDataFactory.Create(IMG));
			appearance.SetImageScale(-1);
			// Creating the signature
			PrivateKeySignature pks = new PrivateKeySignature(pk, digestAlgorithm);
			signer.SignDetached(pks, chain, null, null, null, 0, subfilter);
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

			
			C2_06_SignatureAppearance app = new C2_06_SignatureAppearance();
			app.Sign1(SRC, "Signature1", String.Format(DEST, 1), chain, pk, DigestAlgorithms.
				SHA256, PdfSigner.CryptoStandard.CMS, "Custom appearance example"
				, "Ghent");
//			app.Sign2(SRC, "Signature1", String.Format(DEST, 2), chain, pk, DigestAlgorithms.
//				SHA256, PdfSigner.CryptoStandard.CMS, "Custom appearance example"
//				, "Ghent");
			app.Sign3(SRC, "Signature1", String.Format(DEST, 3), chain, pk, DigestAlgorithms.
				SHA256, PdfSigner.CryptoStandard.CMS, "Custom appearance example"
				, "Ghent");
			app.Sign4(SRC, "Signature1", String.Format(DEST, 4), chain, pk, DigestAlgorithms.
				SHA256, PdfSigner.CryptoStandard.CMS, "Custom appearance example"
				, "Ghent");
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		[NUnit.Framework.Test]
		public virtual void RunTest()
		{
			//Load the license file to use advanced typography features
            Directory.CreateDirectory(NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/");
			C2_06_SignatureAppearance.Main(null);
			String[] resultFiles = new String[] { "signature_appearance1.pdf"//, "signature_appearance2.pdf"
				, "signature_appearance3.pdf", "signature_appearance4.pdf" };
			String destPath = String.Format(outPath, "chapter02");
			String comparePath = String.Format(cmpPath, "chapter02");
			String[] errors = new String[resultFiles.Length];
			bool error = false;
			for (int i = 0; i < resultFiles.Length - 1; i++)
			{
				String resultFile = resultFiles[i];
				String fileErrors = CheckForErrors(destPath + resultFile, comparePath + "cmp_" + 
					resultFile, destPath, null);
				if (fileErrors != null)
				{
					errors[i] = fileErrors;
					error = true;
				}
			}
			// Compare the last documents only in signature data
            CompareSignatures(destPath + resultFiles[resultFiles.Length - 1], comparePath + "cmp_" + resultFiles[resultFiles.Length - 1]);
			if (null != GetErrorMessage())
			{
				errors[3] = GetErrorMessage();
				error = true;
			}
			if (error)
			{
				NUnit.Framework.Assert.Fail(AccumulateErrors(errors));
			}
		}
	}
}
