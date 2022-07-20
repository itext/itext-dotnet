namespace iText.Commons.Bouncycastle.Cms {
    /// <summary>
    /// This interface represents the wrapper for CMSEnvelopedData that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ICMSEnvelopedData {
        /// <summary>
        /// Calls actual
        /// <c>getRecipientInfos</c>
        /// method for the wrapped CMSEnvelopedData object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IRecipientInformationStore"/>
        /// the wrapper for the received RecipientInformationStore object.
        /// </returns>
        IRecipientInformationStore GetRecipientInfos();
    }
}
