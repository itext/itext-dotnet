using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Tagging {
    public class StandardStructureNamespace {
        private static ICollection<PdfName> STD_STRUCT_NAMESPACE_1_7_TYPES = new HashSet<PdfName>();

        private static ICollection<PdfName> STD_STRUCT_NAMESPACE_2_0_TYPES = new HashSet<PdfName>();

        private static readonly PdfString MATH_ML = new PdfString("http://www.w3.org/1998/Math/MathML", null, true
            );

        public static readonly PdfString _1_7 = new PdfString("http://www.iso.org/pdf/ssn", null, true);

        public static readonly PdfString _2_0 = new PdfString("http://www.iso.org/pdf2/ssn", null, true);

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
        public static PdfString GetDefault() {
            return _1_7;
        }

        public static bool IsKnownDomainSpecificNamespace(PdfNamespace @namespace) {
            return MATH_ML.Equals(@namespace.GetNamespaceName());
        }

        public static bool RoleBelongsToStandardNamespace(PdfName role, PdfString standardNamespaceName) {
            if (_1_7.Equals(standardNamespaceName)) {
                return STD_STRUCT_NAMESPACE_1_7_TYPES.Contains(role);
            }
            else {
                if (_2_0.Equals(standardNamespaceName)) {
                    return STD_STRUCT_NAMESPACE_2_0_TYPES.Contains(role) || IsHnRole(role);
                }
            }
            return false;
        }

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
