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
using System.Collections;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;

namespace iText.Signatures.Testutils
{
    class X509MockCertificate : IX509Certificate
    {
        public X509MockCertificate() 
        {
            this.criticalExtensions = new HashSet<string>();
        }

        internal ISet<string> criticalExtensions;

        internal IList extendedKeyUsage;

        public bool[] KeyUsage { get; set; }

        public void SetCriticalExtensions(ISet<string> criticalExtensions)
        {
            this.criticalExtensions = criticalExtensions;
        }

        public IX500Name GetIssuerDN()
        {
            throw new NotImplementedException();
        }

        public IBigInteger GetSerialNumber()
        {
            throw new NotImplementedException();
        }

        public IPublicKey GetPublicKey()
        {
            throw new NotImplementedException();
        }

        public string GetSigAlgOID()
        {
            throw new NotImplementedException();
        }

        public byte[] GetSigAlgParams()
        {
            throw new NotImplementedException();
        }

        public byte[] GetEncoded()
        {
            throw new NotImplementedException();
        }

        public byte[] GetTbsCertificate() {
            return Array.Empty<byte>();
        }

        public IAsn1OctetString GetExtensionValue(string oid)
        {
            throw new NotImplementedException();
        }

        public void Verify(IPublicKey issuerPublicKey)
        {
            throw new NotImplementedException();
        }
        
        public ISet<string> GetCriticalExtensionOids()
        {
            return this.criticalExtensions;
        }

        public void CheckValidity(DateTime time)
        {
            throw new NotImplementedException();
        }

        public string GetEndDateTime()
        {
            throw new NotImplementedException();
        }

        public DateTime GetNotBefore()
        {
            throw new NotImplementedException();
        }

        public DateTime GetNotAfter()
        {
            throw new NotImplementedException();
        }

        public IX500Name GetSubjectDN()
        {
            return null;
        }

        public void SetExtendedKeyUsage(IList extendedKeyUsage)
        {
            this.extendedKeyUsage = extendedKeyUsage;
        }

        public IList GetExtendedKeyUsage()
        {
            return this.extendedKeyUsage;
        }
    }
}
