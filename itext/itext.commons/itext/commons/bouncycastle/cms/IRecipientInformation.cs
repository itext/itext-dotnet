namespace iText.Commons.Bouncycastle.Cms {
    public interface IRecipientInformation {
        byte[] GetContent(IRecipient recipient);

        IRecipientId GetRID();
    }
}
