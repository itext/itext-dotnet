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
using iText.Commons.Bouncycastle.Crypto;

namespace iText.Commons.Bouncycastle.Cms {
    /// <summary>
    /// This interface represents the wrapper for RecipientInformation that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IRecipientInformation {
        /// <summary>
        /// Calls actual
        /// <c>getContent</c>
        /// method for the wrapped RecipientInformation object.
        /// </summary>
        /// <param name="key">wrapper for recipient object to use to recover content encryption key</param>
        /// <returns>the content inside the EnvelopedData this RecipientInformation is associated with.</returns>
        byte[] GetContent(IPrivateKey key);

        /// <summary>
        /// Calls actual
        /// <c>getRID</c>
        /// method for the wrapped RecipientInformation object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IRecipientID"/>
        /// the wrapper for received RecipientId object.
        /// </returns>
        IRecipientID GetRID();
    }
}
