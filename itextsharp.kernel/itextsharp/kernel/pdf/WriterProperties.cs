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
using Org.BouncyCastle.X509;

namespace iTextSharp.Kernel.Pdf
{
	public class WriterProperties
	{
		protected internal int compressionLevel;

		/// <summary>Indicates if to use full compression (using object streams).</summary>
		protected internal bool isFullCompression;

		/// <summary>Indicates if the writer copy objects in a smart mode.</summary>
		/// <remarks>
		/// Indicates if the writer copy objects in a smart mode. If so PdfDictionary and PdfStream will be hashed
		/// and reused if there's an object with the same content later.
		/// </remarks>
		protected internal bool smartMode;

		protected internal bool debugMode;

		protected internal bool addXmpMetadata;

		protected internal PdfVersion pdfVersion;

		protected internal EncryptionProperties encryptionProperties;

		public WriterProperties()
		{
			smartMode = false;
			debugMode = false;
			compressionLevel = CompressionConstants.DEFAULT_COMPRESSION;
			isFullCompression = true;
			encryptionProperties = new EncryptionProperties();
		}

		/// <summary>Defines pdf version for the created document.</summary>
		/// <remarks>Defines pdf version for the created document. Default value is PDF_1_7.</remarks>
		/// <param name="version">version for the document.</param>
		/// <returns>
		/// this
		/// <c>WriterProperties</c>
		/// instance
		/// </returns>
		public virtual iTextSharp.Kernel.Pdf.WriterProperties SetPdfVersion(PdfVersion version
			)
		{
			this.pdfVersion = version;
			return this;
		}

		/// <summary>Enables smart mode.</summary>
		/// <remarks>
		/// Enables smart mode.
		/// <p/>
		/// In smart mode when resources (such as fonts, images,...) are
		/// encountered, a reference to these resources is saved
		/// in a cache, so that they can be reused.
		/// This requires more memory, but reduces the file size
		/// of the resulting PDF document.
		/// </remarks>
		/// <returns>
		/// this
		/// <c>WriterProperties</c>
		/// instance
		/// </returns>
		public virtual iTextSharp.Kernel.Pdf.WriterProperties UseSmartMode()
		{
			this.smartMode = true;
			return this;
		}

		/// <summary>
		/// If true, default XMPMetadata based on
		/// <see cref="PdfDocumentInfo"/>
		/// will be added.
		/// </summary>
		/// <returns>
		/// this
		/// <c>WriterProperties</c>
		/// instance
		/// </returns>
		public virtual iTextSharp.Kernel.Pdf.WriterProperties AddXmpMetadata()
		{
			this.addXmpMetadata = true;
			return this;
		}

		/// <summary>Defines the level of compression for the document.</summary>
		/// <remarks>
		/// Defines the level of compression for the document.
		/// See
		/// <see cref="CompressionConstants"/>
		/// </remarks>
		/// <param name="compressionLevel"/>
		/// <returns>
		/// this
		/// <c>WriterProperties</c>
		/// instance
		/// </returns>
		public virtual iTextSharp.Kernel.Pdf.WriterProperties SetCompressionLevel(int compressionLevel
			)
		{
			this.compressionLevel = compressionLevel;
			return this;
		}

		/// <summary>Defines if full compression mode is enabled.</summary>
		/// <remarks>
		/// Defines if full compression mode is enabled. If enabled, not only the content of the pdf document will be
		/// compressed, but also the pdf document inner structure.
		/// </remarks>
		/// <param name="fullCompressionMode">true - to enable full compression mode, false to disable it
		/// 	</param>
		/// <returns>
		/// this
		/// <c>WriterProperties</c>
		/// instance
		/// </returns>
		public virtual iTextSharp.Kernel.Pdf.WriterProperties SetFullCompressionMode(bool
			 fullCompressionMode)
		{
			this.isFullCompression = fullCompressionMode;
			return this;
		}

		/// <summary>Sets the encryption options for the document.</summary>
		/// <remarks>
		/// Sets the encryption options for the document. The userPassword and the
		/// ownerPassword can be null or have zero length. In this case the ownerPassword
		/// is replaced by a random string. The open permissions for the document can be
		/// AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
		/// AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
		/// The permissions can be combined by ORing them.
		/// See
		/// <see cref="EncryptionConstants"/>
		/// .
		/// </remarks>
		/// <param name="userPassword">the user password. Can be null or empty</param>
		/// <param name="ownerPassword">the owner password. Can be null or empty</param>
		/// <param name="permissions">the user permissions</param>
		/// <param name="encryptionAlgorithm">
		/// the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128,
		/// ENCRYPTION_AES128 or ENCRYPTION_AES256
		/// Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
		/// </param>
		/// <returns>
		/// this
		/// <c>WriterProperties</c>
		/// instance
		/// </returns>
		public virtual iTextSharp.Kernel.Pdf.WriterProperties SetStandardEncryption(byte[]
			 userPassword, byte[] ownerPassword, int permissions, int encryptionAlgorithm)
		{
			encryptionProperties.SetStandardEncryption(userPassword, ownerPassword, permissions
				, encryptionAlgorithm);
			return this;
		}

		/// <summary>Sets the certificate encryption options for the document.</summary>
		/// <remarks>
		/// Sets the certificate encryption options for the document. An array of one or more public certificates
		/// must be provided together with an array of the same size for the permissions for each certificate.
		/// The open permissions for the document can be
		/// AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
		/// AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
		/// The permissions can be combined by ORing them.
		/// Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
		/// See
		/// <see cref="EncryptionConstants"/>
		/// .
		/// </remarks>
		/// <param name="certs">the public certificates to be used for the encryption</param>
		/// <param name="permissions">the user permissions for each of the certificates</param>
		/// <param name="encryptionAlgorithm">
		/// the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128,
		/// ENCRYPTION_AES128 or ENCRYPTION_AES256.
		/// </param>
		/// <returns>
		/// this
		/// <c>WriterProperties</c>
		/// instance
		/// </returns>
		public virtual iTextSharp.Kernel.Pdf.WriterProperties SetPublicKeyEncryption(X509Certificate
			[] certs, int[] permissions, int encryptionAlgorithm)
		{
			encryptionProperties.SetPublicKeyEncryption(certs, permissions, encryptionAlgorithm
				);
			return this;
		}

		/// <summary>This activates debug mode with pdfDebug tool.</summary>
		/// <remarks>
		/// This activates debug mode with pdfDebug tool.
		/// It causes additional overhead of duplicating document bytes into memory, so use it careful.
		/// NEVER use it in production or in any other cases except pdfDebug.
		/// </remarks>
		/// <returns>
		/// this
		/// <c>WriterProperties</c>
		/// instance
		/// </returns>
		public virtual iTextSharp.Kernel.Pdf.WriterProperties UseDebugMode()
		{
			this.debugMode = true;
			return this;
		}

		internal virtual bool IsStandardEncryptionUsed()
		{
			return encryptionProperties.IsStandardEncryptionUsed();
		}

		internal virtual bool IsPublicKeyEncryptionUsed()
		{
			return encryptionProperties.IsPublicKeyEncryptionUsed();
		}
	}
}
