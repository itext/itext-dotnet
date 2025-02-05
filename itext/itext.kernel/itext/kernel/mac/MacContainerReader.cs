/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Mac {
//\cond DO_NOT_DOCUMENT
    internal abstract class MacContainerReader {
        private static readonly IBouncyCastleFactory BC_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private readonly byte[] macContainer;

        private readonly long[] byteRange;

        private readonly byte[] signature;

//\cond DO_NOT_DOCUMENT
        internal MacContainerReader(PdfDictionary authDictionary) {
            this.macContainer = ParseMacContainer(authDictionary);
            this.byteRange = ParseByteRange(authDictionary);
            this.signature = ParseSignature(authDictionary);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static iText.Kernel.Mac.MacContainerReader GetInstance(PdfDictionary authDictionary) {
            PdfName macLocation = authDictionary.GetAsName(PdfName.MACLocation);
            if (PdfName.Standalone.Equals(macLocation)) {
                return new MacStandaloneContainerReader(authDictionary);
            }
            else {
                if (PdfName.AttachedToSig.Equals(macLocation)) {
                    return new MacSignatureContainerReader(authDictionary);
                }
            }
            throw new PdfException(KernelExceptionMessageConstant.MAC_LOCATION_NOT_SPECIFIED);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal abstract byte[] ParseSignature(PdfDictionary authDictionary);
//\endcond

//\cond DO_NOT_DOCUMENT
        internal abstract long[] ParseByteRange(PdfDictionary authDictionary);
//\endcond

//\cond DO_NOT_DOCUMENT
        internal abstract byte[] ParseMacContainer(PdfDictionary authDictionary);
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual long[] GetByteRange() {
            return byteRange;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual byte[] GetSignature() {
            return signature;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual byte[] ParseMac() {
            IAsn1Sequence authDataSequence = GetAuthDataSequence();
            return BC_FACTORY.CreateASN1OctetString(authDataSequence.GetObjectAt(6)).GetOctets();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual IAsn1Set ParseAuthAttributes() {
            IAsn1Sequence authDataSequence = GetAuthDataSequence();
            return BC_FACTORY.CreateASN1Set(BC_FACTORY.CreateASN1TaggedObject(authDataSequence.GetObjectAt(5)), false);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual IAsn1Sequence ParseMessageDigest() {
            IAsn1Set authAttributes = ParseAuthAttributes();
            return BC_FACTORY.CreateASN1Sequence(authAttributes.GetObjectAt(2));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual byte[] ParseMacKey() {
            IAsn1Sequence authDataSequence = GetAuthDataSequence();
            IAsn1Sequence recInfo = BC_FACTORY.CreateASN1Sequence(BC_FACTORY.CreateASN1TaggedObject(BC_FACTORY.CreateASN1Set
                (authDataSequence.GetObjectAt(1)).GetObjectAt(0)).GetObject());
            IAsn1OctetString encryptedKey = BC_FACTORY.CreateASN1OctetString(recInfo.GetObjectAt(3));
            return encryptedKey.GetOctets();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual String ParseDigestAlgorithm() {
            IAsn1Sequence authDataSequence = GetAuthDataSequence();
            IAsn1Object digestAlgorithmContainer = BC_FACTORY.CreateASN1TaggedObject(authDataSequence.GetObjectAt(3)).
                GetObject();
            IDerObjectIdentifier digestAlgorithm;
            if (BC_FACTORY.CreateASN1ObjectIdentifier(digestAlgorithmContainer) != null) {
                digestAlgorithm = BC_FACTORY.CreateASN1ObjectIdentifier(digestAlgorithmContainer);
            }
            else {
                digestAlgorithm = BC_FACTORY.CreateASN1ObjectIdentifier(BC_FACTORY.CreateASN1Sequence(digestAlgorithmContainer
                    ).GetObjectAt(0));
            }
            return digestAlgorithm.GetId();
        }
//\endcond

        private IAsn1Sequence GetAuthDataSequence() {
            IAsn1Sequence contentInfoSequence;
            try {
                using (IAsn1InputStream din = BC_FACTORY.CreateASN1InputStream(new MemoryStream(macContainer))) {
                    contentInfoSequence = BC_FACTORY.CreateASN1Sequence(din.ReadObject());
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.CONTAINER_PARSING_EXCEPTION, e);
            }
            return BC_FACTORY.CreateASN1Sequence(BC_FACTORY.CreateASN1TaggedObject(contentInfoSequence.GetObjectAt(1))
                .GetObject());
        }
    }
//\endcond
}
