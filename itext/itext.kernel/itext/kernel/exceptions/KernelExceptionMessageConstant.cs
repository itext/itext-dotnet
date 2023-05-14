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

namespace iText.Kernel.Exceptions {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class KernelExceptionMessageConstant {
        public const String AMOUNT_OF_BYTES_LESS_THAN_ZERO = "Amount of bytes in the PDF document cannot be less than zero";

        public const String ANNOTATION_SHALL_HAVE_REFERENCE_TO_PAGE = "Annotation shall have reference to page.";

        public const String APPEND_MODE_REQUIRES_A_DOCUMENT_WITHOUT_ERRORS_EVEN_IF_RECOVERY_IS_POSSIBLE = "Append mode requires a document without errors, even if recovery is possible.";

        public const String BAD_CERTIFICATE_AND_KEY = "Bad public key certificate and/or private key.";

        public const String BAD_USER_PASSWORD = "Bad user password. Password is not provided or wrong password provided. Correct password should be passed "
             + "to PdfReader constructor with properties. See ReaderProperties#setPassword() method.";

        public const String BAD_PASSWORD_HASH = "Password hash exceeds 48 bytes and extra bytes are not 0";

        public const String CANNOT_ADD_KID_TO_THE_FLUSHED_ELEMENT = "Cannot add kid to the flushed element.";

        public const String CANNOT_BE_EMBEDDED_DUE_TO_LICENSING_RESTRICTIONS = "{0} cannot be embedded due to licensing restrictions.";

        public const String CANNOT_CLOSE_DOCUMENT = "Cannot close document.";

        public const String CANNOT_CLOSE_DOCUMENT_WITH_ALREADY_FLUSHED_PDF_CATALOG = "Cannot close document with already flushed PDF Catalog.";

        public const String CANNOT_CONVERT_PDF_ARRAY_TO_AN_ARRAY_OF_BOOLEANS = "Cannot convert PdfArray to an array of booleans";

        public const String CANNOT_CONVERT_PDF_ARRAY_TO_DOUBLE_ARRAY = "Cannot convert PdfArray to an array " + "of doubles.";

        public const String CANNOT_CONVERT_PDF_ARRAY_TO_INT_ARRAY = "Cannot convert PdfArray to an array " + "of integers.";

        public const String CANNOT_CONVERT_PDF_ARRAY_TO_FLOAT_ARRAY = "Cannot convert PdfArray to an array " + "of floats.";

        public const String CANNOT_CONVERT_PDF_ARRAY_TO_LONG_ARRAY = "Cannot convert PdfArray to an array of longs.";

        public const String CANNOT_CONVERT_PDF_ARRAY_TO_RECTANGLE = "Cannot convert PdfArray to Rectangle.";

        public const String CANNOT_COPY_FLUSHED_OBJECT = "Cannot copy flushed object.";

        public const String CANNOT_COPY_FLUSHED_TAG = "Cannot copy flushed tag.";

        public const String CANNOT_COPY_OBJECT_CONTENT = "Cannot copy object content.";

        public const String CANNOT_COPY_INDIRECT_OBJECT_FROM_THE_DOCUMENT_THAT_IS_BEING_WRITTEN = "Cannot copy " +
             "indirect object from the document that is being written.";

        public const String CANNOT_COPY_TO_DOCUMENT_OPENED_IN_READING_MODE = "Cannot copy to document opened in " 
            + "reading mode.";

        public const String CANNOT_CREATE_FONT_FROM_NULL_PDF_DICTIONARY = "Cannot create font from null pdf dictionary.";

        public const String CANNOT_CREATE_PDF_IMAGE_XOBJECT_BY_WMF_IMAGE = "Cannot create PdfImageXObject instance "
             + "by WmfImage. Use PdfFormXObject constructor instead.";

        public const String CANNOT_CREATE_PDFSTREAM_BY_INPUT_STREAM_WITHOUT_PDF_DOCUMENT = "Cannot create pdfstream "
             + "by InputStream without PdfDocument.";

        public const String CANNOT_CREATE_TYPE_0_FONT_WITH_TRUE_TYPE_FONT_PROGRAM_WITHOUT_EMBEDDING_IT = "Cannot create Type0 font with true type font program without embedding it.";

        public const String CANNOT_EMBED_STANDARD_FONT = "Standard fonts cannot be embedded.";

        public const String CANNOT_EMBED_TYPE_0_FONT_WITH_CID_FONT_PROGRAM = "Cannot embed Type0 font with CID font program based on non-generic predefined CMap.";

        public const String CANNOT_GET_CONTENT_BYTES = "Cannot get content bytes.";

        public const String CANNOT_GET_PDF_STREAM_BYTES = "Cannot get PdfStream bytes.";

        public const String CANNOT_OPERATE_WITH_FLUSHED_PDF_STREAM = "Cannot operate with the flushed PdfStream.";

        public const String CANNOT_RETRIEVE_MEDIA_BOX_ATTRIBUTE = "Invalid PDF. There is no media box attribute " 
            + "for page or its parents.";

        public const String CANNOT_FIND_IMAGE_DATA_OR_EI = "Cannot find image data or EI.";

        public const String CANNOT_FLUSH_DOCUMENT_ROOT_TAG_BEFORE_DOCUMENT_IS_CLOSED = "Cannot flush document root "
             + "tag before document is closed.";

        public const String CANNOT_FLUSH_OBJECT = "Cannot flush object.";

        public const String CANNOT_MOVE_FLUSHED_TAG = "Cannot move flushed tag";

        public const String CANNOT_MOVE_TO_FLUSHED_KID = "Cannot move to flushed kid.";

        public const String CANNOT_MOVE_TO_MARKED_CONTENT_REFERENCE = "Cannot move to marked content reference.";

        public const String CANNOT_MOVE_TO_PARENT_CURRENT_ELEMENT_IS_ROOT = "Cannot move to parent current element "
             + "is root.";

        public const String CANNOT_MOVE_PAGES_IN_PARTLY_FLUSHED_DOCUMENT = "Cannot move pages in partly flushed " 
            + "document. Page number {0} is already flushed.";

        public const String CANNOT_OPEN_DOCUMENT = "Cannot open document.";

        public const String CANNOT_PARSE_CONTENT_STREAM = "Cannot parse content stream.";

        public const String CANNOT_READ_A_STREAM_IN_ORDER_TO_APPEND_NEW_BYTES = "Cannot read a stream in order to "
             + "append new bytes.";

        public const String CANNOT_READ_PDF_OBJECT = "Cannot read PdfObject.";

        public const String CANNOT_RECOGNISE_DOCUMENT_FONT_WITH_ENCODING = "Cannot recognise document font {0} " +
             "with {1} encoding";

        public const String CANNOT_RELOCATE_ROOT_TAG = "Cannot relocate root tag.";

        public const String CANNOT_RELOCATE_TAG_WHICH_IS_ALREADY_FLUSHED = "Cannot relocate tag which is already "
             + "flushed.";

        public const String CANNOT_RELOCATE_TAG_WHICH_PARENT_IS_ALREADY_FLUSHED = "Cannot relocate tag which " + "parent is already flushed.";

        public const String CANNOT_REMOVE_DOCUMENT_ROOT_TAG = "Cannot remove document root tag.";

        public const String CANNOT_REMOVE_MARKED_CONTENT_REFERENCE_BECAUSE_ITS_PAGE_WAS_ALREADY_FLUSHED = "Cannot "
             + "remove marked content reference, because its page has been already flushed.";

        public const String CANNOT_REMOVE_TAG_BECAUSE_ITS_PARENT_IS_FLUSHED = "Cannot remove tag, because its " + 
            "parent is flushed.";

        public const String CANNOT_SET_DATA_TO_PDF_STREAM_WHICH_WAS_CREATED_BY_INPUT_STREAM = "Cannot set data to "
             + "PdfStream which was created by InputStream.";

        public const String CANNOT_SET_ENCRYPTED_PAYLOAD_TO_DOCUMENT_OPENED_IN_READING_MODE = "Cannot set " + "encrypted payload to a document opened in read only mode.";

        public const String CANNOT_SET_ENCRYPTED_PAYLOAD_TO_ENCRYPTED_DOCUMENT = "Cannot set encrypted payload " +
             "to an encrypted document.";

        public const String CANNOT_SPLIT_DOCUMENT_THAT_IS_BEING_WRITTEN = "Cannot split document that is " + "being written.";

        public const String CANNOT_WRITE_TO_PDF_STREAM = "Cannot write to PdfStream.";

        public const String CANNOT_WRITE_OBJECT_AFTER_IT_WAS_RELEASED = "Cannot write object after it was " + "released. In normal situation the object must be read once again before being written.";

        public const String CERTIFICATE_IS_NOT_PROVIDED_DOCUMENT_IS_ENCRYPTED_WITH_PUBLIC_KEY_CERTIFICATE = "Certificate is not provided. Document is encrypted with public key certificate, it should be passed "
             + "to PdfReader constructor with properties. See ReaderProperties#setPublicKeySecurityParams() " + "method.";

        public const String CF_NOT_FOUND_ENCRYPTION = "/CF not found (encryption)";

        public const String COLOR_SPACE_NOT_FOUND = "ColorSpace not found.";

        public const String CONTENT_STREAM_MUST_NOT_INVOKE_OPERATORS_THAT_SPECIFY_COLORS_OR_OTHER_COLOR_RELATED_PARAMETERS
             = "Content stream must not invoke operators that specify colors or other color related parameters in "
             + "the graphics state.";

        public const String CORRUPTED_OUTLINE_DICTIONARY_HAS_INFINITE_LOOP = "Document outline dictionary is corrupted: some outline (PDF object: \"{0}\") has wrong first/next link "
             + "entry.";

        public const String CORRUPTED_OUTLINE_NO_PARENT_ENTRY = "Document outline is corrupted: some outline (PDF object: \"{0}\") lacks the required parent entry.";

        public const String CORRUPTED_OUTLINE_NO_TITLE_ENTRY = "Document outline is corrupted: some outline (PDF object: \"{0}\") lacks the required title entry.";

        public const String CORRUPTED_ROOT_ENTRY_IN_TRAILER = "The trailer is corrupted: the catalog " + "is corrupted or cannot be referenced from the file's trailer. The PDF cannot be opened.";

        public const String DATA_HANDLER_COUNTER_HAS_BEEN_DISABLED = "Data handler counter has been disabled";

        public const String DEFAULT_CRYPT_FILTER_NOT_FOUND_ENCRYPTION = "/DefaultCryptFilter not found " + "(encryption).";

        public const String THIS_DICTIONARY_KEY_IS_NOT_A_NAME = "Dictionary key {0} is not a name.";

        public const String DICTIONARY_DOES_NOT_HAVE_SUPPORTED_FONT_DATA = "Dictionary doesn't have supported font "
             + "data.";

        public const String DOCUMENT_CLOSED_IT_IS_IMPOSSIBLE_TO_EXECUTE_ACTION = "Document was closed. It is " + "impossible to execute action.";

        public const String DOCUMENT_DOES_NOT_CONTAIN_STRUCT_TREE_ROOT = "Document doesn't contain StructTreeRoot.";

        public const String DOCUMENT_HAS_NO_PDF_CATALOG_OBJECT = "Document has no PDF Catalog object.";

        public const String DOCUMENT_HAS_NOT_BEEN_READ_YET = "The PDF document has not been read yet. Document " +
             "reading occurs in PdfDocument class constructor";

        public const String DOCUMENT_FOR_COPY_TO_CANNOT_BE_NULL = "Document for copyTo cannot be null.";

        public const String DURING_DECOMPRESSION_MULTIPLE_STREAMS_IN_SUM_OCCUPIED_MORE_MEMORY_THAN_ALLOWED = "" + 
            "During decompression multiple streams in sum occupied more memory than allowed. Please either check "
             + "your pdf or increase the allowed single decompressed pdf stream maximum size value by setting " + 
            "the appropriate parameter of ReaderProperties's MemoryLimitsAwareHandler.";

        public const String DURING_DECOMPRESSION_SINGLE_STREAM_OCCUPIED_MORE_MEMORY_THAN_ALLOWED = "During " + "decompression a single stream occupied more memory than allowed. Please either check your pdf or "
             + "increase the allowed multiple decompressed pdf streams maximum size value by setting the appropriate "
             + "parameter of ReaderProperties's MemoryLimitsAwareHandler.";

        public const String DURING_DECOMPRESSION_SINGLE_STREAM_OCCUPIED_MORE_THAN_MAX_INTEGER_VALUE = "During " + 
            "decompression a single stream occupied more than a maximum integer value. Please check your pdf.";

        public const String END_OF_CONTENT_STREAM_REACHED_BEFORE_END_OF_IMAGE_DATA = "End of content stream " + "reached before end of image data.";

        public const String ERROR_WHILE_INITIALIZING_AES_CIPHER = "Exception initializing AES cipher.";

        public const String ERROR_WHILE_READING_OBJECT_STREAM = "Error while reading Object Stream.";

        public const String ENCRYPTED_PAYLOAD_FILE_SPEC_DOES_NOT_HAVE_ENCRYPTED_PAYLOAD_DICTIONARY = "Encrypted " 
            + "payload file spec shall have encrypted payload dictionary.";

        public const String ENCRYPTED_PAYLOAD_FILE_SPEC_SHALL_BE_INDIRECT = "Encrypted payload file spec shall " +
             "be indirect.";

        public const String ENCRYPTED_PAYLOAD_FILE_SPEC_SHALL_HAVE_EF_DICTIONARY = "Encrypted payload file spec " 
            + "shall have 'EF' key. The value of such key shall be a dictionary that contains embedded file stream.";

        public const String ENCRYPTED_PAYLOAD_FILE_SPEC_SHALL_HAVE_TYPE_EQUAL_TO_FILESPEC = "Encrypted payload file "
             + "spec shall have 'Type' key. The value of such key shall be 'Filespec'.";

        public const String ENCRYPTED_PAYLOAD_SHALL_HAVE_TYPE_EQUALS_TO_ENCRYPTED_PAYLOAD_IF_PRESENT = "Encrypted "
             + "payload dictionary shall have field 'Type' equal to 'EncryptedPayload' if present";

        public const String ENCRYPTED_PAYLOAD_SHALL_HAVE_SUBTYPE = "Encrypted payload shall have 'Subtype' field "
             + "specifying crypto filter";

        public const String EXTERNAL_ENTITY_ELEMENT_FOUND_IN_XML = "External entity element found in XML. This entity will not be parsed to prevent XML attacks.";

        public const String FILE_POSITION_0_CROSS_REFERENCE_ENTRY_IN_THIS_XREF_SUBSECTION = "file position 0 cross "
             + "reference entry in this xref subsection.";

        public const String FILTER_CCITTFAXDECODE_IS_ONLY_SUPPORTED_FOR_IMAGES = "Filter CCITTFaxDecode is only " 
            + "supported for images";

        public const String FILTER_IS_NOT_A_NAME_OR_ARRAY = "filter is not a name or array.";

        public const String FLUSHED_PAGE_CANNOT_BE_ADDED_OR_INSERTED = "Flushed page cannot be added or inserted.";

        public const String FLUSHED_PAGE_CANNOT_BE_REMOVED = "Flushed page cannot be removed from a document which "
             + "is tagged or has an AcroForm";

        public const String FLUSHED_STREAM_FILTER_EXCEPTION = "Stream {0} {1} R contains flushed indirect filter object in encrypted document, try to use "
             + "PdfStream#flush() stream before PdfDocument#close() to prevent this exception";

        public const String FLUSHING_HELPER_FLUSHING_MODE_IS_NOT_FOR_DOC_READING_MODE = "Flushing writes the object "
             + "to the output stream and releases it from memory. It is only possible for documents that have a " 
            + "PdfWriter associated with them. Use PageFlushingHelper#releaseDeep method instead.";

        public const String FONT_AND_SIZE_MUST_BE_SET_BEFORE_WRITING_ANY_TEXT = "Font and size must be set before "
             + "writing any text.";

        public const String FONT_EMBEDDING_ISSUE = "Font embedding issue.";

        public const String FORM_XOBJECT_MUST_HAVE_BBOX = "Form XObject must have BBox.";

        public const String FUNCTION_IS_NOT_COMPATIBLE_WITH_COLOR_SPACE = "Function is not compatible with " + "ColorSpace.";

        public const String NO_SUCH_ALGORITHM_FOR_PROVIDER_BCFIPS = "no such algorithm: {0} for provider BCFIPS";

        public const String ILLEGAL_CHARACTER_IN_ASCIIHEXDECODE = "illegal character in ASCIIHexDecode.";

        public const String ILLEGAL_CHARACTER_IN_ASCII85DECODE = "Illegal character in ASCII85Decode.";

        public const String ILLEGAL_LENGTH_VALUE = "Illegal length value.";

        public const String ILLEGAL_R_VALUE = "Illegal R value.";

        public const String ILLEGAL_V_VALUE = "Illegal V value.";

        public const String IN_A_PAGE_LABEL_THE_PAGE_NUMBERS_MUST_BE_GREATER_OR_EQUAL_TO_1 = "In a page label the "
             + "page numbers must be greater or equal to 1.";

        public const String INCORRECT_NUMBER_OF_COMPONENTS = "Incorrect number of components.";

        public const String INVALID_CROSS_REFERENCE_ENTRY_IN_THIS_XREF_SUBSECTION = "Invalid cross reference entry "
             + "in this xref subsection.";

        public const String INVALID_INDIRECT_REFERENCE = "Invalid indirect reference {0}.";

        public const String INVALID_INPUT_FOR_TYPE_2_FUNCTION = "Invalid input value for PDF Type 2 Function, value should be a single number.";

        public const String INVALID_INPUT_FOR_TYPE_3_FUNCTION = "Invalid input value for PDF Type 3 Function, value should be a single number.";

        public const String INVALID_MEDIA_BOX_VALUE = "Tne media box object has incorrect values.";

        public const String INVALID_PAGE_STRUCTURE = "Invalid page structure {0}.";

        public const String INVALID_PAGE_STRUCTURE_PAGES_MUST_BE_PDF_DICTIONARY = "Invalid page structure. /Pages "
             + "must be PdfDictionary.";

        public const String INVALID_TYPE_2_FUNCTION_DOMAIN = "Invalid PDF Type 2 Function object, \"Domain\" array shall consist of 2 numbers.";

        public const String INVALID_TYPE_2_FUNCTION_N = "Invalid PDF Type 2 Function object, \"N\" field should be existed and must be a number.";

        public const String INVALID_TYPE_2_FUNCTION_N_NEGATIVE = "Invalid PDF Type 2 Function object, when \"N\" is negative, values of \"Domain\" shall "
             + "not allow zero as input value.";

        public const String INVALID_TYPE_2_FUNCTION_N_NOT_INTEGER = "Invalid PDF Type 2 Function object, when \"N\" is not an integer, values of \"Domain\" shall "
             + "define the input value as non-negative.";

        public const String INVALID_TYPE_2_FUNCTION_OUTPUT_SIZE = "Invalid PDF Type 2 Function object, if present the size of \"C0\", \"C1\" and half "
             + "\"Range\" shall be equal.";

        public const String INVALID_TYPE_3_FUNCTION_BOUNDS = "Invalid PDF Type 3 Function object, \"Bounds\" elements shall be in order of increasing value, and "
             + "each value shall be within the domain defined by \"Domain\".";

        public const String INVALID_TYPE_3_FUNCTION_DOMAIN = "Invalid PDF Type 3 Function object, \"Domain\" array shall consist of 2 numbers.";

        public const String INVALID_TYPE_3_FUNCTION_FUNCTIONS_OUTPUT = "Invalid PDF Type 3 Function object, the output dimensionality of all functions shall be the same, "
             + "and compatible with the value of \"Range\".";

        public const String INVALID_TYPE_3_FUNCTION_FUNCTIONS_INPUT = "Invalid PDF Type 3 Function object, all functions shall have 1 input value.";

        public const String INVALID_TYPE_3_FUNCTION_NULL_BOUNDS = "Invalid PDF Type 3 Function object, \"Bounds\" array should be exist and it size should corresponds "
             + "to the size of \"Functions\" array.";

        public const String INVALID_TYPE_3_FUNCTION_NULL_ENCODE = "Invalid PDF Type 3 Function object, \"Encode\" array should be exist and it size should be 2 times "
             + "more than \"Functions\" array size.";

        public const String INVALID_TYPE_3_FUNCTION_NULL_FUNCTIONS = "Invalid PDF Type 3 Function object, \"Functions\" array should be exist and can't be empty.";

        public const String INVALID_RANGE_ARRAY = "Invalid range array.";

        public const String INVALID_OFFSET_FOR_THIS_OBJECT = "Invalid offset for object {0}.";

        public const String INVALID_XREF_STREAM = "Invalid xref stream.";

        public const String INVALID_XREF_TABLE = "Invalid xref table.";

        public const String IO_EXCEPTION = "I/O exception.";

        public const String IO_EXCEPTION_WHILE_CREATING_FONT = "I/O exception while creating Font";

        public const String LZW_DECODER_EXCEPTION = "LZW decoder exception.";

        public const String LZW_FLAVOUR_NOT_SUPPORTED = "LZW flavour not supported.";

        public const String MISSING_REQUIRED_FIELD_IN_FONT_DICTIONARY = "Missing required field {0} in font dictionary.";

        public const String MUST_BE_A_TAGGED_DOCUMENT = "Must be a tagged document.";

        public const String NOT_A_VALID_PLACEABLE_WINDOWS_METAFILE = "{0} is not a valid placeable windows " + "metafile.";

        public const String NUMBER_OF_ENTRIES_IN_THIS_XREF_SUBSECTION_NOT_FOUND = "Number of entries in this xref "
             + "subsection not found.";

        public const String NO_COMPATIBLE_ENCRYPTION_FOUND = "No compatible encryption found.";

        public const String NO_GLYPHS_DEFINED_FOR_TYPE_3_FONT = "No glyphs defined for type3 font.";

        public const String NO_KID_WITH_SUCH_ROLE = "No kid with such role.";

        public const String NONINVERTIBLE_MATRIX_CANNOT_BE_PROCESSED = "A noninvertible matrix has been parsed. " 
            + "The behaviour is unpredictable.";

        public const String NOT_A_PLACEABLE_WINDOWS_METAFILE = "Not a placeable windows metafile.";

        public const String NOT_A_WMF_IMAGE = "Not a WMF image.";

        public const String NO_VALID_ENCRYPTION_MODE = "No valid encryption mode.";

        public const String NUMBER_OF_PAGES_CAN_NOT_BE_NEGATIVE = "Number of pages can not be negative.";

        public const String NUMBER_OF_BOOLEANS_IN_THE_ARRAY_DOES_NOT_CORRESPOND_WITH_THE_NUMBER_OF_FIELDS = "The "
             + "number of booleans in the array doesn't correspond with the number of fields.";

        public const String OBJECT_MUST_BE_INDIRECT_TO_WORK_WITH_THIS_WRAPPER = "Object must be indirect to work "
             + "with this wrapper.";

        public const String OBJECT_NUMBER_OF_THE_FIRST_OBJECT_IN_THIS_XREF_SUBSECTION_NOT_FOUND = "Object number "
             + "of the first object in this xref subsection not found.";

        public const String ONLY_IDENTITY_CMAPS_SUPPORTS_WITH_TRUETYPE = "Only Identity CMaps supports with " + "truetype";

        public const String ONLY_BMP_CAN_BE_WRAPPED_IN_WMF = "Only BMP can be wrapped in WMF.";

        public const String OPERATOR_EI_NOT_FOUND_AFTER_END_OF_IMAGE_DATA = "Operator EI not found after the end "
             + "of image data.";

        public const String ORDERING_SHOULD_BE_DETERMINED = "Ordering should be determined for CIDFont of Type0 font.";

        public const String PAGE_CANNOT_BE_ADDED_TO_DOCUMENT_BECAUSE_IT_BELONGS_TO_ANOTHER_DOCUMENT = "The passed page belongs to document {0} (page {1} of the document) and therefore cannot be added "
             + "to this document ({2}).";

        public const String PAGE_IS_NOT_SET_FOR_THE_PDF_TAG_STRUCTURE = "Page is not set for the pdf tag structure.";

        public const String PAGE_ALREADY_FLUSHED = "The page has been already flushed.";

        public const String PDF_ENCRYPTION = "PdfEncryption exception.";

        public const String PDF_DECRYPTION = "Exception occurred with PDF document decryption. One of the possible "
             + "reasons is wrong password or wrong public key certificate and private key.";

        public const String PDF_FORM_XOBJECT_HAS_INVALID_BBOX = "PdfFormXObject has invalid BBox.";

        public const String PDF_OBJECT_STREAM_REACH_MAX_SIZE = "PdfObjectStream reach max size.";

        public const String PDF_PAGES_TREE_COULD_BE_GENERATED_ONLY_ONCE = "PdfPages tree could be generated only "
             + "once.";

        public const String PDF_READER_HAS_BEEN_ALREADY_UTILIZED = "Given PdfReader instance has already been " + 
            "utilized. The PdfReader cannot be reused, please create a new instance.";

        public const String PDF_STARTXREF_IS_NOT_FOLLOWED_BY_A_NUMBER = "PDF startxref is not followed by a number.";

        public const String PDF_STARTXREF_NOT_FOUND = "PDF startxref not found.";

        public const String PDF_INDIRECT_OBJECT_BELONGS_TO_OTHER_PDF_DOCUMENT = "Pdf indirect object belongs to " 
            + "other PDF document. Copy object to current pdf document.";

        public const String PDF_VERSION_IS_NOT_VALID = "PDF version is not valid.";

        public const String PNG_FILTER_UNKNOWN = "PNG filter unknown.";

        public const String PRINT_SCALING_ENFORCE_ENTRY_INVALID = "/PrintScaling shall may appear in the Enforce "
             + "array only if the corresponding entry in the viewer preferences dictionary specifies a valid value "
             + "other than AppDefault";

        public const String REF_ARRAY_ITEMS_IN_STRUCTURE_ELEMENT_DICTIONARY_SHALL_BE_INDIRECT_OBJECTS = "Ref array "
             + "items in structure element dictionary shall be indirect objects.";

        public const String REQUESTED_PAGE_NUMBER_IS_OUT_OF_BOUNDS = "Requested page number {0} is out of bounds.";

        public const String ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE = "Role \"{0}\" is not mapped to any standard "
             + "role.";

        public const String ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE = "Role \"{0}\" in namespace {1} "
             + "is not mapped to any standard role.";

        public const String RESOURCES_CANNOT_BE_NULL = "Resources cannot be null.";

        public const String RESOURCES_DO_NOT_CONTAIN_EXTGSTATE_ENTRY_UNABLE_TO_PROCESS_THIS_OPERATOR = "Resources "
             + "do not contain ExtGState entry. Unable to process operator {0}.";

        public const String SHADING_TYPE_NOT_FOUND = "Shading type not found.";

        public const String STDCF_NOT_FOUND_ENCRYPTION = "/StdCF not found (encryption)";

        public const String STREAM_SHALL_END_WITH_ENDSTREAM = "Stream shall end with endstream keyword.";

        public const String STRUCT_PARENT_INDEX_NOT_FOUND_IN_TAGGED_OBJECT = "StructParent index not found in " + 
            "tagged object.";

        public const String STRUCTURE_ELEMENT_IN_STRUCTURE_DESTINATION_SHALL_BE_AN_INDIRECT_OBJECT = "Structure " 
            + "element referenced by a structure destination shall be an indirect object.";

        public const String STRUCTURE_ELEMENT_SHALL_CONTAIN_PARENT_OBJECT = "StructureElement shall contain parent "
             + "object.";

        public const String STRUCTURE_ELEMENT_DICTIONARY_SHALL_BE_AN_INDIRECT_OBJECT_IN_ORDER_TO_HAVE_CHILDREN = ""
             + "Structure element dictionary shall be an indirect object in order to have children.";

        public const String TAG_CANNOT_BE_MOVED_TO_THE_ANOTHER_DOCUMENTS_TAG_STRUCTURE = "Tag cannot be moved to "
             + "the another document's tag structure.";

        public const String TAG_FROM_THE_EXISTING_TAG_STRUCTURE_IS_FLUSHED_CANNOT_ADD_COPIED_PAGE_TAGS = "Tag from "
             + "the existing tag structure is flushed. Cannot add copied page tags.";

        public const String TAG_STRUCTURE_COPYING_FAILED_IT_MIGHT_BE_CORRUPTED_IN_ONE_OF_THE_DOCUMENTS = "Tag " + 
            "structure copying failed: it might be corrupted in one of the documents.";

        public const String TAG_STRUCTURE_FLUSHING_FAILED_IT_MIGHT_BE_CORRUPTED = "Tag structure flushing failed: "
             + "it might be corrupted.";

        public const String TAG_TREE_POINTER_IS_IN_INVALID_STATE_IT_POINTS_AT_FLUSHED_ELEMENT_USE_MOVE_TO_ROOT = ""
             + "TagTreePointer is in invalid state: it points at flushed element. Use TagTreePointer#moveToRoot.";

        public const String TAG_TREE_POINTER_IS_IN_INVALID_STATE_IT_POINTS_AT_REMOVED_ELEMENT_USE_MOVE_TO_ROOT = ""
             + "TagTreePointer is in invalid state: it points at removed element use TagTreePointer#moveToRoot.";

        public const String THERE_IS_NO_ASSOCIATE_PDF_WRITER_FOR_MAKING_INDIRECTS = "There is no associate " + "PdfWriter for making indirects.";

        public const String THIS_DECODE_PARAMETER_TYPE_IS_NOT_SUPPORTED = "Decode parameter type {0} is not " + "supported.";

        public const String THIS_FILTER_IS_NOT_SUPPORTED = "Filter {0} is not supported.";

        public const String TO_FLUSH_THIS_WRAPPER_UNDERLYING_OBJECT_MUST_BE_ADDED_TO_DOCUMENT = "To manually " + "flush this wrapper, you have to ensure that the object behind this wrapper is added to the document, "
             + "i.e. it has an indirect reference.";

        public const String TRAILER_NOT_FOUND = "Trailer not found.";

        public const String TRAILER_PREV_ENTRY_POINTS_TO_ITS_OWN_CROSS_REFERENCE_SECTION = "Trailer prev entry " +
             "points to its own cross reference section.";

        public const String UNACCEPTABLE_FIELD_VALUE = "{0} is not an acceptable value for the field {1}.";

        public const String UNBALANCED_BEGIN_END_MARKED_CONTENT_OPERATORS = "Unbalanced begin/end marked content "
             + "operators.";

        public const String UNBALANCED_LAYER_OPERATORS = "Unbalanced layer operators.";

        public const String UNBALANCED_SAVE_RESTORE_STATE_OPERATORS = "Unbalanced save restore state operators.";

        public const String UNEXPECTED_CHARACTER_FOUND_AFTER_ID_IN_INLINE_IMAGE = "Unexpected character {0} " + "found after ID in inline image.";

        public const String UNEXPECTED_COLOR_SPACE = "Unexpected ColorSpace: {0}.";

        public const String UNEXPECTED_END_OF_FILE = "Unexpected end of file.";

        public const String UNEXPECTED_TOKEN = "unexpected {0} was encountered.";

        public const String UNEXPECTED_SHADING_TYPE = "Unexpected shading type.";

        public const String UNKNOWN_ENCRYPTION_TYPE_R = "Unknown encryption type R == {0}.";

        public const String UNKNOWN_ENCRYPTION_TYPE_V = "Unknown encryption type V == {0}.";

        public const String UNKNOWN_GRAPHICS_STATE_DICTIONARY = "{0} is an unknown graphics state dictionary.";

        public const String UNKNOWN_PDF_EXCEPTION = "Unknown PdfException.";

        public const String UNSUPPORTED_ASN1_ENCODING = "Unknown ASN1-encoding {0}. Only DER and BER encodings are supported!";

        public const String UNSUPPORTED_FONT_EMBEDDING_STRATEGY = "Unsupported font embedding strategy.";

        public const String UNSUPPORTED_XOBJECT_TYPE = "Unsupported XObject type.";

        public const String WHEN_ADDING_OBJECT_REFERENCE_TO_THE_TAG_TREE_IT_MUST_BE_CONNECTED_TO_NOT_FLUSHED_OBJECT
             = "When adding object reference to the tag tree, it must be connected to not flushed object.";

        public const String WHITE_POINT_IS_INCORRECTLY_SPECIFIED = "White point is incorrectly specified.";

        public const String WMF_IMAGE_EXCEPTION = "WMF image exception.";

        public const String WRONG_MEDIA_BOX_SIZE_TOO_FEW_ARGUMENTS = "Wrong media box size: {0}. Need at least 4 "
             + "arguments";

        public const String XREF_PREV_SHALL_BE_DIRECT_NUMBER_OBJECT = "Prev pointer in xref structure shall be " +
             "direct number object.";

        public const String XREF_SUBSECTION_NOT_FOUND = "xref subsection not found.";

        public const String XREF_STREAM_HAS_CYCLED_REFERENCES = "Xref stream has cycled references. Prev pointer indicates an already visited xref stream.";

        public const String XREF_STRUCTURE_SIZE_EXCEEDED_THE_LIMIT = "Xref structure contains too many elements " 
            + "and may cause OOM exception. You can increase number of elements by setting custom " + "MemoryLimitsAwareHandler.";

        public const String XREF_TABLE_HAS_CYCLED_REFERENCES = "Xref table has cycled references. Prev pointer indicates an already visited xref table.";

        public const String YOU_HAVE_TO_DEFINE_A_BOOLEAN_ARRAY_FOR_THIS_COLLECTION_SORT_DICTIONARY = "You have to "
             + "define a boolean array for this collection sort dictionary.";

        public const String YOU_MUST_SET_A_VALUE_BEFORE_ADDING_A_PREFIX = "You must set a value before adding a " 
            + "prefix.";

        public const String YOU_NEED_A_SINGLE_BOOLEAN_FOR_THIS_COLLECTION_SORT_DICTIONARY = "You need a single " +
             "boolean for this collection sort dictionary.";

        public const String QUAD_POINT_ARRAY_LENGTH_IS_NOT_A_MULTIPLE_OF_EIGHT = "The QuadPoint Array length is " 
            + "not a multiple of 8.";

        public const String FUNCTION_NOT_SET = "The output size depends on the functions, but no function is" + " set yet.";

        public const String INVALID_LENGTH = "The offset + length must be lower than or equal to the length of " +
             "the byte array.";

        public const String INVALID_LENGTH_FOR_WORDSIZE = "The length must be a multiple of {0}.";

        public const String TYPE4_EXECUTION_NOT_SUPPORTED = "Type 4 functions are not yet supported for separation color image extraction.";

        public const String GET_IMAGEBYTES_FOR_SEPARATION_COLOR_ONLY_SUPPORTS_RGB = "Only RGB alternate color spaces are currently supported for extracting separation color images";

        public const String INPUT_NOT_MULTIPLE_OF_DOMAIN_SIZE = "The size of the input array must be a multiple of "
             + "the domain size";

        public const String INPUT_NOT_MULTIPLE_OF_RANGE_SIZE = "The size of the input array must be a multiple of "
             + "the range size";

        public const String PDF_TYPE0_FUNCTION_NOT_NULL_PARAMETERS = "Domain, range and size must be not null";

        public const String PDF_TYPE0_FUNCTION_INVALID_ORDER = "Order must be equal to 1 or 3";

        public const String PDF_TYPE0_FUNCTION_INVALID_DOMAIN = "Invalid domain for PDF function of type 0";

        public const String PDF_TYPE0_FUNCTION_INVALID_RANGE = "Invalid encode array for PDF function of type 0";

        public const String PDF_TYPE0_FUNCTION_INVALID_SIZE = "Invalid size array for PDF function of type 0";

        public const String PDF_TYPE0_FUNCTION_INVALID_ENCODE = "Invalid encode array for PDF function of type 0";

        public const String PDF_TYPE0_FUNCTION_INVALID_DECODE = "Invalid decode array for PDF function of type 0";

        public const String PDF_TYPE0_FUNCTION_INVALID_SAMPLES = "Invalid samples array for PDF function of type 0";

        public const String PDF_TYPE0_FUNCTION_BITS_PER_SAMPLE_INVALID_VALUE = "bitsPerSample value must be from {1, 2, 4, 8, 12, 16, 24, 32}";

        public const String FUCTIONFACTORY_INVALID_OBJECT_TYPE = "Invalid object type, a function must be either a Dictionary or a Stream";

        public const String FUCTIONFACTORY_INVALID_FUNCTION_TYPE = "Invalid function type {0}";

        public const String FUCTIONFACTORY_INVALID_OBJECT_TYPE_TYPE4 = "Invalid object type, a function type 4 requires a stream object";

        public const String FUCTIONFACTORY_INVALID_OBJECT_TYPE_TYPE0 = "Invalid object type, a function type 0 requires a stream object";

        public const String UNSUPPORTED_SECURITY_HANDLER = "Failed to open the document. Security handler {0} is not supported";

        private KernelExceptionMessageConstant() {
        }
    }
}
