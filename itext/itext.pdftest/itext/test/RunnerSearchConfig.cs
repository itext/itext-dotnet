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
