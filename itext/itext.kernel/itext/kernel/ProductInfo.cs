/*

This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
