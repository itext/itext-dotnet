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
