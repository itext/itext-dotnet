/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Bouncycastlefips.Crypto;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Operators;
using Org.BouncyCastle.Security;

namespace iText.Bouncycastlefips.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.RecipientInformation"/>.
    /// </summary>
    public class RecipientInformationBCFips : IRecipientInformation {
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
        public RecipientInformationBCFips(RecipientInformation recipientInformation) {
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
                return recipientInformation.GetContent(new CmsKeyTransEnvelopedRecipient(
                    ((PrivateKeyBCFips)key).GetPrivateKey(), new SecureRandom()));
            }
            catch (CmsException e) {
                throw new CMSExceptionBCFips(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRecipientId GetRID() {
            return new RecipientIdBCFips((KeyTransRecipientID)recipientInformation.RecipientID);
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
            iText.Bouncycastlefips.Cms.RecipientInformationBCFips that = (iText.Bouncycastlefips.Cms.RecipientInformationBCFips
                )o;
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
