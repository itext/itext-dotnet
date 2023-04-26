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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;

namespace iText.IO.Image {
    public class Jpeg2000ImageData : ImageData {
        public class Parameters {
            public int numOfComps;

            public IList<Jpeg2000ImageData.ColorSpecBox> colorSpecBoxes = null;

            public bool isJp2 = false;

            public bool isJpxBaseline = false;

            public byte[] bpcBoxData;
        }

        public class ColorSpecBox : List<int> {
            private byte[] colorProfile;

            public virtual int GetMeth() {
                return (int)this[0];
            }

            public virtual int GetPrec() {
                return (int)this[1];
            }

            public virtual int GetApprox() {
                return (int)this[2];
            }

            public virtual int GetEnumCs() {
                return (int)this[3];
            }

            public virtual byte[] GetColorProfile() {
                return colorProfile;
            }

            internal virtual void SetColorProfile(byte[] colorProfile) {
                this.colorProfile = colorProfile;
            }
        }

        protected internal Jpeg2000ImageData.Parameters parameters;

        protected internal Jpeg2000ImageData(Uri url)
            : base(url, ImageType.JPEG2000) {
        }

        protected internal Jpeg2000ImageData(byte[] bytes)
            : base(bytes, ImageType.JPEG2000) {
        }

        public override bool CanImageBeInline() {
            ILogger logger = ITextLogManager.GetLogger(typeof(ImageData));
            logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.IMAGE_HAS_JPXDECODE_FILTER);
            return false;
        }

        public virtual Jpeg2000ImageData.Parameters GetParameters() {
            return parameters;
        }
    }
}
