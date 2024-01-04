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
namespace iText.Kernel.Pdf {
    /// <summary>Represents the line dash pattern.</summary>
    /// <remarks>
    /// Represents the line dash pattern. The line dash pattern shall control the pattern
    /// of dashes and gaps used to stroke paths. It shall be specified by a dash, gap and
    /// a dash phase.
    /// </remarks>
    public class PdfDashPattern {
        /// <summary>This is the length of a dash.</summary>
        private float dash = -1;

        /// <summary>This is the length of a gap.</summary>
        private float gap = -1;

        /// <summary>This is the phase.</summary>
        private float phase = -1;

        /// <summary>Creates a new line dash pattern.</summary>
        public PdfDashPattern() {
        }

        /// <summary>Creates a new line dash pattern.</summary>
        /// <param name="dash">length of dash</param>
        public PdfDashPattern(float dash) {
            this.dash = dash;
        }

        /// <summary>Creates a new line dash pattern.</summary>
        /// <param name="dash">length of dash</param>
        /// <param name="gap">length of gap</param>
        public PdfDashPattern(float dash, float gap) {
            this.dash = dash;
            this.gap = gap;
        }

        /// <summary>Creates a new line dash pattern.</summary>
        /// <param name="dash">length of dash</param>
        /// <param name="gap">length of gap</param>
        /// <param name="phase">this is the phase</param>
        public PdfDashPattern(float dash, float gap, float phase)
            : this(dash, gap) {
            this.phase = phase;
        }

        /// <summary>Gets dash of PdfDashPattern.</summary>
        /// <returns>float value.</returns>
        public virtual float GetDash() {
            return dash;
        }

        /// <summary>Gets gap of PdfDashPattern.</summary>
        /// <returns>float value.</returns>
        public virtual float GetGap() {
            return gap;
        }

        /// <summary>Gets phase of PdfDashPattern.</summary>
        /// <returns>float value.</returns>
        public virtual float GetPhase() {
            return phase;
        }
    }
}
