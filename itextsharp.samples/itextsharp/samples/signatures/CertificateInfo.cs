/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV

*/
using System;
using Org.BouncyCastle.Asn1.X509;

namespace iTextSharp.Samples.Signatures
{
	public class CertificateInfo
	{
		private X509Name issuer;

		private X509Name subject;

		private DateTime validFrom;

		private DateTime validTo;

		public virtual void SetIssuer(X509Name issuer)
		{
			this.issuer = issuer;
		}

		public virtual X509Name GetIssuer()
		{
			return issuer;
		}

		public virtual void SetSubject(X509Name subject)
		{
			this.subject = subject;
		}

		public virtual X509Name GetSubject()
		{
			return subject;
		}

		public virtual void SetValidFrom(DateTime validFrom)
		{
			this.validFrom = validFrom;
		}

		public virtual DateTime GetValidFrom()
		{
			return validFrom;
		}

		public virtual void SetValidTo(DateTime validTo)
		{
			this.validTo = validTo;
		}

		public virtual DateTime GetValidTo()
		{
			return validTo;
		}
	}
}
