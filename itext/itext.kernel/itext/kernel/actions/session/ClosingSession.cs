/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Actions.Session {
    /// <summary>The class allows to share properties during closing of the document.</summary>
    public class ClosingSession {
        private readonly PdfDocument document;

        private IList<String> producer;

        private readonly IDictionary<String, Object> properties;

        /// <summary>Creates a closing session for the document.</summary>
        /// <param name="document">is a document to be close</param>
        public ClosingSession(PdfDocument document) {
            this.document = document;
            this.properties = new Dictionary<String, Object>();
        }

        /// <summary>Obtains closing document.</summary>
        /// <returns>closing document</returns>
        public virtual PdfDocument GetDocument() {
            return document;
        }

        /// <summary>Gets metadata about products involved into document processing.</summary>
        /// <returns>metadata</returns>
        public virtual IList<String> GetProducer() {
            return producer;
        }

        /// <summary>Sets metadata about products involved into document processing.</summary>
        /// <param name="producer">is a meta data</param>
        public virtual void SetProducer(IList<String> producer) {
            this.producer = producer;
        }

        /// <summary>Gets additional property associated with the provided key.</summary>
        /// <param name="key">is a key</param>
        /// <returns>value associated with the key</returns>
        public virtual Object GetProperty(String key) {
            return properties.Get(key);
        }

        /// <summary>Stores new property.</summary>
        /// <param name="key">is a key</param>
        /// <param name="value">is a value to be associated with the key</param>
        public virtual void SetProperty(String key, Object value) {
            properties.Put(key, value);
        }
    }
}
