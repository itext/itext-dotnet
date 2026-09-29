/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Exceptions;

namespace iText.Bouncycastlefips
{
    public class UnsupportedEncryptionFeatureException : ITextException {
        public const String ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS =
            "Encryption with certificated is currently not supported in Bouncy Castle FIPS mode.";

        public const String PUBLIC_KEY_CREATION_ISNT_SUPPORTED_IN_FIPS =
            "Creation of public key is not supported by BouncyCastle-FIPS yet.";

        public const String RSA_SHA1_SIGNER_ISNT_SUPPORTED_IN_FIPS =
            "Creation of RSA Digest Signer with SHA-1 is not supported by BouncyCastle-FIPS.";

        public UnsupportedEncryptionFeatureException(string msg) : base(msg) {
        }
    }
}
