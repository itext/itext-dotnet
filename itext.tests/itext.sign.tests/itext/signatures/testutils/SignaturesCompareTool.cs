/*

This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Signatures;

namespace iText.Signatures.Testutils {
    public class SignaturesCompareTool {
        private const String OID_MESSAGE_DIGEST = "1.2.840.113549.1.9.4";

        private const String OID_SIGNED_DATA = "1.2.840.113549.1.7.2";

        public static String CompareSignatures(String dest, String cmp) {
            StringBuilder errorText = new StringBuilder();
            try {
                using (PdfDocument outDocument = new PdfDocument(new PdfReader(dest))) {
                    using (PdfDocument cmpDocument = new PdfDocument(new PdfReader(cmp))) {
                        SignatureUtil outSigUtil = new SignatureUtil(outDocument);
                        SignatureUtil cmpSigUtil = new SignatureUtil(cmpDocument);
                        if (!Enumerable.SequenceEqual(cmpSigUtil.GetSignatureNames(), outSigUtil.GetSignatureNames())) {
                            AddError(errorText, "Signatures lists are different:", outSigUtil.GetSignatureNames().ToString(), cmpSigUtil
                                .GetSignatureNames().ToString());
                        }
                        IList<String> signatures = cmpSigUtil.GetSignatureNames();
                        foreach (String sig in signatures) {
                            bool isFailed = false;
                            Asn1Sequence outSignedData = (Asn1Sequence)GetSignatureContent(sig, outSigUtil);
                            Asn1Sequence cmpSignedData = (Asn1Sequence)GetSignatureContent(sig, cmpSigUtil);
                            if (outSignedData.Count != cmpSignedData.Count || outSignedData.Count != 2) {
                                AddError(errorText, "Signature top level elements count is incorrect (should be exactly 2):", outSignedData
                                    .Count.ToString(), cmpSignedData.Count.ToString());
                                isFailed = true;
                            }
                            DerObjectIdentifier outObjId = (DerObjectIdentifier)outSignedData[0];
                            DerObjectIdentifier cmpObjId = (DerObjectIdentifier)cmpSignedData[0];
                            if (!outObjId.Equals(cmpObjId) || !outObjId.Id.Equals(OID_SIGNED_DATA)) {
                                AddError(errorText, "Signatures object identifier is incorrect (should be " + OID_SIGNED_DATA + ")", outObjId
                                    .Id.ToString(), cmpObjId.Id.ToString());
                                isFailed = true;
                            }
                            Asn1Sequence outContent = (Asn1Sequence)((Asn1TaggedObject)outSignedData[1]).GetObject();
                            Asn1Sequence cmpContent = (Asn1Sequence)((Asn1TaggedObject)cmpSignedData[1]).GetObject();
                            if (outContent.Count != cmpContent.Count) {
                                AddError(errorText, "Signatures base elements counts are different", outContent.Count.ToString(), cmpContent
                                    .Count.ToString());
                                isFailed = true;
                            }
                            int signerInfoIndex = GetSignerInfoIndex(cmpContent);
                            if (outContent[signerInfoIndex] is Asn1TaggedObject) {
                                AddError(errorText, "SignerInfo object indexes are different", null, null);
                                isFailed = true;
                            }
                            for (int i = 0; i < cmpContent.Count; i++) {
                                // SignerInfo objects will be compared separately
                                if (i == signerInfoIndex) {
                                    continue;
                                }
                                if (!cmpContent[i].Equals(outContent[i])) {
                                    AddError(errorText, "SignedData objects are different", null, null);
                                    isFailed = true;
                                }
                            }
                            Asn1Set cmpSignerInfos = (Asn1Set)cmpContent[signerInfoIndex];
                            Asn1Set outSignerInfos = (Asn1Set)outContent[signerInfoIndex];
                            // Currently, iText signature validation mechanism do not support signatures,
                            // containing more than one SignerInfo entry. However, it is still valid signature.
                            if (cmpSignerInfos.Count != outSignerInfos.Count || cmpSignerInfos.Count != 1) {
                                AddError(errorText, "Incorrect SignerInfos objects count", outSignerInfos.Count.ToString(), cmpSignerInfos
                                    .Count.ToString());
                                isFailed = true;
                            }
                            Asn1Sequence outSignerInfo = (Asn1Sequence)cmpSignerInfos[0];
                            Asn1Sequence cmpSignerInfo = (Asn1Sequence)outSignerInfos[0];
                            if (cmpSignerInfo.Count != outSignerInfo.Count) {
                                AddError(errorText, "Incorrect SignerInfo entries count", outSignerInfo.Count.ToString(), cmpSignerInfo.Count
                                    .ToString());
                                isFailed = true;
                            }
                            for (int i = 0; i < cmpSignerInfo.Count; i++) {
                                // Skipping comparison of ASN1OctetString fields in SignerInfo. SignerInfo is expected to have
                                // a single field of ASN1OctetString which is SignatureValue, that is expected to be
                                // different in each signature instance.
                                if (outSignerInfo[i] is Asn1OctetString) {
                                    if (cmpSignerInfo[i] is Asn1OctetString) {
                                        continue;
                                    }
                                    else {
                                        AddError(errorText, "Signature values indexes are different!", null, null);
                                        isFailed = true;
                                    }
                                }
                                if (!isFailed) {
                                    isFailed = CompareAsn1Structures(outSignerInfo[i].ToAsn1Object(), cmpSignerInfo[i].ToAsn1Object(), errorText
                                        );
                                }
                            }
                            if (isFailed) {
                                String sigFileName = dest.JSubstring(0, dest.LastIndexOf("."));
                                String outSigFile = sigFileName + "_out.txt";
                                String cmpSigFile = sigFileName + "_cmp.txt";
                                WriteToFile(outSigFile, sig + "\n" + Asn1Dump.DumpAsString(outSignedData, true) + "\n");
                                WriteToFile(cmpSigFile, sig + "\n" + Asn1Dump.DumpAsString(cmpSignedData, true) + "\n");
                                errorText.Insert(0, "See signature output files: \nout: " + UrlUtil.GetNormalizedFileUriString(outSigFile)
                                     + "\ncmp: " + UrlUtil.GetNormalizedFileUriString(cmpSigFile) + "\n");
                            }
                        }
                    }
                }
            }
            catch (Exception e) {
                errorText.Append(e.Message).Append("\n");
            }
            if (!String.IsNullOrEmpty(errorText.ToString())) {
                return errorText.ToString();
            }
            else {
                return null;
            }
        }

        private static void WriteToFile(String path, String content) {
            using (StreamWriter writer = new StreamWriter(path, true)) {
                writer.Write(content);
            }
        }

        private static int GetSignerInfoIndex(Asn1Sequence baseElement) {
            for (int i = 3; i < baseElement.Count; i++) {
                if (!(baseElement[i] is Asn1TaggedObject)) {
                    return i;
                }
            }
            throw new InvalidOperationException("SignerInfo entry has not been found.");
        }

        private static bool CompareAsn1Structures(Asn1Object @out, Asn1Object cmp, StringBuilder errorText) {
            bool isFailed = false;
            if (!@out.GetType().Equals(cmp.GetType())) {
                AddError(errorText, "ASN1 objects types are different", @out.GetType().FullName, cmp.GetType().FullName);
                isFailed = true;
            }
            if (cmp is Asn1TaggedObject || cmp is Asn1Sequence) {
                Asn1Sequence cmpObject;
                Asn1Sequence outObject;
                if (cmp is Asn1TaggedObject) {
                    Asn1TaggedObject cmpTag = (Asn1TaggedObject)cmp;
                    Asn1TaggedObject outTag = (Asn1TaggedObject)@out;
                    if (!(cmpTag.GetObject() is Asn1Sequence)) {
                        if (!cmpTag.GetObject().Equals(outTag.GetObject())) {
                            AddError(errorText, "ASN1 objects are different", Asn1Dump.DumpAsString(outTag, true), Asn1Dump.DumpAsString
                                (cmpTag, true));
                            isFailed = true;
                        }
                        return isFailed;
                    }
                    cmpObject = (Asn1Sequence)(cmpTag).GetObject();
                    outObject = (Asn1Sequence)(outTag).GetObject();
                }
                else {
                    cmpObject = (Asn1Sequence)cmp;
                    outObject = (Asn1Sequence)@out;
                }
                if (cmpObject[0] is DerObjectIdentifier) {
                    DerObjectIdentifier objectIdentifier = (DerObjectIdentifier)(cmpObject[0]);
                    // Message digest should be ignored during comparing
                    if (objectIdentifier.Id.Equals(OID_MESSAGE_DIGEST)) {
                        return isFailed;
                    }
                }
                for (int i = 0; i < cmpObject.Count; i++) {
                    if (!isFailed) {
                        isFailed = CompareAsn1Structures(outObject[i].ToAsn1Object(), cmpObject[i].ToAsn1Object(), errorText);
                    }
                }
            }
            else {
                if (cmp is Asn1Set) {
                    Asn1Set cmpSet = (Asn1Set)cmp;
                    Asn1Set outSet = (Asn1Set)@out;
                    if (!isFailed) {
                        isFailed = CompareAsn1Structures(cmpSet[0].ToAsn1Object(), outSet[0].ToAsn1Object(), errorText);
                    }
                }
                else {
                    if (!cmp.Equals(@out)) {
                        AddError(errorText, "ASN1 objects are different", Asn1Dump.DumpAsString(@out, true), Asn1Dump.DumpAsString
                            (cmp, true));
                        isFailed = true;
                    }
                }
            }
            return isFailed;
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

        private static Asn1Object GetSignatureContent(String signatureName, SignatureUtil util) {
            PdfSignature signature = util.GetSignature(signatureName);
            byte[] contents = signature.GetContents().GetValueBytes();
            Asn1InputStream inputStream = new Asn1InputStream(new MemoryStream(contents));
            return inputStream.ReadObject();
        }
    }
}
