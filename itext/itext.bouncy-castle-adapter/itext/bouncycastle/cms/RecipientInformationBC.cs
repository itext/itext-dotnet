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
using iText.Bouncycastle.Crypto;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.RecipientInformation"/>.
    /// </summary>
    public class RecipientInformationBC : IRecipientInformation {
        private readonly RecipientInformation recipientInformation;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformation"/>.
        /// </summary>
        /// <param name="recipientInformation">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformation"/>
        /// to be wrapped
        /// </param>
        public RecipientInformationBC(RecipientInformation recipientInformation) {
            this.recipientInformation = recipientInformation;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformation"/>.
        /// </returns>
        public virtual RecipientInformation GetRecipientInformation() {
            return recipientInformation;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetContent(IPrivateKey key) {
            try {
                return recipientInformation.GetContent(((PrivateKeyBC)key).GetPrivateKey());
            }
            catch (CmsException e) {
                throw new CmsExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRecipientID GetRID() {
            return new RecipientIDBC(recipientInformation.RecipientID);
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cms.RecipientInformationBC that = (iText.Bouncycastle.Cms.RecipientInformationBC)o;
            return Object.Equals(recipientInformation, that.recipientInformation);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipientInformation);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return recipientInformation.ToString();
        }
    }
}
