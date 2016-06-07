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
using iTextSharp.Forms;
using iTextSharp.Forms.Fields;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Annot;
using iTextSharp.Layout;
using iTextSharp.Layout.Element;
using iTextSharp.Layout.Renderer;
using iTextSharp.Samples.Signatures;
using iTextSharp.Signatures;
using NUnit.Framework;
using Org.BouncyCastle.Pkcs;

namespace iTextSharp.Samples.Signatures.Chapter02
{
	public class C2_11_SignatureWorkflow : SignatureTest
	{
        public static readonly string FORM = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/form.pdf";

        public static readonly string ALICE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/alice";

        public static readonly string BOB = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/bob";

        public static readonly string CAROL = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/carol";

        public static readonly string DAVE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/dave";

        public static readonly string KEYSTORE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/ks";

	    public static readonly char[] PASSWORD = "password".ToCharArray();

        public static readonly string DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/step{0}_signed_by_{1}.pdf";

	    /// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		public static void Main(String[] args)
		{
			C2_11_SignatureWorkflow app = new C2_11_SignatureWorkflow();
			app.CreateForm();
			app.Certify(ALICE, FORM, "sig1", String.Format(DEST, 1, "alice"));
			app.FillOut(String.Format(DEST, 1, "alice"), String.Format(DEST, 2, "alice_and_filled_out_by_bob"
				), "approved_bob", "Read and Approved by Bob");
			app.Sign(BOB, String.Format(DEST, 2, "alice_and_filled_out_by_bob"), "sig2", String
				.Format(DEST, 3, "alice_and_bob"));
			app.FillOut(String.Format(DEST, 3, "alice_and_bob"), String.Format(DEST, 4, "alice_and_bob_filled_out_by_carol"
				), "approved_carol", "Read and Approved by Carol");
			app.Sign(CAROL, String.Format(DEST, 4, "alice_and_bob_filled_out_by_carol"), "sig3"
				, String.Format(DEST, 5, "alice_bob_and_carol"));
			app.FillOutAndSign(DAVE, String.Format(DEST, 5, "alice_bob_and_carol"), "sig4", "approved_dave"
				, "Read and Approved by Dave", String.Format(DEST, 6, "alice_bob_carol_and_dave"
				));
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void CreateForm()
		{
			PdfDocument pdfDoc = new PdfDocument(new PdfWriter(FORM));
			Document doc = new Document(pdfDoc);
			Table table = new Table(1);
			table.AddCell("Written by Alice");
			table.AddCell(CreateSignatureFieldCell("sig1"));
			table.AddCell("For approval by Bob");
			table.AddCell(CreateTextFieldCell("approved_bob"));
			table.AddCell(CreateSignatureFieldCell("sig2"));
			table.AddCell("For approval by Carol");
			table.AddCell(CreateTextFieldCell("approved_carol"));
			table.AddCell(CreateSignatureFieldCell("sig3"));
			table.AddCell("For approval by Dave");
			table.AddCell(CreateTextFieldCell("approved_dave"));
			table.AddCell(CreateSignatureFieldCell("sig4"));
			doc.Add(table);
			doc.Close();
		}

		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Certify(String keystore, String src, String name, String dest
			)
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

			
			/// Creating the reader and the signer
			PdfReader reader = new PdfReader(src);
			PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), true
				);
			// Setting signer options
			signer.SetFieldName(name);
			// TODO DEVSIX-488
			signer.SetCertificationLevel(PdfSigner.CERTIFIED_FORM_FILLING);
			// Creating the signature
			IExternalSignature pks = new PrivateKeySignature(pk, "SHA-256");
			signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS
				);
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void FillOut(String src, String dest, String name, String value)
		{
			PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest), new 
				StampingProperties().UseAppendMode());
			PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
			form.GetField(name).SetValue(value);
			form.GetField(name).SetReadOnly(true);
			pdfDoc.Close();
		}

		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Sign(String keystore, String src, String name, String dest)
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

			// Creating the reader and the signer
			PdfReader reader = new PdfReader(src);
			PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), true
				);
			// Setting signer options
			signer.SetFieldName(name);
			// Creating the signature
			IExternalSignature pks = new PrivateKeySignature(pk, "SHA-256");
			signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS
				);
		}

		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void FillOutAndSign(String keystore, String src, String name, String
			 fname, String value, String dest)
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

			
			// Creating the reader and the signer
			PdfReader reader = new PdfReader(src);
			PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), true
				);
			PdfAcroForm form = PdfAcroForm.GetAcroForm(signer.GetDocument(), true);
			form.GetField(fname).SetValue(value);
			form.GetField(fname).SetReadOnly(true);
			// Setting signer options
			signer.SetFieldName(name);
			// Creating the signature
			IExternalSignature pks = new PrivateKeySignature(pk, "SHA-256");
			signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS
				);
		}

		protected internal virtual Cell CreateTextFieldCell(String name)
		{
			Cell cell = new Cell();
			cell.SetHeight(20);
			cell.SetNextRenderer(new C2_11_SignatureWorkflow.TextFieldCellRenderer(this, cell
				, name));
			return cell;
		}

		protected internal virtual Cell CreateSignatureFieldCell(String name)
		{
			Cell cell = new Cell();
			cell.SetHeight(50);
			cell.SetNextRenderer(new C2_11_SignatureWorkflow.SignatureFieldCellRenderer(this, 
				cell, name));
			return cell;
		}

		internal class TextFieldCellRenderer : CellRenderer
		{
			public String name;

			public TextFieldCellRenderer(C2_11_SignatureWorkflow _enclosing, Cell modelElement
				, String name)
				: base(modelElement)
			{
				this._enclosing = _enclosing;
				this.name = name;
			}

			public override void Draw(DrawContext drawContext)
			{
				base.Draw(drawContext);
				PdfFormField field = PdfFormField.CreateText(drawContext.GetDocument(), this.GetOccupiedAreaBBox
					(), this.name);
				PdfAcroForm.GetAcroForm(drawContext.GetDocument(), true).AddField(field);
			}

			private readonly C2_11_SignatureWorkflow _enclosing;
		}

		internal class SignatureFieldCellRenderer : CellRenderer
		{
			public String name;

			public SignatureFieldCellRenderer(C2_11_SignatureWorkflow _enclosing, Cell modelElement
				, String name)
				: base(modelElement)
			{
				this._enclosing = _enclosing;
				this.name = name;
			}

			public override void Draw(DrawContext drawContext)
			{
				base.Draw(drawContext);
				PdfFormField field = PdfFormField.CreateSignature(drawContext.GetDocument(), this
					.GetOccupiedAreaBBox());
				field.SetFieldName(this.name);
				field.GetWidgets()[0].SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT);
				field.GetWidgets()[0].SetFlags(PdfAnnotation.PRINT);
				PdfAcroForm.GetAcroForm(drawContext.GetDocument(), true).AddField(field);
			}

			private readonly C2_11_SignatureWorkflow _enclosing;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		[NUnit.Framework.Test]
        [Ignore("DEVSIX-488")]
		public virtual void RunTest()
		{
            Directory.CreateDirectory(NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/");
			C2_11_SignatureWorkflow.Main(null);
			String[] resultFiles = new String[] { "step1_signed_by_alice.pdf", "step2_signed_by_alice_and_filled_out_by_bob.pdf"
				, "step3_signed_by_alice_and_bob.pdf", "step4_signed_by_alice_and_bob_filled_out_by_carol.pdf"
				, "step5_signed_by_alice_bob_and_carol.pdf", "step6_signed_by_alice_bob_carol_and_dave.pdf"
				 };
			String destPath = String.Format(outPath, "chapter02");
			String comparePath = String.Format(cmpPath, "chapter02");
			String[] errors = new String[resultFiles.Length];
			bool error = false;

            Dictionary<int, IList<Rectangle>> ignoredAreas = new Dictionary<int, IList<Rectangle>> { { 1, iTextSharp.IO.Util.JavaUtil.ArraysAsList(new Rectangle(38f, 743f, 215f, 
						759f), new Rectangle(38f, 676f, 215f, 692f), new Rectangle(38f, 611f, 215f, 627f
						)) } };
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
