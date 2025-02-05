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
using iText.Commons.Utils;

namespace iText.Signatures.Validation.Context {
    /// <summary>Validation context class, which encapsulates specific context values, related to validation process.
    ///     </summary>
    public class ValidationContext {
        private readonly CertificateSource certificateSource;

        private readonly ValidatorContext validatorContext;

        private readonly TimeBasedContext timeBasedContext;

        private iText.Signatures.Validation.Context.ValidationContext previousValidationContext;

        /// <summary>
        /// Create
        /// <see cref="ValidationContext"/>
        /// instance using provided context values.
        /// </summary>
        /// <param name="validatorContext">
        /// 
        /// <see cref="ValidatorContext"/>
        /// value
        /// </param>
        /// <param name="certificateSource">
        /// 
        /// <see cref="CertificateSource"/>
        /// value
        /// </param>
        /// <param name="timeBasedContext">
        /// 
        /// <see cref="TimeBasedContext"/>
        /// value
        /// </param>
        public ValidationContext(ValidatorContext validatorContext, CertificateSource certificateSource, TimeBasedContext
             timeBasedContext) {
            this.validatorContext = validatorContext;
            this.certificateSource = certificateSource;
            this.timeBasedContext = timeBasedContext;
        }

//\cond DO_NOT_DOCUMENT
        internal ValidationContext(ValidatorContext validatorContext, CertificateSource certificateSource, TimeBasedContext
             timeBasedContext, iText.Signatures.Validation.Context.ValidationContext previousValidationContext)
            : this(validatorContext, certificateSource, timeBasedContext) {
            this.previousValidationContext = previousValidationContext;
        }
//\endcond

        /// <summary>Get previous validation context instance, from which this instance was created.</summary>
        /// <returns>
        /// previous
        /// <see cref="ValidatorContext"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Context.ValidationContext GetPreviousValidationContext() {
            return previousValidationContext;
        }

        /// <summary>Get specific certificate source context value.</summary>
        /// <returns>
        /// 
        /// <see cref="CertificateSource"/>
        /// context value
        /// </returns>
        public virtual CertificateSource GetCertificateSource() {
            return certificateSource;
        }

        /// <summary>
        /// Create new
        /// <see cref="ValidationContext"/>
        /// instance with the provided certificate source context value.
        /// </summary>
        /// <param name="certificateSource">
        /// 
        /// <see cref="CertificateSource"/>
        /// value
        /// </param>
        /// <returns>
        /// new
        /// <see cref="ValidationContext"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Context.ValidationContext SetCertificateSource(CertificateSource
             certificateSource) {
            return new iText.Signatures.Validation.Context.ValidationContext(validatorContext, certificateSource, timeBasedContext
                , this);
        }

        /// <summary>Get specific time-based context value.</summary>
        /// <returns>
        /// 
        /// <see cref="TimeBasedContext"/>
        /// context value
        /// </returns>
        public virtual TimeBasedContext GetTimeBasedContext() {
            return timeBasedContext;
        }

        /// <summary>
        /// Create new
        /// <see cref="ValidationContext"/>
        /// instance with the provided certificate source context value.
        /// </summary>
        /// <param name="timeBasedContext">
        /// 
        /// <see cref="TimeBasedContext"/>
        /// value
        /// </param>
        /// <returns>
        /// new
        /// <see cref="ValidationContext"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Context.ValidationContext SetTimeBasedContext(TimeBasedContext 
            timeBasedContext) {
            return new iText.Signatures.Validation.Context.ValidationContext(validatorContext, certificateSource, timeBasedContext
                , this);
        }

        /// <summary>Get specific validator context value.</summary>
        /// <returns>
        /// 
        /// <see cref="ValidatorContext"/>
        /// context value
        /// </returns>
        public virtual ValidatorContext GetValidatorContext() {
            return validatorContext;
        }

        /// <summary>
        /// Create new
        /// <see cref="ValidationContext"/>
        /// instance with the provided certificate source context value.
        /// </summary>
        /// <param name="validatorContext">
        /// 
        /// <see cref="ValidatorContext"/>
        /// value
        /// </param>
        /// <returns>
        /// new
        /// <see cref="ValidationContext"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Context.ValidationContext SetValidatorContext(ValidatorContext 
            validatorContext) {
            return new iText.Signatures.Validation.Context.ValidationContext(validatorContext, certificateSource, timeBasedContext
                , this);
        }

        /// <summary>
        /// Check if validation contexts chain contains specific
        /// <see cref="CertificateSource"/>
        /// value.
        /// </summary>
        /// <param name="context">
        /// 
        /// <see cref="ValidationContext"/>
        /// instance to start the check from
        /// </param>
        /// <param name="source">
        /// 
        /// <see cref="CertificateSource"/>
        /// value to check
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if validation contexts chain contains provided certificate source,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool CheckIfContextChainContainsCertificateSource(iText.Signatures.Validation.Context.ValidationContext
             context, CertificateSource source) {
            if (context == null) {
                return false;
            }
            if (source == context.GetCertificateSource()) {
                return true;
            }
            return CheckIfContextChainContainsCertificateSource(context.GetPreviousValidationContext(), source);
        }

        /// <summary>
        /// Return string representation of this
        /// <see cref="ValidationContext"/>.
        /// </summary>
        /// <remarks>
        /// Return string representation of this
        /// <see cref="ValidationContext"/>.
        /// Previous validation context is not a part of this representation.
        /// </remarks>
        /// <returns>
        /// a string representation of the
        /// <see cref="ValidationContext"/>
        /// </returns>
        public override String ToString() {
            return "ValidationContext{" + "certificateSource=" + certificateSource + ", validatorContext=" + validatorContext
                 + ", timeBasedContext=" + timeBasedContext + '}';
        }

        /// <summary>Check if the provided object is equal to this one.</summary>
        /// <remarks>
        /// Check if the provided object is equal to this one.
        /// Previous validation context field is not taken into account during this comparison.
        /// </remarks>
        /// <param name="o">the reference object with which to compare</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if provided object is equal to this one,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Signatures.Validation.Context.ValidationContext that = (iText.Signatures.Validation.Context.ValidationContext
                )o;
            return certificateSource == that.certificateSource && validatorContext == that.validatorContext && timeBasedContext
                 == that.timeBasedContext;
        }

        /// <summary>Return a hash code value for this validation context.</summary>
        /// <remarks>
        /// Return a hash code value for this validation context.
        /// Previous validation context field is not taken into account during hash code calculation.
        /// </remarks>
        /// <returns>a hash code value for this validation context</returns>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)certificateSource, validatorContext, timeBasedContext);
        }
    }
}
