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
using Microsoft.Extensions.Logging;
using iText.Commons;

namespace iText.IO.Image {
    /// <summary>A no-op class for WebP image data handling.</summary>
    public sealed class NoWebPLoader : AbstractWebPLoader {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.IO.Image.NoWebPLoader));

//\cond DO_NOT_DOCUMENT
        /// <summary>Standard constructor.</summary>
        internal NoWebPLoader() {
        }
//\endcond

        /// <summary><inheritDoc/></summary>
        protected internal override ImageData GetImageData(byte[] bytes) {
            LOGGER.LogWarning(WebPLogMessageConstant.WEBP_NOT_FOUND);
            return null;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override ImageData GetImageData(Uri url) {
            LOGGER.LogWarning(WebPLogMessageConstant.WEBP_NOT_FOUND);
            return null;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsWebPSupported() {
            LOGGER.LogWarning(WebPLogMessageConstant.WEBP_NOT_FOUND);
            return false;
        }
    }
}
