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
using iTextSharp.IO.Util;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Signatures;
using Org.BouncyCastle.Pkcs;

namespace iTextSharp.Samples.Signatures.Chapter03
{
	public class C3_01_SignWithCAcert : SignatureTest
	{
	    public static readonly string SRC = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/pdfs/hello.pdf";

	    public static readonly string DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter03/hello_cacert.pdf";

	    public static readonly string PROPERTIES = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/signkey.properties";

	    /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Sign(String src, String dest, X509Certificate[] chain, ICipherParameters
			 pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter
			, String reason, String location, ICollection<ICrlClient> crlList, IOcspClient ocspClient
			, ITSAClient tsaClient, int estimatedSize)
		{
			// Creating the reader and the signer
			PdfReader reader = new PdfReader(src);
			PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), false
				);
			// Creating the appearance
			PdfSignatureAppearance appearance = signer.GetSignatureAppearance().SetReason(reason
				).SetLocation(location).SetReuseAppearance(false);
			Rectangle rect = new Rectangle(36, 648, 200, 100);
			appearance.SetPageRect(rect).SetPageNumber(1);
			signer.SetFieldName("sig");
			// Creating the signature
			IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
			signer.SignDetached(pks, chain, crlList, ocspClient, tsaClient, estimatedSize, subfilter
				);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		public static void Main(String[] args)
		{
			Properties properties = new Properties();
			properties.Load(new FileStream(PROPERTIES, FileMode.Open, FileAccess.Read));
            String path = NUnit.Framework.TestContext.CurrentContext.TestDirectory + properties.GetProperty("PRIVATE");
			char[] pass = properties.GetProperty("PASSWORD").ToCharArray();
			string alias = null;
            Pkcs12Store pk12;
            
            pk12 = new Pkcs12Store(new FileStream(path, FileMode.Open, FileAccess.Read), pass);
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

			C3_01_SignWithCAcert app = new C3_01_SignWithCAcert();
			app.Sign(SRC, DEST, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
				.CMS, "Test", "Ghent", null, null, null, 0);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		[NUnit.Framework.Test]
		public virtual void RunTest()
		{
            CreateDestinationFolder(NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter03/");
			C3_01_SignWithCAcert.Main(null);
			String[] resultFiles = new String[] { "hello_cacert.pdf" };
			String destPath = String.Format(outPath, "chapter03");
			String comparePath = String.Format(cmpPath, "chapter03");
			String[] errors = new String[resultFiles.Length];
			bool error = false;
            Dictionary<int, IList<Rectangle>> ignoredAreas = new Dictionary<int, IList<Rectangle>> {{1, JavaUtil.ArraysAsList(new Rectangle(36, 648, 200, 100))}};
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
