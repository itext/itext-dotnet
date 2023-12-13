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

namespace iText.Kernel {
    /// <summary>Describes an iText 7 add on.</summary>
    /// <remarks>
    /// Describes an iText 7 add on. An add on should register itself to a PdfDocument object if it
    /// wants to be included in the debugging information.
    /// </remarks>
    public class ProductInfo {
        private String name;

        private int major;

        private int minor;

        private int patch;

        private bool snapshot;

        /// <summary>Instantiates a ProductInfo object.</summary>
        /// <param name="name">name of the add on</param>
        /// <param name="major">major version of the add on</param>
        /// <param name="minor">minor version of the add on</param>
        /// <param name="patch">patch number of the add on</param>
        /// <param name="snapshot">whether the version of this add on is a snapshot build or not</param>
        public ProductInfo(String name, int major, int minor, int patch, bool snapshot) {
            this.name = name;
            this.major = major;
            this.minor = minor;
            this.patch = patch;
            this.snapshot = snapshot;
        }

        public virtual String GetName() {
            return name;
        }

        public virtual int GetMajor() {
            return major;
        }

        public virtual int GetMinor() {
            return minor;
        }

        public virtual int GetPatch() {
            return patch;
        }

        public virtual bool IsSnapshot() {
            return snapshot;
        }

        public override String ToString() {
            return name + "-" + major + "." + minor + "." + patch + (snapshot ? "-SNAPSHOT" : "");
        }
    }
}
