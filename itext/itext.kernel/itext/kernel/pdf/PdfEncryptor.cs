/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Text;
using iText.Bouncycastleconnector;
using iText.Commons.Actions.Contexts;
using iText.Commons.Bouncycastle;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// This class takes any PDF and returns exactly the same but
    /// encrypted.
    /// </summary>
    /// <remarks>
    /// This class takes any PDF and returns exactly the same but
    /// encrypted. All the content, links, outlines, etc, are kept.
    /// It is also possible to change the info dictionary.
    /// </remarks>
    public sealed class PdfEncryptor {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private IMetaInfo metaInfo;

        private EncryptionProperties properties;

        public PdfEncryptor() {
        }

        /// <summary>Entry point to encrypt a PDF document.</summary>
        /// <param name="reader">the read PDF</param>
        /// <param name="os">the output destination</param>
        /// <param name="properties">
        /// encryption properties. See
        /// <see cref="EncryptionProperties"/>.
        /// </param>
        /// <param name="newInfo">
        /// an optional
        /// <c>String</c>
        /// map to add or change
        /// the info dictionary. Entries with
        /// <see langword="null"/>
        /// values delete the key in the original info dictionary
        /// </param>
        public static void Encrypt(PdfReader reader, Stream os, EncryptionProperties properties, IDictionary<String
            , String> newInfo) {
            new iText.Kernel.Pdf.PdfEncryptor().SetEncryptionProperties(properties).Encrypt(reader, os, newInfo);
        }

        /// <summary>Entry point to encrypt a PDF document.</summary>
        /// <param name="reader">the read PDF</param>
        /// <param name="os">the output destination</param>
        /// <param name="properties">
        /// encryption properties. See
        /// <see cref="EncryptionProperties"/>.
        /// </param>
        public static void Encrypt(PdfReader reader, Stream os, EncryptionProperties properties) {
            Encrypt(reader, os, properties, null);
        }

        /// <summary>Give you a verbose analysis of the permissions.</summary>
        /// <param name="permissions">the permissions value of a PDF file</param>
        /// <returns>a String that explains the meaning of the permissions value</returns>
        public static String GetPermissionsVerbose(int permissions) {
            StringBuilder buf = new StringBuilder("Allowed:");
            if ((EncryptionConstants.ALLOW_PRINTING & permissions) == EncryptionConstants.ALLOW_PRINTING) {
                buf.Append(" Printing");
            }
            if ((EncryptionConstants.ALLOW_MODIFY_CONTENTS & permissions) == EncryptionConstants.ALLOW_MODIFY_CONTENTS
                ) {
                buf.Append(" Modify contents");
            }
            if ((EncryptionConstants.ALLOW_COPY & permissions) == EncryptionConstants.ALLOW_COPY) {
                buf.Append(" Copy");
            }
            if ((EncryptionConstants.ALLOW_MODIFY_ANNOTATIONS & permissions) == EncryptionConstants.ALLOW_MODIFY_ANNOTATIONS
                ) {
                buf.Append(" Modify annotations");
            }
            if ((EncryptionConstants.ALLOW_FILL_IN & permissions) == EncryptionConstants.ALLOW_FILL_IN) {
                buf.Append(" Fill in");
            }
            if ((EncryptionConstants.ALLOW_SCREENREADERS & permissions) == EncryptionConstants.ALLOW_SCREENREADERS) {
                buf.Append(" Screen readers");
            }
            if ((EncryptionConstants.ALLOW_ASSEMBLY & permissions) == EncryptionConstants.ALLOW_ASSEMBLY) {
                buf.Append(" Assembly");
            }
            if ((EncryptionConstants.ALLOW_DEGRADED_PRINTING & permissions) == EncryptionConstants.ALLOW_DEGRADED_PRINTING
                ) {
                buf.Append(" Degraded printing");
            }
            return buf.ToString();
        }

        /// <summary>Tells you if printing is allowed.</summary>
        /// <param name="permissions">the permissions value of a PDF file</param>
        /// <returns>true if printing is allowed</returns>
        public static bool IsPrintingAllowed(int permissions) {
            return (EncryptionConstants.ALLOW_PRINTING & permissions) == EncryptionConstants.ALLOW_PRINTING;
        }

        /// <summary>Tells you if modifying content is allowed.</summary>
        /// <param name="permissions">the permissions value of a PDF file</param>
        /// <returns>true if modifying content is allowed</returns>
        public static bool IsModifyContentsAllowed(int permissions) {
            return (EncryptionConstants.ALLOW_MODIFY_CONTENTS & permissions) == EncryptionConstants.ALLOW_MODIFY_CONTENTS;
        }

        /// <summary>Tells you if copying is allowed.</summary>
        /// <param name="permissions">the permissions value of a PDF file</param>
        /// <returns>true if copying is allowed</returns>
        public static bool IsCopyAllowed(int permissions) {
            return (EncryptionConstants.ALLOW_COPY & permissions) == EncryptionConstants.ALLOW_COPY;
        }

        /// <summary>Tells you if modifying annotations is allowed.</summary>
        /// <param name="permissions">the permissions value of a PDF file</param>
        /// <returns>true if modifying annotations is allowed</returns>
        public static bool IsModifyAnnotationsAllowed(int permissions) {
            return (EncryptionConstants.ALLOW_MODIFY_ANNOTATIONS & permissions) == EncryptionConstants.ALLOW_MODIFY_ANNOTATIONS;
        }

        /// <summary>Tells you if filling in fields is allowed.</summary>
        /// <param name="permissions">the permissions value of a PDF file</param>
        /// <returns>true if filling in fields is allowed</returns>
        public static bool IsFillInAllowed(int permissions) {
            return (EncryptionConstants.ALLOW_FILL_IN & permissions) == EncryptionConstants.ALLOW_FILL_IN;
        }

        /// <summary>Tells you if repurposing for screenreaders is allowed.</summary>
        /// <param name="permissions">the permissions value of a PDF file</param>
        /// <returns>true if repurposing for screenreaders is allowed</returns>
        public static bool IsScreenReadersAllowed(int permissions) {
            return (EncryptionConstants.ALLOW_SCREENREADERS & permissions) == EncryptionConstants.ALLOW_SCREENREADERS;
        }

        /// <summary>Tells you if document assembly is allowed.</summary>
        /// <param name="permissions">the permissions value of a PDF file</param>
        /// <returns>true if document assembly is allowed</returns>
        public static bool IsAssemblyAllowed(int permissions) {
            return (EncryptionConstants.ALLOW_ASSEMBLY & permissions) == EncryptionConstants.ALLOW_ASSEMBLY;
        }

        /// <summary>Tells you if degraded printing is allowed.</summary>
        /// <param name="permissions">the permissions value of a PDF file</param>
        /// <returns>true if degraded printing is allowed</returns>
        public static bool IsDegradedPrintingAllowed(int permissions) {
            return (EncryptionConstants.ALLOW_DEGRADED_PRINTING & permissions) == EncryptionConstants.ALLOW_DEGRADED_PRINTING;
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Commons.Actions.Contexts.IMetaInfo"/>
        /// that will be used during
        /// <see cref="PdfDocument"/>
        /// creation.
        /// </summary>
        /// <param name="metaInfo">meta info to set</param>
        /// <returns>
        /// this
        /// <see cref="PdfEncryptor"/>
        /// instance
        /// </returns>
        public iText.Kernel.Pdf.PdfEncryptor SetEventCountingMetaInfo(IMetaInfo metaInfo) {
            this.metaInfo = metaInfo;
            return this;
        }

        /// <summary>
        /// Sets the
        /// <see cref="EncryptionProperties"/>
        /// </summary>
        /// <param name="properties">the properties to set</param>
        /// <returns>
        /// this
        /// <see cref="PdfEncryptor"/>
        /// instance
        /// </returns>
        public iText.Kernel.Pdf.PdfEncryptor SetEncryptionProperties(EncryptionProperties properties) {
            this.properties = properties;
            return this;
        }

        /// <summary>Entry point to encrypt a PDF document.</summary>
        /// <param name="reader">the read PDF</param>
        /// <param name="os">the output destination</param>
        /// <param name="newInfo">
        /// an optional
        /// <c>String</c>
        /// map to add or change
        /// the info dictionary. Entries with
        /// <see langword="null"/>
        /// values delete the key in the original info dictionary
        /// </param>
        public void Encrypt(PdfReader reader, Stream os, IDictionary<String, String> newInfo) {
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.encryptionProperties = properties;
            StampingProperties stampingProperties = new StampingProperties();
            stampingProperties.SetEventCountingMetaInfo(metaInfo);
            try {
                using (PdfWriter writer = new PdfWriter(os, writerProperties)) {
                    using (PdfDocument document = new PdfDocument(reader, writer, stampingProperties)) {
                        document.GetDocumentInfo().SetMoreInfo(newInfo);
                    }
                }
            }
            catch (System.IO.IOException) {
            }
        }

        //The close() method of OutputStream throws an exception, but we don't need to do anything in this case,
        // because OutputStream#close() does nothing.
        /// <summary>Entry point to encrypt a PDF document.</summary>
        /// <param name="reader">the read PDF</param>
        /// <param name="os">the output destination</param>
        public void Encrypt(PdfReader reader, Stream os) {
            Encrypt(reader, os, (IDictionary<String, String>)null);
        }
    }
}
