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
using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Tagging;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// This class is used to identify standard structure role type based only on it's name for the sake of applying
    /// standard structure attributes.
    /// </summary>
    /// <remarks>
    /// This class is used to identify standard structure role type based only on it's name for the sake of applying
    /// standard structure attributes.
    /// <para />
    /// These types mostly resemble structure type levels in the pdf 1.7 specification, however they are not exact.
    /// In pdf 2.0 some of these types are not even present and moreover, specific roles with the same name might belong
    /// to different type levels depending on context (which consists of kids, parents and their types).
    /// <para />
    /// So, these types are mostly useful for the internal itext usage and are not backed by any spec. They are designed for
    /// the most part to return the value the most suitable and handy for the purposes of accessibility properties applying.
    /// <para />
    /// Here are the main reasons to leave these types as is for now, even after introducing of PDF 2.0:
    /// <list type="bullet">
    /// <item><description>Standard structure types for pdf 1.7 and 2.0 are very alike. There are some differences, like new/removed roles
    /// and attributes, however they are not used in current layout auto tagging mechanism.
    /// </description></item>
    /// <item><description>Differentiating  possible types for the same role based on the context is not supported at the moment.
    /// </description></item>
    /// </list>
    /// In general, the correct way to handle role types would be to have separate classes for every namespace that define type
    /// and apply attributes. However I believe, that for now it is not feasible at the moment to implement this approach.
    /// The right time to improve and replace this class might be when new roles and attributes (specific to the different standard structure namespaces)
    /// will be more widely used in the auto tagging mechanism by default, and also when may be there will be more known
    /// practical examples of utilizing standard structure attributes.
    /// </remarks>
    internal class AccessibleTypes {
//\cond DO_NOT_DOCUMENT
        internal static int Unknown = 0;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int Grouping = 1;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int BlockLevel = 2;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int InlineLevel = 3;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int Illustration = 4;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static ICollection<String> groupingRoles = new HashSet<String>();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static ICollection<String> blockLevelRoles = new HashSet<String>();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static ICollection<String> inlineLevelRoles = new HashSet<String>();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static ICollection<String> illustrationRoles = new HashSet<String>();
//\endcond

        static AccessibleTypes() {
            // Some tag roles are not in any of the sets that define types. Some - because we don't want to write any accessibility
            // properties for them, some - because they are ambiguous for different pdf versions or don't have any possible
            // properties to set at the moment.
            //        StandardStructureTypes.Document
            //        StandardStructureTypes.DocumentFragment
            //        StandardStructureTypes.Artifact
            //        StandardStructureTypes.THead
            //        StandardStructureTypes.TBody
            //        StandardStructureTypes.TFoot
            groupingRoles.Add(StandardRoles.PART);
            groupingRoles.Add(StandardRoles.ART);
            groupingRoles.Add(StandardRoles.SECT);
            groupingRoles.Add(StandardRoles.DIV);
            groupingRoles.Add(StandardRoles.BLOCKQUOTE);
            groupingRoles.Add(StandardRoles.CAPTION);
            groupingRoles.Add(StandardRoles.TOC);
            groupingRoles.Add(StandardRoles.TOCI);
            groupingRoles.Add(StandardRoles.INDEX);
            groupingRoles.Add(StandardRoles.NONSTRUCT);
            groupingRoles.Add(StandardRoles.PRIVATE);
            groupingRoles.Add(StandardRoles.ASIDE);
            blockLevelRoles.Add(StandardRoles.P);
            blockLevelRoles.Add(StandardRoles.H);
            blockLevelRoles.Add(StandardRoles.H1);
            blockLevelRoles.Add(StandardRoles.H2);
            blockLevelRoles.Add(StandardRoles.H3);
            blockLevelRoles.Add(StandardRoles.H4);
            blockLevelRoles.Add(StandardRoles.H5);
            blockLevelRoles.Add(StandardRoles.H6);
            // Hn type is handled separately in identifyType method
            blockLevelRoles.Add(StandardRoles.L);
            blockLevelRoles.Add(StandardRoles.LBL);
            blockLevelRoles.Add(StandardRoles.LI);
            blockLevelRoles.Add(StandardRoles.LBODY);
            blockLevelRoles.Add(StandardRoles.TABLE);
            blockLevelRoles.Add(StandardRoles.TR);
            blockLevelRoles.Add(StandardRoles.TH);
            blockLevelRoles.Add(StandardRoles.TD);
            blockLevelRoles.Add(StandardRoles.TITLE);
            blockLevelRoles.Add(StandardRoles.FENOTE);
            blockLevelRoles.Add(StandardRoles.SUB);
            blockLevelRoles.Add(StandardRoles.CAPTION);
            inlineLevelRoles.Add(StandardRoles.SPAN);
            inlineLevelRoles.Add(StandardRoles.QUOTE);
            inlineLevelRoles.Add(StandardRoles.NOTE);
            inlineLevelRoles.Add(StandardRoles.REFERENCE);
            inlineLevelRoles.Add(StandardRoles.BIBENTRY);
            inlineLevelRoles.Add(StandardRoles.CODE);
            inlineLevelRoles.Add(StandardRoles.LINK);
            inlineLevelRoles.Add(StandardRoles.ANNOT);
            inlineLevelRoles.Add(StandardRoles.RUBY);
            inlineLevelRoles.Add(StandardRoles.WARICHU);
            inlineLevelRoles.Add(StandardRoles.RB);
            inlineLevelRoles.Add(StandardRoles.RT);
            inlineLevelRoles.Add(StandardRoles.RP);
            inlineLevelRoles.Add(StandardRoles.WT);
            inlineLevelRoles.Add(StandardRoles.WP);
            inlineLevelRoles.Add(StandardRoles.EM);
            inlineLevelRoles.Add(StandardRoles.STRONG);
            illustrationRoles.Add(StandardRoles.FIGURE);
            illustrationRoles.Add(StandardRoles.FORMULA);
            illustrationRoles.Add(StandardRoles.FORM);
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Identifies standard structure role type based only on it's name.</summary>
        /// <remarks>
        /// Identifies standard structure role type based only on it's name. The return types might be one of the constants:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="Unknown"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="Grouping"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="BlockLevel"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="InlineLevel"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="Illustration"/>
        /// </description></item>
        /// </list>
        /// See also remarks in the
        /// <see cref="AccessibleTypes"/>
        /// class documentation.
        /// </remarks>
        internal static int IdentifyType(String role) {
            if (groupingRoles.Contains(role)) {
                return Grouping;
            }
            else {
                if (blockLevelRoles.Contains(role) || StandardNamespaces.IsHnRole(role)) {
                    return BlockLevel;
                }
                else {
                    if (inlineLevelRoles.Contains(role)) {
                        return InlineLevel;
                    }
                    else {
                        if (illustrationRoles.Contains(role)) {
                            return Illustration;
                        }
                    }
                }
            }
            return Unknown;
        }
//\endcond
    }
//\endcond
}
