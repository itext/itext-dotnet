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

namespace iText.Test {
    public class RunnerSearchConfig {
        private IList<String> searchPackages = new List<String>();
        private IList<String> searchClasses = new List<String>();
        private IList<String> ignoredPaths = new List<String>();
        private bool isToMarkTestsWithoutAnnotationAsIgnored;

        /// <summary>
        /// Add namespace to search path which is checked for wrapped sample classes. 
        /// </summary>
        /// <param name="fullName">full name of namespace to be checked.</param>
        /// <returns>this RunnerSearchConfig</returns>

        public virtual RunnerSearchConfig AddPackageToRunnerSearchPath(String fullName) {
            searchPackages.Add(fullName);
            return this;
        }

        /// <summary>
        /// Add class to runner. 
        /// </summary>
        /// <param name="fullName">full name of class to be checked.</param>
        /// <returns>this RunnerSearchConfig</returns>
        public virtual RunnerSearchConfig AddClassToRunnerSearchPath(String fullName) {
            searchClasses.Add(fullName);
            return this;
        }

        /// <summary>
        /// Add namespace or class to ignore list. Items from this list won't be checked for wrapped sample classes. 
        /// </summary>
        /// <param name="name">full or partial name of the namespace or class to be omitted by this runner.
        /// <returns>this RunnerSearchConfig</returns>
        ///                 E.g. "Highlevel.Appendix" or "iText.Highlevel.Appendix.TableProperties".</param>
        /// <returns>this RunnerSearchConfig</returns>
        public virtual RunnerSearchConfig IgnorePackageOrClass(String name) {
            ignoredPaths.Add(name);
            return this;
        }
        
        public virtual IList<String> GetSearchPackages() { return searchPackages; }
        public virtual IList<String> GetSearchClasses() { return searchClasses; }
        public virtual IList<String> GetIgnoredPaths() { return ignoredPaths; }
    }
}
