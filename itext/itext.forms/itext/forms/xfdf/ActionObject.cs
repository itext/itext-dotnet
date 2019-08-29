/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
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
using iText.Kernel.Pdf;

namespace iText.Forms.Xfdf {
    /// <summary>Represent Action tag in xfdf document structure.</summary>
    /// <remarks>
    /// Represent Action tag in xfdf document structure.
    /// Content model: ( URI | Launch | GoTo | GoToR | Named ).
    /// Attributes: none.
    /// For more details see paragraph 6.5.1 in Xfdf specification.
    /// </remarks>
    public class ActionObject {
        /// <summary>Type of inner action element.</summary>
        /// <remarks>Type of inner action element. Possible values: URI, Launch, GoTo, GoToR, Named.</remarks>
        private PdfName type;

        /// <summary>Represents Name required attribute of URI element.</summary>
        /// <remarks>Represents Name required attribute of URI element. For more details see paragraph 6.5.30 in Xfdf specification.
        ///     </remarks>
        private PdfString uri;

        /// <summary>Represents IsMap optional attribute of URI element.</summary>
        /// <remarks>Represents IsMap optional attribute of URI element. For more details see paragraph 6.5.30 in Xfdf specification.
        ///     </remarks>
        private bool isMap;

        /// <summary>Represents Name required attribute of Named element.</summary>
        /// <remarks>Represents Name required attribute of Named element. For more details see paragraph 6.5.24 in Xfdf specification.
        ///     </remarks>
        private PdfName nameAction;

        /// <summary>Represents OriginalName required attribute of File inner element of GoToR or Launch element.</summary>
        /// <remarks>
        /// Represents OriginalName required attribute of File inner element of GoToR or Launch element.
        /// Corresponds to F key in go-to action or launch dictionaries.
        /// For more details see paragraphs 6.5.11, 6.5.23 in Xfdf specification.
        /// </remarks>
        private String fileOriginalName;

        /// <summary>Represents NewWindows optional attribute of Launch element.</summary>
        /// <remarks>Represents NewWindows optional attribute of Launch element. For more details see paragraph 6.5.23 in Xfdf specification.
        ///     </remarks>
        private bool isNewWindow;

        /// <summary>Represents Dest inner element of link, GoTo, and GoToR elements.</summary>
        /// <remarks>
        /// Represents Dest inner element of link, GoTo, and GoToR elements.
        /// Corresponds to Dest key in link annotation dictionary.
        /// For more details see paragraph 6.5.10 in Xfdf specification.
        /// </remarks>
        private DestObject destination;

        public ActionObject(PdfName type) {
            this.type = type;
        }

        public virtual PdfName GetType() {
            return type;
        }

        public virtual iText.Forms.Xfdf.ActionObject SetType(PdfName type) {
            this.type = type;
            return this;
        }

        public virtual PdfString GetUri() {
            return uri;
        }

        public virtual iText.Forms.Xfdf.ActionObject SetUri(PdfString uri) {
            this.uri = uri;
            return this;
        }

        public virtual bool IsMap() {
            return isMap;
        }

        public virtual iText.Forms.Xfdf.ActionObject SetMap(bool map) {
            isMap = map;
            return this;
        }

        public virtual PdfName GetNameAction() {
            return nameAction;
        }

        public virtual iText.Forms.Xfdf.ActionObject SetNameAction(PdfName nameAction) {
            this.nameAction = nameAction;
            return this;
        }

        public virtual String GetFileOriginalName() {
            return fileOriginalName;
        }

        public virtual iText.Forms.Xfdf.ActionObject SetFileOriginalName(String fileOriginalName) {
            this.fileOriginalName = fileOriginalName;
            return this;
        }

        public virtual bool IsNewWindow() {
            return isNewWindow;
        }

        public virtual iText.Forms.Xfdf.ActionObject SetNewWindow(bool newWindow) {
            isNewWindow = newWindow;
            return this;
        }

        public virtual DestObject GetDestination() {
            return destination;
        }

        public virtual iText.Forms.Xfdf.ActionObject SetDestination(DestObject destination) {
            this.destination = destination;
            return this;
        }
    }
}
