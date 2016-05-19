/*
$Id$

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
using Java.Security;
using Java.Security.Cert;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.Bouncycastle.Asn1.Ocsp;
using Org.Bouncycastle.Cert;
using Org.Bouncycastle.Cert.Jcajce;
using Org.Bouncycastle.Cert.Ocsp;
using Org.Bouncycastle.Operator;
using Org.Bouncycastle.Operator.BC;
using Org.Bouncycastle.Operator.Jcajce;
using iTextSharp.IO.Log;

namespace iTextSharp.Signatures
{
	/// <summary>
	/// Class that allows you to verify a certificate against
	/// one or more OCSP responses.
	/// </summary>
	public class OCSPVerifier : RootStoreVerifier
	{
		/// <summary>The Logger instance</summary>
		protected internal static readonly ILogger LOGGER = LoggerFactory.GetLogger(typeof(
			iTextSharp.Signatures.OCSPVerifier));

		protected internal const String id_kp_OCSPSigning = "1.3.6.1.5.5.7.3.9";

		/// <summary>The list of OCSP responses.</summary>
		protected internal IList<BasicOCSPResp> ocsps;

		/// <summary>Creates an OCSPVerifier instance.</summary>
		/// <param name="verifier">the next verifier in the chain</param>
		/// <param name="ocsps">a list of OCSP responses</param>
		public OCSPVerifier(CertificateVerifier verifier, IList<BasicOCSPResp> ocsps)
			: base(verifier)
		{
			this.ocsps = ocsps;
		}

		/// <summary>Verifies if a a valid OCSP response is found for the certificate.</summary>
		/// <remarks>
		/// Verifies if a a valid OCSP response is found for the certificate.
		/// If this method returns false, it doesn't mean the certificate isn't valid.
		/// It means we couldn't verify it against any OCSP response that was available.
		/// </remarks>
		/// <param name="signCert">the certificate that needs to be checked</param>
		/// <param name="issuerCert">its issuer</param>
		/// <returns>
		/// a list of <code>VerificationOK</code> objects.
		/// The list will be empty if the certificate couldn't be verified.
		/// </returns>
		/// <seealso cref="RootStoreVerifier.Verify(Org.BouncyCastle.X509.X509Certificate, Org.BouncyCastle.X509.X509Certificate, System.DateTime)
		/// 	"/>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public override IList<VerificationOK> Verify(X509Certificate signCert, X509Certificate
			 issuerCert, DateTime signDate)
		{
			IList<VerificationOK> result = new List<VerificationOK>();
			int validOCSPsFound = 0;
			// first check in the list of OCSP responses that was provided
			if (ocsps != null)
			{
				foreach (BasicOCSPResp ocspResp in ocsps)
				{
					if (Verify(ocspResp, signCert, issuerCert, signDate))
					{
						validOCSPsFound++;
					}
				}
			}
			// then check online if allowed
			bool online = false;
			if (onlineCheckingAllowed && validOCSPsFound == 0)
			{
				if (Verify(GetOcspResponse(signCert, issuerCert), signCert, issuerCert, signDate))
				{
					validOCSPsFound++;
					online = true;
				}
			}
			// show how many valid OCSP responses were found
			LOGGER.Info("Valid OCSPs found: " + validOCSPsFound);
			if (validOCSPsFound > 0)
			{
				result.Add(new VerificationOK(signCert, this.GetType(), "Valid OCSPs Found: " + validOCSPsFound
					 + (online ? " (online)" : "")));
			}
			if (verifier != null)
			{
				result.AddAll(verifier.Verify(signCert, issuerCert, signDate));
			}
			// verify using the previous verifier in the chain (if any)
			return result;
		}

		/// <summary>Verifies a certificate against a single OCSP response</summary>
		/// <param name="ocspResp">the OCSP response</param>
		/// <param name="signCert">the certificate that needs to be checked</param>
		/// <param name="issuerCert">the certificate of CA</param>
		/// <param name="signDate">sign date</param>
		/// <returns>
		/// 
		/// <see langword="true"/>
		/// , in case successful check, otherwise false.
		/// </returns>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual bool Verify(BasicOCSPResp ocspResp, X509Certificate signCert, X509Certificate
			 issuerCert, DateTime signDate)
		{
			if (ocspResp == null)
			{
				return false;
			}
			// Getting the responses
			SingleResp[] resp = ocspResp.GetResponses();
			for (int i = 0; i < resp.Length; i++)
			{
				// check if the serial number corresponds
				if (!signCert.GetSerialNumber().Equals(resp[i].GetCertID().GetSerialNumber()))
				{
					continue;
				}
				// check if the issuer matches
				try
				{
					if (issuerCert == null)
					{
						issuerCert = signCert;
					}
					if (!resp[i].GetCertID().MatchesIssuer(new X509CertificateHolder(issuerCert.GetEncoded
						()), new BcDigestCalculatorProvider()))
					{
						LOGGER.Info("OCSP: Issuers doesn't match.");
						continue;
					}
				}
				catch (OCSPException)
				{
					continue;
				}
				// check if the OCSP response was valid at the time of signing
				DateTime nextUpdate = resp[i].GetNextUpdate();
				if (nextUpdate == null)
				{
					nextUpdate = new DateTime(resp[i].GetThisUpdate().GetTime() + 180000l);
					LOGGER.Info(String.Format("No 'next update' for OCSP Response; assuming {0}", nextUpdate
						));
				}
				if (signDate.After(nextUpdate))
				{
					LOGGER.Info(String.Format("OCSP no longer valid: {0} after {1}", signDate, nextUpdate
						));
					continue;
				}
				// check the status of the certificate
				Object status = resp[i].GetCertStatus();
				if (status == CertificateStatus.GOOD)
				{
					// check if the OCSP response was genuine
					IsValidResponse(ocspResp, issuerCert);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Verifies if an OCSP response is genuine
		/// If it doesn't verify against the issuer certificate and response's certificates, it may verify
		/// using a trusted anchor or cert.
		/// </summary>
		/// <param name="ocspResp">the OCSP response</param>
		/// <param name="issuerCert">the issuer certificate</param>
		/// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
		/// <exception cref="System.IO.IOException"/>
		public virtual void IsValidResponse(BasicOCSPResp ocspResp, X509Certificate issuerCert
			)
		{
			//OCSP response might be signed by the issuer certificate or
			//the Authorized OCSP responder certificate containing the id-kp-OCSPSigning extended key usage extension
			X509Certificate responderCert = null;
			//first check if the issuer certificate signed the response
			//since it is expected to be the most common case
			if (IsSignatureValid(ocspResp, issuerCert))
			{
				responderCert = issuerCert;
			}
			//if the issuer certificate didn't sign the ocsp response, look for authorized ocsp responses
			// from properties or from certificate chain received with response
			if (responderCert == null)
			{
				if (ocspResp.GetCerts() != null)
				{
					//look for existence of Authorized OCSP responder inside the cert chain in ocsp response
					X509CertificateHolder[] certs = ocspResp.GetCerts();
					foreach (X509CertificateHolder cert in certs)
					{
						X509Certificate tempCert;
						try
						{
							tempCert = new JcaX509CertificateConverter().GetCertificate(cert);
						}
						catch (Exception)
						{
							continue;
						}
						IList<String> keyPurposes = null;
						try
						{
							keyPurposes = tempCert.GetExtendedKeyUsage();
							if ((keyPurposes != null) && keyPurposes.Contains(id_kp_OCSPSigning) && IsSignatureValid
								(ocspResp, tempCert))
							{
								responderCert = tempCert;
								break;
							}
						}
						catch (CertificateParsingException)
						{
						}
					}
					// Certificate signing the ocsp response is not found in ocsp response's certificate chain received
					// and is not signed by the issuer certificate.
					if (responderCert == null)
					{
						throw new VerificationException(issuerCert, "OCSP response could not be verified"
							);
					}
				}
				else
				{
					//certificate chain is not present in response received
					//try to verify using rootStore
					if (rootStore != null)
					{
						try
						{
							for (IEnumerator<String> aliases = rootStore.Aliases(); aliases.MoveNext(); )
							{
								String alias = aliases.Current;
								try
								{
									if (!rootStore.IsCertificateEntry(alias))
									{
										continue;
									}
									X509Certificate anchor = (X509Certificate)rootStore.GetCertificate(alias);
									if (IsSignatureValid(ocspResp, anchor))
									{
										responderCert = anchor;
										break;
									}
								}
								catch (GeneralSecurityException)
								{
								}
							}
						}
						catch (KeyStoreException)
						{
							responderCert = (X509Certificate)null;
						}
					}
					// OCSP Response does not contain certificate chain, and response is not signed by any
					// of the rootStore or the issuer certificate.
					if (responderCert == null)
					{
						throw new VerificationException(issuerCert, "OCSP response could not be verified"
							);
					}
				}
			}
			//check "This certificate MUST be issued directly by the CA that issued the certificate in question".
			responderCert.Verify(issuerCert.GetPublicKey());
			// validating ocsp signers certificate
			// Check if responders certificate has id-pkix-ocsp-nocheck extension,
			// in which case we do not validate (perform revocation check on) ocsp certs for lifetime of certificate
			if (responderCert.GetExtensionValue(OCSPObjectIdentifiers.id_pkix_ocsp_nocheck.GetId
				()) == null)
			{
				CRL crl;
				try
				{
					crl = CertificateUtil.GetCRL(responderCert);
				}
				catch (Exception)
				{
					crl = (CRL)null;
				}
				if (crl != null && crl is X509CRL)
				{
					CRLVerifier crlVerifier = new CRLVerifier(null, null);
					crlVerifier.SetRootStore(rootStore);
					crlVerifier.SetOnlineCheckingAllowed(onlineCheckingAllowed);
					crlVerifier.Verify((X509CRL)crl, responderCert, issuerCert, new DateTime());
					return;
				}
			}
			//check if lifetime of certificate is ok
			responderCert.CheckValidity();
		}

		/// <summary>Verifies if the response is valid.</summary>
		/// <remarks>
		/// Verifies if the response is valid.
		/// If it doesn't verify against the issuer certificate and response's certificates, it may verify
		/// using a trusted anchor or cert.
		/// NOTE. Use
		/// <c>isValidResponse()</c>
		/// instead.
		/// </remarks>
		/// <param name="ocspResp">the response object</param>
		/// <param name="issuerCert">the issuer certificate</param>
		/// <returns>true if the response can be trusted</returns>
		[Obsolete]
		public virtual bool VerifyResponse(BasicOCSPResp ocspResp, X509Certificate issuerCert
			)
		{
			try
			{
				IsValidResponse(ocspResp, issuerCert);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>Checks if an OCSP response is genuine</summary>
		/// <param name="ocspResp">the OCSP response</param>
		/// <param name="responderCert">the responder certificate</param>
		/// <returns>true if the OCSP response verifies against the responder certificate</returns>
		public virtual bool IsSignatureValid(BasicOCSPResp ocspResp, X509Certificate responderCert
			)
		{
			try
			{
				ContentVerifierProvider verifierProvider = new JcaContentVerifierProviderBuilder(
					).SetProvider("BC").Build(responderCert.GetPublicKey());
				return ocspResp.IsSignatureValid(verifierProvider);
			}
			catch (OperatorCreationException)
			{
				return false;
			}
			catch (OCSPException)
			{
				return false;
			}
		}

		/// <summary>
		/// Gets an OCSP response online and returns it if the status is GOOD
		/// (without further checking!).
		/// </summary>
		/// <param name="signCert">the signing certificate</param>
		/// <param name="issuerCert">the issuer certificate</param>
		/// <returns>an OCSP response</returns>
		public virtual BasicOCSPResp GetOcspResponse(X509Certificate signCert, X509Certificate
			 issuerCert)
		{
			if (signCert == null && issuerCert == null)
			{
				return null;
			}
			OcspClientBouncyCastle ocsp = new OcspClientBouncyCastle(null);
			BasicOCSPResp ocspResp = ocsp.GetBasicOCSPResp(signCert, issuerCert, null);
			if (ocspResp == null)
			{
				return null;
			}
			SingleResp[] resps = ocspResp.GetResponses();
			foreach (SingleResp resp in resps)
			{
				Object status = resp.GetCertStatus();
				if (status == CertificateStatus.GOOD)
				{
					return ocspResp;
				}
			}
			return null;
		}
	}
}
