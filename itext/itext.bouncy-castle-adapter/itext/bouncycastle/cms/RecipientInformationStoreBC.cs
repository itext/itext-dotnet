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
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.RecipientInformationStore"/>.
    /// </summary>
    public class RecipientInformationStoreBC : IRecipientInformationStore {
        private readonly RecipientInformationStore recipientInformationStore;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformationStore"/>.
        /// </summary>
        /// <param name="recipientInformationStore">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformationStore"/>
        /// to be wrapped
        /// </param>
        public RecipientInformationStoreBC(RecipientInformationStore recipientInformationStore) {
            this.recipientInformationStore = recipientInformationStore;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.RecipientInformationStore"/>.
        /// </returns>
        public virtual RecipientInformationStore GetRecipientInformationStore() {
            return recipientInformationStore;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICollection<IRecipientInformation> GetRecipients() {
            List<IRecipientInformation> recipientInformation = new List<IRecipientInformation>();
            ICollection recipients = recipientInformationStore.GetRecipients();
            foreach (RecipientInformation recipient in recipients) {
                recipientInformation.Add(new RecipientInformationBC(recipient));
            }
            return recipientInformation;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRecipientInformation Get(IRecipientId id) {
            return new RecipientInformationBC(recipientInformationStore[((RecipientIdBC)id).GetRecipientId()]);
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
            iText.Bouncycastle.Cms.RecipientInformationStoreBC that = (iText.Bouncycastle.Cms.RecipientInformationStoreBC
                )o;
            return Object.Equals(recipientInformationStore, that.recipientInformationStore);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(recipientInformationStore);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return recipientInformationStore.ToString();
        }
    }
}
