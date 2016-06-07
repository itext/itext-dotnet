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
using iTextSharp.Signatures;
using Org.BouncyCastle.Pkcs;

namespace iTextSharp.Samples.Signatures.Chapter03
{
	public class C3_12_SignWithEstimatedSize : C3_01_SignWithCAcert
	{
	    public static readonly string SRC = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/pdfs/hello.pdf";

	    public static readonly string DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter03/hello_estimated.pdf";

	    public static readonly string PROPERTIES = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/signkey.properties";

	    public const String expectedOutput = "";

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		public static void Main(String[] args)
		{
            Properties properties = new Properties();
            properties.Load(new FileStream(PROPERTIES, FileMode.Open, FileAccess.Read));
            String path = NUnit.Framework.TestContext.CurrentContext.TestDirectory + properties.GetProperty("PRIVATE");
            char[] pass = properties.GetProperty("PASSWORD").ToCharArray();
            String tsaUrl = properties.GetProperty("TSAURL");
            String tsaUser = properties.GetProperty("TSAUSERNAME");
            String tsaPass = properties.GetProperty("TSAPASSWORD");
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

			IOcspClient ocspClient = new OcspClientBouncyCastle(null);
			ITSAClient tsaClient = new TSAClientBouncyCastle(tsaUrl, tsaUser, tsaPass);
			C3_12_SignWithEstimatedSize app = new C3_12_SignWithEstimatedSize();
			bool succeeded = false;
			int estimatedSize = 1000;
			while (!succeeded)
			{
				try
				{
					System.Console.Out.WriteLine("Attempt: " + estimatedSize + " bytes");
					app.Sign(SRC, DEST, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
						.CMS, "Test", "Ghent", null, ocspClient, null, estimatedSize);
					succeeded = true;
					System.Console.Out.WriteLine("Succeeded!");
				}
				catch (System.IO.IOException ioe)
				{
					System.Console.Out.WriteLine("Not succeeded: " + ioe.Message);
					estimatedSize += 50;
				}
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		[NUnit.Framework.Test]
		public override void RunTest()
		{
            CreateDestinationFolder(NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter03/");
			SetupSystemOutput();
			C3_12_SignWithEstimatedSize.Main(null);
			String sysOut = GetSystemOutput();
			if (!sysOut.Equals(expectedOutput))
			{
				NUnit.Framework.Assert.Fail("Unexpected output.");
			}
			String[] resultFiles = new String[] { "hello_estimated.pdf" };
			String destPath = String.Format(outPath, "chapter03");
			String comparePath = String.Format(cmpPath, "chapter03");
			String[] errors = new String[resultFiles.Length];
			bool error = false;
            Dictionary<int, IList<Rectangle>> ignoredAreas = new Dictionary<int, IList<Rectangle>> { { 1, JavaUtil.ArraysAsList(new Rectangle(36, 648, 200, 100)) } };
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
