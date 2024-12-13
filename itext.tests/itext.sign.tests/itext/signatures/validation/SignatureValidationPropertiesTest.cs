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
using iText.Commons.Utils;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Extensions;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("UnitTest")]
    public class SignatureValidationPropertiesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetParametersValueForSpecificTest() {
            SignatureValidationProperties sut = new SignatureValidationProperties();
            sut.SetParameterValueFor(ValidatorContexts.Of(ValidatorContext.OCSP_VALIDATOR, ValidatorContext.CRL_VALIDATOR
                , ValidatorContext.SIGNATURE_VALIDATOR).GetSet(), CertificateSources.Of(CertificateSource.CRL_ISSUER, 
                CertificateSource.SIGNER_CERT, CertificateSource.TIMESTAMP).GetSet(), TimeBasedContexts.Of(TimeBasedContext
                .HISTORICAL).GetSet(), new SignatureValidationPropertiesTest.IncrementalFreshnessValueSetter(10, 1).GetAction
                ());
            // test the last value added
            NUnit.Framework.Assert.AreEqual(TimeSpan.FromDays(18), sut.GetParametersValueFor(ValidatorContext.SIGNATURE_VALIDATOR
                , CertificateSource.TIMESTAMP, TimeBasedContext.HISTORICAL, ((p) => p.GetFreshness())));
            //test the fifth value added
            NUnit.Framework.Assert.AreEqual(TimeSpan.FromDays(14), sut.GetParametersValueFor(ValidatorContext.CRL_VALIDATOR
                , CertificateSource.SIGNER_CERT, TimeBasedContext.HISTORICAL, ((p) => p.GetFreshness())));
            // test the general default
            NUnit.Framework.Assert.AreEqual(SignatureValidationProperties.DEFAULT_FRESHNESS_HISTORICAL, sut.GetParametersValueFor
                (ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource.OCSP_ISSUER, TimeBasedContext.HISTORICAL
                , ((p) => p.GetFreshness())));
        }

        [NUnit.Framework.Test]
        public virtual void GetParametersValueForDefaultTest() {
            SignatureValidationProperties sut = new SignatureValidationProperties();
            sut.SetParameterValueFor(ValidatorContexts.Of(ValidatorContext.OCSP_VALIDATOR, ValidatorContext.CRL_VALIDATOR
                , ValidatorContext.SIGNATURE_VALIDATOR).GetSet(), CertificateSources.Of(CertificateSource.CRL_ISSUER, 
                CertificateSource.SIGNER_CERT, CertificateSource.TIMESTAMP).GetSet(), TimeBasedContexts.Of(TimeBasedContext
                .HISTORICAL).GetSet(), new SignatureValidationPropertiesTest.IncrementalFreshnessValueSetter(10, 1).GetAction
                ());
            // test the general default
            NUnit.Framework.Assert.AreEqual(SignatureValidationProperties.DEFAULT_FRESHNESS_PRESENT_OCSP, sut.GetParametersValueFor
                (ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource.OCSP_ISSUER, TimeBasedContext.PRESENT
                , ((p) => p.GetFreshness())));
        }

        [NUnit.Framework.Test]
        public virtual void SetDefaultAsLastShouldOverrideAll() {
            SignatureValidationProperties sut = new SignatureValidationProperties();
            sut.SetParameterValueFor(ValidatorContexts.Of(ValidatorContext.OCSP_VALIDATOR, ValidatorContext.CRL_VALIDATOR
                , ValidatorContext.SIGNATURE_VALIDATOR).GetSet(), CertificateSources.Of(CertificateSource.CRL_ISSUER, 
                CertificateSource.SIGNER_CERT, CertificateSource.TIMESTAMP).GetSet(), TimeBasedContexts.Of(TimeBasedContext
                .HISTORICAL).GetSet(), (p) => p.SetFreshness(TimeSpan.FromDays(15)));
            sut.SetParameterValueFor(ValidatorContexts.All().GetSet(), CertificateSources.All().GetSet(), TimeBasedContexts
                .All().GetSet(), (p) => p.SetFreshness(TimeSpan.FromDays(25)));
            // test the general default
            NUnit.Framework.Assert.AreEqual(TimeSpan.FromDays(25), sut.GetParametersValueFor(ValidatorContext.OCSP_VALIDATOR
                , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT, ((p) => p.GetFreshness())));
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetFreshnessTest() {
            SignatureValidationProperties sut = new SignatureValidationProperties();
            sut.SetFreshness(ValidatorContexts.Of(ValidatorContext.CRL_VALIDATOR), CertificateSources.Of(CertificateSource
                .CERT_ISSUER), TimeBasedContexts.Of(TimeBasedContext.HISTORICAL), TimeSpan.FromDays(-10));
            NUnit.Framework.Assert.AreEqual(TimeSpan.FromDays(-10), sut.GetFreshness(new ValidationContext(ValidatorContext
                .CRL_VALIDATOR, CertificateSource.CERT_ISSUER, TimeBasedContext.HISTORICAL)));
            NUnit.Framework.Assert.AreEqual(SignatureValidationProperties.DEFAULT_FRESHNESS_PRESENT_CRL, sut.GetFreshness
                (new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.CERT_ISSUER, TimeBasedContext
                .PRESENT)));
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetContinueAfterFailure() {
            SignatureValidationProperties sut = new SignatureValidationProperties();
            sut.SetContinueAfterFailure(ValidatorContexts.Of(ValidatorContext.SIGNATURE_VALIDATOR), CertificateSources
                .Of(CertificateSource.CERT_ISSUER), true);
            sut.SetContinueAfterFailure(ValidatorContexts.Of(ValidatorContext.SIGNATURE_VALIDATOR), CertificateSources
                .Of(CertificateSource.OCSP_ISSUER), false);
            NUnit.Framework.Assert.AreEqual(true, sut.GetContinueAfterFailure(new ValidationContext(ValidatorContext.SIGNATURE_VALIDATOR
                , CertificateSource.CERT_ISSUER, TimeBasedContext.PRESENT)));
            NUnit.Framework.Assert.AreEqual(false, sut.GetContinueAfterFailure(new ValidationContext(ValidatorContext.
                SIGNATURE_VALIDATOR, CertificateSource.OCSP_ISSUER, TimeBasedContext.PRESENT)));
        }

        [NUnit.Framework.Test]
        public virtual void SetRevocationOnlineFetchingTest() {
            SignatureValidationProperties sut = new SignatureValidationProperties();
            sut.SetRevocationOnlineFetching(ValidatorContexts.Of(ValidatorContext.CRL_VALIDATOR), CertificateSources.All
                (), TimeBasedContexts.Of(TimeBasedContext.PRESENT), SignatureValidationProperties.OnlineFetching.ALWAYS_FETCH
                );
            NUnit.Framework.Assert.AreEqual(SignatureValidationProperties.DEFAULT_ONLINE_FETCHING, sut.GetRevocationOnlineFetching
                (new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.OCSP_ISSUER, TimeBasedContext
                .HISTORICAL)));
            NUnit.Framework.Assert.AreEqual(SignatureValidationProperties.DEFAULT_ONLINE_FETCHING, sut.GetRevocationOnlineFetching
                (new ValidationContext(ValidatorContext.OCSP_VALIDATOR, CertificateSource.OCSP_ISSUER, TimeBasedContext
                .PRESENT)));
            NUnit.Framework.Assert.AreEqual(SignatureValidationProperties.OnlineFetching.ALWAYS_FETCH, sut.GetRevocationOnlineFetching
                (new ValidationContext(ValidatorContext.CRL_VALIDATOR, CertificateSource.OCSP_ISSUER, TimeBasedContext
                .PRESENT)));
        }

        [NUnit.Framework.Test]
        public virtual void SetRequiredExtensionsTest() {
            SignatureValidationProperties sut = new SignatureValidationProperties();
            sut.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.SingletonList<CertificateExtension
                >(new KeyUsageExtension(1)));
            sut.SetRequiredExtensions(CertificateSources.Of(CertificateSource.CERT_ISSUER), JavaCollectionsUtil.SingletonList
                <CertificateExtension>(new KeyUsageExtension(2)));
            sut.SetRequiredExtensions(CertificateSources.Of(CertificateSource.OCSP_ISSUER), JavaCollectionsUtil.SingletonList
                <CertificateExtension>(new KeyUsageExtension(3)));
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.SingletonList(new KeyUsageExtension(1)), sut.GetRequiredExtensions
                (new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource.SIGNER_CERT, TimeBasedContext
                .PRESENT)));
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.SingletonList(new KeyUsageExtension(2)), sut.GetRequiredExtensions
                (new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource.CERT_ISSUER, TimeBasedContext
                .PRESENT)));
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.SingletonList(new KeyUsageExtension(3)), sut.GetRequiredExtensions
                (new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource.OCSP_ISSUER, TimeBasedContext
                .HISTORICAL)));
        }

        [NUnit.Framework.Test]
        public virtual void AddRequiredExtensionsTest() {
            SignatureValidationProperties sut = new SignatureValidationProperties();
            sut.AddRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.SingletonList<CertificateExtension
                >(new KeyUsageExtension(1)));
            sut.AddRequiredExtensions(CertificateSources.Of(CertificateSource.CRL_ISSUER), JavaCollectionsUtil.SingletonList
                <CertificateExtension>(new KeyUsageExtension(2)));
            IList<CertificateExtension> expectedExtensionsSigner = JavaCollectionsUtil.SingletonList<CertificateExtension
                >(new KeyUsageExtension(1));
            IList<CertificateExtension> expectedExtensionsCrlIssuer = new List<CertificateExtension>();
            expectedExtensionsCrlIssuer.Add(new KeyUsageExtension(KeyUsage.CRL_SIGN));
            expectedExtensionsCrlIssuer.Add(new KeyUsageExtension(1));
            expectedExtensionsCrlIssuer.Add(new KeyUsageExtension(2));
            NUnit.Framework.Assert.AreEqual(expectedExtensionsSigner, sut.GetRequiredExtensions(new ValidationContext(
                ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT)
                ));
            NUnit.Framework.Assert.AreEqual(expectedExtensionsCrlIssuer, sut.GetRequiredExtensions(new ValidationContext
                (ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource.CRL_ISSUER, TimeBasedContext.HISTORICAL
                )));
        }

        private class IncrementalFreshnessValueSetter {
            private readonly int increment;

            private int value;

            public IncrementalFreshnessValueSetter(int initialValue, int increment) {
                this.value = initialValue;
                this.increment = increment;
            }

            public virtual Action<SignatureValidationProperties.ContextProperties> GetAction() {
                return (p) => {
                    p.SetFreshness(TimeSpan.FromDays(value));
                    value += increment;
                }
                ;
            }
        }
    }
}
