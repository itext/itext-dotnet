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
using Org.BouncyCastle.Cms.Jcajce;
using iText.Bouncycastlefips.Cms;
using iText.Commons.Bouncycastle.Cms.Jcajce;

namespace iText.Bouncycastlefips.Cms.Jcajce {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.Jcajce.JceKeyTransEnvelopedRecipient"/>.
    /// </summary>
    public class JceKeyTransEnvelopedRecipientBCFips : RecipientBCFips, IJceKeyTransEnvelopedRecipient {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JceKeyTransEnvelopedRecipient"/>.
        /// </summary>
        /// <param name="jceKeyTransEnvelopedRecipient">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JceKeyTransEnvelopedRecipient"/>
        /// to be wrapped
        /// </param>
        public JceKeyTransEnvelopedRecipientBCFips(JceKeyTransEnvelopedRecipient jceKeyTransEnvelopedRecipient)
            : base(jceKeyTransEnvelopedRecipient) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JceKeyTransEnvelopedRecipient"/>.
        /// </returns>
        public virtual JceKeyTransEnvelopedRecipient GetJceKeyTransEnvelopedRecipient() {
            return (JceKeyTransEnvelopedRecipient)GetRecipient();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IJceKeyTransEnvelopedRecipient SetProvider(String provider) {
            GetJceKeyTransEnvelopedRecipient().SetProvider(provider);
            return this;
        }
    }
}
