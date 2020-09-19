/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
namespace iText.Layout.Properties {
    /// <summary>Class to hold background-repeat property.</summary>
    public class BackgroundRepeat {
        private readonly bool repeatX;

        private readonly bool repeatY;

        /// <summary>
        /// Creates a new
        /// <see cref="BackgroundRepeat"/>
        /// instance.
        /// </summary>
        public BackgroundRepeat() {
            this.repeatX = true;
            this.repeatY = true;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BackgroundRepeat"/>
        /// instance.
        /// </summary>
        /// <param name="repeatX">whether the background repeats in the x dimension.</param>
        /// <param name="repeatY">whether the background repeats in the y dimension.</param>
        public BackgroundRepeat(bool repeatX, bool repeatY) {
            this.repeatX = repeatX;
            this.repeatY = repeatY;
        }

        /// <summary>Is repeatX is true.</summary>
        /// <returns>repeatX value</returns>
        public virtual bool IsRepeatX() {
            return repeatX;
        }

        /// <summary>Is repeatY is true.</summary>
        /// <returns>repeatY value</returns>
        public virtual bool IsRepeatY() {
            return repeatY;
        }
    }
}
