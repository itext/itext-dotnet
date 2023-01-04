/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
        /// <remarks>
        /// Type of inner action element. Possible values:
        /// <see cref="iText.Kernel.Pdf.PdfName.URI"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Launch"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.GoTo"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.GoToR"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Named"/>.
        /// </remarks>
        private PdfName type;

        /// <summary>Represents Name, required attribute of URI element.</summary>
        /// <remarks>Represents Name, required attribute of URI element. For more details see paragraph 6.5.30 in Xfdf specification.
        ///     </remarks>
        private PdfString uri;

        /// <summary>Represents IsMap, optional attribute of URI element.</summary>
        /// <remarks>Represents IsMap, optional attribute of URI element. For more details see paragraph 6.5.30 in Xfdf specification.
        ///     </remarks>
        private bool isMap;

        /// <summary>Represents Name, required attribute of Named element.</summary>
        /// <remarks>Represents Name, required attribute of Named element. For more details see paragraph 6.5.24 in Xfdf specification.
        ///     </remarks>
        private PdfName nameAction;

        /// <summary>Represents OriginalName required attribute of File inner element of GoToR or Launch element.</summary>
        /// <remarks>
        /// Represents OriginalName required attribute of File inner element of GoToR or Launch element.
        /// Corresponds to F key in go-to action or launch dictionaries.
        /// For more details see paragraphs 6.5.11, 6.5.23 in Xfdf specification.
        /// </remarks>
        private String fileOriginalName;

        /// <summary>Represents NewWindow, optional attribute of Launch element.</summary>
        /// <remarks>Represents NewWindow, optional attribute of Launch element. For more details see paragraph 6.5.23 in Xfdf specification.
        ///     </remarks>
        private bool isNewWindow;

        /// <summary>Represents Dest, inner element of link, GoTo, and GoToR elements.</summary>
        /// <remarks>
        /// Represents Dest, inner element of link, GoTo, and GoToR elements.
        /// Corresponds to Dest key in link annotation dictionary.
        /// For more details see paragraph 6.5.10 in Xfdf specification.
        /// </remarks>
        private DestObject destination;

        public ActionObject(PdfName type) {
            this.type = type;
        }

        /// <summary>Returns the type of inner action element.</summary>
        /// <remarks>
        /// Returns the type of inner action element. Possible values:
        /// <see cref="iText.Kernel.Pdf.PdfName.URI"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Launch"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.GoTo"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.GoToR"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Named"/>.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// type of inner action element
        /// </returns>
        public virtual PdfName GetType() {
            return type;
        }

        /// <summary>Sets the type of inner action element.</summary>
        /// <remarks>
        /// Sets the type of inner action element. Possible values:
        /// <see cref="iText.Kernel.Pdf.PdfName.URI"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Launch"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.GoTo"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.GoToR"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Named"/>.
        /// </remarks>
        /// <param name="type">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// type of inner action object
        /// </param>
        /// <returns>
        /// current
        /// <see cref="ActionObject"/>
        /// </returns>
        public virtual iText.Forms.Xfdf.ActionObject SetType(PdfName type) {
            this.type = type;
            return this;
        }

        /// <summary>Gets the string value of URI elements.</summary>
        /// <remarks>
        /// Gets the string value of URI elements. Corresponds to Name, required attribute of URI element.
        /// For more details see paragraph 6.5.30 in Xfdf specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// value of URI element
        /// </returns>
        public virtual PdfString GetUri() {
            return uri;
        }

        /// <summary>Sets the string value of URI element.</summary>
        /// <remarks>
        /// Sets the string value of URI element. Corresponds to Name, required attribute of URI element.
        /// For more details see paragraph 6.5.30 in Xfdf specification.
        /// </remarks>
        /// <param name="uri">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// value to be set to URI element
        /// </param>
        /// <returns>
        /// current
        /// <see cref="ActionObject"/>
        /// </returns>
        public virtual iText.Forms.Xfdf.ActionObject SetUri(PdfString uri) {
            this.uri = uri;
            return this;
        }

        /// <summary>Gets IsMap, optional attribute of URI element.</summary>
        /// <remarks>Gets IsMap, optional attribute of URI element. For more details see paragraph 6.5.30 in Xfdf specification.
        ///     </remarks>
        /// <returns>boolean indicating if URI element is a map</returns>
        public virtual bool IsMap() {
            return isMap;
        }

        /// <summary>Sets IsMap, optional attribute of URI element.</summary>
        /// <remarks>Sets IsMap, optional attribute of URI element. For more details see paragraph 6.5.30 in Xfdf specification.
        ///     </remarks>
        /// <param name="map">boolean indicating if URI element is a map</param>
        /// <returns>
        /// current
        /// <see cref="ActionObject"/>
        /// </returns>
        public virtual iText.Forms.Xfdf.ActionObject SetMap(bool map) {
            isMap = map;
            return this;
        }

        /// <summary>Gets the value of Name, required attribute of Named element.</summary>
        /// <remarks>Gets the value of Name, required attribute of Named element. For more details see paragraph 6.5.24 in Xfdf specification.
        ///     </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// value of Name attribute of a named action element
        /// </returns>
        public virtual PdfName GetNameAction() {
            return nameAction;
        }

        /// <summary>Sets the value of Name, required attribute of Named element.</summary>
        /// <remarks>Sets the value of Name, required attribute of Named element. For more details see paragraph 6.5.24 in Xfdf specification.
        ///     </remarks>
        /// <param name="nameAction">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// value to be set to Name attribute of a named action element
        /// </param>
        /// <returns>
        /// current
        /// <see cref="ActionObject"/>
        /// </returns>
        public virtual iText.Forms.Xfdf.ActionObject SetNameAction(PdfName nameAction) {
            this.nameAction = nameAction;
            return this;
        }

        /// <summary>Gets the string value of OriginalName, required attribute of File inner element of GoToR or Launch element.
        ///     </summary>
        /// <remarks>
        /// Gets the string value of OriginalName, required attribute of File inner element of GoToR or Launch element.
        /// Corresponds to F key in go-to action or launch dictionaries.
        /// For more details see paragraphs 6.5.11, 6.5.23 in Xfdf specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// value of OriginalName attribute of current action object
        /// </returns>
        public virtual String GetFileOriginalName() {
            return fileOriginalName;
        }

        /// <summary>Sets the string value of OriginalName, required attribute of File inner element of GoToR or Launch element.
        ///     </summary>
        /// <remarks>
        /// Sets the string value of OriginalName, required attribute of File inner element of GoToR or Launch element.
        /// Corresponds to F key in go-to action or launch dictionaries.
        /// For more details see paragraphs 6.5.11, 6.5.23 in Xfdf specification.
        /// </remarks>
        /// <param name="fileOriginalName">
        /// 
        /// <see cref="System.String"/>
        /// value of OriginalName attribute of action object
        /// </param>
        /// <returns>
        /// current
        /// <see cref="ActionObject"/>
        /// </returns>
        public virtual iText.Forms.Xfdf.ActionObject SetFileOriginalName(String fileOriginalName) {
            this.fileOriginalName = fileOriginalName;
            return this;
        }

        /// <summary>Gets the boolean value of NewWindow, optional attribute of Launch element.</summary>
        /// <remarks>Gets the boolean value of NewWindow, optional attribute of Launch element. For more details see paragraph 6.5.23 in Xfdf specification.
        ///     </remarks>
        /// <returns>boolean indicating if current Launch action element should be opened in a new window</returns>
        public virtual bool IsNewWindow() {
            return isNewWindow;
        }

        /// <summary>Sets the boolean value of NewWindow, optional attribute of Launch element.</summary>
        /// <remarks>Sets the boolean value of NewWindow, optional attribute of Launch element. For more details see paragraph 6.5.23 in Xfdf specification.
        ///     </remarks>
        /// <param name="newWindow">boolean indicating if current Launch action element should be opened in a new window
        ///     </param>
        /// <returns>
        /// current
        /// <see cref="ActionObject"/>
        /// </returns>
        public virtual iText.Forms.Xfdf.ActionObject SetNewWindow(bool newWindow) {
            isNewWindow = newWindow;
            return this;
        }

        /// <summary>Gets Dest, inner element of link, GoTo, and GoToR elements.</summary>
        /// <remarks>
        /// Gets Dest, inner element of link, GoTo, and GoToR elements.
        /// Corresponds to Dest key in link annotation dictionary.
        /// For more details see paragraph 6.5.10 in Xfdf specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="DestObject"/>
        /// destination attribute of current action element
        /// </returns>
        public virtual DestObject GetDestination() {
            return destination;
        }

        /// <summary>Sets Dest, inner element of link, GoTo, and GoToR elements.</summary>
        /// <remarks>
        /// Sets Dest, inner element of link, GoTo, and GoToR elements.
        /// Corresponds to Dest key in link annotation dictionary.
        /// For more details see paragraph 6.5.10 in Xfdf specification.
        /// </remarks>
        /// <param name="destination">
        /// 
        /// <see cref="DestObject"/>
        /// destination attribute of the action element
        /// </param>
        /// <returns>
        /// current
        /// <see cref="ActionObject"/>
        /// </returns>
        public virtual iText.Forms.Xfdf.ActionObject SetDestination(DestObject destination) {
            this.destination = destination;
            return this;
        }
    }
}
