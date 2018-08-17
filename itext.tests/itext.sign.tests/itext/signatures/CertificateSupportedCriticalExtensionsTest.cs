using iText.Signatures.Testutils;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iText.Signatures
{
    class CertificateSupportedCriticalExtensionsTest
    {
        [NUnit.Framework.Test]
        public void SupportedCriticalOIDsTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            ISet criticalExtensions = new HashSet();

            criticalExtensions.Add(OID.X509Extensions.KEY_USAGE);
            criticalExtensions.Add(OID.X509Extensions.BASIC_CONSTRAINTS);

            cert.SetCriticalExtensions(criticalExtensions);

            cert.KeyUsage = new bool[] { true, true };

            NUnit.Framework.Assert.False(SignUtils.HasUnsupportedCriticalExtension(cert));
        }

        [NUnit.Framework.Test]
        public void BasicConstraintsSupportedTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            ISet criticalExtensions = new HashSet();

            criticalExtensions.Add(OID.X509Extensions.BASIC_CONSTRAINTS);

            cert.SetCriticalExtensions(criticalExtensions);

            NUnit.Framework.Assert.False(SignUtils.HasUnsupportedCriticalExtension(cert));
        }

        [NUnit.Framework.Test]
        public void ExtendedKeyUsageWithIdKpTimestampingTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            ISet criticalExtensions = new HashSet();

            criticalExtensions.Add(OID.X509Extensions.EXTENDED_KEY_USAGE);

            cert.SetCriticalExtensions(criticalExtensions);

            IList extendedKeyUsage = new List<string>();
            extendedKeyUsage.Add(OID.X509Extensions.ID_KP_TIMESTAMPING);

            cert.SetExtendedKeyUsage(extendedKeyUsage);

            NUnit.Framework.Assert.False(SignUtils.HasUnsupportedCriticalExtension(cert));
        }

        [NUnit.Framework.Test]
        public void ExtendedKeyUsageWithoutIdKpTimestampingTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            ISet criticalExtensions = new HashSet();

            criticalExtensions.Add(OID.X509Extensions.EXTENDED_KEY_USAGE);

            cert.SetCriticalExtensions(criticalExtensions);

            IList extendedKeyUsage = new List<string>();
            extendedKeyUsage.Add("Not ID KP TIMESTAMPING");

            cert.SetExtendedKeyUsage(extendedKeyUsage);

            NUnit.Framework.Assert.True(SignUtils.HasUnsupportedCriticalExtension(cert));
        }

        [NUnit.Framework.Test]
        public void IdKpTimestampingWithoutExtendedKeyUsageTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            IList extendedKeyUsage = new List<string>();
            extendedKeyUsage.Add(OID.X509Extensions.ID_KP_TIMESTAMPING);

            cert.SetExtendedKeyUsage(extendedKeyUsage);

            NUnit.Framework.Assert.False(SignUtils.HasUnsupportedCriticalExtension(cert));
        }

        [NUnit.Framework.Test]
        public void NotSupportedOIDTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            ISet criticalExtensions = new HashSet();

            criticalExtensions.Add("Totally not supported OID");

            cert.SetCriticalExtensions(criticalExtensions);

            NUnit.Framework.Assert.True(SignUtils.HasUnsupportedCriticalExtension(cert));
        }

        [NUnit.Framework.Test]
        public void CertificateIsNullTest()
        {
            NUnit.Framework.Assert.That(() => {
                SignUtils.HasUnsupportedCriticalExtension(null);
            }, NUnit.Framework.Throws.TypeOf<ArgumentException>());;
        }
    }
}
