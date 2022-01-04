/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using iText.Commons.Utils;

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>
    /// This class encapsulates information about the standard structure namespaces and provides some utility methods
    /// connected to them.
    /// </summary>
    /// <remarks>
    /// This class encapsulates information about the standard structure namespaces and provides some utility methods
    /// connected to them. The main purpose of this class is to determine if the given role in the specified namespace
    /// belongs to the standard or known domain-specific namespace.
    /// <para />
    /// See ISO 32000-2 14.8.6, "Standard structure namespaces"
    /// </remarks>
    public sealed class StandardNamespaces {
        private static readonly ICollection<String> STD_STRUCT_NAMESPACE_1_7_TYPES;

        private static readonly ICollection<String> STD_STRUCT_NAMESPACE_2_0_TYPES;

        // other namespaces
        private const String MATH_ML = "http://www.w3.org/1998/Math/MathML";

        /// <summary>Specifies the name of the standard structure namespace for PDF 1.7</summary>
        public const String PDF_1_7 = "http://iso.org/pdf/ssn";

        /// <summary>Specifies the name of the standard structure namespace for PDF 2.0</summary>
        public const String PDF_2_0 = "http://iso.org/pdf2/ssn";

        static StandardNamespaces() {
            STD_STRUCT_NAMESPACE_1_7_TYPES = JavaCollectionsUtil.UnmodifiableSet(new HashSet<String>(JavaUtil.ArraysAsList
                (StandardRoles.DOCUMENT, StandardRoles.PART, StandardRoles.DIV, StandardRoles.P, StandardRoles.H, StandardRoles
                .H1, StandardRoles.H2, StandardRoles.H3, StandardRoles.H4, StandardRoles.H5, StandardRoles.H6, StandardRoles
                .LBL, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.RUBY
                , StandardRoles.RB, StandardRoles.RT, StandardRoles.RP, StandardRoles.WARICHU, StandardRoles.WT, StandardRoles
                .WP, StandardRoles.L, StandardRoles.LI, StandardRoles.LBODY, StandardRoles.TABLE, StandardRoles.TR, StandardRoles
                .TH, StandardRoles.TD, StandardRoles.THEAD, StandardRoles.TBODY, StandardRoles.TFOOT, StandardRoles.CAPTION
                , StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.SECT, StandardRoles.ART, StandardRoles.BLOCKQUOTE
                , StandardRoles.TOC, StandardRoles.TOCI, StandardRoles.INDEX, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE
                , StandardRoles.QUOTE, StandardRoles.NOTE, StandardRoles.REFERENCE, StandardRoles.BIBENTRY, StandardRoles
                .CODE)));
            STD_STRUCT_NAMESPACE_2_0_TYPES = JavaCollectionsUtil.UnmodifiableSet(new HashSet<String>(JavaUtil.ArraysAsList
                (StandardRoles.DOCUMENT, StandardRoles.DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.SECT, StandardRoles
                .NONSTRUCT, StandardRoles.DIV, StandardRoles.ASIDE, StandardRoles.TITLE, StandardRoles.SUB, StandardRoles
                .P, StandardRoles.H, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles
                .LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.RB, StandardRoles.RT
                , StandardRoles.RP, StandardRoles.WARICHU, StandardRoles.WT, StandardRoles.WP, StandardRoles.FENOTE, StandardRoles
                .L, StandardRoles.LI, StandardRoles.LBODY, StandardRoles.TABLE, StandardRoles.TR, StandardRoles.TH, StandardRoles
                .TD, StandardRoles.THEAD, StandardRoles.TBODY, StandardRoles.TFOOT, StandardRoles.CAPTION, StandardRoles
                .FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT)));
        }

        // Hn, this type is handled in roleBelongsToStandardNamespace method
        /// <summary>Gets the name of the default standard structure namespace.</summary>
        /// <remarks>
        /// Gets the name of the default standard structure namespace. When a namespace is not
        /// explicitly specified for a given structure element or attribute, it shall be assumed to be within this
        /// default standard structure namespace. According to ISO 32000-2 default namespace is
        /// <see cref="PDF_1_7"/>.
        /// </remarks>
        /// <returns>the name of the default standard structure namespace.</returns>
        public static String GetDefault() {
            return PDF_1_7;
        }

        /// <summary>
        /// Checks if the given namespace is identified as the one that is common within broad ranges of documents types
        /// and doesn't require a role mapping for it's roles.
        /// </summary>
        /// <param name="namespace">a namespace to be checked, whether it defines a namespace of the known domain specific language.
        ///     </param>
        /// <returns>
        /// true, if the given
        /// <see cref="PdfNamespace"/>
        /// belongs to the domain-specific namespace, false otherwise.
        /// </returns>
        public static bool IsKnownDomainSpecificNamespace(PdfNamespace @namespace) {
            return MATH_ML.Equals(@namespace.GetNamespaceName());
        }

        /// <summary>Checks if the given role is considered standard in the specified standard namespace.</summary>
        /// <param name="role">a role to be checked if it is standard in the given standard structure namespace.</param>
        /// <param name="standardNamespaceName">
        /// a
        /// <see cref="System.String"/>
        /// identifying standard structure namespace against which given role
        /// will be checked.
        /// </param>
        /// <returns>
        /// false if the given role doesn't belong to the standard roles of the given standard structure namespace or
        /// if the given namespace name is not standard; true otherwise.
        /// </returns>
        public static bool RoleBelongsToStandardNamespace(String role, String standardNamespaceName) {
            if (PDF_1_7.Equals(standardNamespaceName)) {
                return STD_STRUCT_NAMESPACE_1_7_TYPES.Contains(role);
            }
            else {
                if (PDF_2_0.Equals(standardNamespaceName)) {
                    return STD_STRUCT_NAMESPACE_2_0_TYPES.Contains(role) || IsHnRole(role);
                }
            }
            return false;
        }

        /// <summary>Checks if the given role matches the Hn role pattern.</summary>
        /// <remarks>
        /// Checks if the given role matches the Hn role pattern. To match this pattern, the given role
        /// shall always consist of the uppercase letter "H" and one or more digits, representing an unsigned integer
        /// greater than or equal to 1, without leading zeroes or any other prefix or postfix.
        /// </remarks>
        /// <param name="role">
        /// a
        /// <see cref="System.String"/>
        /// that specifies a role to be checked against Hn role pattern.
        /// </param>
        /// <returns>true if the role matches, false otherwise.</returns>
        public static bool IsHnRole(String role) {
            if (role.StartsWith("H") && role.Length > 1 && role[1] != '0') {
                try {
                    return Convert.ToInt32(role.JSubstring(1, role.Length), System.Globalization.CultureInfo.InvariantCulture)
                         > 0;
                }
                catch (Exception) {
                }
            }
            // ignored
            return false;
        }
    }
}
