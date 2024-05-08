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
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Extensions;

namespace iText.Signatures.Validation.V1 {
    /// <summary>This mock class wrapper a real SignatureValidationProperties instance.</summary>
    /// <remarks>
    /// This mock class wrapper a real SignatureValidationProperties instance.
    /// It will track the calls made to it.
    /// You can override a response by adding it with the add{someproperty}Response methods.
    /// These will be served first, when there are no more responses left, the wrapped properties
    /// will be returned.
    /// </remarks>
    public class MockSignatureValidationProperties : SignatureValidationProperties {
        private readonly SignatureValidationProperties wrappedProperties;

        public IList<ValidationContext> continueAfterFailureCalls = new List<ValidationContext>();

        public IList<ValidationContext> freshnessCalls = new List<ValidationContext>();

        public IList<ValidationContext> requiredExtensionsCalls = new List<ValidationContext>();

        public IList<ValidationContext> revocationOnlineFetchingCalls = new List<ValidationContext>();

        private readonly IList<bool> continueAfterFailureResponses = new List<bool>();

        private int continueAfterFailureResponsesIndex = 0;

        private readonly IList<TimeSpan> freshnessResponses = new List<TimeSpan>();

        private int freshnessResponsesIndex = 0;

        private readonly IList<IList<CertificateExtension>> requiredExtensionsResponses = new List<IList<CertificateExtension
            >>();

        private int requiredExtensionsResponsesIndex = 0;

        private readonly IList<SignatureValidationProperties.OnlineFetching> revocationOnlineFetchingResponses = new 
            List<SignatureValidationProperties.OnlineFetching>();

        private int revocationOnlineFetchingResponsesIndex = 0;

        public MockSignatureValidationProperties(SignatureValidationProperties properties) {
            this.wrappedProperties = properties;
        }

        public override bool GetContinueAfterFailure(ValidationContext validationContext) {
            continueAfterFailureCalls.Add(validationContext);
            if (continueAfterFailureResponsesIndex < continueAfterFailureResponses.Count) {
                return continueAfterFailureResponses[continueAfterFailureResponsesIndex++];
            }
            return wrappedProperties.GetContinueAfterFailure(validationContext);
        }

        public override TimeSpan GetFreshness(ValidationContext validationContext) {
            freshnessCalls.Add(validationContext);
            if (freshnessResponsesIndex < freshnessResponses.Count) {
                return freshnessResponses[freshnessResponsesIndex++];
            }
            return wrappedProperties.GetFreshness(validationContext);
        }

        public override IList<CertificateExtension> GetRequiredExtensions(ValidationContext validationContext) {
            requiredExtensionsCalls.Add(validationContext);
            if (requiredExtensionsResponsesIndex < requiredExtensionsResponses.Count) {
                return requiredExtensionsResponses[requiredExtensionsResponsesIndex++];
            }
            return wrappedProperties.GetRequiredExtensions(validationContext);
        }

        public override SignatureValidationProperties.OnlineFetching GetRevocationOnlineFetching(ValidationContext
             validationContext) {
            revocationOnlineFetchingCalls.Add(validationContext);
            if (revocationOnlineFetchingResponsesIndex < revocationOnlineFetchingResponses.Count) {
                return revocationOnlineFetchingResponses[revocationOnlineFetchingResponsesIndex++];
            }
            return wrappedProperties.GetRevocationOnlineFetching(validationContext);
        }

        public virtual iText.Signatures.Validation.V1.MockSignatureValidationProperties AddContinueAfterFailureResponse
            (bool value) {
            continueAfterFailureResponses.Add(value);
            return this;
        }

        public virtual iText.Signatures.Validation.V1.MockSignatureValidationProperties AddFreshnessResponse(TimeSpan
             freshness) {
            freshnessResponses.Add(freshness);
            return this;
        }

        public virtual iText.Signatures.Validation.V1.MockSignatureValidationProperties AddRequiredExtensionsResponses
            (IList<CertificateExtension> requiredExtensions) {
            requiredExtensionsResponses.Add(requiredExtensions);
            return this;
        }

        public virtual iText.Signatures.Validation.V1.MockSignatureValidationProperties AddRevocationOnlineFetchingResponse
            (SignatureValidationProperties.OnlineFetching value) {
            revocationOnlineFetchingResponses.Add(value);
            return this;
        }
    }
}
