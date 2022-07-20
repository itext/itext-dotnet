using System;
using iText.Commons.Bouncycastle.Cms;

namespace iText.Commons.Bouncycastle.Cms.Jcajce {
    /// <summary>
    /// This interface represents the wrapper for JceKeyTransEnvelopedRecipient that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IJceKeyTransEnvelopedRecipient : IRecipient {
        /// <summary>
        /// Calls actual
        /// <c>setProvider</c>
        /// method for the wrapped JceKeyTransEnvelopedRecipient object.
        /// </summary>
        /// <param name="provider">provider name</param>
        /// <returns>
        /// 
        /// <see cref="IJceKeyTransEnvelopedRecipient"/>
        /// this wrapper object.
        /// </returns>
        IJceKeyTransEnvelopedRecipient SetProvider(String provider);
    }
}
