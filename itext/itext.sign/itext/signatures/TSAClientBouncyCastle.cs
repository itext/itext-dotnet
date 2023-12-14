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
using Common.Logging;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.IO.Codec;
using iText.IO.Util;
using iText.Kernel;

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
        /// <summary>The default value for the hash algorithm</summary>
        public const String DEFAULTHASHALGORITHM = "SHA-256";

        /// <summary>The default value for the hash algorithm</summary>
        public const int DEFAULTTOKENSIZE = 4096;

        /// <summary>The Logger instance.</summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Signatures.TSAClientBouncyCastle));

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
        /// <param name="username">user(account) name</param>
        /// <param name="password">password</param>
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
            TimeStampRequestGenerator tsqGenerator = new TimeStampRequestGenerator();
            tsqGenerator.SetCertReq(true);
            if (tsaReqPolicy != null && tsaReqPolicy.Length > 0) {
                tsqGenerator.SetReqPolicy(tsaReqPolicy);
            }
            // tsqGenerator.setReqPolicy("1.3.6.1.4.1.601.10.3.1");
            BigInteger nonce = BigInteger.ValueOf(SystemUtil.GetTimeBasedSeed());
            TimeStampRequest request = tsqGenerator.Generate(new DerObjectIdentifier(DigestAlgorithms.GetAllowedDigest
                (digestAlgorithm)), imprint, nonce);
            byte[] requestBytes = request.GetEncoded();
            // Call the communications layer
            respBytes = GetTSAResponse(requestBytes);
            // Handle the TSA response
            TimeStampResponse response = new TimeStampResponse(respBytes);
            // validate communication level attributes (RFC 3161 PKIStatus)
            response.Validate(request);
            PkiFailureInfo failure = response.GetFailInfo();
            int value = (failure == null) ? 0 : failure.IntValue;
            if (value != 0) {
                // @todo: Translate value of 15 error codes defined by PKIFailureInfo to string
                throw new PdfException(PdfException.InvalidTsa1ResponseCode2).SetMessageParams(tsaURL, value.ToString());
            }
            // @todo: validate the time stap certificate chain (if we want
            //        assure we do not sign using an invalid timestamp).
            // extract just the time stamp token (removes communication status info)
            TimeStampToken tsToken = response.TimeStampToken;
            if (tsToken == null) {
                throw new PdfException(PdfException.Tsa1FailedToReturnTimeStampToken2).SetMessageParams(tsaURL, response.GetStatusString
                    ());
            }
            TimeStampTokenInfo tsTokenInfo = tsToken.TimeStampInfo;
            // to view details
            byte[] encoded = tsToken.GetEncoded();
            LOGGER.Info("Timestamp generated: " + tsTokenInfo.GenTime);
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
                respBytes = Convert.FromBase64String(iText.IO.Util.JavaUtil.GetStringForBytes(respBytes, "US-ASCII"));
            }
            return respBytes;
        }
    }
}
