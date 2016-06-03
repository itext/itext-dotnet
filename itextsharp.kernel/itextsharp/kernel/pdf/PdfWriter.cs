/*

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
using System.IO;
using Org.BouncyCastle.Crypto;
using iTextSharp.IO;
using iTextSharp.IO.Log;
using iTextSharp.IO.Source;
using iTextSharp.IO.Util;
using iTextSharp.Kernel;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfWriter : PdfOutputStream
	{
		private static readonly byte[] obj = ByteUtils.GetIsoBytes(" obj\n");

		private static readonly byte[] endobj = ByteUtils.GetIsoBytes("\nendobj\n");

		private Dictionary<PdfWriter.ByteStore, PdfIndirectReference> streamMap = new Dictionary
			<PdfWriter.ByteStore, PdfIndirectReference>();

		private readonly IntHashtable serialized = new IntHashtable();

		private PdfOutputStream duplicateStream = null;

		protected internal WriterProperties properties;

		/// <summary>Currently active object stream.</summary>
		/// <remarks>
		/// Currently active object stream.
		/// Objects are written to the object stream if fullCompression set to true.
		/// </remarks>
		internal PdfObjectStream objectStream = null;

		protected internal IDictionary<int, PdfIndirectReference> copiedObjects = new Dictionary
			<int, PdfIndirectReference>();

		protected internal bool isUserWarnedAboutAcroFormCopying;

		public PdfWriter(Stream os)
			: this(os, new WriterProperties())
		{
		}

		public PdfWriter(Stream os, WriterProperties properties)
			: base(os)
		{
			// For internal usage only
			//forewarned is forearmed
			this.properties = properties;
			EncryptionProperties encryptProps = properties.encryptionProperties;
			if (properties.IsStandardEncryptionUsed())
			{
				crypto = new PdfEncryption(encryptProps.userPassword, encryptProps.ownerPassword, 
					encryptProps.standardEncryptPermissions, encryptProps.encryptionAlgorithm, PdfEncryption
					.GenerateNewDocumentId());
			}
			else
			{
				if (properties.IsPublicKeyEncryptionUsed())
				{
					crypto = new PdfEncryption(encryptProps.publicCertificates, encryptProps.publicKeyEncryptPermissions
						, encryptProps.encryptionAlgorithm);
				}
			}
			if (properties.debugMode)
			{
				SetDebugMode();
			}
		}

		/// <exception cref="System.IO.FileNotFoundException"/>
		public PdfWriter(String filename)
			: this(filename, new WriterProperties())
		{
		}

		/// <exception cref="System.IO.FileNotFoundException"/>
		public PdfWriter(String filename, WriterProperties properties)
			: this(FileUtil.GetBufferedOutputStream(filename), properties)
		{
		}

		/// <summary>Indicates if to use full compression mode.</summary>
		/// <returns>true if to use full compression, false otherwise.</returns>
		public virtual bool IsFullCompression()
		{
			return properties.isFullCompression != null ? (bool)properties.isFullCompression : 
				false;
		}

		/// <summary>Gets default compression level for @see PdfStream.</summary>
		/// <remarks>
		/// Gets default compression level for @see PdfStream.
		/// For more details @see
		/// <see cref="Java.Util.Zip.Deflater"/>
		/// .
		/// </remarks>
		/// <returns>compression level.</returns>
		public virtual int GetCompressionLevel()
		{
			return properties.compressionLevel;
		}

		/// <summary>Sets default compression level for @see PdfStream.</summary>
		/// <remarks>
		/// Sets default compression level for @see PdfStream.
		/// For more details @see
		/// <see cref="Java.Util.Zip.Deflater"/>
		/// .
		/// </remarks>
		/// <param name="compressionLevel">compression level.</param>
		public virtual iTextSharp.Kernel.Pdf.PdfWriter SetCompressionLevel(int compressionLevel
			)
		{
			this.properties.SetCompressionLevel(compressionLevel);
			return this;
		}

		/// <summary>Sets the smart mode.</summary>
		/// <remarks>
		/// Sets the smart mode.
		/// <p/>
		/// In smart mode when resources (such as fonts, images,...) are
		/// encountered, a reference to these resources is saved
		/// in a cache, so that they can be reused.
		/// This requires more memory, but reduces the file size
		/// of the resulting PDF document.
		/// </remarks>
		public virtual iTextSharp.Kernel.Pdf.PdfWriter SetSmartMode(bool smartMode)
		{
			this.properties.smartMode = smartMode;
			return this;
		}

		/// <exception cref="System.IO.IOException"/>
		public override void Write(int b)
		{
			base.Write(b);
			if (duplicateStream != null)
			{
				duplicateStream.Write(b);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		public override void Write(byte[] b)
		{
			base.Write(b);
			if (duplicateStream != null)
			{
				duplicateStream.Write(b);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		public override void Write(byte[] b, int off, int len)
		{
			base.Write(b, off, len);
			if (duplicateStream != null)
			{
				duplicateStream.Write(b, off, len);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		public override void Close()
		{
			base.Close();
			if (duplicateStream != null)
			{
				duplicateStream.Close();
			}
		}

		/// <summary>Gets the current object stream.</summary>
		/// <returns>object stream.</returns>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		internal virtual PdfObjectStream GetObjectStream()
		{
			if (!IsFullCompression())
			{
				return null;
			}
			if (objectStream == null)
			{
				objectStream = new PdfObjectStream(document);
			}
			else
			{
				if (objectStream.GetSize() == PdfObjectStream.MAX_OBJ_STREAM_SIZE)
				{
					objectStream.Flush();
					objectStream = new PdfObjectStream(objectStream);
				}
			}
			return objectStream;
		}

		/// <summary>Flushes the object.</summary>
		/// <remarks>Flushes the object. Override this method if you want to define custom behaviour for object flushing.
		/// 	</remarks>
		/// <param name="pdfObject">object to flush.</param>
		/// <param name="canBeInObjStm">indicates whether object can be placed into object stream.
		/// 	</param>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		protected internal virtual void FlushObject(PdfObject pdfObject, bool canBeInObjStm
			)
		{
			PdfIndirectReference indirectReference = pdfObject.GetIndirectReference();
			if (IsFullCompression() && canBeInObjStm)
			{
				PdfObjectStream objectStream = GetObjectStream();
				objectStream.AddObject(pdfObject);
			}
			else
			{
				indirectReference.SetOffset(GetCurrentPos());
				WriteToBody(pdfObject);
			}
			((PdfIndirectReference)indirectReference.SetState(PdfObject.FLUSHED)).ClearState(
				PdfObject.MUST_BE_FLUSHED);
			switch (pdfObject.GetObjectType())
			{
				case PdfObject.BOOLEAN:
				case PdfObject.NAME:
				case PdfObject.NULL:
				case PdfObject.NUMBER:
				case PdfObject.STRING:
				{
					((PdfPrimitiveObject)pdfObject).content = null;
					break;
				}

				case PdfObject.ARRAY:
				{
					PdfArray array = ((PdfArray)pdfObject);
					MarkArrayContentToFlush(array);
					array.ReleaseContent();
					break;
				}

				case PdfObject.STREAM:
				case PdfObject.DICTIONARY:
				{
					PdfDictionary dictionary = ((PdfDictionary)pdfObject);
					MarkDictionaryContentToFlush(dictionary);
					dictionary.ReleaseContent();
					break;
				}

				case PdfObject.INDIRECT_REFERENCE:
				{
					MarkObjectToFlush(((PdfIndirectReference)pdfObject).GetRefersTo(false));
					break;
				}
			}
		}

		protected internal virtual PdfObject CopyObject(PdfObject obj, PdfDocument document
			, bool allowDuplicating)
		{
			if (obj is PdfIndirectReference)
			{
				obj = ((PdfIndirectReference)obj).GetRefersTo();
			}
			if (obj == null)
			{
				obj = PdfNull.PDF_NULL;
			}
			if (CheckTypeOfPdfDictionary(obj, PdfName.Catalog))
			{
				ILogger logger = LoggerFactory.GetLogger(typeof(PdfReader));
				logger.Warn(LogMessageConstant.MAKE_COPY_OF_CATALOG_DICTIONARY_IS_FORBIDDEN);
				obj = PdfNull.PDF_NULL;
			}
			PdfIndirectReference indirectReference = obj.GetIndirectReference();
			PdfIndirectReference copiedIndirectReference;
			int copyObjectKey = 0;
			if (!allowDuplicating && indirectReference != null)
			{
				copyObjectKey = GetCopyObjectKey(obj);
				copiedIndirectReference = copiedObjects.Get(copyObjectKey);
				if (copiedIndirectReference != null)
				{
					return copiedIndirectReference.GetRefersTo();
				}
			}
			if (properties.smartMode && !CheckTypeOfPdfDictionary(obj, PdfName.Page))
			{
				PdfObject copiedObject = SmartCopyObject(obj);
				if (copiedObject != null)
				{
					return copiedObjects.Get(GetCopyObjectKey(copiedObject)).GetRefersTo();
				}
			}
			PdfObject newObject = obj.NewInstance();
			if (indirectReference != null)
			{
				if (copyObjectKey == 0)
				{
					copyObjectKey = GetCopyObjectKey(obj);
				}
				PdfIndirectReference reference = newObject.MakeIndirect(document).GetIndirectReference
					();
				copiedObjects[copyObjectKey] = reference;
			}
			newObject.CopyContent(obj, document);
			return newObject;
		}

		/// <summary>Writes object to body of PDF document.</summary>
		/// <param name="pdfObj">object to write.</param>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		protected internal virtual void WriteToBody(PdfObject pdfObj)
		{
			if (crypto != null)
			{
				crypto.SetHashKeyForNextObject(pdfObj.GetIndirectReference().GetObjNumber(), pdfObj
					.GetIndirectReference().GetGenNumber());
			}
			WriteInteger(pdfObj.GetIndirectReference().GetObjNumber()).WriteSpace().WriteInteger
				(pdfObj.GetIndirectReference().GetGenNumber()).WriteBytes(obj);
			Write(pdfObj);
			WriteBytes(endobj);
		}

		/// <summary>Writes PDF header.</summary>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		protected internal virtual void WriteHeader()
		{
			WriteByte('%').WriteString(document.GetPdfVersion().ToString()).WriteString("\n%\u00e2\u00e3\u00cf\u00d3\n"
				);
		}

		/// <summary>Flushes all objects which have not been flushed yet.</summary>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		protected internal virtual void FlushWaitingObjects()
		{
			PdfXrefTable xref = document.GetXref();
			bool needFlush = true;
			while (needFlush)
			{
				needFlush = false;
				for (int i = 1; i < xref.Size(); i++)
				{
					PdfIndirectReference indirectReference = xref.Get(i);
					if (indirectReference != null && indirectReference.CheckState(PdfObject.MUST_BE_FLUSHED
						))
					{
						PdfObject obj = indirectReference.GetRefersTo(false);
						if (obj != null)
						{
							obj.Flush();
							needFlush = true;
						}
					}
				}
			}
			if (objectStream != null && objectStream.GetSize() > 0)
			{
				objectStream.Flush();
				objectStream = null;
			}
		}

		/// <summary>Flushes all modified objects which have not been flushed yet.</summary>
		/// <remarks>Flushes all modified objects which have not been flushed yet. Used in case incremental updates.
		/// 	</remarks>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		protected internal virtual void FlushModifiedWaitingObjects()
		{
			PdfXrefTable xref = document.GetXref();
			for (int i = 1; i < xref.Size(); i++)
			{
				PdfIndirectReference indirectReference = xref.Get(i);
				if (null != indirectReference)
				{
					PdfObject obj = indirectReference.GetRefersTo(false);
					if (obj != null && !obj.Equals(objectStream) && obj.IsModified())
					{
						obj.Flush();
					}
				}
			}
			if (objectStream != null && objectStream.GetSize() > 0)
			{
				objectStream.Flush();
				objectStream = null;
			}
		}

		/// <summary>Calculates hash code for object to be copied.</summary>
		/// <remarks>
		/// Calculates hash code for object to be copied.
		/// The hash code and the copied object is the stored in @{link copiedObjects} hash map to avoid duplications.
		/// </remarks>
		/// <param name="obj">object to be copied.</param>
		/// <returns>calculated hash code.</returns>
		protected internal virtual int GetCopyObjectKey(PdfObject obj)
		{
			PdfIndirectReference reference;
			if (obj.IsIndirectReference())
			{
				reference = (PdfIndirectReference)obj;
			}
			else
			{
				reference = obj.GetIndirectReference();
			}
			int result = reference.GetHashCode();
			result = 31 * result + reference.GetDocument().GetHashCode();
			return result;
		}

		private void MarkArrayContentToFlush(PdfArray array)
		{
			foreach (PdfObject item in array)
			{
				MarkObjectToFlush(item);
			}
		}

		private void MarkDictionaryContentToFlush(PdfDictionary dictionary)
		{
			foreach (PdfObject item in dictionary.Values())
			{
				MarkObjectToFlush(item);
			}
		}

		private void MarkObjectToFlush(PdfObject pdfObject)
		{
			if (pdfObject != null)
			{
				PdfIndirectReference indirectReference = pdfObject.GetIndirectReference();
				if (indirectReference != null)
				{
					if (!indirectReference.CheckState(PdfObject.FLUSHED))
					{
						indirectReference.SetState(PdfObject.MUST_BE_FLUSHED);
					}
				}
				else
				{
					if (pdfObject.GetObjectType() == PdfObject.INDIRECT_REFERENCE)
					{
						if (!pdfObject.CheckState(PdfObject.FLUSHED))
						{
							pdfObject.SetState(PdfObject.MUST_BE_FLUSHED);
						}
					}
					else
					{
						if (pdfObject.GetObjectType() == PdfObject.ARRAY)
						{
							MarkArrayContentToFlush((PdfArray)pdfObject);
						}
						else
						{
							if (pdfObject.GetObjectType() == PdfObject.DICTIONARY)
							{
								MarkDictionaryContentToFlush((PdfDictionary)pdfObject);
							}
						}
					}
				}
			}
		}

		private iTextSharp.Kernel.Pdf.PdfWriter SetDebugMode()
		{
			duplicateStream = new PdfOutputStream(new ByteArrayOutputStream());
			return this;
		}

		private PdfObject SmartCopyObject(PdfObject obj)
		{
			PdfWriter.ByteStore streamKey;
			if (obj.IsStream())
			{
				streamKey = new PdfWriter.ByteStore((PdfStream)obj, serialized);
				PdfIndirectReference streamRef = streamMap.Get(streamKey);
				if (streamRef != null)
				{
					return streamRef;
				}
				streamMap[streamKey] = obj.GetIndirectReference();
			}
			else
			{
				if (obj.IsDictionary())
				{
					streamKey = new PdfWriter.ByteStore((PdfDictionary)obj, serialized);
					PdfIndirectReference streamRef = streamMap.Get(streamKey);
					if (streamRef != null)
					{
						return streamRef.GetRefersTo();
					}
					streamMap[streamKey] = obj.GetIndirectReference();
				}
			}
			return null;
		}

		/// <exception cref="System.IO.IOException"/>
		private byte[] GetDebugBytes()
		{
			if (duplicateStream != null)
			{
				duplicateStream.Flush();
				return ((ByteArrayOutputStream)(duplicateStream.GetOutputStream())).ToArray();
			}
			else
			{
				return null;
			}
		}

		private static bool CheckTypeOfPdfDictionary(PdfObject dictionary, PdfName expectedType
			)
		{
			return dictionary.IsDictionary() && expectedType.Equals(((PdfDictionary)dictionary
				).GetAsName(PdfName.Type));
		}

		internal class ByteStore
		{
			private readonly byte[] b;

			private readonly int hash;

			private IDigest md5;

			private void SerObject(PdfObject obj, int level, ByteBufferOutputStream bb, IntHashtable
				 serialized)
			{
				if (level <= 0)
				{
					return;
				}
				if (obj == null)
				{
					bb.Append("$Lnull");
					return;
				}
				PdfIndirectReference reference = null;
				ByteBufferOutputStream savedBb = null;
				if (obj.IsIndirectReference())
				{
					reference = (PdfIndirectReference)obj;
					int key = GetCopyObjectKey(obj);
					if (serialized.ContainsKey(key))
					{
						bb.Append((int)serialized.Get(key));
						return;
					}
					else
					{
						savedBb = bb;
						bb = new ByteBufferOutputStream();
					}
				}
				if (obj.IsStream())
				{
					bb.Append("$B");
					SerDic((PdfDictionary)obj, level - 1, bb, serialized);
					if (level > 0)
					{
						md5.Reset();
						bb.Append(md5.Digest(((PdfStream)obj).GetBytes(false)));
					}
				}
				else
				{
					if (obj.IsDictionary())
					{
						SerDic((PdfDictionary)obj, level - 1, bb, serialized);
					}
					else
					{
						if (obj.IsArray())
						{
							SerArray((PdfArray)obj, level - 1, bb, serialized);
						}
						else
						{
							if (obj.IsString())
							{
								bb.Append("$S").Append(obj.ToString());
							}
							else
							{
								if (obj.IsName())
								{
									bb.Append("$N").Append(obj.ToString());
								}
								else
								{
									bb.Append("$L").Append(obj.ToString());
								}
							}
						}
					}
				}
				if (savedBb != null)
				{
					int key = GetCopyObjectKey(reference);
					if (!serialized.ContainsKey(key))
					{
						serialized.Put(key, CalculateHash(bb.GetBuffer()));
					}
					savedBb.Append(bb);
				}
			}

			private void SerDic(PdfDictionary dic, int level, ByteBufferOutputStream bb, IntHashtable
				 serialized)
			{
				bb.Append("$D");
				if (level <= 0)
				{
					return;
				}
				PdfName[] keys = new PdfName[dic.KeySet().Count];
				dic.KeySet().ToArray(keys);
				iTextSharp.IO.Util.JavaUtil.Sort(keys);
				foreach (Object key in keys)
				{
					if (key.Equals(PdfName.P) && (dic.Get((PdfName)key).IsIndirectReference() || dic.
						Get((PdfName)key).IsDictionary()) || key.Equals(PdfName.Parent))
					{
						// ignore recursive call
						continue;
					}
					SerObject((PdfObject)key, level, bb, serialized);
					SerObject(dic.Get((PdfName)key, false), level, bb, serialized);
				}
			}

			private void SerArray(PdfArray array, int level, ByteBufferOutputStream bb, IntHashtable
				 serialized)
			{
				bb.Append("$A");
				if (level <= 0)
				{
					return;
				}
				for (int k = 0; k < array.Size(); ++k)
				{
					SerObject(array.Get(k, false), level, bb, serialized);
				}
			}

			internal ByteStore(PdfStream str, IntHashtable serialized)
			{
				try
				{
					md5 = Org.BouncyCastle.Security.DigestUtilities.GetDigest("MD5");
				}
				catch (Exception e)
				{
					throw new PdfException(e);
				}
				ByteBufferOutputStream bb = new ByteBufferOutputStream();
				int level = 100;
				SerObject(str, level, bb, serialized);
				this.b = bb.ToByteArray();
				hash = CalculateHash(this.b);
				md5 = null;
			}

			internal ByteStore(PdfDictionary dict, IntHashtable serialized)
			{
				try
				{
					md5 = Org.BouncyCastle.Security.DigestUtilities.GetDigest("MD5");
				}
				catch (Exception e)
				{
					throw new PdfException(e);
				}
				ByteBufferOutputStream bb = new ByteBufferOutputStream();
				int level = 100;
				SerObject(dict, level, bb, serialized);
				this.b = bb.ToByteArray();
				hash = CalculateHash(this.b);
				md5 = null;
			}

			private static int CalculateHash(byte[] b)
			{
				int hash = 0;
				int len = b.Length;
				for (int k = 0; k < len; ++k)
				{
					hash = hash * 31 + (b[k] & 0xff);
				}
				return hash;
			}

			public override bool Equals(Object obj)
			{
				return obj is PdfWriter.ByteStore && GetHashCode() == obj.GetHashCode() && iTextSharp.IO.Util.JavaUtil.ArraysEquals
					(b, ((PdfWriter.ByteStore)obj).b);
			}

			public override int GetHashCode()
			{
				return hash;
			}

			protected internal virtual int GetCopyObjectKey(PdfObject obj)
			{
				PdfIndirectReference reference;
				if (obj.IsIndirectReference())
				{
					reference = (PdfIndirectReference)obj;
				}
				else
				{
					reference = obj.GetIndirectReference();
				}
				int result = reference.GetHashCode();
				result = 31 * result + reference.GetDocument().GetHashCode();
				return result;
			}
		}
	}
}
