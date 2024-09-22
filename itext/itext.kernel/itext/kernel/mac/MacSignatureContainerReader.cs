using System;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Mac {
//\cond DO_NOT_DOCUMENT
    internal class MacSignatureContainerReader : MacContainerReader {
        private static readonly IBouncyCastleFactory BC_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private const String ID_ATTR_PDF_MAC_DATA = "1.0.32004.1.2";

//\cond DO_NOT_DOCUMENT
        internal MacSignatureContainerReader(PdfDictionary authDictionary)
            : base(authDictionary) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override byte[] ParseSignature(PdfDictionary authDictionary) {
            PdfDictionary signatureDictionary = GetSignatureDictionary(authDictionary);
            PdfString contentsString = signatureDictionary.GetAsString(PdfName.Contents);
            contentsString.MarkAsUnencryptedObject();
            return ParseSignatureValueFromSignatureContainer(contentsString.GetValueBytes());
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override long[] ParseByteRange(PdfDictionary authDictionary) {
            PdfDictionary signatureDictionary = GetSignatureDictionary(authDictionary);
            return signatureDictionary.GetAsArray(PdfName.ByteRange).ToLongArray();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override byte[] ParseMacContainer(PdfDictionary authDictionary) {
            PdfDictionary signatureDictionary = GetSignatureDictionary(authDictionary);
            PdfString contentsString = signatureDictionary.GetAsString(PdfName.Contents);
            contentsString.MarkAsUnencryptedObject();
            return ParseMacContainerFromSignatureContainer(contentsString.GetValueBytes());
        }
//\endcond

        private static byte[] ParseSignatureValueFromSignatureContainer(byte[] signature) {
            try {
                IAsn1Sequence signerInfoSeq = ParseSignerInfoSequence(signature);
                int signatureValueIndex = 3;
                IAsn1TaggedObject taggedSignedAttributes = BC_FACTORY.CreateASN1TaggedObject(signerInfoSeq.GetObjectAt(signatureValueIndex
                    ));
                if (taggedSignedAttributes != null) {
                    ++signatureValueIndex;
                }
                IDerOctetString signatureDataOS = BC_FACTORY.CreateDEROctetString(signerInfoSeq.GetObjectAt(++signatureValueIndex
                    ));
                return signatureDataOS.GetOctets();
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.MAC_EXTRACTION_EXCEPTION, e);
            }
        }

        private static byte[] ParseMacContainerFromSignatureContainer(byte[] signature) {
            try {
                IAsn1Sequence signerInfoSeq = ParseSignerInfoSequence(signature);
                int unsignedAttributesIndex = 3;
                IAsn1TaggedObject taggedSignedAttributes = BC_FACTORY.CreateASN1TaggedObject(signerInfoSeq.GetObjectAt(unsignedAttributesIndex
                    ));
                if (taggedSignedAttributes != null) {
                    ++unsignedAttributesIndex;
                }
                unsignedAttributesIndex += 2;
                if (signerInfoSeq.Size() > unsignedAttributesIndex) {
                    IAsn1Set unsignedAttributes = BC_FACTORY.CreateASN1Set(BC_FACTORY.CreateASN1TaggedObject(signerInfoSeq.GetObjectAt
                        (unsignedAttributesIndex)), false);
                    for (int i = 0; i < unsignedAttributes.Size(); i++) {
                        IAsn1Sequence attrSeq = BC_FACTORY.CreateASN1Sequence(unsignedAttributes.GetObjectAt(i));
                        IDerObjectIdentifier attrType = BC_FACTORY.CreateASN1ObjectIdentifier(attrSeq.GetObjectAt(0));
                        if (ID_ATTR_PDF_MAC_DATA.Equals(attrType.GetId())) {
                            IAsn1Set macSet = BC_FACTORY.CreateASN1Set(attrSeq.GetObjectAt(1));
                            return macSet.GetObjectAt(0).ToASN1Primitive().GetEncoded();
                        }
                    }
                }
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.MAC_EXTRACTION_EXCEPTION, e);
            }
            throw new PdfException(KernelExceptionMessageConstant.MAC_ATTRIBUTE_NOT_SPECIFIED);
        }

        private static PdfDictionary GetSignatureDictionary(PdfDictionary authDictionary) {
            if (authDictionary.GetAsDictionary(PdfName.SigObjRef) == null) {
                throw new PdfException(KernelExceptionMessageConstant.SIG_OBJ_REF_NOT_SPECIFIED);
            }
            return authDictionary.GetAsDictionary(PdfName.SigObjRef);
        }

        private static IAsn1Sequence ParseSignerInfoSequence(byte[] signature) {
            using (IAsn1InputStream @is = BC_FACTORY.CreateASN1InputStream(new MemoryStream(signature))) {
                IAsn1Sequence contentInfo = BC_FACTORY.CreateASN1Sequence(@is.ReadObject());
                IAsn1Sequence signedData = BC_FACTORY.CreateASN1Sequence(BC_FACTORY.CreateASN1TaggedObject(contentInfo.GetObjectAt
                    (1)).GetObject());
                int signerInfoIndex = 4;
                IAsn1TaggedObject taggedObj = BC_FACTORY.CreateASN1TaggedObject(signedData.GetObjectAt(signerInfoIndex));
                if (taggedObj != null) {
                    ++signerInfoIndex;
                }
                return BC_FACTORY.CreateASN1Sequence(BC_FACTORY.CreateASN1Set(signedData.GetObjectAt(signerInfoIndex)).GetObjectAt
                    (0));
            }
        }
    }
//\endcond
}
