/*
$Id: cc50da4c0c8a8cc12290f1556dec7eddc64c1357 $

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
using System.Text;
using com.itextpdf.io.font;
using com.itextpdf.io.source;
using com.itextpdf.io.util;
using java.nio.charset;

namespace com.itextpdf.kernel.pdf
{
	/// <summary>
	/// A
	/// <c>PdfString</c>
	/// -class is the PDF-equivalent of a
	/// JAVA-
	/// <c>String</c>
	/// -object.
	/// <p/>
	/// A string is a sequence of characters delimited by parenthesis.
	/// If a string is too long to be conveniently placed on a single line, it may
	/// be split across multiple lines by using the backslash character (\) at the
	/// end of a line to indicate that the string continues on the following line.
	/// Within a string, the backslash character is used as an escape to specify
	/// unbalanced parenthesis, non-printing ASCII characters, and the backslash
	/// character itself. Use of the \<i>ddd</i> escape sequence is the preferred
	/// way to represent characters outside the printable ASCII character set.<br />
	/// This object is described in the 'Portable Document Format Reference Manual
	/// version 1.7' section 3.2.3 (page 53-56).
	/// <p/>
	/// <see cref="PdfObject"/>
	/// </summary>
	public class PdfString : PdfPrimitiveObject
	{
		private const long serialVersionUID = 390789504287887010L;

		private static String defaultCharset = "UTF-8";

		protected internal String value;

		protected internal String encoding;

		protected internal bool hexWriting = false;

		private int decryptInfoNum = 0;

		private int decryptInfoGen = 0;

		public PdfString(String value, String encoding)
			: base()
		{
			/*
			* using for decryption
			* */
			/*
			* using for decryption
			* */
			this.value = value;
			this.encoding = encoding;
		}

		public PdfString(String value)
			: this(value, null)
		{
		}

		public PdfString(byte[] content)
			: base()
		{
			if (content != null && content.Length > 0)
			{
				StringBuilder str = new StringBuilder(content.Length);
				foreach (byte b in content)
				{
					str.Append((char)(b & 0xff));
				}
				this.value = str.ToString();
			}
			else
			{
				this.value = "";
			}
		}

		/// <summary>Only PdfReader can use this method!</summary>
		protected internal PdfString(byte[] content, bool hexWriting)
			: base(content)
		{
			this.hexWriting = hexWriting;
		}

		private PdfString()
			: base()
		{
		}

		public override byte GetType()
		{
			return STRING;
		}

		public virtual bool IsHexWriting()
		{
			return hexWriting;
		}

		public virtual com.itextpdf.kernel.pdf.PdfString SetHexWriting(bool hexWriting)
		{
			if (value == null)
			{
				GenerateValue();
				content = null;
			}
			this.hexWriting = hexWriting;
			return this;
		}

		public virtual String GetValue()
		{
			if (value == null)
			{
				GenerateValue();
			}
			return value;
		}

		/// <summary>Gets the encoding of this string.</summary>
		public virtual String GetEncoding()
		{
			return encoding;
		}

		/// <summary>Sets the encoding of this string.</summary>
		/// <remarks>
		/// Sets the encoding of this string.
		/// NOTE. Byte content will be removed.
		/// </remarks>
		public virtual void SetEncoding(String encoding)
		{
			this.encoding = encoding;
			this.content = null;
		}

		/// <summary>
		/// Returns the Unicode
		/// <c>String</c>
		/// value of this
		/// <c>PdfString</c>
		/// -object.
		/// </summary>
		public virtual String ToUnicodeString()
		{
			if (encoding != null && encoding.Length != 0)
			{
				return GetValue();
			}
			if (content == null)
			{
				GenerateContent();
			}
			byte[] b = PdfTokenizer.DecodeStringContent(content, hexWriting);
			if (b.Length >= 2 && b[0] == -2 && b[1] == -1)
			{
				return PdfEncodings.ConvertToString(b, PdfEncodings.UNICODE_BIG);
			}
			else
			{
				return PdfEncodings.ConvertToString(b, PdfEncodings.PDF_DOC_ENCODING);
			}
		}

		/// <summary>Gets bytes of String-value considering encoding.</summary>
		/// <returns>byte array</returns>
		public virtual byte[] GetValueBytes()
		{
			// Analog of com.itextpdf.text.pdf.PdfString.getBytes() method in iText5.
			if (value == null)
			{
				GenerateValue();
			}
			if (encoding != null && encoding.Equals(PdfEncodings.UNICODE_BIG) && PdfEncodings
				.IsPdfDocEncoding(value))
			{
				return PdfEncodings.ConvertToBytes(value, PdfEncodings.PDF_DOC_ENCODING);
			}
			else
			{
				return PdfEncodings.ConvertToBytes(value, encoding);
			}
		}

		/// <summary>Marks object to be saved as indirect.</summary>
		/// <param name="document">a document the indirect reference will belong to.</param>
		/// <returns>object itself.</returns>
		public override PdfObject MakeIndirect(PdfDocument document)
		{
			return (com.itextpdf.kernel.pdf.PdfString)base.MakeIndirect(document);
		}

		/// <summary>Marks object to be saved as indirect.</summary>
		/// <param name="document">a document the indirect reference will belong to.</param>
		/// <returns>object itself.</returns>
		public override PdfObject MakeIndirect(PdfDocument document, PdfIndirectReference
			 reference)
		{
			return (com.itextpdf.kernel.pdf.PdfString)base.MakeIndirect(document, reference);
		}

		/// <summary>Copies object to a specified document.</summary>
		/// <remarks>
		/// Copies object to a specified document.
		/// Works only for objects that are read from existing document, otherwise an exception is thrown.
		/// </remarks>
		/// <param name="document">document to copy object to.</param>
		/// <returns>copied object.</returns>
		public override PdfObject CopyTo(PdfDocument document)
		{
			return (com.itextpdf.kernel.pdf.PdfString)base.CopyTo(document, true);
		}

		/// <summary>Copies object to a specified document.</summary>
		/// <remarks>
		/// Copies object to a specified document.
		/// Works only for objects that are read from existing document, otherwise an exception is thrown.
		/// </remarks>
		/// <param name="document">document to copy object to.</param>
		/// <param name="allowDuplicating">
		/// indicates if to allow copy objects which already have been copied.
		/// If object is associated with any indirect reference and allowDuplicating is false then already existing reference will be returned instead of copying object.
		/// If allowDuplicating is true then object will be copied and new indirect reference will be assigned.
		/// </param>
		/// <returns>copied object.</returns>
		public override PdfObject CopyTo(PdfDocument document, bool allowDuplicating)
		{
			return (com.itextpdf.kernel.pdf.PdfString)base.CopyTo(document, allowDuplicating);
		}

		public override String ToString()
		{
			if (value == null)
			{
				return new String(content, Charset.ForName(defaultCharset));
			}
			else
			{
				return GetValue();
			}
		}

		protected internal virtual void GenerateValue()
		{
			System.Diagnostics.Debug.Assert(content != null, "No byte[] content to generate value"
				);
			value = ConvertBytesToString(PdfTokenizer.DecodeStringContent(content, hexWriting
				));
		}

		protected internal override void GenerateContent()
		{
			content = EncodeBytes(GetValueBytes());
		}

		/// <summary>
		/// Decrypt content of an encrypted
		/// <c>PdfString</c>
		/// .
		/// </summary>
		protected internal virtual com.itextpdf.kernel.pdf.PdfString Decrypt(PdfEncryption
			 decrypt)
		{
			if (decrypt != null)
			{
				System.Diagnostics.Debug.Assert(content != null, "No byte content to decrypt value"
					);
				byte[] decodedContent = PdfTokenizer.DecodeStringContent(content, hexWriting);
				content = null;
				decrypt.SetHashKeyForNextObject(decryptInfoNum, decryptInfoGen);
				value = new String(decrypt.DecryptByteArray(decodedContent), Charset.ForName(defaultCharset
					));
			}
			return this;
		}

		/// <summary>
		/// Encrypt content of
		/// <c>value</c>
		/// and set as content.
		/// <c>generateContent()</c>
		/// won't be called.
		/// </summary>
		/// <param name="encrypt">@see PdfEncryption</param>
		/// <returns>true if value was encrypted, otherwise false.</returns>
		protected internal virtual bool Encrypt(PdfEncryption encrypt)
		{
			if (encrypt != null && !encrypt.IsEmbeddedFilesOnly())
			{
				byte[] b = encrypt.EncryptByteArray(GetValueBytes());
				content = EncodeBytes(b);
				return true;
			}
			return false;
		}

		/// <summary>Escape special symbols or convert to hexadecimal string.</summary>
		/// <remarks>
		/// Escape special symbols or convert to hexadecimal string.
		/// This method don't change either
		/// <c>value</c>
		/// or
		/// <c>content</c>
		/// ot the
		/// <c>PdfString</c>
		/// .
		/// </remarks>
		/// <param name="bytes">byte array to manipulate with.</param>
		/// <returns>Hexadecimal string or string with escaped symbols in byte array view.</returns>
		protected internal virtual byte[] EncodeBytes(byte[] bytes)
		{
			if (hexWriting)
			{
				ByteBuffer buf = new ByteBuffer(bytes.Length * 2);
				foreach (byte b in bytes)
				{
					buf.AppendHex(b);
				}
				return buf.GetInternalBuffer();
			}
			else
			{
				ByteBuffer buf = StreamUtil.CreateBufferedEscapedString(bytes);
				return buf.ToByteArray(1, buf.Size() - 2);
			}
		}

		protected internal override PdfObject NewInstance()
		{
			return new com.itextpdf.kernel.pdf.PdfString();
		}

		protected internal override void CopyContent(PdfObject from, PdfDocument document
			)
		{
			base.CopyContent(from, document);
			com.itextpdf.kernel.pdf.PdfString @string = (com.itextpdf.kernel.pdf.PdfString)from;
			value = @string.value;
			hexWriting = @string.hexWriting;
		}

		internal virtual void SetDecryptInfoNum(int decryptInfoNum)
		{
			this.decryptInfoNum = decryptInfoNum;
		}

		internal virtual void SetDecryptInfoGen(int decryptInfoGen)
		{
			this.decryptInfoGen = decryptInfoGen;
		}

		private String ConvertBytesToString(byte[] bytes)
		{
			StringBuilder buffer = new StringBuilder(bytes.Length);
			foreach (byte b in bytes)
			{
				buffer.Append((char)(b & 0xff));
			}
			return buffer.ToString();
		}
	}
}
