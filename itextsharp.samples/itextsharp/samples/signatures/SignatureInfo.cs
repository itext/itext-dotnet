/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV

*/
using System;
using System.Collections.Generic;
using iTextSharp.Kernel.Geom;

namespace iTextSharp.Samples.Signatures
{
	public class SignatureInfo
	{
		private int revisionNumber;

		private bool signatureCoversWholeDocument;

		private Rectangle signaturePosition;

		private String digestAlgorithm;

		private String encryptionAlgorithm;

		private String filterSubtype;

		private String signerName;

		private String alternativeSignerName;

		private DateTime signDate;

		private DateTime timeStamp;

		private String timeStampService;

		private String location;

		private String reason;

		private String contactInfo;

		private bool isCertifiaction;

		private bool isFieldsFillAllowed;

		private bool isAddingAnnotationsAllowed;

		private IList<String> fieldsLocks;

		private IList<CertificateInfo> certificateInfos;

		private String signatureName;

		public virtual void SetRevisionNumber(int revisionNumber)
		{
			this.revisionNumber = revisionNumber;
		}

		public virtual int GetRevisionNumber()
		{
			return revisionNumber;
		}

		public virtual void SetSignatureCoversWholeDocument(bool signatureCoversWholeDocument
			)
		{
			this.signatureCoversWholeDocument = signatureCoversWholeDocument;
		}

		public virtual bool IsSignatureCoversWholeDocument()
		{
			return signatureCoversWholeDocument;
		}

		public virtual void SetSignaturePosition(Rectangle signaturePosition)
		{
			this.signaturePosition = signaturePosition;
		}

		public virtual Rectangle GetSignaturePosition()
		{
			return signaturePosition;
		}

		public virtual void SetDigestAlgorithm(String digestAlgorithm)
		{
			this.digestAlgorithm = digestAlgorithm;
		}

		public virtual String GetDigestAlgorithm()
		{
			return digestAlgorithm;
		}

		public virtual void SetEncryptionAlgorithm(String encryptionAlgorithm)
		{
			this.encryptionAlgorithm = encryptionAlgorithm;
		}

		public virtual String GetEncryptionAlgorithm()
		{
			return encryptionAlgorithm;
		}

		public virtual void SetFilterSubtype(String filterSubtype)
		{
			this.filterSubtype = filterSubtype;
		}

		public virtual String GetFilterSubtype()
		{
			return filterSubtype;
		}

		public virtual void SetSignerName(String signerName)
		{
			this.signerName = signerName;
		}

		public virtual String GetSignerName()
		{
			return signerName;
		}

		public virtual void SetAlternativeSignerName(String alternativeSignerName)
		{
			this.alternativeSignerName = alternativeSignerName;
		}

		public virtual String GetAlternativeSignerName()
		{
			return alternativeSignerName;
		}

		public virtual void SetSignDate(DateTime signDate)
		{
			this.signDate = signDate;
		}

		public virtual DateTime GetSignDate()
		{
			return signDate;
		}

		public virtual void SetTimeStamp(DateTime timeStamp)
		{
			this.timeStamp = timeStamp;
		}

		public virtual DateTime GetTimeStamp()
		{
			return timeStamp;
		}

		public virtual void SetTimeStampService(String timeStampService)
		{
			this.timeStampService = timeStampService;
		}

		public virtual String GetTimeStampService()
		{
			return timeStampService;
		}

		public virtual void SetLocation(String location)
		{
			this.location = location;
		}

		public virtual String GetLocation()
		{
			return location;
		}

		public virtual void SetReason(String reason)
		{
			this.reason = reason;
		}

		public virtual String GetReason()
		{
			return reason;
		}

		public virtual void SetContactInfo(String contactInfo)
		{
			this.contactInfo = contactInfo;
		}

		public virtual String GetContactInfo()
		{
			return contactInfo;
		}

		public virtual void SetIsCertifiaction(bool isCertifiaction)
		{
			this.isCertifiaction = isCertifiaction;
		}

		public virtual bool IsCertifiaction()
		{
			return isCertifiaction;
		}

		public virtual void SetIsFieldsFillAllowed(bool isFieldsFillAllowed)
		{
			this.isFieldsFillAllowed = isFieldsFillAllowed;
		}

		public virtual bool IsFieldsFillAllowed()
		{
			return isFieldsFillAllowed;
		}

		public virtual void SetIsAddingAnnotationsAllowed(bool isAddingAnnotationsAllowed
			)
		{
			this.isAddingAnnotationsAllowed = isAddingAnnotationsAllowed;
		}

		public virtual bool IsAddingAnnotationsAllowed()
		{
			return isAddingAnnotationsAllowed;
		}

		public virtual void SetFieldsLocks(IList<String> fieldsLocks)
		{
			this.fieldsLocks = fieldsLocks;
		}

		public virtual IList<String> GetFieldsLocks()
		{
			return fieldsLocks;
		}

		public virtual void SetCertificateInfos(IList<CertificateInfo> certificateInfos)
		{
			this.certificateInfos = certificateInfos;
		}

		public virtual IList<CertificateInfo> GetCertificateInfos()
		{
			return certificateInfos;
		}

		public virtual void SetSignatureName(String signatureName)
		{
			this.signatureName = signatureName;
		}

		public virtual String GetSignatureName()
		{
			return signatureName;
		}
	}
}
