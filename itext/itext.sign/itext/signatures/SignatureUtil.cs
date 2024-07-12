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
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Exceptions;

namespace iText.Signatures {
    /// <summary>Utility class that provides several convenience methods concerning digital signatures.</summary>
    public class SignatureUtil {
        private readonly PdfDocument document;

        private readonly PdfAcroForm acroForm;

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
            this.document = document;
            // Only create new AcroForm if there is a writer
            this.acroForm = PdfFormCreator.GetAcroForm(document, document.GetWriter() != null);
        }

        /// <summary>
        /// Prepares an
        /// <see cref="PdfPKCS7"/>
        /// instance for the given signature.
        /// </summary>
        /// <remarks>
        /// Prepares an
        /// <see cref="PdfPKCS7"/>
        /// instance for the given signature.
        /// This method handles signature parsing and might throw an exception if
        /// signature is malformed.
        /// <para />
        /// The returned
        /// <see cref="PdfPKCS7"/>
        /// can be used to fetch additional info about the signature
        /// and also to perform integrity check of data signed by the given signature field.
        /// <para />
        /// Prepared
        /// <see cref="PdfPKCS7"/>
        /// instance calculates digest based on signature's /ByteRange entry.
        /// In order to check that /ByteRange is properly defined and given signature indeed covers the current PDF document
        /// revision please use
        /// <see cref="SignatureCoversWholeDocument(System.String)"/>
        /// method.
        /// </remarks>
        /// <param name="signatureFieldName">the signature field name</param>
        /// <returns>
        /// a
        /// <see cref="PdfPKCS7"/>
        /// instance which can be used to fetch additional info about the signature
        /// and also to perform integrity check of data signed by the given signature field.
        /// </returns>
        public virtual PdfPKCS7 ReadSignatureData(String signatureFieldName) {
            PdfSignature signature = GetSignature(signatureFieldName);
            if (signature == null) {
                return null;
            }
            try {
                PdfName sub = signature.GetSubFilter();
                PdfString contents = signature.GetContents();
                PdfPKCS7 pk = null;
                if (sub.Equals(PdfName.Adbe_x509_rsa_sha1)) {
                    PdfString cert = signature.GetPdfObject().GetAsString(PdfName.Cert);
                    if (cert == null) {
                        cert = signature.GetPdfObject().GetAsArray(PdfName.Cert).GetAsString(0);
                    }
                    pk = new PdfPKCS7(PdfEncodings.ConvertToBytes(contents.GetValue(), null), cert.GetValueBytes());
                }
                else {
                    pk = new PdfPKCS7(PdfEncodings.ConvertToBytes(contents.GetValue(), null), sub);
                }
                UpdateByteRange(pk, signature);
                PdfString date = signature.GetDate();
                if (date != null) {
                    pk.SetSignDate(PdfDate.Decode(date.ToString()));
                }
                String signName = signature.GetName();
                pk.SetSignName(signName);
                String reason = signature.GetReason();
                if (reason != null) {
                    pk.SetReason(reason);
                }
                String location = signature.GetLocation();
                if (location != null) {
                    pk.SetLocation(location);
                }
                return pk;
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        /// <summary>
        /// Get
        /// <see cref="PdfSignature"/>
        /// dictionary based on the provided name.
        /// </summary>
        /// <param name="name">signature name</param>
        /// <returns>
        /// 
        /// <see cref="PdfSignature"/>
        /// instance corresponding to the provided name.
        /// <see langword="null"/>
        /// otherwise
        /// </returns>
        public virtual PdfSignature GetSignature(String name) {
            PdfDictionary sigDict = GetSignatureDictionary(name);
            return sigDict != null ? new PdfSignature(sigDict) : null;
        }

        /// <summary>Gets the signature dictionary, the one keyed by /V.</summary>
        /// <param name="name">the field name</param>
        /// <returns>
        /// the signature dictionary keyed by /V or <c>null</c> if the field is not
        /// a signature
        /// </returns>
        public virtual PdfDictionary GetSignatureDictionary(String name) {
            PdfDictionary merged = GetSignatureFormFieldDictionary(name);
            return merged == null ? null : merged.GetAsDictionary(PdfName.V);
        }

        public virtual PdfDictionary GetSignatureFormFieldDictionary(String name) {
            GetSignatureNames();
            if (acroForm == null || !sigNames.ContainsKey(name)) {
                return null;
            }
            PdfFormField field = acroForm.GetField(name);
            return field.GetPdfObject();
        }

        /* Updates the /ByteRange with the provided value */
        private void UpdateByteRange(PdfPKCS7 pkcs7, PdfSignature signature) {
            PdfArray b = signature.GetByteRange();
            RandomAccessFileOrArray rf = document.GetReader().GetSafeFile();
            Stream rg = null;
            try {
                rg = new RASInputStream(new RandomAccessSourceFactory().CreateRanged(rf.CreateSourceView(), b.ToLongArray(
                    )));
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
            PopulateSignatureNames();
            return new List<String>(orderedSignatureNames);
        }

        /// <summary>Gets the field names that have blank signatures.</summary>
        /// <returns>List containing the field names that have blank signatures</returns>
        public virtual IList<String> GetBlankSignatureNames() {
            GetSignatureNames();
            IList<String> sigs = new List<String>();
            if (acroForm != null) {
                foreach (KeyValuePair<String, PdfFormField> entry in acroForm.GetAllFormFields()) {
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
            }
            return sigs;
        }

        /// <summary>Get the amount of signed document revisions.</summary>
        /// <returns>
        /// 
        /// <c>int</c>
        /// amount of signed document revisions
        /// </returns>
        public virtual int GetTotalRevisions() {
            GetSignatureNames();
            return totalRevisions;
        }

        /// <summary>Get signed document revision number, which corresponds to the provided signature name.</summary>
        /// <param name="field">signature name</param>
        /// <returns>
        /// 
        /// <c>int</c>
        /// revision number
        /// </returns>
        public virtual int GetRevision(String field) {
            GetSignatureNames();
            field = GetTranslatedFieldName(field);
            if (!sigNames.ContainsKey(field)) {
                return 0;
            }
            return sigNames.Get(field)[1];
        }

        /// <summary>Get field name, translated using XFA, if any present in the document.</summary>
        /// <param name="name">field name to be translated</param>
        /// <returns>translated field name if XFA is present, original name otherwise</returns>
        public virtual String GetTranslatedFieldName(String name) {
            if (acroForm != null && acroForm.GetXfaForm().IsXfaPresent()) {
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
        public virtual Stream ExtractRevision(String field) {
            GetSignatureNames();
            if (!sigNames.ContainsKey(field)) {
                return null;
            }
            int length = sigNames.Get(field)[0];
            RandomAccessFileOrArray raf = document.GetReader().GetSafeFile();
            return new RASInputStream(new WindowRandomAccessSource(raf.CreateSourceView(), 0, length));
        }

        /// <summary>Checks if the signature covers the entire document (except for signature's Contents) or just a part of it.
        ///     </summary>
        /// <remarks>
        /// Checks if the signature covers the entire document (except for signature's Contents) or just a part of it.
        /// <para />
        /// If this method does not return
        /// <see langword="true"/>
        /// it means that signature in question does not cover the entire
        /// contents of current
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . Such signatures cannot be considered as verifying the PDF document,
        /// because content that is not covered by signature might have been modified since the signature creation.
        /// <para />
        /// </remarks>
        /// <param name="name">the signature field name</param>
        /// <returns>true if the signature covers the entire document, false if it doesn't</returns>
        public virtual bool SignatureCoversWholeDocument(String name) {
            GetSignatureNames();
            if (!sigNames.ContainsKey(name)) {
                return false;
            }
            try {
                SignatureUtil.ContentsChecker signatureReader = new SignatureUtil.ContentsChecker(document.GetReader().GetSafeFile
                    ().CreateSourceView(), document);
                return signatureReader.CheckWhetherSignatureCoversWholeDocument(acroForm.GetField(name));
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

        private void PopulateSignatureNames() {
            if (acroForm == null) {
                return;
            }
            IList<Object[]> sorter = new List<Object[]>();
            foreach (KeyValuePair<String, PdfFormField> entry in acroForm.GetAllFormFields()) {
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
                else {
                    contents.MarkAsUnencryptedObject();
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
            if (!sorter.IsEmpty()) {
                if (((int[])sorter[sorter.Count - 1][1])[0] == document.GetReader().GetFileLength()) {
                    totalRevisions = sorter.Count;
                }
                else {
                    totalRevisions = sorter.Count + 1;
                }
                for (int k = 0; k < sorter.Count; ++k) {
                    Object[] objs = sorter[k];
                    String name = (String)objs[0];
                    int[] p = (int[])objs[1];
                    p[1] = k + 1;
                    sigNames.Put(name, p);
                    orderedSignatureNames.Add(name);
                }
            }
        }

        private class SorterComparator : IComparer<Object[]> {
            public virtual int Compare(Object[] o1, Object[] o2) {
                int n1 = ((int[])o1[1])[0];
                int n2 = ((int[])o2[1])[0];
                return n1 - n2;
            }
        }

        private class ContentsChecker : PdfReader {
            public const int OBJECT_HEADER_OFFSET = 6;

            private long rangeExclusionStart;

            private long rangeExlusionEnd;

            private int currentLevel = 0;

            private int contentsLevel = 1;

            private bool searchInV = true;

            private bool rangeIsCorrect = false;

            public ContentsChecker(IRandomAccessSource byteSource, PdfDocument doc)
                : base(byteSource, null) {
                pdfDocument = doc;
            }

            public virtual bool CheckWhetherSignatureCoversWholeDocument(PdfFormField signatureField) {
                rangeIsCorrect = false;
                PdfDictionary signature = (PdfDictionary)signatureField.GetValue();
                int[] byteRange = ((PdfArray)signature.Get(PdfName.ByteRange)).ToIntArray();
                if (4 != byteRange.Length || 0 != byteRange[0] || tokens.GetSafeFile().Length() != byteRange[2] + byteRange
                    [3]) {
                    return false;
                }
                rangeExclusionStart = byteRange[1];
                rangeExlusionEnd = byteRange[2];
                long signatureOffset;
                if (null != signature.GetIndirectReference()) {
                    signatureOffset = signature.GetIndirectReference().GetOffset();
                    searchInV = true;
                }
                else {
                    signatureOffset = signatureField.GetPdfObject().GetIndirectReference().GetOffset();
                    searchInV = false;
                    contentsLevel++;
                }
                try {
                    tokens.Seek(signatureOffset);
                    tokens.NextValidToken();
                    ReadObject(false, false);
                }
                catch (System.IO.IOException) {
                    // That's not expected because if the signature is invalid, it should have already failed
                    return false;
                }
                return rangeIsCorrect;
            }

            protected override PdfDictionary ReadDictionary(bool objStm) {
                // The method copies the logic of PdfReader's method.
                // Only Contents related checks have been introduced.
                currentLevel++;
                PdfDictionary dic = new PdfDictionary();
                int contentsEntryCount = 0;
                while (!rangeIsCorrect) {
                    tokens.NextValidToken();
                    if (tokens.GetTokenType() == PdfTokenizer.TokenType.EndDic) {
                        currentLevel--;
                        break;
                    }
                    if (tokens.GetTokenType() != PdfTokenizer.TokenType.Name) {
                        tokens.ThrowError(SignExceptionMessageConstant.DICTIONARY_THIS_KEY_IS_NOT_A_NAME, tokens.GetStringValue());
                    }
                    PdfName name = ReadPdfName(true);
                    PdfObject obj;
                    if (PdfName.Contents.Equals(name) && searchInV && contentsLevel == currentLevel) {
                        contentsEntryCount++;
                        if (contentsEntryCount > 1) {
                            rangeIsCorrect = false;
                            break;
                        }
                        long contentsValueStart;
                        obj = ReadObject(true, objStm);
                        long contentsValueEnd;
                        if (obj.IsIndirectReference()) {
                            PdfIndirectReference @ref = (PdfIndirectReference)obj;
                            contentsValueStart = @ref.GetOffset() + CountDigits(@ref.GetObjNumber()) + CountDigits(@ref.GetGenNumber()
                                ) + OBJECT_HEADER_OFFSET;
                            contentsValueEnd = contentsValueStart + 
                                                        //*2 + 2 to account for hex encoding
                                                        ((PdfString)@ref.GetRefersTo()).GetValueBytes().Length * 2L + 2L;
                        }
                        else {
                            contentsValueEnd = tokens.GetPosition();
                            //*2 + 2 to account for hex encoding
                            contentsValueStart = contentsValueEnd - (((PdfString)obj).GetValueBytes().Length * 2L + 2L);
                        }
                        if (contentsValueEnd == rangeExlusionEnd && contentsValueStart == rangeExclusionStart) {
                            rangeIsCorrect = true;
                        }
                    }
                    else {
                        if (PdfName.V.Equals(name) && !searchInV && 1 == currentLevel) {
                            searchInV = true;
                            obj = ReadObject(true, objStm);
                            searchInV = false;
                        }
                        else {
                            obj = ReadObject(true, objStm);
                        }
                    }
                    if (obj == null) {
                        if (tokens.GetTokenType() == PdfTokenizer.TokenType.EndDic) {
                            tokens.ThrowError(SignExceptionMessageConstant.UNEXPECTED_GT_GT);
                        }
                        if (tokens.GetTokenType() == PdfTokenizer.TokenType.EndArray) {
                            tokens.ThrowError(SignExceptionMessageConstant.UNEXPECTED_CLOSE_BRACKET);
                        }
                    }
                    dic.Put(name, obj);
                }
                return dic;
            }

            private static long CountDigits(int number) {
                int x = number;
                if (x == 0) {
                    x = 1;
                }
                int l = 0;
                while (x > 0) {
                    x /= 10;
                    l++;
                }
                return l;
            }
        }
    }
}
