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
using System.IO;
using java.security.cert;
using org.bouncycastle.asn1;
using org.bouncycastle.asn1.x509;

namespace com.itextpdf.signatures
{
	/// <summary>
	/// This class contains a series of static methods that
	/// allow you to retrieve information from a Certificate.
	/// </summary>
	public class CertificateUtil
	{
		// Certificate Revocation Lists
		/// <summary>Gets a CRL from a certificate</summary>
		/// <param name="certificate"/>
		/// <returns>the CRL or null if there's no CRL available</returns>
		/// <exception cref="java.security.cert.CertificateException"/>
		/// <exception cref="java.security.cert.CRLException"/>
		/// <exception cref="System.IO.IOException"/>
		public static CRL GetCRL(X509Certificate certificate)
		{
			return CertificateUtil.GetCRL(CertificateUtil.GetCRLURL(certificate));
		}

		/// <summary>Gets the URL of the Certificate Revocation List for a Certificate</summary>
		/// <param name="certificate">the Certificate</param>
		/// <returns>the String where you can check if the certificate was revoked</returns>
		/// <exception cref="java.security.cert.CertificateParsingException"/>
		/// <exception cref="System.IO.IOException"/>
		public static String GetCRLURL(X509Certificate certificate)
		{
			ASN1Primitive obj;
			try
			{
				obj = GetExtensionValue(certificate, Extension.cRLDistributionPoints.GetId());
			}
			catch (System.IO.IOException)
			{
				obj = (ASN1Primitive)null;
			}
			if (obj == null)
			{
				return null;
			}
			CRLDistPoint dist = CRLDistPoint.GetInstance(obj);
			DistributionPoint[] dists = dist.GetDistributionPoints();
			foreach (DistributionPoint p in dists)
			{
				DistributionPointName distributionPointName = p.GetDistributionPoint();
				if (DistributionPointName.FULL_NAME != distributionPointName.GetType())
				{
					continue;
				}
				GeneralNames generalNames = (GeneralNames)distributionPointName.GetName();
				GeneralName[] names = generalNames.GetNames();
				foreach (GeneralName name in names)
				{
					if (name.GetTagNo() != GeneralName.uniformResourceIdentifier)
					{
						continue;
					}
					DERIA5String derStr = DERIA5String.GetInstance((ASN1TaggedObject)name.ToASN1Primitive
						(), false);
					return derStr.GetString();
				}
			}
			return null;
		}

		/// <summary>Gets the CRL object using a CRL URL.</summary>
		/// <param name="url">the URL where to get the CRL</param>
		/// <returns>a CRL object</returns>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="java.security.cert.CertificateException"/>
		/// <exception cref="java.security.cert.CRLException"/>
		public static CRL GetCRL(String url)
		{
			if (url == null)
			{
				return null;
			}
			Stream @is = new Uri(url).OpenStream();
			CertificateFactory cf = CertificateFactory.GetInstance("X.509");
			return (CRL)cf.GenerateCRL(@is);
		}

		// Online Certificate Status Protocol
		/// <summary>Retrieves the OCSP URL from the given certificate.</summary>
		/// <param name="certificate">the certificate</param>
		/// <returns>the URL or null</returns>
		/// <exception cref="System.IO.IOException"/>
		public static String GetOCSPURL(X509Certificate certificate)
		{
			ASN1Primitive obj;
			try
			{
				obj = GetExtensionValue(certificate, Extension.authorityInfoAccess.GetId());
				if (obj == null)
				{
					return null;
				}
				ASN1Sequence AccessDescriptions = (ASN1Sequence)obj;
				for (int i = 0; i < AccessDescriptions.Size(); i++)
				{
					ASN1Sequence AccessDescription = (ASN1Sequence)AccessDescriptions.GetObjectAt(i);
					if (AccessDescription.Size() != 2)
					{
						continue;
					}
					else
					{
						if (AccessDescription.GetObjectAt(0) is ASN1ObjectIdentifier)
						{
							ASN1ObjectIdentifier id = (ASN1ObjectIdentifier)AccessDescription.GetObjectAt(0);
							if (SecurityIDs.ID_OCSP.Equals(id.GetId()))
							{
								ASN1Primitive description = (ASN1Primitive)AccessDescription.GetObjectAt(1);
								String AccessLocation = GetStringFromGeneralName(description);
								if (AccessLocation == null)
								{
									return "";
								}
								else
								{
									return AccessLocation;
								}
							}
						}
					}
				}
			}
			catch (System.IO.IOException)
			{
				return null;
			}
			return null;
		}

		// Time Stamp Authority
		/// <summary>Gets the URL of the TSA if it's available on the certificate</summary>
		/// <param name="certificate">a certificate</param>
		/// <returns>a TSA URL</returns>
		/// <exception cref="System.IO.IOException"/>
		public static String GetTSAURL(X509Certificate certificate)
		{
			byte[] der = certificate.GetExtensionValue(SecurityIDs.ID_TSA);
			if (der == null)
			{
				return null;
			}
			ASN1Primitive asn1obj;
			try
			{
				asn1obj = ASN1Primitive.FromByteArray(der);
				DEROctetString octets = (DEROctetString)asn1obj;
				asn1obj = ASN1Primitive.FromByteArray(octets.GetOctets());
				ASN1Sequence asn1seq = ASN1Sequence.GetInstance(asn1obj);
				return GetStringFromGeneralName(asn1seq.GetObjectAt(1).ToASN1Primitive());
			}
			catch (System.IO.IOException)
			{
				return null;
			}
		}

		// helper methods
		/// <param name="certificate">the certificate from which we need the ExtensionValue</param>
		/// <param name="oid">the Object Identifier value for the extension.</param>
		/// <returns>the extension value as an ASN1Primitive object</returns>
		/// <exception cref="System.IO.IOException"/>
		private static ASN1Primitive GetExtensionValue(X509Certificate certificate, String
			 oid)
		{
			byte[] bytes = certificate.GetExtensionValue(oid);
			if (bytes == null)
			{
				return null;
			}
			ASN1InputStream aIn = new ASN1InputStream(new MemoryStream(bytes));
			ASN1OctetString octs = (ASN1OctetString)aIn.ReadObject();
			aIn = new ASN1InputStream(new MemoryStream(octs.GetOctets()));
			return aIn.ReadObject();
		}

		/// <summary>Gets a String from an ASN1Primitive</summary>
		/// <param name="names">the ASN1Primitive</param>
		/// <returns>a human-readable String</returns>
		/// <exception cref="System.IO.IOException"/>
		private static String GetStringFromGeneralName(ASN1Primitive names)
		{
			ASN1TaggedObject taggedObject = (ASN1TaggedObject)names;
			return com.itextpdf.io.util.JavaUtil.GetStringForBytes(ASN1OctetString.GetInstance
				(taggedObject, false).GetOctets(), "ISO-8859-1");
		}
	}
}
