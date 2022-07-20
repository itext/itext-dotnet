using System.Collections.Generic;

namespace iText.Commons.Bouncycastle.Cms {
    /// <summary>
    /// This interface represents the wrapper for RecipientInformationStore that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IRecipientInformationStore {
        /// <summary>
        /// Calls actual
        /// <c>getRecipients</c>
        /// method for the wrapped RecipientInformationStore object.
        /// </summary>
        /// <returns>a collection of wrapped recipients.</returns>
        ICollection<IRecipientInformation> GetRecipients();

        /// <summary>
        /// Calls actual
        /// <c>get</c>
        /// method for the wrapped RecipientInformationStore object.
        /// </summary>
        /// <param name="var1">RecipientId wrapper</param>
        /// <returns>
        /// 
        /// <see cref="IRecipientInformation"/>
        /// the wrapper for received RecipientInformation object.
        /// </returns>
        IRecipientInformation Get(IRecipientId var1);
    }
}
