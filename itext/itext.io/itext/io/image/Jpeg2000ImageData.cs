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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;

namespace iText.IO.Image {
    public class Jpeg2000ImageData : ImageData {
        public class Parameters {
            private int numOfComps;

            private IList<Jpeg2000ImageData.ColorSpecBox> colorSpecBoxes = null;

            private bool isJp2 = false;

            private bool isJpxBaseline = false;

            private byte[] bpcBoxData;

            /// <summary>Retrieves number of components of the object.</summary>
            /// <returns>number of components</returns>
            public virtual int GetNumOfComps() {
                return numOfComps;
            }

            /// <summary>Sets number of components of the object.</summary>
            /// <param name="numOfComps">number of components</param>
            public virtual void SetNumOfComps(int numOfComps) {
                this.numOfComps = numOfComps;
            }

            /// <summary>Retrieves the color spec boxes of the object.</summary>
            /// <returns>color spec boxes</returns>
            public virtual IList<Jpeg2000ImageData.ColorSpecBox> GetColorSpecBoxes() {
                return colorSpecBoxes;
            }

            /// <summary>Sets the color spec boxes of the object.</summary>
            /// <param name="colorSpecBoxes">color spec boxes</param>
            public virtual void SetColorSpecBoxes(IList<Jpeg2000ImageData.ColorSpecBox> colorSpecBoxes) {
                this.colorSpecBoxes = colorSpecBoxes;
            }

            /// <summary>Retrieves whether the object is a Jp2.</summary>
            /// <returns>true if it is a jp2, otherwise false</returns>
            public virtual bool IsJp2() {
                return isJp2;
            }

            /// <summary>Sets whether the object is a jp2.</summary>
            /// <param name="jp2">true is it is a jp2, otherwise false</param>
            public virtual void SetJp2(bool jp2) {
                isJp2 = jp2;
            }

            /// <summary>Retrieves whether jpx is baseline.</summary>
            /// <returns>true if jpx is baseline, false otherwise</returns>
            public virtual bool IsJpxBaseline() {
                return isJpxBaseline;
            }

            /// <summary>Sets whether jpx is baseline.</summary>
            /// <param name="jpxBaseline">true if jpx is baseline, false otherwise</param>
            public virtual void SetJpxBaseline(bool jpxBaseline) {
                isJpxBaseline = jpxBaseline;
            }

            /// <summary>Retrieves the bits per component of the box data.</summary>
            /// <returns>bits per component</returns>
            public virtual byte[] GetBpcBoxData() {
                return bpcBoxData;
            }

            /// <summary>Sets the bits per component of the box data.</summary>
            /// <param name="bpcBoxData">bits per component</param>
            public virtual void SetBpcBoxData(byte[] bpcBoxData) {
                this.bpcBoxData = bpcBoxData;
            }
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

//\cond DO_NOT_DOCUMENT
            internal virtual void SetColorProfile(byte[] colorProfile) {
                this.colorProfile = colorProfile;
            }
//\endcond
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
