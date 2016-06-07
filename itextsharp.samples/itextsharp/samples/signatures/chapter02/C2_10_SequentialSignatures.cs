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
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Annot;
using iTextSharp.Layout;
using iTextSharp.Layout.Element;
using iTextSharp.Layout.Renderer;
using iTextSharp.Samples.Signatures;
using iTextSharp.Signatures;
using Org.BouncyCastle.Pkcs;

namespace iTextSharp.Samples.Signatures.Chapter02
{
	public class C2_10_SequentialSignatures : SignatureTest
	{
        public static readonly string FORM = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/multiple_signatures.pdf";

        public static readonly string ALICE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/alice";

        public static readonly string BOB = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/bob";

        public static readonly string CAROL = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/carol";

        public static readonly string KEYSTORE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/ks";

	    public static readonly char[] PASSWORD = "password".ToCharArray();

        public static readonly string DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/signed_by_{0}.pdf";

	    /// <exception cref="System.IO.IOException"/>
		public virtual void CreateForm()
		{
			PdfDocument pdfDoc = new PdfDocument(new PdfWriter(FORM));
			Document doc = new Document(pdfDoc);
			Table table = new Table(1);
			table.AddCell("Signer 1: Alice");
			table.AddCell(CreateSignatureFieldCell("sig1"));
			//        table.addCell("Signer 2: Bob");
			//        table.addCell(createSignatureFieldCell("sig2"));
			//        table.addCell("Signer 3: Carol");
			//        table.addCell(createSignatureFieldCell("sig3"));
			doc.Add(table);
			doc.Close();
		}

		protected internal virtual Cell CreateSignatureFieldCell(String name)
		{
			Cell cell = new Cell();
			cell.SetHeight(50);
			cell.SetNextRenderer(new C2_10_SequentialSignatures.SignatureFieldCellRenderer(this
				, cell, name));
			return cell;
		}

		internal class SignatureFieldCellRenderer : CellRenderer
		{
			public String name;

			public SignatureFieldCellRenderer(C2_10_SequentialSignatures _enclosing, Cell modelElement
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

			private readonly C2_10_SequentialSignatures _enclosing;
		}

		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Sign(String keystore, int level, String src, String name, String
			 dest)
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
			signer.GetDocument().SetFlushUnusedObjects(true);
			// Setting signer options
			signer.SetFieldName(name);
			signer.SetCertificationLevel(level);
			// Creating the signature
			IExternalSignature pks = new PrivateKeySignature(pk, "SHA-256");
			signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS
				);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		public static void Main(String[] args)
		{
			C2_10_SequentialSignatures app = new C2_10_SequentialSignatures();
			app.CreateForm();
			// TODO DEVSIX-488
			app.Sign(ALICE, PdfSigner.CERTIFIED_FORM_FILLING, FORM, "sig1", String.Format(DEST
				, "alice"));
		}

		//        app.sign(BOB, PdfSigner.NOT_CERTIFIED, String.format(DEST, "alice"), "sig2", String.format(DEST, "bob"));
		//        app.sign(CAROL, PdfSigner.NOT_CERTIFIED, String.format(DEST, "bob"), "sig3", String.format(DEST, "carol"));
		//
		//        app.sign(ALICE, PdfSigner.NOT_CERTIFIED, FORM, "sig1", String.format(DEST, "alice2"));
		//        app.sign(BOB, PdfSigner.NOT_CERTIFIED, String.format(DEST, "alice2"), "sig2", String.format(DEST, "bob2"));
		//        app.sign(CAROL, PdfSigner.CERTIFIED_FORM_FILLING, String.format(DEST, "bob2"), "sig3", String.format(DEST, "carol2"));
		//
		//        app.sign(ALICE, PdfSigner.NOT_CERTIFIED, FORM, "sig1", String.format(DEST, "alice3"));
		//        app.sign(BOB, PdfSigner.NOT_CERTIFIED, String.format(DEST, "alice3"), "sig2", String.format(DEST, "bob3"));
		//        app.sign(CAROL, PdfSigner.CERTIFIED_NO_CHANGES_ALLOWED, String.format(DEST, "bob3"), "sig3", String.format(DEST, "carol3"));
		//
		//        app.sign(ALICE, PdfSigner.CERTIFIED_FORM_FILLING, FORM, "sig1", String.format(DEST, "alice4"));
		//        app.sign(BOB, PdfSigner.NOT_CERTIFIED, String.format(DEST, "alice4"), "sig2", String.format(DEST, "bob4"));
		//        app.sign(CAROL, PdfSigner.CERTIFIED_FORM_FILLING, String.format(DEST, "bob4"), "sig3", String.format(DEST, "carol4"));
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		[NUnit.Framework.Test]
		public virtual void RunTest() {
            CreateDestinationFolder(NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter02/");
			C2_10_SequentialSignatures.Main(null);
		}
		//
		//        String[] resultFiles =
		//                new String[]{"signed_by_alice.pdf", "signed_by_bob.pdf", "signed_by_carol.pdf",
		//                        "signed_by_alice2.pdf", "signed_by_bob2.pdf", "signed_by_carol2.pdf",
		//                        "signed_by_alice3.pdf", "signed_by_bob3.pdf", "signed_by_carol3.pdf"};
		//
		//        String destPath = String.format(outPath, "chapter02");
		//        String comparePath = String.format(cmpPath, "chapter02");
		//
		//        String[] errors = new String[resultFiles.length];
		//        boolean error = false;
		//
		//        HashMap<Integer, List<Rectangle>> ignoredAreas = new HashMap<Integer, List<Rectangle>>() {
		//            {
		//                put(1, Arrays.asList(new Rectangle(46, 472, 287, 255)));
		//            }
		//        };
		//
		//        for (int i = 0; i < resultFiles.length; i++) {
		//            String resultFile = resultFiles[i];
		//            String fileErrors = checkForErrors(destPath + resultFile, comparePath + "cmp_" + resultFile, destPath, ignoredAreas);
		//            if (fileErrors != null) {
		//                errors[i] = fileErrors;
		//                error = true;
		//            }
		//        }
		//
		//        if (error) {
		//            fail(accumulateErrors(errors));
		//        }
	}
}
