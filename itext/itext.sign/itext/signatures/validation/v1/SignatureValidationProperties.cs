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
using iText.Commons.Utils.Collections;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Extensions;

namespace iText.Signatures.Validation.V1 {
    /// <summary>
    /// Class which stores properties, which are related to signature validation process.
    /// </summary>
    public class SignatureValidationProperties {
        public const bool DEFAULT_CONTINUE_AFTER_FAILURE = true;

        public static readonly TimeSpan DEFAULT_FRESHNESS_PRESENT_CRL = TimeSpan.FromDays(30);

        public static readonly TimeSpan DEFAULT_FRESHNESS_PRESENT_OCSP = TimeSpan.FromDays(30);

        public static readonly TimeSpan DEFAULT_FRESHNESS_HISTORICAL = TimeSpan.FromMinutes(1);

        public static readonly SignatureValidationProperties.OnlineFetching DEFAULT_ONLINE_FETCHING = SignatureValidationProperties.OnlineFetching
            .FETCH_IF_NO_OTHER_DATA_AVAILABLE;

        private readonly Dictionary<ValidationContext, SignatureValidationProperties.ContextProperties> properties
             = new Dictionary<ValidationContext, SignatureValidationProperties.ContextProperties>();

        /// <summary>
        /// Create <see cref="SignatureValidationProperties"/> with default values.
        /// </summary>
        public SignatureValidationProperties() {
            SetContinueAfterFailure(ValidatorContexts.All(), CertificateSources.All(), DEFAULT_CONTINUE_AFTER_FAILURE);
            SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.All(), DEFAULT_ONLINE_FETCHING
                );
            SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.Of(TimeBasedContext.HISTORICAL
                ), DEFAULT_FRESHNESS_HISTORICAL);
            SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.Of(TimeBasedContext.PRESENT
                ), DEFAULT_FRESHNESS_PRESENT_OCSP);
            SetFreshness(ValidatorContexts.Of(ValidatorContext.CRL_VALIDATOR), CertificateSources.All(), TimeBasedContexts
                .Of(TimeBasedContext.PRESENT), DEFAULT_FRESHNESS_PRESENT_CRL);
            SetRequiredExtensions(CertificateSources.Of(CertificateSource.CRL_ISSUER), JavaCollectionsUtil.SingletonList
                <CertificateExtension>(new KeyUsageExtension(KeyUsage.CRL_SIGN)));
            SetRequiredExtensions(CertificateSources.Of(CertificateSource.OCSP_ISSUER), JavaCollectionsUtil.SingletonList
                <CertificateExtension>(new ExtendedKeyUsageExtension(JavaCollectionsUtil.SingletonList<String>(ExtendedKeyUsageExtension
                .OCSP_SIGNING))));
            SetRequiredExtensions(CertificateSources.Of(CertificateSource.SIGNER_CERT), JavaCollectionsUtil.SingletonList
                <CertificateExtension>(new KeyUsageExtension(KeyUsage.NON_REPUDIATION)));
            IList<CertificateExtension> certIssuerRequiredExtensions = new List<CertificateExtension>();
            certIssuerRequiredExtensions.Add(new KeyUsageExtension(KeyUsage.KEY_CERT_SIGN));
            certIssuerRequiredExtensions.Add(new BasicConstraintsExtension(true));
            SetRequiredExtensions(CertificateSources.Of(CertificateSource.CERT_ISSUER), certIssuerRequiredExtensions);
            SetRequiredExtensions(CertificateSources.Of(CertificateSource.TIMESTAMP), JavaCollectionsUtil.SingletonList
                <CertificateExtension>(new ExtendedKeyUsageExtension(JavaCollectionsUtil.SingletonList<String>(ExtendedKeyUsageExtension
                .TIME_STAMPING))));
        }

        /// <summary>
        /// Returns the freshness setting for the provided validation context or the default context
        /// in milliseconds.
        /// </summary>
        /// <param name="validationContext">the validation context for which to retrieve the freshness setting</param>
        /// <returns>the freshness setting for the provided validation context or the default context in milliseconds</returns>
        public virtual TimeSpan GetFreshness(ValidationContext validationContext) {
            return this.GetParametersValueFor<TimeSpan>(validationContext.GetValidatorContext(), validationContext.GetCertificateSource
                (), validationContext.GetTimeBasedContext(), (p) => p.GetFreshness());
        }

        /// <summary>
        /// Sets the freshness setting for the specified validator,
        /// time based and certificate source contexts in milliseconds.
        /// This parameter specifies how old revocation data can be, compared to validation time, in order to be trustworthy.
        /// </summary>
        /// <param name="validatorContexts">the validators for which to apply the setting</param>
        /// <param name="certificateSources">the certificate sources to</param>
        /// <param name="timeBasedContexts">the date comparison context  for which to apply the setting</param>
        /// <param name="value">the settings value in milliseconds</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureValidationProperties"/>
        /// instance.
        /// </returns>
        public iText.Signatures.Validation.V1.SignatureValidationProperties SetFreshness(ValidatorContexts validatorContexts
            , CertificateSources certificateSources, TimeBasedContexts timeBasedContexts, TimeSpan value) {
            SetParameterValueFor(validatorContexts.GetSet(), certificateSources.GetSet(), timeBasedContexts.GetSet(), 
                (p) => p.SetFreshness(value));
            return this;
        }

        /// <summary>Returns the Continue after failure setting for the provided context or the default context.</summary>
        /// <param name="validationContext">the context for which to retrieve the Continue after failure setting</param>
        /// <returns>the Continue after failure setting for the provided context or the default context</returns>
        public virtual bool GetContinueAfterFailure(ValidationContext validationContext) {
            return this.GetParametersValueFor<bool>(validationContext.GetValidatorContext(), 
                validationContext.GetCertificateSource(), validationContext.GetTimeBasedContext(), 
                (p) => p.GetContinueAfterFailure().GetValueOrDefault());
        }

        /// <summary>
        /// Sets the Continue after failure setting for the provided context.
        /// This parameter specifies if validation is expected to continue after first failure is encountered.
        /// Only <see cref="ValidationResult#INVALID"/> is considered to be a failure.
        /// </summary>
        /// <param name="validatorContexts">the validators for which to set the Continue after failure setting</param>
        /// <param name="certificateSources">the certificateSources for which to set the Continue after failure setting
        ///     </param>
        /// <param name="value">the Continue after failure setting</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureValidationProperties"/>
        /// instance.
        /// </returns>
        public iText.Signatures.Validation.V1.SignatureValidationProperties SetContinueAfterFailure(ValidatorContexts
             validatorContexts, CertificateSources certificateSources, bool value) {
            SetParameterValueFor(validatorContexts.GetSet(), certificateSources.GetSet(), TimeBasedContexts.All().GetSet
                (), (p) => p.SetContinueAfterFailure(value));
            return this;
        }

        /// <summary>Sets the onlineFetching property representing possible online fetching permissions.</summary>
        /// <param name="validationContext">the context for which to retrieve the online fetching setting</param>
        /// <returns>the online fetching setting.</returns>
        public virtual SignatureValidationProperties.OnlineFetching GetRevocationOnlineFetching(ValidationContext 
            validationContext) {
            return this.GetParametersValueFor<SignatureValidationProperties.OnlineFetching>(validationContext.GetValidatorContext
                (), validationContext.GetCertificateSource(), validationContext.GetTimeBasedContext(), (p) => p.GetOnlineFetching
                ());
        }

        /// <summary>Sets the onlineFetching property representing possible online fetching permissions.</summary>
        /// <param name="validatorContexts">the validators for which to set this value</param>
        /// <param name="certificateSources">the certificate source for which to set this value</param>
        /// <param name="timeBasedContexts">time perspective context, at which validation is happening</param>
        /// <param name="onlineFetching">onlineFetching property value to set</param>
        /// <returns>
        /// this same
        /// <see cref="SignatureValidationProperties"/>
        /// instance.
        /// </returns>
        public iText.Signatures.Validation.V1.SignatureValidationProperties SetRevocationOnlineFetching(ValidatorContexts
             validatorContexts, CertificateSources certificateSources, TimeBasedContexts timeBasedContexts, SignatureValidationProperties.OnlineFetching
             onlineFetching) {
            SetParameterValueFor(validatorContexts.GetSet(), certificateSources.GetSet(), timeBasedContexts.GetSet(), 
                (p) => p.SetOnlineFetching(onlineFetching));
            return this;
        }

        /// <summary>Returns required extension for the provided validation context.</summary>
        /// <param name="validationContext">the validation context for which to retrieve required extensions</param>
        /// <returns>required extensions for the provided validation context</returns>
        public virtual IList<CertificateExtension> GetRequiredExtensions(ValidationContext validationContext) {
            return this.GetParametersValueFor<IList<CertificateExtension>>(validationContext.GetValidatorContext(), validationContext
                .GetCertificateSource(), validationContext.GetTimeBasedContext(), (p) => p.GetRequiredExtensions());
        }

        /// <summary>
        /// Set list of extensions which are required to be set to a certificate depending on certificate source.
        /// <para />
        /// By default, required extensions are set to be compliant with common validation norms.
        /// Changing those can result in falsely positive validation result.
        /// </summary>
        /// <param name="certificateSources"><see cref="CertificateSource"/> for extensions to be present</param>
        /// <param name="requiredExtensions">list of required <see cref="CertificateExtension"/></param>
        /// <returns>this same <see cref="SignatureValidationProperties"/> instance</returns>
        public iText.Signatures.Validation.V1.SignatureValidationProperties SetRequiredExtensions(CertificateSources
            certificateSources, IList<CertificateExtension> requiredExtensions) {
            // make a defensive copy of  requiredExtensions and already wrap it with unmodifiableList so that we don't have
            // to do this every time it is retrieved. Now we are protected against changes in th passed list and from
            // changes in the returned list
            IList<CertificateExtension> copy = JavaCollectionsUtil.UnmodifiableList<CertificateExtension>(new List<CertificateExtension
                >(requiredExtensions));
            SetParameterValueFor(ValidatorContexts.All().GetSet(), certificateSources.GetSet(), TimeBasedContexts.All(
                ).GetSet(), (p) => p.SetRequiredExtensions(copy));
            return this;
        }

        /// <summary>This method executes the setter method for every combination of selected validators and certificateSources
        ///     </summary>
        /// <param name="validatorContexts">the validators to execute the setter on</param>
        /// <param name="certificateSources">the certificate sources to execute the setter on</param>
        /// <param name="setter">the setter to execute</param>
        internal void SetParameterValueFor(EnumSet<ValidatorContext> validatorContexts, EnumSet<CertificateSource>
             certificateSources, EnumSet<TimeBasedContext> timeBasedContexts, Action<SignatureValidationProperties.ContextProperties
            > setter) {
            foreach (ValidatorContext validatorContext in validatorContexts) {
                foreach (CertificateSource certificateSource in certificateSources) {
                    foreach (TimeBasedContext timeBasedContext in timeBasedContexts) {
                        ValidationContext vc = new ValidationContext(validatorContext, certificateSource, timeBasedContext);
                        SignatureValidationProperties.ContextProperties cProperties = properties.ComputeIfAbsent(vc, (unused) => new 
                            SignatureValidationProperties.ContextProperties());
                        setter(cProperties);
                    }
                }
            }
        }

        /// <summary>
        /// This method executes the getter method to the most granular parameters set down until the getter returns
        /// a non-null value
        /// </summary>
        /// <param name="validatorContext">the validator for which the value is to be retrieved</param>
        /// <param name="certSource">the certificate source for which the value is to be retrieved</param>
        /// <param name="getter">the getter to get the value from the parameters set</param>
        /// <typeparam name="T">the type of the return value of this method and the getter method</typeparam>
        /// <returns>the first non-null value returned.</returns>
        internal virtual T GetParametersValueFor<T>(ValidatorContext validatorContext, CertificateSource certSource
            , TimeBasedContext timeBasedContext, Func<SignatureValidationProperties.ContextProperties, T> getter) {
            // all three match
            ValidationContext c = new ValidationContext(validatorContext, certSource, timeBasedContext);
            if (properties.ContainsKey(c)) {
                return getter.Invoke(properties.Get(c));
            }
            return default;
        }

        /// <summary>Enum representing possible online fetching permissions.</summary>
        public enum OnlineFetching {
            /// <summary>Permission to always fetch revocation data online.</summary>
            ALWAYS_FETCH,
            /// <summary>Permission to fetch revocation data online if no other revocation data available.</summary>
            FETCH_IF_NO_OTHER_DATA_AVAILABLE,
            /// <summary>Forbids fetching revocation data online.</summary>
            NEVER_FETCH
        }

        internal class ContextProperties {
            private TimeSpan freshness;

            private bool? continueAfterFailure;

            private SignatureValidationProperties.OnlineFetching onlineFetching;

            private IList<CertificateExtension> requiredExtensions;

            public ContextProperties() {
            }

            // Empty constructor.
            public virtual bool? GetContinueAfterFailure() {
                return continueAfterFailure;
            }

            public virtual void SetContinueAfterFailure(bool? continueAfterFailure) {
                this.continueAfterFailure = continueAfterFailure;
            }

            public virtual TimeSpan GetFreshness() {
                return freshness;
            }

            public virtual void SetFreshness(TimeSpan value) {
                freshness = value;
            }

            public virtual SignatureValidationProperties.OnlineFetching GetOnlineFetching() {
                return onlineFetching;
            }

            public virtual void SetOnlineFetching(SignatureValidationProperties.OnlineFetching onlineFetching) {
                this.onlineFetching = onlineFetching;
            }

            public virtual IList<CertificateExtension> GetRequiredExtensions() {
                return requiredExtensions;
            }

            public virtual void SetRequiredExtensions(IList<CertificateExtension> requiredExtensions) {
                this.requiredExtensions = requiredExtensions;
            }
        }
    }
}
