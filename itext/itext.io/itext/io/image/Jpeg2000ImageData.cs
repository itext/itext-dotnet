/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
