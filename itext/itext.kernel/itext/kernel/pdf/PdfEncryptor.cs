/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
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
