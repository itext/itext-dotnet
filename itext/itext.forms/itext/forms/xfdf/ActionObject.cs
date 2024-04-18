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

        /// <summary>
        /// Creates an instance of
        /// <see cref="ActionObject"/>.
        /// </summary>
        /// <param name="type">
        /// type of inner action element. Possible values:
        /// <see cref="iText.Kernel.Pdf.PdfName.URI"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Launch"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.GoTo"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.GoToR"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.PdfName.Named"/>
        /// </param>
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
        /// type of inner action element.
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
        /// <see cref="ActionObject"/>.
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
        /// value of URI element.
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
        /// <see cref="ActionObject"/>.
        /// </returns>
        public virtual iText.Forms.Xfdf.ActionObject SetUri(PdfString uri) {
            this.uri = uri;
            return this;
        }

        /// <summary>Gets IsMap, optional attribute of URI element.</summary>
        /// <remarks>Gets IsMap, optional attribute of URI element. For more details see paragraph 6.5.30 in Xfdf specification.
        ///     </remarks>
        /// <returns>boolean indicating if URI element is a map.</returns>
        public virtual bool IsMap() {
            return isMap;
        }

        /// <summary>Sets IsMap, optional attribute of URI element.</summary>
        /// <remarks>Sets IsMap, optional attribute of URI element. For more details see paragraph 6.5.30 in Xfdf specification.
        ///     </remarks>
        /// <param name="map">boolean indicating if URI element is a map</param>
        /// <returns>
        /// current
        /// <see cref="ActionObject"/>.
        /// </returns>
        public virtual iText.Forms.Xfdf.ActionObject SetMap(bool map) {
            isMap = map;
            return this;
        }

        /// <summary>Gets the value of Name, required attribute of Named element.</summary>
        /// <remarks>
        /// Gets the value of Name, required attribute of Named element.
        /// For more details see paragraph 6.5.24 in Xfdf specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// value of Name attribute of a named action element.
        /// </returns>
        public virtual PdfName GetNameAction() {
            return nameAction;
        }

        /// <summary>Sets the value of Name, required attribute of Named element.</summary>
        /// <remarks>
        /// Sets the value of Name, required attribute of Named element.
        /// For more details see paragraph 6.5.24 in Xfdf specification.
        /// </remarks>
        /// <param name="nameAction">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// value to be set to Name attribute of a named action element
        /// </param>
        /// <returns>
        /// current
        /// <see cref="ActionObject"/>.
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
        /// value of OriginalName attribute of current action object.
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
        /// <see cref="ActionObject"/>.
        /// </returns>
        public virtual iText.Forms.Xfdf.ActionObject SetFileOriginalName(String fileOriginalName) {
            this.fileOriginalName = fileOriginalName;
            return this;
        }

        /// <summary>Gets the boolean value of NewWindow, optional attribute of Launch element.</summary>
        /// <remarks>
        /// Gets the boolean value of NewWindow, optional attribute of Launch element.
        /// For more details see paragraph 6.5.23 in Xfdf specification.
        /// </remarks>
        /// <returns>boolean indicating if current Launch action element should be opened in a new window.</returns>
        public virtual bool IsNewWindow() {
            return isNewWindow;
        }

        /// <summary>Sets the boolean value of NewWindow, optional attribute of Launch element.</summary>
        /// <remarks>
        /// Sets the boolean value of NewWindow, optional attribute of Launch element.
        /// For more details see paragraph 6.5.23 in Xfdf specification.
        /// </remarks>
        /// <param name="newWindow">boolean indicating if current Launch action element should be opened in a new window
        ///     </param>
        /// <returns>
        /// current
        /// <see cref="ActionObject"/>.
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
        /// destination attribute of current action element.
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
        /// <see cref="ActionObject"/>.
        /// </returns>
        public virtual iText.Forms.Xfdf.ActionObject SetDestination(DestObject destination) {
            this.destination = destination;
            return this;
        }
    }
}
