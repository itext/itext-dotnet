/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Linq;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Utilities;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Test;

namespace iText.Signatures.Testutils {
    public class SignaturesCompareTool {
        private const String OID_MESSAGE_DIGEST = "1.2.840.113549.1.9.4";

        private const String OID_SIGNED_DATA = "1.2.840.113549.1.7.2";

        private const String OID_TST_INFO = "1.2.840.113549.1.9.16.1.4";

        private const String OID_SIGNING_TIME = "1.2.840.113549.1.9.5";

        private const String OID_SIGNATURE_TIMESTAMP_ATTRIBUTE = "1.2.840.113549.1.9.16.2.14";

        private const String OID_ADBE_REVOCATION_INFO_ARCHIVAL = "1.2.840.113583.1.1.8";

        private const String OID_OCSP_RESPONSE = "1.3.6.1.5.5.7.48.1.1";

        private const String OID_OCSP_NONCE_EXTENSION = "1.3.6.1.5.5.7.48.1.2";

        private static readonly ICollection<String> IGNORED_OIDS;

        static SignaturesCompareTool() {
            HashSet<String> tempSet = new HashSet<String>();
            tempSet.Add(OID_MESSAGE_DIGEST);
            tempSet.Add(OID_TST_INFO);
            tempSet.Add(OID_SIGNING_TIME);
            tempSet.Add(OID_OCSP_NONCE_EXTENSION);
            IGNORED_OIDS = JavaCollectionsUtil.UnmodifiableSet(tempSet);
        }

        public static String CompareSignatures(String dest, String cmp) {
            return CompareSignatures(dest, cmp, new ReaderProperties(), new ReaderProperties());
        }

        public static String CompareSignatures(String dest, String cmp, ReaderProperties destProperties, ReaderProperties
             cmpProperties) {
            ITextTest.PrintOutCmpPdfNameAndDir(dest, cmp);
            StringBuilder errorText = new StringBuilder();
            try {
                using (PdfDocument outDocument = new PdfDocument(new PdfReader(dest, destProperties))) {
                    using (PdfDocument cmpDocument = new PdfDocument(new PdfReader(cmp, cmpProperties))) {
                        SignatureUtil outSigUtil = new SignatureUtil(outDocument);
                        SignatureUtil cmpSigUtil = new SignatureUtil(cmpDocument);
                        if (!Enumerable.SequenceEqual(cmpSigUtil.GetSignatureNames(), outSigUtil.GetSignatureNames())) {
                            AddError(errorText, "Signatures lists are different:", outSigUtil.GetSignatureNames().ToString(), cmpSigUtil
                                .GetSignatureNames().ToString());
                        }
                        IList<String> signatures = cmpSigUtil.GetSignatureNames();
                        foreach (String sig in signatures) {
                            Asn1Sequence outSignedData = (Asn1Sequence)GetSignatureContent(sig, outSigUtil);
                            Asn1Sequence cmpSignedData = (Asn1Sequence)GetSignatureContent(sig, cmpSigUtil);
                            bool isEqual = CompareSignedData(outSignedData, cmpSignedData, errorText);
                            if (!isEqual) {
                                CreateTxtFilesFromAsn1Sequences(outSignedData, cmpSignedData, dest, sig, errorText);
                            }
                        }
                        CompareDssEntries(outDocument, cmpDocument, dest, errorText);
                    }
                }
            }
            catch (Exception e) {
                errorText.Append(e.Message);
            }
            return String.IsNullOrEmpty(errorText.ToString()) ? null : errorText.ToString();
        }

        private static void CreateTxtFilesFromAsn1Sequences(Asn1Sequence outSignedData, Asn1Sequence cmpSignedData
            , String dest, String sig, StringBuilder errorText) {
            String sigFileName = dest.JSubstring(0, dest.LastIndexOf("."));
            String outSigFile = sigFileName + "_" + sig + "_out.txt";
            String cmpSigFile = sigFileName + "_" + sig + "_cmp.txt";
            WriteToFile(outSigFile, sig + "\n" + Asn1Dump.DumpAsString(outSignedData, true) + "\n");
            WriteToFile(cmpSigFile, sig + "\n" + Asn1Dump.DumpAsString(cmpSignedData, true) + "\n");
            errorText.Insert(0, "See signature output files: " + "\nout: " + UrlUtil.GetNormalizedFileUriString(outSigFile
                ) + "\ncmp: " + UrlUtil.GetNormalizedFileUriString(cmpSigFile) + "\n");
        }

        private static bool CompareDssEntries(PdfDocument outDocument, PdfDocument cmpDocument, String dest, StringBuilder
             errorText) {
            PdfDictionary outDss = outDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            PdfDictionary cmpDss = cmpDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            if (outDss == null || cmpDss == null) {
                if (outDss == cmpDss) {
                    return true;
                }
                AddError(errorText, "DSS dictionaries are different");
                return false;
            }
            bool ocspCertificatesEqual = CompareRevocationDataFromDss(outDss, cmpDss, PdfName.OCSPs, dest, errorText, 
                (outSequence, cmpSequence, errorStringBuilder) => SignaturesCompareTool.CompareAsn1Structures(outSequence
                , cmpSequence, errorStringBuilder));
            bool crlCertificatesEqual = CompareRevocationDataFromDss(outDss, cmpDss, PdfName.CRLs, dest, errorText, (outSequence
                , cmpSequence, errorStringBuilder) => SignaturesCompareTool.CompareSequencesWithSignatureValue(outSequence
                , cmpSequence, errorStringBuilder));
            return ocspCertificatesEqual && crlCertificatesEqual;
        }

        private static bool CompareRevocationDataFromDss(PdfDictionary outDss, PdfDictionary cmpDss, PdfName entryName
            , String dest, StringBuilder errorText, SignaturesCompareTool.SequenceComparator comparator) {
            String errorMessage = entryName.GetValue() + " entries inside DSS dictionaries are different";
            PdfArray outDssEntry = outDss.GetAsArray(entryName);
            PdfArray cmpDssEntry = cmpDss.GetAsArray(entryName);
            if (outDssEntry == null || cmpDssEntry == null) {
                if (outDssEntry == cmpDssEntry) {
                    return true;
                }
                AddError(errorText, errorMessage);
                return false;
            }
            if (outDssEntry.Size() != cmpDssEntry.Size()) {
                AddError(errorText, errorMessage);
                return false;
            }
            for (int i = 0; i < outDssEntry.Size(); ++i) {
                PdfStream outDssEntryItem = outDssEntry.GetAsStream(i);
                PdfStream cmpDssEntryItem = cmpDssEntry.GetAsStream(i);
                if (outDssEntryItem == null || cmpDssEntryItem == null) {
                    if (outDssEntryItem == cmpDssEntryItem) {
                        continue;
                    }
                    AddError(errorText, errorMessage);
                    return false;
                }
                Asn1Sequence outDecodedItem = (Asn1Sequence)Asn1Sequence.FromByteArray(outDssEntryItem.GetBytes());
                Asn1Sequence cmpDecodedItem = (Asn1Sequence)Asn1Sequence.FromByteArray(cmpDssEntryItem.GetBytes());
                if (!comparator(outDecodedItem, cmpDecodedItem, errorText)) {
                    CreateTxtFilesFromAsn1Sequences(outDecodedItem, cmpDecodedItem, dest, "DSS_" + entryName.GetValue() + "_" 
                        + i, errorText);
                    return false;
                }
            }
            return true;
        }

        private static bool CompareOcspResponses(Asn1Encodable[] outOcspResponse, Asn1Encodable[] cmpOcspResponse, 
            StringBuilder errorText) {
            if (outOcspResponse.Length != 2 || cmpOcspResponse.Length != 2) {
                AddError(errorText, "OCSP response has unexpected structure");
            }
            Asn1OctetString outResponseString = (Asn1OctetString)outOcspResponse[1];
            Asn1OctetString cmpResponseString = (Asn1OctetString)cmpOcspResponse[1];
            if (outResponseString.Equals(cmpResponseString)) {
                return true;
            }
            Asn1Sequence parsedOutResponse = (Asn1Sequence)Asn1Sequence.FromByteArray(outResponseString.GetOctets());
            Asn1Sequence parsedCmpResponse = (Asn1Sequence)Asn1Sequence.FromByteArray(cmpResponseString.GetOctets());
            return CompareSequencesWithSignatureValue(parsedOutResponse, parsedCmpResponse, errorText);
        }

        /// <summary>SignedData is top-level CMS-object for signatures, see "5.1.</summary>
        /// <remarks>
        /// SignedData is top-level CMS-object for signatures, see "5.1. SignedData Type" at
        /// https://datatracker.ietf.org/doc/html/rfc5652#section-5.1 .
        /// </remarks>
        /// <param name="outSignedData">current output signed data</param>
        /// <param name="cmpSignedData">reference signed data used for comparison as a ground truth</param>
        /// <param name="errorText">string builder in order to accumulate errors</param>
        /// <returns>true if signed data objects are the similar, false otherwise</returns>
        private static bool CompareSignedData(Asn1Sequence outSignedData, Asn1Sequence cmpSignedData, StringBuilder
             errorText) {
            if (outSignedData.Count != cmpSignedData.Count || outSignedData.Count != 2) {
                AddError(errorText, "Signature top level elements count is incorrect (should be exactly 2):", outSignedData
                    .Count.ToString(), cmpSignedData.Count.ToString());
                return false;
            }
            DerObjectIdentifier outObjId = (DerObjectIdentifier)outSignedData[0];
            DerObjectIdentifier cmpObjId = (DerObjectIdentifier)cmpSignedData[0];
            if (!outObjId.Equals(cmpObjId) || !outObjId.Id.Equals(OID_SIGNED_DATA)) {
                AddError(errorText, "Signatures object identifier is incorrect (should be " + OID_SIGNED_DATA + ")", outObjId
                    .Id.ToString(), cmpObjId.Id.ToString());
                return false;
            }
            Asn1Sequence outContent = (Asn1Sequence)((Asn1TaggedObject)outSignedData[1]).GetObject();
            Asn1Sequence cmpContent = (Asn1Sequence)((Asn1TaggedObject)cmpSignedData[1]).GetObject();
            if (outContent.Count != cmpContent.Count) {
                AddError(errorText, "Signatures base elements counts are different", outContent.Count.ToString(), cmpContent
                    .Count.ToString());
                return false;
            }
            int signerInfoIndex = GetSignerInfoIndex(cmpContent);
            if (!(outContent[signerInfoIndex] is Asn1Set)) {
                AddError(errorText, "SignerInfo object indexes are different", null, null);
                return false;
            }
            for (int i = 0; i < cmpContent.Count; i++) {
                // SignerInfo objects will be compared separately
                if (i == signerInfoIndex) {
                    continue;
                }
                // Sequences and sets related to timestamp token info should be ignored.
                if (OID_TST_INFO.Equals(GetASN1ObjectId(cmpContent[i].ToAsn1Object())) && OID_TST_INFO.Equals(GetASN1ObjectId
                    (outContent[i].ToAsn1Object()))) {
                    continue;
                }
                if (!cmpContent[i].Equals(outContent[i])) {
                    AddError(errorText, "SignedData objects are different", null, null);
                    return false;
                }
            }
            Asn1Set cmpSignerInfos = (Asn1Set)cmpContent[signerInfoIndex];
            Asn1Set outSignerInfos = (Asn1Set)outContent[signerInfoIndex];
            // Currently, iText signature validation mechanism do not support signatures,
            // containing more than one SignerInfo entry. However, it is still valid signature.
            if (cmpSignerInfos.Count != outSignerInfos.Count || cmpSignerInfos.Count != 1) {
                AddError(errorText, "Incorrect SignerInfos objects count", outSignerInfos.Count.ToString(), cmpSignerInfos
                    .Count.ToString());
                return false;
            }
            Asn1Sequence outSignerInfo = (Asn1Sequence)outSignerInfos[0];
            Asn1Sequence cmpSignerInfo = (Asn1Sequence)cmpSignerInfos[0];
            return CompareSequencesWithSignatureValue(outSignerInfo, cmpSignerInfo, errorText);
        }

        private static bool CompareSequencesWithSignatureValue(Asn1Sequence outSequence, Asn1Sequence cmpSequence, 
            StringBuilder errorText) {
            if (cmpSequence.Count != outSequence.Count) {
                AddError(errorText, "Incorrect SignerInfo entries count", outSequence.Count.ToString(), cmpSequence.Count.
                    ToString());
                return false;
            }
            for (int i = 0; i < cmpSequence.Count; i++) {
                // Skipping comparison of encoded strings fields which are SignatureValue fields.
                // They are expected to be different.
                if (outSequence[i] is Asn1OctetString || outSequence[i] is DerBitString) {
                    if (outSequence[i].GetType().Equals(cmpSequence[i].GetType())) {
                        continue;
                    }
                    else {
                        AddError(errorText, "Signature values indexes are different!", null, null);
                        return false;
                    }
                }
                if (!CompareAsn1Structures(outSequence[i].ToAsn1Object(), cmpSequence[i].ToAsn1Object(), errorText)) {
                    return false;
                }
            }
            return true;
        }

        private static bool CompareAsn1Structures(Asn1Object @out, Asn1Object cmp, StringBuilder errorText) {
            if (!@out.GetType().Equals(cmp.GetType())) {
                AddError(errorText, "ASN1 objects types are different", @out.GetType().FullName, cmp.GetType().FullName);
                return false;
            }
            if (cmp is Asn1TaggedObject) {
                return CompareAsn1Structures(((Asn1TaggedObject)@out).GetObject(), ((Asn1TaggedObject)cmp).GetObject(), errorText
                    );
            }
            else {
                if (cmp is Asn1Sequence) {
                    if (!CompareContainers(((Asn1Sequence)@out).ToArray(), ((Asn1Sequence)cmp).ToArray(), errorText)) {
                        AddError(errorText, "ASN1Sequence objects are different");
                        return false;
                    }
                }
                else {
                    if (cmp is Asn1Set) {
                        if (!CompareContainers(((Asn1Set)@out).ToArray(), ((Asn1Set)cmp).ToArray(), errorText)) {
                            AddError(errorText, "ASN1Set objects are different");
                            return false;
                        }
                    }
                    else {
                        if (cmp is DerGeneralizedTime || cmp is DerUtcTime) {
                            // Ignore time values since usually they shouldn't be equal
                            return true;
                        }
                        else {
                            if (!cmp.Equals(@out)) {
                                AddError(errorText, "ASN1 objects are different", Asn1Dump.DumpAsString(@out, true), Asn1Dump.DumpAsString
                                    (cmp, true));
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static bool CompareContainers(Asn1Encodable[] outArray, Asn1Encodable[] cmpArray, StringBuilder errorText
            ) {
            if (cmpArray.Length != outArray.Length) {
                AddError(errorText, "Container lengths are different", JavaUtil.IntegerToString(outArray.Length), JavaUtil.IntegerToString
                    (cmpArray.Length));
                return false;
            }
            String cmpASN1ObjectId = GetASN1ObjectId(cmpArray);
            String outASN1ObjectId = GetASN1ObjectId(outArray);
            if (!Object.Equals(cmpASN1ObjectId, outASN1ObjectId)) {
                AddError(errorText, "Containers ids are different", outASN1ObjectId, cmpASN1ObjectId);
                return false;
            }
            if (IGNORED_OIDS.Contains(cmpASN1ObjectId)) {
                return true;
            }
            if (OID_SIGNATURE_TIMESTAMP_ATTRIBUTE.Equals(cmpASN1ObjectId)) {
                return CompareTimestampAttributes(outArray, cmpArray, errorText);
            }
            if (OID_OCSP_RESPONSE.Equals(cmpASN1ObjectId)) {
                return CompareOcspResponses(outArray, cmpArray, errorText);
            }
            if (OID_ADBE_REVOCATION_INFO_ARCHIVAL.Equals(cmpASN1ObjectId)) {
                return CompareRevocationInfoArchivalAttribute(outArray, cmpArray, errorText);
            }
            for (int i = 0; i < cmpArray.Length; i++) {
                if (!CompareAsn1Structures(outArray[i].ToAsn1Object(), cmpArray[i].ToAsn1Object(), errorText)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>See ISO 32000-2, 12.8.3.3.2 "Revocation of CMS-based signatures"</summary>
        /// <param name="out">out signature revocation info attribute value</param>
        /// <param name="cmp">cmp signature revocation info attribute value</param>
        /// <param name="errorText">string builder in order to accumulate errors</param>
        /// <returns>true if signed data objects are the similar, false otherwise</returns>
        private static bool CompareRevocationInfoArchivalAttribute(Asn1Encodable[] @out, Asn1Encodable[] cmp, StringBuilder
             errorText) {
            String structureIsInvalidError = "Signature revocation info archival attribute structure is invalid";
            if (!IsExpectedRevocationInfoArchivalAttributeStructure(@out) || !IsExpectedRevocationInfoArchivalAttributeStructure
                (cmp)) {
                AddError(errorText, structureIsInvalidError, String.Join("", JavaUtil.ArraysToEnumerable(@out).Select((e) =>
                     Asn1Dump.DumpAsString(e)).ToList()), String.Join("", JavaUtil.ArraysToEnumerable(cmp).Select((e) => Asn1Dump
                    .DumpAsString(e)).ToList()));
                return false;
            }
            Asn1Sequence outSequence = ((Asn1Sequence)((Asn1Set)@out[1])[0].ToAsn1Object());
            Asn1Sequence cmpSequence = ((Asn1Sequence)((Asn1Set)cmp[1])[0].ToAsn1Object());
            if (outSequence.Count != cmpSequence.Count) {
                AddError(errorText, "Signature revocation info archival attributes have different sets of revocation info types (different sizes)"
                    , outSequence.Count.ToString(), cmpSequence.Count.ToString());
                return false;
            }
            for (int i = 0; i < outSequence.Count; i++) {
                if (!(outSequence[i] is Asn1TaggedObject) || !(cmpSequence[i] is Asn1TaggedObject)) {
                    AddError(errorText, structureIsInvalidError, String.Join("", JavaUtil.ArraysToEnumerable(@out).Select((e) =>
                         Asn1Dump.DumpAsString(e)).ToList()), String.Join("", JavaUtil.ArraysToEnumerable(cmp).Select((e) => Asn1Dump
                        .DumpAsString(e)).ToList()));
                    return false;
                }
                Asn1TaggedObject outTaggedObject = (Asn1TaggedObject)outSequence[i];
                Asn1TaggedObject cmpTaggedObject = (Asn1TaggedObject)cmpSequence[i];
                if (outTaggedObject.TagNo != cmpTaggedObject.TagNo) {
                    AddError(errorText, "Signature revocation info archival attributes have different tagged objects tag numbers"
                        , outTaggedObject.TagNo.ToString(), cmpTaggedObject.TagNo.ToString());
                    return false;
                }
                if (!(outTaggedObject.GetObject() is Asn1Sequence) || !(cmpTaggedObject.GetObject() is Asn1Sequence)) {
                    AddError(errorText, structureIsInvalidError, String.Join("", JavaUtil.ArraysToEnumerable(@out).Select((e) =>
                         Asn1Dump.DumpAsString(e)).ToList()), String.Join("", JavaUtil.ArraysToEnumerable(cmp).Select((e) => Asn1Dump
                        .DumpAsString(e)).ToList()));
                    return false;
                }
                // revocation entries can be either CRLs or OCSPs in most cases
                Asn1Sequence outRevocationEntries = (Asn1Sequence)outTaggedObject.GetObject();
                Asn1Sequence cmpRevocationEntries = (Asn1Sequence)cmpTaggedObject.GetObject();
                if (outRevocationEntries.Count != cmpRevocationEntries.Count) {
                    AddError(errorText, "Signature revocation info archival attributes have different number of entries", outRevocationEntries
                        .Count.ToString(), cmpRevocationEntries.Count.ToString());
                    return false;
                }
                if (outTaggedObject.TagNo == 0) {
                    // CRL revocation info case
                    for (int j = 0; j < outRevocationEntries.Count; j++) {
                        if (!(outRevocationEntries[j] is Asn1Sequence) || !(outRevocationEntries[j] is Asn1Sequence)) {
                            AddError(errorText, "Signature revocation info attribute has unexpected CRL entry type", outRevocationEntries
                                [j].GetType().FullName.ToString(), cmpRevocationEntries[j].GetType().FullName.ToString());
                            return false;
                        }
                        if (!CompareSequencesWithSignatureValue(((Asn1Sequence)outRevocationEntries[j]), ((Asn1Sequence)cmpRevocationEntries
                            [j]), errorText)) {
                            AddError(errorText, MessageFormatUtil.Format("Signature revocation info attribute CRLs at {0} are different"
                                , j.ToString()));
                            return false;
                        }
                    }
                }
                else {
                    if (!CompareAsn1Structures(outRevocationEntries, cmpRevocationEntries, errorText)) {
                        AddError(errorText, "Revocation info attribute entries are different");
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsExpectedRevocationInfoArchivalAttributeStructure(Asn1Encodable[] container) {
            return container.Length == 2 && container[1] is Asn1Set && ((Asn1Set)container[1]).Count == 1 && ((Asn1Set
                )container[1])[0].ToAsn1Object() is Asn1Sequence;
        }

        private static bool CompareTimestampAttributes(Asn1Encodable[] @out, Asn1Encodable[] cmp, StringBuilder errorText
            ) {
            if (cmp.Length == 2) {
                if (cmp[1] is Asn1Set && @out[1] is Asn1Set) {
                    Asn1Object outSequence = ((Asn1Set)@out[1])[0].ToAsn1Object();
                    Asn1Object cmpSequence = ((Asn1Set)cmp[1])[0].ToAsn1Object();
                    if (outSequence is Asn1Sequence && cmpSequence is Asn1Sequence) {
                        return CompareSignedData((Asn1Sequence)outSequence, (Asn1Sequence)cmpSequence, errorText);
                    }
                }
            }
            AddError(errorText, "Signature timestamp attribute structure is invalid", String.Join("", JavaUtil.ArraysToEnumerable
                (@out).Select((e) => Asn1Dump.DumpAsString(e)).ToList()), String.Join("", JavaUtil.ArraysToEnumerable(
                cmp).Select((e) => Asn1Dump.DumpAsString(e)).ToList()));
            return false;
        }

        private static int GetSignerInfoIndex(Asn1Sequence baseElement) {
            for (int i = 3; i < baseElement.Count; i++) {
                if (baseElement[i] is Asn1Set) {
                    return i;
                }
            }
            throw new InvalidOperationException("SignerInfo entry has not been found.");
        }

        private static String GetASN1ObjectId(Asn1Object primitive) {
            if (primitive is Asn1Sequence) {
                return GetASN1ObjectId(((Asn1Sequence)primitive).ToArray());
            }
            if (primitive is Asn1Set) {
                return GetASN1ObjectId(((Asn1Set)primitive).ToArray());
            }
            return null;
        }

        private static String GetASN1ObjectId(Asn1Encodable[] primitives) {
            if (primitives.Length != 0 && primitives[0] is DerObjectIdentifier) {
                return ((DerObjectIdentifier)primitives[0]).Id;
            }
            return null;
        }

        private static Asn1Object GetSignatureContent(String signatureName, SignatureUtil util) {
            PdfSignature signature = util.GetSignature(signatureName);
            byte[] contents = signature.GetContents().GetValueBytes();
            Asn1InputStream inputStream = new Asn1InputStream(new MemoryStream(contents));
            return inputStream.ReadObject();
        }

        private static void WriteToFile(String path, String content) {
            using (StreamWriter writer = new StreamWriter(path, true)) {
                writer.Write(content);
            }
        }

        private static void AddError(StringBuilder errorBuilder, String errorText) {
            AddError(errorBuilder, errorText, null, null);
        }

        private static void AddError(StringBuilder errorBuilder, String errorText, String @out, String cmp) {
            errorBuilder.Append(errorText);
            if (null != @out) {
                errorBuilder.Append("\nout: ").Append(@out);
            }
            if (null != cmp) {
                errorBuilder.Append("\ncmp: ").Append(cmp);
            }
            errorBuilder.Append("\n\n");
        }

        internal delegate bool SequenceComparator(Asn1Sequence outSequence, Asn1Sequence cmpSequence, StringBuilder
             errorText);
    }
}
