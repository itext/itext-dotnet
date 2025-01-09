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

namespace iText.IO.Logs {
    /// <summary>Class containing constants to be used in logging.</summary>
    public sealed class IoLogMessageConstant {
        public const String ACTION_WAS_SET_TO_LINK_ANNOTATION_WITH_DESTINATION = "Action was set for a link annotation containing destination. The old destination will be cleared.";

        public const String ALREADY_FLUSHED_INDIRECT_OBJECT_MADE_FREE = "An attempt is made to free already flushed indirect object reference. Indirect reference wasn't freed.";

        public const String ALREADY_TAGGED_HINT_MARKED_ARTIFACT = "A layout tagging hint for which an actual tag was already created in tags structure is marked as artifact."
             + " Existing tag will be left in the tags tree.";

        public const String ASSOCIATED_FILE_SPEC_SHALL_INCLUDE_AFRELATIONSHIP = "For associated files their associated file specification dictionaries shall include the AFRelationship "
             + "key.";

        public const String ATTEMPT_PROCESS_NAN = "Attempt to process NaN in PdfNumber or when writing to PDF. Zero value will be used as a fallback.";

        public const String ATTEMPT_TO_CREATE_A_TAG_FOR_FINISHED_HINT = "Attempt to create a tag for a hint which is already marked as finished, tag will not be created.";

        public const String ATTEMPT_TO_GENERATE_PDF_PAGES_TREE_WITHOUT_ANY_PAGES = "Attempt to generate PDF pages tree without any pages, so a new page will be added.";

        public const String ATTEMPT_TO_MOVE_TO_FLUSHED_PARENT = "An attempt is made to move the tag tree pointer to the tag parent which has been already flushed. Tag "
             + "tree pointer is moved to the root tag instead.";

        public const String CANNOT_ADD_FINISHED_HINT_AS_A_NEW_KID_HINT = "Layout tagging hints addition failed: cannot add a hint that is already marked as finished. Consider "
             + "using com.itextpdf.layout.tagging.LayoutTaggingHelper#moveKidHint method for moving already " + "finished kid hint from not yet finished parent hint.";

        public const String CANNOT_ADD_HINTS_TO_FINISHED_PARENT = "Layout tagging hints addition failed: cannot add new kid hints to a parent which hint is already marked "
             + "as finished. Consider using com.itextpdf.layout.tagging.LayoutTaggingHelper#replaceKidHint " + "method for replacing not yet finished kid hint of a finished parent hint.";

        public const String CANNOT_MOVE_FINISHED_HINT = "Layout tagging hints modification failed: cannot move kid hint for which both itself and it's parent are"
             + " already marked as finished.";

        public const String CANNOT_MOVE_HINT_TO_FINISHED_PARENT = "Layout tagging hints modification failed: cannot move kid hint to a parent that is already marked as "
             + "finished.";

        public const String CANNOT_REPLACE_FINISHED_HINT = "Layout tagging hints modification failed: cannot replace a kid hint that is already marked as finished.";

        public const String CANNOT_RESOLVE_ROLE_IN_NAMESPACE_TOO_MUCH_TRANSITIVE_MAPPINGS = "Cannot resolve \"{0}\" role in {1} namespace mapping to standard role, because of the too much transitive "
             + "mappings.";

        public const String CANNOT_RESOLVE_ROLE_TOO_MUCH_TRANSITIVE_MAPPINGS = "Cannot resolve \"{0}\" role mapping to standard role, because of the too much transitive mappings.";

        public const String CANVAS_ALREADY_FULL_ELEMENT_WILL_BE_SKIPPED = "Canvas is already full. Element will be skipped.";

        public const String CHARACTER_DIRECTION_HAS_NOT_BEEN_DETECTED = "The direction for this character has not been detected: code point {0}. The Other Neutrals algorithm will "
             + "be used.";

        public const String CLIP_ELEMENT = "Element content was clipped because some height properties are set.";

        public const String COLLECTION_DICTIONARY_ALREADY_EXISTS_IT_WILL_BE_MODIFIED = "Collection dictionary already exists. It will be modified.";

        public const String COLORANT_INTENSITIES_INVALID = "Some of colorant intensities are invalid: they are bigger than 1 or less than 0. We will force them to "
             + "become 1 or 0 respectively.";

        public const String COLOR_ALPHA_CHANNEL_IS_IGNORED = "Alpha channel {0} was ignored during color creation. Note that opacity can be achieved in some places by "
             + "using 'setOpacity' method or 'TransparentColor' class";

        public const String COLOR_NOT_PARSED = "Color \"{0}\" was not parsed. It has invalid value. Defaulting to "
             + "black color.";

        public const String COMB_FLAG_MAY_BE_SET_ONLY_IF_MAXLEN_IS_PRESENT = "The Comb flag may be set only if the MaxLen entry is present in the text field dictionary and if the "
             + "Multiline, Password, and FileSelect flags are clear.";

        public const String COULD_NOT_FIND_GLYPH_WITH_CODE = "Could not find glyph with the following code: {0}";

        public const String CREATED_ROOT_TAG_HAS_MAPPING = "Created root tag has role mapping: \"/Document\" " + "role{0} is mapped{1}. Resulting tag structure might have invalid root tag.";

        public const String CREATE_COPY_SHOULD_BE_OVERRIDDEN = "While processing an instance of TextRenderer, " + 
            "iText uses createCopy() to create glyph lines of specific fonts, which represent its parts. " + "So if one extends TextRenderer, one should override createCopy, otherwise if FontSelector "
             + "related logic is triggered, copies of this TextRenderer will have the default behavior " + "rather than the custom one.";

        public const String DESTINATION_NOT_PERMITTED_WHEN_ACTION_IS_SET = "Destinations are not permitted for link annotations that already have actions. The old action will be "
             + "removed.";

        public const String DIRECTONLY_OBJECT_CANNOT_BE_INDIRECT = "DirectOnly object cannot be indirect";

        public const String DOCFONT_HAS_ILLEGAL_DIFFERENCES = "Document Font has illegal differences array. Entry {0} references a glyph ID over 255 and will be "
             + "ignored.";

        public const String DOCUMENT_ALREADY_HAS_FIELD = "The document already has field {0}. Annotations of the fields with this name will be added to the "
             + "existing one as children. If you want to have separate fields, please, rename them manually " + "before copying.";

        public const String DOCUMENT_HAS_CONFLICTING_OCG_NAMES = "Document has conflicting names for optional " + 
            "content groups. Groups with conflicting names will be renamed";

        public const String DOCUMENT_IDS_ARE_CORRUPTED = "The document original and/or modified id is corrupted";

        public const String DOCUMENT_VERSION_IN_CATALOG_CORRUPTED = "The document version specified in catalog is "
             + "corrupted";

        public const String DURING_CONSTRUCTION_OF_ICC_PROFILE_ERROR_OCCURRED = "During the construction of the ICC"
             + " profile, the {0} error with message \"{1}\" occurred, the ICC profile will not be installed in the "
             + "image.";

        public const String ELEMENT_WAS_FORCE_PLACED_KEEP_WITH_NEXT_WILL_BE_IGNORED = "Element was placed in a forced way. Keep with next property will be ignored";

        public const String EMBEDDED_GO_TO_DESTINATION_NOT_SPECIFIED = "No destination in the target was specified for action. Destination entry is mandatory for embedded go-to "
             + "actions.";

        public const String ENCOUNTERED_INVALID_MCR = "Corrupted tag structure: encountered invalid marked content reference - it doesn't refer to any page or "
             + "any mcid. This content reference will be ignored.";

        public const String ENCRYPTED_PAYLOAD_FILE_SPEC_SHALL_HAVE_AFRELATIONSHIP_FILED_EQUAL_TO_ENCRYPTED_PAYLOAD
             = "Encrypted payload file spec shall have 'AFRelationship' filed equal to 'EncryptedPayload'";

        public const String ENCRYPTION_ENTRIES_P_AND_ENCRYPT_METADATA_NOT_CORRESPOND_PERMS_ENTRY = "Encryption dictionary entries P and EncryptMetadata have value that does not correspond to encrypted "
             + "values in Perms key.";

        public const String EXCEPTION_WHILE_CREATING_DEFAULT_FONT = "Exception while creating default font (Helvetica, WinAnsi)";

        public const String EXCEPTION_WHILE_UPDATING_XMPMETADATA = "Exception while updating XmpMetadata";

        public const String EXISTING_TAG_STRUCTURE_ROOT_IS_NOT_STANDARD = "Existing tag structure of the document has a root of \"{0}\" role in \"{1}\" namespace that is not mapped"
             + " to the standard role.";

        public const String FAILED_TO_DETERMINE_CID_FONT_SUBTYPE = "Failed to determine CIDFont subtype. The type of CIDFont shall be CIDFontType0 or CIDFontType2.";

        public const String FAILED_TO_PARSE_ENCODING_STREAM = "Failed to parse encoding stream.";

        public const String FAILED_TO_PROCESS_A_TRANSFORMATION_MATRIX = "Failed to process a transformation matrix which is noninvertible. Some content may be placed not as "
             + "expected.";

        public const String FIELD_VALUE_IS_NOT_CONTAINED_IN_OPT_ARRAY = "Value \"{0}\" is not contained in /Opt array of field \"{1}\".";

        public const String FLEX_ITEM_LAYOUT_RESULT_IS_NOT_FULL = "Flex item layout result isn't full, but it must be. The cross size of the flex item will be 0.";

        public const String FLUSHED_OBJECT_CONTAINS_FREE_REFERENCE = "Flushed object contains indirect reference which is free. Null object will be written instead.";

        public const String FLUSHED_OBJECT_CONTAINS_REFERENCE_WHICH_NOT_REFER_TO_ANY_OBJECT = "Flushed object contains indirect reference which doesn't refer to any other object. Null object will be "
             + "written instead.";

        public const String FONT_DICTIONARY_WITH_NO_FONT_DESCRIPTOR = "Font dictionary does not contain required /FontDescriptor entry.";

        public const String FONT_DICTIONARY_WITH_NO_WIDTHS = "Font dictionary does not contain required /Widths " 
            + "entry.";

        public const String FONT_HAS_INVALID_GLYPH = "Font {0} has invalid glyph: {1}";

        public const String FONT_PROPERTY_MUST_BE_PDF_FONT_OBJECT = "The \"Property.FONT\" property must be a PdfFont object in this context.";

        public const String FONT_SUBSET_ISSUE = "Font subset issue. Full font will be embedded.";

        public const String FORBID_RELEASE_IS_SET = "ForbidRelease flag is set and release is called. Releasing will not be performed.";

        public const String GET_NEXT_RENDERER_SHOULD_BE_OVERRIDDEN = "If a renderer overflows, " + "iText uses this method to create another renderer for the overflow part. So if one wants "
             + "to extend the renderer, one should override this method: otherwise the default method " + "will be used and thus the default rather than the custom renderer will be created.";

        public const String GPOS_LOOKUP_SUBTABLE_FORMAT_NOT_SUPPORTED = "Subtable format {0} of GPOS Lookup Type {1} is not supported yet";

        public const String GRAPHICS_STATE_WAS_DELETED = "Graphics state is always deleted after event dispatching. If you want to preserve it in renderer info, "
             + "use preserveGraphicsState method after receiving renderer info.";

        public const String IF_PATH_IS_SET_VERTICES_SHALL_NOT_BE_PRESENT = "If Path key is set, Vertices key shall not be present. Remove Vertices key before setting Path";

        public const String IMAGE_HAS_AMBIGUOUS_SCALE = "The image cannot be auto scaled and scaled by a certain parameter simultaneously";

        public const String IMAGE_HAS_ICC_PROFILE_WITH_INCOMPATIBLE_NUMBER_OF_COLOR_COMPONENTS_COMPARED_TO_BASE_COLOR_SPACE_IN_INDEXED_COLOR_SPACE
             = "Image has icc profile with incompatible number of color components compared to base color space in image "
             + "indexed color space. The icc profile will be ignored.";

        public const String IMAGE_HAS_ICC_PROFILE_WITH_INCOMPATIBLE_NUMBER_OF_COLOR_COMPONENTS_COMPARED_TO_COLOR_SPACE
             = "Image has icc profile with incompatible number of color components compared to image color space. The icc "
             + "profile will be ignored.";

        public const String IMAGE_HAS_INCORRECT_OR_UNSUPPORTED_BASE_COLOR_SPACE_IN_INDEXED_COLOR_SPACE_OVERRIDDEN_BY_ICC_PROFILE
             = "Image has incorrect or unsupported base color space in indexed color space, it will be overridden by one "
             + "based on embedded icc profile.";

        public const String IMAGE_HAS_INCORRECT_OR_UNSUPPORTED_COLOR_SPACE_OVERRIDDEN_BY_ICC_PROFILE = "Image has incorrect or unsupported color space, that will be overridden by one based on embedded icc "
             + "profile.";

        public const String IMAGE_HAS_JBIG2DECODE_FILTER = "Image cannot be inline if it has JBIG2Decode filter. It"
             + " will be added as an ImageXObject";

        public const String IMAGE_HAS_JPXDECODE_FILTER = "Image cannot be inline if it has JPXDecode filter. It will be added as an ImageXObject";

        public const String IMAGE_HAS_MASK = "Image cannot be inline if it has a Mask";

        public const String IMAGE_SIZE_CANNOT_BE_MORE_4KB = "Inline image size cannot be more than 4KB. It will be added as an ImageXObject";

        public const String INDIRECT_REFERENCE_USED_IN_FLUSHED_OBJECT_MADE_FREE = "An attempt is made to free an indirect reference which was already used in the flushed object. Indirect "
             + "reference wasn't freed.";

        public const String INLINE_BLOCK_ELEMENT_WILL_BE_CLIPPED = "Inline block element does not fit into parent element and will be clipped";

        public const String INVALID_DISTRIBUTION_POINT = "Skipped CRL: {0}";

        public const String INVALID_DESTINATION_TYPE = "When destination's not associated with a Remote or Embedded Go-To action, it shall specify page "
             + "dictionary instead of page number. Otherwise destination might be considered invalid";

        public const String INVALID_INDIRECT_REFERENCE = "Invalid indirect reference {0} {1} R";

        public const String INVALID_KEY_VALUE_KEY_0_HAS_NULL_VALUE = "Invalid key value: key {0} has null value.";

        public const String FILTER_WAS_ALREADY_FLUSHED = "{0} {1} R stream object filter was already flushed, stream compression will remain as in original file. "
             + "Try to flush object containing filter before the PdfDocument#close() method so that stream " + "compression would be processed as intended.";

        public const String LAST_ROW_IS_NOT_COMPLETE = "Last row is not completed. Table bottom border may collapse as you do not expect it";

        public const String MAKE_COPY_OF_CATALOG_DICTIONARY_IS_FORBIDDEN = "Make copy of Catalog dictionary is forbidden.";

        public const String MAPPING_IN_NAMESPACE_OVERWRITTEN = "Existing mapping for {0} in {1} namespace was overwritten.";

        public const String MAPPING_IN_STRUCT_ROOT_OVERWRITTEN = "Existing mapping for {0} in structure tree root role map was {1} and it was overwritten with {2}.";

        public const String METHOD_IS_NOT_IMPLEMENTED_BY_DEFAULT_OTHER_METHOD_WILL_BE_USED = "Method {0} is not implemented by default: please, override and implement it. {1} will be used instead.";

        public const String MULTIPLE_VALUES_ON_A_NON_MULTISELECT_FIELD = "Multiple values were set to a field that does not have MultiSelect flag set.";

        public const String NAME_ALREADY_EXISTS_IN_THE_NAME_TREE = "Name \"{0}\" already exists in the name tree; old value will be replaced by the new one.";

        public const String NOT_TAGGED_PAGES_IN_TAGGED_DOCUMENT = "Not tagged pages are copied to the tagged document. Destination document now may contain not tagged "
             + "content.";

        public const String NUM_TREE_SHALL_NOT_END_WITH_KEY = "Number tree ends with a key which is invalid according to the PDF specification.";

        public const String OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED = "Occupied area has not been initialized. {0}";

        public const String OCG_COPYING_ERROR = "OCG copying caused the following exception: {0}.";

        public const String OCSP_STATUS_IS_REVOKED = "OCSP status is revoked.";

        public const String OCSP_STATUS_IS_UNKNOWN = "OCSP status is unknown.";

        public const String ONE_OF_GROUPED_SOURCES_CLOSING_FAILED = "Closing of one of the grouped sources failed.";

        public const String OPENTYPE_GDEF_TABLE_ERROR = "OpenType GDEF table error: {0}";

        public const String ORPHANS_CONSTRAINT_VIOLATED = "Orphans constraint violated for paragraph split at page {0}. Min number of orphans: {1}; actual: {2}."
             + " \nComment: {3}";

        public const String OUTLINE_DESTINATION_PAGE_NUMBER_IS_OUT_OF_BOUNDS = "Outline destination page number {0} is out of bounds";

        public const String PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE = "Page tree is broken. Failed to retrieve page number {0}. Null will be returned.";

        public const String PAGE_WAS_FLUSHED_ACTION_WILL_NOT_BE_PERFORMED = "Page was flushed. {0} will not be performed.";

        public const String PASSED_PAGE_SHALL_BE_ON_WHICH_CANVAS_WILL_BE_RENDERED = "The page passed to Canvas#enableAutoTagging(PdfPage) method shall be the one on which this canvas will "
             + "be rendered. However the actual passed PdfPage instance sets not such page. This might lead to " +
             "creation of malformed PDF document.";

        public const String PATH_KEY_IS_PRESENT_VERTICES_WILL_BE_IGNORED = "Path key is present. Vertices will be ignored";

        public const String PDF_OBJECT_FLUSHING_NOT_PERFORMED = "PdfObject flushing is not performed: PdfDocument is opened in append mode and the object is not marked "
             + "as modified ( see PdfObject#setModified() ).";

        public const String PDF_READER_CLOSING_FAILED = "PdfReader closing failed due to the error occurred!";

        public const String PDF_REFERS_TO_NOT_EXISTING_PROPERTY_DICTIONARY = "The PDF contains a BDC operator which refers to a not existing Property dictionary: {0}.";

        public const String PDF_WRITER_CLOSING_FAILED = "PdfWriter closing failed due to the error occurred!";

        public const String PNG_IMAGE_HAS_ICC_PROFILE_WITH_INCOMPATIBLE_NUMBER_OF_COLOR_COMPONENTS = "Png image has color profile with incompatible number of color components.";

        public const String POPUP_ENTRY_IS_NOT_POPUP_ANNOTATION = "Popup entry in the markup annotations refers not to the annotation with Popup subtype.";

        public const String PREMATURE_CALL_OF_HANDLE_VIOLATION_METHOD = "Premature call of handleViolation method.";

        public const String PROPERTY_IN_PERCENTS_NOT_SUPPORTED = "Property {0} in percents is not supported";

        public const String RECTANGLE_HAS_NEGATIVE_OR_ZERO_SIZES = "The {0} rectangle has negative or zero sizes. "
             + "It will not be displayed.";

        public const String RECTANGLE_HAS_NEGATIVE_SIZE = "The {0} rectangle has negative size. It will not be displayed.";

        public const String REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED = "The page requested to be removed has already been flushed.";

        public const String RENDERER_WAS_NOT_ABLE_TO_PROCESS_KEEP_WITH_NEXT = "The renderer was not able to process keep with next property properly";

        public const String ROLE_MAPPING_FROM_SOURCE_IS_NOT_COPIED_ALREADY_EXIST = "Role mapping \"{0}\" from source document is not copied. Destination document already has \"{1}\" "
             + "mapping.";

        public const String ROLE_MAPPING_FROM_SOURCE_IS_NOT_COPIED_INVALID = "Role mapping for \"{0}\" from source document is not copied. Mapping to namespace is in an invalid form "
             + "(should be [PdfName, PdfDictionary]).";

        public const String ROTATION_WAS_NOT_CORRECTLY_PROCESSED_FOR_RENDERER = "Rotation was not correctly processed for {0}";

        public const String SOME_TARGET_FIELDS_ARE_NOT_SET_OR_INCORRECT = "Some fields in target dictionary are not set or incorrect. Null will be returned.";

        public const String SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY = "Source document has AcroForm dictionary. The pages you are going to copy may have FormFields, but they "
             + "will not be copied, because you have not used any IPdfPageExtraCopier";

        public const String START_MARKER_MISSING_IN_PFB_FILE = "Start marker is missing in the pfb file";

        public const String STDSCRIPTCONFIG_DESIGNED_ONLY_FOR_STD_SCRIPTS = "StandardScriptConfig class is designed for only standard scripts, otherwise it might be incompatible.";

        public const String STRUCTURE_ELEMENT_REPLACED_BY_ITS_ID_IN_STRUCTURE_DESTINATION = "Structure destinations shall specify structure element ID in remote go-to actions. Structure element has "
             + "been replaced with its ID in the structure destination";

        public const String SUM_OF_TABLE_COLUMNS_IS_GREATER_THAN_100 = "Sum of table columns is greater than 100%.";

        public const String TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH = "Table width is more than expected due to min width of cell(s).";

        public const String TAG_STRUCTURE_INIT_FAILED = "Tag structure initialization failed, tag structure is ignored, it might be corrupted.";

        public const String TOUNICODE_CMAP_MORE_THAN_2_BYTES_NOT_SUPPORTED = "ToUnicode CMap more than 2 bytes not supported.";

        public const String TYPE3_FONT_CANNOT_BE_ADDED = "Type 3 font cannot be added to FontSet. Custom FontProvider class may be created for this purpose.";

        public const String TYPE3_FONT_ISSUE_TAGGED_PDF = "Type 3 font issue. Font Descriptor is required for tagged PDF. FontName shall be specified.";

        public const String TYPE3_FONT_INITIALIZATION_ISSUE = "Type 3 font issue. Font cannot be initialized correctly.";

        public const String TYPOGRAPHY_NOT_FOUND = "Cannot find pdfCalligraph module, which was implicitly " + "required by one of the layout properties";

        public const String UNABLE_TO_APPLY_PAGE_DEPENDENT_PROP_UNKNOWN_PAGE_ON_WHICH_ELEMENT_IS_DRAWN = "Unable to apply page dependent property, because the page on which element is drawn is unknown. Usually "
             + "this means that element was added to the Canvas instance that was created not with constructor " +
             "taking PdfPage as argument. Not processed property: {0}";

        public const String UNABLE_TO_INVERT_GRADIENT_TRANSFORMATION = "Unable to invert gradient transformation, "
             + "ignoring it";

        public const String UNEXPECTED_BEHAVIOUR_DURING_TABLE_ROW_COLLAPSING = "Unexpected behaviour during table row collapsing. Calculated rowspan was less then 1.";

        public const String UNEXPECTED_EVENT_HANDLER_SERVICE_THREAD_EXCEPTION = "Unexpected exception encountered in service thread. Shutting it down.";

        public const String UNKNOWN_CMAP = "Unknown CMap {0}";

        public const String UNKNOWN_COLOR_FORMAT_MUST_BE_RGB_OR_RRGGBB = "Unknown color format: must be rgb or rrggbb.";

        public const String UNKNOWN_ERROR_WHILE_PROCESSING_CMAP = "Unknown error while processing CMap.";

        public const String VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY = "\"{0}\" entry in the \"{1}\" dictionary is a {2} and higher version feature. It is meaningless for the "
             + "current {3} version.";

        public const String WIDOWS_CONSTRAINT_VIOLATED = "Widows constraint violated for paragraph split at page {0}. Min number of widows: {1}; actual:"
             + " {2}.\nComment: {3}";

        public const String WRITER_ENCRYPTION_IS_IGNORED_APPEND = "Writer encryption will be ignored, because append mode is used. Document will preserve the original "
             + "encryption (or will stay unencrypted)";

        public const String WRITER_ENCRYPTION_IS_IGNORED_PRESERVE = "Writer encryption will be ignored, because preservation of encryption is enabled. Document will preserve "
             + "the original encryption (or will stay unencrypted)";

        public const String WRONG_MEDIABOX_SIZE_TOO_MANY_ARGUMENTS = "Wrong media box size: {0}. The arguments beyond the 4th will be ignored";

        public const String XFDF_ANNOTATION_IS_NOT_SUPPORTED = "Xfdf annotation \"{0}\" is not supported";

        public const String XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT = "Xfdf href attribute and pdf document name are different!";

        public const String XFDF_NO_SUCH_FIELD_IN_PDF_DOCUMENT = "Xfdf no such field in pdf document!";

        public const String XFDF_NO_F_OBJECT_TO_COMPARE = "Xfdf no f object to compare.";

        public const String XFDF_UNSUPPORTED_ANNOTATION_ATTRIBUTE = "Xfdf unsupported attribute type";

        public const String XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT = "Error occurred while " + "reading cross reference table. Cross reference table will be rebuilt. No additional information available";

        public const String XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT_WITH_CAUSE = "Error occurred while " + 
            "reading cross reference table. Cross reference table will be rebuilt. Reason: {0}";

        private IoLogMessageConstant() {
        }
        //Private constructor will prevent the instantiation of this class directly
    }
}
