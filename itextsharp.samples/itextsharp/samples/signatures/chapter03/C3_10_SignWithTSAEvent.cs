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
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using iTextSharp.IO.Util;
using iTextSharp.Signatures;
using NUnit.Framework;
using Org.BouncyCastle.Pkcs;

namespace iTextSharp.Samples.Signatures.Chapter03
{
	public class C3_10_SignWithTSAEvent : C3_01_SignWithCAcert
	{
	    public static readonly string SRC = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/pdfs/hello.pdf";

	    public static readonly string DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter03/hello_cacert_ocsp_ts.pdf";

	    public static readonly string PROPERTIES = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/signkey.properties";

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
			TSAClientBouncyCastle tsaClient = new TSAClientBouncyCastle(tsaUrl, tsaUser, tsaPass
				);
			tsaClient.SetTSAInfo(new _ITSAInfoBouncyCastle_66());
			C3_09_SignWithTSA app = new C3_09_SignWithTSA();
			app.Sign(SRC, DEST, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
				.CMS, "Test", "Ghent", null, ocspClient, tsaClient, 0);
		}

		private sealed class _ITSAInfoBouncyCastle_66 : ITSAInfoBouncyCastle
		{
			public _ITSAInfoBouncyCastle_66()
			{
			}

			public void InspectTimeStampTokenInfo(TimeStampTokenInfo info)
			{
				System.Console.Out.WriteLine(info.GenTime);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		[NUnit.Framework.Test]
        [Ignore("Put property file with valid data")]
        public override void RunTest()
        {
            Directory.CreateDirectory(NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/resources/signatures/chapter03/");
			C3_10_SignWithTSAEvent.Main(null);
			String[] resultFiles = new String[] { "hello_cacert_ocsp_ts.pdf" };
			String destPath = String.Format(outPath, "chapter03");
			String comparePath = String.Format(cmpPath, "chapter03");
			String[] errors = new String[resultFiles.Length];
			bool error = false;
			//        HashMap<Integer, List<Rectangle>> ignoredAreas = new HashMap<Integer, List<Rectangle>>() { {
			//            put(1, Arrays.asList(new Rectangle(36, 648, 200, 100)));
			//        }};
			for (int i = 0; i < resultFiles.Length; i++)
			{
				String resultFile = resultFiles[i];
				String fileErrors = CheckForErrors(destPath + resultFile, comparePath + "cmp_" + 
					resultFile, destPath, null);
				/*ignoredAreas*/
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
