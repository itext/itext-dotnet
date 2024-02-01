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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Signatures.Exceptions;

namespace iText.Signatures {
    /// <summary>An exception that is thrown when something is wrong with a certificate.</summary>
    public class VerificationException : AbstractGeneralSecurityException {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        /// <summary>Creates a VerificationException.</summary>
        /// <param name="cert">is a failed certificate</param>
        /// <param name="message">is a reason of failure</param>
        public VerificationException(IX509Certificate cert, String message)
            : base(MessageFormatUtil.Format(SignExceptionMessageConstant.CERTIFICATE_TEMPLATE_FOR_EXCEPTION_MESSAGE, cert
                 == null ? "Unknown" : BOUNCY_CASTLE_FACTORY.CreateX500Name((IX509Certificate)cert).ToString(), message
                )) {
        }
    }
}
