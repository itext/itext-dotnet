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
using Org.BouncyCastle.X509;

namespace iText.Kernel.Crypto.Securityhandler {
    public class PublicKeyRecipient {
        private X509Certificate certificate = null;

        private int permission = 0;

        protected internal byte[] cms = null;

        public PublicKeyRecipient(X509Certificate certificate, int permission) {
            this.certificate = certificate;
            this.permission = permission;
        }

        public virtual X509Certificate GetCertificate() {
            return certificate;
        }

        public virtual int GetPermission() {
            return permission;
        }

        protected internal virtual void SetCms(byte[] cms) {
            this.cms = cms;
        }

        protected internal virtual byte[] GetCms() {
            return cms;
        }
    }
}
