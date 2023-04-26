/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Cmp;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;

namespace iText.Signatures {
    /// <summary>
    /// Time Stamp Authority Client interface implementation using Bouncy Castle
    /// org.bouncycastle.tsp package.
    /// </summary>
    /// <remarks>
    /// Time Stamp Authority Client interface implementation using Bouncy Castle
    /// org.bouncycastle.tsp package.
    /// <para />
    /// Created by Aiken Sam, 2006-11-15, refactored by Martin Brunecky, 07/15/2007
    /// for ease of subclassing.
    /// </remarks>
    public class TSAClientBouncyCastle : ITSAClient {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        /// <summary>The default value for the hash algorithm</summary>
        public const String DEFAULTHASHALGORITHM = "SHA-256";

        /// <summary>The default value for the hash algorithm</summary>
        public const int DEFAULTTOKENSIZE = 4096;

        /// <summary>The Logger instance.</summary>
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.TSAClientBouncyCastle
            ));

        /// <summary>URL of the Time Stamp Authority</summary>
        protected internal String tsaURL;

        /// <summary>TSA Username</summary>
        protected internal String tsaUsername;

        /// <summary>TSA password</summary>
        protected internal String tsaPassword;

        /// <summary>An interface that allows you to inspect the timestamp info.</summary>
        protected internal ITSAInfoBouncyCastle tsaInfo;

        /// <summary>Estimate of the received time stamp token</summary>
        protected internal int tokenSizeEstimate;

        /// <summary>Hash algorithm</summary>
        protected internal String digestAlgorithm;

        /// <summary>TSA request policy</summary>
        private String tsaReqPolicy;

        /// <summary>Creates an instance of a TSAClient that will use BouncyCastle.</summary>
        /// <param name="url">String - Time Stamp Authority URL (i.e. "http://tsatest1.digistamp.com/TSA")</param>
        public TSAClientBouncyCastle(String url)
            : this(url, null, null, DEFAULTTOKENSIZE, DEFAULTHASHALGORITHM) {
        }

        /// <summary>Creates an instance of a TSAClient that will use BouncyCastle.</summary>
        /// <param name="url">String - Time Stamp Authority URL (i.e. "http://tsatest1.digistamp.com/TSA")</param>
        /// <param name="username">String - user(account) name</param>
        /// <param name="password">String - password</param>
        public TSAClientBouncyCastle(String url, String username, String password)
            : this(url, username, password, 4096, DEFAULTHASHALGORITHM) {
        }

        /// <summary>Constructor.</summary>
        /// <remarks>
        /// Constructor.
        /// Note the token size estimate is updated by each call, as the token
        /// size is not likely to change (as long as we call the same TSA using
        /// the same imprint length).
        /// </remarks>
        /// <param name="url">Time Stamp Authority URL (i.e. "http://tsatest1.digistamp.com/TSA")</param>
        /// <param name="username">user(account) name, optional</param>
        /// <param name="password">
        /// password, optional if used in combination with username, the credentials will be used in
        /// basic authentication. Use only in combination with a https url to ensure encryption
        /// </param>
        /// <param name="tokSzEstimate">estimated size of received time stamp token (DER encoded)</param>
        /// <param name="digestAlgorithm">is a hash algorithm</param>
        public TSAClientBouncyCastle(String url, String username, String password, int tokSzEstimate, String digestAlgorithm
            ) {
            this.tsaURL = url;
            this.tsaUsername = username;
            this.tsaPassword = password;
            this.tokenSizeEstimate = tokSzEstimate;
            this.digestAlgorithm = digestAlgorithm;
        }

        /// <param name="tsaInfo">the tsaInfo to set</param>
        public virtual void SetTSAInfo(ITSAInfoBouncyCastle tsaInfo) {
            this.tsaInfo = tsaInfo;
        }

        /// <summary>Get the token size estimate.</summary>
        /// <remarks>
        /// Get the token size estimate.
        /// Returned value reflects the result of the last succesfull call, padded
        /// </remarks>
        /// <returns>an estimate of the token size</returns>
        public virtual int GetTokenSizeEstimate() {
            return tokenSizeEstimate;
        }

        /// <summary>Gets the TSA request policy that will be used when retrieving timestamp token.</summary>
        /// <returns>policy id, or <c>null</c> if not set</returns>
        public virtual String GetTSAReqPolicy() {
            return tsaReqPolicy;
        }

        /// <summary>Sets the TSA request policy that will be used when retrieving timestamp token.</summary>
        /// <param name="tsaReqPolicy">policy id</param>
        public virtual void SetTSAReqPolicy(String tsaReqPolicy) {
            this.tsaReqPolicy = tsaReqPolicy;
        }

        /// <summary>Gets the MessageDigest to digest the data imprint</summary>
        /// <returns>the digest algorithm name</returns>
        public virtual IDigest GetMessageDigest() {
            return SignUtils.GetMessageDigest(digestAlgorithm);
        }

        /// <summary>Get RFC 3161 timeStampToken.</summary>
        /// <remarks>
        /// Get RFC 3161 timeStampToken.
        /// Method may return null indicating that timestamp should be skipped.
        /// </remarks>
        /// <param name="imprint">data imprint to be time-stamped</param>
        /// <returns>encoded, TSA signed data of the timeStampToken</returns>
        public virtual byte[] GetTimeStampToken(byte[] imprint) {
            byte[] respBytes = null;
            // Setup the time stamp request
            ITimeStampRequestGenerator tsqGenerator = BOUNCY_CASTLE_FACTORY.CreateTimeStampRequestGenerator();
            tsqGenerator.SetCertReq(true);
            if (tsaReqPolicy != null && tsaReqPolicy.Length > 0) {
                tsqGenerator.SetReqPolicy(tsaReqPolicy);
            }
            // tsqGenerator.setReqPolicy("1.3.6.1.4.1.601.10.3.1");
            IBigInteger nonce = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateBigInteger().ValueOf
                (SystemUtil.GetTimeBasedSeed());
            ITimeStampRequest request = tsqGenerator.Generate(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(DigestAlgorithms
                .GetAllowedDigest(digestAlgorithm)), imprint, nonce);
            byte[] requestBytes = request.GetEncoded();
            // Call the communications layer
            respBytes = GetTSAResponse(requestBytes);
            // Handle the TSA response
            ITimeStampResponse response = BOUNCY_CASTLE_FACTORY.CreateTimeStampResponse(respBytes);
            // validate communication level attributes (RFC 3161 PKIStatus)
            response.Validate(request);
            IPkiFailureInfo failure = response.GetFailInfo();
            int value = failure.IsNull() ? 0 : failure.IntValue();
            if (value != 0) {
                // @todo: Translate value of 15 error codes defined by PKIFailureInfo to string
                throw new PdfException(SignExceptionMessageConstant.INVALID_TSA_RESPONSE).SetMessageParams(tsaURL, value.ToString
                    ());
            }
            // @todo: validate the time stap certificate chain (if we want
            //        assure we do not sign using an invalid timestamp).
            // extract just the time stamp token (removes communication status info)
            ITimeStampToken tsToken = response.GetTimeStampToken();
            if (tsToken == null) {
                throw new PdfException(SignExceptionMessageConstant.THIS_TSA_FAILED_TO_RETURN_TIME_STAMP_TOKEN).SetMessageParams
                    (tsaURL, response.GetStatusString());
            }
            ITimeStampTokenInfo tsTokenInfo = tsToken.GetTimeStampInfo();
            // to view details
            byte[] encoded = tsToken.GetEncoded();
            LOGGER.LogInformation("Timestamp generated: " + tsTokenInfo.GetGenTime());
            if (tsaInfo != null) {
                tsaInfo.InspectTimeStampTokenInfo(tsTokenInfo);
            }
            // Update our token size estimate for the next call (padded to be safe)
            this.tokenSizeEstimate = encoded.Length + 32;
            return encoded;
        }

        /// <summary>Get timestamp token - communications layer</summary>
        /// <param name="requestBytes">is a byte representation of TSA request</param>
        /// <returns>- byte[] - TSA response, raw bytes (RFC 3161 encoded)</returns>
        protected internal virtual byte[] GetTSAResponse(byte[] requestBytes) {
            // Setup the TSA connection
            SignUtils.TsaResponse response = SignUtils.GetTsaResponseForUserRequest(tsaURL, requestBytes, tsaUsername, 
                tsaPassword);
            // Get TSA response as a byte array
            Stream inp = response.tsaResponseStream;
            MemoryStream baos = new MemoryStream();
            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = inp.JRead(buffer, 0, buffer.Length)) >= 0) {
                baos.Write(buffer, 0, bytesRead);
            }
            byte[] respBytes = baos.ToArray();
            if (response.encoding != null && response.encoding.ToLowerInvariant().Equals("base64".ToLowerInvariant())) {
                respBytes = Convert.FromBase64String(iText.Commons.Utils.JavaUtil.GetStringForBytes(respBytes, "US-ASCII")
                    );
            }
            return respBytes;
        }
    }
}
