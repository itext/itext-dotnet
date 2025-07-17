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
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    [iText.Commons.Utils.NoopAnnotation]
    public class LOTLValidatorTest : ExtendedITextTest {
        private static readonly String SOURCE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/LOTLValidatorTest/";

        [NUnit.Framework.Test]
        public virtual void ValidationTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            LOTLValidator validator = chainBuilder.GetLotlValidator();
            ValidationReport report = validator.Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0));
            IList<IServiceContext> trustedCertificates = validator.GetNationalTrustedCertificates();
            NUnit.Framework.Assert.IsFalse(trustedCertificates.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void LotlUnavailableTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.WithLOTLValidator(() => new _LOTLValidator_63(chainBuilder));
            ValidationReport report = chainBuilder.GetLotlValidator().Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasLogItem((l) => l.WithCheckName(LOTLValidator.LOTL_VALIDATION).WithMessage(LOTLValidator.UNABLE_TO_RETRIEVE_LOTL
                )));
        }

        private sealed class _LOTLValidator_63 : LOTLValidator {
            public _LOTLValidator_63(ValidatorChainBuilder baseArg1)
                : base(baseArg1) {
            }

            protected internal override byte[] GetLotlBytes() {
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void EuJournalCertificatesEmptyTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.WithLOTLValidator(() => new _LOTLValidator_81(chainBuilder));
            ValidationReport report = chainBuilder.GetLotlValidator().Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (2).HasLogItem((l) => l.WithCheckName(LOTLValidator.LOTL_VALIDATION).WithMessage(LOTLValidator.LOTL_VALIDATION_UNSUCCESSFUL
                )));
        }

        private sealed class _LOTLValidator_81 : LOTLValidator {
            public _LOTLValidator_81(ValidatorChainBuilder baseArg1)
                : base(baseArg1) {
            }

            protected internal override IList<IX509Certificate> GetEUJournalCertificates(ValidationReport report) {
                return JavaCollectionsUtil.EmptyList<IX509Certificate>();
            }
        }

        [NUnit.Framework.Test]
        public virtual void LotlWithBrokenPivotsTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.WithLOTLValidator(() => new _LOTLValidator_99(chainBuilder));
            ValidationReport report = chainBuilder.GetLotlValidator().Validate();
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasLogItem((l) => l.WithCheckName(LOTLValidator.LOTL_VALIDATION).WithMessage(LOTLValidator.UNABLE_TO_RETRIEVE_PIVOT
                , (t) => "https://ec.europa.eu/tools/lotl/eu-lotl-pivot-335-BrokenUri.xml")));
        }

        private sealed class _LOTLValidator_99 : LOTLValidator {
            public _LOTLValidator_99(ValidatorChainBuilder baseArg1)
                : base(baseArg1) {
            }

            protected internal override byte[] GetLotlBytes() {
                try {
                    return File.ReadAllBytes(System.IO.Path.Combine(LOTLValidatorTest.SOURCE + "eu-lotl-withBrokenPivot.xml"));
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
            }
        }
    }
}
