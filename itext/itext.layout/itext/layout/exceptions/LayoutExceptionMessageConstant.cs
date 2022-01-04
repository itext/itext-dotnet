/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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

namespace iText.Layout.Exceptions {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class LayoutExceptionMessageConstant {
        public const String CANNOT_ADD_CELL_TO_COMPLETED_LARGE_TABLE = "The large table was completed. It's " + "prohibited to use it anymore. Created different Table instance instead.";

        public const String CANNOT_CREATE_LAYOUT_IMAGE_BY_WMF_IMAGE = "Cannot create layout image by WmfImage " + 
            "instance. First convert the image into FormXObject and then use the corresponding layout image " + "constructor.";

        public const String CANNOT_DRAW_ELEMENTS_ON_ALREADY_FLUSHED_PAGES = "Cannot draw elements on already " + "flushed pages.";

        public const String DOCUMENT_CLOSED_IT_IS_IMPOSSIBLE_TO_EXECUTE_ACTION = "Document was closed. It is " + "impossible to execute action.";

        public const String FLEX_BASIS_CANNOT_BE_NULL = "Flex basis cannot be null.";

        public const String FLEX_GROW_CANNOT_BE_NEGATIVE = "Flex grow cannot be negative.";

        public const String FLEX_SHRINK_CANNOT_BE_NEGATIVE = "Flex shrink cannot be negative.";

        public const String FONT_PROVIDER_NOT_SET_FONT_FAMILY_NOT_RESOLVED = "FontProvider and FontSet are empty. "
             + "Cannot resolve font family name (see ElementPropertyContainer#setFontFamily) without initialized "
             + "FontProvider (see RootElement#setFontProvider).";

        public const String IO_EXCEPTION_WHILE_CREATING_FONT = "I/O exception while creating Font";

        public const String NONINVERTIBLE_MATRIX_CANNOT_BE_PROCESSED = "A noninvertible matrix has been parsed. " 
            + "The behaviour is unpredictable.";

        public const String ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE = "Role \"{0}\" is not mapped to any standard "
             + "role.";

        public const String ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE = "Role \"{0}\" in namespace {1} "
             + "is not mapped to any standard role.";

        private LayoutExceptionMessageConstant() {
        }
    }
}
