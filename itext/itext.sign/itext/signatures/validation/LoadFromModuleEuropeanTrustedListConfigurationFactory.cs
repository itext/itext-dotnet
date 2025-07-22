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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Eutrustedlistsresources;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;

namespace iText.Signatures.Validation {
//\cond DO_NOT_DOCUMENT
    internal class LoadFromModuleEuropeanTrustedListConfigurationFactory : EuropeanTrustedListConfigurationFactory {
//\cond DO_NOT_DOCUMENT
        internal EuropeanTrustedListConfiguration config;
//\endcond

        public LoadFromModuleEuropeanTrustedListConfigurationFactory() {
            config = new EuropeanTrustedListConfiguration();
        }

        public override String GetTrustedListUri() {
            return config.GetTrustedListUri();
        }

        public override String GetCurrentlySupportedPublication() {
            return config.GetCurrentlySupportedPublication();
        }

        public override IList<IX509Certificate> GetCertificates() {
            EuropeanTrustedCertificatesResourceLoader loader = new EuropeanTrustedCertificatesResourceLoader(config);
            return loader.LoadCertificates();
        }
    }
//\endcond
}