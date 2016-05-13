/*
$Id: a0e3c4062294bfc0778c4d0bacc129182d255d4e $

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
using iTextSharp.IO;
using iTextSharp.IO.Log;
using iTextSharp.IO.Source;
using iTextSharp.Kernel;
using iTextSharp.Kernel.Crypto;
using iTextSharp.Kernel.Pdf.Filters;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfOutputStream : OutputStream<iTextSharp.Kernel.Pdf.PdfOutputStream
		>
	{
		private const long serialVersionUID = -548180479472231600L;

		private static readonly byte[] stream = ByteUtils.GetIsoBytes("stream\n");

		private static readonly byte[] endstream = ByteUtils.GetIsoBytes("\nendstream");

		private static readonly byte[] openDict = ByteUtils.GetIsoBytes("<<");

		private static readonly byte[] closeDict = ByteUtils.GetIsoBytes(">>");

		private static readonly byte[] endIndirect = ByteUtils.GetIsoBytes(" R");

		private static readonly byte[] endIndirectWithZeroGenNr = ByteUtils.GetIsoBytes(" 0 R"
			);

		private byte[] duplicateContentBuffer = null;

		/// <summary>Document associated with PdfOutputStream.</summary>
		protected internal PdfDocument document = null;

		/// <summary>Contains the business logic for cryptography.</summary>
		protected internal PdfEncryption crypto;

		public PdfOutputStream(Stream outputStream)
			: base(outputStream)
		{
		}

		// For internal usage only
		public virtual iTextSharp.Kernel.Pdf.PdfOutputStream Write(PdfObject pdfObject)
		{
			if (pdfObject.CheckState(PdfObject.MUST_BE_INDIRECT) && document != null)
			{
				pdfObject.MakeIndirect(document);
				pdfObject = pdfObject.GetIndirectReference();
			}
			if (pdfObject.CheckState(PdfObject.READ_ONLY))
			{
				throw new PdfException(PdfException.CannotWriteObjectAfterItWasReleased);
			}
			switch (pdfObject.GetType())
			{
				case PdfObject.ARRAY:
				{
					Write((PdfArray)pdfObject);
					break;
				}

				case PdfObject.DICTIONARY:
				{
					Write((PdfDictionary)pdfObject);
					break;
				}

				case PdfObject.INDIRECT_REFERENCE:
				{
					Write((PdfIndirectReference)pdfObject);
					break;
				}

				case PdfObject.NAME:
				{
					Write((PdfName)pdfObject);
					break;
				}

				case PdfObject.NULL:
				case PdfObject.BOOLEAN:
				{
					Write((PdfPrimitiveObject)pdfObject);
					break;
				}

				case PdfObject.LITERAL:
				{
					Write((PdfLiteral)pdfObject);
					break;
				}

				case PdfObject.STRING:
				{
					Write((PdfString)pdfObject);
					break;
				}

				case PdfObject.NUMBER:
				{
					Write((PdfNumber)pdfObject);
					break;
				}

				case PdfObject.STREAM:
				{
					Write((PdfStream)pdfObject);
					break;
				}
			}
			return this;
		}

		private void Write(PdfArray pdfArray)
		{
			WriteByte('[');
			for (int i = 0; i < pdfArray.Size(); i++)
			{
				PdfObject value = pdfArray.Get(i, false);
				PdfIndirectReference indirectReference;
				if ((indirectReference = value.GetIndirectReference()) != null)
				{
					Write(indirectReference);
				}
				else
				{
					Write(value);
				}
				if (i < pdfArray.Size() - 1)
				{
					WriteSpace();
				}
			}
			WriteByte(']');
		}

		private void Write(PdfDictionary pdfDictionary)
		{
			WriteBytes(openDict);
			foreach (KeyValuePair<PdfName, PdfObject> entry in pdfDictionary.EntrySet())
			{
				bool isAlreadyWriteSpace = false;
				Write(entry.Key);
				PdfObject value = entry.Value;
				if (value == null)
				{
					Logger logger = LoggerFactory.GetLogger(typeof(iTextSharp.Kernel.Pdf.PdfOutputStream
						));
					logger.Warn(String.Format(LogMessageConstant.INVALID_KEY_VALUE_KEY_0_HAS_NULL_VALUE
						, entry.Key));
					value = PdfNull.PDF_NULL;
				}
				if ((value.GetType() == PdfObject.NUMBER || value.GetType() == PdfObject.LITERAL 
					|| value.GetType() == PdfObject.BOOLEAN || value.GetType() == PdfObject.NULL || 
					value.GetType() == PdfObject.INDIRECT_REFERENCE || value.CheckState(PdfObject.MUST_BE_INDIRECT
					)))
				{
					isAlreadyWriteSpace = true;
					WriteSpace();
				}
				PdfIndirectReference indirectReference;
				if ((indirectReference = value.GetIndirectReference()) != null)
				{
					if (!isAlreadyWriteSpace)
					{
						WriteSpace();
					}
					Write(indirectReference);
				}
				else
				{
					Write(value);
				}
			}
			WriteBytes(closeDict);
		}

		private void Write(PdfIndirectReference indirectReference)
		{
			if (document != null && !indirectReference.GetDocument().Equals(document))
			{
				throw new PdfException(PdfException.PdfInderectObjectBelongToOtherPdfDocument);
			}
			if (indirectReference.GetRefersTo() == null)
			{
				Write(PdfNull.PDF_NULL);
			}
			else
			{
				if (indirectReference.GetGenNumber() == 0)
				{
					WriteInteger(indirectReference.GetObjNumber()).WriteBytes(endIndirectWithZeroGenNr
						);
				}
				else
				{
					WriteInteger(indirectReference.GetObjNumber()).WriteSpace().WriteInteger(indirectReference
						.GetGenNumber()).WriteBytes(endIndirect);
				}
			}
		}

		private void Write(PdfPrimitiveObject pdfPrimitive)
		{
			WriteBytes(pdfPrimitive.GetInternalContent());
		}

		private void Write(PdfLiteral literal)
		{
			literal.SetPosition(GetCurrentPos());
			WriteBytes(literal.GetInternalContent());
		}

		private void Write(PdfString pdfString)
		{
			pdfString.Encrypt(crypto);
			if (pdfString.IsHexWriting())
			{
				WriteByte('<');
				WriteBytes(pdfString.GetInternalContent());
				WriteByte('>');
			}
			else
			{
				WriteByte('(');
				WriteBytes(pdfString.GetInternalContent());
				WriteByte(')');
			}
		}

		private void Write(PdfName name)
		{
			WriteByte('/');
			WriteBytes(name.GetInternalContent());
		}

		private void Write(PdfNumber pdfNumber)
		{
			if (pdfNumber.HasContent())
			{
				WriteBytes(pdfNumber.GetInternalContent());
			}
			else
			{
				if (pdfNumber.IsDoubleNumber())
				{
					WriteDouble(pdfNumber.GetValue());
				}
				else
				{
					WriteInteger(pdfNumber.IntValue());
				}
			}
		}

		private bool IsNotMetadataPdfStream(PdfStream pdfStream)
		{
			return pdfStream.GetAsName(PdfName.Type) == null || (pdfStream.GetAsName(PdfName.
				Type) != null && !pdfStream.GetAsName(PdfName.Type).Equals(PdfName.Metadata));
		}

		private void Write(PdfStream pdfStream)
		{
			try
			{
				bool userDefinedCompression = pdfStream.GetCompressionLevel() != CompressionConstants
					.UNDEFINED_COMPRESSION;
				if (!userDefinedCompression)
				{
					int defaultCompressionLevel = document != null ? document.GetWriter().GetCompressionLevel
						() : CompressionConstants.DEFAULT_COMPRESSION;
					pdfStream.SetCompressionLevel(defaultCompressionLevel);
				}
				bool toCompress = pdfStream.GetCompressionLevel() != CompressionConstants.NO_COMPRESSION;
				bool allowCompression = !pdfStream.ContainsKey(PdfName.Filter) && IsNotMetadataPdfStream
					(pdfStream);
				if (pdfStream.GetInputStream() != null)
				{
					Stream fout = this;
					DeflaterOutputStream def = null;
					OutputStreamEncryption ose = null;
					if (crypto != null && !crypto.IsEmbeddedFilesOnly())
					{
						fout = ose = crypto.GetEncryptionStream(fout);
					}
					if (toCompress && (allowCompression || userDefinedCompression))
					{
						UpdateCompressionFilter(pdfStream);
						fout = def = new DeflaterOutputStream(fout, pdfStream.GetCompressionLevel(), 0x8000
							);
					}
					this.Write((PdfDictionary)pdfStream);
					WriteBytes(iTextSharp.Kernel.Pdf.PdfOutputStream.stream);
					long beginStreamContent = GetCurrentPos();
					byte[] buf = new byte[4192];
					while (true)
					{
						int n = pdfStream.GetInputStream().Read(buf);
						if (n <= 0)
						{
							break;
						}
						fout.Write(buf, 0, n);
					}
					if (def != null)
					{
						def.Finish();
					}
					if (ose != null)
					{
						ose.Finish();
					}
					PdfNumber length = pdfStream.GetAsNumber(PdfName.Length);
					length.SetValue((int)(GetCurrentPos() - beginStreamContent));
					pdfStream.UpdateLength(length.IntValue());
					WriteBytes(iTextSharp.Kernel.Pdf.PdfOutputStream.endstream);
				}
				else
				{
					//When document is opened in stamping mode the output stream can be uninitialized.
					//We have to initialize it and write all data from streams input to streams output.
					if (pdfStream.GetOutputStream() == null && pdfStream.GetIndirectReference().GetReader
						() != null)
					{
						// If new specific compression is set for stream,
						// then compressed stream should be decoded and written with new compression settings
						byte[] bytes = pdfStream.GetIndirectReference().GetReader().ReadStreamBytes(pdfStream
							, false);
						if (userDefinedCompression)
						{
							bytes = DecodeFlateBytes(pdfStream, bytes);
						}
						pdfStream.InitOutputStream(new ByteArrayOutputStream(bytes.Length));
						pdfStream.GetOutputStream().Write(bytes);
					}
					System.Diagnostics.Debug.Assert(pdfStream.GetOutputStream() != null, "PdfStream lost OutputStream"
						);
					ByteArrayOutputStream byteArrayStream;
					try
					{
						if (toCompress && !ContainsFlateFilter(pdfStream) && (allowCompression || userDefinedCompression
							))
						{
							// compress
							UpdateCompressionFilter(pdfStream);
							byteArrayStream = new ByteArrayOutputStream();
							DeflaterOutputStream zip = new DeflaterOutputStream(byteArrayStream, pdfStream.GetCompressionLevel
								());
							if (pdfStream is PdfObjectStream)
							{
								PdfObjectStream objectStream = (PdfObjectStream)pdfStream;
								((ByteArrayOutputStream)objectStream.GetIndexStream().GetOutputStream()).WriteTo(
									zip);
								((ByteArrayOutputStream)objectStream.GetOutputStream().GetOutputStream()).WriteTo
									(zip);
							}
							else
							{
								System.Diagnostics.Debug.Assert(pdfStream.GetOutputStream() != null, "Error in outputStream"
									);
								((ByteArrayOutputStream)pdfStream.GetOutputStream().GetOutputStream()).WriteTo(zip
									);
							}
							zip.Close();
						}
						else
						{
							if (pdfStream is PdfObjectStream)
							{
								PdfObjectStream objectStream = (PdfObjectStream)pdfStream;
								byteArrayStream = new ByteArrayOutputStream();
								((ByteArrayOutputStream)objectStream.GetIndexStream().GetOutputStream()).WriteTo(
									byteArrayStream);
								((ByteArrayOutputStream)objectStream.GetOutputStream().GetOutputStream()).WriteTo
									(byteArrayStream);
							}
							else
							{
								System.Diagnostics.Debug.Assert(pdfStream.GetOutputStream() != null, "Error in outputStream"
									);
								byteArrayStream = (ByteArrayOutputStream)pdfStream.GetOutputStream().GetOutputStream
									();
							}
						}
						if (CheckEncryption(pdfStream))
						{
							ByteArrayOutputStream encodedStream = new ByteArrayOutputStream();
							OutputStreamEncryption ose = crypto.GetEncryptionStream(encodedStream);
							byteArrayStream.WriteTo(ose);
							ose.Finish();
							byteArrayStream = encodedStream;
						}
					}
					catch (System.IO.IOException ioe)
					{
						throw new PdfException(PdfException.IoException, ioe);
					}
					pdfStream.Put(PdfName.Length, new PdfNumber(byteArrayStream.Length));
					pdfStream.UpdateLength((int)byteArrayStream.Length);
					this.Write((PdfDictionary)pdfStream);
					WriteBytes(iTextSharp.Kernel.Pdf.PdfOutputStream.stream);
					byteArrayStream.WriteTo(this);
					WriteBytes(iTextSharp.Kernel.Pdf.PdfOutputStream.endstream);
				}
			}
			catch (System.IO.IOException e)
			{
				throw new PdfException(PdfException.CannotWritePdfStream, e, pdfStream);
			}
		}

		protected internal virtual bool CheckEncryption(PdfStream pdfStream)
		{
			if (crypto == null || crypto.IsEmbeddedFilesOnly())
			{
				return false;
			}
			else
			{
				PdfObject filter = pdfStream.Get(PdfName.Filter, true);
				if (filter != null)
				{
					if (PdfName.Crypt.Equals(filter))
					{
						return false;
					}
					else
					{
						if (filter.GetType() == PdfObject.ARRAY)
						{
							PdfArray filters = (PdfArray)filter;
							if (!filters.IsEmpty() && PdfName.Crypt.Equals(filters.Get(0, true)))
							{
								return false;
							}
						}
					}
				}
				return true;
			}
		}

		protected internal virtual bool ContainsFlateFilter(PdfStream pdfStream)
		{
			PdfObject filter = pdfStream.Get(PdfName.Filter);
			if (filter != null)
			{
				if (filter.GetType() == PdfObject.NAME)
				{
					if (PdfName.FlateDecode.Equals(filter))
					{
						return true;
					}
				}
				else
				{
					if (filter.GetType() == PdfObject.ARRAY)
					{
						if (((PdfArray)filter).Contains(PdfName.FlateDecode))
						{
							return true;
						}
					}
					else
					{
						throw new PdfException(PdfException.FilterIsNotANameOrArray);
					}
				}
			}
			return false;
		}

		protected internal virtual void UpdateCompressionFilter(PdfStream pdfStream)
		{
			PdfObject filter = pdfStream.Get(PdfName.Filter);
			if (filter == null)
			{
				pdfStream.Put(PdfName.Filter, PdfName.FlateDecode);
			}
			else
			{
				PdfArray filters = new PdfArray();
				filters.Add(PdfName.FlateDecode);
				if (filter is PdfArray)
				{
					filters.AddAll((PdfArray)filter);
				}
				else
				{
					filters.Add(filter);
				}
				PdfObject decodeParms = pdfStream.Get(PdfName.DecodeParms);
				if (decodeParms != null)
				{
					if (decodeParms is PdfDictionary)
					{
						PdfArray array = new PdfArray();
						array.Add(new PdfNull());
						array.Add(decodeParms);
						pdfStream.Put(PdfName.DecodeParms, array);
					}
					else
					{
						if (decodeParms is PdfArray)
						{
							((PdfArray)decodeParms).Add(0, new PdfNull());
						}
						else
						{
							throw new PdfException(PdfException.DecodeParameterType1IsNotSupported).SetMessageParams
								(decodeParms.GetType().ToString());
						}
					}
				}
				pdfStream.Put(PdfName.Filter, filters);
			}
		}

		protected internal virtual byte[] DecodeFlateBytes(PdfStream stream, byte[] bytes
			)
		{
			PdfObject filterObject = stream.Get(PdfName.Filter);
			if (filterObject == null)
			{
				return bytes;
			}
			// check if flateDecode filter is on top
			PdfName filterName;
			PdfArray filtersArray = null;
			if (filterObject is PdfName)
			{
				filterName = (PdfName)filterObject;
			}
			else
			{
				if (filterObject is PdfArray)
				{
					filtersArray = (PdfArray)filterObject;
					filterName = filtersArray.GetAsName(0);
				}
				else
				{
					throw new PdfException(PdfException.FilterIsNotANameOrArray);
				}
			}
			if (!PdfName.FlateDecode.Equals(filterName))
			{
				return bytes;
			}
			// get decode params if present
			PdfDictionary decodeParams;
			PdfArray decodeParamsArray = null;
			PdfObject decodeParamsObject = stream.Get(PdfName.DecodeParms);
			if (decodeParamsObject == null)
			{
				decodeParams = null;
			}
			else
			{
				if (decodeParamsObject.GetType() == PdfObject.DICTIONARY)
				{
					decodeParams = (PdfDictionary)decodeParamsObject;
				}
				else
				{
					if (decodeParamsObject.GetType() == PdfObject.ARRAY)
					{
						decodeParamsArray = (PdfArray)decodeParamsObject;
						decodeParams = decodeParamsArray.GetAsDictionary(0);
					}
					else
					{
						throw new PdfException(PdfException.DecodeParameterType1IsNotSupported).SetMessageParams
							(decodeParamsObject.GetType().ToString());
					}
				}
			}
			// decode
			byte[] res = FlateDecodeFilter.FlateDecode(bytes, true);
			if (res == null)
			{
				res = FlateDecodeFilter.FlateDecode(bytes, false);
			}
			bytes = FlateDecodeFilter.DecodePredictor(res, decodeParams);
			//remove filter and decode params
			filterObject = null;
			if (filtersArray != null)
			{
				filtersArray.Remove(0);
				if (filtersArray.Size() == 1)
				{
					filterObject = filtersArray.Get(0);
				}
				else
				{
					if (!filtersArray.IsEmpty())
					{
						filterObject = filtersArray;
					}
				}
			}
			decodeParamsObject = null;
			if (decodeParamsArray != null)
			{
				decodeParamsArray.Remove(0);
				if (decodeParamsArray.Size() == 1 && decodeParamsArray.Get(0).GetType() != PdfObject
					.NULL)
				{
					decodeParamsObject = decodeParamsArray.Get(0);
				}
				else
				{
					if (!decodeParamsArray.IsEmpty())
					{
						decodeParamsObject = decodeParamsArray;
					}
				}
			}
			if (filterObject == null)
			{
				stream.Remove(PdfName.Filter);
			}
			else
			{
				stream.Put(PdfName.Filter, filterObject);
			}
			if (decodeParamsObject == null)
			{
				stream.Remove(PdfName.DecodeParms);
			}
			else
			{
				stream.Put(PdfName.DecodeParms, decodeParamsObject);
			}
			return bytes;
		}
	}
}
