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
using Org.BouncyCastle.X509;
using iTextSharp.IO.Util;
using iTextSharp.Signatures;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;

namespace iTextSharp.Samples.Signatures.Chapter03
{
	public class C3_02_GetCrlUrl : SignatureTest
	{
        public const String expectedOutput = "";

	    public static readonly string PROPERTIES = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/signkey.properties";

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

			for (int i = 0; i < chain.Length; i++)
			{
				X509Certificate cert = (X509Certificate)chain[i];
				System.Console.Out.WriteLine(String.Format("[{0}] {1}", i, cert.SubjectDN));
				System.Console.Out.WriteLine(CertificateUtil.GetCRLURL(cert));
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		[NUnit.Framework.Test]
        [Ignore("Put property file with valid data")]
		public virtual void RunTest()
		{
			SetupSystemOutput();
			C3_02_GetCrlUrl.Main(null);
			String sysOut = GetSystemOutput();
			if (!sysOut.Equals(expectedOutput))
			{
				NUnit.Framework.Assert.Fail("Unexpected output.");
			}
		}
	}
}
