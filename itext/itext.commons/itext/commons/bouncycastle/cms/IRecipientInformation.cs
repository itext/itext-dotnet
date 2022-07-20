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
        /// <param name="recipient">wrapper for recipient object to use to recover content encryption key</param>
        /// <returns>the content inside the EnvelopedData this RecipientInformation is associated with.</returns>
        byte[] GetContent(IRecipient recipient);

        /// <summary>
        /// Calls actual
        /// <c>getRID</c>
        /// method for the wrapped RecipientInformation object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IRecipientId"/>
        /// the wrapper for received RecipientId object.
        /// </returns>
        IRecipientId GetRID();
    }
}
