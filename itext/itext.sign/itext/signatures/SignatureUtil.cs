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
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Pdf;

namespace iText.Signatures {
    /// <summary>Utility class that provides several convenience methods concerning digital signatures.</summary>
    public class SignatureUtil {
        private PdfDocument document;

        private PdfAcroForm acroForm;

        private IDictionary<String, int[]> sigNames;

        private IList<String> orderedSignatureNames;

        private int totalRevisions;

        /// <summary>Creates a SignatureUtil instance.</summary>
        /// <remarks>
        /// Creates a SignatureUtil instance. Sets the acroForm field to the acroForm in the PdfDocument.
        /// iText will create a new AcroForm if the PdfDocument doesn't contain one.
        /// </remarks>
        /// <param name="document">PdfDocument to be inspected</param>
        public SignatureUtil(PdfDocument document) {
            // TODO: REFACTOR. At this moment this serves as storage for some signature-related methods from iText 5 AcroFields
            this.document = document;
            this.acroForm = PdfAcroForm.GetAcroForm(document, true);
        }

        /// <summary>Verifies a signature.</summary>
        /// <remarks>
        /// Verifies a signature. Further verification can be done on the returned
        /// <see cref="PdfPKCS7"/>
        /// object.
        /// </remarks>
        /// <param name="name">the signature field name</param>
        /// <param name="provider">the provider or null for the default provider</param>
        /// <returns>PdfPKCS7 object to continue the verification</returns>
        public virtual PdfPKCS7 VerifySignature(String name) {
            PdfDictionary v = GetSignatureDictionary(name);
            if (v == null) {
                return null;
            }
            try {
                PdfName sub = v.GetAsName(PdfName.SubFilter);
                PdfString contents = v.GetAsString(PdfName.Contents);
                PdfPKCS7 pk = null;
                if (sub.Equals(PdfName.Adbe_x509_rsa_sha1)) {
                    PdfString cert = v.GetAsString(PdfName.Cert);
                    if (cert == null) {
                        cert = v.GetAsArray(PdfName.Cert).GetAsString(0);
                    }
                    pk = new PdfPKCS7(PdfEncodings.ConvertToBytes(contents.GetValue(), null), cert.GetValueBytes());
                }
                else {
                    pk = new PdfPKCS7(PdfEncodings.ConvertToBytes(contents.GetValue(), null), sub);
                }
                UpdateByteRange(pk, v);
                PdfString str = v.GetAsString(PdfName.M);
                if (str != null) {
                    pk.SetSignDate(PdfDate.Decode(str.ToString()));
                }
                PdfObject obj = v.Get(PdfName.Name);
                if (obj != null) {
                    if (obj.IsString()) {
                        pk.SetSignName(((PdfString)obj).ToUnicodeString());
                    }
                    else {
                        if (obj.IsName()) {
                            pk.SetSignName(((PdfName)obj).GetValue());
                        }
                    }
                }
                str = v.GetAsString(PdfName.Reason);
                if (str != null) {
                    pk.SetReason(str.ToUnicodeString());
                }
                str = v.GetAsString(PdfName.Location);
                if (str != null) {
                    pk.SetLocation(str.ToUnicodeString());
                }
                return pk;
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        /// <summary>Gets the signature dictionary, the one keyed by /V.</summary>
        /// <param name="name">the field name</param>
        /// <returns>
        /// the signature dictionary keyed by /V or <CODE>null</CODE> if the field is not
        /// a signature
        /// </returns>
        public virtual PdfDictionary GetSignatureDictionary(String name) {
            GetSignatureNames();
            if (!sigNames.ContainsKey(name)) {
                return null;
            }
            PdfFormField field = acroForm.GetField(name);
            PdfDictionary merged = field.GetPdfObject();
            return merged.GetAsDictionary(PdfName.V);
        }

        /* Updates the /ByteRange with the provided value */
        private void UpdateByteRange(PdfPKCS7 pkcs7, PdfDictionary v) {
            PdfArray b = v.GetAsArray(PdfName.ByteRange);
            RandomAccessFileOrArray rf = document.GetReader().GetSafeFile();
            Stream rg = null;
            try {
                rg = new RASInputStream(new RandomAccessSourceFactory().CreateRanged(rf.CreateSourceView(), AsLongArray(b)
                    ));
                byte[] buf = new byte[8192];
                int rd;
                while ((rd = rg.JRead(buf, 0, buf.Length)) > 0) {
                    pkcs7.Update(buf, 0, rd);
                }
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
            finally {
                try {
                    if (rg != null) {
                        rg.Dispose();
                    }
                }
                catch (System.IO.IOException e) {
                    // this really shouldn't ever happen - the source view we use is based on a Safe view, which is a no-op anyway
                    throw new PdfException(e);
                }
            }
        }

        /// <summary>Gets the field names that have signatures and are signed.</summary>
        /// <returns>List containing the field names that have signatures and are signed</returns>
        public virtual IList<String> GetSignatureNames() {
            if (sigNames != null) {
                return new List<String>(orderedSignatureNames);
            }
            sigNames = new Dictionary<String, int[]>();
            orderedSignatureNames = new List<String>();
            IList<Object[]> sorter = new List<Object[]>();
            foreach (KeyValuePair<String, PdfFormField> entry in acroForm.GetFormFields()) {
                PdfFormField field = entry.Value;
                PdfDictionary merged = field.GetPdfObject();
                if (!PdfName.Sig.Equals(merged.Get(PdfName.FT))) {
                    continue;
                }
                PdfDictionary v = merged.GetAsDictionary(PdfName.V);
                if (v == null) {
                    continue;
                }
                PdfString contents = v.GetAsString(PdfName.Contents);
                if (contents == null) {
                    continue;
                }
                PdfArray ro = v.GetAsArray(PdfName.ByteRange);
                if (ro == null) {
                    continue;
                }
                int rangeSize = ro.Size();
                if (rangeSize < 2) {
                    continue;
                }
                int length = ro.GetAsNumber(rangeSize - 1).IntValue() + ro.GetAsNumber(rangeSize - 2).IntValue();
                sorter.Add(new Object[] { entry.Key, new int[] { length, 0 } });
            }
            JavaCollectionsUtil.Sort(sorter, new SignatureUtil.SorterComparator());
            if (sorter.Count > 0) {
                try {
                    if (((int[])sorter[sorter.Count - 1][1])[0] == document.GetReader().GetFileLength()) {
                        totalRevisions = sorter.Count;
                    }
                    else {
                        totalRevisions = sorter.Count + 1;
                    }
                }
                catch (System.IO.IOException) {
                }
                // TODO: add exception handling (at least some logger)
                for (int k = 0; k < sorter.Count; ++k) {
                    Object[] objs = sorter[k];
                    String name = (String)objs[0];
                    int[] p = (int[])objs[1];
                    p[1] = k + 1;
                    sigNames[name] = p;
                    orderedSignatureNames.Add(name);
                }
            }
            return new List<String>(orderedSignatureNames);
        }

        /// <summary>Gets the field names that have blank signatures.</summary>
        /// <returns>List containing the field names that have blank signatures</returns>
        public virtual IList<String> GetBlankSignatureNames() {
            GetSignatureNames();
            IList<String> sigs = new List<String>();
            foreach (KeyValuePair<String, PdfFormField> entry in acroForm.GetFormFields()) {
                PdfFormField field = entry.Value;
                PdfDictionary merged = field.GetPdfObject();
                if (!PdfName.Sig.Equals(merged.GetAsName(PdfName.FT))) {
                    continue;
                }
                if (sigNames.ContainsKey(entry.Key)) {
                    continue;
                }
                sigs.Add(entry.Key);
            }
            return sigs;
        }

        public virtual int GetTotalRevisions() {
            GetSignatureNames();
            return totalRevisions;
        }

        public virtual int GetRevision(String field) {
            GetSignatureNames();
            field = GetTranslatedFieldName(field);
            if (!sigNames.ContainsKey(field)) {
                return 0;
            }
            return sigNames.Get(field)[1];
        }

        public virtual String GetTranslatedFieldName(String name) {
            if (acroForm.GetXfaForm().IsXfaPresent()) {
                String namex = acroForm.GetXfaForm().FindFieldName(name);
                if (namex != null) {
                    name = namex;
                }
            }
            return name;
        }

        /// <summary>Extracts a revision from the document.</summary>
        /// <param name="field">the signature field name</param>
        /// <returns>an InputStream covering the revision. Returns null if it's not a signature field</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual Stream ExtractRevision(String field) {
            GetSignatureNames();
            if (!sigNames.ContainsKey(field)) {
                return null;
            }
            int length = sigNames.Get(field)[0];
            RandomAccessFileOrArray raf = document.GetReader().GetSafeFile();
            return new RASInputStream(new WindowRandomAccessSource(raf.CreateSourceView(), 0, length));
        }

        /// <summary>Checks if the signature covers the entire document or just part of it.</summary>
        /// <param name="name">the signature field name</param>
        /// <returns>true if the signature covers the entire document, false if it doesn't</returns>
        public virtual bool SignatureCoversWholeDocument(String name) {
            GetSignatureNames();
            if (!sigNames.ContainsKey(name)) {
                return false;
            }
            try {
                return sigNames.Get(name)[0] == document.GetReader().GetFileLength();
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
        }

        /// <summary>Checks whether a name exists as a signature field or not.</summary>
        /// <remarks>Checks whether a name exists as a signature field or not. It checks both signed fields and blank signatures.
        ///     </remarks>
        /// <param name="name">name of the field</param>
        /// <returns>boolean does the signature field exist</returns>
        public virtual bool DoesSignatureFieldExist(String name) {
            return GetBlankSignatureNames().Contains(name) || GetSignatureNames().Contains(name);
        }

        /// <summary>
        /// Converts a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// to an array of longs
        /// </summary>
        /// <param name="pdfArray">PdfArray to be converted</param>
        /// <returns>long[] containing the PdfArray values</returns>
        public static long[] AsLongArray(PdfArray pdfArray) {
            // TODO: copied from iText 5 PdfArray.asLongArray
            long[] rslt = new long[pdfArray.Size()];
            for (int k = 0; k < rslt.Length; ++k) {
                rslt[k] = pdfArray.GetAsNumber(k).LongValue();
            }
            return rslt;
        }

        private class SorterComparator : IComparer<Object[]> {
            public virtual int Compare(Object[] o1, Object[] o2) {
                int n1 = ((int[])o1[1])[0];
                int n2 = ((int[])o2[1])[0];
                return n1 - n2;
            }
        }
    }
}
