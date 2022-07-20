using System.Collections.Generic;

namespace iText.Commons.Bouncycastle.Cms {
    public interface IRecipientInformationStore {
        ICollection<IRecipientInformation> GetRecipients();

        IRecipientInformation Get(IRecipientId var1);
    }
}
