/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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

namespace iText.Forms.Xfdf {
    /// <summary>Represents the BorderStyleAlt element, a child of the link element.</summary>
    /// <remarks>
    /// Represents the BorderStyleAlt element, a child of the link element.
    /// Corresponds to the Border key in the common annotation dictionary.
    /// Content model: border style encoded in the format specified in the border style attributes.
    /// Required attributes: HCornerRadius, VCornerRadius, Width.
    /// Optional attributes: DashPattern.
    /// For more details see paragraph 6.5.3 in Xfdf document specification.
    /// For more details about attributes see paragraph 6.6.19 in Xfdf document specification.
    /// </remarks>
    public class BorderStyleAltObject {
        /// <summary>Number specifying the horizontal corner radius of the rectangular border.</summary>
        /// <remarks>
        /// Number specifying the horizontal corner radius of the rectangular border.
        /// Corresponds to array index 0 in the Border key in the common annotation dictionary.
        /// </remarks>
        private float hCornerRadius;

        /// <summary>Number specifying the vertical corner radius of the rectangular border.</summary>
        /// <remarks>
        /// Number specifying the vertical corner radius of the rectangular border.
        /// Corresponds to array index 1 in the Border key in the common annotation dictionary.
        /// </remarks>
        private float vCornerRadius;

        /// <summary>Number specifying the width of the rectangular border.</summary>
        /// <remarks>
        /// Number specifying the width of the rectangular border.
        /// Corresponds to array index 2 in the Border key in the common annotation dictionary.
        /// </remarks>
        private float width;

        /// <summary>An array of numbers specifying the pattern of dashes and gaps of the border.</summary>
        /// <remarks>
        /// An array of numbers specifying the pattern of dashes and gaps of the border.
        /// Corresponds to array index 3 in the Border key in the common annotation dictionary.
        /// </remarks>
        private float[] dashPattern;

        /// <summary>Encoded border style string.</summary>
        private String content;

        /// <summary>Creates an instance that encapsulates BorderStyleAlt XFDF element data.</summary>
        /// <param name="hCornerRadius">a float value specifying the horizontal corner radius of the rectangular border.
        ///     </param>
        /// <param name="vCornerRadius">a float value specifying the vertical corner radius of the rectangular border.
        ///     </param>
        /// <param name="width">a float value specifying the width of the rectangular border.</param>
        public BorderStyleAltObject(float hCornerRadius, float vCornerRadius, float width) {
            this.hCornerRadius = hCornerRadius;
            this.vCornerRadius = vCornerRadius;
            this.width = width;
        }

        /// <summary>Gets the horizontal corner radius of the rectangular border.</summary>
        /// <remarks>
        /// Gets the horizontal corner radius of the rectangular border.
        /// Corresponds to array index 0 in the Border key in the common annotation dictionary.
        /// </remarks>
        /// <returns>a float value specifying the horizontal corner radius.</returns>
        public virtual float GetHCornerRadius() {
            return hCornerRadius;
        }

        /// <summary>Gets the vertical corner radius of the rectangular border.</summary>
        /// <remarks>
        /// Gets the vertical corner radius of the rectangular border.
        /// Corresponds to array index 1 in the Border key in the common annotation dictionary.
        /// </remarks>
        /// <returns>a float value specifying the vertical corner radius.</returns>
        public virtual float GetVCornerRadius() {
            return vCornerRadius;
        }

        /// <summary>Gets the width of the rectangular border.</summary>
        /// <remarks>
        /// Gets the width of the rectangular border.
        /// Corresponds to array index 2 in the Border key in the common annotation dictionary.
        /// </remarks>
        /// <returns>a float value specifying the width of the border.</returns>
        public virtual float GetWidth() {
            return width;
        }

        /// <summary>Gets the dash pattern of the border.</summary>
        /// <remarks>
        /// Gets the dash pattern of the border.
        /// Corresponds to array index 3 in the Border key in the common annotation dictionary.
        /// </remarks>
        /// <returns>an array of numbers specifying the pattern of dashes and gaps of the border.</returns>
        public virtual float[] GetDashPattern() {
            return dashPattern;
        }

        /// <summary>Sets the dash pattern of the border.</summary>
        /// <remarks>
        /// Sets the dash pattern of the border.
        /// Corresponds to array index 3 in the Border key in the common annotation dictionary.
        /// </remarks>
        /// <param name="dashPattern">an array of numbers specifying the pattern of dashes and gaps of the border.</param>
        /// <returns>
        /// this
        /// <see cref="BorderStyleAltObject"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Xfdf.BorderStyleAltObject SetDashPattern(float[] dashPattern) {
            this.dashPattern = dashPattern;
            return this;
        }

        /// <summary>Gets border style.</summary>
        /// <returns>an encoded border style as string.</returns>
        public virtual String GetContent() {
            return content;
        }

        /// <summary>Sets border style.</summary>
        /// <param name="content">an encoded border style as string.</param>
        /// <returns>
        /// this
        /// <see cref="BorderStyleAltObject"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Xfdf.BorderStyleAltObject SetContent(String content) {
            this.content = content;
            return this;
        }
    }
}
