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

namespace iText.Signatures.Cms {
    /// <summary>This class represents algorithm identifier structure.</summary>
    public class AlgorithmIdentifier {
        private static readonly IBouncyCastleFactory BC_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private readonly String algorithm;

        private readonly IAsn1Object parameters;

        /// <summary>Creates an Algorithm identifier structure without parameters.</summary>
        /// <param name="algorithmId">the Object id of the algorithm</param>
        public AlgorithmIdentifier(String algorithmId) {
            this.algorithm = algorithmId;
            parameters = null;
        }

        /// <summary>Creates an Algorithm identifier structure with parameters.</summary>
        /// <param name="algorithmId">the Object id of the algorithm</param>
        /// <param name="parameters">the algorithm parameters as an ASN1 structure</param>
        public AlgorithmIdentifier(String algorithmId, IAsn1Object parameters) {
            this.algorithm = algorithmId;
            this.parameters = parameters;
        }

        /// <summary>Creates an Algorithm identifier structure with parameters.</summary>
        /// <param name="asnStruct">asn1 encodable to retrieve algorithm identifier</param>
        internal AlgorithmIdentifier(IAsn1Encodable asnStruct) {
            IAsn1Sequence algIdentifier = BC_FACTORY.CreateASN1Sequence(asnStruct);
            IDerObjectIdentifier algOid = BC_FACTORY.CreateASN1ObjectIdentifier(algIdentifier.GetObjectAt(0));
            algorithm = algOid.GetId();
            if (algIdentifier.Size() > 1) {
                parameters = BC_FACTORY.CreateASN1Primitive(algIdentifier.GetObjectAt(1));
            }
            else {
                parameters = null;
            }
        }

        /// <summary>Return the OID of the algorithm.</summary>
        /// <returns>the OID of the algorithm.</returns>
        public virtual String GetAlgorithmOid() {
            return algorithm;
        }

        /// <summary>Return the parameters for the algorithm.</summary>
        /// <returns>the parameters for the algorithm.</returns>
        public virtual IAsn1Object GetParameters() {
            return parameters;
        }

        internal virtual IAsn1Sequence GetAsASN1Sequence() {
            IAsn1EncodableVector algorithmV = BC_FACTORY.CreateASN1EncodableVector();
            algorithmV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(algorithm));
            if (parameters != null) {
                algorithmV.Add(parameters);
            }
            return BC_FACTORY.CreateDERSequence(algorithmV);
        }
    }
}
