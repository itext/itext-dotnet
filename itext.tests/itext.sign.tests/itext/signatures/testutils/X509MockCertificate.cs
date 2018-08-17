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
