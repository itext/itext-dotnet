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
using iText.Commons.Utils;

namespace iText.Signatures.Validation.V1.Context {
    public class ValidationContext {
        private readonly CertificateSource certificateSource;

        private readonly ValidatorContext validatorContext;

        private readonly TimeBasedContext timeBasedContext;

        public ValidationContext(ValidatorContext validatorContext, CertificateSource certificateSource, TimeBasedContext
             timeBasedContext) {
            this.validatorContext = validatorContext;
            this.certificateSource = certificateSource;
            this.timeBasedContext = timeBasedContext;
        }

        public virtual CertificateSource GetCertificateSource() {
            return certificateSource;
        }

        public virtual iText.Signatures.Validation.V1.Context.ValidationContext SetCertificateSource(CertificateSource
             certificateSource) {
            return new iText.Signatures.Validation.V1.Context.ValidationContext(validatorContext, certificateSource, timeBasedContext
                );
        }

        public virtual TimeBasedContext GetTimeBasedContext() {
            return timeBasedContext;
        }

        public virtual iText.Signatures.Validation.V1.Context.ValidationContext SetTimeBasedContext(TimeBasedContext
             timeBasedContext) {
            return new iText.Signatures.Validation.V1.Context.ValidationContext(validatorContext, certificateSource, timeBasedContext
                );
        }

        public virtual ValidatorContext GetValidatorContext() {
            return validatorContext;
        }

        public virtual iText.Signatures.Validation.V1.Context.ValidationContext SetValidatorContext(ValidatorContext
             validatorContext) {
            return new iText.Signatures.Validation.V1.Context.ValidationContext(validatorContext, certificateSource, timeBasedContext
                );
        }

        public override String ToString() {
            return "ValidationContext{" + "certificateSource=" + certificateSource + ", validatorContext=" + validatorContext
                 + ", timeBasedContext=" + timeBasedContext + '}';
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Signatures.Validation.V1.Context.ValidationContext that = (iText.Signatures.Validation.V1.Context.ValidationContext
                )o;
            return certificateSource == that.certificateSource && validatorContext == that.validatorContext && timeBasedContext
                 == that.timeBasedContext;
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)certificateSource, validatorContext, timeBasedContext);
        }
    }
}
