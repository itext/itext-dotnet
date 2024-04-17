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
using iText.Test;

namespace iText.Signatures.Validation.V1.Context {
    [NUnit.Framework.Category("UnitTest")]
    public class ValidationContextTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestInitializingConstructor() {
            ValidationContext sut = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.HISTORICAL);
            NUnit.Framework.Assert.AreEqual(ValidatorContext.CRL_VALIDATOR, sut.GetValidatorContext());
            NUnit.Framework.Assert.AreEqual(CertificateSource.CERT_ISSUER, sut.GetCertificateSource());
            NUnit.Framework.Assert.AreEqual(TimeBasedContext.HISTORICAL, sut.GetTimeBasedContext());
        }

        [NUnit.Framework.Test]
        public virtual void TestSetAndGetCertificateSource() {
            ValidationContext sut = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.HISTORICAL);
            sut = sut.SetCertificateSource(CertificateSource.CRL_ISSUER);
            NUnit.Framework.Assert.AreEqual(CertificateSource.CRL_ISSUER, sut.GetCertificateSource());
        }

        [NUnit.Framework.Test]
        public virtual void TestSetAndGetTemporalContext() {
            ValidationContext sut = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.HISTORICAL);
            sut = sut.SetTimeBasedContext(TimeBasedContext.PRESENT);
            NUnit.Framework.Assert.AreEqual(TimeBasedContext.PRESENT, sut.GetTimeBasedContext());
        }

        [NUnit.Framework.Test]
        public virtual void TestSetAndGetValidator() {
            ValidationContext sut = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.HISTORICAL);
            sut = sut.SetValidatorContext(ValidatorContext.SIGNATURE_VALIDATOR);
            NUnit.Framework.Assert.AreEqual(ValidatorContext.SIGNATURE_VALIDATOR, sut.GetValidatorContext());
        }

        [NUnit.Framework.Test]
        public virtual void TestEquals() {
            ValidationContext sutA = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.HISTORICAL);
            ValidationContext sutB = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.HISTORICAL);
            NUnit.Framework.Assert.AreEqual(sutA, sutB);
            NUnit.Framework.Assert.AreEqual(sutB, sutA);
        }

        [NUnit.Framework.Test]
        public virtual void TestNotEquals() {
            ValidationContext sutA = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.HISTORICAL);
            ValidationContext sutB = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource
                .CERT_ISSUER, TimeBasedContext.HISTORICAL);
            ValidationContext sutC = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.OCSP_ISSUER
                , TimeBasedContext.HISTORICAL);
            ValidationContext sutD = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.PRESENT);
            NUnit.Framework.Assert.AreNotEqual(sutA, sutB);
            NUnit.Framework.Assert.AreNotEqual(sutB, sutA);
            NUnit.Framework.Assert.AreNotEqual(sutC, sutA);
            NUnit.Framework.Assert.AreNotEqual(sutD, sutA);
        }

        [NUnit.Framework.Test]
        public virtual void TestHashCode() {
            ValidationContext sut0 = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.HISTORICAL);
            ValidationContext sutA = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.HISTORICAL);
            ValidationContext sutB = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource
                .CERT_ISSUER, TimeBasedContext.HISTORICAL);
            ValidationContext sutC = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.OCSP_ISSUER
                , TimeBasedContext.HISTORICAL);
            ValidationContext sutD = new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.PRESENT);
            NUnit.Framework.Assert.AreEqual(sutA.GetHashCode(), sut0.GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(sutA.GetHashCode(), sutB.GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(sutA.GetHashCode(), sutC.GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(sutA.GetHashCode(), sutD.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeTest() {
            ValidationContext vc1 = new ValidationContext(ValidatorContext.OCSP_VALIDATOR, CertificateSource.OCSP_ISSUER
                , TimeBasedContext.HISTORICAL);
            ValidationContext vc2 = new ValidationContext(ValidatorContext.OCSP_VALIDATOR, CertificateSource.OCSP_ISSUER
                , TimeBasedContext.HISTORICAL);
            ValidationContext vc3 = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource
                .OCSP_ISSUER, TimeBasedContext.HISTORICAL);
            ValidationContext vc4 = new ValidationContext(ValidatorContext.OCSP_VALIDATOR, CertificateSource.CERT_ISSUER
                , TimeBasedContext.HISTORICAL);
            ValidationContext vc5 = new ValidationContext(ValidatorContext.OCSP_VALIDATOR, CertificateSource.OCSP_ISSUER
                , TimeBasedContext.PRESENT);
            NUnit.Framework.Assert.AreEqual(vc1.GetHashCode(), vc2.GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(vc1.GetHashCode(), vc3.GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(vc1.GetHashCode(), vc4.GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(vc1.GetHashCode(), vc5.GetHashCode());
        }
    }
}
