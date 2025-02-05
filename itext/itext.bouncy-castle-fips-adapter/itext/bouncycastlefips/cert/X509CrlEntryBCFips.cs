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
using iText.Bouncycastlefips.Math;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Math;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Cert;
using System;

namespace iText.Bouncycastlefips.Cert
{
    //\cond DO_NOT_DOCUMENT
    internal class X509CrlEntryBCFips : IX509CrlEntry
    {
        private X509CrlEntry entry;

        public X509CrlEntryBCFips(X509CrlEntry entry)
        {
            this.entry = entry;
        }

        public CRLReason GetReason()
        {
            byte[] reasonExt = entry.GetExtensionValue(new Org.BouncyCastle.Asn1.DerObjectIdentifier("2.5.29.21"));
            if (reasonExt == null) return CRLReason.UNSPECIFIED;
            var reasonEnum = DerEnumerated.GetInstance(Asn1Object.FromByteArray(reasonExt));
            return (CRLReason)reasonEnum.IntValueExact;
        }

        public DateTime GetRevocationDate()
        {
            return entry.RevocationDate;
        }

        public IBigInteger GetSerialNumber()
        {
            return new BigIntegerBCFips(entry.SerialNumber);
        }
    }
    //\endcond
}