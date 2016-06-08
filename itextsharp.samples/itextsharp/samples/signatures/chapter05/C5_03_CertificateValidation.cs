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
using iTextSharp.IO.Log;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using iTextSharp.Signatures;
using iTextSharp.Test;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security.Certificates;

namespace iTextSharp.Samples.Signatures.Chapter05
{
	public class C5_03_CertificateValidation : C5_01_SignatureIntegrity
	{
        public static readonly string ADOBE = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/adobeRootCA.cer";

        public static readonly string CACERT = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/CACertSigningAuthority.crt";

        public static readonly string BRUNO = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/encryption/bruno.crt";

        public static readonly string EXAMPLE3 = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/pdfs/hello_signed1.pdf";

	    public const String expectedOutput = "===== sig =====\r\n" + "Signature covers whole document: True\r\n"
			 + "Document revision: 1 of 1\r\n" + "Integrity check OK? True\r\n" + "Certificates verified against the KeyStore\r\n"
			 + "=== Certificate 0 ===\r\n" + "Issuer: C=BE,ST=OVL,L=Ghent,O=iText Software,OU=IT,CN=Bruno Specimen\r\n"
			 + "Subject: C=BE,ST=OVL,L=Ghent,O=iText Software,OU=IT,CN=Bruno Specimen\r\n" + "Valid from: 2012-08-04\r\n"
			 + "Valid to: 2112-07-11\r\n" + "The certificate was valid at the time of signing.\r\n"
			 + "The certificate is still valid.\r\n" + "=== Checking validity of the document at the time of signing ===\r\n"
             + "iTextSharp.Signatures.OcspClientBouncyCastle: Valid OCSPs found: 0\r\n"
             + "iTextSharp.Signatures.CRLVerifier: Valid CRLs found: 0\r\n"
			 + "The signing certificate couldn't be verified\r\n" + "=== Checking validity of the document today ===\r\n"
             + "iTextSharp.Signatures.CRLVerifier: Valid OCSPs found: 0\r\n"
             + "iTextSharp.Signatures.CRLVerifier: Valid CRLs found: 0\r\n"
			 + "The signing certificate couldn't be verified\r\n" + "\r\n";

        internal List<X509Certificate> ks;


		// public static final String EXAMPLE1 = "results/chapter3/hello_cacert_ocsp_ts.pdf";
		// public static final String EXAMPLE2 = "results/chapter3/hello_token.pdf";
		// public static final String EXAMPLE4 = "results/chapter4/hello_smartcard_Signature.pdf";
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public override PdfPKCS7 VerifySignature(SignatureUtil signUtil, String name)
		{
			PdfPKCS7 pkcs7 = base.VerifySignature(signUtil, name);
			X509Certificate[] certs = pkcs7.GetSignCertificateChain();
			DateTime cal = pkcs7.GetSignDate();
            IList<VerificationException> errors = CertificateVerification.VerifyCertificates(certs, ks, cal);
			if (errors.Count == 0)
			{
				System.Console.Out.WriteLine("Certificates verified against the KeyStore");
			}
			else
			{
				System.Console.Out.WriteLine(errors);
			}
			for (int i = 0; i < certs.Length; i++)
			{
				X509Certificate cert = (X509Certificate)certs[i];
				System.Console.Out.WriteLine("=== Certificate " + i + " ===");
				ShowCertificateInfo(cert, cal.ToUniversalTime());
			}
			X509Certificate signCert = (X509Certificate)certs[0];
			X509Certificate issuerCert = (certs.Length > 1 ? (X509Certificate)certs[1] : null
				);
			System.Console.Out.WriteLine("=== Checking validity of the document at the time of signing ==="
				);
			CheckRevocation(pkcs7, signCert, issuerCert, cal.ToUniversalTime());
			System.Console.Out.WriteLine("=== Checking validity of the document today ===");
			CheckRevocation(pkcs7, signCert, issuerCert, new DateTime());
			return pkcs7;
		}

		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		private static void CheckRevocation(PdfPKCS7 pkcs7, X509Certificate signCert, X509Certificate
			 issuerCert, DateTime date)
		{
			IList<BasicOcspResp> ocsps = new List<BasicOcspResp>();
			if (pkcs7.GetOcsp() != null)
			{
				ocsps.Add(pkcs7.GetOcsp());
			}
			OCSPVerifier ocspVerifier = new OCSPVerifier(null, ocsps);
			IList<VerificationOK> verification = ocspVerifier.Verify(signCert, issuerCert, date
				);
			if (verification.Count == 0)
			{
				IList<X509Crl> crls = new List<X509Crl>();
				if (pkcs7.GetCRLs() != null)
				{
					foreach (X509Crl crl in pkcs7.GetCRLs())
					{
						crls.Add((X509Crl)crl);
					}
				}
				CRLVerifier crlVerifier = new CRLVerifier(null, crls);
                IList<VerificationOK> verificationOks = crlVerifier.Verify(signCert, issuerCert, date);
                foreach (VerificationOK verOK in verificationOks)
                {
                    verification.Add(verOK);
			    }
			}
			if (verification.Count == 0)
			{
				System.Console.Out.WriteLine("The signing certificate couldn't be verified");
			}
			else
			{
				foreach (VerificationOK v in verification)
				{
					System.Console.Out.WriteLine(v);
				}
			}
		}

		public virtual void ShowCertificateInfo(X509Certificate cert, DateTime signDate)
		{
			System.Console.Out.WriteLine("Issuer: " + cert.IssuerDN);
			System.Console.Out.WriteLine("Subject: " + cert.SubjectDN);
		    System.Console.Out.WriteLine("Valid from: " + (cert.NotBefore.ToLocalTime().ToString("yyyy-MM-dd")));

            System.Console.Out.WriteLine("Valid to: " + cert.NotAfter.ToLocalTime().ToString("yyyy-MM-dd"));
			try
			{
				cert.CheckValidity(signDate);
				System.Console.Out.WriteLine("The certificate was valid at the time of signing.");
			}
			catch (CertificateExpiredException)
			{
				System.Console.Out.WriteLine("The certificate was expired at the time of signing."
					);
			}
			catch (CertificateNotYetValidException)
			{
				System.Console.Out.WriteLine("The certificate wasn't valid yet at the time of signing."
					);
			}
			try
			{
				cert.CheckValidity();
				System.Console.Out.WriteLine("The certificate is still valid.");
			}
			catch (CertificateExpiredException)
			{
				System.Console.Out.WriteLine("The certificate has expired.");
			}
			catch (CertificateNotYetValidException)
			{
				System.Console.Out.WriteLine("The certificate isn't valid yet.");
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		public static void Main(String[] args)
		{
			C5_03_CertificateValidation app = new C5_03_CertificateValidation();
            List<X509Certificate> ks = new List<X509Certificate>();
            var parser = new X509CertificateParser();
            X509Certificate adobeCert = parser.ReadCertificate(new FileStream(ADOBE, FileMode
                .Open, FileAccess.Read));
            X509Certificate cacertCert = parser.ReadCertificate(new FileStream(CACERT, FileMode
                .Open, FileAccess.Read));
            X509Certificate brunoCert = parser.ReadCertificate(new FileStream(BRUNO, FileMode
                .Open, FileAccess.Read));
            ks.Add(adobeCert);
            ks.Add(cacertCert);
            ks.Add(brunoCert);
            app.SetKeyStore(ks);
			// app.verifySignatures(EXAMPLE1);
			// app.verifySignatures(EXAMPLE2);
			app.VerifySignatures(EXAMPLE3);
		}

		// app.verifySignatures(EXAMPLE4);
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Javax.Smartcardio.CardException"/>
		[NUnit.Framework.Test]
		public override void RunTest()
		{
            LoggerFactory.GetInstance().SetLogger(new Log4NetLogger());
			SetupSystemOutput();
			C5_03_CertificateValidation.Main(null);
			String sysOut = GetSystemOutput();
			// Replace time added by LOGGER
			String outputForComparison = iTextSharp.IO.Util.StringUtil.ReplaceAll(sysOut, "[0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{3}"
				, "");
			NUnit.Framework.Assert.AreEqual(expectedOutput, outputForComparison, "Unexpected output."
				);
		}

		private void SetKeyStore(List<X509Certificate> ks)
		{
			this.ks = ks;
		}
	}
}
