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
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>This class defines the allowed parent-child relations for the PDF2.0 standard.</summary>
    public class PdfAllowedTagRelations {
        public const String NUMBERED_HEADER = "Hn";

        public const String ACTUAL_CONTENT = "CONTENT";

        private static readonly Regex numberedHeaderPattern = iText.Commons.Utils.StringUtil.RegexCompile("H(\\d+)"
            );

        protected internal readonly IDictionary<String, ICollection<String>> allowedParentChildRelations = new Dictionary
            <String, ICollection<String>>();

        /// <summary>
        /// Creates a new instance of
        /// <see cref="PdfAllowedTagRelations"/>.
        /// </summary>
        public PdfAllowedTagRelations() {
            allowedParentChildRelations.Put("StructTreeRoot", JavaCollectionsUtil.Singleton(StandardRoles.DOCUMENT));
            allowedParentChildRelations.Put(StandardRoles.DOCUMENT, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles
                .TOC, StandardRoles.ASIDE, StandardRoles.BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles
                .P, StandardRoles.NOTE, StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles.TITLE, StandardRoles
                .LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles
                .L, StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.DOCUMENTFRAGMENT, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT
                , StandardRoles.DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles
                .SECT, StandardRoles.TOC, StandardRoles.ASIDE, StandardRoles.BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles
                .TITLE, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.FENOTE, StandardRoles
                .INDEX, StandardRoles.L, StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles
                .ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.PART, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles
                .TOC, StandardRoles.TOCI, StandardRoles.ASIDE, StandardRoles.BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles
                .TITLE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles
                .ANNOT, StandardRoles.FORM, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles.L, StandardRoles.
                BIBENTRY, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles
                .ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.DIV, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles
                .TOC, StandardRoles.TOCI, StandardRoles.ASIDE, StandardRoles.BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles
                .TITLE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN
                , StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles
                .FORM, StandardRoles.RUBY, StandardRoles.RB, StandardRoles.RT, StandardRoles.RP, StandardRoles.WARICHU
                , StandardRoles.WT, StandardRoles.WP, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles.L, StandardRoles
                .LI, StandardRoles.LBODY, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.TR, StandardRoles
                .TH, StandardRoles.TD, StandardRoles.THEAD, StandardRoles.TBODY, StandardRoles.TFOOT, StandardRoles.CAPTION
                , StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.ART, JavaUtil.ArraysAsList(StandardRoles.DOCUMENTFRAGMENT, StandardRoles
                .PART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles.TOC, StandardRoles.ASIDE, StandardRoles.BLOCKQUOTE
                , StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles.CODE
                , NUMBERED_HEADER, StandardRoles.H, StandardRoles.TITLE, StandardRoles.LBL, StandardRoles.LINK, StandardRoles
                .ANNOT, StandardRoles.FORM, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles.L, StandardRoles.
                TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.SECT, JavaUtil.ArraysAsList(StandardRoles.DOCUMENTFRAGMENT, 
                StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles.TOC, StandardRoles
                .ASIDE, StandardRoles.BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles
                .NOTE, StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles.TITLE, StandardRoles.LBL, StandardRoles
                .LINK, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles
                .L, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles
                .ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.TOC, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .TOC, StandardRoles.TOCI, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.CAPTION, StandardRoles
                .ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.TOCI, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles
                .TOC, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles.LBL, StandardRoles
                .REFERENCE, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.ASIDE, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles
                .TOC, StandardRoles.BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles
                .NOTE, StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles.LBL, StandardRoles.LINK, StandardRoles
                .REFERENCE, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles
                .L, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles
                .ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.BLOCKQUOTE, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles
                .TOC, StandardRoles.BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles
                .NOTE, StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles.TITLE, StandardRoles.LBL, StandardRoles
                .LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.FENOTE, StandardRoles
                .INDEX, StandardRoles.L, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.NONSTRUCT, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles
                .TOC, StandardRoles.TOCI, StandardRoles.ASIDE, StandardRoles.BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles
                .TITLE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN
                , StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles
                .FORM, StandardRoles.RUBY, StandardRoles.RB, StandardRoles.RT, StandardRoles.RP, StandardRoles.WARICHU
                , StandardRoles.WT, StandardRoles.WP, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles.L, StandardRoles
                .LI, StandardRoles.LBODY, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.TR, StandardRoles
                .TH, StandardRoles.TD, StandardRoles.THEAD, StandardRoles.TBODY, StandardRoles.TFOOT, StandardRoles.CAPTION
                , StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.PRIVATE, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles
                .TOC, StandardRoles.TOCI, StandardRoles.ASIDE, StandardRoles.BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles
                .TITLE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN
                , StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles
                .FORM, StandardRoles.RUBY, StandardRoles.RB, StandardRoles.RT, StandardRoles.RP, StandardRoles.WARICHU
                , StandardRoles.WT, StandardRoles.WP, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles.L, StandardRoles
                .LI, StandardRoles.LBODY, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.TR, StandardRoles
                .TH, StandardRoles.TD, StandardRoles.THEAD, StandardRoles.TBODY, StandardRoles.TFOOT, StandardRoles.CAPTION
                , StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.TITLE, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .DIV, StandardRoles.ASIDE, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles
                .NOTE, StandardRoles.CODE, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN
                , StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles
                .FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.L, StandardRoles
                .BIBENTRY, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles
                .ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.SUB, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.NOTE, StandardRoles.CODE, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG
                , StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.
                ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles
                .L, StandardRoles.BIBENTRY, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.P, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.NOTE, StandardRoles.CODE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.
                EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE
                , StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE
                , StandardRoles.L, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles.FORMULA
                , StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.NOTE, JavaUtil.ArraysAsList(StandardRoles.DOCUMENTFRAGMENT, 
                StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles.ASIDE, StandardRoles
                .BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles
                .CODE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN
                , StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles
                .FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles
                .L, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.CODE, JavaUtil.ArraysAsList(StandardRoles.DOCUMENTFRAGMENT, 
                StandardRoles.PART, StandardRoles.DIV, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.NOTE
                , StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.REFERENCE
                , StandardRoles.ANNOT, StandardRoles.FENOTE, StandardRoles.BIBENTRY, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(NUMBERED_HEADER, JavaUtil.ArraysAsList(StandardRoles.ART, StandardRoles.SECT
                , StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.NOTE, StandardRoles.CODE, StandardRoles
                .SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.QUOTE
                , StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.
                RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.BIBENTRY, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.H, JavaUtil.ArraysAsList(StandardRoles.ART, StandardRoles.SECT
                , StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.NOTE, StandardRoles.CODE, StandardRoles
                .SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.QUOTE
                , StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.
                RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.BIBENTRY, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.LBL, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.NOTE, StandardRoles.CODE, StandardRoles.SUB, StandardRoles.EM, StandardRoles.STRONG
                , StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.
                ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles
                .BIBENTRY, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.EM, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.NOTE, StandardRoles.CODE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.
                EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE
                , StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE
                , StandardRoles.BIBENTRY, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.STRONG, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.NOTE, StandardRoles.CODE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.
                EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE
                , StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE
                , StandardRoles.BIBENTRY, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.SPAN, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.NOTE, StandardRoles.CODE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.
                EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE
                , StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE
                , StandardRoles.BIBENTRY, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.QUOTE, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.NOTE, StandardRoles.CODE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.
                EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE
                , StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE
                , StandardRoles.BIBENTRY, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.LINK, JavaUtil.ArraysAsList(StandardRoles.DOCUMENTFRAGMENT, 
                StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles.ASIDE, StandardRoles
                .BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles
                .CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles.TITLE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles
                .EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.REFERENCE, StandardRoles
                .ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles
                .L, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.REFERENCE, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.NOTE, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles
                .SPAN, StandardRoles.LINK, StandardRoles.ANNOT, StandardRoles.FENOTE, StandardRoles.BIBENTRY, StandardRoles
                .FIGURE, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.ANNOT, JavaUtil.ArraysAsList(StandardRoles.DOCUMENTFRAGMENT, 
                StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles.ASIDE, StandardRoles
                .BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles.P, StandardRoles.NOTE, StandardRoles.CODE, NUMBERED_HEADER
                , StandardRoles.H, StandardRoles.TITLE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles
                .STRONG, StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles
                .ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles
                .L, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.FORM, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .DIV, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.NOTE, StandardRoles.CODE, StandardRoles
                .LBL, StandardRoles.REFERENCE, StandardRoles.FENOTE, StandardRoles.L, StandardRoles.BIBENTRY, StandardRoles
                .TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.RUBY, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.RB, StandardRoles.RT, StandardRoles.RP, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.RB, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.SUB, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles
                .QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles
                .ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.RT, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.SUB, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles
                .QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles
                .ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.RP, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.SUB, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles
                .QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles
                .ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.WARICHU, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.WT, StandardRoles.WP, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.WT, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.SUB, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles
                .QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles
                .ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.WP, JavaUtil.ArraysAsList(ACTUAL_CONTENT, StandardRoles.NONSTRUCT
                , StandardRoles.PRIVATE, StandardRoles.SUB, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN
                , StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles
                .FORM, StandardRoles.FIGURE, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.FENOTE, JavaUtil.ArraysAsList(StandardRoles.DOCUMENTFRAGMENT
                , StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles.ASIDE, StandardRoles
                .BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles
                .CODE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN
                , StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles
                .FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.L, StandardRoles
                .TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.INDEX, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .DIV, StandardRoles.SECT, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles
                .NOTE, NUMBERED_HEADER, StandardRoles.H, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles.FENOTE
                , StandardRoles.L, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA
                , StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.L, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.L, StandardRoles.LI, StandardRoles.CAPTION, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.LI, JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles.NONSTRUCT
                , StandardRoles.PRIVATE, StandardRoles.LBL, StandardRoles.LBODY, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.LBODY, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles.ASIDE, StandardRoles.BLOCKQUOTE, StandardRoles
                .NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles.CODE, NUMBERED_HEADER
                , StandardRoles.H, StandardRoles.SUB, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles
                .QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles
                .RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles.L, StandardRoles
                .BIBENTRY, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles
                .ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.BIBENTRY, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .DIV, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles
                .LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.LINK, StandardRoles.REFERENCE
                , StandardRoles.ANNOT, StandardRoles.FENOTE, StandardRoles.FIGURE, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.TABLE, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.TR, StandardRoles.THEAD, StandardRoles.TBODY, StandardRoles.TFOOT, StandardRoles
                .CAPTION, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.TR, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.TH, StandardRoles.TD, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.TH, JavaUtil.ArraysAsList(StandardRoles.ART, StandardRoles.DIV
                , StandardRoles.SECT, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles.NOTE
                , StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles.LBL, StandardRoles.EM, StandardRoles
                .STRONG, StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles
                .ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles
                .INDEX, StandardRoles.L, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.TD, JavaUtil.ArraysAsList(StandardRoles.ART, StandardRoles.DIV
                , StandardRoles.SECT, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles.NOTE
                , StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles.LBL, StandardRoles.EM, StandardRoles
                .STRONG, StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles
                .ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles
                .INDEX, StandardRoles.L, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.THEAD, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.TR, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.TBODY, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.TR, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.TFOOT, JavaUtil.ArraysAsList(StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.TR, StandardRoles.ARTIFACT));
            allowedParentChildRelations.Put(StandardRoles.CAPTION, JavaUtil.ArraysAsList(StandardRoles.DOCUMENTFRAGMENT
                , StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles.ASIDE, StandardRoles
                .BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles
                .CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles
                .STRONG, StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles
                .ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles
                .INDEX, StandardRoles.L, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.FIGURE, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles.ASIDE, StandardRoles.BLOCKQUOTE, StandardRoles
                .NONSTRUCT, StandardRoles.PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles.CODE, NUMBERED_HEADER
                , StandardRoles.H, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles
                .SPAN, StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles
                .FORM, StandardRoles.RUBY, StandardRoles.WARICHU, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles
                .L, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles
                .FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
            allowedParentChildRelations.Put(StandardRoles.FORMULA, JavaUtil.ArraysAsList(StandardRoles.PART, StandardRoles
                .DIV, StandardRoles.ASIDE, StandardRoles.BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles.PRIVATE, StandardRoles
                .P, StandardRoles.NOTE, StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles.SUB, StandardRoles
                .LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN, StandardRoles.QUOTE, StandardRoles.LINK
                , StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles.FORM, StandardRoles.RUBY, StandardRoles.
                WARICHU, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles.L, StandardRoles.BIBENTRY, StandardRoles
                .TABLE, StandardRoles.CAPTION, StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT
                ));
            allowedParentChildRelations.Put(StandardRoles.ARTIFACT, JavaUtil.ArraysAsList(StandardRoles.DOCUMENT, StandardRoles
                .DOCUMENTFRAGMENT, StandardRoles.PART, StandardRoles.ART, StandardRoles.DIV, StandardRoles.SECT, StandardRoles
                .TOC, StandardRoles.TOCI, StandardRoles.ASIDE, StandardRoles.BLOCKQUOTE, StandardRoles.NONSTRUCT, StandardRoles
                .PRIVATE, StandardRoles.P, StandardRoles.NOTE, StandardRoles.CODE, NUMBERED_HEADER, StandardRoles.H, StandardRoles
                .TITLE, StandardRoles.SUB, StandardRoles.LBL, StandardRoles.EM, StandardRoles.STRONG, StandardRoles.SPAN
                , StandardRoles.QUOTE, StandardRoles.LINK, StandardRoles.REFERENCE, StandardRoles.ANNOT, StandardRoles
                .FORM, StandardRoles.RUBY, StandardRoles.RB, StandardRoles.RT, StandardRoles.RP, StandardRoles.WARICHU
                , StandardRoles.WT, StandardRoles.WP, StandardRoles.FENOTE, StandardRoles.INDEX, StandardRoles.L, StandardRoles
                .LI, StandardRoles.LBODY, StandardRoles.BIBENTRY, StandardRoles.TABLE, StandardRoles.TR, StandardRoles
                .TH, StandardRoles.TD, StandardRoles.THEAD, StandardRoles.TBODY, StandardRoles.TFOOT, StandardRoles.CAPTION
                , StandardRoles.FIGURE, StandardRoles.FORMULA, StandardRoles.ARTIFACT, ACTUAL_CONTENT));
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
