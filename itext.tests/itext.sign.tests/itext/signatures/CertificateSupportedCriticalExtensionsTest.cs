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
using iText.Signatures.Testutils;
using System;
using System.Collections;
using System.Collections.Generic;
using iText.Test;
using NUnit.Framework;

namespace iText.Signatures
{
    class CertificateSupportedCriticalExtensionsTest : ExtendedITextTest
    {
        [Test]
        public void SupportedCriticalOIDsTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            ISet<string> criticalExtensions = new HashSet<string>();

            criticalExtensions.Add(OID.X509Extensions.KEY_USAGE);
            criticalExtensions.Add(OID.X509Extensions.BASIC_CONSTRAINTS);

            cert.SetCriticalExtensions(criticalExtensions);

            cert.KeyUsage = new bool[] { true, true };

            Assert.False(CertificateVerification.HasUnsupportedCriticalExtension(cert));
        }

        [Test]
        public void BasicConstraintsSupportedTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            ISet<string> criticalExtensions = new HashSet<string>();

            criticalExtensions.Add(OID.X509Extensions.BASIC_CONSTRAINTS);

            cert.SetCriticalExtensions(criticalExtensions);

            Assert.False(CertificateVerification.HasUnsupportedCriticalExtension(cert));
        }

        [Test]
        public void ExtendedKeyUsageWithIdKpTimestampingTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            ISet<string> criticalExtensions = new HashSet<string>();

            criticalExtensions.Add(OID.X509Extensions.EXTENDED_KEY_USAGE);

            cert.SetCriticalExtensions(criticalExtensions);

            IList extendedKeyUsage = new List<string>();
            extendedKeyUsage.Add(OID.X509Extensions.ID_KP_TIMESTAMPING);

            cert.SetExtendedKeyUsage(extendedKeyUsage);

            Assert.False(CertificateVerification.HasUnsupportedCriticalExtension(cert));
        }

        [Test]
        public void ExtendedKeyUsageWithoutIdKpTimestampingTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            ISet<string> criticalExtensions = new HashSet<string>();

            criticalExtensions.Add(OID.X509Extensions.EXTENDED_KEY_USAGE);

            cert.SetCriticalExtensions(criticalExtensions);

            IList extendedKeyUsage = new List<string>();
            extendedKeyUsage.Add("Not ID KP TIMESTAMPING");

            cert.SetExtendedKeyUsage(extendedKeyUsage);

            Assert.False(CertificateVerification.HasUnsupportedCriticalExtension(cert));
        }

        [Test]
        public void IdKpTimestampingWithoutExtendedKeyUsageTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            IList extendedKeyUsage = new List<string>();
            extendedKeyUsage.Add(OID.X509Extensions.ID_KP_TIMESTAMPING);

            cert.SetExtendedKeyUsage(extendedKeyUsage);

            Assert.False(CertificateVerification.HasUnsupportedCriticalExtension(cert));
        }

        [Test]
        public void NotSupportedOIDTest()
        {
            X509MockCertificate cert = new X509MockCertificate();

            ISet<string> criticalExtensions = new HashSet<string>();

            criticalExtensions.Add("Totally not supported OID");

            cert.SetCriticalExtensions(criticalExtensions);

            Assert.True(CertificateVerification.HasUnsupportedCriticalExtension(cert));
        }

        [Test]
        public void CertificateIsNullTest()
        {
            Assert.Catch(typeof(ArgumentException), () => CertificateVerification.HasUnsupportedCriticalExtension(null));
        }
        
        [Test]
        public void CertificateHasNoExtensionsTest()
        {
            X509MockCertificate cert = new X509MockCertificate();
            
            cert.SetCriticalExtensions(null);

            Assert.False(CertificateVerification.HasUnsupportedCriticalExtension(cert));
        }
    }
}
