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
using System.Linq;
using System.Text;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Util;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Test;

namespace iText.Signatures.Testutils {
    public class SignaturesCompareTool {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private const String OID_MESSAGE_DIGEST = "1.2.840.113549.1.9.4";

        private const String OID_SIGNED_DATA = "1.2.840.113549.1.7.2";

        private const String OID_TST_INFO = "1.2.840.113549.1.9.16.1.4";

        private const String OID_SIGNING_TIME = "1.2.840.113549.1.9.5";

        private const String OID_SIGNATURE_TIMESTAMP_ATTRIBUTE = "1.2.840.113549.1.9.16.2.14";

        private const String OID_ADBE_REVOCATION_INFO_ARCHIVAL = "1.2.840.113583.1.1.8";

        private const String OID_OCSP_RESPONSE = "1.3.6.1.5.5.7.48.1.1";

        private const String OID_OCSP_NONCE_EXTENSION = "1.3.6.1.5.5.7.48.1.2";

        private static readonly IAsn1Dump DUMP = BOUNCY_CASTLE_FACTORY.CreateASN1Dump();

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
                            IAsn1Sequence outSignedData = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(GetSignatureContent(sig, outSigUtil
                                ));
                            IAsn1Sequence cmpSignedData = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(GetSignatureContent(sig, cmpSigUtil
                                ));
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

        private static void CreateTxtFilesFromAsn1Sequences(IAsn1Sequence outSignedData, IAsn1Sequence cmpSignedData
            , String dest, String sig, StringBuilder errorText) {
            String sigFileName = dest.JSubstring(0, dest.LastIndexOf("."));
            String outSigFile = sigFileName + "_" + sig + "_out.txt";
            String cmpSigFile = sigFileName + "_" + sig + "_cmp.txt";
            WriteToFile(outSigFile, sig + "\n" + DUMP.DumpAsString(outSignedData, true) + "\n");
            WriteToFile(cmpSigFile, sig + "\n" + DUMP.DumpAsString(cmpSignedData, true) + "\n");
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
                IAsn1Sequence outDecodedItem = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(outDssEntryItem.GetBytes());
                IAsn1Sequence cmpDecodedItem = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(cmpDssEntryItem.GetBytes());
                if (!comparator(outDecodedItem, cmpDecodedItem, errorText)) {
                    CreateTxtFilesFromAsn1Sequences(outDecodedItem, cmpDecodedItem, dest, "DSS_" + entryName.GetValue() + "_" 
                        + i, errorText);
                    return false;
                }
            }
            return true;
        }

        private static bool CompareOcspResponses(IAsn1Encodable[] outOcspResponse, IAsn1Encodable[] cmpOcspResponse
            , StringBuilder errorText) {
            if (outOcspResponse.Length != 2 || cmpOcspResponse.Length != 2) {
                AddError(errorText, "OCSP response has unexpected structure");
            }
            IAsn1OctetString outResponseString = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(outOcspResponse[1]);
            IAsn1OctetString cmpResponseString = BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(cmpOcspResponse[1]);
            if (outResponseString.Equals(cmpResponseString)) {
                return true;
            }
            IAsn1Sequence parsedOutResponse = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(outResponseString.GetOctets());
            IAsn1Sequence parsedCmpResponse = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(cmpResponseString.GetOctets());
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
        private static bool CompareSignedData(IAsn1Sequence outSignedData, IAsn1Sequence cmpSignedData, StringBuilder
             errorText) {
            if (outSignedData.Size() != cmpSignedData.Size() || outSignedData.Size() != 2) {
                AddError(errorText, "Signature top level elements count is incorrect (should be exactly 2):", outSignedData
                    .Size().ToString(), cmpSignedData.Size().ToString());
                return false;
            }
            IDerObjectIdentifier outObjId = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(outSignedData.GetObjectAt
                (0));
            IDerObjectIdentifier cmpObjId = BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(cmpSignedData.GetObjectAt
                (0));
            if (!outObjId.Equals(cmpObjId) || !outObjId.GetId().Equals(OID_SIGNED_DATA)) {
                AddError(errorText, "Signatures object identifier is incorrect (should be " + OID_SIGNED_DATA + ")", outObjId
                    .GetId().ToString(), cmpObjId.GetId().ToString());
                return false;
            }
            IAsn1Sequence outContent = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject
                (outSignedData.GetObjectAt(1)).GetObject());
            IAsn1Sequence cmpContent = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject
                (cmpSignedData.GetObjectAt(1)).GetObject());
            if (outContent.Size() != cmpContent.Size()) {
                AddError(errorText, "Signatures base elements counts are different", outContent.Size().ToString(), cmpContent
                    .Size().ToString());
                return false;
            }
            int signerInfoIndex = GetSignerInfoIndex(cmpContent);
            if (BOUNCY_CASTLE_FACTORY.CreateASN1Set(outContent.GetObjectAt(signerInfoIndex)) == null) {
                AddError(errorText, "SignerInfo object indexes are different", null, null);
                return false;
            }
            for (int i = 0; i < cmpContent.Size(); i++) {
                // SignerInfo objects will be compared separately
                if (i == signerInfoIndex) {
                    continue;
                }
                // Sequences and sets related to timestamp token info should be ignored.
                if (OID_TST_INFO.Equals(GetASN1ObjectId(cmpContent.GetObjectAt(i).ToASN1Primitive())) && OID_TST_INFO.Equals
                    (GetASN1ObjectId(outContent.GetObjectAt(i).ToASN1Primitive()))) {
                    continue;
                }
                if (!cmpContent.GetObjectAt(i).Equals(outContent.GetObjectAt(i))) {
                    AddError(errorText, "SignedData objects are different", null, null);
                    return false;
                }
            }
            IAsn1Set cmpSignerInfos = BOUNCY_CASTLE_FACTORY.CreateASN1Set(cmpContent.GetObjectAt(signerInfoIndex));
            IAsn1Set outSignerInfos = BOUNCY_CASTLE_FACTORY.CreateASN1Set(outContent.GetObjectAt(signerInfoIndex));
            // Currently, iText signature validation mechanism do not support signatures,
            // containing more than one SignerInfo entry. However, it is still valid signature.
            if (cmpSignerInfos.Size() != outSignerInfos.Size() || cmpSignerInfos.Size() != 1) {
                AddError(errorText, "Incorrect SignerInfos objects count", outSignerInfos.Size().ToString(), cmpSignerInfos
                    .Size().ToString());
                return false;
            }
            IAsn1Sequence outSignerInfo = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(outSignerInfos.GetObjectAt(0));
            IAsn1Sequence cmpSignerInfo = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(cmpSignerInfos.GetObjectAt(0));
            return CompareSequencesWithSignatureValue(outSignerInfo, cmpSignerInfo, errorText);
        }

        private static bool CompareSequencesWithSignatureValue(IAsn1Sequence outSequence, IAsn1Sequence cmpSequence
            , StringBuilder errorText) {
            if (cmpSequence.Size() != outSequence.Size()) {
                AddError(errorText, "Incorrect SignerInfo entries count", outSequence.Size().ToString(), cmpSequence.Size(
                    ).ToString());
                return false;
            }
            for (int i = 0; i < cmpSequence.Size(); i++) {
                // Skipping comparison of encoded strings fields which are SignatureValue fields.
                // They are expected to be different.
                if (BOUNCY_CASTLE_FACTORY.CreateASN1OctetString(outSequence.GetObjectAt(i)) != null || BOUNCY_CASTLE_FACTORY
                    .CreateASN1BitString(outSequence.GetObjectAt(i)) != null) {
                    if (outSequence.GetObjectAt(i).GetType().Equals(cmpSequence.GetObjectAt(i).GetType())) {
                        continue;
                    }
                    else {
                        AddError(errorText, "Signature values indexes are different!", null, null);
                        return false;
                    }
                }
                if (!CompareAsn1Structures(outSequence.GetObjectAt(i).ToASN1Primitive(), cmpSequence.GetObjectAt(i).ToASN1Primitive
                    (), errorText)) {
                    return false;
                }
            }
            return true;
        }

        private static bool CompareAsn1Structures(IAsn1Object @out, IAsn1Object cmp, StringBuilder errorText) {
            if (!@out.GetType().Equals(cmp.GetType())) {
                AddError(errorText, "ASN1 objects types are different", @out.GetType().FullName, cmp.GetType().FullName);
                return false;
            }
            if (BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(cmp) != null) {
                return CompareAsn1Structures(BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(@out).GetObject(), BOUNCY_CASTLE_FACTORY
                    .CreateASN1TaggedObject(cmp).GetObject(), errorText);
            }
            else {
                if (BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(cmp) != null) {
                    if (!CompareContainers(BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(@out).ToArray(), BOUNCY_CASTLE_FACTORY.CreateASN1Sequence
                        (cmp).ToArray(), errorText)) {
                        AddError(errorText, "ASN1Sequence objects are different");
                        return false;
                    }
                }
                else {
                    if (BOUNCY_CASTLE_FACTORY.CreateASN1Set(cmp) != null) {
                        if (!CompareContainers(BOUNCY_CASTLE_FACTORY.CreateASN1Set(@out).ToArray(), BOUNCY_CASTLE_FACTORY.CreateASN1Set
                            (cmp).ToArray(), errorText)) {
                            AddError(errorText, "ASN1Set objects are different");
                            return false;
                        }
                    }
                    else {
                        if (BOUNCY_CASTLE_FACTORY.CreateASN1GeneralizedTime(cmp) != null || BOUNCY_CASTLE_FACTORY.CreateASN1UTCTime
                            (cmp) != null) {
                            // Ignore time values since usually they shouldn't be equal
                            return true;
                        }
                        else {
                            if (!cmp.Equals(@out)) {
                                AddError(errorText, "ASN1 objects are different", DUMP.DumpAsString(@out, true), DUMP.DumpAsString(cmp, true
                                    ));
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static bool CompareContainers(IAsn1Encodable[] outArray, IAsn1Encodable[] cmpArray, StringBuilder 
            errorText) {
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
                if (!CompareAsn1Structures(outArray[i].ToASN1Primitive(), cmpArray[i].ToASN1Primitive(), errorText)) {
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
        private static bool CompareRevocationInfoArchivalAttribute(IAsn1Encodable[] @out, IAsn1Encodable[] cmp, StringBuilder
             errorText) {
            String structureIsInvalidError = "Signature revocation info archival attribute structure is invalid";
            if (!IsExpectedRevocationInfoArchivalAttributeStructure(@out) || !IsExpectedRevocationInfoArchivalAttributeStructure
                (cmp)) {
                AddError(errorText, structureIsInvalidError, String.Join("", JavaUtil.ArraysToEnumerable(@out).Select((e) =>
                     DUMP.DumpAsString(e)).ToList()), String.Join("", JavaUtil.ArraysToEnumerable(cmp).Select((e) => DUMP.
                    DumpAsString(e)).ToList()));
                return false;
            }
            IAsn1Sequence outSequence = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(BOUNCY_CASTLE_FACTORY.CreateASN1Set(@out
                [1]).GetObjectAt(0).ToASN1Primitive());
            IAsn1Sequence cmpSequence = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(BOUNCY_CASTLE_FACTORY.CreateASN1Set(cmp
                [1]).GetObjectAt(0).ToASN1Primitive());
            if (outSequence.Size() != cmpSequence.Size()) {
                AddError(errorText, "Signature revocation info archival attributes have different sets of revocation info types "
                     + "(different sizes)", outSequence.Size().ToString(), cmpSequence.Size().ToString());
                return false;
            }
            for (int i = 0; i < outSequence.Size(); i++) {
                if (BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(outSequence.GetObjectAt(i)) == null || BOUNCY_CASTLE_FACTORY
                    .CreateASN1TaggedObject(cmpSequence.GetObjectAt(i)) == null) {
                    AddError(errorText, structureIsInvalidError, String.Join("", JavaUtil.ArraysToEnumerable(@out).Select((e) =>
                         DUMP.DumpAsString(e)).ToList()), String.Join("", JavaUtil.ArraysToEnumerable(cmp).Select((e) => DUMP.
                        DumpAsString(e)).ToList()));
                    return false;
                }
                IAsn1TaggedObject outTaggedObject = BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(outSequence.GetObjectAt(i
                    ));
                IAsn1TaggedObject cmpTaggedObject = BOUNCY_CASTLE_FACTORY.CreateASN1TaggedObject(cmpSequence.GetObjectAt(i
                    ));
                if (outTaggedObject.GetTagNo() != cmpTaggedObject.GetTagNo()) {
                    AddError(errorText, "Signature revocation info archival attributes have different tagged objects tag numbers"
                        , outTaggedObject.GetTagNo().ToString(), cmpTaggedObject.GetTagNo().ToString());
                    return false;
                }
                if (BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(outTaggedObject.GetObject()) == null || BOUNCY_CASTLE_FACTORY
                    .CreateASN1Sequence(cmpTaggedObject.GetObject()) == null) {
                    AddError(errorText, structureIsInvalidError, String.Join("", JavaUtil.ArraysToEnumerable(@out).Select((e) =>
                         DUMP.DumpAsString(e)).ToList()), String.Join("", JavaUtil.ArraysToEnumerable(cmp).Select((e) => DUMP.
                        DumpAsString(e)).ToList()));
                    return false;
                }
                // revocation entries can be either CRLs or OCSPs in most cases
                IAsn1Sequence outRevocationEntries = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(outTaggedObject.GetObject());
                IAsn1Sequence cmpRevocationEntries = BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(cmpTaggedObject.GetObject());
                if (outRevocationEntries.Size() != cmpRevocationEntries.Size()) {
                    AddError(errorText, "Signature revocation info archival attributes have different number of entries", outRevocationEntries
                        .Size().ToString(), cmpRevocationEntries.Size().ToString());
                    return false;
                }
                if (outTaggedObject.GetTagNo() == 0) {
                    // CRL revocation info case
                    for (int j = 0; j < outRevocationEntries.Size(); j++) {
                        if (BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(outRevocationEntries.GetObjectAt(j)) == null || BOUNCY_CASTLE_FACTORY
                            .CreateASN1Sequence(outRevocationEntries.GetObjectAt(j)) == null) {
                            AddError(errorText, "Signature revocation info attribute has unexpected CRL entry type", outRevocationEntries
                                .GetObjectAt(j).GetType().FullName.ToString(), cmpRevocationEntries.GetObjectAt(j).GetType().FullName.
                                ToString());
                            return false;
                        }
                        if (!CompareSequencesWithSignatureValue(BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(outRevocationEntries.GetObjectAt
                            (j)), BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(cmpRevocationEntries.GetObjectAt(j)), errorText)) {
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

        private static bool IsExpectedRevocationInfoArchivalAttributeStructure(IAsn1Encodable[] container) {
            return container.Length == 2 && BOUNCY_CASTLE_FACTORY.CreateASN1Set(container[1]) != null && BOUNCY_CASTLE_FACTORY
                .CreateASN1Set(container[1]).Size() == 1 && BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(BOUNCY_CASTLE_FACTORY
                .CreateASN1Set(container[1]).GetObjectAt(0).ToASN1Primitive()) != null;
        }

        private static bool CompareTimestampAttributes(IAsn1Encodable[] @out, IAsn1Encodable[] cmp, StringBuilder 
            errorText) {
            if (cmp.Length == 2) {
                if (BOUNCY_CASTLE_FACTORY.CreateASN1Set(cmp[1]) != null && BOUNCY_CASTLE_FACTORY.CreateASN1Set(@out[1]) !=
                     null) {
                    IAsn1Object outSequence = BOUNCY_CASTLE_FACTORY.CreateASN1Set(@out[1]).GetObjectAt(0).ToASN1Primitive();
                    IAsn1Object cmpSequence = BOUNCY_CASTLE_FACTORY.CreateASN1Set(cmp[1]).GetObjectAt(0).ToASN1Primitive();
                    if (BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(outSequence) != null && BOUNCY_CASTLE_FACTORY.CreateASN1Sequence
                        (cmpSequence) != null) {
                        return CompareSignedData(BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(outSequence), BOUNCY_CASTLE_FACTORY.CreateASN1Sequence
                            (cmpSequence), errorText);
                    }
                }
            }
            AddError(errorText, "Signature timestamp attribute structure is invalid", String.Join("", JavaUtil.ArraysToEnumerable
                (@out).Select((e) => DUMP.DumpAsString(e)).ToList()), String.Join("", JavaUtil.ArraysToEnumerable(cmp)
                .Select((e) => DUMP.DumpAsString(e)).ToList()));
            return false;
        }

        private static int GetSignerInfoIndex(IAsn1Sequence baseElement) {
            for (int i = 3; i < baseElement.Size(); i++) {
                if (BOUNCY_CASTLE_FACTORY.CreateASN1Set(baseElement.GetObjectAt(i)) != null) {
                    return i;
                }
            }
            throw new InvalidOperationException("SignerInfo entry has not been found.");
        }

        private static String GetASN1ObjectId(IAsn1Object primitive) {
            if (BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(primitive) != null) {
                return GetASN1ObjectId(BOUNCY_CASTLE_FACTORY.CreateASN1Sequence(primitive).ToArray());
            }
            if (BOUNCY_CASTLE_FACTORY.CreateASN1Set(primitive) != null) {
                return GetASN1ObjectId(BOUNCY_CASTLE_FACTORY.CreateASN1Set(primitive).ToArray());
            }
            return null;
        }

        private static String GetASN1ObjectId(IAsn1Encodable[] primitives) {
            if (primitives.Length != 0 && BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(primitives[0]) != null) {
                return BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(primitives[0]).GetId();
            }
            return null;
        }

        private static IAsn1Object GetSignatureContent(String signatureName, SignatureUtil util) {
            PdfSignature signature = util.GetSignature(signatureName);
            byte[] contents = signature.GetContents().GetValueBytes();
            IAsn1InputStream inputStream = BOUNCY_CASTLE_FACTORY.CreateASN1InputStream(new MemoryStream(contents));
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

        internal delegate bool SequenceComparator(IAsn1Sequence outSequence, IAsn1Sequence cmpSequence, StringBuilder
             errorText);
    }
}
