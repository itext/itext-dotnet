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
using iTextSharp.Kernel.Pdf;
using iTextSharp.Signatures;

namespace iTextSharp.Samples.Signatures.Chapter05
{
	public class C5_01_SignatureIntegrity : SignatureTest
	{
        public static readonly string EXAMPLE1 = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/pdfs/hello_level_1_annotated.pdf";

        public static readonly string EXAMPLE2 = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/pdfs/step_4_signed_by_alice_bob_carol_and_dave.pdf";

        public static readonly string EXAMPLE3 = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/pdfs/step_6_signed_by_dave_broken_by_chuck.pdf";

	    public const String expectedOutput = "===== sig =====\r\n" + "Signature covers whole document: False\r\n"
			 + "Document revision: 1 of 2\r\n" + "Integrity check OK? True\r\n" + "\r\n" + "===== sig1 =====\r\n"
			 + "Signature covers whole document: False\r\n" + "Document revision: 1 of 4\r\n" + 
			"Integrity check OK? True\r\n" + "===== sig2 =====\r\n" + "Signature covers whole document: False\r\n"
			 + "Document revision: 2 of 4\r\n" + "Integrity check OK? True\r\n" + "===== sig3 =====\r\n"
			 + "Signature covers whole document: False\r\n" + "Document revision: 3 of 4\r\n" + 
			"Integrity check OK? True\r\n" + "===== sig4 =====\r\n" + "Signature covers whole document: True\r\n"
			 + "Document revision: 4 of 4\r\n" + "Integrity check OK? True\r\n" + "\r\n" + "===== sig1 =====\r\n"
			 + "Signature covers whole document: False\r\n" + "Document revision: 1 of 5\r\n" + 
			"Integrity check OK? True\r\n" + "===== sig2 =====\r\n" + "Signature covers whole document: False\r\n"
			 + "Document revision: 2 of 5\r\n" + "Integrity check OK? True\r\n" + "===== sig3 =====\r\n"
			 + "Signature covers whole document: False\r\n" + "Document revision: 3 of 5\r\n" + 
			"Integrity check OK? True\r\n" + "===== sig4 =====\r\n" + "Signature covers whole document: False\r\n"
			 + "Document revision: 4 of 5\r\n" + "Integrity check OK? True\r\n" + "\r\n";

		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual PdfPKCS7 VerifySignature(SignatureUtil signUtil, String name)
		{
			System.Console.Out.WriteLine("Signature covers whole document: " + signUtil.SignatureCoversWholeDocument
				(name));
			System.Console.Out.WriteLine("Document revision: " + signUtil.GetRevision(name) +
				 " of " + signUtil.GetTotalRevisions());
			PdfPKCS7 pkcs7 = signUtil.VerifySignature(name);
			System.Console.Out.WriteLine("Integrity check OK? " + pkcs7.Verify());
			return pkcs7;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		public virtual void VerifySignatures(String path)
		{
			// System.out.println(path);
			PdfDocument pdfDoc = new PdfDocument(new PdfReader(path));
			SignatureUtil signUtil = new SignatureUtil(pdfDoc);
			IList<String> names = signUtil.GetSignatureNames();
			foreach (String name in names)
			{
				System.Console.Out.WriteLine("===== " + name + " =====");
				VerifySignature(signUtil, name);
			}
			System.Console.Out.WriteLine();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		public static void Main(String[] args)
		{
			C5_01_SignatureIntegrity app = new C5_01_SignatureIntegrity();
			app.VerifySignatures(EXAMPLE1);
			app.VerifySignatures(EXAMPLE2);
			app.VerifySignatures(EXAMPLE3);
		}

		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Javax.Smartcardio.CardException"/>
		[NUnit.Framework.Test]
		public virtual void RunTest()
		{
			SetupSystemOutput();
			C5_01_SignatureIntegrity.Main(null);
			String sysOut = GetSystemOutput();
			NUnit.Framework.Assert.AreEqual(expectedOutput, sysOut, "Unexpected output");
		}
	}
}
