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
using iText.Commons.Utils;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Colors {
    public class DeviceN : Color {
        public DeviceN(PdfSpecialCs.DeviceN cs)
            : this(cs, GetDefaultColorants(cs.GetNumberOfComponents())) {
        }

        public DeviceN(PdfSpecialCs.DeviceN cs, float[] value)
            : base(cs, value) {
        }

        /// <summary>Creates a color in new DeviceN color space.</summary>
        /// <param name="names">the names oif the components</param>
        /// <param name="alternateCs">the alternate color space</param>
        /// <param name="tintTransform">the function to transform color to the alternate color space</param>
        /// <param name="value">the values for the components of this color</param>
        [System.ObsoleteAttribute(@"Use constructor DeviceN(System.Collections.Generic.IList{E}, iText.Kernel.Pdf.Colorspace.PdfColorSpace, iText.Kernel.Pdf.Function.IPdfFunction, float[]) DeviceN} instead."
            )]
        public DeviceN(IList<String> names, PdfColorSpace alternateCs, PdfFunction tintTransform, float[] value)
            : this(new PdfSpecialCs.DeviceN(names, alternateCs, tintTransform), value) {
        }

        /// <summary>Creates a color in a new DeviceN color space.</summary>
        /// <param name="names">the names oif the components</param>
        /// <param name="alternateCs">the alternate color space</param>
        /// <param name="tintTransform">the function to transform color to the alternate color space</param>
        /// <param name="value">the values for the components of this color</param>
        public DeviceN(IList<String> names, PdfColorSpace alternateCs, IPdfFunction tintTransform, float[] value)
            : this(new PdfSpecialCs.DeviceN(names, alternateCs, tintTransform), value) {
        }

        private static float[] GetDefaultColorants(int numOfColorants) {
            float[] colorants = new float[numOfColorants];
            JavaUtil.Fill(colorants, 1f);
            return colorants;
        }
    }
}
