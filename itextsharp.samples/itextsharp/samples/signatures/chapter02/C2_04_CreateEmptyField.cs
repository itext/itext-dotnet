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
using iTextSharp.Forms;
using iTextSharp.Forms.Fields;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Annot;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Pdf.Xobject;
using iTextSharp.Layout;
using iTextSharp.Layout.Element;
using iTextSharp.Layout.Property;
using iTextSharp.Samples.Signatures;
using iTextSharp.Signatures;
using Org.BouncyCastle.Pkcs;

namespace iTextSharp.Samples.Signatures.Chapter02
{
	public class C2_04_CreateEmptyField : SignatureTest
	{
        public static readonly string KEYSTORE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/ks";

	    public static readonly char[] PASSWORD = "password".ToCharArray();

        public static readonly string SRC = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/pdfs/hello.pdf";

        public static readonly string DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/field_signed.pdf";

        public static readonly string UNSIGNED = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/hello_empty.pdf";

	    public const String SIGNAME = "Signature1";

        public static readonly string UNSIGNED2 = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/hello_empty2.pdf";

	    /// <exception cref="System.IO.IOException"/>
		public virtual void CreatePdf(String filename)
		{
			PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
			Document doc = new Document(pdfDoc);
			doc.Add(new Paragraph("Hello World!"));
			// create a signature form field
			PdfFormField field = PdfFormField.CreateSignature(pdfDoc, new Rectangle(72, 632, 
				200, 100));
			field.SetFieldName(SIGNAME);
			// set the widget properties
			field.SetPage(1);
			field.GetWidgets()[0].SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT).SetFlags(PdfAnnotation
				.PRINT);
			PdfDictionary mkDictionary = field.GetWidgets()[0].GetAppearanceCharacteristics();
			if (null == mkDictionary)
			{
				mkDictionary = new PdfDictionary();
			}
			PdfArray black = new PdfArray();
			black.Add(new PdfNumber(iTextSharp.Kernel.Color.Color.BLACK.GetColorValue()[0]));
			black.Add(new PdfNumber(iTextSharp.Kernel.Color.Color.BLACK.GetColorValue()[1]));
			black.Add(new PdfNumber(iTextSharp.Kernel.Color.Color.BLACK.GetColorValue()[2]));
			mkDictionary.Put(PdfName.BC, black);
			PdfArray white = new PdfArray();
			black.Add(new PdfNumber(iTextSharp.Kernel.Color.Color.WHITE.GetColorValue()[0]));
			black.Add(new PdfNumber(iTextSharp.Kernel.Color.Color.WHITE.GetColorValue()[1]));
			black.Add(new PdfNumber(iTextSharp.Kernel.Color.Color.WHITE.GetColorValue()[2]));
			mkDictionary.Put(PdfName.BG, white);
			field.GetWidgets()[0].SetAppearanceCharacteristics(mkDictionary);
			// add the field
			PdfAcroForm.GetAcroForm(pdfDoc, true).AddField(field);
			// maybe you want to define an appearance
			Rectangle rect = new Rectangle(0, 0, 200, 100);
			PdfFormXObject xObject = new PdfFormXObject(rect);
			PdfCanvas canvas = new PdfCanvas(xObject, pdfDoc);
			canvas.SetStrokeColor(iTextSharp.Kernel.Color.Color.BLUE).SetFillColor(iTextSharp.Kernel.Color.Color
				.LIGHT_GRAY).Rectangle(0.5f, 0.5f, 199.5f, 99.5f).FillStroke().SetFillColor(iTextSharp.Kernel.Color.Color
				.BLUE);
			new iTextSharp.Layout.Canvas(canvas, pdfDoc, rect).ShowTextAligned("SIGN HERE", 100
				, 50, TextAlignment.CENTER, (float)(Math.PI / 180) * 25);
			// TODO Acrobat does not render new appearance (Foxit however does)
			field.GetWidgets()[0].SetNormalAppearance(xObject.GetPdfObject());
			// Close the document
			doc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void AddField(String src, String dest)
		{
			PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
			// create a signature form field
			PdfSignatureFormField field = PdfFormField.CreateSignature(pdfDoc, new Rectangle(
				72, 632, 200, 100));
			field.SetFieldName(SIGNAME);
			// set the widget properties
			field.GetWidgets()[0].SetHighlightMode(PdfAnnotation.HIGHLIGHT_OUTLINE).SetFlags(
				PdfAnnotation.PRINT);
			// add the field
			PdfAcroForm.GetAcroForm(pdfDoc, true).AddField(field);
			// close the document
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		public static void Main(String[] args)
		{
            Directory.CreateDirectory(NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/");
			C2_04_CreateEmptyField appCreate = new C2_04_CreateEmptyField();
			appCreate.CreatePdf(UNSIGNED);
			appCreate.AddField(SRC, UNSIGNED2);

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

			C2_03_SignEmptyField appSign = new C2_03_SignEmptyField();
			appSign.Sign(UNSIGNED, SIGNAME, DEST, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, "Test", "Ghent");
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		[NUnit.Framework.Test]
		public virtual void RunTest()
		{
			C2_04_CreateEmptyField.Main(null);
			String[] resultFiles = new String[] { "field_signed.pdf", "hello_empty.pdf", "hello_empty2.pdf"
				 };
			String destPath = String.Format(outPath, "chapter02");
			String comparePath = String.Format(cmpPath, "chapter02");
			String[] errors = new String[resultFiles.Length];
			bool error = false;
            Dictionary<int, IList<Rectangle>> ignoredAreas = new Dictionary<int, IList<Rectangle>> {{1, iTextSharp.IO.Util.JavaUtil.ArraysAsList(new Rectangle(72, 632, 200, 100))}};
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
