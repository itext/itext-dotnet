/*
$Id: f15f74438f9998e6fe9f656f20bf5964f879f486 $

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using java.security;
using java.security.cert;
using org.bouncycastle.@operator.jcajce;
using org.bouncycastle.cert.ocsp;
using org.bouncycastle.cms.jcajce;
using org.bouncycastle.tsp;

namespace com.itextpdf.signatures
{
	/// <summary>This class consists of some methods that allow you to verify certificates.
	/// 	</summary>
	public class CertificateVerification
	{
		/// <summary>Verifies a single certificate.</summary>
		/// <param name="cert">the certificate to verify</param>
		/// <param name="crls">the certificate revocation list or <CODE>null</CODE></param>
		/// <param name="calendar">the date or <CODE>null</CODE> for the current date</param>
		/// <returns>
		/// a <CODE>String</CODE> with the error description or <CODE>null</CODE>
		/// if no error
		/// </returns>
		public static String VerifyCertificate(X509Certificate cert, ICollection<CRL> crls
			, Calendar calendar)
		{
			if (calendar == null)
			{
				calendar = new GregorianCalendar();
			}
			if (cert.HasUnsupportedCriticalExtension())
			{
				foreach (String oid in cert.GetCriticalExtensionOIDs())
				{
					// KEY USAGE and DIGITAL SIGNING is ALLOWED
					if ("2.5.29.15".Equals(oid) && cert.GetKeyUsage()[0])
					{
						continue;
					}
					try
					{
						// EXTENDED KEY USAGE and TIMESTAMPING is ALLOWED
						if ("2.5.29.37".Equals(oid) && cert.GetExtendedKeyUsage().Contains("1.3.6.1.5.5.7.3.8"
							))
						{
							continue;
						}
					}
					catch (CertificateParsingException)
					{
					}
					// DO NOTHING;
					return "Has unsupported critical extension";
				}
			}
			try
			{
				cert.CheckValidity(calendar.GetTime());
			}
			catch (Exception e)
			{
				return e.Message;
			}
			if (crls != null)
			{
				foreach (CRL crl in crls)
				{
					if (crl.IsRevoked(cert))
					{
						return "Certificate revoked";
					}
				}
			}
			return null;
		}

		/// <summary>Verifies a certificate chain against a KeyStore.</summary>
		/// <param name="certs">the certificate chain</param>
		/// <param name="keystore">the <CODE>KeyStore</CODE></param>
		/// <param name="crls">the certificate revocation list or <CODE>null</CODE></param>
		/// <param name="calendar">the date or <CODE>null</CODE> for the current date</param>
		/// <returns>
		/// <CODE>null</CODE> if the certificate chain could be validated or a
		/// <CODE>Object[]{cert,error}</CODE> where <CODE>cert</CODE> is the
		/// failed certificate and <CODE>error</CODE> is the error message
		/// </returns>
		public static IList<VerificationException> VerifyCertificates(Certificate[] certs
			, KeyStore keystore, ICollection<CRL> crls, Calendar calendar)
		{
			IList<VerificationException> result = new List<VerificationException>();
			if (calendar == null)
			{
				calendar = new GregorianCalendar();
			}
			for (int k = 0; k < certs.Length; ++k)
			{
				X509Certificate cert = (X509Certificate)certs[k];
				String err = VerifyCertificate(cert, crls, calendar);
				if (err != null)
				{
					result.Add(new VerificationException(cert, err));
				}
				try
				{
					for (IEnumerator<String> aliases = keystore.Aliases(); aliases.MoveNext(); )
					{
						try
						{
							String alias = aliases.Current;
							if (!keystore.IsCertificateEntry(alias))
							{
								continue;
							}
							X509Certificate certStoreX509 = (X509Certificate)keystore.GetCertificate(alias);
							if (VerifyCertificate(certStoreX509, crls, calendar) != null)
							{
								continue;
							}
							try
							{
								cert.Verify(certStoreX509.GetPublicKey());
								return result;
							}
							catch (Exception)
							{
								continue;
							}
						}
						catch (Exception)
						{
						}
					}
				}
				catch (Exception)
				{
				}
				int j;
				for (j = 0; j < certs.Length; ++j)
				{
					if (j == k)
					{
						continue;
					}
					X509Certificate certNext = (X509Certificate)certs[j];
					try
					{
						cert.Verify(certNext.GetPublicKey());
						break;
					}
					catch (Exception)
					{
					}
				}
				if (j == certs.Length)
				{
					result.Add(new VerificationException(cert, "Cannot be verified against the KeyStore or the certificate chain"
						));
				}
			}
			if (result.Count == 0)
			{
				result.Add(new VerificationException((Certificate)null, "Invalid state. Possible circular certificate chain"
					));
			}
			return result;
		}

		/// <summary>Verifies a certificate chain against a KeyStore.</summary>
		/// <param name="certs">the certificate chain</param>
		/// <param name="keystore">the <CODE>KeyStore</CODE></param>
		/// <param name="calendar">the date or <CODE>null</CODE> for the current date</param>
		/// <returns>
		/// <CODE>null</CODE> if the certificate chain could be validated or a
		/// <CODE>Object[]{cert,error}</CODE> where <CODE>cert</CODE> is the
		/// failed certificate and <CODE>error</CODE> is the error message
		/// </returns>
		public static IList<VerificationException> VerifyCertificates(Certificate[] certs
			, KeyStore keystore, Calendar calendar)
		{
			return VerifyCertificates(certs, keystore, null, calendar);
		}

		/// <summary>Verifies an OCSP response against a KeyStore.</summary>
		/// <param name="ocsp">the OCSP response</param>
		/// <param name="keystore">the <CODE>KeyStore</CODE></param>
		/// <param name="provider">the provider or <CODE>null</CODE> to use the BouncyCastle provider
		/// 	</param>
		/// <returns><CODE>true</CODE> is a certificate was found</returns>
		public static bool VerifyOcspCertificates(BasicOCSPResp ocsp, KeyStore keystore, 
			String provider)
		{
			if (provider == null)
			{
				provider = "BC";
			}
			try
			{
				for (IEnumerator<String> aliases = keystore.Aliases(); aliases.MoveNext(); )
				{
					try
					{
						String alias = aliases.Current;
						if (!keystore.IsCertificateEntry(alias))
						{
							continue;
						}
						X509Certificate certStoreX509 = (X509Certificate)keystore.GetCertificate(alias);
						if (ocsp.IsSignatureValid(new JcaContentVerifierProviderBuilder().SetProvider(provider
							).Build(certStoreX509.GetPublicKey())))
						{
							return true;
						}
					}
					catch (Exception)
					{
					}
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		/// <summary>Verifies a time stamp against a KeyStore.</summary>
		/// <param name="ts">the time stamp</param>
		/// <param name="keystore">the <CODE>KeyStore</CODE></param>
		/// <param name="provider">the provider or <CODE>null</CODE> to use the BouncyCastle provider
		/// 	</param>
		/// <returns><CODE>true</CODE> is a certificate was found</returns>
		public static bool VerifyTimestampCertificates(TimeStampToken ts, KeyStore keystore
			, String provider)
		{
			if (provider == null)
			{
				provider = "BC";
			}
			try
			{
				for (IEnumerator<String> aliases = keystore.Aliases(); aliases.MoveNext(); )
				{
					try
					{
						String alias = aliases.Current;
						if (!keystore.IsCertificateEntry(alias))
						{
							continue;
						}
						X509Certificate certStoreX509 = (X509Certificate)keystore.GetCertificate(alias);
						ts.IsSignatureValid(new JcaSimpleSignerInfoVerifierBuilder().SetProvider(provider
							).Build(certStoreX509));
						return true;
					}
					catch (Exception)
					{
					}
				}
			}
			catch (Exception)
			{
			}
			return false;
		}
	}
}
