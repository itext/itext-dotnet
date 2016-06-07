/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV

*/
using System;
using System.Collections.Generic;

namespace iTextSharp.Samples.Signatures
{
	internal class SignedDocumentInfo
	{
		private List<String> signatureNames;

		private int numberOfTotalRevisions;

		private IList<SignatureInfo> signatureInfos;

		public virtual void SetNumberOfTotalRevisions(int numberOfTotalRevisions)
		{
			this.numberOfTotalRevisions = numberOfTotalRevisions;
		}

		public virtual int GetNumberOfTotalRevisions()
		{
			return numberOfTotalRevisions;
		}

		public virtual void SetSignatureInfos(IList<SignatureInfo> signatureInfos)
		{
			this.signatureInfos = signatureInfos;
		}

		public virtual IList<SignatureInfo> GetSignatureInfos()
		{
			return signatureInfos;
		}
	}
}
