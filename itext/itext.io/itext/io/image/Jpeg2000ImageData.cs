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
using System.Collections.Generic;
using iText.Commons.Logs;

namespace iText.IO.Image {
    /// <summary>Image data and parsed metadata for a JPEG 2000 image.</summary>
    public class Jpeg2000ImageData : ImageData {
        /// <summary>Holds metadata parsed from a JPEG 2000 codestream or JP2 container.</summary>
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

        /// <summary>Represents a JPEG 2000 color specification box.</summary>
        /// <remarks>
        /// Represents a JPEG 2000 color specification box.
        /// <para />
        /// The first four list values are the method, precedence, approximation, and enumerated color space.
        /// </remarks>
        public class ColorSpecBox : List<int> {
            private byte[] colorProfile;

            /// <summary>Gets the color-specification method.</summary>
            /// <returns>
            /// method value stored at index
            /// <c>0</c>
            /// </returns>
            public virtual int GetMeth() {
                return (int)this[0];
            }

            /// <summary>Gets the color-specification precedence.</summary>
            /// <returns>
            /// precedence value stored at index
            /// <c>1</c>
            /// </returns>
            public virtual int GetPrec() {
                return (int)this[1];
            }

            /// <summary>Gets the color-specification approximation.</summary>
            /// <returns>
            /// approximation value stored at index
            /// <c>2</c>
            /// </returns>
            public virtual int GetApprox() {
                return (int)this[2];
            }

            /// <summary>Gets the enumerated color space.</summary>
            /// <returns>
            /// color-space value stored at index
            /// <c>3</c>
            /// </returns>
            public virtual int GetEnumCs() {
                return (int)this[3];
            }

            /// <summary>Gets the embedded color profile.</summary>
            /// <returns>
            /// retained profile bytes, or
            /// <see langword="null"/>
            /// </returns>
            public virtual byte[] GetColorProfile() {
                return colorProfile;
            }

//\cond DO_NOT_DOCUMENT
            internal virtual void SetColorProfile(byte[] colorProfile) {
                this.colorProfile = colorProfile;
            }
//\endcond
        }

        /// <summary>
        /// Parsed JPEG 2000 parameters, or
        /// <see langword="null"/>
        /// before processing.
        /// </summary>
        protected internal Jpeg2000ImageData.Parameters parameters;

        /// <summary>Creates JPEG 2000 image data to be loaded from a URL.</summary>
        /// <param name="url">
        /// source URL, not
        /// <see langword="null"/>
        /// </param>
        protected internal Jpeg2000ImageData(Uri url)
            : base(url, ImageType.JPEG2000) {
        }

        /// <summary>Creates JPEG 2000 image data from encoded bytes.</summary>
        /// <param name="bytes">encoded JPEG 2000 bytes; the array is retained</param>
        protected internal Jpeg2000ImageData(byte[] bytes)
            : base(bytes, ImageType.JPEG2000) {
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <see langword="false"/>
        /// , because JPEG 2000 images require a JPXDecode filter
        /// </returns>
        public override bool CanImageBeInline() {
            LazyLogger logger = new LazyLogger(typeof(ImageData));
            logger.Warn(() => iText.IO.Logs.IoLogMessageConstant.IMAGE_HAS_JPXDECODE_FILTER);
            return false;
        }

        /// <summary>Gets metadata parsed from the JPEG 2000 image.</summary>
        /// <returns>
        /// parsed parameters, or
        /// <see langword="null"/>
        /// before processing
        /// </returns>
        public virtual Jpeg2000ImageData.Parameters GetParameters() {
            return parameters;
        }
    }
}
