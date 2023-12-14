/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Linq;
using System.Text;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Utilities.Collections;
using System.Collections;

namespace iText.Signatures.Testutils
{
    class X509MockCertificate : X509Certificate
    {
        public X509MockCertificate() 
        {
            this.criticalExtensions = new HashSet();
        }

        internal ISet criticalExtensions;

        internal IList extendedKeyUsage;

        public bool[] KeyUsage { get; set; }

        protected override X509Extensions GetX509Extensions()
        {
            throw new NotImplementedException();
        }

        public void SetCriticalExtensions(ISet criticalExtensions)
        {
            this.criticalExtensions = criticalExtensions;
        }

        public override ISet GetCriticalExtensionOids()
        {
            return this.criticalExtensions;
        }

        public override bool[] GetKeyUsage()
        {
            return this.KeyUsage;
        }
        
        public override X509Name SubjectDN
        {
            get { return null; }
        }

        public void SetExtendedKeyUsage(IList extendedKeyUsage)
        {
            this.extendedKeyUsage = extendedKeyUsage;
        }

        public override IList GetExtendedKeyUsage()
        {
            return this.extendedKeyUsage;
        }
    }
}
