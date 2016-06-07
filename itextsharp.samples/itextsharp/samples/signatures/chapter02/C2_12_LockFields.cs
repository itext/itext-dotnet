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
using iTextSharp.Layout;
using iTextSharp.Layout.Element;
using iTextSharp.Layout.Renderer;
using iTextSharp.Samples.Signatures;
using iTextSharp.Signatures;
using NUnit.Framework;
using Org.BouncyCastle.Pkcs;

namespace iTextSharp.Samples.Signatures.Chapter02
{
	public class C2_12_LockFields : SignatureTest
	{
        public static readonly string FORM = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/form_lock.pdf";

        public static readonly string ALICE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/alice";

        public static readonly string BOB = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/bob";

        public static readonly string CAROL = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/carol";

        public static readonly string DAVE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/dave";

        public static readonly string KEYSTORE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/ks";

	    public static readonly char[] PASSWORD = "password".ToCharArray();

        public static readonly string DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/step_{0}_signed_by_{1}.pdf";

	    /// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		public static void Main(String[] args)
		{
			C2_12_LockFields app = new C2_12_LockFields();
			app.CreateForm();
			app.Certify(ALICE, FORM, "sig1", String.Format(DEST, 1, "alice"));
			app.FillOutAndSign(BOB, String.Format(DEST, 1, "alice"), "sig2", "approved_bob", 
				"Read and Approved by Bob", String.Format(DEST, 2, "alice_and_bob"));
			app.FillOutAndSign(CAROL, String.Format(DEST, 2, "alice_and_bob"), "sig3", "approved_carol"
				, "Read and Approved by Carol", String.Format(DEST, 3, "alice_bob_and_carol"));
			app.FillOutAndSign(DAVE, String.Format(DEST, 3, "alice_bob_and_carol"), "sig4", "approved_dave"
				, "Read and Approved by Dave", String.Format(DEST, 4, "alice_bob_carol_and_dave"
				));
			app.FillOut(String.Format(DEST, 2, "alice_and_bob"), String.Format(DEST, 5, "alice_and_bob_broken_by_chuck"
				), "approved_bob", "Changed by Chuck");
			app.FillOut(String.Format(DEST, 4, "alice_bob_carol_and_dave"), String.Format(DEST
				, 6, "dave_broken_by_chuck"), "approved_carol", "Changed by Chuck");
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void CreateForm()
		{
			PdfDocument pdfDoc = new PdfDocument(new PdfWriter(FORM));
			Document doc = new Document(pdfDoc);
			Table table = new Table(1);
			table.AddCell("Written by Alice");
			table.AddCell(CreateSignatureFieldCell("sig1", null));
			table.AddCell("For approval by Bob");
			table.AddCell(CreateTextFieldCell("approved_bob"));
			PdfSigFieldLockDictionary Lock = new PdfSigFieldLockDictionary().SetFieldLock(PdfSigFieldLockDictionary.LockAction
				.INCLUDE, "sig1", "approved_bob", "sig2");
			table.AddCell(CreateSignatureFieldCell("sig2", Lock));
			table.AddCell("For approval by Carol");
			table.AddCell(CreateTextFieldCell("approved_carol"));
			Lock = new PdfSigFieldLockDictionary().SetFieldLock(PdfSigFieldLockDictionary.LockAction
				.EXCLUDE, "approved_dave", "sig4");
			table.AddCell(CreateSignatureFieldCell("sig3", Lock));
			table.AddCell("For approval by Dave");
			table.AddCell(CreateTextFieldCell("approved_dave"));
			Lock = new PdfSigFieldLockDictionary().SetDocumentPermissions(PdfSigFieldLockDictionary.LockPermissions
				.NO_CHANGES_ALLOWED);
			table.AddCell(CreateSignatureFieldCell("sig4", Lock));
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

			
			PdfReader reader = new PdfReader(src);
			PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), true
				);
			signer.SetFieldName(name);
			// TODO DEVSIX-488
			signer.SetCertificationLevel(PdfSigner.CERTIFIED_FORM_FILLING);
			PdfAcroForm form = PdfAcroForm.GetAcroForm(signer.GetDocument(), true);
			form.GetField(name).SetReadOnly(true);
			PrivateKeySignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
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
			form.GetField(name).SetReadOnly(true);
			form.GetField(fname).SetReadOnly(true);
			// Setting signer options
			signer.SetFieldName(name);
			// Creating the signature
			PrivateKeySignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
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
			PrivateKeySignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
			signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS
				);
		}

		protected internal virtual Cell CreateTextFieldCell(String name)
		{
			Cell cell = new Cell();
			cell.SetHeight(20);
			cell.SetNextRenderer(new C2_12_LockFields.TextFieldCellRenderer(this, cell, name)
				);
			return cell;
		}

		/// <exception cref="System.IO.IOException"/>
		protected internal virtual Cell CreateSignatureFieldCell(String name, PdfSigFieldLockDictionary
			 Lock)
		{
			Cell cell = new Cell();
			cell.SetHeight(50);
			cell.SetNextRenderer(new C2_12_LockFields.SignatureFieldCellRenderer(this, cell, 
				name, Lock));
			return cell;
		}

		internal class TextFieldCellRenderer : CellRenderer
		{
			public String name;

			public TextFieldCellRenderer(C2_12_LockFields _enclosing, Cell modelElement, String
				 name)
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

			private readonly C2_12_LockFields _enclosing;
		}

		internal class SignatureFieldCellRenderer : CellRenderer
		{
			public String name;

			public PdfSigFieldLockDictionary Lock;

			public SignatureFieldCellRenderer(C2_12_LockFields _enclosing, Cell modelElement, 
				String name, PdfSigFieldLockDictionary Lock)
				: base(modelElement)
			{
				this._enclosing = _enclosing;
				this.name = name;
				this.Lock = Lock;
			}

			public override void Draw(DrawContext drawContext)
			{
				base.Draw(drawContext);
				PdfFormField field = PdfFormField.CreateSignature(drawContext.GetDocument(), this
					.GetOccupiedAreaBBox());
				field.SetFieldName(this.name);
				if (this.Lock != null)
				{
					field.Put(PdfName.Lock, this.Lock.MakeIndirect(drawContext.GetDocument()).GetPdfObject
						());
				}
				field.GetWidgets()[0].SetFlag(PdfAnnotation.PRINT);
				field.GetWidgets()[0].SetHighlightMode(PdfAnnotation.HIGHLIGHT_INVERT);
				PdfAcroForm.GetAcroForm(drawContext.GetDocument(), true).AddField(field);
			}

			private readonly C2_12_LockFields _enclosing;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		[NUnit.Framework.Test]
        [Ignore("DEVSIX-488")]
		public virtual void RunTest()
		{
            Directory.CreateDirectory(NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/");
			C2_12_LockFields.Main(null);
			String[] resultFiles = new String[] { "step_1_signed_by_alice.pdf", "step_2_signed_by_alice_and_bob.pdf"
				, "step_3_signed_by_alice_bob_and_carol.pdf", "step_4_signed_by_alice_bob_carol_and_dave.pdf"
				, "step_5_signed_by_alice_and_bob_broken_by_chuck.pdf", "step_6_signed_by_dave_broken_by_chuck.pdf"
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
