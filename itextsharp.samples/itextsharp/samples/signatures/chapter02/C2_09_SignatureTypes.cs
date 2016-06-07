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
using Org.BouncyCastle.X509;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Annot;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Layout;
using iTextSharp.Layout.Property;
using iTextSharp.Signatures;
using Org.BouncyCastle.Pkcs;

namespace iTextSharp.Samples.Signatures.Chapter02
{
	public class C2_09_SignatureTypes : SignatureTest
	{
        public static readonly string KEYSTORE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/ks";

	    public static readonly char[] PASSWORD = "password".ToCharArray();

        public static readonly string SRC = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/pdfs/hello.pdf";

        public static readonly string DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/hello_level_{0}.pdf";

	    /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Sign(String src, String dest, X509Certificate[] chain, ICipherParameters
			 pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter
			, int certificationLevel, String reason, String location)
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
			Rectangle rect = new Rectangle(36, 648, 200, 100);
			appearance.SetPageRect(rect).SetPageNumber(1);
			signer.SetFieldName("sig");
			signer.SetCertificationLevel(certificationLevel);
			// Creating the signature
			PrivateKeySignature pks = new PrivateKeySignature(pk, digestAlgorithm);
			signer.SignDetached(pks, chain, null, null, null, 0, subfilter);
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void AddText(String src, String dest)
		{
			PdfReader reader = new PdfReader(src);
			PdfDocument pdfDoc = new PdfDocument(reader, new PdfWriter(dest), new StampingProperties
				().UseAppendMode());
			new Canvas(new PdfCanvas(pdfDoc.GetFirstPage()), pdfDoc, pdfDoc.GetFirstPage().GetPageSize
				()).ShowTextAligned("TOP SECRET", 36, 820, TextAlignment.LEFT);
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void AddAnnotation(String src, String dest)
		{
			PdfReader reader = new PdfReader(src);
			PdfDocument pdfDoc = new PdfDocument(reader, new PdfWriter(dest), new StampingProperties
				().UseAppendMode());
			PdfAnnotation comment = new PdfTextAnnotation(new Rectangle(200, 800, 50, 20)).SetIconName
				(new PdfName("Comment")).SetTitle(new PdfString("Finally Signed!")).SetContents(
				"Bruno Specimen has finally signed the document").SetOpen(true);
			pdfDoc.GetFirstPage().AddAnnotation(comment);
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void AddWrongAnnotation(String src, String dest)
		{
			PdfReader reader = new PdfReader(src);
			PdfDocument pdfDoc = new PdfDocument(reader, new PdfWriter(dest));
			PdfAnnotation comment = new PdfTextAnnotation(new Rectangle(200, 800, 50, 20)).SetIconName
				(new PdfName("Comment")).SetTitle(new PdfString("Finally Signed!")).SetContents(
				"Bruno Specimen has finally signed the document").SetOpen(true);
			pdfDoc.GetFirstPage().AddAnnotation(comment);
			pdfDoc.Close();
		}

		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void SignAgain(String src, String dest, X509Certificate[] chain, ICipherParameters
			 pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter
			, String reason, String location)
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
			Rectangle rect = new Rectangle(36, 700, 200, 100);
			appearance.SetPageRect(rect).SetPageNumber(1);
			signer.SetFieldName("Signature2");
			// Creating the signature
			PrivateKeySignature pks = new PrivateKeySignature(pk, digestAlgorithm);
			signer.SignDetached(pks, chain, null, null, null, 0, subfilter);
		}

		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
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

			
			C2_09_SignatureTypes app = new C2_09_SignatureTypes();
			// TODO DEVSIX-488
			app.Sign(SRC, String.Format(DEST, 1), chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, PdfSigner.NOT_CERTIFIED, "Test 1", "Ghent"
				);
			app.Sign(SRC, String.Format(DEST, 2), chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, PdfSigner.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS
				, "Test 1", "Ghent");
			app.Sign(SRC, String.Format(DEST, 3), chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, PdfSigner.CERTIFIED_FORM_FILLING, "Test 1"
				, "Ghent");
			app.Sign(SRC, String.Format(DEST, 4), chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, PdfSigner.CERTIFIED_NO_CHANGES_ALLOWED
				, "Test 1", "Ghent");
			app.AddAnnotation(String.Format(DEST, 1), String.Format(DEST, "1_annotated"));
			app.AddAnnotation(String.Format(DEST, 2), String.Format(DEST, "2_annotated"));
			app.AddAnnotation(String.Format(DEST, 3), String.Format(DEST, "3_annotated"));
			app.AddAnnotation(String.Format(DEST, 4), String.Format(DEST, "4_annotated"));
			app.AddWrongAnnotation(String.Format(DEST, 1), String.Format(DEST, "1_annotated_wrong"
				));
			app.AddWrongAnnotation(SRC, String.Format(DEST, "1_annotated_wrong"));
			app.AddText(String.Format(DEST, 1), String.Format(DEST, "1_text"));
			app.SignAgain(String.Format(DEST, 1), String.Format(DEST, "1_double"), chain, pk, 
				DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, "Second signature test"
				, "Gent");
			app.SignAgain(String.Format(DEST, 2), String.Format(DEST, "2_double"), chain, pk, 
				DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, "Second signature test"
				, "Gent");
			app.SignAgain(String.Format(DEST, 3), String.Format(DEST, "3_double"), chain, pk, 
				DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, "Second signature test"
				, "Gent");
			app.SignAgain(String.Format(DEST, 4), String.Format(DEST, "4_double"), chain, pk, 
				DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, "Second signature test"
				, "Gent");
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		[NUnit.Framework.Test]
		public virtual void RunTest() {
            CreateDestinationFolder(NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/");
			C2_09_SignatureTypes.Main(null);
			String[] resultFiles = new String[] { "hello_level_1.pdf", "hello_level_2.pdf", "hello_level_3.pdf"
				, "hello_level_4.pdf", "hello_level_2_annotated.pdf", "hello_level_3_annotated.pdf"
				, "hello_level_4_annotated.pdf", "hello_level_1_text.pdf", "hello_level_1_double.pdf"
				, "hello_level_2_double.pdf", "hello_level_3_double.pdf", "hello_level_4_double.pdf"
				 };
			// this document's signature is not broken, that's why verifier doesn't show any errors;
			// this document is invalid from certificate point of view, which is not checked by itext
			String destPath = String.Format(outPath, "chapter02");
			String comparePath = String.Format(cmpPath, "chapter02");
			String[] errors = new String[resultFiles.Length];
			bool error = false;
			int indexOfInvalidFile = 4;
            Dictionary<int, IList<Rectangle>> ignoredAreas = new Dictionary<int, IList<Rectangle>> { { 1, iTextSharp.IO.Util.JavaUtil.ArraysAsList(new Rectangle(38f, 758f, 110f, 
						763f), new Rectangle(38f, 710f, 110f, 715f)) } };

			for (int i = 0; i < resultFiles.Length; i++)
			{
				String resultFile = resultFiles[i];
				String fileErrors = CheckForErrors(destPath + resultFile, comparePath + "cmp_" + 
					resultFile, destPath, ignoredAreas);
				if (i == indexOfInvalidFile)
				{
					if (fileErrors == null)
					{
						errors[i] = "Document signature was expected to be invalid.";
						error = true;
					}
				}
				else
				{
					if (fileErrors != null)
					{
						errors[i] = fileErrors;
						error = true;
					}
				}
			}
			if (error)
			{
				NUnit.Framework.Assert.Fail(AccumulateErrors(errors));
			}
		}
	}
}
