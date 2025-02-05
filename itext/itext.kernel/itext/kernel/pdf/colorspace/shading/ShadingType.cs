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
namespace iText.Kernel.Pdf.Colorspace.Shading {
    /// <summary>The constants of shading type (see ISO-320001 Table 78).</summary>
    public sealed class ShadingType {
        /// <summary>The int value of function-based shading type</summary>
        public const int FUNCTION_BASED = 1;

        /// <summary>The int value of axial shading type</summary>
        public const int AXIAL = 2;

        /// <summary>The int value of radial shading type</summary>
        public const int RADIAL = 3;

        /// <summary>The int value of free-form Gouraud-shaded triangle mesh shading type</summary>
        public const int FREE_FORM_GOURAUD_SHADED_TRIANGLE_MESH = 4;

        /// <summary>The int value of lattice-form Gouraud-shaded triangle mesh shading type</summary>
        public const int LATTICE_FORM_GOURAUD_SHADED_TRIANGLE_MESH = 5;

        /// <summary>The int value of coons patch meshes shading type</summary>
        public const int COONS_PATCH_MESH = 6;

        /// <summary>The int value of tensor-product patch meshes shading type</summary>
        public const int TENSOR_PRODUCT_PATCH_MESH = 7;

        private ShadingType() {
        }
    }
}
