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
