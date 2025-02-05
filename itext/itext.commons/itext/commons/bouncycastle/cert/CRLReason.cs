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
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace iText.Commons.Bouncycastle.Cert
{
    /// <summary>
    /// The CRLReason enumeration specifies the reason that a certificate is revoked, as defined in RFC 5280: Internet X.509 Public Key Infrastructure Certificate and CRL Profile .
    /// </summary>
    public enum CRLReason
    {
        /// <summary>
        /// This reason indicates that it is unspecified as to why the
        /// certificate has been revoked.
        /// </summary>
        UNSPECIFIED,

        /// <summary>
        /// This reason indicates that it is known or suspected that the
        /// certificate subject's private key has been compromised. It applies
        /// to end-entity certificates only.
        /// <summary>
        KEY_COMPROMISE,

        /// <summary>
        ///This reason indicates that it is known or suspected that the
        ///certificate subject's private key has been compromised. It applies
        ///to certificate authority (CA) certificates only.
        /// </summary>
        CA_COMPROMISE,

        /// <summary>
        ///This reason indicates that the subject's name or other information
        ///has changed.
        /// </summary>
        AFFILIATION_CHANGED,

        /// <summary>
        ///This reason indicates that the certificate has been superseded.
        /// </summary>
        SUPERSEDED,

        /// <summary>
        ///This reason indicates that the certificate is no longer needed.
        /// </summary>
        CESSATION_OF_OPERATION,

        /// <summary>
        ///This reason indicates that the certificate has been put on hold.
        /// </summary>
        CERTIFICATE_HOLD,

        /// <summary>
        ///Unused reason.
        /// </summary>
        UNUSED,

        /// <summary>
        ///This reason indicates that the certificate was previously on hold
        ///and should be removed from the CRL. It is for use with delta CRLs.
        /// </summary>
        REMOVE_FROM_CRL,

        /// <summary>
        ///This reason indicates that the privileges granted to the subject of
        ///the certificate have been withdrawn.
        /// </summary>
        PRIVILEGE_WITHDRAWN,

        /// <summary>
        ///This reason indicates that it is known or suspected that the
        ///certificate subject's private key has been compromised. It applies
        ///to authority attribute (AA) certificates only.
        /// </summary>
        AA_COMPROMISE
    }
}
