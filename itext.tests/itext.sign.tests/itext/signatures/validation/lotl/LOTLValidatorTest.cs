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
using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Lotl {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    [iText.Commons.Utils.NoopAnnotation]
    public class LotlValidatorTest : ExtendedITextTest {
        private static readonly String SOURCE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/lotl/LOTLValidatorTest/";

        [NUnit.Framework.Test]
        public virtual void ValidationTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.WithLOTLFetchingProperties(new LotlFetchingProperties());
            LotlValidator validator = chainBuilder.GetLotlValidator();
            ValidationReport report = validator.Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
            IList<CountryServiceContext> trustedCertificates = validator.GetNationalTrustedCertificates();
            NUnit.Framework.Assert.IsFalse(trustedCertificates.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void LotlWithConfiguredSchemaNamesTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties();
            lotlFetchingProperties.AddSchemaName("HU");
            lotlFetchingProperties.AddSchemaName("EE");
            chainBuilder.WithLOTLFetchingProperties(lotlFetchingProperties);
            LotlValidator validator = chainBuilder.GetLotlValidator();
            ValidationReport report = validator.Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
            IList<CountryServiceContext> trustedCertificates = validator.GetNationalTrustedCertificates();
            NUnit.Framework.Assert.IsFalse(trustedCertificates.IsEmpty());
            // Assuming Estonian and Hungarian LOTL files don't have more than a thousand certificates.
            NUnit.Framework.Assert.IsTrue(trustedCertificates.Count < 1000);
        }

        [NUnit.Framework.Test]
        public virtual void LotlWithInvalidSchemaNameTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            LotlFetchingProperties lotlFetchingProperties = new LotlFetchingProperties();
            lotlFetchingProperties.AddSchemaName("Invalid");
            chainBuilder.WithLOTLFetchingProperties(lotlFetchingProperties);
            LotlValidator validator = chainBuilder.GetLotlValidator();
            ValidationReport report = validator.Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
            IList<CountryServiceContext> trustedCertificates = validator.GetNationalTrustedCertificates();
            NUnit.Framework.Assert.IsTrue(trustedCertificates.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void LotlUnavailableTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.WithLOTLFetchingProperties(new LotlFetchingProperties());
            chainBuilder.WithLOTLValidator(() => new _LOTLValidator_102(chainBuilder));
            ValidationReport report = chainBuilder.GetLotlValidator().Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasLogItem((l) => l.WithCheckName(LotlValidator.LOTL_VALIDATION).WithMessage(LotlValidator.UNABLE_TO_RETRIEVE_LOTL
                )));
        }

        private sealed class _LOTLValidator_102 : LotlValidator {
            public _LOTLValidator_102(ValidatorChainBuilder baseArg1)
                : base(baseArg1) {
            }

            protected internal override byte[] GetLotlBytes() {
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void EuJournalCertificatesEmptyTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.WithLOTLFetchingProperties(new LotlFetchingProperties());
            chainBuilder.WithLOTLValidator(() => new _LOTLValidator_121(chainBuilder));
            ValidationReport report = chainBuilder.GetLotlValidator().Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (2).HasLogItem((l) => l.WithCheckName(LotlValidator.LOTL_VALIDATION).WithMessage(LotlValidator.LOTL_VALIDATION_UNSUCCESSFUL
                )));
        }

        private sealed class _LOTLValidator_121 : LotlValidator {
            public _LOTLValidator_121(ValidatorChainBuilder baseArg1)
                : base(baseArg1) {
            }

            protected internal override IList<IX509Certificate> GetEUJournalCertificates(ValidationReport report) {
                return JavaCollectionsUtil.EmptyList<IX509Certificate>();
            }
        }

        [NUnit.Framework.Test]
        public virtual void LotlWithBrokenPivotsTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.WithLOTLFetchingProperties(new LotlFetchingProperties());
            chainBuilder.WithLOTLValidator(() => new _LOTLValidator_140(chainBuilder));
            ValidationReport report = chainBuilder.GetLotlValidator().Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasLogItem((l) => l.WithCheckName(LotlValidator.LOTL_VALIDATION).WithMessage(LotlValidator.UNABLE_TO_RETRIEVE_PIVOT
                , (t) => "https://ec.europa.eu/tools/lotl/eu-lotl-pivot-335-BrokenUri.xml")));
        }

        private sealed class _LOTLValidator_140 : LotlValidator {
            public _LOTLValidator_140(ValidatorChainBuilder baseArg1)
                : base(baseArg1) {
            }

            protected internal override byte[] GetLotlBytes() {
                try {
                    return File.ReadAllBytes(System.IO.Path.Combine(LotlValidatorTest.SOURCE + "eu-lotl-withBrokenPivot.xml"));
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
            }
        }
    }
}
