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
using iText.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Math;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;
using System;

namespace iText.Bouncycastle.X509
{
    public class X509CrlEntryBC : IX509CrlEntry
    {
        private X509CrlEntry entry;

        public X509CrlEntryBC(X509CrlEntry entry)
        {
            this.entry = entry;
        }

        public CRLReason GetReason()
        {
            var reasonExt = entry.GetExtensionValue(new Org.BouncyCastle.Asn1.DerObjectIdentifier("2.5.29.21"));
            if (reasonExt == null) return CRLReason.UNSPECIFIED;
            var reasonEnum = DerEnumerated.GetInstance(reasonExt);
            return (CRLReason)reasonEnum.IntValueExact;
        }

        public DateTime GetRevocationDate()
        {
            return entry.RevocationDate;            
        }

        public IBigInteger GetSerialNumber()
        {
            return new BigIntegerBC(entry.SerialNumber);            
        }
    }
}