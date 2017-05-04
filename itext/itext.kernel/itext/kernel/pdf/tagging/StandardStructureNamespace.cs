using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>
    /// This class encapsulates information about the standard structure namespaces and provides some utility methods
    /// connected to them.
    /// </summary>
    /// <remarks>
    /// This class encapsulates information about the standard structure namespaces and provides some utility methods
    /// connected to them. The main purpose of this class is to determine if the given role in the specified namespace
    /// belongs to the standard or known domain-specific namespace.
    /// <p>See ISO 32000-2 14.8.6, "Standard structure namespaces"</p>
    /// </remarks>
    public sealed class StandardStructureNamespace {
        private static readonly ICollection<PdfName> STD_STRUCT_NAMESPACE_1_7_TYPES;

        private static readonly ICollection<PdfName> STD_STRUCT_NAMESPACE_2_0_TYPES;

        private const String MATH_ML = "http://www.w3.org/1998/Math/MathML";

        /// <summary>Specifies the name of the standard structure namespace for PDF 1.7</summary>
        public const String PDF_1_7 = "http://iso.org/pdf/ssn";

        /// <summary>Specifies the name of the standard structure namespace for PDF 2.0</summary>
        public const String PDF_2_0 = "http://iso.org/pdf2/ssn";

        static StandardStructureNamespace() {
            // other namespaces
            STD_STRUCT_NAMESPACE_1_7_TYPES = new HashSet<PdfName>(iText.IO.Util.JavaUtil.ArraysAsList(PdfName.Document
                , PdfName.Part, PdfName.Div, PdfName.P, PdfName.H, PdfName.H1, PdfName.H2, PdfName.H3, PdfName.H4, PdfName
                .H5, PdfName.H6, PdfName.Lbl, PdfName.Span, PdfName.Link, PdfName.Annot, PdfName.Form, PdfName.Ruby, PdfName
                .RB, PdfName.RT, PdfName.RP, PdfName.Warichu, PdfName.WT, PdfName.WP, PdfName.L, PdfName.LI, PdfName.LBody
                , PdfName.Table, PdfName.TR, PdfName.TH, PdfName.TD, PdfName.THead, PdfName.TBody, PdfName.TFoot, PdfName
                .Caption, PdfName.Figure, PdfName.Formula, PdfName.Sect, PdfName.Art, PdfName.BlockQuote, PdfName.TOC, 
                PdfName.TOCI, PdfName.Index, PdfName.NonStruct, PdfName.Private, PdfName.Quote, PdfName.Note, PdfName.
                Reference, PdfName.BibEntry, PdfName.Code));
            STD_STRUCT_NAMESPACE_2_0_TYPES = new HashSet<PdfName>(iText.IO.Util.JavaUtil.ArraysAsList(PdfName.Document
                , PdfName.DocumentFragment, PdfName.Part, PdfName.Div, PdfName.Aside, PdfName.Title, PdfName.Sub, PdfName
                .P, PdfName.H, PdfName.Lbl, PdfName.Em, PdfName.Strong, PdfName.Span, PdfName.Link, PdfName.Annot, PdfName
                .Form, PdfName.Ruby, PdfName.RB, PdfName.RT, PdfName.RP, PdfName.Warichu, PdfName.WT, PdfName.WP, PdfName
                .FENote, PdfName.L, PdfName.LI, PdfName.LBody, PdfName.Table, PdfName.TR, PdfName.TH, PdfName.TD, PdfName
                .THead, PdfName.TBody, PdfName.TFoot, PdfName.Caption, PdfName.Figure, PdfName.Formula, PdfName.Artifact
                ));
        }

        // Hn, this type is handled in roleBelongsToStandardNamespace method
        /// <summary>Gets the name of the default standard structure namespace.</summary>
        /// <remarks>
        /// Gets the name of the default standard structure namespace. When a namespace is not
        /// explicitly specified for a given structure element or attribute, it shall be assumed to be within this
        /// default standard structure namespace.
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
        public static bool RoleBelongsToStandardNamespace(PdfName role, String standardNamespaceName) {
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

        /// <summary>
        /// Checks if the given
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// matches the Hn role pattern. To match this pattern, the given role
        /// shall always consist of the uppercase letter "H" and one or more digits, representing an unsigned integer
        /// greater than or equal to 1, without leading zeroes or any other prefix or postfix
        /// </summary>
        /// <param name="role">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// that specifies a role to be checked against Hn role pattern.
        /// </param>
        /// <returns>true if the role matches, false otherwise.</returns>
        public static bool IsHnRole(PdfName role) {
            String roleStrVal = role.GetValue();
            if (roleStrVal.StartsWith("H") && roleStrVal.Length > 1 && roleStrVal[1] != '0') {
                try {
                    return System.Convert.ToInt32(roleStrVal.JSubstring(1, roleStrVal.Length)) > 0;
                }
                catch (Exception) {
                }
            }
            // ignored
            return false;
        }
    }
}
