/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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

namespace iText.Pdfua.Exceptions {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class PdfUAExceptionMessageConstants {
        public const String ONE_OR_MORE_STANDARD_ROLE_REMAPPED = "One or more standard types are remapped.";

        public const String TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING = "Tag hasn't been added before adding content to the canvas.";

        public const String CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT = "Content is neither marked as Artifact nor tagged as real content.";

        public const String ARTIFACT_CANT_BE_INSIDE_REAL_CONTENT = "Content marked as artifact may only reside in Artifact content.";

        public const String REAL_CONTENT_CANT_BE_INSIDE_ARTIFACT = "Content marked as content may not reside in Artifact content.";

        public const String CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT = "Content with MCID, but MCID wasn't found in StructTreeRoot.";

        public const String FONT_SHOULD_BE_EMBEDDED = "Following font(s) are not embedded: {0}";

        public const String METADATA_SHALL_BE_PRESENT_IN_THE_CATALOG_DICTIONARY = "Metadata shall be present in the catalog dictionary";

        public const String REAL_CONTENT_INSIDE_ARTIFACT_OR_VICE_VERSA = "Tagged content is present inside content marked as Artifact or vice versa.";

        public const String SUSPECTS_ENTRY_IN_MARK_INFO_DICTIONARY_SHALL_NOT_HAVE_A_VALUE_OF_TRUE = "Suspects entry in mark info dictionary shall not have a value of true.";

        public const String TAG_MAPPING_DOESNT_TERMINATE_WITH_STANDARD_TYPE = "\"{0}\" tag mapping does not terminate with a standard type.";

        public const String IMAGE_SHALL_HAVE_ALT = "Figure tags shall include an alternative representation or " +
             "replacement text. call com.itextpdf.kernel.pdf.tagutils.AccessibilityProperties#setActualText or com"
             + ".itextpdf.kernel.pdf.tagutils.AccessibilityProperties#setAlternateDescription to be PDF/UA compliant.";

        public const String CELL_HAS_INVALID_ROLE = "Cell: row {0} ({1}) col {2} has invalid role.";

        public const String TABLE_CONTAINS_EMPTY_CELLS = "Cell: row {0} ({1}) col {2} is empty, each row should " 
            + "have the same amount of columns when taking into account spanning.";

        public const String CELL_CANT_BE_DETERMINED_ALGORITHMICALLY = "TD cell row:{0} col:{1} in table {2} does" 
            + " not contain a valid Headers attribute, and Headers for this cell cannot be determined algorithmically.";

        public const String FORMULA_SHALL_HAVE_ALT = "Formula tags shall include an alternative representation or "
             + "replacement text.";

        public const String NOTE_TAG_SHALL_HAVE_ID_ENTRY = "Note tags shall include a unique ID entry.";

        public const String EMBEDDED_FONTS_SHALL_DEFINE_ALL_REFERENCED_GLYPHS = "Embedded fonts shall define all "
             + "glyphs referenced for rendering within the conforming file.";

        public const String DOCUMENT_SHALL_CONTAIN_XMP_METADATA_STREAM = "Document shall contain a XMP metadata stream.";

        public const String METADATA_SHALL_CONTAIN_UA_VERSION_IDENTIFIER = "Metadata shall contain correct pdfuaid:part version identifier.";

        public const String METADATA_SHALL_CONTAIN_DC_TITLE_ENTRY = "Metadata shall contain dc:title entry.";

        public const String INVALID_PDF_VERSION = "Specified document pdf version isn't supported in pdf/ua.";

        public const String H1_IS_SKIPPED = "Heading level 1 is skipped in a descending sequence of header levels.";

        public const String HN_IS_SKIPPED = "Heading level {0} is skipped in a descending sequence of header " + "levels.";

        public const String MORE_THAN_ONE_H_TAG = "A node contains more than one H tag.";

        public const String DOCUMENT_USES_BOTH_H_AND_HN = "Document uses both H and H# tags.";

        public const String DOCUMENT_SHALL_CONTAIN_VALID_LANG_ENTRY = "Document does not contain valid lang entry.";

        public const String MISSING_VIEWER_PREFERENCES = "ViewerPreferences dictionary of the Catalog dictionary "
             + "does not contain a DisplayDocTitle entry.";

        public const String VIEWER_PREFERENCES_IS_FALSE = "ViewerPreferences dictionary of the Catalog dictionary "
             + "contains a DisplayDocTitle entry with a value of false.";

        public const String NAME_ENTRY_IS_MISSING_OR_EMPTY_IN_OCG = "Name entry is missing or has " + "an empty string as its value in an Optional Content Configuration Dictionary.";

        public const String OCG_SHALL_NOT_CONTAIN_AS_ENTRY = "An AS entry appears in an Optional Content.";

        public const String OCG_PROPERTIES_CONFIG_SHALL_BE_AN_ARRAY = "Optional Content properties " + "configs shall be an array.";

        public const String DYNAMIC_XFA_FORMS_SHALL_NOT_BE_USED = "Dynamic XFA forms shall not be used.";

        public const String FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY = "File specification dictionary shall contain f key and uf key.";

        public const String NON_UNIQUE_ID_ENTRY_IN_STRUCT_TREE_ROOT = "ID entry '{0}' shall be unique among all elements "
             + "in the document’s structure hierarchy";

        public const String FORM_FIELD_SHALL_CONTAIN_ALT_ENTRY = "Interactive formfields should have an alternative "
             + "description. You can set it using FormField#getAccessibilityProperties().setAlternateDescription" 
            + "(\"your alternative description\").";

        public const String LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK = "A link annotation is not nested within a <Link> tag.";

        public const String ANNOT_TRAP_NET_IS_NOT_PERMITTED = "Annotations of subtype TrapNet shall not be permitted.";

        public const String PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_S = "A page with annotation(s) doesn't contains Tabs key with S value.";

        public const String P_VALUE_IS_ABSENT_IN_ENCRYPTION_DICTIONARY = "Permissions are absent " + "in pdf encryption dictionary.";

        public const String TENTH_BIT_OF_P_VALUE_IN_ENCRYPTION_SHOULD_BE_NON_ZERO = "10th bit of P value of " + "Encryption dictionary should be 1 if the document is tagged.";

        private PdfUAExceptionMessageConstants() {
        }
        // Empty constructor
    }
}
