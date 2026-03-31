/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.IO;

namespace iText.IO.Resolver.Resource {
    /// <summary>Extended interface for classes that can retrieve data from resources by URL.</summary>
    /// <remarks>
    /// Extended interface for classes that can retrieve data from resources by URL.
    /// An extra method with http headers and request data is added.
    /// </remarks>
    public interface IAdvancedResourceRetriever : IResourceRetriever {
        /// <summary>
        /// Gets the
        /// <see cref="System.IO.Stream"/>
        /// with the data from a provided URL by instantiating an HTTP connection to the URL.
        /// </summary>
        /// <param name="url">the source URL</param>
        /// <param name="request">data to send to the URL</param>
        /// <param name="headers">HTTP headers to set for the outgoing connection</param>
        /// <returns>the input stream with the retrieved data</returns>
        Stream Get(Uri url, byte[] request, IDictionary<String, String> headers);
    }
}
