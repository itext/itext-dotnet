using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>This class defines the allowed parent-child relations for the PDF2.0 standard.</summary>
    public class PdfAllowedTagRelations {
        private const String ACTUAL_CONTENT = "CONTENT";

        private const String OBJR_CONTENT = "OBJR";

        public const String NUMBERED_HEADER = "Hn";

        private static readonly Regex numberedHeaderPattern = iText.Commons.Utils.StringUtil.RegexCompile("H(\\d+)"
            );

        protected internal readonly IDictionary<String, ICollection<String>> allowedParentChildRelations = new Dictionary
            <String, ICollection<String>>();

        /// <summary>
        /// Creates a new instance of
        /// <see cref="PdfAllowedTagRelations"/>.
        /// </summary>
        public PdfAllowedTagRelations() {
            allowedParentChildRelations.Put(StandardRoles.DOCUMENT, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.DIV, StandardRoles.ASIDE, StandardRoles.P, NUMBERED_HEADER
                , StandardRoles.H, StandardRoles.TITLE, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles
                .L, StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.DOCUMENTFRAGMENT, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT
                , StandardRoles.DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.DIV, StandardRoles.ASIDE, StandardRoles
                .P, NUMBERED_HEADER, StandardRoles.H, StandardRoles.TITLE, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles
                .FORM, StandardRoles.L, StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles
                .ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.PART, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.DIV, StandardRoles.ASIDE, StandardRoles.P, NUMBERED_HEADER
                , StandardRoles.H, StandardRoles.TITLE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.LINK, StandardRoles
                .ANNOT, StandardRoles.FORM, StandardRoles.FENOTE, StandardRoles.L, StandardRoles.TABLE, StandardRoles.
                CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.DIV, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.DIV, StandardRoles.ASIDE, StandardRoles.P, NUMBERED_HEADER
                , StandardRoles.H, StandardRoles.TITLE, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles
                .FENOTE, StandardRoles.L, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.ASIDE, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.DIV, StandardRoles.P, NUMBERED_HEADER, StandardRoles
                .H, StandardRoles.LBL, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.FENOTE
                , StandardRoles.L, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.TITLE, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .DIV, StandardRoles.ASIDE, StandardRoles.P, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, 
                StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles
                .WARICHU, StandardRoles.L, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.SUB, JavaUtil.ArraysAsList(StandardRoles.LBL, StandardRoles.
                EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM
                , StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.L, StandardRoles.FIGURE, StandardRoles.FORMULA
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.P, JavaUtil.ArraysAsList(StandardRoles.SUB, StandardRoles.LBL
                , StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, 
                StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.L, 
                StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            // Headers get a special treatment, since PDF2.0 there is no limit to the number of levels
            allowedParentChildRelations.Put(NUMBERED_HEADER, JavaUtil.ArraysAsList(StandardRoles.SUB, StandardRoles.LBL
                , StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, 
                StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FIGURE, StandardRoles.FORMULA
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.H, JavaUtil.ArraysAsList(StandardRoles.SUB, StandardRoles.LBL
                , StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, 
                StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FIGURE, StandardRoles.FORMULA
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.LBL, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles.
                SUB, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT
                , StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FIGURE, StandardRoles.FORMULA
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.EM, JavaUtil.ArraysAsList(StandardRoles.SUB, StandardRoles.LBL
                , StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, 
                StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FIGURE, StandardRoles.FORMULA
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.STRONG, JavaUtil.ArraysAsList(StandardRoles.SUB, StandardRoles
                .LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT
                , StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FIGURE, StandardRoles.FORMULA
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.SPAN, JavaUtil.ArraysAsList(StandardRoles.SUB, StandardRoles
                .LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT
                , StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FIGURE, StandardRoles.FORMULA
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.LINK, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles
                .SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.ANNOT
                , StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles
                .ARTIFACT, ACTUAL_CONTENT, OBJR_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.ANNOT, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles
                .SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK
                , StandardRoles.ANNOT, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FIGURE, StandardRoles.
                FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT, OBJR_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.FORM, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles
                .LBL, StandardRoles.CAPTION, StandardRoles.ARTIFACT, OBJR_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.RUBY, JavaUtil.ArraysAsList(StandardRoles.RB, StandardRoles.
                RT, StandardRoles.RP, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.RB, JavaUtil.ArraysAsList(StandardRoles.SUB, StandardRoles.EM
                , StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.RT, JavaUtil.ArraysAsList(StandardRoles.SUB, StandardRoles.EM
                , StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.RP, JavaUtil.ArraysAsList(StandardRoles.SUB, StandardRoles.EM
                , StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.WARICHU, JavaUtil.ArraysAsList(StandardRoles.WT, StandardRoles
                .WP, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.WT, JavaUtil.ArraysAsList(StandardRoles.SUB, StandardRoles.EM
                , StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.WP, JavaUtil.ArraysAsList(StandardRoles.SUB, StandardRoles.EM
                , StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.FENOTE, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .DIV, StandardRoles.ASIDE, StandardRoles.P, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles
                .STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles
                .RUBY, StandardRoles.WARICHU, StandardRoles.L, StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.L, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles.L, 
                StandardRoles.LI, StandardRoles.CAPTION, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.LI, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles.LBL
                , StandardRoles.FENOTE, StandardRoles.LBODY, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.LBODY, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .DIV, StandardRoles.ASIDE, StandardRoles.P, NUMBERED_HEADER, StandardRoles.H, StandardRoles.SUB, StandardRoles
                .EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.
                FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.L, StandardRoles.
                TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.TABLE, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles
                .TR, StandardRoles.THEAD, StandardRoles.TBODY, StandardRoles.TFOOT, StandardRoles.CAPTION, StandardRoles
                .ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.TR, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles.TH
                , StandardRoles.TD, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.TH, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles.P
                , NUMBERED_HEADER, StandardRoles.H, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles
                .SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.
                WARICHU, StandardRoles.FENOTE, StandardRoles.L, StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.TD, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles.P
                , NUMBERED_HEADER, StandardRoles.H, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles
                .SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.
                WARICHU, StandardRoles.FENOTE, StandardRoles.L, StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.THEAD, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles
                .TR, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.TBODY, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles
                .TR, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.TFOOT, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles
                .TR, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.CAPTION, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .DIV, StandardRoles.ASIDE, StandardRoles.P, NUMBERED_HEADER, StandardRoles.H, StandardRoles.SUB, StandardRoles
                .LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT
                , StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.L
                , StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.FIGURE, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .DIV, StandardRoles.ASIDE, StandardRoles.P, NUMBERED_HEADER, StandardRoles.H, StandardRoles.LBL, StandardRoles
                .EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.
                FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.L, StandardRoles.
                TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.FORMULA, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .DIV, StandardRoles.ASIDE, StandardRoles.P, NUMBERED_HEADER, StandardRoles.H, StandardRoles.SUB, StandardRoles
                .LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT
                , StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.L
                , StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles
                .ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.ARTIFACT, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.DIV, StandardRoles.ASIDE, StandardRoles.P, NUMBERED_HEADER
                , StandardRoles.H, StandardRoles.TITLE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles
                .STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles
                .RUBY, StandardRoles.RB, StandardRoles.RT, StandardRoles.RP, StandardRoles.WARICHU, StandardRoles.WT, 
                StandardRoles.WP, StandardRoles.FENOTE, StandardRoles.L, StandardRoles.LI, StandardRoles.LBODY, StandardRoles
                .TABLE, StandardRoles.TR, StandardRoles.TH, StandardRoles.TD, StandardRoles.THEAD, StandardRoles.TBODY
                , StandardRoles.TFOOT, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles
                .ARTIFACT, ACTUAL_CONTENT));
        }

        /// <summary>Checks if the given parent-child relation is allowed.</summary>
        /// <param name="parentRole">The parent role.</param>
        /// <param name="childRole">The child role.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the relation is allowed,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool IsRelationAllowed(String parentRole, String childRole) {
            ICollection<String> allowedChildren = allowedParentChildRelations.Get(NormalizeRole(parentRole));
            if (allowedChildren != null) {
                return allowedChildren.Contains(NormalizeRole(childRole));
            }
            throw new ArgumentException("parentRole " + parentRole + " is not a valid structure tree role");
        }

        /// <summary>Checks if the given parent role allows content.</summary>
        /// <param name="parentRole">The parent role.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the parent role allows content,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool IsContentAllowedInRole(String parentRole) {
            ICollection<String> allowedChildren = allowedParentChildRelations.Get(NormalizeRole(parentRole));
            if (allowedChildren != null) {
                return allowedChildren.Contains(ACTUAL_CONTENT);
            }
            throw new ArgumentException("parentRole " + parentRole + " is not a valid structure tree role");
        }

        /// <summary>Checks if the given parent role allows content object.</summary>
        /// <param name="parentRole">The parent role.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the parent role allows content object,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool IsContentObjectAllowedInRole(String parentRole) {
            ICollection<String> allowedChildren = allowedParentChildRelations.Get(NormalizeRole(parentRole));
            if (allowedChildren != null) {
                return allowedChildren.Contains(OBJR_CONTENT);
            }
            throw new ArgumentException("parentRole " + parentRole + " is not a valid structure tree role");
        }

        /// <summary>Normalizes the role.</summary>
        /// <param name="role">The role to normalize.</param>
        /// <returns>The normalized role.</returns>
        public virtual String NormalizeRole(String role) {
            if (role == null) {
                return null;
            }
            if (iText.Commons.Utils.Matcher.Match(numberedHeaderPattern, role).Matches()) {
                return NUMBERED_HEADER;
            }
            return role;
        }
    }
}
