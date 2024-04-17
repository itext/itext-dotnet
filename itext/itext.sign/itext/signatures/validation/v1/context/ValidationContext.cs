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
