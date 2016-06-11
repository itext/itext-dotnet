using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using iTextSharp.Kernel;
using iTextSharp.Kernel.Pdf;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;

namespace iTextSharp.Signatures
{
	internal static class SignUtils
	{
        internal static readonly DateTime UNDEFINED_TIMESTAMP_DATE = DateTime.MaxValue;

		/// <exception cref="CertificateException"/>
		/// <exception cref="CrlException"/>
		internal static X509Crl ParseCrlFromStream(Stream input)
		{
			return new X509CrlParser().ReadCrl(input);
        }

        internal static X509Crl ParseCrlFromUrl(String crlurl) {
            X509CrlParser crlParser = new X509CrlParser();
            // Creates the CRL
            Stream url = WebRequest.Create(crlurl).GetResponse().GetResponseStream();
            return crlParser.ReadCrl(url);
        }

        internal static byte[] GetExtensionValueByOid(X509Certificate certificate, String oid) {
            Asn1OctetString extensionValue = certificate.GetExtensionValue(oid);
            return extensionValue != null ? extensionValue.GetOctets() : null;
        }

	    internal static IDigest GetMessageDigest(String hashAlgorithm) {
            return DigestUtilities.GetDigest(hashAlgorithm);
        }

        internal static Stream GetHttpResponse(Uri urlt) {
            HttpWebRequest con = (HttpWebRequest)WebRequest.Create(urlt);
            HttpWebResponse response = (HttpWebResponse)con.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new PdfException(PdfException.InvalidHttpResponse1).SetMessageParams(response.StatusCode);
            return response.GetResponseStream();
        }

	    internal static CertificateID GenerateCertificateId(X509Certificate issuerCert, BigInteger serialNumber, String hashAlgorithm) {
	        return new CertificateID(hashAlgorithm, issuerCert, serialNumber);
	    }

	    internal static Stream GetHttpResponseForOcspRequest(byte[] request, Uri urlt) {
            HttpWebRequest con = (HttpWebRequest)WebRequest.Create(urlt);
            con.ContentLength = request.Length;
            con.ContentType = "application/ocsp-request";
            con.Accept = "application/ocsp-response";
            con.Method = "POST";
            Stream outp = con.GetRequestStream();
            outp.Write(request, 0, request.Length);
            outp.Close();
            HttpWebResponse response = (HttpWebResponse)con.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new PdfException(PdfException.InvalidHttpResponse1).SetMessageParams(response.StatusCode);

            return response.GetResponseStream();
        }

	    internal static OcspReq GenerateOcspRequestWithNonce(CertificateID id) {
            OcspReqGenerator gen = new OcspReqGenerator();
            gen.AddRequest(id);

            // create details for nonce extension
            IDictionary extensions = new Hashtable();

            extensions[OcspObjectIdentifiers.PkixOcspNonce] = new X509Extension(false, new DerOctetString(new DerOctetString(PdfEncryption.GenerateNewDocumentId()).GetEncoded()));

            gen.SetRequestExtensions(new X509Extensions(extensions));
            return gen.Generate();
        }

	    internal static bool IsSignatureValid(BasicOcspResp validator, X509Certificate certStoreX509) {
	        return validator.Verify(certStoreX509.GetPublicKey());
	    }

	    internal static void IsSignatureValid(TimeStampToken validator, X509Certificate certStoreX509) {
	        validator.Validate(certStoreX509);
	    }

	    internal static bool CheckIfIssuersMatch(CertificateID certID, X509Certificate issuerCert) {
	        return certID.MatchesIssuer(issuerCert);
	    }

	    internal static DateTime Add180Sec(DateTime date) {
	        return date.AddSeconds(180);
	    }

	    internal static IEnumerable<X509Certificate> GetCertsFromOcspResponse(BasicOcspResp ocspResp) {
	        return ocspResp.GetCerts();
        }

        internal static List<X509Certificate> ReadAllCerts(byte[] contentsKey) {
            X509CertificateParser cf = new X509CertificateParser();
            List<X509Certificate> certs = new List<X509Certificate>();

            foreach (X509Certificate cc in cf.ReadCertificates(contentsKey)) {
                certs.Add(cc);
            }
            return certs;
        }

	    internal static T GetFirstElement<T>(IEnumerable<T> enumerable) {
	        return enumerable.First();
	    }

	    internal static X509Name GetIssuerX509Name(Asn1Sequence issuerAndSerialNumber) {
	        return X509Name.GetInstance(issuerAndSerialNumber[0]);
	    }

	    internal static String DateToString(DateTime signDate) {
	        return signDate.ToLocalTime().ToString("yyyy.MM.dd HH:mm:ss zzz");
	    }

        internal class TsaResponse {
            internal String encoding;
            internal Stream tsaResponseStream;
        }

	    internal static TsaResponse GetTsaResponseForUserRequest(String tsaUrl, byte[] requestBytes, String tsaUsername, String tsaPassword) {
	        HttpWebRequest con;
            try {
	            con = (HttpWebRequest) WebRequest.Create(tsaUrl);
	        } catch (Exception e) {
                throw new PdfException(PdfException.FailedToGetTsaResponseFrom1).SetMessageParams(tsaUrl);
            }
            con.ContentLength = requestBytes.Length;
            con.ContentType = "application/timestamp-query";
            con.Method = "POST";
            if ((tsaUsername != null) && !tsaUsername.Equals("")) {
                string authInfo = tsaUsername + ":" + tsaPassword;
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo), Base64FormattingOptions.None);
                con.Headers["Authorization"] = "Basic " + authInfo;
            }
            Stream outp = con.GetRequestStream();
            outp.Write(requestBytes, 0, requestBytes.Length);
            outp.Close();
            HttpWebResponse httpWebResponse = (HttpWebResponse)con.GetResponse();

            TsaResponse response = new TsaResponse();
	        response.tsaResponseStream = httpWebResponse.GetResponseStream();
	        response.encoding = httpWebResponse.ContentEncoding;
            return response;
	    }

	    internal static bool HasUnsupportedCriticalExtension(X509Certificate cert) {
            foreach (String oid in cert.GetCriticalExtensionOids()) {
                if (oid == X509Extensions.KeyUsage.Id
                    || oid == X509Extensions.CertificatePolicies.Id
                    || oid == X509Extensions.PolicyMappings.Id
                    || oid == X509Extensions.InhibitAnyPolicy.Id
                    || oid == X509Extensions.CrlDistributionPoints.Id
                    || oid == X509Extensions.IssuingDistributionPoint.Id
                    || oid == X509Extensions.DeltaCrlIndicator.Id
                    || oid == X509Extensions.PolicyConstraints.Id
                    || oid == X509Extensions.BasicConstraints.Id
                    || oid == X509Extensions.SubjectAlternativeName.Id
                    || oid == X509Extensions.NameConstraints.Id) {
                    continue;
                }
                try {
                    // EXTENDED KEY USAGE and TIMESTAMPING is ALLOWED
                    if (oid == X509Extensions.ExtendedKeyUsage.Id && cert.GetExtendedKeyUsage().Contains("1.3.6.1.5.5.7.3.8")) {
                        continue;
                    }
                } catch (CertificateParsingException) {
                    // DO NOTHING;
                }
                return true;
            }
	        return false;
	    }
        
        internal static DateTime GetTimeStampDate(TimeStampToken timeStampToken) {
            return timeStampToken.TimeStampInfo.GenTime;
        }

        internal static ISigner GetSignatureHelper(String algorithm) {
            return SignerUtilities.GetSigner(algorithm);
        }

	    internal static IEnumerable<X509Certificate> GetCertificates(List<X509Certificate> rootStore) {
	        return rootStore;
	    }
    }
}
