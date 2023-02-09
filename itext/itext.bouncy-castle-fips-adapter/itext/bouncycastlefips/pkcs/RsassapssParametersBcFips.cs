/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Math;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Pkcs;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Pkcs {
    /// <summary>
    /// BC-FIPS wrapper implementation for
    /// <see cref="IRsassaPssParameters"/>.
    /// </summary>
    public class RsassaPssParametersBcFips : ASN1EncodableBCFips, IRsassaPssParameters {
        private readonly RsassaPssParameters @params;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Pkcs.RSASSAPSSparams"/>.
        /// </summary>
        /// <param name="params">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Pkcs.RSASSAPSSparams"/>
        /// to be wrapped
        /// </param>
        public RsassaPssParametersBcFips(RsassaPssParameters @params)
            : base(@params) 
        {
            this.@params = @params;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier GetHashAlgorithm() 
        {
            return new AlgorithmIdentifierBCFips(@params.HashAlgorithm);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier GetMaskGenAlgorithm() 
        {
            return new AlgorithmIdentifierBCFips(@params.MaskGenAlgorithm);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Integer GetSaltLength()
        {
            return new ASN1IntegerBCFips(@params.SaltLength);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Integer GetTrailerField() 
        {
            return new ASN1IntegerBCFips(@params.TrailerField);
        }
    }
}
