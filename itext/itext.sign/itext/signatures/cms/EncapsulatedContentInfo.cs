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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Kernel.Crypto;

namespace iText.Signatures.Cms {
    /// <summary>This class represents the signed content.</summary>
    public class EncapsulatedContentInfo {
        private static readonly IBouncyCastleFactory BC_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        /// <summary>Object identifier of the content field</summary>
        private String eContentType = OID.PKCS7_DATA;

        /// <summary>Optional.</summary>
        /// <remarks>
        /// Optional.
        /// <para />
        /// The actual content as an octet string. Does not have to be DER encoded.
        /// </remarks>
        private IAsn1OctetString eContent;

        /// <summary>Creates an EncapsulatedContentInfo with contenttype and content.</summary>
        /// <param name="eContentType">the content type Oid (object id)</param>
        /// <param name="eContent">the content</param>
        public EncapsulatedContentInfo(String eContentType, IAsn1OctetString eContent) {
            this.eContentType = eContentType;
            this.eContent = eContent;
        }

        /// <summary>Creates an EncapsulatedContentInfo with contenttype.</summary>
        /// <param name="eContentType">the content type Oid (object id)</param>
        public EncapsulatedContentInfo(String eContentType) {
            this.eContentType = eContentType;
        }

        /// <summary>Creates a default EncapsulatedContentInfo.</summary>
        public EncapsulatedContentInfo() {
        }

//\cond DO_NOT_DOCUMENT
        // Empty constructor.
        internal EncapsulatedContentInfo(IAsn1Sequence lencapContentInfo) {
            IDerObjectIdentifier eContentTypeOid = BC_FACTORY.CreateASN1ObjectIdentifier(lencapContentInfo.GetObjectAt
                (0));
            IAsn1OctetString eContentElem = null;
            if (lencapContentInfo.Size() > 1) {
                IAsn1TaggedObject taggedElement = BC_FACTORY.CreateASN1TaggedObject(lencapContentInfo.GetObjectAt(1));
                eContentElem = BC_FACTORY.CreateASN1OctetString(taggedElement.GetObject());
                if (eContentElem != null) {
                    eContent = eContentElem;
                }
            }
            eContentType = eContentTypeOid.GetId();
        }
//\endcond

        /// <summary>Returns the contenttype oid.</summary>
        /// <returns>the contenttype oid.</returns>
        public virtual String GetContentType() {
            return eContentType;
        }

        /// <summary>Returns the content.</summary>
        /// <returns>the content.</returns>
        public virtual IAsn1OctetString GetContent() {
            return eContent;
        }
    }
}
